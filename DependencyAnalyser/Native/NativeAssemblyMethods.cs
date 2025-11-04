using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Extensions;
using AssemblyDependencyAnalyser.Implementation;
using PeNet;

namespace AssemblyDependencyAnalyser.Native
{
    public static class NativeAssemblyMethods
    {
        public static IAnalysedFile GetNativeAnalysedFile(Stream stream)
        {
            // Native assembly analysis logic goes here
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            try
            {
                var peFile = new PeFile(ms.ToArray());
                var dependencies = peFile.ImportedFunctions?.Select(f => f.DLL).Distinct().ToList() ?? new List<string>();
                var fileType = peFile.IsDll ? FileType.NativeDll : FileType.NativeExe;

                return new AnalysedFile(peFile.GetModuleName() ?? "Native Assembly", fileType, dependencies);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
