namespace DependencyAnalysisTests
{
    public static class TestPathHelper
    {
        public static string GetProjectPath()
        {
            // Start in bin\Debug\netX.Y\
            var dir = new DirectoryInfo(AppContext.BaseDirectory);

            // Walk up until you find the project folder (which contains TestFiles)
            while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "TestFiles")))
                dir = dir.Parent;

            if (dir == null)
                throw new DirectoryNotFoundException("Could not find TestFiles folder.");

            return dir.FullName;
        }

        public static string GetTestFilesPath(string subfolder = "")
        {
            var root = GetProjectPath();
            var path = Path.Combine(root, "TestFiles", subfolder);
            return Path.GetFullPath(path);
        }
    }
}
