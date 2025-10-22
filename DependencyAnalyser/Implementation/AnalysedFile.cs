using DependencyAnalyser.DotNet.CommonInterfaces;

namespace DependencyAnalyser.DotNet.Implementation
{
    public class AnalysedFile : IAnalysedFile
    {
        public string Name { get; }

        public IList<string> Dependencies { get; }

        public AnalysedFile(string name, IList<string> dependencies)
        {
            Name = name;
            Dependencies = dependencies;
        }
    }
}
