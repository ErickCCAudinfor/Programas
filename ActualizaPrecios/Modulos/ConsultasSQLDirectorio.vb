Imports System.IO
Imports System.Text

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
        Return LeerSql(rutaArchivo)

    End Function

    ''' <summary>
    ''' Lee el .sql detectando su codificación.
    '''
    ''' Hace falta porque SSMS guarda por defecto en ANSI (Windows-1252) y File.ReadAllText
    ''' decodifica como UTF-8: las tildes se convertían en U+FFFD y eso rompe la consulta
    ''' cuando el acento está en un alias de columna, no solo en un comentario.
    ''' </summary>
    Private Function LeerSql(ruta As String) As String

        Dim bytes = File.ReadAllBytes(ruta)

        ' UTF-16, que sí trae BOM obligatorio
        If bytes.Length >= 2 Then
            If bytes(0) = &HFF AndAlso bytes(1) = &HFE Then Return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2)
            If bytes(0) = &HFE AndAlso bytes(1) = &HFF Then Return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2)
        End If

        ' UTF-8 con BOM: hay que saltarlo, o queda un carácter invisible antes del SQL
        Dim desde = 0
        If bytes.Length >= 3 AndAlso bytes(0) = &HEF AndAlso bytes(1) = &HBB AndAlso bytes(2) = &HBF Then desde = 3

        ' Sin BOM: si es UTF-8 válido se usa; si no, es ANSI
        Try
            Return New UTF8Encoding(False, True).GetString(bytes, desde, bytes.Length - desde)
        Catch ex As DecoderFallbackException
            Return DecodificarWindows1252(bytes)
        End Try

    End Function

    ''' <summary>
    ''' Windows-1252 a mano. Coincide con Latin-1 salvo en el rango 80-9F, donde 1252 mete
    ''' el euro y las comillas tipográficas. Se hace así para no depender del paquete
    ''' System.Text.Encoding.CodePages: en .NET 6 Encoding.GetEncoding(1252) lanza
    ''' NotSupportedException si no se registra antes el proveedor.
    ''' </summary>
    Private Function DecodificarWindows1252(bytes As Byte()) As String

        ' Posiciones 0-31 de esta cadena = bytes 80-9F. El 0xFFFD marca los no definidos.
        Const Altos As String = "€" & ChrW(&HFFFD) & "‚ƒ„…†‡ˆ‰Š‹Œ" & ChrW(&HFFFD) & "Ž" &
                                ChrW(&HFFFD) & ChrW(&HFFFD) & "‘’“”•–—˜™š›œ" & ChrW(&HFFFD) & "žŸ"

        Dim sb As New StringBuilder(bytes.Length)

        For Each b In bytes
            If b >= &H80 AndAlso b <= &H9F Then
                sb.Append(Altos(b - &H80))
            Else
                sb.Append(ChrW(b))
            End If
        Next

        Return sb.ToString()

    End Function

End Module
