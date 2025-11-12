using AssemblyDependencyAnalyser.CommonInterfaces;

namespace AssemblyDependencyAnalyser.Implementation
{
    /// <inheritdoc/>
    public class AnalysedJavaAppFile : AnalysedJavaFile, IAnalysedApplicationFile
    {
        /// <inheritdoc/>
        public IList<string> Dependents { get; } = new List<string>();

        /// <summary>
        /// Creates an instance of <see cref="IAnalysedApplicationFile"/> (Java specific).
        /// </summary>
        /// <param name="name">Application file name.</param>
        /// <param name="dependencies">List of dependencies.</param>
        /// <param name="groupId">Group ID (root namespace).</param>
        /// <param name="version">Assembly version.</param>
        /// <param name="compilerVersion">Compiler version (Maven only).</param>
        public AnalysedJavaAppFile(string name, IList<string> dependencies, string? groupId = null, string? version = null, 
            int? compilerVersion = null) : base(name, dependencies, groupId, version, compilerVersion)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="IAnalysedApplicationFile"/> (Java specific) from <see cref="IAnalysedJavaFile"/>.
        /// </summary>
        /// <param name="analysedFile"><see cref="IAnalysedJavaFile"/> to base from.</param>
        public AnalysedJavaAppFile(IAnalysedJavaFile analysedFile) : base(analysedFile.Name, analysedFile.Dependencies, 
            analysedFile.GroupId, analysedFile.Version, analysedFile.CompilerVersion)
        {
        }
    }
}
