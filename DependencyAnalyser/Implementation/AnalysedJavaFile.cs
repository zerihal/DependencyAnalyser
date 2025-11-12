using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation
{
    /// <inheritdoc/>
    public class AnalysedJavaFile : AnalysedFile, IAnalysedJavaFile
    {
        /// <inheritdoc/>
        public string? GroupId { get; }

        /// <inheritdoc/>
        public string? Version { get; }

        /// <inheritdoc/>
        public int? CompilerVersion { get; }

        /// <inheritdoc/>
        public bool IsMavenAssembly { get; }

        /// <summary>
        /// Creates an instance of <see cref="IAnalysedJavaFile"/>.
        /// </summary>
        /// <param name="name">Analysed file name.</param>
        /// <param name="dependencies">List of dependencies.</param>
        /// <param name="groupId">Group ID (if applicable).</param>
        /// <param name="version">File version (if found).</param>
        /// <param name="compilerVersion">Compiler version (applicable for Maven jar files only).</param>
        /// <param name="isMavenAssembly">Flag to indicate whether this is a Maven assembly (default <see langword="true"/>).</param>
        public AnalysedJavaFile(string name, IList<string> dependencies, string? groupId = null, 
            string? version = null, int? compilerVersion = null, bool isMavenAssembly = true) : base(name, 
                FileType.JavaArchive, dependencies, AssemblyType.Java)
        {
            GroupId = groupId;
            Version = version;
            CompilerVersion = compilerVersion;
            IsMavenAssembly = isMavenAssembly;
        }

        /// <summary>
        /// Gets a basic instance of <see cref="IAnalysedJavaFile"/> with no dependencies or analysis.
        /// </summary>
        /// <param name="name">Name of the assembly or null if not found.</param>
        /// <param name="version">Version of the assembly or null if not found.</param>
        /// <returns><see cref="IAnalysedJavaFile"/> instance.</returns>
        public static IAnalysedJavaFile NonMavenJavaFile(string? name, string? version)
        {
            return new AnalysedJavaFile(name ?? "Non-Maven Java File", new List<string>(), version: version,
                isMavenAssembly: false) { HasBeenAnalysed = false };
        }

        /// <summary>
        /// Gets a basic instance of <see cref="IAnalysedJavaFile"/> with no dependencies or analysis.
        /// </summary>
        /// <returns><see cref="IAnalysedJavaFile"/> instance.</returns>
        public static IAnalysedJavaFile NonMavenJavaFile() => NonMavenJavaFile(null, null);
    }
}
