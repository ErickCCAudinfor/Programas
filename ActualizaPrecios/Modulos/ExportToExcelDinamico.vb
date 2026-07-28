Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Spreadsheet

Module ExportToExcelDinamico

    Friend Const EXCEL_MAX_ROWS As Integer = 1_000_000

    Function FetchDataTable(connectionString As String, consultaSQL As String) As DataTable
        Using conexion As New SqlConnection(connectionString)
            Dim comando As New SqlCommand(consultaSQL, conexion)
            Dim adaptador As New SqlDataAdapter(comando)
            Dim tablaDatos As New DataTable()
            conexion.Open()
            comando.CommandTimeout = 10000
            adaptador.Fill(tablaDatos)
            Return tablaDatos
        End Using
    End Function

    ''' <summary>
    ''' Igual que FetchDataTable pero cancelable. Al cancelar se llama a SqlCommand.Cancel(),
    ''' que aborta la consulta en el servidor en vez de dejarla corriendo.
    ''' timeoutSegundos = 0 significa sin límite de tiempo.
    ''' </summary>
    Function FetchDataTableCancelable(connectionString As String, consultaSQL As String,
                                      token As Threading.CancellationToken,
                                      timeoutSegundos As Integer) As DataTable

        Using conexion As New SqlConnection(connectionString)
            Using comando As New SqlCommand(consultaSQL, conexion)
                comando.CommandTimeout = timeoutSegundos

                Using token.Register(Sub()
                                         Try
                                             comando.Cancel()
                                         Catch
                                             ' El comando ya había terminado: nada que abortar.
                                         End Try
                                     End Sub)

                    Using adaptador As New SqlDataAdapter(comando)
                        Dim tablaDatos As New DataTable()
                        conexion.Open()
                        Try
                            adaptador.Fill(tablaDatos)
                        Catch ex As SqlException When token.IsCancellationRequested
                            ' Cancel() hace que Fill lance SqlException; se traduce a la excepción esperada.
                            Throw New OperationCanceledException(token)
                        End Try
                        Return tablaDatos
                    End Using

                End Using
            End Using
        End Using

    End Function

    Function EscribirDataTableAExcel(dt As DataTable, rutaArchivo As String, hojaNombre As String) As Integer
        If dt.Rows.Count = 0 Then Return 0

        Dim workbook As XLWorkbook
        If File.Exists(rutaArchivo) Then
            Try
                workbook = New XLWorkbook(rutaArchivo)
            Catch
                workbook = New XLWorkbook()
            End Try
        Else
            workbook = New XLWorkbook()
        End If

        Using workbook
            If workbook.Worksheets.Any(Function(ws) ws.Name = hojaNombre) Then
                workbook.Worksheets.Delete(hojaNombre)
            End If
            Dim hoja = workbook.Worksheets.Add(hojaNombre)

            For i As Integer = 0 To dt.Columns.Count - 1
                hoja.Cell(1, i + 1).Value = dt.Columns(i).ColumnName
                hoja.Cell(1, i + 1).Style.Font.Bold = True
            Next

            Dim filaExcel As Integer = 2
            For Each fila As DataRow In dt.Rows
                For columnaIndex As Integer = 0 To dt.Columns.Count - 1
                    Dim valorCelda As Object = fila(columnaIndex)
                    If IsDBNull(valorCelda) Then
                        hoja.Cell(filaExcel, columnaIndex + 1).SetValue("")
                    Else
                        Select Case valorCelda.GetType()
                            Case GetType(Int16), GetType(Int32), GetType(Int64)
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToInt64(valorCelda))
                            Case GetType(Single), GetType(Double), GetType(Decimal)
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToDouble(valorCelda)).Style.NumberFormat.Format = "#,##0.0"
                            Case GetType(DateTime)
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToDateTime(valorCelda))
                            Case GetType(Boolean)
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToBoolean(valorCelda))
                            Case Else
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(valorCelda.ToString())
                        End Select
                    End If
                Next
                filaExcel += 1
            Next

            If workbook.Worksheets.Any(Function(ws) ws.Name = "Hoja1") Then
                workbook.Worksheets.Delete("Hoja1")
            End If
            If dt.Rows.Count <= 50_000 Then
                hoja.Columns().AdjustToContents()
            End If
            workbook.SaveAs(rutaArchivo)
        End Using

        Return dt.Rows.Count
    End Function

    Sub EscribirFilasEnHoja(hoja As IXLWorksheet, dt As DataTable, filaInicio As Integer)
        Dim filaExcel As Integer = filaInicio
        For Each fila As DataRow In dt.Rows
            For columnaIndex As Integer = 0 To dt.Columns.Count - 1
                Dim valorCelda As Object = fila(columnaIndex)
                If IsDBNull(valorCelda) Then
                    hoja.Cell(filaExcel, columnaIndex + 1).SetValue("")
                Else
                    Select Case valorCelda.GetType()
                        Case GetType(Int16), GetType(Int32), GetType(Int64)
                            hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToInt64(valorCelda))
                        Case GetType(Single), GetType(Double), GetType(Decimal)
                            hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToDouble(valorCelda)).Style.NumberFormat.Format = "#,##0.0"
                        Case GetType(DateTime)
                            hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToDateTime(valorCelda))
                        Case GetType(Boolean)
                            hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToBoolean(valorCelda))
                        Case Else
                            hoja.Cell(filaExcel, columnaIndex + 1).SetValue(valorCelda.ToString())
                    End Select
                End If
            Next
            filaExcel += 1
        Next
    End Sub

    Sub AppendDataTableAExcel(dt As DataTable, rutaArchivo As String, hojaNombre As String)
        If dt.Rows.Count = 0 Then Return

        Dim workbook As XLWorkbook
        If File.Exists(rutaArchivo) Then
            Try
                workbook = New XLWorkbook(rutaArchivo)
            Catch
                workbook = New XLWorkbook()
            End Try
        Else
            workbook = New XLWorkbook()
        End If

        Using workbook
            Dim hoja As IXLWorksheet
            Dim filaInicio As Integer

            If workbook.Worksheets.Any(Function(ws) ws.Name = hojaNombre) Then
                hoja = workbook.Worksheet(hojaNombre)
                Dim lastRow = hoja.LastRowUsed()
                filaInicio = If(lastRow IsNot Nothing, lastRow.RowNumber() + 1, 2)
            Else
                hoja = workbook.Worksheets.Add(hojaNombre)
                For i As Integer = 0 To dt.Columns.Count - 1
                    hoja.Cell(1, i + 1).Value = dt.Columns(i).ColumnName
                    hoja.Cell(1, i + 1).Style.Font.Bold = True
                Next
                filaInicio = 2
            End If

            If workbook.Worksheets.Any(Function(ws) ws.Name = "Hoja1") Then
                workbook.Worksheets.Delete("Hoja1")
            End If

            Dim filaExcel As Integer = filaInicio
            For Each fila As DataRow In dt.Rows
                For columnaIndex As Integer = 0 To dt.Columns.Count - 1
                    Dim valorCelda As Object = fila(columnaIndex)
                    If IsDBNull(valorCelda) Then
                        hoja.Cell(filaExcel, columnaIndex + 1).SetValue("")
                    Else
                        Select Case valorCelda.GetType()
                            Case GetType(Int16), GetType(Int32), GetType(Int64)
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToInt64(valorCelda))
                            Case GetType(Single), GetType(Double), GetType(Decimal)
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToDouble(valorCelda)).Style.NumberFormat.Format = "#,##0.0"
                            Case GetType(DateTime)
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToDateTime(valorCelda))
                            Case GetType(Boolean)
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToBoolean(valorCelda))
                            Case Else
                                hoja.Cell(filaExcel, columnaIndex + 1).SetValue(valorCelda.ToString())
                        End Select
                    End If
                Next
                filaExcel += 1
            Next

            workbook.SaveAs(rutaArchivo)
        End Using
    End Sub

    Sub ExportarConsultaAExcel(connectionString As String, consultaSQL As String, rutaArchivo As String, hojaNombre As String)
        Try
            Dim tablaDatos = FetchDataTable(connectionString, consultaSQL)
            If tablaDatos.Rows.Count = 0 Then Exit Sub
            EscribirDataTableAExcel(tablaDatos, rutaArchivo, hojaNombre)
        Catch ex As Exception
            Throw
        End Try
    End Sub

    ''' <summary>
    ''' Escribe filas directamente a disco usando OpenXmlWriter (streaming).
    ''' No acumula nada en RAM: válido para millones de filas sin OutOfMemoryException.
    ''' </summary>
    Sub EscribirStreamingAExcel(rows As IEnumerable(Of DataRow), columns As DataColumnCollection, rutaArchivo As String, hojaNombre As String)
        If rows Is Nothing Then Return
        Dim sheetName As String = If(hojaNombre.Length > 31, hojaNombre.Substring(0, 31), hojaNombre)

        Using doc As SpreadsheetDocument = SpreadsheetDocument.Create(rutaArchivo, SpreadsheetDocumentType.Workbook)
            Dim workbookPart As WorkbookPart = doc.AddWorkbookPart()
            workbookPart.Workbook = New Workbook()

            Dim worksheetPart As WorksheetPart = workbookPart.AddNewPart(Of WorksheetPart)()

            Using writer As OpenXmlWriter = OpenXmlWriter.Create(worksheetPart)
                writer.WriteStartElement(New Worksheet())
                writer.WriteStartElement(New SheetData())

                ' Cabecera
                writer.WriteStartElement(New Row())
                For Each col As DataColumn In columns
                    writer.WriteStartElement(New Cell() With {.DataType = CellValues.InlineString})
                    writer.WriteElement(New InlineString(New Text(col.ColumnName)))
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement() ' Row

                ' Datos fila a fila
                For Each fila As DataRow In rows
                    writer.WriteStartElement(New Row())
                    For Each val As Object In fila.ItemArray
                        If IsDBNull(val) OrElse val Is Nothing Then
                            writer.WriteStartElement(New Cell() With {.DataType = CellValues.InlineString})
                            writer.WriteElement(New InlineString(New Text("")))
                            writer.WriteEndElement()
                        ElseIf TypeOf val Is Short OrElse TypeOf val Is Integer OrElse TypeOf val Is Long Then
                            writer.WriteStartElement(New Cell() With {.DataType = CellValues.Number})
                            writer.WriteElement(New CellValue(val.ToString()))
                            writer.WriteEndElement()
                        ElseIf TypeOf val Is Single OrElse TypeOf val Is Double OrElse TypeOf val Is Decimal Then
                            writer.WriteStartElement(New Cell() With {.DataType = CellValues.Number})
                            writer.WriteElement(New CellValue(Convert.ToDouble(val).ToString("G", CultureInfo.InvariantCulture)))
                            writer.WriteEndElement()
                        ElseIf TypeOf val Is Boolean Then
                            writer.WriteStartElement(New Cell() With {.DataType = CellValues.Boolean})
                            writer.WriteElement(New CellValue(If(CBool(val), "1", "0")))
                            writer.WriteEndElement()
                        ElseIf TypeOf val Is DateTime Then
                            writer.WriteStartElement(New Cell() With {.DataType = CellValues.InlineString})
                            writer.WriteElement(New InlineString(New Text(CDate(val).ToString("dd/MM/yyyy HH:mm:ss"))))
                            writer.WriteEndElement()
                        Else
                            writer.WriteStartElement(New Cell() With {.DataType = CellValues.InlineString})
                            writer.WriteElement(New InlineString(New Text(val.ToString())))
                            writer.WriteEndElement()
                        End If
                    Next
                    writer.WriteEndElement() ' Row
                Next

                writer.WriteEndElement() ' SheetData
                writer.WriteEndElement() ' Worksheet
            End Using

            ' Registrar la hoja en el workbook
            Dim sheets As Sheets = workbookPart.Workbook.AppendChild(New Sheets())
            sheets.Append(New Sheet() With {
                .Id = workbookPart.GetIdOfPart(worksheetPart),
                .SheetId = 1UI,
                .Name = sheetName
            })
            workbookPart.Workbook.Save()
        End Using
    End Sub

End Module
