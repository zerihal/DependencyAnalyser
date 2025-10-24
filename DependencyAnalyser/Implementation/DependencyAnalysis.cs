using DependencyAnalyser.DotNet.CommonInterfaces;
using DependencyAnalyser.DotNet.Extensions;
using System.Reflection;

namespace DependencyAnalyser.DotNet.Implementation
{
    public class DependencyAnalysis : IDependencyAnalysis
    {
        public IAnalysedFile AnalyseAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                return AnalyseAssembly(Assembly.LoadFrom(assemblyPath));
            }

            // Log?
            return AnalysedFile.UnsupportedFile(true);
        }

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
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        analysisAssembly = Assembly.Load(ms.ToArray());
                    }
                }
                else if (assembly is byte[] byteArray)
                {
                    analysisAssembly = Assembly.Load(byteArray);
                }
                else
                {
                    return AnalysedFile.UnsupportedFile();
                }
            }
            catch (BadImageFormatException)
            {
                // This might be a native (C++) assembly
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

            var fileType = analysisAssembly.EntryPoint != null ? Enums.FileType.DotNetExe : Enums.FileType.DotNetDll;
            var referenceAssemblies = analysisAssembly.GetReferencedAssemblies().Select(a => a.FullName).ToList();

            return new AnalysedFile(analysisAssembly.FullName ?? ".NET Assembly", fileType, referenceAssemblies);
        }

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
