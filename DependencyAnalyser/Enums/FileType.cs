namespace AssemblyDependencyAnalyser.Enums
{
    public enum FileType
    {
        /// <summary>
        /// .NET DLL Assembly.
        /// </summary>
        DotNetDll,
        /// <summary>
        /// .NET EXE Assembly.
        /// </summary>
        DotNetExe,
        /// <summary>
        /// Native DLL Assembly.
        /// </summary>
        NativeDll,
        /// <summary>
        /// Native EXE Assembly or .NET Core bootstrapper EXE.
        /// </summary>
        NativeExe,
        /// <summary>
        /// Java archive file (i.e. *.jar).
        /// </summary>
        JavaArchive,
        /// <summary>
        /// Unsupported file type.
        /// </summary>
        Unsupported,
        /// <summary>
        /// Invalid file type.
        /// </summary>
        Invalid
    }
}
