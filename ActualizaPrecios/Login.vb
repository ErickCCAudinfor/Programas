Public Class Login
    Dim complementos As New Complementos()
    ReadOnly Usuario As String = "SIGE"
    ReadOnly Clave As String = "SIGE2025"

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Application.Exit()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If TextBox1.Text.Equals(Usuario) AndAlso TextBox2.Text.Equals(Clave) Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                complementos.MostrarMensajePersonalizado("Credenciales incorrectas")
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
End Class