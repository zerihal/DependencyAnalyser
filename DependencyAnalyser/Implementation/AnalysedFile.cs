using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Implementation
{
    public class AnalysedFile : IAnalysedFile, IEquatable<AnalysedFile?>
    {
        public Guid ID { get; } = Guid.NewGuid();

        public string Name { get; }

        public IList<string> Dependencies { get; }

        public FileType Type { get; }

        public bool HasBeenAnalysed { get; }

        public DotNetFrameworkVersionInfo? DotNetFrameworkVersionInfo { get; }

        public AnalysedFile(string name, FileType type, IList<string> dependencies, DotNetFrameworkVersionInfo? dotnetFrameworkInfo = null)
        {
            Name = name;
            Type = type;
            Dependencies = dependencies;

            if (dotnetFrameworkInfo?.HasValue == true)
                DotNetFrameworkVersionInfo = dotnetFrameworkInfo;

            HasBeenAnalysed = true;
        }

        public static AnalysedFile UnsupportedFile(bool isInvalid = false)
        {
            return new AnalysedFile(string.Empty, isInvalid ? FileType.Invalid : FileType.Unsupported, new List<string>());
        }

        public static AnalysedFile OtherExeFile(string fileName)
        {
            return new AnalysedFile(fileName, FileType.DotNetCoreBootstrapperOrOtherExe, new List<string>());
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AnalysedFile);
        }

        public bool Equals(AnalysedFile? other)
        {
            return other is not null &&
                   ID.Equals(other.ID);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public static bool operator ==(AnalysedFile? left, AnalysedFile? right)
        {
            return EqualityComparer<AnalysedFile>.Default.Equals(left, right);
        }

        public static bool operator !=(AnalysedFile? left, AnalysedFile? right)
        {
            return !(left == right);
        }
    }
}
