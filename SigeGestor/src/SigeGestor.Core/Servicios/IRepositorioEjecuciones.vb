Imports System.Threading
Imports SigeGestor.Core.Modelos

Namespace Servicios

    ''' <summary>
    ''' Acceso al histórico de ejecuciones.
    '''
    ''' Sigue siendo interfaz aunque hoy solo haya una implementación
    ''' (<see cref="RepositorioEjecucionesLog"/>, sobre ficheros): si algún día el histórico
    ''' pasa a una tabla, se añade otra implementación y se cambia en un único sitio, el de
    ''' App.xaml.cs. Hubo una tercera en memoria para poder terminar el panel de inicio antes
    ''' de que existiera el log; se ha borrado al dejar de usarse.
    ''' </summary>
    Public Interface IRepositorioEjecuciones

        ''' <summary>Cifras de la cabecera del inicio.</summary>
        Function ObtenerMetricasAsync(Optional ct As CancellationToken = Nothing) As Task(Of MetricasInicio)

        ''' <summary>Últimas ejecuciones del equipo, de la más reciente a la más antigua.</summary>
        Function ObtenerRecientesAsync(cuantas As Integer,
                                       Optional ct As CancellationToken = Nothing) As Task(Of IReadOnlyList(Of Ejecucion))

        ''' <summary>
        ''' Operaciones que más usa una persona. Es lo que hace que ocho perfiles distintos
        ''' entren cada uno directo a lo suyo.
        ''' </summary>
        Function ObtenerFrecuentesAsync(login As String,
                                        cuantas As Integer,
                                        Optional ct As CancellationToken = Nothing) As Task(Of IReadOnlyList(Of OperacionFrecuente))

    End Interface

End Namespace
