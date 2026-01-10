using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation
{
    /// <inheritdoc cref="IAnalysedFile"/>
    public class AnalysedFile : IAnalysedFile, IEquatable<AnalysedFile?>
    {
        /// <summary>
        /// Flag to indicate wether this assembly is a .NET core exe.
        /// </summary>
        protected bool _dotNetCoreExeIndicator;

        /// <inheritdoc/>
        public Guid ID { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IList<string> Dependencies { get; }

        /// <inheritdoc/>
        public FileType Type { get; }

        /// <inheritdoc/>
        public AssemblyType AssemblyType { get; }

        /// <inheritdoc/>
        public bool HasBeenAnalysed { get; protected set; }

        /// <inheritdoc/>
        public bool PossibleDotNetCoreBootstrapper => Type == FileType.NativeExe && _dotNetCoreExeIndicator;

        /// <inheritdoc/>
        public DotNetFrameworkVersionInfo? DotNetFrameworkVersionInfo { get; protected set; }

        /// <inheritdoc/>
        public string? AnalyseError { get; protected set; }

        /// <summary>
        /// Creates an instance of <see cref="IAnalysedFile"/>.
        /// </summary>
        /// <param name="name">Analysed file name.</param>
        /// <param name="type">Analysed file type (<see cref="FileType"/>).</param>
        /// <param name="dependencies">List of dependencies.</param>
        /// <param name="assemblyType">Assembly type.</param>
        /// <param name="dotnetFrameworkInfo">.NET framework info (if applicable - default is <see langword="null")./></param>
        public AnalysedFile(string name, FileType type, IList<string> dependencies, AssemblyType assemblyType, DotNetFrameworkVersionInfo? dotnetFrameworkInfo = null)
        {
            Name = name;
            Type = type;
            Dependencies = dependencies;
            AssemblyType = assemblyType;

            if (dotnetFrameworkInfo?.HasValue == true)
                DotNetFrameworkVersionInfo = dotnetFrameworkInfo;

            HasBeenAnalysed = true;
        }

        /// <summary>
        /// Creates an instance of <see cref="IAnalysedFile"/> with .NET Core executable indicator.
        /// </summary>
        /// <param name="name">Analysed file name.</param>
        /// <param name="type">Analysed file type (<see cref="FileType"/>).</param>
        /// <param name="dependencies">List of dependencies.</param>
        /// <param name="assemblyType">Assembly type.</param>
        /// <param name="dotNetCoreExeIndicator">Flag to indicate whether this is a possible .NET core exe.</param>
        public AnalysedFile(string name, FileType type, IList<string> dependencies, AssemblyType assemblyType, bool dotNetCoreExeIndicator)
            : this(name, type, dependencies, assemblyType)
        {
            _dotNetCoreExeIndicator = dotNetCoreExeIndicator;

            // If indication is that this is a .NET core exe, this should be considered as a mixed assembly type rather than pure native.
            if (_dotNetCoreExeIndicator && AssemblyType == AssemblyType.Native)
                AssemblyType = AssemblyType.Mixed;
        }

        /// <summary>
        /// Creates an instance of <see cref="AnalysedFile"/> representing an unsupported file.
        /// </summary>
        /// <param name="isInvalid">
        /// Flag to indicate whether the file is invalid for analysis (i.e. file was null or had exception during parsing).
        /// </param>
        /// <returns>Base instance of <see cref="IAnalysedFile"/> with unsupported type specified.</returns>
        public static AnalysedFile UnsupportedFile(bool isInvalid = false)
        {
            return new AnalysedFile(string.Empty, isInvalid ? FileType.Invalid : FileType.Unsupported, new List<string>(), AssemblyType.Unknown) 
            { 
                HasBeenAnalysed = false
            };
        }

        /// <summary>
        /// Creates an instance of an analysed file representing an unsupported or invalid .NET file.
        /// </summary>
        /// <param name="isInvalid">
        /// Indicates whether the file should be marked as invalid. If <see langword="true"/>, the file is considered
        /// invalid; otherwise, it is considered unsupported.
        /// </param>
        /// <param name="errorMsg">
        /// An optional error message describing the reason the file is unsupported or invalid. Can be <see
        /// langword="null"/> if no error message is available.
        /// </param>
        /// <returns>
        /// An <see cref="AnalysedFile"/> instance representing an unsupported or invalid .NET file, with the specified
        /// error state and message.
        /// </returns>
        public static AnalysedFile UnsupportedDotNetFile(bool isInvalid = false, string? errorMsg = null)
        {
            return new AnalysedFile(string.Empty, isInvalid ? FileType.Invalid : FileType.Unsupported, new List<string>(), AssemblyType.Managed) 
            { 
                AnalyseError = errorMsg,
                HasBeenAnalysed = false
            };
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            return Equals(obj as AnalysedFile);
        }

        /// <summary>
        /// Determines whether the specified <see cref="AnalysedFile"/> is equal to the current <see cref="AnalysedFile"/>.
        /// </summary>
        public bool Equals(AnalysedFile? other)
        {
            return other is not null &&
                   ID.Equals(other.ID);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        /// <summary>
        /// Equality operator for <see cref="AnalysedFile"/>.
        /// </summary>
        public static bool operator ==(AnalysedFile? left, AnalysedFile? right)
        {
            return EqualityComparer<AnalysedFile>.Default.Equals(left, right);
        }

        /// <summary>
        /// Inequality operator for <see cref="AnalysedFile"/>.
        /// </summary>
        public static bool operator !=(AnalysedFile? left, AnalysedFile? right)
        {
            return !(left == right);
        }
    }
}
