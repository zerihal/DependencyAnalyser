using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Extensions;
using AssemblyDependencyAnalyser.Native;
using System.Reflection;

namespace AssemblyDependencyAnalyser.Implementation
{
    public class DependencyAnalyser : IDependencyAnalyser
    {
        /// <inheritdoc/>
        public IAnalysedFile AnalyseAssembly(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
            {
                if (HelperMethods.GetFileType(assemblyPath) != AssemblyType.Managed)
                {
                    // C/C++ exe or dll, or maybe .NET core bootstrapper (exe) - get native analysed file.
                    try
                    {
                        return NativeAssemblyMethods.GetNativeAnalysedFile(File.OpenRead(assemblyPath));
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
                if (assembly is Assembly asm)
                {
                    analysisAssembly = asm;
                }
                else if (assembly is Stream stream)
                {
                    if (HelperMethods.GetAssemblyType(stream) != AssemblyType.Managed)
                    {
                        return NativeAssemblyMethods.GetNativeAnalysedFile(stream);
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

                    if (HelperMethods.GetAssemblyType(bytesStream) != AssemblyType.Managed)
                    {
                        return NativeAssemblyMethods.GetNativeAnalysedFile(bytesStream);
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

            return new AnalysedFile(analysisAssembly.FullName ?? ".NET Assembly", fileType, referenceAssemblies, 
                HelperMethods.GetFrameworkVersionInfo(analysisAssembly));
        }

        /// <inheritdoc/>
        public IEnumerable<IAnalysedApplicationFile> AnalyseApplication(string projectPath)
        {
            var analysedFiles = new List<IAnalysedApplicationFile>();

            if (!Directory.Exists(projectPath))
            {
                foreach (var file in Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories))
                {
                    if (file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                        file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        var analysedFile = AnalyseAssembly(file);
                        analysedFiles.Add(analysedFile.ToAnalysedApplicationFile());
                    }
                }

                foreach (var analysedFile in analysedFiles)
                {
                    var dependants = analysedFiles.Where(af => 
                        af.Dependencies.Contains(analysedFile.Name)).Select(af => af.Name).ToList();

                    analysedFile.Dependents.ToList().AddRange(dependants);
                }
            }

            return analysedFiles;
        }
    }
}
