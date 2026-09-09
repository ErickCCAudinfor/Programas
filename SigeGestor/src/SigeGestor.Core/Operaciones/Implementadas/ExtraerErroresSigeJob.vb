Imports System.Data
Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Saca a Excel los mensajes de error de los trabajos de SigeJob indicados, leídos del XML
    ''' de resultado de cada paso. Es ConsultaExtraerErroresSigeJobbyId.sql.
    '''
    ''' UNA CONSULTA PARA TODOS, Y EL REPARTO EN CÓDIGO. Esta es la decisión que tiene chicha,
    ''' porque hay dos formas razonables de hacerlo y cada una pierde algo:
    '''
    '''   · Una consulta por trabajo, con ConsultaPorLista: se ve el resultado de cada uno, pero
    '''     son N idas a la base y N recorridos del CROSS APPLY sobre el XML, que es la parte
    '''     cara. Y si idsigejob no estuviera indexado en SigeStep, N recorridos completos.
    '''
    '''   · Un IN con todos: una sola ida y un solo recorrido, pero se pierde el detalle. Y en
    '''     una herramienta de diagnóstico, saber QUÉ TRABAJO NO TIENE ERRORES es justo la
    '''     información que se viene a buscar.
    '''
    ''' Así que se hace el IN —una consulta— y el reparto por trabajo se hace aquí con la
    ''' columna IdSigeJob, que para eso se añadió. Se gana lo de las dos: una sola consulta y
    ''' cada trabajo con su propio resultado en la pantalla.
    '''
    ''' La consulta se lanza en PrepararAsync, antes del bucle; ProcesarAsync ya no toca la base,
    ''' solo mira lo que trajo.
    ''' </summary>
    Public Class ExtraerErroresSigeJob
        Inherits OperacionPorEntrada

        Public Const Marcador As String = "idsSigeJobReplace"

        ''' <summary>
        ''' Se llama NombrePlantilla y no Plantilla porque VB no distingue mayúsculas: con ese
        ''' nombre chocaba con la variable local «plantilla» de PrepararAsync y la llamada
        ''' acababa resolviendo a la variable —de tipo PlantillaSql— en vez de a la constante.
        ''' </summary>
        Private Const NombrePlantilla As String = "ErroresSigeJob"

        ''' <summary>Nombre de la columna por la que se reparte. Ver el .sql.</summary>
        Private Const ColumnaTrabajo As String = "IdSigeJob"

        Private ReadOnly _sql As RepositorioSql

        Public Sub New(sql As RepositorioSql)
            _sql = sql
        End Sub

        ' Estado de la ejecución en curso. Se reinicia en PrepararAsync porque la instancia del
        ' catálogo se reutiliza en cada lanzamiento.
        Private _todos As DataTable
        Private _carpeta As String = ""
        Private ReadOnly _ficheros As New List(Of String)

        Protected Overrides Async Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _todos?.Dispose()
            _todos = Nothing
            _ficheros.Clear()

            _carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                          RutasSalida.Asegurar("ErroresSigeJob"),
                          ctx.CarpetaDestino)

            If ctx.Entradas.Count = 0 Then Return

            ' Los Id vienen del analizador, que solo deja pasar dígitos, así que el IN se compone
            ' sin comillas y sin escapar nada. Aun así se vuelve a filtrar: esta cadena acaba
            ' dentro del SQL y no se concatena nada que no se haya comprobado aquí mismo.
            Dim ids = ctx.Entradas.
                Select(Function(e) e.Trim()).
                Where(Function(e) e.Length > 0 AndAlso e.All(AddressOf Char.IsDigit)).
                Distinct().
                ToList()

            ' Ninguna entrada válida. NO se puede seguir y dejar que cada una salga como «sin
            ' errores»: eso es la conclusión contraria a la verdadera, y es justo lo que se ha
            ' arreglado ya en otras operaciones. Se aborta con el motivo.
            '
            ' Se comprueba ANTES de conectar: es gratis y no tiene sentido abrir una conexión
            ' para descubrir después que no hay nada que preguntar.
            If ids.Count = 0 Then
                Throw New InvalidOperationException(
                    "Ninguno de los identificadores es un número. El IdSigeJob son solo dígitos.")
            End If

            ' Helper.QuerySelect no lanza si no puede conectar, devuelve el error en el DataSet,
            ' y sin esto los N trabajos saldrían como «sin errores», que es exactamente la
            ' conclusión contraria a la verdadera.
            Dim problema = Await ctx.ProbarConexionAsync().ConfigureAwait(False)
            If problema.Length > 0 Then
                Dim donde = If(String.IsNullOrWhiteSpace(ctx.Entorno?.Nombre),
                               "la base de datos", ctx.Entorno.Nombre)
                Throw New InvalidOperationException($"No se puede conectar a {donde}: {problema}")
            End If

            Dim plantilla As New PlantillaSql(_sql.Obtener(NombrePlantilla))
            plantilla.Poner(Marcador, String.Join(",", ids))

            Dim pendientes = plantilla.MarcadoresPendientes()
            If pendientes.Count > 0 Then
                Throw New InvalidOperationException(
                    $"Marcadores sin resolver en {NombrePlantilla}.sql: {String.Join(", ", pendientes)}")
            End If

            Dim sql = plantilla.ToString()

            _todos = Await Task.Run(
                Function()
                    Dim resultado = Helper.QuerySelect(sql, ctx.CadenaConexion)

                    Dim errores = Helper.GetError(resultado)
                    If errores.HasError Then
                        Throw New InvalidOperationException(
                            $"La consulta ha fallado: {errores.DescripcionError}")
                    End If

                    If resultado.Tables.Count = 0 Then Return New DataTable()
                    Return resultado.Tables(0)
                End Function).ConfigureAwait(False)

        End Function

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim trabajo = entrada.Trim()

            ' Esta entrada no llegó al IN, así que no se ha preguntado por ella. Decir «sin
            ' errores» sería mentir: no se sabe nada de este trabajo.
            If trabajo.Length = 0 OrElse Not trabajo.All(AddressOf Char.IsDigit) Then
                Return Task.FromResult(ResultadoEntrada.Fallo("no es un IdSigeJob: solo dígitos"))
            End If

            If _todos Is Nothing OrElse _todos.Rows.Count = 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos("sin errores"))
            End If

            Dim suyas = Filas(trabajo)

            If suyas.Length = 0 Then
                ' Es un resultado bueno, no un fallo: el trabajo terminó sin incidencias de error.
                Return Task.FromResult(ResultadoEntrada.SinDatos("sin errores"))
            End If

            ' Con «un fichero por trabajo» marcado, cada uno sale en su Excel.
            If ctx.Dividir Then
                Using suyo = Recortar(suyas)
                    Dim escrito = EscritorExcel.Escribir(
                        suyo, _carpeta, $"ErroresSigeJob_{trabajo}", $"Job {trabajo}")

                    _ficheros.AddRange(escrito.Ficheros)

                    Return Task.FromResult(
                        ResultadoEntrada.ConDatos(
                            suyas.Length,
                            Redaccion.Cuenta(suyas.Length, "error", "errores")) _
                        .Genera(escrito.Ficheros.ToArray()))
                End Using
            End If

            Return Task.FromResult(ResultadoEntrada.ConDatos(
                suyas.Length, Redaccion.Cuenta(suyas.Length, "error", "errores")))

        End Function

        Protected Overrides Function CerrarAsync(ctx As ContextoEjecucion,
                                                 resultado As ResultadoOperacion) As Task

            Try
                ' Sin dividir, un único Excel con todo. Lleva la columna IdSigeJob, así que se
                ' puede filtrar por trabajo dentro del propio fichero.
                If Not ctx.Dividir AndAlso _todos IsNot Nothing AndAlso _todos.Rows.Count > 0 Then
                    Dim escrito = EscritorExcel.Escribir(
                        _todos, _carpeta, "ErroresSigeJob", "Errores")

                    _ficheros.AddRange(escrito.Ficheros)
                End If

                If _ficheros.Count = 0 Then
                    resultado.Mensaje = "Ninguno de los trabajos tiene errores"
                    Return Task.CompletedTask
                End If

                resultado.Mensaje = Redaccion.Unir(
                    If(_ficheros.Count = 1,
                       IO.Path.GetFileName(_ficheros(0)),
                       Redaccion.Cuenta(_ficheros.Count, "fichero")),
                    Redaccion.Cuenta(If(_todos?.Rows.Count, 0), "error", "errores"))

                resultado.AnadirSalidas(_ficheros.ToArray())

            Finally
                _todos?.Dispose()
                _todos = Nothing
            End Try

            Return Task.CompletedTask

        End Function

        ''' <summary>
        ''' Las filas de un trabajo. Se compara como TEXTO y no con Select("IdSigeJob = 123"):
        ''' el tipo de la columna lo decide el proveedor —puede venir Int32, Int64 o Decimal— y
        ''' una expresión de DataTable con el tipo equivocado lanza en tiempo de ejecución.
        ''' </summary>
        Private Function Filas(trabajo As String) As DataRow()

            If Not _todos.Columns.Contains(ColumnaTrabajo) Then Return Array.Empty(Of DataRow)()

            Return _todos.Rows.Cast(Of DataRow)().
                Where(Function(f) Not f.IsNull(ColumnaTrabajo) AndAlso
                                  Convert.ToString(f(ColumnaTrabajo)).Trim() = trabajo).
                ToArray()

        End Function

        ''' <summary>Una tabla nueva con solo esas filas, para escribirla aparte.</summary>
        Private Function Recortar(filas As DataRow()) As DataTable

            Dim recorte = _todos.Clone()
            For Each f In filas
                recorte.ImportRow(f)
            Next
            Return recorte

        End Function

    End Class

End Namespace
