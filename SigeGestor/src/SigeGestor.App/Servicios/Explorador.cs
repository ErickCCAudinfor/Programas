using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace SigeGestor.App.Servicios;

/// <summary>
/// Abrir en el explorador lo que ha generado una operación.
///
/// Estaba dentro de EjecucionPanel.xaml.cs. Se saca aquí al aparecer el segundo consumidor —la
/// columna «Fichero» de la tabla de actividad— antes de que hubiera dos copias que mantener,
/// igual que se hizo con la propia tabla en Theme\Actividad.xaml.
///
/// La fila de actividad, además, no tiene dónde poner un manejador: su plantilla vive en un
/// diccionario de tema, que no lleva código detrás. Desde un comando del ViewModel sí se puede
/// llamar aquí.
/// </summary>
public static class Explorador
{
    /// <summary>
    /// Abre la carpeta y, si se puede, deja el fichero seleccionado dentro.
    ///
    /// Con un fichero concreto se usa «explorer /select», que es lo que lo deja marcado; con
    /// una carpeta —los PDF, los trozos de XML— se abre y punto, porque ahí el nombre de cada
    /// uno no dice nada.
    ///
    /// La ruta va entre comillas: ConsultasBO cuelga del escritorio y ahí hay espacios en
    /// cuanto el perfil los tiene.
    /// </summary>
    /// <param name="carpeta">Carpeta a abrir. Es lo mínimo que hace falta.</param>
    /// <param name="seleccionar">Fichero a dejar marcado, o cadena vacía.</param>
    /// <param name="dueño">Ventana para los avisos. Si es null se usa la principal.</param>
    public static void Abrir(string carpeta, string seleccionar = "", Window? dueño = null)
    {
        dueño ??= Application.Current?.MainWindow;

        try
        {
            if (seleccionar.Length > 0 && File.Exists(seleccionar))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{seleccionar}\"")
                {
                    UseShellExecute = true
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(carpeta) || !Directory.Exists(carpeta))
            {
                Avisar(dueño,
                    "La carpeta ya no está:" + Environment.NewLine + Environment.NewLine +
                    (string.IsNullOrWhiteSpace(carpeta) ? "(no se guardó la ruta)" : carpeta),
                    MessageBoxImage.Information);
                return;
            }

            Process.Start(new ProcessStartInfo(carpeta) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Avisar(dueño,
                "No se ha podido abrir la carpeta." + Environment.NewLine + Environment.NewLine +
                carpeta + Environment.NewLine + Environment.NewLine +
                $"Detalle: {ex.Message}",
                MessageBoxImage.Warning);
        }
    }

    private static void Avisar(Window? dueño, string texto, MessageBoxImage icono)
    {
        if (dueño is not null)
        {
            MessageBox.Show(dueño, texto, "Abrir carpeta", MessageBoxButton.OK, icono);
            return;
        }

        MessageBox.Show(texto, "Abrir carpeta", MessageBoxButton.OK, icono);
    }
}
