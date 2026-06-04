using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ServiceProcess;

namespace SigeDeployer.Models
{
    public class ServiceEntry : INotifyPropertyChanged
    {
        private ServiceControllerStatus _status = ServiceControllerStatus.Stopped;
        private bool _isChecked = true;
        private string _statusText = "Detenido";
        private bool _isLoading;

        public string ServiceName { get; set; } = string.Empty;
        public string DestinationPath { get; set; } = string.Empty;

        public bool IsChecked
        {
            get => _isChecked;
            set { _isChecked = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTransitioning));
                OnPropertyChanged(nameof(CanStart));
                OnPropertyChanged(nameof(CanStop));
            }
        }

        public ServiceControllerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                StatusText = value switch
                {
                    ServiceControllerStatus.Running      => "Ejecutando",
                    ServiceControllerStatus.Stopped      => "Detenido",
                    ServiceControllerStatus.StartPending => "Iniciando...",
                    ServiceControllerStatus.StopPending  => "Deteniendo...",
                    _ => value.ToString()
                };
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(IsTransitioning));
                OnPropertyChanged(nameof(CanStart));
                OnPropertyChanged(nameof(CanStop));
            }
        }

        public string StatusText
        {
            get => _statusText;
            private set { _statusText = value; OnPropertyChanged(); }
        }

        public bool IsRunning      => _status == ServiceControllerStatus.Running;
        public bool IsTransitioning => _isLoading
                                    || _status == ServiceControllerStatus.StartPending
                                    || _status == ServiceControllerStatus.StopPending;
        public bool CanStart       => !IsTransitioning && _status == ServiceControllerStatus.Stopped;
        public bool CanStop        => !IsTransitioning && _status == ServiceControllerStatus.Running;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
