using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Sugerencias;

namespace SigeGestor.App.ViewModels;

/// <summary>Una sugerencia tal como se ve en la lista.</summary>
public sealed partial class SugerenciaVm : ObservableObject
{
    public SugerenciaVm(Sugerencia sugerencia, string loginActual)
    {
        Sugerencia = sugerencia;
        EsMia = string.Equals(sugerencia.Login, loginActual, StringComparison.OrdinalIgnoreCase);
    }

    public Sugerencia Sugerencia { get; }

    /// <summary>Solo quien la escribió puede cambiarla o borrarla.</summary>
    public bool EsMia { get; }

    public string Titulo => Sugerencia.Titulo;
    public string Detalle => Sugerencia.Detalle;
    public string Autor => string.IsNullOrWhiteSpace(Sugerencia.Nombre) ? Sugerencia.Login : Sugerencia.Nombre;
    public string Iniciales => Apariencia.Iniciales(Autor);
    public string Cuando => Apariencia.TiempoRelativo(Sugerencia.Creada);

    public Visibility VisibilidadDetalle =>
        !string.IsNullOrWhiteSpace(Detalle) && Expandido ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Se marca cuando se editó después de crearla, que si no no se nota.</summary>
    public Visibility VisibilidadEditada =>
        Sugerencia.Modificada.HasValue ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadAcciones =>
        EsMia && Expandido ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Si se ve el cuerpo. Igual que en Novedades: con una docena de sugerencias largas la
    /// pantalla se hacía interminable y no se veía de un vistazo qué hay. Plegada se queda el
    /// título, el autor y el estado, que es lo que se busca al entrar.
    /// </summary>
    [ObservableProperty] private bool _expandido;

    partial void OnExpandidoChanged(bool value)
    {
        OnPropertyChanged(nameof(VisibilidadDetalle));
        OnPropertyChanged(nameof(VisibilidadRespuesta));
        OnPropertyChanged(nameof(VisibilidadAcciones));
    }

    // ---------- Respuesta de quien la revisa ----------

    public string Respuesta => Sugerencia.Respuesta;

    public Visibility VisibilidadRespuesta =>
        !string.IsNullOrWhiteSpace(Respuesta) && Expandido ? Visibility.Visible : Visibility.Collapsed;

    // ---------- Estado ----------

    public string EstadoTexto => Sugerencia.Estado switch
    {
        EstadoSugerencia.EnEstudio => "En estudio",
        EstadoSugerencia.Hecha => "Hecha",
        EstadoSugerencia.Descartada => "Descartada",
        _ => "Pendiente"
    };

    public Brush EstadoFondo => Apariencia.Pincel(Sugerencia.Estado switch
    {
        EstadoSugerencia.Hecha => "OkSoftBrush",
        EstadoSugerencia.EnEstudio => "AccentSoftBrush",
        EstadoSugerencia.Descartada => "SunkBrush",
        _ => "WarnSoftBrush"
    });

    public Brush EstadoBorde => Apariencia.Pincel(Sugerencia.Estado switch
    {
        EstadoSugerencia.Hecha => "OkLineBrush",
        EstadoSugerencia.EnEstudio => "AccentLineBrush",
        EstadoSugerencia.Descartada => "HairBrush",
        _ => "ProLineBrush"
    });

    public Brush EstadoColorTexto => Apariencia.Pincel(Sugerencia.Estado switch
    {
        EstadoSugerencia.Hecha => "OkBrush",
        EstadoSugerencia.EnEstudio => "AccentBrush",
        EstadoSugerencia.Descartada => "Ink4Brush",
        _ => "WarnBrush"
    });
}

/// <summary>
/// La pantalla de sugerencias: lo que el equipo propone para la aplicación.
///
/// Cada una es un fichero en Sugerencias\ junto al ejecutable, que vive en el .13, así que la
/// lista es la misma para las ocho personas sin montar nada. Ver RepositorioSugerencias.
///
/// Quien la escribe puede corregirla y borrarla; el resto solo la lee. No es por seguridad
/// —cualquiera puede abrir la carpeta— sino para que no se pisen sin querer.
/// </summary>
public sealed partial class SugerenciasViewModel : ObservableObject
{
    private readonly RepositorioSugerencias _repositorio = new();
    private readonly Usuario _usuario;

    public SugerenciasViewModel(Usuario usuario)
    {
        _usuario = usuario;
        Recargar();
    }

    /// <summary>Todas las que hay, sin filtrar. La pantalla pinta Vista.</summary>
    private readonly List<SugerenciaVm> _todas = new();

    public ObservableCollection<SugerenciaVm> Sugerencias { get; } = new();

    [ObservableProperty] private string _error = "";
    [ObservableProperty] private string _aviso = "";

    public Visibility VisibilidadError =>
        string.IsNullOrEmpty(Error) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility VisibilidadAviso =>
        string.IsNullOrEmpty(Aviso) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnErrorChanged(string value) => OnPropertyChanged(nameof(VisibilidadError));
    partial void OnAvisoChanged(string value) => OnPropertyChanged(nameof(VisibilidadAviso));

    public Visibility VisibilidadVacio =>
        Sugerencias.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    public string Recuento => _todas.Count switch
    {
        0 => "ninguna todavía",
        1 => "1 sugerencia",
        _ => $"{_todas.Count} sugerencias"
    };

    // ============================================================
    // EL FORMULARIO
    //
    // El mismo sirve para crear y para editar: con _editando a null es una nueva, y con algo
    // dentro se está corrigiendo esa. Así no hay dos cuadros que mantener iguales.
    // ============================================================

    private Sugerencia? _editando;

    [ObservableProperty] private string _titulo = "";
    [ObservableProperty] private string _detalle = "";
    [ObservableProperty] private bool _escribiendo;

    public string TextoBotonGuardar => _editando is null ? "Enviar sugerencia" : "Guardar cambios";

    public string TituloFormulario => _editando is null ? "NUEVA SUGERENCIA" : "EDITANDO LA SUGERENCIA";

    public bool PuedeGuardar => Titulo.Trim().Length >= 3;

    partial void OnTituloChanged(string value) => OnPropertyChanged(nameof(PuedeGuardar));

    [RelayCommand]
    private void Escribir()
    {
        _editando = null;
        Titulo = "";
        Detalle = "";
        Escribiendo = true;
        Avisar();
    }

    [RelayCommand]
    private void Editar(SugerenciaVm? fila)
    {
        if (fila is null || !fila.EsMia) return;

        _editando = fila.Sugerencia;
        Titulo = fila.Sugerencia.Titulo;
        Detalle = fila.Sugerencia.Detalle;
        Escribiendo = true;
        Avisar();
    }

    [RelayCommand]
    private void Cancelar()
    {
        _editando = null;
        Escribiendo = false;
        Titulo = "";
        Detalle = "";
        Avisar();
    }

    [RelayCommand]
    private void Guardar()
    {
        if (!PuedeGuardar) return;

        Error = "";
        Aviso = "";

        try
        {
            if (_editando is null)
            {
                _repositorio.Crear(Titulo, Detalle, _usuario.Login, _usuario.Nombre);
                Aviso = "Gracias, queda apuntada.";
            }
            else
            {
                _editando.Titulo = Titulo.Trim();
                _editando.Detalle = Detalle.Trim();
                _editando.Modificada = DateTime.Now;
                _repositorio.Guardar(_editando);
                Aviso = "Sugerencia actualizada.";
            }

            Cancelar();
            Recargar();
        }
        catch (Exception ex)
        {
            Error = $"No se ha podido guardar: {ex.Message}";
        }
    }

    /// <summary>
    /// Borra. La vista pregunta ANTES de llamar aquí: lo que se borra es lo que alguien
    /// escribió y no hay papelera de la que sacarlo.
    /// </summary>
    public void BorrarConfirmado(SugerenciaVm fila)
    {
        if (!fila.EsMia) return;

        Error = "";
        Aviso = "";

        try
        {
            _repositorio.Borrar(fila.Sugerencia.Id);
            Aviso = "Sugerencia borrada.";
            Recargar();
        }
        catch (Exception ex)
        {
            Error = $"No se ha podido borrar: {ex.Message}";
        }
    }

    /// <summary>Relee la carpeta. También trae lo que hayan escrito los demás mientras tanto.</summary>
    [RelayCommand]
    public void Recargar()
    {
        _todas.Clear();

        try
        {
            foreach (var s in _repositorio.Listar())
            {
                _todas.Add(new SugerenciaVm(s, _usuario.Login));
            }
        }
        catch (Exception ex)
        {
            Error = $"No se han podido leer las sugerencias: {ex.Message}";
        }

        AplicarFiltro();
    }

    // ============================================================
    // FILTRO
    //
    // Por estado y por texto. El texto busca en el título, en el detalle y en el autor, que
    // es por donde se busca de verdad: «lo que pidió fulano» o «la del maxímetro».
    // ============================================================

    public IReadOnlyList<string> Estados { get; } =
        new[] { "Todas", "Pendiente", "En estudio", "Hecha", "Descartada" };

    [ObservableProperty] private string _estadoElegido = "Todas";
    [ObservableProperty] private string _busqueda = "";

    partial void OnEstadoElegidoChanged(string value) => AplicarFiltro();
    partial void OnBusquedaChanged(string value) => AplicarFiltro();

    [RelayCommand]
    private void LimpiarFiltro()
    {
        EstadoElegido = "Todas";
        Busqueda = "";
    }

    public Visibility VisibilidadFiltrado =>
        _todas.Count != Sugerencias.Count ? Visibility.Visible : Visibility.Collapsed;

    public string TextoFiltrado => $"{Sugerencias.Count} de {_todas.Count}";

    private void AplicarFiltro()
    {
        var texto = (Busqueda ?? "").Trim();

        IEnumerable<SugerenciaVm> vista = _todas;

        if (!string.IsNullOrEmpty(EstadoElegido) && EstadoElegido != "Todas")
        {
            vista = vista.Where(s => s.EstadoTexto == EstadoElegido);
        }

        if (texto.Length > 0)
        {
            vista = vista.Where(s =>
                Contiene(s.Titulo, texto) || Contiene(s.Detalle, texto) ||
                Contiene(s.Autor, texto) || Contiene(s.Respuesta, texto));
        }

        Sugerencias.Clear();
        foreach (var s in vista) Sugerencias.Add(s);

        OnPropertyChanged(nameof(Recuento));
        OnPropertyChanged(nameof(VisibilidadVacio));
        OnPropertyChanged(nameof(VisibilidadFiltrado));
        OnPropertyChanged(nameof(TextoFiltrado));
    }

    /// <summary>Sin distinguir mayúsculas ni tildes: nadie las teclea al buscar.</summary>
    private static bool Contiene(string donde, string que) =>
        (donde ?? "").IndexOf(que, StringComparison.OrdinalIgnoreCase) >= 0;

    // ============================================================
    // PLEGAR Y DESPLEGAR TODAS
    // ============================================================

    [RelayCommand]
    private void DesplegarTodas()
    {
        foreach (var s in Sugerencias) s.Expandido = true;
    }

    [RelayCommand]
    private void PlegarTodas()
    {
        foreach (var s in Sugerencias) s.Expandido = false;
    }

    private void Avisar()
    {
        OnPropertyChanged(nameof(TextoBotonGuardar));
        OnPropertyChanged(nameof(TituloFormulario));
        OnPropertyChanged(nameof(PuedeGuardar));
    }
}
