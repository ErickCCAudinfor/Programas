Module Program

    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Do
            Dim loginForm As New Login()

            If loginForm.ShowDialog() = DialogResult.OK Then
                If loginForm.IsLoginReport Then
                    Application.Run(New ModeloImpresionForm())
                Else
                    Application.Run(New Form1(loginForm.NombreUsario))
                End If

            Else
                Exit Do
            End If
        Loop
    End Sub
End Module