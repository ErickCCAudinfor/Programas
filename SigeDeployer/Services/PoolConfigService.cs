using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SigeDeployer.Models;

namespace SigeDeployer.Services
{
    public static class PoolConfigService
    {
        private static readonly string ConfigFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Configs");

        private static readonly string PoolsFile = Path.Combine(ConfigFolder, "Pools.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static List<PoolConfig> LoadAll()
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

            if (File.Exists(PoolsFile))
            {
                try
                {
                    var json = File.ReadAllText(PoolsFile);
                    var list = JsonSerializer.Deserialize<List<PoolConfig>>(json, JsonOptions);
                    if (list != null && list.Count > 0) return list;
                }
                catch { }
            }

            var defaults = CreateDefaults();
            SaveAll(defaults);
            return defaults;
        }

        public static void SaveAll(List<PoolConfig> pools)
        {
            if (!Directory.Exists(ConfigFolder))
                Directory.CreateDirectory(ConfigFolder);

            File.WriteAllText(PoolsFile, JsonSerializer.Serialize(pools, JsonOptions));
        }

        private static List<PoolConfig> CreateDefaults()
        {
            const string sourceBase = @"C:\Users\Administrador.AUDINSERV\Desktop\VersionPool\";

            return new List<PoolConfig>
            {
                new()
                {
                    Name = "Pool",
                    SourceExePath = sourceBase + "SigePool.exe",
                    ExeName = "SigePool.exe",
                    Servers = new()
                    {
                        new() { Path = @"\\172.31.100.13\c$\Audinfor\SigePool", Enabled = false },
                        new() { Path = @"\\172.31.100.19\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.20\c$\Audinfor\SigePool", Enabled = false },
                        new() { Path = @"\\172.31.100.21\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.23\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.25\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.26\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.27\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.34\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.40\c$\Audinfor\SigePool", Enabled = false },
                    }
                },
                new()
                {
                    Name = "Pool Independiente",
                    SourceExePath = sourceBase + "SigePoolIndependiente.exe",
                    ExeName = "SigePoolIndependiente.exe",
                    Servers = new()
                    {
                        new() { Path = @"\\172.31.100.13\c$\Audinfor\SigePoolInd", Enabled = false },
                        new() { Path = @"\\172.31.100.19\c$\Audinfor\SigePoolInd", Enabled = true },
                        new() { Path = @"\\172.31.100.20\c$\Audinfor\SigePoolInd", Enabled = true },
                        new() { Path = @"\\172.31.100.21\c$\Audinfor\SigePoolInd", Enabled = true },
                        new() { Path = @"\\172.31.100.23\c$\Audinfor\SigePoolInd", Enabled = true },
                        new() { Path = @"\\172.31.100.25\c$\Audinfor\SigePoolInd", Enabled = true },
                        new() { Path = @"\\172.31.100.26\c$\Audinfor\SigePoolInd", Enabled = true },
                        new() { Path = @"\\172.31.100.27\c$\Audinfor\SigePoolInd", Enabled = true },
                        new() { Path = @"\\172.31.100.34\c$\Audinfor\SigePoolInd", Enabled = false },
                        new() { Path = @"\\172.31.100.40\c$\Audinfor\SigePoolInd", Enabled = true },
                    }
                },
                new()
                {
                    Name = "Pool Auxiliar",
                    SourceExePath = sourceBase + "SigePoolAuxiliar.exe",
                    ExeName = "SigePoolAuxiliar.exe",
                    Servers = new()
                    {
                        new() { Path = @"\\172.31.100.13\c$\Audinfor\SigePool", Enabled = false },
                        new() { Path = @"\\172.31.100.19\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.20\c$\Audinfor\SigePool", Enabled = false },
                        new() { Path = @"\\172.31.100.21\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.23\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.25\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.26\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.27\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.34\c$\Audinfor\SigePool", Enabled = true },
                        new() { Path = @"\\172.31.100.40\c$\Audinfor\SigePool", Enabled = false },
                    }
                },
                new()
                {
                    Name = "Pool Gas",
                    SourceExePath = sourceBase + "SigePoolGas.exe",
                    ExeName = "SigePoolGas.exe",
                    Servers = new()
                    {
                        new() { Path = @"\\172.31.100.13\c$\Audinfor\SigePoolGas", Enabled = false },
                        new() { Path = @"\\172.31.100.19\c$\Audinfor\SigePoolGas", Enabled = true },
                        new() { Path = @"\\172.31.100.20\c$\Audinfor\SigePoolGas", Enabled = true },
                        new() { Path = @"\\172.31.100.21\c$\Audinfor\SigePoolGas", Enabled = true },
                        new() { Path = @"\\172.31.100.23\c$\Audinfor\SigePoolGas", Enabled = true },
                        new() { Path = @"\\172.31.100.25\c$\Audinfor\SigePoolGas", Enabled = true },
                        new() { Path = @"\\172.31.100.26\c$\Audinfor\SigePoolGas", Enabled = true },
                        new() { Path = @"\\172.31.100.27\c$\Audinfor\SigePoolGas", Enabled = true },
                        new() { Path = @"\\172.31.100.34\c$\Audinfor\SigePoolGas", Enabled = true },
                        new() { Path = @"\\172.31.100.40\c$\Audinfor\SigePoolGas", Enabled = true },
                    }
                }
            };
        }
    }
}
