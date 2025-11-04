using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation
{
    public class AnalysedApplicationFile : AnalysedFile, IAnalysedApplicationFile
    {
        public IList<string> Dependents { get; }

        public AnalysedApplicationFile(string name, FileType type, IList<string> dependencies) : 
            base(name, type, dependencies)
        {
            Dependents = new List<string>();
        }
    }
}
