namespace DependencyAnalyser.DotNet.CommonInterfaces
{
    public interface IAnalysedFile
    {
        public string Name { get; }

        public IList<string> Dependencies { get; }
    }
}
