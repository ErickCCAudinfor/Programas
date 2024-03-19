Imports System.Data.SqlClient

Imports ClosedXML.Excel
Public Class ValidacionExcel

    Public ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Sub EjecutarConsultasYGuardarEnExcel(consultas As List(Of String), nombreArchivo As String)
        Try
            ' Creamos un nuevo libro de Excel
            Using excelWorkbook As New XLWorkbook()
                ' Para cada consulta en la lista
                For i As Integer = 0 To consultas.Count - 1
                    Dim consulta As String = consultas(i)

                    ' Crear una conexión a la base de datos
                    Using connection As New SqlConnection(connectionString)
                        ' Abrir la conexión
                        connection.Open()

                        ' Crear un comando SQL para ejecutar la consulta
                        Using command As New SqlCommand(consulta, connection)
                            ' Crear un lector de datos para leer los resultados de la consulta
                            Using reader As SqlDataReader = command.ExecuteReader()
                                ' Crear una nueva hoja de cálculo en el libro de Excel
                                Dim excelWorksheet As IXLWorksheet = excelWorkbook.Worksheets.Add("Hoja" & (i + 1).ToString())

                                ' Escribir los nombres de las columnas como encabezados de columna en la primera fila
                                Dim headerRow As Integer = 1
                                Dim columnNames As New HashSet(Of String)() ' Usaremos esto para mantener un registro de los nombres de columna únicos

                                For j As Integer = 0 To reader.FieldCount - 1
                                    Dim columnName As String = reader.GetName(j)

                                    ' Si el nombre de la columna ya existe, agregar un sufijo numérico para hacerlo único
                                    Dim uniqueColumnName As String = columnName
                                    Dim suffix As Integer = 2

                                    While columnNames.Contains(uniqueColumnName)
                                        uniqueColumnName = columnName & suffix
                                        suffix += 1
                                    End While

                                    columnNames.Add(uniqueColumnName)

                                    excelWorksheet.Cell(headerRow, j + 1).Value = uniqueColumnName
                                    excelWorksheet.Cell(headerRow, j + 1).Style.Font.Bold = True ' Negrita
                                    excelWorksheet.Cell(headerRow, j + 1).Style.Fill.BackgroundColor = XLColor.Gray ' Fondo de color (por ejemplo, Aqua)
                                Next

                                ' Escribir los datos en la hoja de cálculo
                                Dim row As Integer = headerRow + 1
                                While reader.Read()
                                    For j As Integer = 0 To reader.FieldCount - 1
                                        Dim value As Object = reader.GetValue(j)
                                        ' Eliminar los saltos de línea
                                        If TypeOf value Is String Then
                                            value = value.ToString().Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ")
                                        End If
                                        ' Convertir el valor a un tipo compatible con ClosedXML
                                        If TypeOf value Is DBNull Then
                                            value = "NULL" ' Tratar valores DBNull como "NULL"
                                        ElseIf TypeOf value Is Boolean Then
                                            ' Tratar campos booleanos como 1 o 0 en lugar de True o False
                                            value = If(DirectCast(value, Boolean), 1, 0)
                                        End If

                                        ' Asignar el valor a la celda como String
                                        excelWorksheet.Cell(row, j + 1).Value = If(value IsNot Nothing, value.ToString(), "")
                                    Next
                                    row += 1
                                End While

                                ' Crear una tabla con estilos automáticamente
                                Dim lastRow As Integer = row - 1 ' La última fila escrita
                                Dim lastColumn As Integer = reader.FieldCount ' El número de columnas
                                Dim rng As IXLRange = excelWorksheet.Range(excelWorksheet.Cell(headerRow, 1), excelWorksheet.Cell(lastRow, lastColumn))
                                Dim table = rng.CreateTable()

                                ' Aplicar estilos adicionales a la tabla si es necesario
                                ' Por ejemplo:
                                'table.Style.Border.OutsideBorder = XLBorderStyleValues.
                                table.Style.Border.OutsideBorderColor = XLColor.Black
                            End Using
                        End Using
                    End Using
                Next

                ' Guardar el libro de Excel en el archivo especificado
                excelWorkbook.SaveAs(nombreArchivo)
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al ejecutar consultas y guardar en Excel: " & ex.Message)
        End Try
    End Sub

    'Public Sub EjecutarConsultasYGuardarEnExcelVariableGas(consultas As List(Of String), nombreArchivo As String)
    '    Try
    '        ' Creamos un nuevo libro de Excel
    '        Using excelWorkbook As New XLWorkbook()
    '            ' Para cada consulta en la lista
    '            For i As Integer = 0 To consultas.Count - 1
    '                Dim consulta As String = consultas(i)

    '                ' Crear una conexión a la base de datos
    '                Using connection As New SqlConnection(connectionString)
    '                    ' Abrir la conexión
    '                    connection.Open()

    '                    ' Crear un comando SQL para ejecutar la consulta
    '                    Using command As New SqlCommand(consulta, connection)
    '                        ' Crear un lector de datos para leer los resultados de la consulta
    '                        Using reader As SqlDataReader = command.ExecuteReader()
    '                            ' Crear una nueva hoja de cálculo en el libro de Excel
    '                            Dim excelWorksheet As IXLWorksheet = excelWorkbook.Worksheets.Add("Hoja" & (i + 1).ToString())

    '                            ' Escribir los nombres de las columnas como encabezados de columna en la primera fila
    '                            Dim headerRow As Integer = 1
    '                            Dim columnNames As New HashSet(Of String)() ' Usaremos esto para mantener un registro de los nombres de columna únicos

    '                            For j As Integer = 0 To reader.FieldCount - 1
    '                                Dim columnName As String = reader.GetName(j)

    '                                ' Si el nombre de la columna ya existe, agregar un sufijo numérico para hacerlo único
    '                                Dim uniqueColumnName As String = columnName
    '                                Dim suffix As Integer = 2

    '                                While columnNames.Contains(uniqueColumnName)
    '                                    uniqueColumnName = columnName & suffix
    '                                    suffix += 1
    '                                End While

    '                                columnNames.Add(uniqueColumnName)

    '                                excelWorksheet.Cell(headerRow, j + 1).Value = uniqueColumnName
    '                                excelWorksheet.Cell(headerRow, j + 1).Style.Font.Bold = True ' Negrita
    '                                excelWorksheet.Cell(headerRow, j + 1).Style.Fill.BackgroundColor = XLColor.Gray ' Fondo de color (por ejemplo, Aqua)
    '                            Next

    '                            ' Escribir los datos en la hoja de cálculo
    '                            Dim row As Integer = headerRow + 1
    '                            ' Escribir los datos en la hoja de cálculo
    '                            While reader.Read()
    '                                For j As Integer = 0 To reader.FieldCount - 1
    '                                    Dim value As Object = reader.GetValue(j)
    '                                    ' Eliminar los saltos de línea y convertir el valor a un tipo compatible con ClosedXML
    '                                    If TypeOf value Is String Then
    '                                        value = value.ToString().Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ")
    '                                    ElseIf TypeOf value Is DBNull Then
    '                                        value = "NULL" ' Tratar valores DBNull como "NULL"
    '                                    ElseIf TypeOf value Is Boolean Then
    '                                        ' Tratar campos booleanos como 1 o 0 en lugar de True o False
    '                                        value = If(DirectCast(value, Boolean), 1, 0)
    '                                    End If

    '                                    ' Si la columna es "Descripcion" o "ImporteBase"
    '                                    '' Si la columna es "Descripcion" o "ImporteBase"
    '                                    ' Variable para llevar un registro de la cantidad de columnas creadas para "Descripción"
    '                                    Dim descripcionColumnCount As Integer = 0

    '                                    ' ...

    '                                    ' Dentro del bucle For para leer los datos
    '                                    If reader.GetName(j) = "Descripcion" AndAlso value IsNot DBNull.Value Then
    '                                        Dim parts As String() = value.ToString().Split(New String() {"||"}, StringSplitOptions.None)
    '                                        Dim columnOffset As Integer = 0
    '                                        For Each part As String In parts
    '                                            columnOffset += 1
    '                                            Dim uniqueColumnName As String = "Descripcion_" & columnOffset.ToString()
    '                                            columnNames.Add(uniqueColumnName)
    '                                            excelWorksheet.Cell(headerRow, j + 1 + columnOffset).Value = uniqueColumnName
    '                                            excelWorksheet.Cell(row, j + 1 + columnOffset).Value = part
    '                                            descripcionColumnCount += 1 ' Incrementar la cuenta de columnas para "Descripción"
    '                                        Next
    '                                    ElseIf reader.GetName(j) = "ImporteBase" AndAlso value IsNot DBNull.Value Then
    '                                        Dim parts As String() = value.ToString().Split(New String() {"||"}, StringSplitOptions.None)
    '                                        For Each part As String In parts
    '                                            ' Empezar a escribir desde la siguiente columna después de la última columna creada para "Descripción"
    '                                            Dim columnIndex As Integer = j + descripcionColumnCount + 1
    '                                            excelWorksheet.Cell(headerRow, columnIndex).Value = "ImporteBase_" & columnIndex.ToString()
    '                                            excelWorksheet.Cell(row, columnIndex).Value = part
    '                                        Next
    '                                    End If

    '                                    'Else
    '                                    ' Asignar el valor a la celda como String
    '                                    excelWorksheet.Cell(row, j + 1).Value = If(value IsNot Nothing, value.ToString(), "")
    '                                    'End If


    '                                Next
    '                                row += 1
    '                            End While


    '                            ' Crear una tabla con estilos automáticamente
    '                            Dim lastRow As Integer = row - 1 ' La última fila escrita
    '                            Dim lastColumn As Integer = reader.FieldCount + columnNames.Count ' El número de columnas
    '                            Dim rng As IXLRange = excelWorksheet.Range(excelWorksheet.Cell(headerRow, 1), excelWorksheet.Cell(lastRow, lastColumn))
    '                            Dim table = rng.CreateTable()

    '                            ' Aplicar estilos adicionales a la tabla si es necesario
    '                            ' Por ejemplo:
    '                            'table.Style.Border.OutsideBorder = XLBorderStyleValues.
    '                            table.Style.Border.OutsideBorderColor = XLColor.Black
    '                        End Using
    '                    End Using
    '                End Using
    '            Next

    '            ' Guardar el libro de Excel en el archivo especificado
    '            excelWorkbook.SaveAs(nombreArchivo)
    '        End Using
    '    Catch ex As Exception
    '        MessageBox.Show("Error al ejecutar consultas y guardar en Excel: " & ex.Message)
    '    End Try
    'End Sub

End Class
