using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using SigeGestor.Core.Operaciones;

namespace SigeGestor.App.ViewModels;

/// <summary>Una operación en la lista de resultados.</summary>
public sealed class ResultadoVm
{
    public required ResultadoBusqueda Resultado { get; init; }

    public DefinicionOperacion Definicion => Resultado.Definicion;

    public string Nombre => Definicion.Nombre;
    public string Motivo => Resultado.Motivo;
    public string Descripcion => Definicion.Descripcion;

    public Geometry Icono => Apariencia.Icono(Apariencia.ClaveIconoDeGrupo(Definicion.Seccion.ToString()));

    /// <summary>Se marcan las de escritura: importa saberlo antes de entrar.</summary>
    public Visibility VisibilidadEscribe =>
        Definicion.EsEscritura ? Visibility.Visible : Visibility.Collapsed;
}

/// <summary>
/// El buscador de la barra lateral.
///
/// Antes era un botón con «(en construcción)» en el tooltip: se pulsaba y no pasaba nada. Con
/// 45 operaciones en seis secciones, escribir dos letras es más rápido que recordar dónde
/// estaba cada cosa.
/// </summary>
public sealed partial class BuscadorViewModel : ObservableObject
{
    public ObservableCollection<ResultadoVm> Resultados { get; } = new();

    [ObservableProperty]
    private bool _abierto;

    partial void OnAbiertoChanged(bool value)
    {
        if (!value) return;

        // Al abrir se limpia: un buscador que recuerda lo anterior obliga a borrarlo antes de
        // escribir, y es lo primero que se hace siempre.
        Texto = string.Empty;
    }

    [ObservableProperty]
    private string _texto = string.Empty;

    partial void OnTextoChanged(string value) => Buscar();

    [ObservableProperty]
    private ResultadoVm? _seleccionado;

    private void Buscar()
    {
        Resultados.Clear();

        foreach (var r in Buscador.Buscar(Texto))
        {
            Resultados.Add(new ResultadoVm { Resultado = r });
        }

        // Se preselecciona el primero para que Intro funcione sin tocar las flechas.
        Seleccionado = Resultados.FirstOrDefault();

        OnPropertyChanged(nameof(VisibilidadResultados));
        OnPropertyChanged(nameof(VisibilidadSinResultados));
        OnPropertyChanged(nameof(VisibilidadAyuda));
        OnPropertyChanged(nameof(TextoSinResultados));
    }

    public Visibility VisibilidadResultados =>
        Resultados.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Se ha escrito algo y no encaja nada.</summary>
    public Visibility VisibilidadSinResultados =>
        Texto.Trim().Length > 0 && Resultados.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>Sin escribir nada: se dice qué se puede buscar en lugar de dejarlo vacío.</summary>
    public Visibility VisibilidadAyuda =>
        Texto.Trim().Length == 0 ? Visibility.Visible : Visibility.Collapsed;

    public string TextoSinResultados => $"Ninguna operación encaja con «{Texto.Trim()}».";

    /// <summary>Mueve la selección con las flechas sin salir de la caja de texto.</summary>
    public void Mover(int pasos)
    {
        if (Resultados.Count == 0) return;

        var actual = Seleccionado is null ? -1 : Resultados.IndexOf(Seleccionado);
        var siguiente = actual + pasos;

        // Se topa en los extremos en vez de dar la vuelta: con la lista a la vista, saltar del
        // final al principio desorienta más de lo que ayuda.
        siguiente = Math.Max(0, Math.Min(Resultados.Count - 1, siguiente));

        Seleccionado = Resultados[siguiente];
    }
}
