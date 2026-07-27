''' <summary>
''' Muestra el historial de novedades de la aplicación. Las versiones posteriores a la
''' última que leyó el usuario se marcan como "Nuevo".
''' </summary>
Public Class NovedadesForm

    Private ReadOnly ColorNavy As Color = Color.FromArgb(25, 65, 120)
    Private ReadOnly ColorTexto As Color = Color.FromArgb(55, 65, 81)
    Private ReadOnly ColorSuave As Color = Color.FromArgb(110, 135, 175)
    Private ReadOnly ColorAcento As Color = Color.FromArgb(245, 158, 11)

    Private ReadOnly VersionLeidaPrevia As String

    ''' <param name="versionLeidaPrevia">
    ''' Última versión que el usuario había leído antes de abrir esta ventana. Se usa
    ''' solo para decidir qué entradas llevan la etiqueta "Nuevo".
    ''' </param>
    Public Sub New(versionLeidaPrevia As String)
        InitializeComponent()
        Me.VersionLeidaPrevia = If(versionLeidaPrevia, "")
    End Sub

    Private Sub NovedadesForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblSubtitulo.Text = "Cambios y mejoras del " & AppInfo.Nombre

        btnCerrar.BackColor = Color.FromArgb(25, 118, 210)
        btnCerrar.ForeColor = Color.White
        btnCerrar.FlatAppearance.BorderColor = Color.FromArgb(70, 150, 230)
        btnCerrar.FlatAppearance.BorderSize = 1

        ConstruirListado()
    End Sub

    Private Sub btnCerrar_Click(sender As Object, e As EventArgs) Handles btnCerrar.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    ''' <summary>
    ''' Pinta el historial de arriba abajo acumulando la Y de cada bloque.
    ''' </summary>
    Private Sub ConstruirListado()

        pnlLista.SuspendLayout()
        pnlLista.Controls.Clear()

        ' Se reserva hueco para la barra de desplazamiento vertical.
        Dim izquierda As Integer = pnlLista.Padding.Left
        Dim anchoUtil As Integer = pnlLista.ClientSize.Width - pnlLista.Padding.Left - pnlLista.Padding.Right - SystemInformation.VerticalScrollBarWidth
        Dim y As Integer = pnlLista.Padding.Top

        If NovedadesApp.Historial Is Nothing OrElse NovedadesApp.Historial.Count = 0 Then
            pnlLista.Controls.Add(CrearTexto("Todavía no hay novedades registradas.", izquierda, y, anchoUtil, 9.0F, FontStyle.Italic, ColorSuave))
            pnlLista.ResumeLayout()
            Return
        End If

        Dim indiceLeida As Integer = IndiceVersionLeida()

        For i As Integer = 0 To NovedadesApp.Historial.Count - 1

            Dim novedad = NovedadesApp.Historial(i)
            Dim esNueva As Boolean = (i < indiceLeida)

            ' --- Cabecera: versión + fecha ---
            Dim lblVersion = CrearTexto($"Versión {novedad.Version}   ·   {novedad.Fecha:dd/MM/yyyy}", izquierda, y, anchoUtil, 10.0F, FontStyle.Bold, ColorNavy)
            pnlLista.Controls.Add(lblVersion)

            If esNueva Then
                Dim lblNuevo As New Label With {
                    .Text = "NUEVO",
                    .AutoSize = False,
                    .Size = New Size(52, 18),
                    .Location = New Point(izquierda + lblVersion.Width + 10, y + 1),
                    .Font = New Font("Segoe UI Semibold", 7.5F, FontStyle.Bold),
                    .ForeColor = Color.White,
                    .BackColor = ColorAcento,
                    .TextAlign = ContentAlignment.MiddleCenter
                }
                pnlLista.Controls.Add(lblNuevo)
            End If

            y += lblVersion.Height + 2

            ' --- Título del bloque ---
            If Not String.IsNullOrWhiteSpace(novedad.Titulo) Then
                Dim lblTit = CrearTexto(novedad.Titulo, izquierda, y, anchoUtil, 9.0F, FontStyle.Bold, ColorTexto)
                pnlLista.Controls.Add(lblTit)
                y += lblTit.Height + 4
            End If

            ' --- Cambios ---
            If novedad.Cambios IsNot Nothing Then
                For Each cambio In novedad.Cambios
                    Dim lblCambio = CrearTexto("•  " & cambio, izquierda + 8, y, anchoUtil - 8, 9.0F, FontStyle.Regular, ColorTexto)
                    pnlLista.Controls.Add(lblCambio)
                    y += lblCambio.Height + 3
                Next
            End If

            ' --- Separador entre versiones ---
            If i < NovedadesApp.Historial.Count - 1 Then
                y += 8
                Dim linea As New Panel With {
                    .BackColor = Color.FromArgb(218, 228, 242),
                    .Location = New Point(izquierda, y),
                    .Size = New Size(anchoUtil, 1)
                }
                pnlLista.Controls.Add(linea)
                y += 13
            End If

        Next

        pnlLista.ResumeLayout()

    End Sub

    ''' <summary>
    ''' Posición en el historial de la última versión leída. Las entradas anteriores a
    ''' ese índice son las que el usuario no ha visto. Si no la encuentra (primera vez o
    ''' versión retirada del historial), todo se considera nuevo.
    ''' </summary>
    Private Function IndiceVersionLeida() As Integer

        If String.IsNullOrEmpty(VersionLeidaPrevia) Then Return NovedadesApp.Historial.Count

        Dim indice = NovedadesApp.Historial.FindIndex(Function(n) String.Equals(n.Version, VersionLeidaPrevia, StringComparison.OrdinalIgnoreCase))

        Return If(indice < 0, NovedadesApp.Historial.Count, indice)

    End Function

    Private Function CrearTexto(texto As String, x As Integer, y As Integer, anchoMaximo As Integer, tamano As Single, estilo As FontStyle, color As Color) As Label

        Dim nombreFuente = If(estilo = FontStyle.Bold, "Segoe UI Semibold", "Segoe UI")

        Return New Label With {
            .Text = texto,
            .AutoSize = True,
            .MaximumSize = New Size(anchoMaximo, 0),
            .Location = New Point(x, y),
            .Font = New Font(nombreFuente, tamano, estilo),
            .ForeColor = color,
            .BackColor = Color.Transparent
        }

    End Function

End Class
