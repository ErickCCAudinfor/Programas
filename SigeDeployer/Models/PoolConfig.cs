using System.Collections.Generic;

namespace SigeDeployer.Models
{
    public class PoolConfig
    {
        public string Name { get; set; } = string.Empty;
        public string SourceExePath { get; set; } = string.Empty;
        public string ExeName { get; set; } = string.Empty;
        public List<PoolServer> Servers { get; set; } = new();

        public override string ToString() => Name;
    }

    public class PoolServer
    {
        public string Path { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
    }
}
