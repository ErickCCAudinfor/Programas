Imports System.IO
Imports System.Text.Json

Public Class RegistrarBD
    Private _Empresas As List(Of EmpresaBD)
    Private _RutaConfig As String
    Private _complementos As New Complementos
    Public Sub New(Empresas As List(Of EmpresaBD), RutaConfig As String)
        InitializeComponent()
        Me._Empresas = Empresas
        Me._RutaConfig = RutaConfig
    End Sub
    Private Sub AgregarBDBoton_Click(sender As Object, e As EventArgs) Handles AgregarBDBoton.Click
        Try
            Dim BDAgregar As New EmpresaBD
            BDAgregar.Nombre = NombreBox.Text
            BDAgregar.Servidor = ServidorBox.Text
            BDAgregar.BaseDatos = BDBox.Text
            BDAgregar.Usuario = CryptoHelper.Cifrar(UsuarioBox.Text)
            BDAgregar.Password = CryptoHelper.Cifrar(ClaveBox.Text)
            BDAgregar.VPN = VPNCheck.Checked
            _Empresas.Add(BDAgregar)

            Dim json = JsonSerializer.Serialize(_Empresas, New JsonSerializerOptions With {.WriteIndented = True})
            File.WriteAllText(_RutaConfig, json)

            _complementos.MostrarMensajePersonalizado($"BD {BDAgregar.Nombre} registrada.")


        Catch ex As Exception
            Throw
        Finally
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Try
    End Sub
End Class