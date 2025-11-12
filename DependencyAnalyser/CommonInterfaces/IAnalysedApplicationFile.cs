namespace AssemblyDependencyAnalyser.CommonInterfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAnalysedApplicationFile : IAnalysedFile
    {
        /// <summary>
        /// List of dependents (other assemblies that depend on this file).
        /// </summary>
        public IList<string> Dependents { get; }
    }
}
