using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Seguridad;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Una empresa en la rejilla. Envuelve el modelo para poder mostrar el estado de la última
/// prueba de conexión sin guardarlo en el JSON.
/// </summary>
public sealed partial class EmpresaVm : ObservableObject
{
    public EmpresaVm(EmpresaBD empresa)
    {
        Empresa = empresa;
    }

    public EmpresaBD Empresa { get; }

    public string Nombre => Empresa.Nombre;
    public string Servidor => Empresa.Servidor;
    public string BaseDatos => Empresa.BaseDatos;
    public string Vpn => Empresa.VPN ? "Sí" : "—";

    /// <summary>
    /// El usuario, descifrado, solo para verlo en la rejilla. La contraseña no se muestra
    /// nunca: no hay ninguna razón para tenerla en pantalla y sí para no tenerla.
    /// </summary>
    public string Usuario
    {
        get
        {
            try { return Cifrado.Descifrar(Empresa.Usuario); }
            catch { return "(no se puede descifrar)"; }
        }
    }

    [ObservableProperty]
    private string _estado = string.Empty;

    [ObservableProperty]
    private bool _estadoBien;
}

/// <summary>
/// La pantalla de empresas: la lista de bases de clientes de Config\Empresas.json.
///
/// NO SON LOS ENTORNOS. Los entornos —Producción, Réplica, UAT— son tres, están en
/// Entornos.json y no se editan desde aquí: llevan las credenciales con las que se lanza todo y
/// tocarlas en caliente desde la aplicación es pedir un disgusto. Las empresas son la lista
/// abierta de bases de clientes, y solo se usan para los modelos de impresión.
/// </summary>
public sealed partial class EmpresasViewModel : ObservableObject
{
    private readonly RepositorioEmpresas _repositorio = new();

    public EmpresasViewModel()
    {
        Recargar();
    }

    public ObservableCollection<EmpresaVm> Empresas { get; } = new();

    public string Ruta => _repositorio.Ruta;

    [ObservableProperty]
    private string _error = string.Empty;

    public Visibility VisibilidadError =>
        string.IsNullOrEmpty(Error) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnErrorChanged(string value) => OnPropertyChanged(nameof(VisibilidadError));

    [ObservableProperty]
    private string _aviso = string.Empty;

    public Visibility VisibilidadAviso =>
        string.IsNullOrEmpty(Aviso) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnAvisoChanged(string value) => OnPropertyChanged(nameof(VisibilidadAviso));

    [ObservableProperty]
    private EmpresaVm? _seleccionada;

    partial void OnSeleccionadaChanged(EmpresaVm? value)
    {
        OnPropertyChanged(nameof(HaySeleccion));
    }

    public bool HaySeleccion => Seleccionada is not null;

    public string Recuento => Empresas.Count switch
    {
        0 => "Ninguna empresa registrada",
        1 => "1 empresa",
        _ => $"{Empresas.Count} empresas"
    };

    public Visibility VisibilidadVacio =>
        Empresas.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    public void Recargar()
    {
        Error = string.Empty;
        Aviso = string.Empty;
        Empresas.Clear();

        try
        {
            foreach (var empresa in _repositorio.Cargar())
            {
                Empresas.Add(new EmpresaVm(empresa));
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }

        Seleccionada = null;
        OnPropertyChanged(nameof(Recuento));
        OnPropertyChanged(nameof(VisibilidadVacio));
    }

    /// <summary>
    /// Guarda la lista completa. Se llama tras cada alta, edición o baja: son tres empresas,
    /// no hay nada que ganar acumulando cambios y sí que perder si la aplicación se cierra.
    /// </summary>
    public bool Guardar()
    {
        try
        {
            _repositorio.Guardar(Empresas.Select(e => e.Empresa));
            Error = string.Empty;
            return true;
        }
        catch (Exception ex)
        {
            Error = $"No se ha podido guardar {Ruta}: {ex.Message}";
            return false;
        }
    }

    public void Anadir(EmpresaBD empresa)
    {
        Empresas.Add(new EmpresaVm(empresa));
        if (Guardar())
        {
            Aviso = $"«{empresa.Nombre}» registrada.";
            OnPropertyChanged(nameof(Recuento));
            OnPropertyChanged(nameof(VisibilidadVacio));
        }
    }

    /// <summary>
    /// Sustituye la empresa seleccionada. Se reemplaza el elemento entero en lugar de mutar el
    /// modelo para que la rejilla se refresque: los campos del modelo no notifican cambios.
    /// </summary>
    public void Reemplazar(EmpresaVm vieja, EmpresaBD nueva)
    {
        var indice = Empresas.IndexOf(vieja);
        if (indice < 0) return;

        Empresas[indice] = new EmpresaVm(nueva);
        Seleccionada = Empresas[indice];

        if (Guardar()) Aviso = $"«{nueva.Nombre}» actualizada.";
    }

    public void Quitar(EmpresaVm empresa)
    {
        if (!Empresas.Remove(empresa)) return;

        Seleccionada = null;
        if (Guardar())
        {
            Aviso = $"«{empresa.Nombre}» eliminada del fichero.";
            OnPropertyChanged(nameof(Recuento));
            OnPropertyChanged(nameof(VisibilidadVacio));
        }
    }

    [ObservableProperty]
    private bool _probando;

    /// <summary>
    /// Abre y cierra contra cada empresa. En ActualizaPrecios no se podía comprobar: la base se
    /// registraba y el fallo salía más tarde, al usarla, con un mensaje que no decía por qué.
    /// </summary>
    public async Task ProbarTodasAsync()
    {
        if (Probando) return;

        Probando = true;
        Aviso = string.Empty;

        try
        {
            foreach (var vm in Empresas)
            {
                vm.Estado = "probando…";
                vm.EstadoBien = false;

                var motivo = await _repositorio.ProbarAsync(vm.Empresa);

                vm.EstadoBien = motivo.Length == 0;
                vm.Estado = vm.EstadoBien ? "conecta" : motivo;
            }

            var bien = Empresas.Count(e => e.EstadoBien);
            Aviso = bien == Empresas.Count
                ? $"Conectan las {bien}."
                : $"Conectan {bien} de {Empresas.Count}.";
        }
        finally
        {
            Probando = false;
        }
    }
}
