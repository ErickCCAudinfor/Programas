Public Class Complementos
    Implements IComplementos
    Public Sub MostrarMensajePersonalizado(mensaje As String) Implements IComplementos.MostrarMensajePersonalizado
        Dim mensajeForm As New Form()
        mensajeForm.Text = "Gestor de Datos SIGE"
        mensajeForm.Size = New System.Drawing.Size(300, 150)
        mensajeForm.FormBorderStyle = FormBorderStyle.FixedDialog
        mensajeForm.StartPosition = FormStartPosition.CenterScreen

        ' Agregar icono a la ventana emergente
        mensajeForm.Icon = SystemIcons.Information

        Dim labelMensaje As New Label()
        labelMensaje.Text = mensaje
        labelMensaje.AutoSize = True
        labelMensaje.MaximumSize = New Size(260, 0) ' Establecer el ancho máximo para el ajuste del texto
        labelMensaje.Location = New System.Drawing.Point(20, 20)

        Dim btnOK As New Button()
        btnOK.Text = "OK"
        btnOK.DialogResult = DialogResult.OK
        btnOK.Location = New System.Drawing.Point(110, 70)
        btnOK.Size = New System.Drawing.Size(75, 23)

        mensajeForm.Controls.Add(labelMensaje)
        mensajeForm.Controls.Add(btnOK)

        mensajeForm.ShowDialog()
    End Sub

End Class
