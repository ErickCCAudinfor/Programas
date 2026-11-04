using System.ComponentModel;
using System.Runtime.CompilerServices;
using Convertidor.Conversion;

namespace Convertidor;

public abstract class Notificable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Cambiar<T>(ref T campo, T valor, [CallerMemberName] string? nombre = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return;
        campo = valor;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}

public enum EstadoArchivo { Pendiente, Convirtiendo, Hecho, Error, NoAdmitido }

/// <summary>Una fila de la lista de archivos.</summary>
public sealed class ElementoArchivo : Notificable
{
    public ElementoArchivo(string ruta)
    {
        Ruta = ruta;
        Nombre = Path.GetFileName(ruta);
        Familia = Catalogo.Familia(ruta) ?? "—";
        Destinos = Catalogo.Destinos(ruta);
        if (Destinos.Count == 0) Poner(EstadoArchivo.NoAdmitido, "Este tipo de archivo no se puede convertir");
    }

    public string Ruta { get; }
    public string Nombre { get; }
    public string Familia { get; }
    public IReadOnlyList<FormatoDestino> Destinos { get; }

    private EstadoArchivo _estado;
    public EstadoArchivo Estado { get => _estado; private set => Cambiar(ref _estado, value); }

    private string _mensaje = "";
    public string Mensaje { get => _mensaje; private set => Cambiar(ref _mensaje, value); }

    /// <summary>Lo último que se generó, para abrirlo con doble clic.</summary>
    public string? Resultado { get; set; }

    public void Poner(EstadoArchivo estado, string mensaje)
    {
        Estado = estado;
        Mensaje = mensaje;
    }
}

/// <summary>Un botón de la fila "Convertir a".</summary>
public sealed class OpcionFormato : Notificable
{
    public OpcionFormato(string clave, string nombre, int admiten, int total)
    {
        Clave = clave;
        Nombre = nombre;
        Admiten = admiten;
        Total = total;
    }

    public string Clave { get; }
    public string Nombre { get; }
    public int Admiten { get; }
    public int Total { get; }

    /// <summary>Si no vale para todos los archivos de la lista, se dice para cuántos.</summary>
    public string Texto => Admiten == Total ? Nombre : $"{Nombre}  ·  {Admiten} de {Total}";

    private bool _elegida;
    public bool Elegida { get => _elegida; set => Cambiar(ref _elegida, value); }
}

/// <summary>Una página en la columna de miniaturas de la ventana de recorte.</summary>
public sealed class Miniatura : Notificable
{
    public Miniatura(int pagina, double altoCaja)
    {
        Pagina = pagina;
        AltoCaja = altoCaja;
    }

    public int Pagina { get; }
    public string Texto => $"{Pagina + 1}";
    public double AltoCaja { get; }

    private System.Windows.Media.ImageSource? _imagen;
    public System.Windows.Media.ImageSource? Imagen { get => _imagen; set => Cambiar(ref _imagen, value); }
}
