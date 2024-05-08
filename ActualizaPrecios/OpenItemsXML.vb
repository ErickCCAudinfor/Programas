Imports System.IO
Imports System.Xml

Public Class OpenItemsXML
    Sub New()

    End Sub
    Public Function FormatearXML(Ruta As String) As Long
        Dim Eliminados As Long = 0
        Try

            Dim archivoXML As String = Path.GetFileName(Ruta)
            Dim Directorio As String = Path.GetDirectoryName(Ruta)

            Dim Destino2 = $"{Directorio}\ParaImportar"
            If Not Directory.Exists(Destino2) Then
                Directory.CreateDirectory(Destino2)
            End If
            Destino2 = Path.Combine(Destino2, archivoXML)
            File.Copy(Ruta, Destino2, True)
            Eliminados = ProcesarInvoiceNodes(Destino2)
        Catch ex As Exception
            Throw
        End Try
        Return Eliminados
    End Function

    Private Function ProcesarInvoiceNodes(Ruta As String) As Long
        Dim eliminados As Integer = 0
        Try

            Dim xmlDoc As New XmlDocument()
            xmlDoc.Load(Ruta)
            ' Obtener el espacio de nombres (namespace) del XML
            Dim xmlnsManager As New XmlNamespaceManager(xmlDoc.NameTable)
            xmlnsManager.AddNamespace("ns", "http://localhost/elegibilidad")
            ' Obtener todos los elementos <invoice> en el XML
            Dim invoiceNodes As XmlNodeList = xmlDoc.SelectNodes("//ns:invoice", xmlnsManager)
            ' Recorrer todos los elementos <invoice> y verificar <audinforContract>

            For Each invoiceNode As XmlNode In invoiceNodes
                Dim inner = invoiceNode.InnerXml
                ' Verificar si se encuentra la etiqueta audinforcontract
                If inner.Contains($"<audinforContract xmlns=""http://localhost/elegibilidad""/>") OrElse inner.Contains($"<audinforContract xmlns=""http://localhost/elegibilidad"" />") Then
                    invoiceNode.ParentNode.RemoveChild(invoiceNode)
                    eliminados += 1
                End If

                Application.DoEvents()

            Next
            xmlnsManager.RemoveNamespace("ns", "http://localhost/elegibilidad")
            ' Guardar el XML resultante de nuevo en el archivo
            xmlDoc.Save(Ruta)

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        Return eliminados
    End Function




End Class
