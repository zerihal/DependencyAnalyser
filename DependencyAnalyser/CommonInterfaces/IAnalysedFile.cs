using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Implementation;

namespace AssemblyDependencyAnalyser.CommonInterfaces
{
    public interface IAnalysedFile
    {
        public Guid ID { get; }

        public string Name { get; }

        public IList<string> Dependencies { get; }

        public FileType Type { get; }

        public bool HasBeenAnalysed { get; }

        public DotNetFrameworkVersionInfo? DotNetFrameworkVersionInfo { get; }
    }
}
