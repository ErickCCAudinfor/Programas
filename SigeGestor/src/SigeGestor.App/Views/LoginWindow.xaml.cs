using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SigeGestor.App.ViewModels;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _vm;

    /// <summary>Evita que sincronizar clave oculta y visible se llame a sí mismo en bucle.</summary>
    private bool _sincronizando;

    public LoginWindow(LoginViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;

        _vm.AccesoConcedido += (_, usuario) => Acceder(usuario);

        // En Loaded la ventana todavía no está activada y Focus() se pierde: el foco acaba
        // en el primer control enfocable del árbol. ContentRendered ya es seguro.
        ContentRendered += (_, _) => Keyboard.Focus(CajaUsuario);
    }

    /// <summary>Usuario autenticado cuando la ventana se cierra con éxito. Null si se cancela.</summary>
    public Usuario? UsuarioAutenticado { get; private set; }

    private void Acceder(Usuario usuario)
    {
        UsuarioAutenticado = usuario;
        DialogResult = true;
        Close();
    }

    private async void Entrar_Click(object sender, RoutedEventArgs e)
    {
        var clave = CajaClaveVisible.Visibility == Visibility.Visible
            ? CajaClaveVisible.Text
            : CajaClave.Password;

        await _vm.EntrarAsync(clave);
    }

    private void Cerrar_Click(object sender, RoutedEventArgs e) => Close();

    private void Ventana_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Arrastrar desde cualquier zona vacía: la ventana no tiene barra de título nativa.
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void CajaClave_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _vm.LimpiarError();

        if (_sincronizando) return;
        _sincronizando = true;
        CajaClaveVisible.Text = CajaClave.Password;
        _sincronizando = false;
    }

    private void CajaClaveVisible_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vm.LimpiarError();

        if (_sincronizando) return;
        _sincronizando = true;
        CajaClave.Password = CajaClaveVisible.Text;
        _sincronizando = false;
    }

    private void VerClave_Click(object sender, RoutedEventArgs e)
    {
        var mostrando = CajaClaveVisible.Visibility == Visibility.Visible;

        if (mostrando)
        {
            CajaClaveVisible.Visibility = Visibility.Collapsed;
            CajaClave.Visibility = Visibility.Visible;
            TachonOjo.Visibility = Visibility.Collapsed;
            CajaClave.Focus();
            CajaClave.SelectAll();
        }
        else
        {
            CajaClave.Visibility = Visibility.Collapsed;
            CajaClaveVisible.Visibility = Visibility.Visible;
            TachonOjo.Visibility = Visibility.Visible;
            CajaClaveVisible.Focus();
            CajaClaveVisible.CaretIndex = CajaClaveVisible.Text.Length;
        }
    }
}
