namespace AssemblyDependencyAnalyser.EventArguments
{
    /// <summary>
    /// Provides data for the event that signals the completion of solution analysis, including the total number of
    /// projects analyzed.
    /// </summary>
    public class SolutionAnalysisCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the total number of projects that have been analyzed.
        /// </summary>
        public int ProjectsAnalysed { get; }

        /// <summary>
        /// Initializes a new instance of the SolutionAnalysisCompleteEventArgs class with the specified number of
        /// analyzed projects.
        /// </summary>
        /// <param name="projectsAnalysed">The total number of projects that were analyzed. Must be zero or greater.</param>
        public SolutionAnalysisCompleteEventArgs(int projectsAnalysed)
        {
            ProjectsAnalysed = projectsAnalysed;
        }
    }
}
