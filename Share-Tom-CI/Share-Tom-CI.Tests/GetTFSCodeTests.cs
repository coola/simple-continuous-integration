using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests
    {
        [Fact]
        public void checkIfWeHaveConnectionToTFS()
        {
            var connectionValidator = new ConnectionValidator();
            var validate = connectionValidator.Validate();
            Assert.True(validate);
        }

        [Fact]
        public void RetrieveCodeFromTFS()
        {
            var codeManager = new CodeManager();
            var codeFolderPath = codeManager.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }

        [Fact]
        public void BuildSolutionOnCode()
        {
            var codeManager = new CodeManager();
            var codeFolderPath = codeManager.GetCode();
            Assert.NotEqual(codeFolderPath, string.Empty);
        }

        [Fact]
        public void CheckGettingSolutionFile()
        {
            var solutionFile = CodeManager.GetSolutionFile(GetTestFolderPath);
            Assert.False(string.IsNullOrEmpty(solutionFile));
        }

        private static string GetTestFolderPath => @"C:\Data\Source\ShareTomBuildDir_2016_10_14-15_47_30_ver_2198";

        [Fact]
        public void CheckBuild()
        {
            var buildManager = new BuildManager(GetTestFolderPath);
            var buildResult = buildManager.BuildSolution();
            Assert.True(buildResult);
        }
    }
}
