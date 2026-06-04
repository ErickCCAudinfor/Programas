using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SigeDeployer.Models;

namespace SigeDeployer.Services
{
    public static class ConfigService
    {
        private static readonly string ConfigFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Configs");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static List<CompanyConfig> LoadAll()
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

            var list = new List<CompanyConfig>();
            foreach (var file in Directory.GetFiles(ConfigFolder, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var cfg = JsonSerializer.Deserialize<CompanyConfig>(json, JsonOptions);
                    if (cfg != null) list.Add(cfg);
                }
                catch { }
            }

            if (list.Count == 0)
            {
                var defaultCfg = CreateDefault();
                Save(defaultCfg);
                list.Add(defaultCfg);
            }

            return list;
        }

        public static void Save(CompanyConfig config)
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

            var safeName = string.Concat(config.Name.Split(Path.GetInvalidFileNameChars()));
            var path = Path.Combine(ConfigFolder, $"{safeName}.json");
            File.WriteAllText(path, JsonSerializer.Serialize(config, JsonOptions));
        }

        public static void Delete(CompanyConfig config)
        {
            var safeName = string.Concat(config.Name.Split(Path.GetInvalidFileNameChars()));
            var path = Path.Combine(ConfigFolder, $"{safeName}.json");
            if (File.Exists(path)) File.Delete(path);
        }

        public static void Rename(CompanyConfig config, string oldName)
        {
            var oldSafe = string.Concat(oldName.Split(Path.GetInvalidFileNameChars()));
            var oldPath = Path.Combine(ConfigFolder, $"{oldSafe}.json");
            if (File.Exists(oldPath)) File.Delete(oldPath);
            Save(config);
        }

        private static CompanyConfig CreateDefault()
        {
            return new CompanyConfig
            {
                Name = "Empresa por defecto",
                BasePath = @"C:\Users\administrador.AUDINSERV\Desktop\Versiones\",
                WinRarPath = @"C:\Program Files\WinRAR\WinRAR.exe",
                RarPrefix = "Sige.ServerWCF_1_0_",
                RarSuffix = "_0",
                StopTimeoutSeconds = 40,
                UnlockWaitSeconds = 30,
                ExtractionWaitSeconds = 10,
                CopyRetries = 4,
                Services = new()
                {
                    new() { ServiceName = "SigeTotal",           DestinationPath = @"F:\SigeServer\SigeTotal" },
                    new() { ServiceName = "SigeTotalBO",         DestinationPath = @"F:\SigeServer\SigeTotalBO" },
                    new() { ServiceName = "SigeTotalFacturacion",DestinationPath = @"F:\SigeServer\SigeTotalFacturacion" },
                    new() { ServiceName = "SigeTotalFinanzas",   DestinationPath = @"F:\SigeServer\SigeTotalFinanzas" },
                    new() { ServiceName = "SigeTotalSwitching",  DestinationPath = @"F:\SigeServer\SigeTotalSwitching" },
                    new() { ServiceName = "SigeTotalBatch",      DestinationPath = @"G:\SigeServer\SigeTotalBatch" },
                    new() { ServiceName = "SigeTotalBatch2",     DestinationPath = @"G:\SigeServer\SigeTotalBatch2" },
                    new() { ServiceName = "SigeTotalBatch3",     DestinationPath = @"G:\SigeServer\SigeTotalBatch3" },
                    new() { ServiceName = "SigeTotalBatch4",     DestinationPath = @"G:\SigeServer\SigeTotalBatch4" },
                }
            };
        }
    }
}
