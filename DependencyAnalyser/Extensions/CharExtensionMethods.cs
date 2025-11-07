namespace AssemblyDependencyAnalyser.Extensions
{
    public static class CharExtensionMethods
    {
        /// <summary>
        /// Determines if the character is a dot ('.').
        /// </summary>
        /// <param name="c">Input char.</param>
        /// <returns><see langword="True"/> if char is a dot, otherwise <see langword="false"/>.</returns>
        public static bool IsDot(this char c) => c == '.';
    }
}
