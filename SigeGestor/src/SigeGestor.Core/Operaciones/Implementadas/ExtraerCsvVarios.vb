Option Strict Off   ' Usa FuncionesGenericas, Helper y FacsCSV, portados sin Option Strict.

Imports System.Data
Imports System.IO
Imports System.Text
Imports ClosedXML.Excel
Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Saca a CSV los datos de facturación de las facturas cuyos Id vienen en un Excel. Es el
    ''' Button14 («Extraer CSV Varios») de ActualizaPrecios, que llamaba a ValidacionExcel.CSV3.
    '''
    ''' LA CONSULTA SE LANZA DOS VECES, y no es un descuido del original: las facturas que
    ''' tienen CodigoContrato se unen con el contrato y las que no, con el cliente. La misma
    ''' plantilla con dos condiciones distintas, y los resultados se juntan. Está comprobado
    ''' que el marcador de la condición se usa de verdad (línea 131 del .sql).
    '''
    ''' ES UNA SOLA OPERACIÓN, NO UNA POR FACTURA: la consulta pide TODOS los Id de golpe en un
    ''' IN. Partirla por factura serían mil viajes a la base para el mismo resultado, y además la
    ''' consulta son 127 líneas de CTEs sobre potencias contratadas: se paga una vez.
    '''
    ''' SALE UN CSV Y NO UN EXCEL, igual que el original: son ficheros de decenas de miles de
    ''' filas que se cargan en otra herramienta, y el punto y coma es el separador que espera
    ''' Excel en español.
    ''' </summary>
    Public Class ExtraerCsvVarios
        Inherits OperacionUnica
        Implements IEntradasDesdeExcel
        Implements IEsquemaExcel

        ''' <summary>Cabecera del CSV, con las 29 columnas del original y en su orden.</summary>
        Private Shared ReadOnly Columnas As String() = {
            "CIFDNI", "RazonSocial", "Direccion", "CodigoCUPS", "CodPostal", "Poblacion",
            "Provincia", "codigocontrato", "SECTOR", "PotContratadaP1", "PotContratadaP2",
            "PotContratadaP3", "PotContratadaP4", "PotContratadaP5", "PotContratadaP6",
            "Tarifa", "Distribuidora", "FechaFactura", "ImporteTotal", "textotipocobro",
            "NumeroFactura", "idfacturaorigen", "FechaDesde", "FechaHasta", "ImporteElectrico",
            "ImporteClick", "PorcentajeIVA", "BaseIVA", "ImporteIVA"
        }

        ''' <summary>Cómo se une FacturaVentaCabecera cuando la factura tiene contrato.</summary>
        Private Const PorContrato As String = "fvc.codigocontrato = c.codigocontrato"

        ''' <summary>Y cuando no lo tiene: por cliente.</summary>
        Private Const PorCliente As String = "fvc.idcliente = cl.idcliente"

        Private ReadOnly _sql As RepositorioSql

        Public Sub New(sql As RepositorioSql)
            _sql = sql
        End Sub

        Public ReadOnly Property Esquema As EsquemaExcel Implements IEsquemaExcel.Esquema
            Get
                Return New EsquemaExcel(
                    {New ColumnaExcel("IdFacturaVentaCabecera", "4185203",
                                      "Solo el número. Una fila por factura")},
                    aviso:="Es el Id interno de la factura, no su serie y número. " &
                           "Las columnas de la B en adelante no se leen.")
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

                    ' Solo números: el original hacía ListaIdInicial.Add(id) con lo que hubiera y
                    ' un texto en la columna acababa dentro del IN, que revienta la consulta
                    ' entera con «Error al convertir el valor nvarchar».
                    Dim n As Long
                    If Long.TryParse(celda.Value.ToString().Trim(), n) AndAlso n > 0 Then
                        ids.Add(n.ToString())
                    End If
                Next
            End Using

            Return ids

        End Function

        Protected Overrides Async Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                            avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            If Not File.Exists(ctx.RutaExcel) Then
                Return ResultadoEntrada.Fallo("elige el fichero de Excel")
            End If

            Dim ids = ctx.Entradas.ToList()
            If ids.Count = 0 Then
                Return ResultadoEntrada.SinDatos("el Excel no tenía ningún Id de factura")
            End If

            ' Se comprueba la conexión antes de nada: sin esto, con la base caída GetFacVentaLista
            ' devuelve lista vacía —se come el error— y esto saldría como «ninguna de las facturas
            ' existe», que manda a mirar el Excel cuando el problema es otro.
            avisar("Comprobando la conexión…")
            Dim problema = Await ctx.ProbarConexionAsync().ConfigureAwait(False)
            If problema.Length > 0 Then
                Return ResultadoEntrada.Fallo($"no se puede conectar a {ctx.Entorno?.Nombre}: {problema}")
            End If

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)

            avisar($"Buscando {Redaccion.Cuenta(ids.Count, "factura")}…")

            Dim facturas = Await Task.Run(
                Function() funciones.GetFacVentaLista(String.Join(",", ids))).ConfigureAwait(False)

            If facturas Is Nothing OrElse facturas.Count = 0 Then
                Return ResultadoEntrada.SinDatos("ninguno de los Id existe en FacturaVentaCabecera")
            End If

            ctx.AbortarSiCancelado()

            ' La división del original: con contrato por un lado, sin contrato por otro.
            Dim conContrato = facturas.Where(Function(f) If(f.CodigoContrato, 0) > 0) _
                                      .Select(Function(f) f.IdFacturaVentaCabecera).ToList()

            Dim sinContrato = facturas.Where(Function(f) If(f.CodigoContrato, 0) <= 0) _
                                      .Select(Function(f) f.IdFacturaVentaCabecera).ToList()

            Dim filas As New List(Of FacsCSV)

            If conContrato.Count > 0 Then
                avisar($"Consultando {Redaccion.Cuenta(conContrato.Count, "factura")} con contrato…")
                filas.AddRange(Await ConsultarAsync(ctx, conContrato, PorContrato).ConfigureAwait(False))
            End If

            If sinContrato.Count > 0 Then
                avisar($"Consultando {Redaccion.Cuenta(sinContrato.Count, "factura")} sin contrato…")
                filas.AddRange(Await ConsultarAsync(ctx, sinContrato, PorCliente).ConfigureAwait(False))
            End If

            If filas.Count = 0 Then
                Return ResultadoEntrada.SinDatos(
                    $"las {facturas.Count:N0} facturas existen, pero la consulta no ha devuelto filas")
            End If

            ctx.AbortarSiCancelado()

            Dim carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                             RutasSalida.Asegurar("CSVFacturas"),
                             ctx.CarpetaDestino)
            Directory.CreateDirectory(carpeta)

            Dim ruta = Path.Combine(carpeta, $"CSVFACVA_{DateTime.Now:yyyyMMdd_HHmm}.csv")

            avisar($"Escribiendo {Redaccion.Cuenta(filas.Count, "fila")}…")
            Escribir(filas, ruta)

            Dim mensaje = Redaccion.Unir(
                Path.GetFileName(ruta),
                Redaccion.Cuenta(filas.Count, "fila"),
                If(sinContrato.Count = 0, "",
                   $"{sinContrato.Count:N0} sin contrato, unidas por cliente"))

            Return ResultadoEntrada.ConDatos(filas.Count, mensaje).Genera(ruta)

        End Function

        ''' <summary>
        ''' Lanza la plantilla con una lista de Id y una condición de unión.
        ''' </summary>
        Private Async Function ConsultarAsync(ctx As ContextoEjecucion,
                                              ids As List(Of Long),
                                              condicion As String) As Task(Of List(Of FacsCSV))

            Dim plantilla As New PlantillaSql(_sql.Obtener("FacturasCSVVarios"))

            plantilla.Poner("idsReplace", String.Join(",", ids))
            plantilla.Poner("condicionReplace", condicion)

            Dim pendientes = plantilla.MarcadoresPendientes()
            If pendientes.Count > 0 Then
                Throw New InvalidOperationException(
                    $"Marcadores sin resolver en FacturasCSVVarios.sql: {String.Join(", ", pendientes)}")
            End If

            Dim sql = plantilla.ToString()

            Return Await Task.Run(
                Function()
                    Dim resultado = Helper.QuerySelect(sql, ctx.CadenaConexion)

                    Dim errores = Helper.GetError(resultado)
                    If errores.HasError Then
                        ' El original se comía esto con un Catch vacío y devolvía lista vacía, así
                        ' que un error de SQL salía como «no hay datos».
                        Throw New InvalidOperationException(
                            $"La consulta ha fallado: {errores.DescripcionError}")
                    End If

                    If resultado.Tables.Count = 0 Then Return New List(Of FacsCSV)

                    Return Helper.FillObjectFromDatatable(resultado.Tables(0), GetType(FacsCSV)) _
                                 .Cast(Of FacsCSV).ToList()
                End Function).ConfigureAwait(False)

        End Function

        ''' <summary>
        ''' El CSV, con las mismas 29 columnas y el mismo separador que el original.
        '''
        ''' UTF-8 CON BOM, al contrario que el original, que usaba Encoding.UTF8 en el
        ''' StreamWriter —que también lleva BOM— pero conviene dejarlo dicho: sin BOM, Excel
        ''' abre el fichero como ANSI y las tildes de las razones sociales salen partidas.
        '''
        ''' Los valores se saneen del separador: una razón social con punto y coma dentro
        ''' descolocaba todas las columnas de esa fila y el original no lo miraba.
        ''' </summary>
        Private Shared Sub Escribir(filas As List(Of FacsCSV), ruta As String)

            Using escritor As New StreamWriter(ruta, False, New UTF8Encoding(True))

                escritor.WriteLine(String.Join(";", Columnas))

                For Each f In filas
                    escritor.WriteLine(String.Join(";", {
                        Limpio(f.CIFDNI), Limpio(f.RazonSocial), Limpio(f.Direccion),
                        Limpio(f.CodigoCUPS), Limpio(f.CodPostal), Limpio(f.Poblacion),
                        Limpio(f.Provincia), Limpio(f.CodigoContrato), Limpio(f.Sector),
                        Limpio(f.PotContratadaP1), Limpio(f.PotContratadaP2), Limpio(f.PotContratadaP3),
                        Limpio(f.PotContratadaP4), Limpio(f.PotContratadaP5), Limpio(f.PotContratadaP6),
                        Limpio(f.Tarifa), Limpio(f.Distribuidora), Limpio(f.FechaFactura),
                        Limpio(f.ImporteTotal), Limpio(f.TextoTipoCobro), Limpio(f.NumeroFactura),
                        Limpio(f.idfacturaorigen), Limpio(f.FechaDesde), Limpio(f.FechaHasta),
                        Limpio(f.ImporteElectrico), Limpio(f.ImporteClick), Limpio(f.PorcentajeIVA),
                        Limpio(f.BaseIVA), Limpio(f.ImporteIVA)}))
                Next

            End Using

        End Sub

        ''' <summary>
        ''' Un valor listo para el CSV: sin el separador dentro y sin saltos de línea.
        '''
        ''' No se entrecomilla, se sustituye. Entrecomillar obligaría a duplicar las comillas
        ''' internas y a que quien lo lea respete el formato; cambiar el punto y coma por una
        ''' coma no rompe nada y el fichero sigue abriéndose de un doble clic.
        ''' </summary>
        Private Shared Function Limpio(valor As Object) As String

            If valor Is Nothing OrElse Convert.IsDBNull(valor) Then Return ""

            Return Convert.ToString(valor) _
                          .Replace(";", ",") _
                          .Replace(vbCr, " ") _
                          .Replace(vbLf, " ") _
                          .Trim()

        End Function

    End Class

End Namespace
