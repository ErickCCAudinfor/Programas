Imports System.IO
Imports System.Text.Json
Imports SigeGestor.Core.Contratos

Namespace Contratos

    ''' <summary>
    ''' Tipos de XML que sabe trocear la aplicación, de Config\TipoXml.json junto al ejecutable.
    '''
    ''' Es el mismo fichero y el mismo formato que Json\TipoXml.json de ActualizaPrecios —una
    ''' lista de {Raiz, Nodo}— para poder copiarlo tal cual y para que añadir un tipo siga
    ''' siendo editar un JSON y no recompilar.
    '''
    ''' Si el fichero no está, se devuelven los tres tipos que traía el original en vez de una
    ''' lista vacía: con lista vacía la operación no se podría lanzar, y el caso normal es que
    ''' el fichero simplemente no se haya copiado al publicar en el .13.
    ''' </summary>
    Public Module RepositorioTiposXml

        Public Const NombreFichero As String = "TipoXml.json"

        Private ReadOnly OpcionesJson As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True,
            .ReadCommentHandling = JsonCommentHandling.Skip,
            .AllowTrailingCommas = True
        }

        ''' <summary>Los que traía TipoXml.json de ActualizaPrecios.</summary>
        Private ReadOnly PorDefecto As (Raiz As String, Nodo As String)() = {
            ("FacturasDual", "Factura"),
            ("Contratos", "Contrato"),
            ("Clientes", "Cliente")
        }

        Public ReadOnly Property Ruta As String
            Get
                Return Path.Combine(AppContext.BaseDirectory,
                                    Configuracion.RepositorioEntornos.NombreCarpeta,
                                    NombreFichero)
            End Get
        End Property

        Private Class TipoXmlJson
            Public Property Raiz As String = ""
            Public Property Nodo As String = ""
        End Class

        ''' <summary>
        ''' El Valor de cada opción es «Raiz|Nodo», que es lo que necesita el troceador. El
        ''' nombre visible es el nodo, igual que el desplegable del original.
        ''' </summary>
        Public Function Cargar() As IReadOnlyList(Of OpcionLista)

            Dim tipos As List(Of TipoXmlJson) = Nothing

            Try
                If File.Exists(Ruta) Then
                    tipos = JsonSerializer.Deserialize(Of List(Of TipoXmlJson))(
                        File.ReadAllText(Ruta), OpcionesJson)
                End If
            Catch ex As Exception
                ' JSON roto: se cae a los de siempre. No merece tumbar la operación.
                tipos = Nothing
            End Try

            Dim opciones As New List(Of OpcionLista)

            If tipos IsNot Nothing Then
                For Each t In tipos
                    If String.IsNullOrWhiteSpace(t.Raiz) OrElse String.IsNullOrWhiteSpace(t.Nodo) Then Continue For
                    opciones.Add(New OpcionLista With {
                        .Nombre = $"{t.Nodo} (raíz {t.Raiz})",
                        .Valor = $"{t.Raiz}|{t.Nodo}"
                    })
                Next
            End If

            If opciones.Count = 0 Then
                For Each t In PorDefecto
                    opciones.Add(New OpcionLista With {
                        .Nombre = $"{t.Nodo} (raíz {t.Raiz})",
                        .Valor = $"{t.Raiz}|{t.Nodo}"
                    })
                Next
            End If

            Return opciones

        End Function

    End Module

End Namespace
