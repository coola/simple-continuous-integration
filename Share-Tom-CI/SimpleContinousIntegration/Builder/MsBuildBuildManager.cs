using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using SimpleContinousIntegration.Process;
using SimpleContinousIntegration.RestorePackages;

namespace SimpleContinousIntegration.Builder
{
    public class MsBuildBuildManager : IBuilder
    {
        private const string defaultTarget = "Build";
        private readonly string _codeFolderPath;
        private readonly string _configuration;
        private readonly string _platform;

        public MsBuildBuildManager(string codeFolderPath, string configuration, string platform)
        {
            _codeFolderPath = codeFolderPath;
            _configuration = configuration;
            _platform = platform;
             CurrentAssemblyList = new List<string>();
        }

        public static string GetCurrentMsBuildPath()
        {
            var value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\MSBuild\\ToolsVersions\\14.0", "MSBuildToolsPath", null );
            return value as string;
        }

        public bool BuildSolution()
        {
            var restorePackagesManager = new RestorePackagesManager(_codeFolderPath);
            restorePackagesManager.RestorePackages();
            
            var processManager = new ProcessManager(_codeFolderPath, Path.Combine(GetCurrentMsBuildPath(), "msbuild.exe"), 
                $"/property:Configuration=\"{_configuration}\" /property:Platform=\"{_platform}\"");
            var exitCode = processManager.Run();

            PopulateAssemblyList();

            return exitCode == 0;
        }

        private void PopulateAssemblyList()
        {
            var enumerateFiles = Directory.EnumerateFiles(
                _codeFolderPath, "*.*", SearchOption.AllDirectories).ToList();

            var onlyDlls = enumerateFiles.Where(a => a.EndsWith(".dll") && !a.Contains("xunit"));

            CurrentAssemblyList.AddRange(onlyDlls);
        }

        public List<string> CurrentAssemblyList { get; }
    }
}