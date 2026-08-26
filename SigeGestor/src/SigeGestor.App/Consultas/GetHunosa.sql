--Erick _Gestor Sige_ Hunosa
With XMLNAMESPACES('http://localhost/elegibilidad' as "XS") 
,FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturaVentaCabecera where idcliente=50320 and SerieFactura is not null and Entorno='E1'
and FechaFactura>='DesdeFechaReplace' and FechaFactura<='hastaFechaReplace')),
LineasFactura as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,InfoLineaXML,ImporteBase,IsAjusteCAPGas
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
--group by  IdFacturaVentaCabecera,
),LineaPrecioCargosEN1 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv1
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130004 and CodigoPeriodoXML=1)
,LineaAgrupadoPrecioCargosEN1 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv1) PCE1 from LineaPrecioCargosEN1
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosEN2 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv2
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130004 and CodigoPeriodoXML=2)
,LineaAgrupadoPrecioCargosEN2 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv2) PCE2 from LineaPrecioCargosEN2
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosEN3 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv3
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130004 and CodigoPeriodoXML=3)
,LineaAgrupadoPrecioCargosEN3 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv3) PCE3 from LineaPrecioCargosEN3
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosEN4 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv4
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130004 and CodigoPeriodoXML=4)
,LineaAgrupadoPrecioCargosEN4 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv4) PCE4 from LineaPrecioCargosEN4
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosEN5 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv5
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130004 and CodigoPeriodoXML=5)
,LineaAgrupadoPrecioCargosEN5 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv5) PCE5 from LineaPrecioCargosEN5
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosEN6 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv6
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130004 and CodigoPeriodoXML=6)
,LineaAgrupadoPrecioCargosEN6 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv6) PCE6 from LineaPrecioCargosEN6
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosPotencia1 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv1
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130002 and CodigoPeriodoXML=1)
,LineaAgrupadoPrecioCargosPotencia1 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv1) PCP1 from LineaPrecioCargosPotencia1
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosPotencia2 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv2
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130002 and CodigoPeriodoXML=2)
,LineaAgrupadoPrecioCargosPotencia2 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv2) PCP2 from LineaPrecioCargosPotencia2
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosPotencia3 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv3
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130002 and CodigoPeriodoXML=3)
,LineaAgrupadoPrecioCargosPotencia3 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv3) PCP3 from LineaPrecioCargosPotencia3
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosPotencia4 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv4
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130002 and CodigoPeriodoXML=4)
,LineaAgrupadoPrecioCargosPotencia4 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv4) PCP4 from LineaPrecioCargosPotencia4
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosPotencia5 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv5
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130002 and CodigoPeriodoXML=5)
,LineaAgrupadoPrecioCargosPotencia5 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv5) PCP5 from LineaPrecioCargosPotencia5
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),LineaPrecioCargosPotencia6 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) pv6
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=130002 and CodigoPeriodoXML=6)
,LineaAgrupadoPrecioCargosPotencia6 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv6) PCP6 from LineaPrecioCargosPotencia6
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),
LineaCapGas as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(ImporteBase) ImporteCapGas
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and IsAjusteCAPGas=1
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)
,LineaPrecioVariableP1 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0) pv1
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=1)
,LineaAgrupadoPrecioVariableP1 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv1) pv1 from LineaPrecioVariableP1
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)
,LineaPrecioVariableP2 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0) pv2
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=2)
,LineaAgrupadoPrecioVariableP2 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv2) pv2 from LineaPrecioVariableP2
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)
,LineaPrecioVariableP3 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0) pv3
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=3)
,LineaAgrupadoPrecioVariableP3 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv3) pv3 from LineaPrecioVariableP3
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)
,LineaPrecioVariableP4 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0) pv4
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=4)
,LineaAgrupadoPrecioVariableP4 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv4) pv4 from LineaPrecioVariableP4
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)
,LineaPrecioVariableP5 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0) pv5
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=5)
,LineaAgrupadoPrecioVariableP5 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv5) pv5 from LineaPrecioVariableP5
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)
,LineaPrecioVariableP6 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0) pv6
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=6)
,LineaAgrupadoPrecioVariableP6 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(pv6) pv6 from LineaPrecioVariableP6
group  by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)
--left join LineasFactura fvlpP1 with (nolock) on fvc.idfacturaventacabecera = fvlpP1.idfacturaventacabecera and fvlpP1.Facturaconcepto=130002 and fvlpP1.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
--,replace(ISNULL(fvlP1.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioVariableP1
,LineasVariable as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,
case when CodigoPeriodoXML =1 then sum(ImporteBase) else 0 end  ImporteVariableP1
--sum(ImporteBase) 
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=1
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto,CodigoPeriodoXML),
LineasVariable2 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,
case when CodigoPeriodoXML =2 then sum(ImporteBase) else 0 end  ImporteVariableP2
--sum(ImporteBase) 
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=2
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto,CodigoPeriodoXML
),
LineasVariable3 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,
case when CodigoPeriodoXML =3 then sum(ImporteBase) else 0 end  ImporteVariableP3
--sum(ImporteBase) 
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=3
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto,CodigoPeriodoXML),
LineasVariable4 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,
case when CodigoPeriodoXML =4 then sum(ImporteBase) else 0 end  ImporteVariableP4
--sum(ImporteBase) 
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=4
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto,CodigoPeriodoXML),
LineasVariable5 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,
case when CodigoPeriodoXML =5 then sum(ImporteBase) else 0 end  ImporteVariableP5
--sum(ImporteBase) 
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=5
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto,CodigoPeriodoXML),
LineasVariable6 as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,
case when CodigoPeriodoXML =6 then sum(ImporteBase) else 0 end  ImporteVariableP6
--sum(ImporteBase) 
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=30004 and CodigoPeriodoXML=6
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto,CodigoPeriodoXML
)
,
LineaAlquiler as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(ImporteBase) ImporteBaseAlquiler
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=50002
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
),
LineaImpuestoElectrico as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(ImporteBase) ImporteBaseElectrico
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=60001
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)
,
LineaMaximetro as (
select IdFacturaVentaCabecera,Entorno,FacturaConcepto,sum(ImporteBase) ImporteBaseMaximetro
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=20006
group by  IdFacturaVentaCabecera,Entorno,FacturaConcepto
)

,
ConsumosReactiva(id, r1,r2,r3,r4,r5,r6,pr1,pr2,pr3,pr4,pr5,pr6)
AS
(
	select facturaventacabecera.IdFacturaventaCabecera,
	ConsumoRP1.Consumo as ConsumoP1, ConsumoRP2.Consumo as ConsumoP2, ConsumoRP3.Consumo as ConsumoP3, ConsumoRP4.Consumo as ConsumoP4, ConsumoRP5.Consumo as ConsumoP5, ConsumoRP6.Consumo as ConsumoP6, 
	ConsumoRP1.Precio as EnergiaPrecioP1, ConsumoRP2.Precio as EnergiaPrecioP2, ConsumoRP3.Precio as EnergiaPrecioP3, ConsumoRP4.Precio as EnergiaPrecioP4, ConsumoRP5.Precio as EnergiaPrecioP5, ConsumoRP6.Precio as EnergiaPrecioP6
	
	from facturaventacabecera WITH (NOLOCK)
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
cl.Identidad as CIFDNI
,dbo.formateardenominacion(cl.nombre,cl.apellido1,cl.apellido2,cl.razonsocial) as RazonSocial
,cll.NombreCalle +' '+ cups.Aclarador as Direccion
,CUPS.CodigoCUPS
,cups.CodPostal
,ciu.TextoCiudad as Poblacion
,pv.TextoProvincia as Provincia
,c.codigocontrato
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
,fvc.SerieFactura+' '+cast (fvc.NumeroFactura as varchar) As NumeroFactura
,fvc.idfacturaorigen
,convert(varchar, fvc.FechaLecturaAnteriorXML, 103) as FechaDesde
,convert(varchar,fvc.FechaLecturaActualXML, 103) as FechaHasta
,ISNULL(fvc.InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceraHistorialConsumos/PeriodoConsumo/FacturaInfoCabeceraHistorialPeriodoConsumoDTO/ConsumoActiva)[1]', 'decimal(18,3)'), 0) as ConsumoTotalKwh
,replace(LElectrico.ImporteBaseElectrico,'.',',') As ImpuestoElectrico
,replace(LAlquiler.ImporteBaseAlquiler,'.',',') As AlquilerContador
,fvt.PorcentajeImpuesto As PorcentajeIVA
,replace(fvt.ImporteBase,'.',',') As BaseIVA
,replace(fvt.ImporteImpuesto,'.',',') As ImporteIVA
,replace(ll1.maximetro,'.',',') as PotenciaMaxPeriodo1
,replace(ll2.maximetro,'.',',') as PotenciaMaxPeriodo2
,replace(ll3.maximetro,'.',',') as PotenciaMaxPeriodo3
,replace(ll4.maximetro,'.',',') as PotenciaMaxPeriodo4
,replace(ll5.maximetro,'.',',') as PotenciaMaxPeriodo5
,replace(ll6.maximetro,'.',',') as PotenciaMaxPeriodo6
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
,replace(LMaxi.ImporteBaseMaximetro,'.',',') as ImporteMaximetro
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
,replace((ISNULL(cr.r1,0.0) * ISNULL(cr.pr1, 0.0) + ISNULL(cr.r2,0.0) * ISNULL(cr.pr2, 0.0) 
+ ISNULL(cr.r3,0.0) * ISNULL(cr.pr3,0.0) + ISNULL(cr.r4,0.0) * ISNULL(cr.pr4, 0.0) 
+ ISNULL(cr.r5,0.0) * ISNULL(cr.pr5, 0.0) + ISNULL(cr.r6,0.0) * ISNULL(cr.pr6, 0.0)),'.',',') as CosteReactiva
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
,replace(ISNULL(LineasVariable.ImporteVariableP1,0),'.',',') as ImporteVariableP1
,replace(ISNULL(LineasVariable2.ImporteVariableP2,0),'.',',') as ImporteVariableP2
,replace(ISNULL(LineasVariable3.ImporteVariableP3,0),'.',',') as ImporteVariableP3
,replace(ISNULL(LineasVariable4.ImporteVariableP4,0),'.',',') as ImporteVariableP4
,replace(ISNULL(LineasVariable5.ImporteVariableP5,0),'.',',') as ImporteVariableP5
,replace(ISNULL(LineasVariable6.ImporteVariableP6,0),'.',',') as ImporteVariableP6
,replace(ISNULL(LineaCapGas.ImporteCapGas,0),'.',',') as ImporteCAPGAS
,replace(ISNULL(LAPV1.pv1, 0),'.',',') AS PrecioVariableP1
,replace(ISNULL(LAPV2.pv2, 0),'.',',') AS PrecioVariableP2
,replace(ISNULL(LAPV3.pv3, 0),'.',',') AS PrecioVariableP3
,replace(ISNULL(LAPV4.pv4, 0),'.',',') AS PrecioVariableP4
,replace(ISNULL(LAPV5.pv5, 0),'.',',') AS PrecioVariableP5
,replace(ISNULL(LAPV6.pv6, 0),'.',',') AS PrecioVariableP6
,replace(ISNULL(LCP1.PCP1,0),'.',',') AS PrecioCargoPontenciaP1
,replace(ISNULL(LCP2.PCP2,0),'.',',')  AS PrecioCargoPontenciaP2
,replace(ISNULL(LCP3.PCP3,0),'.',',')  AS PrecioCargoPontenciaP3
,replace(ISNULL(LCP4.PCP4,0),'.',',')  AS PrecioCargoPontenciaP4
,replace(ISNULL(LCP5.PCP5,0),'.',',')  AS PrecioCargoPontenciaP5
,replace(ISNULL(LCP6.PCP6,0),'.',',')  AS PrecioCargoPontenciaP6
,replace(ISNULL(LCEN1.PCE1,0),'.',',') AS PrecioCargoEnergiaP1
,replace(ISNULL(LCEN2.PCE2,0),'.',',') AS PrecioCargoEnergiaP2
,replace(ISNULL(LCEN3.PCE3,0),'.',',') AS PrecioCargoEnergiaP3
,replace(ISNULL(LCEN4.PCE4,0),'.',',') AS PrecioCargoEnergiaP4
,replace(ISNULL(LCEN5.PCE5,0),'.',',') AS PrecioCargoEnergiaP5
,replace(ISNULL(LCEN6.PCE6,0),'.',',') AS PrecioCargoEnergiaP6
,Replace(fcc.FacturaXML.value('(//XS:Periodo/XS:ValorEnergiaCapacitiva)[1]', 'nvarchar(max)'),'.',',') As EnergiaCapacitiva
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
inner join Tarifa t with (nolock) on cast(fvc.IdTarifaPeajeXML as bigint) = t.IdTarifa
inner join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
inner join lectura l  with (nolock) on fvc.idfacturaventacabecera = l.IdFacturaVentaCabeceraSectorC or (fvc.IdFacturaOrigen = l.IdFacturaVentaCabeceraSectorC and fvc.SerieFactura like '%ABO%')
left join FacturaCompraCabecera fcc on l.IdFacturaCompraCabecera = fcc.IdFacturaCompraCabecera
left join LecturaLinea ll1 with (nolock) on l.IdLectura = ll1.IdLectura and ll1.idtarifapeajeperiodolectura in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join LecturaLinea ll2 with (nolock) on l.IdLectura = ll2.IdLectura	and ll2.idtarifapeajeperiodolectura in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join LecturaLinea ll3 with (nolock) on l.IdLectura = ll3.IdLectura	and ll3.idtarifapeajeperiodolectura in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join LecturaLinea ll4 with (nolock) on l.IdLectura = ll4.IdLectura	and ll4.idtarifapeajeperiodolectura in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join LecturaLinea ll5 with (nolock) on l.IdLectura = ll5.IdLectura	and ll5.idtarifapeajeperiodolectura in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join LecturaLinea ll6 with (nolock) on l.IdLectura = ll6.IdLectura	and ll6.idtarifapeajeperiodolectura in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join TarifaPeajePrecio tpp1a with (nolock) on l.IdTarifaPeaje = tpp1a.IdTarifaPeaje and tpp1a.fechaInicio='01/04/2025' and tpp1a.idtarifapeajeperiodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join TarifaPeajePrecio tpp2a with (nolock) on l.IdTarifaPeaje = tpp2a.IdTarifaPeaje and tpp2a.fechaInicio='01/04/2025' and tpp2a.idtarifapeajeperiodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join TarifaPeajePrecio tpp3a with (nolock) on l.IdTarifaPeaje = tpp3a.IdTarifaPeaje and tpp3a.fechaInicio='01/04/2025' and tpp3a.idtarifapeajeperiodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join TarifaPeajePrecio tpp4a with (nolock) on l.IdTarifaPeaje = tpp4a.IdTarifaPeaje and tpp4a.fechaInicio='01/04/2025' and tpp4a.idtarifapeajeperiodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join TarifaPeajePrecio tpp5a with (nolock) on l.IdTarifaPeaje = tpp5a.IdTarifaPeaje and tpp5a.fechaInicio='01/04/2025' and tpp5a.idtarifapeajeperiodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join TarifaPeajePrecio tpp6a with (nolock) on l.IdTarifaPeaje = tpp6a.IdTarifaPeaje and tpp6a.fechaInicio='01/04/2025' and tpp6a.idtarifapeajeperiodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join ConsumosReactiva cr with (nolock) ON cr.id = fvc.IdFacturaVentaCabecera	
left join ImportesPotencia on ImportesPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesEnergia on ImportesEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesReactiva on ImportesReactiva.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesExcesos on ImportesExcesos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join DescuentosPotencia on DescuentosPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join DescuentosEnergia on DescuentosEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join ImportesAlquileres on ImportesAlquileres.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join ImportesProductos on ImportesProductos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join LineasVariable on fvc.IdFacturaVentaCabecera = LineasVariable.IdFacturaVentaCabecera
left join LineasVariable2 on fvc.IdFacturaVentaCabecera = LineasVariable2.IdFacturaVentaCabecera
left join LineasVariable3 on fvc.IdFacturaVentaCabecera = LineasVariable3.IdFacturaVentaCabecera
left join LineasVariable4 on fvc.IdFacturaVentaCabecera = LineasVariable4.IdFacturaVentaCabecera
left join LineasVariable5 on fvc.IdFacturaVentaCabecera = LineasVariable5.IdFacturaVentaCabecera
left join LineasVariable6 on fvc.IdFacturaVentaCabecera = LineasVariable6.IdFacturaVentaCabecera
left join LineaCapGas on fvc.IdFacturaVentaCabecera = LineaCapGas.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioVariableP1 LAPV1 on fvc.IdFacturaVentaCabecera = LAPV1.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioVariableP2 LAPV2 on fvc.IdFacturaVentaCabecera = LAPV2.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioVariableP3 LAPV3 on fvc.IdFacturaVentaCabecera = LAPV3.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioVariableP4 LAPV4 on fvc.IdFacturaVentaCabecera = LAPV4.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioVariableP5 LAPV5 on fvc.IdFacturaVentaCabecera = LAPV5.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioVariableP6 LAPV6 on fvc.IdFacturaVentaCabecera = LAPV6.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosPotencia1 LCP1 on fvc.IdFacturaVentaCabecera = LCP1.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosPotencia2 LCP2 on fvc.IdFacturaVentaCabecera = LCP2.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosPotencia3 LCP3 on fvc.IdFacturaVentaCabecera = LCP3.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosPotencia4 LCP4 on fvc.IdFacturaVentaCabecera = LCP4.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosPotencia5 LCP5 on fvc.IdFacturaVentaCabecera = LCP5.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosPotencia6 LCP6 on fvc.IdFacturaVentaCabecera = LCP6.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosEN1 LCEN1 on fvc.IdFacturaVentaCabecera = LCEN1.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosEN2 LCEN2 on fvc.IdFacturaVentaCabecera = LCEN2.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosEN3 LCEN3 on fvc.IdFacturaVentaCabecera = LCEN3.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosEN4 LCEN4 on fvc.IdFacturaVentaCabecera = LCEN4.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosEN5 LCEN5 on fvc.IdFacturaVentaCabecera = LCEN5.IdFacturaVentaCabecera
left join LineaAgrupadoPrecioCargosEN6 LCEN6 on fvc.IdFacturaVentaCabecera = LCEN6.IdFacturaVentaCabecera
left join LineaAlquiler LAlquiler with (nolock) on fvc.idfacturaventacabecera =LAlquiler.IdFacturaVentaCabecera
left join LineaImpuestoElectrico LElectrico with (nolock) on fvc.idfacturaventacabecera =LElectrico.IdFacturaVentaCabecera
left join LineaMaximetro LMaxi  with (nolock) on fvc.idfacturaventacabecera =LMaxi.IdFacturaVentaCabecera
where fvc.idfacturaventacabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)