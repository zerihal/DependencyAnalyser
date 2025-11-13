namespace AssemblyDependencyAnalyser.CommonInterfaces
{
    /// <summary>
    /// Dependency analyser that accepts either a single or collection assembly files, or an archive
    /// (e.g. zip file) containing an entire project or collection of files.
    /// </summary>
    public interface IDependencyAnalyser
    {
        /// <summary>
        /// Analyses the assembly located at the specified path.
        /// </summary>
        /// <param name="assemblyPath">File path for the file to analyse.</param>
        /// <returns><see cref="IAnalysedFile"/> for the source file.</returns>
        public IAnalysedFile AnalyseAssembly(string assemblyPath);

        /// <summary>
        /// Analyses the specified assembly object.
        /// </summary>
        /// <param name="assembly">Assembly object to analyse.</param>
        /// <returns><see cref="IAnalysedFile"/> for the source object.</returns>
        public IAnalysedFile AnalyseAssembly(object assembly);

        /// <summary>
        /// Analyses an application archive (e.g. zip file).
        /// </summary>
        /// <param name="archiveFile">Archive file containing application files.</param>
        /// <returns>
        /// Collection of <see cref="IAnalysedApplicationFile"/>, which contain dependencies and dependents within the project.
        /// </returns>
        public IEnumerable<IAnalysedApplicationFile> AnalyseApplicationArchive(string archiveFile);

        /// <summary>
        /// Analyses the application located at the specified project path.
        /// </summary>
        /// <param name="projectPath">Project path.</param>
        /// <returns>
        /// Collection of <see cref="IAnalysedApplicationFile"/>, which contain dependencies and dependents within the project.
        /// </returns>
        public IEnumerable<IAnalysedApplicationFile> AnalyseApplication(string projectPath);
    }
}
