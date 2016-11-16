using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleContinousIntegration.Log;
using SimpleContinousIntegration.Process;

namespace SimpleContinousIntegration.Test
{
    public class TestManager
    {
        private readonly string _buildFolder;
        private readonly List<string> _assemblyList;

        public TestManager(string buildFolder, List<string> assemblyList)
        {
            if (assemblyList == null) throw new ArgumentException("Test assembly list can't be null");
            _buildFolder = buildFolder;
            _assemblyList = assemblyList;
        }

        public string[] GetTestableAssemblies()
        {
            var result = new List<string>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var assemblyPath in _assemblyList)
            {
                var directoryName = Path.GetDirectoryName(assemblyPath);
                if (directoryName == null) continue;
                var strings = directoryName.Split('\\');
                var projectName = strings[strings.Length - 3];
                if (assemblyPath.EndsWith($"{projectName}.dll"))
                {
                    result.Add(assemblyPath);
                }
            }
            return result.ToArray();
        }

        public bool RunTests()
        {
            LogManager.Log("Running tests", TextColor.Red);

            var processManager = new ProcessManager(_buildFolder,
                $@"{BuildFolder.BuildFolderManager.AssemblyDirectory()}\xunit.console.exe", string.Join(" ", GetTestableAssemblies()));
            var exitCode = processManager.Run();

            LogManager.Log("End of running tests", TextColor.Green);

            return exitCode == 0;
        }
    }
}