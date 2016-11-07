using System;

namespace SimpleContinousIntegration.Console
{
    class Program
    {

        //private const string testDebugConfiguration = "Debug";
        //private const string testAnyCPUPlatform = "Any CPU";
        //private const string testServiceAddress = "https://keringdev.visualstudio.com/";
        //private const string testUserName = "coola";
        //private const string testPassword = "CoolaHaslo123";
        //private const string testProjectFolderPath = "$/ShARe-Evolution/ShARe-TOM";

        private const string testDebugConfiguration = "Debug";
        private const string testAnyCPUPlatform = "Any CPU";
        private const string testServiceAddress = "https://coola.visualstudio.com/";
        private const string testUserName = "testCoola";
        private const string testPassword = "CoolaHaslo123";
        private const string testProjectFolderPath = "$/CITestProject";

        static void Main(string[] args)
        {
            var testWorkingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Builds\";
            var ci = new CI(testServiceAddress, testProjectFolderPath, testUserName, testPassword, testWorkingDirectoryPath,
                null, testDebugConfiguration, testAnyCPUPlatform);
            ci.RetrieveCodeAndBuildAndRunTestsAndSaveResults();
        }
    }
}
