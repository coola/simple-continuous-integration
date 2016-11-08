using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CheckIfGoodCommitIsOKTests
    {
        [Fact]
        public void CheckIfGoodCommitIsOK()
        {
            var mainTestsClass = new GetTFSCodeTests();
            var testCiConnectionManager = mainTestsClass.GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var resultOfBuild = mainTestsClass.RetrieveCodeAndBuild(GetTFSCodeTests.TestCommits.BuildOKTestWrong);
            Assert.True(resultOfBuild);
        }
    }
}