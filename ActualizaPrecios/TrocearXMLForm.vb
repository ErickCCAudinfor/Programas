Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports System.Xml
Imports SigeCom.Repository

Public Class TrocearXMLForm
    Dim Complementos As New Complementos
    Private TipoXML As List(Of TipoXML)
    Private RutaConfig As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json", "TipoXML.json")
    Public Sub New()

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()
        CargarTipos()
        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().

    End Sub

    Private Async Sub BotonTrocear_Click(sender As Object, e As EventArgs) Handles BotonTrocear.Click
        Try
            Dim TipoXMLSeleccionado As TipoXML = CType(ComboTipoXml.SelectedItem, TipoXML)
            If TextRuta.Text.Length = 0 And TextSalida.Text.Length = 0 Then Return
            CargarImagen.Visible = True
            Await Task.Run(Sub() TrocearFacturasDual(TextRuta.Text, TextSalida.Text, CInt(TextoTamaño.Text), TipoXMLSeleccionado.Nodo, TipoXMLSeleccionado.Raiz))
            CargarImagen.Visible = False
        Catch ex As Exception
            Complementos.MostrarMensajePersonalizado(ex.Message)
            CargarImagen.Visible = False
        End Try
    End Sub
    Private Sub BotonTrocear_Changed(sender As Object, e As EventArgs) Handles ComboTipoXml.TextChanged
        Try
            Dim TipoXMLSeleccionado As TipoXML = CType(ComboTipoXml.SelectedItem, TipoXML)
            LabelTipoNodo.Text = TipoXMLSeleccionado.Raiz
        Catch ex As Exception

        End Try
    End Sub
    Sub TrocearFacturasDual(rutaEntrada As String, rutaSalidaBase As String, maxKB As Integer, LocalName As String, StartElement As String)

        Dim maxBytes As Integer = maxKB * 1024
        Dim indice As Integer = 0

        Dim nsDefault As String = "http://localhost/elegibilidad"
        Dim nsXsd As String = "http://www.w3.org/2001/XMLSchema"
        Dim nsXsi As String = "http://www.w3.org/2001/XMLSchema-instance"

        Dim settings As New XmlWriterSettings With {.Encoding = Encoding.UTF8, .Indent = True, .OmitXmlDeclaration = False}

        Using reader As XmlReader = XmlReader.Create(rutaEntrada)

            Dim ms As MemoryStream = Nothing
            Dim writer As XmlWriter = Nothing

            While reader.Read()

                If reader.NodeType = XmlNodeType.Element AndAlso reader.LocalName = LocalName Then

                    ' Nuevo fichero si no existe o se supera tamaño
                    If writer Is Nothing OrElse ms.Length >= maxBytes Then
                        Cerrar(writer, ms, rutaSalidaBase, indice)

                        ms = New MemoryStream()
                        writer = XmlWriter.Create(ms, settings)

                        writer.WriteStartDocument()

                        writer.WriteStartElement(StartElement, nsDefault)
                        writer.WriteAttributeString("xmlns", "xsd", Nothing, nsXsd)
                        writer.WriteAttributeString("xmlns", "xsi", Nothing, nsXsi)

                        indice += 1
                    End If

                    ' Copia completa de la factura
                    writer.WriteNode(reader, True)
                End If
            End While

            ' Cerrar último fichero
            Cerrar(writer, ms, rutaSalidaBase, indice)
        End Using
    End Sub
    Private Sub Cerrar(
        writer As XmlWriter,
        ms As MemoryStream,
        rutaSalidaBase As String,
        indice As Integer)

        If writer Is Nothing Then Exit Sub

        writer.WriteEndElement()   ' </FacturasDual>
        writer.WriteEndDocument()
        writer.Flush()
        writer.Close()

        File.WriteAllBytes($"{rutaSalidaBase}_{indice}.xml", ms.ToArray())
    End Sub

    Private Sub CargarTipos()

        If Not File.Exists(RutaConfig) Then
            File.WriteAllText(RutaConfig, "[]")
        End If
        Dim json = File.ReadAllText(RutaConfig)

        TipoXML = JsonSerializer.Deserialize(Of List(Of TipoXML))(json,
        New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True})
        ComboTipoXml.DataSource = Nothing
        ComboTipoXml.DataSource = TipoXML
        ComboTipoXml.DisplayMember = "Nodo"
    End Sub

End Class