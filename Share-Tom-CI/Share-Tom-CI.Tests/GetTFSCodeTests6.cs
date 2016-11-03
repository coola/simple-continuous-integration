using Xunit;

namespace Share_Tom_CI.Tests
{
    public class GetTFSCodeTests6
    {
        [Fact]
        public void CheckGettingSolutionFile()
        {
            var codeFolderPath = GetTFSCodeTests.GetCode();
            var solutionFile = CodeManager.GetSolutionFile(codeFolderPath);
            Assert.False(string.IsNullOrEmpty(solutionFile));
        }
    }
}