using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Extensions;
using AssemblyDependencyAnalyser.Implementation;

namespace DependencyAnalysisTests
{
    public class SingleFile
    {
        [Fact]
        public void DotNetAnalysis()
        {
            var dotnetTestProjectPath = TestPathHelper.GetTestFilesPath(TestFiles.TestApp1Root);
            Assert.True(Directory.Exists(dotnetTestProjectPath), "Test project path does not exist.");

            IDependencyAnalyser analyser = new DependencyAnalyser();

            // This is a .NET core exe so cannot be parsed as not a real assembly. Check that this is the
            // case.
            var analysedExe = analyser.AnalyseAssembly(Path.Combine(dotnetTestProjectPath, TestFiles.TestExe1));
            Assert.True(analysedExe.HasBeenAnalysed, "Unable to analyse .NET core bootstrapper exe.");
            Assert.True(analysedExe.PossibleDotNetCoreBootstrapper, "Did not detect .NET core bootstrapper exe.");

#if NET9_0_OR_GREATER
            // Now test the .NET core dll for the exe. Although this is a DLL, it is really the executable
            // assembly. Analyser should detect this.
            var analysedDll = analyser.AnalyseAssembly(Path.Combine(dotnetTestProjectPath, TestFiles.TestDll1));
            Assert.True(analysedDll.HasBeenAnalysed, "Unable to analyse .NET core assembly dll.");
            Assert.Equal(6, analysedDll.Dependencies.Count);
            Assert.Equal(FileType.DotNetExe, analysedDll.Type);

            analysedDll = analyser.AnalyseAssembly(Path.Combine(dotnetTestProjectPath, TestFiles.TestDll2));
            Assert.True(analysedDll.HasBeenAnalysed, "Unable to analyse .NET core assembly dll.");
            Assert.Equal(2, analysedDll.Dependencies.Count);
            Assert.Equal(FileType.DotNetDll, analysedDll.Type);

            // This should have a reference to Common - check this now.
            var analysedDllCommonDep = analysedDll.GetDependencyAttributes().FirstOrDefault(da => da.Contains("Common"));
            Assert.NotNull(analysedDllCommonDep);
#endif
        }

        [Fact]
        public void StreamFileAnalysis()
        {
            // As per above test but this tests with an input of type stream.
            var dotnetTestProjectPath = TestPathHelper.GetTestFilesPath(TestFiles.TestApp1Root);
            Assert.True(Directory.Exists(dotnetTestProjectPath), "Test project path does not exist.");

            IDependencyAnalyser analyser = new DependencyAnalyser();

            var fileStream = File.OpenRead(Path.Combine(dotnetTestProjectPath, TestFiles.TestDll1));
            var analysedDll = analyser.AnalyseAssembly(fileStream);

#if NET8_0
            Assert.False(analysedDll.HasBeenAnalysed, "Analysis of .NET 9 dll should not be possible from .NET 8 runtime");
#else
            Assert.Equal(6, analysedDll.Dependencies.Count);
            Assert.Equal(FileType.DotNetExe, analysedDll.Type);
#endif
        }

        [Fact]
        public void NativeAnalysis()
        {
            var nativeTestProjectPath = TestPathHelper.GetTestFilesPath(TestFiles.TestApp2Root);
            Assert.True(Directory.Exists(nativeTestProjectPath), "Test project path does not exist.");

            IDependencyAnalyser analyser = new DependencyAnalyser();

            var analysedExe = analyser.AnalyseAssembly(Path.Combine(nativeTestProjectPath, TestFiles.TestExe2));
            Assert.True(analysedExe.HasBeenAnalysed, "Unable to analyse native exe.");
            Assert.Equal(11, analysedExe.Dependencies.Count);
            Assert.Contains("CppTestAppCommon.dll", analysedExe.Dependencies);
            Assert.Equal(FileType.NativeExe, analysedExe.Type);

            var analysedDll = analyser.AnalyseAssembly(Path.Combine(nativeTestProjectPath, TestFiles.TestDll3));
            Assert.True(analysedExe.HasBeenAnalysed, "Unable to analyse native dll.");
            Assert.Equal(5, analysedDll.Dependencies.Count);
            Assert.Equal(FileType.NativeDll, analysedDll.Type);
        }
    }
}
