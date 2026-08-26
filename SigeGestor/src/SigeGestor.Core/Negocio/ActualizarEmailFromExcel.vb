Option Strict Off   ' Portado de ActualizaPrecios tal cual.

Imports System.IO
Imports System.Text.RegularExpressions
Imports SigeGestor.Core.Configuracion

''' <summary>
''' Actualiza el email y el teléfono o móvil de los contratos de un Excel.
'''
''' PORTADO LITERAL de ActualizaPrecios: son 200 líneas de casuística sobre ClienteContacto y
''' ContratoContacto —existe o no, borrar antes de insertar, móvil frente a fijo según el
''' primer dígito— y no es algo que convenga rehacer de memoria. Se quedó fuera del primer
''' porte porque dependía de EPPlus; ahora lo cubre el adaptador de CompatEPPlus.vb.
'''
''' NO se ha traído ConsultaCNAE, el otro método de la clase original: era una prueba de un
''' ticket concreto, con la ruta y el nombre del fichero escritos a mano (SOPTOT-10083).
''' </summary>
Public Class ActualizarEmailFromExcel

    Public Property connectionString As String

    Public Property RutaExcel As String

    Public Sub New(connectionString As String)
        Me.connectionString = connectionString
    End Sub

    ''' <summary>
    ''' Incidencias de la última ejecución: filas sin datos, contratos que no existen, emails
    ''' mal formados. El original las escribía en un fichero y no las devolvía, así que la
    ''' interfaz no podía decir cuántas había habido.
    ''' </summary>
    Public ReadOnly Property Incidencias As New List(Of String)

    ''' <summary>Ruta del fichero de incidencias, si se ha escrito alguno.</summary>
    Public Property RutaIncidencias As String = ""

    Private ReadOnly Property NombreUsuarioEquipo As String = Environment.UserName

    Public Async Function ActualizarEmailFromExcelAsync() As Task(Of Long)
        Dim contador = 0L
        Dim contadorTlfnoMovil = 0L
        Dim excelFilePath As String = $"{RutaExcel}"
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
                    Dim TlfnoMovil As String = If(worksheet.Cells(row, 3).Value IsNot Nothing, worksheet.Cells(row, 3).Value?.ToString(), String.Empty)
                    ' Verificar si el email está vacío
                    Dim SaltarEMail = False
                    If String.IsNullOrEmpty(Emails) AndAlso String.IsNullOrEmpty(TlfnoMovil) Then
                        ' Agregar un mensaje indicando que el código CNAE está vacío
                        Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - sin email en fila {row} y sín móvil"})
                        Continue For ' Saltar al siguiente ciclo
                    End If
                    If Not String.IsNullOrEmpty(Emails) AndAlso String.IsNullOrEmpty(TlfnoMovil) Then
                        SaltarEMail = False
                    End If
                    If String.IsNullOrEmpty(Emails) AndAlso Not String.IsNullOrEmpty(TlfnoMovil) Then
                        SaltarEMail = True
                    End If
                    ' Aquí puedes continuar con el procesamiento de los emails válidos
                    ' ...
                    Dim ContratoA = Await Task.Run(Function() funciones.GetContrato(CLng(CodContrato)))
                    If IsNothing(ContratoA) OrElse ContratoA.IdContrato = 0 Then
                        Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - No existe en BD "})
                        Continue For ' Saltar al siguiente ciclo
                    End If
                    If Not SaltarEMail Then
#Region "Validador email"
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

#End Region
                        If Not regexMatch Then
                            ' Ninguno de los emails es válido
                            Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - email NO valido {row}"})
                        Else
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
                                Else
                                    Dim InsertCc = Await Task.Run(Function() funciones.InsertContractoContacto(If(ContratoA.Entorno = "E1", "G1", "G2"), ContratoA.CodigoContrato, objClienteContacto.IdClienteContacto))
                                    If InsertCc > 0 Then
                                        Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - actualizado - email: {correosConcatenados} "})
                                        contador += 1
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
                        End If

                    End If
                    If String.IsNullOrEmpty(TlfnoMovil) Then
                        ' Agregar un mensaje indicando que el código CNAE está vacío
                        Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - sin tlfno en fila {row}"})
                    Else
                        'Busco el cliente y el tlfnoMovil en ClienteContacto de BD si existe me guardo el clientecontacto
                        Dim IstlfnoText = TipoTelefono(TlfnoMovil)
                        If IstlfnoText.Equals("Número no válido") Then
                            Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato}- TlfnoMovil erróneo: {TlfnoMovil}"})
                            Continue For
                        End If
                        Dim objClienteContactotlfno = Await Task.Run(Function() funciones.GetClienteContacto(ContratoA.IdCliente, TlfnoMovil))
                        If Not IsNothing(objClienteContactotlfno) AndAlso objClienteContactotlfno.IdClienteContacto > 0 Then
                            'habra quitar el tlfno si ya habia uno marcado en el contrato
                            Dim objContratoContactoTlfnoMovil = Await Task.Run(Function() funciones.GetContratoContactobyCodContratoTlfno(ContratoA.CodigoContrato, objClienteContactotlfno.TipoContacto))
                            If Not IsNothing(objContratoContactoTlfnoMovil) AndAlso objContratoContactoTlfnoMovil.IdContratoContacto > 0 Then
                                'Borro el tlfno de ContratoContacto
                                Dim borrado = Await Task.Run(Function() funciones.DeleteContratoContactobyIdContratoContacto(objContratoContactoTlfnoMovil.IdContratoContacto))
                                If borrado > 0 Then ' si se ha borrado hago el insert
                                    ' hago el insert en conctratocontacto
                                    Dim InsertCc = Await Task.Run(Function() funciones.InsertContractoContacto(objContratoContactoTlfnoMovil.Entorno, ContratoA.CodigoContrato, objClienteContactotlfno.IdClienteContacto))
                                    If InsertCc > 0 Then
                                        Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - actualizado - TlfnoMovil: {TlfnoMovil} "})
                                        contadorTlfnoMovil += 1
                                    End If
                                End If
                            End If
                        Else  ' no existe lo inserto
                            Dim Istlfno = If(TipoTelefono(TlfnoMovil).Equals("T"), True, False) 'Compruebo si Movil o TLFNO
                            'Se inserta el nuevo tlfnoMovil en clientecontacto
                            Dim InsertCL = Await Task.Run(Function() funciones.InsertClienteContactoTlfnoMovil(ContratoA.IdCliente, TlfnoMovil, Istlfno))
                            If InsertCL > 0 Then
                                'nos obtenemos de nuevo el ClienteContacto
                                Dim objClienteContacto3 = Await Task.Run(Function() funciones.GetClienteContacto(ContratoA.IdCliente, TlfnoMovil))
                                If Not IsNothing(objClienteContacto3) AndAlso objClienteContacto3.IdClienteContacto > 0 Then
                                    'habra quitar el tlfono si ya habia uno marcado en el contrato
                                    Dim objContratoContactov4 = Await Task.Run(Function() funciones.GetContratoContactobyCodContratoTlfno(ContratoA.CodigoContrato, objClienteContacto3.TipoContacto))
                                    If Not IsNothing(objContratoContactov4) AndAlso objContratoContactov4.IdContratoContacto > 0 Then
                                        'Borro el tlfnoMovil de ContratoContacto
                                        Dim borrado = Await Task.Run(Function() funciones.DeleteContratoContactobyIdContratoContacto(objContratoContactov4.IdContratoContacto))
                                        If borrado > 0 Then ' si se ha borrado hago el insert
                                            ' hago el insert en conctratocontacto
                                            Dim InsertCc = Await Task.Run(Function() funciones.InsertContractoContacto(objContratoContactov4.Entorno, ContratoA.CodigoContrato, objClienteContacto3.IdClienteContacto))
                                            If InsertCc > 0 Then
                                                Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - actualizado - TlfnoMovil: {TlfnoMovil} "})
                                                contadorTlfnoMovil += 1
                                            End If
                                        End If
                                    Else
                                        Dim InsertCc = Await Task.Run(Function() funciones.InsertContractoContacto(objClienteContacto3.Entorno, ContratoA.CodigoContrato, objClienteContacto3.IdClienteContacto))
                                        If InsertCc > 0 Then
                                            Datos.Add(New List(Of Object) From {$"Contrato: {CodContrato} - actualizado - TlfnoMovil: {TlfnoMovil} "})
                                            contadorTlfnoMovil += 1
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If


                Next
            End Using

            'MessageBox.Show($"Proceso Completado")
        Catch ex As Exception
            ' Manejo de excepciones
            Throw
        Finally
            'Si hay datos escribo en el excel
            ' CAMBIADO: el original volcaba esto a un Excel en el escritorio raso con la clase
            ' Excel, que no se ha portado. Va a un .txt en ConsultasBO\Contactos, que es donde
            ' está el resto, y con hora en el nombre para no pisar el de la ejecución anterior.
            If Datos.Count > 0 Then
                EscribirIncidencias(Datos)
            End If
        End Try

        Return contador
    End Function

    Private Function TipoTelefono(numero As String) As String
        numero = numero.Trim().Replace(" ", "").Replace("-", "")

        If numero.Length <> 9 OrElse Not IsNumeric(numero) Then
            Return "Número no válido"
        End If

        Select Case numero.Substring(0, 1)
            Case "6", "7"
                Return "M"
            Case "8", "9"
                Return "T"
            Case Else
                Return "T"
        End Select
    End Function
    Private Sub EscribirIncidencias(datos As List(Of List(Of Object)))

        For Each fila In datos
            If fila Is Nothing OrElse fila.Count = 0 Then Continue For
            Incidencias.Add(CStr(fila(0)))
        Next

        Try
            Dim carpeta = RutasSalida.Asegurar("Contactos")
            RutaIncidencias = Path.Combine(carpeta, $"Email_{DateTime.Now:yyyyMMdd_HHmm}.txt")
            File.WriteAllLines(RutaIncidencias, Incidencias)
        Catch ex As Exception
            ' Que no se pueda escribir el log no invalida los contratos ya actualizados.
            RutaIncidencias = ""
        End Try

    End Sub

End Class
