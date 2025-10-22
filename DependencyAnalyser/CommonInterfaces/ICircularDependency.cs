namespace DependencyAnalyser.DotNet.CommonInterfaces
{
    public interface ICircularDependency
    {
        public IList<string> DependencyChain { get; }
    }
}
