Imports System.Data
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Consulta que se lanza una vez por cada entrada de la lista: un CIF, un CUPS.
    '''
    ''' Cubre lo que en ActualizaPrecios eran EjecutarPorEntrada y EjecutarCurva. Con «un
    ''' fichero por entrada» marcado escribe un Excel por cada una; sin marcar, acumula todo
    ''' y escribe un único libro al final.
    '''
    ''' SOBRE LA MEMORIA: al acumular se guardan las filas en un solo DataTable y se escribe
    ''' una vez. En ActualizaPrecios este camino provocaba OutOfMemoryException con las curvas
    ''' y se resolvió llamando a GC.Collect() a mano en cada iteración. Aquí se libera cada
    ''' DataTable en cuanto se ha copiado, que es lo que de verdad hacía falta.
    ''' </summary>
    Public Class ConsultaPorLista
        Inherits OperacionPorEntrada

        Private ReadOnly _sql As RepositorioSql
        Private ReadOnly _plantilla As String
        Private ReadOnly _prefijoFichero As String
        Private ReadOnly _formatoFecha As String
        Private ReadOnly _marcadorEntrada As String
        Private ReadOnly _transformar As Func(Of String, String)

        ' Estado de la ejecución en curso. Se reinicia en PrepararAsync porque la misma
        ' instancia del catálogo se reutiliza en cada lanzamiento.
        Private _acumulado As DataTable
        Private ReadOnly _ficheros As New List(Of String)

        ''' <param name="marcadorEntrada">
        ''' Marcador del .sql donde va la entrada: identidadReplace, CIFReplace, cupsLikeReplace…
        ''' </param>
        ''' <param name="transformar">
        ''' Cómo se escribe la entrada dentro del SQL. Por defecto va tal cual, que es lo que
        ''' esperan los identidadReplace y CIFReplace. Las curvas necesitan
        ''' <see cref="ComoListaIn"/>.
        ''' </param>
        Public Sub New(sql As RepositorioSql,
                       plantilla As String,
                       prefijoFichero As String,
                       marcadorEntrada As String,
                       Optional formatoFecha As String = ConsultaPorFechas.FormatoBarras,
                       Optional transformar As Func(Of String, String) = Nothing)

            _sql = sql
            _plantilla = plantilla
            _prefijoFichero = prefijoFichero
            _marcadorEntrada = marcadorEntrada
            _formatoFecha = formatoFecha
            _transformar = transformar
        End Sub

        ''' <summary>
        ''' Un elemento de lista IN, entrecomillado y recortado a 20 caracteres.
        '''
        ''' Es lo que piden los .sql de las tres curvas: «left(cups,20) IN (joinCupsReplace)».
        ''' Se recorta porque el CUPS puede traer sufijo de punto de medida y el SQL compara
        ''' solo los 20 primeros caracteres.
        '''
        ''' OJO: en ActualizaPrecios esto está roto. El VB sustituye «cupsLikeReplace», que no
        ''' existe en ninguna plantilla, y deja «joinCupsReplace» literal, así que SQL Server
        ''' contesta «Invalid column name 'joinCupsReplace'». Alguien actualizó los .sql a la
        ''' versión particionada por meses sin tocar el código.
        ''' </summary>
        Public Shared Function ComoListaIn(entrada As String) As String
            Dim recortada = If(entrada.Length > 20, entrada.Substring(0, 20), entrada)
            ' Las comillas simples se doblan: el valor ya viene validado por el analizador,
            ' pero concatenar en SQL sin escapar no se hace nunca.
            Return $"N'{recortada.Replace("'", "''")}'"
        End Function

        Protected Overrides Function PrepararAsync(ctx As ContextoEjecucion) As Task
            _acumulado?.Dispose()
            _acumulado = Nothing
            _ficheros.Clear()
            Return Task.CompletedTask
        End Function

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim plantilla As New PlantillaSql(_sql.Obtener(_plantilla))

            plantilla.PonerFecha("DesdeFechaReplace", ctx.Desde, _formatoFecha)
            plantilla.PonerFecha("hastaFechaReplace", ctx.Hasta, _formatoFecha)
            plantilla.PonerFecha("fechaReplace", ctx.Desde, _formatoFecha)
            plantilla.PonerFecha("fechahastaReplace", ctx.Hasta, _formatoFecha)

            plantilla.Poner(_marcadorEntrada, If(_transformar Is Nothing, entrada, _transformar(entrada)))

            Dim pendientes = plantilla.MarcadoresPendientes()
            If pendientes.Count > 0 Then
                Return ResultadoEntrada.Fallo(
                    $"Marcadores sin resolver en {_plantilla}.sql: {String.Join(", ", pendientes)}")
            End If

            Dim tabla = Await ConsultarAsync(ctx, plantilla.ToString()).ConfigureAwait(False)

            Try
                If tabla.Rows.Count = 0 Then
                    Return ResultadoEntrada.SinDatos("sin datos")
                End If

                Dim filas = tabla.Rows.Count

                If ctx.Dividir Then
                    Dim nombre = If(String.IsNullOrEmpty(_prefijoFichero), entrada, $"{_prefijoFichero}_{entrada}")
                    Dim escrito = EscritorExcel.Escribir(tabla, ctx.CarpetaDestino, nombre, entrada)
                    _ficheros.AddRange(escrito.Ficheros)
                    Return ResultadoEntrada.ConDatos(filas, $"{filas:N0} filas")
                End If

                ' Acumular para un único libro al final
                If _acumulado Is Nothing Then _acumulado = tabla.Clone()
                For Each fila As DataRow In tabla.Rows
                    _acumulado.ImportRow(fila)
                Next

                Return ResultadoEntrada.ConDatos(filas, $"{filas:N0} filas")

            Finally
                ' Se suelta en cuanto se ha copiado: es lo que evita que 200 CUPS de curva
                ' acaben en OutOfMemory.
                tabla.Dispose()
            End Try

        End Function

        Protected Overrides Function CerrarAsync(ctx As ContextoEjecucion,
                                                 resultado As ResultadoOperacion) As Task

            If Not ctx.Dividir AndAlso _acumulado IsNot Nothing AndAlso _acumulado.Rows.Count > 0 Then
                Dim nombre = If(String.IsNullOrEmpty(_prefijoFichero), "Consulta", _prefijoFichero)
                Dim escrito = EscritorExcel.Escribir(_acumulado, ctx.CarpetaDestino, nombre, nombre)
                _ficheros.AddRange(escrito.Ficheros)
            End If

            _acumulado?.Dispose()
            _acumulado = Nothing

            If _ficheros.Count > 0 Then
                resultado.Mensaje = If(_ficheros.Count = 1,
                                       IO.Path.GetFileName(_ficheros(0)),
                                       $"{_ficheros.Count} ficheros en la carpeta")
            End If

            Return Task.CompletedTask

        End Function

        Private Shared Async Function ConsultarAsync(ctx As ContextoEjecucion, sql As String) As Task(Of DataTable)

            Dim tabla As New DataTable()

            Using conexion As New SqlConnection(ctx.CadenaConexion)
                Await conexion.OpenAsync(ctx.Cancelacion).ConfigureAwait(False)

                Using comando As New SqlCommand(sql, conexion)
                    ' Sin límite: las curvas de un mes tardan lo que tardan. Para cortar está
                    ' Cancelar, que cierra la conexión.
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
