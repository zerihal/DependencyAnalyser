namespace AssemblyDependencyAnalyser.Implementation
{
    /// <summary>
    /// .NET framework metadata.
    /// </summary>
    public class DotNetFrameworkVersionInfo
    {
        /// <summary>
        /// Framework version string (e.g. net472)
        /// </summary>
        public string? FrameworkVersionString { get; }

        /// <summary>
        /// Major and minor .NET version (e.g. 4.8, 5.0, 6.0, etc)
        /// </summary>
        public double? Version { get; }

        /// <summary>
        /// Flag to indicate whether any version information is available.
        /// </summary>
        public bool HasValue => FrameworkVersionString != null || Version != null;

        /// <summary>
        /// Creates an instance of <see cref="DotNetFrameworkVersionInfo"/>.
        /// </summary>
        /// <param name="frameworkVersionString">Framework version string.</param>
        /// <param name="version">Framework version (major and minor version number).</param>
        public DotNetFrameworkVersionInfo(string? frameworkVersionString, double? version)
        {
            FrameworkVersionString = frameworkVersionString;
            Version = version;
        }
    }
}
