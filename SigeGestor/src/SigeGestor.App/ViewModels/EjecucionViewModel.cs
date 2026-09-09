using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

/// <summary>
/// Una carpeta con lo que la ejecución ha dejado dentro.
///
/// La ruta se muestra COMPLETA y en varias líneas si hace falta. Antes viajaba dentro del
/// mensaje de resultado, que es de una sola línea y se pinta en tres sitios: se cortaba en los
/// tres —«…\ConsultasBO\Con…»— y encima empujaba fuera el recuento, que es lo que se lee de un
/// vistazo. Aquí, además, se puede abrir.
/// </summary>
public sealed class SalidaVm
{
    public SalidaVm(string carpeta, IReadOnlyList<string> ficheros)
    {
        Carpeta = carpeta;

        // Con cuatro nombres ya se sabe qué hay; más allá, lo que importa es la carpeta.
        Ficheros = ficheros.Count switch
        {
            0 => string.Empty,
            <= 4 => string.Join(", ", ficheros),
            _ => string.Join(", ", ficheros.Take(4)) + $" y {ficheros.Count - 4:N0} más"
        };

        // Con un solo fichero se abre el explorador con él ya seleccionado; con varios, o con
        // ninguno, se abre la carpeta y punto.
        Seleccionar = ficheros.Count == 1 ? Path.Combine(carpeta, ficheros[0]) : string.Empty;
    }

    /// <summary>Ruta completa de la carpeta.</summary>
    public string Carpeta { get; }

    /// <summary>Los nombres de dentro, o cadena vacía si solo se informó de la carpeta.</summary>
    public string Ficheros { get; }

    /// <summary>Fichero a dejar seleccionado al abrir, si hay uno solo.</summary>
    public string Seleccionar { get; }

    public Visibility VisibilidadFicheros =>
        string.IsNullOrEmpty(Ficheros) ? Visibility.Collapsed : Visibility.Visible;
}

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

    /// <summary>
    /// Todo lo que no salió con datos: fallos y sin-datos, con su motivo.
    ///
    /// Se separa de Fallidas porque no son lo mismo a la hora de actuar: un fallo se puede
    /// reintentar, un «sin PDF en la base» no. Pero las dos cosas hay que poder copiarlas.
    /// </summary>
    public IReadOnlyList<LineaProgreso> Incidencias { get; private set; } = Array.Empty<LineaProgreso>();

    public Visibility VisibilidadIncidencias =>
        Incidencias.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Título del bloque: distingue si hay fallos, sin-datos, o de las dos cosas.</summary>
    [ObservableProperty] private string _tituloIncidencias = "";

    /// <summary>Las primeras líneas, para verlas sin abrir nada.</summary>
    [ObservableProperty] private string _detalleIncidencias = "";

    /// <summary>Aviso de cuántas no se muestran.</summary>
    [ObservableProperty] private string _restoIncidencias = "";

    public Visibility VisibilidadRestoIncidencias =>
        string.IsNullOrEmpty(RestoIncidencias) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnRestoIncidenciasChanged(string value) =>
        OnPropertyChanged(nameof(VisibilidadRestoIncidencias));

    // ============================================================
    // LO QUE SE HA GENERADO
    // ============================================================

    /// <summary>
    /// Ficheros y carpetas que ha dejado la ejecución, agrupados por carpeta.
    ///
    /// Media docena de operaciones pegaban la ruta dentro del mensaje —«2 incidencias en
    /// C:\Users\ErickCC\Desktop\ConsultasBO\Contactos»— y ese mensaje se pinta en tres sitios
    /// de una sola línea: la cabecera, el registro y el resumen. En los tres se cortaba, así
    /// que la ruta no se leía en ninguno. Aquí sale entera y con un botón para abrirla.
    /// </summary>
    public IReadOnlyList<SalidaVm> Salidas { get; private set; } = Array.Empty<SalidaVm>();

    public Visibility VisibilidadSalidas =>
        Salidas.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Confirmación de que se ha copiado. Se borra al volver a ejecutar.</summary>
    [ObservableProperty] private string _avisoCopiado = "";

    public Visibility VisibilidadAvisoCopiado =>
        string.IsNullOrEmpty(AvisoCopiado) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnAvisoCopiadoChanged(string value) =>
        OnPropertyChanged(nameof(VisibilidadAvisoCopiado));

    /// <summary>
    /// Solo las entradas, una por línea: es lo que se pega de vuelta en el cuadro de la
    /// operación para relanzarlas.
    /// </summary>
    public string ParaPegar =>
        string.Join(Environment.NewLine, Incidencias.Select(i => i.Entrada));

    /// <summary>
    /// Entrada, estado y motivo separados por tabulador: al pegarlo en Excel cae en tres
    /// columnas. Es el formato útil para mandárselo a alguien o guardarlo.
    /// </summary>
    public string ParaExcel
    {
        get
        {
            const string tab = "\t";

            return string.Join(
                Environment.NewLine,
                Incidencias.Select(i => i.Entrada + tab + EtiquetaEstado(i.Estado) + tab + i.Mensaje));
        }
    }

    private static string EtiquetaEstado(EstadoEntrada estado) => estado switch
    {
        EstadoEntrada.Fallo => "error",
        EstadoEntrada.SinDatos => "sin datos",
        _ => estado.ToString()
    };

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

        RellenarIncidencias(r);
        RellenarSalidas(r);

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

    /// <summary>
    /// Monta el bloque de incidencias. Se muestran las primeras con su motivo completo —que es
    /// lo que en el registro no cabía y salía cortado— y el resto se cuenta; para verlas todas
    /// está el botón de copiar.
    /// </summary>
    private void RellenarIncidencias(ResultadoOperacion r)
    {
        Incidencias = r.Incidencias;
        AvisoCopiado = string.Empty;

        OnPropertyChanged(nameof(VisibilidadIncidencias));
        OnPropertyChanged(nameof(ParaPegar));
        OnPropertyChanged(nameof(ParaExcel));

        if (Incidencias.Count == 0)
        {
            TituloIncidencias = string.Empty;
            DetalleIncidencias = string.Empty;
            RestoIncidencias = string.Empty;
            return;
        }

        var conError = Incidencias.Count(i => i.Estado == EstadoEntrada.Fallo);
        var sinDatos = Incidencias.Count - conError;

        TituloIncidencias = (conError, sinDatos) switch
        {
            (0, 1) => "1 entrada sin resultado",
            (0, _) => $"{sinDatos:N0} entradas sin resultado",
            (1, 0) => "1 entrada con error",
            (_, 0) => $"{conError:N0} entradas con error",
            _ => $"{conError:N0} con error y {sinDatos:N0} sin resultado"
        };

        // Doce caben en pantalla sin empujar el resto. El resto se cuenta y se copia.
        const int aMostrar = 12;

        DetalleIncidencias = string.Join(
            Environment.NewLine,
            Incidencias.Take(aMostrar).Select(i => $"{i.Entrada}   {i.Mensaje}"));

        var restan = Incidencias.Count - aMostrar;
        RestoIncidencias = restan > 0
            ? $"…y {restan:N0} más. Cópialas para verlas todas."
            : string.Empty;
    }

    /// <summary>
    /// Agrupa lo generado por carpeta.
    ///
    /// Se agrupa porque una operación de 200 facturas informa de una carpeta y no de 200
    /// ficheros, pero otras informan de ficheros con nombre —el Excel de incidencias, la
    /// plantilla— y de esos interesa ver cuál es. Cada ruta se mira en el disco para saber si
    /// es carpeta o fichero, que es lo único que lo distingue sin ambigüedad.
    /// </summary>
    private void RellenarSalidas(ResultadoOperacion r)
    {
        if (r.Salidas.Count == 0)
        {
            Salidas = Array.Empty<SalidaVm>();
            OnPropertyChanged(nameof(VisibilidadSalidas));
            return;
        }

        var porCarpeta = new List<(string Carpeta, List<string> Ficheros)>();

        foreach (var ruta in r.Salidas)
        {
            string carpeta;
            string? fichero = null;

            if (Directory.Exists(ruta))
            {
                carpeta = ruta;
            }
            else
            {
                // Puede no existir todavía —o ya no— y aun así la carpeta sirve para abrirla.
                carpeta = Path.GetDirectoryName(ruta) ?? ruta;
                fichero = Path.GetFileName(ruta);
            }

            var grupo = porCarpeta.FirstOrDefault(
                g => string.Equals(g.Carpeta, carpeta, StringComparison.OrdinalIgnoreCase));

            if (grupo.Carpeta is null)
            {
                grupo = (carpeta, new List<string>());
                porCarpeta.Add(grupo);
            }

            if (fichero is not null && !grupo.Ficheros.Contains(fichero, StringComparer.OrdinalIgnoreCase))
            {
                grupo.Ficheros.Add(fichero);
            }
        }

        Salidas = porCarpeta.Select(g => new SalidaVm(g.Carpeta, g.Ficheros)).ToList();
        OnPropertyChanged(nameof(VisibilidadSalidas));
    }

    /// <summary>Lo copia la vista al portapapeles; aquí solo se confirma.</summary>
    public void ConfirmarCopiado(int cuantas, string queEs)
    {
        AvisoCopiado = cuantas == 1
            ? $"1 entrada copiada ({queEs})."
            : $"{cuantas:N0} entradas copiadas ({queEs}).";
    }
}
