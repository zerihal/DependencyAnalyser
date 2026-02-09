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
            Assert.True(File.Exists(TestSln1), $"Test solution not found at: {TestSln1}");
            var analysedSln = new AnalysedSolutionFile(Path.Combine(TestPathHelper.GetTestFilesPath("TestSolution1"), TestSln1));
            analysedSln.AnalyseSolution();

            Assert.Equal(2, analysedSln.Projects.Count);
            Assert.True(analysedSln.Projects.All(p => p.ProjectType == DotNetProjectType.Legacy));

            Assert.True(File.Exists(TestSln2), $"Test solution not found at: {TestSln2}");
            var analysedSln2 = new AnalysedSolutionFile(Path.Combine(TestPathHelper.GetTestFilesPath("TestSolution2"), TestSln2));
            analysedSln2.AnalyseSolution();

            Assert.Equal(2, analysedSln2.Projects.Count);
            Assert.True(analysedSln2.Projects.All(p => p.ProjectType == DotNetProjectType.Sdk));
        }
    }
}
