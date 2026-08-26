Imports System.Diagnostics

Namespace Operaciones

    ''' <summary>
    ''' Operación de un solo paso: no recorre una lista.
    '''
    ''' Es lo que necesitan las consultas que solo piden fechas — Hunosa, GAM, CAM… — donde no
    ''' hay entradas que iterar, solo una consulta que tarda y un Excel que escribir. Reportan
    ''' avance por mensaje («Consultando…», «Escribiendo 41.208 filas…») en lugar de por
    ''' contador, porque hasta que la consulta vuelve no hay nada que contar.
    ''' </summary>
    Public MustInherit Class OperacionUnica
        Implements IOperacionEjecutable

        ''' <summary>
        ''' Hace el trabajo. Puede llamar a <paramref name="avisar"/> tantas veces como quiera
        ''' para ir contando por dónde va.
        ''' </summary>
        Protected MustOverride Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                         avisar As Action(Of String)) As Task(Of ResultadoEntrada)

        Public Async Function EjecutarAsync(ctx As ContextoEjecucion,
                                            informar As Action(Of ProgresoOperacion)) _
            As Task(Of ResultadoOperacion) Implements IOperacionEjecutable.EjecutarAsync

            Dim reloj = Stopwatch.StartNew()
            Dim vivo As New List(Of LineaProgreso)
            Dim estados(0) As EstadoEntrada
            estados(0) = EstadoEntrada.EnCurso

            Dim avisar As Action(Of String) =
                Sub(mensaje)
                    informar(New ProgresoOperacion With {
                        .Procesados = 0,
                        .Total = 1,
                        .EntradaActual = mensaje,
                        .Transcurrido = reloj.Elapsed,
                        .Estados = DirectCast(estados.Clone(), EstadoEntrada()),
                        .Ultimas = vivo.ToList()
                    })
                End Sub

            avisar("Preparando…")

            Dim r As ResultadoEntrada
            Dim cancelada = False
            Try
                r = Await EjecutarUnaAsync(ctx, avisar).ConfigureAwait(False)
            Catch ex As OperationCanceledException
                cancelada = True
                r = ResultadoEntrada.Fallo("Cancelada")
            Catch ex As Exception
                r = ResultadoEntrada.Fallo(ex.Message)
            End Try

            reloj.Stop()
            r.Duracion = reloj.Elapsed
            estados(0) = r.Estado

            vivo.Add(New LineaProgreso With {
                .Momento = DateTime.Now,
                .Entrada = ctx.Definicion.Nombre,
                .Estado = r.Estado,
                .Registros = r.Registros,
                .Mensaje = r.Mensaje,
                .Duracion = r.Duracion
            })

            informar(New ProgresoOperacion With {
                .Procesados = 1,
                .Total = 1,
                .EntradaActual = r.Mensaje,
                .Registros = r.Registros,
                .Errores = If(r.Estado = EstadoEntrada.Fallo, 1, 0),
                .SinDatos = If(r.Estado = EstadoEntrada.SinDatos, 1, 0),
                .Transcurrido = reloj.Elapsed,
                .Estados = DirectCast(estados.Clone(), EstadoEntrada()),
                .Ultimas = vivo.ToList()
            })

            Return New ResultadoOperacion With {
                .Total = 1,
                .ConDatos = If(r.Estado = EstadoEntrada.ConDatos, 1, 0),
                .SinDatos = If(r.Estado = EstadoEntrada.SinDatos, 1, 0),
                .Errores = If(r.Estado = EstadoEntrada.Fallo AndAlso Not cancelada, 1, 0),
                .Registros = r.Registros,
                .Duracion = reloj.Elapsed,
                .Cancelada = cancelada,
                .Mensaje = r.Mensaje
            }

        End Function

    End Class

End Namespace
