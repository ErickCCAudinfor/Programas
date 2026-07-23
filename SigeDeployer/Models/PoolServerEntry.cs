using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SigeDeployer.Models
{
    public class PoolServerEntry : INotifyPropertyChanged
    {
        private string _serverPath = string.Empty;
        private bool _isChecked = true;

        public string ServerPath
        {
            get => _serverPath;
            set { _serverPath = value; OnPropertyChanged(); }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set { _isChecked = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
