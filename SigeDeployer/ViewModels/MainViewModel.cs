using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SigeDeployer.Models;
using SigeDeployer.Services;

namespace SigeDeployer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private CompanyConfig? _selectedCompany;
        private string _version = string.Empty;
        private bool _isBusy;
        private string _busyText = string.Empty;
        private CancellationTokenSource? _cts;
        private DeploymentService? _deployer;
        private readonly PoolDeploymentService _poolDeployer;
        private readonly DispatcherTimer _refreshTimer;
        private PoolConfig? _selectedPool;

        public ObservableCollection<CompanyConfig> Companies { get; } = new();
        public ObservableCollection<ServiceEntry> ServiceEntries { get; } = new();
        public ObservableCollection<LogEntry> LogEntries { get; } = new();
        public ObservableCollection<PoolConfig> Pools { get; } = new();
        public ObservableCollection<PoolServerEntry> PoolServerEntries { get; } = new();

        public PoolConfig? SelectedPool
        {
            get => _selectedPool;
            set
            {
                _selectedPool = value;
                OnPropertyChanged();
                LoadPoolServerEntries();
            }
        }

        public CompanyConfig? SelectedCompany
        {
            get => _selectedCompany;
            set
            {
                _selectedCompany = value;
                OnPropertyChanged();
                LoadServiceEntries();
                RefreshDeployer();
                _ = RefreshStatusAsync();
            }
        }

        public string Version
        {
            get => _version;
            set { _version = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsIdle)); }
        }

        public bool IsIdle => !_isBusy;

        public string BusyText
        {
            get => _busyText;
            set { _busyText = value; OnPropertyChanged(); }
        }

        public ICommand InstallWithRarCommand { get; }
        public ICommand InstallDllOnlyCommand { get; }
        public ICommand RefreshStatusCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand StartServiceCommand { get; }
        public ICommand StopServiceCommand { get; }
        public ICommand AddCompanyCommand { get; }
        public ICommand SaveCompanyCommand { get; }
        public ICommand DeleteCompanyCommand { get; }
        public ICommand AddServiceCommand { get; }
        public ICommand RemoveServiceCommand { get; }
        public ICommand ClearLogCommand { get; }
        public ICommand UpdatePoolCommand { get; }
        public ICommand CleanPoolCommand { get; }
        public ICommand SavePoolCommand { get; }
        public ICommand AddPoolCommand { get; }
        public ICommand DeletePoolCommand { get; }
        public ICommand AddPoolServerCommand { get; }
        public ICommand RemovePoolServerCommand { get; }

        public MainViewModel()
        {
            InstallWithRarCommand = new RelayCommand(async _ => await ExecuteDeployAsync(fullDeploy: true), _ => IsIdle && !string.IsNullOrWhiteSpace(Version) && SelectedCompany != null);
            InstallDllOnlyCommand = new RelayCommand(async _ => await ExecuteDeployAsync(fullDeploy: false), _ => IsIdle && !string.IsNullOrWhiteSpace(Version) && SelectedCompany != null);
            RefreshStatusCommand = new RelayCommand(async _ => await RefreshStatusAsync(), _ => IsIdle);
            CancelCommand = new RelayCommand(_ => _cts?.Cancel(), _ => IsBusy);
            StartServiceCommand = new RelayCommand(async p => await StartStopServiceAsync(p as ServiceEntry, start: true), _ => IsIdle);
            StopServiceCommand = new RelayCommand(async p => await StartStopServiceAsync(p as ServiceEntry, start: false), _ => IsIdle);
            AddCompanyCommand = new RelayCommand(_ => AddCompany());
            SaveCompanyCommand = new RelayCommand(_ => SaveCurrentCompany(), _ => SelectedCompany != null);
            DeleteCompanyCommand = new RelayCommand(_ => DeleteCurrentCompany(), _ => SelectedCompany != null && Companies.Count > 1);
            AddServiceCommand = new RelayCommand(_ => AddService(), _ => SelectedCompany != null);
            RemoveServiceCommand = new RelayCommand(p => RemoveService(p as ServiceEntry), _ => SelectedCompany != null);
            ClearLogCommand = new RelayCommand(_ => LogEntries.Clear());
            UpdatePoolCommand = new RelayCommand(async _ => await ExecutePoolDeployAsync(), _ => IsIdle && SelectedPool != null);
            CleanPoolCommand = new RelayCommand(async _ => await ExecutePoolCleanAsync(), _ => IsIdle && SelectedPool != null);
            SavePoolCommand = new RelayCommand(_ => SaveCurrentPool(), _ => SelectedPool != null);
            AddPoolCommand = new RelayCommand(_ => AddPool());
            DeletePoolCommand = new RelayCommand(_ => DeleteCurrentPool(), _ => SelectedPool != null && Pools.Count > 1);
            AddPoolServerCommand = new RelayCommand(_ => AddPoolServer(), _ => SelectedPool != null);
            RemovePoolServerCommand = new RelayCommand(p => RemovePoolServer(p as PoolServerEntry), _ => SelectedPool != null);

            _poolDeployer = new PoolDeploymentService(AddLog);

            LoadCompanies();
            LoadPools();

            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _refreshTimer.Tick += async (_, _) => { if (IsIdle) await RefreshStatusAsync(); };
            _refreshTimer.Start();
        }

        private void LoadCompanies()
        {
            var list = ConfigService.LoadAll();
            Companies.Clear();
            foreach (var c in list) Companies.Add(c);
            SelectedCompany = Companies.FirstOrDefault();
        }

        private void LoadServiceEntries()
        {
            ServiceEntries.Clear();
            if (_selectedCompany == null) return;
            foreach (var svc in _selectedCompany.Services)
                ServiceEntries.Add(new ServiceEntry { ServiceName = svc.ServiceName, DestinationPath = svc.DestinationPath });
        }

        private void RefreshDeployer()
        {
            _deployer = new DeploymentService(AddLog);
        }

        private async Task RefreshStatusAsync()
        {
            if (_deployer == null) return;
            var snapshot = ServiceEntries.ToList();
            var results = await Task.Run(() =>
                snapshot.Select(e => (entry: e, status: _deployer.GetServiceStatus(e.ServiceName))).ToList()
            );
            foreach (var (entry, status) in results)
                entry.Status = status;
        }

        private async Task ExecuteDeployAsync(bool fullDeploy)
        {
            if (SelectedCompany == null || _deployer == null) return;
            var selected = ServiceEntries.Where(e => e.IsChecked)
                .Select(e => new ServiceDestination { ServiceName = e.ServiceName, DestinationPath = e.DestinationPath })
                .ToList();

            if (!selected.Any())
            {
                AddLog("No hay servicios seleccionados.", LogLevel.Warning);
                return;
            }

            _cts = new CancellationTokenSource();
            IsBusy = true;
            BusyText = fullDeploy ? "Instalando con RAR..." : "Instalando DLLs...";

            try
            {
                bool ok = fullDeploy
                    ? await _deployer.RunFullDeployAsync(SelectedCompany, Version, selected, _cts.Token)
                    : await _deployer.RunDllOnlyDeployAsync(SelectedCompany, Version, selected, _cts.Token);

                AddLog(ok ? "✔ Despliegue finalizado correctamente." : "✘ El despliegue terminó con errores.", ok ? LogLevel.Success : LogLevel.Error);
            }
            catch (OperationCanceledException)
            {
                AddLog("Operación cancelada por el usuario.", LogLevel.Warning);
            }
            catch (Exception ex)
            {
                AddLog($"Error inesperado: {ex.Message}", LogLevel.Error);
            }
            finally
            {
                IsBusy = false;
                await RefreshStatusAsync();
            }
        }

        private async Task StartStopServiceAsync(ServiceEntry? entry, bool start)
        {
            if (entry == null || _deployer == null) return;
            entry.IsLoading = true;
            IsBusy = true;
            BusyText = start ? $"Iniciando {entry.ServiceName}..." : $"Deteniendo {entry.ServiceName}...";
            try
            {
                if (start) await _deployer.StartServiceAsync(entry.ServiceName);
                else await _deployer.StopServiceAsync(entry.ServiceName);
                entry.Status = _deployer.GetServiceStatus(entry.ServiceName);
            }
            finally
            {
                entry.IsLoading = false;
                IsBusy = false;
            }
        }

        private void AddLog(string message, LogLevel level)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                LogEntries.Add(new LogEntry { Message = $"[{DateTime.Now:HH:mm:ss}] {message}", Level = level });
                if (LogEntries.Count > 500) LogEntries.RemoveAt(0);
            });
        }

        private void AddCompany()
        {
            var newCompany = new CompanyConfig
            {
                Name = $"Nueva Empresa {Companies.Count + 1}",
                BasePath = @"C:\Versiones\",
                Services = new List<ServiceDestination>()
            };
            ConfigService.Save(newCompany);
            Companies.Add(newCompany);
            SelectedCompany = newCompany;
        }

        private void SaveCurrentCompany()
        {
            if (SelectedCompany == null) return;
            SelectedCompany.Services = ServiceEntries
                .Select(e => new ServiceDestination { ServiceName = e.ServiceName, DestinationPath = e.DestinationPath })
                .ToList();
            ConfigService.Save(SelectedCompany);
            AddLog($"Configuración guardada: {SelectedCompany.Name}", LogLevel.Success);
        }

        private void DeleteCurrentCompany()
        {
            if (SelectedCompany == null || Companies.Count <= 1) return;
            var toDelete = SelectedCompany;
            ConfigService.Delete(toDelete);
            Companies.Remove(toDelete);
            SelectedCompany = Companies.FirstOrDefault();
        }

        private void AddService()
        {
            ServiceEntries.Add(new ServiceEntry { ServiceName = "NuevoServicio", DestinationPath = @"C:\Ruta\Destino" });
        }

        private void RemoveService(ServiceEntry? entry)
        {
            if (entry != null) ServiceEntries.Remove(entry);
        }

        private void LoadPools()
        {
            var list = PoolConfigService.LoadAll();
            Pools.Clear();
            foreach (var p in list) Pools.Add(p);
            SelectedPool = Pools.FirstOrDefault();
        }

        private void LoadPoolServerEntries()
        {
            PoolServerEntries.Clear();
            if (_selectedPool == null) return;
            foreach (var srv in _selectedPool.Servers)
                PoolServerEntries.Add(new PoolServerEntry { ServerPath = srv.Path, IsChecked = srv.Enabled });
        }

        private async Task ExecutePoolDeployAsync()
        {
            if (SelectedPool == null) return;
            var servers = PoolServerEntries.Where(e => e.IsChecked)
                .Select(e => e.ServerPath)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (!servers.Any())
            {
                AddLog("No hay servidores seleccionados.", LogLevel.Warning);
                return;
            }

            _cts = new CancellationTokenSource();
            IsBusy = true;
            BusyText = $"Actualizando {SelectedPool.Name}...";

            try
            {
                bool ok = await _poolDeployer.DeployPoolAsync(SelectedPool, servers, _cts.Token);
                AddLog(ok ? "✔ Pool actualizado correctamente en todos los servidores." : "✘ La actualización del pool terminó con errores.", ok ? LogLevel.Success : LogLevel.Error);
            }
            catch (OperationCanceledException)
            {
                AddLog("Operación cancelada por el usuario.", LogLevel.Warning);
            }
            catch (Exception ex)
            {
                AddLog($"Error inesperado: {ex.Message}", LogLevel.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecutePoolCleanAsync()
        {
            if (SelectedPool == null) return;
            var servers = PoolServerEntries.Where(e => e.IsChecked)
                .Select(e => e.ServerPath)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            if (!servers.Any())
            {
                AddLog("No hay servidores seleccionados.", LogLevel.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Se eliminarán los backups de {SelectedPool.Name} en {servers.Count} servidor(es).\n\n" +
                "El ejecutable activo NO se toca, solo los .exe renombrados con fecha.\n\n¿Continuar?",
                "Limpieza de backups",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            _cts = new CancellationTokenSource();
            IsBusy = true;
            BusyText = $"Limpiando backups de {SelectedPool.Name}...";

            try
            {
                bool ok = await _poolDeployer.CleanBackupsAsync(SelectedPool, servers, _cts.Token);
                if (!ok)
                    AddLog("✘ La limpieza terminó con errores en algunos servidores.", LogLevel.Error);
            }
            catch (OperationCanceledException)
            {
                AddLog("Operación cancelada por el usuario.", LogLevel.Warning);
            }
            catch (Exception ex)
            {
                AddLog($"Error inesperado: {ex.Message}", LogLevel.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SaveCurrentPool()
        {
            if (SelectedPool == null) return;
            SelectedPool.Servers = PoolServerEntries
                .Select(e => new PoolServer { Path = e.ServerPath, Enabled = e.IsChecked })
                .ToList();
            PoolConfigService.SaveAll(Pools.ToList());
            AddLog($"Configuración de pool guardada: {SelectedPool.Name}", LogLevel.Success);
        }

        private void AddPool()
        {
            var newPool = new PoolConfig
            {
                Name = $"Nuevo Pool {Pools.Count + 1}",
                SourceExePath = @"C:\Users\Administrador.AUDINSERV\Desktop\VersionPool\SigePool.exe",
                ExeName = "SigePool.exe",
                Servers = new List<PoolServer>()
            };
            Pools.Add(newPool);
            PoolConfigService.SaveAll(Pools.ToList());
            SelectedPool = newPool;
        }

        private void DeleteCurrentPool()
        {
            if (SelectedPool == null || Pools.Count <= 1) return;
            var toDelete = SelectedPool;
            Pools.Remove(toDelete);
            PoolConfigService.SaveAll(Pools.ToList());
            SelectedPool = Pools.FirstOrDefault();
        }

        private void AddPoolServer()
        {
            PoolServerEntries.Add(new PoolServerEntry { ServerPath = @"\\172.31.100.XX\c$\Audinfor\SigePool" });
        }

        private void RemovePoolServer(PoolServerEntry? entry)
        {
            if (entry != null) PoolServerEntries.Remove(entry);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class LogEntry
    {
        public string Message { get; set; } = string.Empty;
        public LogLevel Level { get; set; }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<object?, Task> _asyncExecute;
        private readonly Func<object?, bool>? _canExecute;
        private bool _isExecuting;

        public RelayCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
        {
            _asyncExecute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            : this(p => { execute(p); return Task.CompletedTask; }, canExecute) { }

        public bool CanExecute(object? parameter)
            => !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

        public async void Execute(object? parameter)
        {
            if (_isExecuting) return;
            _isExecuting = true;
            RaiseCanExecuteChanged();
            try { await _asyncExecute(parameter); }
            finally { _isExecuting = false; RaiseCanExecuteChanged(); }
        }

        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged() =>
            Application.Current?.Dispatcher.BeginInvoke(CommandManager.InvalidateRequerySuggested);
    }
}
