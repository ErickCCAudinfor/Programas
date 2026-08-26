using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Un tipo de lista como opción del segmentado. Envuelve el enum para poder mostrar su
/// etiqueta sin meter un conversor por el medio.
/// </summary>
public sealed class TipoListaVm
{
    public required TipoLista Tipo { get; init; }
    public string Etiqueta => AnalizadorEntradas.Etiqueta(Tipo);
}

/// <summary>Un entorno como opción del selector. Puede venir deshabilitado y con su motivo.</summary>
public sealed class EntornoOpcionVm
{
    public required EntornoBD Entorno { get; init; }
    public required bool Habilitado { get; init; }
    public string Motivo { get; init; } = "";

    public string Nombre => Entorno.Nombre;
    public string Abreviatura => Entorno.Abreviatura;
    public Brush Marca => Apariencia.MarcaEntorno(Entorno.Clave);
    public string Descripcion => Entorno.Descripcion;

    public string Ayuda => Habilitado ? Descripcion : $"{Descripcion} · {Motivo}";
}

/// <summary>
/// El formulario de una operación, generado a partir de su definición.
///
/// Una sola vista sirve a 43 de las 45 operaciones del catálogo —las otras dos son rejillas de
/// mantenimiento y tienen página propia—: qué campos aparecen, qué se valida y qué entornos se
/// ofrecen sale todo de <see cref="DefinicionOperacion"/>. Esto es lo que evita que se repita
/// la historia de Form1 con treinta y un botones y su lógica pegada.
/// </summary>
public sealed partial class OperacionViewModel : ObservableObject
{
    private readonly RepositorioEntornos _entornos;

    public OperacionViewModel(DefinicionOperacion definicion, RepositorioEntornos entornos)
    {
        Definicion = definicion;
        _entornos = entornos;

        TiposLista = AnalizadorEntradas.TiposDe(definicion);
        Tipos = TiposLista.Select(t => new TipoListaVm { Tipo = t }).ToList();
        _tipoLista = TiposLista.Count > 0 ? TiposLista[0] : TipoLista.Contratos;
        _tipoSeleccionado = Tipos.FirstOrDefault();

        Entornos = new ObservableCollection<EntornoOpcionVm>(ConstruirEntornos());
        _entornoSeleccionado = Entornos.FirstOrDefault(o => o.Habilitado && o.Entorno.Clave == definicion.EntornoPorDefecto)
                               ?? Entornos.FirstOrDefault(o => o.Habilitado);

        _dividir = definicion.PermiteDividir && definicion.Seccion == SeccionOperacion.Consultas;

        foreach (var campo in definicion.Campos)
        {
            var vm = new CampoVm(campo);
            vm.Cambiado += (emisor, args) =>
            {
                RevisarValidez();
                if (emisor is not CampoVm cambiado) return;

                RellenarDesdeAsync(cambiado).ContinueWith(
                    t => { }, TaskContinuationOptions.OnlyOnFaulted);

                RecargarDependientesAsync(cambiado).ContinueWith(
                    t => { }, TaskContinuationOptions.OnlyOnFaulted);
            };
            Campos.Add(vm);
        }
    }

    /// <summary>
    /// Pregunta a la operación si el cambio de este campo rellena otros y aplica lo que
    /// devuelva. Es lo que hace que al elegir un producto se pongan solos su importe, su
    /// impuesto y sus casillas, igual que en ActualizaPrecios.
    ///
    /// No se toca lo que el usuario ya haya escrito a mano: solo se rellenan los campos que
    /// siguen vacíos o que se rellenaron en una pasada anterior de esto mismo.
    /// </summary>
    private async Task RellenarDesdeAsync(CampoVm origen)
    {
        if (Definicion.Ejecutable is not IRellenaCampos rellenador) return;
        if (EntornoSeleccionado is null) return;
        if (_rellenando) return;

        var valor = origen.Valor;
        if (valor.Length == 0) return;

        _rellenando = true;
        try
        {
            var cadena = _entornos.CadenaConexion(
                EntornoSeleccionado.Entorno, baseDatos: Definicion.BaseDatosAlternativa);

            var valores = await rellenador.RellenarAsync(cadena, origen.Clave, valor);

            foreach (var (clave, v) in valores)
            {
                var destino = Campos.FirstOrDefault(c => c.Clave == clave);
                if (destino is null || ReferenceEquals(destino, origen)) continue;
                if (!_rellenados.Contains(clave) && !destino.EstaVacio) continue;

                destino.Fijar(v);
                _rellenados.Add(clave);
            }
        }
        catch (Exception ex)
        {
            // El relleno es una comodidad: si la base no responde se sigue pudiendo escribir
            // todo a mano. No se bloquea el formulario por esto.
            ErrorRelleno = $"No se han podido traer los valores del producto: {ex.Message}";
        }
        finally
        {
            _rellenando = false;
            RevisarValidez();
        }
    }

    /// <summary>
    /// Recarga las listas que dependen de este campo. Es lo que llena el desplegable de
    /// clientes de pago al escribir el CIF: en el original había que darle a un botón «Buscar
    /// Cliente Pago», y si te lo saltabas el desplegable se quedaba vacío sin decir por qué.
    /// </summary>
    private async Task RecargarDependientesAsync(CampoVm origen)
    {
        if (EntornoSeleccionado is null) return;

        var dependientes = Campos
            .Where(c => c.Definicion.DependeDe == origen.Clave && c.Clave != origen.Clave)
            .ToList();

        if (dependientes.Count == 0) return;

        var cadena = _entornos.CadenaConexion(
            EntornoSeleccionado.Entorno, baseDatos: Definicion.BaseDatosAlternativa);

        foreach (var campo in dependientes)
        {
            await campo.CargarAsync(cadena, origen.Valor);
        }

        RevisarValidez();
    }

    private bool _rellenando;
    private readonly HashSet<string> _rellenados = new();

    [ObservableProperty]
    private string _errorRelleno = string.Empty;

    public Visibility VisibilidadErrorRelleno =>
        string.IsNullOrEmpty(ErrorRelleno) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnErrorRellenoChanged(string value) =>
        OnPropertyChanged(nameof(VisibilidadErrorRelleno));

    /// <summary>Campos propios de la operación, declarados en el catálogo.</summary>
    public ObservableCollection<CampoVm> Campos { get; } = new();

    public Visibility VisibilidadCampos =>
        Campos.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Carga las opciones de los campos de selección contra el entorno elegido. Se llama al
    /// abrir y cada vez que se cambia de entorno: los Id de agente de UAT no son los de
    /// Producción, así que arrastrar el valor escribiría un Id que apunta a otra cosa.
    /// </summary>
    public async Task CargarCamposAsync()
    {
        if (EntornoSeleccionado is null) return;
        if (!Campos.Any(c => c.Definicion.Tipo == TipoCampo.Seleccion)) return;

        var cadena = _entornos.CadenaConexion(
            EntornoSeleccionado.Entorno, baseDatos: Definicion.BaseDatosAlternativa);

        foreach (var campo in Campos)
        {
            await campo.CargarAsync(cadena);
        }
    }

    public DefinicionOperacion Definicion { get; }

    public string Titulo => Definicion.Nombre;
    public string Descripcion => Definicion.Descripcion;
    public string Seccion => Definicion.Seccion.ToString();

    // ============================================================
    // ENTORNO
    // ============================================================

    public ObservableCollection<EntornoOpcionVm> Entornos { get; }

    [ObservableProperty]
    private EntornoOpcionVm? _entornoSeleccionado;

    /// <summary>
    /// En las operaciones de escritura, Replica no se ofrece: aparece deshabilitada con su
    /// motivo en vez de desaparecer, para que se entienda por qué no está.
    /// </summary>
    private IEnumerable<EntornoOpcionVm> ConstruirEntornos()
    {
        IReadOnlyList<EntornoBD> todos;
        try
        {
            todos = _entornos.Cargar();
        }
        catch (ConfiguracionNoDisponibleException)
        {
            return Array.Empty<EntornoOpcionVm>();
        }

        return todos.Select(e => new EntornoOpcionVm
        {
            Entorno = e,
            Habilitado = !(Definicion.EsEscritura && e.SoloLectura),
            Motivo = e.SoloLectura ? "solo lectura" : ""
        });
    }

    partial void OnEntornoSeleccionadoChanged(EntornoOpcionVm? value)
    {
        // Se recargan las listas: dependen del entorno.
        _ = CargarCamposAsync();

        OnPropertyChanged(nameof(EsProduccion));
        OnPropertyChanged(nameof(ColorAccion));
        OnPropertyChanged(nameof(VisibilidadAvisoProduccion));
        OnPropertyChanged(nameof(VisibilidadAvisoConsultaProduccion));
    }

    public bool EsProduccion => EntornoSeleccionado?.Entorno.Clave == ClaveEntorno.Produccion;

    /// <summary>
    /// El botón de acción hereda el color del entorno cuando la operación escribe en
    /// Producción. En el resto de casos, el azul de marca.
    ///
    /// El ámbar de Producción tiene que seguir ganando al azul: es la única señal de color
    /// que avisa de que el botón va a escribir en la base de verdad.
    /// </summary>
    public Brush ColorAccion => Apariencia.Pincel(
        Definicion.EsEscritura && EsProduccion ? "ProBrush" : "MarcaBrush");

    public Visibility VisibilidadAvisoProduccion =>
        Definicion.EsEscritura && EsProduccion ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Consultar contra Producción es posible pero desaconsejable: se avisa.</summary>
    public Visibility VisibilidadAvisoConsultaProduccion =>
        !Definicion.EsEscritura && EsProduccion ? Visibility.Visible : Visibility.Collapsed;

    // ============================================================
    // LISTA PEGADA
    // ============================================================

    public IReadOnlyList<TipoLista> TiposLista { get; }

    public Visibility VisibilidadLista =>
        Definicion.PideLista ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Solo se muestra el selector si la operación admite más de un tipo.</summary>
    public Visibility VisibilidadSelectorTipo =>
        TiposLista.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

    public IReadOnlyList<TipoListaVm> Tipos { get; }

    [ObservableProperty]
    private TipoListaVm? _tipoSeleccionado;

    [ObservableProperty]
    private TipoLista _tipoLista;

    [ObservableProperty]
    private string _textoLista = string.Empty;

    partial void OnTipoSeleccionadoChanged(TipoListaVm? value)
    {
        if (value is not null) TipoLista = value.Tipo;
    }

    partial void OnTipoListaChanged(TipoLista value) => Reanalizar();

    partial void OnTextoListaChanged(string value) => Reanalizar();

    private ResultadoAnalisis _analisis = new();

    public string EtiquetaLista => AnalizadorEntradas.Etiqueta(TipoLista);

    public int Reconocidos => _analisis.Validos.Count;

    public string TextoRecuento
    {
        get
        {
            if (string.IsNullOrWhiteSpace(TextoLista)) return "Pega la lista aquí";

            var unidad = AnalizadorEntradas.Unidad(TipoLista, Reconocidos);
            var texto = $"{Reconocidos:N0} {unidad}";

            if (_analisis.Repetidos > 0) texto += $" · {_analisis.Repetidos} repetido{(_analisis.Repetidos == 1 ? "" : "s")}";
            if (_analisis.Descartados.Count > 0) texto += $" · {_analisis.Descartados.Count} descartado{(_analisis.Descartados.Count == 1 ? "" : "s")}";

            return texto;
        }
    }

    public Brush ColorRecuento => Apariencia.Pincel(
        Reconocidos > 0 ? "OkBrush" : string.IsNullOrWhiteSpace(TextoLista) ? "Ink4Brush" : "BadBrush");

    public Visibility VisibilidadDescartes =>
        _analisis.Descartados.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Se enseñan los primeros descartes, no todos: la lista puede ser enorme.</summary>
    public string TextoDescartes
    {
        get
        {
            if (_analisis.Descartados.Count == 0) return "";

            var muestra = string.Join(", ", _analisis.Descartados.Take(4));
            var resto = _analisis.Descartados.Count - Math.Min(4, _analisis.Descartados.Count);
            if (resto > 0) muestra += $" y {resto} más";

            return $"No encajan con el formato esperado ({AnalizadorEntradas.FormatoEsperado(TipoLista)}): {muestra}";
        }
    }

    /// <summary>
    /// Lo que va a recorrer el ejecutor. Normalmente es la lista pegada; en las operaciones que
    /// se alimentan de un Excel, una entrada por fila del fichero.
    /// </summary>
    public IReadOnlyList<string> Entradas =>
        _entradasExcel.Count > 0 ? _entradasExcel : _analisis.Validos;

    // ---------- Entradas que vienen del Excel ----------

    private IReadOnlyList<string> _entradasExcel = Array.Empty<string>();

    [ObservableProperty]
    private string _errorExcel = string.Empty;

    public Visibility VisibilidadErrorExcel =>
        string.IsNullOrEmpty(ErrorExcel) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnErrorExcelChanged(string value) =>
        OnPropertyChanged(nameof(VisibilidadErrorExcel));

    /// <summary>
    /// Cuenta de filas del Excel, para decirlo antes de lanzar. Vacío si la operación no se
    /// alimenta de un Excel o si todavía no hay fichero elegido.
    /// </summary>
    public string TextoFilasExcel =>
        _entradasExcel.Count == 0
            ? string.Empty
            : $"{_entradasExcel.Count:N0} {(_entradasExcel.Count == 1 ? "fila con datos" : "filas con datos")}";

    public Visibility VisibilidadFilasExcel =>
        _entradasExcel.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>
    /// Relee el Excel para saber cuántas filas trae. Se llama al elegir fichero, no al ejecutar,
    /// para que el recuento se vea antes de tocar nada.
    /// </summary>
    private void ReleerExcel()
    {
        _entradasExcel = Array.Empty<string>();
        ErrorExcel = string.Empty;

        if (Definicion.Ejecutable is IEntradasDesdeExcel fuente
            && !string.IsNullOrWhiteSpace(RutaExcel)
            && File.Exists(RutaExcel))
        {
            var lector = fuente;
            try
            {
                _entradasExcel = lector.LeerEntradas(RutaExcel);
                if (_entradasExcel.Count == 0)
                {
                    ErrorExcel = "El fichero no tiene ninguna fila con datos por debajo de la cabecera.";
                }
            }
            catch (Exception ex)
            {
                // Suele ser el propio Excel abierto en otra ventana, o un .xls antiguo.
                ErrorExcel = $"No se ha podido leer el fichero: {ex.Message}";
            }
        }

        OnPropertyChanged(nameof(Entradas));
        OnPropertyChanged(nameof(TextoFilasExcel));
        OnPropertyChanged(nameof(VisibilidadFilasExcel));
    }

    private void Reanalizar()
    {
        _analisis = AnalizadorEntradas.Analizar(TextoLista, TipoLista);

        OnPropertyChanged(nameof(EtiquetaLista));
        OnPropertyChanged(nameof(Reconocidos));
        OnPropertyChanged(nameof(TextoRecuento));
        OnPropertyChanged(nameof(ColorRecuento));
        OnPropertyChanged(nameof(VisibilidadDescartes));
        OnPropertyChanged(nameof(TextoDescartes));
        OnPropertyChanged(nameof(Entradas));
        RevisarValidez();
    }

    public void Limpiar() => TextoLista = string.Empty;

    // ============================================================
    // RESTO DE CAMPOS
    // ============================================================

    public Visibility VisibilidadFechas =>
        Definicion.Pide(EntradasOperacion.Fechas) ? Visibility.Visible : Visibility.Collapsed;

    [ObservableProperty]
    private DateTime _desde = DateTime.Today.AddMonths(-1);

    [ObservableProperty]
    private DateTime _hasta = DateTime.Today;

    partial void OnDesdeChanged(DateTime value) => RevisarValidez();
    partial void OnHastaChanged(DateTime value) => RevisarValidez();

    public Visibility VisibilidadGrupoTarifa =>
        Definicion.Pide(EntradasOperacion.GrupoTarifa) ? Visibility.Visible : Visibility.Collapsed;

    [ObservableProperty]
    private string _grupoTarifa = string.Empty;

    partial void OnGrupoTarifaChanged(string value) => RevisarValidez();

    // ------------------------------------------------------------
    // FILTRO POR EL GRUPO DE TARIFA ACTUAL
    //
    // Portado de UpdateContratoTarifa: no toca todos los contratos de la lista, solo los que
    // cumplen una de estas dos condiciones. En ActualizaPrecios esto era una casilla
    // «Personalizada» que habilitaba un campo; aquí son dos opciones excluyentes, porque es
    // lo que son y así se lee la regla.
    // ------------------------------------------------------------

    public Visibility VisibilidadFiltroActual =>
        Definicion.Pide(EntradasOperacion.FiltroTarifaActual) ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// True = solo contratos cuyo grupo actual contenga «personalizada». Es el valor por
    /// defecto, igual que en ActualizaPrecios.
    /// </summary>
    [ObservableProperty]
    private bool _soloPersonalizadas = true;

    [ObservableProperty]
    private string _grupoTarifaActual = string.Empty;

    partial void OnSoloPersonalizadasChanged(bool value)
    {
        OnPropertyChanged(nameof(FiltroPorGrupoActual));
        RevisarValidez();
    }

    partial void OnGrupoTarifaActualChanged(string value) => RevisarValidez();

    /// <summary>Lo contrario de <see cref="SoloPersonalizadas"/>, para el segundo radio.</summary>
    public bool FiltroPorGrupoActual
    {
        get => !SoloPersonalizadas;
        set => SoloPersonalizadas = !value;
    }

    public Visibility VisibilidadTexto =>
        Definicion.Pide(EntradasOperacion.Texto) ? Visibility.Visible : Visibility.Collapsed;

    public string EtiquetaTexto =>
        string.IsNullOrEmpty(Definicion.EtiquetaTexto) ? "Texto" : Definicion.EtiquetaTexto;

    [ObservableProperty]
    private string _texto = string.Empty;

    partial void OnTextoChanged(string value) => RevisarValidez();

    public Visibility VisibilidadExcel =>
        Definicion.Pide(EntradasOperacion.Excel) ? Visibility.Visible : Visibility.Collapsed;

    [ObservableProperty]
    private string _rutaExcel = string.Empty;

    partial void OnRutaExcelChanged(string value)
    {
        ReleerExcel();
        RevisarValidez();
    }

    public Visibility VisibilidadCarpeta =>
        Definicion.Pide(EntradasOperacion.Carpeta) ? Visibility.Visible : Visibility.Collapsed;

    [ObservableProperty]
    // Por defecto, Escritorio\ConsultasBO del usuario que ejecuta. Se resuelve por
    // usuario y por máquina, así que cada uno de los ocho encuentra lo suyo en su sitio.
    private string _carpetaDestino = RutasSalida.Predeterminada;

    partial void OnCarpetaDestinoChanged(string value) => RevisarValidez();

    public Visibility VisibilidadDividir =>
        Definicion.PermiteDividir ? Visibility.Visible : Visibility.Collapsed;

    [ObservableProperty]
    private bool _dividir;

    public string EtiquetaDividir =>
        $"Un fichero por cada {AnalizadorEntradas.Unidad(TipoLista, 1)}";

    // ============================================================
    // VALIDACIÓN Y ACCIÓN
    // ============================================================

    public string EtiquetaAccion
    {
        get
        {
            var basica = string.IsNullOrEmpty(Definicion.EtiquetaAccion) ? "Ejecutar" : Definicion.EtiquetaAccion;

            // El botón dice qué va a pasar, no solo «Ejecutar».
            if (Definicion.PideLista && Reconocidos > 0)
            {
                return $"{basica} · {Reconocidos:N0} {AnalizadorEntradas.Unidad(TipoLista, Reconocidos)}";
            }
            return basica;
        }
    }

    [ObservableProperty]
    private string _mensajeValidacion = string.Empty;

    public bool Completo => string.IsNullOrEmpty(MensajeValidacion);

    /// <summary>
    /// La operación no está portada todavía, así que el botón queda deshabilitado aunque el
    /// formulario esté completo. Mejor eso que un botón que no hace nada.
    /// </summary>
    public bool PuedeEjecutar => Completo && Definicion.Implementada;

    public Visibility VisibilidadPendiente =>
        Definicion.Implementada ? Visibility.Collapsed : Visibility.Visible;

    public Visibility VisibilidadValidacion =>
        Completo ? Visibility.Collapsed : Visibility.Visible;

    public Visibility VisibilidadRevisar =>
        Definicion.EsEscritura ? Visibility.Visible : Visibility.Collapsed;

    partial void OnMensajeValidacionChanged(string value)
    {
        OnPropertyChanged(nameof(Completo));
        OnPropertyChanged(nameof(PuedeEjecutar));
        OnPropertyChanged(nameof(VisibilidadValidacion));
    }

    /// <summary>
    /// Un solo mensaje con lo primero que falta, no una lista de siete errores: el usuario
    /// arregla de uno en uno de todos modos.
    /// </summary>
    public void RevisarValidez()
    {
        MensajeValidacion = PrimerProblema();
        OnPropertyChanged(nameof(EtiquetaAccion));
        OnPropertyChanged(nameof(EtiquetaDividir));
    }

    private string PrimerProblema()
    {
        if (EntornoSeleccionado is null)
            return "No hay ningún entorno configurado con el que ejecutar esto.";

        if (Definicion.PideLista && Reconocidos == 0)
            return $"Pega al menos un {AnalizadorEntradas.Unidad(TipoLista, 1)}.";

        if (Definicion.Pide(EntradasOperacion.GrupoTarifa) && string.IsNullOrWhiteSpace(GrupoTarifa))
            return "Falta el grupo de tarifa destino.";

        var incompleto = Campos.FirstOrDefault(c => !c.Completo);
        if (incompleto is not null)
        {
            // Si el campo sabe qué está mal —un numérico fuera de rango— se dice eso, que es
            // más útil que «falta tamaño» cuando el campo está relleno pero con algo inválido.
            return incompleto.Problema.Length > 0
                ? $"{incompleto.Etiqueta}: {incompleto.Problema}"
                : $"Falta {incompleto.Etiqueta.ToLowerInvariant()}.";
        }

        if (Definicion.Pide(EntradasOperacion.FiltroTarifaActual)
            && FiltroPorGrupoActual
            && string.IsNullOrWhiteSpace(GrupoTarifaActual))
            return "Indica el grupo de tarifa actual por el que filtrar, o vuelve a «solo personalizadas».";

        if (Definicion.Pide(EntradasOperacion.Texto) && string.IsNullOrWhiteSpace(Texto))
            return $"Falta {EtiquetaTexto.ToLowerInvariant()}.";

        if (Definicion.Pide(EntradasOperacion.Fechas) && Hasta < Desde)
            return "La fecha final es anterior a la inicial.";

        if (Definicion.Pide(EntradasOperacion.Excel) && string.IsNullOrWhiteSpace(RutaExcel))
            return "Elige el fichero de Excel.";

        if (ErrorExcel.Length > 0) return ErrorExcel;

        // Una operación que se alimenta del Excel sin filas no tiene nada que hacer.
        if (Definicion.Ejecutable is IEntradasDesdeExcel && Entradas.Count == 0)
            return "El fichero no tiene filas que procesar.";

        if (Definicion.Pide(EntradasOperacion.Carpeta) && string.IsNullOrWhiteSpace(CarpetaDestino))
            return "Elige la carpeta de destino.";

        return string.Empty;
    }

    /// <summary>
    /// Empaqueta lo que hay en el formulario para el ejecutor. La cadena de conexión se
    /// resuelve aquí y no antes: depende del entorno que el usuario haya elegido.
    /// </summary>
    public ContextoEjecucion ConstruirContexto(IReadOnlyList<string>? soloEstas = null)
    {
        var entorno = EntornoSeleccionado!.Entorno;

        return new ContextoEjecucion
        {
            Definicion = Definicion,
            Entorno = entorno,
            // Las curvas piden SigeTotalTM: mismo servidor y credenciales, otra base.
            CadenaConexion = _entornos.CadenaConexion(
                entorno, baseDatos: Definicion.BaseDatosAlternativa),
            Entradas = soloEstas ?? Entradas,
            TipoLista = TipoLista,
            Desde = Desde,
            Hasta = Hasta,
            Texto = Texto,
            GrupoTarifa = GrupoTarifa,
            GrupoTarifaActual = GrupoTarifaActual,
            SoloPersonalizadas = SoloPersonalizadas,
            CarpetaDestino = CarpetaDestino,
            RutaExcel = RutaExcel,
            Dividir = Dividir,
            Campos = Campos.ToDictionary(c => c.Clave, c => c.Valor)
        };
    }
}
