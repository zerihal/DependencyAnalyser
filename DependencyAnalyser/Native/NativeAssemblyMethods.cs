using DependencyAnalyser.DotNet.CommonInterfaces;
using DependencyAnalyser.DotNet.Enums;
using DependencyAnalyser.DotNet.Extensions;
using DependencyAnalyser.DotNet.Implementation;
using PeNet;

namespace DependencyAnalyser.DotNet.Native
{
    public static class NativeAssemblyMethods
    {
        public static IAnalysedFile GetNativeAnalysedFile(Stream stream)
        {
            // Native assembly analysis logic goes here
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var peFile = new PeFile(ms.ToArray());

            var dependencies = peFile.ImportedFunctions?.Select(f => f.DLL).Distinct().ToList() ?? new List<string>();
            var fileType = peFile.IsDll ? FileType.NativeDll : FileType.NativeExe;

            return new AnalysedFile(peFile.GetModuleName() ?? "Native Assembly", fileType, dependencies);
        }
    }
}
