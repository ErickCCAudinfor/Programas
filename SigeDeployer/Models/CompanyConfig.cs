using System.Collections.Generic;

namespace SigeDeployer.Models
{
    public class CompanyConfig
    {
        public string Name { get; set; } = string.Empty;
        public string BasePath { get; set; } = string.Empty;
        public string WinRarPath { get; set; } = @"C:\Program Files\WinRAR\WinRAR.exe";
        public string RarPrefix { get; set; } = "Sige.ServerWCF_1_0_";
        public string RarSuffix { get; set; } = "_0";
        public int StopTimeoutSeconds { get; set; } = 40;
        public int UnlockWaitSeconds { get; set; } = 30;
        public int ExtractionWaitSeconds { get; set; } = 10;
        public int CopyRetries { get; set; } = 4;
        public List<ServiceDestination> Services { get; set; } = new();
    }

    public class ServiceDestination
    {
        public string ServiceName { get; set; } = string.Empty;
        public string DestinationPath { get; set; } = string.Empty;
    }
}
