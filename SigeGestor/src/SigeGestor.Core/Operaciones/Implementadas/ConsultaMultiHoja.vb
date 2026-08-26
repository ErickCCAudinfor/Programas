Imports System.Data
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>Una hoja del libro: su nombre y la plantilla que la llena.</summary>
    Public Class HojaConsulta
        Public Property Nombre As String = ""
        Public Property Plantilla As String = ""
    End Class

    ''' <summary>
    ''' Varias consultas en un mismo libro, cada una en su hoja. Es Clicks TODO, que entrega
    ''' un solo Excel con Luz y Gas.
    '''
    ''' Equivale a EjecutarHojas de ActualizaPrecios, con una diferencia: allí se abría y
    ''' guardaba el libro una vez por hoja, así que con dos hojas el fichero se escribía dos
    ''' veces. Aquí se consultan todas y se escribe el libro una sola vez.
    ''' </summary>
    Public Class ConsultaMultiHoja
        Inherits OperacionUnica

        Private ReadOnly _sql As RepositorioSql
        Private ReadOnly _nombreFichero As String
        Private ReadOnly _hojas As IReadOnlyList(Of HojaConsulta)
        Private ReadOnly _pideFechas As Boolean

        Public Sub New(sql As RepositorioSql,
                       nombreFichero As String,
                       hojas As IReadOnlyList(Of HojaConsulta),
                       Optional pideFechas As Boolean = False)
            _sql = sql
            _nombreFichero = nombreFichero
            _hojas = hojas
            _pideFechas = pideFechas
        End Sub

        Protected Overrides Async Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                            avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim tablas As New List(Of KeyValuePair(Of String, DataTable))
            Dim total As Long = 0

            Try
                For Each hoja In _hojas
                    ctx.AbortarSiCancelado()
                    avisar($"Consultando {hoja.Nombre}…")

                    Dim plantilla As New PlantillaSql(_sql.Obtener(hoja.Plantilla))
                    If _pideFechas Then
                        plantilla.PonerFecha("DesdeFechaReplace", ctx.Desde, ConsultaPorFechas.FormatoBarras)
                        plantilla.PonerFecha("hastaFechaReplace", ctx.Hasta, ConsultaPorFechas.FormatoBarras)
                    End If

                    Dim pendientes = plantilla.MarcadoresPendientes()
                    If pendientes.Count > 0 Then
                        Return ResultadoEntrada.Fallo(
                            $"Marcadores sin resolver en {hoja.Plantilla}.sql: {String.Join(", ", pendientes)}")
                    End If

                    Dim tabla As New DataTable()
                    Using conexion As New SqlConnection(ctx.CadenaConexion)
                        Await conexion.OpenAsync(ctx.Cancelacion).ConfigureAwait(False)
                        Using comando As New SqlCommand(plantilla.ToString(), conexion)
                            comando.CommandTimeout = 0
                            Using lector = Await comando.ExecuteReaderAsync(ctx.Cancelacion).ConfigureAwait(False)
                                tabla.Load(lector)
                            End Using
                        End Using
                    End Using

                    tablas.Add(New KeyValuePair(Of String, DataTable)(hoja.Nombre, tabla))
                    total += tabla.Rows.Count
                Next

                If total = 0 Then
                    Return ResultadoEntrada.SinDatos("Ninguna de las hojas ha devuelto filas")
                End If

                avisar($"Escribiendo {total:N0} filas en {_hojas.Count} hojas…")

                Dim ruta = EscritorExcel.EscribirVariasHojas(tablas, ctx.CarpetaDestino, _nombreFichero)
                Return ResultadoEntrada.ConDatos(total, $"{total:N0} filas en {IO.Path.GetFileName(ruta)}")

            Finally
                For Each par In tablas
                    par.Value.Dispose()
                Next
            End Try

        End Function

    End Class

End Namespace
