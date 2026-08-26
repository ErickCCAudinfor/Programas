Imports System.IO
Imports PdfSharp.Pdf
Imports PdfSharp.Pdf.IO

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Une varios PDF en uno. Es el UnirPDFs de Form1, con PDFsharp igual que el original.
    ''' </summary>
    Public Module UnirPdf

        ''' <summary>
        ''' Añade las páginas de cada PDF de la lista, en orden, a un documento nuevo.
        '''
        ''' Los ficheros que no se puedan abrir se saltan en vez de tumbar la unión completa:
        ''' el original no lo hacía y un solo PDF corrupto perdía el lote entero. Los saltados
        ''' se devuelven para poder decirlo.
        ''' </summary>
        Public Function Unir(rutas As IEnumerable(Of String), rutaSalida As String) As IReadOnlyList(Of String)

            Dim saltados As New List(Of String)

            Using salida As New PdfDocument()

                For Each ruta In rutas
                    If Not File.Exists(ruta) Then
                        saltados.Add(Path.GetFileName(ruta))
                        Continue For
                    End If

                    Try
                        Using entrada = PdfReader.Open(ruta, PdfDocumentOpenMode.Import)
                            For i = 0 To entrada.PageCount - 1
                                salida.AddPage(entrada.Pages(i))
                            Next
                        End Using
                    Catch ex As Exception
                        saltados.Add(Path.GetFileName(ruta))
                    End Try
                Next

                If salida.PageCount = 0 Then
                    Throw New InvalidOperationException("ninguno de los PDF se ha podido leer")
                End If

                salida.Save(rutaSalida)

            End Using

            Return saltados

        End Function

    End Module

End Namespace
