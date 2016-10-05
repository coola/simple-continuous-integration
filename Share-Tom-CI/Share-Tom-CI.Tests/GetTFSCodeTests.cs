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

    }
}
