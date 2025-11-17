Imports System.IO
Imports OfficeOpenXml

Public Class Excel
    Dim complementos As New Complementos()
    Public Sub EscribirEnExcel(rutaCarpeta As String, datos As List(Of List(Of Object)), NombreArchivo As String)
        Try
            Dim rutaArchivo As String = ""
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            Using excelPackage As New ExcelPackage()
                ' Agregar una hoja de trabajo al libro de Excel
                Dim worksheet = excelPackage.Workbook.Worksheets.Add("Hoja1")

                ' Escribir los datos en la hoja de trabajo
                For fila As Integer = 0 To datos.Count - 1
                    For columna As Integer = 0 To datos(fila).Count - 1
                        worksheet.Cells(fila + 1, columna + 1).Value = datos(fila)(columna)
                    Next
                Next


                Dim NombreArch = $"{NombreArchivo}_LogExcel_{Date.Today.ToString("ddMMyyyy")}.xlsx"
                rutaArchivo = Path.Combine(rutaCarpeta, NombreArch)
                If Not Directory.Exists(rutaCarpeta) Then
                    Directory.CreateDirectory(rutaCarpeta)
                End If

                ' Verificar si el archivo existe, y si no, crearlo
                If Not File.Exists(rutaArchivo) Then
                    File.Create(rutaArchivo).Close()
                End If
                ' Guardar el libro de Excel
                Dim fileInfo As New System.IO.FileInfo(rutaArchivo)
                excelPackage.SaveAs(fileInfo)
            End Using

            complementos.MostrarMensajePersonalizado($"Hay posibles errores, revise el archivo generado: {rutaArchivo}")
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado("Error al escribir en el archivo Excel: " & ex.Message)
        End Try
    End Sub
End Class
