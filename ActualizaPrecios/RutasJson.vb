Imports System.IO

Public Module RutasJson
    Public RutaConfigEmpresas As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json", "EmpresasBD.json")
    Public RutaConfigUsuarios As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json", "Usuarios.json")
End Module
