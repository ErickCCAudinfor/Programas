Imports System.Data.SqlClient
Imports System.IO
Imports ClosedXML.Excel

Module ExportToExcelDinamico
    Sub ExportarConsultaAExcel(connectionString As String, consultaSQL As String, rutaArchivo As String, hojaNombre As String)
        Try
            ' 1. Conectar a la base de datos y ejecutar la consulta
            Using conexion As New SqlConnection(connectionString)
                Dim comando As New SqlCommand(consultaSQL, conexion)
                Dim adaptador As New SqlDataAdapter(comando)
                Dim tablaDatos As New DataTable()

                conexion.Open()
                comando.CommandTimeout = 10000
                adaptador.Fill(tablaDatos)
                conexion.Close()

                ' 2. Verificar si hay datos
                If tablaDatos.Rows.Count = 0 Then
                    'Console.WriteLine("La consulta no devolvió registros.")
                    Exit Sub ' Salir sin generar el archivo
                End If

                Dim workbook As XLWorkbook

                If File.Exists(rutaArchivo) Then
                    ' Si el archivo existe, intenta abrirlo
                    Try
                        workbook = New XLWorkbook(rutaArchivo)
                    Catch ex As Exception
                        ' Si no se puede abrir (probablemente por estar vacío o malformado), crea uno nuevo
                        workbook = New XLWorkbook()
                    End Try
                Else
                    ' Si no existe, crea un archivo nuevo
                    workbook = New XLWorkbook()
                End If

                Using workbook
                    ' 4. Verificar si la hoja ya existe y evitar duplicados
                    Dim hoja As IXLWorksheet
                    If workbook.Worksheets.Any(Function(ws) ws.Name = hojaNombre) Then
                        hoja = workbook.Worksheet(hojaNombre) ' Si la hoja existe, la usa
                    Else
                        hoja = workbook.Worksheets.Add(hojaNombre) ' Si no, la crea
                    End If


                    ' 5. Escribir los encabezados (nombres de las columnas) en la primera fila
                    For i As Integer = 0 To tablaDatos.Columns.Count - 1
                        hoja.Cell(1, i + 1).Value = tablaDatos.Columns(i).ColumnName
                        hoja.Cell(1, i + 1).Style.Font.Bold = True ' Hacer que los encabezados sean en negrita
                    Next

                    ' 6. Insertar los datos a partir de la segunda fila
                    Dim filaExcel As Integer = 2 ' Comienza en la fila 2 (después de los encabezados)
                    For Each fila As DataRow In tablaDatos.Rows
                        For columnaIndex As Integer = 0 To tablaDatos.Columns.Count - 1
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
                    Dim nombreHoja As String = "Hoja1"
                    If workbook.Worksheets.Any(Function(ws) ws.Name = nombreHoja) Then
                        workbook.Worksheets.Delete(nombreHoja)
                    End If
                    ' 7. Ajustar columnas y guardar
                    hoja.Columns().AdjustToContents()
                    workbook.SaveAs(rutaArchivo)

                    'Console.WriteLine("Excel generado correctamente en: " & rutaArchivo)
                End Using
            End Using
        Catch ex As Exception
            'Console.WriteLine("Error: " & ex.Message)
            Throw
        End Try

    End Sub
End Module
