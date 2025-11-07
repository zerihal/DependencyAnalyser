using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation
{
    public class AnalysedFile : IAnalysedFile, IEquatable<AnalysedFile?>
    {
        private bool _dotNetCoreExeIndicator;

        /// <inheritdoc/>
        public Guid ID { get; } = Guid.NewGuid();

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IList<string> Dependencies { get; }

        /// <inheritdoc/>
        public FileType Type { get; }

        /// <inheritdoc/>
        public bool HasBeenAnalysed { get; }

        /// <inheritdoc/>
        public bool PossibleDotNetCoreBootstrapper => Type == FileType.NativeExe && _dotNetCoreExeIndicator;

        /// <inheritdoc/>
        public DotNetFrameworkVersionInfo? DotNetFrameworkVersionInfo { get; }

        /// <summary>
        /// Creates an instance of <see cref="IAnalysedFile"/>.
        /// </summary>
        /// <param name="name">Analysed file name.</param>
        /// <param name="type">Analysed file type (<see cref="FileType"/>).</param>
        /// <param name="dependencies">List of dependencies.</param>
        /// <param name="dotnetFrameworkInfo">.NET framework info (if applicable - default is <see langword="null")./></param>
        public AnalysedFile(string name, FileType type, IList<string> dependencies, DotNetFrameworkVersionInfo? dotnetFrameworkInfo = null)
        {
            Name = name;
            Type = type;
            Dependencies = dependencies;

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
        /// <param name="dotNetCoreExeIndicator">Flag to indicate whether this is a possible .NET core exe.</param>
        public AnalysedFile(string name, FileType type, IList<string> dependencies, bool dotNetCoreExeIndicator)
            : this(name, type, dependencies)
        {
            _dotNetCoreExeIndicator = dotNetCoreExeIndicator;
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
            return new AnalysedFile(string.Empty, isInvalid ? FileType.Invalid : FileType.Unsupported, new List<string>());
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
