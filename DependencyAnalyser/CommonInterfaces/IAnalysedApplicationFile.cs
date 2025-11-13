namespace AssemblyDependencyAnalyser.CommonInterfaces
{
    /// <summary>
    /// Analysed application file (i.e. from a complete application or collection of assemblies).
    /// </summary>
    public interface IAnalysedApplicationFile : IAnalysedFile
    {
        /// <summary>
        /// List of dependents (other assemblies that depend on this file).
        /// </summary>
        public IList<string> Dependents { get; }
    }
}
