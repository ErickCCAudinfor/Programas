using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Contratos;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Un campo declarado en la definición, listo para pintar.
///
/// Los de selección cargan sus opciones de la base del entorno elegido, así que se recargan si
/// se cambia de entorno: los IdAgente de UAT no son los de Producción, y arrastrar el valor de
/// un entorno a otro escribiría un Id que apunta a otra cosa.
/// </summary>
public sealed partial class CampoVm : ObservableObject
{
    private readonly RepositorioListas _listas = new();

    public CampoVm(CampoOperacion definicion)
    {
        Definicion = definicion;
        _texto = definicion.ValorInicial;

        // Las casillas también respetan su valor inicial. Antes solo lo hacía el texto, así que
        // una casilla declarada con ValorInicial = "1" salía desmarcada y no había forma de
        // dejar activada por defecto una opción que sí lo estaba antes —los históricos de las
        // curvas—, salvo invirtiendo la etiqueta, que se lee mucho peor.
        _marcado = definicion.Tipo == TipoCampo.Booleano && definicion.ValorInicial == "1";
    }

    public CampoOperacion Definicion { get; }

    public string Clave => Definicion.Clave;
    public string Etiqueta => Definicion.Etiqueta;
    public string Ayuda => Definicion.Ayuda;
    public bool Requerido => Definicion.Requerido;

    public Visibility VisibilidadTexto =>
        Definicion.Tipo == TipoCampo.Texto ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadSeleccion =>
        Definicion.Tipo == TipoCampo.Seleccion ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadNumero =>
        Definicion.Tipo == TipoCampo.Numero ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadFichero =>
        Definicion.Tipo == TipoCampo.Fichero ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadImporte =>
        Definicion.Tipo == TipoCampo.Importe ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadBooleano =>
        Definicion.Tipo == TipoCampo.Booleano ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VisibilidadFecha =>
        Definicion.Tipo == TipoCampo.Fecha ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// En los booleanos la etiqueta va dentro de la casilla, no encima: una etiqueta arriba y
    /// una casilla vacía debajo se lee mal.
    /// </summary>
    public Visibility VisibilidadEtiquetaEncima =>
        Definicion.Tipo == TipoCampo.Booleano ? Visibility.Collapsed : Visibility.Visible;

    // ---------- Casilla ----------

    [ObservableProperty]
    private bool _marcado;

    partial void OnMarcadoChanged(bool value) => Cambiado?.Invoke(this, EventArgs.Empty);

    // ---------- Fecha ----------

    [ObservableProperty]
    private DateTime? _fecha;

    partial void OnFechaChanged(DateTime? value) => Cambiado?.Invoke(this, EventArgs.Empty);

    public Visibility VisibilidadAyuda =>
        string.IsNullOrEmpty(Ayuda) ? Visibility.Collapsed : Visibility.Visible;

    // ---------- Texto ----------

    [ObservableProperty]
    private string _texto = string.Empty;

    partial void OnTextoChanged(string value)
    {
        OnPropertyChanged(nameof(Problema));
        OnPropertyChanged(nameof(VisibilidadProblema));
        Cambiado?.Invoke(this, EventArgs.Empty);
    }

    // ---------- Selección ----------

    public ObservableCollection<OpcionLista> Opciones { get; } = new();

    [ObservableProperty]
    private OpcionLista? _opcion;

    partial void OnOpcionChanged(OpcionLista? value) => Cambiado?.Invoke(this, EventArgs.Empty);

    [ObservableProperty]
    private bool _cargandoOpciones;

    [ObservableProperty]
    private string _errorOpciones = string.Empty;

    public Visibility VisibilidadErrorOpciones =>
        string.IsNullOrEmpty(ErrorOpciones) ? Visibility.Collapsed : Visibility.Visible;

    partial void OnErrorOpcionesChanged(string value) =>
        OnPropertyChanged(nameof(VisibilidadErrorOpciones));

    /// <summary>Avisa al formulario de que hay que revalidar.</summary>
    public event EventHandler? Cambiado;

    /// <summary>
    /// Valor tal como llega al ejecutable: el texto, o el Id de la opción elegida. Cadena
    /// vacía cuando no hay nada, que en administrador significa «sin administrador».
    /// </summary>
    public string Valor
    {
        get
        {
            // La casilla viaja como 1/0 y la fecha como ISO, para que el Core no tenga que
            // adivinar el formato de la máquina de quien la escribió.
            if (Definicion.Tipo == TipoCampo.Booleano) return Marcado ? "1" : "0";
            if (Definicion.Tipo == TipoCampo.Fecha)
                return Fecha.HasValue ? Fecha.Value.ToString("yyyy-MM-dd") : string.Empty;

            if (Definicion.Tipo != TipoCampo.Seleccion) return Texto.Trim();
            if (Opcion is null) return string.Empty;

            // Los orígenes de configuración no tienen Id: lo que viaja es su Valor —el tipo de
            // XML manda «Raíz|Nodo»—. Con Id, 0 sigue significando «ninguno».
            if (!string.IsNullOrEmpty(Opcion.Valor)) return Opcion.Valor;
            return Opcion.Id == 0 ? string.Empty : Opcion.Id.ToString();
        }
    }

    /// <summary>
    /// Completo y, en los numéricos, además válido: un número dentro del rango declarado. Sin
    /// esto un «12a» llegaría al Core y el fallo se vería al ejecutar y no al rellenar.
    /// </summary>
    public bool Completo
    {
        get
        {
            // Una casilla siempre tiene valor: nunca está «incompleta».
            if (Definicion.Tipo == TipoCampo.Booleano) return true;

            if (Valor.Length > 0 && Problema.Length > 0) return false;
            return !Requerido || Valor.Length > 0;
        }
    }

    private bool NumeroValido => int.TryParse(Valor, out var n) && EnRango(n);

    private bool ImporteValido =>
        decimal.TryParse(Valor, NumberStyles.Number, CultureInfo.CurrentCulture, out var d)
        && EnRango(d);

    private bool EnRango(decimal n)
    {
        if (Definicion.Minimo == 0 && Definicion.Maximo == 0) return true;
        return n >= Definicion.Minimo && n <= Definicion.Maximo;
    }

    /// <summary>Qué falta o qué está mal, para decirlo junto al campo. Vacío si está bien.</summary>
    public string Problema
    {
        get
        {
            if (Valor.Length == 0) return string.Empty;

            if (Definicion.Tipo == TipoCampo.Numero && !NumeroValido)
            {
                return Definicion.Minimo != 0 || Definicion.Maximo != 0
                    ? $"Tiene que ser un número entre {Definicion.Minimo:N0} y {Definicion.Maximo:N0}."
                    : "Tiene que ser un número entero.";
            }

            if (Definicion.Tipo == TipoCampo.Importe && !ImporteValido)
            {
                return Definicion.Minimo != 0 || Definicion.Maximo != 0
                    ? $"Tiene que ser un importe entre {Definicion.Minimo:N0} y {Definicion.Maximo:N0}."
                    : "Tiene que ser un importe. Usa la coma para los decimales.";
            }

            if (Definicion.Tipo == TipoCampo.Fichero && !File.Exists(Valor))
            {
                return "Ese fichero ya no existe.";
            }

            return string.Empty;
        }
    }

    public Visibility VisibilidadProblema =>
        Problema.Length == 0 ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>
    /// Si el campo está sin tocar. Los booleanos nunca lo están —una casilla sin marcar es una
    /// decisión—, y por eso el relleno automático sí los sobrescribe la primera vez.
    /// </summary>
    public bool EstaVacio => Definicion.Tipo switch
    {
        TipoCampo.Booleano => true,
        TipoCampo.Fecha => !Fecha.HasValue,
        TipoCampo.Seleccion => Opcion is null,
        _ => Texto.Length == 0
    };

    /// <summary>
    /// Pone un valor en el campo desde fuera, en el mismo formato en que lo entrega
    /// <see cref="Valor"/>. Lo usa el relleno automático de la operación.
    /// </summary>
    public void Fijar(string valor)
    {
        switch (Definicion.Tipo)
        {
            case TipoCampo.Booleano:
                Marcado = valor == "1";
                break;

            case TipoCampo.Fecha:
                Fecha = DateTime.TryParse(valor, CultureInfo.InvariantCulture,
                                          DateTimeStyles.None, out var f) ? f : null;
                break;

            case TipoCampo.Seleccion:
                Opcion = Opciones.FirstOrDefault(
                    o => (!string.IsNullOrEmpty(o.Valor) ? o.Valor : o.Id.ToString()) == valor);
                break;

            default:
                Texto = valor;
                break;
        }
    }

    // ============================================================
    // FILTRAR POR EL SUMINISTRO DE LA LISTA
    // ============================================================

    /// <summary>
    /// Todas las opciones tal como vinieron de la base, sin filtrar. Se guardan aparte porque
    /// el filtro por suministro cambia con la lista pegada: si se hubiera recortado Opciones al
    /// cargar, al vaciar el cuadro habría que volver a preguntar a la base para recuperarlas.
    /// </summary>
    private IReadOnlyList<OpcionLista> _todas = Array.Empty<OpcionLista>();

    private TipoSuministro _suministro = TipoSuministro.SinResolver;

    /// <summary>
    /// Deja en el desplegable solo las opciones del suministro indicado.
    ///
    /// El Entorno de cada opción llega ya traducido a «luz» o «gas» desde la consulta de
    /// productos, así que aquí se compara con eso y no con G1/G2.
    ///
    /// Con SinResolver se muestran todas: es lo que pasa mientras se escribe, con una lista
    /// vacía, con contratos que no existen o si la base no responde. Con Mezclado no se muestra
    /// ninguna, porque no hay ninguna que valga para toda la lista.
    /// </summary>
    public void AplicarSuministro(TipoSuministro suministro)
    {
        if (!Definicion.FiltraPorSuministro) return;
        if (Definicion.Tipo != TipoCampo.Seleccion) return;
        if (_suministro == suministro) return;

        _suministro = suministro;
        Reconstruir();
    }

    /// <summary>
    /// Rellena Opciones desde _todas aplicando el filtro que haya.
    ///
    /// Lo llaman los dos caminos —cambiar de suministro y recargar de la base al cambiar de
    /// entorno— y por eso está en un solo sitio: la primera versión filtraba solo al cambiar de
    /// suministro, así que una recarga dejaba el desplegable con los productos de los dos
    /// entornos otra vez, y encima sin que se notara.
    /// </summary>
    private void Reconstruir()
    {
        // Lo elegido se conserva si sobrevive al filtro: cambiar de tipo de lista sin querer no
        // debería obligar a volver a buscar el producto.
        var elegido = Opcion;

        Opciones.Clear();

        // La opción de vaciar no lleva entorno y no se filtra nunca: significa «ninguno», y eso
        // vale igual para luz que para gas.
        if (Definicion.AdmiteVacio)
        {
            Opciones.Add(new OpcionLista { Id = 0, Nombre = Definicion.EtiquetaVacio });
        }

        foreach (var o in _todas.Where(o => Encaja(o, _suministro)))
        {
            Opciones.Add(o);
        }

        Opcion = elegido is not null && Opciones.Contains(elegido)
            ? elegido
            : (Definicion.AdmiteVacio && Opciones.Count > 0 ? Opciones[0] : null);

        OnPropertyChanged(nameof(SinOpcionesPorSuministro));
        Cambiado?.Invoke(this, EventArgs.Empty);
    }

    private static bool Encaja(OpcionLista o, TipoSuministro suministro) => suministro switch
    {
        TipoSuministro.Luz => string.Equals(o.Entorno, "luz", StringComparison.OrdinalIgnoreCase),
        TipoSuministro.Gas => string.Equals(o.Entorno, "gas", StringComparison.OrdinalIgnoreCase),
        TipoSuministro.Mezclado => false,
        _ => true
    };

    /// <summary>
    /// Se ha filtrado y no ha quedado ninguna. Lo explica quien pinta el formulario.
    ///
    /// Con un error de carga por delante esto es False: ahí el desplegable está vacío por otro
    /// motivo y quien manda es ErrorOpciones.
    /// </summary>
    public bool SinOpcionesPorSuministro =>
        Definicion.FiltraPorSuministro
        && ErrorOpciones.Length == 0
        && _todas.Count > 0
        && Opciones.Count == 0;

    /// <summary>
    /// Recarga las opciones para el entorno indicado. Los de texto no hacen nada.
    /// </summary>
    public async System.Threading.Tasks.Task CargarAsync(string cadenaConexion, string filtro = "")
    {
        if (Definicion.Tipo != TipoCampo.Seleccion) return;

        // Las opciones escritas en el catálogo no van a la base: se ponen y se acaba.
        if (Definicion.OpcionesFijas.Count > 0)
        {
            if (Opciones.Count > 0) return;   // ya están puestas: no se pierde la elección

            foreach (var fija in Definicion.OpcionesFijas)
            {
                // Id 0 y Valor con lo que toca viajar. La opción cuyo valor es cadena vacía
                // acaba significando «sin valor», que es justo «no tocar».
                Opciones.Add(new OpcionLista { Id = 0, Nombre = fija.Etiqueta, Valor = fija.Valor });
            }

            Opcion = Opciones[0];
            return;
        }

        CargandoOpciones = true;
        ErrorOpciones = string.Empty;
        Opciones.Clear();
        Opcion = null;

        try
        {
            _todas = await _listas.CargarAsync(cadenaConexion, Definicion.Origen, filtro: filtro);

            // Reconstruir pone la opción de vaciar, aplica el filtro por suministro si lo hay y
            // deja seleccionado lo que toque. Antes se hacía aquí a mano y el filtro se perdía
            // en cada recarga.
            Reconstruir();
        }
        catch (Exception ex)
        {
            // Se sueltan también las de la carga anterior. Si no, quedaban en _todas y
            // SinOpcionesPorSuministro daba verdadero —hay opciones guardadas y ninguna en
            // pantalla—, así que el formulario decía «no hay productos para este suministro»
            // cuando lo que había pasado es que la base no respondió. Dos motivos distintos con
            // el mismo aspecto es peor que no decir nada.
            _todas = Array.Empty<OpcionLista>();
            ErrorOpciones = $"No se han podido cargar las opciones: {ex.Message}";
        }
        finally
        {
            CargandoOpciones = false;
            Cambiado?.Invoke(this, EventArgs.Empty);
        }
    }
}
