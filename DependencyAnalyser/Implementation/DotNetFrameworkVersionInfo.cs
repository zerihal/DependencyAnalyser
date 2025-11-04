namespace AssemblyDependencyAnalyser.Implementation
{
    public class DotNetFrameworkVersionInfo
    {
        /// <summary>
        /// Framework version string (e.g. net472)
        /// </summary>
        public string? FrameworkVersionString { get; }

        /// <summary>
        /// Major .NET version (e.g. 4, 5, 6, etc)
        /// </summary>
        public double? Version { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasValue => FrameworkVersionString != null || Version != null;

        public DotNetFrameworkVersionInfo(string? frameworkVersionString, double? majorVersion)
        {
            FrameworkVersionString = frameworkVersionString;
            Version = majorVersion;
        }
    }
}
