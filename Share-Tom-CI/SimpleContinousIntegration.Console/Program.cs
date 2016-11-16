using System;

namespace SimpleContinousIntegration.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new[] {"https://coola.visualstudio.com/", "$/CITestProject", "testCoola", "CoolaHaslo123"};

            args = new[] {"https://keringdev.visualstudio.com/", "$/ShARe-Evolution/ShARe-TOM", "coola", "CoolaHaslo123"};

            if (args.Length == 0)
            {
                System.Console.Out.WriteLine(string.Empty);
                System.Console.Out.WriteLine("Usage: ");
                System.Console.Out.WriteLine("   SimpleContinousIntegration.Console.exe pathToServer remoteServerProjectPath userName password");
                System.Console.Out.WriteLine(string.Empty);
                System.Console.Out.WriteLine("Example:");
                System.Console.Out.WriteLine("   SimpleContinousIntegration.Console.exe https://coola.visualstudio.com/ $/CITestProject testCoola CoolaHaslo123");
                System.Console.Out.WriteLine(string.Empty);
                System.Console.Out.WriteLine("**************************************************************************************************************");
                System.Console.Out.WriteLine("Please note that userName nad password are those set as alternate authentication credentials on site https://coola.visualstudio.com/_details/security/altcreds");
            }
            else
            {
                var testServiceAddress = args[0];
                var testProjectFolderPath = args[1];
                var testUserName = args[2];
                var testPassword = args[3];
                var testWorkingDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                                               @"\Builds\";
                var ci = new CI(testServiceAddress, testProjectFolderPath, testUserName, testPassword,
                    testWorkingDirectoryPath,
                    null, "Debug", "Any CPU");
                ci.RetrieveCodeAndBuildAndRunTestsAndSaveResultsContinous();
            }
        }
    }
}
