using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.CommonInterfaces.DotNet
{
    /// <summary>
    /// Represents an analysed project file (csproj), including its unique identifier, name, target frameworks, project
    /// dependencies, and package references.
    /// </summary>
    /// <remarks>
    /// This interface provides access to key metadata and dependency information for a project file that has been 
    /// analysed. It can be used to inspect the project's structure, its referenced frameworks, and its relationships 
    /// to other projects and packages. Implementations are expected to provide read-only access to these properties.
    /// </remarks>
    public interface IAnalysedProjectFile
    {
        /// <summary>
        /// Unique identifier for the analysed project file.
        /// </summary>
        public Guid ID { get; }
    
        /// <summary>
        /// Name of the analysed project file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the full path to the file associated with this instance.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the type of the .NET project represented by this instance.
        /// </summary>
        public DotNetProjectType ProjectType { get; }

        /// <summary>
        /// Gets the full path to the AssemblyInfo file associated with the current project, if available.
        /// </summary>
        public string? AssemblyInfoPath { get; }

        /// <summary>
        /// Gets a value indicating whether an AssemblyInfo file is present in the project.
        /// </summary>
        public bool HasAssemblyInfoFile { get; }

        /// <summary>
        /// List of target frameworks for the analysed project file.
        /// </summary>
        public IList<string> TargetFrameworks { get; }

        /// <summary>
        /// List of project names that this project depends on.
        /// </summary>
        public IList<string> ProjectDependencies { get; }

        /// <summary>
        /// Gets the list of dependencies that are not part of the current project.
        /// </summary>
        public IList<string> NonProjectDependencies { get; }

        /// <summary>
        /// List of package references (i.e. NuGet packages) that this project depends on.
        /// </summary>
        public IList<string> PackageReferences { get; }
    }
}
