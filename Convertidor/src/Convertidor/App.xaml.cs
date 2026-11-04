using System.Windows;

namespace Convertidor;

public partial class App : Application
{
    /// <summary>
    /// Arrastrar archivos encima del .exe (o de un acceso directo) hace que Windows lo arranque
    /// con sus rutas como argumentos: llegan aquí y se meten directamente en la lista.
    /// </summary>
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var ventana = new MainWindow();
        ventana.Show();
        if (e.Args.Length > 0) ventana.Anadir(e.Args);
    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(e.Exception.Message, "Convertidor", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
