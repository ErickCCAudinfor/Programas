Imports System.IO

Public Class EditarModeloImpresionForm
    Private _complementos As New Complementos
    Private _Modelo As ModeloDeImpresion
    Private _esNuevo As Boolean
    Private _ConnectionString As String
    Private _Funciones As New FuncionesGenericas(_ConnectionString)
    Private _ModeloBinGlobal As Byte()
    Sub New(Modelo As ModeloDeImpresion, ConnectionString As String)

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()
        Me._Modelo = Modelo
        _ConnectionString = ConnectionString
        _Funciones = New FuncionesGenericas(_ConnectionString)
    End Sub

    Public Sub New(ConnectionString As String)
        InitializeComponent()
        _Modelo = New ModeloDeImpresion()
        _esNuevo = True
        _ConnectionString = ConnectionString
        _Funciones = New FuncionesGenericas(_ConnectionString)
    End Sub

    Private Sub EditarModeloImpresionForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not _esNuevo Then
            TextIdModelo.Text = _Modelo.IdModeloDeImpresion.ToString()
            TextDescripModelo.Text = _Modelo.DescripcionModeloDeImpresion
            TextClassName.Text = _Modelo.ClassName
            TextRptFileName.Text = _Modelo.RptFileName
            Dim bin = _Funciones.GetModeloBinario(_Modelo.IdModeloDeImpresion).Modelo
            LabelBinario.Text = bin?.Length.ToString
            _Modelo.Modelo = bin
        Else
            TextIdModelo.Text = "(nuevo)"
        End If
        InicializarComboEntorno(If(Not _esNuevo, _Modelo.Entorno, Nothing))
        InicializarComboTipoModelo(_Modelo.CodigoTipoModeloDeImpresion)
    End Sub

    Private Sub InicializarComboEntorno(Optional valorActual As String = Nothing)
        ComboEntorno.Items.Clear()
        ComboEntorno.Items.Add("Electricidad (G1)")
        ComboEntorno.Items.Add("Gas (G2)")
        If valorActual = "G1" Then
            ComboEntorno.SelectedIndex = 0
        ElseIf valorActual = "G2" Then
            ComboEntorno.SelectedIndex = 1
        Else
            ComboEntorno.SelectedIndex = -1
        End If
    End Sub


    Private Sub BotonModeloBin(sender As Object, e As EventArgs) Handles Button2.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Crystal Reports (*.rpt)|*.rpt"
            ofd.Title = "Seleccionar modelo de impresión"

            If ofd.ShowDialog() = DialogResult.OK Then
                CargarModeloDesdeArchivo(ofd.FileName)
            End If
        End Using
    End Sub
    Private Sub CargarModeloDesdeArchivo(ruta As String)

        _Modelo.Modelo = File.ReadAllBytes(ruta)
        _Modelo.RptFileName = Path.GetFileName(ruta)
        LabelBinario.Text = _Modelo.Modelo.Length.ToString
        TextRptFileName.Text = _Modelo.RptFileName
    End Sub

    Private Async Sub Guardar_Click(sender As Object, e As EventArgs) Handles Guardar.Click
        Dim numFilas As Long = 0

        Try
            PrepararModeloDesdeUI()
            MostrarEstadoGuardando(True)

            numFilas = Await GuardarModeloAsync()

            If numFilas > 0 Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End If

        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)

        Finally
            MostrarEstadoGuardando(False)
        End Try
    End Sub


    Private Sub PrepararModeloDesdeUI()
        Select Case ComboEntorno.SelectedIndex
            Case 0
                _Modelo.Entorno = "G1"
            Case 1
                _Modelo.Entorno = "G2"
            Case Else
                _Modelo.Entorno = String.Empty
        End Select
        _Modelo.DescripcionModeloDeImpresion = TextDescripModelo.Text
        _Modelo.CodigoTipoModeloDeImpresion = CInt(ComboTipoModelo.SelectedValue)
        _Modelo.ClassName = TextClassName.Text
    End Sub

    Private Function GuardarModeloAsync() As Task(Of Long)
        If _esNuevo Then
            Return Task.Run(Function() _Funciones.InsertModeloImpresion(_Modelo))
        Else
            Return Task.Run(Function() _Funciones.UpdateModeloImpresion(_Modelo))
        End If
    End Function
    Private Sub MostrarEstadoGuardando(mostrar As Boolean)
        PictureBox2.Visible = mostrar
        TextConsultando.Text = If(mostrar, "Guardando", String.Empty)
    End Sub


    Private Sub InicializarComboTipoModelo(Optional valorSeleccionado As Integer? = Nothing)

        If _ConnectionString.ToLower.Contains("sigetotal") Then
            ComboTipoModelo.DataSource = EnumHelper.EnumToComboBoxList(Of TipoModeloImpresionTotal)()
        Else
            ComboTipoModelo.DataSource = EnumHelper.EnumToComboBoxList(Of TipoModeloImpresionGeneral)()
        End If

        ComboTipoModelo.DisplayMember = "Text"
        ComboTipoModelo.ValueMember = "Value"

        'Seleccionar valor recibido
        If valorSeleccionado.HasValue Then
            ComboTipoModelo.SelectedValue = valorSeleccionado.Value
        End If

    End Sub

End Class