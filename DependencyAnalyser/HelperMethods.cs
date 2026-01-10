using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Extensions;
using AssemblyDependencyAnalyser.Implementation;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace AssemblyDependencyAnalyser
{
    public static class HelperMethods
    {
        /// <summary>
        /// Gets the assembly type (Managed, Native, Mixed).
        /// </summary>
        /// <param name="stream">File stream for the assembly.</param>
        /// <returns><see cref="AssemblyType"/> of the input stream.</returns>
        public static AssemblyType GetAssemblyType(Stream stream)
        {
            try
            {
                using var peReader = new PEReader(stream, PEStreamOptions.LeaveOpen);

                if (peReader.HasMetadata && peReader.GetMetadataReader() != null)
                    return AssemblyType.Managed;

                if (peReader.PEHeaders.CorHeader is CorHeader corHeader)
                {
                    if ((corHeader.Flags & CorFlags.ILOnly) == 0)
                        return AssemblyType.Mixed;
                }

                return AssemblyType.Native;
            }
            finally
            {
                // Ensure stream position is reset to 0.
                stream.Position = 0;
            }
        }

        /// <summary>
        /// Gets the assembly type (Managed, Native, Mixed) from a file path.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns><see cref="AssemblyType"/> of the input file.</returns>
        public static AssemblyType GetFileType(string filePath)
        {
            var stream = File.OpenRead(filePath);
            return GetAssemblyType(stream);
        }

        /// <summary>
        /// Converts a byte array to a MemoryStream.
        /// </summary>
        /// <param name="bytes">Input byte array.</param>
        /// <returns>Stream from bytes.</returns>
        public static Stream GetStreamFromBytes(byte[] bytes) => new MemoryStream(bytes);

        public static DotNetFrameworkVersionInfo? GetFrameworkVersionInfo(Assembly assembly)
        {
            var frameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

            if (frameworkAttribute != null)
            {
                var frameworkString = frameworkAttribute.FrameworkDisplayName;
                var netVersion = GetDotNetVersion(frameworkAttribute.FrameworkName);
                if (frameworkString != null || netVersion != null)
                {
                    return new DotNetFrameworkVersionInfo(frameworkString, netVersion);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the .NET version from the Target Framework attribute string.
        /// </summary>
        /// <param name="frameworkAttributeName">Target framework attribute string.</param>
        /// <returns>.NET version (e.g. 4.72, 5, 6, etc), or null if target framework string cannot be parsed.</returns>
        public static double? GetDotNetVersion(string? frameworkAttributeName)
        {
            if (!string.IsNullOrWhiteSpace(frameworkAttributeName) && frameworkAttributeName.Any(char.IsDigit))
            {
                var frameworkAttSubStr = Regex.Match(frameworkAttributeName, @"\d.*").Value;

                if (frameworkAttSubStr != null)
                {
                    var versionStr = string.Empty;

                    foreach (var frameworkAttChar in frameworkAttSubStr)
                    {
                        if (char.IsDigit(frameworkAttChar) || frameworkAttChar.IsDot())
                        {
                            // We have already got major and minor version parts so break
                            if (frameworkAttChar.IsDot() && versionStr.Contains('.'))
                                break;

                            versionStr += frameworkAttChar;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (versionStr != string.Empty && double.TryParse(versionStr, out var dotNetVer))
                    {
                        return dotNetVer;
                    }
                }
            }

            return null;
        }
    }
}
