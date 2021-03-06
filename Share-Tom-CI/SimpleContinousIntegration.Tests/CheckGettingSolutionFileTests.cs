﻿using Xunit;

namespace SimpleContinousIntegration.Tests
{
    public class CheckGettingSolutionFileTests
    {
        [Fact]
        public void CheckGettingSolutionFile()
        {
            var codeFolderPath = new TestUtilities().GetCode();
            var solutionFile = BuildFolder.BuildFolderManager.GetSolutionFile(codeFolderPath);
            Assert.False(string.IsNullOrEmpty(solutionFile));
        }
    }
}
