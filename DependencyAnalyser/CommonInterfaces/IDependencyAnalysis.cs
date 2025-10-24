namespace DependencyAnalyser.DotNet.CommonInterfaces
{
    public interface IDependencyAnalysis
    {
        public IAnalysedFile AnalyseAssembly(string assemblyPath);

        public IAnalysedFile AnalyseAssembly(object assembly);

        public IEnumerable<IAnalysedApplicationFile> AnalyseApplication(string projectPath);
    }
}
