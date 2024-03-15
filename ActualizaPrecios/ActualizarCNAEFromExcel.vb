Imports System.IO
Imports OfficeOpenXml
Imports System.Data.SqlClient
Imports DocumentFormat.OpenXml.Drawing.Charts

Public Class ActualizarCNAEFromExcel
    Private _connectionString As String
    Public Property connectionString() As String
        Get
            Return _connectionString
        End Get
        Set(ByVal value As String)
            _connectionString = value
        End Set
    End Property

    Private _RutaExcel As String
    Public Property RutaExcel() As String
        Get
            Return _RutaExcel
        End Get
        Set(ByVal value As String)
            _RutaExcel = value
        End Set
    End Property

    Public Sub New(connectionString As String)
        Me.connectionString = connectionString
    End Sub
    Private _datos As New List(Of List(Of Object))()
    Public Property Datos() As List(Of List(Of Object))
        Get
            Return _datos
        End Get
        Set(ByVal value As List(Of List(Of Object)))
            _datos = value
        End Set
    End Property


    Private ReadOnly Property NombreUsuarioEquipo As String = Environment.UserName
    Public Async Function ActualizarCNAEFromExcelAsync() As Task(Of Long)
        Dim contador = 0L
        Dim excelFilePath As String = $"{RutaExcel}"
        Dim Excel As New Excel
        Try

            'Dim datos As New List(Of List(Of Object))()

            'Esto por que estoy usando una licencia no comercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            ' Abrir el archivo de Excel
            Dim funciones As New FuncionesGenericas(connectionString)
            Using package As New ExcelPackage(New FileInfo(excelFilePath))
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)

                Dim rowCount As Integer = worksheet.Dimension.Rows

                ' Iterar sobre cada fila del archivo Excel
                For row As Integer = 2 To rowCount ' Empezamos en la fila 2 para ignorar el encabezado
                    Dim cif As String = worksheet.Cells(row, 1).Value.ToString()
                    Dim codCnae As String = If(worksheet.Cells(row, 2).Value IsNot Nothing, worksheet.Cells(row, 2).Value.ToString(), String.Empty)

                    ' Verificar si el código CNAE está vacío
                    If String.IsNullOrEmpty(codCnae) Then
                        ' Agregar un mensaje indicando que el código CNAE está vacío
                        Datos.Add(New List(Of Object) From {$"CIF: {cif} _ La celda CNAE está vacía en la fila " & row})
                        Continue For ' Saltar al siguiente ciclo
                    End If
                    'Buscamos el CNAE antes de seguir con los contratos

                    Dim objCnae = Await Task.Run(Function() funciones.getCNAEbyCodigo(codCnae))
                    'Si hay CNAE  buscaremos los contratos asociados a ese CIF
                    If Not IsNothing(objCnae) AndAlso objCnae.IdCNAE > 0 AndAlso cif.Trim.Length > 0 Then
                        ' Buscar en la base de datos el CIF, y obtengo los contratos asociados a ese CIF
                        Dim Contract = Await Task.Run(Function() funciones.GetListContratobyCIF(cif))
                        If Not IsNothing(Contract) AndAlso Contract.Count > 0 Then
                            For Each elemnt In Contract
                                'Hago el update
                                Dim isOK = Await Task.Run(Function() funciones.UpdateContratoCNAE(elemnt.CodigoContrato, objCnae.IdCNAE))
                                If isOK = 0 Then
                                    'Si de vuelve 0 algo ha fallado por lo que escribo en el fichero
                                    Datos.Add(New List(Of Object) From {$"{cif} -> contrato: {elemnt.CodigoContrato} NO se ha actualizado"})
                                Else
                                    contador += 1
                                    Datos.Add(New List(Of Object) From {$"{cif} -> contrato: {elemnt.CodigoContrato} Actualizado."})
                                End If
                            Next
                        Else
                            Datos.Add(New List(Of Object) From {$"{cif} -> No tiene contratos"})
                        End If
                    Else  'Si no hay CNAE, escribo el CNAE y el CIF
                        Datos.Add(New List(Of Object) From {$"CNAE: {codCnae} del cliente: {cif} no se ha encontrado en la Base de datos."})
                    End If
                Next
            End Using
            'Si hay datos escribo en el excel
            If Datos.Count > 0 Then
                Excel.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos)
            End If

            MessageBox.Show($"Proceso Completado")
        Catch ex As Exception
            If Datos.Count > 0 Then
                Excel.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos)
            End If
            Throw
        End Try
        Return contador
    End Function

End Class
