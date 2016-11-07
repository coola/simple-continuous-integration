using System.Diagnostics;

namespace SimpleContinousIntegration.Process
{
    public class ProcessManager
    {
        private readonly string _codeFolderPath;
        private readonly string _filePathAndName;
        private readonly string _arguments;

        public ProcessManager(string codeFolderPath, string filePathAndName, string arguments)
        {
            _codeFolderPath = codeFolderPath;
            _filePathAndName = filePathAndName;
            _arguments = arguments;
        }

        public int Run()
        {
             var p = new System.Diagnostics.Process
            {
                StartInfo =
                {
                    WorkingDirectory = _codeFolderPath,
                    FileName = _filePathAndName,
                    Arguments = _arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            p.Start();

            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            LogManager.Log("Output:");
            LogManager.Log(output);

            return p.ExitCode;
        }
    }
}