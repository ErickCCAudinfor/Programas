using System.Windows;
using System.Windows.Media;

namespace SigeGestor.App.Views.Dialogos;

/// <summary>De qué va el diálogo. Decide el color de la marca y el texto de los botones.</summary>
public enum TonoDialogo
{
    /// <summary>Solo informa. Un botón.</summary>
    Aviso,

    /// <summary>Hay que decidir, y lo que se decide escribe en algún sitio.</summary>
    Cuidado,

    /// <summary>Se borra algo. El botón de seguir va en rojo.</summary>
    Destructivo
}

/// <summary>
/// El diálogo de confirmación de la aplicación.
///
/// POR QUÉ NO EL DE WINDOWS. Hasta ahora había trece <c>MessageBox.Show</c> repartidos por
/// siete ficheros. Funcionan, pero son una ventana del sistema: tipografía distinta, botones
/// distintos y —lo que lo destapó— fondo BLANCO cuando la aplicación está en tema oscuro.
/// Salta a la vista que es otra cosa.
///
/// Además el de Windows no deja decir «Guardar la copia» y «No guardarla»: solo Sí y No. Y
/// un «¿Seguir? Sí/No» obliga a releer la pregunta para saber qué hace el Sí.
/// </summary>
public partial class PreguntaWindow : Window
{
    private PreguntaWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Pregunta algo que se puede responder con «adelante» o «déjalo».
    /// Devuelve <c>true</c> solo si se pulsa el botón de adelante.
    /// </summary>
    /// <param name="dueño">La ventana sobre la que sale centrado.</param>
    /// <param name="titulo">De qué va, en una línea.</param>
    /// <param name="cuerpo">Qué va a pasar exactamente: cuántas filas, en qué entorno.</param>
    /// <param name="siTexto">
    /// El botón de seguir, EN VERBO Y DICIENDO QUÉ HACE: «Guardar los cambios», «Borrar el
    /// modelo». Nunca «Sí», que obliga a volver a leer la pregunta.
    /// </param>
    /// <param name="noTexto">El de irse. Por defecto «Cancelar».</param>
    public static bool Preguntar(
        Window? dueño,
        string titulo,
        string cuerpo,
        string siTexto,
        string noTexto = "Cancelar",
        TonoDialogo tono = TonoDialogo.Cuidado)
    {
        var v = Montar(dueño, titulo, cuerpo, tono);

        v.BotonSi.Content = siTexto;
        v.BotonNo.Content = noTexto;

        // El foco en el que NO hace nada: un Intro a ciegas no debe escribir en Producción.
        v.BotonNo.Focus();

        return v.ShowDialog() == true;
    }

    /// <summary>Informa y ya está. Un solo botón.</summary>
    public static void Avisar(
        Window? dueño,
        string titulo,
        string cuerpo,
        TonoDialogo tono = TonoDialogo.Aviso)
    {
        var v = Montar(dueño, titulo, cuerpo, tono);

        v.BotonNo.Visibility = Visibility.Collapsed;
        v.BotonSi.Content = "Entendido";
        v.BotonSi.IsCancel = true;          // que Esc también lo cierre
        v.BotonSi.Focus();

        v.ShowDialog();
    }

    private static PreguntaWindow Montar(Window? dueño, string titulo, string cuerpo, TonoDialogo tono)
    {
        var v = new PreguntaWindow { Titulo = { Text = titulo }, Cuerpo = { Text = cuerpo } };

        // Owner solo si está cargado: con una ventana que aún no se ve, WPF lanza.
        if (dueño is not null && dueño.IsLoaded) v.Owner = dueño;
        else v.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        v.Title = titulo;

        var recurso = tono switch
        {
            TonoDialogo.Destructivo => "BadBrush",
            TonoDialogo.Aviso => "MarcaBrush",
            _ => "WarnBrush"
        };
        v.Marca.Background = (Brush)Application.Current.Resources[recurso];

        if (tono == TonoDialogo.Destructivo)
        {
            v.BotonSi.Background = (Brush)Application.Current.Resources["BadBrush"];
            v.BotonSi.BorderBrush = (Brush)Application.Current.Resources["BadBrush"];

            // OnBad, no blanco: en oscuro el rojo se aclara hasta #F87171 y el blanco encima
            // se queda en 2,8:1. El token se da la vuelta con el tema; el blanco fijo no.
            v.BotonSi.Foreground = (Brush)Application.Current.Resources["OnBadBrush"];
        }

        return v;
    }

    private void Si_Click(object sender, RoutedEventArgs e) => DialogResult = true;

    private void No_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
