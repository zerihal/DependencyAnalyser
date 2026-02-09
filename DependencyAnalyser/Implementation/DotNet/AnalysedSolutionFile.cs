using AssemblyDependencyAnalyser.CommonInterfaces.DotNet;
using AssemblyDependencyAnalyser.EventArguments;
using System.Xml.Linq;

namespace AssemblyDependencyAnalyser.Implementation.DotNet
{
    /// <inheritdoc/>
    public class AnalysedSolutionFile : IAnalysedSolutionFile
    {
        private readonly string _solutionFileDirectory;
        private readonly string _slnExtension;

        /// <inheritdoc/>
        public event EventHandler<SolutionAnalysisCompleteEventArgs>? SolutionAnalysisComplete;

        /// <inheritdoc/>
        public Guid ID { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IList<IAnalysedProjectFile> Projects { get; } = [];

        /// <summary>
        /// Initializes a new instance of the AnalysedSolutionFile class for the specified solution file path.
        /// </summary>
        /// <param name="solutionFilePath">
        /// The full path to the solution file to be analyzed. Cannot be null, empty, or refer to a non-existent
        /// directory.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if solutionFilePath is null, empty, or does not contain a valid directory component.
        /// </exception>
        public AnalysedSolutionFile(string solutionFilePath)
        {
            Name = Path.GetFileNameWithoutExtension(solutionFilePath);
            _slnExtension = Path.GetExtension(solutionFilePath);
            _solutionFileDirectory = Path.GetDirectoryName(solutionFilePath) ??
                throw new ArgumentException("Invalid solution file path.", nameof(solutionFilePath));
        }

        /// <inheritdoc/>
        public void AnalyseSolution()
        {
            AnalyseSolutionAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public async Task AnalyseSolutionAsync()
        {
            var projectFilePaths = GetProjectFilePathsFromSolution();

            foreach (var projectFilePath in projectFilePaths)
            {
                var analysedProject = new AnalysedProjectFile(projectFilePath);
                Projects.Add(analysedProject);
            }

            OnSolutionAnalysisComplete();
            await Task.CompletedTask;
        }

        private IEnumerable<string> GetProjectFilePathsFromSolution()
        {
            var slnFile = Path.Combine(_solutionFileDirectory, $"{Name}{_slnExtension}");

            if (_slnExtension == ".slnx")
            {
                var slnxProjects = new List<string>();
                var slnxContent = XDocument.Load(slnFile);
                var projectElements = slnxContent.Descendants("Project");
                foreach (var projectElement in projectElements)
                {
                    var relativePath = projectElement.Attribute("Path")?.Value;
                    if (!string.IsNullOrWhiteSpace(relativePath))
                    {
                        var fullPath = Path.Combine(_solutionFileDirectory, relativePath);
                        if (File.Exists(fullPath))
                        {
                            slnxProjects.Add(fullPath);
                        }
                    }
                }

                return slnxProjects;
            }
            else
            {
                return DotNetProjectHelperMethods.GetProjectFiles(slnFile);
            }
        }

        private void OnSolutionAnalysisComplete()
        {
            SolutionAnalysisComplete?.Invoke(this, new SolutionAnalysisCompleteEventArgs(Projects.Count));
        }
    }
}
