using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Modelos;

namespace SigeGestor.App.ViewModels;

/// <summary>
/// Una ejecución que está en marcha, o que ha acabado y todavía no se ha visto.
///
/// Guarda la página además del ViewModel: es lo que permite volver a ella. El armazón cambia
/// de pantalla con «AreaContenido.Content = otra», y los formularios de operación no se
/// guardaban en caché a propósito —entrar de nuevo debe dar un formulario limpio—, así que al
/// salir de una ejecución no quedaba nada que la referenciase. La tarea seguía corriendo, pero
/// no había manera de volver a verla.
/// </summary>
public sealed class TareaEnCurso
{
    public required string Operacion { get; init; }
    public required EjecucionViewModel Ejecucion { get; init; }

    /// <summary>Sección a la que pertenece. Para la fila de la rejilla de ejecuciones.</summary>
    public required string Grupo { get; init; }

    /// <summary>Entorno contra el que corre. Para el chip de la fila.</summary>
    public required ClaveEntorno Entorno { get; init; }

    /// <summary>La página a la que hay que volver para ver esta ejecución.</summary>
    public required object Pagina { get; init; }

    /// <summary>Cuándo se lanzó. Para ordenar y para decir «hace 3 min».</summary>
    public DateTime Momento { get; } = DateTime.Now;

    public bool Terminada => Ejecucion.Terminada;
}

/// <summary>
/// Lo que hay corriendo ahora mismo, para que la cabecera lo diga desde cualquier pantalla.
///
/// EL PROBLEMA QUE RESUELVE: lanzas una consulta de 200 CUPS, te vas a otra operación mientras
/// termina, y desde ese momento no sabes si sigue, si acabó o si falló. La ejecución no se
/// paraba —eso funcionaba— pero era invisible y no se podía volver a ella.
///
/// Se queda con las terminadas hasta que se ven, y no solo con las que corren: enterarse de que
/// acabó es tan necesario como saber que sigue.
/// </summary>
public sealed partial class EjecucionesEnCurso : ObservableObject
{
    private readonly List<TareaEnCurso> _tareas = new();

    /// <summary>Se pide volver a la página de una ejecución.</summary>
    public event EventHandler<TareaEnCurso>? VolverPedido;

    /// <summary>
    /// La lista ha cambiado: se ha añadido una, se ha quitado o una ha terminado.
    ///
    /// SEPARADO DEL PropertyChanged del progreso a propósito. El contador cambia en cada
    /// entrada procesada —200 veces en una consulta de 200 CUPS— y quien escucha esto rehace
    /// la rejilla de ejecuciones. Sin separarlo, la rejilla se reconstruiría 200 veces.
    /// </summary>
    public event EventHandler? ListaCambiada;

    public IReadOnlyList<TareaEnCurso> Tareas => _tareas;

    public void Anadir(TareaEnCurso tarea)
    {
        _tareas.Add(tarea);

        // El indicador se refresca con el progreso de la ejecución: Procesados cambia en cada
        // entrada, y es lo que hace que el contador de la cabecera avance solo.
        tarea.Ejecucion.PropertyChanged += Refrescar;

        Avisar();
        ListaCambiada?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Quita una ejecución del indicador. Se llama al volver a verla y al cerrar su página.
    /// </summary>
    public void Quitar(TareaEnCurso tarea)
    {
        if (!_tareas.Remove(tarea)) return;

        tarea.Ejecucion.PropertyChanged -= Refrescar;

        Avisar();
        ListaCambiada?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Quita las de una página que se está descartando, corran o no.</summary>
    public void QuitarDe(object pagina)
    {
        foreach (var t in _tareas.Where(t => ReferenceEquals(t.Pagina, pagina)).ToList())
        {
            Quitar(t);
        }
    }

    public void Volver(TareaEnCurso tarea) => VolverPedido?.Invoke(this, tarea);

    /// <summary>La más reciente, que es la que se nombra en el indicador.</summary>
    private TareaEnCurso? Ultima => _tareas.LastOrDefault();

    public Visibility Visibilidad =>
        _tareas.Count == 0 ? Visibility.Collapsed : Visibility.Visible;

    public bool AlgunaCorriendo => _tareas.Any(t => !t.Terminada);

    /// <summary>
    /// Lo que se lee en la cabecera. Con una, su nombre y por dónde va; con varias, cuántas.
    /// </summary>
    public string Titulo
    {
        get
        {
            var corriendo = _tareas.Count(t => !t.Terminada);

            if (_tareas.Count > 1)
            {
                return corriendo > 0
                    ? $"{corriendo:N0} de {_tareas.Count:N0} en marcha"
                    : $"{_tareas.Count:N0} terminadas";
            }

            var u = Ultima;
            if (u is null) return "";

            return u.Terminada ? $"{u.Operacion} · terminada" : u.Operacion;
        }
    }

    /// <summary>El contador, o el desglose si ya acabó. Vacío con varias.</summary>
    public string Detalle
    {
        get
        {
            if (_tareas.Count != 1) return "";

            var u = Ultima!;
            if (!u.Terminada) return u.Ejecucion.Contador;

            return u.Ejecucion.ResErrores > 0
                ? $"{u.Ejecucion.ResErrores:N0} con error"
                : "sin errores";
        }
    }

    /// <summary>
    /// Ámbar mientras corre, rojo si algo falló, verde si acabó bien. El ámbar es a propósito
    /// el mismo que avisa de Producción: significa «esto está pasando ahora».
    /// </summary>
    public Brush Color => Apariencia.Pincel(
        AlgunaCorriendo ? "WarnBrush"
        : _tareas.Any(t => t.Ejecucion.ResErrores > 0) ? "BadBrush"
        : "OkBrush");

    public string Ayuda => AlgunaCorriendo
        ? "Sigue en marcha. Pulsa para volver a verla."
        : "Ya ha terminado. Pulsa para ver el resultado.";

    private void Refrescar(object? emisor, PropertyChangedEventArgs e)
    {
        // Solo lo que cambia el indicador. Sin filtro, cada entrada procesada dispararía una
        // decena de avisos por las propiedades derivadas del panel.
        if (e.PropertyName is not (nameof(EjecucionViewModel.Contador)
                                   or nameof(EjecucionViewModel.Procesados)
                                   or nameof(EjecucionViewModel.Terminada))) return;

        Avisar();

        // Terminar SÍ es un cambio de lista: la rejilla tiene que cambiar la fila viva por la
        // del histórico, que es la que trae el resultado de verdad.
        if (e.PropertyName == nameof(EjecucionViewModel.Terminada))
        {
            ListaCambiada?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Avisar()
    {
        OnPropertyChanged(nameof(Tareas));
        OnPropertyChanged(nameof(Visibilidad));
        OnPropertyChanged(nameof(AlgunaCorriendo));
        OnPropertyChanged(nameof(Titulo));
        OnPropertyChanged(nameof(Detalle));
        OnPropertyChanged(nameof(Color));
        OnPropertyChanged(nameof(Ayuda));
    }
}
