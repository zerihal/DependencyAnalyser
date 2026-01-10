using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Exceptions;
using AssemblyDependencyAnalyser.Extensions;
using AssemblyDependencyAnalyser.Java;
using AssemblyDependencyAnalyser.Logging;
using AssemblyDependencyAnalyser.Native;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using System.IO.Compression;
using System.Reflection;

namespace AssemblyDependencyAnalyser.Implementation
{
    /// <inheritdoc cref="IDependencyAnalyser"/>
    public class DependencyAnalyser : IDependencyAnalyser
    {
        private double? _runtimeNetVersion;

        /// <summary>
        /// Initializes a new instance of the DependencyAnalyser class, preparing it to analyze dependencies and
        /// retrieve .NET runtime version information for the current assembly.
        /// </summary>
        /// <remarks>
        /// If the .NET runtime version cannot be determined, analysis errors will not include
        /// .NET mismatch information. This constructor logs any exceptions encountered during initialization but allows
        /// the instance to be created regardless.
        /// </remarks>
        public DependencyAnalyser()
        {
            try
            {
                if (HelperMethods.GetFrameworkVersionInfo(Assembly.GetExecutingAssembly()) is DotNetFrameworkVersionInfo fwInfo)
                {
                    _runtimeNetVersion = fwInfo.Version;
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Exception getting .NET runtime version ({e.Message}). " +
                    $"Analysis errors will not include .NET mismatch information.", LogLevel.Warning);
            }
        }

        /// <inheritdoc/>
        public IAnalysedFile AnalyseAssembly(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
            {
                // Check whether this is a Java file - is so then return IAnalysedJavaFile
                if (JavaAssemblyMethods.IsJavaArchive(assemblyPath, out var analysedJavaFile))
                    return analysedJavaFile ?? AnalysedJavaFile.NonMavenJavaFile();

                if (HelperMethods.GetFileType(assemblyPath) != AssemblyType.Managed)
                {
                    // C/C++ exe or dll, or maybe .NET core bootstrapper (exe) - get native analysed file.
                    try
                    {
                        return NativeAssemblyMethods.GetNativeAnalysedFile(File.OpenRead(assemblyPath), AssemblyType.Native);
                    }
                    catch (FileAnalysisException e)
                    {
                        Logger.LogException("Native assembly analysis error", e);
                    }
                }
                else
                {
                    try
                    {
                        return AnalyseAssembly(Assembly.LoadFrom(assemblyPath));
                    }
                    catch
                    {
                        return AnalyseAssembly(File.OpenRead(assemblyPath));
                    }
                }
            }

            Logger.Log($"File from {assemblyPath} is not supported for analysis");
            return AnalysedFile.UnsupportedFile(true);
        }

        /// <inheritdoc/>
        public IAnalysedFile AnalyseAssembly(object assembly)
        {
            Assembly? analysisAssembly = null;

            try
            {
                var assemblyType = AssemblyType.Unknown;

                if (assembly is Assembly asm)
                {
                    analysisAssembly = asm;
                    assemblyType = AssemblyType.Managed;
                }
                else if (assembly is Stream stream)
                {
                    if (JavaAssemblyMethods.IsJavaArchive(stream, out var analysedJavaFile))
                        return analysedJavaFile ?? AnalysedJavaFile.NonMavenJavaFile();

                    assemblyType = HelperMethods.GetAssemblyType(stream);

                    if (assemblyType != AssemblyType.Managed)
                    {
                        return NativeAssemblyMethods.GetNativeAnalysedFile(stream, assemblyType);
                    }

                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        analysisAssembly = Assembly.Load(ms.ToArray());
                    }
                }
                else if (assembly is byte[] byteArray)
                {
                    var bytesStream = HelperMethods.GetStreamFromBytes(byteArray);

                    if (JavaAssemblyMethods.IsJavaArchive(bytesStream, out var analysedJavaFile))
                        return analysedJavaFile ?? AnalysedJavaFile.NonMavenJavaFile();

                    assemblyType = HelperMethods.GetAssemblyType(bytesStream);

                    if (assemblyType != AssemblyType.Managed)
                    {
                        return NativeAssemblyMethods.GetNativeAnalysedFile(bytesStream, assemblyType);
                    }

                    analysisAssembly = Assembly.Load(byteArray);
                }
                else
                {
                    return AnalysedFile.UnsupportedFile();
                }
            }
            catch (Exception e)
            {
                Logger.LogException("Assembly analysis error", e);
                return AnalysedFile.UnsupportedFile(true);
            }

            if (analysisAssembly == null)
            {
                // Should not be reached, but just in case
                Logger.Log("Analysis assembly is null", LogLevel.Warning);
                return AnalysedFile.UnsupportedFile();
            }

            // If the target assembly .NET version is higher than the runtime version, return unsupported file. The below will attempt to obtain as much
            // information as possible from the exception.
            try
            {
                var fileType = analysisAssembly.EntryPoint != null ? FileType.DotNetExe : FileType.DotNetDll;
                var referenceAssemblies = analysisAssembly.GetReferencedAssemblies().Select(a => a.FullName).ToList();

                return new AnalysedFile(analysisAssembly.FullName ?? ".NET Assembly", fileType, referenceAssemblies, AssemblyType.Managed,
                    HelperMethods.GetFrameworkVersionInfo(analysisAssembly));
            }
            catch (FileNotFoundException ex1)
            {
                if (_runtimeNetVersion != null && TryGetFrameworkVersionFromFileName(ex1.FileName, out var netFrameworkVersion))
                {
                    if (netFrameworkVersion > _runtimeNetVersion)
                    {
                        var error = $"Missing assembly {ex1.FileName}. Target .NET version {netFrameworkVersion} is higher than runtime .NET version {_runtimeNetVersion}.";
                        return AnalysedFile.UnsupportedDotNetFile(errorMsg: error);
                    }
                }

                return AnalysedFile.UnsupportedDotNetFile(
                    errorMsg: $"Possible missing assembly due to higher target .NET version than runtime version (exception: {ex1.Message})");
            }
            catch (Exception ex2)
            {
                return AnalysedFile.UnsupportedDotNetFile(errorMsg: $"General .NET assembly analysis error (exception: {ex2.Message})");
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IAnalysedApplicationFile> AnalyseApplicationArchive(string archiveFile)
        {
            if (File.Exists(archiveFile))
            {
                var tempDir = Path.Combine(Path.GetTempPath(), $"DepAnalyser_{Guid.NewGuid()}");
                Directory.CreateDirectory(tempDir);
                try
                {
                    if (ExtractArchive(archiveFile, tempDir))
                        return AnalyseApplication(tempDir);
                }
                finally
                {
                    try
                    {
                        Directory.Delete(tempDir, true);
                    }
                    catch (Exception e)
                    {
                        Logger.Log($"Exception deleting temp directory for application analysis ({e.Message}). " +
                            $"This should be auto cleared at later point.", LogLevel.Warning);
                    }
                }
            }

            return [];
        }

        /// <inheritdoc/>
        public IEnumerable<IAnalysedApplicationFile> AnalyseApplication(string projectPath)
        {
            var analysedFiles = new List<IAnalysedApplicationFile>();

            if (Directory.Exists(projectPath))
            {
                foreach (var file in Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories))
                {
                    if (file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                        file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        //var analysedFile = AnalyseAssembly(file);
                        analysedFiles.Add(AnalyseAssembly(file).ToAnalysedApplicationFile());
                    }
                    else if (file.EndsWith(".jar", StringComparison.OrdinalIgnoreCase))
                    {
                        analysedFiles.Add(AnalyseAssembly(file).ToAnalysedApplicationFile());
                    }
                }

                foreach (var analysedFile in analysedFiles)
                {
                    var dependants = analysedFiles.Where(af => 
                        af.Dependencies.Contains(analysedFile.Name)).Select(af => af.Name)?.ToList();

                    if (dependants != null)
                    {
                        foreach (var dep in dependants)
                            analysedFile.Dependents.Add(dep);
                    }
                }
            }

            return analysedFiles;
        }

        /// <summary>
        /// Extracts an archive to the specified destination (i.e. temp folder).
        /// </summary>
        /// <param name="archivePath">Archive file (e.g. zip, tar, 7z, etc).</param>
        /// <param name="destination">Destination to extact files.</param>
        /// <returns><see langword="True"/> if files extracted or <see langword="false"/> if unrecognised file or exception.</returns>
        private bool ExtractArchive(string archivePath, string destination)
        {
            try
            {
                string ext = Path.GetExtension(archivePath).ToLowerInvariant();

                if (ext == ".zip")
                {
                    ZipFile.ExtractToDirectory(archivePath, destination);
                    return true;
                }

                IArchive archive = ext switch
                {
                    ".7z" => SevenZipArchive.Open(archivePath),
                    ".tar" => TarArchive.Open(archivePath),
                    ".gz" => TarArchive.Open(archivePath), // handles .tar.gz via stream
                    _ => ArchiveFactory.Open(archivePath)
                };

                if (archive != null)
                {
                    using (archive)
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (!entry.IsDirectory)
                            {
                                entry.WriteToDirectory(destination, new ExtractionOptions
                                {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                            }
                        }

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogException("Error extracting archive", e);
            }

            return false;
        }

        /// <summary>
        /// Attempts to extract the .NET Framework version number from the specified file name.
        /// </summary>
        /// <remarks>
        /// This method does not throw exceptions for invalid or unexpected file name formats. If the file name
        /// does not contain a recognizable version segment, or if parsing fails, the method returns false
        /// and sets the output parameter to zero.
        /// </remarks>
        /// <param name="filename">
        /// The file name to parse for a .NET Framework version segment. Must contain a segment in the format
        /// 'version={number}'.
        /// </param>
        /// <param name="netFrameworkVersion">
        /// When this method returns, contains the parsed .NET Framework version number if extraction succeeds;
        /// otherwise, zero.
        /// </param>
        /// <returns>true if the .NET Framework version was successfully extracted from the file name; otherwise, false.</returns>
        private bool TryGetFrameworkVersionFromFileName(string? filename, out double netFrameworkVersion)
        {
            netFrameworkVersion = 0.0;

            if (filename == null)
                return false;

            try
            {
                var versionSegment = filename.Split(',').FirstOrDefault(s => s.ToLower().Contains("version="))?.Trim();

                if (versionSegment != null)
                {
                    var verNo = versionSegment.Replace("version=", "", StringComparison.OrdinalIgnoreCase);

                    if (HelperMethods.GetDotNetVersion(verNo) is double verNoDbl)
                    {
                        netFrameworkVersion = verNoDbl;
                        return true;
                    }
                }
            }
            catch
            {
                // Ignore
            }

            return false;
        }
    }
}
