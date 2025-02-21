Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml.Office2010.Excel
Imports DocumentFormat.OpenXml.Spreadsheet
Imports DocumentFormat.OpenXml.Wordprocessing
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
        Dim Condition = "fvc.CodigoContrato = c.CodigoContrato"
        Try
            'Esto por que estoy usando una licencia no comercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            ' Abrir el archivo de Excel
            Dim funciones As New FuncionesGenericas(connectionString)
            Using package As New ExcelPackage(New FileInfo(excelFilePath))
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)
                Dim rowCount As Integer = worksheet.Dimension.Rows
                Using writer As New StreamWriter(outputCsvPath, False, Encoding.UTF8)
                    writer.WriteLine("CIFDNI;RazonSocial;Direccion;CodigoCUPS;CodPostal;Poblacion;Provincia;codigocontrato;SECTOR;PotContratadaP1;PotContratadaP2;PotContratadaP3;PotContratadaP4;PotContratadaP5;PotContratadaP6;Tarifa;Distribuidora;FechaFactura;ImporteTotal;textotipocobro;NumeroFactura;idfacturaorigen;FechaDesde;FechaHasta;ImporteElectrico;ImporteClick;PorcentajeIVA;BaseIVA;ImporteIVA")
                    ' Iterar sobre cada fila del archivo Excel

                    Using connection As New SqlConnection(connectionString)
                        connection.Open()
                        For row As Integer = 2 To rowCount
                            Dim id As String = worksheet.Cells(row, 1).Value?.ToString()
                            If Not String.IsNullOrEmpty(id) Then
                                Contador += 1

                                Dim F = funciones.GetFacVenta(id)
                                If Not IsNothing(F) AndAlso Not IsNothing(F.CodigoContrato) AndAlso F.CodigoContrato > 0 Then
                                    Condition = "fvc.CodigoContrato = c.CodigoContrato"
                                Else
                                    Condition = "fvc.idcliente = cl.idcliente"
                                End If
                                ' Consultar la base de datos para esta ID
#Region "Consulta"
                                Dim query As String = $"--Facturas VA SUEZ -- ErickCC
With 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) 

where IdFacturaVentaCabecera in ({id}
)),

 PotenciaReemplazadaP1 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp1.PotenciaContratada, '.', ',') AS PotContratadaP1
    FROM contrato c
left join ContratoPotencia cp1 with (nolock) on cp1.idcontrato = c.idcontrato and cp1.IdTarifaPeriodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)

)

, PotenciaReemplazadaP2 AS (
    SELECT 
		c.idcontrato,
        REPLACE(cp2.PotenciaContratada, '.', ',') AS PotContratadaP2

    FROM contrato c
left join ContratoPotencia cp2 with (nolock) on cp2.idcontrato = c.idcontrato and cp2.IdTarifaPeriodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)

)
, PotenciaReemplazadaP3 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp3.PotenciaContratada, '.', ',') AS PotContratadaP3
    FROM contrato c
left join ContratoPotencia cp3 with (nolock) on cp3.idcontrato = c.idcontrato and cp3.IdTarifaPeriodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
)
, PotenciaReemplazadaP4 AS (
    SELECT 
        c.idcontrato,

        REPLACE(cp4.PotenciaContratada, '.', ',') AS PotContratadaP4
    FROM contrato c
left join ContratoPotencia cp4 with (nolock) on cp4.idcontrato = c.idcontrato and cp4.IdTarifaPeriodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)

)
, PotenciaReemplazadaP5 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp5.PotenciaContratada, '.', ',') AS PotContratadaP5
    FROM contrato c
left join ContratoPotencia cp5 with (nolock) on cp5.idcontrato = c.idcontrato and cp5.IdTarifaPeriodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)

)
, PotenciaReemplazadaP6 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp6.PotenciaContratada, '.', ',') AS PotContratadaP6
    FROM contrato c
left join ContratoPotencia cp6 with (nolock) on cp6.idcontrato = c.idcontrato and cp6.IdTarifaPeriodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
)

,ProductoClick as (
select IdFacturaVentaCabecera, sum(ImporteBase) ImporteClick from facturaventalinea where
Descripcion = 'Ajuste término productos click'
group by IdFacturaVentaCabecera
)
,ProductoElectrico as (
select IdFacturaVentaCabecera, sum(ImporteBase)  ImporteElectrico from facturaventalinea where
Descripcion = 'Impuesto Electricidad'
group by IdFacturaVentaCabecera
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
, pr.PotContratadaP1
, pr2.PotContratadaP2
, pr3.PotContratadaP3
, pr4.PotContratadaP4
, pr5.PotContratadaP5
, pr6.PotContratadaP6
,t.textotarifa as Tarifa
,d.NombreFiscal as Distribuidora
,fvc.FechaFactura
,replace(fvt.ImporteTotal,'.',',') As ImporteTotal
,tip.textotipocobro
,fvc.SerieFactura+' '+cast (fvc.NumeroFactura as varchar) As NumeroFactura
,fvc.idfacturaorigen
,convert(varchar, fvc.FechaLecturaAnteriorXML, 103) as FechaDesde
,convert(varchar,fvc.FechaLecturaActualXML, 103) as FechaHasta
,replace(ProductoElectrico.ImporteElectrico,'.',',') ImporteElectrico
,replace(ProductoClick.ImporteClick,'.',',') ImporteClick
,fvt.PorcentajeImpuesto As PorcentajeIVA
,replace(fvt.ImporteBase,'.',',') As BaseIVA
,replace(fvt.ImporteImpuesto,'.',',') As ImporteIVA


from  contrato c with (nolock)
inner join cliente cl with (nolock) on cl.idcliente = c.idcliente
inner join CUPS with (nolock) on c.idcups = cups.idcups
inner join callejero cll with (nolock) on cll.IdCallejero = cups.IdCallejero
inner join Ciudad ciu with (nolock) on ciu.idciudad = cups.idciudad
inner join provincia pv with (nolock) on ciu.idprovincia=pv.idprovincia
LEFT JOIN PotenciaReemplazadaP1 pr  ON pr.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP2 pr2 ON pr2.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP3 pr3 ON pr3.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP4 pr4 ON pr4.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP5 pr5 ON pr5.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP6 pr6 ON pr6.idcontrato = c.idcontrato
LEFT JOIN FacturaVentaCabecera fvc WITH (NOLOCK) ON {Condition}
left join FacturaVentaTotal fvt with (nolock) on fvc.idfacturaventacabecera = fvt.idfacturaventacabecera
left join ProductoClick on fvc.IdFacturaVentaCabecera = ProductoClick.IdFacturaVentaCabecera
left join ProductoElectrico on fvc.IdFacturaVentaCabecera = ProductoElectrico.IdFacturaVentaCabecera
left join CarteraCobro cc with (nolock) on fvc.idfacturaventacabecera = cc.idfacturaventacabecera
left join clientepago clp on c.idclientepago = clp.idclientepago
left join tipocobro tip on clp.idtipocobro = tip.idtipocobro
left join Tarifa t with (nolock) on c.idtarifa = t.IdTarifa
left join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
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
                                        writer.WriteLine($"{readerf("CIFDNI")};{readerf("RazonSocial")};{readerf("Direccion")};{readerf("CodigoCUPS")};{readerf("CodPostal")};{readerf("Poblacion")};{readerf("Provincia")};{readerf("codigocontrato")};{readerf("Sector")};{readerf("PotContratadaP1")};{readerf("PotContratadaP2")};{readerf("PotContratadaP3")};{readerf("PotContratadaP4")};{readerf("PotContratadaP5")};{readerf("PotContratadaP6")};{readerf("Tarifa")};{readerf("Distribuidora")};{readerf("FechaFactura")};{readerf("ImporteTotal")};{readerf("textotipocobro")};{readerf("NumeroFactura")};{readerf("idfacturaorigen")};{readerf("FechaDesde")};{readerf("FechaHasta")};{readerf("ImporteElectrico")};{readerf("ImporteClick")};{readerf("PorcentajeIVA")};{readerf("BaseIVA")};{readerf("ImporteIVA")}")
                                        'writer.WriteLine($"{readerf("Campo1")},{readerf("Campo2")},{readerf("Campo3")},...")
                                    End While
                                End Using
                                'End Using
                                'Console.WriteLine($"Factura {Contador} _ Id -> {id}")
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


    Public Sub CSV2(excelFilePath As String, outputCsvPath As String)
        Dim Excel As New Excel
        Dim Datos As New List(Of List(Of Object))()
        Dim Contador As Long = 0
        Dim Condition = "fvc.CodigoContrato = c.CodigoContrato"
        Dim ListaIds As New List(Of Long)
        Dim ListaIdsCl As New List(Of Long)
        Dim ListaIdInicial As New List(Of Long)
        Try
            'Esto por que estoy usando una licencia no comercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            ' Abrir el archivo de Excel
            Dim funciones As New FuncionesGenericas(connectionString)
            Using package As New ExcelPackage(New FileInfo(excelFilePath))
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)
                Dim rowCount As Integer = worksheet.Dimension.Rows
                'Se lee el excel y nos guardamos las ids en una lista
                For row As Integer = 2 To rowCount
                    Dim id As String = worksheet.Cells(row, 1).Value?.ToString()
                    ListaIdInicial.Add(id)
                Next
                'Nos obtenemos las facturas segun las ids leidas
                Dim ListaF = funciones.GetFacVentaLista(String.Join(",", ListaIdInicial))
                For Each L In ListaF
                    'La fac puedes tener o no codigocontrato, por lo que las dividimos, por un lado las que sí tienen y por otro las que no, pero si tienen idcliente
                    If Not IsNothing(L) AndAlso Not IsNothing(L.CodigoContrato) AndAlso L.CodigoContrato > 0 Then
                        ListaIds.Add(L.IdFacturaVentaCabecera)
                    Else
                        ListaIdsCl.Add(L.IdFacturaVentaCabecera)
                    End If
                Next
                'Preparamos las ids
                Dim joinIdC = String.Join(",", ListaIds)
                Dim joinIdCl = String.Join(",", ListaIdsCl)

                Dim query = ""
                Dim query2 = ""
                'Si hay datos, le pasamos la ids con sí tienen codigocontrato, y la condición que ira en left join de para la query, si 
                'tiene codigocontrato le pasamos la condicion del codigocontrato y las facturas que tienen codigocontrato
                If joinIdC.Length > 5 Then
                    query = PrepararQuery(joinIdC, "fvc.codigocontrato = c.codigocontrato")
                End If
                'Si hay datos, le pasamos la ids que no tienen codcontrato, pero si idcliente, y la condición que ira en left join de para la query
                'Idcliente
                If joinIdCl.Length > 5 Then
                    query2 = PrepararQuery(joinIdCl, "fvc.idcliente = cl.idcliente")
                End If

                Dim ResultF As New List(Of FacsCSV)
                'Una vez preparada la consulta, le pasamos la query, y los resultados se añaden a la lista de ResultF
                'Query para las facs que tienen codigocontrato
                If joinIdC.Length > 5 Then
                    ResultF.AddRange(ResultadoQuery(query, connectionString))
                End If
                'Query para las facs que tienen idcliente
                If joinIdCl.Length > 5 Then
                    ResultF.AddRange(ResultadoQuery(query2, connectionString))
                End If
                'Escribo en el csv los resultados
                Using writer As New StreamWriter(outputCsvPath, False, Encoding.UTF8)
                    writer.WriteLine("CIFDNI;RazonSocial;Direccion;CodigoCUPS;CodPostal;Poblacion;Provincia;codigocontrato;SECTOR;PotContratadaP1;PotContratadaP2;PotContratadaP3;PotContratadaP4;PotContratadaP5;PotContratadaP6;Tarifa;Distribuidora;FechaFactura;ImporteTotal;textotipocobro;NumeroFactura;idfacturaorigen;FechaDesde;FechaHasta;ImporteElectrico;ImporteClick;PorcentajeIVA;BaseIVA;ImporteIVA")
                    For Each r In ResultF
                        Contador += 1
                        writer.WriteLine($"{r.CIFDNI};{r.RazonSocial};{r.Direccion};{r.CodigoCUPS};{r.CodPostal};{r.Poblacion};{r.Provincia};{r.CodigoContrato};{r.Sector};{r.PotContratadaP1};{r.PotContratadaP2};{r.PotContratadaP3};{r.PotContratadaP4};{r.PotContratadaP5};{r.PotContratadaP6};{r.Tarifa};{r.Distribuidora};{r.FechaFactura};{r.ImporteTotal};{r.TextoTipoCobro};{r.NumeroFactura};{r.idfacturaorigen};{r.FechaDesde};{r.FechaHasta};{r.ImporteElectrico};{r.ImporteClick};{r.PorcentajeIVA};{r.BaseIVA};{r.ImporteIVA}")
                    Next
                    ' Conexión se cierra automáticamente al final del bloque Using
                End Using
            End Using
        Catch ex As Exception
            Throw

        End Try
    End Sub


    Private Function PrepararQuery(id As String, condition As String) As String
#Region "Consulta"
        Dim query As String = $"--Facturas VA SUEZ -- ErickCC
With 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) 

where IdFacturaVentaCabecera in ({id}
)),

 PotenciaReemplazadaP1 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp1.PotenciaContratada, '.', ',') AS PotContratadaP1
    FROM contrato c
left join ContratoPotencia cp1 with (nolock) on cp1.idcontrato = c.idcontrato and cp1.IdTarifaPeriodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)

)

, PotenciaReemplazadaP2 AS (
    SELECT 
		c.idcontrato,
        REPLACE(cp2.PotenciaContratada, '.', ',') AS PotContratadaP2

    FROM contrato c
left join ContratoPotencia cp2 with (nolock) on cp2.idcontrato = c.idcontrato and cp2.IdTarifaPeriodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)

)
, PotenciaReemplazadaP3 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp3.PotenciaContratada, '.', ',') AS PotContratadaP3
    FROM contrato c
left join ContratoPotencia cp3 with (nolock) on cp3.idcontrato = c.idcontrato and cp3.IdTarifaPeriodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
)
, PotenciaReemplazadaP4 AS (
    SELECT 
        c.idcontrato,

        REPLACE(cp4.PotenciaContratada, '.', ',') AS PotContratadaP4
    FROM contrato c
left join ContratoPotencia cp4 with (nolock) on cp4.idcontrato = c.idcontrato and cp4.IdTarifaPeriodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)

)
, PotenciaReemplazadaP5 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp5.PotenciaContratada, '.', ',') AS PotContratadaP5
    FROM contrato c
left join ContratoPotencia cp5 with (nolock) on cp5.idcontrato = c.idcontrato and cp5.IdTarifaPeriodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)

)
, PotenciaReemplazadaP6 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp6.PotenciaContratada, '.', ',') AS PotContratadaP6
    FROM contrato c
left join ContratoPotencia cp6 with (nolock) on cp6.idcontrato = c.idcontrato and cp6.IdTarifaPeriodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
)

,ProductoClick as (
select IdFacturaVentaCabecera, sum(ImporteBase) ImporteClick from facturaventalinea where
Descripcion = 'Ajuste término productos click'
group by IdFacturaVentaCabecera
)
,ProductoElectrico as (
select IdFacturaVentaCabecera, sum(ImporteBase)  ImporteElectrico from facturaventalinea where
Descripcion = 'Impuesto Electricidad'
group by IdFacturaVentaCabecera
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
, pr.PotContratadaP1
, pr2.PotContratadaP2
, pr3.PotContratadaP3
, pr4.PotContratadaP4
, pr5.PotContratadaP5
, pr6.PotContratadaP6
,t.textotarifa as Tarifa
,d.NombreFiscal as Distribuidora
,fvc.FechaFactura
,replace(fvt.ImporteTotal,'.',',') As ImporteTotal
,tip.textotipocobro
,fvc.SerieFactura+' '+cast (fvc.NumeroFactura as varchar) As NumeroFactura
,fvc.idfacturaorigen
,convert(varchar, fvc.FechaLecturaAnteriorXML, 103) as FechaDesde
,convert(varchar,fvc.FechaLecturaActualXML, 103) as FechaHasta
,replace(ProductoElectrico.ImporteElectrico,'.',',') ImporteElectrico
,replace(ProductoClick.ImporteClick,'.',',') ImporteClick
,fvt.PorcentajeImpuesto As PorcentajeIVA
,replace(fvt.ImporteBase,'.',',') As BaseIVA
,replace(fvt.ImporteImpuesto,'.',',') As ImporteIVA


from  contrato c with (nolock)
inner join cliente cl with (nolock) on cl.idcliente = c.idcliente
inner join CUPS with (nolock) on c.idcups = cups.idcups
inner join callejero cll with (nolock) on cll.IdCallejero = cups.IdCallejero
inner join Ciudad ciu with (nolock) on ciu.idciudad = cups.idciudad
inner join provincia pv with (nolock) on ciu.idprovincia=pv.idprovincia
LEFT JOIN PotenciaReemplazadaP1 pr  ON pr.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP2 pr2 ON pr2.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP3 pr3 ON pr3.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP4 pr4 ON pr4.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP5 pr5 ON pr5.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP6 pr6 ON pr6.idcontrato = c.idcontrato
LEFT JOIN FacturaVentaCabecera fvc WITH (NOLOCK) ON {condition}
left join FacturaVentaTotal fvt with (nolock) on fvc.idfacturaventacabecera = fvt.idfacturaventacabecera
left join ProductoClick on fvc.IdFacturaVentaCabecera = ProductoClick.IdFacturaVentaCabecera
left join ProductoElectrico on fvc.IdFacturaVentaCabecera = ProductoElectrico.IdFacturaVentaCabecera
left join CarteraCobro cc with (nolock) on fvc.idfacturaventacabecera = cc.idfacturaventacabecera
left join clientepago clp on c.idclientepago = clp.idclientepago
left join tipocobro tip on clp.idtipocobro = tip.idtipocobro
left join Tarifa t with (nolock) on c.idtarifa = t.IdTarifa
left join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
where fvc.idfacturaventacabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
"
#End Region
        Return query
    End Function

    Private Function ResultadoQuery(Query As String, connectionString2 As String) As List(Of FacsCSV)
        Dim ResultFac As New List(Of FacsCSV)
        Try
            Dim Result = Helper.QuerySelect(Query, connectionString2)
            Dim errores = Helper.GetError(Result)
            If Not errores.HasError Then
                'Escribir errores en un log'

                Dim Fac = Helper.FillObjectFromDatatable(Result.Tables(0), GetType(FacsCSV)).Cast(Of FacsCSV).ToList
                If Not IsNothing(Fac) AndAlso Fac.Count > 1 Then
                    ResultFac = Fac
                End If
                Dim pepe = 0
            End If
        Catch ex As Exception

        End Try
        Return ResultFac
    End Function


    Public Sub CSV3(excelFilePath As String, outputCsvPath As String)
        Dim Excel As New Excel
        Dim Contador As Long = 0
        Dim funciones As New FuncionesGenericas(connectionString)
        Dim ListaIdInicial As New List(Of Long)

        Try
            ' Esto porque estoy usando una licencia no comercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            ' Abrir el archivo de Excel
            Using package As New ExcelPackage(New FileInfo(excelFilePath))
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)
                Dim rowCount As Integer = worksheet.Dimension.Rows

                ' Leer el Excel y almacenar las IDs en una lista
                For row As Integer = 2 To rowCount
                    Dim id As String = worksheet.Cells(row, 1).Value?.ToString()
                    ListaIdInicial.Add(id)
                Next
            End Using

            ' Obtener las facturas según las IDs leídas
            Dim ListaF = funciones.GetFacVentaLista(String.Join(",", ListaIdInicial))

            ' Dividir las facturas según si tienen o no `CodigoContrato`
            Dim ListaIds = ListaF.Where(Function(L) If(L.CodigoContrato, 0) > 0).Select(Function(L) L.IdFacturaVentaCabecera).ToList()
            Dim ListaIdsCl = ListaF.Where(Function(L) If(L.CodigoContrato, 0) <= 0).Select(Function(L) L.IdFacturaVentaCabecera).ToList()

            ' Inicializar una lista para almacenar los resultados
            Dim ResultF As New List(Of FacsCSV)

            ' Si hay IDs en `ListaIds`, ejecutar la consulta para facturas con `CodigoContrato`
            If ListaIds.Any() Then
                ResultF.AddRange(GenerarConsulta(ListaIds, "fvc.codigocontrato = c.codigocontrato"))
            End If

            ' Si hay IDs en `ListaIdsCl`, ejecutar la consulta para facturas con `IdCliente`
            If ListaIdsCl.Any() Then
                ResultF.AddRange(GenerarConsulta(ListaIdsCl, "fvc.idcliente = cl.idcliente"))
            End If

            ' Escribir los resultados en el CSV
            Using writer As New StreamWriter(outputCsvPath, False, Encoding.UTF8)
                writer.WriteLine("CIFDNI;RazonSocial;Direccion;CodigoCUPS;CodPostal;Poblacion;Provincia;codigocontrato;SECTOR;PotContratadaP1;PotContratadaP2;PotContratadaP3;PotContratadaP4;PotContratadaP5;PotContratadaP6;Tarifa;Distribuidora;FechaFactura;ImporteTotal;textotipocobro;NumeroFactura;idfacturaorigen;FechaDesde;FechaHasta;ImporteElectrico;ImporteClick;PorcentajeIVA;BaseIVA;ImporteIVA")
                For Each r In ResultF
                    Contador += 1
                    writer.WriteLine($"{r.CIFDNI};{r.RazonSocial};{r.Direccion};{r.CodigoCUPS};{r.CodPostal};{r.Poblacion};{r.Provincia};{r.CodigoContrato};{r.Sector};{r.PotContratadaP1};{r.PotContratadaP2};{r.PotContratadaP3};{r.PotContratadaP4};{r.PotContratadaP5};{r.PotContratadaP6};{r.Tarifa};{r.Distribuidora};{r.FechaFactura};{r.ImporteTotal};{r.TextoTipoCobro};{r.NumeroFactura};{r.idfacturaorigen};{r.FechaDesde};{r.FechaHasta};{r.ImporteElectrico};{r.ImporteClick};{r.PorcentajeIVA};{r.BaseIVA};{r.ImporteIVA}")
                Next
            End Using

        Catch ex As Exception
            Throw
        End Try
    End Sub

    ' Función para generar las consultas según las IDs y la condición
    Private Function GenerarConsulta(ListaIds As List(Of Long), condicion As String) As List(Of FacsCSV)
        If ListaIds.Count = 0 Then Return New List(Of FacsCSV)()
        Dim query As String = PrepararQuery(String.Join(",", ListaIds), condicion)
        Return ResultadoQuery(query, connectionString)
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
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 1 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',') AS SumaPrecioP1,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 2 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 2 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 2 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',') AS SumaPrecioP2,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 3 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 3 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 3 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',') AS SumaPrecioP3,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 4 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 4 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 4 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',') AS SumaPrecioP4,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 5 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 5 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 5 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',') AS SumaPrecioP5,

     REPLACE(ISNULL(MAX(CASE WHEN fvl.codigoperiodoXML = 6 THEN  ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlb.codigoperiodoXML = 6 THEN  ISNULL(fvlb.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0) +
			ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 6 THEN  ISNULL(fvlc.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',') AS SumaPrecioP6,

    -- Consumos de cada periodo
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 1 THEN 
        ISNULL(fvlc.TotConsumoEnergiaXML, 0)
        ELSE NULL END), 0), '.', ',') AS ConsumoP1,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 2 THEN 
        ISNULL(fvlc.TotConsumoEnergiaXML, 0)
        ELSE NULL END), 0), '.', ',') AS ConsumoP2,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 3 THEN 
        ISNULL(fvlc.TotConsumoEnergiaXML, 0)
        ELSE NULL END), 0), '.', ',') AS ConsumoP3,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 4 THEN 
        ISNULL(fvlc.TotConsumoEnergiaXML, 0)
        ELSE NULL END), 0), '.', ',') AS ConsumoP4,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 5 THEN 
        ISNULL(fvlc.TotConsumoEnergiaXML, 0)
        ELSE NULL END), 0), '.', ',') AS ConsumoP5,
    REPLACE(ISNULL(MAX(CASE WHEN fvlc.codigoperiodoXML = 6 THEN 
        ISNULL(fvlc.TotConsumoEnergiaXML, 0)
        ELSE NULL END), 0), '.', ',') AS ConsumoP6

FROM FacturaVentaCabecera fvc WITH (NOLOCK)
LEFT JOIN FacturaVentaLinea fvl WITH (NOLOCK) ON fvl.idfacturaventacabecera = fvc.idfacturaventacabecera AND fvl.Facturaconcepto IN (130004)
LEFT JOIN FacturaVentaLinea fvlb WITH (NOLOCK) ON fvlb.idfacturaventacabecera = fvc.idfacturaventacabecera AND fvlb.Facturaconcepto IN (30001)
LEFT JOIN FacturaVentaLinea fvlc WITH (NOLOCK) ON fvlc.idfacturaventacabecera = fvc.idfacturaventacabecera AND fvlc.Facturaconcepto = 30002

WHERE fvc.IdFacturaVentaCabecera IN (
    SELECT idfacturaventacabecera 
    FROM FacturaVentaCabecera WITH (NOLOCK) 
    WHERE NumeroFactura IN (SELECT NumeroFactura FROM FacturasVentaConsulta WITH (NOLOCK))
    AND SerieFactura LIKE 'FELEC'
)
GROUP BY 
    fvc.CodigoContrato,
    fvc.SerieFactura,
    fvc.NumeroFactura;

"

                        ' Ejecutar consulta
                        Dim command As New SqlCommand(query, Connection)
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
                    WITH FacturasVentaConsulta (NumeroFactura, CodigoContrato) AS (
                        SELECT MAX(NumeroFactura), CodigoContrato 
                        FROM FacturaVentaCabecera WITH (NOLOCK) 
                        WHERE CodigoContrato = @CodigoContrato
                        AND SerieFactura IN ('FGAS')
                        GROUP BY CodigoContrato
                    )
                    SELECT 
                        fvc.CodigoContrato,
                        fvc.SerieFactura,
                        fvc.NumeroFactura,
                        REPLACE(
                            ISNULL(
                                ISNULL(fvl1.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0) +
                                ISNULL(fvl2.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0) +
                                ISNULL(fvl3.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0) +
                                ISNULL(fvl4.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoTerminoFijoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0) +
                                ISNULL(fvl5.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0), 
                            0), '.', ',') AS PrecioMedioTotal
                    FROM FacturaVentaCabecera fvc WITH (NOLOCK)
                    LEFT JOIN FacturaVentaLinea fvl1 WITH (NOLOCK) ON fvl1.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera AND fvl1.FacturaConcepto IN (90012) AND fvl1.CodigoPeriodoXML = 1
                    LEFT JOIN FacturaVentaLinea fvl2 WITH (NOLOCK) ON fvl2.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera AND fvl2.FacturaConcepto IN (90038) AND fvl2.CodigoPeriodoXML = 1
                    LEFT JOIN FacturaVentaLinea fvl3 WITH (NOLOCK) ON fvl3.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera AND fvl3.FacturaConcepto IN (90062) AND fvl3.CodigoPeriodoXML = 1
                    LEFT JOIN FacturaVentaLinea fvl4 WITH (NOLOCK) ON fvl4.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera AND fvl4.FacturaConcepto IN (90066) AND fvl4.CodigoPeriodoXML = 1
                    LEFT JOIN FacturaVentaLinea fvl5 WITH (NOLOCK) ON fvl5.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera AND fvl5.FacturaConcepto IN (90001) AND fvl5.CodigoPeriodoXML = 1
                    WHERE fvc.IdFacturaVentaCabecera IN (
                        SELECT IdFacturaVentaCabecera 
                        FROM FacturaVentaCabecera 
                        WHERE NumeroFactura IN (SELECT NumeroFactura FROM FacturasVentaConsulta) 
                        AND SerieFactura LIKE 'FGAS'
                    )
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

    Sub ConsultaTopLidia(NumFacsCompra As List(Of String), Ubicacion As String)
        Try
            ' Crear un nuevo archivo Excel
            Dim filePath As String = Ubicacion
            Dim fileInfo As New FileInfo(filePath)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Using package As New ExcelPackage(fileInfo)
                Dim worksheet = package.Workbook.Worksheets.Add("Consulta")

                ' Escribir encabezados
                worksheet.Cells(1, 1).Value = "IdFacturaVentaCabecera"
                worksheet.Cells(1, 2).Value = "NumerofacturaCompra"
                worksheet.Cells(1, 3).Value = "SerieFactura"
                worksheet.Cells(1, 4).Value = "NumerofacturaVenta"
                worksheet.Cells(1, 5).Value = "CodigoContrato"
                worksheet.Cells(1, 6).Value = "TipoAu"
                worksheet.Cells(1, 7).Value = "LineasClick"
                worksheet.Cells(1, 8).Value = "Autoconsumo"
                worksheet.Cells(1, 9).Value = "ImporteAutoconsumo"
                worksheet.Cells(1, 10).Value = "ImporteTotalClick"
                worksheet.Cells(1, 11).Value = "ProductosAutoconsumo"
                worksheet.Cells(1, 12).Value = "LineasAutoconsumo"
                worksheet.Cells(1, 13).Value = "CO"
                worksheet.Cells(1, 14).Value = "TerminoEnergiaTarifaMLP1"
                worksheet.Cells(1, 15).Value = "TerminoEnergiaTarifaMLP2"
                worksheet.Cells(1, 16).Value = "TerminoEnergiaTarifaMLP3"
                worksheet.Cells(1, 17).Value = "TerminoEnergiaTarifaMLP4"
                worksheet.Cells(1, 18).Value = "TerminoEnergiaTarifaMLP5"
                worksheet.Cells(1, 19).Value = "TerminoEnergiaTarifaMLP6"
                worksheet.Cells(1, 20).Value = "COinterno"
                worksheet.Cells(1, 21).Value = "FechaContrato"
                worksheet.Cells(1, 22).Value = "FechaAplicacionPrecios"
                worksheet.Row(1).Style.Font.Bold = True

                Dim currentRow As Integer = 2

                ' Cadena de conexión
                ' Dim connectionString As String = "Server=TU_SERVIDOR;Database=TU_BASE_DE_DATOS;Trusted_Connection=True;"



                For Each FacCompra As String In NumFacsCompra
                    Using connection As New SqlConnection(connectionString)
                        connection.Open()
                        ' Consulta SQL que recupera los datos
                        Dim query As String = "
                    with XMLNAMESPACES('http://localhost/elegibilidad' as ""XS"") 

,facturas as (select idfacturacompracabecera from FacturaCompraCabecera with (nolock) where NumeroFactura =@NumFacCompra
)

,TipoAuto as (select idfacturacompracabecera,isnull(FacturaXML.value('(//XS:DatosFacturaATR/XS:TipoAutoconsumo)[1]', 'nvarchar(max)'),0) as TipoAu 
from FacturaCompraCabecera
where IdFacturaCompraCabecera in (select IdFacturaCompraCabecera from facturas))

,FacturasEnergiaML as (select fvl.idfacturaventacabecera
,CodigoPeriodoXML
,infolineaxml   
from
FacturaCompraCabecera fcc
inner join facturas with (nolock) on facturas.IdFacturaCompraCabecera = fcc.IdFacturaCompraCabecera
inner join lectura l with (nolock) on l.IdFacturaCompraCabecera = fcc.IdFacturaCompraCabecera or l.IdFacturaCompraCabecera = fcc.idfacturaorigen
inner join facturaventacabecera fvc with (nolock) on fvc.idfacturaventacabecera = l.idfacturaventacabecerasectorc
inner join FacturaVentaLinea fvl with (nolock) on  fvl.idfacturaventacabecera =fvc.IdFacturaVentaCabecera and fvl.FacturaConcepto=30002)

select fvc.IdFacturaVentaCabecera
,fcc.Numerofactura NumerofacturaC
,fvc.SerieFactura
,fvc.NumeroFactura
,fcc.CodigoContrato
,TipoAuto.TipoAu
,Count(fvlclick.facturaconcepto) As LineasClick 
,max(fvlAuto.Descripcion) as Autoconsumo 
,replace(max(fvlAuto.importebase),'.',',') as ImporteAutoconsumo
,sum(fvlclick.ImporteBase) As ImporteTotalClick
,replace(fvlAutoP.importebase,'.',',') as ProductosAutoconsumo
,count(fvlAuto.ImporteBase) as LineasAutoconsumo
,replace(paCO.Importe,'.',',') as CO
,REPLACE(ISNULL(MAX(CASE WHEN FacturasEnergiaML.CodigoPeriodoXML = 1 THEN  ISNULL(FacturasEnergiaML.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',')  TerminoEnergiaTarifaMLP1
,REPLACE(ISNULL(MAX(CASE WHEN FacturasEnergiaML.CodigoPeriodoXML = 2 THEN  ISNULL(FacturasEnergiaML.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',')  TerminoEnergiaTarifaMLP2
,REPLACE(ISNULL(MAX(CASE WHEN FacturasEnergiaML.CodigoPeriodoXML = 3 THEN  ISNULL(FacturasEnergiaML.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',')  TerminoEnergiaTarifaMLP3
,REPLACE(ISNULL(MAX(CASE WHEN FacturasEnergiaML.CodigoPeriodoXML = 4 THEN  ISNULL(FacturasEnergiaML.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',')  TerminoEnergiaTarifaMLP4
,REPLACE(ISNULL(MAX(CASE WHEN FacturasEnergiaML.CodigoPeriodoXML = 5 THEN  ISNULL(FacturasEnergiaML.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',')  TerminoEnergiaTarifaMLP5
,REPLACE(ISNULL(MAX(CASE WHEN FacturasEnergiaML.CodigoPeriodoXML = 6 THEN  ISNULL(FacturasEnergiaML.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)ELSE NULL END), 0), '.', ',')  TerminoEnergiaTarifaMLP6
,replace(paCOi.Importe,'.',',') as COinterno
,c.FechaContrato
,c.FechaAplicacionPrecios
from FacturaCompraCabecera fcc with (nolock)
inner join lectura l with (nolock) on l.IdFacturaCompraCabecera = fcc.IdFacturaCompraCabecera or l.IdFacturaCompraCabecera = fcc.idfacturaorigen
left join contrato c with (nolock) on fcc.CodigoContrato = c.CodigoContrato
left join FacturaVentaCabecera fvc with (nolock) on fvc.idfacturaventacabecera = l.idfacturaventacabecerasectorc
left join FacturaVentaLinea fvlclick with (nolock) on fvlclick.idfacturaventacabecera = fvc.idfacturaventacabecera and fvlclick.FacturaConcepto=30006
left join FacturaVentaLinea fvlAuto with (nolock) on fvlAuto.idfacturaventacabecera = fvc.idfacturaventacabecera  and fvlAuto.facturaconcepto in (30008,30009)
left join FacturaVentaLinea fvlAutoP with (nolock) on fvlAutoP.idfacturaventacabecera = fvc.idfacturaventacabecera  and fvlAutoP.facturaconcepto in (100001) and fvlAutoP.descripcion like 'Autoconsumo'
left join ProductoAsignacion paCO with (nolock) on paCO.IdContrato = c.IdContrato and paCO.IdProducto in (4,30)
left join ProductoAsignacion paCOi with (nolock) on paCOi.IdContrato = c.IdContrato  and paCOi.IdProducto in (90,133)
left join TipoAuto with (nolock) on TipoAuto.idfacturacompracabecera = fcc.IdFacturaCompraCabecera
left join FacturasEnergiaML  with (nolock) on fvc.idfacturaventacabecera = FacturasEnergiaML.idfacturaventacabecera 
where fcc.IdFacturaCompraCabecera in (select IdFacturaCompraCabecera from facturas with (nolock)) 
group by fcc.Numerofactura,fcc.CodigoContrato,fvc.IdFacturaVentaCabecera,fvc.SerieFactura,fvc.NumeroFactura,c.FechaContrato,c.FechaAplicacionPrecios,fvlAutoP.importebase,paCO.Importe,paCOi.Importe,TipoAuto.TipoAu
,FacturasEnergiaML.IdFacturaVentaCabecera
"

                        ' Ejecutar consulta
                        Dim command As New SqlCommand(query, connection)
                        command.Parameters.AddWithValue("@NumFacCompra", FacCompra.ToString)
                        command.CommandTimeout = 100000
                        ' Leer los resultados
                        Using reader As SqlDataReader = command.ExecuteReader()
                            While reader.Read()
                                worksheet.Cells(currentRow, 1).Value = reader("IdFacturaVentaCabecera")
                                worksheet.Cells(currentRow, 2).Value = reader("NumerofacturaC")
                                worksheet.Cells(currentRow, 3).Value = reader("SerieFactura")
                                worksheet.Cells(currentRow, 4).Value = reader("NumeroFactura").ToString
                                worksheet.Cells(currentRow, 5).Value = reader("CodigoContrato").ToString
                                worksheet.Cells(currentRow, 6).Value = reader("TipoAu").ToString
                                worksheet.Cells(currentRow, 7).Value = reader("LineasClick").ToString
                                worksheet.Cells(currentRow, 8).Value = reader("Autoconsumo")
                                Dim ImporteAutoconsumo As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("ImporteAutoconsumo").ToString(), ImporteAutoconsumo) Then
                                    worksheet.Cells(currentRow, 9).Value = ImporteAutoconsumo
                                Else
                                    worksheet.Cells(currentRow, 9).Value = 0
                                End If
                                worksheet.Cells(currentRow, 9).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal
                                ' 
                                Dim ImporteTotalClick As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("ImporteTotalClick").ToString(), ImporteTotalClick) Then
                                    worksheet.Cells(currentRow, 10).Value = ImporteTotalClick
                                Else
                                    worksheet.Cells(currentRow, 10).Value = 0
                                End If
                                worksheet.Cells(currentRow, 10).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal     

                                Dim ProductosAutoconsumo As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("ProductosAutoconsumo").ToString(), ProductosAutoconsumo) Then
                                    worksheet.Cells(currentRow, 11).Value = ProductosAutoconsumo
                                Else
                                    worksheet.Cells(currentRow, 11).Value = 0
                                End If
                                worksheet.Cells(currentRow, 11).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal  

                                worksheet.Cells(currentRow, 12).Value = reader("LineasAutoconsumo").ToString

                                Dim CO As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("CO").ToString(), CO) Then
                                    worksheet.Cells(currentRow, 13).Value = CO
                                Else
                                    worksheet.Cells(currentRow, 13).Value = 0
                                End If
                                worksheet.Cells(currentRow, 13).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal  

                                Dim TerminoEnergiaTarifaMLP1 As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("TerminoEnergiaTarifaMLP1").ToString(), TerminoEnergiaTarifaMLP1) Then
                                    worksheet.Cells(currentRow, 14).Value = TerminoEnergiaTarifaMLP1
                                Else
                                    worksheet.Cells(currentRow, 14).Value = 0
                                End If
                                worksheet.Cells(currentRow, 14).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal 
                                Dim TerminoEnergiaTarifaMLP2 As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("TerminoEnergiaTarifaMLP2").ToString(), TerminoEnergiaTarifaMLP2) Then
                                    worksheet.Cells(currentRow, 15).Value = TerminoEnergiaTarifaMLP2
                                Else
                                    worksheet.Cells(currentRow, 15).Value = 0
                                End If
                                worksheet.Cells(currentRow, 15).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal 
                                Dim TerminoEnergiaTarifaMLP3 As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("TerminoEnergiaTarifaMLP3").ToString(), TerminoEnergiaTarifaMLP3) Then
                                    worksheet.Cells(currentRow, 16).Value = TerminoEnergiaTarifaMLP3
                                Else
                                    worksheet.Cells(currentRow, 16).Value = 0
                                End If
                                worksheet.Cells(currentRow, 16).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal 
                                Dim TerminoEnergiaTarifaMLP4 As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("TerminoEnergiaTarifaMLP4").ToString(), TerminoEnergiaTarifaMLP4) Then
                                    worksheet.Cells(currentRow, 17).Value = TerminoEnergiaTarifaMLP4
                                Else
                                    worksheet.Cells(currentRow, 17).Value = 0
                                End If
                                worksheet.Cells(currentRow, 17).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal 
                                Dim TerminoEnergiaTarifaMLP5 As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("TerminoEnergiaTarifaMLP5").ToString(), TerminoEnergiaTarifaMLP5) Then
                                    worksheet.Cells(currentRow, 18).Value = TerminoEnergiaTarifaMLP5
                                Else
                                    worksheet.Cells(currentRow, 18).Value = 0
                                End If
                                worksheet.Cells(currentRow, 18).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal 
                                Dim TerminoEnergiaTarifaMLP6 As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("TerminoEnergiaTarifaMLP6").ToString(), TerminoEnergiaTarifaMLP6) Then
                                    worksheet.Cells(currentRow, 19).Value = TerminoEnergiaTarifaMLP6
                                Else
                                    worksheet.Cells(currentRow, 19).Value = 0
                                End If
                                worksheet.Cells(currentRow, 19).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal 

                                Dim COinterno As Decimal
                                ' Intentar convertir y asignar el valor
                                If Decimal.TryParse(reader("COinterno").ToString(), COinterno) Then
                                    worksheet.Cells(currentRow, 20).Value = COinterno
                                Else
                                    worksheet.Cells(currentRow, 20).Value = 0
                                End If
                                worksheet.Cells(currentRow, 20).Style.Numberformat.Format = "#,##0.000000" ' Formato decimal 

                                worksheet.Cells(currentRow, 21).Value = reader("FechaContrato").ToString
                                worksheet.Cells(currentRow, 22).Value = reader("FechaAplicacionPrecios").ToString
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


    Public Function BuscarCAEMasivo(RutaExcel As String) As Long
        Dim contador As Long = 0
        Dim excelFilePath As String = RutaExcel
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial
        Dim CodigosCUPS As New List(Of String)
        Try
            Using package As New ExcelPackage(New FileInfo(excelFilePath))
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)
                Dim rowCount As Integer = worksheet.Dimension.Rows

                ' Leer códigos de contrato del Excel
                Dim codigosContrato As New List(Of Long)()
                For row As Integer = 2 To rowCount
                    Dim CodContrato As String = worksheet.Cells(row, 1).Value?.ToString()
                    Dim Cups As String = worksheet.Cells(row, 3).Value?.ToString()
                    If Cups.Length > 5 Then
                        CodigosCUPS.Add(Cups)
                    End If
                    If Not String.IsNullOrEmpty(CodContrato) Then
                        codigosContrato.Add(CodContrato)
                    End If
                Next

                ' Procesar por lotes
                Dim lotes As New List(Of List(Of Long))()
                Dim tamanioLote As Integer = 20 ' Ajusta este valor según tus necesidades y recursos
                For i As Integer = 0 To codigosContrato.Count - 1 Step tamanioLote
                    lotes.Add(codigosContrato.Skip(i).Take(tamanioLote).ToList())
                Next

                ' Crear hoja de resultados
                package.Workbook.Worksheets.Add("Consulta")
                Dim hojaResultados As ExcelWorksheet = package.Workbook.Worksheets("Consulta")
                hojaResultados.Cells(1, 1).Value = "CodigoCUPS"
                hojaResultados.Cells(1, 2).Value = "codigocontrato"
                hojaResultados.Cells(1, 3).Value = "fechacontrato"
                hojaResultados.Cells(1, 4).Value = "fechaalta"
                hojaResultados.Cells(1, 5).Value = "ConsumoEstimado"
                hojaResultados.Cells(1, 6).Value = "Consumo_Activa_1"
                hojaResultados.Cells(1, 7).Value = "Consumo_Activa_2"
                hojaResultados.Cells(1, 8).Value = "Consumo_Activa_3"
                hojaResultados.Cells(1, 9).Value = "Consumo_Activa_4"
                hojaResultados.Cells(1, 10).Value = "Consumo_Activa_5"
                hojaResultados.Cells(1, 11).Value = "Consumo_Activa_6"
                hojaResultados.Cells(1, 12).Value = "TotalFacturas_Emitidas2024"
                hojaResultados.Row(1).Style.Font.Bold = True
                Dim currentRow As Integer = 2

                Using connection As New SqlConnection(connectionString)
                    connection.Open()

                    For Each lote As List(Of Long) In lotes
                        ' Crear tabla de parámetros para el lote actual
                        ' Crear DataTable para TVP
                        Dim contratoTable As New DataTable()
                        contratoTable.Columns.Add("codigocontrato", GetType(Long))

                        ' Llenar el DataTable con los contratos del lote actual
                        For Each codigo In lote
                            contratoTable.Rows.Add(codigo)
                        Next
                        ' Consulta SQL parametrizada
                        Dim query As String = "--Consultas Erick
WITH LineasLectura AS (
    SELECT ll.idlectura, ll.IdTarifaPeajePeriodoLectura, SUM(ll.maximetro) AS maximetro, SUM(ll.ConsumoActiva) AS ConsumoActiva,
           SUM(ll.ConsumoReactiva) AS ConsumoReactiva, SUM(ll.ActivaExtra) AS ActivaExtra
    FROM LecturaLinea ll WITH (NOLOCK)
    INNER JOIN Lectura l WITH (NOLOCK) ON l.IdLectura = ll.IdLectura
    GROUP BY ll.idlectura, ll.IdTarifaPeajePeriodoLectura
),
AgrupacionFacs AS (
    SELECT fvc.codigocontrato,
           REPLACE(SUM(ISNULL(ll1.ConsumoActiva, 0.0) + ISNULL(ll1.ActivaExtra, 0.0)), '.', ',') AS Consumo_Activa_1,
           REPLACE(SUM(ISNULL(ll2.ConsumoActiva, 0.0) + ISNULL(ll2.ActivaExtra, 0.0)), '.', ',') AS Consumo_Activa_2,
           REPLACE(SUM(ISNULL(ll3.ConsumoActiva, 0.0) + ISNULL(ll3.ActivaExtra, 0.0)), '.', ',') AS Consumo_Activa_3,
           REPLACE(SUM(ISNULL(ll4.ConsumoActiva, 0.0) + ISNULL(ll4.ActivaExtra, 0.0)), '.', ',') AS Consumo_Activa_4,
           REPLACE(SUM(ISNULL(ll5.ConsumoActiva, 0.0) + ISNULL(ll5.ActivaExtra, 0.0)), '.', ',') AS Consumo_Activa_5,
           REPLACE(SUM(ISNULL(ll6.ConsumoActiva, 0.0) + ISNULL(ll6.ActivaExtra, 0.0)), '.', ',') AS Consumo_Activa_6,
           COUNT(IdFacturaVentaCabecera) AS TotalFacturas_Emitidas2024
    FROM FacturaVentaCabecera fvc
    LEFT JOIN Lectura l WITH (NOLOCK) ON fvc.idfacturaventacabecera = l.IdFacturaVentaCabeceraSectorC OR (fvc.IdFacturaOrigen = l.IdFacturaVentaCabeceraSectorC AND fvc.SerieFactura LIKE '%ABO%')
    LEFT JOIN LineasLectura ll1 WITH (NOLOCK) ON l.IdLectura = ll1.IdLectura AND ll1.idtarifapeajeperiodolectura IN (20202001, 20203001, 20206101, 20206201, 20206301, 20206401, 20208001, 20208101)
    LEFT JOIN LineasLectura ll2 WITH (NOLOCK) ON l.IdLectura = ll2.IdLectura AND ll2.idtarifapeajeperiodolectura IN (20202002, 20203002, 20206102, 20206202, 20206302, 20206402, 20208002, 20208102)
    LEFT JOIN LineasLectura ll3 WITH (NOLOCK) ON l.IdLectura = ll3.IdLectura AND ll3.idtarifapeajeperiodolectura IN (20202003, 20203003, 20206103, 20206203, 20206303, 20206403, 20208003, 20208103)
    LEFT JOIN LineasLectura ll4 WITH (NOLOCK) ON l.IdLectura = ll4.IdLectura AND ll4.idtarifapeajeperiodolectura IN (20203004, 20206104, 20206204, 20206304, 20206404, 20208004, 20208104)
    LEFT JOIN LineasLectura ll5 WITH (NOLOCK) ON l.IdLectura = ll5.IdLectura AND ll5.idtarifapeajeperiodolectura IN (20203005, 20206105, 20206205, 20206305, 20206405, 20208005, 20208105)
    LEFT JOIN LineasLectura ll6 WITH (NOLOCK) ON l.IdLectura = ll6.IdLectura AND ll6.idtarifapeajeperiodolectura IN (20203006, 20206106, 20206206, 20206306, 20206406, 20208006, 20208106)
    WHERE fvc.seriefactura IS NOT NULL AND fvc.facturacategoria = 'EN' AND FechaFactura BETWEEN '01/01/2024' AND '31/12/2024'
    GROUP BY fvc.CodigoContrato
)
SELECT CodigoCUPS,c.codigocontrato, CAST(FechaContrato AS DATE) AS fechacontrato, CAST(FechaAlta AS DATE) AS fechaalta, 
       REPLACE(ConsumoEstimado, '.', ',') AS ConsumoEstimado, agrupacionfacs.* 
FROM contrato c
inner join CUPS on c.IdCups = CUPS.IdCups
LEFT JOIN AgrupacionFacs ON c.codigocontrato = agrupacionfacs.codigocontrato
WHERE c.codigocontrato IN (SELECT codigocontrato FROM @CodigoContratos);-- AQUI ESTA EL CAMBIO
"

                        Using command As New SqlCommand(query, connection)
                            command.CommandTimeout = 100000
                            Dim param As SqlParameter = command.Parameters.AddWithValue("@CodigoContratos", contratoTable)
                            param.SqlDbType = SqlDbType.Structured
                            param.TypeName = "CodigoContratoTableType"
                            Using reader As SqlDataReader = command.ExecuteReader()
                                While reader.Read()
                                    hojaResultados.Cells(currentRow, 1).Value = reader("CodigoCUPS").ToString()
                                    hojaResultados.Cells(currentRow, 2).Value = reader("codigocontrato").ToString()
                                    hojaResultados.Cells(currentRow, 3).Value = reader("fechacontrato").ToString
                                    hojaResultados.Cells(currentRow, 4).Value = reader("fechaalta").ToString
                                    hojaResultados.Cells(currentRow, 5).Value = reader("ConsumoEstimado").ToString
                                    hojaResultados.Cells(currentRow, 6).Value = reader("Consumo_Activa_1").ToString
                                    hojaResultados.Cells(currentRow, 7).Value = reader("Consumo_Activa_2").ToString
                                    hojaResultados.Cells(currentRow, 8).Value = reader("Consumo_Activa_3").ToString
                                    hojaResultados.Cells(currentRow, 9).Value = reader("Consumo_Activa_4").ToString
                                    hojaResultados.Cells(currentRow, 10).Value = reader("Consumo_Activa_5").ToString
                                    hojaResultados.Cells(currentRow, 11).Value = reader("Consumo_Activa_6").ToString
                                    hojaResultados.Cells(currentRow, 12).Value = reader("TotalFacturas_Emitidas2024").ToString
                                    currentRow += 1
                                End While
                            End Using
                        End Using
                    Next ' Para cada lote
                End Using ' Para la conexión

                package.Save() ' Guarda los cambios en el Excel
            End Using ' Para el paquete Excel

            If CodigosCUPS.Count > 0 Then
                BuscarCAEMasivo(RutaExcel, CodigosCUPS)
            End If

        Catch ex As Exception
            Throw ' Re-lanza la excepción para que se maneje en otro lugar
        Finally
            ' No es necesario cerrar la conexión aquí, se cierra automáticamente con Using
        End Try

        Return contador
    End Function
    Public Function BuscarCAEMasivo(RutaExcel As String, Cups As List(Of String)) As Long
        Dim contador As Long = 0
        Dim excelFilePath As String = RutaExcel
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial
        Dim CodigosCUPS As New List(Of String)
        Try
            Using package As New ExcelPackage(New FileInfo(excelFilePath))
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)
                Dim rowCount As Integer = worksheet.Dimension.Rows

                ' Procesar por lotes
                Dim lotes As New List(Of List(Of String))()
                Dim tamanioLote As Integer = 20
                For i As Integer = 0 To Cups.Count - 1 Step tamanioLote
                    lotes.Add(Cups.Skip(i).Take(tamanioLote).ToList())
                Next

                ' Crear hoja de resultados
                package.Workbook.Worksheets.Add("AgrupadoPorCUPS")
                Dim hojaResultados As ExcelWorksheet = package.Workbook.Worksheets("AgrupadoPorCUPS")
                hojaResultados.Cells(1, 1).Value = "CodigoCUPS"
                hojaResultados.Cells(1, 2).Value = "Facturado_P1"
                hojaResultados.Cells(1, 3).Value = "Facturado_P2"
                hojaResultados.Cells(1, 4).Value = "Facturado_P3"
                hojaResultados.Cells(1, 5).Value = "Facturado_P4"
                hojaResultados.Cells(1, 6).Value = "Facturado_P5"
                hojaResultados.Cells(1, 7).Value = "Facturado_P6"
                hojaResultados.Cells(1, 8).Value = "TotalFacturasbyCups_Emitidas2024"
                hojaResultados.Row(1).Style.Font.Bold = True
                Dim currentRow As Integer = 2

                Using connection As New SqlConnection(connectionString)
                    connection.Open()

                    For Each lote As List(Of String) In lotes
                        Dim cupsTable As New DataTable()
                        cupsTable.Columns.Add("CodigoCUPS", GetType(String))

                        ' Llenar el DataTable con los CUPS del lote actual
                        For Each codigo In lote
                            cupsTable.Rows.Add(codigo)
                        Next

                        ' Consulta SQL corregida
                        Dim query As String = "WITH 
LineasLectura AS (
    SELECT ll.idlectura, ll.IdTarifaPeajePeriodoLectura, 
           SUM(ll.maximetro) AS maximetro, 
           SUM(ll.ConsumoActiva) AS ConsumoActiva,
           SUM(ll.ConsumoReactiva) AS ConsumoReactiva, 
           SUM(ll.ActivaExtra) AS ActivaExtra
    FROM LecturaLinea ll WITH (NOLOCK)
    INNER JOIN Lectura l WITH (NOLOCK) ON l.IdLectura = ll.IdLectura
    GROUP BY ll.idlectura, ll.IdTarifaPeajePeriodoLectura
),
AgrupacionFacs AS (
    SELECT fvc.codigocontrato,
           SUM(ISNULL(ll1.ConsumoActiva, 0.0) + ISNULL(ll1.ActivaExtra, 0.0)) AS Consumo_Activa_1,
           SUM(ISNULL(ll2.ConsumoActiva, 0.0) + ISNULL(ll2.ActivaExtra, 0.0)) AS Consumo_Activa_2,
           SUM(ISNULL(ll3.ConsumoActiva, 0.0) + ISNULL(ll3.ActivaExtra, 0.0)) AS Consumo_Activa_3,
           SUM(ISNULL(ll4.ConsumoActiva, 0.0) + ISNULL(ll4.ActivaExtra, 0.0)) AS Consumo_Activa_4,
           SUM(ISNULL(ll5.ConsumoActiva, 0.0) + ISNULL(ll5.ActivaExtra, 0.0)) AS Consumo_Activa_5,
           SUM(ISNULL(ll6.ConsumoActiva, 0.0) + ISNULL(ll6.ActivaExtra, 0.0)) AS Consumo_Activa_6,
           COUNT(IdFacturaVentaCabecera) AS TotalFacturas_Emitidas2024,
           MIN(l.FechaLecturaAnterior) AS PrimeraLectura,   -- Primer lectura
           MAX(l.FechaLectura) AS UltimaLectura       -- Última lectura
    FROM FacturaVentaCabecera fvc
    LEFT JOIN Lectura l WITH (NOLOCK) ON fvc.idfacturaventacabecera = l.IdFacturaVentaCabeceraSectorC 
                                      OR (fvc.IdFacturaOrigen = l.IdFacturaVentaCabeceraSectorC 
                                          AND fvc.SerieFactura LIKE '%ABO%')
    LEFT JOIN LineasLectura ll1 WITH (NOLOCK) ON l.IdLectura = ll1.IdLectura 
        AND ll1.idtarifapeajeperiodolectura IN (20202001, 20203001, 20206101, 20206201, 20206301, 20206401, 20208001, 20208101)
    LEFT JOIN LineasLectura ll2 WITH (NOLOCK) ON l.IdLectura = ll2.IdLectura 
        AND ll2.idtarifapeajeperiodolectura IN (20202002, 20203002, 20206102, 20206202, 20206302, 20206402, 20208002, 20208102)
    LEFT JOIN LineasLectura ll3 WITH (NOLOCK) ON l.IdLectura = ll3.IdLectura 
        AND ll3.idtarifapeajeperiodolectura IN (20202003, 20203003, 20206103, 20206203, 20206303, 20206403, 20208003, 20208103)
    LEFT JOIN LineasLectura ll4 WITH (NOLOCK) ON l.IdLectura = ll4.IdLectura 
        AND ll4.idtarifapeajeperiodolectura IN (20203004, 20206104, 20206204, 20206304, 20206404, 20208004, 20208104)
    LEFT JOIN LineasLectura ll5 WITH (NOLOCK) ON l.IdLectura = ll5.IdLectura 
        AND ll5.idtarifapeajeperiodolectura IN (20203005, 20206105, 20206205, 20206305, 20206405, 20208005, 20208105)
    LEFT JOIN LineasLectura ll6 WITH (NOLOCK) ON l.IdLectura = ll6.IdLectura 
        AND ll6.idtarifapeajeperiodolectura IN (20203006, 20206106, 20206206, 20206306, 20206406, 20208006, 20208106)
    WHERE fvc.seriefactura IS NOT NULL 
      AND fvc.facturacategoria = 'EN' 
      AND FechaFactura BETWEEN '01/01/2024' AND '31/12/2024'
    GROUP BY fvc.CodigoContrato
)
SELECT 
    CUPS.CodigoCUPS, 
    REPLACE(SUM(agrupacionfacs.Consumo_Activa_1), '.', ',') AS Facturado_P1,
    REPLACE(SUM(agrupacionfacs.Consumo_Activa_2), '.', ',') AS Facturado_P2,
    REPLACE(SUM(agrupacionfacs.Consumo_Activa_3), '.', ',') AS Facturado_P3,
    REPLACE(SUM(agrupacionfacs.Consumo_Activa_4), '.', ',') AS Facturado_P4,
    REPLACE(SUM(agrupacionfacs.Consumo_Activa_5), '.', ',') AS Facturado_P5,
    REPLACE(SUM(agrupacionfacs.Consumo_Activa_6), '.', ',') AS Facturado_P6,
    SUM(TotalFacturas_Emitidas2024) AS TotalFacturasbyCups_Emitidas2024,
    MIN(agrupacionfacs.PrimeraLectura) AS PrimeraLectura,    -- Añadido para la primera lectura
    MAX(agrupacionfacs.UltimaLectura) AS UltimaLectura       -- Añadido para la última lectura
FROM contrato c
INNER JOIN CUPS ON c.IdCups = CUPS.IdCups
LEFT JOIN AgrupacionFacs ON c.codigocontrato = agrupacionfacs.codigocontrato
INNER JOIN @CodigosCups cc ON LEFT(CUPS.CodigoCUPS, 20) = LEFT(cc.CodigoCUPS, 20)
group by CUPS.CodigoCUPS"

                        Using command As New SqlCommand(query, connection)
                            Dim param = command.Parameters.AddWithValue("@CodigosCups", cupsTable)
                            param.SqlDbType = SqlDbType.Structured
                            param.TypeName = "CodigoCUPSTableType"
                            Using reader As SqlDataReader = command.ExecuteReader()
                                While reader.Read()
                                    For i As Integer = 1 To 8
                                        hojaResultados.Cells(currentRow, i).Value = reader(i - 1).ToString()
                                    Next
                                    currentRow += 1
                                End While
                            End Using
                        End Using
                    Next
                End Using
                package.Save()
            End Using
        Catch ex As Exception
            Throw
        End Try
    End Function

End Class
