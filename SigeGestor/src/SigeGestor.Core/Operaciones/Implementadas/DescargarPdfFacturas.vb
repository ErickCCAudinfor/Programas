Option Strict Off   ' Usa los DTO portados.

Imports System.IO
Imports SigeGestor.Core.Configuracion

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Maquinaria común de las tres operaciones que bajan el PDF de una lista de facturas.
    '''
    ''' En ActualizaPrecios eran tres manejadores de botón con el mismo cuerpo copiado tres
    ''' veces —Button12, Button29 y PDFBotonAgrupado— y la única diferencia real era en qué
    ''' carpeta cae cada PDF. Aquí eso es un método, <see cref="SubcarpetaDe"/>, y lo demás se
    ''' escribe una sola vez.
    '''
    ''' MEJORA SOBRE EL ORIGINAL: allí las tres bajaban la lista entera dentro de un único
    ''' Task.Run con un GIF girando, sin decir por dónde iban ni qué facturas no existían; si
    ''' una fallaba, se perdía en silencio. Aquí cada factura es una entrada, con su estado y su
    ''' motivo, y una que falle no detiene las demás.
    ''' </summary>
    Public MustInherit Class DescargarPdfFacturas
        Inherits OperacionPorEntrada

        ''' <summary>
        ''' Subcarpeta —relativa a la carpeta de salida— donde va el PDF de esta factura.
        ''' Cadena vacía para dejarlo en la propia carpeta de salida.
        ''' </summary>
        Protected MustOverride Function SubcarpetaDe(factura As String,
                                                     funciones As FuncionesGenericas) As String

        Private _carpetaBase As String = ""

        ''' <summary>Rutas de los PDF escritos, en el orden de la lista.</summary>
        Protected ReadOnly Escritos As New List(Of String)

        Protected Overrides Function PrepararAsync(ctx As ContextoEjecucion) As Task

            Escritos.Clear()

            ' ActualizaPrecios ya dejaba estos PDF en Escritorio\ConsultasBO\PDFFacturas, así
            ' que se mantiene el mismo sitio salvo que se elija otro en el formulario.
            _carpetaBase = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                              RutasSalida.Asegurar("PDFFacturas"),
                              ctx.CarpetaDestino)

            Directory.CreateDirectory(_carpetaBase)
            Return Task.CompletedTask

        End Function

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim factura = entrada.Trim()
            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)

            Dim bytes = funciones.ExtraerPDFFactura(factura)
            If bytes Is Nothing OrElse bytes.Length = 0 Then
                ' El original hacía «If bytesPDF Is Nothing Then Return» y seguía sin más: la
                ' factura desaparecía del recuento. Aquí se dice.
                Return Task.FromResult(ResultadoEntrada.SinDatos("sin PDF en la base"))
            End If

            Dim carpeta = _carpetaBase
            Dim sub_ = SubcarpetaDe(factura, funciones)
            If Not String.IsNullOrWhiteSpace(sub_) Then
                carpeta = Path.Combine(_carpetaBase, NombreDeCarpetaValido(sub_))
                Directory.CreateDirectory(carpeta)
            End If

            Dim ruta = Path.Combine(carpeta, NombreDePdf(factura))
            File.WriteAllBytes(ruta, bytes)
            Escritos.Add(ruta)

            Dim donde = If(String.IsNullOrWhiteSpace(sub_), "", $" en {Path.GetFileName(carpeta)}")
            Return Task.FromResult(ResultadoEntrada.ConDatos(1, $"{Kb(bytes.Length)}{donde}"))

        End Function

        ''' <summary>Carpeta de salida ya resuelta. Para el mensaje final.</summary>
        Protected ReadOnly Property CarpetaBase As String
            Get
                Return _carpetaBase
            End Get
        End Property

        ''' <summary>
        ''' Nombre del PDF, con la misma regla que el original: FELEC pasa a FELEC_ para que se
        ''' distinga la serie del número, y se corta a 100 caracteres.
        ''' </summary>
        Private Shared Function NombreDePdf(factura As String) As String

            Dim nombre = factura.Replace("FELEC", "FELEC_")
            If nombre.Length > 100 Then nombre = nombre.Substring(0, 100)
            Return nombre & ".PDF"

        End Function

        ''' <summary>
        ''' El original componía la carpeta con la denominación del cliente tal cual venía de la
        ''' base. Una denominación con / o con : —hay razones sociales con barras— hacía saltar
        ''' ArgumentException y se comía la excepción del Task.Run entera, así que se perdía el
        ''' lote completo sin explicación. Aquí se sanea.
        ''' </summary>
        Private Shared Function NombreDeCarpetaValido(nombre As String) As String

            Dim limpio = nombre
            For Each c In Path.GetInvalidFileNameChars()
                limpio = limpio.Replace(c, "_"c)
            Next

            limpio = limpio.Trim().TrimEnd("."c)
            If limpio.Length > 120 Then limpio = limpio.Substring(0, 120)

            Return If(limpio.Length = 0, "SinNombre", limpio)

        End Function

        Private Shared Function Kb(bytes As Integer) As String
            If bytes < 1024 Then Return $"{bytes} B"
            Return $"{bytes / 1024.0:N0} KB"
        End Function

    End Class

    ''' <summary>
    ''' Baja el PDF de cada factura a una sola carpeta y, al terminar, los une todos en
    ''' Facturas_Unificadas.pdf. Es el Button12 de ActualizaPrecios.
    ''' </summary>
    Public Class ExtraerPdfFacturas
        Inherits DescargarPdfFacturas

        Protected Overrides Function SubcarpetaDe(factura As String,
                                                  funciones As FuncionesGenericas) As String
            Return ""   ' todos juntos
        End Function

        Protected Overrides Function CerrarAsync(ctx As ContextoEjecucion,
                                                 resultado As ResultadoOperacion) As Task

            ' El original unía SIEMPRE que hubiera bajado al menos una, incluso si se había
            ' cancelado a medias. Se mantiene: un PDF unificado parcial sigue sirviendo, y el
            ' recuento de arriba ya dice que se canceló.
            If Escritos.Count = 0 Then
                resultado.Mensaje = "No se ha descargado ningún PDF"
                Return Task.CompletedTask
            End If

            Dim unificado = Path.Combine(CarpetaBase, "Facturas_Unificadas.pdf")

            Try
                UnirPdf.Unir(Escritos, unificado)
                resultado.Mensaje = $"{Escritos.Count:N0} PDF en {CarpetaBase}, unidos en Facturas_Unificadas.pdf"
            Catch ex As Exception
                ' Los PDF sueltos ya están escritos: que falle la unión no invalida el trabajo.
                resultado.Mensaje = $"{Escritos.Count:N0} PDF en {CarpetaBase} · no se han podido unir: {ex.Message}"
            End Try

            Return Task.CompletedTask

        End Function

    End Class

    ''' <summary>
    ''' Baja el PDF de cada factura a una carpeta por cliente, «Denominación-_Identidad».
    ''' Es el Button29 («Extr. PDF Cliente») de ActualizaPrecios.
    ''' </summary>
    Public Class ExtraerPdfPorCliente
        Inherits DescargarPdfFacturas

        Protected Overrides Function SubcarpetaDe(factura As String,
                                                  funciones As FuncionesGenericas) As String

            Dim cliente = funciones.GetClientebyFac(factura)
            If cliente Is Nothing Then Return "SinCliente"

            ' Misma forma de nombre que el original: «Denominación-_Identidad».
            Dim clave = $"{cliente.Denominacion}-_{cliente.Identidad}"
            Return If(clave.Trim() = "-_", "SinCliente", clave)

        End Function

        Protected Overrides Function CerrarAsync(ctx As ContextoEjecucion,
                                                 resultado As ResultadoOperacion) As Task

            resultado.Mensaje = If(Escritos.Count = 0,
                                   "No se ha descargado ningún PDF",
                                   $"{Escritos.Count:N0} PDF por cliente en {CarpetaBase}")
            Return Task.CompletedTask

        End Function

    End Class

    ''' <summary>
    ''' Baja el PDF de cada factura a una carpeta por número de pedido de facturación.
    ''' Es el PDFBotonAgrupado de ActualizaPrecios.
    '''
    ''' No une nada: la descripción del catálogo decía «une en un solo PDF», pero el original
    ''' solo agrupa en carpetas. Se ha corregido la descripción, no el comportamiento.
    ''' </summary>
    Public Class ExtraerPdfPorPedido
        Inherits DescargarPdfFacturas

        Protected Overrides Function SubcarpetaDe(factura As String,
                                                  funciones As FuncionesGenericas) As String

            Dim datos = funciones.GetNumPedidoFacturacionbyFac(factura)
            Dim pedido = If(datos Is Nothing, "", CStr(datos.NumPedidoFacturacion))

            ' Sin número de pedido el original creaba una carpeta con nombre vacío, lo que
            ' dejaba el PDF en la raíz sin avisar. Aquí va a una carpeta que se ve.
            Return If(String.IsNullOrWhiteSpace(pedido), "SinNumeroDePedido", pedido)

        End Function

        Protected Overrides Function CerrarAsync(ctx As ContextoEjecucion,
                                                 resultado As ResultadoOperacion) As Task

            resultado.Mensaje = If(Escritos.Count = 0,
                                   "No se ha descargado ningún PDF",
                                   $"{Escritos.Count:N0} PDF por nº de pedido en {CarpetaBase}")
            Return Task.CompletedTask

        End Function

    End Class

End Namespace
