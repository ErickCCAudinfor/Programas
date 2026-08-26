Imports System.IO
Imports System.Text.Json

Namespace Configuracion

    ''' <summary>Lo que la aplicación recuerda de una persona entre sesiones.</summary>
    Public Class EstadoUsuario

        Public Property Login As String = ""

        ''' <summary>Última versión de novedades que ha visto. Vacío = no ha visto ninguna.</summary>
        Public Property VersionNovedadesLeida As String = ""

    End Class

    ''' <summary>
    ''' Estado por usuario, en Config\Usuarios.json junto al ejecutable.
    '''
    ''' Es el equivalente del Usuarios.json de ActualizaPrecios, y funciona por el mismo
    ''' mecanismo: el .exe vive en el .13 y las ocho personas lo abren desde ahí, así que
    ''' AppContext.BaseDirectory apunta al .13 para todas y el fichero queda compartido.
    '''
    ''' AQUÍ SÍ HAY ESCRITURA CONCURRENTE DE VERDAD, al contrario que en el log de ejecuciones:
    ''' es un único fichero que ocho procesos pueden reescribir. Por eso se relee justo antes
    ''' de guardar y solo se toca la entrada propia — así dos personas marcando las novedades
    ''' como leídas a la vez no se borran la una a la otra—, y se escribe por reemplazo para
    ''' que un corte no deje el fichero a medias.
    '''
    ''' Y por eso mismo NO se guarda aquí nada que importe: si una escritura se pierde, lo
    ''' único que pasa es que a alguien le vuelve a salir el aviso de novedades una vez más.
    ''' </summary>
    Public Class RepositorioEstadoUsuario

        Public Const NombreFichero As String = "Usuarios.json"

        Private Shared ReadOnly OpcionesLectura As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True,
            .ReadCommentHandling = JsonCommentHandling.Skip,
            .AllowTrailingCommas = True
        }

        Private Shared ReadOnly OpcionesEscritura As New JsonSerializerOptions With {
            .WriteIndented = True
        }

        Public ReadOnly Property Ruta As String
            Get
                Return Path.Combine(AppContext.BaseDirectory,
                                    RepositorioEntornos.NombreCarpeta,
                                    NombreFichero)
            End Get
        End Property

        ''' <summary>
        ''' El estado de un login. Nunca devuelve Nothing: si no hay fichero o no hay entrada,
        ''' devuelve uno vacío, que se interpreta como «no ha visto ninguna novedad».
        ''' </summary>
        Public Function Obtener(login As String) As EstadoUsuario

            Dim todos = Leer()

            Dim mio = todos.FirstOrDefault(
                Function(e) String.Equals(e.Login, login, StringComparison.OrdinalIgnoreCase))

            Return If(mio, New EstadoUsuario With {.Login = login})

        End Function

        ''' <summary>
        ''' Guarda el estado de un login dejando intacto el del resto. Si no se puede escribir
        ''' no se lanza nada: es estado de comodidad, no datos.
        ''' </summary>
        Public Sub Guardar(estado As EstadoUsuario)

            If estado Is Nothing OrElse String.IsNullOrWhiteSpace(estado.Login) Then Exit Sub

            Try
                ' Se relee AHORA, no se usa una lista cargada antes: entre medias otra persona
                ' puede haber guardado lo suyo.
                Dim todos = Leer().ToList()

                todos.RemoveAll(
                    Function(e) String.Equals(e.Login, estado.Login, StringComparison.OrdinalIgnoreCase))
                todos.Add(estado)

                Directory.CreateDirectory(Path.GetDirectoryName(Ruta))

                Dim temporal = Ruta & ".tmp"
                File.WriteAllText(temporal, JsonSerializer.Serialize(todos, OpcionesEscritura))

                If File.Exists(Ruta) Then
                    File.Replace(temporal, Ruta, Nothing, ignoreMetadataErrors:=True)
                Else
                    File.Move(temporal, Ruta)
                End If

            Catch ex As Exception
                ' Fichero bloqueado por otro proceso, o sin permisos. Se pierde la marca y a
                ' esa persona le volverá a salir el aviso. No merece molestar a nadie.
            End Try

        End Sub

        Private Function Leer() As IReadOnlyList(Of EstadoUsuario)

            If Not File.Exists(Ruta) Then Return Array.Empty(Of EstadoUsuario)()

            Try
                Dim leidos = JsonSerializer.Deserialize(Of List(Of EstadoUsuario))(
                    File.ReadAllText(Ruta), OpcionesLectura)

                If leidos Is Nothing Then Return Array.Empty(Of EstadoUsuario)()

                Return leidos.Where(Function(e) Not String.IsNullOrWhiteSpace(e.Login)).ToList()

            Catch ex As Exception
                ' JSON roto: se empieza de cero en vez de impedir entrar en la aplicación.
                Return Array.Empty(Of EstadoUsuario)()
            End Try

        End Function

    End Class

End Namespace
