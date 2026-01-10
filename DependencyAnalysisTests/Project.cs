using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Implementation;

namespace DependencyAnalysisTests
{
    public class Project
    {
        private const string CommonDll = "Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        private const string CommonExDll = "CommonEx, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        private const string CppCommonDll = "CppTestAppCommon.dll";

        [Fact]
        public void DotNetProject()
        {
            var dotnetTestProjectPath = TestPathHelper.GetTestFilesPath(TestFiles.TestApp1Root);
            Assert.True(Directory.Exists(dotnetTestProjectPath), "Test project path does not exist.");

            var analysedFiles = new DependencyAnalyser().AnalyseApplication(dotnetTestProjectPath)?.ToList();
            Assert.NotNull(analysedFiles);
            Assert.Equal(6, analysedFiles.Count);

#if NET9_0_OR_GREATER
            var common = analysedFiles.FirstOrDefault(f => f.Name == CommonDll);
            var commonEx = analysedFiles.FirstOrDefault(f => f.Name == CommonExDll);

            Assert.NotNull(common);
            Assert.NotNull(commonEx);

            // Check dependants of Common and CommonEx
            Assert.Equal(4, common.Dependents.Count);
            Assert.Equal(2, commonEx.Dependents.Count);
#endif
        }

        [Fact]
        public void NativeProject()
        {
            var nativeTestProjectPath = TestPathHelper.GetTestFilesPath(TestFiles.TestApp2Root);
            Assert.True(Directory.Exists(nativeTestProjectPath), "Test project path does not exist.");

            var analysedFiles = new DependencyAnalyser().AnalyseApplication(nativeTestProjectPath)?.ToList();
            Assert.NotNull(analysedFiles);
            Assert.Equal(3, analysedFiles.Count);

            var cppCommon = analysedFiles.FirstOrDefault(f => f.Name == CppCommonDll);
            Assert.NotNull(cppCommon);

            // Check dependants of CppTestAppCommon.dll
            Assert.Single(cppCommon.Dependents);
        }

        [Fact]
        public void ProjectArchive()
        {
            var archivesPath = TestPathHelper.GetTestFilesPath("Archives");
            Assert.True(Directory.Exists(archivesPath), "Archives path does not exits");

            var analyser = new DependencyAnalyser();
            var analysedFiles = analyser.AnalyseApplicationArchive(Path.Combine(archivesPath, $"{TestFiles.TestApp1Root}.zip"))?.ToList();

#if NET8_0
            // For .NET 8, there should still be 6 analysed files, but several should have analysis errors due the test archive 
            // being created in .NET 9.
            Assert.NotNull(analysedFiles);
            Assert.Equal(6, analysedFiles.Count);
            Assert.NotNull(analysedFiles.FirstOrDefault(f => f.AssemblyType == AssemblyType.Managed && f.AnalyseError != null));
#else
            Assert.NotNull(analysedFiles);
            Assert.Equal(6, analysedFiles.Count);

            analysedFiles = analyser.AnalyseApplicationArchive(Path.Combine(archivesPath, $"{TestFiles.TestApp2Root}.7z"))?.ToList();
            Assert.NotNull(analysedFiles);
            Assert.Equal(3, analysedFiles.Count);

            analysedFiles = analyser.AnalyseApplicationArchive(Path.Combine(archivesPath, $"{TestFiles.TestApp2Root}.tar"))?.ToList();
            Assert.NotNull(analysedFiles);
            Assert.Equal(3, analysedFiles.Count);
#endif
        }
    }
}
