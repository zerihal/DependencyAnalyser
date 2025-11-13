using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Implementation;
using PeNet;

namespace AssemblyDependencyAnalyser.Extensions
{
    /// <summary>
    /// Analysed file extension methods.
    /// </summary>
    public static class AnalysedFileExtensionMethods
    {
        /// <summary>
        /// Converts an IAnalysedFile (or IAnalysedJavaFile) to an IAnalysedApplicationFile.
        /// </summary>
        /// <param name="analysedFile"><see cref="IAnalysedFile"/> to convert.</param>
        /// <returns><see cref="IAnalysedApplicationFile"/> from <see cref="IAnalysedFile"/></returns>
        public static IAnalysedApplicationFile ToAnalysedApplicationFile(this IAnalysedFile analysedFile)
        {
            if (analysedFile is IAnalysedJavaFile analysedJavaFile)
                return new AnalysedJavaAppFile(analysedJavaFile);

            return new AnalysedApplicationFile(analysedFile);
        }

        /// <summary>
        /// Gets dependency attributes as string arrays (applicable for .NET assemblies).
        /// </summary>
        /// <param name="analysedFile"><see cref="IAnalysedFile"/> to obtain dependency attributes for.</param>
        /// <returns>Collection of dependency attribute arrays.</returns>
        public static IEnumerable<string[]> GetDependencyAttributes(this IAnalysedFile analysedFile)
        {
            var dependencyAttributes = new List<string[]>();

            foreach (var dependency in analysedFile.Dependencies)
                dependencyAttributes.Add(dependency.Split(','));

            return dependencyAttributes;
        }

        /// <summary>
        /// Returns the internal module name (assembly name) from the PE export directory.
        /// Returns null if the PE has no export table.
        /// </summary>
        public static string? GetModuleName(this PeFile pe)
        {
            if (pe.ImageExportDirectory == null)
                return null;

            int fileOffset = RvaToFileOffset(pe, pe.ImageExportDirectory.Name);

            return fileOffset >= 0 ? pe.RawFile.ReadAsciiString(fileOffset) : null;
        }

        /// <summary>
        /// Converts an RVA (relative virtual address) to a file offset using the section headers.
        /// </summary>
        private static int RvaToFileOffset(PeFile pe, uint rva)
        {
            if (pe.ImageSectionHeaders != null)
            {
                foreach (var section in pe.ImageSectionHeaders)
                {
                    uint sectionStart = section.VirtualAddress;
                    uint sectionEnd = sectionStart + Math.Max(section.VirtualSize, section.SizeOfRawData);

                    if (rva >= sectionStart && rva < sectionEnd)
                        return (int)(rva - sectionStart + section.PointerToRawData);
                }
            }

            return -1; // RVA not found
        }
    }
}
