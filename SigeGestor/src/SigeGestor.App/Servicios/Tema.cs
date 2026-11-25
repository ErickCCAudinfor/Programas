using System;
using System.Linq;
using System.Windows;

namespace SigeGestor.App.Servicios;

/// <summary>
/// Claro u oscuro.
///
/// CÓMO FUNCIONA, que no es evidente. El tema oscuro es un diccionario que solo redefine los
/// colores (Theme\Tokens.Oscuro.xaml) y tiene que ir en SEGUNDO lugar, justo detrás de
/// Tokens.xaml y DELANTE de todo lo demás.
///
/// No vale fundirlo al final. Un <c>StaticResource</c> se resuelve UNA vez, al parsear el
/// diccionario que lo contiene: Controls.xaml pregunta por PanelBrush al cargarse y, si
/// entonces manda el claro, se queda con el blanco para siempre. Añadir el oscuro después
/// cambia lo que piden las VISTAS —que se cargan más tarde— pero no lo que ya pidieron los
/// estilos. Se vio en la primera prueba: paneles oscuros, título negro y cajas blancas.
///
/// Por eso cambiar de tema no es añadir ni quitar un diccionario: es rehacer la lista entera
/// en orden. Y como las ventanas ya pintadas tampoco revisan lo que resolvieron, el cambio
/// se aplica a lo que se crea DESPUÉS.
/// </summary>
public static class Tema
{
    /// <summary>Los diccionarios en su orden, sin el del tema.</summary>
    private static readonly string[] Orden =
    {
        "Theme/Tokens.xaml",
        "Theme/Typography.xaml",
        "Theme/Icons.xaml",
        "Theme/Controls.xaml",
        "Theme/Desplegables.xaml",
        "Theme/Shell.xaml",
        "Theme/Formulario.xaml",
        "Theme/Rejilla.xaml",
        "Theme/Actividad.xaml"
    };

    private const string Oscuro = "Theme/Tokens.Oscuro.xaml";

    /// <summary>Lo que hay puesto ahora mismo.</summary>
    public static bool EsOscuro { get; private set; }

    /// <summary>
    /// Rehace los diccionarios de la aplicación con el tema pedido. Si ya está puesto no
    /// hace nada: reconstruir por gusto tira todos los pinceles y repinta entero.
    /// </summary>
    public static void Aplicar(bool oscuro)
    {
        if (oscuro == EsOscuro && Application.Current.Resources.MergedDictionaries.Count > 0) return;

        var fundidos = Application.Current.Resources.MergedDictionaries;
        fundidos.Clear();

        foreach (var fuente in Orden)
        {
            fundidos.Add(Cargar(fuente));

            // El oscuro, pegado detrás de Tokens: pisa sus colores antes de que nadie
            // más los lea.
            if (oscuro && fuente == Orden[0]) fundidos.Add(Cargar(Oscuro));
        }

        EsOscuro = oscuro;
    }

    // Ojo: el ensamblado se llama SigeGestor, no SigeGestor.App —eso es el namespace—.
    private static ResourceDictionary Cargar(string fuente) =>
        new() { Source = new Uri($"pack://application:,,,/SigeGestor;component/{fuente}") };
}
