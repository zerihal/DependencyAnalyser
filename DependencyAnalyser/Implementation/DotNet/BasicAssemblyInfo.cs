using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation.DotNet
{
    /// <summary>
    /// Provides basic information about an assembly.
    /// </summary>
    /// <remarks>
    /// Identifies whether the assembly is a .NET assembly and, when applicable, provides its
    /// framework version information.
    /// </remarks>
    public class BasicAssemblyInfo
    {
        /// <summary>
        /// Returns information about the .NET Framework version.
        /// </summary>
        public DotNetFrameworkVersionInfo? FrameworkVersionInfo { get; }

        /// <summary>
        /// Indicates whether this is a .NET assembly.
        /// </summary>
        public bool IsDotNetAssembly { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAssemblyInfo"/> class.
        /// </summary>
        /// <param name="filePath">The path to the assembly file.</param>
        public BasicAssemblyInfo(string filePath)
        {
            if (HelperMethods.GetFileType(filePath) == AssemblyType.Managed)
            {
                IsDotNetAssembly = true;
                FrameworkVersionInfo = BasicAssemblyInfoHelper.GetDotNetFrameworkVersionInfo(filePath);
            }
            else
            {
                IsDotNetAssembly = false;
                FrameworkVersionInfo = null;
            }
        }
    }
}
