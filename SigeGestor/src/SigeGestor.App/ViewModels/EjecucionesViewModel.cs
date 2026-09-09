using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;
using SigeGestor.Core.Servicios;

namespace SigeGestor.App.ViewModels;

/// <summary>Una opción de filtro: lo que se muestra y con qué se compara.</summary>
public sealed class FiltroVm
{
    public required string Etiqueta { get; init; }

    /// <summary>Cadena vacía = «todos».</summary>
    public required string Valor { get; init; }

    public override string ToString() => Etiqueta;
}

/// <summary>
/// El histórico de ejecuciones del equipo, con filtros.
///
/// La barra lateral tenía esta entrada desde el primer día y al pulsarla no pasaba nada:
/// CrearPagina devolvía null y MostrarSeccion se salía en silencio. El inicio ya enseñaba las
/// últimas, pero sin poder filtrar ni ver más allá de un puñado.
///
/// SE FILTRA EN MEMORIA, no en el repositorio. La retención del log es de tres días y son ocho
/// personas: son unos cientos de líneas. Montar el filtrado en la capa de datos, contra
/// ficheros por SMB en el .13, sería más código y más lento.
/// </summary>
public sealed partial class EjecucionesViewModel : ObservableObject
{
    /// <summary>
    /// Techo de lo que se pide al repositorio. Con tres días de retención no se llega ni de
    /// lejos, pero si alguien sube la retención esto evita cargar el log entero de golpe.
    /// </summary>
    private const int Techo = 5000;

    private readonly IRepositorioEjecuciones _repositorio;
    private readonly Usuario _usuario;

    /// <summary>
    /// Lo que está corriendo en este equipo ahora mismo, o Nothing si no se ha pasado.
    ///
    /// POR QUÉ HACE FALTA: el histórico se escribe AL TERMINAR, así que mientras una operación
    /// corre no hay nada en el log. El filtro «En marcha» de esta misma rejilla no podía casar
    /// con nada: era una opción muerta desde el primer día. Lanzabas la descarga de 70 PDF, te
    /// venías aquí a ver si seguía y la rejilla decía que no había ninguna ejecución.
    ///
    /// Son las de ESTE equipo, no las del resto. Las de los demás no se pueden saber sin
    /// escribirlas en el log al arrancar, y eso dejaría registros fantasma «en marcha» para
    /// siempre en cuanto a alguien se le cerrase la aplicación a media consulta.
    /// </summary>
    private readonly EjecucionesEnCurso? _enCurso;

    private IReadOnlyList<Ejecucion> _todas = Array.Empty<Ejecucion>();

    /// <summary>
    /// Se ha pulsado cancelar en una fila. Lo recoge la página para preguntar antes de hacerlo:
    /// una consulta de cuarenta minutos no se tira por un clic de más.
    /// </summary>
    public event EventHandler<EjecucionVm>? CancelacionPedida;

    public EjecucionesViewModel(IRepositorioEjecuciones repositorio,
                                Usuario usuario,
                                EjecucionesEnCurso? enCurso = null)
    {
        _repositorio = repositorio;
        _usuario = usuario;
        _enCurso = enCurso;

        if (_enCurso is not null)
        {
            // Solo cuando la lista cambia, no en cada entrada procesada: eso lo lleva la propia
            // fila, que se suscribe al progreso y actualiza su contador sin rehacer la rejilla.
            _enCurso.ListaCambiada += RegistroCambiado;
        }

        Estados = new List<FiltroVm>
        {
            new() { Etiqueta = "Cualquier estado", Valor = "" },
            new() { Etiqueta = "Completadas",     Valor = nameof(EstadoEjecucion.Completada) },
            new() { Etiqueta = "Con errores",     Valor = nameof(EstadoEjecucion.ConErrores) },
            new() { Etiqueta = "Sin cambios",     Valor = nameof(EstadoEjecucion.SinCambios) },
            new() { Etiqueta = "Canceladas",      Valor = nameof(EstadoEjecucion.Cancelada) },
            new() { Etiqueta = "En marcha",       Valor = nameof(EstadoEjecucion.EnCurso) }
        };
        _estado = Estados[0];

        Entornos = new List<FiltroVm>
        {
            new() { Etiqueta = "Cualquier entorno", Valor = "" },
            new() { Etiqueta = "Producción",        Valor = nameof(ClaveEntorno.Produccion) },
            new() { Etiqueta = "Replica",           Valor = nameof(ClaveEntorno.Replica) },
            new() { Etiqueta = "UAT",               Valor = nameof(ClaveEntorno.Uat) }
        };
        _entorno = Entornos[0];

        // Ojo: la seleccionada tiene que ser LA MISMA instancia que está en la colección, no
        // una equivalente. Con dos instancias distintas el desplegable sale en blanco.
        Secciones = new ObservableCollection<FiltroVm>(SeccionesVacio());
        _seccion = Secciones[0];

        Personas = new ObservableCollection<FiltroVm>(PersonasVacio());
        _persona = Personas[0];
    }

    private static List<FiltroVm> SeccionesVacio() =>
        new() { new FiltroVm { Etiqueta = "Cualquier sección", Valor = "" } };

    private static List<FiltroVm> PersonasVacio() =>
        new() { new FiltroVm { Etiqueta = "Todo el equipo", Valor = "" } };

    // ============================================================
    // CARGA
    // ============================================================

    public ObservableCollection<EjecucionVm> Filas { get; } = new();

    [ObservableProperty]
    private bool _cargando;

    [ObservableProperty]
    private string _error = string.Empty;

    public Visibility VisibilidadError =>
        string.IsNullOrEmpty(Error) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnErrorChanged(string value) => OnPropertyChanged(nameof(VisibilidadError));

    public async Task CargarAsync()
    {
        Cargando = true;
        Error = string.Empty;

        try
        {
            _todas = await _repositorio.ObtenerRecientesAsync(Techo);

            // Las listas de sección y persona salen de lo que hay en el log, no de una lista
            // fija: filtrar por alguien que no ha lanzado nada no sirve de nada.
            //
            // Y DE LO QUE ESTÁ CORRIENDO TAMBIÉN. Sacándolas solo del log, la sección de una
            // operación que se acaba de lanzar no aparecía en el desplegable —el log se escribe
            // al terminar— así que no se podía filtrar por ella justo cuando más interesa. Con
            // el histórico vacío un lunes, «Facturas» no existía como opción aunque hubiera una
            // descarga de PDF en marcha.
            var enMarcha = EnMarcha().ToList();

            Seccion = Rellenar(Secciones, SeccionesVacio(), Seccion,
                               _todas.Select(e => e.Grupo)
                                     .Concat(enMarcha.Select(t => t.Grupo))
                                     .Where(g => !string.IsNullOrWhiteSpace(g)));

            Persona = Rellenar(Personas, PersonasVacio(), Persona,
                               _todas.Select(e => e.NombreUsuario)
                                     .Concat(enMarcha.Select(_ => _usuario.Nombre))
                                     .Where(n => !string.IsNullOrWhiteSpace(n)));
        }
        catch (Exception ex)
        {
            _todas = Array.Empty<Ejecucion>();
            Error = $"No se ha podido leer el histórico: {ex.Message}";
        }
        finally
        {
            Cargando = false;
            Aplicar();
        }
    }

    /// <summary>
    /// Rehace una lista de filtro y devuelve qué hay que dejar seleccionado.
    ///
    /// DEVUELVE LA SELECCIÓN, no la deja como estaba, y eso es lo importante: al rehacer la
    /// lista los elementos son instancias NUEVAS, así que la que estuviera seleccionada ya no
    /// pertenece a la lista y el desplegable sale EN BLANCO. Se busca la equivalente por su
    /// valor y se selecciona esa; si el valor ha desaparecido del log, se cae a «todos».
    /// </summary>
    private static FiltroVm Rellenar(ObservableCollection<FiltroVm> destino,
                                     List<FiltroVm> cabecera,
                                     FiltroVm? seleccionado,
                                     IEnumerable<string> valores)
    {
        destino.Clear();
        foreach (var f in cabecera) destino.Add(f);

        foreach (var v in valores.Distinct(StringComparer.OrdinalIgnoreCase)
                                 .OrderBy(v => v, StringComparer.CurrentCulture))
        {
            destino.Add(new FiltroVm { Etiqueta = v, Valor = v });
        }

        var valorAntes = seleccionado?.Valor ?? "";

        return destino.FirstOrDefault(
            f => string.Equals(f.Valor, valorAntes, StringComparison.OrdinalIgnoreCase))
               ?? destino[0];
    }

    // ============================================================
    // FILTROS
    // ============================================================

    public IReadOnlyList<FiltroVm> Estados { get; }
    public IReadOnlyList<FiltroVm> Entornos { get; }
    public ObservableCollection<FiltroVm> Secciones { get; }
    public ObservableCollection<FiltroVm> Personas { get; }

    [ObservableProperty]
    private FiltroVm _estado;

    partial void OnEstadoChanged(FiltroVm value) => Aplicar();

    [ObservableProperty]
    private FiltroVm _entorno;

    partial void OnEntornoChanged(FiltroVm value) => Aplicar();

    [ObservableProperty]
    private FiltroVm _seccion;

    partial void OnSeccionChanged(FiltroVm value) => Aplicar();

    [ObservableProperty]
    private FiltroVm _persona;

    partial void OnPersonaChanged(FiltroVm value) => Aplicar();

    [ObservableProperty]
    private string _busqueda = string.Empty;

    partial void OnBusquedaChanged(string value) => Aplicar();

    /// <summary>Atajo: solo lo mío. Es lo que se quiere el 90 % de las veces.</summary>
    [ObservableProperty]
    private bool _soloMias;

    partial void OnSoloMiasChanged(bool value) => Aplicar();

    public void Limpiar()
    {
        SoloMias = false;
        Busqueda = string.Empty;
        Estado = Estados[0];
        Entorno = Entornos[0];
        if (Secciones.Count > 0) Seccion = Secciones[0];
        if (Personas.Count > 0) Persona = Personas[0];
    }

    private void Aplicar()
    {
        Filas.Clear();

        // Las que corren van PRIMERO y no ordenadas por fecha con el resto: son lo que se
        // viene a mirar, y son las más recientes por definición.
        foreach (var t in EnMarcha().Where(EncajaViva))
        {
            // La fila no cancela sola: avisa, y quien pinta la rejilla pregunta antes.
            Filas.Add(new EjecucionVm(t, fila => CancelacionPedida?.Invoke(this, fila)));
        }

        foreach (var e in _todas.Where(Encaja))
        {
            Filas.Add(new EjecucionVm(e));
        }

        // Se apunta aquí y no solo en RegistroCambiado: si la rejilla se abre con algo ya
        // corriendo, el contador arrancaba en 0 y al terminar la tarea la comparación no veía
        // el cambio, así que la fila viva desaparecía sin releer el histórico y su registro de
        // verdad no salía hasta pulsar «Recargar».
        _corriendoAntes = EnMarcha().Count();

        OnPropertyChanged(nameof(Recuento));
        OnPropertyChanged(nameof(Resumen));
        OnPropertyChanged(nameof(VisibilidadFilas));
        OnPropertyChanged(nameof(VisibilidadVacio));
        OnPropertyChanged(nameof(HayFiltro));
    }

    /// <summary>
    /// Las que siguen corriendo. Las terminadas NO se listan aquí aunque el indicador de la
    /// cabecera las mantenga: en cuanto acaban se escriben en el log, y sacarlas por los dos
    /// sitios daría la misma ejecución dos veces en la rejilla.
    /// </summary>
    private IEnumerable<TareaEnCurso> EnMarcha() =>
        _enCurso is null
            ? Array.Empty<TareaEnCurso>()
            : _enCurso.Tareas.Where(t => !t.Terminada).Reverse();

    /// <summary>
    /// Mismos filtros que las del histórico, con lo que se sabe de una que aún corre: no tiene
    /// resultado ni duración, y siempre es de quien está delante y de este equipo.
    /// </summary>
    private bool EncajaViva(TareaEnCurso t)
    {
        if (Estado?.Valor.Length > 0 && Estado.Valor != nameof(EstadoEjecucion.EnCurso)) return false;
        if (Entorno?.Valor.Length > 0 && t.Entorno.ToString() != Entorno.Valor) return false;

        if (Seccion?.Valor.Length > 0
            && !string.Equals(t.Grupo, Seccion.Valor, StringComparison.OrdinalIgnoreCase))
            return false;

        // La lanza quien está delante, así que «solo mías» no la descarta nunca; y por persona,
        // solo encaja si la elegida es esa misma.
        if (Persona?.Valor.Length > 0
            && !string.Equals(_usuario.Nombre, Persona.Valor, StringComparison.OrdinalIgnoreCase))
            return false;

        var texto = Busqueda.Trim();
        if (texto.Length > 0)
        {
            var c = StringComparison.OrdinalIgnoreCase;
            if (!t.Operacion.Contains(texto, c) && !t.Grupo.Contains(texto, c)) return false;
        }

        return true;
    }

    /// <summary>
    /// Ha cambiado lo que hay en marcha. Si algo ha TERMINADO se relee el histórico: ese es el
    /// momento en que aparece su registro de verdad, con su resultado y su duración.
    /// </summary>
    private void RegistroCambiado(object? emisor, EventArgs e)
    {
        // Menos corriendo que antes significa que algo ha terminado, y ese es el momento en
        // que su registro aparece en el log con su resultado y su duración: se relee.
        if (EnMarcha().Count() < _corriendoAntes)
        {
            _ = CargarAsync();
            return;
        }

        Aplicar();
    }

    private int _corriendoAntes;

    private bool Encaja(Ejecucion e)
    {
        if (SoloMias && !string.Equals(e.Login, _usuario.Login, StringComparison.OrdinalIgnoreCase))
            return false;

        if (Estado?.Valor.Length > 0 && e.Estado.ToString() != Estado.Valor) return false;
        if (Entorno?.Valor.Length > 0 && e.Entorno.ToString() != Entorno.Valor) return false;

        if (Seccion?.Valor.Length > 0
            && !string.Equals(e.Grupo, Seccion.Valor, StringComparison.OrdinalIgnoreCase))
            return false;

        if (Persona?.Valor.Length > 0
            && !string.Equals(e.NombreUsuario, Persona.Valor, StringComparison.OrdinalIgnoreCase))
            return false;

        var texto = Busqueda.Trim();
        if (texto.Length > 0)
        {
            var c = StringComparison.OrdinalIgnoreCase;
            var encaja = (e.Operacion ?? "").Contains(texto, c)
                         || (e.Detalle ?? "").Contains(texto, c)
                         || (e.NombreUsuario ?? "").Contains(texto, c)
                         || (e.Equipo ?? "").Contains(texto, c);
            if (!encaja) return false;
        }

        return true;
    }

    public bool HayFiltro =>
        SoloMias
        || Busqueda.Trim().Length > 0
        || Estado?.Valor.Length > 0
        || Entorno?.Valor.Length > 0
        || Seccion?.Valor.Length > 0
        || Persona?.Valor.Length > 0;

    /// <summary>Total del que se filtra: el histórico más lo que corre ahora.</summary>
    private int Disponibles => _todas.Count + EnMarcha().Count();

    public string Recuento => Disponibles == 0
        ? ""
        : Filas.Count == Disponibles
            ? Redaccion.Cuenta(Disponibles, "ejecución", "ejecuciones")
            : $"{Filas.Count:N0} de {Disponibles:N0}";

    /// <summary>Cifras de lo filtrado. Sirve para ver de un golpe cuánto ha fallado.</summary>
    public string Resumen
    {
        get
        {
            if (Filas.Count == 0) return "";

            var conError = Filas.Count(f => f.Estado == EstadoEjecucion.ConErrores);
            var sinCambios = Filas.Count(f => f.Estado == EstadoEjecucion.SinCambios);
            var canceladas = Filas.Count(f => f.Estado == EstadoEjecucion.Cancelada);

            var partes = new List<string>();
            if (conError > 0) partes.Add($"{conError} con errores");
            if (sinCambios > 0) partes.Add($"{sinCambios} sin cambios");
            if (canceladas > 0) partes.Add($"{canceladas} canceladas");

            return partes.Count == 0 ? "todas bien" : string.Join(" · ", partes);
        }
    }

    public Visibility VisibilidadFilas => Filas.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadVacio =>
        !Cargando && Filas.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    partial void OnCargandoChanged(bool value)
    {
        OnPropertyChanged(nameof(VisibilidadVacio));
        OnPropertyChanged(nameof(VisibilidadCargando));
    }

    public Visibility VisibilidadCargando => Cargando ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Qué decir cuando no hay nada. Distingue «no hay log» de «tus filtros no dejan pasar
    /// nada», que son dos problemas distintos y con distinta solución.
    /// </summary>
    public string TextoVacio => Disponibles == 0
        ? "Todavía no hay ninguna ejecución registrada. El histórico se conserva unos pocos días, así que estar vacío al empezar la semana es normal."
        : "Ninguna ejecución encaja con los filtros. Prueba a quitar alguno.";
}
