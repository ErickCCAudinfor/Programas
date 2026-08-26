using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Servicios;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Panel de inicio: las cuatro cifras del turno y los accesos rápidos de la persona que ha
/// entrado.
///
/// Todo el formato de cifras se hace aquí. El Core devuelve números crudos porque no sabe si
/// se van a pintar como «1,86 M» o como «1.866.240».
/// </summary>
public sealed partial class InicioViewModel : ObservableObject
{
    private readonly IRepositorioEjecuciones _repositorio;
    private readonly Usuario _usuario;

    public InicioViewModel(IRepositorioEjecuciones repositorio, Usuario usuario)
    {
        _repositorio = repositorio;
        _usuario = usuario;
    }

    public string Saludo => $"{FranjaDelDia()}, {PrimerNombre(_usuario.Nombre)}";

    public string Subtitulo =>
        $"{DateTime.Now:dddd d 'de' MMMM} · esto es lo que lleva el equipo hoy.";

    private static string FranjaDelDia() => DateTime.Now.Hour switch
    {
        < 13 => "Buenos días",
        < 21 => "Buenas tardes",
        _ => "Buenas noches"
    };

    private static string PrimerNombre(string nombre)
    {
        var limpio = (nombre ?? "").Trim();
        if (limpio.Length == 0) return "de nuevo";

        var espacio = limpio.IndexOf(' ');
        return espacio > 0 ? limpio[..espacio] : limpio;
    }

    public ObservableCollection<MetricaVm> Metricas { get; } = new();

    public ObservableCollection<AccesoVm> Accesos { get; } = new();

    public ObservableCollection<EjecucionVm> Actividad { get; } = new();

    /// <summary>Días de log que se conservan. Se muestra en el estado vacío.</summary>
    public int DiasRetencion { get; init; } = 3;

    public Visibility VisibilidadActividad =>
        Actividad.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadSinActividad =>
        Actividad.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadAccesos =>
        Accesos.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadSinAccesos =>
        Accesos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    public string TextoSinActividad =>
        $"Aún no hay nada registrado. El log guarda {DiasRetencion} días y se va llenando " +
        "a medida que el equipo lanza operaciones.";

    [ObservableProperty]
    private bool _cargando = true;

    [ObservableProperty]
    private string _mensajeError = string.Empty;

    public bool HayError => !string.IsNullOrEmpty(MensajeError);

    public Visibility VisibilidadCargando => Cargando ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadContenido =>
        !Cargando && !HayError ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadError => HayError ? Visibility.Visible : Visibility.Collapsed;

    partial void OnCargandoChanged(bool value)
    {
        OnPropertyChanged(nameof(VisibilidadCargando));
        OnPropertyChanged(nameof(VisibilidadContenido));
    }

    partial void OnMensajeErrorChanged(string value)
    {
        OnPropertyChanged(nameof(HayError));
        OnPropertyChanged(nameof(VisibilidadContenido));
        OnPropertyChanged(nameof(VisibilidadError));
    }

    public async Task CargarAsync(CancellationToken ct = default)
    {
        Cargando = true;
        MensajeError = string.Empty;

        try
        {
            // En paralelo: son consultas independientes y así la espera es la de la más
            // lenta, no la suma.
            var tareaMetricas = _repositorio.ObtenerMetricasAsync(ct);
            var tareaAccesos = _repositorio.ObtenerFrecuentesAsync(_usuario.Login, 3, ct);
            var tareaActividad = _repositorio.ObtenerRecientesAsync(6, ct);

            await Task.WhenAll(tareaMetricas, tareaAccesos, tareaActividad).ConfigureAwait(true);

            Metricas.Clear();
            foreach (var metrica in ConstruirMetricas(tareaMetricas.Result))
            {
                Metricas.Add(metrica);
            }

            Accesos.Clear();
            var indice = 0;
            foreach (var frecuente in tareaAccesos.Result)
            {
                Accesos.Add(new AccesoVm
                {
                    Titulo = frecuente.Operacion,
                    Grupo = frecuente.Grupo,
                    Entorno = frecuente.Entorno,
                    Detalle = $"{Apariencia.TiempoRelativo(frecuente.UltimaVez)} · {frecuente.UltimoResultado}",
                    Destacada = indice == 0
                });
                indice++;
            }

            Actividad.Clear();
            foreach (var ejecucion in tareaActividad.Result)
            {
                Actividad.Add(new EjecucionVm(ejecucion));
            }

            OnPropertyChanged(nameof(VisibilidadActividad));
            OnPropertyChanged(nameof(VisibilidadSinActividad));
            OnPropertyChanged(nameof(VisibilidadAccesos));
            OnPropertyChanged(nameof(VisibilidadSinAccesos));
        }
        catch (OperationCanceledException)
        {
            // La ventana se ha cerrado mientras cargaba: no hay nada que informar.
        }
        catch (Exception ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            Cargando = false;
        }
    }

    private static IEnumerable<MetricaVm> ConstruirMetricas(MetricasInicio m)
    {
        yield return new MetricaVm
        {
            Titulo = "Ejecuciones hoy",
            ClaveIcono = "IcoEjecutar",
            Valor = m.Ejecuciones.Valor.ToString("N0"),
            Delta = FormatoDeltaAbsoluto(m.Ejecuciones.Delta),
            Tendencia = TendenciaDe(m.Ejecuciones.Delta),
            Serie = m.Ejecuciones.Serie
        };

        yield return new MetricaVm
        {
            Titulo = "Filas procesadas",
            ClaveIcono = "IcoEjecuciones",
            Valor = (m.Filas.Valor / 1_000_000d).ToString("N2"),
            Unidad = "M",
            Delta = FormatoDeltaPorcentual(m.Filas.DeltaPorcentual),
            Tendencia = TendenciaDe(m.Filas.Delta),
            Serie = m.Filas.Serie
        };

        var errores = (int)m.ConError.Valor;
        yield return new MetricaVm
        {
            Titulo = "Con error",
            ClaveIcono = "IcoAviso",
            Valor = errores.ToString("N0"),
            Delta = FormatoDeltaAbsoluto(m.ConError.Delta),
            Tendencia = TendenciaDe(m.ConError.Delta),
            MejorEsMenos = true,
            Alarmante = errores > 0,
            Serie = m.ConError.Serie
        };

        // La duración media se mueve poco. Por debajo del umbral se presenta como estable y
        // sin flecha: una flecha junto a la palabra «estable» se contradice sola.
        var deltaDuracion = m.DuracionMedia.Delta;
        var duracionEstable = Math.Abs(deltaDuracion) < UmbralSegundosEstable;

        yield return new MetricaVm
        {
            Titulo = "Duración media",
            ClaveIcono = "IcoReloj",
            Valor = Apariencia.Duracion(TimeSpan.FromSeconds(m.DuracionMedia.Valor)),
            Delta = duracionEstable ? "estable" : $"{Math.Abs(deltaDuracion):N0} s",
            Tendencia = duracionEstable ? Tendencia.Plana : TendenciaDe(deltaDuracion),
            MejorEsMenos = true,
            Serie = m.DuracionMedia.Serie
        };
    }

    /// <summary>Cambios de duración por debajo de esto no se consideran movimiento.</summary>
    private const double UmbralSegundosEstable = 5;

    private static Tendencia TendenciaDe(double delta) => delta switch
    {
        > 0 => Tendencia.Sube,
        < 0 => Tendencia.Baja,
        _ => Tendencia.Plana
    };

    private static string FormatoDeltaAbsoluto(double delta) =>
        delta == 0 ? "sin cambios" : Math.Abs(delta).ToString("N0");

    private static string FormatoDeltaPorcentual(double? porcentaje) =>
        porcentaje is null ? "sin comparación" : $"{Math.Abs(porcentaje.Value):N0} %";
}
