Imports OfficeOpenXml
Imports System.IO
Imports System.Text.RegularExpressions

Public Class ActualizarEmailFromExcel
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


    Public Async Function ActualizarEmailFromExcelAsync() As Task(Of Long)
        Dim contador = 0L
        Dim excelFilePath As String = $"{RutaExcel}"
        Dim Excel As New Excel
        Dim Datos As New List(Of List(Of Object))()

        Try
            'Esto por que estoy usando una licencia no comercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            ' Abrir el archivo de Excel
            Dim funciones As New FuncionesGenericas(connectionString)
            Using package As New ExcelPackage(New FileInfo(excelFilePath))
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)

                Dim rowCount As Integer = worksheet.Dimension.Rows

                ' Iterar sobre cada fila del archivo Excel
                For row As Integer = 2 To rowCount ' Empezamos en la fila 2 para ignorar el encabezado
                    Dim CodContrato As String = worksheet.Cells(row, 1).Value?.ToString()
                    Dim Emails As String = If(worksheet.Cells(row, 2).Value IsNot Nothing, worksheet.Cells(row, 2).Value?.ToString(), String.Empty)

                    ' Verificar si el email está vacío
                    If String.IsNullOrEmpty(Emails) Then
                        ' Agregar un mensaje indicando que el código CNAE está vacío
                        Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - sin email en fila {row}"})
                        Continue For ' Saltar al siguiente ciclo
                    End If

                    ' Lista para almacenar correos electrónicos válidos
                    Dim correosValidos As New List(Of String)()

                    ' Verificar si al menos uno de los correos electrónicos es válido utilizando una expresión regular
                    Dim emailsArray As String() = Emails.Split(";"c)
                    Dim regexPattern As String = "^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$"
                    Dim regexMatch As Boolean = False

                    For Each email As String In emailsArray
                        Dim trimmedEmail As String = email.Trim() ' Eliminar espacios adicionales alrededor del correo electrónico
                        If Regex.IsMatch(trimmedEmail, regexPattern) Then
                            correosValidos.Add(trimmedEmail) ' Agregar correo electrónico válido a la lista
                            regexMatch = True
                        Else
                            ' El email no coincide con el formato esperado
                            Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - email no válido '{trimmedEmail}' en fila {row}"})
                        End If
                    Next

                    ' Concatenar correos electrónicos válidos en una sola cadena sin espacios adicionales
                    Dim correosConcatenados As String = String.Join(";", correosValidos)

                    If Not regexMatch Then
                        ' Ninguno de los emails es válido
                        Continue For ' Saltar al siguiente ciclo
                    End If

                    ' Aquí puedes continuar con el procesamiento de los emails válidos
                    ' ...
                    Dim ContratoA = Await Task.Run(Function() funciones.GetContrato(CLng(CodContrato)))
                    If IsNothing(ContratoA) OrElse ContratoA.IdContrato = 0 Then
                        Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - No existe en BD "})
                        Continue For ' Saltar al siguiente ciclo
                    End If




                    'Busco el cliente y el email en ClienteContacto de BD si existe me guardo el clientecontacto
                    Dim objClienteContacto = Await Task.Run(Function() funciones.GetClienteContacto(ContratoA.IdCliente, correosConcatenados))
                    If Not IsNothing(objClienteContacto) AndAlso objClienteContacto.IdClienteContacto > 0 Then
                        'habra quitar el email si ya habia uno marcado en el contrato
                        Dim objContratoContacto = Await Task.Run(Function() funciones.GetContratoContactobyCodContratoEmail(ContratoA.CodigoContrato))
                        If Not IsNothing(objContratoContacto) AndAlso objContratoContacto.IdContratoContacto > 0 Then
                            'Borro el email de ContratoContacto
                            Dim borrado = Await Task.Run(Function() funciones.DeleteContratoContactobyIdContratoContacto(objContratoContacto.IdContratoContacto))
                            If borrado > 0 Then ' si se ha borrado hago el insert
                                ' hago el insert en conctratocontacto
                                Dim InsertCc = Await Task.Run(Function() funciones.InsertContractoContacto(objContratoContacto.Entorno, ContratoA.CodigoContrato, objClienteContacto.IdClienteContacto))
                                If InsertCc > 0 Then
                                    Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - actualizado - email: {correosConcatenados} "})
                                    contador += 1
                                End If
                            End If
                        End If
                    Else  ' no existe lo inserto
                        'Se inserta el nuevo email en clientecontacto
                        Dim InsertCL = Await Task.Run(Function() funciones.InsertClienteContactoEmail(ContratoA.IdCliente, correosConcatenados))
                        If InsertCL > 0 Then
                            'nos obtenemos de nuevo el ClienteContacto
                            Dim objClienteContacto2 = Await Task.Run(Function() funciones.GetClienteContacto(ContratoA.IdCliente, correosConcatenados))
                            If Not IsNothing(objClienteContacto2) AndAlso objClienteContacto2.IdClienteContacto > 0 Then
                                'habra quitar el email si ya habia uno marcado en el contrato
                                Dim objContratoContacto = Await Task.Run(Function() funciones.GetContratoContactobyCodContratoEmail(ContratoA.CodigoContrato))
                                If Not IsNothing(objContratoContacto) AndAlso objContratoContacto.IdContratoContacto > 0 Then
                                    'Borro el email de ContratoContacto
                                    Dim borrado = Await Task.Run(Function() funciones.DeleteContratoContactobyIdContratoContacto(objContratoContacto.IdContratoContacto))
                                    If borrado > 0 Then ' si se ha borrado hago el insert
                                        ' hago el insert en conctratocontacto
                                        Dim InsertCc = Await Task.Run(Function() funciones.InsertContractoContacto(objContratoContacto.Entorno, ContratoA.CodigoContrato, objClienteContacto2.IdClienteContacto))
                                        If InsertCc > 0 Then
                                            Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - actualizado - email: {correosConcatenados} "})
                                            contador += 1
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
            End Using

            'Si hay datos escribo en el excel
            If Datos.Count > 0 Then
                Excel.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos)
            End If

            'MessageBox.Show($"Proceso Completado")
        Catch ex As Exception
            Excel.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos)
            ' Manejo de excepciones

            Throw
        End Try

        Return contador
    End Function



    'Buscamos el contrato antes de seguir con los contratos

    'Dim objCnae = Await Task.Run(Function() funciones.getCNAEbyCodigo(codCnae))
    ''Si hay CNAE  buscaremos los contratos asociados a ese CIF
    'If Not IsNothing(objCnae) AndAlso objCnae.IdCNAE > 0 AndAlso cif.Trim.Length > 0 Then
    '    ' Buscar en la base de datos el CIF, y obtengo los contratos asociados a ese CIF
    '    Dim Contract = Await Task.Run(Function() funciones.GetListContratobyCIF(cif))
    '    If Not IsNothing(Contract) AndAlso Contract.Count > 0 Then
    '        For Each elemnt In Contract
    '            'Hago el update
    '            Dim isOK = Await Task.Run(Function() funciones.UpdateContratoCNAE(elemnt.CodigoContrato, objCnae.IdCNAE))
    '            If isOK = 0 Then
    '                'Si de vuelve 0 algo ha fallado por lo que escribo en el fichero
    '                Datos.Add(New List(Of Object) From {$"{cif} -> contrato: {elemnt.CodigoContrato} NO se ha actualizado"})
    '            Else
    '                contador += 1
    '                Datos.Add(New List(Of Object) From {$"{cif} -> contrato: {elemnt.CodigoContrato} Actualizado."})
    '            End If
    '        Next
    '    Else
    '        Datos.Add(New List(Of Object) From {$"{cif} -> No tiene contratos"})
    '    End If
    'Else  'Si no hay CNAE, escribo el CNAE y el CIF
    '    Datos.Add(New List(Of Object) From {$"CNAE: {codCnae} del cliente: {cif} no se ha encontrado en la Base de datos."})
    'End If
End Class
