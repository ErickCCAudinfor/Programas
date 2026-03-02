Imports System.IO
Imports System.Text.Json

Public Module SesionActual
    Public UsuarioLogueado As UsuarioValidacion
    Public Sub MarcarUsuarioDesconectado(usuario As UsuarioValidacion)

        If Not File.Exists(RutaConfigUsuarios) Then Exit Sub

        Dim json = File.ReadAllText(RutaConfigUsuarios)

        Dim usuarios = JsonSerializer.Deserialize(Of List(Of UsuarioValidacion))(
            json,
            New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
        )

        If usuarios Is Nothing Then Exit Sub

        Dim usuarioExistente = usuarios.FirstOrDefault(Function(u) u.Nombre = usuario.Nombre AndAlso u.login = usuario.login)

        If usuarioExistente IsNot Nothing AndAlso usuarioExistente.logeado Then
            usuarioExistente.logeado = False
        End If

        Dim jsonFinal = JsonSerializer.Serialize(usuarios, New JsonSerializerOptions With {.WriteIndented = True})

        File.WriteAllText(RutaConfigUsuarios, jsonFinal)

    End Sub
End Module