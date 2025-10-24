using DependencyAnalyser.DotNet.CommonInterfaces;
using DependencyAnalyser.DotNet.Implementation;

namespace DependencyAnalyser.DotNet.Extensions
{
    public static class AnalysedFileExtensionMethods
    {
        public static IAnalysedApplicationFile ToAnalysedApplicationFile(this IAnalysedFile analysedFile)
        {
            return new AnalysedApplicationFile(analysedFile.Name, analysedFile.Type, 
                analysedFile.Dependencies.ToList());
        }
    }
}
