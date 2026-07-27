Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Text.Json
Imports ClosedXML.Excel
Imports SigeCom.Repository

Public Class Login
    Dim complementos As New Complementos()
    Private originalSize As Size
    Private originalLocation As Point
    Public NombreUsario As String
    Public IsLoginReport As Boolean = False
    Public UsuarioLogueado As UsuarioValidacion
    Private Sub Login_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        originalSize = ExitPicture.Size
        originalLocation = ExitPicture.Location
        PasswordBox.PasswordChar = "*"c
        AplicarTema()
    End Sub

    Private Sub AplicarTema()
        ' Versión publicada: siempre desde AppInfo, nunca un literal en el diseñador
        Label1.Text = AppInfo.VersionEtiqueta

        ' Card interior: blanco puro → azul-blanco suave
        pnlContenido.BackColor = Color.FromArgb(248, 251, 255)

        ' Separadores visibles (eran blancos sobre blanco)
        Panel1.BackColor = Color.FromArgb(195, 215, 245)
        Panel2.BackColor = Color.FromArgb(195, 215, 245)

        ' Inputs: fondo igual al card
        UsuarioBox.BackColor = Color.FromArgb(248, 251, 255)
        PasswordBox.BackColor = Color.FromArgb(248, 251, 255)

        ' Labels de campo: Segoe UI Semibold navy
        Dim fuenteLbl As New Font("Segoe UI Semibold", 8.5F, FontStyle.Bold)
        Dim colorNavy As Color = Color.FromArgb(20, 55, 110)
        UsuarioLbl.Font = fuenteLbl
        UsuarioLbl.ForeColor = colorNavy
        Clavelbl.Font = fuenteLbl
        Clavelbl.ForeColor = colorNavy

        ' Copyright: tono suave
        Dim colorCopy As Color = Color.FromArgb(110, 135, 175)
        Label4.ForeColor = colorCopy
        Label5.ForeColor = colorCopy
    End Sub

#Region "eventos"


    Private Sub PictureBox1_MouseEnter(sender As Object, e As EventArgs) Handles ExitPicture.MouseEnter
        ExitPicture.Size = New Size(originalSize.Width + 6, originalSize.Height + 6)
        ExitPicture.Location = New Point(originalLocation.X - 3, originalLocation.Y - 3)
    End Sub

    Private Sub PictureBox1_MouseLeave(sender As Object, e As EventArgs) Handles ExitPicture.MouseLeave
        ExitPicture.Size = originalSize
        ExitPicture.Location = originalLocation
    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles ExitPicture.Click
        If SesionActual.UsuarioLogueado IsNot Nothing Then
            MarcarUsuarioDesconectado(SesionActual.UsuarioLogueado)
        End If
        Application.Exit()

    End Sub
#End Region
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If Not ValidarCampos() Then Exit Sub

            TextoValidar.Text = "Validando credenciales..."
            Button1.Enabled = False

            Dim funciones As New FuncionesGenericas("data source=172.31.100.12; initial catalog=SigeTotal;User ID=Sige;Password=SigeNew;")

            Dim userBD = Await Task.Run(Function()
                                            Return funciones.UsuarioValidacion(
                                        UsuarioBox.Text.Trim(),
                                        PasswordBox.Text,
                                        IsLoginReport
                                    )
                                        End Function)

            If userBD Is Nothing Then
                TextoValidar.Text = "Credenciales incorrectas"
                Return
            End If

            ' Guardar usuario en memoria
            userBD.logeado = True
            SesionActual.UsuarioLogueado = userBD

            ' Marcar logeado = True en el JSON

            RegistraroActualizarUsuario(userBD)

            ' Retornar OK y cerrar login
            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            TextoValidar.Text = ""
            complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            Button1.Enabled = True
        End Try
    End Sub
    Private Sub RegistraroActualizarUsuario(usuario As UsuarioValidacion)

        Try
            Dim jsonusuarios As List(Of UsuarioValidacion)

            ' Si el archivo no existe → lista nueva
            If Not File.Exists(RutaConfigUsuarios) Then
                jsonusuarios = New List(Of UsuarioValidacion)
            Else
                Dim json = File.ReadAllText(RutaConfigUsuarios)

                jsonusuarios = JsonSerializer.Deserialize(Of List(Of UsuarioValidacion))(json, New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True})

                If jsonusuarios Is Nothing Then
                    jsonusuarios = New List(Of UsuarioValidacion)
                End If
            End If

            ' Buscar usuario existente
            Dim usuarioExistente = jsonusuarios.FirstOrDefault(Function(u) u.Nombre = usuario.Nombre AndAlso u.login = usuario.login)

            If usuarioExistente IsNot Nothing Then
                ' Actualizar
                usuarioExistente.Servidor = Environment.MachineName
                usuarioExistente.logeado = True
                usuarioExistente.Password = ""
                'usuarioExistente.Password = usuario.Password
            Else
                ' Insertar
                usuario.Servidor = Environment.MachineName
                usuario.logeado = True
                usuario.Password = ""
                jsonusuarios.Add(usuario)
            End If

            ' Guardar
            Dim jsonFinal = JsonSerializer.Serialize(jsonusuarios, New JsonSerializerOptions With {.WriteIndented = True})

            File.WriteAllText(RutaConfigUsuarios, jsonFinal)
        Catch ex As Exception
            Throw
        Finally
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End Try

    End Sub

#Region "validaciones"
    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(UsuarioBox.Text) AndAlso
       String.IsNullOrWhiteSpace(PasswordBox.Text) Then

            TextoValidar.Text = "¿Y los datos?"
            Return False

        ElseIf String.IsNullOrWhiteSpace(UsuarioBox.Text) Then
            TextoValidar.Text = "Falta el usuario..."
            Return False

        ElseIf String.IsNullOrWhiteSpace(PasswordBox.Text) Then
            TextoValidar.Text = "Falta la clave..."
            Return False
        End If

        TextoValidar.Text = ""
        Return True
    End Function
#End Region

    Sub excel()
        ' Ruta del archivo original y del archivo nuevo
        Dim rutaOrigen As String = "C:  \Users\ErickCC\Desktop\Erick\estructura_cnae2009_v3.xlsx"
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