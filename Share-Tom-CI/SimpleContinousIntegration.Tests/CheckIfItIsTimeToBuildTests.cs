using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CheckIfItIsTimeToBuildTests
    {
        [Fact]
        public void CheckIfItIsTimeToBuild()
        {
            var ci = new GetTFSCodeTests().CiFactory();
            Assert.False(ci.ItIsTimeToBuild());
        }
    }
}
