
Imports System.Data.OleDb
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml.Wordprocessing
Imports OfficeOpenXml

Public Class Form1
    Dim complementos As New Complementos()
    'Dim LoadingWF As New LoadingWF
    Private Property ipDB As String = "data source=172.31.100.12"
    'Private ReadOnly Property ipDB As String = "data source=172.31.100.50\TOTALUAT;"
    Private Property nameDB As String = "initial catalog=SigeTotal;"
    'Private ReadOnly Property nameDB As String = "initial catalog=SigeTotalUAT;"
    Private ReadOnly Property userDB As String = "User ID=Sige;"
    Private ReadOnly Property passDB As String = "Password=SigeNew;"
    Private ReadOnly Property NombreUsuarioEquipo As String = Environment.UserName
    Private Property connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"

    Private ContratoTarifaSrv As New ContratoTarifaSrv(connectionString)

    Private Funciones As New FuncionesGenericas(connectionString)

    Private isExpanded As Boolean = False ' Para rastrear si la pestaña está expandida o contraída

    Private Async Sub Actualizar(sender As Object, e As EventArgs) Handles Button1.Click
        Dim ExcelDatos As New Excel
        Dim Datos As New List(Of List(Of Object))
        Try
            Dim totalContratos = 0
            Dim ContratoActualizar As New List(Of Long)
            Dim Con As New List(Of Long)
            Con = GetConSinSplit(TextBox2.Text)

            If CheckBox1.Checked Then 'CUPS
                'Dim Cups As New List(Of String)
                'Cups.Add("ES0027700038574004TJ")
                'Contratos = Funciones.BuscarbyCups(Cups, ipDB, nameDB, userDB, passDB)

            End If
            If CheckBox2.Checked Then 'Contrato
                Dim yesorNot1 = MsgBox($"Hay un total de {Con.Count} contratos, ¿Seguir con la actualización?", vbYesNo)
                If yesorNot1 = 6 OrElse yesorNot1 = 1 Then
                    PictureBox2.Visible = True
                    ContratoActualizar = Await Task.Run(Function() Funciones.BuscarbyCodigocontrato(Con))
                    totalContratos = Con.Count
                    PictureBox2.Visible = False
                End If
            End If
            'If CheckBox3.Checked Then 'Cliente

            'End If
            Dim yesorNot As MsgBoxResult
            Dim todoOK = False
            'Escribo los valores que tiene ahora, para posteriormente comparar o hacer uso de este y dejarlo como esta
            Funciones.EscribirContratoTarifaAntesCambios(ContratoActualizar)
            If ContratoActualizar.Count > 0 Then
                If totalContratos <> ContratoActualizar.Count Then
                    yesorNot = MsgBox("Los contratos filtratos y los contratos encontrados no coinciden. ¿Actualizar de todas formas?", vbYesNo)
                Else
                    todoOK = True
                End If
                If yesorNot = 6 OrElse yesorNot = 1 OrElse todoOK Then
                    PictureBox2.Visible = True
                    Dim ContratosTXT = Await Task.Run(Function() ActualizarRegistros(ContratoActualizar, Datos))
                    PictureBox2.Visible = False

                    If Not IsNothing(Datos) AndAlso Datos.Count > 0 Then
                        ExcelDatos.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos, "PreciosErrores")
                    End If
                    complementos.MostrarMensajePersonalizado($"{ContratosTXT}. Contratos iniciales:{ContratoActualizar.Count} contratos")
                Else
                    complementos.MostrarMensajePersonalizado($"Se ha cancelado la actualización")
                End If
            Else
                complementos.MostrarMensajePersonalizado($"Sin Contratos")
            End If

        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado("Exception: " + ex.Message)
        End Try

    End Sub

    Private Function ActualizarRegistros(ListaCodigo As List(Of Long), ByRef Datos As List(Of List(Of Object))) As String
        Dim Contador = 0I
        Dim ContratosSinActualizar = "Contratos sin actualizarse: "

        'Dim Datos As New List(Of List(Of Object))()
        Try
            Dim ContratoTra As New List(Of ContratoTarifa)
            Dim ContratoTraMergeado As New List(Of ContratoTarifa)

            'Me busco solo contratos que tengan fechaHasta is null y sea personalizada
            For Each cod In ListaCodigo
                ContratoTra.Add(ContratoTarifaSrv.GetContratoTarifaPersonalizadaByCodigoContrato(cod, TextBox3.Text, CheckBox4.Checked))
            Next

            Dim pepe = 1
            ' Dependiendo de si el PerfilFacturacion del ContratoTarifa es indexado,
            ' cargaremos los precios de IndexadoPrecioSrv o TarifaPrecioSrv.
            ' Estos precios serán los mismos que se muestran en el programa de presupuestos,
            ' los vigentes para una Tarifa, TarifaGrupo y fecha concreta.

            ' Guardamos el nuevo ContratoTarifa, le pasamos la tarifagrupo a asignar
            If ListaCodigo.Count = ContratoTra.Count Then

                For Each elment In ContratoTra

                    If Not IsNothing(elment.IdContratoTarifa) AndAlso elment.IdContratoTarifa > 0 Then
                        'ContratoTraMergeado.Add(ContratoTarifaSrv.UpdateContratoTarifa(elment, NuevaTarifaGrupo, TarifaGrupoActual, IsPersonalizada))
                        Dim ContratoActualizar = ContratoTarifaSrv.UpdateContratoTarifa(elment, TextBox1.Text, TextBox3.Text, CheckBox4.Checked)
                        'Dim ContratoTarifaViejo = elment
                        If Not IsNothing(ContratoActualizar) AndAlso ContratoActualizar.IdContratoTarifa > 0 Then
                            ' Dependiendo de si el PerfilFacturacion del ContratoTarifa es indexado,
                            ' cargaremos los precios de IndexadoPrecioSrv o TarifaPrecioSrv.
                            ' Estos precios serán los mismos que se muestran en el programa de presupuestos,
                            ' los vigentes para una Tarifa, TarifaGrupo y fecha concreta.
                            Dim Contrato As Contrato = Funciones.GetContrato(If(elment.CodigoContrato, 0L))
                            Dim FechaPresupuesto As DateTime = DateTime.Today 'New DateTime(Now.Year, Now.Month, Now.Day)
                            If Not IsNothing(Contrato) AndAlso Contrato.IdContrato > 0L Then
                                If Not IsNothing(Contrato.FechaAplicacionPrecios) AndAlso Contrato.FechaAplicacionPrecios > DateTime.MinValue Then
                                    FechaPresupuesto = If(Contrato.FechaAplicacionPrecios, DateTime.Now)
                                Else
                                    If Not IsNothing(Contrato.FechaContrato) AndAlso Contrato.FechaContrato > DateTime.MinValue Then
                                        FechaPresupuesto = If(Contrato.FechaContrato, DateTime.Now)
                                    End If
                                End If

                            End If



                            If Not IsNothing(ContratoActualizar) Then
                                ContratoActualizar.PerfilFacturacion = Funciones.GetPerfilFacturacion(If(ContratoActualizar.IdPerfilFacturacion, 0))

                                If Not IsNothing(ContratoActualizar.PerfilFacturacion) Then
                                    Dim tarifasPrecioContratoGuardar As New List(Of TarifaPrecioContrato) ' Lista para guardar los nuevos precios
                                    Dim OldtarifasPrecioContratoQuitar As New List(Of TarifaPrecioContrato) ' Lista a con los viejos precios
                                    'Dim TarifaPerdidaCalculadaContratoGuardar As New ObservableCollection(Of TarifaPerdidaCalculadaContratoDTO)
                                    Dim isFijoIndex As Boolean = False
                                    'Compruebo si el nuevocontratotarifa es fijo o indexado
                                    If ContratoActualizar.PerfilFacturacion.isPerfilIndexado() Then
                                        isFijoIndex = True
                                        If ContratoActualizar.Entorno = "G1" Then

                                            Dim indexadosPrecios As List(Of IndexadoPrecio) = Funciones.GetDTOAllPeriodosIndx(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto).OrderBy(Function(f) f.IdIndexadoPrecio).ToList
                                            ''Avisar si no hay precios para grabar
                                            If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
                                                For Each eleindexadoPrecio As IndexadoPrecio In indexadosPrecios
                                                    Dim tarifaPrecioContrato As New TarifaPrecioContrato

                                                    tarifaPrecioContrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
                                                    tarifaPrecioContrato.IdIndexadoPrecio = eleindexadoPrecio.IdIndexadoPrecio
                                                    tarifaPrecioContrato.TextoTarifaPeriodo = eleindexadoPrecio.TextoTarifaPeriodo
                                                    tarifaPrecioContrato.IdTarifaPeriodo = eleindexadoPrecio.IdTarifaPeriodo
                                                    tarifaPrecioContrato.Entorno = eleindexadoPrecio.Entorno
                                                    tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
                                                Next
                                            Else
                                                ''Avisar si no hay precios para grabar
                                                'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_TARIFA_PRECIO_VIGENTE"))
                                            End If
                                        Else
                                            Dim indexadosPrecios As List(Of IndexadoPrecioGas) = Funciones.GetDTOAllPeriodosIndxGas(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto).OrderBy(Function(f) f.IdIndexadoPrecioGas).ToList

                                            ''Avisar si no hay precios para grabar
                                            If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
                                                For Each indexadoPrecio As IndexadoPrecioGas In indexadosPrecios
                                                    Dim tarifaPrecioContrato As New TarifaPrecioContrato

                                                    tarifaPrecioContrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
                                                    tarifaPrecioContrato.IdIndexadoPrecioGas = indexadoPrecio.IdIndexadoPrecioGas
                                                    tarifaPrecioContrato.IdTarifaPeriodo = indexadoPrecio.IdTarifaPeriodo
                                                    tarifaPrecioContrato.TextoTarifaPeriodo = indexadoPrecio.TextoTarifaPeriodo
                                                    tarifaPrecioContrato.Entorno = indexadoPrecio.Entorno
                                                    tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
                                                Next
                                            End If
                                        End If

                                    Else
                                        Dim tarifasPrecios As List(Of TarifaPrecio) = Funciones.GetDTOAllPeriodosTarifaPrecio(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto).OrderBy(Function(f) f.IdTarifaPrecio).ToList

                                        If Not IsNothing(tarifasPrecios) AndAlso tarifasPrecios.Count > 0 Then
                                            For Each tarifaPrecio As TarifaPrecio In tarifasPrecios
                                                Dim tarifapreciocontrato As New TarifaPrecioContrato

                                                tarifapreciocontrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
                                                tarifapreciocontrato.IdTarifaPrecio = tarifaPrecio.IdTarifaPrecio
                                                tarifapreciocontrato.IdTarifaPeriodo = tarifaPrecio.IdTarifaPeriodo
                                                tarifapreciocontrato.TextoTarifaPeriodo = tarifaPrecio.TextoTarifaPeriodo
                                                tarifapreciocontrato.Entorno = tarifaPrecio.Entorno
                                                tarifasPrecioContratoGuardar.Add(tarifapreciocontrato)
                                            Next
                                        Else
                                            ''Avisar si no hay precios para grabar
                                            'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_TARIFA_PRECIO_VIGENTE"))
                                        End If

                                    End If
                                    If elment.TextoTarifa.Contains("TDVE") Then

                                        Funciones.InsertTarifaPrecioContrato(tarifasPrecioContratoGuardar)
                                        Contador += 1
                                    Else

                                        'Compruebo si el contratotarifaviejo es fijo o indexado
                                        Dim TaPrecionContrato = Funciones.GetPrecioContratoTarifaV2(elment)
                                        If Not IsNothing(TaPrecionContrato) AndAlso TaPrecionContrato.IdContratoTarifa > 0 Then
                                            If elment.Entorno = "G1" AndAlso TaPrecionContrato.IdIndexadoPrecio > 0 Then
                                                OldtarifasPrecioContratoQuitar = Funciones.GetPrecioContratoTarifaIndex(elment).OrderBy(Function(f) f.IdTarifaPeriodo).ToList
                                            ElseIf elment.Entorno = "G1" AndAlso TaPrecionContrato.IdTarifaPrecio > 0 Then
                                                OldtarifasPrecioContratoQuitar = Funciones.GetPrecioContratoTarifa(elment).OrderBy(Function(f) f.IdTarifaPeriodo).ToList
                                            ElseIf elment.Entorno = "G2" AndAlso TaPrecionContrato.IdIndexadoPrecioGas > 0 Then
                                                OldtarifasPrecioContratoQuitar = Funciones.GetPrecioContratoTarifaIndexGas(elment).OrderBy(Function(f) f.IdTarifaPeriodo).ToList
                                            Else
                                                Datos.Add(New List(Of Object) From {$"Contrato: {elment.CodigoContrato}- idContratotarifa ={elment.IdContratoTarifa} - No actualizado - Sin precios - revisar "})
                                                Continue For
                                            End If
                                        Else
                                            Datos.Add(New List(Of Object) From {$"Contrato: {elment.CodigoContrato} - idContratotarifa ={elment.IdContratoTarifa} - El precio personalizado no existe, Imposible actualizar a la nueva tarifagrupo"})
                                            Continue For
                                            'Throw New Exception("El precio personalizado no existe, Imposible actualizar a la nueva tarifagrupo " + elment.CodigoContrato)
                                        End If


                                        'Compruebo que haya valores en los dos sitios
                                        If Not IsNothing(tarifasPrecioContratoGuardar) AndAlso tarifasPrecioContratoGuardar.Count > 0 AndAlso Not IsNothing(OldtarifasPrecioContratoQuitar) AndAlso OldtarifasPrecioContratoQuitar.Count > 0 Then
                                            ' Guardamos todos los registros de TarifaPrecioContrato generados.
                                            Funciones.UpdatePrecioContratoTarifa(tarifasPrecioContratoGuardar, OldtarifasPrecioContratoQuitar, isFijoIndex)
                                            Contador += 1
                                        Else
                                            Datos.Add(New List(Of Object) From {$"Contrato: {elment.CodigoContrato} _ idContratotarifa ={elment.IdContratoTarifa} , precios no encontrados "})
                                            Continue For
                                            'Throw New Exception("Imposible continuar, precios no encontrados. Contrato: " + elment.CodigoContrato)
                                        End If
                                    End If
                                    'objTarifaPerdidaCalculadaSrv.MergeUpdateTarifaPerdidaCalculadaContratoDTO(TarifaPerdidaCalculadaContratoGuardar, New ObservableCollection(Of TarifaPerdidaCalculadaContratoDTO))
                                    'If Actualizado > 0 Then
                                    '    Contador = +1
                                    'Else
                                    '    ContratosSinActualizar += elment.CodigoContrato + " "
                                    'End If
                                Else
                                    'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_PERFIL_FACTURACION"))
                                End If
                            End If
                        End If
                    Else
                        'Throw New Exception("Se cancela, no hay registros a actualizar")
                        Datos.Add(New List(Of Object) From {$"Contrato: {elment.CodigoContrato} _ idContratotarifa ={elment.IdContratoTarifa} , ContratoTarifa Vacio "})
                    End If

                Next
            Else
                complementos.MostrarMensajePersonalizado("las listas no coinciden")

            End If

        Catch ex As Exception
            Datos.Add(New List(Of Object) From {$"Error: {ex.Message}"})
            Throw
        Finally
            'ExcelDatos.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos)
        End Try
        Return $"Se han actualizado {Contador}. {ContratosSinActualizar}"
    End Function

    'CUPS
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            ' Si CheckBox1 está marcado, deshabilitar CheckBox2 y CheckBox3
            CheckBox2.Enabled = False
            CheckBox3.Enabled = False
            TextBox2.Enabled = True

        Else
            ' Si CheckBox1 no está marcado, habilitar CheckBox2 y CheckBox3
            CheckBox2.Enabled = True
            CheckBox3.Enabled = True
            TextBox2.Enabled = False

        End If
    End Sub
    'Contrato
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        ' Verificar si CheckBox2 está marcado
        If CheckBox2.Checked Then
            ' Si CheckBox2 está marcado, deshabilitar CheckBox1 y CheckBox3
            CheckBox1.Enabled = False
            CheckBox3.Enabled = False
            TextBox2.Enabled = True
        Else
            ' Si CheckBox2 no está marcado, habilitar CheckBox1 y CheckBox3
            CheckBox1.Enabled = True
            CheckBox3.Enabled = True
            TextBox2.Enabled = False
        End If
    End Sub
    'Cliente
    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        ' Verificar si CheckBox3 está marcado
        If CheckBox3.Checked Then
            ' Si CheckBox3 está marcado, deshabilitar CheckBox1 y CheckBox2
            CheckBox1.Enabled = False
            CheckBox2.Enabled = False
            TextBox2.Enabled = True
        Else
            ' Si CheckBox3 no está marcado, habilitar CheckBox1 y CheckBox2
            CheckBox1.Enabled = True
            CheckBox2.Enabled = True
            TextBox2.Enabled = False
        End If
    End Sub


    'Si hay contrato escrito habilitamos el texto de tarifa grupo
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        If TextBox2.Enabled AndAlso TextBox2.Text.Trim.Length >= 1 Then
            ' Si CheckBox3 está marcado, deshabilitar CheckBox1 y CheckBox2
            TextBox1.Enabled = True
            Button17.Enabled = True
            'TextBox2.Height = TextRenderer.MeasureText(TextBox2.Text, TextBox2.Font, New Size(TextBox2.Width, Int32.MaxValue), TextFormatFlags.WordBreak).Height + 5 ' Añade un pequeño margen
        Else
            ' Si CheckBox3 no está marcado, habilitar CheckBox1 y CheckBox2
            TextBox1.Enabled = False
            Button17.Enabled = False
        End If

    End Sub

    'Productos Asignacion, si no ha escrito nada en textotarifagrupo no buscamos nada, y enviamos mensaje
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            If CheckBox1.Checked OrElse CheckBox3.Checked Then
                complementos.MostrarMensajePersonalizado($"Habilitado solo para el check de contratos")
                Exit Sub
            End If

            Dim Con = GetConSinSplit(TextBox2.Text)
            If Con.Count > 0 Then
                Dim Entorno = If(Funciones.GetContrato(Con.FirstOrDefault).Entorno = "E1", "G1", "G2")
                Dim ModiCo As New ProductosAsig(Entorno, Con, connectionString, NombreUsuarioEquipo)
                ModiCo.Show()
            Else
                complementos.MostrarMensajePersonalizado($"Ingrese al menos un contrato")
            End If

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Para separar los contratos introducidos con comas(,)
    Private Function GetConSinSplit(contxt As String) As List(Of Long)
        Dim Con As New List(Of Long)
        Try
            If contxt.Contains(",") Then
                ' Si la cadena ya contiene comas, dividir la cadena utilizando solo comas como delimitadores
                Dim contratos = contxt.Replace(vbCrLf, "")
                Dim contratosSeparados As String() = contratos.Split(","c)
                For Each contratoTexto As String In contratosSeparados
                    Dim codigosCon As Long
                    If Long.TryParse(contratoTexto.Trim(), codigosCon) Then
                        Con.Add(codigosCon)
                    End If
                Next
            Else
                ' Si la cadena no contiene comas, eliminar espacios en blanco de la cadena
                Dim contratosTexto As String = contxt.Replace(" ", "")
                ' Separar la cadena en una matriz de cadenas utilizando comas, saltos de línea y espacios en blanco como delimitadores
                Dim delimiters As Char() = {","c, ControlChars.Lf, ControlChars.Cr}
                Dim contratosSeparados As String() = contratosTexto.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                For Each contratoTexto As String In contratosSeparados
                    Dim codigosCon As Long
                    If Long.TryParse(contratoTexto.Trim(), codigosCon) Then
                        Con.Add(codigosCon)
                    End If
                Next
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
        Return Con
    End Function


    Private Function GetConSinSplitCupsCIFS(contxt As String) As List(Of String)
        Dim Con As New List(Of String)
        Try

            If contxt.Contains(",") Then
                ' Separar la cadena en una matriz de cadenas utilizando la coma como delimitador
                Dim contratosTexto As String = contxt.Replace(vbCrLf, "")
                Dim contratosSeparados As String() = contratosTexto.Split(","c)
                For Each cupstexto As String In contratosSeparados
                    If cupstexto.Length > 1 Then
                        Con.Add(cupstexto)
                    End If
                Next
            Else
                ' Si la cadena no contiene comas, eliminar espacios en blanco de la cadena
                Dim contratosTexto As String = contxt.Replace(" ", "")
                ' Separar la cadena en una matriz de cadenas utilizando comas, saltos de línea y espacios en blanco como delimitadores
                Dim delimiters As Char() = {","c, ControlChars.Lf, ControlChars.Cr}
                Dim contratosSeparados As String() = contratosTexto.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                For Each contratoTexto As String In contratosSeparados
                    If contratoTexto.Length > 1 Then
                        Con.Add(contratoTexto)
                    End If
                Next
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
        Return Con
    End Function

    Private Function GetFacsSinSplit(Facs As String) As List(Of String)
        Dim Con As New List(Of String)
        Try
            If Facs.Contains(",") Then
                ' Si la cadena ya contiene comas, dividir la cadena utilizando solo comas como delimitadores
                Dim FacsTexto As String = Facs.Replace(" ", "")
                Dim FacsTexto2 As String = FacsTexto.Replace(vbTab, "")
                Dim contratosSeparados As String() = FacsTexto2.Split(","c)
                For Each facsl As String In contratosSeparados
                    If Facs.Length > 1 Then
                        Con.Add(Replace(facsl, "_", "")) 'Si tiene guiones bajos reemplazo y unifico serie y numero
                    End If
                Next
            Else
                ' Si la cadena no contiene comas, eliminar espacios en blanco de la cadena
                Dim FacsTexto As String = Facs.Replace(" ", "")
                Dim FacsTexto2 As String = FacsTexto.Replace(vbTab, "")
                ' Separar la cadena en una matriz de cadenas utilizando comas, saltos de línea y espacios en blanco como delimitadores
                Dim delimiters As Char() = {","c, ControlChars.Lf, ControlChars.Cr}
                Dim contratosSeparados As String() = FacsTexto2.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                For Each facsl As String In contratosSeparados
                    If Facs.Length > 1 Then
                        Con.Add(Replace(facsl, "_", "")) 'Si tiene guiones bajos reemplazo y unifico serie y numero
                    End If
                Next
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
        Return Con
    End Function

    'Habilitar o deshabilitar el botón de actualizar si no hay un texto de tarifa grupo 
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Try
            If TextBox1.Text.Trim.Length > 0 Then
                Button1.Enabled = True
            Else
                Button1.Enabled = False
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Crear las validaciones
    Private Async Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            'PictureBox2.Visible = True
            PictureBox2.Visible = True
            Dim listas As New List(Of String)
            Dim Validaciones As New ValidacionExcel(connectionString)
#Region "Consulta 1"
            Dim resultadoConsulta1 = "select c.codigocontrato
,cups.CodigoCUPS
,c.Confirmado
,convert(varchar,c.FechaCreacion, 103) as FechaCreacion
,convert(varchar,c.FechaPrevistaActivacion, 103) as FechaPrevistaActivacion
,stf.TextoFechaEfecto
,c.observaciones 
,cs.textosituacion 
from contrato c 
inner join cups on cups.IdCups = c.idcups
inner join ContratoSituacion cs on c.IdContratoSituacion = cs.IdContratoSituacion
left join SolicitudTipoFechaEfecto stf on stf.IdSolicitudTipoFechaEfecto = c.IdSolicitudTipoFechaEfecto
inner join Solicitud s on s.CodigoContrato = c.CodigoContrato and (s.idsolicitudtipo in (1009
,1010
,1011
,1013
,50103
,50112) or s.IdSolicitudTipo is null)
where c.idcontratosituacion in (4,14) and Confirmado=0
order by c.CodigoContrato"
#End Region
#Region "Consulta 2"
            Dim resultadoConsulta2 = "select c.codigocontrato
,cups.CodigoCUPS
,c.Confirmado
,convert(varchar,c.FechaCreacion, 103) as FechaCreacion
,convert(varchar,c.FechaPrevistaActivacion, 103) as FechaPrevistaActivacion
,stf.TextoFechaEfecto
,c.observaciones 
,ss.Nombre SituacionSolicitud
,s.FechaApertura FechaAperturaSolicitud
from contrato c 
inner join cups on cups.IdCups = c.idcups
left join SolicitudTipoFechaEfecto stf on stf.IdSolicitudTipoFechaEfecto = c.IdSolicitudTipoFechaEfecto

inner join Solicitud s on s.CodigoContrato = c.CodigoContrato and s.IdUsuario in (1610,
1611,5900,5901,5902,5903,5904,5905,5906,5907,5908,5909,5910,5911,5884,5885,5886,5887,5888,5889,5890,5891,5892,5893,5894,5895,5896,5897,5898,5899) 
left join SolicitudSituacion ss on  s.IdSolicitudSituacion = ss.IdSolicitudSituacion
where c.idcontratosituacion=4 order by s.FechaApertura desc"
#End Region
#Region "Consulta 3"
            Dim resultadoConsulta3 = ";with 
ContratoTarifaVigenteMaxima as
(    select CodigoContrato,
           max(FechaDesde) as FechaDesde
    from ContratoTarifa
    where GetDate() between FechaDesde and FechaHasta
    group by CodigoContrato)
,ContratoTarifaFechaAjustada as
(    select ct.CodigoContrato,
           ctvm.FechaDesde as fechaCTVM,
           ct.FechaDesde as fechaCT,
           case when ctvm.FechaDesde is not null then ctvm.FechaDesde else ct.FechaDesde end as Fecha
    from ContratoTarifa ct left join ContratoTarifaVigenteMaxima ctvm on ctvm.CodigoContrato = ct.CodigoContrato)
,ContratoTarifaFechaMaxima as
(    select CodigoContrato,
           max(Fecha) as FechaDesde
    from ContratoTarifaFechaAjustada
    group by CodigoContrato)
,ContratoTarifaVigente as
(    select ctfm.Codigocontrato,
           ctfm.FechaDesde,
           Entorno,
           IdTarifaGrupo,
           IdTarifa,
           IdPerfilFacturacion
    from ContratoTarifa ct 
	inner join ContratoTarifaFechaMaxima ctfm on ct.CodigoContrato = ctfm.CodigoContrato and ct.FechaDesde = ctfm.FechaDesde)
select Solicitud.IdSolicitud as Solicitud
,SolicitudTipo.NombreSolicitudTipo as TipoSolicitud
,u.Nombre
,SolicitudSituacion.Nombre as Situacion 
,Cliente.Identidad as Cliente
,Solicitud.CodigoContrato as Contrato
,Contrato.FechaCreacion as 'Fecha creacion contrato'
,CASE
	WHEN (Cliente.Nombre is null) OR (Cliente.Nombre = '')
		THEN Cliente.RazonSocial
		ELSE CONCAT(Cliente.Nombre, ' ' , ISNULL(Cliente.Apellido1, '') , ' ' , ISNULL(Cliente.Apellido2, ''))
		END as Nombre
,CUPS.CodigoCUPS as CUPS
,convert(varchar,Solicitud.FechaApertura,103) as 'Fecha apertura'
,convert(varchar,Solicitud.FechaCierre,103) as 'Fecha cierre'
,convert(varchar,Contrato.FechaAlta,103) as 'Fecha alta'
,convert(varchar,Contrato.FechaPrevistaActivacion,103) as 'F. Prev. Act.'
,SolicitudTipoFechaEfecto.TextoFechaEfecto as 'Texto Fecha Efecto'
,convert(varchar,Contrato.FechaPrevistaBaja,103) as 'F. Prev. Baja'
,convert(varchar,Contrato.FechaBaja,103) as 'Fecha baja'
,Motivobaja.TextoBaja as 'Motivo baja'
,MotivoRechazo.TextoRechazo as 'Motivo Rechazo'
,Solicitud.Observaciones 
,Case
	when Agente.CodigoTipoAgente=2
		then Agente.NombreAgente
    when Agente.CodigoTipoAgente=3
        then Agenteb.NombreAgente
        else null
    End As NombreAgente
,Case
	when Agente.CodigoTipoAgente=3
		then Agente.NombreAgente
        else null
    End As NombreSubAgente
,CASE
	when ClienteContactoTelefono.TipoContacto = 'T'
		then ClienteContactoTelefono.Valor
	when ClienteContactoTelefono.TipoContacto = 'M'
		then ClienteContactoTelefono.Valor
	end as TelefonoAgente
,Tarifa.TextoTarifa As Tarifa
, CONCAT( CallejeroTipoVia.TextoVia,' ',Callejero.NombreCalle, ' ', Cliente.Numero, ' ' ,Cliente.Aclarador) as Direccion
, Ciudad.TextoCiudad as Pobllacion
, ClienteContactoEmail.Valor as EMail
,eq.IdEquipoMedida as 'Nº Equipo Medida'
,ContratoPotenciaMaxima.PotenciaMaxima as 'Potencia Actual'
,Solicitud.ValorTrafo as 'Situación Libre'
,ModoLectura.Descripcion as 'Modo lectura'
from Solicitud with(nolock)
left join Usuario u with(nolock) on u.IdUsuario = Solicitud.IdUsuario
left join SolicitudTipoFechaEfecto on Solicitud.IdSolicitudTipoFechaEfecto=SolicitudTipoFechaEfecto.IdSolicitudTipoFechaEfecto
left join SolicitudTipo with(nolock) on SolicitudTipo.IdSolicitudTipo = Solicitud.IdSolicitudTipo
left join SolicitudSituacion with(nolock) on SolicitudSituacion.IdSolicitudSituacion = Solicitud.IdSolicitudSituacion
left join Contrato with(nolock) on Contrato.CodigoContrato = Solicitud.CodigoContrato
left join Cliente with(nolock) on Cliente.IdCliente = Contrato.IdCliente
left join CUPS with(nolock) on CUPS.IdCups = Contrato.IdCups
left join MotivoBaja with(nolock) on MotivoBaja.IdMotivoBaja = Contrato.IdMotivoBaja
left join MotivoRechazo with(nolock) on MotivoRechazo.IdMotivoRechazo = Solicitud.IdMotivoRechazo
left join Agente with(nolock) on Agente.IdAgente = Contrato.IdAgente
left join (select IdAgente, NombreAgente, IdAgenteNivelAnterior from Agente with(nolock)) as Agenteb on Agenteb.IdAgente = Agente.IdAgenteNivelAnterior
left join ClienteContacto as ClienteContactoTelefono with(nolock) on ClienteContactoTelefono.IdCliente = Cliente.IdCliente AND ClienteContactoTelefono.TipoContacto = 'T' and ClienteContactoTelefono.PorDefecto = 1
left join Tarifa with(nolock) on Tarifa.IdTarifa = Contrato.IdTarifa
left join Callejero with(nolock) on Callejero.IdCallejero = Cliente.IdCallejero
left join CallejeroTipoVia with(nolock) on CallejeroTipoVia.IdCallejeroTipoVia = Callejero.IdCallejeroTipoVia
left join Ciudad with(nolock) on Ciudad.IdCiudad = CUPS.IdCiudad
left join ClienteContacto as ClienteContactoEmail with(nolock) on ClienteContactoEmail.IdCliente = Cliente.IdCliente AND ClienteContactoEmail.TipoContacto = 'E' and ClienteContactoEmail.PorDefecto = 1
LEFT JOIN (Select Top 1 Entorno, CodigoContrato, NumeroSerie, IdEquipoModelo, Min(IdEquipoMedida) as IdEquipoMedida, IsInstalado as IsInstalado from EquipoMedida with(nolock) where isinstalado=1  group by Entorno, CodigoContrato, NumeroSerie, IdEquipoModelo, IsInstalado) as eq ON (Contrato.CodigoContrato = eq.CodigoContrato and eq.Entorno = 'G2')
LEFT JOIN EquipoModelo with(nolock) ON (eq.IdEquipoModelo =EquipoModelo.IdEquipoModelo) 
LEFT JOIN (
			SELECT	IdContrato, MAX(PotenciaContratada)	AS PotenciaMaxima 
				FROM ContratoPotencia with(nolock)
				GROUP BY IdContrato
			  ) AS ContratoPotenciaMaxima 
			  ON ContratoPotenciaMaxima.IdContrato = Contrato.IdContrato
LEFT JOIN ModoLectura with(nolock) ON Solicitud.IdModoLectura = ModoLectura.IdModoLectura
left join ContratoTarifaVigente with (nolock) on ContratoTarifaVigente.CodigoContrato = Contrato.CodigoContrato
left join TarifaGrupo with (nolock) on TarifaGrupo.IdTarifaGrupo = ContratoTarifaVigente.IdTarifaGrupo
where   Solicitud.FechaApertura  >= DATEADD (dd, 0, DATEDIFF (dd, 0, GETDATE() - 1))
order by Solicitud.IdSolicitudTipo, Solicitud.FechaApertura "
#End Region
            listas.Add(resultadoConsulta1)
            listas.Add(resultadoConsulta2)
            listas.Add(resultadoConsulta3)
            'where Solicitud.FechaApertura >= convert(date,Getdate(),3)
            ' Ruta del archivo CSV
            'Dim rutaArchivo As String = $"C:\Users\ErickCC\Documents\TotalDoc\Validaciones\Validaciones{Date.Today.ToString("ddMMyyyy")}.xlsx"
            Dim rutaCarpeta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO"
            Dim rutaArchivo = IO.Path.Combine(rutaCarpeta, $"Validaciones_{Date.Today.ToString("ddMMyyyy")}.xlsx")
            ' Verificar si la carpeta existe, y si no, crearla
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If

            ' Verificar si el archivo existe, y si no, crearlo
            If Not File.Exists(rutaArchivo) Then
                File.Create(rutaArchivo).Close()
            End If

            Await Task.Run(Sub() Validaciones.EjecutarConsultasYGuardarEnExcel(listas, rutaArchivo))
            'PictureBox2.Visible = False
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado($"Se han creado los datos en el archivo Excel en: {rutaArchivo}")

            'MessageBox.Show($"Se han creado los datos en el archivo Excel en: {rutaArchivo}")
        Catch ex As Exception
            'PictureBox2.Visible = False
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)

        End Try
    End Sub

    'Codigos DIR
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            If CheckBox1.Checked OrElse CheckBox3.Checked Then
                complementos.MostrarMensajePersonalizado($"Habilitado solo para el check de contratos")
                Exit Sub
            End If

            Dim Con = GetConSinSplit(TextBox2.Text)
            If Con.Count > 0 Then
                Dim CodigoDir As New CodigoDir(connectionString, Con)
                CodigoDir.Show()
            Else
                complementos.MostrarMensajePersonalizado($"Ingrese al menos un contrato")
            End If

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Actualizar CNAES 
    Private Async Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog()

            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaArchivo As String = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog() = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename As String In openFileDialog1.FileNames
                    rutaArchivo = filename
                Next
            End If

            Dim stopwatch As New Stopwatch()
            stopwatch.Start() ' Iniciar el cronómetro
            'Dim Empieza As TimeSpan = stopwatch.Elapsed
            Dim ActualizarCNAE As New ActualizarCNAEFromExcel(connectionString)
            If rutaArchivo.Length > 0 Then
                PictureBox2.Visible = True
                ActualizarCNAE.RutaExcel = rutaArchivo
                Dim contratosActualizado = Await ActualizarCNAE.ActualizarCNAEFromExcelAsync()

                ' Detener el cronómetro y obtener el tiempo transcurrido
                stopwatch.Stop()
                PictureBox2.Visible = False
                Dim tiempoTranscurrido As TimeSpan = stopwatch.Elapsed

                complementos.MostrarMensajePersonalizado($"Se han actualizado {contratosActualizado} contratos. Tiempo transcurrido: {tiempoTranscurrido.TotalMinutes} minutos.")
            Else
                complementos.MostrarMensajePersonalizado($"Escriba una ruta para seguir.")
            End If

        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Volver a RenovarContratos
    Private Async Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            PictureBox2.Visible = True
            Dim RenovadoANull = 0L
            'Si esta por cups
            If CheckBox1.Checked Then
                Dim Cups = GetConSinSplitCupsCIFS(TextBox2.Text)
                If Cups.Count > 0 Then
                    For Each cps In Cups
                        Dim cps20 As String = Replace(cps, " ", "").Substring(0, Math.Min(20, cps.Length)) 'saco los primeros 20 caracteres

                        Dim ContratoActivo = Funciones.GetListContratobyCUPS(Replace(cps20.Trim, " ", "")).Where(Function(f) If(f.IdContratoSituacion, 0L) = 1).FirstOrDefault ' Buscamos solo el activo
                        If Not IsNothing(ContratoActivo) AndAlso ContratoActivo.IdContrato > 0 AndAlso ContratoActivo.IdContratoSituacion = 1 Then ' solo si es activo
                            RenovadoANull = Await Task.Run(Function() Funciones.VolverARenovar(ContratoActivo.CodigoContrato))
                        End If


                    Next
                End If
            End If
            ' Si esta por contrato
            If CheckBox2.Checked Then
                Dim Con = GetConSinSplit(TextBox2.Text)
                If Con.Count > 0 Then
                    For Each elemnt In Con
                        Dim ConActivo = Funciones.GetContrato(elemnt)
                        If Not IsNothing(ConActivo) AndAlso ConActivo.IdContrato > 0 AndAlso ConActivo.IdContratoSituacion = 1 Then ' solo si es activo
                            RenovadoANull = Await Task.Run(Function() Funciones.VolverARenovar(ConActivo.CodigoContrato))
                        End If
                    Next
                End If
            End If
            'Si esta por Cliente
            If CheckBox3.Checked Then
                Dim CIFS = GetConSinSplitCupsCIFS(TextBox2.Text)
                If CIFS.Count > 0 Then
                    For Each cif In CIFS
                        Dim ListContratoActivo = Funciones.GetListContratobyCIF(cif.Trim).Where(Function(f) If(f.IdContratoSituacion, 0L) = 1).ToList ' Buscamos solo el activo
                        If Not IsNothing(ListContratoActivo) AndAlso ListContratoActivo.Count > 0 Then

                            For Each ContratoActivo In ListContratoActivo.Where(Function(f) f.IdContratoSituacion = 1) 'por siacaso
                                If Not IsNothing(ContratoActivo) AndAlso ContratoActivo.IdContrato > 0 AndAlso ContratoActivo.IdContratoSituacion = 1 Then ' solo si es activo
                                    RenovadoANull = Await Task.Run(Function() Funciones.VolverARenovar(ContratoActivo.CodigoContrato))
                                End If
                            Next
                        End If


                    Next
                End If
            End If
            PictureBox2.Visible = False

            If RenovadoANull > 0 Then
                complementos.MostrarMensajePersonalizado($"Contratos listos para ser renovados")
            Else
                complementos.MostrarMensajePersonalizado($"Ningún contrato renovado")
            End If
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Revisa si ha habido algún contrato que no se haya configurado bien
    Private Async Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Try

            'Dim RutaArchivo = Funciones.RevisaTarifaPrecioContratoPersonalizada()
            'MessageBox.Show($"Se ha generado la revisión en la siguiente ruta:{RutaArchivo}")
            PictureBox2.Visible = True
            Dim table As New TablaRevisaPreciosPersonalizados(connectionString)
            Await Task.Run(Sub() table.cargar())
            PictureBox2.Visible = False
            table.Show()
            'Funciones.RevisaTarifaPrecioContratoPersonalizadaGas()
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    ''Para saber si PRO o AUT
    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.TextChanged

        Try
            If ipDB.Trim.Contains("172.31.100.12") AndAlso RadioButton1.Checked Then
                Label5.Text = "BD PRO  172.31.100.12 SigeTotal"
                'ElseIf ipDB.Trim.Contains("172.31.100.29") AndAlso RadioButton2.Checked Then
                '    Label5.Text = "BD UAT  172.31.100.29 SigeTotal(Replica)"
                'ElseIf RadioButton3.Checked Then
                '    Label5.Text = "BD UAT  172.31.100.50 SigeTotalUAT"
            End If
        Catch ex As Exception

        End Try

    End Sub

    'Actualizar emails desde Excel_ FIla  contrato 1 y fila 2 el email
    Private Async Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim contratosActualizado = 0
        Dim tiempoTranscurrido As TimeSpan
        Try

            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog

            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaArchivo = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaArchivo = filename
                Next
            End If

            Dim stopwatch As New Stopwatch
            stopwatch.Start() ' Iniciar el cronómetro
            'Dim Empieza As TimeSpan = stopwatch.Elapsed
            Dim ActualizarEmail As New ActualizarEmailFromExcel(connectionString)
            If rutaArchivo.Length > 0 Then
                PictureBox2.Visible = True
                ActualizarEmail.RutaExcel = rutaArchivo
                contratosActualizado = Await ActualizarEmail.ActualizarEmailFromExcelAsync

                ' Detener el cronómetro y obtener el tiempo transcurrido
                stopwatch.Stop()
                tiempoTranscurrido = stopwatch.Elapsed
            End If

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado($"Se han actualizado {contratosActualizado} contratos. Tiempo transcurrido: {tiempoTranscurrido.TotalMinutes} minutos.")
        End Try
    End Sub

    'Check box personalizado, habilita o deshabilita
    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        Try
            If CheckBox4.Checked = False Then
                TextBox3.Enabled = True
            End If

            If CheckBox4.Checked Then
                TextBox3.Enabled = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    ' Para modificar el agente del contrato
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Try

            Dim ListaCodContrato As New List(Of Long)

            'Check Cups
            If CheckBox1.Checked Then
                Dim Cups = GetConSinSplitCupsCIFS(TextBox2.Text)
                If Cups.Count > 0 Then
                    For Each cps In Cups
                        Dim cps20 As String = Replace(cps, " ", "").Substring(0, Math.Min(20, cps.Length)) 'saco los primeros 20 caracteres

                        Dim ListContratos = Funciones.GetListContratobyCUPS(Replace(cps20.Trim, " ", "")).ToList
                        If ListContratos.Count > 0 Then
                            For Each elemnt In ListContratos
                                If Not IsNothing(elemnt) AndAlso elemnt.IdContrato > 0 Then
                                    ListaCodContrato.Add(If(elemnt.CodigoContrato, 0L))
                                End If
                            Next
                        End If

                    Next
                End If
            End If
            ' Check contrato
            If CheckBox2.Checked Then
                Dim ConC = GetConSinSplit(TextBox2.Text)
                If ConC.Count > 0 Then
                    For Each elemnt In ConC
                        Dim ContratoC = Funciones.GetContrato(elemnt)
                        If Not IsNothing(ContratoC) AndAlso ContratoC.IdContrato > 0 Then ' solo si es activo
                            ListaCodContrato.Add(If(ContratoC.CodigoContrato, 0L))
                        End If
                    Next
                End If
            End If
            'Check Cliente
            If CheckBox3.Checked Then
                Dim CIFS = GetConSinSplitCupsCIFS(TextBox2.Text)
                If CIFS.Count > 0 Then
                    For Each cif In CIFS
                        Dim ListContratos = Funciones.GetListContratobyCIF(cif.Trim).ToList
                        If ListContratos.Count > 0 Then
                            For Each elemnt In ListContratos
                                If Not IsNothing(elemnt) AndAlso elemnt.IdContrato > 0 Then
                                    ListaCodContrato.Add(If(elemnt.CodigoContrato, 0L))
                                End If
                            Next
                        End If
                    Next
                End If
            End If

            '

            If ListaCodContrato.Count > 0 Then
                Dim Agentes As New Agentes(ListaCodContrato, connectionString)
                Agentes.Show()
            Else
                complementos.MostrarMensajePersonalizado($"No hay contratos")
            End If

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
    ' Para modificar el Administrador del contrato
    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        Try

            Dim ListaCodContrato As New List(Of Long)

            'Check Cups
            If CheckBox1.Checked Then
                Dim Cups = GetConSinSplitCupsCIFS(TextBox2.Text)
                If Cups.Count > 0 Then
                    For Each cps In Cups
                        Dim cps20 = Replace(cps, " ", "").Substring(0, Math.Min(20, cps.Length)) 'saco los primeros 20 caracteres

                        Dim ListContratos = Funciones.GetListContratobyCUPS(Replace(cps20.Trim, " ", "")).ToList
                        If ListContratos.Count > 0 Then
                            For Each elemnt In ListContratos
                                If Not IsNothing(elemnt) AndAlso elemnt.IdContrato > 0 Then
                                    ListaCodContrato.Add(If(elemnt.CodigoContrato, 0L))
                                End If
                            Next
                        End If

                    Next
                End If
            End If
            ' Check contrato
            If CheckBox2.Checked Then
                Dim ConC = GetConSinSplit(TextBox2.Text)
                If ConC.Count > 0 Then
                    For Each elemnt In ConC
                        Dim ContratoC = Funciones.GetContrato(elemnt)
                        If Not IsNothing(ContratoC) AndAlso ContratoC.IdContrato > 0 Then ' solo si es activo
                            ListaCodContrato.Add(If(ContratoC.CodigoContrato, 0L))
                        End If
                    Next
                End If
            End If
            'Check Cliente
            If CheckBox3.Checked Then
                Dim CIFS = GetConSinSplitCupsCIFS(TextBox2.Text)
                If CIFS.Count > 0 Then
                    For Each cif In CIFS
                        Dim ListContratos = Funciones.GetListContratobyCIF(cif.Trim).ToList
                        If ListContratos.Count > 0 Then
                            For Each elemnt In ListContratos
                                If Not IsNothing(elemnt) AndAlso elemnt.IdContrato > 0 Then
                                    ListaCodContrato.Add(If(elemnt.CodigoContrato, 0L))
                                End If
                            Next
                        End If
                    Next
                End If
            End If

            '

            If ListaCodContrato.Count > 0 Then
                Dim Administradores As New AdministradoresWF(ListaCodContrato, connectionString)
                Administradores.Show()
            Else
                complementos.MostrarMensajePersonalizado($"No hay contratos")
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Limpiar filtros
    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Try

            TextBox2.Text = ""
            TextBox1.Text = ""
            TextBox3.Text = ""

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Extraer PDFs
    Private Async Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        Try
            Dim listaFacs = GetFacsSinSplit(TextBox2.Text)
            If listaFacs.Count > 0 Then
                Dim Destino = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO\PDFFacturas"
                If Not Directory.Exists(Destino) Then
                    Directory.CreateDirectory(Destino)
                End If
                Dim ComprobarFacs As New List(Of String)
                PictureBox2.Visible = True
                Await Task.Run(Sub()
                                   For Each elemnt In listaFacs
                                       Dim Facs = Funciones.ExtraerPDFFactura(elemnt)
                                       If IsNothing(Facs) Then
                                           Continue For
                                       End If
                                       ComprobarFacs.Add(elemnt)
                                       Dim NameFac = Replace(elemnt, "FELEC", "FELEC_")
                                       Dim originalFileName = $"{NameFac}.PDF"
                                       Dim nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName)
                                       Dim newFileName = Mid(nameWithoutExtension, 1, 100) & Path.GetExtension(originalFileName)
                                       Dim TempFileName = Path.Combine(Destino, newFileName)
                                       File.WriteAllBytes(TempFileName, Facs)
                                   Next
                               End Sub)
                PictureBox2.Visible = False
                If ComprobarFacs.Count > 0 Then
                    complementos.MostrarMensajePersonalizado("PDF descargados. Pulse Aceptar para abrir la carpeta contenedora")
                    Process.Start("explorer.exe", Destino)
                Else
                    complementos.MostrarMensajePersonalizado("Ningún PDF se ha descargado")
                End If

            Else
                complementos.MostrarMensajePersonalizado("No hay facturas en los filtros")
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try

    End Sub

    ' Open Items
    Private Async Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        Try

            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog

            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaArchivo = ""
            Dim RutaNueva = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaArchivo = filename
                Next
            End If

            'Muevo primero el archivo a una ruta local
            ' Definir las rutas de origen y destino
            Dim origen = rutaArchivo
            Dim destino = $"\\172.31.100.13\Total\FicherosExport\Import\OriginalItems"
            Dim NameFile = Path.GetFileName(origen)
            'Compruebo si existe el destino
            If Not Directory.Exists(destino) Then
                Directory.CreateDirectory(destino)
            End If

            ' Verificar si el archivo existe en la ruta de origen
            If File.Exists(origen) Then
                ' Nos guardamos el original en la carpeta OriginalItems
                File.Move(origen, destino + "\" + NameFile)
                RutaNueva = destino + "\" + NameFile
                'Console.WriteLine("Archivo movido exitosamente a: " & destino)
            Else
                'Console.WriteLine("Archivo no encontrado en la ruta de origen: " & origen)
            End If


#Region "FTP"

            ' Definir los parámetros de conexión
            'Dim servidorFTP As String = "172.31.100.13" ' Reemplazar con la dirección IP o nombre de host
            'Dim usuarioFTP As String = "audin\administrador"
            'Dim contrasenaFTP As String = "Azal3a$2020"

            '' Definir los archivos de origen y destino
            'Dim archivoOrigen As String = "archivo_origen.txt" ' Reemplazar con el nombre del archivo en el servidor FTP
            'Dim archivoDestino As String = "C:\Ruta\al\Archivo\Destino.txt" ' Reemplazar con la ruta completa del archivo en tu equipo local

            '' Establecer la conexión con el servidor FTP
            'Dim ftp As New System.Net.FtpClient.FtpClient()
            'ftp.Host = servidorFTP
            'ftp.Credentials = New System.Net.NetworkCredential(usuarioFTP, contrasenaFTP)
            '' Descargar el archivo del servidor FTP
            'Try
            '    Dim p = ftp.DirectoryExists(origen)
            '    Dim rr = ""
            'Catch ex As Exception
            '    Console.WriteLine("Error al descargar el archivo:", ex.Message)
            '    Exit Sub
            'End Try

            ' Cerrar la conexión FTP
            'ftp.Disconnect()
#End Region

            Dim stopwatch As New Stopwatch
            stopwatch.Start() ' Iniciar el cronómetro
            'Dim Empieza As TimeSpan = stopwatch.Elapsed
            'Dim ActualizarEmail As New ActualizarEmailFromExcel(connectionString)
            If RutaNueva.Length > 0 Then
                Dim OpenItms As New OpenItemsXML
                PictureBox2.Visible = True
                'Pasamos la nueva ruta
                Dim Open = Await Task.Run(Function() OpenItms.FormatearXML(RutaNueva))
                PictureBox2.Visible = False
                complementos.MostrarMensajePersonalizado($"Se han eliminado {Open} nodos del tipo <audinforContract/>.\nSe ha guardado en la siguiente ruta: {Path.GetDirectoryName(rutaArchivo)}")
            End If

        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    Private Async Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim rutaCarpeta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\CSVFACVA"
        Dim rutaArchivo = Path.Combine(rutaCarpeta, $"CSV_VA_{Date.Today.ToString("ddMMyyyy")}.csv")
        Dim contratosActualizado = 0
        Dim tiempoTranscurrido As TimeSpan
        Dim creado = False
        Try
            Dim Validaciones As New ValidacionExcel(connectionString)


            ' Verificar si la carpeta existe, y si no, crearla
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If

            ' Verificar si el archivo existe, y si no, crearlo
            If Not File.Exists(rutaArchivo) Then
                File.Create(rutaArchivo).Close()
            End If

            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog

            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaEscogida = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaEscogida = filename
                Next
            End If
            'Dim Empieza As TimeSpan = stopwatch.Elapsed
            If rutaEscogida.Length > 0 Then
                PictureBox2.Visible = True
                Dim stopwatch As New Stopwatch
                stopwatch.Start() ' Iniciar el cronómetro
                Await Task.Run(Sub() Validaciones.CSV3(rutaEscogida, rutaArchivo))
                creado = True
                ' Detener el cronómetro y obtener el tiempo transcurrido
                stopwatch.Stop()
                tiempoTranscurrido = stopwatch.Elapsed
            End If
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            PictureBox2.Visible = False
            If creado Then
                complementos.MostrarMensajePersonalizado($"Se han creado los datos en el archivo Excel en: {rutaArchivo} Tiempo transcurrido: {tiempoTranscurrido.TotalMinutes.ToString("F2")} minutos.")
            End If
        End Try
    End Sub

    ' Desglosar la descripcion del click de cada factura
#Region "Desglosado Click"
    Private Async Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        Try
            Dim ListaFClicks As New List(Of ClickFac)

            Dim rutaCarpeta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ClickFacs"
            Dim rutaArchivo = Path.Combine(rutaCarpeta, $"DesglosadoClick_{Date.Today.ToString("ddMMyyyy")}.xlsx")
            Dim listaFacs = GetFacsSinSplit(TextBox2.Text)

            If listaFacs.Count > 0 Then
                PictureBox2.Visible = True
                Await Task.Run(Sub()
                                   GetClickDesglosado(listaFacs, ListaFClicks)
                                   ' Verificar si la carpeta existe, y si no, crearla
                                   If Not Directory.Exists(rutaCarpeta) Then
                                       Directory.CreateDirectory(rutaCarpeta)
                                   End If

                                   ' Verificar si el archivo existe, y si no, crearlo
                                   If Not File.Exists(rutaArchivo) Then
                                       File.Create(rutaArchivo).Close()
                                   End If
                                   Dim Val As New ValidacionExcel(connectionString)
                                   Val.GuardarEnExcelClick(ListaFClicks, rutaArchivo) 'Escribo en el excel

                               End Sub)
                PictureBox2.Visible = False
                complementos.MostrarMensajePersonalizado($"Archivo Excel guardado en: {rutaArchivo}")
            Else
                complementos.MostrarMensajePersonalizado("No hay facturas")
            End If


        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
    Function EvaluarExpresion(expresion As String) As Double
        ' Reemplazar comas por puntos para evitar errores en cálculos
        expresion = expresion.Replace(",", ".")
        expresion = expresion.TrimEnd(")"c)
        Dim resultado As Double = New DataTable().Compute(expresion, Nothing)
        Return resultado
    End Function

    Private Sub GetClickDesglosado(ByRef listaFacs As List(Of String), ByRef ListaFClicks As List(Of ClickFac))
        'Dim ListaFClicks As New List(Of ClickFac)
        Try
            For Each facs In listaFacs
                Dim fClicks = Funciones.GetFacClick(facs)

                ' Expresión regular corregida para capturar toda la ecuación
                Dim regex As New Regex("(\d+(?:,\d+)?)%.*\((Coste P\d+) = (\d+(?:,\d+)?)\*\((.+)\)?")

                Dim regexPorcentaje As New Regex("(\d{1,3}(?:,\d{1,3})?)%")


                For Each f In fClicks
                    Dim texto As String = f.Descripcion
                    Dim match As Match = regex.Match(texto)

                    If match.Success Then
                        ' Capturar valores
                        Dim periodo As String = match.Groups(2).Value
                        Dim cultura As New Globalization.CultureInfo("es-ES")
                        Dim consumo As Decimal = Convert.ToDecimal(match.Groups(3).Value, cultura)

                        Dim expresion As String = match.Groups(4).Value.Trim()

                        ' Calcular el valor de la expresión
                        Try
                            Dim resultado As Decimal = EvaluarExpresion(expresion)
                            f.ClickCalculado = resultado
                            f.ConsumokWh = consumo
                            f.Periodo = Replace(periodo, "Coste", "")

                        Catch ex As Exception
                            Throw
                        End Try
                    End If

                    ' Extraer porcentaje
                    Dim matchPorcentaje As Match = regexPorcentaje.Match(f.Descripcion)
                    If matchPorcentaje.Success Then
                        f.Porcentaje = Convert.ToDecimal(matchPorcentaje.Groups(1).Value.Replace(",", "."))
                    Else
                        f.Porcentaje = 0.0
                    End If

                    ListaFClicks.Add(f)
                Next
            Next
        Catch ex As Exception
            Throw
        End Try
    End Sub


#End Region

#Region "Apuntado BD"



    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        Try
            If RadioButton1.Checked Then
                Label5.Text = "BD PRO  172.31.100.12 SigeTotal"
                RadioButton2.Checked = False
                RadioButton3.Checked = False
                ipDB = "data source=172.31.100.12;"
                nameDB = "initial catalog=SigeTotal;"
                connectionString = $"{ipDB}{nameDB}{userDB}{passDB}"
                Funciones = New FuncionesGenericas(connectionString)
                ContratoTarifaSrv = New ContratoTarifaSrv(connectionString)
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        Try
            If RadioButton2.Checked Then
                Label5.Text = "BD Replica 172.31.100.29 SigeTotal"
                RadioButton1.Checked = False
                RadioButton3.Checked = False
                ipDB = "data source=172.31.100.29;"
                nameDB = "initial catalog=SigeTotal;"
                connectionString = $"{ipDB}{nameDB}{userDB}{passDB}"
                Funciones = New FuncionesGenericas(connectionString)
                ContratoTarifaSrv = New ContratoTarifaSrv(connectionString)
                HabilitarDesHabilitarButtons(False)
            Else
                HabilitarDesHabilitarButtons(True)

            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        Try
            If RadioButton3.Checked Then
                Label5.Text = "BD UAT  172.31.100.50 SigeTotalUAT"
                RadioButton1.Checked = False
                RadioButton2.Checked = False
                ipDB = "data source=172.31.100.50;"
                nameDB = "initial catalog=SigeTotalUAT;"
                connectionString = $"{ipDB}{nameDB}{userDB}{passDB}"
                Funciones = New FuncionesGenericas(connectionString)
                ContratoTarifaSrv = New ContratoTarifaSrv(connectionString)

            End If
        Catch ex As Exception

        End Try

    End Sub
#End Region
    Private Sub HabilitarDesHabilitarButtons(Habilitar As Boolean)
        Button1.Enabled = Habilitar
        Button2.Enabled = Habilitar
        Button3.Enabled = Habilitar
        Button5.Enabled = Habilitar
        Button6.Enabled = Habilitar
        Button7.Enabled = Habilitar
        Button8.Enabled = Habilitar
        Button9.Enabled = Habilitar
        Button11.Enabled = Habilitar
    End Sub

    'Para leer los nombres de los pdfs
    'Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
    '    Dim folderPath = "C:\Users\ErickCC\Desktop\PDFFacturas"

    '    ' Archivo donde se guardarán los nombres de los PDFs
    '    Dim outputFile = "C:\Users\ErickCC\Desktop\PDFFacturas\NombresPDF.txt"

    '    Try
    '        ' Obtén la lista de archivos PDF en el directorio especificado
    '        Dim pdfFiles = Directory.GetFiles(folderPath, "*.pdf")

    '        ' Usar StreamWriter para escribir los nombres de los archivos en el .txt
    '        Using writer As New StreamWriter(outputFile, False) ' False para sobrescribir si ya existe
    '            For Each pdfFile In pdfFiles
    '                ' Obtener solo el nombre del archivo (sin la ruta completa)
    '                Dim fileName = Path.GetFileNameWithoutExtension(pdfFile)

    '                ' Escribir el nombre en el archivo de texto
    '                writer.WriteLine(fileName)
    '            Next
    '        End Using

    '        Console.WriteLine("Los nombres de los archivos PDF se han guardado correctamente.")
    '    Catch ex As Exception
    '        Console.WriteLine("Ocurrió un error: " & ex.Message)
    '    End Try
    'End Sub


    ' Aplicar Precios
    Private Async Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        Try
            Dim ListaContratos = GetConSinSplit(TextBox2.Text)
            Dim yesorNot1 = MsgBox($"Hay {ListaContratos.Count} contratos, ¿Aplicar precios con fecha {DateTimePicker1.Value.Date}?", vbYesNo)
            If yesorNot1 = 6 OrElse yesorNot1 = 1 Then

                PictureBox2.Visible = True
                Await Task.Run(Sub()
                                   Dim ContratosC = Funciones.GetContratoTarifaPersonalizado(ListaContratos)
                                   For Each c In ContratosC
                                       Dim FechaVigencia = DateTimePicker1.Value
                                       Funciones.AplicarPrecios(c.CodigoContrato, FechaVigencia)
                                   Next
                               End Sub)

            End If
        Catch ex As Exception
        Finally
            PictureBox2.Visible = False
        End Try
    End Sub

    ' Sin más
    'Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
    '    Try
    '        Dim rutaCarpeta As String = "C:\Audinfor\Sige\Total\EFactura\Export\20241204" ' Cambia esta ruta según corresponda
    '        Dim archivoSalida As String = "C:\Users\ErickCC\Desktop\Erick\facturas_extraidas.txt"

    '        ' Lista para almacenar los números de factura
    '        Dim numerosFactura As New List(Of String)

    '        ' Recorrer los archivos de la carpeta
    '        For Each archivo In Directory.GetFiles(rutaCarpeta, "*.xml")
    '            Try
    '                ' Cargar el archivo XML
    '                Dim xmlDoc As New XmlDocument()
    '                xmlDoc.Load(archivo)

    '                ' Recorrer manualmente los nodos para encontrar <BatchIdentifier>
    '                Dim nodoBatchIdentifier As XmlNode = BuscarNodo(xmlDoc.DocumentElement, "BatchIdentifier")

    '                ' Si el nodo se encuentra, procesar el contenido
    '                If nodoBatchIdentifier IsNot Nothing Then
    '                    Dim identificador As String = nodoBatchIdentifier.InnerText

    '                    ' Separar el identificador para encontrar los números de factura
    '                    Dim indiceFactura As Integer = identificador.IndexOf("2400")
    '                    If indiceFactura >= 0 Then
    '                        ' Extraer todo desde "2400" en adelante
    '                        Dim numeroFactura As String = identificador.Substring(indiceFactura)
    '                        numerosFactura.Add(numeroFactura)
    '                    End If
    '                Else
    '                    Console.WriteLine($"No se encontró <BatchIdentifier> en el archivo {Path.GetFileName(archivo)}.")
    '                End If
    '            Catch ex As Exception
    '                Console.WriteLine($"Error procesando el archivo {Path.GetFileName(archivo)}: {ex.Message}")
    '            End Try
    '        Next

    '        ' Guardar los números de factura en un archivo de texto
    '        File.WriteAllLines(archivoSalida, numerosFactura)

    '        Console.WriteLine($"Se han extraído {numerosFactura.Count} números de factura. Guardados en {archivoSalida}.")
    '    Catch ex As Exception

    '    End Try
    'End Sub

    ' Método para buscar nodos recursivamente por nombre (sin considerar espacios de nombres)
    Function BuscarNodo(ByVal nodo As XmlNode, ByVal nombre As String) As XmlNode
        If nodo.Name = nombre Then
            Return nodo
        End If

        For Each hijo As XmlNode In nodo.ChildNodes
            Dim resultado As XmlNode = BuscarNodo(hijo, nombre)
            If resultado IsNot Nothing Then
                Return resultado
            End If
        Next

        Return Nothing
    End Function

    'Penalizaciones
    Private Async Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        Dim PenaOk = False
        Dim RutaFinal = ""
        Try
            Dim Con = GetConSinSplit(TextBox2.Text)
            If Con.Count > 0 Then
                Dim Validaciones As New ValidacionExcel(connectionString)
                Dim listaCodLuz As New List(Of Long)
                Dim listaCodGas As New List(Of Long)
                PictureBox2.Visible = True

                For Each ConFor In Con
                    Dim ContratoBBDD = Funciones.GetContrato(ConFor)
                    If Not IsNothing(ContratoBBDD) AndAlso ContratoBBDD.CodigoContrato > 0 AndAlso ContratoBBDD.Entorno = "E1" Then
                        listaCodLuz.Add(ContratoBBDD.CodigoContrato)
                    End If
                    If Not IsNothing(ContratoBBDD) AndAlso ContratoBBDD.CodigoContrato > 0 AndAlso ContratoBBDD.Entorno = "E2" Then
                        listaCodGas.Add(ContratoBBDD.CodigoContrato)
                    End If
                Next
                Dim rutaCarpeta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO"
                Dim rutaArchivo = Path.Combine(rutaCarpeta, $"Penalizaciones_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                ' Verificar si la carpeta existe, y si no, crearla
                If Not Directory.Exists(rutaCarpeta) Then
                    Directory.CreateDirectory(rutaCarpeta)
                End If

                ' Verificar si el archivo existe, y si no, crearlo
                If Not File.Exists(rutaArchivo) Then
                    File.Create(rutaArchivo).Close()
                End If
                RutaFinal = rutaArchivo
                If listaCodLuz.Count > 0 Then
                    Await Task.Run(Sub() Validaciones.PenalizacionesLuz(listaCodLuz, rutaArchivo))
                    PenaOk = True
                End If
                If listaCodGas.Count > 0 Then
                    Await Task.Run(Sub() Validaciones.PenalizacionesGas(listaCodGas, rutaArchivo))
                    PenaOk = True
                End If
            Else
                complementos.MostrarMensajePersonalizado($"Ingrese al menos un contrato")
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado($"{ex.Message}")
        Finally
            PictureBox2.Visible = False

            If PenaOk Then
                complementos.MostrarMensajePersonalizado($"Penalizaciones Generadas en {RutaFinal}")
            Else
                complementos.MostrarMensajePersonalizado($"Penalizaciones No realizada")
            End If
        End Try
    End Sub

    'Consulta Top
    Private Async Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        Dim PenaOk = False
        Dim RutaFinal = ""
        Try
            Dim Facs = GetConSinSplitCupsCIFS(TextBox2.Text)
            If Facs.Count > 0 Then
                Dim Validaciones As New ValidacionExcel(connectionString)
                PictureBox2.Visible = True

                Dim rutaCarpeta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO"
                Dim rutaArchivo = Path.Combine(rutaCarpeta, $"ConsultaTopLidia_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                ' Verificar si la carpeta existe, y si no, crearla
                If Not Directory.Exists(rutaCarpeta) Then
                    Directory.CreateDirectory(rutaCarpeta)
                End If

                ' Verificar si el archivo existe, y si no, crearlo
                If Not File.Exists(rutaArchivo) Then
                    File.Create(rutaArchivo).Close()
                End If
                RutaFinal = rutaArchivo
                If Facs.Count > 0 Then
                    Await Task.Run(Sub() Validaciones.ConsultaTopLidia(Facs, rutaArchivo))
                    PenaOk = True
                End If
            Else
                complementos.MostrarMensajePersonalizado($"No hay registros a buscar")
            End If
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado($"{ex.Message}")
        Finally
            PictureBox2.Visible = False

            If PenaOk Then
                complementos.MostrarMensajePersonalizado($"Consultas Generadas en {RutaFinal}")
            Else
                complementos.MostrarMensajePersonalizado($"Consultas No realizada")
            End If
        End Try
    End Sub

    'Verifica si los contratos tienes licitacion
    Private Async Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        Try
            Dim ListaContratos = GetConSinSplit(TextBox2.Text)
            If ListaContratos.Count > 0 Then
                PictureBox2.Visible = True
                Dim Resultados = Await Task.Run(Function() Funciones.VerificarLicitacion(ListaContratos))
                PictureBox2.Visible = False

                If Resultados.Count > 0 Then
                    Dim cod = Resultados.Select(Function(s) s.CodigoContrato).ToList
                    complementos.Complementos_MostrarMensajePersonalizadoCopiar($"Los siguientes contratos tienen el check de licitacion: {String.Join(",", cod)} ", $"{String.Join(",", cod)}")
                Else
                    complementos.MostrarMensajePersonalizado("No hay contratos con el check de licitacion")
                End If
            End If
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado($"{ex.Message}")
        End Try
    End Sub

    'CAE
    Private Async Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        Dim rutaCarpeta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultaCAE"
        Dim rutaArchivo = ""
        Dim contratosActualizado = 0
        Dim tiempoTranscurrido As TimeSpan
        Dim creado = False
        Try
            Dim Validaciones As New ValidacionExcel(connectionString)


            ' Verificar si la carpeta existe, y si no, crearla
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If


            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog

            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaEscogida = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaEscogida = filename
                Next
            End If


            'Dim Empieza As TimeSpan = stopwatch.Elapsed
            Dim ActualizarEmail As New ActualizarEmailFromExcel(connectionString)
            If rutaEscogida.Length > 0 Then
                rutaArchivo = rutaEscogida
                PictureBox2.Visible = True
                Dim stopwatch As New Stopwatch
                stopwatch.Start() ' Iniciar el cronómetro
                Await Task.Run(Sub() Validaciones.BuscarCAEMasivo(rutaEscogida))
                creado = True
                ' Detener el cronómetro y obtener el tiempo transcurrido
                stopwatch.Stop()
                tiempoTranscurrido = stopwatch.Elapsed
            End If
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            PictureBox2.Visible = False

            If creado Then
                complementos.MostrarMensajePersonalizado($"Se han creado los datos en el archivo Excel en: {rutaArchivo} Tiempo transcurrido: {tiempoTranscurrido.TotalMinutes.ToString("F2")} minutos.")
            End If
        End Try
    End Sub

    'Actualiza el calendario masivamente, añade el nuevo y  cierra el calendario anterior, ademas de mantener el mismo perfil
    Private Async Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        Dim ExcelDatos As New Excel
        Dim Datos As New List(Of List(Of Object))
        Try
            Dim totalContratos = 0
            Dim ContratoActualizar As New List(Of Long)
            Dim Con As New List(Of Long)
            'Con = GetConSinSplit(TextBox2.Text)
            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog

            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaArchivo = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaArchivo = filename
                Next
            End If
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            Using package As New ExcelPackage(New FileInfo(rutaArchivo))
                Dim worksheet = package.Workbook.Worksheets(0)
                Dim rowCount = worksheet.Dimension.Rows

                ' Leer códigos de contrato del Excel
                Dim codigosContrato As New List(Of Long)
                For row = 2 To rowCount
                    Dim CodContrato = worksheet.Cells(row, 1).Value?.ToString
                    Dim idcontratotarifa = worksheet.Cells(row, 2).Value?.ToString
                    'Dim Cups As String = worksheet.Cells(row, 3).Value?.ToString()
                    If Not String.IsNullOrEmpty(idcontratotarifa) Then
                        Con.Add(idcontratotarifa)
                    End If
                Next
            End Using
            Dim yesorNot As MsgBoxResult
            Dim todoOK = False
            'Escribo los valores que tiene ahora, para posteriormente comparar o hacer uso de este y dejarlo como esta
            'Funciones.EscribirContratoTarifaAntesCambios(ContratoActualizar)
            If Con.Count > 0 Then
                If Con.Count < 0 Then
                    yesorNot = MsgBox("No hay contratos a actualizar. ¿Actualizar de todas formas?", vbYesNo)
                Else
                    todoOK = True
                End If
                If yesorNot = 6 OrElse yesorNot = 1 OrElse todoOK Then
                    PictureBox2.Visible = True
                    Await Task.Run(Sub()
                                       'TextBox1 TarifagrupoViejo
                                       'TextBox3 TarifagrupoNuevo
                                       'Podemos añadir una funcación para cerrar el calendario viejo antes de añadir el nuevo, pendiente implementar
                                       For Each c In Con
                                           Dim tgNuevo = Funciones.GetCalendarioNuevoTarifa(c, TextBox3.Text, TextBox1.Text)
                                           Dim FechaAplicar = DateTimePicker1.Value.Date
                                           Dim CodigoContrato = Funciones.GetOnlyCodigoContratobyIdContratoTarifa(c)
                                           If Not IsNothing(CodigoContrato) AndAlso CodigoContrato <> 0 AndAlso tgNuevo.IdTarifaGrupo Then
                                               Dim ok = Funciones.InsertTarifaGrupoCalendario(tgNuevo.Entorno, CodigoContrato, tgNuevo.IdTarifaGrupo, tgNuevo.IdTarifa, tgNuevo.IdPerfilFacturacion, FechaAplicar)
                                               Funciones.AplicarPreciosV2(CodigoContrato, FechaAplicar)
                                               Dim Pepe = 0
                                           End If
                                       Next
                                   End Sub)
                    PictureBox2.Visible = False

                    If Not IsNothing(Datos) AndAlso Datos.Count > 0 Then
                        ExcelDatos.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos, "PreciosErrores")
                    End If
                    complementos.MostrarMensajePersonalizado($"Contratos iniciales:{Con.Count} contratos")
                Else
                    complementos.MostrarMensajePersonalizado($"Se ha cancelado la actualización")
                End If
            Else
                complementos.MostrarMensajePersonalizado($"Sin Contratos")
            End If

        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado("Exception: " + ex.Message)
        End Try
    End Sub

    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
        Dim rutaExcel = "C:\Users\ErickCC\Downloads\Industriales_2024S1_v2.xlsx"
        Dim rutaXML = "C:\Users\ErickCC\Desktop\Erick\archivoF.xml"

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial

        ' Cargar archivo Excel
        Dim package As New ExcelPackage(New FileInfo(rutaExcel))
        Dim ws = package.Workbook.Worksheets(0) ' Primera hoja

        ' Crear documento XML
        Dim xmlDoc As New XmlDocument

        ' Agregar declaración XML con encoding utf-8
        Dim xmlDeclaration = xmlDoc.CreateProcessingInstruction("xml", "version=""1.0"" encoding=""utf-8""")
        xmlDoc.AppendChild(xmlDeclaration)

        ' Crear el elemento raíz con el namespace
        Dim root = xmlDoc.CreateElement("InformacionIndustrialAnualNR")
        root.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema")
        root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance")
        root.SetAttribute("xmlns", "http://tempuri.org/XMLIndustrialAnualNuevo.xsd")
        xmlDoc.AppendChild(root)

        ' Agregar el nodo EMPRESA
        Dim empresa = xmlDoc.CreateElement("EMPRESA")
        empresa.SetAttribute("xmlns", "") ' Añadir xmlns vacío
        root.AppendChild(empresa)

        ' Agregar información de la empresa
        empresa.AppendChild(CreateElementWithText(xmlDoc, "PAIS", "ES"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "RAZON_SOCIAL", "TOTALENERGIES ELECTRICIDAD Y GAS ESPAÑA, S.A."))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "CIF", "A87803862"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "VAT", "900834937"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "DOMICILIO", "MADRID"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "DIRECCION", "CALLE RIBERA DEL LOIRA"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "CODIGO_POSTAL", "28042"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "PROVINCIA", "28"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "MUNICIPIO", "079"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "TELEFONO", "900834937"))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "FAX", String.Empty))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "MOVIL", String.Empty))
        empresa.AppendChild(CreateElementWithText(xmlDoc, "E_MAIL", "tge.info@total.com"))

        ' Agregar el nodo BANDAS
        Dim bandas = xmlDoc.CreateElement("BANDAS")
        bandas.SetAttribute("xmlns", "") ' Añadir xmlns vacío
        root.AppendChild(bandas)

        ' Leer filas y columnas de Excel
        Dim filas = ws.Dimension.Rows
        Dim columnas = ws.Dimension.Columns

        ' Leer encabezados (asumiendo que están en la primera fila)
        Dim headers As New List(Of String)
        For col = 1 To columnas
            headers.Add(ws.Cells(1, col).Text)
        Next

        ' Leer datos y generar las bandas
        ' Leer datos y generar las bandas
        For fila = 2 To filas
            Dim banda = ws.Cells(fila, 1).Text ' Primera columna: Banda

            ' Omitir la banda "Banda_IF"
            If banda = "IF" Then
                Continue For
            End If

            Dim bandaElement = xmlDoc.CreateElement("Banda_" & banda)

            ' Generar los datos de la banda
            For col = 2 To columnas
                Dim nodo = xmlDoc.CreateElement(headers(col - 1))
                nodo.InnerText = ws.Cells(fila, col).Text
                bandaElement.AppendChild(nodo)
            Next

            bandas.AppendChild(bandaElement)
        Next


        ' Agregar el nodo NOTIFICACION
        Dim notificacion = xmlDoc.CreateElement("NOTIFICACION")
        notificacion.SetAttribute("xmlns", "") ' Añadir xmlns vacío
        root.AppendChild(notificacion)

        ' Agregar campos de notificación (vacíos por defecto)
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "PAIS", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "DOMICILIO", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "DIRECCION", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "CODIGO_POSTAL", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "PROVINCIA", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "MUNICIPIO", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "TELEFONO", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "FAX", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "MOVIL", String.Empty))
        notificacion.AppendChild(CreateElementWithText(xmlDoc, "E_MAIL", String.Empty))

        ' Guardar el XML en la ruta especificada
        xmlDoc.Save(rutaXML)
    End Sub

    ' Función para crear nodos XML auto-cerrados cuando no hay contenido
    Private Function CreateElementWithText(doc As XmlDocument, elementName As String, text As String) As XmlElement
        Dim element As XmlElement = doc.CreateElement(elementName)

        ' Si el texto está vacío o es Nothing, se deja sin InnerText para que se genere <NODO/>
        If Not String.IsNullOrEmpty(text.Trim()) Then
            element.InnerText = text
        End If

        Return element
    End Function

    'buscar Consultas checks
    Private Async Sub Button23_Click(sender As Object, e As EventArgs) Handles Button23.Click
        Try
            Dim conexion As String = connectionString
            Dim rutaCarpeta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO"
            Dim DesdeF = DateTimePicker3.Value.Date.ToString("dd/MM/yyyy")
            Dim HastaF = DateTimePicker2.Value.Date.ToString("dd/MM/yyyy")

            ' Verificar si la carpeta existe, y si no, crearla
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If

            ' Lista para las tareas
            Dim tasks As New List(Of Task)

            PictureBox2.Visible = True
            Dim RutaFinal = rutaCarpeta + ":"
            ' Si el CheckBox5 está marcado, crear archivo para Luz y Gas
            If CheckBox5.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Consulta_ClicksTODO" ' Nombre específico para esta consulta
                                       Dim rutaArchivoLuzGas = IO.Path.Combine(rutaCarpeta, $"{Name}_LuzGas_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim consultaLuz As String = ConsultasSQL.GetClickLuz
                                       ExportarConsultaAExcel(conexion, consultaLuz, rutaArchivoLuzGas, "Luz")
                                       Dim consultaGas As String = ConsultasSQL.GetClickGas
                                       ExportarConsultaAExcel(conexion, consultaGas, rutaArchivoLuzGas, "Gas")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            ' Si el CheckBox6 está marcado, crear archivo para Hunosa
            If CheckBox6.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Hunosa"
                                       Dim rutaArchivoHunosa = IO.Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim Hunosa As String = ConsultasSQL.GetHunosa(DesdeF, HastaF)
                                       ExportarConsultaAExcel(conexion, Hunosa, rutaArchivoHunosa, "Hunosa")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            ' Si el CheckBox8 está marcado, crear archivo para Cadasa
            If CheckBox8.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Cadasa"
                                       Dim rutaArchivoCadasa = IO.Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim Cadasa As String = ConsultasSQL.GetCadasa(DesdeF, HastaF)
                                       ExportarConsultaAExcel(conexion, Cadasa, rutaArchivoCadasa, "Cadasa")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            ' Si el CheckBox9 está marcado, crear archivo para Quantum
            If CheckBox9.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Quantum"
                                       Dim rutaArchivoQuantum = IO.Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim Quantum As String = ConsultasSQL.GetQuantum(DesdeF, HastaF)
                                       ExportarConsultaAExcel(conexion, Quantum, rutaArchivoQuantum, "Quantum")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            ' Si el CheckBox10 está marcado, crear archivo para RechazosVeolia
            If CheckBox10.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Contratos"
                                       Dim rutaArchivoRechazosVeolia = IO.Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim RechazosVeolia As String = ConsultasSQL.GetRechazosVeolia
                                       ExportarConsultaAExcel(conexion, RechazosVeolia, rutaArchivoRechazosVeolia, "Veolia")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            If CheckBox11.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "GAM"
                                       Dim rutaArchivoGAM = IO.Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim ConsultaGAM As String = ConsultasSQL.GetGAM(DesdeF, HastaF)
                                       ExportarConsultaAExcel(conexion, ConsultaGAM, rutaArchivoGAM, Name)
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            'Check Cups
            If CheckBox1.Checked Then
                Dim Cups = GetConSinSplitCupsCIFS(TextBox2.Text)
                Dim conexionv2 = "data source=172.31.100.30;initial catalog=SigeTotalTM;User ID=Sige;Password=SigeNew;"
                If CheckBox12.Checked AndAlso Cups.Count > 0 Then
                    tasks.Add(Task.Run(Sub()
                                           Dim Name = "CurvaHoraria"
                                           Dim rutaArchivoCurva = IO.Path.Combine(rutaCarpeta, $"{Name}_{DateTimePicker3.Value.Date.ToString("ddMMyyyy")}_{DateTimePicker2.Value.Date.ToString("ddMMyyyy")}.xlsx")
                                           Dim listaCups As New List(Of String)
                                           For Each c In Cups
                                               listaCups.Add(Replace(c, " ", "").Substring(0, Math.Min(20, c.Length)))
                                           Next
                                           Dim ConsultaCurva As String = ConsultasSQL.GetCurvaHoraria(DesdeF, HastaF, listaCups)
                                           ExportarConsultaAExcel(conexionv2, ConsultaCurva, rutaArchivoCurva, Name)
                                           RutaFinal += " " + Name
                                       End Sub))
                End If

                If CheckBox13.Checked AndAlso Cups.Count > 0 Then
                    tasks.Add(Task.Run(Sub()
                                           Dim Name = "CuartoHoraria"
                                           Dim rutaArchivoCuartoHoraria = IO.Path.Combine(rutaCarpeta, $"{Name}_{DateTimePicker3.Value.Date.ToString("ddMMyyyy")}_{DateTimePicker2.Value.Date.ToString("ddMMyyyy")}.xlsx")
                                           Dim listaCups As New List(Of String)
                                           For Each c In Cups
                                               listaCups.Add(Replace(c, " ", "").Substring(0, Math.Min(20, c.Length)))
                                           Next
                                           Dim ConsultaCurvaCuarto As String = ConsultasSQL.GetCurvaCuartoHoraria(DesdeF, HastaF, listaCups)
                                           ExportarConsultaAExcel(conexionv2, ConsultaCurvaCuarto, rutaArchivoCuartoHoraria, Name)
                                           RutaFinal += " " + Name
                                       End Sub))
                End If
                If CheckFacturable.Checked AndAlso Cups.Count > 0 Then
                    tasks.Add(Task.Run(Sub()
                                           Dim Name = "Facturable"
                                           Dim rutaArchivoFacturable = IO.Path.Combine(rutaCarpeta, $"{Name}_{DateTimePicker3.Value.Date.ToString("ddMMyyyy")}_{DateTimePicker2.Value.Date.ToString("ddMMyyyy")}.xlsx")
                                           Dim listaCups As New List(Of String)
                                           For Each c In Cups
                                               listaCups.Add(Replace(c, " ", "").Substring(0, Math.Min(20, c.Length)))
                                           Next
                                           Dim ConsultaFacturable As String = ConsultasSQL.GetCurvaFacturable(DesdeF, HastaF, listaCups)
                                           ExportarConsultaAExcel(conexionv2, ConsultaFacturable, rutaArchivoFacturable, Name)
                                           RutaFinal += " " + Name
                                       End Sub))
                End If
            End If

            If Norauto.Checked Then
                'Check Cliente
                If CheckBox3.Checked Then
                    Dim CIFS = GetConSinSplitCupsCIFS(TextBox2.Text)
                    If CIFS.Count > 0 Then
                        For Each cif In CIFS
                            tasks.Add(Task.Run(Sub()
                                                   Dim Name = cif
                                                   Dim rutaArchivoNor = IO.Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                                   Dim ConsultaNor As String = ConsultasSQL.GetConsultaNorauto(DesdeF, HastaF, cif)
                                                   ExportarConsultaAExcel(conexion, ConsultaNor, rutaArchivoNor, Name)
                                                   RutaFinal += " " + Name
                                               End Sub))
                        Next
                    End If
                End If
            End If



            ' Esperar a que todas las tareas se completen
            Await Task.WhenAll(tasks)
            PictureBox2.Visible = False
            complementos.Complementos_MostrarMensajePersonalizadoCopiar($"Consulta generada en:{RutaFinal}", "")
        Catch ex As Exception
            PictureBox2.Visible = False
            'PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado("Exception: " + ex.Message)
            'Finally
            'PictureBox2.Visible = False
            'PictureBox2.Visible = False
        End Try
    End Sub

    'DEVS
    Private Sub Button24_Click(sender As Object, e As EventArgs)
        ' Ruta de la carpeta donde están los archivos XML
        Dim carpetaXML = "C:\Users\ErickCC\Documents\DEV"

        ' Lista de números de factura a buscar
        Dim facturasBuscar As New List(Of String) From {
"FELEC1900012641", "FELEC2500002942", "FELEC2500008845", "FELEC2500009232", "FELEC2500009464", "FELEC2500013084", "FELEC2500013102", "FELEC2500017816", "FELEC2500057366", "FELEC2500063394",
"FELEC2500116144", "FELEC2500117856", "FELEC2500131639", "FELEC2500131640", "FELEC2500133562", "FELEC2500166645", "FELEC2500167023", "FELEC2500167137", "FELEC2500167335", "FELEC2500167577",
"FELEC2500167662", "FELEC2500168431", "FELEC2500168568", "FELEC2500168575", "FELEC2500169694", "FELEC2500169874", "FELEC2500170153", "FELEC2500170264", "FELEC2500170617", "FELEC2500170963",
"FELEC2500171175", "FELEC2500171191", "FELEC2500171258", "FELEC2500171345", "FELEC2500171527", "FELEC2500171576", "FELEC2500171891", "FELEC2500172076", "FELEC2500172560", "FELEC2500173062",
"FELEC2500174634", "FELEC2500174926", "FELEC2500175085", "FELEC2500175207", "FELEC2500175297", "FELEC2500175302", "FELEC2500175378", "FELEC2500175479", "FELEC2500175986", "FELEC2500176100",
"FELEC2500176303", "FELEC2500176318", "FELEC2500176664", "FELEC2500176685", "FELEC2500178613", "FELEC2500178727", "FELEC2500178872", "FELEC2500178873", "FELEC2500179240", "FELEC2500179245",
"FELEC2500179530", "FELEC2500179581", "FELEC2500179618", "FELEC2500179857", "FELEC2500179965", "FELEC2500180704", "FELEC2500180780", "FELEC2500181066", "FELEC2500183198", "FELEC2500183598",
"FELEC2500184134", "FELEC2500184511", "FELEC2500184694", "FELEC2500184698", "FELEC2500185704", "FELEC2500185745", "FELEC2500185856", "FELEC2500185933", "FELEC2500186089", "FELEC2500186192",
"FELEC2500186321", "FELEC2500186506", "FELEC2500187095", "FELEC2500187301", "FELEC2500187547", "FELEC2500187974", "FELEC2500188189", "FELEC2500188277", "FELEC2500188550", "FELEC2500188666",
"FELEC2500188977", "FELEC2500189231", "FELEC2500189452", "FELEC2500189467", "FELEC2500189489", "FELEC2500190413", "FELEC2500190591", "FELEC2500190909", "FELEC2500191110", "FELEC2500191376",
"FELEC2500191409", "FELEC2500191410", "FELEC2500191411", "FELEC2500191412", "FELEC2500191473", "FELEC2500191492", "FELEC2500191615", "FELEC2500191757", "FELEC2500191791", "FELEC2500191880",
"FELEC2500192084", "FELEC2500192133", "FELEC2500192289", "FELEC2500192304", "FELEC2500192445", "FELEC2500192524", "FELEC2500192680", "FELEC2500192874", "FELEC2500193911", "FELEC2500194060",
"FELEC2500194181", "FELEC2500194241", "FELEC2500194321", "FELEC2500194457", "FELEC2500194458", "FELEC2500194536", "FELEC2500194555", "FELEC2500194571", "FELEC2500194658", "FELEC2500194671",
"FELEC2500194673", "FELEC2500194714", "FELEC2500194728", "FELEC2500194733", "FELEC2500194752", "FELEC2500194782", "FELEC2500194912", "FELEC2500195068", "FELEC2500195095", "FELEC2500195359",
"FGAS2500013301", "FGAS2500013366", "FGAS2500015298", "FGAS2500015382", "FGAS2500015457", "FGAS2500015521", "FGAS2500015554", "FGAS2500016023", "FGAS2500016080", "FGAS2500016201",
"VARIOS2500001511", "VARIOS2500001753", "VARIOS2500002528", "VARIOS2500002530", "VARIOS2500002532", "VARIOS2500002533", "VARIOS2500002547", "VARIOS2500002550", "VARIOS2500002554", "VARIOS2500002596",
"VARIOS2500002635", "VARIOS2500003025", "VARIOS2500003039"
        }

        ' Archivo donde se guardarán los resultados
        Dim archivoResultados = "C:\Users\ErickCC\Documents\DEV\resultados.txt"

        ' Limpiar el archivo antes de escribir los resultados
        File.WriteAllText(archivoResultados, "")

        ' Lista para almacenar los archivos donde se encontraron facturas
        Dim archivosEncontrados As New List(Of String)

        ' Obtener todos los archivos XML en la carpeta
        For Each archivo In Directory.GetFiles(carpetaXML, "*.xml")
            ' Cargar el XML
            Dim doc = XDocument.Load(archivo)

            ' Buscar todos los <invoiceNumber> en el archivo
            Dim facturasEnXML = doc.Descendants.Where(Function(x) x.Name.LocalName = "invoiceNumber").Select(Function(x) x.Value)

            ' Verificar si alguna factura de la lista está en este XML
            Dim coincidencias = facturasBuscar.Intersect(facturasEnXML).ToList

            ' Si hay coincidencias, guardar el nombre del archivo
            If coincidencias.Any Then
                archivosEncontrados.Add(Path.GetFileName(archivo))
                ' Escribir en el archivo de texto
                File.AppendAllText(archivoResultados, $"Factura(s) {String.Join(", ", coincidencias)} encontrada(s) en: {Path.GetFileName(archivo)}{Environment.NewLine}")
            End If
        Next

        ' Mostrar mensaje final con los resultados
        If archivosEncontrados.Any Then
            Console.WriteLine($"Facturas encontradas en los archivos: {String.Join(", ", archivosEncontrados)}")
        Else
            Console.WriteLine("No se encontraron coincidencias.")
        End If
    End Sub

    ' Consulta CNAE by contratos
    Private Async Sub Button25_Click(sender As Object, e As EventArgs)
        Dim contratosActualizado = 0
        Dim tiempoTranscurrido As TimeSpan
        Try

            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog

            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaArchivo = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaArchivo = filename
                Next
            End If

            Dim stopwatch As New Stopwatch
            stopwatch.Start() ' Iniciar el cronómetro
            'Dim Empieza As TimeSpan = stopwatch.Elapsed
            Dim ActualizarEmail As New ActualizarEmailFromExcel(connectionString)
            If rutaArchivo.Length > 0 Then
                PictureBox2.Visible = True
                ActualizarEmail.RutaExcel = rutaArchivo
                contratosActualizado = Await Task.Run(Function() ActualizarEmail.ConsultaCNAE)

                ' Detener el cronómetro y obtener el tiempo transcurrido
                stopwatch.Stop()
                tiempoTranscurrido = stopwatch.Elapsed
            End If

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado($"Se han actualizado {contratosActualizado} contratos. Tiempo transcurrido: {tiempoTranscurrido.TotalMinutes} minutos.")
        End Try
    End Sub

    ' Actualiza masivamente contratos 
    Private Sub Button25_Click_1(sender As Object, e As EventArgs) Handles Button25.Click
        Try
            Dim ListaCodContrato As New List(Of Long)
            Dim EsLuz As Boolean = False
            Dim EsGas As Boolean = False
            Dim AmbosEntornos = False
            'Check Cups
            If CheckBox2.Checked Then
                Dim Con = GetConSinSplit(TextBox2.Text)
                If Con.Count > 0 Then
                    Dim contrato = Funciones.GetContratoMasivo(Con)
                    For Each elemnt In contrato
                        If elemnt.IdContrato > 0 Then
                            ListaCodContrato.Add(elemnt.CodigoContrato)
                            'Comprobamos el entorno
                            If elemnt.Entorno = "E1" Then
                                EsLuz = True
                            ElseIf elemnt.Entorno = "E2" Then
                                EsGas = True
                            End If
                        End If
                    Next
                End If
            End If
            If ListaCodContrato.Count > 0 Then
                AmbosEntornos = (EsLuz AndAlso EsGas)

                Dim ContratoForm As New ContratoForm(ListaCodContrato, connectionString, NombreUsuarioEquipo, AmbosEntornos)
                ContratoForm.Show()
            Else
                complementos.MostrarMensajePersonalizado($"No hay contratos")
            End If
        Catch ex As Exception

        End Try
    End Sub

    'Extrae documentos
    Private Async Sub Button24_Click_1(sender As Object, e As EventArgs) Handles Button24.Click
        Dim ExcelDatos As New Excel
        Dim Datos As New List(Of List(Of Object))
        Try
            Dim totalContratos = 0
            Dim ContratoActualizar As New List(Of Long)
            Dim IdDocumentos As New List(Of Long)
            'Con = GetConSinSplit(TextBox2.Text)
            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog
            Dim Destino = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO\DocumentosGenerales"
            If Not Directory.Exists(Destino) Then
                Directory.CreateDirectory(Destino)
            End If
            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaArchivo = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaArchivo = filename
                Next
            End If
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            If rutaArchivo.Length > 0 Then
                Using package As New ExcelPackage(New FileInfo(rutaArchivo))
                    Dim worksheet = package.Workbook.Worksheets(0)
                    Dim rowCount = worksheet.Dimension.Rows

                    ' Leer códigos de contrato del Excel
                    Dim codigosContrato As New List(Of Long)
                    For row = 2 To rowCount
                        Dim IdDocumento = worksheet.Cells(row, 1).Value?.ToString
                        'Dim idcontratotarifa = worksheet.Cells(row, 2).Value?.ToString
                        'Dim Cups As String = worksheet.Cells(row, 3).Value?.ToString()
                        If Not String.IsNullOrEmpty(IdDocumento) Then
                            IdDocumentos.Add(IdDocumento)
                        End If
                    Next
                End Using

                If IdDocumentos.Count > 0 Then
                    PictureBox2.Visible = True
                    Await Task.Run(Sub()

                                       For Each c In IdDocumentos
                                           'Comprobamos si ha traido el documento de BD
                                           Dim Doc As Byte() = Nothing
                                           If Doc Is Nothing Then ' si no lo ha traido, lo buscamos en disco
                                               Funciones.DocumentDataFromCopiaAnioSiNulo(Doc, c)
                                           End If
                                           If Doc Is Nothing Then
                                               Continue For
                                           End If
                                           Dim originalFileName = $"Documento_{c}.PDF"
                                           Dim nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName)
                                           Dim newFileName = Mid(nameWithoutExtension, 1, 100) & Path.GetExtension(originalFileName)
                                           Dim TempFileName = Path.Combine(Destino, newFileName)
                                           File.WriteAllBytes(TempFileName, Doc)
                                       Next
                                   End Sub)

                Else
                    complementos.MostrarMensajePersonalizado($"Sin Documentos")
                End If
                PictureBox2.Visible = False
            End If

        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado("Exception: " + ex.Message)
        End Try
    End Sub

    Private Sub Login_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        Dim rect As Rectangle = Me.ClientRectangle

        ' Evitar error si aún no tiene tamaño válido
        If rect.Width <= 0 OrElse rect.Height <= 0 Then
            Exit Sub
        End If

        ' Definimos los colores en RGB
        Dim color1 As System.Drawing.Color = System.Drawing.Color.FromArgb(160, 30, 34)
        Dim color2 As System.Drawing.Color = System.Drawing.Color.FromArgb(96, 109, 140)
        Dim color3 As System.Drawing.Color = System.Drawing.Color.FromArgb(233, 231, 226)

        ' Creamos el gradiente
        Using brush As New LinearGradientBrush(rect, color1, color3, 222.0F)
            Dim blend As New ColorBlend()
            blend.Colors = New System.Drawing.Color() {color1, color2, color3}
            blend.Positions = New Single() {0.0F, 0.5F, 1.0F}

            brush.InterpolationColors = blend
            e.Graphics.FillRectangle(brush, rect)
        End Using
    End Sub


    Private Sub Login_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Me.Invalidate() ' Obliga a repintar con el tamaño correcto
    End Sub

    Private Async Sub Button26_Click(sender As Object, e As EventArgs) Handles Button26.Click
        Dim ExcelDatos As New Excel
        Dim Datos As New List(Of ContratoTarifa)
        Dim ListaErrores As New List(Of String)
        Try
            Dim totalContratos = 0
            Dim ContratoActualizar As New List(Of Long)

            'Con = GetConSinSplit(TextBox2.Text)
            ' Crear una instancia de OpenFileDialog
            Dim openFileDialog1 As New OpenFileDialog
            Dim Destino = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO\DocumentosGenerales"
            If Not Directory.Exists(Destino) Then
                Directory.CreateDirectory(Destino)
            End If
            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaArchivo = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaArchivo = filename
                Next
            End If
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            If rutaArchivo.Length > 0 Then
                Using package As New ExcelPackage(New FileInfo(rutaArchivo))
                    Dim worksheet = package.Workbook.Worksheets("Update")
                    Dim rowCount = worksheet.Dimension.Rows

                    ' Leer códigos de contrato del Excel
                    Dim codigosContrato As New List(Of Long)
                    For row = 2 To rowCount
                        Dim contrat As New ContratoTarifa
                        Dim CodContrato = worksheet.Cells(row, 1).Value?.ToString
                        Dim FechaContrato = worksheet.Cells(row, 7).Value?.ToString
                        Dim IdTarifagrupo = worksheet.Cells(row, 12).Value?.ToString
                        Dim idContratoTarifa = worksheet.Cells(row, 14).Value?.ToString

                        Dim FechaContratoExcel As DateTime?

                        If FechaContrato IsNot Nothing Then
                            Dim fechaTmp As DateTime
                            If DateTime.TryParse(FechaContrato.ToString(), fechaTmp) Then
                                FechaContratoExcel = fechaTmp
                            End If
                        End If

                        contrat.IdContratoTarifa = idContratoTarifa
                        contrat.CodigoContrato = CodContrato
                        contrat.FechaDesde = FechaContratoExcel
                        contrat.IdTarifaGrupo = IdTarifagrupo
                        Datos.Add(contrat)
                    Next
                End Using

                If Datos.Count > 0 Then
                    Dim yesorNot1 = MsgBox($"Hay {Datos.Count} contratos, ¿Aplicar precios?", vbYesNo)
                    If yesorNot1 = 6 OrElse yesorNot1 = 1 Then

                        PictureBox2.Visible = True
                        Await Task.Run(Sub()
                                           For Each d In Datos
                                               Dim ContratosC = Funciones.GetContratoTarifaExcel(d)
                                               If Not ContratosC Is Nothing AndAlso ContratosC.IdContratoTarifa > 0 Then
                                                   Dim FechaVigencia = d.FechaDesde
                                                   Try
                                                       Funciones.aplicapreciosFromEcel(d)
                                                   Catch ex As Exception
                                                       ListaErrores.Add($"{d.CodigoContrato}_ idcontratotarifa:{d.IdContratoTarifa} ->  {ex.Message} ")
                                                   End Try
                                               Else
                                                   ListaErrores.Add($"{d.CodigoContrato} no se ha encontrado el tarifagrupo a aplicar precios")
                                               End If
                                           Next
                                       End Sub)
                    End If
                Else
                    'complementos.MostrarMensajePersonalizado($"Sin Datos")
                End If
                PictureBox2.Visible = False
            End If

        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado("Exception: " + ex.Message)
        Finally
            For Each er In ListaErrores
                EscribirEnArchivo(er)
            Next
            complementos.MostrarMensajePersonalizado($"Contrato actualizados")
        End Try
    End Sub

    Public Sub EscribirEnArchivo(Errores As String)
        Try
            ' Si el archivo no existe, se creará; de lo contrario, se anexará al archivo existente
            Using writer As StreamWriter = New StreamWriter($"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO\archivo.txt", True)
                ' Escribir los valores en el archivo de texto
                writer.WriteLine($"{Errores}")
            End Using
        Catch ex As Exception
            ' Manejar cualquier excepción que pueda ocurrir durante la escritura en el archivo
            Console.WriteLine("Error al escribir en el archivo: " & ex.Message)
        End Try
    End Sub

End Class
