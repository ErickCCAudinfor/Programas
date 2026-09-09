using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Configuracion;
using SigeGestor.Core.Contratos;
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

/// <summary>
/// Una columna del Excel de muestra: la tira vertical que se pinta con su letra, su título y
/// su valor de ejemplo, uno debajo de otro, como en la hoja de cálculo.
/// </summary>
public sealed class ColumnaExcelVm
{
    public ColumnaExcelVm(int indice, ColumnaExcel columna)
    {
        // A, B, C… Más de 26 columnas no las tiene ninguna operación —la mayor son 12— así que
        // no se hace la doble letra (AA, AB); si algún día hiciera falta, salta a la vista.
        Letra = ((char)('A' + indice)).ToString();
        Titulo = columna.Titulo;
        Ejemplo = columna.Ejemplo;
        Nota = columna.Nota;
        Obligatoria = columna.Obligatoria;
    }

    public string Letra { get; }
    public string Titulo { get; }
    public string Ejemplo { get; }
    public string Nota { get; }
    public bool Obligatoria { get; }

    /// <summary>
    /// La nota de esta columna, con su letra delante: «B · Emails: varios separados por punto
    /// y coma».
    ///
    /// SOLO se genera si la columna tiene algo que aclarar. Lo de «puede ir vacía» NO va aquí:
    /// en la importación de productos hay ocho columnas opcionales y salían ocho líneas
    /// idénticas que tapaban las notas que sí decían algo. Las opcionales se listan juntas en
    /// una sola línea, en NotasEsquema.
    /// </summary>
    public string Aclaracion =>
        Nota.Length == 0 ? string.Empty : $"{Letra} · {Titulo}: {Nota}";

    public Visibility VisibilidadAclaracion =>
        Aclaracion.Length == 0 ? Visibility.Collapsed : Visibility.Visible;
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
    private readonly RepositorioContratos _contratos = new();

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

    // ============================================================
    // LOS CAMPOS, A UNA O DOS COLUMNAS
    // ============================================================
    //
    // Masivo contrato tiene 31 campos —22 desplegables, 7 textos y 2 fechas— y en una sola
    // columna la tarjeta se iba muy por debajo del borde de la ventana: para llegar al último
    // había que desplazarse un buen rato, y no se podía ver de un golpe qué se está cambiando.
    // A dos columnas son 16 filas en vez de 31.
    //
    // EL UMBRAL ESTÁ MEDIDO, y el primer número que puse estaba mal. De las 46 operaciones solo
    // 11 tienen campos, y el reparto es 31 (Masivo contrato), 9 (Añadir productos), 3, 3 y siete
    // con uno solo. Puse 8, que metía a las dos grandes, y midiendo salió esto:
    //
    //                            1 columna                2 columnas
    //   Masivo contrato (31)     tarjeta 2.353 · pág 2.735   tarjeta 1.285 · pág 1.930   mejor
    //   Añadir productos (9)     tarjeta   644 · pág 1.009   tarjeta   551 · pág 1.178   PEOR
    //
    // Con nueve campos la tarjeta apenas baja —644 a 551, porque las ayudas debajo de cada campo
    // no se parten— y al ensancharla a 700 deja de caber al lado de la lista, así que el
    // WrapPanel la baja y eso cuesta 263 px. El remedio salía más caro que la enfermedad.
    //
    // Así que 20: solo Masivo contrato, que es la única donde la tarjeta es tan alta que partirla
    // compensa perder la colocación en paralelo.

    private const int CamposParaDosColumnas = 20;

    public bool DosColumnas => Campos.Count >= CamposParaDosColumnas;

    /// <summary>
    /// Ancho de la tarjeta de parámetros. Más ancha con dos columnas, porque partir los 490 en
    /// dos deja columnas de 225 y ahí los desplegables largos —los CNAE, los modelos de
    /// impresión— se cortan por la mitad y hay que adivinar el valor.
    /// </summary>
    public double AnchoParametros => DosColumnas ? 700 : 490;

    /// <summary>
    /// Ancho de la segunda columna: la mitad, o CERO cuando no hay segunda columna. Se enlaza
    /// directamente al ColumnDefinition, que acepta GridLength, en vez de montar dos bloques
    /// de XAML distintos.
    /// </summary>
    public GridLength AnchoColumnaDerecha =>
        DosColumnas ? new GridLength(1, GridUnitType.Star) : new GridLength(0);

    /// <summary>
    /// Los campos de la columna izquierda. Con una sola columna, todos.
    ///
    /// SE PARTE POR MITADES y no alternando uno a cada lado: los de Masivo contrato van
    /// agrupados por bloque —las dos fechas juntas, los cuatro datos del representante
    /// juntos— y repartirlos alternando rompería esos grupos. Partiendo por la mitad, cada
    /// columna se sigue leyendo de arriba abajo con sus bloques intactos.
    ///
    /// La izquierda se queda el de más cuando son impares, que es lo que hace que la columna
    /// más larga sea la primera y no la segunda.
    /// </summary>
    public IReadOnlyList<CampoVm> CamposIzquierda =>
        _izquierda ??= DosColumnas ? Campos.Take(Mitad).ToList() : Campos.ToList();

    public IReadOnlyList<CampoVm> CamposDerecha =>
        _derecha ??= DosColumnas ? Campos.Skip(Mitad).ToList() : Array.Empty<CampoVm>();

    // Se calculan UNA vez. Sin cachear devolvían una lista nueva en cada acceso: WPF se enlaza
    // a la primera y se queda con ella, así que funcionaba, pero cualquiera que las comparase
    // —o que las leyera dos veces— obtenía objetos distintos por el mismo dato. Campos se
    // rellena en el constructor y no cambia, así que no hay nada que invalidar.
    private IReadOnlyList<CampoVm>? _izquierda;
    private IReadOnlyList<CampoVm>? _derecha;

    private int Mitad => (Campos.Count + 1) / 2;

    /// <summary>
    /// Si la tarjeta de «Parámetros» tiene algo dentro.
    ///
    /// Hace falta desde que la lista y los parámetros se colocan uno al lado del otro: en las
    /// operaciones que solo piden la lista, la tarjeta salía con el título y nada debajo, y
    /// puesta al lado se veía como un hueco. Antes, apilada, se notaba menos, pero tampoco
    /// tenía sentido.
    ///
    /// Es el OR de las secciones que la tarjeta pinta. Dividir no cuenta: esa casilla vive en
    /// la tarjeta de la lista.
    /// </summary>
    public Visibility VisibilidadParametros =>
        VisibilidadCampos == Visibility.Visible
        || VisibilidadGrupoTarifa == Visibility.Visible
        || VisibilidadFiltroActual == Visibility.Visible
        || VisibilidadTexto == Visibility.Visible
        || VisibilidadFechas == Visibility.Visible
        || VisibilidadExcel == Visibility.Visible
        || VisibilidadCarpeta == Visibility.Visible
            ? Visibility.Visible
            : Visibility.Collapsed;

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

        // Y se vuelve a mirar de qué son los contratos: los códigos de UAT no son los de
        // Producción, así que el suministro de la lista puede ser distinto en cada entorno.
        DetectarSuministro();
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

        DetectarSuministro();
    }

    // ============================================================
    // LUZ O GAS: DE QUÉ SON LOS CONTRATOS PEGADOS
    // ============================================================
    //
    // Los maestros de SIGE están duplicados por entorno —los productos de luz en G1, los de gas
    // en G2— y no se pueden cruzar. Sin saber de qué es la lista, el desplegable traía los de
    // los dos y era fácil elegir uno que no valía para ningún contrato, sin enterarse hasta ver
    // el resultado.
    //
    // ActualizaPrecios miraba el entorno del PRIMER contrato y cargaba solo ese: con una lista
    // mezclada metía un producto de luz en contratos de gas sin decir nada. Aquí se mira la
    // lista entera y, si está mezclada, se dice y no se deja seguir.

    private CancellationTokenSource? _ctsSuministro;

    private TipoSuministro _suministro = TipoSuministro.SinResolver;

    /// <summary>Qué se está haciendo con el suministro, para decirlo debajo de la lista.</summary>
    [ObservableProperty] private string _mensajeSuministro = string.Empty;

    public Visibility VisibilidadMensajeSuministro =>
        string.IsNullOrEmpty(MensajeSuministro) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnMensajeSuministroChanged(string value) =>
        OnPropertyChanged(nameof(VisibilidadMensajeSuministro));

    /// <summary>Rojo cuando la lista está mezclada, que es lo que impide seguir.</summary>
    public Brush ColorSuministro =>
        Apariencia.Pincel(_suministro == TipoSuministro.Mezclado ? "BadBrush" : "Ink3Brush");

    /// <summary>
    /// Lanza la detección con retardo.
    ///
    /// EL RETARDO IMPORTA: esto va a la base, y sin él se consultaría en cada tecla mientras se
    /// escribe o se pega. Se espera a que la lista deje de cambiar y se cancela lo anterior, así
    /// que pegar 5.000 contratos son una consulta y no cinco mil.
    ///
    /// No se espera el resultado (fire and forget) a propósito: escribir en el cuadro no puede
    /// quedarse bloqueado esperando a SQL. Lo que llega, llega, y si no llega no se filtra.
    /// </summary>
    private void DetectarSuministro()
    {
        if (!Campos.Any(c => c.Definicion.FiltraPorSuministro)) return;

        _ctsSuministro?.Cancel();
        _ctsSuministro?.Dispose();
        _ctsSuministro = new CancellationTokenSource();

        _ = DetectarSuministroAsync(_ctsSuministro.Token);
    }

    private async Task DetectarSuministroAsync(CancellationToken ct)
    {
        try
        {
            await Task.Delay(450, ct).ConfigureAwait(true);

            var entradas = Entradas;

            if (entradas.Count == 0 || EntornoSeleccionado is null)
            {
                Fijar(TipoSuministro.SinResolver, string.Empty);
                return;
            }

            MensajeSuministro = "Comprobando si son de luz o de gas…";
            OnPropertyChanged(nameof(ColorSuministro));

            var cadena = _entornos.CadenaConexion(
                EntornoSeleccionado.Entorno, baseDatos: Definicion.BaseDatosAlternativa);

            var suministro = await _contratos
                .SuministroDeAsync(cadena, entradas, TipoLista, ct)
                .ConfigureAwait(true);

            ct.ThrowIfCancellationRequested();

            Fijar(suministro, suministro switch
            {
                TipoSuministro.Luz => "Los contratos son de luz: solo salen los productos de luz.",
                TipoSuministro.Gas => "Los contratos son de gas: solo salen los productos de gas.",
                TipoSuministro.Mezclado =>
                    "La lista tiene contratos de luz y de gas. Un producto es de uno de los dos, " +
                    "así que no se puede asignar a los dos a la vez: separa la lista y hazlo en " +
                    "dos veces.",
                _ => string.Empty
            });
        }
        catch (OperationCanceledException)
        {
            // La lista ha vuelto a cambiar: manda la detección nueva.
        }
        catch (Exception)
        {
            // Que no se pueda averiguar no bloquea nada: se muestran todos los productos y la
            // comprobación por contrato sigue impidiendo escribir donde no toca. No se pinta
            // error: el usuario no ha pedido esto, es una ayuda.
            Fijar(TipoSuministro.SinResolver, string.Empty);
        }
    }

    private void Fijar(TipoSuministro suministro, string mensaje)
    {
        _suministro = suministro;
        MensajeSuministro = mensaje;
        OnPropertyChanged(nameof(ColorSuministro));

        foreach (var campo in Campos)
        {
            campo.AplicarSuministro(suministro);
        }

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

    // ============================================================
    // CÓMO HAY QUE MONTAR EL EXCEL
    // ============================================================
    //
    // Estas operaciones leen POR POSICIÓN: la columna 1 es el contrato pase lo que pase, y el
    // título de la cabecera no se lee. Eso no estaba dicho en ninguna parte, así que quien
    // preparaba el fichero tenía que adivinar el orden —o preguntar— y un Excel con las
    // columnas cambiadas de sitio no da error: mete los datos donde no van.
    //
    // Se pinta como una hoja de cálculo, con la letra de cada columna y el número de fila, para
    // que se vea de un golpe qué va en cada sitio y que los datos empiezan en la fila 2.

    private EsquemaExcel? Esquema =>
        Definicion.Ejecutable is IEsquemaExcel con ? con.Esquema : null;

    public Visibility VisibilidadEsquema =>
        Esquema is null ? Visibility.Collapsed : Visibility.Visible;

    public IReadOnlyList<ColumnaExcelVm> ColumnasEsquema =>
        Esquema is null
            ? Array.Empty<ColumnaExcelVm>()
            : Esquema.Columnas.Select((c, i) => new ColumnaExcelVm(i, c)).ToList();

    /// <summary>Número de la fila de la cabecera, para la columna de números.</summary>
    public string FilaCabecera => Esquema is null ? "" : Esquema.FilasDeCabecera.ToString();

    /// <summary>Número de la primera fila con datos.</summary>
    public string FilaDatos => Esquema is null ? "" : Esquema.PrimeraFilaConDatos.ToString();

    /// <summary>
    /// El resumen de arriba: cuántas columnas, en qué hoja y desde qué fila se lee.
    /// </summary>
    public string ResumenEsquema
    {
        get
        {
            if (Esquema is null) return string.Empty;

            var partes = new List<string>
            {
                Redaccion.Cuenta(Esquema.Columnas.Count, "columna"),
                Esquema.Hoja.Length > 0
                    ? $"en una hoja llamada «{Esquema.Hoja}»"
                    : "en la primera hoja, se llame como se llame",
                $"datos desde la fila {Esquema.PrimeraFilaConDatos}"
            };

            return string.Join(" · ", partes);
        }
    }

    /// <summary>Las notas de las columnas que tienen algo que aclarar, y el aviso general.</summary>
    public IReadOnlyList<string> NotasEsquema
    {
        get
        {
            if (Esquema is null) return Array.Empty<string>();

            var columnas = ColumnasEsquema;

            var notas = columnas
                .Where(c => c.Aclaracion.Length > 0)
                .Select(c => c.Aclaracion)
                .ToList();

            // Las opcionales, en una sola línea. Una por línea eran ocho renglones iguales en
            // la importación de productos.
            var opcionales = columnas.Where(c => !c.Obligatoria).Select(c => c.Letra).ToList();
            if (opcionales.Count > 0)
            {
                notas.Add(opcionales.Count == 1
                    ? $"La columna {opcionales[0]} puede ir vacía."
                    : $"Pueden ir vacías: {string.Join(", ", opcionales)}.");
            }

            // Lo que más se equivoca: sobra una columna al final y se da por hecho que da error.
            notas.Add("Lo que haya en columnas de más se ignora, y la fila " +
                      $"{Esquema.FilasDeCabecera} no se lee: es la cabecera.");

            if (Esquema.Aviso.Length > 0) notas.Add(Esquema.Aviso);

            return notas;
        }
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

        // Va ANTES de mirar los campos: con la lista mezclada el desplegable de producto se
        // queda vacío, y decir «falta producto» mandaría a buscar algo que no puede existir.
        if (_suministro == TipoSuministro.Mezclado)
            return "La lista mezcla contratos de luz y de gas. Sepárala y hazlo en dos veces.";

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
