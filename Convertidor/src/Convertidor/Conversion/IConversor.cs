namespace Convertidor.Conversion;

/// <summary>
/// Un formato al que se puede convertir.
///
/// La <see cref="Clave"/> es lo que une formatos de conversores distintos: un .docx y un .xlsx
/// ofrecen los dos "pdf", y así en la ventana sale un único botón PDF que vale para ambos,
/// aunque por debajo lo haga Word en un caso y Excel en el otro.
/// </summary>
public sealed record FormatoDestino(string Clave, string Nombre, string Extension);

/// <summary>
/// Lo que comparten todas las conversiones de una tanda. Hoy solo es la sesión de Office,
/// para abrir Word o Excel una vez por tanda y no una vez por archivo.
/// </summary>
public sealed class ContextoConversion : IDisposable
{
    public SesionOffice Office { get; } = new();

    public void Dispose() => Office.Dispose();
}

public interface IConversor
{
    /// <summary>Lo que se ve en la columna "Tipo" de la ventana.</summary>
    string Familia { get; }

    /// <param name="extension">En minúsculas y con punto: ".png".</param>
    bool Admite(string extension);

    /// <summary>Formatos a los que puede pasar este archivo en concreto.</summary>
    IReadOnlyList<FormatoDestino> Destinos(string ruta);

    /// <summary>Convierte y devuelve la ruta creada, que puede ser un archivo o una carpeta.</summary>
    string Convertir(string ruta, FormatoDestino destino, string carpetaSalida, ContextoConversion contexto);
}
