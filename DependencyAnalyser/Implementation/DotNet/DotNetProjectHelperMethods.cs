using AssemblyDependencyAnalyser.Enums;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AssemblyDependencyAnalyser.Implementation.DotNet
{
    /// <summary>
    /// Provides helper methods for analyzing and extracting metadata from .NET project files.
    /// </summary>
    internal static class DotNetProjectHelperMethods
    {
        // Pattern matches lines like:
        // Project("{GUID}") = "ProjectName", "relative\path\to\project.csproj", "{GUID}"
        private static Regex ProjectLineRegex = new Regex(@"Project\(""\{.*\}""\)\s*=\s*""[^""]+"",\s*""([^""]+)""", 
            RegexOptions.Compiled);

        /// <summary>
        /// Analyses a .NET project file and populates the provided project file model with metadata such as project
        /// type, target frameworks, dependencies, and package references.
        /// </summary>
        /// <remarks>
        /// This method updates the properties of the supplied <paramref name="projectFile"/> instance based on the contents 
        /// of the project file. It determines the project type (such as SDK-style or legacy), extracts target frameworks, 
        /// and identifies both project and non-project dependencies, as well as NuGet package references. The method does 
        /// not return a value; all results are set on the provided model.
        /// </remarks>
        /// <param name="projectFile">
        /// The project file model to populate with analysis results. The <paramref name="projectFile"/> parameter must
        /// have a valid <c>FilePath</c> property pointing to an existing .NET project file.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// Thrown if the file specified by <paramref name="projectFile"/>.<c>FilePath</c> does not exist.
        /// </exception>
        internal static void AnalyseDotNetProjectFile(AnalysedProjectFile projectFile)
        {
            if (!File.Exists(projectFile.FilePath))
                throw new FileNotFoundException($"Project file not found: {projectFile.FilePath}");

            var projFile = TryLoadXmlFile(projectFile.FilePath);

            var root = projFile.Root;

            if (root != null)
            {
                // 1. Get project type (e.g. SDK-style or legacy) and assembly info path (if exists). Note that
                // root element of a csproj file is always the project element.

                projectFile.ProjectType = root?.Attribute("Sdk") != null ? DotNetProjectType.Sdk :
                    root?.Attribute("ToolsVersion") != null ? DotNetProjectType.Legacy : DotNetProjectType.OtherOrUnknown;

                if (!(projectFile.ProjectType == DotNetProjectType.Sdk && GeneratesAssemblyInfo(projFile)))
                {
                    if (Path.GetDirectoryName(projectFile.FilePath) is string csprojDir)
                        projectFile.AssemblyInfoPath = GetAssemblyInfoFile(csprojDir);
                }

                // 2. Get target frameworks.

                var tfmElement = projFile.Descendants().FirstOrDefault(d => d.Name.LocalName == "TargetFramework" ||
                    d.Name.LocalName == "TargetFrameworks");

                projectFile.TargetFrameworks = tfmElement?.Value.Split(';', StringSplitOptions.RemoveEmptyEntries) ?? [];

                // 3. Get project dependencies (i.e. other projects referenced via <ProjectReference>). This rather confusing looking predicate gets
                // the ProjectReference elements, then attribute value for "Include" where it is NOT null or empty and of type string (which always 
                // is, but OfType just sorts out VS precompliler issue), then returns case insensitve list of unique entries.
                projectFile.ProjectDependencies = projFile.Descendants().Where(e => e.Name.LocalName == "ProjectReference").Select(e => e.Attribute("Include")?.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v)).Select(Path.GetFileNameWithoutExtension).OfType<string>()
                    .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                // 4. Get non project dependency references (similar to the above, but need to strip out additional info such as version or culture
                // and do not need to remove file extension).
                projectFile.NonProjectDependencies = projFile.Descendants().Where(e => e.Name.LocalName == "Reference").Select(e => e.Attribute("Include")?.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v!.Split(',')[0]).OfType<string>().Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                // 4. Get package references (i.e. NuGet packages referenced via <PackageReference>).
                projectFile.PackageReferences = projFile.Descendants().Where(e => e.Name.LocalName == "PackageReference").Select(e => e.Attribute("Include")?.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v)).OfType<string>().Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }
        }

        /// <summary>
        /// Loads an XML file from the specified path and returns its contents as an XDocument.
        /// </summary>
        /// <param name="filePath">The path to the XML file to load. The file must exist and be accessible.</param>
        /// <returns>An XDocument representing the contents of the loaded XML file.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the XML file cannot be loaded, such as when the file does not exist, is inaccessible, or is not a
        /// valid XML document.</exception>
        internal static XDocument TryLoadXmlFile(string filePath)
        {
            try
            {
                return XDocument.Load(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load project file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Enumerates all existing project files (.csproj, .vbproj, etc.) in a solution safely.
        /// Works cross-platform and ignores malformed or missing entries.
        /// </summary>
        /// <param name="solutionFilePath">Full path to the .sln file</param>
        /// <returns>Enumerable of absolute paths to project files</returns>
        internal static IEnumerable<string> GetProjectFiles(string solutionFilePath)
        {
            if (!File.Exists(solutionFilePath))
                throw new FileNotFoundException("Solution file not found.", solutionFilePath);

            var solutionDirectory = Path.GetDirectoryName(solutionFilePath)!;
            var slnContent = File.ReadAllLines(solutionFilePath);

            foreach (var line in slnContent)
            {
                if (!line.StartsWith("Project(", StringComparison.OrdinalIgnoreCase))
                    continue;

                var match = ProjectLineRegex.Match(line);
                if (!match.Success)
                    continue; // skip malformed lines

                var relativePath = match.Groups[1].Value
                    .Trim()
                    .Replace('\\', Path.DirectorySeparatorChar) // normalize slashes
                    .Replace('/', Path.DirectorySeparatorChar);

                var fullPath = Path.Combine(solutionDirectory, relativePath);

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"Warning: Project file not found: {fullPath}");
                    continue; // skip missing projects
                }

                yield return fullPath;
            }
        }

        private static string? GetAssemblyInfoFile(string projectDir)
        {
            return Directory.EnumerateFiles(projectDir, "AssemblyInfo.cs", SearchOption.AllDirectories).FirstOrDefault();
        }

        private static bool GeneratesAssemblyInfo(XDocument csprojFile)
        {
            return csprojFile.Descendants("GenerateAssemblyInfo").Any(e => string.Equals(e.Value, "true",
                StringComparison.OrdinalIgnoreCase));
        }
    }
}
