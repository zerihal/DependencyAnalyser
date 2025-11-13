using AssemblyDependencyAnalyser.Enums;

namespace AssemblyDependencyAnalyser.Exceptions
{
    /// <summary>
    /// File analysis exception.
    /// </summary>
    public class FileAnalysisException : Exception
    {
        /// <summary>
        /// Source assembly type.
        /// </summary>
        public AssemblyType SourceType { get; }

        /// <summary>
        /// File type.
        /// </summary>
        public FileType FileType { get; }

        /// <summary>
        /// Default constructor to create instance of <see cref="FileAnalysisException"/>.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public FileAnalysisException(string? message) : base(message)
        {
            SourceType = AssemblyType.Unknown;
            FileType = FileType.Invalid;
        }

        /// <summary>
        /// Creates instance of <see cref="FileAnalysisException"/> with source assembly type.
        /// </summary>
        /// <param name="sourceType">Source assembly type.</param>
        /// <param name="message">Exception message.</param>
        public FileAnalysisException(AssemblyType sourceType, string? message) : this(message)
        {
            SourceType = sourceType;
        }

        /// <summary>
        /// Creates instance of <see cref="FileAnalysisException"/> with source assembly and file type.
        /// </summary>
        /// <param name="sourceType">Source assembly type.</param>
        /// <param name="fileType">File type.</param>
        /// <param name="message">Exception message.</param>
        public FileAnalysisException(AssemblyType sourceType, FileType fileType, string? message) : base(message)
        {
            SourceType = sourceType;
            FileType = fileType;
        }
    }
}
