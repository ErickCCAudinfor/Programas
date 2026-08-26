Imports System.Data
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Consulta que solo necesita un rango de fechas y entrega un Excel.
    '''
    ''' Cubre de una vez las consultas que en ActualizaPrecios eran EjecutarSimple: Hunosa,
    ''' Cadasa, Quantum, GAM, CAM, Trébol Luz y Gas, Santa Lucía, JC Castilla-La Mancha y
    ''' Rechazos Veolia. Solo cambian el .sql, el formato de fecha y el nombre del fichero,
    ''' así que van como datos y no como diez clases iguales.
    ''' </summary>
    Public Class ConsultaPorFechas
        Inherits OperacionUnica

        ''' <summary>Formato de fecha que espera el .sql. No es el mismo en todos.</summary>
        Public Const FormatoBarras As String = "dd/MM/yyyy"
        Public Const FormatoCompacto As String = "yyyyMMdd"
        Public Const FormatoGuiones As String = "dd-MM-yyyy"

        Private ReadOnly _sql As RepositorioSql
        Private ReadOnly _plantilla As String
        Private ReadOnly _nombreFichero As String
        Private ReadOnly _nombreHoja As String
        Private ReadOnly _formatoFecha As String
        Private ReadOnly _pideFechas As Boolean
        Private ReadOnly _extra As Action(Of PlantillaSql, ContextoEjecucion)

        ''' <param name="plantilla">Nombre del .sql, sin extensión.</param>
        ''' <param name="pideFechas">False en las que no llevan rango, como Rechazos Veolia.</param>
        ''' <param name="extra">
        ''' Marcadores propios de esta consulta: el nombre de agente de Cuentas LB2B, la lista
        ''' de facturas de Activa y Reactiva… Se aplican después de las fechas.
        ''' </param>
        Public Sub New(sql As RepositorioSql,
                       plantilla As String,
                       nombreFichero As String,
                       nombreHoja As String,
                       Optional formatoFecha As String = FormatoBarras,
                       Optional pideFechas As Boolean = True,
                       Optional extra As Action(Of PlantillaSql, ContextoEjecucion) = Nothing)

            _sql = sql
            _plantilla = plantilla
            _nombreFichero = nombreFichero
            _nombreHoja = nombreHoja
            _formatoFecha = formatoFecha
            _pideFechas = pideFechas
            _extra = extra
        End Sub

        Protected Overrides Async Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                            avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim plantilla As New PlantillaSql(_sql.Obtener(_plantilla))

            If _pideFechas Then
                plantilla.PonerFecha("DesdeFechaReplace", ctx.Desde, _formatoFecha)
                plantilla.PonerFecha("hastaFechaReplace", ctx.Hasta, _formatoFecha)
                ' Norauto y alguna otra usan estos otros nombres para lo mismo.
                plantilla.PonerFecha("fechaReplace", ctx.Desde, _formatoFecha)
                plantilla.PonerFecha("fechahastaReplace", ctx.Hasta, _formatoFecha)
            End If

            _extra?.Invoke(plantilla, ctx)

            ' Si queda algún marcador sin sustituir, se para aquí: mandarlo a SQL Server daría
            ' un error de sintaxis incomprensible en vez de decir qué falta.
            Dim pendientes = plantilla.MarcadoresPendientes()
            If pendientes.Count > 0 Then
                Return ResultadoEntrada.Fallo(
                    $"La plantilla {_plantilla}.sql tiene marcadores sin resolver: {String.Join(", ", pendientes)}")
            End If

            avisar("Consultando…")

            Using tabla = Await ConsultarAsync(ctx, plantilla.ToString()).ConfigureAwait(False)

                If tabla.Rows.Count = 0 Then
                    Return ResultadoEntrada.SinDatos("La consulta no ha devuelto filas")
                End If

                avisar($"Escribiendo {tabla.Rows.Count:N0} filas en Excel…")

                Dim escrito = EscritorExcel.Escribir(tabla, ctx.CarpetaDestino, _nombreFichero, _nombreHoja)

                Dim resumen = If(escrito.Ficheros.Count = 1,
                                 $"{escrito.Filas:N0} filas en {IO.Path.GetFileName(escrito.Ficheros(0))}",
                                 $"{escrito.Filas:N0} filas en {escrito.Ficheros.Count} ficheros")

                Return ResultadoEntrada.ConDatos(escrito.Filas, resumen)
            End Using

        End Function

        ''' <summary>
        ''' Sin tiempo límite a propósito: hay consultas que tardan lo que tienen que tardar y
        ''' abortarlas solas no ayuda. Para cortar está Cancelar, que cierra la conexión.
        ''' </summary>
        Private Shared Async Function ConsultarAsync(ctx As ContextoEjecucion, sql As String) As Task(Of DataTable)

            Dim tabla As New DataTable()

            Using conexion As New SqlConnection(ctx.CadenaConexion)
                Await conexion.OpenAsync(ctx.Cancelacion).ConfigureAwait(False)

                Using comando As New SqlCommand(sql, conexion)
                    comando.CommandTimeout = 0

                    Using lector = Await comando.ExecuteReaderAsync(ctx.Cancelacion).ConfigureAwait(False)
                        tabla.Load(lector)
                    End Using
                End Using
            End Using

            Return tabla

        End Function

    End Class

End Namespace
