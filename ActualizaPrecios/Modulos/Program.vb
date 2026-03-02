

Module Program

    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Do
            Dim loginForm As New Login()

            ' Mostrar login
            If loginForm.ShowDialog() = DialogResult.OK Then

                ' Guardar usuario logueado en memoria
                'Dim usuarioLogueado = loginForm.UsuarioLogueado
                'SesionActual.UsuarioLogueado = usuarioLogueado

                ' Abrir la app principal según el tipo
                If loginForm.IsLoginReport Then
                    ' Report
                    Dim frm As New ModeloImpresionForm()
                    Application.Run(frm)
                Else
                    Dim frm As New Form1(usuarioLogueado.Nombre)
                    Application.Run(frm)
                End If

            Else

                Exit Do

            End If

        Loop
    End Sub

End Module