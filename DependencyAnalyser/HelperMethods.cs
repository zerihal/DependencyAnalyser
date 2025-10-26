using DependencyAnalyser.DotNet.Enums;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace DependencyAnalyser.DotNet
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
            using var peReader = new PEReader(stream);

            if (peReader.HasMetadata && peReader.GetMetadataReader() != null)
                return AssemblyType.Managed;

            if (peReader.PEHeaders.CorHeader is CorHeader corHeader)
            {
                if ((corHeader.Flags & CorFlags.ILOnly) == 0)
                    return AssemblyType.Mixed;
            }

            return AssemblyType.Native;
        }

        /// <summary>
        /// Converts a byte array to a MemoryStream.
        /// </summary>
        /// <param name="bytes">Input byte array.</param>
        /// <returns>Stream from bytes.</returns>
        public static Stream GetStreamFromBytes(byte[] bytes) => new MemoryStream(bytes);
    }
}
