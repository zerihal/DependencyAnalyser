namespace AssemblyDependencyAnalyser.CommonInterfaces
{
    public interface IAnalysedApplicationFile : IAnalysedFile
    {
        public IList<string> Dependents { get; }
    }
}
