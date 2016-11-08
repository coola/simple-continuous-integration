using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CheckIfGoodCommitIsOKTests
    {
        [Fact]
        public void CheckIfGoodCommitIsOK()
        {
            var mainTestsClass = new TestUtilities();
            var testCiConnectionManager = mainTestsClass.GetTestCIConnectionManager();
            Assert.True(testCiConnectionManager.Validate());
            var resultOfBuild = mainTestsClass.RetrieveCodeAndBuild(TestUtilities.TestCommits.BuildOKTestWrong);
            Assert.True(resultOfBuild);
        }
    }
}