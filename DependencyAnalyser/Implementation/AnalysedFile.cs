using DependencyAnalyser.DotNet.CommonInterfaces;
using DependencyAnalyser.DotNet.Enums;

namespace DependencyAnalyser.DotNet.Implementation
{
    public class AnalysedFile : IAnalysedFile, IEquatable<AnalysedFile?>
    {
        public Guid ID { get; } = Guid.NewGuid();

        public string Name { get; }

        public IList<string> Dependencies { get; }

        public FileType Type { get; }

        public AnalysedFile(string name, FileType type, IList<string> dependencies)
        {
            Name = name;
            Type = type;
            Dependencies = dependencies;
        }

        public static AnalysedFile UnsupportedFile(bool isInvalid = false)
        {
            return new AnalysedFile(string.Empty, isInvalid ? FileType.Invalid : FileType.Unsupported, new List<string>());
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
