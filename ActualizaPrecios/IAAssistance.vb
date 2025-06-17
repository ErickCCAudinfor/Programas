Public Class IAAssistance

    Public Sub New()

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().

    End Sub

    Private Async Sub btnPreguntar_Click_1(sender As Object, e As EventArgs) Handles btnPreguntar.Click
        Dim ia As New IAChatAssistant()

        ' Validación rápida
        If String.IsNullOrWhiteSpace(txtPregunta.Text) Then
            MessageBox.Show("Por favor, escribe una pregunta.")
            Exit Sub
        End If

        ' Desactivar botón mientras responde
        btnPreguntar.Enabled = False
        txtRespuesta.Text = "Pensando..."

        Try
            Dim respuesta As String = ""

#If DEBUG Then
            respuesta = Await ia.PreguntarAsync(txtPregunta.Text, "C:\Users\ErickCC\Documents\TotalDoc\Programas\ActualizaPrecios\IAText\MANUAL_JADE_INTEGRACION_IA.docx")
#Else
        
        respuesta = Await ia.PreguntarAsync(txtPregunta.Text, "\\172.31.100.13\c$\AppConsultasSIGE\IAJADE\MANUAL_JADE_INTEGRACION_IA.docx")
#End If

            txtRespuesta.Text = respuesta
        Catch ex As Exception
            txtRespuesta.Text = "Ocurrió un error: " & ex.Message
        Finally
            btnPreguntar.Enabled = True
        End Try
    End Sub
End Class