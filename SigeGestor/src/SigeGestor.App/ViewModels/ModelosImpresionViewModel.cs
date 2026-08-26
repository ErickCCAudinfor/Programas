using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Impresion;
using SigeGestor.Core;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.ViewModels;

/// <summary>Una empresa en la lista de destinos, con su casilla de selección.</summary>
public sealed partial class DestinoVm : ObservableObject
{
    public DestinoVm(EmpresaBD empresa)
    {
        Empresa = empresa;
    }

    public EmpresaBD Empresa { get; }

    public string Nombre => Empresa.Nombre;
    public string Descripcion => Empresa.Descripcion;
    public Visibility VisibilidadVpn => Empresa.VPN ? Visibility.Visible : Visibility.Collapsed;

    [ObservableProperty]
    private bool _elegida;

    partial void OnElegidaChanged(bool value) => Cambiado?.Invoke(this, EventArgs.Empty);

    public event EventHandler? Cambiado;

    /// <summary>Resultado de la última operación masiva sobre esta empresa.</summary>
    [ObservableProperty]
    private string _resultado = string.Empty;

    [ObservableProperty]
    private bool _resultadoBien;
}

/// <summary>Una fila de la rejilla de modelos.</summary>
public sealed class ModeloVm
{
    public required ModeloDeImpresion Modelo { get; init; }

    public long Id => Modelo.IdModeloDeImpresion;
    public string Descripcion => Modelo.DescripcionModeloDeImpresion ?? "";
    public string Entorno => Modelo.Entorno switch
    {
        "G1" => "Luz (G1)",
        "G2" => "Gas (G2)",
        _ => Modelo.Entorno ?? ""
    };
    public string Tipo => ModelosImpresionViewModel.NombreTipo(Modelo.CodigoTipoModeloDeImpresion);
    public string ClassName => Modelo.ClassName ?? "";
    public string RptFileName => Modelo.RptFileName ?? "";
}

/// <summary>Un tipo de modelo, para el desplegable.</summary>
public sealed class TipoModeloVm
{
    public required int Codigo { get; init; }
    public required string Nombre { get; init; }
    public override string ToString() => Nombre;
}

/// <summary>
/// La pantalla de modelos de impresión.
///
/// Junta en una sola cosa lo que en ActualizaPrecios eran tres formularios: ModeloImpresionForm
/// (la rejilla por empresa con su filtro), EditarModeloImpresionForm (el alta y la edición) y
/// AnadirMasivoEmpresaForm (subir, actualizar y comprobar sobre varias empresas a la vez).
/// </summary>
public sealed partial class ModelosImpresionViewModel : ObservableObject
{
    private readonly RepositorioEmpresas _empresas = new();
    private readonly ServicioModelosImpresion _servicio = new();

    private IReadOnlyList<ModeloDeImpresion> _todos = Array.Empty<ModeloDeImpresion>();
    private CancellationTokenSource? _cancelacion;

    public ModelosImpresionViewModel()
    {
        Tipos = Enum.GetValues<TipoModeloImpresionGeneral>()
            .Select(t => new TipoModeloVm { Codigo = (int)t, Nombre = NombreTipo((int)t) })
            .ToList();

        CargarEmpresas();
    }

    /// <summary>
    /// El nombre legible de un tipo. El enum viene con nombres pegados —AvisoImpagado,
    /// Pre_Contrato— así que se separan para que se lean en un desplegable.
    /// </summary>
    public static string NombreTipo(int codigo)
    {
        var nombre = Enum.IsDefined(typeof(TipoModeloImpresionGeneral), codigo)
            ? ((TipoModeloImpresionGeneral)codigo).ToString()
            : codigo.ToString();

        nombre = nombre.Replace('_', ' ');

        // Separa las mayúsculas interiores: «AvisoImpagado» -> «Aviso impagado».
        var partes = new List<char>();
        for (var i = 0; i < nombre.Length; i++)
        {
            if (i > 0 && char.IsUpper(nombre[i]) && nombre[i - 1] != ' ')
            {
                partes.Add(' ');
                partes.Add(char.ToLowerInvariant(nombre[i]));
            }
            else
            {
                partes.Add(nombre[i]);
            }
        }

        return $"{new string(partes.ToArray())} ({codigo})";
    }

    public IReadOnlyList<TipoModeloVm> Tipos { get; }

    // ============================================================
    // EMPRESAS
    // ============================================================

    public ObservableCollection<DestinoVm> Destinos { get; } = new();

    [ObservableProperty]
    private DestinoVm? _empresaActiva;

    partial void OnEmpresaActivaChanged(DestinoVm? value)
    {
        OnPropertyChanged(nameof(TituloModelos));
        _ = CargarModelosAsync();
    }

    /// <summary>
    /// Título de la rejilla. Se compone aquí y no en el XAML porque allí obligaba a partirlo en
    /// dos Run, y Run.Text enlaza en TwoWay por omisión: contra una propiedad de solo lectura
    /// eso lanza InvalidOperationException nada más pintarse. Además así el caso sin empresa
    /// dice «Modelos» y no «Modelos de » con el hueco colgando.
    /// </summary>
    public string TituloModelos =>
        EmpresaActiva is null ? "Modelos" : $"Modelos de {EmpresaActiva.Nombre}";

    public string RutaEmpresas => _empresas.Ruta;

    public Visibility VisibilidadSinEmpresas =>
        Destinos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    private void CargarEmpresas()
    {
        Destinos.Clear();

        try
        {
            foreach (var empresa in _empresas.Cargar())
            {
                var vm = new DestinoVm(empresa);
                vm.Cambiado += (_, _) => RevisarSeleccion();
                Destinos.Add(vm);
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }

        EmpresaActiva = Destinos.FirstOrDefault();

        OnPropertyChanged(nameof(VisibilidadSinEmpresas));
        RevisarSeleccion();
    }

    public IReadOnlyList<DestinoVm> Elegidas => Destinos.Where(d => d.Elegida).ToList();

    public int NumeroElegidas => Elegidas.Count;

    public string TextoElegidas => NumeroElegidas switch
    {
        0 => "Ninguna empresa marcada",
        1 => "1 empresa marcada",
        _ => $"{NumeroElegidas} empresas marcadas"
    };

    private void RevisarSeleccion()
    {
        OnPropertyChanged(nameof(NumeroElegidas));
        OnPropertyChanged(nameof(TextoElegidas));
        OnPropertyChanged(nameof(PuedeSubir));
        OnPropertyChanged(nameof(PuedeActualizar));
        OnPropertyChanged(nameof(PuedeComprobar));
    }

    public void MarcarTodas(bool marcadas)
    {
        foreach (var destino in Destinos) destino.Elegida = marcadas;
    }

    // ============================================================
    // REJILLA DE MODELOS DE LA EMPRESA ACTIVA
    // ============================================================

    public ObservableCollection<ModeloVm> Modelos { get; } = new();

    [ObservableProperty]
    private ModeloVm? _modeloSeleccionado;

    partial void OnModeloSeleccionadoChanged(ModeloVm? value) =>
        OnPropertyChanged(nameof(HayModelo));

    public bool HayModelo => ModeloSeleccionado is not null;

    [ObservableProperty]
    private string _filtro = string.Empty;

    partial void OnFiltroChanged(string value) => AplicarFiltro();

    [ObservableProperty]
    private bool _cargando;

    public string Recuento => _todos.Count == 0
        ? ""
        : Modelos.Count == _todos.Count
            ? $"{_todos.Count:N0} modelos"
            : $"{Modelos.Count:N0} de {_todos.Count:N0}";

    public async Task CargarModelosAsync()
    {
        Modelos.Clear();
        _todos = Array.Empty<ModeloDeImpresion>();
        ModeloSeleccionado = null;
        Error = string.Empty;
        OnPropertyChanged(nameof(Recuento));

        if (EmpresaActiva is null) return;

        Cargando = true;
        try
        {
            _todos = await _servicio.Listar(EmpresaActiva.Empresa);
            AplicarFiltro();
        }
        catch (Exception ex)
        {
            var motivo = ex.Message;
            if (EmpresaActiva.Empresa.VPN) motivo += " · esta empresa requiere VPN";
            Error = $"No se han podido traer los modelos de {EmpresaActiva.Nombre}: {motivo}";
        }
        finally
        {
            Cargando = false;
        }
    }

    /// <summary>
    /// Filtra por descripción, entorno, clase o nombre de fichero, igual que el original.
    /// </summary>
    private void AplicarFiltro()
    {
        Modelos.Clear();

        var texto = Filtro.Trim();

        foreach (var modelo in _todos)
        {
            if (texto.Length > 0 && !Encaja(modelo, texto)) continue;
            Modelos.Add(new ModeloVm { Modelo = modelo });
        }

        OnPropertyChanged(nameof(Recuento));
    }

    private static bool Encaja(ModeloDeImpresion m, string texto)
    {
        var c = StringComparison.OrdinalIgnoreCase;
        return (m.DescripcionModeloDeImpresion ?? "").Contains(texto, c)
               || (m.Entorno ?? "").Contains(texto, c)
               || (m.ClassName ?? "").Contains(texto, c)
               || (m.RptFileName ?? "").Contains(texto, c);
    }

    // ============================================================
    // MASIVO
    // ============================================================

    /// <summary>Datos del modelo con el que se opera en masivo.</summary>
    [ObservableProperty]
    private string _descripcionMasivo = string.Empty;

    partial void OnDescripcionMasivoChanged(string value) => RevisarSeleccion();

    [ObservableProperty]
    private string _classNameMasivo = string.Empty;

    /// <summary>G1 luz, G2 gas.</summary>
    [ObservableProperty]
    private string _entornoMasivo = "G1";

    [ObservableProperty]
    private TipoModeloVm? _tipoMasivo;

    partial void OnTipoMasivoChanged(TipoModeloVm? value) => RevisarSeleccion();

    private byte[]? _reportMasivo;
    private string _nombreReportMasivo = "";

    public string TextoReport => _reportMasivo is null
        ? "Ningún .rpt cargado"
        : $"{_nombreReportMasivo} · {_reportMasivo.Length / 1024.0:N0} KB";

    public void CargarReport(string ruta)
    {
        try
        {
            _reportMasivo = System.IO.File.ReadAllBytes(ruta);
            _nombreReportMasivo = System.IO.Path.GetFileName(ruta);

            // El .rpt vacío o de dos bytes no es un report: la comprobación del original
            // («LEN(modelo) >= 10») da por hecho que un modelo de verdad pesa.
            if (_reportMasivo.Length < 10)
            {
                _reportMasivo = null;
                Error = "Ese fichero está vacío o es demasiado pequeño para ser un report.";
            }
            else
            {
                Error = string.Empty;
            }
        }
        catch (Exception ex)
        {
            _reportMasivo = null;
            Error = $"No se ha podido leer el fichero: {ex.Message}";
        }

        OnPropertyChanged(nameof(TextoReport));
        RevisarSeleccion();
    }

    public bool PuedeSubir =>
        !Trabajando && NumeroElegidas > 0 && _reportMasivo is not null
        && DescripcionMasivo.Trim().Length > 0 && TipoMasivo is not null;

    public bool PuedeActualizar => PuedeSubir;

    public bool PuedeComprobar =>
        !Trabajando && NumeroElegidas > 0
        && DescripcionMasivo.Trim().Length > 0 && TipoMasivo is not null;

    [ObservableProperty]
    private bool _trabajando;

    partial void OnTrabajandoChanged(bool value) => RevisarSeleccion();

    [ObservableProperty]
    private string _progreso = string.Empty;

    public Visibility VisibilidadProgreso =>
        string.IsNullOrEmpty(Progreso) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnProgresoChanged(string value) => OnPropertyChanged(nameof(VisibilidadProgreso));

    [ObservableProperty]
    private string _resumen = string.Empty;

    public Visibility VisibilidadResumen =>
        string.IsNullOrEmpty(Resumen) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnResumenChanged(string value) => OnPropertyChanged(nameof(VisibilidadResumen));

    [ObservableProperty]
    private string _error = string.Empty;

    public Visibility VisibilidadError =>
        string.IsNullOrEmpty(Error) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnErrorChanged(string value) => OnPropertyChanged(nameof(VisibilidadError));

    private ModeloDeImpresion ComponerMasivo() => new()
    {
        Entorno = EntornoMasivo,
        DescripcionModeloDeImpresion = DescripcionMasivo.Trim(),
        CodigoTipoModeloDeImpresion = TipoMasivo!.Codigo,
        ClassName = ClassNameMasivo.Trim(),
        RptFileName = _nombreReportMasivo,
        Modelo = _reportMasivo
    };

    public Task SubirAsync() =>
        EjecutarMasivoAsync((servicio, empresas, avisar, ct) =>
            servicio.SubirAsync(empresas, ComponerMasivo(), avisar, ct), "Subida");

    public Task ActualizarAsync() =>
        EjecutarMasivoAsync((servicio, empresas, avisar, ct) =>
            servicio.ActualizarAsync(empresas, ComponerMasivo(), avisar, ct), "Actualización");

    public Task ComprobarAsync() =>
        EjecutarMasivoAsync((servicio, empresas, avisar, ct) =>
            servicio.ComprobarAsync(empresas, TipoMasivo!.Codigo, DescripcionMasivo.Trim(), avisar, ct),
            "Comprobación");

    /// <summary>
    /// El bucle de una operación masiva. Los tres botones del original repetían el mismo
    /// cuerpo —validar, recorrer, contar éxitos, montar el mensaje— con pequeñas variaciones.
    /// </summary>
    private async Task EjecutarMasivoAsync(
        Func<ServicioModelosImpresion, IEnumerable<EmpresaBD>, Action<string>, CancellationToken,
             Task<IReadOnlyList<ResultadoEmpresa>>> hacer,
        string queEs)
    {
        var elegidas = Elegidas;
        if (elegidas.Count == 0) return;

        foreach (var destino in Destinos)
        {
            destino.Resultado = string.Empty;
            destino.ResultadoBien = false;
        }

        Error = string.Empty;
        Resumen = string.Empty;
        Trabajando = true;

        _cancelacion?.Dispose();
        _cancelacion = new CancellationTokenSource();

        try
        {
            var resultados = await hacer(
                _servicio,
                elegidas.Select(d => d.Empresa),
                mensaje => Progreso = mensaje,
                _cancelacion.Token);

            // Cada resultado a su fila, para verlo sin leer un párrafo.
            foreach (var resultado in resultados)
            {
                var destino = Destinos.FirstOrDefault(d => d.Nombre == resultado.Empresa);
                if (destino is null) continue;
                destino.ResultadoBien = resultado.Bien;
                destino.Resultado = resultado.Detalle;
            }

            var bien = resultados.Count(r => r.Bien);
            Resumen = bien == resultados.Count
                ? $"{queEs} terminada: las {bien} bien."
                : $"{queEs} terminada: {bien} de {resultados.Count} bien. El detalle está en cada fila.";

            // Si la empresa activa era una de las tocadas, la rejilla ya no vale.
            if (EmpresaActiva is not null && elegidas.Contains(EmpresaActiva))
            {
                await CargarModelosAsync();
            }
        }
        catch (OperationCanceledException)
        {
            Resumen = $"{queEs} cancelada. Lo que ya se hubiera aplicado no se deshace.";
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            Trabajando = false;
            Progreso = string.Empty;
        }
    }

    public void Cancelar() => _cancelacion?.Cancel();

    // ============================================================
    // ALTA, EDICIÓN Y BAJA EN LA EMPRESA ACTIVA
    // ============================================================

    public Task<byte[]?> BinarioAsync(long idModelo) =>
        EmpresaActiva is null
            ? Task.FromResult<byte[]?>(null)
            : _servicio.Binario(EmpresaActiva.Empresa, idModelo);

    public async Task<string> GuardarAsync(ModeloDeImpresion modelo, bool esNuevo)
    {
        if (EmpresaActiva is null) return "No hay empresa elegida.";

        try
        {
            // OJO: UpdateModeloImpresion hace «Modelo = @Modelo» siempre, así que guardar sin
            // binario BORRA el report. Editar solo la descripción dejaría el modelo sin nada
            // que imprimir. Si no se ha elegido un .rpt nuevo, se trae el que ya está y se
            // vuelve a escribir tal cual.
            //
            // ActualizaPrecios lo evitaba descargando el binario CADA VEZ que se abría la
            // ventana de edición, aunque no se fuera a cambiar. Son varios megas por fila
            // contra una base remota; así solo se paga al guardar.
            if (!esNuevo && modelo.Modelo is null)
            {
                modelo.Modelo = await _servicio.Binario(EmpresaActiva.Empresa, modelo.IdModeloDeImpresion);

                if (modelo.Modelo is null)
                {
                    return "No se ha podido recuperar el report actual, y guardar así lo borraría. " +
                           "Vuelve a intentarlo, o elige el fichero .rpt de nuevo.";
                }
            }

            var resultado = await _servicio.Guardar(EmpresaActiva.Empresa, modelo, esNuevo);
            if (resultado <= 0) return "La base no ha devuelto ninguna fila afectada.";

            await CargarModelosAsync();
            Resumen = esNuevo
                ? $"Modelo creado en {EmpresaActiva.Nombre} con Id {resultado}."
                : $"Modelo actualizado en {EmpresaActiva.Nombre}.";
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<string> BorrarAsync(long idModelo)
    {
        if (EmpresaActiva is null) return "No hay empresa elegida.";

        try
        {
            var filas = await _servicio.Borrar(EmpresaActiva.Empresa, idModelo);
            if (filas <= 0) return "No se ha borrado ninguna fila.";

            await CargarModelosAsync();
            Resumen = $"Modelo {idModelo} borrado de {EmpresaActiva.Nombre}.";
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public void Recargar()
    {
        CargarEmpresas();
    }
}
