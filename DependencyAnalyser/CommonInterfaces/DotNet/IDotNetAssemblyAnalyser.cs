using AssemblyDependencyAnalyser.Implementation.DotNet;

namespace AssemblyDependencyAnalyser.CommonInterfaces.DotNet
{
    /// <summary>
    /// Provides analysis operations for .NET assemblies.
    /// </summary>
    /// <remarks>Supports dependency analysis and retrieval of basic assembly information.</remarks>
    public interface IDotNetAssemblyAnalyser : IDependencyAnalyser
    {
        /// <summary>
        /// Retrieves basic information about an assembly file.
        /// </summary>
        /// <param name="filePath">The path to the assembly file.</param>
        /// <returns><see cref="BasicAssemblyInfo"/> instance.</returns>
        /// <remarks>
        /// Basic information includes .NET version and can be obtained regardless of
        /// runtime assembly version that AssemblyDependencyAnalyser is running on. 
        /// This is useful for determining if an assembly can be analysed by the current 
        /// runtime.
        /// </remarks>
        public BasicAssemblyInfo GetBasicAssemblyInfo(string filePath);
    }
}
