Imports System.IO
Imports System.Text.Json

''' <summary>
''' Gestiona, contra Usuarios.json, qué versión de novedades ha leído cada usuario.
''' </summary>
Public Module NovedadesUsuario

    Private ReadOnly OpcionesLectura As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
    Private ReadOnly OpcionesEscritura As New JsonSerializerOptions With {.WriteIndented = True}

    ''' <summary>
    ''' True si al usuario le quedan novedades por leer. Ante cualquier problema
    ''' devuelve False, para no dejar la campana parpadeando sin motivo.
    ''' </summary>
    Public Function HayNovedadesSinLeer(usuario As UsuarioValidacion) As Boolean

        If usuario Is Nothing Then Return False
        If String.IsNullOrEmpty(NovedadesApp.VersionActual) Then Return False

        Return Not String.Equals(VersionLeidaDe(usuario), NovedadesApp.VersionActual, StringComparison.OrdinalIgnoreCase)

    End Function

    ''' <summary>
    ''' Última versión de novedades que consta como leída para el usuario en Usuarios.json.
    ''' El JSON es la fuente de verdad: el objeto en memoria viene de la BD, que no conoce
    ''' este dato. Devuelve "" si no hay constancia.
    ''' </summary>
    Public Function VersionLeidaDe(usuario As UsuarioValidacion) As String

        If usuario Is Nothing Then Return ""

        Try
            Dim guardado = BuscarUsuario(LeerUsuarios(), usuario)
            Return If(guardado Is Nothing, "", If(guardado.VersionNovedadesLeida, ""))
        Catch ex As Exception
            ' Si el JSON no se puede leer, se asume leído para no dejar la campana parpadeando.
            Return NovedadesApp.VersionActual
        End Try

    End Function

    ''' <summary>
    ''' Marca las novedades de la versión actual como leídas para este usuario y lo
    ''' persiste en Usuarios.json. Devuelve False si no se pudo guardar.
    ''' </summary>
    Public Function MarcarNovedadesLeidas(usuario As UsuarioValidacion) As Boolean

        If usuario Is Nothing Then Return False

        Try
            Dim usuarios = LeerUsuarios()
            Dim guardado = BuscarUsuario(usuarios, usuario)

            If guardado Is Nothing Then
                ' No debería pasar (el login lo registra), pero así no se pierde la lectura.
                guardado = New UsuarioValidacion With {
                    .Nombre = usuario.Nombre,
                    .login = usuario.login,
                    .Password = "",
                    .Servidor = Environment.MachineName,
                    .logeado = usuario.logeado
                }
                usuarios.Add(guardado)
            End If

            guardado.VersionNovedadesLeida = NovedadesApp.VersionActual

            GuardarUsuarios(usuarios)
            Return True

        Catch ex As Exception
            Return False
        End Try

    End Function

#Region "acceso al json"

    Private Function LeerUsuarios() As List(Of UsuarioValidacion)

        If Not File.Exists(RutaConfigUsuarios) Then Return New List(Of UsuarioValidacion)

        Dim json = File.ReadAllText(RutaConfigUsuarios)
        If String.IsNullOrWhiteSpace(json) Then Return New List(Of UsuarioValidacion)

        Dim usuarios = JsonSerializer.Deserialize(Of List(Of UsuarioValidacion))(json, OpcionesLectura)

        Return If(usuarios, New List(Of UsuarioValidacion))

    End Function

    Private Sub GuardarUsuarios(usuarios As List(Of UsuarioValidacion))

        Dim carpeta = Path.GetDirectoryName(RutaConfigUsuarios)
        If Not String.IsNullOrEmpty(carpeta) AndAlso Not Directory.Exists(carpeta) Then
            Directory.CreateDirectory(carpeta)
        End If

        File.WriteAllText(RutaConfigUsuarios, JsonSerializer.Serialize(usuarios, OpcionesEscritura))

    End Sub

    Private Function BuscarUsuario(usuarios As List(Of UsuarioValidacion), usuario As UsuarioValidacion) As UsuarioValidacion

        Return usuarios.FirstOrDefault(Function(u) u.Nombre = usuario.Nombre AndAlso u.login = usuario.login)

    End Function

#End Region

End Module
