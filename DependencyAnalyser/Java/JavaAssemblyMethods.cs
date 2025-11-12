using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Implementation;
using System.IO.Compression;
using System.Xml.Linq;

namespace AssemblyDependencyAnalyser.Java
{
    /// <summary>
    /// Java assembly analysis methods.
    /// </summary>
    public static class JavaAssemblyMethods
    {
        private const string ManifestImpTitle = "Implementation-Title:";
        private const string ManifestImpVersion = "Implementation-Version:";

        /// <summary>
        /// Checks whether the file is a Java assembly (jar file), returning the analysed file if so.
        /// </summary>
        /// <param name="file">File (including path) to check.</param>
        /// <param name="analysedFile">Instance of <see cref="IAnalysedJavaFile"/> if file is a jar file.</param>
        /// <returns><see langword="True"/> if file is a valid jar file, otherwise <see langword="false"/>.</returns>
        public static bool IsJavaArchive(string file, out IAnalysedJavaFile? analysedFile)
        {
            analysedFile = null;

            if (File.Exists(file) && Path.GetExtension(file) == ".jar" && File.OpenRead(file) is FileStream stream)
            {
                return (IsJavaArchive(stream, out analysedFile));
            }

            return false;
        }

        /// <summary>
        /// Checks whether the stream is a Java assembly (jar file), returning the analysed file if so.
        /// </summary>
        /// <param name="file">Stream to check.</param>
        /// <param name="analysedFile">Instance of <see cref="IAnalysedJavaFile"/> if file is a jar stream.</param>
        /// <returns><see langword="True"/> if stream is a valid jar file, otherwise <see langword="false"/>.</returns>
        public static bool IsJavaArchive(Stream file, out IAnalysedJavaFile? analysedFile)
        {
            analysedFile = null;

            try
            {
                // First check whether this is a zip stream (.jar files are essentially a zip archive)
                if (IsZipStream(file))
                {
                    // We can now extract the zip and look for the manifest and pom.xml if the manifest exists
                    // (which will indicate that it is a java archive).
                    file.Position = 0;

                    using (var zip = new ZipArchive(file, ZipArchiveMode.Read, leaveOpen: false))
                    {
                        var manifest = zip.Entries.FirstOrDefault(e => string.Equals(e.FullName, 
                            "META-INF/MANIFEST.MF", StringComparison.OrdinalIgnoreCase));

                        if (manifest != null)
                        {
                            // This is a Java archive - look for pom.xml as this will provide us with dependencies

                            // If there is no pom.xml, this has not been built with Maven structure. In this case
                            // this is just compiled class files and so we can only get metadata from the manifest.
                            // Further analysis can be done by iterative through the classes I think but this needs
                            // to be looked into.

                            var pomFile = zip.Entries.FirstOrDefault(e => e.FullName.EndsWith("/pom.xml", 
                                StringComparison.OrdinalIgnoreCase));

                            if (pomFile != null)
                            {
                                using var pomFs = pomFile.Open();
                                var doc = XDocument.Load(pomFs);

                                // Extract general info
                                var groupId = doc.Descendants("groupId").FirstOrDefault()?.Value;
                                var assemblyName = doc.Descendants("artifactId").FirstOrDefault()?.Value;
                                var version = doc.Descendants("version").FirstOrDefault()?.Value;
                                int? compilerVersion = int.TryParse(doc.Descendants("maven.compiler.target").FirstOrDefault()?.Value, out var cVer) ? cVer : null;

                                // Extract dependencies
                                var pomDependencies = doc.Descendants("dependency")
                                    .Select(d => new
                                    {
                                        GroupId = d.Element("groupId")?.Value,
                                        ArtifactId = d.Element("artifactId")?.Value,
                                        Version = d.Element("version")?.Value
                                    }).ToList();

                                var fileDeps = new List<string>();

                                foreach (var dependency in pomDependencies)
                                {
                                    if (!string.IsNullOrWhiteSpace(dependency.ArtifactId))
                                    {
                                        var addDepInfo = string.Empty;

                                        if (!string.IsNullOrWhiteSpace(dependency.GroupId))
                                            addDepInfo += dependency.GroupId;

                                        if (!string.IsNullOrWhiteSpace(dependency.Version))
                                        {
                                            if (addDepInfo != string.Empty)
                                                addDepInfo += ", ";

                                            addDepInfo += dependency.Version;
                                        }

                                        if (addDepInfo != string.Empty)
                                            addDepInfo = $" ({addDepInfo})";

                                        fileDeps.Add(dependency.ArtifactId + addDepInfo);
                                    }
                                }

                                analysedFile = new AnalysedJavaFile(assemblyName ?? "Java Assembly", fileDeps, groupId, version, compilerVersion);
                                return true;
                            }
                            else
                            {
                                // Try and get assembly name and version from the manifest (if present)
                                var manifestStream = manifest.Open();
                                using var reader = new StreamReader(manifestStream);
                                
                                string? line = null;
                                string? name = null;
                                string? version = null;

                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (line == ManifestImpTitle)
                                    {
                                        name = line.Trim();
                                        continue;
                                    }

                                    if (line == ManifestImpVersion)
                                    {
                                        version = line.Trim();
                                        continue;
                                    }

                                    if (name != null && version != null)
                                        break;
                                }

                                // No pom.xml found - this jar file is just compiled .class files. Analysis is possible at low level
                                // in .NET (bit analysis), but this is complex and potentially unreliable. For now, just return a non
                                // Maven analysed Java file (HasBeenAnalysed will be false).
                                analysedFile = AnalysedJavaFile.NonMavenJavaFile(name, version);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Log?
            }

            return false;
        }

        /// <summary>
        /// Checks whether the stream is a zip (archive) file.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns><see langword="True"/> if stream is from an archive file, otherwise <see langword="false"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if stream cannot seek.</exception>
        private static bool IsZipStream(Stream stream)
        {
            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream must support seeking.");

            long originalPosition = stream.Position;
            Span<byte> signature = stackalloc byte[4];
            int bytesRead = stream.Read(signature);
            stream.Position = originalPosition;

            return bytesRead == 4 &&
                   signature[0] == 0x50 && // 'P'
                   signature[1] == 0x4B && // 'K'
                   (signature[2] == 0x03 || signature[2] == 0x05 || signature[2] == 0x07);
        }
    }
}
