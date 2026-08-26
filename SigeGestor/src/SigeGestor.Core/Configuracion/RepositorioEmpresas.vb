Imports System.IO
Imports System.Text.Json
Imports System.Threading
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Modelos
Imports SigeGestor.Core.Seguridad

Namespace Configuracion

    ''' <summary>
    ''' Lee y escribe Config\Empresas.json, la lista de bases de clientes.
    '''
    ''' Es el sustituto de Json\EmpresasBD.json de ActualizaPrecios, con el mismo formato y el
    ''' mismo cifrado, así que el fichero de allí se puede copiar tal cual.
    '''
    ''' A DIFERENCIA de Entornos.json, este fichero SÍ se edita desde la aplicación: la pantalla
    ''' de Empresas escribe aquí. Como el .exe vive en el .13 y todos abren el mismo fichero,
    ''' guardar es escritura compartida —ver <see cref="Guardar"/>—.
    ''' </summary>
    Public Class RepositorioEmpresas

        Public Const NombreFichero As String = "Empresas.json"

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
        ''' Las empresas registradas. Si el fichero no está, lista vacía: no es un error, es que
        ''' todavía no se ha registrado ninguna. Que falte no puede impedir abrir la pantalla.
        ''' </summary>
        Public Function Cargar() As IReadOnlyList(Of EmpresaBD)

            If Not File.Exists(Ruta) Then Return Array.Empty(Of EmpresaBD)()

            Try
                Dim empresas = JsonSerializer.Deserialize(Of List(Of EmpresaBD))(
                    File.ReadAllText(Ruta), OpcionesLectura)

                If empresas Is Nothing Then Return Array.Empty(Of EmpresaBD)()

                Return empresas.Where(Function(e) Not String.IsNullOrWhiteSpace(e.Nombre)).ToList()

            Catch ex As Exception
                Throw New ConfiguracionNoDisponibleException(
                    Ruta, $"El fichero de empresas no se puede leer: {ex.Message}", ex)
            End Try

        End Function

        ''' <summary>
        ''' Guarda la lista completa.
        '''
        ''' Se escribe a un temporal y se reemplaza, en vez de abrir el fichero definitivo y
        ''' escribir encima. Con el .exe compartido en el .13 hay ocho personas apuntando al
        ''' mismo fichero: si se corta la escritura a medias, el original se queda truncado y
        ''' nadie puede abrir la pantalla. Con reemplazo, o está el viejo o está el nuevo.
        ''' </summary>
        Public Sub Guardar(empresas As IEnumerable(Of EmpresaBD))

            Dim lista = If(empresas Is Nothing, New List(Of EmpresaBD), empresas.ToList())

            Directory.CreateDirectory(Path.GetDirectoryName(Ruta))

            Dim temporal = Ruta & ".tmp"
            File.WriteAllText(temporal, JsonSerializer.Serialize(lista, OpcionesEscritura))

            If File.Exists(Ruta) Then
                ' Con copia de seguridad: si el reemplazo falla a medias, queda el .bak.
                File.Replace(temporal, Ruta, Ruta & ".bak", ignoreMetadataErrors:=True)
            Else
                File.Move(temporal, Ruta)
            End If

        End Sub

        ''' <summary>
        ''' Cadena de conexión de una empresa. Igual que la de los entornos, incluido
        ''' TrustServerCertificate: los servidores de SIGE no tienen certificado válido y
        ''' Microsoft.Data.SqlClient cifra por defecto desde la versión 4.
        ''' </summary>
        Public Function CadenaConexion(empresa As EmpresaBD,
                                       Optional segundosTimeout As Integer = 15) As String

            Dim constructor As New SqlConnectionStringBuilder With {
                .DataSource = empresa.Servidor,
                .InitialCatalog = empresa.BaseDatos,
                .UserID = Cifrado.Descifrar(empresa.Usuario),
                .Password = Cifrado.Descifrar(empresa.Password),
                .TrustServerCertificate = True,
                .ConnectTimeout = segundosTimeout,
                .ApplicationName = "SigeGestor"
            }
            Return constructor.ConnectionString

        End Function

        ''' <summary>
        ''' Abre y cierra, para saber si los datos valen antes de guardarlos. Devuelve cadena
        ''' vacía si conecta, y el motivo si no.
        '''
        ''' En ActualizaPrecios no había forma de comprobarlo: se registraba la base y el fallo
        ''' aparecía después, al intentar usarla, con un mensaje que no decía que fuese por eso.
        ''' </summary>
        Public Async Function ProbarAsync(empresa As EmpresaBD,
                                          Optional ct As CancellationToken = Nothing) As Task(Of String)

            Try
                Using conexion As New SqlConnection(CadenaConexion(empresa, segundosTimeout:=8))
                    Await conexion.OpenAsync(ct).ConfigureAwait(False)
                End Using
                Return ""

            Catch ex As OperationCanceledException
                Throw
            Catch ex As Exception
                Dim motivo = ex.Message

                ' El mensaje de red de SqlClient son cinco líneas de las que solo la primera
                ' dice algo. Se recorta para que quepa junto al campo.
                Dim corte = motivo.IndexOf(vbLf)
                If corte > 0 Then motivo = motivo.Substring(0, corte).Trim()

                If empresa.VPN Then motivo &= " · esta empresa requiere VPN: comprueba que está conectada"

                Return motivo
            End Try

        End Function

    End Class

End Namespace
