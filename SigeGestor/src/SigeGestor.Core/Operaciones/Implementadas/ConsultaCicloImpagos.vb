Imports System.Data
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' En qué punto del ciclo de impagos está cada factura: OPEN ITEMS, devoluciones, carta
    ''' de aviso, carta de corte, remesado o cobrado. Y si saldría en Comunicados Impago para
    ''' crear cada carta; y si no, por qué.
    '''
    ''' Es Consulta_Ciclo_Impagos_Ver.sql, de Erick. Solo lectura: SELECT y tablas
    ''' temporales.
    '''
    ''' LA LISTA VA COMO PARÁMETRO, no concatenada en el texto. Es la diferencia con las
    ''' demás consultas por lista, que sustituyen un marcador: aquí son identificadores de
    ''' factura que escribe una persona, y aunque el cuadro de entrada ya los valida, un
    ''' parámetro no hay forma de que se rompa. El cuadro de facturas hace la limpieza
    ''' —líneas, espacios, comillas, comas, duplicadas— así que aquí llegan listas.
    '''
    ''' DEVUELVE DOS RESULTADOS y por eso no vale ConsultaMultiHoja, que lanza una consulta
    ''' por hoja: aquí las dos salen de la misma ejecución, que además comparte las tablas
    ''' temporales. El libro lleva la hoja «Ciclo» con el detalle y «Resumen» con el recuento
    ''' por situación.
    ''' </summary>
    Public Class ConsultaCicloImpagos
        Inherits OperacionUnica

        Private Const Plantilla As String = "ConsultaCicloImpagos"
        Private Const NombreFichero As String = "CicloImpagos"

        Private ReadOnly _sql As RepositorioSql

        Public Sub New(sql As RepositorioSql)
            _sql = sql
        End Sub

        Protected Overrides Async Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                            avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim facturas = ctx.Entradas
            If facturas Is Nothing OrElse facturas.Count = 0 Then
                Return ResultadoEntrada.Fallo("No hay ninguna factura en la lista")
            End If

            Dim texto = _sql.Obtener(Plantilla)

            ' La plantilla no lleva marcadores: la lista entra por parámetro. Se comprueba
            ' igual, por si alguien añade uno al .sql y se olvida de rellenarlo.
            Dim pendientes = New PlantillaSql(texto).MarcadoresPendientes()
            If pendientes.Count > 0 Then
                Return ResultadoEntrada.Fallo(
                    $"Marcadores sin resolver en {Plantilla}.sql: {String.Join(", ", pendientes)}")
            End If

            Dim ciclo As New DataTable("Ciclo")
            Dim resumen As New DataTable("Resumen")

            Try
                avisar($"Consultando {Redaccion.Cuenta(facturas.Count, "factura")}…")

                Using conexion As New SqlConnection(ctx.CadenaConexion)
                    Await conexion.OpenAsync(ctx.Cancelacion).ConfigureAwait(False)

                    Using comando As New SqlCommand(texto, conexion)
                        comando.CommandTimeout = 0

                        ' Separadas por «;», que es por donde parte el STRING_SPLIT de la
                        ' plantilla. Vienen ya sin espacios ni comillas del cuadro de entrada.
                        comando.Parameters.Add("@Lista", SqlDbType.NVarChar, -1).Value =
                            String.Join(";", facturas)

                        Using lector = Await comando.ExecuteReaderAsync(ctx.Cancelacion).ConfigureAwait(False)
                            ciclo.Load(lector)                 ' Load avanza al siguiente resultado
                            resumen.Load(lector)
                        End Using
                    End Using
                End Using

                If ciclo.Rows.Count = 0 Then
                    Return ResultadoEntrada.SinDatos("Ninguna de esas facturas existe")
                End If

                avisar($"Escribiendo {Redaccion.Cuenta(ciclo.Rows.Count, "fila")}…")

                Dim hojas As New List(Of KeyValuePair(Of String, DataTable)) From {
                    New KeyValuePair(Of String, DataTable)("Ciclo", ciclo),
                    New KeyValuePair(Of String, DataTable)("Resumen", resumen)
                }

                Dim ruta = EscritorExcel.EscribirVariasHojas(hojas, ctx.CarpetaDestino, NombreFichero)

                Return ResultadoEntrada.ConDatos(ciclo.Rows.Count, Titular(resumen, ciclo.Rows.Count)).
                    Genera(ruta)

            Finally
                ciclo.Dispose()
                resumen.Dispose()
            End Try

        End Function

        ''' <summary>
        ''' El reparto por situación, en una línea.
        '''
        ''' Es lo que se ve en el panel al acabar, y para tres o cuatro facturas suele ser
        ''' toda la respuesta: así no hay que abrir el Excel para saber que una está en
        ''' CARTA_AVISO y otra sin cartera.
        ''' </summary>
        Private Shared Function Titular(resumen As DataTable, filas As Long) As String

            If resumen Is Nothing OrElse resumen.Rows.Count = 0 Then
                Return Redaccion.Cuenta(filas, "cartera")
            End If

            Dim partes = resumen.Rows.Cast(Of DataRow)().
                Select(Function(f) $"{f("Carteras")} {Convert.ToString(f("Situacion"))}")

            Return $"{Redaccion.Cuenta(filas, "cartera")} · " & String.Join(" · ", partes)

        End Function

    End Class

End Namespace
