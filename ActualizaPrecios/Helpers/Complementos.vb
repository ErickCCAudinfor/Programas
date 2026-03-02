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
        btnOK.Location = New System.Drawing.Point(110, 80)
        btnOK.Size = New System.Drawing.Size(75, 23)

        mensajeForm.Controls.Add(labelMensaje)
        mensajeForm.Controls.Add(btnOK)

        mensajeForm.ShowDialog()
    End Sub


    Public Sub Complementos_MostrarMensajePersonalizadoCopiar(mensaje As String, TextoCopiarOpcional As String) Implements IComplementos.MostrarMensajePersonalizadoCopiar
        ' Crear el formulario personalizado
        Dim mensajeForm As New Form()
        mensajeForm.Text = "Gestor de Datos SIGE"
        mensajeForm.Size = New System.Drawing.Size(300, 180)
        mensajeForm.FormBorderStyle = FormBorderStyle.FixedDialog
        mensajeForm.StartPosition = FormStartPosition.CenterScreen
        mensajeForm.MaximizeBox = False
        mensajeForm.MinimizeBox = False

        ' Agregar icono al formulario
        mensajeForm.Icon = SystemIcons.Information

        ' Crear el Label para mostrar el mensaje
        Dim labelMensaje As New Label()
        labelMensaje.Text = mensaje
        labelMensaje.AutoSize = True
        labelMensaje.MaximumSize = New Size(260, 0) ' Ajustar el texto si es muy largo
        labelMensaje.Location = New System.Drawing.Point(20, 20)
        Dim textoSize As Size
        Using g As Graphics = mensajeForm.CreateGraphics()
            textoSize = g.MeasureString(mensaje, labelMensaje.Font, labelMensaje.MaximumSize.Width).ToSize()
        End Using
        Dim paddingHorizontal As Integer = 40
        Dim paddingVertical As Integer = 100 ' Considera espacio para botones
        mensajeForm.Size = New Size(Math.Max(textoSize.Width + paddingHorizontal, 300), textoSize.Height + paddingVertical)

        ' Crear el botón "OK"
        Dim btnOK As New Button()
        btnOK.Text = "OK"
        btnOK.DialogResult = DialogResult.OK
        btnOK.Location = New System.Drawing.Point(50, 110)
        btnOK.Size = New System.Drawing.Size(75, 23)

        ' Crear el botón "Copiar"
        Dim btnCopiar As New Button()
        btnCopiar.Text = "Copiar"
        btnCopiar.Location = New System.Drawing.Point(150, 110)
        btnCopiar.Size = New System.Drawing.Size(75, 23)

        Dim botonesY As Integer = mensajeForm.ClientSize.Height - 45 ' Posición vertical de los botones
        btnOK.Location = New Point((mensajeForm.ClientSize.Width \ 2) - btnOK.Width - 10, botonesY)
        btnCopiar.Location = New Point((mensajeForm.ClientSize.Width \ 2) + 10, botonesY)


        ' Agregar evento al botón "Copiar"
        AddHandler btnCopiar.Click, Sub()
                                        Clipboard.SetText(If(TextoCopiarOpcional.Length > 0, TextoCopiarOpcional, mensaje))
                                        MessageBox.Show(If(TextoCopiarOpcional.Length > 0, "Datos Copiados", "Mensaje copiado al portapapeles."), "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                    End Sub

        ' Agregar controles al formulario
        mensajeForm.Controls.Add(labelMensaje)
        mensajeForm.Controls.Add(btnOK)
        mensajeForm.Controls.Add(btnCopiar)

        ' Mostrar el formulario como modal
        mensajeForm.ShowDialog()
    End Sub
End Class
