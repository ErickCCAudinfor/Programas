Public Class LoadingWF
    Public Sub New()
        Try
            InitializeComponent()
        Catch ex As Exception
            Throw
        End Try
    End Sub
    'Private Sub Load_Load(sender As Object, e As EventArgs)
    '    Load.Load("Loading_2.gif")
    '    Load.Location = New Point(Me.Width / 2 - Load.Width / 2, Load.Location = New Point(Me.Height / 2 - Load.Height / 2))
    'End Sub
    'Public Overloads Sub Show()
    '    Load.Show()
    'End Sub
    'Public Overloads Sub Close()
    '    Load.Hide()
    'End Sub
End Class