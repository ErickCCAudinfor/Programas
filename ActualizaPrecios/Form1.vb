
Imports System.Collections.Concurrent
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Reflection.PortableExecutable
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml
Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml.Wordprocessing
Imports OfficeOpenXml
Imports PdfSharp.Pdf
Imports PdfSharp.Pdf.IO

Public Class Form1


    Dim complementos As New Complementos()
    Public Property NombreLogin As String = ""
    'Dim LoadingWF As New LoadingWF
    Private Property ipDB As String = "data source=172.31.100.12"
    'Private ReadOnly Property ipDB As String = "data source=172.31.100.50\TOTALUAT;"
    Private Property nameDB As String = "initial catalog=SigeTotal;"
    'Private ReadOnly Property nameDB As String = "initial catalog=SigeTotalUAT;"
    Private ReadOnly Property userDB As String = "User ID=Sige;"
    Private ReadOnly Property passDB As String = "Password=SigeNew;"
    Private ReadOnly Property NombreUsuarioEquipo As String = Environment.UserName
    Private Property connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"

    Private Property rutaCarpetaGlobal = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO"
    Private ContratoTarifaSrv As New ContratoTarifaSrv(connectionString)

    Private Funciones As New FuncionesGenericas(connectionString)

    Private isExpanded As Boolean = False ' Para rastrear si la pestaña está expandida o contraída
    Public Sub New(NombreLogin As String)
        InitializeComponent()
        Me.NombreLogin = NombreLogin
        Text += " - " + NombreLogin
    End Sub

    Private Sub SetTextSafe(ctrl As System.Windows.Forms.Control, text As String)
        If ctrl.InvokeRequired Then
            ctrl.Invoke(New Action(Sub() ctrl.Text = text))
        Else
            ctrl.Text = text
        End If
    End Sub

    Private Sub MostrarLoading(mostrar As Boolean)
        PictureBox2.Visible = mostrar
        TextConsultando.Visible = mostrar
        If Not mostrar Then TextConsultando.Text = ""
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

    'Private Async Sub Actualizar(sender As Object, e As EventArgs) Handles Button1.Click
    '    Dim ExcelDatos As New Excel
    '    Dim Datos As New List(Of List(Of Object))
    '    Try
    '        Dim totalContratos = 0
    '        Dim ContratoActualizar As New List(Of Long)
    '        Dim Con As New List(Of Long)
    '        Con = GetConSinSplit(TextBox2.Text)

    '        If CheckBox1.Checked Then 'CUPS
    '            'Dim Cups As New List(Of String)
    '            'Cups.Add("ES0027700038574004TJ")
    '            'Contratos = Funciones.BuscarbyCups(Cups, ipDB, nameDB, userDB, passDB)

    '        End If
    '        If CheckBox2.Checked Then 'Contrato
    '            Dim yesorNot1 = MsgBox($"Hay un total de {Con.Count} contratos, ¿Seguir con la actualización?", vbYesNo)
    '            If yesorNot1 = 6 OrElse yesorNot1 = 1 Then
    '                PictureBox2.Visible = True
    '                ContratoActualizar = Await Task.Run(Function() Funciones.BuscarbyCodigocontrato(Con))
    '                totalContratos = Con.Count
    '                PictureBox2.Visible = False
    '            End If
    '        End If
    '        'If CheckBox3.Checked Then 'Cliente

    '        'End If
    '        Dim yesorNot As MsgBoxResult
    '        Dim todoOK = False
    '        'Escribo los valores que tiene ahora, para posteriormente comparar o hacer uso de este y dejarlo como esta
    '        Funciones.EscribirContratoTarifaAntesCambios(ContratoActualizar)
    '        If ContratoActualizar.Count > 0 Then
    '            If totalContratos <> ContratoActualizar.Count Then
    '                yesorNot = MsgBox("Los contratos filtratos y los contratos encontrados no coinciden. ¿Actualizar de todas formas?", vbYesNo)
    '            Else
    '                todoOK = True
    '            End If
    '            If yesorNot = 6 OrElse yesorNot = 1 OrElse todoOK Then
    '                PictureBox2.Visible = True
    '                Dim ContratosTXT = Await Task.Run(Function() ActualizarRegistros(ContratoActualizar, Datos))
    '                PictureBox2.Visible = False

    '                If Not IsNothing(Datos) AndAlso Datos.Count > 0 Then
    '                    ExcelDatos.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos, "PreciosErrores")
    '                End If
    '                complementos.MostrarMensajePersonalizado($"{ContratosTXT}. Contratos iniciales:{ContratoActualizar.Count} contratos")
    '            Else
    '                complementos.MostrarMensajePersonalizado($"Se ha cancelado la actualización")
    '            End If
    '        Else
    '            complementos.MostrarMensajePersonalizado($"Sin Contratos")
    '        End If

    '    Catch ex As Exception
    '        PictureBox2.Visible = False
    '        complementos.MostrarMensajePersonalizado("Exception: " + ex.Message)
    '    End Try

    'End Sub
#Region "Actualizar Precios Refactorizado"

    Private Async Sub Actualizar(sender As Object, e As EventArgs) Handles BotonActualizar.Click
        Try
            Dim ExcelDatos As New Excel
            PictureBox2.Visible = False

            Dim contratosEntrada = GetConSinSplit(TextBox2.Text)
            Dim contratosFiltrados = Await ObtenerContratos(contratosEntrada)

            If Not ConfirmarActualizar(contratosEntrada.Count, contratosFiltrados.Count) Then
                complementos.MostrarMensajePersonalizado("Se ha cancelado la actualización")
                Exit Sub
            End If

            Funciones.EscribirContratoTarifaAntesCambios(contratosFiltrados)

            Dim Datos As New List(Of List(Of Object))

            PictureBox2.Visible = True
            Dim resultado = Await ProcesarContratos(contratosFiltrados, Datos)
            PictureBox2.Visible = False

            ' Exportación centralizada
            ExportarErrores(Datos)

            complementos.MostrarMensajePersonalizado($"{resultado}. Contratos iniciales: {contratosFiltrados.Count}")

        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado("Exception: " & ex.Message)
        End Try
    End Sub


    Private Async Function ObtenerContratos(entrada As List(Of Long)) As Task(Of List(Of Long))
        If CheckBox2.Checked Then
            Dim respuesta = MsgBox($"Hay un total de {entrada.Count} contratos. ¿Continuar?",
                               vbYesNo)

            If respuesta <> vbYes Then
                Return New List(Of Long)
            End If

            Return Await Task.Run(Function() Funciones.BuscarbyCodigocontrato(entrada))
        End If

        Return New List(Of Long)
    End Function

    Private Function ConfirmarActualizar(totalEntrada As Integer, totalFiltrados As Integer) As Boolean
        If totalFiltrados = 0 Then
            complementos.MostrarMensajePersonalizado("Sin contratos")
            Return False
        End If

        If totalEntrada <> totalFiltrados Then
            Dim r = MsgBox("Los contratos filtrados y los encontrados no coinciden. ¿Actualizar igual?", vbYesNo)
            Return r = vbYes
        End If

        Return True
    End Function

    Private Async Function ProcesarContratos(lista As List(Of Long), datos As List(Of List(Of Object))) As Task(Of String)
        Return Await Task.Run(Function() ActualizarRegistros(lista, datos))
    End Function


    Private Sub ExportarErrores(datos As List(Of List(Of Object)))
        If datos Is Nothing OrElse datos.Count = 0 Then Exit Sub

        Dim excel = New Excel()
        Dim ruta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\"
        excel.EscribirEnExcel(ruta, datos, "PreciosErrores")
    End Sub

    Private Function ActualizarRegistros(ListaCodigo As List(Of Long),
                                     ByRef Datos As List(Of List(Of Object))) As String

        Dim contratosActualizados As Integer = 0
        Dim msgContratosNoActualizados As String = "Contratos sin actualizarse: "

        Try
            Dim contratosTarifa = ObtenerContratosTarifaValidos(ListaCodigo)

            If contratosTarifa.Count <> ListaCodigo.Count Then
                complementos.MostrarMensajePersonalizado("Las listas no coinciden")
                Return "Error: listas no coinciden"
            End If

            For Each ct In contratosTarifa
                ProcesarContratoTarifa(ct, contratosActualizados, Datos)
            Next

        Catch ex As Exception
            RegistrarError(Datos, $"Error general: {ex.Message}")
            Throw

        End Try

        Return $"Se han actualizado {contratosActualizados}. {msgContratosNoActualizados}"
    End Function

    Private Function ObtenerContratosTarifaValidos(lista As List(Of Long)) As List(Of ContratoTarifa)
        Dim result As New List(Of ContratoTarifa)

        For Each cod In lista
            Dim ct = ContratoTarifaSrv.GetContratoTarifaPersonalizadaByCodigoContrato(
                     cod, TextViejoTarifaGrupo.Text, CheckBox4.Checked)

            If ct IsNot Nothing AndAlso ct.IdContratoTarifa > 0 Then
                result.Add(ct)
            End If
        Next

        Return result
    End Function

    Private Sub ProcesarContratoTarifa(elment As ContratoTarifa,
                                   ByRef contador As Integer,
                                   ByRef datos As List(Of List(Of Object)))

        If elment Is Nothing OrElse elment.IdContratoTarifa <= 0 Then
            RegistrarError(datos, $"Contrato {elment?.CodigoContrato}: ContratoTarifa vacío")
            Exit Sub
        End If

        Dim contratoAct = ContratoTarifaSrv.UpdateContratoTarifa(elment, TextTarifaGrupo.Text, TextViejoTarifaGrupo.Text, CheckBox4.Checked)

        If contratoAct Is Nothing OrElse contratoAct.IdContratoTarifa <= 0 Then
            RegistrarError(datos, $"Contrato {elment.CodigoContrato}: No se pudo actualizar ContratoTarifa")
            Exit Sub
        End If

        contratoAct.PerfilFacturacion = Funciones.GetPerfilFacturacion(contratoAct.IdPerfilFacturacion)

        If contratoAct.PerfilFacturacion Is Nothing Then
            RegistrarError(datos, $"Contrato {elment.CodigoContrato}: Sin PerfilFacturacion")
            ContratoTarifaSrv.UpdateContratoTarifaSiError(elment)
            Exit Sub
        End If

        Dim preciosNuevos = ObtenerPreciosNuevos(contratoAct)
        If preciosNuevos.Count = 0 Then
            RegistrarError(datos, $"Contrato {elment.CodigoContrato}: Sin precios nuevos")
            ContratoTarifaSrv.UpdateContratoTarifaSiError(elment)
            Exit Sub
        End If

        ' TDVE no sustituye, inserta directamente
        If elment.TextoTarifa.Contains("TDVE") Then
            Funciones.InsertTarifaPrecioContrato(preciosNuevos)
            contador += 1
            Exit Sub
        End If

        Dim preciosViejos = ObtenerPreciosAntiguos(elment)
        If preciosViejos.Count = 0 Then
            RegistrarError(datos, $"Contrato {elment.CodigoContrato}: Sin precios antiguos")
            ContratoTarifaSrv.UpdateContratoTarifaSiError(elment)
            Exit Sub
        End If

        Funciones.UpdatePrecioContratoTarifa(
        preciosNuevos,
        preciosViejos,
        contratoAct.PerfilFacturacion.isPerfilIndexado()
    )

        contador += 1
    End Sub


    Private Function ObtenerPreciosNuevos(contrato As ContratoTarifa) As List(Of TarifaPrecioContrato)
        Dim lista As New List(Of TarifaPrecioContrato)
        Dim ContratoBD As Contrato = Funciones.GetContrato(If(contrato.CodigoContrato, 0L))
        Dim fecha As DateTime = ObtenerFechaPresupuesto(ContratoBD)
        Dim esIndexado As Boolean = contrato.PerfilFacturacion.isPerfilIndexado()

        If esIndexado Then
            ' --- INDEXADO ---
            If contrato.Entorno = "G1" Then
                Dim precios = Funciones.GetDTOAllPeriodosIndx(contrato.IdTarifa, contrato.IdTarifaGrupo, fecha).OrderBy(Function(f) f.IdIndexadoPrecio).ToList()
                For Each p In precios
                    lista.Add(New TarifaPrecioContrato With {
                    .IdContratoTarifa = contrato.IdContratoTarifa,
                    .IdIndexadoPrecio = p.IdIndexadoPrecio,
                    .IdTarifaPeriodo = p.IdTarifaPeriodo,
                    .TextoTarifaPeriodo = p.TextoTarifaPeriodo,
                    .Entorno = p.Entorno
                })
                Next

            Else ' G2 (Gas)
                Dim precios = Funciones.GetDTOAllPeriodosIndxGasByFechaFinPresupuesto(contrato.Entorno, contrato.IdTarifa, contrato.IdTarifaGrupo, fecha)
                For Each p In precios
                    lista.Add(New TarifaPrecioContrato With {
                    .IdContratoTarifa = contrato.IdContratoTarifa,
                    .IdIndexadoPrecioGas = p.IdIndexadoPrecioGas,
                    .IdTarifaPeriodo = p.IdTarifaPeriodo,
                    .TextoTarifaPeriodo = p.TextoTarifaPeriodo,
                    .Entorno = p.Entorno
                })
                Next
            End If

        Else
            ' --- FIJO ---
            Dim precios = Funciones.GetDTOAllPeriodosTarifaPrecio(contrato.IdTarifa, contrato.IdTarifaGrupo, fecha).OrderBy(Function(f) f.IdTarifaPrecio).ToList()

            For Each p In precios
                lista.Add(New TarifaPrecioContrato With {
                .IdContratoTarifa = contrato.IdContratoTarifa,
                .IdTarifaPrecio = p.IdTarifaPrecio,
                .IdTarifaPeriodo = p.IdTarifaPeriodo,
                .TextoTarifaPeriodo = p.TextoTarifaPeriodo,
                .Entorno = p.Entorno
            })
            Next
        End If

        Return lista
    End Function

    Private Function ObtenerPreciosAntiguos(contrato As ContratoTarifa) As List(Of TarifaPrecioContrato)
        Dim lista As New List(Of TarifaPrecioContrato)
        Dim precio = Funciones.GetPrecioContratoTarifaV2(contrato)

        If precio Is Nothing OrElse precio.IdContratoTarifa <= 0 Then
            Return lista
        End If

        If contrato.Entorno = "G1" AndAlso precio.IdIndexadoPrecio > 0 Then
            lista = Funciones.GetPrecioContratoTarifaIndex(contrato).OrderBy(Function(f) f.IdTarifaPeriodo).ToList()

        ElseIf (contrato.Entorno = "G1" OrElse contrato.Entorno = "G2") AndAlso precio.IdTarifaPrecio > 0 Then
            lista = Funciones.GetPrecioContratoTarifa(contrato).OrderBy(Function(f) f.IdTarifaPeriodo).ToList()

        ElseIf contrato.Entorno = "G2" AndAlso precio.IdIndexadoPrecioGas > 0 Then
            lista = Funciones.GetPrecioContratoTarifaIndexGas(contrato).OrderBy(Function(f) f.IdTarifaPeriodo).ToList()

        End If

        Return lista
    End Function

    Private Function ObtenerFechaPresupuesto(contrato As Contrato) As DateTime
        If contrato Is Nothing Then Return DateTime.Today

        If contrato.FechaAplicacionPrecios.HasValue AndAlso contrato.FechaAplicacionPrecios > Date.MinValue Then
            Return contrato.FechaAplicacionPrecios
        End If

        If contrato.FechaContrato.HasValue AndAlso contrato.FechaContrato > Date.MinValue Then
            Return contrato.FechaContrato
        End If

        Return DateTime.Today
    End Function



    Private Sub RegistrarError(datos As List(Of List(Of Object)), mensaje As String)
        datos.Add(New List(Of Object) From {mensaje})
    End Sub

    'Private Function ActualizarRegistros_OLD(ListaCodigo As List(Of Long), ByRef Datos As List(Of List(Of Object))) As String
    '    Dim Contador = 0I
    '    Dim ContratosSinActualizar = "Contratos sin actualizarse: "

    '    'Dim Datos As New List(Of List(Of Object))()
    '    Try
    '        Dim ContratoTra As New List(Of ContratoTarifa)
    '        Dim ContratoTraMergeado As New List(Of ContratoTarifa)

    '        'Me busco solo contratos que tengan fechaHasta is null y sea personalizada
    '        For Each cod In ListaCodigo
    '            Dim CT = ContratoTarifaSrv.GetContratoTarifaPersonalizadaByCodigoContrato(cod, TextBox3.Text, CheckBox4.Checked)
    '            If Not CT Is Nothing AndAlso CT.IdContratoTarifa > 0 Then
    '                ContratoTra.Add(CT)
    '            End If

    '        Next

    '        Dim pepe = 1
    '        ' Dependiendo de si el PerfilFacturacion del ContratoTarifa es indexado,
    '        ' cargaremos los precios de IndexadoPrecioSrv o TarifaPrecioSrv.
    '        ' Estos precios serán los mismos que se muestran en el programa de presupuestos,
    '        ' los vigentes para una Tarifa, TarifaGrupo y fecha concreta.

    '        ' Guardamos el nuevo ContratoTarifa, le pasamos la tarifagrupo a asignar

    '        If ListaCodigo.Count = ContratoTra.Count Then

    '            For Each elment In ContratoTra

    '                If Not IsNothing(elment.IdContratoTarifa) AndAlso elment.IdContratoTarifa > 0 Then
    '                    'ContratoTraMergeado.Add(ContratoTarifaSrv.UpdateContratoTarifa(elment, NuevaTarifaGrupo, TarifaGrupoActual, IsPersonalizada))
    '                    Dim ContratoActualizar = ContratoTarifaSrv.UpdateContratoTarifa(elment, TextBox1.Text, TextBox3.Text, CheckBox4.Checked)
    '                    'Dim ContratoTarifaViejo = elment
    '                    If Not IsNothing(ContratoActualizar) AndAlso ContratoActualizar.IdContratoTarifa > 0 Then
    '                        ' Dependiendo de si el PerfilFacturacion del ContratoTarifa es indexado,
    '                        ' cargaremos los precios de IndexadoPrecioSrv o TarifaPrecioSrv.
    '                        ' Estos precios serán los mismos que se muestran en el programa de presupuestos,
    '                        ' los vigentes para una Tarifa, TarifaGrupo y fecha concreta.
    '                        Dim Contrato As Contrato = Funciones.GetContrato(If(elment.CodigoContrato, 0L))
    '                        Dim FechaPresupuesto As DateTime = DateTime.Today 'New DateTime(Now.Year, Now.Month, Now.Day)
    '                        If Not IsNothing(Contrato) AndAlso Contrato.IdContrato > 0L Then
    '                            If Not IsNothing(Contrato.FechaAplicacionPrecios) AndAlso Contrato.FechaAplicacionPrecios > DateTime.MinValue Then
    '                                FechaPresupuesto = If(Contrato.FechaAplicacionPrecios, DateTime.Now)
    '                            Else
    '                                If Not IsNothing(Contrato.FechaContrato) AndAlso Contrato.FechaContrato > DateTime.MinValue Then
    '                                    FechaPresupuesto = If(Contrato.FechaContrato, DateTime.Now)
    '                                End If
    '                            End If

    '                        End If



    '                        If Not IsNothing(ContratoActualizar) Then
    '                            ContratoActualizar.PerfilFacturacion = Funciones.GetPerfilFacturacion(If(ContratoActualizar.IdPerfilFacturacion, 0))

    '                            If Not IsNothing(ContratoActualizar.PerfilFacturacion) Then
    '                                Dim tarifasPrecioContratoGuardar As New List(Of TarifaPrecioContrato) ' Lista para guardar los nuevos precios
    '                                Dim OldtarifasPrecioContratoQuitar As New List(Of TarifaPrecioContrato) ' Lista a con los viejos precios
    '                                'Dim TarifaPerdidaCalculadaContratoGuardar As New ObservableCollection(Of TarifaPerdidaCalculadaContratoDTO)
    '                                Dim isFijoIndex As Boolean = False
    '                                'Compruebo si el nuevocontratotarifa es fijo o indexado
    '                                If ContratoActualizar.PerfilFacturacion.isPerfilIndexado() Then
    '                                    isFijoIndex = True
    '                                    If ContratoActualizar.Entorno = "G1" Then

    '                                        Dim indexadosPrecios As List(Of IndexadoPrecio) = Funciones.GetDTOAllPeriodosIndx(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto).OrderBy(Function(f) f.IdIndexadoPrecio).ToList
    '                                        ''Avisar si no hay precios para grabar
    '                                        If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
    '                                            For Each eleindexadoPrecio As IndexadoPrecio In indexadosPrecios
    '                                                Dim tarifaPrecioContrato As New TarifaPrecioContrato

    '                                                tarifaPrecioContrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
    '                                                tarifaPrecioContrato.IdIndexadoPrecio = eleindexadoPrecio.IdIndexadoPrecio
    '                                                tarifaPrecioContrato.TextoTarifaPeriodo = eleindexadoPrecio.TextoTarifaPeriodo
    '                                                tarifaPrecioContrato.IdTarifaPeriodo = eleindexadoPrecio.IdTarifaPeriodo
    '                                                tarifaPrecioContrato.Entorno = eleindexadoPrecio.Entorno
    '                                                tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
    '                                            Next
    '                                        Else
    '                                            ''Avisar si no hay precios para grabar
    '                                            'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_TARIFA_PRECIO_VIGENTE"))
    '                                        End If
    '                                    Else
    '                                        Dim indexadosPrecios As List(Of IndexadoPrecioGas) = Funciones.GetDTOAllPeriodosIndxGas(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto).OrderBy(Function(f) f.IdIndexadoPrecioGas).ToList

    '                                        ''Avisar si no hay precios para grabar
    '                                        If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
    '                                            For Each indexadoPrecio As IndexadoPrecioGas In indexadosPrecios
    '                                                Dim tarifaPrecioContrato As New TarifaPrecioContrato

    '                                                tarifaPrecioContrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
    '                                                tarifaPrecioContrato.IdIndexadoPrecioGas = indexadoPrecio.IdIndexadoPrecioGas
    '                                                tarifaPrecioContrato.IdTarifaPeriodo = indexadoPrecio.IdTarifaPeriodo
    '                                                tarifaPrecioContrato.TextoTarifaPeriodo = indexadoPrecio.TextoTarifaPeriodo
    '                                                tarifaPrecioContrato.Entorno = indexadoPrecio.Entorno
    '                                                tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
    '                                            Next
    '                                        End If
    '                                    End If

    '                                Else
    '                                    Dim tarifasPrecios As List(Of TarifaPrecio) = Funciones.GetDTOAllPeriodosTarifaPrecio(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto).OrderBy(Function(f) f.IdTarifaPrecio).ToList

    '                                    If Not IsNothing(tarifasPrecios) AndAlso tarifasPrecios.Count > 0 Then
    '                                        For Each tarifaPrecio As TarifaPrecio In tarifasPrecios
    '                                            Dim tarifapreciocontrato As New TarifaPrecioContrato

    '                                            tarifapreciocontrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
    '                                            tarifapreciocontrato.IdTarifaPrecio = tarifaPrecio.IdTarifaPrecio
    '                                            tarifapreciocontrato.IdTarifaPeriodo = tarifaPrecio.IdTarifaPeriodo
    '                                            tarifapreciocontrato.TextoTarifaPeriodo = tarifaPrecio.TextoTarifaPeriodo
    '                                            tarifapreciocontrato.Entorno = tarifaPrecio.Entorno
    '                                            tarifasPrecioContratoGuardar.Add(tarifapreciocontrato)
    '                                        Next
    '                                    Else
    '                                        ''Avisar si no hay precios para grabar
    '                                        'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_TARIFA_PRECIO_VIGENTE"))
    '                                    End If

    '                                End If
    '                                If elment.TextoTarifa.Contains("TDVE") Then

    '                                    Funciones.InsertTarifaPrecioContrato(tarifasPrecioContratoGuardar)
    '                                    Contador += 1
    '                                Else

    '                                    'Compruebo si el contratotarifaviejo es fijo o indexado
    '                                    Dim TaPrecionContrato = Funciones.GetPrecioContratoTarifaV2(elment)
    '                                    If Not IsNothing(TaPrecionContrato) AndAlso TaPrecionContrato.IdContratoTarifa > 0 Then
    '                                        If elment.Entorno = "G1" AndAlso TaPrecionContrato.IdIndexadoPrecio > 0 Then
    '                                            OldtarifasPrecioContratoQuitar = Funciones.GetPrecioContratoTarifaIndex(elment).OrderBy(Function(f) f.IdTarifaPeriodo).ToList
    '                                        ElseIf (elment.Entorno = "G1" OrElse elment.Entorno = "G2") AndAlso TaPrecionContrato.IdTarifaPrecio > 0 Then
    '                                            OldtarifasPrecioContratoQuitar = Funciones.GetPrecioContratoTarifa(elment).OrderBy(Function(f) f.IdTarifaPeriodo).ToList
    '                                        ElseIf elment.Entorno = "G2" AndAlso TaPrecionContrato.IdIndexadoPrecioGas > 0 Then
    '                                            OldtarifasPrecioContratoQuitar = Funciones.GetPrecioContratoTarifaIndexGas(elment).OrderBy(Function(f) f.IdTarifaPeriodo).ToList
    '                                        Else
    '                                            Datos.Add(New List(Of Object) From {$"Contrato: {elment.CodigoContrato}- idContratotarifa ={elment.IdContratoTarifa} - No actualizado - Sin precios - revisar "})
    '                                            Continue For
    '                                        End If
    '                                    Else
    '                                        Datos.Add(New List(Of Object) From {$"Contrato: {elment.CodigoContrato} - idContratotarifa ={elment.IdContratoTarifa} - El precio personalizado no existe, Imposible actualizar a la nueva tarifagrupo"})
    '                                        Continue For
    '                                        'Throw New Exception("El precio personalizado no existe, Imposible actualizar a la nueva tarifagrupo " + elment.CodigoContrato)
    '                                    End If


    '                                    'Compruebo que haya valores en los dos sitios
    '                                    If Not IsNothing(tarifasPrecioContratoGuardar) AndAlso tarifasPrecioContratoGuardar.Count > 0 AndAlso Not IsNothing(OldtarifasPrecioContratoQuitar) AndAlso OldtarifasPrecioContratoQuitar.Count > 0 Then
    '                                        ' Guardamos todos los registros de TarifaPrecioContrato generados.
    '                                        Funciones.UpdatePrecioContratoTarifa(tarifasPrecioContratoGuardar, OldtarifasPrecioContratoQuitar, isFijoIndex)
    '                                        Contador += 1
    '                                    Else
    '                                        Datos.Add(New List(Of Object) From {$"Contrato: {elment.CodigoContrato} _ idContratotarifa ={elment.IdContratoTarifa} , precios no encontrados "})
    '                                        Continue For
    '                                        'Throw New Exception("Imposible continuar, precios no encontrados. Contrato: " + elment.CodigoContrato)
    '                                    End If
    '                                End If
    '                                'objTarifaPerdidaCalculadaSrv.MergeUpdateTarifaPerdidaCalculadaContratoDTO(TarifaPerdidaCalculadaContratoGuardar, New ObservableCollection(Of TarifaPerdidaCalculadaContratoDTO))
    '                                'If Actualizado > 0 Then
    '                                '    Contador = +1
    '                                'Else
    '                                '    ContratosSinActualizar += elment.CodigoContrato + " "
    '                                'End If
    '                            Else
    '                                'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_PERFIL_FACTURACION"))
    '                            End If
    '                        End If
    '                    End If
    '                Else
    '                    'Throw New Exception("Se cancela, no hay registros a actualizar")
    '                    Datos.Add(New List(Of Object) From {$"Contrato: {elment.CodigoContrato} _ idContratotarifa ={elment.IdContratoTarifa} , ContratoTarifa Vacio "})
    '                End If

    '            Next
    '        Else
    '            complementos.MostrarMensajePersonalizado("las listas no coinciden")

    '        End If

    '    Catch ex As Exception
    '        Datos.Add(New List(Of Object) From {$"Error: {ex.Message}"})
    '        Throw
    '    Finally
    '        'ExcelDatos.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos)
    '    End Try
    '    Return $"Se han actualizado {Contador}. {ContratosSinActualizar}"
    'End Function
#End Region

#Region "Cosas basicas"
    'Limpiar filtros
    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Try

            TextBox2.Text = ""
            TextTarifaGrupo.Text = ""
            TextViejoTarifaGrupo.Text = ""

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub


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
            TextTarifaGrupo.Enabled = True
            Button17.Enabled = True
            'TextBox2.Height = TextRenderer.MeasureText(TextBox2.Text, TextBox2.Font, New Size(TextBox2.Width, Int32.MaxValue), TextFormatFlags.WordBreak).Height + 5 ' Añade un pequeño margen
        Else
            ' Si CheckBox3 no está marcado, habilitar CheckBox1 y CheckBox2
            TextTarifaGrupo.Enabled = False
            Button17.Enabled = False
        End If

    End Sub

    'Check box personalizado, habilita o deshabilita
    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        Try
            If CheckBox4.Checked = False Then
                TextViejoTarifaGrupo.Enabled = True
            End If

            If CheckBox4.Checked Then
                TextViejoTarifaGrupo.Enabled = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub HabilitarDesHabilitarButtons(Habilitar As Boolean)
        BotonActualizar.Enabled = Habilitar
        Button2.Enabled = Habilitar
        Button3.Enabled = Habilitar
        Button5.Enabled = Habilitar
        Button6.Enabled = Habilitar
        Button7.Enabled = Habilitar
        Button8.Enabled = Habilitar
        Button9.Enabled = Habilitar
        Button11.Enabled = Habilitar
    End Sub
#End Region

#Region "Control para los datos ingresados en la caja de texto"
    Private Function SplitEntrada(texto As String, Optional quitarUnderscore As Boolean = False) As List(Of String)
        Dim resultado As New List(Of String)

        Try
            If String.IsNullOrWhiteSpace(texto) Then Return resultado

            ' Normalizar saltos de línea: convertir CRLF y CR a LF
            Dim normalizado = texto.Replace(vbCrLf, ControlChars.Lf).Replace(vbCr, ControlChars.Lf)

            ' Delimitadores: coma y salto de línea (LF)
            Dim delimiters As Char() = {","c, ControlChars.Lf}

            ' Dividir sin eliminar elementos vacíos inicialmente (por si hay espacios)
            Dim partes = normalizado.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)

            For Each p In partes
                Dim item = p.Trim()
                ' Quitar espacios internos y tabs
                item = item.Replace(" "c, "").Replace(ControlChars.Tab, "")

                If quitarUnderscore Then
                    item = item.Replace("_", "")
                End If

                If item.Length > 0 Then resultado.Add(item)
            Next

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try

        Return resultado
    End Function



    'Para separar los contratos introducidos con comas(,)
    Private Function GetConSinSplit(contxt As String) As List(Of Long)
        Dim salida As New List(Of Long)

        For Each t In SplitEntrada(contxt)
            Dim n As Long
            If Long.TryParse(t, n) Then salida.Add(n)
        Next

        Return salida
    End Function

    'Sirve para CUPS,CIFS o contratos
    Private Function GetConSinSplitCupsCIFS(contxt As String) As List(Of String)
        Return SplitEntrada(contxt)
    End Function

    'Para separar las facturas
    Private Function GetFacsSinSplit(Facs As String) As List(Of String)
        Return SplitEntrada(Facs, quitarUnderscore:=True)
    End Function
#End Region

    'Habilitar o deshabilitar el botón de actualizar si no hay un texto de tarifa grupo 
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextTarifaGrupo.TextChanged
        Try
            If TextTarifaGrupo.Text.Trim.Length > 0 Then
                BotonActualizar.Enabled = True
            Else
                BotonActualizar.Enabled = False
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Productos Asignacion, si no ha escrito nada en textotarifagrupo no buscamos nada, y enviamos mensaje
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            ' Solo habilitado si el check de contratos está activo
            If CheckBox1.Checked OrElse CheckBox3.Checked Then
                complementos.MostrarMensajePersonalizado("Habilitado solo para el check de contratos")
                Exit Sub
            End If

            ' Obtener lista de contratos
            Dim contratos = GetConSinSplit(TextBox2.Text)

            If contratos Is Nothing OrElse contratos.Count = 0 Then
                complementos.MostrarMensajePersonalizado("Ingrese al menos un contrato")
                Exit Sub
            End If

            ' Determinar entorno basado en el primer contrato
            Dim primerContrato = contratos.First()
            Dim contratoEntidad = Funciones.GetContrato(primerContrato)
            Dim entorno As String = If(contratoEntidad?.Entorno = "E1", "G1", "G2")

            ' Abrir formulario de productos asignación
            Dim ventanaProductos = New ProductosAsig(entorno, contratos, connectionString, NombreUsuarioEquipo)
            ventanaProductos.Show()

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Crear las validaciones
    Private Async Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            PictureBox2.Visible = True

            Dim listas As New List(Of String) From {
            ConsultasSQL.validacionesScript1,
           ConsultasSQL.validacionesScript2,
            ConsultasSQL.validacionesScript3
        }
            Dim rutaArchivo = ObtenerRutaArchivo("Validaciones")
            Await EjecutarValidacionesAsync(listas, rutaArchivo)
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado($"Se han creado los datos en el archivo Excel en: {rutaArchivo}")
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
    Private Async Function EjecutarValidacionesAsync(listas As List(Of String), rutaArchivo As String) As Task
        Dim validaciones As New ValidacionExcel(connectionString)
        Await Task.Run(Sub() validaciones.EjecutarConsultasYGuardarEnExcel(listas, rutaArchivo))
    End Function

    Private Function ObtenerRutaArchivo(Optional nombreArchivo As String = "", Optional nombreCarpetaNueva As String = "", Optional extensionArchivo As String = "xlsx") As String
        Dim rutaFinal = rutaCarpetaGlobal

        ' Si se indica carpeta nueva, combinarla
        If Not String.IsNullOrEmpty(nombreCarpetaNueva) Then
            rutaFinal = System.IO.Path.Combine(rutaFinal, nombreCarpetaNueva)
        End If

        ' Crear carpeta si no existe
        If Not Directory.Exists(rutaFinal) Then Directory.CreateDirectory(rutaFinal)

        ' Devolver ruta de archivo o carpeta
        If Not String.IsNullOrEmpty(nombreArchivo) Then
            Dim nombreCompleto = $"{nombreArchivo}_{Date.Today:ddMMyyyy}.{extensionArchivo}"
            Return System.IO.Path.Combine(rutaFinal, nombreCompleto)
        Else
            Return rutaFinal
        End If
    End Function



    Private Function SeleccionarArchivo() As String
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Title = "Seleccionar archivo"
            openFileDialog.Multiselect = False ' Mejor procesar un archivo a la vez
            openFileDialog.Filter = "Archivos Excel (*.xlsx;*.xls)|*.xlsx;*.xls|Todos los archivos (*.*)|*.*"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Return openFileDialog.FileName
            End If
        End Using
        Return String.Empty
    End Function

    'Codigos DIR
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            If CheckBox1.Checked OrElse CheckBox3.Checked Then
                complementos.MostrarMensajePersonalizado($"Habilitado solo para el check de contratos")
                Exit Sub
            End If

            Dim Con = GetConSinSplit(TextBox2.Text)
            If Con Is Nothing OrElse Con.Count = 0 Then
                complementos.MostrarMensajePersonalizado($"Ingrese al menos un contrato")
                Exit Sub
            End If
            Using codigoDir As New CodigoDir(connectionString, Con)
                codigoDir.ShowDialog()
            End Using
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Actualizar CNAES 
    Private Async Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Try
            Dim rutaArchivo = SeleccionarArchivo()
            If String.IsNullOrEmpty(rutaArchivo) Then
                complementos.MostrarMensajePersonalizado("No se seleccionó ningún archivo.")
                Return
            End If

            PictureBox2.Visible = True
            Dim stopwatch As New Stopwatch()
            stopwatch.Start()

            Dim actualizarCNAE As New ActualizarCNAEFromExcel(connectionString) With {
            .RutaExcel = rutaArchivo
        }

            Dim contratosActualizados = Await actualizarCNAE.ActualizarCNAEFromExcelAsync()

            stopwatch.Stop()
            PictureBox2.Visible = False

            complementos.MostrarMensajePersonalizado($"Se han actualizado {contratosActualizados} contratos. Tiempo transcurrido: {stopwatch.Elapsed.TotalMinutes:F2} minutos.")
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
#Region "Renovar Contratos"


    'Volver a RenovarContratos
    Private Async Function RenovarContratosActivos(listaContratos As IEnumerable(Of Contrato)) As Task(Of Long)
        Dim totalRenovados As Long = 0
        For Each contrato In listaContratos.Where(Function(c) c.IdContratoSituacion = 1)
            If contrato IsNot Nothing AndAlso contrato.IdContrato > 0 Then
                totalRenovados = Await Task.Run(Function() Funciones.VolverARenovar(contrato.CodigoContrato))
            End If
        Next
        Return totalRenovados
    End Function

    Private Function ObtenerContratosActivosPorCUPS(cupsList As List(Of String)) As List(Of Contrato)
        Dim contratos As New List(Of Contrato)
        For Each cps In cupsList
            Dim cps20 = Replace(cps, " ", "").Substring(0, Math.Min(20, cps.Length)).Trim()
            Dim contratoActivo = Funciones.GetListContratobyCUPS(cps20).FirstOrDefault(Function(f) f.IdContratoSituacion = 1)
            If contratoActivo IsNot Nothing Then contratos.Add(contratoActivo)
        Next
        Return contratos
    End Function

    Private Function ObtenerContratosActivosPorContrato(contratosList As List(Of Long)) As List(Of Contrato)
        Return contratosList.
        Select(Function(c) Funciones.GetContrato(c)).
        Where(Function(c) c IsNot Nothing AndAlso c.IdContratoSituacion = 1).
        ToList()
    End Function

    Private Function ObtenerContratosActivosPorCliente(cifsList As List(Of String)) As List(Of Contrato)
        Dim contratos As New List(Of Contrato)
        For Each cif In cifsList
            contratos.AddRange(Funciones.GetListContratobyCIF(cif.Trim).Where(Function(c) c.IdContratoSituacion = 1))
        Next
        Return contratos
    End Function

    Private Async Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            PictureBox2.Visible = True
            Dim totalRenovados As Long = 0

            If CheckBox1.Checked Then
                Dim cups = GetConSinSplitCupsCIFS(TextBox2.Text)
                If cups.Count > 0 Then
                    totalRenovados = Await RenovarContratosActivos(ObtenerContratosActivosPorCUPS(cups))
                End If
            End If

            If CheckBox2.Checked Then
                Dim contratos = GetConSinSplit(TextBox2.Text)
                If contratos.Count > 0 Then
                    totalRenovados = Await RenovarContratosActivos(ObtenerContratosActivosPorContrato(contratos))
                End If
            End If

            If CheckBox3.Checked Then
                Dim cifs = GetConSinSplitCupsCIFS(TextBox2.Text)
                If cifs.Count > 0 Then
                    totalRenovados = Await RenovarContratosActivos(ObtenerContratosActivosPorCliente(cifs))
                End If
            End If

            PictureBox2.Visible = False

            complementos.MostrarMensajePersonalizado(
            If(totalRenovados > 0, "Contratos listos para ser renovados", "Ningún contrato renovado")
        )
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub


    'Private Async Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
    '    Try
    '        PictureBox2.Visible = True
    '        Dim RenovadoANull = 0L
    '        'Si esta por cups
    '        If CheckBox1.Checked Then
    '            Dim Cups = GetConSinSplitCupsCIFS(TextBox2.Text)
    '            If Cups.Count > 0 Then
    '                For Each cps In Cups
    '                    Dim cps20 As String = Replace(cps, " ", "").Substring(0, Math.Min(20, cps.Length)) 'saco los primeros 20 caracteres

    '                    Dim ContratoActivo = Funciones.GetListContratobyCUPS(Replace(cps20.Trim, " ", "")).Where(Function(f) If(f.IdContratoSituacion, 0L) = 1).FirstOrDefault ' Buscamos solo el activo
    '                    If Not IsNothing(ContratoActivo) AndAlso ContratoActivo.IdContrato > 0 AndAlso ContratoActivo.IdContratoSituacion = 1 Then ' solo si es activo
    '                        RenovadoANull = Await Task.Run(Function() Funciones.VolverARenovar(ContratoActivo.CodigoContrato))
    '                    End If


    '                Next
    '            End If
    '        End If
    '        ' Si esta por contrato
    '        If CheckBox2.Checked Then
    '            Dim Con = GetConSinSplit(TextBox2.Text)
    '            If Con.Count > 0 Then
    '                For Each elemnt In Con
    '                    Dim ConActivo = Funciones.GetContrato(elemnt)
    '                    If Not IsNothing(ConActivo) AndAlso ConActivo.IdContrato > 0 AndAlso ConActivo.IdContratoSituacion = 1 Then ' solo si es activo
    '                        RenovadoANull = Await Task.Run(Function() Funciones.VolverARenovar(ConActivo.CodigoContrato))
    '                    End If
    '                Next
    '            End If
    '        End If
    '        'Si esta por Cliente
    '        If CheckBox3.Checked Then
    '            Dim CIFS = GetConSinSplitCupsCIFS(TextBox2.Text)
    '            If CIFS.Count > 0 Then
    '                For Each cif In CIFS
    '                    Dim ListContratoActivo = Funciones.GetListContratobyCIF(cif.Trim).Where(Function(f) If(f.IdContratoSituacion, 0L) = 1).ToList ' Buscamos solo el activo
    '                    If Not IsNothing(ListContratoActivo) AndAlso ListContratoActivo.Count > 0 Then

    '                        For Each ContratoActivo In ListContratoActivo.Where(Function(f) f.IdContratoSituacion = 1) 'por siacaso
    '                            If Not IsNothing(ContratoActivo) AndAlso ContratoActivo.IdContrato > 0 AndAlso ContratoActivo.IdContratoSituacion = 1 Then ' solo si es activo
    '                                RenovadoANull = Await Task.Run(Function() Funciones.VolverARenovar(ContratoActivo.CodigoContrato))
    '                            End If
    '                        Next
    '                    End If


    '                Next
    '            End If
    '        End If
    '        PictureBox2.Visible = False

    '        If RenovadoANull > 0 Then
    '            complementos.MostrarMensajePersonalizado($"Contratos listos para ser renovados")
    '        Else
    '            complementos.MostrarMensajePersonalizado($"Ningún contrato renovado")
    '        End If
    '    Catch ex As Exception
    '        PictureBox2.Visible = False
    '        complementos.MostrarMensajePersonalizado(ex.Message)
    '    End Try
    'End Sub
#End Region

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


    'Actualizar emails desde Excel_ FIla  contrato 1 y fila 2 el email
    Private Async Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim contratosActualizados As Integer = 0
        Dim tiempoTranscurrido As TimeSpan = TimeSpan.Zero

        Try
            Dim rutaArchivo = SeleccionarArchivo()
            If String.IsNullOrEmpty(rutaArchivo) Then
                complementos.MostrarMensajePersonalizado("No se seleccionó ningún archivo.")
                Return
            End If

            PictureBox2.Visible = True
            Dim stopwatch As New Stopwatch()
            stopwatch.Start()

            Dim actualizarEmail As New ActualizarEmailFromExcel(connectionString) With {
            .RutaExcel = rutaArchivo
        }

            contratosActualizados = Await actualizarEmail.ActualizarEmailFromExcelAsync()

            stopwatch.Stop()
            tiempoTranscurrido = stopwatch.Elapsed
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(
            $"Se han actualizado {contratosActualizados} contratos. Tiempo transcurrido: {tiempoTranscurrido.TotalMinutes:F2} minutos."
        )
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
                        If Not IsNothing(ContratoC) AndAlso ContratoC.IdContrato > 0 Then
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

#Region "Extraer PDFS de facturas"


    'Extraer PDFs
    Private Async Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        Try
            Dim listaFacs = GetFacsSinSplit(TextBox2.Text)
            If listaFacs.Count = 0 Then
                complementos.MostrarMensajePersonalizado("No hay facturas en los filtros")
                Return
            End If

            Dim destino = ObtenerRutaArchivo(, "PDFFacturas",)

            Dim comprobadas As New List(Of String)
            PictureBox2.Visible = True

            Await Task.Run(Sub()
                               For Each elem In listaFacs
                                   GuardarPDFFactura(elem, destino, comprobadas)
                               Next
                           End Sub)

            PictureBox2.Visible = False

            If comprobadas.Count > 0 Then
                Await Task.Run(Sub()
                                   DestinoPDFunificado(listaFacs, destino)
                               End Sub)
                complementos.MostrarMensajePersonalizado("PDF descargados. Pulse Aceptar para abrir la carpeta contenedora")
                Process.Start("explorer.exe", destino)
            Else
                complementos.MostrarMensajePersonalizado("Ningún PDF se ha descargado")
            End If
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
    Public Sub DestinoPDFunificado(listaFacs As List(Of String), destino As String)
        Try
            Dim rutasPDF = listaFacs.Select(Function(f) Path.Combine(destino, GenerarNombrePDF(f))).Where(Function(r) File.Exists(r)).ToList()
            Dim rutaSalidaUnificado = Path.Combine(destino, "Facturas_Unificadas.pdf")
            UnirPDFs(rutasPDF, rutaSalidaUnificado)
        Catch ex As Exception
            Throw
        End Try
    End Sub

    ' Helper para aislar la lógica de guardado de PDF
    Private Sub GuardarPDFFactura(factura As String, destino As String, listaComprobadas As List(Of String))
        Dim bytesPDF = Funciones.ExtraerPDFFactura(factura)
        If bytesPDF Is Nothing Then Return

        listaComprobadas.Add(factura)
        Dim nombreArchivo = GenerarNombrePDF(factura)
        Dim rutaCompleta = Path.Combine(destino, nombreArchivo)
        File.WriteAllBytes(rutaCompleta, bytesPDF)
    End Sub

    ' Helper para generar nombres seguros de archivo PDF
    Private Function GenerarNombrePDF(factura As String) As String
        Dim nameFac = Replace(factura, "FELEC", "FELEC_")
        Dim originalFileName = $"{nameFac}.PDF"
        Dim nameSinExtension = Path.GetFileNameWithoutExtension(originalFileName)
        Return Mid(nameSinExtension, 1, 100) & Path.GetExtension(originalFileName)
    End Function

    Private Sub UnirPDFs(rutasPDF As List(Of String), rutaSalida As String)
        Dim outputDocument As New PdfDocument()
        For Each pdf In rutasPDF
            Dim inputDocument = PdfReader.Open(pdf, PdfDocumentOpenMode.Import)

            For i As Integer = 0 To inputDocument.PageCount - 1
                outputDocument.AddPage(inputDocument.Pages(i))
            Next
        Next

        outputDocument.Save(rutaSalida)
    End Sub



#End Region
    ' Open Items
    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        'Try

        '            ' Crear una instancia de OpenFileDialog
        '            Dim openFileDialog1 As New OpenFileDialog

        '            ' Configurar propiedades del diálogo
        '            openFileDialog1.Title = "Seleccionar archivos"
        '            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
        '            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
        '            Dim rutaArchivo = ""
        '            Dim RutaNueva = ""
        '            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
        '            If openFileDialog1.ShowDialog = DialogResult.OK Then
        '                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
        '                For Each filename In openFileDialog1.FileNames
        '                    rutaArchivo = filename
        '                Next
        '            End If

        '            'Muevo primero el archivo a una ruta local
        '            ' Definir las rutas de origen y destino
        '            Dim origen = rutaArchivo
        '            Dim destino = $"\\172.31.100.13\Total\FicherosExport\Import\OriginalItems"
        '            Dim NameFile = Path.GetFileName(origen)
        '            'Compruebo si existe el destino
        '            If Not Directory.Exists(destino) Then
        '                Directory.CreateDirectory(destino)
        '            End If

        '            ' Verificar si el archivo existe en la ruta de origen
        '            If File.Exists(origen) Then
        '                ' Nos guardamos el original en la carpeta OriginalItems
        '                File.Move(origen, destino + "\" + NameFile)
        '                RutaNueva = destino + "\" + NameFile
        '                'Console.WriteLine("Archivo movido exitosamente a: " & destino)
        '            Else
        '                'Console.WriteLine("Archivo no encontrado en la ruta de origen: " & origen)
        '            End If


        '#Region "FTP"

        '            ' Definir los parámetros de conexión
        '            'Dim servidorFTP As String = "172.31.100.13" ' Reemplazar con la dirección IP o nombre de host
        '            'Dim usuarioFTP As String = "audin\administrador"
        '            'Dim contrasenaFTP As String = "Azal3a$2020"

        '            '' Definir los archivos de origen y destino
        '            'Dim archivoOrigen As String = "archivo_origen.txt" ' Reemplazar con el nombre del archivo en el servidor FTP
        '            'Dim archivoDestino As String = "C:\Ruta\al\Archivo\Destino.txt" ' Reemplazar con la ruta completa del archivo en tu equipo local

        '            '' Establecer la conexión con el servidor FTP
        '            'Dim ftp As New System.Net.FtpClient.FtpClient()
        '            'ftp.Host = servidorFTP
        '            'ftp.Credentials = New System.Net.NetworkCredential(usuarioFTP, contrasenaFTP)
        '            '' Descargar el archivo del servidor FTP
        '            'Try
        '            '    Dim p = ftp.DirectoryExists(origen)
        '            '    Dim rr = ""
        '            'Catch ex As Exception
        '            '    Console.WriteLine("Error al descargar el archivo:", ex.Message)
        '            '    Exit Sub
        '            'End Try

        '            ' Cerrar la conexión FTP
        '            'ftp.Disconnect()
        '#End Region

        '            Dim stopwatch As New Stopwatch
        '            stopwatch.Start() ' Iniciar el cronómetro
        '            'Dim Empieza As TimeSpan = stopwatch.Elapsed
        '            'Dim ActualizarEmail As New ActualizarEmailFromExcel(connectionString)
        '            If RutaNueva.Length > 0 Then
        '                Dim OpenItms As New OpenItemsXML
        '                PictureBox2.Visible = True
        '                'Pasamos la nueva ruta
        '                Dim Open = Await Task.Run(Function() OpenItms.FormatearXML(RutaNueva))
        '                PictureBox2.Visible = False
        '                complementos.MostrarMensajePersonalizado($"Se han eliminado {Open} nodos del tipo <audinforContract/>.\nSe ha guardado en la siguiente ruta: {Path.GetDirectoryName(rutaArchivo)}")
        '            End If

        '        Catch ex As Exception
        '            PictureBox2.Visible = False
        '            complementos.MostrarMensajePersonalizado(ex.Message)
        '        End Try
    End Sub

    'Crea un CSV de las facturas que hay en el excel
    Private Async Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim tiempoTranscurrido As TimeSpan = TimeSpan.Zero
        Try
            Dim rutaArchivo = ObtenerRutaArchivo("csv", "CSVFACVA", "csv")
            Dim rutaEscogida = SeleccionarArchivo()
            If String.IsNullOrEmpty(rutaEscogida) Then
                complementos.MostrarMensajePersonalizado("Archivo no seleccionado")
                Return
            End If

            PictureBox2.Visible = True
            Dim stopwatch As New Stopwatch()
            stopwatch.Start()

            Dim validaciones As New ValidacionExcel(connectionString)
            Await Task.Run(Sub() validaciones.CSV3(rutaEscogida, rutaArchivo))

            stopwatch.Stop()
            tiempoTranscurrido = stopwatch.Elapsed

            complementos.MostrarMensajePersonalizado($"Se han creado los datos en el archivo Excel en: {rutaArchivo}. Tiempo transcurrido: {tiempoTranscurrido.TotalMinutes:F2} minutos.")
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            PictureBox2.Visible = False
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
                Label5.Text = "BD Replica 172.31.100.30 SigeTotal"
                RadioButton1.Checked = False
                RadioButton3.Checked = False
                ipDB = "data source=172.31.100.30;"
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
        Dim RutaFinal = ""
        Dim penalizacionesGeneradas As Boolean = False
        Try
            ' Obtener contratos desde la UI
            Dim contratos = GetConSinSplit(TextBox2.Text)
            If contratos.Count = 0 Then
                complementos.MostrarMensajePersonalizado("Ingrese al menos un contrato")
                Return
            End If
            'If Con.Count > 0 Then
            RutaFinal = Path.Combine(rutaCarpetaGlobal, $"Penalizaciones_{Date.Today.ToString("ddMMyyyy")}.xlsx")
            ' Verificar si la carpeta existe, y si no, crearla
            Dim Validaciones As New ValidacionExcel(connectionString, RutaFinal, contratos, Funciones, Nothing)
            PictureBox2.Visible = True
            penalizacionesGeneradas = Await Validaciones.GenerarPenalizacionesAsync()
            'Else
            '    complementos.MostrarMensajePersonalizado($"Ingrese al menos un contrato")
            'End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado($"{ex.Message}")
        Finally
            PictureBox2.Visible = False

            If penalizacionesGeneradas Then
                complementos.MostrarMensajePersonalizado($"Penalizaciones generadas en: {RutaFinal}")
            Else
                complementos.MostrarMensajePersonalizado("Penalizaciones no realizadas")
            End If
        End Try
    End Sub

    'Consulta Top
    Private Async Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        Dim PenaOk = False
        Dim RutaFinal = ""
        Try
            Dim Facs = GetConSinSplitCupsCIFS(TextBox2.Text)
            If Facs.Count = 0 Then
                complementos.MostrarMensajePersonalizado($"No hay facturas a buscar")
                Return
            End If
            RutaFinal = Path.Combine(rutaCarpetaGlobal, $"ConsultaTopLidia_{Date.Today.ToString("ddMMyyyy")}.xlsx")
            Dim Validaciones As New ValidacionExcel(connectionString, RutaFinal, Nothing, Funciones, Facs)
            PictureBox2.Visible = True
            If Facs.Count > 0 Then
                Await Task.Run(Sub() Validaciones.ConsultaTopLidia())
                PenaOk = True
            End If

        Catch ex As Exception
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
            If ListaContratos.Count = 0 Then
                complementos.MostrarMensajePersonalizado("Ingrese al menos un contrato")
                Return
            End If
            PictureBox2.Visible = True
            Dim Resultados = Await Task.Run(Function() Funciones.VerificarLicitacion(ListaContratos))
            PictureBox2.Visible = False

            If Resultados.Count > 0 Then
                Dim cod = Resultados.Select(Function(s) s.CodigoContrato).ToList
                complementos.Complementos_MostrarMensajePersonalizadoCopiar($"Los siguientes contratos tienen el check de licitacion: {String.Join(",", cod)} ", $"{String.Join(",", cod)}")
            Else
                complementos.MostrarMensajePersonalizado("No hay contratos con el check de licitacion")
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
#Region "Viejo"
    'Private Async Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
    '    Dim ExcelDatos As New Excel
    '    Dim Datos As New List(Of List(Of Object))
    '    Try
    '        Dim totalContratos = 0
    '        Dim ContratoActualizar As New List(Of Long)
    '        Dim Con As New List(Of ContratoTarifa)
    '        'Con = GetConSinSplit(TextBox2.Text)
    '        ' Crear una instancia de OpenFileDialog
    '        Dim openFileDialog1 As New OpenFileDialog

    '        ' Configurar propiedades del diálogo
    '        openFileDialog1.Title = "Seleccionar archivos"
    '        openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
    '        openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
    '        Dim rutaArchivo = ""
    '        ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
    '        If openFileDialog1.ShowDialog = DialogResult.OK Then
    '            ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
    '            For Each filename In openFileDialog1.FileNames
    '                rutaArchivo = filename
    '            Next
    '        End If
    '        ExcelPackage.LicenseContext = LicenseContext.NonCommercial
    '        Using package As New ExcelPackage(New FileInfo(rutaArchivo))
    '            Dim worksheet = package.Workbook.Worksheets(0)
    '            Dim rowCount = worksheet.Dimension.Rows

    '            ' Leer códigos de contrato del Excel
    '            Dim codigosContrato As New List(Of Long)
    '            For row = 2 To rowCount
    '                Dim CodContrato = worksheet.Cells(row, 1).Value?.ToString
    '                Dim idcontratotarifa = worksheet.Cells(row, 2).Value?.ToString
    '                Dim FechaCierre = CDate(worksheet.Cells(row, 3).Value?.ToString)
    '                Dim FechaAplicar = CDate(worksheet.Cells(row, 4).Value?.ToString)
    '                'Dim Cups As String = worksheet.Cells(row, 3).Value?.ToString()
    '                If Not String.IsNullOrEmpty(idcontratotarifa) Then
    '                    Dim conExcel As New ContratoTarifa With {.IdContratoTarifa = idcontratotarifa, .CodigoContrato = CodContrato, .FechaHasta = FechaCierre, .FechaDesde = FechaAplicar
    '                    }
    '                    Con.Add(conExcel)
    '                End If
    '            Next
    '        End Using
    '        Dim yesorNot As MsgBoxResult
    '        Dim todoOK = False
    '        'Escribo los valores que tiene ahora, para posteriormente comparar o hacer uso de este y dejarlo como esta
    '        'Funciones.EscribirContratoTarifaAntesCambios(ContratoActualizar)
    '        If Con.Count > 0 Then
    '            If Con.Count < 0 Then
    '                yesorNot = MsgBox("No hay contratos a actualizar. ¿Actualizar de todas formas?", vbYesNo)
    '            Else
    '                todoOK = True
    '            End If
    '            If yesorNot = 6 OrElse yesorNot = 1 OrElse todoOK Then
    '                PictureBox2.Visible = True
    '                Await Task.Run(Sub()
    '                                   'TextBox1 TarifagrupoViejo
    '                                   'TextBox3 TarifagrupoNuevo
    '                                   'Podemos añadir una funcación para cerrar el calendario viejo antes de añadir el nuevo, pendiente implementar
    '                                   For Each c In Con
    '                                       Dim tgNuevo = Funciones.GetCalendarioNuevoTarifa(c.IdContratoTarifa, TextBox3.Text, TextBox1.Text, c.FechaHasta) ' IdContratoTarifa (viejo), textotarifagrupo viejo, textarifagrupoNuevo, fechaCierre 
    '                                       'Dim FechaAplicar = DateTimePicker1.Value.Date
    '                                       Dim CodigoContrato = Funciones.GetOnlyCodigoContratobyIdContratoTarifa(c.IdContratoTarifa)
    '                                       If Not IsNothing(CodigoContrato) AndAlso CodigoContrato <> 0 AndAlso tgNuevo.IdTarifaGrupo Then 'Creamos el nuevo calendario y aplicamos Precios 
    '                                           Dim ok = Funciones.InsertTarifaGrupoCalendario(tgNuevo.Entorno, CodigoContrato, tgNuevo.IdTarifaGrupo, tgNuevo.IdTarifa, tgNuevo.IdPerfilFacturacion, c.FechaDesde)
    '                                           Funciones.AplicarPreciosV2(CodigoContrato, c.FechaDesde)
    '                                           Dim Pepe = 0
    '                                       End If
    '                                   Next
    '                               End Sub)
    '                PictureBox2.Visible = False

    '                If Not IsNothing(Datos) AndAlso Datos.Count > 0 Then
    '                    ExcelDatos.EscribirEnExcel($"C:\Users\{NombreUsuarioEquipo}\Desktop\", Datos, "PreciosErrores")
    '                End If
    '                complementos.MostrarMensajePersonalizado($"Contratos iniciales:{Con.Count} contratos")
    '            Else
    '                complementos.MostrarMensajePersonalizado($"Se ha cancelado la actualización")
    '            End If
    '        Else
    '            complementos.MostrarMensajePersonalizado($"Sin Contratos")
    '        End If

    '    Catch ex As Exception
    '        PictureBox2.Visible = False
    '        complementos.MostrarMensajePersonalizado("Exception: " + ex.Message)
    '    End Try
    'End Sub
#End Region
    Private Async Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        Try
            Dim rutaArchivo = SeleccionarArchivo()
            If String.IsNullOrWhiteSpace(rutaArchivo) Then Return

            Dim excel As New Excel()
            Dim contratos = excel.LeerContratosDesdeExcel(rutaArchivo)

            If contratos.Count = 0 Then
                complementos.MostrarMensajePersonalizado("Sin contratos")
                Return
            End If

            If MsgBox($"Se actualizarán {contratos.Count} contratos. ¿Continuar?", vbYesNo) <> vbYes Then
                complementos.MostrarMensajePersonalizado("Se ha cancelado la actualización")
                Return
            End If

            MostrarLoading(True)

            Await ProcesarContratosAsync(contratos)

            complementos.MostrarMensajePersonalizado($"Contratos procesados: {contratos.Count}")

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado("Exception: " & ex.Message)
        Finally
            MostrarLoading(False)
        End Try
    End Sub

    Private Async Function ProcesarContratosAsync(contratos As List(Of ContratoTarifa)) As Task

        Dim noRealizados As New ConcurrentBag(Of ContratoTarifa)
        Dim total = contratos.Count
        Dim NombreArchivo = $"ErroresCalendarioMasivo_{Now.ToString("ddMMyyyy_HHmmss")}"
        Await Task.Run(Sub()

                           Dim contador As Integer = 1
                           Dim contadorErrores As Integer = 1
                           For Each c In contratos

                               Dim tgActual = Funciones.GetContratoTarifabyCodContrato(c.CodigoContrato, c.textotarifagrupoViejo)


                               If tgActual Is Nothing OrElse tgActual.IdContratoTarifa <= 0 Then
                                   noRealizados.Add(c)
                                   contadorErrores += 1
                                   Continue For
                               End If

                               If Not CambioCalendarioValido(tgActual.FechaDesde, c.FechaDesde) Then
                                   'noRealizados.Add(c)
                                   contadorErrores += 1
                                   EscribirEnArchivo($"{c.CodigoContrato} - Nueva fecha ({c.FechaDesde:dd/MM/yyyy}) anterior a la tarifa actual ({tgActual.FechaDesde:dd/MM/yyyy})", NombreArchivo)
                                   Continue For
                               End If

                               Dim tgNuevo = Funciones.GetCalendarioNuevoTarifa(tgActual.IdContratoTarifa, c.textotarifagrupoViejo, c.textotarifagrupoNuevo, c.FechaHasta)
                               Dim codigoContrato = Funciones.GetOnlyCodigoContratobyIdContratoTarifa(tgActual.IdContratoTarifa)
                               If codigoContrato > 0 AndAlso tgNuevo IsNot Nothing AndAlso tgNuevo.IdTarifaGrupo <> 0 Then
                                   Funciones.InsertTarifaGrupoCalendario(tgNuevo.Entorno, codigoContrato, tgNuevo.IdTarifaGrupo, tgNuevo.IdTarifa, tgNuevo.IdPerfilFacturacion, c.FechaDesde)
                                   Funciones.AplicarPreciosV2(codigoContrato, c.FechaDesde)
                               End If
                               SetTextSafe(TextConsultando, $"Insertando y/o aplicando precios: {contador}/{total}. Posibles errores {contadorErrores}/{total}")
                               contador += 1
                           Next

                       End Sub)

        ' Guardar errores fuera del Task (mejor)
        For Each er In noRealizados
            EscribirEnArchivo($"{er.CodigoContrato}--{er.textotarifagrupoViejo}--{er.textotarifagrupoNuevo}", NombreArchivo)
        Next

    End Function

    Private Function CambioCalendarioValido(fechaDesdeActual As Date, nuevaFechaDesde As Date) As Boolean

        ' No puedes crear una tarifa antes de la actual
        ' porque al cerrar la actual generas intervalo inválido
        If nuevaFechaDesde < fechaDesdeActual Then
            Return False
        End If

        Return True

    End Function

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
    Private Async Sub Button23_Click(sender As Object, e As EventArgs) Handles BotonConsultar.Click
        Try
            Dim conexion = connectionString
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
            TextConsultando.Visible = True
            Dim RutaFinal = rutaCarpeta + ":"
            ' Si el CheckBox5 está marcado, crear archivo para Luz y Gas
            If CheckBox5.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Consulta_ClicksTODO" ' Nombre específico para esta consulta
                                       Dim rutaArchivoLuzGas = Path.Combine(rutaCarpeta, $"{Name}_LuzGas_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim consultaLuz = ConsultasSQL.GetClickLuz
                                       SetTextSafe(TextConsultando, $"Consultando y generando Excel {Name} Luz entre fechas {DesdeF}-{HastaF}")
                                       ExportarConsultaAExcel(conexion, consultaLuz, rutaArchivoLuzGas, "Luz")
                                       Dim consultaGas = ConsultasSQL.GetClickGas
                                       SetTextSafe(TextConsultando, $"Consultando y generando Excel {Name} Gas entre fechas {DesdeF}-{HastaF}")
                                       ExportarConsultaAExcel(conexion, consultaGas, rutaArchivoLuzGas, "Gas")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            ' Si el CheckBox6 está marcado, crear archivo para Hunosa
            If CheckBox6.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Hunosa"
                                       Dim rutaArchivoHunosa = Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim Hunosa = ConsultasSQL.GetHunosa(DesdeF, HastaF)
                                       SetTextSafe(TextConsultando, $"Consultando y generando Excel {Name} entre fechas {DesdeF}-{HastaF}")
                                       ExportarConsultaAExcel(conexion, Hunosa, rutaArchivoHunosa, "Hunosa")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            ' Si el CheckBox8 está marcado, crear archivo para Cadasa
            If CheckBox8.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Cadasa"
                                       Dim rutaArchivoCadasa = Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim Cadasa = ConsultasSQL.GetCadasa(DesdeF, HastaF)
                                       SetTextSafe(TextConsultando, $"Consultando y generando Excel {Name} entre fechas {DesdeF}-{HastaF}")
                                       ExportarConsultaAExcel(conexion, Cadasa, rutaArchivoCadasa, "Cadasa")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            ' Si el CheckBox9 está marcado, crear archivo para Quantum
            If CheckBox9.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "Quantum"
                                       Dim rutaArchivoQuantum = Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim Quantum = ConsultasSQL.GetQuantum(DesdeF, HastaF)
                                       SetTextSafe(TextConsultando, $"Consultando y generando Excel {Name} entre fechas {DesdeF}-{HastaF}")
                                       ExportarConsultaAExcel(conexion, Quantum, rutaArchivoQuantum, "Quantum")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            ' Si el CheckBox10 está marcado, crear archivo para RechazosVeolia
            If CheckBox10.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "RechazosVeolia"
                                       Dim rutaArchivoRechazosVeolia = Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim RechazosVeolia = ConsultasSQL.GetRechazosVeolia
                                       SetTextSafe(TextConsultando, $"Consultando y generando Excel {Name} entre fechas {DesdeF}-{HastaF}")
                                       ExportarConsultaAExcel(conexion, RechazosVeolia, rutaArchivoRechazosVeolia, "Veolia")
                                       RutaFinal += " " + Name
                                   End Sub))
            End If

            If CheckBox11.Checked Then
                tasks.Add(Task.Run(Sub()
                                       Dim Name = "GAM"
                                       Dim rutaArchivoGAM = Path.Combine(rutaCarpeta, $"{Name}_{Date.Today.ToString("ddMMyyyy")}.xlsx")
                                       Dim ConsultaGAM = ConsultasSQL.GetGAM(DesdeF, HastaF)
                                       SetTextSafe(TextConsultando, $"Consultando y generando Excel {Name} entre fechas {DesdeF}-{HastaF}")
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
                                           Dim rutaArchivoCurva = Path.Combine(rutaCarpeta, $"{Name}_{DateTimePicker3.Value.Date.ToString("ddMMyyyy")}_{DateTimePicker2.Value.Date.ToString("ddMMyyyy")}.xlsx")
                                           Dim listaCups As New List(Of String)
                                           For Each c In Cups
                                               listaCups.Add(Replace(c, " ", "").Substring(0, Math.Min(20, c.Length)))
                                           Next
                                           If DividirChck.Checked Then ' divide en excels
                                               For Each fCups In listaCups
                                                   Dim ConsultaCurva = ConsultasSQL.GetCurvaHoraria(DesdeF, HastaF, , fCups)
                                                   SetTextSafe(TextConsultando, $"Consultando Cups: {fCups}")
                                                   ExportarConsultaAExcel(conexionv2, ConsultaCurva, Path.Combine(rutaCarpeta, $"{Name}_{fCups}.xlsx"), fCups)
                                                   RutaFinal += " " + fCups
                                               Next

                                           Else
                                               Dim ConsultaCurva = ConsultasSQL.GetCurvaHoraria(DesdeF, HastaF, listaCups)
                                               SetTextSafe(TextConsultando, $"Consultando y generando Excel de {listaCups.Count} CUPS")
                                               ExportarConsultaAExcel(conexionv2, ConsultaCurva, rutaArchivoCurva, Name)
                                               RutaFinal += " " + Name
                                           End If
                                       End Sub))
                End If

                If CheckBox13.Checked AndAlso Cups.Count > 0 Then
                    tasks.Add(Task.Run(Sub()
                                           Dim Name = "CuartoHoraria"
                                           Dim rutaArchivoCuartoHoraria = Path.Combine(rutaCarpeta, $"{Name}_{DateTimePicker3.Value.Date.ToString("ddMMyyyy")}_{DateTimePicker2.Value.Date.ToString("ddMMyyyy")}.xlsx")
                                           Dim listaCups As New List(Of String)
                                           For Each c In Cups
                                               listaCups.Add(Replace(c, " ", "").Substring(0, Math.Min(20, c.Length)))
                                           Next
                                           If DividirChck.Checked Then ' divide en excels
                                               For Each fCups In listaCups
                                                   Dim ConsultaCurvaCuarto = ConsultasSQL.GetCurvaCuartoHoraria(DesdeF, HastaF, , fCups)
                                                   SetTextSafe(TextConsultando, $"Consultando Cups: {fCups}")
                                                   ExportarConsultaAExcel(conexionv2, ConsultaCurvaCuarto, Path.Combine(rutaCarpeta, $"{Name}_{fCups}.xlsx"), fCups)
                                                   RutaFinal += " " + fCups
                                               Next

                                           Else
                                               Dim ConsultaCurvaCuarto = ConsultasSQL.GetCurvaCuartoHoraria(DesdeF, HastaF, listaCups)
                                               SetTextSafe(TextConsultando, $"Consultando y generando Excel de {listaCups.Count} CUPS")
                                               ExportarConsultaAExcel(conexionv2, ConsultaCurvaCuarto, rutaArchivoCuartoHoraria, Name)
                                               RutaFinal += " " + Name
                                           End If

                                       End Sub))
                End If
                If CheckFacturable.Checked AndAlso Cups.Count > 0 Then
                    tasks.Add(Task.Run(Sub()
                                           Dim Name = "Facturable"
                                           Dim rutaArchivoFacturable = Path.Combine(rutaCarpeta, $"{Name}_{DateTimePicker3.Value.Date.ToString("ddMMyyyy")}_{DateTimePicker2.Value.Date.ToString("ddMMyyyy")}.xlsx")
                                           Dim listaCups As New List(Of String)
                                           For Each c In Cups
                                               listaCups.Add(Replace(c, " ", "").Substring(0, Math.Min(20, c.Length)))
                                           Next
                                           If DividirChck.Checked Then ' divide en excels
                                               For Each fCups In listaCups
                                                   Dim ConsultaFacturable = ConsultasSQL.GetCurvaFacturable(DesdeF, HastaF, , fCups)
                                                   SetTextSafe(TextConsultando, $"Consultando Cups: {fCups}")
                                                   ExportarConsultaAExcel(conexionv2, ConsultaFacturable, Path.Combine(rutaCarpeta, $"{Name}_{fCups}.xlsx"), fCups)
                                                   RutaFinal += " " + fCups
                                               Next

                                           Else
                                               Dim ConsultaFacturable = ConsultasSQL.GetCurvaFacturable(DesdeF, HastaF, listaCups)
                                               SetTextSafe(TextConsultando, $"Consultando y generando Excel de {listaCups.Count} CUPS")
                                               ExportarConsultaAExcel(conexionv2, ConsultaFacturable, rutaArchivoFacturable, Name)
                                               RutaFinal += " " + Name
                                           End If

                                       End Sub))
                End If
            End If

            If Norauto.Checked Then
                'Check Cliente
                If CheckBox3.Checked Then
                    Dim CIFS = GetConSinSplitCupsCIFS(TextBox2.Text)
                    Dim completed = 0
                    For Each cif In CIFS
                        tasks.Add(Task.Run(Sub()
                                               Dim Name = cif
                                               Dim rutaArchivoNor = Path.Combine(rutaCarpeta, $"{Name}_{Date.Today:ddMMyyyy}.xlsx")
                                               Dim ConsultaNor = ConsultasSQL.GetConsultaNorauto(DesdeF, HastaF, cif)
                                               ExportarConsultaAExcel(conexion, ConsultaNor, rutaArchivoNor, Name)
                                               Interlocked.Increment(completed)
                                               SetTextSafe(TextConsultando, $"Progreso: {completed}/{CIFS.Count} completados...")
                                           End Sub))
                    Next
                End If
            End If
            ' Esperar a que todas las tareas se completen
            Await Task.WhenAll(tasks)
            PictureBox2.Visible = False
            TextConsultando.Visible = False
            TextConsultando.Text = ""
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
    Private Async Sub BotonBuscarF(sender As Object, e As EventArgs) Handles BuscarFButton.Click
        Dim carpetaXML = "\\172.31.100.13\Total\FicherosExport\Import"
        'Dim carpetaxml As String = "C:\Users\ErickCC\Desktop\Erick\DEVOL_COPIA"
        ' Archivo donde se guardarán los resultados
        Dim archivoResultados = $"C:\Users\{NombreUsuarioEquipo}\Documents\resultados.txt"
        File.WriteAllText(archivoResultados, "") ' Limpia el archivo antes de escribir

        ' Límite de fecha (formato yyyymmdd)
        Dim stexto = TextBox2.Text
        Dim limiteFecha As Integer = stexto ' Hasta el 30 de junio de 2025

        ' --- Seleccionar Excel ---
        Dim openFileDialog1 As New OpenFileDialog With {
        .Title = "Seleccionar archivo Excel con facturas",
        .Multiselect = False,
        .Filter = "Archivos Excel (*.xlsx)|*.xlsx|Todos los archivos (*.*)|*.*"
    }

        If openFileDialog1.ShowDialog <> DialogResult.OK Then
            MessageBox.Show("No se seleccionó ningún archivo.")
            Exit Sub
        End If

        Dim rutaArchivo = openFileDialog1.FileName
        Dim facturasBuscar As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ' --- Leer facturas del Excel ---
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial
        Using package As New ExcelPackage(New FileInfo(rutaArchivo))
            Dim worksheet = package.Workbook.Worksheets(0)
            Dim rowCount = worksheet.Dimension.Rows

            For row = 2 To rowCount ' asume encabezado en fila 1
                Dim facs = worksheet.Cells(row, 1).Text.Trim
                If facs <> "" Then facturasBuscar.Add(facs)
            Next
        End Using

        ' --- Buscar coincidencias en los XML hasta la fecha límite ---
        ' --- Buscar coincidencias en los XML hasta la fecha límite ---
        Dim archivosFiltrados As New List(Of String)

        Dim fechaHoy As Integer = Date.Now.ToString("yyyyMMdd")

        ' --- Filtrar carpetas ---
        For Each subcarpeta In Directory.GetDirectories(carpetaXML)
            Dim nombreCarpeta = Path.GetFileName(subcarpeta)

            If nombreCarpeta.Length = 8 AndAlso IsNumeric(nombreCarpeta) Then
                Dim fechaCarpeta As Integer = nombreCarpeta

                ' --- Determinar rango según si la fecha límite es pasada o futura ---
                If limiteFecha >= fechaHoy Then
                    ' Búsqueda hacia el futuro
                    If fechaCarpeta >= fechaHoy AndAlso fechaCarpeta <= limiteFecha Then
                        Dim archivosDEVOL = Directory.GetFiles(subcarpeta, "DEVOL_*.xml")
                        archivosFiltrados.AddRange(archivosDEVOL)
                    End If
                Else
                    ' Búsqueda hacia atrás
                    If fechaCarpeta <= fechaHoy AndAlso fechaCarpeta >= limiteFecha Then
                        Dim archivosDEVOL = Directory.GetFiles(subcarpeta, "DEVOL_*.xml")
                        archivosFiltrados.AddRange(archivosDEVOL)
                    End If
                End If
            End If
        Next


        ' --- Analizar los XML filtrados ---
        Dim archivosEncontrados As New List(Of String)
        ' --- Procesar archivos en un hilo en segundo plano ---
        PictureBox2.Visible = True
        Await Task.Run(Sub()
                           For Each archivo In archivosFiltrados
                               Try
                                   Dim doc = XDocument.Load(archivo)
                                   Dim facturasEnXML = doc.Descendants _
                .Where(Function(x) x.Name.LocalName = "invoiceNumber") _
                .Select(Function(x) x.Value.Trim) _
                .ToHashSet(StringComparer.OrdinalIgnoreCase)

                                   Dim coincidencias = facturasBuscar.Intersect(facturasEnXML).ToList

                                   If coincidencias.Any Then
                                       ' 🔒 Bloqueo para evitar escribir simultáneamente desde varios hilos
                                       SyncLock archivoResultados
                                           archivosEncontrados.Add(archivo)
                                           File.AppendAllText(archivoResultados,
                        $"Factura(s): {String.Join(", ", coincidencias)} encontrada(s) en {archivo}{Environment.NewLine}")
                                       End SyncLock
                                   End If

                               Catch ex As Exception
                                   SyncLock archivoResultados
                                       File.AppendAllText(archivoResultados,
                    $"Error al leer {archivo}: {ex.Message}{Environment.NewLine}")
                                   End SyncLock
                               End Try
                           Next
                       End Sub)
        PictureBox2.Visible = False
        ' --- Mostrar resultados ---
        If archivosEncontrados.Any Then
            MessageBox.Show($"Facturas encontradas en {archivosEncontrados.Count} archivo(s)." & vbCrLf &
                        $"Detalles en: {archivoResultados}")
        Else
            MessageBox.Show("No se encontraron coincidencias en las carpetas hasta agosto 2025.")
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

    'Private Sub Login_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
    '    Dim rect As Rectangle = Me.ClientRectangle

    '    ' Evitar error si aún no tiene tamaño válido
    '    If rect.Width <= 0 OrElse rect.Height <= 0 Then
    '        Exit Sub
    '    End If

    '    ' Definimos los colores en RGB
    '    Dim color1 As System.Drawing.Color = System.Drawing.Color.FromArgb(160, 30, 34)
    '    Dim color2 As System.Drawing.Color = System.Drawing.Color.FromArgb(96, 109, 140)
    '    Dim color3 As System.Drawing.Color = System.Drawing.Color.FromArgb(233, 231, 226)

    '    ' Creamos el gradiente
    '    Using brush As New LinearGradientBrush(rect, color1, color3, 222.0F)
    '        Dim blend As New ColorBlend()
    '        blend.Colors = New System.Drawing.Color() {color1, color2, color3}
    '        blend.Positions = New Single() {0.0F, 0.5F, 1.0F}

    '        brush.InterpolationColors = blend
    '        e.Graphics.FillRectangle(brush, rect)
    '    End Using
    'End Sub


    Private Sub Login_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Me.Invalidate() ' Obliga a repintar con el tamaño correcto
    End Sub

    'Aplicar Precios Excel
    Private Async Sub Button26_Click(sender As Object, e As EventArgs) Handles AplicarPreciosExcelButton.Click
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
                    Dim worksheet = package.Workbook.Worksheets("Hoja1")
                    Dim rowCount = worksheet.Dimension.Rows

                    ' Leer códigos de contrato del Excel
                    Dim codigosContrato As New List(Of Long)
                    For row = 2 To rowCount
                        Dim contrat As New ContratoTarifa

                        Dim FechaContrato = worksheet.Cells(row, 1).Value?.ToString
                        Dim idContratoTarifa = worksheet.Cells(row, 2).Value?.ToString
                        Dim IdTarifagrupo = worksheet.Cells(row, 3).Value?.ToString
                        Dim CodContrato = worksheet.Cells(row, 4).Value?.ToString

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
                    TextConsultando.Visible = True
                    TextConsultando.Text = "Esperando confirmación"
                    Dim yesorNot1 = MsgBox($"Hay {Datos.Count} contratos, ¿Aplicar precios?", vbYesNo)
                    If yesorNot1 = 6 OrElse yesorNot1 = 1 Then

                        PictureBox2.Visible = True
                        Await Task.Run(Sub()
                                           For Each d In Datos
                                               Dim ContratosC = Funciones.GetContratoTarifaExcel(d)
                                               If Not ContratosC Is Nothing AndAlso ContratosC.IdContratoTarifa > 0 Then
                                                   'Dim FechaVigencia = d.FechaDesde

                                                   Try
                                                       Funciones.aplicapreciosFromEcel(d)
                                                       SetTextSafe(TextConsultando, $"Aplicando Precios: {d.CodigoContrato}")
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
            TextConsultando.Visible = False
            TextConsultando.Text = ""
            For Each er In ListaErrores
                EscribirEnArchivo(er)
            Next
            complementos.MostrarMensajePersonalizado($"Contrato actualizados")
        End Try
    End Sub

    Public Sub EscribirEnArchivo(Errores As String, Optional NombreFicheroTXT As String = "Errores")
        Try
            ' Si el archivo no existe, se creará; de lo contrario, se anexará al archivo existente
            Using writer As StreamWriter = New StreamWriter($"{rutaCarpetaGlobal}\{NombreFicheroTXT}.txt", True)
                ' Escribir los valores en el archivo de texto
                writer.WriteLine($"{Errores}")
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Async Sub Button27_Click(sender As Object, e As EventArgs) Handles Button27.Click
        Try
            Await Task.Run(Sub()
                               CopiarArchivosDEVOL()
                           End Sub)

        Catch ex As Exception

        End Try
    End Sub
    Private Sub CopiarArchivosDEVOL()
        Try
            ' Carpeta origen y destino
            Dim carpetaOrigen As String = "C:\Audinfor\Sige\Total\FicherosExport\Import"
            Dim carpetaDestino As String = "C:\Audinfor\Sige\Total\FicherosExport\Import\DEVOL_COPIA"

            ' Crear carpeta destino si no existe
            If Not Directory.Exists(carpetaDestino) Then
                Directory.CreateDirectory(carpetaDestino)
            End If

            ' Fecha límite escrita en el TextBox (ejemplo: 20250701)
            Dim stexto = TextBox2.Text.Trim()
            If Not IsNumeric(stexto) OrElse stexto.Length <> 8 Then
                MessageBox.Show("Introduce una fecha válida en formato YYYYMMDD.")
                Exit Sub
            End If

            Dim fechaInicio As Integer = CInt(stexto)
            Dim fechaHoy As Integer = CInt(Date.Now.ToString("yyyyMMdd"))
            Dim contadorCopiados As Integer = 0

            ' Recorrer las subcarpetas del origen
            For Each subcarpeta In Directory.GetDirectories(carpetaOrigen)
                Dim nombreCarpeta As String = Path.GetFileName(subcarpeta)

                ' Solo carpetas con formato yyyymmdd
                If nombreCarpeta.Length = 8 AndAlso IsNumeric(nombreCarpeta) Then
                    Dim fechaCarpeta As Integer = CInt(nombreCarpeta)

                    ' 🔁 Ahora copiamos desde la fecha indicada hasta hoy
                    If fechaCarpeta >= fechaInicio AndAlso fechaCarpeta <= fechaHoy Then
                        ' Buscar archivos que empiecen por DEVOL_ y terminen en .xml
                        Dim archivosDEVOL = Directory.GetFiles(subcarpeta, "DEVOL_*.xml")

                        ' Crear la subcarpeta en el destino (si no existe)
                        Dim carpetaDestinoFecha As String = Path.Combine(carpetaDestino, nombreCarpeta)
                        If Not Directory.Exists(carpetaDestinoFecha) Then
                            Directory.CreateDirectory(carpetaDestinoFecha)
                        End If

                        ' Copiar los archivos
                        For Each archivo In archivosDEVOL
                            Try
                                Dim nombreArchivo As String = Path.GetFileName(archivo)
                                Dim destinoFinal As String = Path.Combine(carpetaDestinoFecha, nombreArchivo)

                                File.Copy(archivo, destinoFinal, True)
                                contadorCopiados += 1
                            Catch ex As Exception
                                File.AppendAllText(
                                Path.Combine(carpetaDestino, "errores_copia.txt"),
                                $"Error copiando {archivo}: {ex.Message}{Environment.NewLine}"
                            )
                            End Try
                        Next
                    End If
                End If
            Next

            MessageBox.Show($"✅ Se copiaron {contadorCopiados} archivos DEVOL_ desde {fechaInicio} hasta {fechaHoy}.", "Proceso completado")

        Catch ex As Exception
            MessageBox.Show($"Error general: {ex.Message}", "Error")
        End Try
    End Sub

    Private Async Sub Button28_Click(sender As Object, e As EventArgs) Handles Button28.Click
        Dim carpetaXML = "C:\Audinfor\Sige\Total\FicherosExport\Import"
        'Dim carpetaxml As String = "C:\Users\ErickCC\Desktop\Erick\DEVOL_COPIA"
        ' Archivo donde se guardarán los resultados
        Dim archivoResultados = $"C:\Users\{NombreUsuarioEquipo}\Documents\resultadosi.txt"
        File.WriteAllText(archivoResultados, "") ' Limpia el archivo antes de escribir

        ' Límite de fecha (formato yyyymmdd)
        Dim stexto = TextBox2.Text
        Dim limiteFecha As Integer = stexto ' Hasta el 30 de junio de 2025

        ' --- Seleccionar Excel ---
        Dim openFileDialog1 As New OpenFileDialog With {
        .Title = "Seleccionar archivo Excel con facturas",
        .Multiselect = False,
        .Filter = "Archivos Excel (*.xlsx)|*.xlsx|Todos los archivos (*.*)|*.*"
    }

        If openFileDialog1.ShowDialog <> DialogResult.OK Then
            MessageBox.Show("No se seleccionó ningún archivo.")
            Exit Sub
        End If

        Dim rutaArchivo = openFileDialog1.FileName
        Dim facturasBuscar As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ' --- Leer facturas del Excel ---
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial
        Using package As New ExcelPackage(New FileInfo(rutaArchivo))
            Dim worksheet = package.Workbook.Worksheets(0)
            Dim rowCount = worksheet.Dimension.Rows

            For row = 2 To rowCount ' asume encabezado en fila 1
                Dim facs = worksheet.Cells(row, 1).Text.Trim
                If facs <> "" Then facturasBuscar.Add(facs)
            Next
        End Using

        ' --- Buscar coincidencias en los XML hasta la fecha límite ---
        ' --- Buscar coincidencias en los XML hasta la fecha límite ---
        Dim archivosFiltrados As New List(Of String)

        Dim fechaHoy As Integer = Date.Now.ToString("yyyyMMdd")

        ' --- Filtrar carpetas ---
        For Each subcarpeta In Directory.GetDirectories(carpetaXML)
            Dim nombreCarpeta = Path.GetFileName(subcarpeta)

            If nombreCarpeta.Length = 8 AndAlso IsNumeric(nombreCarpeta) Then
                Dim fechaCarpeta As Integer = nombreCarpeta

                ' --- Determinar rango según si la fecha límite es pasada o futura ---
                If limiteFecha >= fechaHoy Then
                    ' Búsqueda hacia el futuro
                    If fechaCarpeta >= fechaHoy AndAlso fechaCarpeta <= limiteFecha Then
                        Dim archivosDEVOL = Directory.GetFiles(subcarpeta, "ESOPEN_*.xml")
                        archivosFiltrados.AddRange(archivosDEVOL)
                    End If
                Else
                    ' Búsqueda hacia atrás
                    If fechaCarpeta <= fechaHoy AndAlso fechaCarpeta >= limiteFecha Then
                        Dim archivosDEVOL = Directory.GetFiles(subcarpeta, "ESOPEN_*.xml")
                        archivosFiltrados.AddRange(archivosDEVOL)
                    End If
                End If
            End If
        Next


        ' --- Analizar los XML filtrados ---
        Dim archivosEncontrados As New List(Of String)
        ' --- Procesar archivos en un hilo en segundo plano ---
        PictureBox2.Visible = True
        Await Task.Run(Sub()
                           For Each archivo In archivosFiltrados
                               Try
                                   Dim doc = XDocument.Load(archivo)
                                   Dim facturasEnXML = doc.Descendants _
                .Where(Function(x) x.Name.LocalName = "invoiceNumber") _
                .Select(Function(x) x.Value.Trim) _
                .ToHashSet(StringComparer.OrdinalIgnoreCase)

                                   Dim coincidencias = facturasBuscar.Intersect(facturasEnXML).ToList

                                   If coincidencias.Any Then
                                       ' 🔒 Bloqueo para evitar escribir simultáneamente desde varios hilos
                                       SyncLock archivoResultados
                                           archivosEncontrados.Add(archivo)
                                           File.AppendAllText(archivoResultados,
                        $"Factura(s): {String.Join(", ", coincidencias)} encontrada(s) en {archivo}{Environment.NewLine}")
                                       End SyncLock
                                   End If

                               Catch ex As Exception
                                   SyncLock archivoResultados
                                       File.AppendAllText(archivoResultados,
                    $"Error al leer {archivo}: {ex.Message}{Environment.NewLine}")
                                   End SyncLock
                               End Try
                           Next
                       End Sub)
        PictureBox2.Visible = False
        ' --- Mostrar resultados ---
        If archivosEncontrados.Any Then
            MessageBox.Show($"Facturas encontradas en {archivosEncontrados.Count} archivo(s)." & vbCrLf &
                        $"Detalles en: {archivoResultados}")
        Else
            MessageBox.Show($"No se encontraron coincidencias en las carpetas hasta {TextBox2.Text} .")
        End If
    End Sub

    'Renombrar XML
    Private Sub Button29_Click(sender As Object, e As EventArgs)
        Dim ruta As String = "C:\Users\ErickCC\Documents\SOPTOT-11272_XML"

        ' Obtener todos los archivos XML en la carpeta
        Dim archivos = Directory.GetFiles(ruta, "*.xml")

        Dim total As Integer = 0
        Dim renombrados As Integer = 0

        For Each archivo In archivos
            total += 1
            Try
                Dim doc As New XmlDocument()
                doc.Load(archivo)

                ' Debido al namespace de facturae, necesitamos usar NamespaceManager
                Dim nsmgr As New XmlNamespaceManager(doc.NameTable)
                nsmgr.AddNamespace("f", "http://www.facturae.es/Facturae/2014/v3.2.1/Facturae")

                ' Buscar las etiquetas dentro de cualquier InvoiceHeader
                Dim serieNode As XmlNode = doc.SelectSingleNode("//f:InvoiceSeriesCode", nsmgr)
                Dim numeroNode As XmlNode = doc.SelectSingleNode("//f:InvoiceNumber", nsmgr)

                ' Algunos XML tienen xmlns="" dentro del nodo <Invoices>,
                ' así que si no encuentra nada, probamos sin namespace
                If serieNode Is Nothing OrElse numeroNode Is Nothing Then
                    serieNode = doc.SelectSingleNode("//InvoiceSeriesCode")
                    numeroNode = doc.SelectSingleNode("//InvoiceNumber")
                End If

                If serieNode Is Nothing OrElse numeroNode Is Nothing Then
                    Console.WriteLine($"No se encontró serie o número en: {Path.GetFileName(archivo)}")
                    Continue For
                End If

                Dim serie As String = serieNode.InnerText.Trim()
                Dim numero As String = numeroNode.InnerText.Trim()

                ' Validar que ambos valores existan
                If String.IsNullOrEmpty(serie) OrElse String.IsNullOrEmpty(numero) Then
                    Console.WriteLine($"Serie o número vacío en: {Path.GetFileName(archivo)}")
                    Continue For
                End If

                ' Crear nuevo nombre
                Dim nuevoNombre As String = $"{serie}_{numero}.xml"
                Dim carpeta As String = Path.GetDirectoryName(archivo)
                Dim destino As String = Path.Combine(carpeta, nuevoNombre)

                ' Evitar sobrescribir si ya existe
                If File.Exists(destino) Then
                    Console.WriteLine($"Ya existe: {Path.GetFileName(destino)}, se omite.")
                    Continue For
                End If

                ' Renombrar
                File.Move(archivo, destino)
                Console.WriteLine($"Renombrado: {Path.GetFileName(archivo)} -> {nuevoNombre}")
                renombrados += 1

            Catch ex As Exception
                Console.WriteLine($"Error en {Path.GetFileName(archivo)}: {ex.Message}")
            End Try
        Next

        Console.WriteLine()
        Console.WriteLine($"Proceso completado. {renombrados} de {total} archivos renombrados.")
        Console.ReadLine()
    End Sub

    Private Async Sub OrganizarFacsbyCliente(sender As Object, e As EventArgs) Handles Button29.Click
        Try
            Dim listaFacs = GetFacsSinSplit(TextBox2.Text)
            If listaFacs.Count = 0 Then
                complementos.MostrarMensajePersonalizado("No hay facturas en los filtros")
                Return
            End If

            Dim DestinoBase = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO\PDFFacturas"
            If Not Directory.Exists(DestinoBase) Then Directory.CreateDirectory(DestinoBase)



            PictureBox2.Visible = True
            Await Task.Run(Sub()
                               ' Agrupar facturas por cliente
                               ' Supongo que Funciones.ObtenerCliente(fac) devuelve un objeto con Nombre e Identidad
                               Dim facturasPorCliente As New Dictionary(Of String, List(Of String))
                               For Each fac In listaFacs
                                   Dim cliente = Funciones.GetClientebyFac(fac) ' {Nombre, Identidad}
                                   Dim claveCarpeta = $"{cliente.Denominacion}-_{cliente.Identidad}"
                                   If Not facturasPorCliente.ContainsKey(claveCarpeta) Then
                                       facturasPorCliente(claveCarpeta) = New List(Of String)
                                   End If
                                   facturasPorCliente(claveCarpeta).Add(fac)
                               Next

                               For Each kvp In facturasPorCliente
                                   Dim carpetaCliente = Path.Combine(DestinoBase, kvp.Key)
                                   If Not Directory.Exists(carpetaCliente) Then Directory.CreateDirectory(carpetaCliente)

                                   For Each fac In kvp.Value
                                       Dim pdfBytes = Funciones.ExtraerPDFFactura(fac)
                                       If pdfBytes Is Nothing Then Continue For

                                       Dim NameFac = Replace(fac, "FELEC", "FELEC_")
                                       Dim originalFileName = $"{NameFac}.PDF"
                                       Dim nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName)
                                       Dim newFileName = Mid(nameWithoutExtension, 1, 100) & Path.GetExtension(originalFileName)
                                       Dim TempFileName = Path.Combine(carpetaCliente, newFileName)

                                       File.WriteAllBytes(TempFileName, pdfBytes)
                                   Next
                               Next
                           End Sub)
            PictureBox2.Visible = False

            complementos.MostrarMensajePersonalizado("PDFs descargados. Pulse Aceptar para abrir la carpeta contenedora")
            Process.Start("explorer.exe", DestinoBase)

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    ' Variable global para saber si el panel está expandido
    Dim PanelExpandido As Boolean = True
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            PanelLateral.Width = 0
            PanelExpandido = False
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try

    End Sub

    Private Sub btnExpandir_Click(sender As Object, e As EventArgs) Handles btnExpandir.Click
        TimerPanel.Start()
    End Sub

    Private Sub TimerPanel_Tick(sender As Object, e As EventArgs) Handles TimerPanel.Tick
        If PanelExpandido Then

            ' Contraer
            PanelLateral.Width -= 10
            If PanelLateral.Width <= 0 Then
                PanelExpandido = False
                TimerPanel.Stop()
            End If
            'PanelLateral.SendToBack()
        Else
            ' Expandir
            PanelLateral.BringToFront()
            PanelLateral.Width += 10
            If PanelLateral.Width >= 220 Then
                PanelExpandido = True
                TimerPanel.Stop()
            End If
        End If
    End Sub

    Private Sub Button30_Click(sender As Object, e As EventArgs) Handles Button30.Click
        Try
            ' Carpeta origen y destino
            ' --- Seleccionar Excel ---
            Dim folderDialog As New FolderBrowserDialog With {
    .Description = "Selecciona la carpeta"
}

            If folderDialog.ShowDialog <> DialogResult.OK Then
                MessageBox.Show("No se seleccionó ninguna carpeta")
                Exit Sub
            End If
            Dim carpetaOrigen As String = folderDialog.SelectedPath


            Dim stexto = TextBox2.Text.Trim()
            If Not IsNumeric(stexto) OrElse stexto.Length <> 8 Then
                complementos.MostrarMensajePersonalizado("Introduce una fecha válida en formato yyyyMMdd.")
                Exit Sub
            End If

            Dim fechaInicio As Integer = CInt(stexto)
            Dim fechaHoy As Integer = CInt(Date.Now.ToString("yyyyMMdd"))
            Dim FacturasDev As New List(Of String)
            For Each subcarpeta In Directory.GetDirectories(carpetaOrigen)
                Dim nombreCarpeta As String = Path.GetFileName(subcarpeta)

                ' Solo carpetas con formato yyyymmdd
                If nombreCarpeta.Length = 8 AndAlso IsNumeric(nombreCarpeta) Then
                    Dim fechaCarpeta As Integer = CInt(nombreCarpeta)

                    ' 🔁 Ahora copiamos desde la fecha indicada hasta hoy
                    If fechaCarpeta >= fechaInicio AndAlso fechaCarpeta <= fechaHoy Then
                        ' Buscar archivos que empiecen por DEVOL_ y terminen en .xml
                        Dim archivosDEVOL = Directory.GetFiles(subcarpeta, "DEVOL_*.xml")

                        ' Copiar los archivos
                        For Each archivo In archivosDEVOL
                            ' Cargar el XML
                            Dim doc As XDocument = XDocument.Load(archivo)

                            ' Declarar el namespace (muy importante)
                            Dim ns As XNamespace = "http://localhost/elegibilidad"

                            ' Obtener todos los invoiceNumber
                            Dim facturas = From inv In doc.Descendants(ns + "invoice")
                                           Select inv.Element(ns + "invoiceNumber")?.Value

                            For Each numeroFactura In facturas
                                FacturasDev.Add(numeroFactura)
                            Next
                        Next
                    End If
                End If
            Next



            Dim joinstringFacs = String.Join(",", FacturasDev)
            complementos.Complementos_MostrarMensajePersonalizadoCopiar($"Hay un total de {FacturasDev.Count}, pulse en COPIAR para obtener las facturas encontradas", joinstringFacs)

            'complementos.MostrarMensajePersonalizado($"✅ Se copiaron {contadorCopiados} archivos DEVOL_ desde {fechaInicio} hasta {fechaHoy}.", "Proceso completado")

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado($"Error general: {ex.Message}")
        End Try
    End Sub

    Private Async Sub Button31_Click(sender As Object, e As EventArgs)
        Try
            ' Carpeta origen (donde están los XML)
            Dim carpetaOrigen As String = "C:\Users\ErickCC\Desktop\Erick\Peajes"

            ' Carpeta destino (donde se copiarán los XML que coincidan)
            Dim carpetaDestino As String = "C:\Users\ErickCC\Desktop\Erick\PeajesFiltrados"

            ' Crear carpeta destino si no existe
            If Not Directory.Exists(carpetaDestino) Then
                Directory.CreateDirectory(carpetaDestino)
            End If

            ' Lista de CUPS a buscar
            Dim cupsBuscados As String() = {
                "ES0238330000422158AG",
                "ES0238330000166926SA"
            }
            PictureBox2.Visible = True
            Await Task.Run(Sub()
                               ' Buscar todos los archivos XML en la carpeta origen
                               Dim archivosXml = Directory.GetFiles(carpetaOrigen, "*.xml", SearchOption.TopDirectoryOnly)

                               For Each archivo In archivosXml
                                   Try
                                       ' Cargar el XML con su namespace
                                       Dim doc As XDocument = XDocument.Load(archivo)

                                       ' Definir el namespace del XML
                                       Dim ns As XNamespace = "http://localhost/sctd/B7031"

                                       ' Buscar el valor del nodo <cups>
                                       Dim cups = doc.Descendants(ns + "cups").FirstOrDefault()

                                       If cups IsNot Nothing Then
                                           ' Si el CUPS coincide con alguno de los buscados, copiamos el archivo
                                           If cupsBuscados.Contains(cups.Value.Trim()) Then
                                               Dim nombreArchivo As String = Path.GetFileName(archivo)
                                               Dim destino As String = Path.Combine(carpetaDestino, nombreArchivo)
                                               File.Copy(archivo, destino, True)
                                               Console.WriteLine($"Copiado: {nombreArchivo}")
                                           End If
                                       End If

                                   Catch ex As Exception
                                       'Console.WriteLine($"Error procesando {Path.GetFileName(archivo)}: {ex.Message}")
                                   End Try
                               Next
                           End Sub)
            PictureBox2.Visible = False
        Catch ex As Exception

        End Try
    End Sub

    Private Async Sub CheckearPerfilar_Click(sender As Object, e As EventArgs) Handles CheckearPerfilar.Click
        Try
            ' --- 1. Obtener facturas normalizadas ---
            Dim facturasAtr = ObtenerFacturasNormalizadas()
            If facturasAtr.Count = 0 Then
                complementos.MostrarMensajePersonalizado("Debes introducir al menos una factura")
                Return
            End If

            ' --- 2. Ejecutar proceso en segundo plano ---
            MostrarEstado(True, "Marcando perfilar en la lectura...")

            Dim filasAfectadas = Await Task.Run(Function()
                                                    Dim datos = ConsultasSQL.BuscarFacturaATR(facturasAtr)
                                                    Return Funciones.UpdateMarcarPerfilarLectura(datos)
                                                End Function)

            complementos.MostrarMensajePersonalizado($"Lecturas marcadas: {filasAfectadas}")

        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            MostrarEstado(False)
        End Try
    End Sub
    Private Function ObtenerFacturasNormalizadas() As List(Of String)
        Return GetFacsSinSplit(TextBox2.Text) _
            .Select(Function(f) f.Trim()) _
            .Where(Function(f) Not String.IsNullOrWhiteSpace(f)) _
            .Distinct() _
            .ToList()
    End Function

    Private Sub MostrarEstado(visible As Boolean, Optional mensaje As String = "")
        PictureBox2.Visible = visible
        TextConsultando.Visible = visible
        TextConsultando.Text = If(visible, mensaje, "")
    End Sub

    Private Sub Button26_Click_1(sender As Object, e As EventArgs)
        Try
            Procesar()
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
    Sub Procesar()
        Dim ruta As String = "\\172.31.100.13\Total\FicherosExport\Export"
        Dim contratosBuscados As New List(Of String) From {
            "5048104", "5048053", "5048102"
        }

        Dim rutaSalida As String = $"{rutaCarpetaGlobal}\resultado_contratos.txt"

        Using escritor As New StreamWriter(rutaSalida, append:=False)
            ' Obtener solo archivos: ESCONT_*.xml
            Dim archivos = Directory.GetFiles(ruta, "ESCONT_*.xml")

            For Each fichero In archivos
                Dim nombre As String = Path.GetFileName(fichero)

                ' ---  Filtrar por día 15  ---
                ' Los ESCONT suelen tener fecha dentro del nombre: ESCONT_20250115_...
                If Not nombre.Contains("16_") Then
                    Continue For
                End If

                Try
                    Dim xml As XDocument = XDocument.Load(fichero)

                    ' Namespace del XML
                    Dim ns As XNamespace = "http://localhost/elegibilidad"

                    ' Extraer todos los contratos dentro del XML
                    Dim listaContratos = xml.Descendants(ns + "Contrato")

                    For Each c In listaContratos
                        Dim codigoContrato As String = c.Element(ns + "CodigoContrato")?.Value

                        If Not String.IsNullOrEmpty(codigoContrato) Then
                            If contratosBuscados.Contains(codigoContrato) Then
                                escritor.WriteLine($"Contrato {codigoContrato} encontrado en: {nombre}")
                            End If
                        End If
                    Next

                Catch ex As Exception
                    escritor.WriteLine($"ERROR leyendo {nombre}: {ex.Message}")
                End Try

            Next
        End Using

    End Sub

    Private Sub TrocearXMLButton_Click(sender As Object, e As EventArgs) Handles TrocearXMLButton.Click
        Try
            Dim Trocear = New TrocearXMLForm()
            Trocear.show
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Private Sub CerrarForm(sender As Object, e As EventArgs) Handles Me.FormClosing
    '    MarcarUsuarioDesconectado(SesionActual.UsuarioLogueado)
    'End Sub
End Class
