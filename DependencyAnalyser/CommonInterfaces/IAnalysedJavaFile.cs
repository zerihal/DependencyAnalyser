namespace AssemblyDependencyAnalyser.CommonInterfaces
{
    /// <summary>
    /// Analysed Java file (derived from base analysed file).
    /// </summary>
    public interface IAnalysedJavaFile : IAnalysedFile
    {
        /// <summary>
        /// Group ID (root namespace).
        /// </summary>
        public string? GroupId { get; }

        /// <summary>
        /// File version.
        /// </summary>
        public string? Version { get; }

        /// <summary>
        /// Compiler version (Maven).
        /// </summary>
        public int? CompilerVersion { get; }

        /// <summary>
        /// Flag to indicate whether this is a Maven assembly (if not then it cannot currently be analysed 
        /// by this tool).
        /// </summary>
        public bool IsMavenAssembly { get; }
    }
}
