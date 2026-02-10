Imports SigeCom.Repository
Imports System.IO
Imports System.Text.Json

Public Class RegistrarBD
    Private Empresas As List(Of EmpresaBD)
    Private RutaConfig As String
    Public Sub New(Empresas As List(Of EmpresaBD), RutaConfig As String)
        InitializeComponent()
        Me.Empresas = Empresas
        Me.RutaConfig = RutaConfig
    End Sub
    Private Sub AgregarBDBoton_Click(sender As Object, e As EventArgs) Handles AgregarBDBoton.Click
        Try
            Dim BDAgregar As New EmpresaBD
            BDAgregar.Nombre = NombreBox.Text
            BDAgregar.Servidor = ServidorBox.Text
            BDAgregar.BaseDatos = BDBox.Text
            BDAgregar.Usuario = CryptoHelper.Cifrar(UsuarioBox.Text)
            BDAgregar.Password = CryptoHelper.Cifrar(ClaveBox.Text)
            Empresas.Add(BDAgregar)

            Dim json = JsonSerializer.Serialize(Empresas, New JsonSerializerOptions With {.WriteIndented = True})
            File.WriteAllText(RutaConfig, json)

        Catch ex As Exception
            Throw
        End Try
    End Sub
End Class