namespace AssemblyDependencyAnalyser.CommonInterfaces
{
    public interface IDependencyAnalyser
    {
        public IAnalysedFile AnalyseAssembly(string assemblyPath);

        public IAnalysedFile AnalyseAssembly(object assembly);

        public IEnumerable<IAnalysedApplicationFile> AnalyseApplication(string projectPath);
    }
}
