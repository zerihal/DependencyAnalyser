using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Extensions;
using AssemblyDependencyAnalyser.Implementation;
using PeNet;
using System.Diagnostics;

namespace AssemblyDependencyAnalyser.Native
{
    public static class NativeAssemblyMethods
    {
        /// <summary>
        /// Analyses a native assembly from a stream and returns an <see cref="IAnalysedFile"/> instance.
        /// </summary>
        /// <param name="stream">Assembly/file/object stream.</param>
        /// <returns><see cref="IAnalysedFile"/> for the native file.</returns>
        public static IAnalysedFile GetNativeAnalysedFile(Stream stream, AssemblyType assemblyType)
        {
            // Native assembly analysis logic goes here
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            ms.Position = 0;

            try
            {
                var peFile = new PeFile(ms.ToArray());
                var dependencies = peFile.ImportedFunctions?.Select(f => f.DLL).Distinct().ToList() ?? new List<string>();
                var fileType = peFile.IsDll ? FileType.NativeDll : FileType.NativeExe;

                return new AnalysedFile(peFile.GetModuleName() ?? "Native Assembly", fileType, dependencies, assemblyType, 
                    PossibleDotNetCoreBootstrapper(peFile));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Determines whether the specified PE file is a possible .NET Core bootstrapper executable.
        /// </summary>
        /// <param name="peFile"><see cref="PeFile"/> from object stream.</param>
        /// <returns>
        /// <see langword="true"/> is heuristically indicated the file may be a .NET core bootstrapper (exe), otherwise <see langword="false"/>.
        /// </returns>
        public static bool PossibleDotNetCoreBootstrapper(PeFile peFile)
        {
            // If it's a DLL, it cannot be a .NET Core bootstrapper exe
            if (peFile.IsDll)
                return false;

            // CLR header will always be null for native assemblies, so check first in case a .NET one has slipped through
            if (peFile.ImageComDescriptor != null)
                return true;

            // Check imported functions for known .NET Core runtime DLLs
            var imports = peFile.ImportedFunctions?
                .Select(f => f.DLL?.ToLowerInvariant())
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .ToList() ?? new List<string?>();

            // First check for .NET Core specific runtime imports
            if (imports.Any(d => d != null && (d.Contains("coreclr") || d.Contains("hostfxr") || d.Contains("hostpolicy"))))
                return true;

            // Next check whether there are no C++ specific runtime imports (would expect at least one of these)
            if (!imports.Any(d => d != null && (d.Contains("vcruntime") || d.Contains("msvcp") || d.Contains("ucrtbase"))))
                return true;

            return false;
        }
    }
}
