using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Implementation;

namespace AssemblyDependencyAnalyser.CommonInterfaces
{
    public interface IAnalysedFile
    {
        /// <summary>
        /// Unique identifier for the analysed file.
        /// </summary>
        public Guid ID { get; }

        /// <summary>
        /// Name of the analysed file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// List of dependencies for the analysed file.
        /// </summary>
        public IList<string> Dependencies { get; }

        /// <summary>
        /// Type (<see cref="FileType"/>) of the analysed file.
        /// </summary>
        public FileType Type { get; }

        /// <summary>
        /// 
        /// </summary>
        public AssemblyType AssemblyType { get; }

        /// <summary>
        /// Indicates whether the file has been analysed.
        /// </summary>
        public bool HasBeenAnalysed { get; }

        /// <summary>
        /// Indicates whether the analysed file is a possible .NET Core bootstrapper executable.
        /// </summary>
        /// <remarks>
        /// Note: A .NET Core bootstrapper will return as a native executable, however it cannot be guaranteed that this
        /// property will be accurate as it is based on heuristics (i.e. checking for the absence of C++ runtime dependencies 
        /// and/or presence of .NET Core runtime dependencies). This property should therefore be used as an indicator only.
        /// </remarks>
        public bool PossibleDotNetCoreBootstrapper { get; }

        /// <summary>
        /// Information about the .NET Framework version if applicable.
        /// </summary>
        public DotNetFrameworkVersionInfo? DotNetFrameworkVersionInfo { get; }
    }
}
