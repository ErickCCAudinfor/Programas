using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SigeDeployer.Models;

namespace SigeDeployer.Services
{
    public class PoolDeploymentService
    {
        private readonly Action<string, LogLevel> _log;

        public PoolDeploymentService(Action<string, LogLevel> logAction)
        {
            _log = logAction;
        }

        public async Task<bool> DeployPoolAsync(
            PoolConfig config,
            IEnumerable<string> servers,
            CancellationToken ct)
        {
            if (!File.Exists(config.SourceExePath))
            {
                _log($"No se encontró el ejecutable origen: {config.SourceExePath}", LogLevel.Error);
                return false;
            }

            _log($"Actualizando {config.Name} → {config.ExeName}", LogLevel.Info);

            bool allOk = true;
            foreach (var server in servers)
            {
                ct.ThrowIfCancellationRequested();
                bool ok = await Task.Run(() => DeployToServer(config, server), ct);
                if (!ok) allOk = false;
            }

            return allOk;
        }

        private bool DeployToServer(PoolConfig config, string server)
        {
            var targetPath = Path.Combine(server, config.ExeName);

            try
            {
                if (File.Exists(targetPath))
                {
                    var backupPath = BuildBackupPath(server, config.ExeName);
                    File.Move(targetPath, backupPath);
                    _log($"  [{server}] Backup: {Path.GetFileName(backupPath)}", LogLevel.Info);
                }
                else
                {
                    _log($"  [{server}] No se encontró {config.ExeName} (se copiará como nuevo)", LogLevel.Warning);
                }

                File.Copy(config.SourceExePath, targetPath, overwrite: true);
                _log($"  [{server}] Copiado nuevo {config.ExeName} ✔", LogLevel.Success);
                return true;
            }
            catch (Exception ex)
            {
                _log($"  [{server}] Error: {ex.Message}", LogLevel.Error);
                return false;
            }
        }

        private static string BuildBackupPath(string server, string exeName)
        {
            var baseName = Path.GetFileNameWithoutExtension(exeName);
            var date = DateTime.Now.ToString("ddMMyyyy");
            var backupPath = Path.Combine(server, $"_{baseName}_{date}.exe");

            if (File.Exists(backupPath))
            {
                var time = DateTime.Now.ToString("HHmmss");
                backupPath = Path.Combine(server, $"_{baseName}_{date}_{time}.exe");
            }

            return backupPath;
        }

        public async Task<bool> CleanBackupsAsync(
            PoolConfig config,
            IEnumerable<string> servers,
            CancellationToken ct)
        {
            var baseName = Path.GetFileNameWithoutExtension(config.ExeName);
            var backupRegex = new Regex(
                $@"^_?{Regex.Escape(baseName)}_?\d{{8}}(_\d{{6}})?\.exe$",
                RegexOptions.IgnoreCase);

            _log($"Limpiando backups de {config.Name} ({baseName})...", LogLevel.Info);

            bool allOk = true;
            int totalFiles = 0;
            long totalBytes = 0;

            foreach (var server in servers)
            {
                ct.ThrowIfCancellationRequested();
                var (ok, files, bytes) = await Task.Run(() => CleanServerBackups(server, backupRegex), ct);
                if (!ok) allOk = false;
                totalFiles += files;
                totalBytes += bytes;
            }

            _log($"Limpieza finalizada: {totalFiles} archivo(s) eliminado(s), {totalBytes / 1024.0 / 1024.0:F1} MB liberados.",
                 totalFiles > 0 ? LogLevel.Success : LogLevel.Info);
            return allOk;
        }

        private (bool ok, int files, long bytes) CleanServerBackups(string server, Regex backupRegex)
        {
            try
            {
                if (!Directory.Exists(server))
                {
                    _log($"  [{server}] Ruta no accesible.", LogLevel.Warning);
                    return (false, 0, 0);
                }

                var backups = Directory.GetFiles(server, "*.exe")
                    .Where(f => backupRegex.IsMatch(Path.GetFileName(f)))
                    .ToList();

                if (!backups.Any())
                {
                    _log($"  [{server}] Sin backups que eliminar.", LogLevel.Info);
                    return (true, 0, 0);
                }

                bool ok = true;
                int deleted = 0;
                long bytes = 0;

                foreach (var file in backups)
                {
                    try
                    {
                        var size = new FileInfo(file).Length;
                        File.Delete(file);
                        deleted++;
                        bytes += size;
                        _log($"  [{server}] Eliminado: {Path.GetFileName(file)} ({size / 1024.0 / 1024.0:F1} MB)", LogLevel.Success);
                    }
                    catch (Exception ex)
                    {
                        _log($"  [{server}] No se pudo eliminar {Path.GetFileName(file)}: {ex.Message}", LogLevel.Error);
                        ok = false;
                    }
                }

                return (ok, deleted, bytes);
            }
            catch (Exception ex)
            {
                _log($"  [{server}] Error: {ex.Message}", LogLevel.Error);
                return (false, 0, 0);
            }
        }
    }
}
