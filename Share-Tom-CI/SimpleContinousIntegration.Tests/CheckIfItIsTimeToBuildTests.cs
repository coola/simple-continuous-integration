using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CheckIfItIsTimeToBuildTests
    {
        [Fact]
        public void CheckIfItIsTimeToBuild()
        {
            var ci = new TestUtilities().CiFactory();
            Assert.False(ci.ItIsTimeToBuild());
        }
    }
}
