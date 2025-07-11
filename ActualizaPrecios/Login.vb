Imports ClosedXML.Excel

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
                DialogResult = DialogResult.OK
                Close()
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

    'Private Sub Button2_Click(sender As Object, e As EventArgs)
    '    Try
    '        excel()
    '    Catch ex As Exception

    '    End Try
    'End Sub
    Sub excel()
        ' Ruta del archivo original y del archivo nuevo
        Dim rutaOrigen As String = "C:\Users\ErickCC\Desktop\Erick\estructura_cnae2009_v3.xlsx"
        Dim rutaDestino As String = "C:\Users\ErickCC\Desktop\Erick\SeparadoPorGrupos.xlsx"

        ' Abrir archivo origen
        Dim wbOrigen As New XLWorkbook(rutaOrigen)
        Dim wsOrigen = wbOrigen.Worksheet(1) ' Usa la primera hoja

        ' Crear archivo destino
        Dim wbDestino As New XLWorkbook()
        Dim wsDestino = wbDestino.AddWorksheet("Separado")

        ' Diccionarios para columnas y filas por grupo
        Dim dictGrupos As New Dictionary(Of String, Integer) ' Grupo -> Columna
        Dim dictFilas As New Dictionary(Of String, Integer)   ' Grupo -> Fila actual

        Dim ultimaFila As Integer = wsOrigen.LastRowUsed().RowNumber()

        For i = 2 To ultimaFila ' Asumiendo encabezado en fila 1
            Dim codCnae As String = wsOrigen.Cell(i, 1).GetString()
            Dim codIntegr As String = wsOrigen.Cell(i, 2).GetString()
            Dim titulo As String = wsOrigen.Cell(i, 3).GetString()

            ' Extraer letra del grupo desde CODINTEGR
            Dim grupo As String = codIntegr.Substring(0, 1)

            ' Crear encabezado si es nuevo grupo
            If Not dictGrupos.ContainsKey(grupo) Then
                Dim nuevaColumna As Integer = dictGrupos.Count + 1
                dictGrupos(grupo) = nuevaColumna
                dictFilas(grupo) = 2
                wsDestino.Cell(1, nuevaColumna).Value = grupo
            End If

            ' Combinar código y título
            Dim texto As String = $"''{codCnae}'"
            ' Si prefieres otro formato: Dim texto As String = titulo & " (" & codCnae & ")"

            ' Escribir texto en columna correcta
            Dim colDestino As Integer = dictGrupos(grupo)
            Dim filaDestino As Integer = dictFilas(grupo)

            wsDestino.Cell(filaDestino, colDestino).Value = texto

            ' Avanzar fila para ese grupo
            dictFilas(grupo) += 1
        Next

        wbDestino.SaveAs(rutaDestino)

        Console.WriteLine("Archivo generado correctamente en: " & rutaDestino)
    End Sub
End Class