using AssemblyDependencyAnalyser.Enums;
using AssemblyDependencyAnalyser.Implementation.DotNet;

namespace DependencyAnalysisTests
{
    public class DotNetSolution
    {
        private const string TestSln1 = "LegacyConsoleApp.slnx";
        private const string TestSln2 = "ClipboardTestExt.sln";

        [Fact]
        public void AnalyseSolution()
        {
            var testSln1Path = Path.Combine(TestPathHelper.GetTestFilesPath("TestSolution1"), TestSln1);
            Assert.True(File.Exists(testSln1Path), $"Test solution not found at: {testSln1Path}");
            var analysedSln = new AnalysedSolutionFile(testSln1Path);
            analysedSln.AnalyseSolution();

            Assert.Equal(2, analysedSln.Projects.Count);
            Assert.True(analysedSln.Projects.All(p => p.ProjectType == DotNetProjectType.Legacy));

            var testSln2Path = Path.Combine(TestPathHelper.GetTestFilesPath("TestSolution2"), TestSln2);
            Assert.True(File.Exists(testSln2Path), $"Test solution not found at: {testSln2Path}");
            var analysedSln2 = new AnalysedSolutionFile(testSln2Path);
            analysedSln2.AnalyseSolution();

            Assert.Equal(2, analysedSln2.Projects.Count);
            Assert.True(analysedSln2.Projects.All(p => p.ProjectType == DotNetProjectType.Sdk));
        }
    }
}
