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
                                For j As Integer = 0 To reader.FieldCount - 1
                                    Dim columnName As String = reader.GetName(j)
                                    excelWorksheet.Cell(1, j + 1).Value = columnName
                                    excelWorksheet.Cell(1, j + 1).Style.Font.Bold = True ' Negrita
                                    excelWorksheet.Cell(1, j + 1).Style.Fill.BackgroundColor = XLColor.Aqua ' Fondo de color (por ejemplo, Aqua)
                                Next

                                ' Escribir los datos en la hoja de cálculo
                                Dim row As Integer = 2
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

End Class
