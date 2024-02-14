Public Class CodigoDir

    Public ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    'Cambiar automaticamente el valor del resto de codigos dir
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Try
            TextBox2.Text = TextBox1.Text
            TextBox3.Text = TextBox2.Text
        Catch ex As Exception

        End Try
    End Sub
End Class