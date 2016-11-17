using Microsoft.Win32;
using SimpleContinousIntegration.Log;

namespace SimpleContinousIntegration.Environmental
{
    public class LongPathsEnabler
    {
        public static string FileSystemRegistryKey
            => "HKEY_CURRENT_USER\\SYSTEM\\CurrentControlSet\\Control\\FileSystem";

        public static string LongPathsEnabledRegistryDwordEntry => "LongPathsEnabled";

        public static void SetValueIfNeccesarry()
        {
            if (GetValue() != null) return;
            SetValue();
        }

        public static object GetValue()
        {
            return Registry.GetValue(FileSystemRegistryKey,
                LongPathsEnabledRegistryDwordEntry, null);
        }

        public static void SetValue()
        {
            LogManager.Log($"Enabling registry key: {FileSystemRegistryKey} value: {LongPathsEnabledRegistryDwordEntry}", TextColor.Blue);
            Registry.SetValue(FileSystemRegistryKey,
                LongPathsEnabledRegistryDwordEntry, 1);
        }
    }
}