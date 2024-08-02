Imports System.Data.SqlClient
Imports System.IO
Imports System.Text

Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml.Spreadsheet
Imports ExcelDataReader
Imports OfficeOpenXml
Public Class ValidacionExcel

    Public ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Sub EjecutarConsultasYGuardarEnExcel(consultas As List(Of String), nombreArchivo As String)
        Try
            ' Creamos un nuevo libro de Excel
            Using excelWorkbook As New XLWorkbook()
                ' Para cada consulta en la lista
                For i As Integer = 0 To consultas.Count - 1
                    Dim consulta As String = consultas(i)

                    ' Crear una conexión a la base de datos
                    Using connection As New SqlConnection(connectionString)
                        ' Abrir la conexión
                        connection.Open()

                        ' Crear un comando SQL para ejecutar la consulta
                        Using command As New SqlCommand(consulta, connection)
                            ' Crear un lector de datos para leer los resultados de la consulta
                            Using reader As SqlDataReader = command.ExecuteReader()
                                ' Crear una nueva hoja de cálculo en el libro de Excel
                                Dim excelWorksheet As IXLWorksheet = excelWorkbook.Worksheets.Add("Hoja" & (i + 1).ToString())

                                ' Escribir los nombres de las columnas como encabezados de columna en la primera fila
                                Dim headerRow As Integer = 1
                                Dim columnNames As New HashSet(Of String)() ' Usaremos esto para mantener un registro de los nombres de columna únicos

                                For j As Integer = 0 To reader.FieldCount - 1
                                    Dim columnName As String = reader.GetName(j)

                                    ' Si el nombre de la columna ya existe, agregar un sufijo numérico para hacerlo único
                                    Dim uniqueColumnName As String = columnName
                                    Dim suffix As Integer = 2

                                    While columnNames.Contains(uniqueColumnName)
                                        uniqueColumnName = columnName & suffix
                                        suffix += 1
                                    End While

                                    columnNames.Add(uniqueColumnName)

                                    excelWorksheet.Cell(headerRow, j + 1).Value = uniqueColumnName
                                    excelWorksheet.Cell(headerRow, j + 1).Style.Font.Bold = True ' Negrita
                                    excelWorksheet.Cell(headerRow, j + 1).Style.Fill.BackgroundColor = XLColor.Gray ' Fondo de color (por ejemplo, Aqua)
                                Next

                                ' Escribir los datos en la hoja de cálculo
                                Dim row As Integer = headerRow + 1
                                While reader.Read()
                                    For j As Integer = 0 To reader.FieldCount - 1
                                        Dim value As Object = reader.GetValue(j)
                                        ' Eliminar los saltos de línea
                                        If TypeOf value Is String Then
                                            value = value.ToString().Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ")
                                        End If
                                        ' Convertir el valor a un tipo compatible con ClosedXML
                                        If TypeOf value Is DBNull Then
                                            value = "NULL" ' Tratar valores DBNull como "NULL"
                                        ElseIf TypeOf value Is Boolean Then
                                            ' Tratar campos booleanos como 1 o 0 en lugar de True o False
                                            value = If(DirectCast(value, Boolean), 1, 0)
                                        End If

                                        ' Asignar el valor a la celda como String
                                        excelWorksheet.Cell(row, j + 1).Value = If(value IsNot Nothing, value.ToString(), "")
                                    Next
                                    row += 1
                                End While

                                ' Crear una tabla con estilos automáticamente
                                Dim lastRow As Integer = row - 1 ' La última fila escrita
                                Dim lastColumn As Integer = reader.FieldCount ' El número de columnas
                                Dim rng As IXLRange = excelWorksheet.Range(excelWorksheet.Cell(headerRow, 1), excelWorksheet.Cell(lastRow, lastColumn))
                                Dim table = rng.CreateTable()

                                ' Aplicar estilos adicionales a la tabla si es necesario
                                ' Por ejemplo:
                                'table.Style.Border.OutsideBorder = XLBorderStyleValues.
                                table.Style.Border.OutsideBorderColor = XLColor.Black
                            End Using
                        End Using
                    End Using
                Next

                ' Guardar el libro de Excel en el archivo especificado
                excelWorkbook.SaveAs(nombreArchivo)
            End Using
        Catch ex As Exception
            MessageBox.Show("Error al ejecutar consultas y guardar en Excel: " & ex.Message)
        End Try
    End Sub


    Public Sub CSV(excelFilePath As String, outputCsvPath As String)
        Dim Excel As New Excel
        Dim Datos As New List(Of List(Of Object))()
        Dim Contador As Long = 0
        Try
            'Esto por que estoy usando una licencia no comercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            ' Abrir el archivo de Excel
            Dim funciones As New FuncionesGenericas(connectionString)
            Using package As New ExcelPackage(New FileInfo(excelFilePath))
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)
                Dim rowCount As Integer = worksheet.Dimension.Rows
                Using writer As New StreamWriter(outputCsvPath, False, Encoding.UTF8)
                    writer.WriteLine("CIFDNI;RazonSocial;Direccion;CodigoCUPS;CodPostal;Poblacion;Provincia;codigocontrato;SECTOR;PotContratadaP1;PotContratadaP2;PotContratadaP3;PotContratadaP4;PotContratadaP5;PotContratadaP6;Tarifa;Distribuidora;FechaFactura;ImporteTotal;textotipocobro;NumeroFactura;idfacturaorigen;FechaDesde;FechaHasta;PorcentajeIVA;BaseIVA;ImporteIVA")
                    ' Iterar sobre cada fila del archivo Excel

                    Using connection As New SqlConnection(connectionString)
                        connection.Open()
                        For row As Integer = 2 To rowCount
                            Dim id As String = worksheet.Cells(row, 1).Value?.ToString()
                            If Not String.IsNullOrEmpty(id) Then
                                Contador += 1
                                ' Consultar la base de datos para esta ID
#Region "Consulta"
                                Dim query As String = $"--Facturas VA SUEZ -- ErickCC
With 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) 

where IdFacturaVentaCabecera in ({id}
)),

ConsumosReactiva(id, r1,r2,r3,r4,r5,r6,pr1,pr2,pr3,pr4,pr5,pr6)
AS
(
	select facturaventacabecera.IdFacturaventaCabecera,
	ConsumoRP1.Consumo as ConsumoP1, ConsumoRP2.Consumo as ConsumoP2, ConsumoRP3.Consumo as ConsumoP3, ConsumoRP4.Consumo as ConsumoP4, ConsumoRP5.Consumo as ConsumoP5, ConsumoRP6.Consumo as ConsumoP6, 
	ConsumoRP1.Precio as EnergiaPrecioP1, ConsumoRP2.Precio as EnergiaPrecioP2, ConsumoRP3.Precio as EnergiaPrecioP3, ConsumoRP4.Precio as EnergiaPrecioP4, ConsumoRP5.Precio as EnergiaPrecioP5, ConsumoRP6.Precio as EnergiaPrecioP6
	
	from facturaventacabecera WITH (NOLOCK)
	--Consumos reactiva
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT FacturaVentaLinea.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   FacturaVentaLinea WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=1 group by l.Id, Consumo) as ConsumoRP1 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP1.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT FacturaVentaLinea.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   FacturaVentaLinea WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=2 group by l.Id, Consumo) as ConsumoRP2 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP2.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT FacturaVentaLinea.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   FacturaVentaLinea WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=3 group by l.Id, Consumo) as ConsumoRP3 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP3.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT FacturaVentaLinea.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   FacturaVentaLinea WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=4 group by l.Id, Consumo) as ConsumoRP4 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP4.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT FacturaVentaLinea.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   FacturaVentaLinea WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=5 group by l.Id, Consumo) as ConsumoRP5 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP5.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT FacturaVentaLinea.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   FacturaVentaLinea WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=6 group by l.Id, Consumo) as ConsumoRP6 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP6.Id
	WHERE facturaventacabecera.IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)),
PreseleccionContratos as (
select IdContrato, CodigoContrato, TipoImprimir, IdGrupoImprimir, IdModeloFactura, IdModeloFacturaGestinel, IdTarifa, IdCliente from Contrato 
where Entorno = 'E1' and Contrato.CodigoContrato in (select codigocontrato from FacturasVentaConsulta)
),
PreseleccionFacturas as (
select IdFacturaVentaCabecera, FacturaVentaCabecera.SerieFactura, NumeroFactura, FacturaCategoria, PreseleccionContratos.CodigoContrato, FechaFactura, FacturaVentaCabecera.IdFacturaTipo,facturaventacabecera.fechalecturaactualxml,facturaventacabecera.fechalecturaanteriorxml
,IdTipoImpuesto, PreseleccionContratos.IdTarifa,
IdFacturaRectificativa, IdFacturaAbono, IdFacturaOrigen, cast(isnull(InfoCabeceraXML.value('(//ConsumoActiva)[1]', 'nvarchar(max)'), '0') as decimal(18,2)) as Consumo, IsGestinel, IdContratoDocumento,
PreseleccionContratos.IdContrato, PreseleccionContratos.IdCliente, IdCanal, PreseleccionContratos.IdModeloFactura, PreseleccionContratos.IdModeloFacturaGestinel, FacturaTipo.TextoFacturaTipo
from PreseleccionContratos with(nolock)
inner join FacturaVentaCabecera on PreseleccionContratos.IdContrato = FacturaVentaCabecera.IdContrato 
left join FacturaTipo on FacturaTipo.IdFacturaTipo = FacturaVentaCabecera.IdFacturaTipo
where FacturaVentaCabecera.Entorno = 'E1' and IsFactura = 1
)
,
Lineas as (
select PreseleccionFacturas.IdFacturaVentaCabecera, FacturaConcepto, ImporteBase  from PreseleccionFacturas
inner join FacturaVentaLinea on FacturaVentaLinea.IdFacturaVentaCabecera = PreseleccionFacturas.IdFacturaVentaCabecera
),
ImportesPotencia as (
select Lineas.IdFacturaVentaCabecera, sum(Lineas.ImporteBase) as Importe from Lineas
where (FacturaConcepto between 10000 and 19999 or FacturaConcepto in (130001,130002, 131001)) or 
(FacturaConcepto in (90003, 90018, 90035,90036,90039,90040, 90042, 90043, 90044, 90045, 90049, 90050, 90053, 90054, 90056, 90057, 90058, 90059, 90070))
group by Lineas.IdFacturaVentaCabecera
),
ImportesEnergia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where (FacturaConcepto between 30000 and 39999 or FacturaConcepto in (130003,130004, 131003)) or 
(FacturaConcepto in (90001,90002,90012,90031,90032,90062,90038,90048,90052))
group by Lineas.IdFacturaVentaCabecera
),
ImportesReactiva as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where FacturaConcepto between 40000 and 49999 
group by Lineas.IdFacturaVentaCabecera
),
ImportesExcesos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where (FacturaConcepto between 20000 and 29999) or (FacturaConcepto in (90037,90041,90051,90055))
group by Lineas.IdFacturaVentaCabecera
),
DescuentosPotencia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where (FacturaConcepto in (120001, 120004)) or (FacturaConcepto in (120006, 90007))
group by Lineas.IdFacturaVentaCabecera
),
DescuentosEnergia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where (FacturaConcepto in (120002, 120005)) or (FacturaConcepto in (120007, 90006))
group by Lineas.IdFacturaVentaCabecera
),
ImportesAlquileres as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where (FacturaConcepto between 50000 and 59999) or (FacturaConcepto in (120007, 90006))
group by Lineas.IdFacturaVentaCabecera
),
ImportesProductos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where FacturaConcepto in (100001, 100002, 110001, 90009)
group by Lineas.IdFacturaVentaCabecera
)




Select 
--,CODIGOPROYECTO (VACIO)
cl.Identidad as CIFDNI
,dbo.formateardenominacion(cl.nombre,cl.apellido1,cl.apellido2,cl.razonsocial) as RazonSocial
--,DESCRIPCION (VACIO)
,cll.NombreCalle +' '+ cups.Aclarador as Direccion
,CUPS.CodigoCUPS
--,NUM CONTADOR (VACIO)
,cups.CodPostal
,ciu.TextoCiudad as Poblacion
,pv.TextoProvincia as Provincia
,c.codigocontrato
, case when c.Entorno ='E1' then 'Electricidad' else 'Gas' end Sector
,replace(cp1.PotenciaContratada,'.',',') as PotContratadaP1
,replace(cp2.PotenciaContratada,'.',',') as PotContratadaP2
,replace(cp3.PotenciaContratada,'.',',') as PotContratadaP3
,replace(cp4.PotenciaContratada,'.',',') as PotContratadaP4
,replace(cp5.PotenciaContratada,'.',',') as PotContratadaP5
,replace(cp6.PotenciaContratada,'.',',') as PotContratadaP6
,t.textotarifa as Tarifa
,d.NombreFiscal as Distribuidora
,fvc.FechaFactura
,replace(fvt.ImporteTotal,'.',',') As ImporteTotal
,tip.textotipocobro
,fvc.SerieFactura+' '+cast (fvc.NumeroFactura as varchar) As NumeroFactura
,fvc.idfacturaorigen
,convert(varchar, fvc.FechaLecturaAnteriorXML, 103) as FechaDesde
,convert(varchar,fvc.FechaLecturaActualXML, 103) as FechaHasta
,fvt.PorcentajeImpuesto As PorcentajeIVA
,replace(fvt.ImporteBase,'.',',') As BaseIVA
,replace(fvt.ImporteImpuesto,'.',',') As ImporteIVA


from  contrato c with (nolock)
inner join cliente cl with (nolock) on cl.idcliente = c.idcliente
inner join CUPS with (nolock) on c.idcups = cups.idcups
inner join callejero cll with (nolock) on cll.IdCallejero = cups.IdCallejero
inner join Ciudad ciu with (nolock) on ciu.idciudad = cups.idciudad
inner join provincia pv with (nolock) on ciu.idprovincia=pv.idprovincia
left join ContratoPotencia cp1 with (nolock) on cp1.idcontrato = c.idcontrato and cp1.IdTarifaPeriodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join ContratoPotencia cp2 with (nolock) on cp2.idcontrato = c.idcontrato and cp2.IdTarifaPeriodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join ContratoPotencia cp3 with (nolock) on cp3.idcontrato = c.idcontrato and cp3.IdTarifaPeriodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join ContratoPotencia cp4 with (nolock) on cp4.idcontrato = c.idcontrato and cp4.IdTarifaPeriodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join ContratoPotencia cp5 with (nolock) on cp5.idcontrato = c.idcontrato and cp5.IdTarifaPeriodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join ContratoPotencia cp6 with (nolock) on cp6.idcontrato = c.idcontrato and cp6.IdTarifaPeriodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join FacturaVentaCabecera fvc with (nolock) on (fvc.idcliente = c.idcliente or fvc.CodigoContrato = c.CodigoContrato)
left join FacturaVentaTotal fvt with (nolock) on fvc.idfacturaventacabecera = fvt.idfacturaventacabecera
left join CarteraCobro cc with (nolock) on fvc.idfacturaventacabecera = cc.idfacturaventacabecera
left join clientepago clp on c.idclientepago = clp.idclientepago
left join tipocobro tip on clp.idtipocobro = tip.idtipocobro
left join Tarifa t with (nolock) on c.idtarifa = t.IdTarifa
left join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
left join facturaventalinea fvlIE with (nolock) on fvc.idfacturaventacabecera = fvlIE.idfacturaventacabecera and fvlIE.Facturaconcepto=60001
left join facturaventalinea fvlCON with (nolock) on fvc.idfacturaventacabecera = fvlCON.idfacturaventacabecera and fvlCON.Facturaconcepto=50002
left join facturaventalinea fvlMAX with (nolock) on fvc.idfacturaventacabecera = fvlMAX.idfacturaventacabecera and fvlMAX.Facturaconcepto=20006


where fvc.idfacturaventacabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)

"
#End Region
                                'Using connection As New SqlConnection(connectionString)
                                'Using connection As New SqlConnection(connectionString)
                                '    connection.Open()

                                Dim command As New SqlCommand(query, connection)
                                command.Parameters.AddWithValue("@ID", id)

                                'connection.Open()
                                Using readerf As SqlDataReader = command.ExecuteReader()
                                    While readerf.Read()
                                        ' Escribir cada fila en el CSV
                                        writer.WriteLine($"{readerf("CIFDNI")};{readerf("RazonSocial")};{readerf("Direccion")};{readerf("CodigoCUPS")};{readerf("CodPostal")};{readerf("Poblacion")};{readerf("Provincia")};{readerf("codigocontrato")};{readerf("Sector")};{readerf("PotContratadaP1")};{readerf("PotContratadaP2")};{readerf("PotContratadaP3")};{readerf("PotContratadaP4")};{readerf("PotContratadaP5")};{readerf("PotContratadaP6")};{readerf("Tarifa")};{readerf("Distribuidora")};{readerf("FechaFactura")};{readerf("ImporteTotal")};{readerf("textotipocobro")};{readerf("NumeroFactura")};{readerf("idfacturaorigen")};{readerf("FechaDesde")};{readerf("FechaHasta")};{readerf("PorcentajeIVA")};{readerf("BaseIVA")};{readerf("ImporteIVA")}")
                                        'writer.WriteLine($"{readerf("Campo1")},{readerf("Campo2")},{readerf("Campo3")},...")
                                    End While
                                End Using
                                'End Using
                                Console.WriteLine($"Factura {Contador} _ Id -> {id}")
                            End If
                        Next
                        ' Conexión se cierra automáticamente al final del bloque Using
                    End Using

                End Using
            End Using
        Catch ex As Exception
            Throw
            ' Manejo de excepciones

            Throw
        End Try
    End Sub



    Public Sub GuardarEnExcelClick(fClicks As List(Of ClickFac), rutaArchivo As String)
        ' Crear un nuevo libro de Excel
        Dim workbook As New XLWorkbook()
        ' Agregar una hoja
        Dim worksheet = workbook.Worksheets.Add("Datos")

        ' Escribir los encabezados de las columnas
        worksheet.Cell(1, 1).Value = "NFactura"
        worksheet.Cell(1, 2).Value = "Descripcion"
        worksheet.Cell(1, 3).Value = "ImporteBaseSIGE"
        worksheet.Cell(1, 4).Value = "Periodo"
        worksheet.Cell(1, 5).Value = "Porcentaje"
        worksheet.Cell(1, 6).Value = "ConsumokWh"
        worksheet.Cell(1, 7).Value = "ClickCalculado"
        worksheet.Cell(1, 8).Value = "ImporteBaseCalculado"

        ' Escribir los datos de cada objeto ClickFac
        Dim row As Integer = 2 ' Comenzar desde la fila 2 para evitar la fila de encabezado
        For Each item In fClicks
            worksheet.Cell(row, 1).Value = item.NFactura
            worksheet.Cell(row, 2).Value = item.Descripcion
            worksheet.Cell(row, 3).Value = item.ImporteBase
            worksheet.Cell(row, 4).Value = item.Periodo
            worksheet.Cell(row, 5).Value = item.Porcentaje
            worksheet.Cell(row, 6).Value = item.ConsumokWh
            worksheet.Cell(row, 7).Value = item.ClickCalculado
            worksheet.Cell(row, 8).Value = item.ClickCalculado * item.ConsumokWh
            row += 1
        Next

        ' Guardar el archivo
        workbook.SaveAs(rutaArchivo)
        'Console.WriteLine($"Archivo Excel guardado en: {rutaArchivo}")
    End Sub



    'Public Sub EjecutarConsultasYGuardarEnExcelVariableGas(consultas As List(Of String), nombreArchivo As String)
    '    Try
    '        ' Creamos un nuevo libro de Excel
    '        Using excelWorkbook As New XLWorkbook()
    '            ' Para cada consulta en la lista
    '            For i As Integer = 0 To consultas.Count - 1
    '                Dim consulta As String = consultas(i)

    '                ' Crear una conexión a la base de datos
    '                Using connection As New SqlConnection(connectionString)
    '                    ' Abrir la conexión
    '                    connection.Open()

    '                    ' Crear un comando SQL para ejecutar la consulta
    '                    Using command As New SqlCommand(consulta, connection)
    '                        ' Crear un lector de datos para leer los resultados de la consulta
    '                        Using reader As SqlDataReader = command.ExecuteReader()
    '                            ' Crear una nueva hoja de cálculo en el libro de Excel
    '                            Dim excelWorksheet As IXLWorksheet = excelWorkbook.Worksheets.Add("Hoja" & (i + 1).ToString())

    '                            ' Escribir los nombres de las columnas como encabezados de columna en la primera fila
    '                            Dim headerRow As Integer = 1
    '                            Dim columnNames As New HashSet(Of String)() ' Usaremos esto para mantener un registro de los nombres de columna únicos

    '                            For j As Integer = 0 To reader.FieldCount - 1
    '                                Dim columnName As String = reader.GetName(j)

    '                                ' Si el nombre de la columna ya existe, agregar un sufijo numérico para hacerlo único
    '                                Dim uniqueColumnName As String = columnName
    '                                Dim suffix As Integer = 2

    '                                While columnNames.Contains(uniqueColumnName)
    '                                    uniqueColumnName = columnName & suffix
    '                                    suffix += 1
    '                                End While

    '                                columnNames.Add(uniqueColumnName)

    '                                excelWorksheet.Cell(headerRow, j + 1).Value = uniqueColumnName
    '                                excelWorksheet.Cell(headerRow, j + 1).Style.Font.Bold = True ' Negrita
    '                                excelWorksheet.Cell(headerRow, j + 1).Style.Fill.BackgroundColor = XLColor.Gray ' Fondo de color (por ejemplo, Aqua)
    '                            Next

    '                            ' Escribir los datos en la hoja de cálculo
    '                            Dim row As Integer = headerRow + 1
    '                            ' Escribir los datos en la hoja de cálculo
    '                            While reader.Read()
    '                                For j As Integer = 0 To reader.FieldCount - 1
    '                                    Dim value As Object = reader.GetValue(j)
    '                                    ' Eliminar los saltos de línea y convertir el valor a un tipo compatible con ClosedXML
    '                                    If TypeOf value Is String Then
    '                                        value = value.ToString().Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ")
    '                                    ElseIf TypeOf value Is DBNull Then
    '                                        value = "NULL" ' Tratar valores DBNull como "NULL"
    '                                    ElseIf TypeOf value Is Boolean Then
    '                                        ' Tratar campos booleanos como 1 o 0 en lugar de True o False
    '                                        value = If(DirectCast(value, Boolean), 1, 0)
    '                                    End If

    '                                    ' Si la columna es "Descripcion" o "ImporteBase"
    '                                    '' Si la columna es "Descripcion" o "ImporteBase"
    '                                    ' Variable para llevar un registro de la cantidad de columnas creadas para "Descripción"
    '                                    Dim descripcionColumnCount As Integer = 0

    '                                    ' ...

    '                                    ' Dentro del bucle For para leer los datos
    '                                    If reader.GetName(j) = "Descripcion" AndAlso value IsNot DBNull.Value Then
    '                                        Dim parts As String() = value.ToString().Split(New String() {"||"}, StringSplitOptions.None)
    '                                        Dim columnOffset As Integer = 0
    '                                        For Each part As String In parts
    '                                            columnOffset += 1
    '                                            Dim uniqueColumnName As String = "Descripcion_" & columnOffset.ToString()
    '                                            columnNames.Add(uniqueColumnName)
    '                                            excelWorksheet.Cell(headerRow, j + 1 + columnOffset).Value = uniqueColumnName
    '                                            excelWorksheet.Cell(row, j + 1 + columnOffset).Value = part
    '                                            descripcionColumnCount += 1 ' Incrementar la cuenta de columnas para "Descripción"
    '                                        Next
    '                                    ElseIf reader.GetName(j) = "ImporteBase" AndAlso value IsNot DBNull.Value Then
    '                                        Dim parts As String() = value.ToString().Split(New String() {"||"}, StringSplitOptions.None)
    '                                        For Each part As String In parts
    '                                            ' Empezar a escribir desde la siguiente columna después de la última columna creada para "Descripción"
    '                                            Dim columnIndex As Integer = j + descripcionColumnCount + 1
    '                                            excelWorksheet.Cell(headerRow, columnIndex).Value = "ImporteBase_" & columnIndex.ToString()
    '                                            excelWorksheet.Cell(row, columnIndex).Value = part
    '                                        Next
    '                                    End If

    '                                    'Else
    '                                    ' Asignar el valor a la celda como String
    '                                    excelWorksheet.Cell(row, j + 1).Value = If(value IsNot Nothing, value.ToString(), "")
    '                                    'End If


    '                                Next
    '                                row += 1
    '                            End While


    '                            ' Crear una tabla con estilos automáticamente
    '                            Dim lastRow As Integer = row - 1 ' La última fila escrita
    '                            Dim lastColumn As Integer = reader.FieldCount + columnNames.Count ' El número de columnas
    '                            Dim rng As IXLRange = excelWorksheet.Range(excelWorksheet.Cell(headerRow, 1), excelWorksheet.Cell(lastRow, lastColumn))
    '                            Dim table = rng.CreateTable()

    '                            ' Aplicar estilos adicionales a la tabla si es necesario
    '                            ' Por ejemplo:
    '                            'table.Style.Border.OutsideBorder = XLBorderStyleValues.
    '                            table.Style.Border.OutsideBorderColor = XLColor.Black
    '                        End Using
    '                    End Using
    '                End Using
    '            Next

    '            ' Guardar el libro de Excel en el archivo especificado
    '            excelWorkbook.SaveAs(nombreArchivo)
    '        End Using
    '    Catch ex As Exception
    '        MessageBox.Show("Error al ejecutar consultas y guardar en Excel: " & ex.Message)
    '    End Try
    'End Sub

End Class
