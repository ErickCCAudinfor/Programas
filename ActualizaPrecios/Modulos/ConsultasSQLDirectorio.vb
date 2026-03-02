Imports System.IO

Module ConsultasSQLDirectorio

    Private ReadOnly RutaConsultas As String =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConsultasSIGE")

    Public Function ObtenerConsulta(nombreConsulta As String) As String

        ' Construir ruta del archivo
        Dim rutaArchivo = Path.Combine(RutaConsultas, $"{nombreConsulta}.sql")

        ' Validar que exista
        If Not File.Exists(rutaArchivo) Then
            Throw New FileNotFoundException($"No existe la consulta: {rutaArchivo}")
        End If

        ' Leer contenido del SQL
        Return File.ReadAllText(rutaArchivo)

    End Function

End Module