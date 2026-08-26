Option Strict Off   ' Convive con el resto del código portado.

Imports System.Data
Imports System.IO
Imports ClosedXML.Excel
Imports Microsoft.Data.SqlClient

''' <summary>
''' ADAPTADOR DE COMPATIBILIDAD para el código portado de ActualizaPrecios.
'''
''' Allí, FuncionesGenericas usa «New ValidacionExcel(connectionString)» en tres sitios
''' —EscribirContratoTarifaAntesCambios y las dos RevisaTarifaPrecioContratoPersonalizada—
''' y de esa clase de 1.506 líneas solo llama a EjecutarConsultasYGuardarEnExcel. El nombre
''' engaña: no valida nada, vuelca consultas a un Excel.
'''
''' Se reimplementa aquí ese único método, con ClosedXML, que es lo que ya usaba el original.
''' Así no hay que arrastrar el fichero entero ni EPPlus, que es de pago desde la versión 5.
'''
''' Si algún día se porta la validación de plantillas de verdad, esta clase se sustituye.
''' </summary>
Public Class ValidacionExcel

    Public Property connectionString As String

    Public Sub New(cadenaConexion As String)
        connectionString = cadenaConexion
        Funciones = New FuncionesGenericas(cadenaConexion)
    End Sub

    ' ------------------------------------------------------------------
    ' Estado que usan las penalizaciones, con los mismos nombres que en ActualizaPrecios para
    ' poder traer sus métodos sin tocar el cálculo.
    '
    ' Diferencia con el original: allí «path» era la ruta de UN fichero, la misma para luz y
    ' para gas. Aquí es la CARPETA, y cada tipo saca su propio fichero (ver
    ' GenerarPenalizacionesAsync).
    '
    ' Y no se trae «facturas»: en el original lo usaba otra parte de ValidacionExcel, no las
    ' penalizaciones. Aquí sobraría, así que el constructor tiene cuatro argumentos y no cinco.
    '
    ' OJO al original: allí «Private Funciones As New FuncionesGenericas(connectionString)» es
    ' un inicializador de campo, y corre ANTES de que el constructor asigne connectionString —
    ' así que recibía Nothing. Solo se salvaba porque el constructor de cinco argumentos lo
    ' sobrescribía. Aquí se construye dentro del constructor, cuando ya hay valor.
    ' ------------------------------------------------------------------
    Public ReadOnly Property path As String
    Public ReadOnly Property contratos As New List(Of Long)

    Private Funciones As FuncionesGenericas

    ''' <summary>Ficheros escritos en la última llamada a GenerarPenalizacionesAsync.</summary>
    Public ReadOnly Property Generados As New List(Of String)

    Public Sub New(cadenaConexion As String,
                   carpetaSalida As String,
                   listaContratos As List(Of Long),
                   funcionesGenericas As FuncionesGenericas)
        connectionString = cadenaConexion
        path = carpetaSalida
        contratos = If(listaContratos, New List(Of Long))
        Funciones = If(funcionesGenericas, New FuncionesGenericas(cadenaConexion))
    End Sub


    ''' <summary>
    ''' Ejecuta cada consulta y deja cada resultado en su propia hoja del mismo libro.
    ''' Mantiene el comportamiento del original, incluido el desduplicado de nombres de
    ''' columna repetidos: sin eso, dos columnas con el mismo alias rompían la escritura.
    ''' </summary>
    Public Sub EjecutarConsultasYGuardarEnExcel(consultas As List(Of String), nombreArchivo As String)

        If consultas Is Nothing OrElse consultas.Count = 0 Then Exit Sub

        Dim carpeta = IO.Path.GetDirectoryName(nombreArchivo)
        If Not String.IsNullOrEmpty(carpeta) Then IO.Directory.CreateDirectory(carpeta)

        Using libro As New XLWorkbook()

            For i = 0 To consultas.Count - 1

                Dim hoja As IXLWorksheet = libro.Worksheets.Add("Hoja" & (i + 1).ToString())

                Using conexion As New SqlConnection(connectionString)
                    conexion.Open()

                    Using comando As New SqlCommand(consultas(i), conexion)
                        comando.CommandTimeout = 0

                        Using lector = comando.ExecuteReader()

                            ' Cabeceras, con sufijo numérico si un nombre se repite
                            Dim usados As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                            For j = 0 To lector.FieldCount - 1
                                Dim nombre = lector.GetName(j)
                                If String.IsNullOrWhiteSpace(nombre) Then nombre = $"Columna{j + 1}"

                                Dim unico = nombre
                                Dim sufijo = 2
                                While Not usados.Add(unico)
                                    unico = $"{nombre}_{sufijo}"
                                    sufijo += 1
                                End While

                                hoja.Cell(1, j + 1).Value = unico
                            Next
                            hoja.Row(1).Style.Font.Bold = True

                            Dim fila = 2
                            While lector.Read()
                                For j = 0 To lector.FieldCount - 1
                                    If lector.IsDBNull(j) Then Continue For
                                    hoja.Cell(fila, j + 1).Value = XLCellValue.FromObject(lector.GetValue(j))
                                Next
                                fila += 1
                            End While

                        End Using
                    End Using
                End Using
            Next

            libro.SaveAs(nombreArchivo)
        End Using

    End Sub


    ' ==================================================================
    ' PENALIZACIONES — copiadas LITERALMENTE de ValidacionExcel de
    ' ActualizaPrecios (378 líneas). No se ha tocado ni el cálculo ni el
    ' SQL: lo único que las hacía incompatibles era EPPlus, y de eso se
    ' encarga el adaptador de CompatEPPlus.vb.
    ' ==================================================================

    ''' <summary>
    ''' Clasifica los contratos por entorno —E1 luz, E2 gas— y genera un Excel por tipo.
    '''
    ''' ESTO SÍ SE HA CAMBIADO respecto al original, y a propósito. Allí los dos tipos escribían
    ''' en la MISMA ruta, y cada uno creaba el libro desde cero: si en la lista había luz y gas,
    ''' gas machacaba el fichero de luz y el usuario se quedaba solo con la mitad, sin aviso.
    ''' Además el nombre llevaba solo la fecha, así que dos ejecuciones el mismo día se pisaban.
    '''
    ''' Aquí cada tipo saca su propio fichero, con hora, y nunca se sobrescribe nada.
    ''' El cálculo de PenalizacionesLuz y PenalizacionesGas no se ha tocado.
    ''' </summary>
    Public Async Function GenerarPenalizacionesAsync() As Task(Of Boolean)

        Generados.Clear()

        Dim luz As New List(Of Long)
        Dim gas As New List(Of Long)

        ' Clasificación de contratos
        For Each con In contratos
            Dim info = Funciones.GetContrato(con)
            If info Is Nothing OrElse info.CodigoContrato <= 0 Then Continue For

            Select Case info.Entorno
                Case "E1" : luz.Add(info.CodigoContrato)
                Case "E2" : gas.Add(info.CodigoContrato)
            End Select
        Next

        If luz.Count > 0 Then
            Dim destino = RutaLibre("Penalizaciones_LUZ")
            Await Task.Run(Sub() PenalizacionesLuz(luz, destino)).ConfigureAwait(False)
            Generados.Add(destino)
        End If

        If gas.Count > 0 Then
            Dim destino = RutaLibre("Penalizaciones_GAS")
            Await Task.Run(Sub() PenalizacionesGas(gas, destino)).ConfigureAwait(False)
            Generados.Add(destino)
        End If

        Return Generados.Count > 0

    End Function

    ''' <summary>
    ''' Ruta de fichero que no exista todavía dentro de la carpeta de salida. Con fecha y hora,
    ''' y si aun así coincide, sufijo numérico: no se sobrescribe un volcado anterior nunca.
    ''' </summary>
    Private Function RutaLibre(nombreBase As String) As String

        Directory.CreateDirectory(path)

        ' Se cualifica System.IO.Path entero porque la propiedad «path» de esta clase —así se
        ' llama en el original— tapa el nombre del tipo.
        Dim sello = DateTime.Now.ToString("yyyyMMdd_HHmm")
        Dim candidata = System.IO.Path.Combine(path, $"{nombreBase}_{sello}.xlsx")

        Dim n = 2
        While File.Exists(candidata)
            candidata = System.IO.Path.Combine(path, $"{nombreBase}_{sello}_{n}.xlsx")
            n += 1
        End While

        Return candidata

    End Function

    Sub PenalizacionesLuz(CodigoContrato As List(Of Long), Ubicacion As String)
        Try
            ' Crear un nuevo archivo Excel
            Dim filePath As String = Ubicacion
            Dim fileInfo As New FileInfo(filePath)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Using package As New ExcelPackage(fileInfo)
                Dim worksheet = package.Workbook.Worksheets.Add("P_LUZ")

                ' Escribir encabezados
                worksheet.Cells(1, 1).Value = "CodigoContrato"
                worksheet.Cells(1, 2).Value = "SerieFactura"
                worksheet.Cells(1, 3).Value = "NumeroFactura"
                worksheet.Cells(1, 4).Value = "SumaPrecioP1"
                worksheet.Cells(1, 5).Value = "SumaPrecioP2"
                worksheet.Cells(1, 6).Value = "SumaPrecioP3"
                worksheet.Cells(1, 7).Value = "SumaPrecioP4"
                worksheet.Cells(1, 8).Value = "SumaPrecioP5"
                worksheet.Cells(1, 9).Value = "SumaPrecioP6"
                worksheet.Cells(1, 10).Value = "ConsumoP1"
                worksheet.Cells(1, 11).Value = "ConsumoP2"
                worksheet.Cells(1, 12).Value = "ConsumoP3"
                worksheet.Cells(1, 13).Value = "ConsumoP4"
                worksheet.Cells(1, 14).Value = "ConsumoP5"
                worksheet.Cells(1, 15).Value = "ConsumoP6"
                worksheet.Row(1).Style.Font.Bold = True

                Dim currentRow As Integer = 2

                ' Cadena de conexión
                ' Dim connectionString As String = "Server=TU_SERVIDOR;Database=TU_BASE_DE_DATOS;Trusted_Connection=True;"



                For Each codigo As Long In CodigoContrato
                    Using connection As New SqlConnection(connectionString)
                        connection.Open()
                        ' Consulta SQL que recupera los datos
                        Dim query As String = "
                    WITH FacturasVentaConsulta AS (
    SELECT 
        MAX(NumeroFactura) AS NumeroFactura, 
        CodigoContrato 
    FROM 
        FacturaVentaCabecera WITH (NOLOCK) 
    WHERE 
        CodigoContrato IN (
@CodigoContrato
        )
    AND SerieFactura = 'FELEC'
    GROUP BY 
        CodigoContrato
)
SELECT 
    fvc.CodigoContrato,
    fvc.SerieFactura,
    fvc.NumeroFactura,

    -- Precios de cada periodo sumando PrecioCargo y PrecioMedio de las tablas correspondientes
    REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 1 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 1 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 1 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvld.codigoperiodoXML = 1 THEN  fvld.PrecioMedioVariable ELSE NULL END), 0), '.', ',') AS SumaPrecioP1,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 2 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 2 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 2 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvld.codigoperiodoXML = 2 THEN  fvld.PrecioMedioVariable ELSE NULL END), 0), '.', ',') AS SumaPrecioP2,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 3 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 3 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 3 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvld.codigoperiodoXML = 3 THEN  fvld.PrecioMedioVariable ELSE NULL END), 0), '.', ',') AS SumaPrecioP3,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 4 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 4 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 4 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvld.codigoperiodoXML = 4 THEN  fvld.PrecioMedioVariable ELSE NULL END), 0), '.', ',') AS SumaPrecioP4,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 5 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 5 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 5 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvld.codigoperiodoXML = 5 THEN  fvld.PrecioMedioVariable ELSE NULL END), 0), '.', ',') AS SumaPrecioP5,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 6 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 6 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 6 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvld.codigoperiodoXML = 6 THEN  fvld.PrecioMedioVariable ELSE NULL END), 0), '.', ',') AS SumaPrecioP6,

    -- Consumos de cada periodo
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 1 or (fvld.codigoperiodoXML = 1 and tg.TextoTarifaGrupo like '%soul%') THEN 
        case when tg.TextoTarifaGrupo like '%soul%' then fvld.ConsumoVariable else ISNULL(fvlc.TotConsumoEnergiaXML, 0) end
        ELSE NULL END), 0), '.', ',') AS ConsumoP1,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 2 or (fvld.codigoperiodoXML = 2 and tg.TextoTarifaGrupo like '%soul%') THEN 
        case when tg.TextoTarifaGrupo like '%soul%' then fvld.ConsumoVariable else ISNULL(fvlc.TotConsumoEnergiaXML, 0) end
        ELSE NULL END), 0), '.', ',') AS ConsumoP2,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 3 or (fvld.codigoperiodoXML = 3 and tg.TextoTarifaGrupo like '%soul%') THEN 
                case when tg.TextoTarifaGrupo like '%soul%' then fvld.ConsumoVariable else ISNULL(fvlc.TotConsumoEnergiaXML, 0) end
        ELSE NULL END), 0), '.', ',') AS ConsumoP3,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 4 or (fvld.codigoperiodoXML = 4 and tg.TextoTarifaGrupo like '%soul%') THEN 
        case when tg.TextoTarifaGrupo like '%soul%' then fvld.ConsumoVariable else ISNULL(fvlc.TotConsumoEnergiaXML, 0) end
        ELSE NULL END), 0), '.', ',') AS ConsumoP4,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 5 or (fvld.codigoperiodoXML = 5 and tg.TextoTarifaGrupo like '%soul%') THEN 
        case when tg.TextoTarifaGrupo like '%soul%' then fvld.ConsumoVariable else ISNULL(fvlc.TotConsumoEnergiaXML, 0) end
        ELSE NULL END), 0), '.', ',') AS ConsumoP5,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 6 or (fvld.codigoperiodoXML = 6 and tg.TextoTarifaGrupo like '%soul%') THEN 
        case when tg.TextoTarifaGrupo like '%soul%' then fvld.ConsumoVariable else ISNULL(fvlc.TotConsumoEnergiaXML, 0) end
        ELSE NULL END), 0), '.', ',') AS ConsumoP6
		,tg.TextoTarifaGrupo
FROM FacturaVentaCabecera fvc WITH (NOLOCK)
LEFT JOIN FacturaVentaLinea fvl WITH (NOLOCK) ON fvl.idfacturaventacabecera = fvc.idfacturaventacabecera AND fvl.Facturaconcepto IN (130004)
LEFT JOIN FacturaVentaLinea fvlb WITH (NOLOCK) ON fvlb.idfacturaventacabecera = fvc.idfacturaventacabecera AND fvlb.Facturaconcepto IN (30001)
LEFT JOIN FacturaVentaLinea fvlc WITH (NOLOCK) ON fvlc.idfacturaventacabecera = fvc.idfacturaventacabecera AND fvlc.Facturaconcepto = 30002
left join (
select IdFacturaVentaCabecera
, sum(ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0))PrecioMedioVariable
, max(TotConsumoEnergiaXML) ConsumoVariable
,CodigoPeriodoXML
from facturaventalinea fvl
where facturaconcepto=30004
group by fvl.IdFacturaVentaCabecera, fvl.CodigoPeriodoXML
)  fvld ON fvld.idfacturaventacabecera = fvc.idfacturaventacabecera
left join TarifaGrupo tg on fvc.IdTarifaGrupoXML = tg.IdTarifaGrupo
WHERE fvc.IdFacturaVentaCabecera IN (
    SELECT idfacturaventacabecera 
    FROM FacturaVentaCabecera WITH (NOLOCK) 
    WHERE NumeroFactura IN (SELECT NumeroFactura FROM FacturasVentaConsulta WITH (NOLOCK))
    AND SerieFactura LIKE 'FELEC'
)
GROUP BY 
    fvc.CodigoContrato,
    fvc.SerieFactura,
    fvc.NumeroFactura
	,fvc.IdFacturaVentaCabecera
	,tg.TextoTarifaGrupo;

"

                        ' Ejecutar consulta
                        Dim command As New SqlCommand(query, connection)
                        command.Parameters.AddWithValue("@CodigoContrato", codigo)
                        command.CommandTimeout = 100000
                        ' Leer los resultados
                        Using reader As SqlDataReader = command.ExecuteReader()
                            While reader.Read()
                                worksheet.Cells(currentRow, 1).Value = reader("CodigoContrato")
                                worksheet.Cells(currentRow, 2).Value = reader("SerieFactura")
                                worksheet.Cells(currentRow, 3).Value = reader("NumeroFactura").ToString
                                Dim precioMedio As Decimal

                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("SumaPrecioP1").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 4).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 4).Value = 0
                                End If
                                worksheet.Cells(currentRow, 4).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal

                                ' Repetir el mismo proceso para los demás campos
                                If Decimal.TryParse(reader("SumaPrecioP2").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 5).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 5).Value = 0
                                End If
                                worksheet.Cells(currentRow, 5).Style.Numberformat.Format = "#,##0.000000"

                                If Decimal.TryParse(reader("SumaPrecioP3").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 6).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 6).Value = 0
                                End If
                                worksheet.Cells(currentRow, 6).Style.Numberformat.Format = "#,##0.000000"

                                If Decimal.TryParse(reader("SumaPrecioP4").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 7).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 7).Value = 0
                                End If
                                worksheet.Cells(currentRow, 7).Style.Numberformat.Format = "#,##0.000000"

                                If Decimal.TryParse(reader("SumaPrecioP5").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 8).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 8).Value = 0
                                End If
                                worksheet.Cells(currentRow, 8).Style.Numberformat.Format = "#,##0.000000"

                                If Decimal.TryParse(reader("SumaPrecioP6").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 9).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 9).Value = 0
                                End If
                                worksheet.Cells(currentRow, 9).Style.Numberformat.Format = "#,##0.000000"

                                ' Asegúrate de repetir el proceso para los campos de consumo
                                ' ConsumoP1, ConsumoP2, etc.
                                If Decimal.TryParse(reader("ConsumoP1").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 10).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 10).Value = 0
                                End If
                                worksheet.Cells(currentRow, 10).Style.Numberformat.Format = "#,##0.0"

                                If Decimal.TryParse(reader("ConsumoP2").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 11).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 11).Value = 0
                                End If
                                worksheet.Cells(currentRow, 11).Style.Numberformat.Format = "#,##0.0"

                                If Decimal.TryParse(reader("ConsumoP3").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 12).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 12).Value = 0
                                End If
                                worksheet.Cells(currentRow, 12).Style.Numberformat.Format = "#,##0.0"

                                If Decimal.TryParse(reader("ConsumoP4").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 13).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 13).Value = 0
                                End If
                                worksheet.Cells(currentRow, 13).Style.Numberformat.Format = "#,##0.0"

                                If Decimal.TryParse(reader("ConsumoP5").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 14).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 14).Value = 0
                                End If
                                worksheet.Cells(currentRow, 14).Style.Numberformat.Format = "#,##0.0"

                                If Decimal.TryParse(reader("ConsumoP6").ToString(), precioMedio) Then
                                    worksheet.Cells(currentRow, 15).Value = precioMedio
                                Else
                                    worksheet.Cells(currentRow, 15).Value = 0
                                End If
                                worksheet.Cells(currentRow, 15).Style.Numberformat.Format = "#,##0.0"

                                currentRow += 1
                            End While
                        End Using
                    End Using

                Next

                ' Guardar el archivo
                package.Save()
            End Using


        Catch ex As Exception
            Throw
        End Try
    End Sub

    Sub PenalizacionesGas(CodigoContrato As List(Of Long), Ubicacion As String)
        Try

            ' Crear un nuevo archivo Excel
            Dim filePath As String = Ubicacion
            Dim fileInfo As New FileInfo(filePath)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            ' Usar EPPlus para manejar Excel
            Using package As New ExcelPackage(fileInfo)
                Dim worksheet = package.Workbook.Worksheets.Add("P_GAS")

                ' Escribir encabezados
                worksheet.Cells(1, 1).Value = "CodigoContrato"
                worksheet.Cells(1, 2).Value = "SerieFactura"
                worksheet.Cells(1, 3).Value = "NumeroFactura"
                worksheet.Cells(1, 4).Value = "PrecioMedioTotal"
                worksheet.Row(1).Style.Font.Bold = True

                Dim currentRow As Integer = 2

                Using connection As New SqlConnection(connectionString)
                    connection.Open()

                    For Each codigo As Integer In CodigoContrato
                        Dim query As String = "
                    WITH FacturasVentaConsulta AS (
    SELECT 
        NumeroFactura, 
        CodigoContrato,
        ROW_NUMBER() OVER (PARTITION BY CodigoContrato ORDER BY NumeroFactura DESC) AS rn
    FROM FacturaVentaCabecera WITH (NOLOCK)
    WHERE CodigoContrato =@CodigoContrato
    AND SerieFactura = 'FGAS'
)
SELECT 
    fvc.CodigoContrato,
    fvc.SerieFactura,
    fvc.NumeroFactura,
    REPLACE(
        ISNULL(SUM(
            CASE 
                WHEN fvl.Facturaconcepto = 90066 
                THEN ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoTerminoFijoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0)
                ELSE ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0)
            END
        ), 0),
        '.', ','
    ) AS PrecioMedioTotal
FROM FacturaVentaCabecera fvc WITH (NOLOCK)
JOIN FacturasVentaConsulta fvcq ON fvc.NumeroFactura = fvcq.NumeroFactura AND fvcq.rn = 1
LEFT JOIN FacturaVentaLinea fvl WITH (NOLOCK) 
    ON fvl.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
    AND fvl.Facturaconcepto IN (90012, 90038, 90062, 90066, 90001) 
    AND fvl.codigoperiodoXML = 1
WHERE fvc.SerieFactura = 'FGAS'
GROUP BY fvc.CodigoContrato, fvc.SerieFactura, fvc.NumeroFactura;

                    "

                        Using command As New SqlCommand(query, connection)
                            command.Parameters.AddWithValue("@CodigoContrato", codigo)

                            Using reader As SqlDataReader = command.ExecuteReader()
                                While reader.Read()
                                    ' Escribir datos al Excel
                                    worksheet.Cells(currentRow, 1).Value = reader("CodigoContrato")
                                    worksheet.Cells(currentRow, 2).Value = reader("SerieFactura").ToString
                                    worksheet.Cells(currentRow, 3).Value = reader("NumeroFactura").ToString
                                    Dim precioMedio As Decimal
                                    If Decimal.TryParse(reader("PrecioMedioTotal").ToString(), precioMedio) Then
                                        worksheet.Cells(currentRow, 4).Value = precioMedio
                                        worksheet.Cells(currentRow, 4).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal
                                    Else
                                        worksheet.Cells(currentRow, 4).Value = 0 ' En caso de error, poner 0
                                    End If
                                    currentRow += 1
                                End While
                            End Using
                        End Using
                    Next
                End Using

                ' Guardar el archivo Excel
                package.Save()
            End Using

            'Console.WriteLine("Resultados guardados en " & filePath)
        Catch ex As Exception
            Throw
        End Try
    End Sub

End Class
