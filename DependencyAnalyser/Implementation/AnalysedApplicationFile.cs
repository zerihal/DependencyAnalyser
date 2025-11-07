using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation
{
    public class AnalysedApplicationFile : AnalysedFile, IAnalysedApplicationFile
    {
        /// <inheritdoc/>
        public IList<string> Dependents { get; }

        /// <summary>
        /// Creates an instance of <see cref="AnalysedApplicationFile"/>.
        /// </summary>
        /// <param name="name">Application file name.</param>
        /// <param name="type">File type (<see cref="FileType"/>)</param>
        /// <param name="dependencies">List of dependencies.</param>
        public AnalysedApplicationFile(string name, FileType type, IList<string> dependencies) : 
            base(name, type, dependencies)
        {
            Dependents = new List<string>();
        }
    }
}
