using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;

namespace AssemblyDependencyAnalyser.Implementation.DotNet
{
    internal static class BasicAssemblyInfoHelper
    {
        /// <summary>
        /// Retrieves .NET Framework version information from an assembly.
        /// </summary>
        /// <param name="filePath">The path to the assembly file.</param>
        /// <returns>
        /// A <see cref="DotNetFrameworkVersionInfo"/> instance containing the target framework 
        /// information, or <see langword="null"/> if the assembly has no metadata or a valid 
        /// target framework attribute.
        /// </returns>
        internal static DotNetFrameworkVersionInfo? GetDotNetFrameworkVersionInfo(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var peReader = new PEReader(stream);

            if (!peReader.HasMetadata)
                return null;

            var metadataReader = peReader.GetMetadataReader();

            foreach (var attributeHandle in metadataReader.CustomAttributes)
            {
                var attribute = metadataReader.GetCustomAttribute(attributeHandle);

                var attributeTypeName = GetAttributeTypeFullName(metadataReader, attribute.Constructor);
                if (!string.Equals(attributeTypeName, typeof(TargetFrameworkAttribute).FullName, StringComparison.Ordinal))
                    continue;

                var valueReader = metadataReader.GetBlobReader(attribute.Value);

                // Custom attribute blob prolog must be 0x0001
                if (valueReader.ReadUInt16() != 0x0001)
                    return null;

                // First ctor argument of TargetFrameworkAttribute is frameworkName (string)
                var frameworkName = valueReader.ReadSerializedString();
                if (string.IsNullOrWhiteSpace(frameworkName))
                    return null;

                var frameworkVersionString = ToFrameworkVersionString(frameworkName);
                var version = HelperMethods.GetDotNetVersion(frameworkName);

                return new DotNetFrameworkVersionInfo(frameworkVersionString, version);
            }

            return null;
        }

        private static string? GetAttributeTypeFullName(MetadataReader metadataReader, EntityHandle constructorHandle)
        {
            switch (constructorHandle.Kind)
            {
                case HandleKind.MemberReference:
                    {
                        var memberReference = metadataReader.GetMemberReference((MemberReferenceHandle)constructorHandle);
                        return GetTypeFullName(metadataReader, memberReference.Parent);
                    }

                case HandleKind.MethodDefinition:
                    {
                        var methodDefinition = metadataReader.GetMethodDefinition((MethodDefinitionHandle)constructorHandle);
                        var declaringType = methodDefinition.GetDeclaringType();
                        return GetTypeFullName(metadataReader, declaringType);
                    }

                default:
                    return null;
            }
        }

        private static string? GetTypeFullName(MetadataReader metadataReader, EntityHandle typeHandle)
        {
            switch (typeHandle.Kind)
            {
                case HandleKind.TypeReference:
                    {
                        var typeReference = metadataReader.GetTypeReference((TypeReferenceHandle)typeHandle);
                        var ns = metadataReader.GetString(typeReference.Namespace);
                        var name = metadataReader.GetString(typeReference.Name);
                        return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
                    }

                case HandleKind.TypeDefinition:
                    {
                        var typeDefinition = metadataReader.GetTypeDefinition((TypeDefinitionHandle)typeHandle);
                        var ns = metadataReader.GetString(typeDefinition.Namespace);
                        var name = metadataReader.GetString(typeDefinition.Name);
                        return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
                    }

                default:
                    return null;
            }
        }

        private static string ToFrameworkVersionString(string frameworkName)
        {
            // .NET Standard 2.1 compatible split + manual trim
            var parts = frameworkName.Split([','], StringSplitOptions.RemoveEmptyEntries).
                Select(p => p.Trim()).ToArray();

            if (parts.Length == 0)
                return frameworkName;

            var identifier = parts[0];
            var versionPart = parts.FirstOrDefault(p => p.StartsWith("Version=", StringComparison.OrdinalIgnoreCase));
            if (versionPart is null)
                return frameworkName;

            var version = versionPart.Substring("Version=".Length).TrimStart('v', 'V');

            return identifier switch
            {
                ".NETFramework" => $"net{version.Replace(".", string.Empty)}",
                ".NETCoreApp" => $"net{version}",
                ".NETStandard" => $"netstandard{version}",
                _ => frameworkName
            };
        }
    }
}
