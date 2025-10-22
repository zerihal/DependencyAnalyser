using DependencyAnalyser.DotNet.CommonInterfaces;

namespace DependencyAnalyser.DotNet.Implementation
{
    public class CircularDependency : ICircularDependency
    {
        public IList<string> DependencyChain { get; }

        public CircularDependency(IList<string> dependencyChain)
        {
            DependencyChain = dependencyChain;
        }
    }
}
