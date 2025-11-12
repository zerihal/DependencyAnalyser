# Dependency Analyser for managed and native assemblies
Provides methods for simple analysis of single assembly files and multiple files (such as from a project directory or zip archive).

## Analysis methods include

- AnalyseAssembly(string assemblyPath) - returns an instance of IAnalysedFile that provides basic analysis of the input file and list of dependencies.
- AnalyseAssembly(object assembly) - as above, but takes an object as the argument, which can be an Assembly object, stream, or byte array.
- AnalyseApplication(string projectPath) - returns a collection of IAnalysedApplicationFile, which includes the dependencies and dependants for each assembly within the project path, allowing relationships to be mapped between the different assemblies.
- AnalyseApplicationArchive(string archiveFile) - as above, but takes an archive file containing the project files (supporting at least zip, 7z, tar, and gz).
