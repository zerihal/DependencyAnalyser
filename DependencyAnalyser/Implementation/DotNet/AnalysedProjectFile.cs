using AssemblyDependencyAnalyser.CommonInterfaces.DotNet;
using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation.DotNet
{
    /// <inheritdoc cref="IAnalysedProjectFile"/>
    public class AnalysedProjectFile : IAnalysedProjectFile
    {
        /// <inheritdoc/>
        public Guid ID { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string FilePath { get; }

        /// <inheritdoc/>
        public DotNetProjectType ProjectType { get; internal set; }

        /// <inheritdoc/>
        public string? AssemblyInfoPath { get; internal set; }

        /// <inheritdoc/>
        public bool HasAssemblyInfoFile => AssemblyInfoPath != null;

        /// <inheritdoc/>
        public IList<string> TargetFrameworks { get; internal set; } = [];

        /// <inheritdoc/>
        public IList<string> ProjectDependencies { get; internal set; } = [];

        /// <inheritdoc/>
        public IList<string> NonProjectDependencies { get; internal set; } = [];

        /// <inheritdoc/>
        public IList<string> PackageReferences { get; internal set; } = [];

        /// <summary>
        /// Initializes a new instance of the AnalysedProjectFile class for the specified project file path and performs
        /// analysis on the file.
        /// </summary>
        /// <param name="filePath">The full path to the project file to be analyzed. Cannot be null or empty.</param>
        public AnalysedProjectFile(string filePath)
        {
            FilePath = filePath;
            Name = Path.GetFileNameWithoutExtension(FilePath);
            DotNetProjectHelperMethods.AnalyseDotNetProjectFile(this);
        }
    }
}
