Module Program

    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Do
            Dim loginForm As New Login()

            If loginForm.ShowDialog() = DialogResult.OK Then

                Application.Run(New Form1(loginForm.NombreUsario))
            Else
                Exit Do
            End If
        Loop
    End Sub
End Module