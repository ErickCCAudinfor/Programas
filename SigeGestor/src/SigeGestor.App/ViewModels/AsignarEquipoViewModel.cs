using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Lecturas;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.ViewModels;

/// <summary>Una lectura en la rejilla, con el equipo que se le va a poner.</summary>
public sealed partial class LecturaVm : ObservableObject
{
    public LecturaVm(LecturaPrefactura lectura)
    {
        Lectura = lectura;
        _elegido = lectura.IdEquipoMedida;
    }

    public LecturaPrefactura Lectura { get; }

    public long IdLectura => Lectura.IdLectura;
    public string NumeroFactura => Lectura.NumeroFactura;
    public string Entorno => Lectura.Entorno;
    public string FechaLectura => Lectura.FechaLectura?.ToString("dd/MM/yyyy") ?? "";
    public string FechaLecturaAnterior => Lectura.FechaLecturaAnterior?.ToString("dd/MM/yyyy") ?? "";
    public string Estado =>
        $"{(Lectura.Vigente ? "vigente" : "no vigente")}" +
        $"{(Lectura.Facturar ? " · facturar" : "")}" +
        $"{(Lectura.Facturado ? " · facturada" : "")}";

    /// <summary>El que ya tenía. Si venía con equipo, esta lectura no se toca.</summary>
    public string EquipoActual =>
        Lectura.IdEquipoMedida is null
            ? "—"
            : $"{Lectura.NumeroSerie} ({Lectura.IdEquipoMedida})";

    public bool YaTenia => Lectura.IdEquipoMedida is not null;

    /// <summary>Lo que se va a poner. Null = esta fila se deja como está.</summary>
    [ObservableProperty] private long? _elegido;

    partial void OnElegidoChanged(long? value)
    {
        OnPropertyChanged(nameof(VaACambiar));
        Cambiado?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? Cambiado;

    /// <summary>Solo se tocan las que no tenían equipo y a las que se les ha puesto uno.</summary>
    public bool VaACambiar => !YaTenia && Elegido is not null;

    /// <summary>Resultado del último guardado, para verlo en la propia fila.</summary>
    [ObservableProperty] private string _resultado = "";
    [ObservableProperty] private bool _resultadoBien;
}

/// <summary>
/// Asignar el equipo de medida a las lecturas de una prefactura.
///
/// Sale de una sugerencia del equipo: al grabar lecturas a mano no se les pone el número de
/// equipo y la prefactura sale sin contador. Antes se arreglaba con un UPDATE a mano.
///
/// ESCRIBE EN LA BASE, así que solo se ofrecen los entornos de escritura —Réplica no— igual
/// que en el resto de operaciones que tocan datos.
/// </summary>
public sealed partial class AsignarEquipoViewModel : ObservableObject
{
    private readonly RepositorioEntornos _entornos;

    public AsignarEquipoViewModel(RepositorioEntornos entornos)
    {
        _entornos = entornos;

        Entornos = _entornos.Disponibles(paraEscritura: true).ToList();
        _entornoElegido = Entornos.FirstOrDefault();
    }

    public IReadOnlyList<EntornoBD> Entornos { get; }

    [ObservableProperty] private EntornoBD? _entornoElegido;

    // ---------- Búsqueda ----------

    [ObservableProperty] private string _contrato = "";
    [ObservableProperty] private string _numeroFactura = "";
    [ObservableProperty] private bool _soloSinEquipo = true;
    [ObservableProperty] private bool _trabajando;

    partial void OnContratoChanged(string value) => OnPropertyChanged(nameof(PuedeBuscar));
    partial void OnTrabajandoChanged(bool value)
    {
        OnPropertyChanged(nameof(PuedeBuscar));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    public bool PuedeBuscar =>
        !Trabajando && long.TryParse(Contrato.Trim(), out var c) && c > 0 && EntornoElegido is not null;

    public ObservableCollection<LecturaVm> Lecturas { get; } = new();
    public ObservableCollection<EquipoMedida> Equipos { get; } = new();

    [ObservableProperty] private EquipoMedida? _equipoParaTodas;

    [ObservableProperty] private string _error = "";
    [ObservableProperty] private string _aviso = "";

    partial void OnErrorChanged(string value) => OnPropertyChanged(nameof(VisibilidadError));
    partial void OnAvisoChanged(string value) => OnPropertyChanged(nameof(VisibilidadAviso));

    public Visibility VisibilidadError =>
        string.IsNullOrEmpty(Error) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility VisibilidadAviso =>
        string.IsNullOrEmpty(Aviso) ? Visibility.Collapsed : Visibility.Visible;

    public Visibility VisibilidadVacio =>
        Lecturas.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    public string Recuento
    {
        get
        {
            if (Lecturas.Count == 0) return "";
            var cambian = Lecturas.Count(l => l.VaACambiar);
            var conEquipo = Lecturas.Count(l => l.YaTenia);

            return $"{Lecturas.Count} lecturas · {conEquipo} ya tienen equipo · " +
                   $"{cambian} marcadas para cambiar";
        }
    }

    /// <summary>Hay algo que guardar y no se está ya guardando.</summary>
    public bool PuedeGuardar => !Trabajando && Lecturas.Any(l => l.VaACambiar);

    [RelayCommand]
    private async Task Buscar()
    {
        if (!PuedeBuscar) return;

        Error = "";
        Aviso = "";
        Trabajando = true;

        try
        {
            var contrato = long.Parse(Contrato.Trim());
            var servicio = new ServicioEquipoLectura(_entornos.CadenaConexion(EntornoElegido!, 120));

            var equipos = await servicio.EquiposAsync(contrato).ConfigureAwait(true);
            Equipos.Clear();
            foreach (var e in equipos) Equipos.Add(e);

            var lecturas = await servicio.BuscarAsync(
                contrato, NumeroFactura, SoloSinEquipo).ConfigureAwait(true);

            Lecturas.Clear();
            foreach (var l in lecturas)
            {
                var vm = new LecturaVm(l);
                vm.Cambiado += (_, _) => Refrescar();
                Lecturas.Add(vm);
            }

            if (Lecturas.Count == 0)
            {
                Aviso = SoloSinEquipo
                    ? "No hay lecturas sin equipo para ese contrato. Desmarca la casilla para verlas todas."
                    : "No hay lecturas para ese contrato con esa factura.";
            }
            else if (Equipos.Count == 0)
            {
                Error = "El contrato no tiene equipos de medida, así que no hay ninguno que asignar.";
            }
        }
        catch (Exception ex)
        {
            Error = $"No se ha podido buscar: {ex.Message}";
        }
        finally
        {
            Trabajando = false;
            Refrescar();
        }
    }

    /// <summary>Pone el mismo equipo en todas las que no lo tengan. Lo normal es que sea uno.</summary>
    [RelayCommand]
    private void AplicarATodas()
    {
        if (EquipoParaTodas is null) return;

        foreach (var l in Lecturas.Where(l => !l.YaTenia))
        {
            l.Elegido = EquipoParaTodas.IdEquipoMedida;
        }

        Refrescar();
    }

    [RelayCommand]
    private void QuitarSeleccion()
    {
        foreach (var l in Lecturas.Where(l => !l.YaTenia)) l.Elegido = null;
        Refrescar();
    }

    /// <summary>
    /// Guarda. La vista pregunta ANTES de llamar aquí: escribe en Producción.
    /// Cada lectura va en su propio UPDATE, y el fallo de una no detiene las demás.
    /// </summary>
    public async Task GuardarConfirmadoAsync()
    {
        Error = "";
        Aviso = "";
        Trabajando = true;

        var hechas = 0;
        var saltadas = 0;
        var fallos = 0;

        try
        {
            var servicio = new ServicioEquipoLectura(_entornos.CadenaConexion(EntornoElegido!, 120));

            foreach (var l in Lecturas.Where(l => l.VaACambiar).ToList())
            {
                try
                {
                    var filas = await servicio.AsignarAsync(l.IdLectura, l.Elegido!.Value)
                                              .ConfigureAwait(true);

                    if (filas > 0)
                    {
                        hechas++;
                        l.Resultado = "asignado";
                        l.ResultadoBien = true;
                    }
                    else
                    {
                        // 0 filas = ya tenía equipo. No es un error: alguien se adelantó.
                        saltadas++;
                        l.Resultado = "ya tenía equipo, no se toca";
                        l.ResultadoBien = false;
                    }
                }
                catch (Exception ex)
                {
                    fallos++;
                    l.Resultado = ex.Message;
                    l.ResultadoBien = false;
                }
            }

            Aviso = $"{hechas} asignadas" +
                    (saltadas > 0 ? $" · {saltadas} ya tenían equipo" : "") +
                    (fallos > 0 ? $" · {fallos} con error" : "");

            if (fallos > 0) Error = "Alguna lectura ha fallado; el motivo está en su fila.";
        }
        catch (Exception ex)
        {
            Error = $"No se ha podido guardar: {ex.Message}";
        }
        finally
        {
            Trabajando = false;
            // Se vuelve a buscar para ver el estado de verdad, no el que creemos que quedó.
            await Buscar().ConfigureAwait(true);
        }
    }

    public string ResumenParaConfirmar
    {
        get
        {
            var n = Lecturas.Count(l => l.VaACambiar);
            var entorno = EntornoElegido?.Nombre ?? "";
            return $"Se va a asignar el equipo a {n} lectura{(n == 1 ? "" : "s")} en {entorno}.";
        }
    }

    private void Refrescar()
    {
        OnPropertyChanged(nameof(Recuento));
        OnPropertyChanged(nameof(PuedeGuardar));
        OnPropertyChanged(nameof(VisibilidadVacio));
    }
}
