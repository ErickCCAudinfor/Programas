using System;
using System.Windows.Controls;
using SigeGestor.App.ViewModels;

namespace SigeGestor.App.Views.Paginas;

/// <summary>
/// El historial de cambios de la aplicación. Es el NovedadesForm de ActualizaPrecios.
///
/// Marca como leído al mostrarse, que es lo que hacía la campana del Form1.
/// </summary>
public partial class NovedadesPage : UserControl
{
    private readonly NovedadesViewModel _vm;
    private bool _marcado;

    public NovedadesPage(NovedadesViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;

        // Al mostrarse, no en el constructor: la página se cachea y se reutiliza al volver a
        // entrar, así que el constructor solo corre la primera vez. Y el guard evita reescribir
        // el fichero del .13 cada vez que se navega aquí.
        Loaded += (_, _) =>
        {
            if (_marcado) return;
            _marcado = true;
            _vm.MarcarLeido();
        };
    }
}
