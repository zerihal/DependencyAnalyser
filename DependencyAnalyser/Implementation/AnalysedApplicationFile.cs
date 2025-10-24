using DependencyAnalyser.DotNet.CommonInterfaces;
using DependencyAnalyser.DotNet.Enums;

namespace DependencyAnalyser.DotNet.Implementation
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
