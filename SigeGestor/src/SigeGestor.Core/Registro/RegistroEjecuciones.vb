Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Threading
Imports SigeGestor.Core.Modelos

Namespace Registro

    ''' <summary>
    ''' El histórico de ejecuciones, en ficheros de log junto al ejecutable. Sin tabla en base
    ''' de datos.
    '''
    ''' CÓMO QUEDA COMPARTIDO ENTRE EL EQUIPO: el .exe vive en el servidor .13 y las ocho
    ''' personas lo abren desde ahí, así que AppContext.BaseDirectory apunta al .13 para todas.
    ''' Es el mismo mecanismo por el que hoy se comparte el Usuarios.json de ActualizaPrecios.
    '''
    ''' UN FICHERO POR MÁQUINA Y DÍA, no uno común: ocho procesos de equipos distintos
    ''' añadiendo líneas al mismo fichero por SMB es justo donde los apéndices se entrelazan y
    ''' aparecen líneas partidas. Escribiendo cada uno el suyo no hay bloqueos que gestionar, y
    ''' la retención se reduce a borrar ficheros por fecha.
    '''
    ''' Formato JSON Lines: una ejecución por línea. Es apéndice puro — nunca hay que releer y
    ''' reescribir el fichero — así que una escritura a medias solo puede estropear su propia
    ''' línea, y al leer se descarta.
    ''' </summary>
    Public Class RegistroEjecuciones

        Public Const NombreCarpeta As String = "Logs"
        Private Const Prefijo As String = "ejecuciones-"
        Private Const Extension As String = ".jsonl"

        ''' <summary>Días de log que se conservan. Lo de más se borra al arrancar.</summary>
        Public Property DiasRetencion As Integer = 3

        Private Shared ReadOnly OpcionesJson As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True,
            .WriteIndented = False
        }

        Private ReadOnly _equipo As String = SanearParaNombre(Environment.MachineName)

        Public ReadOnly Property Carpeta As String
            Get
                Return Path.Combine(AppContext.BaseDirectory, NombreCarpeta)
            End Get
        End Property

        ''' <summary>Fichero que le toca a esta máquina para un día dado.</summary>
        Private Function RutaDe(dia As Date) As String
            Return Path.Combine(Carpeta, $"{Prefijo}{dia:yyyyMMdd}-{_equipo}{Extension}")
        End Function

        ' ============================================================
        ' ESCRITURA
        ' ============================================================

        ''' <summary>
        ''' Añade una ejecución al log. No lanza nunca: que falle el registro no debe tumbar
        ''' la operación que el usuario acaba de completar. Devuelve False si no pudo escribir.
        ''' </summary>
        Public Async Function RegistrarAsync(ejecucion As Ejecucion,
                                             Optional ct As CancellationToken = Nothing) As Task(Of Boolean)

            Try
                Directory.CreateDirectory(Carpeta)

                Dim linea As New LineaRegistro With {
                    .Id = ejecucion.Id,
                    .Momento = ejecucion.Momento,
                    .Operacion = ejecucion.Operacion,
                    .Grupo = ejecucion.Grupo,
                    .Detalle = ejecucion.Detalle,
                    .NombreUsuario = ejecucion.NombreUsuario,
                    .Login = ejecucion.Login,
                    .Entorno = CInt(ejecucion.Entorno),
                    .Estado = CInt(ejecucion.Estado),
                    .Registros = ejecucion.Registros,
                    .Errores = ejecucion.Errores,
                    .Segundos = Math.Round(ejecucion.Duracion.TotalSeconds, 1),
                    .Equipo = Environment.MachineName
                }

                Dim texto = JsonSerializer.Serialize(linea, OpcionesJson) & Environment.NewLine
                Dim bytes = Encoding.UTF8.GetBytes(texto)

                ' FileShare.Read para que otro proceso pueda estar leyendo el log mientras
                ' escribimos. Un reintento corto cubre el caso de que la propia máquina tenga
                ' dos instancias abiertas.
                ' El Await de la espera va fuera del Catch: VB no permite Await dentro de un
                ' bloque Catch, a diferencia de C#.
                For intento = 1 To 3
                    Dim reintentar = False
                    Try
                        Using fs As New FileStream(RutaDe(ejecucion.Momento.Date),
                                                   FileMode.Append, FileAccess.Write, FileShare.Read,
                                                   bufferSize:=4096, useAsync:=True)
                            Await fs.WriteAsync(bytes, 0, bytes.Length, ct).ConfigureAwait(False)
                            Await fs.FlushAsync(ct).ConfigureAwait(False)
                        End Using
                        Return True
                    Catch ex As IOException When intento < 3
                        reintentar = True
                    End Try

                    If reintentar Then
                        Await Task.Delay(60 * intento, ct).ConfigureAwait(False)
                    End If
                Next

                Return False

            Catch ex As OperationCanceledException
                Throw
            Catch ex As Exception
                Return False
            End Try

        End Function

        ' ============================================================
        ' LECTURA
        ' ============================================================

        ''' <summary>
        ''' Todas las ejecuciones registradas desde una fecha, de la más reciente a la más
        ''' antigua, juntando los ficheros de todas las máquinas.
        '''
        ''' Las líneas ilegibles se saltan en silencio: un log a medio escribir es normal si
        ''' alguien está lanzando algo ahora mismo, y no es motivo para no mostrar el resto.
        ''' </summary>
        Public Async Function LeerAsync(desde As DateTime,
                                        Optional ct As CancellationToken = Nothing) As Task(Of IReadOnlyList(Of Ejecucion))

            If Not Directory.Exists(Carpeta) Then
                Return Array.Empty(Of Ejecucion)()
            End If

            Dim resultado As New List(Of Ejecucion)

            For Each ruta In FicherosDesde(desde.Date)
                ct.ThrowIfCancellationRequested()

                Dim lineas As String()
                Try
                    lineas = Await LeerLineasAsync(ruta, ct).ConfigureAwait(False)
                Catch ex As IOException
                    Continue For   ' alguien lo tiene abierto en exclusiva: se ignora este fichero
                End Try

                For Each linea In lineas
                    If String.IsNullOrWhiteSpace(linea) Then Continue For

                    Dim registro As LineaRegistro = Nothing
                    Try
                        registro = JsonSerializer.Deserialize(Of LineaRegistro)(linea, OpcionesJson)
                    Catch ex As JsonException
                        Continue For   ' línea partida o corrupta
                    End Try

                    If registro Is Nothing OrElse registro.Momento < desde Then Continue For

                    resultado.Add(New Ejecucion With {
                        .Id = registro.Id,
                        .Momento = registro.Momento,
                        .Operacion = registro.Operacion,
                        .Grupo = registro.Grupo,
                        .Detalle = registro.Detalle,
                        .NombreUsuario = registro.NombreUsuario,
                        .Login = registro.Login,
                        .Entorno = CType(registro.Entorno, ClaveEntorno),
                        .Estado = CType(registro.Estado, EstadoEjecucion),
                        .Registros = registro.Registros,
                        .Errores = registro.Errores,
                        .Duracion = TimeSpan.FromSeconds(registro.Segundos),
                        .Equipo = registro.Equipo
                    })
                Next
            Next

            Return resultado.OrderByDescending(Function(e) e.Momento).ToList()

        End Function

        Private Shared Async Function LeerLineasAsync(ruta As String, ct As CancellationToken) As Task(Of String())
            ' FileShare.ReadWrite: hay que poder leer aunque otro proceso esté añadiendo.
            Using fs As New FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite,
                                       bufferSize:=8192, useAsync:=True)
                Using lector As New StreamReader(fs, Encoding.UTF8)
                    Dim contenido = Await lector.ReadToEndAsync().ConfigureAwait(False)
                    ct.ThrowIfCancellationRequested()
                    Return contenido.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                End Using
            End Using
        End Function

        Private Function FicherosDesde(dia As Date) As IEnumerable(Of String)
            Return Directory.EnumerateFiles(Carpeta, Prefijo & "*" & Extension) _
                            .Where(Function(r) FechaDe(r) >= dia) _
                            .OrderBy(Function(r) FechaDe(r))
        End Function

        ''' <summary>
        ''' Fecha que lleva el nombre del fichero. Si no se puede interpretar devuelve
        ''' MinValue, de modo que la purga lo trate como antiguo y lo acabe barriendo.
        ''' </summary>
        Private Shared Function FechaDe(ruta As String) As Date
            Dim nombre = Path.GetFileNameWithoutExtension(ruta)
            If nombre.Length < Prefijo.Length + 8 Then Return Date.MinValue

            Dim trozo = nombre.Substring(Prefijo.Length, 8)
            Dim fecha As Date
            If Date.TryParseExact(trozo, "yyyyMMdd", Globalization.CultureInfo.InvariantCulture,
                                  Globalization.DateTimeStyles.None, fecha) Then
                Return fecha
            End If
            Return Date.MinValue
        End Function

        ' ============================================================
        ' RETENCIÓN
        ' ============================================================

        ''' <summary>
        ''' Borra los ficheros anteriores a la ventana de retención. Se llama al arrancar.
        ''' Devuelve cuántos ha borrado. No lanza: si un fichero está en uso, se queda para
        ''' la próxima.
        ''' </summary>
        Public Function Purgar() As Integer
            If Not Directory.Exists(Carpeta) Then Return 0

            Dim limite = Date.Today.AddDays(-Math.Max(1, DiasRetencion))
            Dim borrados = 0

            For Each ruta In Directory.EnumerateFiles(Carpeta, Prefijo & "*" & Extension).ToList()
                If FechaDe(ruta) >= limite Then Continue For
                Try
                    File.Delete(ruta)
                    borrados += 1
                Catch ex As IOException
                    ' lo tiene abierto otra instancia: ya caerá en el próximo arranque
                Catch ex As UnauthorizedAccessException
                End Try
            Next

            Return borrados
        End Function

        Private Shared Function SanearParaNombre(texto As String) As String
            Dim invalidos = Path.GetInvalidFileNameChars()
            Dim sb As New StringBuilder(texto.Length)
            For Each c In texto
                sb.Append(If(Array.IndexOf(invalidos, c) >= 0 OrElse c = "-"c, "_"c, c))
            Next
            Return sb.ToString()
        End Function

    End Class

End Namespace
