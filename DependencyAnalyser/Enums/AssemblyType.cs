namespace AssemblyDependencyAnalyser.Enums
{
    public enum AssemblyType
    {
        /// <summary>
        /// .NET Managed Assembly
        /// </summary>
        Managed,
        /// <summary>
        /// Native Assembly (probably C/C++)
        /// </summary>
        Native,
        /// <summary>
        /// Mixed-mode Assembly (C++/CLI)
        /// </summary>
        Mixed,
        /// <summary>
        /// Java Assembly.
        /// </summary>
        Java,
        /// <summary>
        /// Unknown Assembly.
        /// </summary>
        Unknown
    }
}
