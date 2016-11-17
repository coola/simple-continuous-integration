using Microsoft.Win32;
using SimpleContinousIntegration.Environmental;
using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class RegistryTests
    {
        [Fact]
        public void TestLongPathEntry()
        {
            LongPathsEnabler.SetValue();
            var longPathsEnabled = LongPathsEnabler.GetValue();
            Assert.NotNull(longPathsEnabled);
        }
    }
}