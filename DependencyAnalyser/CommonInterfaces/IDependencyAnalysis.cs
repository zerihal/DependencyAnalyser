namespace DependencyAnalyser.DotNet.CommonInterfaces
{
    public interface IDependencyAnalysis
    {
        public IAnalysedFile AnalyseAssembly(string assemblyPath);

        public IAnalysedFile AnalyseAssembly(object assembly);

        public IEnumerable<IAnalysedFile> AnalyseProject(string projectPath);

        public IEnumerable<ICircularDependency> FindCircularDependencies(IEnumerable<IAnalysedFile> analysedFiles);
    }
}
