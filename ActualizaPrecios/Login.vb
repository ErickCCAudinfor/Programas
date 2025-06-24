Public Class Login
    Dim complementos As New Complementos()
    ReadOnly Usuario As String = "SIGE"
    ReadOnly Clave As String = "SIGE2025"
    Private originalSize As Size
    Private originalLocation As Point

    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        originalSize = ExitPicture.Size
        originalLocation = ExitPicture.Location
        PasswordBox.PasswordChar = "*"c
    End Sub

    Private Sub PictureBox1_MouseEnter(sender As Object, e As EventArgs) Handles ExitPicture.MouseEnter
        ExitPicture.Size = New Size(originalSize.Width + 6, originalSize.Height + 6)
        ExitPicture.Location = New Point(originalLocation.X - 3, originalLocation.Y - 3)
    End Sub

    Private Sub PictureBox1_MouseLeave(sender As Object, e As EventArgs) Handles ExitPicture.MouseLeave
        ExitPicture.Size = originalSize
        ExitPicture.Location = originalLocation
    End Sub



    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles ExitPicture.Click
        Application.Exit()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If UsuarioBox.Text.Length = 0 And PasswordBox.Text.Length > 0 Then
                complementos.MostrarMensajePersonalizado("Falta el usuario...")
                Exit Sub
            End If
            If PasswordBox.Text.Length = 0 And UsuarioBox.Text.Length > 0 Then
                complementos.MostrarMensajePersonalizado("Falta la clave...")
                Exit Sub
            End If
            If PasswordBox.Text.Length = 0 AndAlso PasswordBox.Text.Length = 0 Then
                complementos.MostrarMensajePersonalizado("¿Y los datos?")
                Exit Sub
            End If
            If UsuarioBox.Text.Equals(Usuario) AndAlso PasswordBox.Text.Equals(Clave) Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                complementos.MostrarMensajePersonalizado("Credenciales incorrectas")
                Exit Sub
            End If
        Catch ex As Exception
            complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    Private Sub LabelGestorDatosSIGE_Click(sender As Object, e As EventArgs) Handles LabelGestorDatosSIGE.Click

    End Sub

End Class