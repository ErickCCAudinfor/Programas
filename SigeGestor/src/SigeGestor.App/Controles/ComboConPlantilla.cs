using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SigeGestor.App.Controles;

/// <summary>
/// Hace que un ComboBox con <c>DisplayMemberPath</c> muestre el valor también cuando está
/// cerrado, con una plantilla propia.
///
/// POR QUÉ HACE FALTA. Al retemplar el ComboBox, lo que se ve cerrado lo pinta un
/// ContentPresenter con <c>SelectionBoxItemTemplate</c>. Y ese valor llega a NULL mientras
/// no se haya desplegado la lista una vez: WPF genera la plantilla de DisplayMemberPath al
/// realizar los contenedores, no al seleccionar. Sin plantilla, el ContentPresenter cae en
/// ToString() y el desplegable del entorno mostraba «SigeGestor.Core.Modelos.EntornoBD» en
/// lugar de «Producción».
///
/// No pasaba antes porque la plantilla de Windows tiene su propio apaño para esto.
///
/// Lo que se hace es lo que haría uno a mano en cada vista: cambiar DisplayMemberPath por un
/// ItemTemplate equivalente. Así, en vez de tocar los 15 desplegables de la aplicación —y
/// acordarse en el siguiente—, lo aplica el estilo implícito a todos.
///
/// OJO: WPF no deja tener los dos a la vez; hay que vaciar DisplayMemberPath ANTES de poner
/// el ItemTemplate o salta «Cannot set both DisplayMemberPath and ItemTemplate».
/// </summary>
public static class ComboConPlantilla
{
    public static readonly DependencyProperty ActivoProperty =
        DependencyProperty.RegisterAttached(
            "Activo", typeof(bool), typeof(ComboConPlantilla),
            new PropertyMetadata(false, AlCambiar));

    public static void SetActivo(DependencyObject d, bool valor) => d.SetValue(ActivoProperty, valor);

    public static bool GetActivo(DependencyObject d) => (bool)d.GetValue(ActivoProperty);

    private static void AlCambiar(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ComboBox combo || !(bool)e.NewValue) return;

        // En Loaded, no aquí: el estilo se aplica antes de que la vista haya puesto
        // DisplayMemberPath, así que ahora mismo todavía estaría vacío.
        combo.Loaded += (_, _) => Convertir(combo);
        if (combo.IsLoaded) Convertir(combo);
    }

    private static void Convertir(ComboBox combo)
    {
        var ruta = combo.DisplayMemberPath;

        if (string.IsNullOrEmpty(ruta) || combo.ItemTemplate is not null) return;

        var texto = new FrameworkElementFactory(typeof(TextBlock));
        texto.SetBinding(TextBlock.TextProperty, new Binding(ruta));
        texto.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);

        // Primero se vacía: los dos juntos lanzan.
        combo.DisplayMemberPath = string.Empty;
        combo.ItemTemplate = new DataTemplate { VisualTree = texto };
    }
}
