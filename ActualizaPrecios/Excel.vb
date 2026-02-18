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

    Public Function LeerContratosDesdeExcel(rutaArchivo As String) As List(Of ContratoTarifa)

        Dim resultado As New List(Of ContratoTarifa)
        Try


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Using package As New ExcelPackage(New FileInfo(rutaArchivo))
                Dim worksheet = package.Workbook.Worksheets(0)
                Dim rowCount = worksheet.Dimension.Rows

                For row = 2 To rowCount

                    Dim idTexto = worksheet.Cells(row, 2).Value?.ToString()
                    If String.IsNullOrWhiteSpace(idTexto) Then Continue For

                    Dim id As Long
                    If Not Long.TryParse(idTexto, id) Then Continue For

                    Dim fechaAplicar As Date
                    Date.TryParse(worksheet.Cells(row, 3).Value?.ToString(), fechaAplicar)

                    Dim fechaCierre As Date
                    Date.TryParse(worksheet.Cells(row, 4).Value?.ToString(), fechaCierre)



                    resultado.Add(New ContratoTarifa With {
                    .IdContratoTarifa = id,
                    .CodigoContrato = worksheet.Cells(row, 1).Value?.ToString(),
                    .FechaHasta = fechaCierre,
                    .FechaDesde = fechaAplicar
                })
                Next
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return resultado
    End Function
End Class
