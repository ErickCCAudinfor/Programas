using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SigeGestor.App.Servicios;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Lecturas;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.ViewModels;

/// <summary>Una lectura en la rejilla, con el equipo que se le va a poner.</summary>
public sealed partial class LecturaVm : ObservableObject
{
    public LecturaVm(LecturaPrefactura lectura)
    {
        Lectura = lectura;
    }

    public LecturaPrefactura Lectura { get; }

    public long IdLectura => Lectura.IdLectura;

    /// <summary>
    /// De qué contrato es. Se busca por una LISTA de contratos, así que sin esto no hay forma
    /// de saber a cuál pertenece cada fila.
    /// </summary>
    public long CodigoContrato => Lectura.CodigoContrato;
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

    /// <summary>
    /// El IdEquipoMedida que se teclea para esta lectura. Es texto y no un número porque se
    /// escribe a mano y hay que poder dejarlo a medias o en blanco sin que la celda proteste.
    /// </summary>
    [ObservableProperty] private string _idEquipo = "";

    partial void OnIdEquipoChanged(string value)
    {
        OnPropertyChanged(nameof(VaACambiar));
        OnPropertyChanged(nameof(Problema));
        OnPropertyChanged(nameof(TextoResultado));
        Cambiado?.Invoke(this, EventArgs.Empty);
    }

    partial void OnResultadoChanged(string value) => OnPropertyChanged(nameof(TextoResultado));

    /// <summary>El id tecleado, si es un número válido.</summary>
    public long? Elegido =>
        long.TryParse((IdEquipo ?? "").Trim(), out var n) && n > 0 ? n : null;

    /// <summary>Qué le pasa a lo tecleado. Vacío si está bien.</summary>
    public string Problema =>
        (IdEquipo ?? "").Trim().Length == 0 || Elegido is not null
            ? ""
            : "no es un número";

    public event EventHandler? Cambiado;

    /// <summary>Solo se tocan las que no tenían equipo y a las que se les ha puesto uno.</summary>
    public bool VaACambiar => !YaTenia && Elegido is not null;

    /// <summary>Resultado del último guardado, para verlo en la propia fila.</summary>
    [ObservableProperty] private string _resultado = "";
    [ObservableProperty] private bool _resultadoBien;

    /// <summary>
    /// Lo que se lee en la columna RESULTADO.
    ///
    /// Si lo tecleado está mal, lo dice AQUÍ. Antes el motivo solo estaba en el tooltip de la
    /// celda: el botón de guardar se apagaba y había que pasar el ratón por encima de la fila
    /// para enterarse de por qué. La columna está vacía hasta que se guarda, así que cabe.
    /// </summary>
    public string TextoResultado => Problema.Length > 0 ? $"⚠ {Problema}" : Resultado;
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

    public AsignarEquipoViewModel(RepositorioEntornos entornos,
                                  AnotadorDeCambios? anotador = null)
    {
        _entornos = entornos;
        _anotador = anotador;

        Entornos = _entornos.Disponibles(paraEscritura: true).ToList();
        _entornoElegido = Entornos.FirstOrDefault();
    }

    /// <summary>
    /// Quien deja el cambio en el log de ejecuciones. Opcional: sin el, se guarda igual pero
    /// no se registra. Ver AnotadorDeCambios.
    /// </summary>
    private readonly AnotadorDeCambios? _anotador;

    public IReadOnlyList<EntornoBD> Entornos { get; }

    [ObservableProperty] private EntornoBD? _entornoElegido;

    // ---------- Búsqueda ----------

    /// <summary>
    /// La lista de contratos, pegada. Se parte con el mismo analizador que el resto de la
    /// aplicación, así que admite comas, puntos y coma, tabuladores y saltos de línea, y
    /// descarta repetidos.
    /// </summary>
    [ObservableProperty] private string _contrato = "";

    /// <summary>Los contratos reconocidos de lo pegado.</summary>
    public IReadOnlyList<long> ContratosValidos
    {
        get
        {
            var analisis = AnalizadorEntradas.Analizar(Contrato, TipoLista.Contratos);
            var lista = new List<long>();
            foreach (var v in analisis.Validos)
            {
                if (long.TryParse(v, out var n) && n > 0) lista.Add(n);
            }
            return lista;
        }
    }

    /// <summary>Lo que se lee bajo el cuadro: cuántos han entrado y qué se ha descartado.</summary>
    public string ResumenContratos
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Contrato)) return "";

            var analisis = AnalizadorEntradas.Analizar(Contrato, TipoLista.Contratos);
            var partes = new List<string> { $"{analisis.Validos.Count} contratos" };

            if (analisis.Repetidos > 0) partes.Add($"{analisis.Repetidos} repetidos");
            if (analisis.Descartados.Count > 0)
            {
                partes.Add($"descartados: {string.Join(", ", analisis.Descartados.Take(5))}" +
                           (analisis.Descartados.Count > 5 ? " y más" : ""));
            }

            return string.Join(" · ", partes);
        }
    }
    [ObservableProperty] private string _numeroFactura = "";
    [ObservableProperty] private bool _soloSinEquipo = true;
    [ObservableProperty] private bool _trabajando;

    partial void OnContratoChanged(string value)
    {
        OnPropertyChanged(nameof(PuedeBuscar));
        OnPropertyChanged(nameof(ResumenContratos));
    }
    partial void OnTrabajandoChanged(bool value)
    {
        OnPropertyChanged(nameof(PuedeBuscar));
        OnPropertyChanged(nameof(PuedeGuardar));
        OnPropertyChanged(nameof(TextoBuscar));
    }

    public bool PuedeBuscar =>
        !Trabajando && ContratosValidos.Count > 0 && EntornoElegido is not null;

    /// <summary>
    /// Lo que pone el botón de buscar.
    ///
    /// Contra Producción esto tarda lo suyo, y hasta ahora lo único que pasaba era que el
    /// botón se apagaba: desde fuera, igual que si no hubiera llegado la pulsación. Que lo
    /// diga el propio botón sale más barato que un indicador aparte y se mira donde se acaba
    /// de pulsar.
    /// </summary>
    public string TextoBuscar => Trabajando ? "Buscando…" : "Buscar";

    public ObservableCollection<LecturaVm> Lecturas { get; } = new();

    /// <summary>
    /// El id que se pone de golpe en todas las filas. No se ofrece ningún catálogo de
    /// equipos a propósito: el id lo trae quien hace el cambio.
    /// </summary>
    [ObservableProperty] private string _idParaTodas = "";

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
            var malos = Lecturas.Count(l => l.Problema.Length > 0);
            var contratos = Lecturas.Select(l => l.Lectura.CodigoContrato).Distinct().Count();

            return $"{Lecturas.Count} lecturas de {contratos} contratos · " +
                   $"{conEquipo} ya tienen equipo · {cambian} marcadas para cambiar" +
                   (malos > 0 ? $" · {malos} con el id mal escrito" : "");
        }
    }

    /// <summary>Hay algo que guardar y no se está ya guardando.</summary>
    /// <summary>
    /// Con algún id mal escrito NO se deja guardar: es preferible a asignar la mitad y que
    /// quien lo lanza no se entere de cuál se quedó fuera.
    /// </summary>
    public bool PuedeGuardar =>
        !Trabajando && Lecturas.Any(l => l.VaACambiar) && !Lecturas.Any(l => l.Problema.Length > 0);

    [RelayCommand]
    private async Task Buscar()
    {
        if (!PuedeBuscar) return;

        Error = "";
        Aviso = "";
        Trabajando = true;

        try
        {
            var contratos = ContratosValidos;
            var servicio = new ServicioEquipoLectura(_entornos.CadenaConexion(EntornoElegido!, 120));

            var lecturas = await servicio.BuscarAsync(
                contratos, NumeroFactura, SoloSinEquipo).ConfigureAwait(true);

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
                    ? "No hay lecturas sin equipo en esos contratos. Desmarca la casilla para verlas todas."
                    : "No hay lecturas en esos contratos con esa factura.";
            }
            else
            {
                // Cuántos de los pedidos han traído algo. Con una lista pegada es lo primero
                // que se mira: si faltan, es que el número está mal o no tienen lecturas así.
                var conLecturas = Lecturas.Select(l => l.Lectura.CodigoContrato).Distinct().Count();
                if (conLecturas < contratos.Count)
                {
                    Aviso = $"{conLecturas} de los {contratos.Count} contratos han traído lecturas.";
                }
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

    /// <summary>
    /// Pone el mismo id en todas las que no tengan equipo. Útil cuando la lista es de un solo
    /// contrato; con varios, lo normal es que cada uno lleve el suyo.
    /// </summary>
    [RelayCommand]
    private void AplicarATodas()
    {
        var texto = (IdParaTodas ?? "").Trim();
        if (texto.Length == 0) return;

        foreach (var l in Lecturas.Where(l => !l.YaTenia)) l.IdEquipo = texto;

        Refrescar();
    }

    [RelayCommand]
    private void QuitarSeleccion()
    {
        foreach (var l in Lecturas.Where(l => !l.YaTenia)) l.IdEquipo = "";
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
        var arranque = DateTime.Now;
        var contratos = Lecturas.Where(l => l.VaACambiar)
                                .Select(l => l.CodigoContrato)
                                .Distinct()
                                .Count();

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

            // AL LOG. Esto escribe en Producción, así que tiene que verse en Ejecuciones:
            // quién, cuándo, en qué entorno y sobre cuántas lecturas.
            if (_anotador is not null)
            {
                var detalle = $"{hechas:N0} lectura{(hechas == 1 ? "" : "s")} de {contratos:N0} " +
                              $"contrato{(contratos == 1 ? "" : "s")}" +
                              (saltadas > 0 ? $" · {saltadas:N0} ya tenían equipo" : "") +
                              (fallos > 0 ? $" · {fallos:N0} con error" : "");

                await _anotador.AnotarAsync(
                    "Asignar nº de equipo en prefacturas",
                    SeccionOperacion.Contratos,
                    detalle,
                    EntornoElegido!.Clave,
                    AnotadorDeCambios.EstadoPor(hechas, fallos),
                    registros: hechas,
                    errores: fallos,
                    duracion: DateTime.Now - arranque).ConfigureAwait(true);
            }
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
