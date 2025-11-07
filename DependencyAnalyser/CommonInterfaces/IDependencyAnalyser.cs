namespace AssemblyDependencyAnalyser.CommonInterfaces
{
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
        /// Analyses the application located at the specified project path.
        /// </summary>
        /// <param name="projectPath">Project path.</param>
        /// <returns>
        /// Collection of <see cref="IAnalysedApplicationFile"/>, which contain dependencies and dependents within the project.
        /// </returns>
        public IEnumerable<IAnalysedApplicationFile> AnalyseApplication(string projectPath);
    }
}
