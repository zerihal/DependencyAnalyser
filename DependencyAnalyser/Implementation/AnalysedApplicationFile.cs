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
        public AnalysedApplicationFile(string name, FileType type, IList<string> dependencies, AssemblyType assemblyType, 
            DotNetFrameworkVersionInfo? dotNetInfo = null) : base(name, type, dependencies, assemblyType, dotNetInfo)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AnalysedApplicationFile"/>.
        /// </summary>
        /// <param name="name">Application file name.</param>
        /// <param name="type">File type (<see cref="FileType"/>)</param>
        /// <param name="dependencies">List of dependencies.</param>
        /// <param name="dotNetCoreExeIndicator">Flag to indicate whether this is a possible .NET core exe.</param>
        public AnalysedApplicationFile(string name, FileType type, IList<string> dependencies, AssemblyType assemblyType, 
            bool dotNetCoreExeIndicator) : base(name, type, dependencies, assemblyType, dotNetCoreExeIndicator)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="AnalysedApplicationFile"/> from an existing <see cref="IAnalysedFile"/>.
        /// </summary>
        /// <param name="analysedFile"><see cref="IAnalysedFile"/> instance to base from.</param>
        public AnalysedApplicationFile(IAnalysedFile analysedFile) :
            base(analysedFile.Name, analysedFile.Type, analysedFile.Dependencies, analysedFile.AssemblyType)
        {
            DotNetFrameworkVersionInfo = analysedFile.DotNetFrameworkVersionInfo;
            _dotNetCoreExeIndicator = analysedFile.PossibleDotNetCoreBootstrapper;
        }
    }
}
