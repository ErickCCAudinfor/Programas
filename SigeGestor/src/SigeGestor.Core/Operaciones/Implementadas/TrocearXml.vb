Imports System.IO
Imports System.Text
Imports System.Xml
Imports SigeGestor.Core.Configuracion

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Parte un XML grande en ficheros más pequeños, cortando por nodo completo y sin pasarse
    ''' del tamaño indicado. Es el TrocearXMLForm de ActualizaPrecios.
    '''
    ''' El algoritmo es el mismo: se lee con XmlReader en streaming —el fichero de entrada puede
    ''' tener cientos de megas y no cabe en memoria—, se copia cada nodo entero con WriteNode y
    ''' se abre fichero nuevo en cuanto el actual pasa del tamaño. Los tres espacios de nombres
    ''' del original se mantienen literales: el XML de salida tiene que validar igual.
    ''' </summary>
    Public Class TrocearXml
        Inherits OperacionUnica

        Public Const ClaveTipo As String = "tipoXml"
        Public Const ClaveEntrada As String = "ficheroEntrada"
        Public Const ClaveTamano As String = "tamanoKb"

        ' Del original, tal cual. El XML de elegibilidad los espera exactamente así.
        Private Const NsPorDefecto As String = "http://localhost/elegibilidad"
        Private Const NsXsd As String = "http://www.w3.org/2001/XMLSchema"
        Private Const NsXsi As String = "http://www.w3.org/2001/XMLSchema-instance"

        Protected Overrides Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                      avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim entrada = ctx.Campo(ClaveEntrada)
            If Not File.Exists(entrada) Then
                Return Task.FromResult(ResultadoEntrada.Fallo($"no existe el fichero '{entrada}'"))
            End If

            Dim tipo = ctx.Campo(ClaveTipo).Split("|"c)
            If tipo.Length <> 2 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("elige el tipo de XML"))
            End If
            Dim raiz = tipo(0)
            Dim nodo = tipo(1)

            Dim kb As Integer
            If Not Integer.TryParse(ctx.Campo(ClaveTamano), kb) OrElse kb <= 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("el tamaño debe ser un número de KB mayor que cero"))
            End If

            ' El original pedía la ruta de salida entera —incluido el prefijo del nombre— en una
            ' caja de texto, y si te equivocabas escribía los trozos donde no era. Aquí se pide
            ' carpeta y el prefijo sale del nombre del fichero de entrada.
            Dim carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                             RutasSalida.Asegurar("XMLTroceado"),
                             ctx.CarpetaDestino)
            Directory.CreateDirectory(carpeta)

            Dim baseSalida = Path.Combine(carpeta, Path.GetFileNameWithoutExtension(entrada))

            Dim tamanoEntrada = New FileInfo(entrada).Length
            avisar($"Troceando {Mb(tamanoEntrada)} por <{nodo}> en trozos de {kb:N0} KB…")

            Dim resultado = Trocear(entrada, baseSalida, kb, nodo, raiz, ctx,
                                    Sub(hechos, leidos)
                                        avisar($"{Redaccion.Cuenta(hechos, "fichero")} · " &
                                               $"{leidos:N0} <{nodo}> copiados")
                                    End Sub)

            If resultado.Nodos = 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos(
                    $"no se ha encontrado ningún <{nodo}> en el XML: ¿es el tipo correcto?"))
            End If

            ' La carpeta va en Salidas y no en el texto: son cientos de trozos, el nombre de
            ' cada uno no dice nada y lo que hace falta es poder abrir donde han caído.
            Return Task.FromResult(ResultadoEntrada.ConDatos(
                resultado.Nodos,
                $"{Redaccion.Cuenta(resultado.Ficheros, "fichero")} con " &
                $"{resultado.Nodos:N0} <{nodo}>") _
                .Genera(carpeta))

        End Function

        ''' <summary>
        ''' Mismo recorrido que TrocearFacturasDual del original, con dos cosas añadidas: se
        ''' atiende la cancelación y se cuentan los nodos, para poder decir si el tipo elegido
        ''' no casaba con el fichero en vez de dejar cero ficheros sin explicación.
        ''' </summary>
        Private Shared Function Trocear(rutaEntrada As String,
                                        baseSalida As String,
                                        maxKb As Integer,
                                        nombreNodo As String,
                                        elementoRaiz As String,
                                        ctx As ContextoEjecucion,
                                        informar As Action(Of Integer, Long)) As (Ficheros As Integer, Nodos As Long)

            Dim maxBytes As Long = CLng(maxKb) * 1024
            Dim indice = 0
            Dim nodos As Long = 0

            Dim ajustes As New XmlWriterSettings With {
                .Encoding = Encoding.UTF8,
                .Indent = True,
                .OmitXmlDeclaration = False
            }

            Using lector As XmlReader = XmlReader.Create(rutaEntrada)

                Dim ms As MemoryStream = Nothing
                Dim escritor As XmlWriter = Nothing

                Try
                    While lector.Read()

                        If lector.NodeType <> XmlNodeType.Element OrElse lector.LocalName <> nombreNodo Then
                            Continue While
                        End If

                        ctx.AbortarSiCancelado()

                        ' Fichero nuevo si no hay ninguno abierto o si el actual ya se pasó.
                        If escritor Is Nothing OrElse ms.Length >= maxBytes Then
                            indice += 1
                            Cerrar(escritor, ms, baseSalida, indice - 1)

                            ms = New MemoryStream()
                            escritor = XmlWriter.Create(ms, ajustes)

                            escritor.WriteStartDocument()
                            escritor.WriteStartElement(elementoRaiz, NsPorDefecto)
                            escritor.WriteAttributeString("xmlns", "xsd", Nothing, NsXsd)
                            escritor.WriteAttributeString("xmlns", "xsi", Nothing, NsXsi)

                            informar(indice, nodos)
                        End If

                        escritor.WriteNode(lector, True)
                        nodos += 1

                        If nodos Mod 500 = 0 Then informar(indice, nodos)

                    End While

                    Cerrar(escritor, ms, baseSalida, indice)
                    escritor = Nothing

                Finally
                    ' Si se cancela a media escritura hay que soltar el writer y el stream: el
                    ' original no lo hacía y dejaba el fichero a medias y el handle abierto.
                    If escritor IsNot Nothing Then escritor.Close()
                    If ms IsNot Nothing Then ms.Dispose()
                End Try

            End Using

            Return (indice, nodos)

        End Function

        ''' <summary>
        ''' Cierra el trozo en curso y lo escribe. El índice que llega es el del fichero que se
        ''' está cerrando, no el del siguiente.
        ''' </summary>
        Private Shared Sub Cerrar(escritor As XmlWriter,
                                  ms As MemoryStream,
                                  baseSalida As String,
                                  indice As Integer)

            If escritor Is Nothing OrElse indice <= 0 Then Exit Sub

            escritor.WriteEndElement()
            escritor.WriteEndDocument()
            escritor.Flush()
            escritor.Close()

            File.WriteAllBytes($"{baseSalida}_{indice}.xml", ms.ToArray())
            ms.Dispose()

        End Sub

        Private Shared Function Mb(bytes As Long) As String
            If bytes < 1024L * 1024L Then Return $"{bytes / 1024.0:N0} KB"
            Return $"{bytes / (1024.0 * 1024.0):N1} MB"
        End Function

    End Class

End Namespace
