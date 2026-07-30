Imports System.IO

''' <summary>
''' Pide al usuario una lista de valores para el segundo paso de una consulta encadenada.
''' Se usa cuando ese paso necesita identificadores que salen del Excel del primer paso.
''' </summary>
Public Class PedirDatosForm

    Private ReadOnly RutaExcelPaso1 As String
    Private ReadOnly SoloNumeros As Boolean

    ''' <summary>Valores validados que ha introducido el usuario. Vacío si ha omitido el paso.</summary>
    Public ReadOnly Property Valores As New List(Of String)

    ''' <param name="rutaExcel">Excel del primer paso, para poder abrirlo desde aquí. Puede ser Nothing.</param>
    Public Sub New(titulo As String, peticion As String, rutaExcel As String, soloNumeros As Boolean)
        InitializeComponent()
        lblTitulo.Text = titulo
        lblPeticion.Text = peticion
        Me.Text = titulo
        Me.RutaExcelPaso1 = rutaExcel
        Me.SoloNumeros = soloNumeros
    End Sub

    Private Sub PedirDatosForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        EstiloBoton(btnAceptar, Color.FromArgb(25, 118, 210), Color.FromArgb(70, 150, 230))
        EstiloBoton(btnOmitir, Color.FromArgb(120, 140, 170), Color.FromArgb(150, 170, 195))
        EstiloBoton(btnAbrirExcel, Color.FromArgb(85, 108, 138), Color.FromArgb(105, 128, 158))

        btnAbrirExcel.Visible = Not String.IsNullOrEmpty(RutaExcelPaso1) AndAlso File.Exists(RutaExcelPaso1)
        ActualizarContador()
    End Sub

    Private Sub EstiloBoton(btn As Button, fondo As Color, borde As Color)
        btn.BackColor = fondo
        btn.ForeColor = Color.White
        btn.FlatAppearance.BorderColor = borde
        btn.FlatAppearance.BorderSize = 1
    End Sub

    Private Sub txtDatos_TextChanged(sender As Object, e As EventArgs) Handles txtDatos.TextChanged
        ActualizarContador()
    End Sub

    Private Sub ActualizarContador()
        Dim cuantos = Trocear().Count
        lblContador.Text = If(cuantos = 0, "", $"{cuantos:N0} valor(es) detectado(s)")
        btnAceptar.Enabled = cuantos > 0
    End Sub

    ''' <summary>
    ''' Trocea por comas y saltos de línea. Con SoloNumeros descarta cualquier cosa que no sea
    ''' un número: además de evitar un IN (...) mal formado, impide colar SQL por el cuadro.
    ''' </summary>
    Private Function Trocear() As List(Of String)

        Dim salida As New List(Of String)
        If String.IsNullOrWhiteSpace(txtDatos.Text) Then Return salida

        Dim normalizado = txtDatos.Text.Replace(vbCrLf, ControlChars.Lf).Replace(vbCr, ControlChars.Lf)

        For Each parte In normalizado.Split({","c, ";"c, ControlChars.Lf, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
            Dim item = parte.Trim().Replace(" ", "")
            If item.Length = 0 Then Continue For

            If SoloNumeros Then
                Dim n As Long
                If Not Long.TryParse(item, n) Then Continue For
                item = n.ToString()
            End If

            If Not salida.Contains(item) Then salida.Add(item)
        Next

        Return salida

    End Function

    Private Sub btnAbrirExcel_Click(sender As Object, e As EventArgs) Handles btnAbrirExcel.Click
        Try
            Process.Start(New ProcessStartInfo(RutaExcelPaso1) With {.UseShellExecute = True})
        Catch ex As Exception
            MessageBox.Show("No se pudo abrir el fichero: " & ex.Message, "Novedades", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub btnAceptar_Click(sender As Object, e As EventArgs) Handles btnAceptar.Click

        Dim valores = Trocear()
        If valores.Count = 0 Then
            MessageBox.Show("No se ha detectado ningún valor válido.", "Segundo paso", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Me.Valores.Clear()
        Me.Valores.AddRange(valores)
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub btnOmitir_Click(sender As Object, e As EventArgs) Handles btnOmitir.Click
        Me.Valores.Clear()
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
