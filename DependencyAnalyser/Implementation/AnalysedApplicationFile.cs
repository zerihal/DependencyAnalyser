using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation
{
    public class AnalysedApplicationFile : AnalysedFile, IAnalysedApplicationFile
    {
        /// <inheritdoc/>
        public IList<string> Dependents { get; } = new List<string>();

        /// <summary>
        /// Creates an instance of <see cref="AnalysedApplicationFile"/>.
        /// </summary>
        /// <param name="name">Application file name.</param>
        /// <param name="type">File type (<see cref="FileType"/>)</param>
        /// <param name="dependencies">List of dependencies.</param>
        /// <param name="dotNetInfo">.NET framework info (if applicable - default is <see langword="null")./></param>
        public AnalysedApplicationFile(string name, FileType type, IList<string> dependencies, DotNetFrameworkVersionInfo? dotNetInfo = null) : 
            base(name, type, dependencies, dotNetInfo)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AnalysedApplicationFile"/>.
        /// </summary>
        /// <param name="name">Application file name.</param>
        /// <param name="type">File type (<see cref="FileType"/>)</param>
        /// <param name="dependencies">List of dependencies.</param>
        /// <param name="dotNetCoreExeIndicator">Flag to indicate whether this is a possible .NET core exe.</param>
        public AnalysedApplicationFile(string name, FileType type, IList<string> dependencies, bool dotNetCoreExeIndicator) :
            base(name, type, dependencies, dotNetCoreExeIndicator)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AnalysedApplicationFile"/> from an existing <see cref="IAnalysedFile"/>.
        /// </summary>
        /// <param name="analysedFile"><see cref="IAnalysedFile"/> instance to base from.</param>
        public AnalysedApplicationFile(IAnalysedFile analysedFile) :
            base(analysedFile.Name, analysedFile.Type, analysedFile.Dependencies)
        {
            DotNetFrameworkVersionInfo = analysedFile.DotNetFrameworkVersionInfo;
            _dotNetCoreExeIndicator = analysedFile.PossibleDotNetCoreBootstrapper;
        }
    }
}
