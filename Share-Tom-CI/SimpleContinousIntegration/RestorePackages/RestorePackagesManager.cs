using SimpleContinousIntegration.Process;

namespace SimpleContinousIntegration.RestorePackages
{
    public class RestorePackagesManager
    {
        private readonly string _codeFolderPath;

        public RestorePackagesManager(string codeFolderPath)
        {
            _codeFolderPath = codeFolderPath;
        }

        public void RestorePackages()
        {
            LogManager.Log("Restoring packages", TextColor.Red);

            var processManager = new ProcessManager(_codeFolderPath, $@"{CodeManager.AssemblyDirectory()}\nuget.exe", "restore");
            processManager.Run();

            LogManager.Log("End of restoring packages", TextColor.Green);
        }
    }
}