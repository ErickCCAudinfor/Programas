Imports System.Threading
Imports SigeGestor.Core.Modelos
Imports SigeGestor.Core.Registro

Namespace Servicios

    ''' <summary>
    ''' El histórico, servido desde los ficheros de log junto al ejecutable.
    '''
    ''' SOBRE LAS SERIES DE LOS SPARKLINE: el log se borra a los pocos días, así que no se
    ''' pueden dibujar ocho días de historia. Las series son 8 tramos de 3 horas sobre las
    ''' últimas 24 h, que cabe de sobra en la ventana de retención y además dice algo más
    ''' útil sobre el turno de hoy.
    ''' </summary>
    Public Class RepositorioEjecucionesLog
        Implements IRepositorioEjecuciones

        ''' <summary>Tramos del sparkline y horas que cubre cada uno.</summary>
        Private Const Tramos As Integer = 8
        Private Const HorasPorTramo As Integer = 3

        Private ReadOnly _registro As RegistroEjecuciones

        Public Sub New(registro As RegistroEjecuciones)
            _registro = registro
        End Sub

        Public Async Function ObtenerMetricasAsync(Optional ct As CancellationToken = Nothing) _
            As Task(Of MetricasInicio) Implements IRepositorioEjecuciones.ObtenerMetricasAsync

            ' Se lee desde ayer: hace falta para comparar hoy con ayer en los deltas.
            Dim ayer = Date.Today.AddDays(-1)
            Dim todas = Await _registro.LeerAsync(ayer, ct).ConfigureAwait(False)

            Dim deHoy = todas.Where(Function(e) e.Momento.Date = Date.Today).ToList()
            Dim deAyer = todas.Where(Function(e) e.Momento.Date = ayer).ToList()

            Return New MetricasInicio With {
                .Ejecuciones = New SerieMetrica With {
                    .Valor = deHoy.Count,
                    .ValorAnterior = deAyer.Count,
                    .Serie = SerieDe(todas, Function(grupo) grupo.Count)
                },
                .Filas = New SerieMetrica With {
                    .Valor = deHoy.Sum(Function(e) e.Registros),
                    .ValorAnterior = deAyer.Sum(Function(e) e.Registros),
                    .Serie = SerieDe(todas, Function(grupo) grupo.Sum(Function(e) e.Registros))
                },
                .ConError = New SerieMetrica With {
                    .Valor = ConteoConError(deHoy),
                    .ValorAnterior = ConteoConError(deAyer),
                    .Serie = SerieDe(todas, Function(grupo) ConteoConError(grupo))
                },
                .DuracionMedia = New SerieMetrica With {
                    .Valor = MediaSegundos(deHoy),
                    .ValorAnterior = MediaSegundos(deAyer),
                    .Serie = SerieDe(todas, Function(grupo) MediaSegundos(grupo))
                }
            }

        End Function

        ''' <summary>
        ''' Reparte las ejecuciones de las últimas 24 h en tramos de 3 h y aplica el
        ''' agregador a cada tramo. Los tramos sin nada valen cero, no se omiten: si no,
        ''' el sparkline mentiría sobre el eje temporal.
        ''' </summary>
        Private Shared Function SerieDe(todas As IEnumerable(Of Ejecucion),
                                        agregar As Func(Of IReadOnlyList(Of Ejecucion), Double)) As IReadOnlyList(Of Double)

            Dim ahora = DateTime.Now
            ' Se ancla al inicio de la hora en curso para que los tramos no se muevan
            ' con los segundos y el sparkline no tiemble entre dos cargas seguidas.
            Dim finVentana = New DateTime(ahora.Year, ahora.Month, ahora.Day, ahora.Hour, 0, 0).AddHours(1)
            Dim inicioVentana = finVentana.AddHours(-Tramos * HorasPorTramo)

            Dim lista = todas.Where(Function(e) e.Momento >= inicioVentana AndAlso e.Momento < finVentana).ToList()
            Dim serie(Tramos - 1) As Double

            For i = 0 To Tramos - 1
                Dim desde = inicioVentana.AddHours(i * HorasPorTramo)
                Dim hasta = desde.AddHours(HorasPorTramo)
                Dim delTramo = lista.Where(Function(e) e.Momento >= desde AndAlso e.Momento < hasta).ToList()
                serie(i) = agregar(delTramo)
            Next

            Return serie
        End Function

        ''' <summary>
        ''' Cuántas fallaron. Va en un método aparte porque en VB no se puede escribir
        ''' lista.Count(predicado): el compilador resuelve primero la propiedad Count de
        ''' List e intenta indexar un entero. Hay que pasar por Where.
        ''' </summary>
        Private Shared Function ConteoConError(ejecuciones As IEnumerable(Of Ejecucion)) As Double
            Return ejecuciones.Where(Function(e) e.Errores > 0).Count()
        End Function

        Private Shared Function MediaSegundos(ejecuciones As IReadOnlyList(Of Ejecucion)) As Double
            ' Las que están en curso no tienen duración todavía y falsearían la media.
            Dim terminadas = ejecuciones.Where(Function(e) e.Estado <> EstadoEjecucion.EnCurso).ToList()
            If terminadas.Count = 0 Then Return 0
            Return terminadas.Average(Function(e) e.Duracion.TotalSeconds)
        End Function

        Public Async Function ObtenerRecientesAsync(cuantas As Integer,
                                                    Optional ct As CancellationToken = Nothing) _
            As Task(Of IReadOnlyList(Of Ejecucion)) Implements IRepositorioEjecuciones.ObtenerRecientesAsync

            Dim desde = Date.Today.AddDays(-_registro.DiasRetencion)
            Dim todas = Await _registro.LeerAsync(desde, ct).ConfigureAwait(False)
            Return todas.Take(Math.Max(0, cuantas)).ToList()

        End Function

        Public Async Function ObtenerFrecuentesAsync(login As String,
                                                     cuantas As Integer,
                                                     Optional ct As CancellationToken = Nothing) _
            As Task(Of IReadOnlyList(Of OperacionFrecuente)) Implements IRepositorioEjecuciones.ObtenerFrecuentesAsync

            Dim desde = Date.Today.AddDays(-_registro.DiasRetencion)
            Dim todas = Await _registro.LeerAsync(desde, ct).ConfigureAwait(False)

            Dim mias = todas.Where(Function(e) String.Equals(e.Login, login, StringComparison.OrdinalIgnoreCase)).ToList()
            If mias.Count = 0 Then Return Array.Empty(Of OperacionFrecuente)()

            ' Se ordena por veces usada y, a igualdad, por la más reciente: con una ventana
            ' de tres días muchas operaciones empatan a una vez.
            Return mias.GroupBy(Function(e) e.Operacion, StringComparer.OrdinalIgnoreCase) _
                       .Select(Function(g)
                                   Dim ultima = g.OrderByDescending(Function(e) e.Momento).First()
                                   Return New OperacionFrecuente With {
                                       .Operacion = ultima.Operacion,
                                       .Grupo = ultima.Grupo,
                                       .Entorno = ultima.Entorno,
                                       .UltimaVez = ultima.Momento,
                                       .UltimoResultado = ResumenDe(ultima),
                                       .Veces = g.Count()
                                   }
                               End Function) _
                       .OrderByDescending(Function(o) o.Veces) _
                       .ThenByDescending(Function(o) o.UltimaVez) _
                       .Take(Math.Max(0, cuantas)) _
                       .ToList()

        End Function

        ''' <summary>Lo que se muestra en la tarjeta: «216 contratos», «54 ficheros».</summary>
        Private Shared Function ResumenDe(ejecucion As Ejecucion) As String
            If ejecucion.Errores > 0 Then
                Return $"{ejecucion.Errores:N0} con error"
            End If
            If ejecucion.Registros = 0 Then
                Return "sin resultados"
            End If
            Return $"{ejecucion.Registros:N0} {UnidadDe(ejecucion.Grupo)}"
        End Function

        Private Shared Function UnidadDe(grupo As String) As String
            Select Case grupo
                Case "Precios", "Contratos" : Return "contratos"
                Case "Productos" : Return "asignaciones"
                Case "Consultas" : Return "filas"
                Case "Facturas y PDF" : Return "ficheros"
                Case Else : Return "registros"
            End Select
        End Function

    End Class

End Namespace
