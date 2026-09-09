Option Strict Off   ' Usa FuncionesGenericas, portada sin Option Strict.

Imports System.IO
Imports ClosedXML.Excel
Imports SigeGestor.Core.Configuracion

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Baja a disco los documentos cuyos Id vienen en un Excel. Es el Button24
    ''' («Extraer Documentos Generales») de ActualizaPrecios.
    '''
    ''' OJO CON DE DÓNDE VIENEN LOS DOCUMENTOS: no de la base de datos, aunque lo parezca.
    ''' DocumentDataFromCopiaAnioSiNulo llama a una API HTTP —URL_API_DOCUMENTOS, hoy
    ''' http://172.31.100.31:8045/documentos/— con un token que saca de SysMainControl. Eso
    ''' significa dos cosas que conviene tener presentes:
    '''
    '''   · La URL está fija en el código de FuncionesGenericas, no en Entornos.json. Si la API
    '''     se muda de máquina hay que recompilar.
    '''   · Los documentos NO dependen del entorno elegido. Se pida contra Producción, Replica o
    '''     UAT, la API es la misma. El entorno solo se usa para el token.
    '''
    ''' UNA ENTRADA POR DOCUMENTO, al contrario que el original, que los recorría en un bucle
    ''' dentro de un Task.Run y no decía nada de los que no encontraba: si de 200 Id la API
    ''' devolvía 40, los otros 160 se saltaban en silencio con un Continue For. Aquí cada Id es
    ''' una entrada con su propio resultado, así que se ve cuáles han venido y cuáles no.
    ''' </summary>
    Public Class ExtraerDocumentosGenerales
        Inherits OperacionPorEntrada
        Implements IEntradasDesdeExcel
        Implements IEsquemaExcel

        Public ReadOnly Property Esquema As EsquemaExcel Implements IEsquemaExcel.Esquema
            Get
                Return New EsquemaExcel(
                    {New ColumnaExcel("IdDocumento", "1845201",
                                      "Solo el número. Una fila por documento")},
                    aviso:="Las columnas de la B en adelante no se leen.")
            End Get
        End Property

        ''' <summary>Los Id del Excel, uno por fila desde la 2. Igual que el original.</summary>
        Public Function LeerEntradas(rutaExcel As String) As IReadOnlyList(Of String) _
            Implements IEntradasDesdeExcel.LeerEntradas

            Dim ids As New List(Of String)

            Using flujo As New FileStream(rutaExcel, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Dim libro As New XLWorkbook(flujo)
                Dim hoja = libro.Worksheets.First()

                Dim usado = hoja.RangeUsed()
                If usado Is Nothing Then Return ids

                For fila = 2 To usado.LastRow().RowNumber()
                    Dim celda = hoja.Cell(fila, 1)
                    If celda.IsEmpty() Then Continue For

                    Dim texto = celda.Value.ToString().Trim()
                    If texto.Length = 0 Then Continue For

                    ids.Add(texto)
                Next
            End Using

            Return ids

        End Function

        Private _carpeta As String = ""
        Private ReadOnly _escritos As New List(Of String)

        Protected Overrides Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _escritos.Clear()

            ' El original lo dejaba en Escritorio\ConsultasBO\DocumentosGenerales. Se mantiene el
            ' mismo sitio salvo que se elija otro en el formulario.
            _carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                          RutasSalida.Asegurar("DocumentosGenerales"),
                          ctx.CarpetaDestino)

            Directory.CreateDirectory(_carpeta)
            Return Task.CompletedTask

        End Function

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim id As Long
            If Not Long.TryParse(entrada.Trim(), id) OrElse id <= 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("el Id del documento no es un número"))
            End If

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)

            ' El original hacía «Dim Doc As Byte() = Nothing : If Doc Is Nothing Then ...», y esa
            ' comprobación era siempre verdadera porque lo acababa de poner a Nothing. Parece el
            ' resto de una versión que primero lo intentaba por otro sitio. Aquí se llama y ya.
            Dim datos As Byte() = Nothing
            funciones.DocumentDataFromCopiaAnioSiNulo(datos, id)

            If datos Is Nothing OrElse datos.Length = 0 Then
                ' El original hacía Continue For y el documento desaparecía del recuento.
                Return Task.FromResult(ResultadoEntrada.SinDatos("la API no ha devuelto el documento"))
            End If

            Dim ruta = Path.Combine(_carpeta, $"Documento_{id}.PDF")
            File.WriteAllBytes(ruta, datos)
            _escritos.Add(ruta)

            Return Task.FromResult(
                ResultadoEntrada.ConDatos(1, Kb(datos.Length)).Genera(_carpeta))

        End Function

        Protected Overrides Function CerrarAsync(ctx As ContextoEjecucion,
                                                 resultado As ResultadoOperacion) As Task

            resultado.Mensaje = If(_escritos.Count = 0,
                                   "No se ha bajado ningún documento",
                                   Redaccion.Cuenta(_escritos.Count, "documento"))

            Return Task.CompletedTask

        End Function

        ''' <summary>Tamaño en KB, como en la descarga de PDF de facturas.</summary>
        Private Shared Function Kb(bytes As Integer) As String
            Return $"{Math.Max(1, CInt(Math.Round(bytes / 1024.0))):N0} KB"
        End Function

    End Class

End Namespace
