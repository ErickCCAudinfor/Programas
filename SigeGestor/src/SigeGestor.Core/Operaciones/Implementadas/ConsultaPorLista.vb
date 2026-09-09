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

        ''' <summary>Marcador del .sql donde va el nombre de la tabla, una vez por bloque.</summary>
        Public Const MarcadorTabla As String = "tablaReplace"

        ''' <summary>Clave de la casilla «buscar también en los históricos».</summary>
        Public Const ClaveHistoricos As String = "historicos"

        Private ReadOnly _sql As RepositorioSql
        Private ReadOnly _plantilla As String
        Private ReadOnly _prefijoFichero As String
        Private ReadOnly _formatoFecha As String
        Private ReadOnly _marcadorEntrada As String
        Private ReadOnly _transformar As Func(Of String, String)
        Private ReadOnly _tablaBase As String
        Private ReadOnly _ordenarPor As String

        Private ReadOnly _tablasHistoricas As New TablasHistoricas

        ' Tablas resueltas para la ejecución en curso. Se buscan UNA VEZ en PrepararAsync: son
        ' las mismas para los 200 CUPS de la lista.
        Private _tablas As IReadOnlyList(Of String) = Array.Empty(Of String)()
        Private _avisoTablas As String = ""

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
        ''' <param name="tablaBase">
        ''' Tabla sobre la que va la consulta, cuando la plantilla es UN SOLO bloque con
        ''' <see cref="MarcadorTabla"/> en lugar del FROM. Estando puesto, el SQL se compone
        ''' repitiendo ese bloque —una vez por tabla— unido con UNION ALL, y los históricos se
        ''' buscan en la base en vez de venir escritos en el .sql. Vacío para las consultas
        ''' normales, que traen su FROM hecho.
        ''' </param>
        ''' <param name="ordenarPor">
        ''' Columna del ORDER BY final. Va aparte del .sql porque con los bloques unidos el
        ''' ORDER BY solo puede ir una vez, al final de todo.
        ''' </param>
        Public Sub New(sql As RepositorioSql,
                       plantilla As String,
                       prefijoFichero As String,
                       marcadorEntrada As String,
                       Optional formatoFecha As String = ConsultaPorFechas.FormatoBarras,
                       Optional transformar As Func(Of String, String) = Nothing,
                       Optional tablaBase As String = "",
                       Optional ordenarPor As String = "")

            _sql = sql
            _plantilla = plantilla
            _prefijoFichero = prefijoFichero
            _marcadorEntrada = marcadorEntrada
            _formatoFecha = formatoFecha
            _transformar = transformar
            _tablaBase = tablaBase
            _ordenarPor = ordenarPor
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

        Protected Overrides Async Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _acumulado?.Dispose()
            _acumulado = Nothing
            _ficheros.Clear()
            _tablas = Array.Empty(Of String)()
            _avisoTablas = ""

            If _tablaBase.Length = 0 Then Return

            ' La casilla del formulario. Si la operación no la declara, se buscan igual: es lo
            ' que hacían los .sql con su UNION ALL escrito a mano, y quitarlo por defecto
            ' devolvería de menos sin que nadie lo hubiera pedido.
            Dim mirarHistoricos = ctx.Campo(ClaveHistoricos) <> "0"

            If Not mirarHistoricos Then
                _tablas = {_tablaBase}
                Return
            End If

            Dim encontradas = Await _tablasHistoricas _
                .BuscarAsync(ctx.CadenaConexion, _tablaBase, ctx.Cancelacion) _
                .ConfigureAwait(False)

            If encontradas.Base.Length = 0 Then
                ' Se deja la principal igualmente: que falle la consulta diciendo que la tabla
                ' no existe es más claro que un «sin datos» de las 200 entradas.
                _tablas = {_tablaBase}
                _avisoTablas = $"no se ha encontrado la tabla {_tablaBase} en esta base"
                Return
            End If

            _tablas = encontradas.Todas
            _avisoTablas = encontradas.Aviso

        End Function

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim plantilla As New PlantillaSql(Componer())

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
                    Return ResultadoEntrada.ConDatos(filas, Redaccion.Cuenta(filas, "fila")) _
                        .Genera(escrito.Ficheros.ToArray())
                End If

                ' Acumular para un único libro al final
                If _acumulado Is Nothing Then _acumulado = tabla.Clone()
                For Each fila As DataRow In tabla.Rows
                    _acumulado.ImportRow(fila)
                Next

                Return ResultadoEntrada.ConDatos(filas, Redaccion.Cuenta(filas, "fila"))

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

            Dim partes As New List(Of String)

            If _ficheros.Count > 0 Then
                partes.Add(If(_ficheros.Count = 1,
                              IO.Path.GetFileName(_ficheros(0)),
                              Redaccion.Cuenta(_ficheros.Count, "fichero")))
                resultado.AnadirSalidas(_ficheros.ToArray())
            End If

            ' EN QUÉ TABLAS SE HA BUSCADO. Antes no se decía en ninguna parte: los históricos
            ' venían escritos en el .sql y no había forma de saber, viendo el resultado, si se
            ' habían mirado todos o si faltaba alguno. Que es justo lo que estaba pasando.
            If _tablas.Count > 1 Then
                partes.Add($"{Redaccion.Cuenta(_tablas.Count, "tabla")} " &
                           $"(la principal y {Redaccion.Cuenta(_tablas.Count - 1, "histórico", "históricos")})")
            ElseIf _tablas.Count = 1 AndAlso _tablaBase.Length > 0 Then
                partes.Add("solo la tabla principal")
            End If

            If _avisoTablas.Length > 0 Then partes.Add(_avisoTablas)

            resultado.Mensaje = Redaccion.Unir(partes.ToArray())

            Return Task.CompletedTask

        End Function

        ''' <summary>
        ''' El SQL de partida, con los marcadores de fecha y entrada todavía sin sustituir.
        '''
        ''' Sin tabla base es la plantilla tal cual, que ya trae su FROM. Con tabla base, el
        ''' bloque de la plantilla repetido una vez por tabla y unido con UNION ALL, y el ORDER
        ''' BY al final —solo puede ir una vez, después del último bloque—.
        '''
        ''' El marcador de la tabla acaba en «Replace» a propósito: si un bloque se quedara sin
        ''' sustituir, MarcadoresPendientes lo caza antes de mandar nada a SQL Server.
        ''' </summary>
        Private Function Componer() As String

            Dim texto = _sql.Obtener(_plantilla)

            If _tablaBase.Length = 0 Then Return texto

            ' Los comentarios de cabecera del .sql se sacan del bloque y se ponen UNA vez: si no,
            ' un CUPS con nueve tablas repetía nueve veces las seis líneas de explicación, y el
            ' SQL generado es justo lo que se mira cuando una consulta falla.
            Dim cabecera As String = Nothing
            Dim cuerpo = SinCabecera(texto, cabecera)

            Dim bloques = _tablas.Select(
                Function(t) New PlantillaSql(cuerpo).Poner(MarcadorTabla, t).ToString().Trim())

            Dim sql = String.Join($"{Environment.NewLine}UNION ALL{Environment.NewLine}", bloques)

            If cabecera.Length > 0 Then sql = cabecera & Environment.NewLine & sql

            If _ordenarPor.Length > 0 Then
                sql &= $"{Environment.NewLine}ORDER BY {_ordenarPor};"
            End If

            Return sql

        End Function

        ''' <summary>
        ''' Separa las líneas de comentario del principio del .sql —las que empiezan por «--» y
        ''' las vacías— del SQL de verdad. Devuelve el cuerpo y deja la cabecera en
        ''' <paramref name="cabecera"/>.
        ''' </summary>
        Friend Shared Function SinCabecera(texto As String, ByRef cabecera As String) As String

            Dim lineas = texto.Replace(vbCrLf, vbLf).Split(ChrW(10))
            Dim i = 0

            While i < lineas.Length
                Dim recortada = lineas(i).Trim()
                If recortada.Length > 0 AndAlso Not recortada.StartsWith("--") Then Exit While
                i += 1
            End While

            cabecera = String.Join(Environment.NewLine, lineas.Take(i)).TrimEnd()
            Return String.Join(Environment.NewLine, lineas.Skip(i)).Trim()

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
