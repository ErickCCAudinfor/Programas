using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;
using SigeGestor.Core.Registro;

namespace SigeGestor.App.ViewModels;

/// <summary>Una línea del registro en vivo, ya formateada.</summary>
public sealed class LineaVm
{
    public LineaVm(LineaProgreso linea)
    {
        Hora = linea.Momento.ToString("HH:mm:ss");
        Entrada = linea.Entrada;
        Detalle = string.IsNullOrEmpty(linea.Mensaje)
            ? linea.Registros.ToString("N0")
            : linea.Mensaje;
        Duracion = $"{linea.Duracion.TotalSeconds:0.0} s";
        Estado = linea.Estado;
    }

    public string Hora { get; }
    public string Entrada { get; }
    public string Detalle { get; }
    public string Duracion { get; }
    public EstadoEntrada Estado { get; }

    public Brush Color => Apariencia.Pincel(Estado switch
    {
        EstadoEntrada.ConDatos => "OkBrush",
        EstadoEntrada.SinDatos => "Ink4Brush",
        EstadoEntrada.Fallo => "BadBrush",
        _ => "Ink3Brush"
    });

    public Brush Fondo => Apariencia.Pincel(
        Estado == EstadoEntrada.Fallo ? "BadSoftBrush" : "SunkBrush");

    public string Simbolo => Estado switch
    {
        EstadoEntrada.ConDatos => "✓",
        EstadoEntrada.SinDatos => "–",
        EstadoEntrada.Fallo => "✕",
        _ => "…"
    };
}

/// <summary>
/// Conduce la ejecución de una operación y expone su progreso.
///
/// El progreso llega desde un hilo de fondo, así que cada aviso se reencola en el hilo de
/// interfaz. Y se limita el ritmo de refresco: con entradas rápidas llegarían cientos de
/// avisos por segundo y la interfaz se pasaría el rato redibujando en vez de dejar avanzar
/// la operación.
/// </summary>
public sealed partial class EjecucionViewModel : ObservableObject
{
    /// <summary>Mínimo entre repintados. 12 por segundo es de sobra para el ojo.</summary>
    private static readonly TimeSpan Refresco = TimeSpan.FromMilliseconds(80);

    private readonly RegistroEjecuciones _registro;
    private readonly Usuario _usuario;
    private CancellationTokenSource? _cts;
    private DateTime _ultimoPintado = DateTime.MinValue;

    public EjecucionViewModel(RegistroEjecuciones registro, Usuario usuario)
    {
        _registro = registro;
        _usuario = usuario;
    }

    public ObservableCollection<LineaVm> Registro { get; } = new();

    // ---------- Estado ----------

    [ObservableProperty] private bool _enMarcha;
    [ObservableProperty] private bool _terminada;
    [ObservableProperty] private string _titulo = "";
    [ObservableProperty] private string _subtitulo = "";
    [ObservableProperty] private string _entradaActual = "";
    [ObservableProperty] private int _procesados;
    [ObservableProperty] private int _total;
    [ObservableProperty] private double _fraccion;
    [ObservableProperty] private string _transcurrido = "0:00";
    [ObservableProperty] private string _restante = "";
    [ObservableProperty] private string _registrosTexto = "0";
    [ObservableProperty] private IReadOnlyList<EstadoEntrada> _estados = Array.Empty<EstadoEntrada>();

    public string Contador => $"{Procesados:N0} / {Total:N0}";

    partial void OnProcesadosChanged(int value) => OnPropertyChanged(nameof(Contador));
    partial void OnTotalChanged(int value) => OnPropertyChanged(nameof(Contador));

    public Visibility VisibilidadEnMarcha => EnMarcha ? Visibility.Visible : Visibility.Collapsed;
    public Visibility VisibilidadResultado => Terminada ? Visibility.Visible : Visibility.Collapsed;

    partial void OnEnMarchaChanged(bool value) => OnPropertyChanged(nameof(VisibilidadEnMarcha));
    partial void OnTerminadaChanged(bool value) => OnPropertyChanged(nameof(VisibilidadResultado));

    // ---------- Resumen ----------

    [ObservableProperty] private string _resumenTitulo = "";
    [ObservableProperty] private string _resumenDetalle = "";
    [ObservableProperty] private int _resConDatos;
    [ObservableProperty] private int _resSinDatos;
    [ObservableProperty] private int _resErrores;
    [ObservableProperty] private string _resRegistros = "0";
    [ObservableProperty] private string _textoFallidas = "";
    [ObservableProperty] private string _explicacionSinCambios = "";

    public Visibility VisibilidadSinCambios =>
        string.IsNullOrEmpty(ExplicacionSinCambios) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnExplicacionSinCambiosChanged(string value) =>
        OnPropertyChanged(nameof(VisibilidadSinCambios));

    public Visibility VisibilidadFallidas =>
        ResErrores > 0 ? Visibility.Visible : Visibility.Collapsed;

    partial void OnResErroresChanged(int value)
    {
        OnPropertyChanged(nameof(VisibilidadFallidas));
        OnPropertyChanged(nameof(ColorResumen));
    }

    public Brush ColorResumen => Apariencia.Pincel(ResErrores > 0 ? "BadBrush" : "OkBrush");

    /// <summary>Las que fallaron, para poder relanzar solo eso.</summary>
    public IReadOnlyList<string> Fallidas { get; private set; } = Array.Empty<string>();

    // ---------- Ejecución ----------

    public async Task EjecutarAsync(ContextoEjecucion ctx)
    {
        var ejecutable = ctx.Definicion.Ejecutable;
        if (ejecutable is null) return;

        _cts = new CancellationTokenSource();
        ctx.Cancelacion = _cts.Token;
        ctx.Usuario = _usuario;

        Registro.Clear();
        Titulo = ctx.Definicion.Nombre;
        Subtitulo = $"{ctx.Entradas.Count:N0} {AnalizadorEntradas.Unidad(ctx.TipoLista, ctx.Entradas.Count)} · {ctx.Entorno.Nombre}";
        Total = ctx.Entradas.Count;
        Procesados = 0;
        Fraccion = 0;
        Estados = Array.Empty<EstadoEntrada>();
        EnMarcha = true;
        Terminada = false;

        var arranque = DateTime.Now;
        var sincronizador = SynchronizationContext.Current;

        void Informar(ProgresoOperacion p)
        {
            // El aviso llega del hilo de fondo. Se filtra por ritmo antes de saltar al hilo
            // de interfaz para no encolar cientos de mensajes que nadie va a ver.
            var ahora = DateTime.Now;
            var esFinal = p.Procesados >= p.Total;
            if (!esFinal && ahora - _ultimoPintado < Refresco) return;
            _ultimoPintado = ahora;

            sincronizador?.Post(_ => Pintar(p), null);
        }

        ResultadoOperacion resultado;
        try
        {
            resultado = await Task.Run(() => ejecutable.EjecutarAsync(ctx, Informar), _cts.Token)
                                  .ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            resultado = new ResultadoOperacion { Total = Total, Cancelada = true };
        }
        catch (Exception ex)
        {
            resultado = new ResultadoOperacion { Total = Total, Errores = 1, Mensaje = ex.Message };
        }

        EnMarcha = false;
        MostrarResultado(ctx, resultado);

        await RegistrarAsync(ctx, resultado, arranque).ConfigureAwait(true);
    }

    private void Pintar(ProgresoOperacion p)
    {
        Procesados = p.Procesados;
        Total = p.Total;
        Fraccion = p.Fraccion;
        EntradaActual = p.EntradaActual;
        Estados = p.Estados;
        RegistrosTexto = p.Registros.ToString("N0");
        Transcurrido = Apariencia.Duracion(p.Transcurrido);
        Restante = p.Restante.HasValue ? $"~{Apariencia.Duracion(p.Restante.Value)}" : "";

        // Se reconstruye la lista en vez de ir añadiendo: el progreso ya trae la ventana de
        // las últimas líneas y así no hay que llevar la cuenta de por dónde iba.
        Registro.Clear();
        foreach (var linea in p.Ultimas.AsEnumerable().Reverse())
        {
            Registro.Add(new LineaVm(linea));
        }
    }

    private void MostrarResultado(ContextoEjecucion ctx, ResultadoOperacion r)
    {
        ResConDatos = r.ConDatos;
        ResSinDatos = r.SinDatos;
        ResErrores = r.Errores;
        ResRegistros = r.Registros.ToString("N0");
        Fallidas = r.Fallidas;

        ResumenTitulo = r.Estado switch
        {
            EstadoEjecucion.Cancelada => "Cancelada",
            EstadoEjecucion.ConErrores => "Terminada con errores",
            // Que no haya cambiado nada se dice explícitamente. Antes ponía «Terminada» con
            // un 0 al lado y había que adivinar si eso era bueno o malo.
            EstadoEjecucion.SinCambios => "Terminada sin cambios",
            _ => "Terminada"
        };

        ResumenDetalle = $"{ctx.Entorno.Nombre} · {Apariencia.Duracion(r.Duracion)} · " +
                         $"{r.Total:N0} {AnalizadorEntradas.Unidad(ctx.TipoLista, r.Total)} · {r.Desglose}";

        if (!string.IsNullOrEmpty(r.Mensaje)) ResumenDetalle += $" · {r.Mensaje}";

        // Cuando no se ha tocado nada se explica el motivo, que es lo que el registro decía
        // entrada por entrada y se perdía al salir de aquí.
        ExplicacionSinCambios = r.Estado == EstadoEjecucion.SinCambios
            ? "Ninguna de las entradas procedía: mira el registro de arriba para ver el motivo de cada una. " +
              "No se ha modificado nada, así que no hay nada que deshacer."
            : string.Empty;

        if (r.Fallidas.Count > 0)
        {
            var muestra = string.Join(", ", r.Fallidas.Take(3));
            var resto = r.Fallidas.Count - Math.Min(3, r.Fallidas.Count);
            TextoFallidas = resto > 0 ? $"{muestra} y {resto} más" : muestra;
        }

        Terminada = true;
    }

    /// <summary>
    /// Deja la ejecución en el log. Es lo que hace que aparezca en la actividad del equipo
    /// del inicio; hasta ahora eso solo se alimentaba de los ficheros de muestra.
    /// </summary>
    private async Task RegistrarAsync(ContextoEjecucion ctx, ResultadoOperacion r, DateTime arranque)
    {
        var ejecucion = new Ejecucion
        {
            Id = DateTime.Now.Ticks,
            Momento = arranque,
            Operacion = ctx.Definicion.Nombre,
            Grupo = ctx.Definicion.Seccion.ToString(),
            // El desglose va al log: es lo que hace que en la actividad del equipo se entienda
            // un resultado 0 sin tener que volver a abrir la operación.
            Detalle = $"{r.Total:N0} {AnalizadorEntradas.Unidad(ctx.TipoLista, r.Total)} · {r.Desglose}",
            NombreUsuario = _usuario.Nombre,
            Login = _usuario.Login,
            Entorno = ctx.Entorno.Clave,
            Estado = r.Estado,
            Registros = r.Registros,
            Errores = r.Errores,
            Duracion = r.Duracion
        };

        await _registro.RegistrarAsync(ejecucion).ConfigureAwait(true);
    }

    public void Cancelar() => _cts?.Cancel();
}
