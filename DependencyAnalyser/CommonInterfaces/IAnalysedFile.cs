using DependencyAnalyser.DotNet.Enums;

namespace DependencyAnalyser.DotNet.CommonInterfaces
{
    public interface IAnalysedFile
    {
        public Guid ID { get; }

        public string Name { get; }

        public IList<string> Dependencies { get; }

        public FileType Type { get; }
    }
}
