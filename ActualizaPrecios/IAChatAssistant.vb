Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.Json
Imports ClosedXML.Excel
Imports iTextSharp.text.pdf
Imports Xceed.Words.NET

Public Class IAChatAssistant
    Private ReadOnly apiKey As String = "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"
    Private ReadOnly apiUrl As String = "https://openrouter.ai/api/v1/chat/completions"

    Public Async Function PreguntarAsync(preguntaUsuario As String, rutaArchivoContexto As String) As Task(Of String)
        Dim contexto As String = LeerArchivo(rutaArchivoContexto)

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", apiKey)
            client.DefaultRequestHeaders.Add("HTTP-Referer", "https://tudominio.com")
            client.DefaultRequestHeaders.Add("X-Title", "IAAssistance")

            Dim requestBody = New With {
                .model = "openai/gpt-3.5-turbo",
                .messages = {
                    New With {.role = "system", .content = contexto},
                    New With {.role = "user", .content = preguntaUsuario}
                }
            }

            Dim options As New JsonSerializerOptions With {
                .PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }

            Dim json As String = JsonSerializer.Serialize(requestBody, options)
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")

            Dim response As HttpResponseMessage = Await client.PostAsync(apiUrl, content)
            Dim result As String = Await response.Content.ReadAsStringAsync()

            If Not response.IsSuccessStatusCode Then
                Return $"Error HTTP {response.StatusCode}: {result}"
            End If

            Dim doc = JsonDocument.Parse(result)
            Dim message = doc.RootElement.GetProperty("choices")(0).GetProperty("message").GetProperty("content").GetString()

            Return message
        End Using
    End Function

    Private Function LeerArchivo(ruta As String) As String
        Dim ext = Path.GetExtension(ruta).ToLower()
        Select Case ext
            Case ".txt"
                Return File.ReadAllText(ruta)
            Case ".docx"
                Return LeerWord(ruta)
            'Case ".pdf"
            '    Return LeerPdf(ruta)
            Case ".xlsx"
                Return LeerExcel(ruta)
            Case Else
                Throw New NotSupportedException($"Extensión {ext} no soportada.")
        End Select
    End Function

    Private Function LeerWord(ruta As String) As String
        Dim tempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(ruta))
        File.Copy(ruta, tempPath, True) ' Copia el archivo temporalmente
        Try
            Using doc As DocX = DocX.Load(tempPath)
                Return doc.Text
            End Using
        Finally
            ' Intentar borrar el archivo temporal
            Try
                If File.Exists(tempPath) Then
                    File.Delete(tempPath)
                End If
            Catch ex As Exception
                ' Si no se puede borrar, simplemente ignora o registra el error
            End Try
        End Try
    End Function


    'Private Function LeerPdf(ruta As String) As String
    '    Dim texto As New StringBuilder()
    '    Using pdf = PdfDocument.Open(ruta)
    '        For Each page In pdf.GetPages()
    '            texto.AppendLine(page.Text)
    '        Next
    '    End Using
    '    Return texto.ToString()
    'End Function

    Private Function LeerExcel(ruta As String) As String
        Dim texto As New StringBuilder()
        Using workbook As New XLWorkbook(ruta)
            Dim worksheet = workbook.Worksheet(1)
            For Each row In worksheet.RowsUsed()
                For Each cell In row.CellsUsed()
                    texto.Append(cell.GetValue(Of String)() & vbTab)
                Next
                texto.AppendLine()
            Next
        End Using
        Return texto.ToString()
    End Function

End Class
