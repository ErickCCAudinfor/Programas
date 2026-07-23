using System.Collections.Specialized;
using System.Windows;
using Microsoft.Win32;
using SigeDeployer.ViewModels;

namespace SigeDeployer
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;

            _vm.LogEntries.CollectionChanged += LogEntries_CollectionChanged;
        }

        private void LogEntries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                Dispatcher.BeginInvoke(() => LogScrollViewer.ScrollToBottom());
        }

        private void BrowseBasePath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog { Title = "Seleccionar carpeta de versiones" };
            if (dialog.ShowDialog() == true && _vm.SelectedCompany != null)
                _vm.SelectedCompany.BasePath = dialog.FolderName + "\\";
        }

        private void BrowseWinRar_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Seleccionar WinRAR.exe",
                Filter = "Ejecutables (*.exe)|*.exe",
                FileName = "WinRAR.exe"
            };
            if (dialog.ShowDialog() == true && _vm.SelectedCompany != null)
                _vm.SelectedCompany.WinRarPath = dialog.FileName;
        }

        private void BrowsePoolSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Seleccionar ejecutable del pool",
                Filter = "Ejecutables (*.exe)|*.exe"
            };
            if (dialog.ShowDialog() == true && _vm.SelectedPool != null)
            {
                _vm.SelectedPool.SourceExePath = dialog.FileName;
                _vm.SelectedPool.ExeName = System.IO.Path.GetFileName(dialog.FileName);
            }
        }
    }
}