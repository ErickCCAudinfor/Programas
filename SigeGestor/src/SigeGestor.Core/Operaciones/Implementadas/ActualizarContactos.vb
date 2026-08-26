Option Strict Off   ' Usa la clase portada.

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Actualiza email y teléfono o móvil de los contratos de un Excel de tres columnas:
    ''' CodContrato, Emails —varios separados por punto y coma— y TlfnoMovil.
    '''
    ''' El trabajo lo hace ActualizarEmailFromExcel, portada literal de ActualizaPrecios.
    '''
    ''' ES UNA SOLA OPERACIÓN, NO UNA POR FILA: el método portado recorre el Excel por dentro y
    ''' devuelve un contador. Partirlo por filas obligaría a reescribir sus 200 líneas de
    ''' casuística de contactos, que es justo lo que no interesa tocar. Lo que sí se hace es
    ''' recoger las incidencias que antes solo iban a un fichero y decirlas aquí.
    ''' </summary>
    Public Class ActualizarContactos
        Inherits OperacionUnica

        Protected Overrides Async Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                            avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            If Not IO.File.Exists(ctx.RutaExcel) Then
                Return ResultadoEntrada.Fallo("elige el fichero de Excel")
            End If

            avisar("Leyendo el Excel y actualizando contactos…")

            Dim actualizador As New ActualizarEmailFromExcel(ctx.CadenaConexion) With {
                .RutaExcel = ctx.RutaExcel
            }

            Dim actualizados = Await actualizador.ActualizarEmailFromExcelAsync().ConfigureAwait(False)

            Dim incidencias = actualizador.Incidencias.Count

            ' El contador que devuelve el método portado solo cuenta los EMAILS actualizados: el
            ' de teléfonos se cuenta aparte en una variable local que no sale del método. Así que
            ' aquí no se puede afirmar cuántos teléfonos se han tocado, y no se inventa: para eso
            ' está el fichero de incidencias, que sí lista línea a línea lo que ha pasado.
            Dim mensaje As String

            If actualizados = 0 AndAlso incidencias = 0 Then
                Return ResultadoEntrada.SinDatos("el Excel no tenía filas con datos")
            End If

            If actualizados = 0 Then
                mensaje = $"ningún email actualizado · {incidencias:N0} incidencias"
                If actualizador.RutaIncidencias.Length > 0 Then
                    mensaje &= $" en {actualizador.RutaIncidencias}"
                End If
                Return ResultadoEntrada.SinDatos(mensaje)
            End If

            mensaje = $"{actualizados:N0} emails actualizados"
            If incidencias > 0 Then
                mensaje &= $" · {incidencias:N0} incidencias"
                If actualizador.RutaIncidencias.Length > 0 Then
                    mensaje &= $" en {actualizador.RutaIncidencias}"
                End If
            End If

            Return ResultadoEntrada.ConDatos(actualizados, mensaje)

        End Function

    End Class

End Namespace
