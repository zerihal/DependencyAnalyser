using AssemblyDependencyAnalyser.CommonInterfaces;
using AssemblyDependencyAnalyser.Implementation;

namespace DependencyAnalysisTests
{
    public class SingleFile
    {
        [Fact]
        public void DotNetAnalysis()
        {
            var dotnetTestProjectPath = TestPathHelper.GetTestFilesPath("TestApp1");
            Assert.True(Directory.Exists(dotnetTestProjectPath), "Test project path does not exist.");

            IDependencyAnalyser analyser = new DependencyAnalyser();

            var analysedExe = analyser.AnalyseAssembly(Path.Combine(dotnetTestProjectPath, "AssemblyAnalyserTestAppDotNet.dll"));
        }
    }
}
