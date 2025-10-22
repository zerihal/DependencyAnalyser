using DependencyAnalyser.DotNet.CommonInterfaces;

namespace DependencyAnalyser.DotNet.Implementation
{
    public class DependencyAnalysis : IDependencyAnalysis
    {
        public IAnalysedFile AnalyseAssembly(string assemblyPath)
        {
            throw new NotImplementedException();
        }

        public IAnalysedFile AnalyseAssembly(object assembly)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAnalysedFile> AnalyseProject(string projectPath)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ICircularDependency> FindCircularDependencies(IEnumerable<IAnalysedFile> analysedFiles)
        {
            throw new NotImplementedException();
        }
    }
}
