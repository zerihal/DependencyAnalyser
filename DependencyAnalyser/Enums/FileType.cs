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
        /// Unsupported file type.
        /// </summary>
        Unsupported,
        /// <summary>
        /// Invalid file type.
        /// </summary>
        Invalid
    }
}
