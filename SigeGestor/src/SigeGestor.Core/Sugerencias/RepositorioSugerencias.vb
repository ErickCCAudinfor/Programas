Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization

Namespace Sugerencias

    ''' <summary>En qué punto está una sugerencia. Lo mueve quien la revisa.</summary>
    Public Enum EstadoSugerencia
        Pendiente = 0
        EnEstudio = 1
        Hecha = 2
        Descartada = 3
    End Enum

    ''' <summary>
    ''' Algo que alguien del equipo propone para la aplicación.
    '''
    ''' Los nombres de las propiedades salen tal cual al JSON y el fichero se puede abrir con
    ''' el Bloc de notas, igual que Usuarios.json.
    ''' </summary>
    Public Class Sugerencia

        ''' <summary>
        ''' Identificador y nombre del fichero a la vez. Se compone con la fecha y el login
        ''' para que dos personas no puedan generar el mismo.
        ''' </summary>
        Public Property Id As String = ""

        Public Property Titulo As String = ""

        Public Property Detalle As String = ""

        ''' <summary>Login de quien la propuso. Solo esa persona puede editarla o borrarla.</summary>
        Public Property Login As String = ""

        Public Property Nombre As String = ""

        Public Property Creada As Date

        Public Property Modificada As Date?

        <JsonConverter(GetType(JsonStringEnumConverter))>
        Public Property Estado As EstadoSugerencia

        ''' <summary>Lo que conteste quien la revise. Lo ve quien la propuso.</summary>
        Public Property Respuesta As String = ""

    End Class

    ''' <summary>
    ''' Las sugerencias del equipo, en la carpeta Sugerencias\ junto al ejecutable.
    '''
    ''' UN FICHERO POR SUGERENCIA, y no un JSON con todas dentro. Es la diferencia con
    ''' Config\Usuarios.json: allí se puede perder una escritura sin consecuencias —a alguien
    ''' le sale otra vez el aviso de novedades— y aquí no, porque lo que se pierde es lo que
    ''' ha escrito una persona. Con un fichero por sugerencia, cada quien toca el suyo y dos
    ''' personas guardando a la vez no pueden pisarse: es el mismo razonamiento que llevó al
    ''' log de ejecuciones a tener un fichero por máquina y día.
    '''
    ''' El .exe vive en el .13 y las ocho personas lo abren desde ahí, así que
    ''' AppContext.BaseDirectory apunta al .13 para todas y la carpeta queda compartida sin
    ''' montar nada.
    ''' </summary>
    Public Class RepositorioSugerencias

        Public Const NombreCarpeta As String = "Sugerencias"

        Private Const Extension As String = ".json"

        Private Shared ReadOnly OpcionesLectura As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True,
            .ReadCommentHandling = JsonCommentHandling.Skip,
            .AllowTrailingCommas = True
        }

        Private Shared ReadOnly OpcionesEscritura As New JsonSerializerOptions With {
            .WriteIndented = True,
            .Encoder = Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }

        Public ReadOnly Property Carpeta As String
            Get
                Return Path.Combine(AppContext.BaseDirectory, NombreCarpeta)
            End Get
        End Property

        ''' <summary>
        ''' Todas, de la más reciente a la más antigua. Una sugerencia ilegible se salta en vez
        ''' de impedir ver el resto.
        ''' </summary>
        Public Function Listar() As IReadOnlyList(Of Sugerencia)

            If Not Directory.Exists(Carpeta) Then Return Array.Empty(Of Sugerencia)()

            Dim lista As New List(Of Sugerencia)()

            For Each ruta In Directory.GetFiles(Carpeta, "*" & Extension)
                Try
                    Dim s = JsonSerializer.Deserialize(Of Sugerencia)(
                        File.ReadAllText(ruta, Encoding.UTF8), OpcionesLectura)

                    If s IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(s.Id) Then lista.Add(s)

                Catch ex As Exception
                    ' Fichero a medias o roto. Se ignora esa y se siguen leyendo las demás.
                End Try
            Next

            Return lista.OrderByDescending(Function(s) s.Creada).ToList()

        End Function

        ''' <summary>
        ''' Crea una sugerencia y devuelve lo guardado. El Id lleva la fecha y el login, así que
        ''' dos personas escribiendo a la vez no pueden generar el mismo fichero.
        ''' </summary>
        Public Function Crear(titulo As String, detalle As String,
                              login As String, nombre As String) As Sugerencia

            Dim s As New Sugerencia With {
                .Id = IdLibre(login),
                .Titulo = titulo.Trim(),
                .Detalle = If(detalle, "").Trim(),
                .Login = login,
                .Nombre = nombre,
                .Creada = Date.Now,
                .Estado = EstadoSugerencia.Pendiente
            }

            Guardar(s)
            Return s

        End Function

        ''' <summary>Reescribe una sugerencia entera. Lanza si no se puede: aquí sí importa.</summary>
        Public Sub Guardar(sugerencia As Sugerencia)

            If sugerencia Is Nothing OrElse String.IsNullOrWhiteSpace(sugerencia.Id) Then
                Throw New ArgumentException("La sugerencia no tiene Id.", NameOf(sugerencia))
            End If

            Directory.CreateDirectory(Carpeta)

            Dim ruta = RutaDe(sugerencia.Id)
            Dim temporal = ruta & ".tmp"

            ' Temporal y reemplazo, para que un corte de red a mitad no deje el fichero
            ' escrito por la mitad y la sugerencia ilegible.
            File.WriteAllText(temporal, JsonSerializer.Serialize(sugerencia, OpcionesEscritura), Encoding.UTF8)

            If File.Exists(ruta) Then
                File.Replace(temporal, ruta, Nothing, ignoreMetadataErrors:=True)
            Else
                File.Move(temporal, ruta)
            End If

        End Sub

        Public Sub Borrar(id As String)

            If String.IsNullOrWhiteSpace(id) Then Exit Sub

            Dim ruta = RutaDe(id)
            If File.Exists(ruta) Then File.Delete(ruta)

        End Sub

        ''' <summary>
        ''' Un Id que no exista todavía.
        '''
        ''' Llevaba solo la fecha al segundo y el login, y con eso dos sugerencias de la misma
        ''' persona escritas dentro del mismo segundo salían con el mismo nombre de fichero: la
        ''' segunda sobrescribía a la primera sin decir nada. Pasa más de lo que parece al
        ''' escribir dos seguidas, y es justo el fallo que no se puede dejar: lo que se pierde
        ''' es lo que alguien acaba de escribir.
        '''
        ''' Ahora lleva milésimas y, si aun así coincide —dos personas, o dos ventanas, en la
        ''' misma milésima—, se le va añadiendo un sufijo hasta encontrar hueco.
        ''' </summary>
        Private Function IdLibre(login As String) As String

            Dim baseId = $"{Date.Now:yyyyMMdd-HHmmss-fff}-{SanearParaNombre(login)}"

            If Not File.Exists(RutaDe(baseId)) Then Return baseId

            For i = 2 To 100
                Dim conSufijo = $"{baseId}-{i}"
                If Not File.Exists(RutaDe(conSufijo)) Then Return conSufijo
            Next

            ' Cien choques en la misma milésima no es un caso real, pero devolver algo que
            ' pise lo de otro sí sería un problema de verdad.
            Return $"{baseId}-{Guid.NewGuid():N}"

        End Function

        Private Function RutaDe(id As String) As String
            Return Path.Combine(Carpeta, SanearParaNombre(id) & Extension)
        End Function

        ''' <summary>
        ''' Quita de un texto lo que no vale en un nombre de fichero. El Id acaba siendo el
        ''' nombre, y un login con una barra dejaría la ruta fuera de la carpeta.
        ''' </summary>
        Private Shared Function SanearParaNombre(texto As String) As String

            If String.IsNullOrWhiteSpace(texto) Then Return "sin-nombre"

            Dim sb As New StringBuilder()
            Dim prohibidos = Path.GetInvalidFileNameChars()

            For Each c In texto.Trim()
                sb.Append(If(Array.IndexOf(prohibidos, c) >= 0 OrElse c = "."c, "_"c, c))
            Next

            Return sb.ToString()

        End Function

    End Class

End Namespace
