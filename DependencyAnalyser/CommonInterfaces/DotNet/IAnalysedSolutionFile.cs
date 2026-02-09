using AssemblyDependencyAnalyser.EventArguments;

namespace AssemblyDependencyAnalyser.CommonInterfaces.DotNet
{
    /// <summary>
    /// Represents an analysed solution file, providing access to its unique identifier, name, and associated analysed
    /// project files.
    /// </summary>
    public interface IAnalysedSolutionFile
    {
        /// <summary>
        /// Occurs when the analysis of a solution has completed.
        /// </summary>
        event EventHandler<SolutionAnalysisCompleteEventArgs>? SolutionAnalysisComplete;

        /// <summary>
        /// Gets the unique identifier for the instance.
        /// </summary>
        public Guid ID { get; }

        /// <summary>
        /// Gets the name associated with the current instance.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the collection of analysed project files (<see cref="IAnalysedProjectFile"/>) associated with the 
        /// this solution.
        /// </summary>
        public IList<IAnalysedProjectFile> Projects { get; }

        /// <summary>
        /// Analyses the projects within the solution file and populates the <see cref="Projects"/> collection 
        /// with the results.
        /// </summary>
        public void AnalyseSolution();

        /// <summary>
        /// Performs an asynchronous analysis of the current solution.
        /// </summary>
        /// <returns>A task that represents the asynchronous analysis operation.</returns>
        public Task AnalyseSolutionAsync();
    }
}
