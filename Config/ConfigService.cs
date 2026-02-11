using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PocketLLM.Config
{
    public class ConfigService
    {
        private static readonly string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PocketLLM");
        private static readonly string configPath = Path.Combine(folderPath, "config.local.json");
        private static readonly string RUN_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private static readonly string APP_NAME = "PocketLLM";

        public static ConfigClass Load()
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (!File.Exists(configPath))
            {
                var defaultConfig = new ConfigClass();
                Save(defaultConfig);
                return defaultConfig;
            }

            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<ConfigClass>(json);
        }

        public static void Save(ConfigClass config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
        }

        public static void SetAutoStart(bool enable)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true))
            {
                if (enable)
                {
                    key.SetValue(APP_NAME, $"\"{System.Reflection.Assembly.GetExecutingAssembly().Location}\"");
                }
                else
                {
                    key.DeleteValue(APP_NAME, false);
                }
            }
        }
    }
}
