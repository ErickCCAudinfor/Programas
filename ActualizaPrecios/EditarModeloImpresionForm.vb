Imports System.IO

Public Class EditarModeloImpresionForm
    Private _complementos As New Complementos
    Private _Modelo As ModeloDeImpresion
    Private _esNuevo As Boolean
    Private _ConnectionString As String
    Private _Funciones As New FuncionesGenericas(_ConnectionString)
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
            TextEntorno.Text = _Modelo.Entorno
            TextDescripModelo.Text = _Modelo.DescripcionModeloDeImpresion
            TextClassName.Text = _Modelo.ClassName
            TextRptFileName.Text = _Modelo.RptFileName
            Dim bin = _Funciones.GetModeloBinario(_Modelo.IdModeloDeImpresion).Modelo
            LabelBinario.Text = bin?.Length.ToString
        Else
            TextIdModelo.Text = "(nuevo)"
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

End Class