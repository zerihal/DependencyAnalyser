namespace DependencyAnalyser.DotNet.CommonInterfaces
{
    public interface IAnalysedApplicationFile : IAnalysedFile
    {
        public IList<string> Dependents { get; }
    }
}
