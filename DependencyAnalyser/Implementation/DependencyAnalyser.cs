using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Extensions;
using AssemblyDependencyAnalyser.Java;
using AssemblyDependencyAnalyser.Native;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using System.IO.Compression;
using System.Reflection;

namespace AssemblyDependencyAnalyser.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class DependencyAnalyser : IDependencyAnalyser
    {
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
                    catch
                    {
                        // Log?
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

            // Log?
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
                // Log exception
                return AnalysedFile.UnsupportedFile(true);
            }

            if (analysisAssembly == null)
            {
                // Should not be reached, but just in case
                return AnalysedFile.UnsupportedFile();
            }

            var fileType = analysisAssembly.EntryPoint != null ? FileType.DotNetExe : FileType.DotNetDll;
            var referenceAssemblies = analysisAssembly.GetReferencedAssemblies().Select(a => a.FullName).ToList();

            return new AnalysedFile(analysisAssembly.FullName ?? ".NET Assembly", fileType, referenceAssemblies, AssemblyType.Managed,
                HelperMethods.GetFrameworkVersionInfo(analysisAssembly));
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
                    catch
                    {
                        // Log?
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
            catch (Exception)
            {
                // Log?
            }

            return false;
        }
    }
}
