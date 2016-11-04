

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SimpleContinousIntegration
{
    public class TestManager
    {
        private readonly string _buildFolder;
        private readonly List<string> _assemblyList;

        public TestManager(string buildFolder, List<string> assemblyList)
        {
            _buildFolder = buildFolder;
            _assemblyList = assemblyList;
        }

        private string[] GetTestableAssemblies()
        {
            var result = new List<string>();
           
            foreach (var assemblyPath in _assemblyList)
            {
                var directoryName = Path.GetDirectoryName(assemblyPath);
                if (directoryName == null) continue;
                var files = Directory.GetFiles(directoryName);
                if (files.Any(file => file.Contains("xunit") && file.EndsWith(".dll")))
                {
                    result.Add(assemblyPath);
                }
            }

            return result.ToArray();
        }

        public bool RunTests()
        {
            var p = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = _buildFolder,
                    FileName = $@"{CodeManager.AssemblyDirectory()}\xunit.console.exe",
                    Arguments = string.Join(" ", GetTestableAssemblies()),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };
            p.Start();

            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Console.WriteLine("Output:");
            Console.WriteLine(output);

            return p.ExitCode == 0;
        }
    }
}