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

            'Columna 1 codigocontrato
            'Columna 2 fechaaplicar nueva
            'Columna 3 fechacierre anterior calendario
            'Columna 4 grupo tarifa vieja 
            'Columna 5 grupo tarifa nueva
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Using package As New ExcelPackage(New FileInfo(rutaArchivo))
                Dim worksheet = package.Workbook.Worksheets(0)
                Dim rowCount = worksheet.Dimension.Rows

                For row = 2 To rowCount

                    Dim CodContratoTexto = worksheet.Cells(row, 1).Value?.ToString()
                    If String.IsNullOrWhiteSpace(CodContratoTexto) Then Continue For

                    Dim codContrato As Long
                    If Not Long.TryParse(CodContratoTexto, codContrato) Then Continue For

                    Dim fechaAplicar As Date
                    Dim fechaAplicar2 As Date
                    Date.TryParse(worksheet.Cells(row, 2).Value?.ToString(), fechaAplicar)

                    Dim fechaCierre As Date
                    Date.TryParse(worksheet.Cells(row, 3).Value?.ToString(), fechaCierre)

                    Dim grupoviejo = worksheet.Cells(row, 4).Value?.ToString()
                    Dim gruponuevo = worksheet.Cells(row, 5).Value?.ToString()

                    Dim IsQ As Boolean
                    Boolean.TryParse(worksheet.Cells(row, 6).Value?.ToString(), IsQ)

                    Dim IsMantenerPerfil As Boolean
                    Boolean.TryParse(worksheet.Cells(row, 7).Value?.ToString(), IsMantenerPerfil)
                    Date.TryParse(worksheet.Cells(row, 8).Value?.ToString(), fechaAplicar2)
                    resultado.Add(New ContratoTarifa With {
                    .CodigoContrato = codContrato, 'Columna 1                    
                    .FechaDesde = fechaAplicar, ' Columna 2
                    .FechaHasta = fechaCierre, 'Columna 3
                    .textotarifagrupoViejo = grupoviejo, ' 4
                    .textotarifagrupoNuevo = gruponuevo, '5
                    .IsQ = IsQ,'6
                    .MantenerPerfil = IsMantenerPerfil, '7                    
                    .FechaAplicar = fechaAplicar2 '8
                })
                Next
            End Using
        Catch ex As Exception
            Throw
        End Try
        Return resultado
    End Function
End Class
