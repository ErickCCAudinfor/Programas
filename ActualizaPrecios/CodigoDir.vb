Public Class CodigoDir

    Public ReadOnly Property connectionString As String
    Public ReadOnly Property CodContratos As List(Of Long)
    Public Sub New(connectionString As String, CodContratos As List(Of Long))
        Try
            InitializeComponent()
            Me.connectionString = connectionString
            Me.CodContratos = CodContratos
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim funciones As New FuncionesGenericas(connectionString)
            If TextBox1.Text.Trim.Length > 0 AndAlso TextBox1.Text.Trim.Length > 0 AndAlso TextBox1.Text.Trim.Length > 0 Then
                If funciones.UpdateCodigosDir(TextBox1.Text, TextBox2.Text, TextBox3.Text, CodContratos) > 0 Then
                    MessageBox.Show("Contratos actualizados")
                End If

            Else
                MessageBox.Show("Ingrese los códigos en los 3 campos")
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    'Cambiar automaticamente el valor del resto de codigos dir
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Try
            TextBox2.Text = TextBox1.Text
            TextBox3.Text = TextBox2.Text
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

End Class