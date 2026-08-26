$"--Erick _Gestor Sige_ GAM
With 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) where IdFacturaVentaCabecera in (
select IdFacturaVentaCabecera from FacturaVentaCabecera 
where fechafactura>='DesdeFechaReplace' 
and fechafactura<='hastaFechaReplace' 
and IdCliente in (select IdCliente 
				  from cliente 
				  where Identidad in ('B88425707','A48138051','B33382433','B88586953','B78078904','B88586953','B78078904','A47067509')) 
				  and SerieFactura is not null and Entorno='E1')
),
LineasFactura as (
select IdFacturaVentaCabecera,IdFacturaVentaLinea,Entorno,FacturaConcepto,InfoLineaXML,ImporteBase,IsAjusteCAPGas
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
)
,ImporteClick (idfacturaventacabecera,ImporteBase) as
(Select idfacturaventacabecera,sum(importebase) from LineasFactura with(nolock) where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
and Facturaconcepto=30006 and (isajustecapgas=0 or isajustecapgas is null)
group by IdFacturaVentaCabecera
),
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
		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=1 group by l.Id, Consumo) as ConsumoRP1 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP1.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=2 group by l.Id, Consumo) as ConsumoRP2 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP2.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=3 group by l.Id, Consumo) as ConsumoRP3 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP3.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=4 group by l.Id, Consumo) as ConsumoRP4 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP4.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
		where  FacturaConcepto like '40%')  as l where CodPeriodo=5 group by l.Id, Consumo) as ConsumoRP5 
		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP5.Id
		left join 		
		(Select Id, Consumo, Max(Precio) as Precio from(
		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
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
inner join LineasFactura on LineasFactura.IdFacturaVentaCabecera = PreseleccionFacturas.IdFacturaVentaCabecera
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
--,SECTOR (SiempreElec)
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
--,FORMA DE PAGO (SIEMPRE TRANSFERENCIA)
,fvc.SerieFactura+' '+cast (fvc.NumeroFactura as varchar) As NumeroFactura
,fvc.idfacturaorigen
,convert(varchar, fvc.FechaLecturaAnteriorXML, 103) as FechaDesde
,convert(varchar,fvc.FechaLecturaActualXML, 103) as FechaHasta
,ISNULL(fvc.InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceraHistorialConsumos/PeriodoConsumo/FacturaInfoCabeceraHistorialPeriodoConsumoDTO/ConsumoActiva)[1]', 'decimal(18,3)'), 0) as ConsumoTotalKwh
--,T_POTENCIA(€) Columna AC
--,T_ENERGIA(€) Columna AD
--,T_REACTIVA(€) 
,replace(fvlIE.ImporteBase,'.',',') As ImpuestoElectrico
,replace(fvlCON.ImporteBase,'.',',') As AlquilerContador
,fvt.PorcentajeImpuesto As PorcentajeIVA
,replace(fvt.ImporteBase,'.',',') As BaseIVA
,replace(fvt.ImporteImpuesto,'.',',') As ImporteIVA
--IMPORTE POTENCIA PERIODO 1 Columna AK
--PRECIO POTENCIA PERIODO 1 Columna AL
,replace(ll1.maximetro,'.',',') as PotenciaMaxPeriodo1
--Potencia a Facturar p1 (lo sacamos previamente)
,replace(ll2.maximetro,'.',',') as PotenciaMaxPeriodo2
--Potencia a Facturar p2 (lo sacamos previamente)
,replace(ll3.maximetro,'.',',') as PotenciaMaxPeriodo3
--Potencia a Facturar p3 (lo sacamos previamente)
,replace(ll4.maximetro,'.',',') as PotenciaMaxPeriodo4
--Potencia a Facturar p4 (lo sacamos previamente)
,replace(ll5.maximetro,'.',',') as PotenciaMaxPeriodo5
--Potencia a Facturar p5 (lo sacamos previamente)
,replace(ll6.maximetro,'.',',') as PotenciaMaxPeriodo6
--Potencia a Facturar p6 (lo sacamos previamente)
,replace(tpp1a.PotenciaPrecio,'.',',') as PrecioPotenciaP1A
,replace(tpp1a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP1A
,replace(tpp2a.PotenciaPrecio,'.',',') as PrecioPotenciaP2A
,replace(tpp2a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP2A
,replace(tpp3a.PotenciaPrecio,'.',',') as PrecioPotenciaP3A
,replace(tpp3a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP3A
,replace(tpp4a.PotenciaPrecio,'.',',') as PrecioPotenciaP4A
,replace(tpp4a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP4A
,replace(tpp5a.PotenciaPrecio,'.',',') as PrecioPotenciaP5A
,replace(tpp5a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP5A
,replace(tpp6a.PotenciaPrecio,'.',',') as PrecioPotenciaP6A
,replace(tpp6a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP6A
,replace(tpp1a.EnergiaPrecio,'.',',') as PrecioEnergiaP1A
,replace(tpp1a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP1A
,replace(tpp2a.EnergiaPrecio,'.',',') as PrecioEnergiaP2A
,replace(tpp2a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP2A
,replace(tpp3a.EnergiaPrecio,'.',',') as PrecioEnergiaP3A
,replace(tpp3a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP3A
,replace(tpp4a.EnergiaPrecio,'.',',') as PrecioEnergiaP4A
,replace(tpp4a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP4A
,replace(tpp5a.EnergiaPrecio,'.',',') as PrecioEnergiaP5A
,replace(tpp5a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP5A
,replace(tpp6a.EnergiaPrecio,'.',',') as PrecioEnergiaP6A
,replace(tpp6a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP6A
,replace(tpp1b.PotenciaPrecio,'.',',') as PrecioPotenciaP1B
,replace(tpp1b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP1B
,replace(tpp2b.PotenciaPrecio,'.',',') as PrecioPotenciaP2B
,replace(tpp2b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP2B
,replace(tpp3b.PotenciaPrecio,'.',',') as PrecioPotenciaP3B
,replace(tpp3b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP3B
,replace(tpp4b.PotenciaPrecio,'.',',') as PrecioPotenciaP4B
,replace(tpp4b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP4B
,replace(tpp5b.PotenciaPrecio,'.',',') as PrecioPotenciaP5B
,replace(tpp5b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP5B
,replace(tpp6b.PotenciaPrecio,'.',',') as PrecioPotenciaP6B
,replace(tpp6b.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP6B
,replace(tpp1b.EnergiaPrecio,'.',',') as PrecioEnergiaP1B
,replace(tpp1b.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP1B
,replace(tpp2b.EnergiaPrecio,'.',',') as PrecioEnergiaP2B
,replace(tpp2b.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP2B
,replace(tpp3b.EnergiaPrecio,'.',',') as PrecioEnergiaP3
,replace(tpp3b.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP3B
,replace(tpp4b.EnergiaPrecio,'.',',') as PrecioEnergiaP4B
,replace(tpp4b.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP4B
,replace(tpp5b.EnergiaPrecio,'.',',') as PrecioEnergiaP5B
,replace(tpp5b.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP5B
,replace(tpp6b.EnergiaPrecio,'.',',') as PrecioEnergiaP6B
,replace(tpp6b.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP6B
,replace(fvlMAX.ImporteBase,'.',',') as ImporteMaximetro
,replace(ll1.ConsumoActiva,'.',',') as ConsumoActivaP1
,replace(ll1.ActivaExtra,'.',',') as ActivaExtraP1
,replace(ll2.ConsumoActiva,'.',',') as ConsumoActivaP2
,replace(ll2.ActivaExtra,'.',',') as ActivaExtraP2
,replace(ll3.ConsumoActiva,'.',',') as ConsumoActivaP3
,replace(ll3.ActivaExtra,'.',',') as ActivaExtraP3
,replace(ll4.ConsumoActiva,'.',',') as ConsumoActivaP4
,replace(ll4.ActivaExtra,'.',',') as ActivaExtraP4
,replace(ll5.ConsumoActiva,'.',',') as ConsumoActivaP5
,replace(ll5.ActivaExtra,'.',',') as ActivaExtraP5
,replace(ll6.ConsumoActiva,'.',',') as ConsumoActivaP6
,replace(ll6.ActivaExtra,'.',',') as ActivaExtraP6
,replace(cr.r1,'.',',') as ConsumoReactivaP1
,replace(cr.r2,'.',',') as ConsumoReactivaP2
,replace(cr.r3,'.',',') as ConsumoReactivaP3
,replace(cr.r4,'.',',') as ConsumoReactivaP4
,replace(cr.r5,'.',',') as ConsumoReactivaP5
,replace(cr.r6,'.',',') as ConsumoReactivaP6
,replace(cr.pr1,'.',',') as PrecioReactivaP1
,replace(cr.pr2,'.',',') as PrecioReactivaP2
,replace(cr.pr3,'.',',') as PrecioReactivaP3
,replace(cr.pr4,'.',',') as PrecioReactivaP4
,replace(cr.pr5,'.',',') as PrecioReactivaP5
,replace(cr.pr6,'.',',') as PrecioReactivaP6
,(ISNULL(cr.r1,0.0) * ISNULL(cr.pr1, 0.0) + ISNULL(cr.r2,0.0) * ISNULL(cr.pr2, 0.0) 
+ ISNULL(cr.r3,0.0) * ISNULL(cr.pr3,0.0) + ISNULL(cr.r4,0.0) * ISNULL(cr.pr4, 0.0) 
+ ISNULL(cr.r5,0.0) * ISNULL(cr.pr5, 0.0) + ISNULL(cr.r6,0.0) * ISNULL(cr.pr6, 0.0)) as CosteReactiva
,replace(ll1.activaActual,'.',',') as LectActivaP1
,replace(ll2.activaActual,'.',',') as LectActivaP2
,replace(ll3.activaActual,'.',',') as LectActivaP3
,replace(ll4.activaActual,'.',',') as LectActivaP4
,replace(ll5.activaActual,'.',',') as LectActivaP5
,replace(ll6.activaActual,'.',',') as LectActivaP6
,replace(ll1.ActivaAnterior,'.',',') as LectAntActivaP1
,replace(ll2.ActivaAnterior,'.',',') as LectAntActivaP2
,replace(ll3.ActivaAnterior,'.',',') as LectAntActivaP3
,replace(ll4.ActivaAnterior,'.',',') as LectAntActivaP4
,replace(ll5.ActivaAnterior,'.',',') as LectAntActivaP5
,replace(ll6.ActivaAnterior,'.',',') as LectAntActivaP6
,replace(ll1.ReactivaActual,'.',',') as LectReActivaP1
,replace(ll2.ReactivaActual,'.',',') as LectReActivaP2
,replace(ll3.ReactivaActual,'.',',') as LectReActivaP3
,replace(ll4.ReactivaActual,'.',',') as LectReActivaP4
,replace(ll5.ReactivaActual,'.',',') as LectReActivaP5
,replace(ll6.ReactivaActual,'.',',') as LectReActivaP6
,replace(ll1.ReActivaAnterior,'.',',') as LectAntReActivaP1
,replace(ll2.ReActivaAnterior,'.',',') as LectAntReActivaP2
,replace(ll3.ReActivaAnterior,'.',',') as LectAntReActivaP3
,replace(ll4.ReActivaAnterior,'.',',') as LectAntReActivaP4
,replace(ll5.ReActivaAnterior,'.',',') as LectAntReActivaP5
,replace(ll6.ReActivaAnterior,'.',',') as LectAntReActivaP6
,replace(ll1.Maximetro,'.',',') as LectMaximetroP1
,replace(ll2.Maximetro,'.',',') as LectMaximetroP2
,replace(ll3.Maximetro,'.',',') as LectMaximetroP3
,replace(ll4.Maximetro,'.',',') as LectMaximetroP4
,replace(ll5.Maximetro,'.',',') as LectMaximetroP5
,replace(ll6.Maximetro,'.',',') as LectMaximetroP6
,replace(isnull(ImportesPotencia.Importe, 0),'.',',') as ImportePotenciaContratacion
,replace(isnull(ImportesEnergia.Importe, 0),'.',',') as ImporteEnergiaContratacion
,replace(isnull(ImportesReactiva.Importe, 0),'.',',') as ImporteReactivaContratacion
,replace(isnull(ImportesExcesos.Importe, 0),'.',',') as ImporteExcesosContratacion
,replace(isnull(DescuentosPotencia.Importe, 0),'.',',') as DescuentoPotenciaContratacion
,replace(isnull(DescuentosEnergia.Importe, 0),'.',',') as DescuentoEnergiaContratacion
,replace(isnull(ImportesAlquileres.Importe, 0),'.',',') as ImporteAlquilerContratacion
,replace(isnull(ImportesProductos.Importe, 0),'.',',') as ImporteProductosContratacion
,replace(fvlP1.importebase,'.',',') as ImporteVariableP1
,replace(fvlP2.importebase,'.',',') as ImporteVariableP2
,replace(fvlP3.importebase,'.',',') as ImporteVariableP3
,replace(fvlP4.importebase,'.',',') as ImporteVariableP4
,replace(fvlP5.importebase,'.',',') as ImporteVariableP5
,replace(fvlP6.importebase,'.',',') as ImporteVariableP6
,replace(fvlCAP.importebase,'.',',') as ImporteCAPGAS
,replace(PrecioVariableP1,'.',',') AS PrecioVariableP1
,replace(PrecioVariableP2,'.',',') AS PrecioVariableP2
,replace(PrecioVariableP3,'.',',') AS PrecioVariableP3
,replace(PrecioVariableP4,'.',',') AS PrecioVariableP4
,replace(PrecioVariableP5,'.',',') AS PrecioVariableP5
,replace(PrecioVariableP6,'.',',') AS PrecioVariableP6
,replace(PrecioCargoPontenciaP1,'.',',')  PrecioCargoPontenciaP1
,replace(PrecioCargoPontenciaP2,'.',',')PrecioCargoPontenciaP2
,replace(PrecioCargoPontenciaP3,'.',',')PrecioCargoPontenciaP3
,replace(PrecioCargoPontenciaP4,'.',',')PrecioCargoPontenciaP4
,replace(PrecioCargoPontenciaP5,'.',',')PrecioCargoPontenciaP5
,replace(PrecioCargoPontenciaP6,'.',',') PrecioCargoPontenciaP6
,replace(PrecioCargoEnergiaP1,'.',',') AS PrecioCargoEnergiaP1
,replace(PrecioCargoEnergiaP2,'.',',') AS PrecioCargoEnergiaP2
,replace(PrecioCargoEnergiaP3,'.',',') AS PrecioCargoEnergiaP3
,replace(PrecioCargoEnergiaP4,'.',',') AS PrecioCargoEnergiaP4
,replace(PrecioCargoEnergiaP5,'.',',') AS PrecioCargoEnergiaP5
,replace(PrecioCargoEnergiaP6,'.',',') AS PrecioCargoEnergiaP6
,replace(ISNULL(fvlclick.ImporteBase,0),'.',',') As ImporteClick
,fvlClickAjuste.descripcion As DescripcionAjusteClick
,replace(fvlClickAjuste.ImporteBase,'.',',') As ImporteAjusteClick
from contrato c with (nolock)
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
inner join FacturaVentaCabecera fvc with (nolock) on fvc.codigocontrato = c.codigocontrato
inner join FacturaVentaTotal fvt with (nolock) on fvc.idfacturaventacabecera = fvt.idfacturaventacabecera
inner join CarteraCobro cc with (nolock) on fvc.idfacturaventacabecera = cc.idfacturaventacabecera
inner join Tarifa t with (nolock) on fvc.IdTarifaPeajeXML = t.IdTarifa
inner join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
left join LineasFactura fvlIE with (nolock) on fvc.idfacturaventacabecera = fvlIE.idfacturaventacabecera and fvlIE.Facturaconcepto=60001
left join --nuevo Erick
    (select IdFacturaVentaCabecera, sum(ImporteBase) importebase 
     from LineasFactura with (nolock) 
     where FacturaConcepto = 50002
     group by IdFacturaVentaCabecera
    ) as fvlCON on fvc.idfacturaventacabecera = fvlCON.idfacturaventacabecera
left join LineasFactura fvlMAX with (nolock) on fvc.idfacturaventacabecera = fvlMAX.idfacturaventacabecera and fvlMAX.Facturaconcepto=20006
inner join lectura l  with (nolock) on fvc.idfacturaventacabecera = l.IdFacturaVentaCabeceraSectorC or fvc.IdFacturaOrigen = l.IdFacturaVentaCabeceraSectorC
left join LecturaLinea ll1 with (nolock) on l.IdLectura = ll1.IdLectura and ll1.idtarifapeajeperiodolectura in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join LecturaLinea ll2 with (nolock) on l.IdLectura = ll2.IdLectura	and ll2.idtarifapeajeperiodolectura in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join LecturaLinea ll3 with (nolock) on l.IdLectura = ll3.IdLectura	and ll3.idtarifapeajeperiodolectura in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join LecturaLinea ll4 with (nolock) on l.IdLectura = ll4.IdLectura	and ll4.idtarifapeajeperiodolectura in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join LecturaLinea ll5 with (nolock) on l.IdLectura = ll5.IdLectura	and ll5.idtarifapeajeperiodolectura in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join LecturaLinea ll6 with (nolock) on l.IdLectura = ll6.IdLectura	and ll6.idtarifapeajeperiodolectura in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join TarifaPeajePrecio tpp1a with (nolock) on l.IdTarifaPeaje = tpp1a.IdTarifaPeaje and tpp1a.fechaInicio='01/01/2022' and tpp1a.idtarifapeajeperiodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join TarifaPeajePrecio tpp2a with (nolock) on l.IdTarifaPeaje = tpp2a.IdTarifaPeaje and tpp2a.fechaInicio='01/01/2022' and tpp2a.idtarifapeajeperiodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join TarifaPeajePrecio tpp3a with (nolock) on l.IdTarifaPeaje = tpp3a.IdTarifaPeaje and tpp3a.fechaInicio='01/01/2022' and tpp3a.idtarifapeajeperiodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join TarifaPeajePrecio tpp4a with (nolock) on l.IdTarifaPeaje = tpp4a.IdTarifaPeaje and tpp4a.fechaInicio='01/01/2022' and tpp4a.idtarifapeajeperiodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join TarifaPeajePrecio tpp5a with (nolock) on l.IdTarifaPeaje = tpp5a.IdTarifaPeaje and tpp5a.fechaInicio='01/01/2022' and tpp5a.idtarifapeajeperiodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join TarifaPeajePrecio tpp6a with (nolock) on l.IdTarifaPeaje = tpp6a.IdTarifaPeaje and tpp6a.fechaInicio='01/01/2022' and tpp6a.idtarifapeajeperiodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join TarifaPeajePrecio tpp1b with (nolock) on l.IdTarifaPeaje = tpp1b.IdTarifaPeaje and tpp1b.fechafinal is null and tpp1b.idtarifapeajeperiodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join TarifaPeajePrecio tpp2b with (nolock) on l.IdTarifaPeaje = tpp2b.IdTarifaPeaje and tpp2b.fechafinal is null and tpp2b.idtarifapeajeperiodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join TarifaPeajePrecio tpp3b with (nolock) on l.IdTarifaPeaje = tpp3b.IdTarifaPeaje and tpp3b.fechafinal is null and tpp3b.idtarifapeajeperiodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join TarifaPeajePrecio tpp4b with (nolock) on l.IdTarifaPeaje = tpp4b.IdTarifaPeaje and tpp4b.fechafinal is null and tpp4b.idtarifapeajeperiodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join TarifaPeajePrecio tpp5b with (nolock) on l.IdTarifaPeaje = tpp5b.IdTarifaPeaje and tpp5b.fechafinal is null and tpp5b.idtarifapeajeperiodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join TarifaPeajePrecio tpp6b with (nolock) on l.IdTarifaPeaje = tpp6b.IdTarifaPeaje and tpp6b.fechafinal is null and tpp6b.idtarifapeajeperiodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join ConsumosReactiva cr with (nolock) ON cr.id = fvc.IdFacturaVentaCabecera	
left join ImportesPotencia on ImportesPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesEnergia on ImportesEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesReactiva on ImportesReactiva.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesExcesos on ImportesExcesos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join DescuentosPotencia on DescuentosPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join DescuentosEnergia on DescuentosEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join ImportesAlquileres on ImportesAlquileres.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join ImportesProductos on ImportesProductos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)) PrecioVariableP1
     from LineasFactura with (nolock) 
     where FacturaConcepto = 30004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
     group by IdFacturaVentaCabecera
    ) as fvlP1 on fvc.idfacturaventacabecera = fvlP1.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)) PrecioVariableP2
     from LineasFactura with (nolock) 
     where FacturaConcepto = 30004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
     group by IdFacturaVentaCabecera
    ) as fvlP2 on fvc.idfacturaventacabecera = fvlP2.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)) PrecioVariableP3
     from LineasFactura with (nolock) 
     where FacturaConcepto = 30004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
     group by IdFacturaVentaCabecera
    ) as fvlP3 on fvc.idfacturaventacabecera = fvlP3.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)) PrecioVariableP4
     from LineasFactura with (nolock) 
     where FacturaConcepto = 30004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
     group by IdFacturaVentaCabecera
    ) as fvlP4 on fvc.idfacturaventacabecera = fvlP4.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)) PrecioVariableP5
     from LineasFactura with (nolock) 
     where FacturaConcepto = 30004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
     group by IdFacturaVentaCabecera
    ) as fvlP5 on fvc.idfacturaventacabecera = fvlP5.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)) PrecioVariableP6
     from LineasFactura with (nolock) 
     where FacturaConcepto = 30004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
     group by IdFacturaVentaCabecera
    ) as fvlP6 on fvc.idfacturaventacabecera = fvlP6.idfacturaventacabecera
left join LineasFactura fvlCAP with (nolock) on fvc.idfacturaventacabecera = fvlCAP.idfacturaventacabecera and fvlCAP.Facturaconcepto=30004 and fvlCAP.IsAjusteCAPGas=1
left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase, sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0))  PrecioCargoPontenciaP1
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130002 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
     group by IdFacturaVentaCabecera
    ) as fvlpP1  on fvc.idfacturaventacabecera = fvlpP1.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase , sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0))  PrecioCargoPontenciaP2
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130002 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
     group by IdFacturaVentaCabecera
    ) as fvlpP2  on fvc.idfacturaventacabecera = fvlpP2.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase , sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0))  PrecioCargoPontenciaP3
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130002 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
     group by IdFacturaVentaCabecera
    ) as fvlpP3  on fvc.idfacturaventacabecera = fvlpP3.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase , sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0))  PrecioCargoPontenciaP4
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130002 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
     group by IdFacturaVentaCabecera
    ) as fvlpP4  on fvc.idfacturaventacabecera = fvlpP4.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase , sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0))  PrecioCargoPontenciaP5
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130002 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
     group by IdFacturaVentaCabecera
    ) as fvlpP5  on fvc.idfacturaventacabecera = fvlpP5.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase , sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0))  PrecioCargoPontenciaP6
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130002 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
     group by IdFacturaVentaCabecera
    ) as fvlpP6  on fvc.idfacturaventacabecera = fvlpP6.idfacturaventacabecera

	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase , sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)) PrecioCargoEnergiaP1
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
     group by IdFacturaVentaCabecera
    ) as fvleP1  on fvc.idfacturaventacabecera = fvleP1.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)) PrecioCargoEnergiaP2
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
     group by IdFacturaVentaCabecera
    ) as fvleP2  on fvc.idfacturaventacabecera = fvleP2.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)) PrecioCargoEnergiaP3
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
     group by IdFacturaVentaCabecera
    ) as fvleP3  on fvc.idfacturaventacabecera = fvleP3.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)) PrecioCargoEnergiaP4
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
     group by IdFacturaVentaCabecera
    ) as fvleP4  on fvc.idfacturaventacabecera = fvleP4.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)) PrecioCargoEnergiaP5
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
     group by IdFacturaVentaCabecera
    ) as fvleP5  on fvc.idfacturaventacabecera = fvleP5.idfacturaventacabecera
	left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)) PrecioCargoEnergiaP6
     from LineasFactura with (nolock) 
     where FacturaConcepto = 130004 and InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
     group by IdFacturaVentaCabecera
    ) as fvleP6  on fvc.idfacturaventacabecera = fvleP6.idfacturaventacabecera
--left join facturaventalinea fvlClickP1 with (nolock) on fvc.idfacturaventacabecera = fvlClickP1.idfacturaventacabecera and fvlClickP1.Facturaconcepto=30006 and fvlClickP1.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
--left join facturaventalinea fvlClickP2 with (nolock) on fvc.idfacturaventacabecera = fvlClickP2.idfacturaventacabecera and fvlClickP2.Facturaconcepto=30006 and fvlClickP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
--left join facturaventalinea fvlClickP3 with (nolock) on fvc.idfacturaventacabecera = fvlClickP3.idfacturaventacabecera and fvlClickP3.Facturaconcepto=30006 and fvlClickP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
--left join facturaventalinea fvlClickP4 with (nolock) on fvc.idfacturaventacabecera = fvlClickP4.idfacturaventacabecera and fvlClickP4.Facturaconcepto=30006 and fvlClickP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
--left join facturaventalinea fvlClickP5 with (nolock) on fvc.idfacturaventacabecera = fvlClickP5.idfacturaventacabecera and fvlClickP5.Facturaconcepto=30006 and fvlClickP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
--left join facturaventalinea fvlClickP6 with (nolock) on fvc.idfacturaventacabecera = fvlClickP6.idfacturaventacabecera and fvlClickP6.Facturaconcepto=30006 and fvlClickP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
left join ImporteClick fvlClick with (nolock) on fvc.idfacturaventacabecera = fvlClick.idfacturaventacabecera
left join facturaventalinea fvlClickAjuste with (nolock) on fvc.idfacturaventacabecera = fvlClickAjuste.idfacturaventacabecera and fvlClickAjuste.Facturaconcepto=30006 and fvlClickAjuste.isajustecapgas=1
where fvc.idfacturaventacabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)

"