using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using SigeDeployer.Models;

namespace SigeDeployer.Services
{
    public class DeploymentService
    {
        private readonly Action<string, LogLevel> _log;

        public DeploymentService(Action<string, LogLevel> logAction)
        {
            _log = logAction;
        }

        public async Task<bool> RunFullDeployAsync(
            CompanyConfig config,
            string version,
            IEnumerable<ServiceDestination> selectedServices,
            CancellationToken ct)
        {
            var services = selectedServices.ToList();
            var rarFile = Path.Combine(config.BasePath, $"{config.RarPrefix}{version}{config.RarSuffix}.rar");
            var extractFolder = config.BasePath;

            if (!File.Exists(rarFile))
            {
                _log($"No se encontró el archivo RAR: {rarFile}", LogLevel.Error);
                return false;
            }

            _log($"Descomprimiendo versión {version}...", LogLevel.Info);
            bool extracted = await ExtractRarAsync(config.WinRarPath, rarFile, extractFolder, ct);
            if (!extracted) return false;

            _log($"Esperando {config.ExtractionWaitSeconds}s para asegurar la descompresión...", LogLevel.Warning);
            await Task.Delay(config.ExtractionWaitSeconds * 1000, ct);

            var sourceFolder = Path.Combine(config.BasePath, $"{config.RarPrefix}{version}{config.RarSuffix}");
            if (!Directory.Exists(sourceFolder) || !Directory.EnumerateFiles(sourceFolder).Any())
            {
                _log($"Carpeta descomprimida no existe o vacía: {sourceFolder}", LogLevel.Error);
                return false;
            }
            _log($"Descompresión validada: {sourceFolder}", LogLevel.Success);

            return await DeployFromFolderAsync(config, sourceFolder, services, ct);
        }

        public async Task<bool> RunDllOnlyDeployAsync(
            CompanyConfig config,
            string version,
            IEnumerable<ServiceDestination> selectedServices,
            CancellationToken ct)
        {
            var services = selectedServices.ToList();
            var sourceFolder = Path.Combine(config.BasePath, $"{config.RarPrefix}{version}{config.RarSuffix}");

            if (!Directory.Exists(sourceFolder) || !Directory.EnumerateFiles(sourceFolder).Any())
            {
                _log($"Carpeta origen no existe o vacía: {sourceFolder}", LogLevel.Error);
                return false;
            }
            _log($"Carpeta origen validada: {sourceFolder}", LogLevel.Success);

            return await DeployFromFolderAsync(config, sourceFolder, services, ct);
        }

        private async Task<bool> DeployFromFolderAsync(
            CompanyConfig config,
            string sourceFolder,
            List<ServiceDestination> services,
            CancellationToken ct)
        {
            var serviceNames = services.Select(s => s.ServiceName).ToArray();

            _log("Deteniendo servicios...", LogLevel.Info);
            await StopServicesAsync(serviceNames);

            bool allStopped = await WaitForServicesStoppedAsync(serviceNames, config.StopTimeoutSeconds, ct);
            if (!allStopped)
            {
                _log("No todos los servicios se detuvieron. Abortando.", LogLevel.Error);
                return false;
            }

            _log($"Esperando {config.UnlockWaitSeconds}s para desbloqueo de archivos...", LogLevel.Warning);
            await Task.Delay(config.UnlockWaitSeconds * 1000, ct);

            _log("Iniciando copia de archivos...", LogLevel.Info);
            bool copyOk = await CopyFilesAsync(sourceFolder, services, config.CopyRetries, ct);

            _log("Iniciando servicios...", LogLevel.Info);
            await StartServicesAsync(serviceNames);

            _log("Proceso completado.", LogLevel.Success);
            return copyOk;
        }

        private async Task StopServicesAsync(string[] serviceNames)
        {
            var tasks = serviceNames.Select(name => Task.Run(() =>
            {
                try
                {
                    using var svc = new ServiceController(name);
                    if (svc.Status != ServiceControllerStatus.Stopped)
                    {
                        svc.Stop();
                        _log($"  Deteniendo: {name}", LogLevel.Info);
                    }
                    else
                    {
                        _log($"  Ya estaba detenido: {name}", LogLevel.Warning);
                    }
                }
                catch (Exception ex)
                {
                    _log($"  Error al detener {name}: {ex.Message}", LogLevel.Error);
                }
            }));
            await Task.WhenAll(tasks);
        }

        private async Task<bool> WaitForServicesStoppedAsync(string[] serviceNames, int timeoutSeconds, CancellationToken ct)
        {
            var deadline = DateTime.Now.AddSeconds(timeoutSeconds);
            while (DateTime.Now < deadline)
            {
                ct.ThrowIfCancellationRequested();
                var stillRunning = serviceNames.Where(name =>
                {
                    try { using var s = new ServiceController(name); return s.Status != ServiceControllerStatus.Stopped; }
                    catch { return false; }
                }).ToList();

                if (!stillRunning.Any()) return true;
                _log($"  Esperando: {string.Join(", ", stillRunning)}", LogLevel.Warning);
                await Task.Delay(2000, ct);
            }

            var notStopped = serviceNames.Where(name =>
            {
                try { using var s = new ServiceController(name); return s.Status != ServiceControllerStatus.Stopped; }
                catch { return false; }
            }).ToList();

            if (notStopped.Any())
                _log($"Timeout. Servicios no detenidos: {string.Join(", ", notStopped)}", LogLevel.Error);

            return !notStopped.Any();
        }

        private async Task<bool> CopyFilesAsync(
            string sourceFolder,
            List<ServiceDestination> services,
            int maxRetries,
            CancellationToken ct)
        {
            bool allOk = true;
            foreach (var svc in services)
            {
                ct.ThrowIfCancellationRequested();
                int retries = maxRetries;
                bool copied = false;
                while (retries > 0 && !copied)
                {
                    try
                    {
                        _log($"  Copiando a {svc.DestinationPath}...", LogLevel.Info);
                        foreach (var file in Directory.GetFiles(sourceFolder))
                        {
                            var dest = Path.Combine(svc.DestinationPath, Path.GetFileName(file));
                            File.Copy(file, dest, overwrite: true);
                        }
                        _log($"  Copia completada → {svc.DestinationPath}", LogLevel.Success);
                        copied = true;
                    }
                    catch (Exception ex)
                    {
                        retries--;
                        _log($"  Error copiando a {svc.DestinationPath}: {ex.Message} (reintentos: {retries})", LogLevel.Warning);
                        if (retries > 0) await Task.Delay(5000, ct);
                    }
                }
                if (!copied)
                {
                    _log($"  FALLO al copiar a {svc.DestinationPath} tras {maxRetries} intentos.", LogLevel.Error);
                    allOk = false;
                }
            }
            return allOk;
        }

        private async Task StartServicesAsync(string[] serviceNames)
        {
            var tasks = serviceNames.Select(name => Task.Run(() =>
            {
                try
                {
                    using var svc = new ServiceController(name);
                    if (svc.Status == ServiceControllerStatus.Running)
                    {
                        _log($"  {name} ya está en ejecución.", LogLevel.Warning);
                        return;
                    }
                    _log($"  Iniciando {name}...", LogLevel.Warning);
                    svc.Start();
                    svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    _log($"  {name} → Iniciado ✔", LogLevel.Success);
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    _log($"  Timeout esperando inicio de {name}.", LogLevel.Error);
                }
                catch (Exception ex)
                {
                    _log($"  Error al iniciar {name}: {ex.Message}", LogLevel.Error);
                }
            }));
            await Task.WhenAll(tasks);
        }

        private async Task<bool> ExtractRarAsync(string winRarPath, string rarFile, string destination, CancellationToken ct)
        {
            if (!File.Exists(winRarPath))
            {
                _log($"WinRAR no encontrado en: {winRarPath}", LogLevel.Error);
                return false;
            }

            var tcs = new TaskCompletionSource<bool>();
            var psi = new ProcessStartInfo
            {
                FileName = winRarPath,
                Arguments = $"x -y \"{rarFile}\" \"{destination}\\\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            process.OutputDataReceived += (_, e) => { if (e.Data != null) _log($"  [WinRAR] {e.Data}", LogLevel.Info); };
            process.ErrorDataReceived += (_, e) => { if (e.Data != null) _log($"  [WinRAR ERR] {e.Data}", LogLevel.Warning); };
            process.Exited += (_, _) => tcs.TrySetResult(process.ExitCode == 0);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            ct.Register(() => { try { process.Kill(); } catch { } });

            bool success = await tcs.Task;
            if (!success) _log("Error al descomprimir el archivo RAR.", LogLevel.Error);
            else _log("Descompresión completada.", LogLevel.Success);

            return success;
        }

        public ServiceControllerStatus GetServiceStatus(string serviceName)
        {
            try
            {
                using var svc = new ServiceController(serviceName);
                return svc.Status;
            }
            catch
            {
                return ServiceControllerStatus.Stopped;
            }
        }

        public async Task StartServiceAsync(string serviceName)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var svc = new ServiceController(serviceName);
                    if (svc.Status == ServiceControllerStatus.Running)
                    {
                        _log($"{serviceName} ya está en ejecución.", LogLevel.Warning);
                        return;
                    }
                    _log($"Iniciando {serviceName}...", LogLevel.Warning);
                    svc.Start();
                    svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    _log($"{serviceName} → Iniciado ✔", LogLevel.Success);
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    _log($"Timeout al iniciar {serviceName}.", LogLevel.Error);
                }
                catch (Exception ex)
                {
                    _log($"Error al iniciar {serviceName}: {ex.Message}", LogLevel.Error);
                }
            });
        }

        public async Task StopServiceAsync(string serviceName)
        {
            await Task.Run(() =>
            {
                try
                {
                    using var svc = new ServiceController(serviceName);
                    if (svc.Status == ServiceControllerStatus.Stopped)
                    {
                        _log($"{serviceName} ya está detenido.", LogLevel.Warning);
                        return;
                    }
                    _log($"Deteniendo {serviceName}...", LogLevel.Warning);
                    svc.Stop();
                    svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                    _log($"{serviceName} → Detenido ✔", LogLevel.Success);
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    _log($"Timeout al detener {serviceName}.", LogLevel.Error);
                }
                catch (Exception ex)
                {
                    _log($"Error al detener {serviceName}: {ex.Message}", LogLevel.Error);
                }
            });
        }
    }

    public enum LogLevel { Info, Success, Warning, Error }
}
