--Erick _Gestor Sige_ Cadasa
with XMLNAMESPACES('http://localhost/elegibilidad' as "XS") 
,FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturaVentaCabecera 
where IdCliente=50906 and SerieFactura is not null and Entorno='E1' and FechaFactura>='DesdeFechaReplace' and FechaFactura<='hastaFechaReplace'))

,FacturasVentalinea (idfacturaventacabecera,facturaconcepto,importebase)
as
(select idfacturaventacabecera,facturaconcepto,SUM(importebase) from FacturaVentaLinea
where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto in (30001,30002,30004,10001,130002,130004,100001)
group by idfacturaventacabecera,facturaconcepto)

,ConsumosReactiva(id, r1,r2,r3,r4,r5,r6,pr1,pr2,pr3,pr4,pr5,pr6)
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
--ImportesPotencia as (
--select Lineas.IdFacturaVentaCabecera, sum(Lineas.ImporteBase) as Importe from Lineas
--where (FacturaConcepto between 10000 and 19999 or FacturaConcepto in (130001,130002, 131001)) or 
--(FacturaConcepto in (90003, 90018, 90035,90036,90039,90040, 90042, 90043, 90044, 90045, 90049, 90050, 90053, 90054, 90056, 90057, 90058, 90059, 90070))
--group by Lineas.IdFacturaVentaCabecera
--),
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
)--,
--DescuentosPotencia as (
--Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
--where (FacturaConcepto in (120001, 120004)) or (FacturaConcepto in (120006, 90007))
--group by Lineas.IdFacturaVentaCabecera
--),
--DescuentosEnergia as (
--Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
--where (FacturaConcepto in (120002, 120005)) or (FacturaConcepto in (120007, 90006))
--group by Lineas.IdFacturaVentaCabecera
--),
--ImportesAlquileres as (
--Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
--where (FacturaConcepto between 50000 and 59999) or (FacturaConcepto in (120007, 90006))
--group by Lineas.IdFacturaVentaCabecera
--),
--ImportesProductos as (
--Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
--where FacturaConcepto in (100001, 100002, 110001, 90009)
--group by Lineas.IdFacturaVentaCabecera
--)




Select
fvc.SerieFactura+''+cast (fvc.NumeroFactura as varchar) As  'N Documento Oficial'
,'TOTALENERGIES ELECTRICIDAD Y GAS ESPAÑA, S.A.' As 'Nombre Del Emisor'
,t.textotarifa as Tarifa
,cast(CUPS.CodigoCUPS as varchar(20)) as CUPS
,'FacturaAjustada' as 'N Factura Ajustada'
,case  when fvc.idfacturaorigen is not  null 
THEN (select fv.SerieFactura+''+cast (fv.NumeroFactura as varchar) from FacturaVentaCabecera fv where fv.IdFacturaVentaCabecera=fvc.idfacturaorigen)
Else ' '
End as 'N Factura Anulada' 
,convert(varchar,fvc.FechaFactura, 103) as 'Fecha De Emision'
,convert(varchar, ISNULL(fvc.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaInicioFactura)[1]', 'date'),fvc.FechaLecturaAnteriorXML), 103) as 'Fecha Desde'
,convert(varchar,ISNULL(fvc.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaFinalFactura)[1]', 'date'),fvc.FechaLecturaActualXML), 103) as 'Fecha Hasta'
,datediff(DAY,fvc.FechaLecturaAnteriorXML,fvc.FechaLecturaActualXML) as 'Numero Dias Periodo Calculo'
,cll.NombreCalle +' '+ cups.Aclarador as 'Direccion Del Suministro'
,ciu.TextoCiudad as Poblacion
,c.codigocontrato as Contrato
,fvt.PorcentajeImpuesto As 'Porcentaje Impuesto'
,replace(fvt.ImporteBase,'.',',') As 'Base Imponible'
,replace(fvt.ImporteTotal,'.',',') As 'Importe Total'
,replace(fvt.ImporteImpuesto,'.',',') As IVA
,replace(isnull(fvlev.ImporteBase , 0),'.',',') as 'Coste Energia'
,convert(float,isnull(fvle.importebase,0))+ convert(float,isnull(fvlte.importebase,0)) as 'Total Importe Cargo Energía + Peaje por'
,replace(isnull(fvle.importebase,0),'.',',') as 'Importe Cargo Energía'
,replace(isnull(fvlte.importebase,0),'.',',') as 'Importe Peaje Energía'
,convert(float,isnull(fvlp.importebase,0))+ convert(float,isnull(fvltp.importebase,0)) as 'Total Importe Potencia cargo + peaje'
,replace(isnull(fvlp.importebase,0),'.',',') as 'Importe Cargo Potencia'
,replace(isnull(fvltp.importebase,0),'.',',') as 'Importe Potencia Peaje'
,replace(isnull(ImportesExcesos.Importe, 0),'.',',') as 'Importe Exceso Potencia 6x'
--,replace((ISNULL(cr.r1,0.0) * ISNULL(cr.pr1, 0.0) + ISNULL(cr.r2,0.0) * ISNULL(cr.pr2, 0.0) 
--+ ISNULL(cr.r3,0.0) * ISNULL(cr.pr3,0.0) + ISNULL(cr.r4,0.0) * ISNULL(cr.pr4, 0.0) 
--+ ISNULL(cr.r5,0.0) * ISNULL(cr.pr5, 0.0) + ISNULL(cr.r6,0.0) * ISNULL(cr.pr6, 0.0)),'.',',') as 'Importe Exceso Reactiva'
,replace(isnull(Importesreactiva.Importe, 0),'.',',')as 'Importe Exceso Reactiva'
,replace(fvlIE.ImporteBase,'.',',') As 'Importe Impuesto Electrico General'
,'' as 'Impuesto Eléctrico 0,5 €/MWh'
,'' as 'Impuesto Eléctrico 1,0 €/MWh'
, replace(ISNULL(fvlIEE.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoImpuesto/PorcentajeAplicado)[1]', 'decimal(18,6)'), 0),'.',',') as 'Tipo Impositivo IEE'
,replace(isnull(fvlpo.importebase,0),'.',',') as  'Otros conceptos'
,replace(fvlCON.ImporteBase,'.',',') As 'Importe Alquiler Contador (1)'
,replace(isnull(ImportesEnergia.Importe, 0),'.',',')  as 'Importe Termino Energia Activa'
,replace(ISNULL(fvc.InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceraHistorialConsumos/PeriodoConsumo/FacturaInfoCabeceraHistorialPeriodoConsumoDTO/ConsumoActiva)[1]', 'decimal(18,3)'), 0),'.',',') as 'Consumo Energia Activa'
,replace(ll1.ConsumoActiva,'.',',') as 'Consumo Activa P1'
,replace(ll2.ConsumoActiva,'.',',') as 'Consumo Activa P2'
,replace(ll3.ConsumoActiva,'.',',') as 'Consumo Activa P3'
,replace(ll4.ConsumoActiva,'.',',') as 'Consumo Activa P4'
,replace(ll5.ConsumoActiva,'.',',') as 'Consumo Activa P5'
,replace(ll6.ConsumoActiva,'.',',') as 'Consumo Activa P6'
,replace(ll1.ActivaExtra,'.',',') as 'Activa Extra P1'
,replace(ll2.ActivaExtra,'.',',') as 'Activa Extra P2'
,replace(ll3.ActivaExtra,'.',',') as 'Activa Extra P3'
,replace(ll4.ActivaExtra,'.',',') as 'Activa Extra P4'
,replace(ll5.ActivaExtra,'.',',') as 'Activa Extra P5'
,replace(ll6.ActivaExtra,'.',',') as 'Activa Extra P6'
,replace(ll1.ActivaAnterior,'.',',') as 'Inicio Contador Activa P1'
,replace(ll1.activaActual,'.',',') as   'Fin Contador Activa P1'	   
,replace(ll2.ActivaAnterior,'.',',') as 'Inicio Contador Activa P2'
,replace(ll2.activaActual,'.',',') as   'Fin Contador Activa P2'		   
,replace(ll3.ActivaAnterior,'.',',') as 'Inicio Contador Activa P3'
,replace(ll3.activaActual,'.',',') as   'Fin Contador Activa P3'		   
,replace(ll4.ActivaAnterior,'.',',') as 'Inicio Contador Activa P4'
,replace(ll4.activaActual,'.',',') as   'Fin Contador Activa P4'		   
,replace(ll5.ActivaAnterior,'.',',') as 'Inicio Contador Activa P5'
,replace(ll5.activaActual,'.',',') as   'Fin Contador Activa P5'		   
,replace(ll6.ActivaAnterior,'.',',') as 'Inicio Contador Activa P6'
,replace(ll6.activaActual,'.',',') as   'Fin Contador Activa P6'
,convert(float,isnull(tpp1b.EnergiaPrecio,0)) + convert(float,isnull(tpp1b.CargoEnergiaPrecio,0)) + convert(float,(ISNULL(fvlP1.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0))) as 'Precio Activa P1'
,convert(float,isnull(tpp2b.EnergiaPrecio,0)) + convert(float,isnull(tpp2b.CargoEnergiaPrecio,0)) + convert(float,(ISNULL(fvlP2.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0))) as 'Precio Activa P2'
,convert(float,isnull(tpp3b.EnergiaPrecio,0)) + convert(float,isnull(tpp3b.CargoEnergiaPrecio,0)) + convert(float,(ISNULL(fvlP3.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0))) as 'Precio Activa P3'
,convert(float,isnull(tpp4b.EnergiaPrecio,0)) + convert(float,isnull(tpp4b.CargoEnergiaPrecio,0)) + convert(float,(ISNULL(fvlP4.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0))) as 'Precio Activa P4'
,convert(float,isnull(tpp5b.EnergiaPrecio,0)) + convert(float,isnull(tpp5b.CargoEnergiaPrecio,0)) + convert(float,(ISNULL(fvlP5.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0))) as 'Precio Activa P5'
,convert(float,isnull(tpp6b.EnergiaPrecio,0)) + convert(float,isnull(tpp6b.CargoEnergiaPrecio,0)) + convert(float,(ISNULL(fvlP6.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0))) as 'Precio Activa P6'
,convert(float,isnull(ll1.ConsumoReactiva,0))+convert(float,isnull(ll2.ConsumoReactiva,0))+convert(float,isnull(ll3.ConsumoReactiva,0))+convert(float,isnull(ll4.ConsumoReactiva,0))+convert(float,isnull(ll5.ConsumoReactiva,0))+convert(float,isnull(ll6.ConsumoReactiva,0)) As 'Consumo Energia Reactiva'
,replace(ll1.ConsumoReactiva,'.',',') as 'Consumo Reactiva P1'
,replace(ll2.ConsumoReactiva,'.',',') as 'Consumo Reactiva P2'
,replace(ll3.ConsumoReactiva,'.',',') as 'Consumo Reactiva P3'
,replace(ll4.ConsumoReactiva,'.',',') as 'Consumo Reactiva P4'
,replace(ll5.ConsumoReactiva,'.',',') as 'Consumo Reactiva P5'
,replace(ll6.ConsumoReactiva,'.',',') as 'Consumo Reactiva P6' 
,replace(ll1.ReActivaAnterior,'.',',') as 'Inicio Contador ReActiva P1'
,replace(ll1.ReactivaActual,'.',',') as	  'Fin Contador ReActiva P1'
,replace(ll2.ReActivaAnterior,'.',',') as 'Inicio Contador ReActiva P2'
,replace(ll2.ReactivaActual,'.',',') as   'Fin Contador ReActiva P2'
,replace(ll3.ReActivaAnterior,'.',',') as 'Inicio Contador ReActiva P3'
,replace(ll3.ReactivaActual,'.',',') as   'Fin Contador ReActiva P3'
,replace(ll4.ReActivaAnterior,'.',',') as 'Inicio Contador ReActiva P4'
,replace(ll4.ReactivaActual,'.',',') as   'Fin Contador ReActiva P4'
,replace(ll5.ReActivaAnterior,'.',',') as 'Inicio Contador ReActiva P5'
,replace(ll5.ReactivaActual,'.',',') as   'Fin Contador ReActiva P5'
,replace(ll6.ReActivaAnterior,'.',',') as 'Inicio Contador ReActiva P6'
,replace(ll6.ReactivaActual,'.',',') as   'Fin Contador ReActiva P6'
,replace(cr.pr1,'.',',') as 'Precio Reactiva P1'
,replace(cr.pr2,'.',',') as 'Precio Reactiva P2'
,replace(cr.pr3,'.',',') as 'Precio Reactiva P3'
,replace(cr.pr4,'.',',') as 'Precio Reactiva P4'
,replace(cr.pr5,'.',',') as 'Precio Reactiva P5'
,replace(cr.pr6,'.',',') as 'Precio Reactiva P6'
,replace(ll1.maximetro,'.',',') as ' Lectura Potencia Maxima P1'
,replace(ll2.maximetro,'.',',') as ' Lectura Potencia Maxima P2'
,replace(ll3.maximetro,'.',',') as ' Lectura Potencia Maxima P3'
,replace(ll4.maximetro,'.',',') as ' Lectura Potencia Maxima P4'
,replace(ll5.maximetro,'.',',') as ' Lectura Potencia Maxima P5'
,replace(ll6.maximetro,'.',',') as ' Lectura Potencia Maxima P6'
,replace(ll1.MaximetroExceso,'.',',') as 'Exceso de potencia en kw P1'
,replace(ll2.MaximetroExceso,'.',',') as 'Exceso de potencia en kw P2'
,replace(ll3.MaximetroExceso,'.',',') as 'Exceso de potencia en kw P3'
,replace(ll4.MaximetroExceso,'.',',') as 'Exceso de potencia en kw P4'
,replace(ll5.MaximetroExceso,'.',',') as 'Exceso de potencia en kw P5'
,replace(ll6.MaximetroExceso,'.',',') as 'Exceso de potencia en kw P6'
,'' as 'Precio Exceso P1'
,'' as 'Precio Exceso P2'
,'' as 'Precio Exceso P3'
,'' as 'Precio Exceso P4'
,'' as 'Precio Exceso P5'
,'' as 'Precio Exceso P6'
,ISNULL(Replace(fcc.FacturaXML.value('(//XS:EnergiaCapacitiva/XS:ImporteTotalEnergiaCapacitiva)[1]', 'decimal(18,6)'),'.',','),0) as 'Importe de Energía capacitiva 1'
,replace(cp1.PotenciaContratada,'.',',') as 'Potencia Contratada P1'
,replace(cp2.PotenciaContratada,'.',',') as 'Potencia Contratada P2'
,replace(cp3.PotenciaContratada,'.',',') as 'Potencia Contratada P3'
,replace(cp4.PotenciaContratada,'.',',') as 'Potencia Contratada P4'
,replace(cp5.PotenciaContratada,'.',',') as 'Potencia Contratada P5'
,replace(cp6.PotenciaContratada,'.',',') as 'Potencia Contratada P6'
,fvlCAP.ImporteBase as 'Mecanismo de ajuste RDL 10/2022'--Importe base CAPGAS



--,cl.Identidad as CIFDNI
--,dbo.formateardenominacion(cl.nombre,cl.apellido1,cl.apellido2,cl.razonsocial) as RazonSocial
--,cups.CodPostal
--,pv.TextoProvincia as Provincia
--,d.NombreFiscal as Distribuidora
--,replace(tpp1b.PotenciaPrecio,'.',',') as PrecioPotenciaP1B
--,replace(tpp1b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP1B
--,replace(tpp2b.PotenciaPrecio,'.',',') as PrecioPotenciaP2B
--,replace(tpp2b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP2B
--,replace(tpp3b.PotenciaPrecio,'.',',') as PrecioPotenciaP3B
--,replace(tpp3b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP3B
--,replace(tpp4b.PotenciaPrecio,'.',',') as PrecioPotenciaP4B
--,replace(tpp4b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP4B
--,replace(tpp5b.PotenciaPrecio,'.',',') as PrecioPotenciaP5B
--,replace(tpp5b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP5B
--,replace(tpp6b.PotenciaPrecio,'.',',') as PrecioPotenciaP6B
--,replace(tpp6b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP6B
--,replace(fvlMAX.ImporteBase,'.',',') as ImporteMaximetro

--,replace(ll1.Maximetro,'.',',') as LectMaximetroP1
--,replace(ll2.Maximetro,'.',',') as LectMaximetroP2
--,replace(ll3.Maximetro,'.',',') as LectMaximetroP3
--,replace(ll4.Maximetro,'.',',') as LectMaximetroP4
--,replace(ll5.Maximetro,'.',',') as LectMaximetroP5
--,replace(ll6.Maximetro,'.',',') as LectMaximetroP6
--,replace(isnull(ImportesPotencia.Importe, 0),'.',',') as ImportePotenciaContratacion
--,replace(isnull(ImportesReactiva.Importe, 0),'.',',') as ImporteReactivaContratacion
--,replace(isnull(DescuentosPotencia.Importe, 0),'.',',') as DescuentoPotenciaContratacion
--,replace(isnull(DescuentosEnergia.Importe, 0),'.',',') as DescuentoEnergiaContratacion
--,replace(isnull(ImportesAlquileres.Importe, 0),'.',',') as ImporteAlquilerContratacion
--,replace(isnull(ImportesProductos.Importe, 0),'.',',') as ImporteProductosContratacion
--,replace(fvlP1.importebase,'.',',') as ImporteVariableP1
--,replace(fvlP2.importebase,'.',',') as ImporteVariableP2
--,replace(fvlP3.importebase,'.',',') as ImporteVariableP3
--,replace(fvlP4.importebase,'.',',') as ImporteVariableP4
--,replace(fvlP5.importebase,'.',',') as ImporteVariableP5
--,replace(fvlP6.importebase,'.',',') as ImporteVariableP6
--,replace(fvlCAP.importebase,'.',',') as ImporteCAPGAS
--,replace(ISNULL(fvlpP1.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP1
--,replace(ISNULL(fvlpP2.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP2
--,replace(ISNULL(fvlpP3.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP3
--,replace(ISNULL(fvlpP4.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP4
--,replace(ISNULL(fvlpP5.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP5
--,replace(ISNULL(fvlpP6.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP6
--,replace(ISNULL(fvleP1.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP1
--,replace(ISNULL(fvleP2.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP2
--,replace(ISNULL(fvleP3.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP3
--,replace(ISNULL(fvleP4.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP4
--,replace(ISNULL(fvleP5.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP5
--,replace(ISNULL(fvleP6.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP6

from contrato c with (nolock)
--inner join cliente cl with (nolock) on cl.idcliente = c.idcliente
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
inner join FacturaVentaCabecera fvc with (nolock) on fvc.codigocontrato = c.codigocontrato
left join PerfilFacturacion pf with (nolock) on pf.idperfilfacturacion = ISNULL(fvc.InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceralectura/IdPerfilFacturacion)[1]', 'decimal(18,6)'),0)
inner join FacturaVentaTotal fvt with (nolock) on fvc.idfacturaventacabecera = fvt.idfacturaventacabecera
inner join CarteraCobro cc with (nolock) on fvc.idfacturaventacabecera = cc.idfacturaventacabecera
inner join Tarifa t with (nolock) on fvc.IdTarifaPeajeXML = t.IdTarifa
inner join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
left join facturaventalinea fvlIE with (nolock) on fvc.idfacturaventacabecera = fvlIE.idfacturaventacabecera and fvlIE.Facturaconcepto in (60001,60002)
left join facturaventalinea fvlCON with (nolock) on fvc.idfacturaventacabecera = fvlCON.idfacturaventacabecera and fvlCON.Facturaconcepto=50002
--left join facturaventalinea fvlMAX with (nolock) on fvc.idfacturaventacabecera = fvlMAX.idfacturaventacabecera and fvlMAX.Facturaconcepto=20006
inner join lectura l  with (nolock) on fvc.idfacturaventacabecera = l.IdFacturaVentaCabeceraSectorC or (fvc.IdFacturaOrigen = l.IdFacturaVentaCabeceraSectorC and fvc.SerieFactura like '%ABO%')
left join FacturaCompraCabecera fcc on l.IdFacturaCompraCabecera = fcc.IdFacturaCompraCabecera
left join LecturaLinea ll1 with (nolock) on l.IdLectura = ll1.IdLectura and ll1.idtarifapeajeperiodolectura in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join LecturaLinea ll2 with (nolock) on l.IdLectura = ll2.IdLectura	and ll2.idtarifapeajeperiodolectura in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join LecturaLinea ll3 with (nolock) on l.IdLectura = ll3.IdLectura	and ll3.idtarifapeajeperiodolectura in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join LecturaLinea ll4 with (nolock) on l.IdLectura = ll4.IdLectura	and ll4.idtarifapeajeperiodolectura in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join LecturaLinea ll5 with (nolock) on l.IdLectura = ll5.IdLectura	and ll5.idtarifapeajeperiodolectura in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join LecturaLinea ll6 with (nolock) on l.IdLectura = ll6.IdLectura	and ll6.idtarifapeajeperiodolectura in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join TarifaPeajePrecio tpp1b with (nolock) on l.IdTarifaPeaje = tpp1b.IdTarifaPeaje and tpp1b.fechafinal is null and tpp1b.idtarifapeajeperiodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join TarifaPeajePrecio tpp2b with (nolock) on l.IdTarifaPeaje = tpp2b.IdTarifaPeaje and tpp2b.fechafinal is null and tpp2b.idtarifapeajeperiodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join TarifaPeajePrecio tpp3b with (nolock) on l.IdTarifaPeaje = tpp3b.IdTarifaPeaje and tpp3b.fechafinal is null and tpp3b.idtarifapeajeperiodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join TarifaPeajePrecio tpp4b with (nolock) on l.IdTarifaPeaje = tpp4b.IdTarifaPeaje and tpp4b.fechafinal is null and tpp4b.idtarifapeajeperiodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join TarifaPeajePrecio tpp5b with (nolock) on l.IdTarifaPeaje = tpp5b.IdTarifaPeaje and tpp5b.fechafinal is null and tpp5b.idtarifapeajeperiodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join TarifaPeajePrecio tpp6b with (nolock) on l.IdTarifaPeaje = tpp6b.IdTarifaPeaje and tpp6b.fechafinal is null and tpp6b.idtarifapeajeperiodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join ConsumosReactiva cr with (nolock) ON cr.id = fvc.IdFacturaVentaCabecera	
--left join ImportesPotencia on ImportesPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesEnergia on ImportesEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesReactiva on ImportesReactiva.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesExcesos on ImportesExcesos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
--left join DescuentosPotencia on DescuentosPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
--left join DescuentosEnergia on DescuentosEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
--left join ImportesAlquileres on ImportesAlquileres.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
--left join ImportesProductos on ImportesProductos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
--left join FacturasVentalinea fvlc with (nolock) on fvc.IdFacturaVentaCabecera = fvlc.idfacturaventacabecera
left join facturaventalinea fvlP1 with (nolock) on fvc.idfacturaventacabecera = fvlP1.idfacturaventacabecera and fvlP1.Facturaconcepto=30004 and fvlP1.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
left join facturaventalinea fvlP2 with (nolock) on fvc.idfacturaventacabecera = fvlP2.idfacturaventacabecera and fvlP2.Facturaconcepto=30004 and fvlP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
left join facturaventalinea fvlP3 with (nolock) on fvc.idfacturaventacabecera = fvlP3.idfacturaventacabecera and fvlP3.Facturaconcepto=30004 and fvlP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
left join facturaventalinea fvlP4 with (nolock) on fvc.idfacturaventacabecera = fvlP4.idfacturaventacabecera and fvlP4.Facturaconcepto=30004 and fvlP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
left join facturaventalinea fvlP5 with (nolock) on fvc.idfacturaventacabecera = fvlP5.idfacturaventacabecera and fvlP5.Facturaconcepto=30004 and fvlP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
left join facturaventalinea fvlP6 with (nolock) on fvc.idfacturaventacabecera = fvlP6.idfacturaventacabecera and fvlP6.Facturaconcepto=30004 and fvlP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
left join facturaventalinea fvlIEE with (nolock) on fvc.idfacturaventacabecera = fvlIEE.idfacturaventacabecera and fvlIEE.Facturaconcepto=60001 
left join FacturasVentalinea fvlp with (nolock) on fvc.idfacturaventacabecera = fvlp.idfacturaventacabecera and fvlp.Facturaconcepto=130002 
left join facturaventalinea fvlCAP with (nolock) on fvc.idfacturaventacabecera = fvlCAP.idfacturaventacabecera and fvlCAP.Facturaconcepto=30004 and fvlCAP.IsAjusteCAPGas=1
left join FacturasVentalinea fvlev with (nolock) on fvc.idfacturaventacabecera = fvlev.idfacturaventacabecera and (fvlev.Facturaconcepto=30004 or fvlev.facturaconcepto=30002)
--left join facturaventalinea fvlpP2 with (nolock) on fvc.idfacturaventacabecera = fvlpP2.idfacturaventacabecera and fvlpP2.Facturaconcepto=130002 and fvlpP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
--left join facturaventalinea fvlpP3 with (nolock) on fvc.idfacturaventacabecera = fvlpP3.idfacturaventacabecera and fvlpP3.Facturaconcepto=130002 and fvlpP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
--left join facturaventalinea fvlpP4 with (nolock) on fvc.idfacturaventacabecera = fvlpP4.idfacturaventacabecera and fvlpP4.Facturaconcepto=130002 and fvlpP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
--left join facturaventalinea fvlpP5 with (nolock) on fvc.idfacturaventacabecera = fvlpP5.idfacturaventacabecera and fvlpP5.Facturaconcepto=130002 and fvlpP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
--left join facturaventalinea fvlpP6 with (nolock) on fvc.idfacturaventacabecera = fvlpP6.idfacturaventacabecera and fvlpP6.Facturaconcepto=130002 and fvlpP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
left join FacturasVentalinea fvle with (nolock) on fvc.idfacturaventacabecera = fvle.idfacturaventacabecera and fvle.Facturaconcepto=130004 
--left join facturaventalinea fvleP2 with (nolock) on fvc.idfacturaventacabecera = fvleP2.idfacturaventacabecera and fvleP2.Facturaconcepto=130004 and fvleP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
--left join facturaventalinea fvleP3 with (nolock) on fvc.idfacturaventacabecera = fvleP3.idfacturaventacabecera and fvleP3.Facturaconcepto=130004 and fvleP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
--left join facturaventalinea fvleP4 with (nolock) on fvc.idfacturaventacabecera = fvleP4.idfacturaventacabecera and fvleP4.Facturaconcepto=130004 and fvleP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
--left join facturaventalinea fvleP5 with (nolock) on fvc.idfacturaventacabecera = fvleP5.idfacturaventacabecera and fvleP5.Facturaconcepto=130004 and fvleP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
--left join facturaventalinea fvleP6 with (nolock) on fvc.idfacturaventacabecera = fvleP6.idfacturaventacabecera and fvleP6.Facturaconcepto=130004 and fvleP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
left join FacturasVentalinea fvltp with (nolock) on fvc.idfacturaventacabecera = fvltp.idfacturaventacabecera and fvltp.Facturaconcepto=10001
--left join facturaventalinea fvltpP2 with (nolock) on fvc.idfacturaventacabecera = fvltpP2.idfacturaventacabecera and fvltpP2.Facturaconcepto=10001 and fvltpP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
--left join facturaventalinea fvltpP3 with (nolock) on fvc.idfacturaventacabecera = fvltpP3.idfacturaventacabecera and fvltpP3.Facturaconcepto=10001 and fvltpP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
--left join facturaventalinea fvltpP4 with (nolock) on fvc.idfacturaventacabecera = fvltpP4.idfacturaventacabecera and fvltpP4.Facturaconcepto=10001 and fvltpP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
--left join facturaventalinea fvltpP5 with (nolock) on fvc.idfacturaventacabecera = fvltpP5.idfacturaventacabecera and fvltpP5.Facturaconcepto=10001 and fvltpP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
--left join facturaventalinea fvltpP6 with (nolock) on fvc.idfacturaventacabecera = fvltpP6.idfacturaventacabecera and fvltpP6.Facturaconcepto=10001 and fvltpP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
left join FacturasVentalinea fvlte with (nolock) on fvc.idfacturaventacabecera = fvlte.idfacturaventacabecera and fvlte.Facturaconcepto=30001 
left join FacturasVentalinea fvlpo with (nolock) on fvc.idfacturaventacabecera = fvlpo.idfacturaventacabecera and fvlpo.Facturaconcepto=100001 
--left join facturaventalinea fvlteP2 with (nolock) on fvc.idfacturaventacabecera = fvlteP2.idfacturaventacabecera and fvlteP2.Facturaconcepto=30001 and fvlteP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
--left join facturaventalinea fvlteP3 with (nolock) on fvc.idfacturaventacabecera = fvlteP3.idfacturaventacabecera and fvlteP3.Facturaconcepto=30001 and fvlteP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
--left join facturaventalinea fvlteP4 with (nolock) on fvc.idfacturaventacabecera = fvlteP4.idfacturaventacabecera and fvlteP4.Facturaconcepto=30001 and fvlteP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
--left join facturaventalinea fvlteP5 with (nolock) on fvc.idfacturaventacabecera = fvlteP5.idfacturaventacabecera and fvlteP5.Facturaconcepto=30001 and fvlteP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
--left join facturaventalinea fvlteP6 with (nolock) on fvc.idfacturaventacabecera = fvlteP6.idfacturaventacabecera and fvlteP6.Facturaconcepto=30001 and fvlteP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
where fvc.idfacturaventacabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)