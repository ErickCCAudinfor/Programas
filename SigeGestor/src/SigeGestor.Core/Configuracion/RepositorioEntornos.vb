Imports System.IO
Imports System.Text.Json
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Modelos
Imports SigeGestor.Core.Seguridad

Namespace Configuracion

    ''' <summary>
    ''' Se lanza cuando falta o está mal el fichero de entornos. Se distingue del resto de
    ''' errores para que la interfaz pueda decir exactamente qué fichero crear y dónde.
    ''' </summary>
    Public Class ConfiguracionNoDisponibleException
        Inherits Exception

        Public Property Ruta As String

        Public Sub New(ruta As String, mensaje As String)
            MyBase.New(mensaje)
            Me.Ruta = ruta
        End Sub

        Public Sub New(ruta As String, mensaje As String, interna As Exception)
            MyBase.New(mensaje, interna)
            Me.Ruta = ruta
        End Sub

    End Class

    ''' <summary>
    ''' Lee Config\Entornos.json de junto al ejecutable.
    '''
    ''' Ese fichero NO va en el repositorio (lleva credenciales, aunque cifradas) y NO se
    ''' sobrescribe al publicar. Está el Entornos.ejemplo.json como plantilla.
    ''' </summary>
    Public Class RepositorioEntornos

        Public Const NombreCarpeta As String = "Config"
        Public Const NombreFichero As String = "Entornos.json"

        Private Shared ReadOnly OpcionesJson As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True,
            .ReadCommentHandling = JsonCommentHandling.Skip,
            .AllowTrailingCommas = True
        }

        Private _entornos As IReadOnlyList(Of EntornoBD)

        Public ReadOnly Property Ruta As String
            Get
                Return Path.Combine(AppContext.BaseDirectory, NombreCarpeta, NombreFichero)
            End Get
        End Property

        ''' <summary>Todos los entornos configurados, ordenados por clave (PRO, REP, UAT).</summary>
        Public Function Cargar() As IReadOnlyList(Of EntornoBD)
            If _entornos IsNot Nothing Then Return _entornos

            If Not File.Exists(Ruta) Then
                Throw New ConfiguracionNoDisponibleException(
                    Ruta,
                    $"No se encuentra {NombreCarpeta}\{NombreFichero}. Copia Entornos.ejemplo.json, renómbralo y rellena los datos de conexión.")
            End If

            Dim leidos As List(Of EntornoBD)
            Try
                Dim json = File.ReadAllText(Ruta)
                leidos = JsonSerializer.Deserialize(Of List(Of EntornoBD))(json, OpcionesJson)
            Catch ex As JsonException
                Throw New ConfiguracionNoDisponibleException(
                    Ruta,
                    $"{NombreFichero} no es un JSON válido: {ex.Message}", ex)
            End Try

            If leidos Is Nothing OrElse leidos.Count = 0 Then
                Throw New ConfiguracionNoDisponibleException(
                    Ruta, $"{NombreFichero} no contiene ningún entorno.")
            End If

            _entornos = leidos.OrderBy(Function(e) CInt(e.Clave)).ToList()
            Return _entornos
        End Function

        Public Function Obtener(clave As ClaveEntorno) As EntornoBD
            Dim encontrado = Cargar().FirstOrDefault(Function(e) e.Clave = clave)
            If encontrado Is Nothing Then
                Throw New ConfiguracionNoDisponibleException(
                    Ruta, $"No hay ningún entorno con clave '{clave}' en {NombreFichero}.")
            End If
            Return encontrado
        End Function

        ''' <summary>
        ''' Entornos válidos para una operación. Las de escritura no ofrecen los marcados como
        ''' solo lectura, que es lo que deja Replica fuera de Precios, Contratos y Productos.
        ''' </summary>
        Public Function Disponibles(paraEscritura As Boolean) As IReadOnlyList(Of EntornoBD)
            If Not paraEscritura Then Return Cargar()
            Return Cargar().Where(Function(e) Not e.SoloLectura).ToList()
        End Function

        ''' <summary>
        ''' Cadena de conexión de un entorno, con las credenciales descifradas.
        '''
        ''' TrustServerCertificate va a True a propósito: Microsoft.Data.SqlClient cifra la
        ''' conexión por defecto desde la versión 4, y los servidores de SIGE no tienen un
        ''' certificado válido para el nombre con el que se les llama. Sin esto, todas las
        ''' conexiones fallarían con un error de cadena de certificados.
        ''' </summary>
        Public Function CadenaConexion(entorno As EntornoBD,
                                       Optional segundosTimeout As Integer = 15,
                                       Optional baseDatos As String = "") As String
            Dim constructor As New SqlConnectionStringBuilder With {
                .DataSource = entorno.Servidor,
                .InitialCatalog = If(String.IsNullOrWhiteSpace(baseDatos), entorno.BaseDatos, baseDatos),
                .UserID = Cifrado.Descifrar(entorno.Usuario),
                .Password = Cifrado.Descifrar(entorno.Password),
                .TrustServerCertificate = True,
                .ConnectTimeout = segundosTimeout,
                .ApplicationName = "SigeGestor"
            }
            Return constructor.ConnectionString
        End Function

        Public Function CadenaConexion(clave As ClaveEntorno) As String
            Return CadenaConexion(Obtener(clave))
        End Function

    End Class

End Namespace
