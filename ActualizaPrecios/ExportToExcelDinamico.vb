Imports System.Data.SqlClient
Imports System.IO
Imports ClosedXML.Excel
Imports OfficeOpenXml

Module ExportToExcelDinamico
    'Pinta Todos los datos recibidos del la consulta 
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

    'Pinta los datos por databla en excel
    Sub ExportarConsultaAExcelV2(ListatablaDatos As List(Of DataTable), rutaArchivo As String, hojaNombre As String)
        Try
            If ListatablaDatos Is Nothing OrElse ListatablaDatos.Count = 0 Then Exit Sub

            Dim workbook As XLWorkbook

            If File.Exists(rutaArchivo) Then
                Try
                    workbook = New XLWorkbook(rutaArchivo)
                Catch ex As Exception
                    workbook = New XLWorkbook()
                End Try
            Else
                workbook = New XLWorkbook()
            End If

            Using workbook
                Dim hoja As IXLWorksheet
                If workbook.Worksheets.Any(Function(ws) ws.Name = hojaNombre) Then
                    hoja = workbook.Worksheet(hojaNombre)
                Else
                    hoja = workbook.Worksheets.Add(hojaNombre)
                End If

                ' Escribir encabezados una sola vez si no existen
                If hoja.Cell(1, 1).IsEmpty Then
                    For i As Integer = 0 To ListatablaDatos(0).Columns.Count - 1
                        hoja.Cell(1, i + 1).Value = ListatablaDatos(0).Columns(i).ColumnName
                        hoja.Cell(1, i + 1).Style.Font.Bold = True
                    Next
                End If

                ' Calcular la fila de inicio (para no sobrescribir)
                Dim filaExcel As Integer = hoja.LastRowUsed()?.RowNumber() + 1
                If filaExcel <= 1 Then filaExcel = 2 ' Si no hay datos, empezar en la fila 2

                ' Insertar todos los registros uno debajo del otro
                For Each tabladatos In ListatablaDatos
                    For Each fila As DataRow In tabladatos.Rows
                        For columnaIndex As Integer = 0 To tabladatos.Columns.Count - 1
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
                                        hoja.Cell(filaExcel, columnaIndex + 1).Style.DateFormat.Format = "dd/MM/yyyy"
                                    Case GetType(Boolean)
                                        hoja.Cell(filaExcel, columnaIndex + 1).SetValue(Convert.ToBoolean(valorCelda))
                                    Case Else
                                        hoja.Cell(filaExcel, columnaIndex + 1).SetValue(valorCelda.ToString())
                                End Select
                            End If
                        Next
                        filaExcel += 1
                    Next
                Next

                hoja.Columns().AdjustToContents()
                workbook.SaveAs(rutaArchivo)
            End Using

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Async Function ProcesarConsultaDesdeExcelAsync(
    subcarpetaDestino As String,
    nombreArchivoSalida As String,
    nombreHoja As Object,
    columnaId As Integer,
    accionPorId As Func(Of Long, DataTable),
    NombreUsuarioEquipo As String,
    UsarExcel As Boolean) As Task

        Try
            Dim IdDocumentos As New List(Of Long)
            Dim rutaArchivo As String = ""

            Dim rutaBase = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO"
            Dim Destino = Path.Combine(rutaBase, subcarpetaDestino)

            If Not Directory.Exists(Destino) Then
                Directory.CreateDirectory(Destino)
            End If

            ' Si es true, leemos desde el Excel las IDs
            If UsarExcel Then
                Using openFileDialog1 As New OpenFileDialog
                    openFileDialog1.Title = "Seleccionar archivos"
                    openFileDialog1.Multiselect = False
                    openFileDialog1.Filter = "Todos los archivos (*.*)|*.*"

                    If openFileDialog1.ShowDialog = DialogResult.OK Then
                        rutaArchivo = openFileDialog1.FileName
                    End If
                End Using

                If String.IsNullOrEmpty(rutaArchivo) Then Return

                ' Leer IDs desde Excel
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial
                Using package As New ExcelPackage(New FileInfo(rutaArchivo))
                    Dim worksheet = If(TypeOf nombreHoja Is Integer,
                                   package.Workbook.Worksheets(CInt(nombreHoja)),
                                   package.Workbook.Worksheets(nombreHoja.ToString()))

                    Dim rowCount = worksheet.Dimension.Rows
                    For row = 2 To rowCount
                        Dim idTexto = worksheet.Cells(row, columnaId).Value?.ToString()
                        If Not String.IsNullOrEmpty(idTexto) AndAlso Long.TryParse(idTexto, Nothing) Then
                            IdDocumentos.Add(Convert.ToInt64(idTexto))
                        End If
                    Next
                End Using

                If IdDocumentos.Count = 0 Then
                    Throw New Exception("Sin registros en el Excel")
                End If
            End If

            Await Task.Run(Sub()
                               Dim rutaSalida = Path.Combine(Destino, nombreArchivoSalida)
                               Dim ListaTablas As New List(Of DataTable)
                               Dim lockLista As New Object()

                               If UsarExcel Then
                                   Dim tareas As New List(Of Task)
                                   Dim contadorFacturas As Integer = 0

                                   For Each id In IdDocumentos
                                       tareas.Add(Task.Run(Sub()
                                                               Try
                                                                   Dim tabla As DataTable = accionPorId(id)
                                                                   If tabla IsNot Nothing Then
                                                                       SyncLock lockLista
                                                                           ListaTablas.Add(tabla)
                                                                           contadorFacturas += 1
                                                                       End SyncLock
                                                                   End If
                                                               Catch ex As Exception
                                                                   ' Manejo de error individual
                                                               End Try
                                                           End Sub))

                                       If tareas.Count >= 1 Then
                                           Task.WaitAll(tareas.ToArray())
                                           tareas.Clear()

                                           ' Guardar si se llegó a 1000 facturas
                                           SyncLock lockLista
                                               If contadorFacturas >= 1 Then
                                                   ExportarConsultaAExcelV2(New List(Of DataTable)(ListaTablas), rutaSalida, Path.GetFileNameWithoutExtension(nombreArchivoSalida))
                                                   ListaTablas.Clear()
                                                   contadorFacturas = 0
                                               End If
                                           End SyncLock
                                       End If
                                   Next

                                   ' Esperar tareas restantes
                                   If tareas.Count > 0 Then Task.WaitAll(tareas.ToArray())

                                   ' Guardar cualquier resto de facturas que no alcanzaron las 1000
                                   SyncLock lockLista
                                       If ListaTablas.Count > 0 Then
                                           ExportarConsultaAExcelV2(ListaTablas, rutaSalida, Path.GetFileNameWithoutExtension(nombreArchivoSalida))
                                           ListaTablas.Clear()
                                       End If
                                   End SyncLock

                               Else
                                   ' Consulta sin ID (por combo)
                                   Try
                                       Dim tabla As DataTable = accionPorId(0)
                                       If tabla IsNot Nothing Then
                                           ListaTablas.Add(tabla)
                                       End If
                                   Catch ex As Exception
                                       Throw
                                   End Try

                                   ExportarConsultaAExcelV2(ListaTablas, rutaSalida, Path.GetFileNameWithoutExtension(nombreArchivoSalida))
                               End If
                           End Sub)


        Catch ex As Exception
            Throw
        End Try
    End Function


    'Public Async Function ProcesarConsultaDesdeExcelAsync(subcarpetaDestino As String, nombreArchivoSalida As String, nombreHoja As Object, columnaId As Integer, accionPorId As Func(Of Long, DataTable), NombreUsuarioEquipo As String, UsarExcel As Boolean) As Task
    '    Try


    '        Dim IdDocumentos As New List(Of Long)
    '        Dim rutaArchivo As String = ""

    '        Dim rutaBase = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO"
    '        Dim Destino = Path.Combine(rutaBase, subcarpetaDestino)

    '        If Not Directory.Exists(Destino) Then
    '            Directory.CreateDirectory(Destino)
    '        End If
    '        'Si es true, leemos desde el excel las ids
    '        If UsarExcel Then


    '            ' Selección de archivo
    '            Using openFileDialog1 As New OpenFileDialog
    '                openFileDialog1.Title = "Seleccionar archivos"
    '                openFileDialog1.Multiselect = False
    '                openFileDialog1.Filter = "Todos los archivos (*.*)|*.*"

    '                If openFileDialog1.ShowDialog = DialogResult.OK Then
    '                    rutaArchivo = openFileDialog1.FileName
    '                End If
    '            End Using

    '            If String.IsNullOrEmpty(rutaArchivo) Then Return

    '            ' Leer IDs desde Excel
    '            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
    '            Using package As New ExcelPackage(New FileInfo(rutaArchivo))
    '                Dim worksheet = If(TypeOf nombreHoja Is Integer,
    '                               package.Workbook.Worksheets(CInt(nombreHoja)),
    '                               package.Workbook.Worksheets(nombreHoja.ToString()))

    '                Dim rowCount = worksheet.Dimension.Rows
    '                For row = 2 To rowCount
    '                    Dim idTexto = worksheet.Cells(row, columnaId).Value?.ToString()
    '                    If Not String.IsNullOrEmpty(idTexto) AndAlso Long.TryParse(idTexto, Nothing) Then
    '                        IdDocumentos.Add(Convert.ToInt64(idTexto))
    '                    End If
    '                Next
    '            End Using

    '            If IdDocumentos.Count = 0 Then
    '                Throw New Exception("Sin registros en el excel")
    '            End If
    '        End If
    '        'PictureBox2.Visible = True

    '        Await Task.Run(Sub()
    '                           Dim ListaTablas As New List(Of DataTable)
    '                           Dim rutaSalida = Path.Combine(Destino, nombreArchivoSalida)
    '                           'si es true  se recorre la lista de iddocumentos
    '                           If UsarExcel Then
    '                               For Each id In IdDocumentos
    '                                   Try
    '                                       Dim tabla As DataTable = accionPorId(id)
    '                                       If tabla IsNot Nothing Then
    '                                           ListaTablas.Add(tabla)
    '                                       End If
    '                                   Catch ex As Exception
    '                                       ' Manejo de error por ID
    '                                   End Try
    '                               Next
    '                           Else
    '                               'si no, le pasamos directamente el datatable, lo añadimos a la lista y exportamos
    '                               Try
    '                                   ' le paso cero para que ignore la id
    '                                   Dim tabla As DataTable = accionPorId(0)
    '                                   If tabla IsNot Nothing Then
    '                                       ListaTablas.Add(tabla)
    '                                   End If
    '                               Catch ex As Exception
    '                                   Throw
    '                               End Try
    '                           End If
    '                           ExportarConsultaAExcelV2(ListaTablas, rutaSalida, Path.GetFileNameWithoutExtension(nombreArchivoSalida))

    '                       End Sub)

    '        'PictureBox2.Visible = False
    '    Catch ex As Exception
    '        Throw
    '    End Try
    'End Function


End Module
