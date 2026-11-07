using AssemblyDependencyAnalyser.CommonInterfaces.DotNet;

namespace AssemblyDependencyAnalyser.Implementation.DotNet
{
    /// <inheritdoc cref="IDotNetAssemblyAnalyser"/>
    public class DotNetAssemblyAnalyser : DependencyAnalyser, IDotNetAssemblyAnalyser
    {
        /// <inheritdoc/>
        public BasicAssemblyInfo GetBasicAssemblyInfo(string filePath)
        {
            return new BasicAssemblyInfo(filePath);
        }
    }
}
