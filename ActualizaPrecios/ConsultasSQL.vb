Public Class ConsultasSQL
	' Consulta para obtener todos Clicks Luz
	Public Shared ReadOnly GetClickLuz As String =
		"select distinct Codigocontrato,Codigocups,
Textomulticlick
,CodigotipoMercado
,case CodigoTipoClick 
when 0 then 'Carga Base'
when 1 then 'Apuntado PassThrough'
when 2 then 'Apuntado base consumo real por periodo'
else 'SinTipo' end as TipoClick
,fechadesde
,fechahasta
,Fechaclick
,replace(PotenciaTotal,'.',',') As PotenciaTotal
,replace(Horas,'.',',') As Horas
,replace(PrecioConsumo,'.',',') as PrecioConsumo
,replace(TotalConsumo,'.',',') as TotalConsumo
,replace(preciofee,'.',',') as PrecioFee
,replace(multiclicklinea.porcentajereparto,'.',',') as PorcentajeReparto
from multiclickcabecera 
left join MultiClickLinea on MultiClickLinea.IdMultiClickCabecera = MultiClickCabecera.IdMultiClickCabecera
left join contrato c on c.IdContrato = MultiClickLinea.IdContrato
left join cups on cups.IdCups = c.idcups"

	' Consulta para obtener todos Clicks Gas
	Public Shared ReadOnly GetClickGas As String =
		"select  distinct
fi.IdFormulaIndexado
,CUPS.codigocups
,CodigoContrato
,c.IdContratoSituacion
,cs.textosituacion
,tc.textocoeficiente As Mercado
,case when fi.ismezcla=1 then 'SI'
 when fi.ismezcla=0 then 'NO'END AS AplicarPFijo
,replace(fi.valorpreciofijo,'.',',') as 'Precio Fijo (€/MWh)'
,case when fi.aplicarporcentaje=1 then 'SI'
 when fi.aplicarporcentaje=0 then 'NO'END  As AplicarPorcentaje
,replace(fi.Porcentaje,'.',',') as 'Porcentaje (% P.Fijo)'
,case when fi.Iscierrecargabase=1 then 'SI'
 when ISNULL(fi.Iscierrecargabase,0)=0  then 'NO' END As CierreEnCargaBase
,replace(ISNULL(fi.Consumocargabase,0),'.',',') As MWCargaBase
,fi.fechadesde as DesdeFecha
,fi.fechahasta As HastaFecha
from formulaindexado fi 
inner join cups on fi.idcups = cups.idcups
inner join Contrato c on fi.IdCups = c.IdCups
inner join contratosituacion cs  on c.idcontratosituacion = cs.idcontratosituacion
inner join tipocoeficiente tc on tc.idtipocoeficiente=fi.idtipocoeficiente"

	Public Shared Function GetHunosa(DesdeFecha As Date, hastaFecha As Date) As String
		Return $"--Erick _Gestor Sige_ Hunosa
With XMLNAMESPACES('http://localhost/elegibilidad' as ""XS"") 
,FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturaVentaCabecera where idcliente=50320 and SerieFactura is not null and Entorno='E1'
and FechaFactura>='{DesdeFecha.ToString("dd/MM/yyyy")}' and FechaFactura<='{hastaFecha.ToString("dd/MM/yyyy")}')),
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
"
	End Function

	Public Shared Function GetCadasa(DesdeFecha As Date, hastaFecha As Date) As String
		Return $"--Erick _Gestor Sige_ Cadasa
with XMLNAMESPACES('http://localhost/elegibilidad' as ""XS"") 
,FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturaVentaCabecera 
where IdCliente=50906 and SerieFactura is not null and Entorno='E1' and FechaFactura>='{DesdeFecha.ToString("dd/MM/yyyy")}' and FechaFactura<='{hastaFecha.ToString("dd/MM/yyyy")}'))

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
"
	End Function

	Public Shared Function GetRechazosVeolia() As String
		Return $" --Erick _Gestor Sige_ RechazosVeolia
select  distinct 
case c.Entorno 
when 'E1' then 'Electricidad'
Else 'GAS' End as Negocio
,cups.CodigoCUPS
,c.codigocontrato
,t.textotarifa
,cl.Identidad
,dbo.formateardenominacion(cl.nombre,cl.apellido1,cl.apellido2,cl.razonsocial) as Sociedad
,c.fechaalta
,c.fechabaja
,cs.textosituacion
,s.IdSolicitud
,mr.TextoRechazo
,consumoestimado
,c.FechaVto 
from contrato c
inner join cups on cups.IdCups = c.IdCups
inner join cliente cl on cl.IdCliente = c.IdCliente
left join Tarifa t on t.idtarifa = c.IdTarifa
inner join ContratoSituacion cs on cs.IdContratoSituacion = c.IdContratoSituacion
left join Solicitud s on s.CodigoContrato = c.CodigoContrato
left join MotivoRechazo mr on s.IdMotivoRechazo = mr.IdMotivoRechazo
where cl.Identidad in (
'A28233922'
,'A15208408'
,'A58295031'
,'A20071429'
,'V20681623'
,'U09964313')
and c.idcontratosituacion not in  (2,18,19,22,3,9,17,28,29,34,43,48)
"
	End Function
	Public Shared Function GetQuantum(DesdeFecha As Date, hastaFecha As Date) As String
		Return $"--Erick _Gestor Sige_ Quantum

With 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturaVentaCabecera 
where IdCliente in (74987
,524
,1970
,853
,74774
,74776
,74778
,74779
,74790
,74791
,74792
,74795
,74797
,1969
,1971
,1965
,1966
,1968
,858
,75003
,74998
,75000
,75001
,13400) and FechaFactura>='{DesdeFecha.ToString("dd/MM/yyyy")}' and FechaFactura<='{hastaFecha.ToString("dd/MM/yyyy")}' and SerieFactura is not null and Entorno='E1')),

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
select IdContrato, CodigoContrato, TipoImprimir, IdGrupoImprimir, IdModeloFactura, IdModeloFacturaGestinel, IdTarifa, IdCliente from Contrato WITH (NOLOCK)
where Entorno = 'E1' and Contrato.CodigoContrato in (select codigocontrato from FacturasVentaConsulta)
),
PreseleccionFacturas as (
select IdFacturaVentaCabecera, FacturaVentaCabecera.SerieFactura, NumeroFactura, FacturaCategoria, PreseleccionContratos.CodigoContrato, FechaFactura, FacturaVentaCabecera.IdFacturaTipo,facturaventacabecera.fechalecturaactualxml,facturaventacabecera.fechalecturaanteriorxml
,IdTipoImpuesto, PreseleccionContratos.IdTarifa,
IdFacturaRectificativa, IdFacturaAbono, IdFacturaOrigen, cast(isnull(InfoCabeceraXML.value('(//ConsumoActiva)[1]', 'nvarchar(max)'), '0') as decimal(18,2)) as Consumo, IsGestinel, IdContratoDocumento,
PreseleccionContratos.IdContrato, PreseleccionContratos.IdCliente, IdCanal, PreseleccionContratos.IdModeloFactura, PreseleccionContratos.IdModeloFacturaGestinel, FacturaTipo.TextoFacturaTipo
from PreseleccionContratos with(nolock)
inner join FacturaVentaCabecera with(nolock) on PreseleccionContratos.IdContrato = FacturaVentaCabecera.IdContrato 
left join FacturaTipo with(nolock) on FacturaTipo.IdFacturaTipo = FacturaVentaCabecera.IdFacturaTipo
where FacturaVentaCabecera.Entorno = 'E1' and IsFactura = 1
)
,
Lineas as (
select PreseleccionFacturas.IdFacturaVentaCabecera, FacturaConcepto, ImporteBase  from PreseleccionFacturas with(nolock)
inner join FacturaVentaLinea with(nolock) on FacturaVentaLinea.IdFacturaVentaCabecera = PreseleccionFacturas.IdFacturaVentaCabecera
),
ImportesPotencia as (
select Lineas.IdFacturaVentaCabecera, sum(Lineas.ImporteBase) as Importe from Lineas with(nolock)
where (FacturaConcepto between 10000 and 19999 or FacturaConcepto in (130001,130002, 131001)) or 
(FacturaConcepto in (90003, 90018, 90035,90036,90039,90040, 90042, 90043, 90044, 90045, 90049, 90050, 90053, 90054, 90056, 90057, 90058, 90059, 90070))
group by Lineas.IdFacturaVentaCabecera
),
ImportesEnergia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with(nolock) 
where (FacturaConcepto between 30000 and 39999 or FacturaConcepto in (130003,130004, 131003)) or 
(FacturaConcepto in (90001,90002,90012,90031,90032,90062,90038,90048,90052))
group by Lineas.IdFacturaVentaCabecera
),
ImportesReactiva as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with(nolock)
where FacturaConcepto between 40000 and 49999 
group by Lineas.IdFacturaVentaCabecera
),
ImportesExcesos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with(nolock)
where (FacturaConcepto between 20000 and 29999) or (FacturaConcepto in (90037,90041,90051,90055))
group by Lineas.IdFacturaVentaCabecera
),
DescuentosPotencia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with(nolock)
where (FacturaConcepto in (120001, 120004)) or (FacturaConcepto in (120006, 90007))
group by Lineas.IdFacturaVentaCabecera
),
DescuentosEnergia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with(nolock)
where (FacturaConcepto in (120002, 120005)) or (FacturaConcepto in (120007, 90006))
group by Lineas.IdFacturaVentaCabecera
),
ImportesAlquileres as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with(nolock)
where (FacturaConcepto between 50000 and 59999) or (FacturaConcepto in (120007, 90006))
group by Lineas.IdFacturaVentaCabecera
),
ImportesProductos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with(nolock)
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
--,replace(tpp1a.PotenciaPrecio,'.',',') as PrecioPotenciaP1A
--,replace(tpp1a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP1A
--,replace(tpp2a.PotenciaPrecio,'.',',') as PrecioPotenciaP2A
--,replace(tpp2a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP2A
--,replace(tpp3a.PotenciaPrecio,'.',',') as PrecioPotenciaP3A
--,replace(tpp3a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP3A
--,replace(tpp4a.PotenciaPrecio,'.',',') as PrecioPotenciaP4A
--,replace(tpp4a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP4A
--,replace(tpp5a.PotenciaPrecio,'.',',') as PrecioPotenciaP5A
--,replace(tpp5a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP5A
--,replace(tpp6a.PotenciaPrecio,'.',',') as PrecioPotenciaP6A
--,replace(tpp6a.CargoPotenciaPrecio,'.',',') as PrecioCargoPotenciaP6A
--,replace(tpp1a.EnergiaPrecio,'.',',') as PrecioEnergiaP1A
--,replace(tpp1a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP1A
--,replace(tpp2a.EnergiaPrecio,'.',',') as PrecioEnergiaP2A
--,replace(tpp2a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP2A
--,replace(tpp3a.EnergiaPrecio,'.',',') as PrecioEnergiaP3A
--,replace(tpp3a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP3A
--,replace(tpp4a.EnergiaPrecio,'.',',') as PrecioEnergiaP4A
--,replace(tpp4a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP4A
--,replace(tpp5a.EnergiaPrecio,'.',',') as PrecioEnergiaP5A
--,replace(tpp5a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP5A
--,replace(tpp6a.EnergiaPrecio,'.',',') as PrecioEnergiaP6A
--,replace(tpp6a.CargoEnergiaPrecio,'.',',') as PrecioCargoEnergiaP6A
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
,replace(ll1.ConsumoReactiva,'.',',') as 'ConsumoReactivaP1'
,replace(ll2.ConsumoReactiva,'.',',') as 'ConsumoReactivaP2'
,replace(ll3.ConsumoReactiva,'.',',') as 'ConsumoReactivaP3'
,replace(ll4.ConsumoReactiva,'.',',') as 'ConsumoReactivaP4'
,replace(ll5.ConsumoReactiva,'.',',') as 'ConsumoReactivaP5'
,replace(ll6.ConsumoReactiva,'.',',') as 'ConsumoReactivaP6' 
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
,replace(fvlP1.importebase,'.',',') as ImporteVariableP1
,replace(fvlP2.importebase,'.',',') as ImporteVariableP2
,replace(fvlP3.importebase,'.',',') as ImporteVariableP3
,replace(fvlP4.importebase,'.',',') as ImporteVariableP4
,replace(fvlP5.importebase,'.',',') as ImporteVariableP5
,replace(fvlP6.importebase,'.',',') as ImporteVariableP6
,replace(fvlCAP.importebase,'.',',') as ImporteCAPGAS
,replace(ISNULL(fvlP1.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioVariableP1
,replace(ISNULL(fvlP2.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioVariableP2
,replace(ISNULL(fvlP3.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioVariableP3
,replace(ISNULL(fvlP4.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioVariableP4
,replace(ISNULL(fvlP5.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioVariableP5
,replace(ISNULL(fvlP6.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioVariableP6
,replace(ISNULL(fvlpP1.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP1
,replace(ISNULL(fvlpP2.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP2
,replace(ISNULL(fvlpP3.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP3
,replace(ISNULL(fvlpP4.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP4
,replace(ISNULL(fvlpP5.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP5
,replace(ISNULL(fvlpP6.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoPontenciaP6
,replace(ISNULL(fvleP1.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP1
,replace(ISNULL(fvleP2.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP2
,replace(ISNULL(fvleP3.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP3
,replace(ISNULL(fvleP4.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP4
,replace(ISNULL(fvleP5.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP5
,replace(ISNULL(fvleP6.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0),'.',',') AS PrecioCargoEnergiaP6

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
left join facturaventalinea fvlIE with (nolock) on fvc.idfacturaventacabecera = fvlIE.idfacturaventacabecera and fvlIE.Facturaconcepto=60001
left join facturaventalinea fvlCON with (nolock) on fvc.idfacturaventacabecera = fvlCON.idfacturaventacabecera and fvlCON.Facturaconcepto=50002
left join facturaventalinea fvlMAX with (nolock) on fvc.idfacturaventacabecera = fvlMAX.idfacturaventacabecera and fvlMAX.Facturaconcepto=20006
inner join lectura l  with (nolock) on fvc.idfacturaventacabecera = l.IdFacturaVentaCabeceraSectorC or (fvc.IdFacturaOrigen = l.IdFacturaVentaCabeceraSectorC and fvc.SerieFactura like '%ABO%')
left join LecturaLinea ll1 with (nolock) on l.IdLectura = ll1.IdLectura and ll1.idtarifapeajeperiodolectura in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join LecturaLinea ll2 with (nolock) on l.IdLectura = ll2.IdLectura	and ll2.idtarifapeajeperiodolectura in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join LecturaLinea ll3 with (nolock) on l.IdLectura = ll3.IdLectura	and ll3.idtarifapeajeperiodolectura in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join LecturaLinea ll4 with (nolock) on l.IdLectura = ll4.IdLectura	and ll4.idtarifapeajeperiodolectura in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join LecturaLinea ll5 with (nolock) on l.IdLectura = ll5.IdLectura	and ll5.idtarifapeajeperiodolectura in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join LecturaLinea ll6 with (nolock) on l.IdLectura = ll6.IdLectura	and ll6.idtarifapeajeperiodolectura in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
--left join TarifaPeajePrecio tpp1a with (nolock) on l.IdTarifaPeaje = tpp1a.IdTarifaPeaje and tpp1a.fechaInicio='01/01/2022' and tpp1a.idtarifapeajeperiodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
--left join TarifaPeajePrecio tpp2a with (nolock) on l.IdTarifaPeaje = tpp2a.IdTarifaPeaje and tpp2a.fechaInicio='01/01/2022' and tpp2a.idtarifapeajeperiodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
--left join TarifaPeajePrecio tpp3a with (nolock) on l.IdTarifaPeaje = tpp3a.IdTarifaPeaje and tpp3a.fechaInicio='01/01/2022' and tpp3a.idtarifapeajeperiodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
--left join TarifaPeajePrecio tpp4a with (nolock) on l.IdTarifaPeaje = tpp4a.IdTarifaPeaje and tpp4a.fechaInicio='01/01/2022' and tpp4a.idtarifapeajeperiodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
--left join TarifaPeajePrecio tpp5a with (nolock) on l.IdTarifaPeaje = tpp5a.IdTarifaPeaje and tpp5a.fechaInicio='01/01/2022' and tpp5a.idtarifapeajeperiodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
--left join TarifaPeajePrecio tpp6a with (nolock) on l.IdTarifaPeaje = tpp6a.IdTarifaPeaje and tpp6a.fechaInicio='01/01/2022' and tpp6a.idtarifapeajeperiodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join TarifaPeajePrecio tpp1b with (nolock) on l.IdTarifaPeaje = tpp1b.IdTarifaPeaje and tpp1b.fechafinal is null and tpp1b.idtarifapeajeperiodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join TarifaPeajePrecio tpp2b with (nolock) on l.IdTarifaPeaje = tpp2b.IdTarifaPeaje and tpp2b.fechafinal is null and tpp2b.idtarifapeajeperiodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join TarifaPeajePrecio tpp3b with (nolock) on l.IdTarifaPeaje = tpp3b.IdTarifaPeaje and tpp3b.fechafinal is null and tpp3b.idtarifapeajeperiodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join TarifaPeajePrecio tpp4b with (nolock) on l.IdTarifaPeaje = tpp4b.IdTarifaPeaje and tpp4b.fechafinal is null and tpp4b.idtarifapeajeperiodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join TarifaPeajePrecio tpp5b with (nolock) on l.IdTarifaPeaje = tpp5b.IdTarifaPeaje and tpp5b.fechafinal is null and tpp5b.idtarifapeajeperiodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join TarifaPeajePrecio tpp6b with (nolock) on l.IdTarifaPeaje = tpp6b.IdTarifaPeaje and tpp6b.fechafinal is null and tpp6b.idtarifapeajeperiodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join ConsumosReactiva cr with (nolock) ON cr.id = fvc.IdFacturaVentaCabecera	
left join ImportesPotencia with(nolock) on ImportesPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesEnergia with(nolock) on ImportesEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesReactiva with(nolock) on ImportesReactiva.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesExcesos with(nolock) on ImportesExcesos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join DescuentosPotencia with(nolock) on DescuentosPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join DescuentosEnergia with(nolock) on DescuentosEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join ImportesAlquileres with(nolock) on ImportesAlquileres.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join ImportesProductos with(nolock) on ImportesProductos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join facturaventalinea fvlP1 with (nolock) on fvc.idfacturaventacabecera = fvlP1.idfacturaventacabecera and fvlP1.Facturaconcepto=30004 and fvlP1.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
left join facturaventalinea fvlP2 with (nolock) on fvc.idfacturaventacabecera = fvlP2.idfacturaventacabecera and fvlP2.Facturaconcepto=30004 and fvlP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
left join facturaventalinea fvlP3 with (nolock) on fvc.idfacturaventacabecera = fvlP3.idfacturaventacabecera and fvlP3.Facturaconcepto=30004 and fvlP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
left join facturaventalinea fvlP4 with (nolock) on fvc.idfacturaventacabecera = fvlP4.idfacturaventacabecera and fvlP4.Facturaconcepto=30004 and fvlP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
left join facturaventalinea fvlP5 with (nolock) on fvc.idfacturaventacabecera = fvlP5.idfacturaventacabecera and fvlP5.Facturaconcepto=30004 and fvlP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
left join facturaventalinea fvlP6 with (nolock) on fvc.idfacturaventacabecera = fvlP6.idfacturaventacabecera and fvlP6.Facturaconcepto=30004 and fvlP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
left join facturaventalinea fvlCAP with (nolock) on fvc.idfacturaventacabecera = fvlCAP.idfacturaventacabecera and fvlCAP.Facturaconcepto=30004 and fvlCAP.IsAjusteCAPGas=1
left join facturaventalinea fvlpP1 with (nolock) on fvc.idfacturaventacabecera = fvlpP1.idfacturaventacabecera and fvlpP1.Facturaconcepto=130002 and fvlpP1.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
left join facturaventalinea fvlpP2 with (nolock) on fvc.idfacturaventacabecera = fvlpP2.idfacturaventacabecera and fvlpP2.Facturaconcepto=130002 and fvlpP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
left join facturaventalinea fvlpP3 with (nolock) on fvc.idfacturaventacabecera = fvlpP3.idfacturaventacabecera and fvlpP3.Facturaconcepto=130002 and fvlpP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
left join facturaventalinea fvlpP4 with (nolock) on fvc.idfacturaventacabecera = fvlpP4.idfacturaventacabecera and fvlpP4.Facturaconcepto=130002 and fvlpP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
left join facturaventalinea fvlpP5 with (nolock) on fvc.idfacturaventacabecera = fvlpP5.idfacturaventacabecera and fvlpP5.Facturaconcepto=130002 and fvlpP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
left join facturaventalinea fvlpP6 with (nolock) on fvc.idfacturaventacabecera = fvlpP6.idfacturaventacabecera and fvlpP6.Facturaconcepto=130002 and fvlpP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
left join facturaventalinea fvleP1 with (nolock) on fvc.idfacturaventacabecera = fvleP1.idfacturaventacabecera and fvleP1.Facturaconcepto=130004 and fvleP1.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
left join facturaventalinea fvleP2 with (nolock) on fvc.idfacturaventacabecera = fvleP2.idfacturaventacabecera and fvleP2.Facturaconcepto=130004 and fvleP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
left join facturaventalinea fvleP3 with (nolock) on fvc.idfacturaventacabecera = fvleP3.idfacturaventacabecera and fvleP3.Facturaconcepto=130004 and fvleP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
left join facturaventalinea fvleP4 with (nolock) on fvc.idfacturaventacabecera = fvleP4.idfacturaventacabecera and fvleP4.Facturaconcepto=130004 and fvleP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
left join facturaventalinea fvleP5 with (nolock) on fvc.idfacturaventacabecera = fvleP5.idfacturaventacabecera and fvleP5.Facturaconcepto=130004 and fvleP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
left join facturaventalinea fvleP6 with (nolock) on fvc.idfacturaventacabecera = fvleP6.idfacturaventacabecera and fvleP6.Facturaconcepto=130004 and fvleP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6

where fvc.idfacturaventacabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta with(nolock))

"
	End Function
	Public Shared Function GetGAM(DesdeFecha As Date, hastaFecha As Date) As String
		Return $"--Erick _Gestor Sige_ GAM
With 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) where IdFacturaVentaCabecera in (
select IdFacturaVentaCabecera from FacturaVentaCabecera 
where fechafactura>='{DesdeFecha.ToString("dd/MM/yyyy")}' 
and fechafactura<='{hastaFecha.ToString("dd/MM/yyyy")}' 
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
	End Function
	Public Shared Function GetCurvaHoraria(DesdeFecha As Date, hastaFecha As Date, Optional ListaCups As List(Of String) = Nothing, Optional Cups As String = "") As String
		Dim joinCups = ""
		If Cups.Length > 1 Then
			joinCups = $"'{Cups}'"
		End If
		If Not ListaCups Is Nothing AndAlso ListaCups.Count >= 1 Then
			joinCups = String.Join(",", ListaCups.Select(Function(c) $"'{c.Trim}'"))
		End If

		Return $"--Erick _Gestor Sige_ CurvaHoraria
SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria
WHERE left(cups,20) IN ({joinCups}) AND FechaMedida BETWEEN '{DesdeFecha:dd-MM-yyyy}' AND '{hastaFecha:dd-MM-yyyy}'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H_082024
WHERE left(cups,20) IN ({joinCups}) AND FechaMedida BETWEEN '{DesdeFecha:dd-MM-yyyy}' AND '{hastaFecha:dd-MM-yyyy}'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H_092024
WHERE left(cups,20) IN ({joinCups}) AND FechaMedida BETWEEN '{DesdeFecha:dd-MM-yyyy}' AND '{hastaFecha:dd-MM-yyyy}'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H_032024
WHERE left(cups,20) IN ({joinCups}) AND FechaMedida BETWEEN '{DesdeFecha:dd-MM-yyyy}' AND '{hastaFecha:dd-MM-yyyy}'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H_022025
WHERE left(cups,20) IN ({joinCups}) AND FechaMedida BETWEEN '{DesdeFecha:dd-MM-yyyy}' AND '{hastaFecha:dd-MM-yyyy}'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H
WHERE left(cups,20) IN ({joinCups}) AND FechaMedida BETWEEN '{DesdeFecha:dd-MM-yyyy}' AND '{hastaFecha:dd-MM-yyyy}'

ORDER BY FechaMedida;"
	End Function


	Public Shared Function GetCurvaCuartoHoraria(DesdeFecha As Date, hastaFecha As Date, Optional ListaCups As List(Of String) = Nothing, Optional Cups As String = "") As String
		Dim joinCups = ""
		If Cups.Length > 1 Then
			joinCups = $"'{Cups}'"
		End If
		If Not ListaCups Is Nothing AndAlso ListaCups.Count >= 1 Then
			joinCups = String.Join(",", ListaCups.Select(Function(c) $"'{c.Trim}'"))
		End If

		Return $"--Erick _Gestor Sige_ CurvaCuartoHoraria
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_032024
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_022025
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_092024
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_082025
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
"
	End Function

	Public Shared Function GetCurvaFacturable(DesdeFecha As Date, hastaFecha As Date, Optional ListaCups As List(Of String) = Nothing, Optional Cups As String = "") As String
		Dim joinCups = ""
		If Cups.Length > 1 Then
			joinCups = $"'{Cups}'"
		End If
		If Not ListaCups Is Nothing AndAlso ListaCups.Count >= 1 Then
			joinCups = String.Join(",", ListaCups.Select(Function(c) $"'{c.Trim}'"))
		End If

		Return $"--Erick _Gestor Sige_ CurvaFacturable
SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable
WHERE left(cups,20) IN ( {joinCups})   AND FechaMedida BETWEEN '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'

UNION ALL

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H
WHERE left(cups,20) IN ( {joinCups})   AND FechaMedida BETWEEN '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'

UNION ALL

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_022025
WHERE left(cups,20) IN ( {joinCups})   AND FechaMedida BETWEEN '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'

UNION ALL

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_032024
WHERE left(cups,20) IN ( {joinCups})  AND FechaMedida BETWEEN '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'

UNION ALL

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_082024
WHERE left(cups,20) IN ( {joinCups})   AND FechaMedida BETWEEN '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL 

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_082025
WHERE left(cups,20) IN ( {joinCups})   AND FechaMedida BETWEEN '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL 

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_092024
WHERE left(cups,20) IN ( {joinCups})   AND FechaMedida BETWEEN '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'

ORDER BY FechaMedida;"
	End Function
	Public Shared Function GetConsultaContrato(codCntrato As List(Of Long)) As String
		Dim codCntratojoin = String.Join(",", codCntrato)
		Return $"select c.Entorno,codigocontrato,c.idcontratosituacion, cs.textosituacion, c.SituacionScoring
,cast(FechaAlta as date) fechaalta, cast(FechaVto as date) fechavto
,DiasVencimiento
,Observaciones
,c.IdTipoImpuesto
, TextoImpuesto
, IsAgruparFacturas UnificarFacturas
,RevisionFra Revision
,TextoRevision TextoRevisionFra
, c.IdModeloFactura
,mifactura.DescripcionModeloDeImpresion
,c.IdModeloFacturaVarios
,mivarios.DescripcionModeloDeImpresion
,c.IdCNAE
,CNAE.TextoCNAE
,c.IdModeloContrato
,TipoImprimir
,case
when TipoImprimir ='P' then 'Papel y Email'
when TipoImprimir ='E' then 'Email'
when TipoImprimir ='W' then 'Web'
when TipoImprimir ='Q' then 'Papel'
when TipoImprimir ='R' then 'Recibo'
when TipoImprimir ='F' then 'FACE' end TipoImprimirTexto
,micontrato.DescripcionModeloDeImpresion
,IsRenovacionProcesada
,NoRenovar
,Representante
,c.IdColectivoRep
,Colectivo.TextoColectivo
,IdentificadorRep
,EmailRep
,SMSRep
,c.IdClientePago
,isnull(NombreP,'???')+'/'+isnull(IdentidadPago,'???')+'/'+isnull(colecClientePago.TextoColectivo,'???')+'/'+isnull(TextoTipoCobro,'???')+'/'+isnull(IBAN,'???')+'/'+isnull(TextoBanco,'???') ClientePagoUnificado
,Autoconsumo
,IsAutoconsumoNoCompensable
,ExencionIE
,IsLicitacion
,IdTipoAutoconsumo
from contrato c
inner join contratosituacion cs on c.idcontratosituacion = cs.idcontratosituacion
left join SituacionScoring scg on c.SituacionScoring=scg.Nombre
left join TipoImpuesto ti on c.IdTipoImpuesto = ti.IdTipoImpuesto
left join ModeloDeImpresion mifactura on (c.IdModeloFactura = mifactura.IdModeloDeImpresion )
left join ModeloDeImpresion mivarios on (c.IdModeloFacturaVarios = mivarios.IdModeloDeImpresion)
left join ModeloDeImpresion micontrato on (c.IdModeloContrato = micontrato.IdModeloDeImpresion)
left join CNAE on c.IdCNAE = CNAE.IdCNAE
left join Colectivo on c.IdColectivoRep = Colectivo.IdColectivo
left join ClientePago cp on c.IdClientePago  = cp.IdClientePago
left join TipoCobro tp on cp.IdTipoCobro = tp.IdTipoCobro
left join Colectivo colecClientePago on cp.IdColectivo = colecClientePago.IdColectivo
left join Banco b on cp.IdBanco=b.IdBanco
where codigocontrato in({codCntratojoin})"

	End Function

	Public Shared Function GetConsultaNorauto(DesdeFecha As Date, hastaFecha As Date, CIF As String) As String
		'Dim codCntratojoin = String.Join(",", codCntrato)
		Return $"--Erick _Gestor Sige_ Norauto
	create table #TmpId (LongParameter BIGINT)--FLN: 2023-08-06 Es mucho más rápido en una tabla temporal que en una variable de tabla.

		INSERT INTO #TmpId select idfacturaventacabecera from facturaventacabecera where  facturacategoria='EN' and (fechafactura between '{DesdeFecha:dd-MM-yyyy}' and '{hastaFecha:dd-MM-yyyy}') and idcliente in(select idcliente from cliente where 
	identidad in (
'{CIF}'

	) and seriefactura is not null)

	;WITH  FacturasFiltradas as
	(
		select IdFacturaVentaCabecera, f.Entorno, f.SerieFactura, f.NumeroFactura, f.IsFactura, f.IsIncidencia, f.IdCliente, f.IdClientePago, f.IdClienteEnvio, f.IdContrato, f.CodigoContrato
			, f.VersionContrato, f.FechaFactura, IdTipoCobro, f.IBAN, f.FechaVencimiento, f.IdCanal, f.IdFacturaTipo, f.FacturaCategoria, f.IdTipoImpuesto, f.IsFacturaConIE, DescuentoBase, DescuentoTotal
			, f.Imprimida, FechaEmision, FechaEnvio, VersionReport, f.IsCorrecta, f.IsProcesada, f.IsEnlaceContable, f.IdSCSAsientoCabecera, f.IdFacturaAbono, f.IdFacturaRectificativa, f.IdFacturaOrigen
			, f.IdTarifaPeajeXML, f.IdTarifaGrupoXML, f.IdTarifaXML, f.FechaLecturaActualXML, f.FechaLecturaAnteriorXML
			, f.FechaRecepcion
			, f.IdPerfilFacturacionXML as IdPerfilFacturacion
			, Lectura.IdLectura
			, Cliente.RazonSocial
			, Cliente.Identidad
			, contrato.Version
			, cups.CodigoCUPS
			, callejero.NombreCalle + ' ' + ISNULL(convert(varchar(10), cups.Numero),'') as NombreCalle
			, ciudad.TextoCiudad
			, cups.CodPostal
			, Provincia.TextoProvincia
			, f.OrigenMedida
			, tarifapeaje.TextoTarifaPeaje
			, tarifa.TextoTarifa
			, lectura.FechaLecturaAnterior
			, lectura.FechaLectura
			, tarifagrupo.TextoTarifaGrupo
			, distribuidora.NombreFiscal
			, FacturaCompraCabecera.NumeroFactura as NumfacturaATR
		from #TmpId as s 
		inner join FacturaVentaCabecera  as f WITH (NOLOCK)
		on s.LongParameter = f.IdFacturaVentaCabecera
		left join Lectura WITH (NOLOCK) on Lectura.IdFacturaVentaCabeceraSectorC = f.IdFacturaVentaCabecera and Lectura.Facturar = 1
		left join Cliente WITH(NOLOCK) on Cliente.IdCliente = f.IdCliente
		left join contrato WITH(NOLOCK) on contrato.codigocontrato = f.codigocontrato
		left join cups WITH(NOLOCK) on cups.IdCups = contrato.IdCups
		left join callejero with(nolock) on callejero.IdCallejero=cups.IdCallejero
		left join Ciudad with(nolock) on ciudad.idciudad = callejero.IdCiudad
		left join Provincia with(nolock) on Provincia.IdProvincia = ciudad.IdProvincia
		left join TarifaPeaje with(nolock) on tarifapeaje.IdTarifaPeaje=lectura.IdTarifaPeaje
		left join Tarifa with(nolock) on tarifa.IdTarifa=lectura.IdTarifa
		left join tarifagrupo with(nolock) on tarifagrupo.idtarifagrupo = f.IdTarifaGrupoXML
		left join Distribuidora with(nolock) on distribuidora.IdDistribuidora = cups.iddistribuidora
		left join FacturaCompraCabecera with(nolock) on FacturaCompraCabecera.IdFacturaCompraCabecera = lectura.IdFacturaCompraCabecera
	)
	, LecturasSeleccionadas as 
	(
			select LecturaLinea.IdLectura, 
			CASE periodohorario.codigoperiodo WHEN 1 then Maximetro ELSE 0 END as MaximetroP1,
			CASE periodohorario.codigoperiodo WHEN 2 then Maximetro ELSE 0 END as MaximetroP2,
			CASE periodohorario.codigoperiodo WHEN 3 then Maximetro ELSE 0 END as MaximetroP3,
			CASE periodohorario.codigoperiodo WHEN 4 then Maximetro ELSE 0 END as MaximetroP4,
			CASE periodohorario.codigoperiodo WHEN 5 then Maximetro ELSE 0 END as MaximetroP5,
			CASE periodohorario.codigoperiodo WHEN 6 then Maximetro ELSE 0 END as MaximetroP6
			from lecturalinea WITH (NOLOCK) 
			inner join FacturasFiltradas WITH (NOLOCK) on FacturasFiltradas.IdLectura = LecturaLinea.IdLectura
			left join TarifaPeajePeriodoLectura WITH (NOLOCK) on TarifaPeajePeriodoLectura.IdTarifaPeajePeriodoLectura=lecturalinea.IdTarifaPeajePeriodoLectura  
			left join PeriodoHorario WITH (NOLOCK) on TarifaPeajePeriodoLectura.IdPeriodoHorario = PeriodoHorario.IdPeriodoHorario 
	)
	, LecturasAgrupadas as 
	(
	SELECT IdLectura, MAX(MaximetroP1) as MaximetroP1, MAX(MaximetroP2) as MaximetroP2, MAX(MaximetroP3) as MaximetroP3, MAX(MaximetroP4) as MaximetroP4, MAX(MaximetroP5) as MaximetroP5, MAX(MaximetroP6) as MaximetroP6 
	FROM LecturasSeleccionadas WITH (NOLOCK) group by IdLectura
	)
	, ConsumosEnergiaTarifaAcceso as
	(
			SELECT FacturaVentaLinea.IdFacturaVentaCabecera, ISNULL(TotConsumoEnergiaXML, 0) AS Consumo, 
			ISNULL(PrecioMedioEnergiaXML, 0) AS Precio,
			CodigoPeriodoXML as CodPeriodo 
			, ImporteBase
			FROM FacturaVentaLinea WITH (NOLOCK) inner join #TmpId  as FacturasFiltradas  on FacturaVentaLinea.IdFacturaVentaCabecera = FacturasFiltradas.LongParameter
			where (FacturaConcepto = 30001)
	)
	, PeriodoConsumosEnergiaTarifaAcceso as
	(
		select IdFacturaVentaCabecera, ImporteBase,
		CASE codperiodo WHEN 1 then Consumo ELSE 0 END as ConsumoRP1,
		CASE codperiodo WHEN 2 then Consumo ELSE 0 END as ConsumoRP2,
		CASE codperiodo WHEN 3 then Consumo ELSE 0 END as ConsumoRP3,
		CASE codperiodo WHEN 4 then Consumo ELSE 0 END as ConsumoRP4,
		CASE codperiodo WHEN 5 then Consumo ELSE 0 END as ConsumoRP5,
		CASE codperiodo WHEN 6 then Consumo ELSE 0 END as ConsumoRP6,

		CASE codperiodo WHEN 1 then Precio ELSE 0 END as PrecioRP1,
		CASE codperiodo WHEN 2 then Precio ELSE 0 END as PrecioRP2,
		CASE codperiodo WHEN 3 then Precio ELSE 0 END as PrecioRP3,
		CASE codperiodo WHEN 4 then Precio ELSE 0 END as PrecioRP4,
		CASE codperiodo WHEN 5 then Precio ELSE 0 END as PrecioRP5,
		CASE codperiodo WHEN 6 then Precio ELSE 0 END as PrecioRP6,
		codperiodo
		from ConsumosEnergiaTarifaAcceso  WITH (NOLOCK)
	)
	, AgrupadaPeriodoConsumosEnergiaTarifaAcceso as
	(
		SELECT IdFacturaVentaCabecera , SUM(ConsumoRP1) as ConsumoP1,  SUM(ConsumoRP2) as ConsumoP2,  SUM(ConsumoRP3) as ConsumoP3, 
		SUM(ConsumoRP4) as ConsumoP4,  SUM(ConsumoRP5) as ConsumoP5,  SUM(ConsumoRP6) as ConsumoP6
		, SUM(PrecioRP1) as EnergiaPrecioP1, SUM(PrecioRP2) as EnergiaPrecioP2, SUM(PrecioRP3) as EnergiaPrecioP3, SUM(PrecioRP4) as EnergiaPrecioP4,SUM(PrecioRP5) as EnergiaPrecioP5,SUM(PrecioRP6) as EnergiaPrecioP6
		, SUM(ImporteBase) as ImporteTarifaAcceso
		FROM PeriodoConsumosEnergiaTarifaAcceso WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)
	, Consumos as
	(
			SELECT FacturaVentaLinea.IdFacturaVentaCabecera, ISNULL(TotConsumoEnergiaXML, 0) AS Consumo, 
			case FacturaConcepto when 30006 then
			ISNULL(PrecioClick, 0)
			else
			ISNULL(PrecioMedioEnergiaXML, 0) END AS Precio,
			CodigoPeriodoXML as CodPeriodo 
			FROM   FacturaVentaLinea WITH (NOLOCK) inner join #TmpId as FacturasFiltradas on FacturaVentaLinea.IdFacturaVentaCabecera = FacturasFiltradas.LongParameter
			where  (FacturaConcepto between 30000 and 30999) AND (FacturaConcepto <> 30001)
	)
	, PeriodoConsumos as
	(
		select IdFacturaVentaCabecera, 		
		CASE codperiodo WHEN 1 then Consumo ELSE 0 END as ConsumoRP1,
		CASE codperiodo WHEN 2 then Consumo ELSE 0 END as ConsumoRP2,
		CASE codperiodo WHEN 3 then Consumo ELSE 0 END as ConsumoRP3,
		CASE codperiodo WHEN 4 then Consumo ELSE 0 END as ConsumoRP4,
		CASE codperiodo WHEN 5 then Consumo ELSE 0 END as ConsumoRP5,
		CASE codperiodo WHEN 6 then Consumo ELSE 0 END as ConsumoRP6,

		CASE codperiodo WHEN 1 then Precio ELSE 0 END as PrecioRP1,
		CASE codperiodo WHEN 2 then Precio ELSE 0 END as PrecioRP2,
		CASE codperiodo WHEN 3 then Precio ELSE 0 END as PrecioRP3,
		CASE codperiodo WHEN 4 then Precio ELSE 0 END as PrecioRP4,
		CASE codperiodo WHEN 5 then Precio ELSE 0 END as PrecioRP5,
		CASE codperiodo WHEN 6 then Precio ELSE 0 END as PrecioRP6,
		codperiodo
		from Consumos WITH (NOLOCK)
	)
	, AgrupadaPeriodoConsumos as
	(
		SELECT IdFacturaVentaCabecera , SUM(ConsumoRP1) as ConsumoP1,  SUM(ConsumoRP2) as ConsumoP2,  SUM(ConsumoRP3) as ConsumoP3, 
		SUM(ConsumoRP4) as ConsumoP4,  SUM(ConsumoRP5) as ConsumoP5,  SUM(ConsumoRP6) as ConsumoP6
		, SUM(PrecioRP1) as EnergiaPrecioP1, SUM(PrecioRP2) as EnergiaPrecioP2, SUM(PrecioRP3) as EnergiaPrecioP3, SUM(PrecioRP4) as EnergiaPrecioP4,SUM(PrecioRP5) as EnergiaPrecioP5,SUM(PrecioRP6) as EnergiaPrecioP6
		FROM PeriodoConsumos WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)
	, ConsumosReactiva AS
	(
			SELECT fvl.idfacturaventacabecera
			,ISNULL(ll.ConsumoReactiva, 0) as Consumo
			,ISNULL(PrecioMedioReactivaXML, 0) AS Precio
			, CodigoPeriodoXML as CodPeriodo 
			, tppl.TextoTarifaPeajePeriodoLectura as CodPeriodoLectura
			, ImporteBase 
			from LecturaLinea ll
			INNER join Lectura l on l.IdLectura = ll.idlectura
			LEFT join FacturaVentaLinea fvl WITH (NOLOCK)  on fvl.IdFacturaVentaCabecera = l.IdFacturaVentaCabeceraSectorC and (FacturaConcepto between 40000 and 40999)
			INNER join tarifapeajeperiodolectura tppl on tppl.idtarifapeajeperiodolectura = ll.idtarifapeajeperiodolectura
	)
	, PeriodoConsumosReactiva as
	(
		select IdFacturaVentaCabecera, ImporteBase,		
		CASE codperiodolectura WHEN 'P1' then Consumo ELSE 0 END as ConsumoReactivaP1,
		CASE codperiodolectura WHEN 'P2' then Consumo ELSE 0 END as ConsumoReactivaP2,
		CASE codperiodolectura WHEN 'P3' then Consumo ELSE 0 END as ConsumoReactivaP3,
		CASE codperiodolectura WHEN 'P4' then Consumo ELSE 0 END as ConsumoReactivaP4,
		CASE codperiodolectura WHEN 'P5' then Consumo ELSE 0 END as ConsumoReactivaP5,
		CASE codperiodolectura WHEN 'P6' then Consumo ELSE 0 END as ConsumoReactivaP6,

		CASE codperiodo WHEN 1 then Precio ELSE 0 END as PrecioReactivaP1,
		CASE codperiodo WHEN 2 then Precio ELSE 0 END as PrecioReactivaP2,
		CASE codperiodo WHEN 3 then Precio ELSE 0 END as PrecioReactivaP3,
		CASE codperiodo WHEN 4 then Precio ELSE 0 END as PrecioReactivaP4,
		CASE codperiodo WHEN 5 then Precio ELSE 0 END as PrecioReactivaP5,
		CASE codperiodo WHEN 6 then Precio ELSE 0 END as PrecioReactivaP6
		from ConsumosReactiva  WITH (NOLOCK)
	)
	, AgrupadaPeriodoReactiva as
	(
		SELECT IdFacturaVentaCabecera , Max(ConsumoReactivaP1) as ConsumoReactivaP1,  Max(ConsumoReactivaP2) as ConsumoReactivaP2,  Max(ConsumoReactivaP3) as ConsumoReactivaP3, 
		Max(ConsumoReactivaP4) as ConsumoReactivaP4,  Max(ConsumoReactivaP5) as ConsumoReactivaP5,  Max(ConsumoReactivaP6) as ConsumoReactivaP6
		, Max(PrecioReactivaP1) as PrecioReactivaP1, Max(PrecioReactivaP2) as PrecioReactivaP2, Max(PrecioReactivaP3) as PrecioReactivaP3, Max(PrecioReactivaP4) as PrecioReactivaP4
		,Max(PrecioReactivaP5) as PrecioReactivaP5,Max(PrecioReactivaP6) as PrecioReactivaP6
		, SUM(ImporteBase) as ImportePeriodoReactiva
		FROM PeriodoConsumosReactiva WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)
	, Potencia as
	(
		SELECT l.IdFacturaVentaCabecera as IdFacturaVentaCabecera, ISNULL(PotenciaAFacturarXML, 0) AS PotenciaFacturar
		, ISNULL(PotenciaContratadaXML, 0) AS PotenciaContratada,
		l.CodigoPeriodoXML as CodPeriodo
		, ISNULL(PrecioMedioPotenciaXML, 0) AS Precio 
		FROM FacturaVentaLinea as l WITH (NOLOCK)
			inner join FacturasFiltradas as f WITH (NOLOCK)
		on l.IdFacturaVentaCabecera = f.IdFacturaVentaCabecera
		where (FacturaConcepto between 10000 and 10999)
	)
	, PeriodoPotencia as
	(
		select IdFacturaVentaCabecera, 	
		CASE codperiodo WHEN 1 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP1,
		CASE codperiodo WHEN 2 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP2,
		CASE codperiodo WHEN 3 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP3,
		CASE codperiodo WHEN 4 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP4,
		CASE codperiodo WHEN 5 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP5,
		CASE codperiodo WHEN 6 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP6,
	
		CASE codperiodo WHEN 1 then PotenciaContratada ELSE 0 END as PotenciaContratadaP1,
		CASE codperiodo WHEN 2 then PotenciaContratada ELSE 0 END as PotenciaContratadaP2,
		CASE codperiodo WHEN 3 then PotenciaContratada ELSE 0 END as PotenciaContratadaP3,
		CASE codperiodo WHEN 4 then PotenciaContratada ELSE 0 END as PotenciaContratadaP4,
		CASE codperiodo WHEN 5 then PotenciaContratada ELSE 0 END as PotenciaContratadaP5,
		CASE codperiodo WHEN 6 then PotenciaContratada ELSE 0 END as PotenciaContratadaP6,

		CASE codperiodo WHEN 1 then Precio ELSE 0 END as PrecioP1,
		CASE codperiodo WHEN 2 then Precio ELSE 0 END as PrecioP2,
		CASE codperiodo WHEN 3 then Precio ELSE 0 END as PrecioP3,
		CASE codperiodo WHEN 4 then Precio ELSE 0 END as PrecioP4,
		CASE codperiodo WHEN 5 then Precio ELSE 0 END as PrecioP5,
		CASE codperiodo WHEN 6 then Precio ELSE 0 END as PrecioP6
		from Potencia  WITH (NOLOCK)
	)
	, AgrupadaPeriodoPotencia as
	(
		SELECT IdFacturaVentaCabecera , 
		MAX(PotenciaFacturarP1) as PotenciaFacturaP1,  MAX(PotenciaFacturarP2) as PotenciaFacturaP2,  MAX(PotenciaFacturarP3) as PotenciaFacturaP3, 
		MAX(PotenciaFacturarP4) as PotenciaFacturaP4,  MAX(PotenciaFacturarP5) as PotenciaFacturaP5,  MAX(PotenciaFacturarP6) as PotenciaFacturaP6,
		MAX(PotenciaContratadaP1) as PotenciaContratadaP1,  MAX(PotenciaContratadaP2) as PotenciaContratadaP2,  MAX(PotenciaContratadaP3) as PotenciaContratadaP3, 
		MAX(PotenciaContratadaP4) as PotenciaContratadaP4,  MAX(PotenciaContratadaP5) as PotenciaContratadaP5,  MAX(PotenciaContratadaP6) as PotenciaContratadaP6
		, MAX(PrecioP1) as PotenciaPrecioP1, MAX(PrecioP2) as PotenciaPrecioP2, MAX(PrecioP3) as PotenciaPrecioP3, MAX(PrecioP4) as PotenciaPrecioP4,MAX(PrecioP5) as PotenciaPrecioP5,MAX(PrecioP6) as PotenciaPrecioP6
		FROM PeriodoPotencia WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)
	, SerieNumeroFacturasOrigen as
	(
		select CONCAT( origen.seriefactura, ' ',  origen.numerofactura) as SerieNumeroFacturaOrigen, principal.IdFacturaVentaCabecera
		from FacturaVentaCabecera principal WITH (NOLOCK)
		inner join FacturaVentaCabecera origen  WITH (NOLOCK)
		on principal.IdFacturaOrigen = origen.IdFacturaVentaCabecera
	),
	--cargos
	--Cargos Energia
	 CargosEnergia AS
	(
			SELECT FacturaVentaLinea.IdFacturaVentaCabecera as IdFacturaVentaCabecera
			, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/Energia)[1]', 'decimal'), 0) AS ConsumoCargoEnergia
			,		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,9)'), 0) AS PrecioCargoEnergia
			,ImporteBase
			,FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo
		
			FROM   FacturaVentaLinea WITH (NOLOCK) inner join #TmpId as f
			on FacturaVentaLinea.IdFacturaVentaCabecera = f.LongParameter
			where  (FacturaConcepto IN (130004,130003))
	),
	PeriodoCargosEnergia as (
	select IdFacturaVentaCabecera, 	
		CASE codperiodo WHEN 1 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP1,
		CASE codperiodo WHEN 2 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP2,
		CASE codperiodo WHEN 3 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP3,
		CASE codperiodo WHEN 4 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP4,
		CASE codperiodo WHEN 5 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP5,
		CASE codperiodo WHEN 6 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP6,

		CASE codperiodo WHEN 1 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP1,
		CASE codperiodo WHEN 2 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP2,
		CASE codperiodo WHEN 3 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP3,
		CASE codperiodo WHEN 4 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP4,
		CASE codperiodo WHEN 5 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP5,
		CASE codperiodo WHEN 6 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP6,

		CASE codperiodo WHEN 1 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP1,
		CASE codperiodo WHEN 2 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP2,
		CASE codperiodo WHEN 3 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP3,
		CASE codperiodo WHEN 4 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP4,
		CASE codperiodo WHEN 5 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP5,
		CASE codperiodo WHEN 6 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP6

		from CargosEnergia WITH (NOLOCK)
	),
	AgrupadaPeriodoCargosEnergia as (
	SELECT IdFacturaVentaCabecera , SUM(CargosEnergiaP1) as CargosEnergiaP1,  SUM(CargosEnergiaP2) as CargosEnergiaP2,  SUM(CargosEnergiaP3) as CargosEnergiaP3, 
		SUM(CargosEnergiaP4) as CargosEnergiaP4,  SUM(CargosEnergiaP5) as CargosEnergiaP5,  SUM(CargosEnergiaP6) as CargosEnergiaP6,
		MAX(CargosPrecioEnergiaP1) as CargosPrecioEnergiaP1, MAX(CargosPrecioEnergiaP2) as CargosPrecioEnergiaP2, MAX(CargosPrecioEnergiaP3) as CargosPrecioEnergiaP3,
		MAX(CargosPrecioEnergiaP4) as CargosPrecioEnergiaP4,MAX(CargosPrecioEnergiaP5) as CargosPrecioEnergiaP5,MAX(CargosPrecioEnergiaP6) as CargosPrecioEnergiaP6,

		MAX(CargosTotalPrecioEnergiaP1) as CargosTotalPrecioEnergiaP1, MAX(CargosTotalPrecioEnergiaP2) as CargosTotalPrecioEnergiaP2, MAX(CargosTotalPrecioEnergiaP3) as CargosTotalPrecioEnergiaP3,
		MAX(CargosTotalPrecioEnergiaP4) as CargosTotalPrecioEnergiaP4,MAX(CargosTotalPrecioEnergiaP5) as CargosTotalPrecioEnergiaP5,MAX(CargosTotalPrecioEnergiaP6) as CargosTotalPrecioEnergiaP6
	
		FROM PeriodoCargosEnergia WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	),
	--Cargos Potencia
	CargosPotencia AS
	(
			SELECT FacturaVentaLinea.IdFacturaVentaCabecera as IdFacturaVentaCabecera
			, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/Potencia)[1]', 'decimal'), 0) AS ConsumoCargoPotencia
			,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,9)'), 0) AS PrecioCargoPotencia
			,FacturaVentaLinea.ImporteBase
			,FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo
		
			FROM   FacturaVentaLinea WITH (NOLOCK) inner join #TmpId as f
			on FacturaVentaLinea.IdFacturaVentaCabecera = f.LongParameter
			where  (FacturaConcepto IN (130001,130002))
	),
	PeriodoCargosPotencia as (
	select IdFacturaVentaCabecera, 	
		CASE codperiodo WHEN 1 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP1,
		CASE codperiodo WHEN 2 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP2,
		CASE codperiodo WHEN 3 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP3,
		CASE codperiodo WHEN 4 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP4,
		CASE codperiodo WHEN 5 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP5,
		CASE codperiodo WHEN 6 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP6,

		CASE codperiodo WHEN 1 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP1,
		CASE codperiodo WHEN 2 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP2,
		CASE codperiodo WHEN 3 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP3,
		CASE codperiodo WHEN 4 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP4,
		CASE codperiodo WHEN 5 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP5,
		CASE codperiodo WHEN 6 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP6,

		CASE codperiodo WHEN 1 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP1,
		CASE codperiodo WHEN 2 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP2,
		CASE codperiodo WHEN 3 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP3,
		CASE codperiodo WHEN 4 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP4,
		CASE codperiodo WHEN 5 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP5,
		CASE codperiodo WHEN 6 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP6
	   
		from CargosPotencia WITH (NOLOCK)
	),
	AgrupadaPeriodoCargosPotencia as (
	SELECT IdFacturaVentaCabecera , SUM(CargosPotenciaP1) as CargosPotenciaP1,  SUM(CargosPotenciaP2) as CargosPotenciaP2,  SUM(CargosPotenciaP3) as CargosPotenciaP3, 
		SUM(CargosPotenciaP4) as CargosPotenciaP4,  SUM(CargosPotenciaP5) as CargosPotenciaP5,  SUM(CargosPotenciaP6) as CargosPotenciaP6,

			MAX(CargosPrecioPotenciaP1) as CargosPrecioPotenciaP1, MAX(CargosPrecioPotenciaP2) as CargosPrecioPotenciaP2, MAX(CargosPrecioPotenciaP3) as CargosPrecioPotenciaP3,
		MAX(CargosPrecioPotenciaP4) as CargosPrecioPotenciaP4,MAX(CargosPrecioPotenciaP5) as CargosPrecioPotenciaP5,MAX(CargosPrecioPotenciaP6) as CargosPrecioPotenciaP6,

			MAX(CargosTotalPrecioPotenciaP1) as CargosTotalPrecioPotenciaP1, MAX(CargosTotalPrecioPotenciaP2) as CargosTotalPrecioPotenciaP2, MAX(CargosTotalPrecioPotenciaP3) as CargosTotalPrecioPotenciaP3,
		MAX(CargosTotalPrecioPotenciaP4) as CargosTotalPrecioPotenciaP4,MAX(CargosTotalPrecioPotenciaP5) as CargosTotalPrecioPotenciaP5,MAX(CargosTotalPrecioPotenciaP6) as CargosTotalPrecioPotenciaP6
	
		FROM PeriodoCargosPotencia WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)

	--genera consulta
	select	v.Identidad,v.RazonSocial as Cliente,v.CodigoContrato as contrato,v.Version as V,v.CodigoCUPS as cups,v.NombreCalle as direccion,v.TextoCiudad as poblacion,v.OrigenMedida as OrigenCurva,v.CodPostal as CodigoPostal,
			v.TextoProvincia as Provincia,v.SerieFactura + ' ' + convert(varchar(20),v.NumeroFactura) as 'Serie/Numero',CONVERT(VARCHAR, v.fechafactura, 103) as FechaFactura,v.NumfacturaATR as NroFacturaATR, 
			CONVERT(VARCHAR, v.fecharecepcion, 103) as FechaRecepcion,v.TextoTarifaPeaje as TarifaPeaje,v.TextoTarifa as Tarifa,v.TextoTarifaGrupo as Grupo, CONVERT(VARCHAR, v.FechaLecturaAnterior, 103) as LecturaAnterior,
			CONVERT(VARCHAR, v.FechaLectura, 103) as FechaLectura, ImporteIE.Importe as IE, ImporteAlquiler.Importe as Alquiler,'' as ImporteProductos, 


			(select sum(ImporteBase) from FacturaVentaTotal where IdFacturaVentaCabecera in (v.IdFacturaVentaCabecera)) as ImporteBase,
			(select sum(ImporteImpuesto) from FacturaVentaTotal where IdFacturaVentaCabecera in (v.IdFacturaVentaCabecera)) as ImporteImpuesto,
			(select sum(ImporteTotal) from FacturaVentaTotal where IdFacturaVentaCabecera in (v.IdFacturaVentaCabecera)) as ImporteTotal,

			cast(Round(CASE WHEN ISNULL(ConsumoP.ConsumoP1,0) <> 0 THEN ConsumoP.ConsumoP1 ELSE ConsumoPTA.ConsumoP1 END + 
					CASE WHEN ISNULL(ConsumoP.ConsumoP2,0) <> 0 THEN ConsumoP.ConsumoP2 ELSE ConsumoPTA.ConsumoP2 END +
					CASE WHEN ISNULL(ConsumoP.ConsumoP3,0) <> 0 THEN ConsumoP.ConsumoP3 ELSE ConsumoPTA.ConsumoP3 END +
					CASE WHEN ISNULL(ConsumoP.ConsumoP4,0) <> 0 THEN ConsumoP.ConsumoP4 ELSE ConsumoPTA.ConsumoP4 END +
					CASE WHEN ISNULL(ConsumoP.ConsumoP5,0) <> 0 THEN ConsumoP.ConsumoP5 ELSE ConsumoPTA.ConsumoP5 END +
					CASE WHEN ISNULL(ConsumoP.ConsumoP6,0) <> 0 THEN ConsumoP.ConsumoP6 ELSE ConsumoPTA.ConsumoP6 END,0) as decimal(10,2)) as Consumo,

			0 as reactiva,

			cast(round(PotenciaP.PotenciaContratadaP1 +
						PotenciaP.PotenciaContratadaP2 +
						PotenciaP.PotenciaContratadaP3 +
						PotenciaP.PotenciaContratadaP4 +
						PotenciaP.PotenciaContratadaP5 +
						PotenciaP.PotenciaContratadaP6,2) as decimal(10,2)) as Potencia,

			v.NombreFiscal as Distribuidora,
			0 as ImporteAdicional,
			v.SerieFactura + ' ' + convert(varchar(20),v.NumeroFactura) as NumeroFactura,
			PotenciaP.PotenciaContratadaP1 as PotenciaContratadaP1,
			PotenciaP.PotenciaContratadaP2 as PotenciaContratadaP2,
			PotenciaP.PotenciaContratadaP3 as PotenciaContratadaP3,
			PotenciaP.PotenciaContratadaP4 as PotenciaContratadaP4,
			PotenciaP.PotenciaContratadaP5 as PotenciaContratadaP5,
			PotenciaP.PotenciaContratadaP6 as PotenciaContratadaP6,

			PotenciaP.PotenciaFacturaP1 as PotenciafacturadaP1,
			PotenciaP.PotenciaFacturaP2 as PotenciafacturadaP2,
			PotenciaP.PotenciaFacturaP3 as PotenciafacturadaP3,
			PotenciaP.PotenciaFacturaP4 as PotenciafacturadaP4,
			PotenciaP.PotenciaFacturaP5 as PotenciafacturadaP5,
			PotenciaP.PotenciaFacturaP6 as PotenciafacturadaP6,

			PotenciaP.PotenciaPrecioP1 as PrecioPotenciaP1,
			PotenciaP.PotenciaPrecioP2 as PrecioPotenciaP2,
			PotenciaP.PotenciaPrecioP3 as PrecioPotenciaP3,
			PotenciaP.PotenciaPrecioP4 as PrecioPotenciaP4,
			PotenciaP.PotenciaPrecioP5 as PrecioPotenciaP5,
			PotenciaP.PotenciaPrecioP6 as PrecioPotenciaP6,

			round(CASE WHEN ISNULL(ConsumoP.ConsumoP1,0) <> 0 THEN ConsumoP.ConsumoP1 ELSE ConsumoPTA.ConsumoP1 END,2) as ConsumoP1, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP2,0) <> 0 THEN ConsumoP.ConsumoP2 ELSE ConsumoPTA.ConsumoP2 END,2) as ConsumoP2, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP3,0) <> 0 THEN ConsumoP.ConsumoP3 ELSE ConsumoPTA.ConsumoP3 END,2) as ConsumoP3, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP4,0) <> 0 THEN ConsumoP.ConsumoP4 ELSE ConsumoPTA.ConsumoP4 END,2) as ConsumoP4, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP5,0) <> 0 THEN ConsumoP.ConsumoP5 ELSE ConsumoPTA.ConsumoP5 END,2) as ConsumoP5, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP6,0) <> 0 THEN ConsumoP.ConsumoP6 ELSE ConsumoPTA.ConsumoP6 END,2) as ConsumoP6,

			ConsumoP.EnergiaPrecioP1 as EnergiaPrecioP1, 
			ConsumoP.EnergiaPrecioP2 as EnergiaPrecioP2,
			ConsumoP.EnergiaPrecioP3 as EnergiaPrecioP3, 
			ConsumoP.EnergiaPrecioP4 as EnergiaPrecioP4, 
			ConsumoP.EnergiaPrecioP5 as EnergiaPrecioP5, 
			ConsumoP.EnergiaPrecioP6 as EnergiaPrecioP6,
	 
			( ISNULL(ConsumoP.ConsumoP1, ISNULL(ConsumoPTA.ConsumoP1,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP1, ISNULL(ConsumoPTA.EnergiaPrecioP1,0.0))
			+ ISNULL(ConsumoP.ConsumoP2, ISNULL(ConsumoPTA.ConsumoP2,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP2, ISNULL(ConsumoPTA.EnergiaPrecioP2,0.0))
			+ ISNULL(ConsumoP.ConsumoP3, ISNULL(ConsumoPTA.ConsumoP3,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP3, ISNULL(ConsumoPTA.EnergiaPrecioP3,0.0))
			+ ISNULL(ConsumoP.ConsumoP4, ISNULL(ConsumoPTA.ConsumoP4,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP4, ISNULL(ConsumoPTA.EnergiaPrecioP4,0.0))
			+ ISNULL(ConsumoP.ConsumoP5, ISNULL(ConsumoPTA.ConsumoP5,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP5, ISNULL(ConsumoPTA.EnergiaPrecioP5,0.0))
			+ ISNULL(ConsumoP.ConsumoP6, ISNULL(ConsumoPTA.ConsumoP6,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP6, ISNULL(ConsumoPTA.EnergiaPrecioP6,0.0))) as CosteActiva,

			 ISNULL(f.ConsumoReactivaP1,0) as ConsumoRP1
			,ISNULL(f.ConsumoReactivaP2,0) as ConsumoRP2
			,ISNULL(f.ConsumoReactivaP3,0) as ConsumoRP3
			,ISNULL(f.ConsumoReactivaP4,0) as ConsumoRP4
			,ISNULL(f.ConsumoReactivaP5,0) as ConsumoRP5
			,ISNULL(f.ConsumoReactivaP6,0) as ConsumoRP6,

			 ISNULL(f.PrecioReactivaP1,0) as PrecioEnergiaRP1
			,ISNULL(f.PrecioReactivaP2,0) as PrecioEnergiaRP2
			,ISNULL(f.PrecioReactivaP3,0) as PrecioEnergiaRP3
			,ISNULL(f.PrecioReactivaP4,0) as PrecioEnergiaRP4
			,ISNULL(f.PrecioReactivaP5,0) as PrecioEnergiaRP5
			,ISNULL(f.PrecioReactivaP6,0) as PrecioEnergiaRP6,

			CASE WHEN ISNULL(ImporteReactiva.Importe,0) <> 0 THEN ImporteReactiva.Importe ELSE  
			(ISNULL(f.ConsumoReactivaP1,0.0) * ISNULL(f.PrecioReactivaP1, 0.0) + ISNULL(f.ConsumoReactivaP2,0.0) * ISNULL(f.PrecioReactivaP2, 0.0) 
			+ ISNULL(f.ConsumoReactivaP3,0.0) * ISNULL(f.PrecioReactivaP3, 0.0) + ISNULL(f.ConsumoReactivaP4,0.0) * ISNULL(f.PrecioReactivaP4, 0.0) 
			+ ISNULL(f.ConsumoReactivaP5,0.0) * ISNULL(f.PrecioReactivaP5, 0.0) + ISNULL(f.ConsumoReactivaP6,0.0) * ISNULL(f.PrecioReactivaP6, 0.0)) END as CosteReactiva,

			  LecturaP.MaximetroP1 as MaximetroP1
			, LecturaP.MaximetroP2 as MaximetroP2
			, LecturaP.MaximetroP3 as MaximetroP3
			, LecturaP.MaximetroP4 as MaximetroP4
			, LecturaP.MaximetroP5 as MaximetroP5
			, LecturaP.MaximetroP6 as MaximetroP6, 

			 Agente.NombreAgente as Comercial,
			 PerfilFacturacion.TextoPerfilFacturacion as PerfilFacturacion, 
			 ConsumoPTA.ImporteTarifaAcceso as ImporteEnergiaAcceso,
			 isnull(ImporteVariable.Importe,0) AS ImporteEnergiaVariable,
			 ImportePotencia.Importe as ImportePotencia,
			 isnull(ExcesoDistribuidora.ImporteBase,0) As ImporteExcesoDistribuidora,
			 '' as SerieNumeroFacturaOrigen,

			CargosPotenciaP.CargosPotenciaP1 as CargoPotenciaP1, 
			CargosPotenciaP.CargosPrecioPotenciaP1 as CargoPotenciaPrecioP1,
			CargospotenciaP.CargosTotalPrecioPotenciaP1,
			CargosPotenciaP.CargosPotenciaP2 as CargoPotenciaP2, 
			CargosPotenciaP.CargosPrecioPotenciaP2 as CargoPotenciaPrecioP2,
			CargospotenciaP.CargosTotalPrecioPotenciaP2,
			CargosPotenciaP.CargosPotenciaP3 as CargoPotenciaP3,
			CargosPotenciaP.CargosPrecioPotenciaP3 as CargoPotenciaPrecioP3,
			CargospotenciaP.CargosTotalPrecioPotenciaP3,
			CargosPotenciaP.CargosPotenciaP4 as CargoPotenciaP4, 
			CargosPotenciaP.CargosPrecioPotenciaP4 as CargoPotenciaPrecioP4,
			CargospotenciaP.CargosTotalPrecioPotenciaP4,
			CargosPotenciaP.CargosPotenciaP5 as CargoPotenciaP5, 
			CargosPotenciaP.CargosPrecioPotenciaP5 as CargoPotenciaPrecioP5,
			CargospotenciaP.CargosTotalPrecioPotenciaP5,
			CargosPotenciaP.CargosPotenciaP6 as CargoPotenciaP6,
			CargosPotenciaP.CargosPrecioPotenciaP6 as CargoPotenciaPrecioP6,
			CargospotenciaP.CargosTotalPrecioPotenciaP6,
			(CargospotenciaP.CargosTotalPrecioPotenciaP1+CargospotenciaP.CargosTotalPrecioPotenciaP2+CargospotenciaP.CargosTotalPrecioPotenciaP3+CargospotenciaP.CargosTotalPrecioPotenciaP4+CargospotenciaP.CargosTotalPrecioPotenciaP5+CargospotenciaP.CargosTotalPrecioPotenciaP6) as CargosTotalPrecioPotencia,
		
			CargosEnergiaP.CargosEnergiaP1 as CargoEnergiaP1,
			CargosEnergiap.CargosPrecioEnergiaP1 as CargoEnergiaPrecioP1,
			CargosEnergiap.CargosTotalPrecioEnergiaP1 as CargosTotalPrecioEnergiaP1,
			CargosEnergiaP.CargosEnergiaP2 as CargoEnergiaP2,
			CargosEnergiap.CargosPrecioEnergiaP2 as CargoEnergiaPrecioP2,
			CargosEnergiap.CargosTotalPrecioEnergiaP2 as CargosTotalPrecioEnergiaP2,
			CargosEnergiaP.CargosEnergiaP3 as CargoEnergiaP3,
			CargosEnergiap.CargosPrecioEnergiaP3 as CargoEnergiaPrecioP3,
			CargosEnergiap.CargosTotalPrecioEnergiaP3 as CargosTotalPrecioEnergiaP3,
			CargosEnergiaP.CargosEnergiaP4 as CargoEnergiaP4,
			CargosEnergiap.CargosPrecioEnergiaP4 as CargoEnergiaPrecioP4,
			CargosEnergiap.CargosTotalPrecioEnergiaP4 as CargosTotalPrecioEnergiaP4,
			CargosEnergiaP.CargosEnergiaP5 as CargoEnergiaP5,
			CargosEnergiap.CargosPrecioEnergiaP5 as CargoEnergiaPrecioP5,
			CargosEnergiap.CargosTotalPrecioEnergiaP5 as CargosTotalPrecioEnergiaP5,
			CargosEnergiaP.CargosEnergiaP6 as CargoEnergiaP6,
			CargosEnergiap.CargosPrecioEnergiaP6 as CargoEnergiaPrecioP6,
			CargosEnergiap.CargosTotalPrecioEnergiaP6 as CargosTotalPrecioEnergiaP6,
			(CargosEnergiap.CargosTotalPrecioEnergiaP1 + CargosEnergiap.CargosTotalPrecioEnergiaP2 + CargosEnergiap.CargosTotalPrecioEnergiaP3 + CargosEnergiap.CargosTotalPrecioEnergiaP4 +CargosEnergiap.CargosTotalPrecioEnergiaP5+ CargosEnergiap.CargosTotalPrecioEnergiaP6)as CargosTotalPrecioEnergia,

			(select sum(importebase) from FacturaVentaLinea where idfacturaventacabecera in (v.IdFacturaVentaCabecera) and facturaconcepto=30006 group by idfacturaventacabecera) as TotalTerminoProductosClick,
		
			(select sum(importebase) from FacturaVentaLinea where idfacturaventacabecera in (v.IdFacturaVentaCabecera) and facturaconcepto=30008 group by idfacturaventacabecera) as AutoConsumo,

			'' as DiferenciaPagosPorCapacidad,
			0 as AjusteCapGas,
			0 as ServicioAjusteSistema

		 FROM FacturasFiltradas as v WITH (NOLOCK)	

		 -- Reactiva
		 left join AgrupadaPeriodoReactiva  as f WITH (NOLOCK) on v.IdFacturaVentaCabecera = f.IdFacturaVentaCabecera

		 -- Consumos
		 left join AgrupadaPeriodoConsumos as ConsumoP WITH (NOLOCK) on v.IdFacturaVentaCabecera = ConsumoP.IdFacturaVentaCabecera

		 -- Consumos Energia Tarifa Acceso
		 left join AgrupadaPeriodoConsumosEnergiaTarifaAcceso as ConsumoPTA WITH (NOLOCK) on v.IdFacturaVentaCabecera = ConsumoPTA.IdFacturaVentaCabecera

		--Potencias
		left join AgrupadaPeriodoPotencia as PotenciaP WITH (NOLOCK) on v.IdFacturaVentaCabecera = PotenciaP.IdFacturaVentaCabecera

		--Maximetros
		left join LecturasAgrupadas as LecturaP WITH (NOLOCK) on v.IdLectura = LecturaP.IdLectura 
		
		--Contrato
		left join Contrato WITH (NOLOCK) on Contrato.IdContrato = v.IdContrato
		
		--Agente
		left join Agente WITH (NOLOCK) on Agente.IdAgente = Contrato.IdAgente
	
		--PerfilFacturacion
		left join PerfilFacturacion WITH (NOLOCK) on v.IdPerfilFacturacion = PerfilFacturacion.IdPerfilFacturacion
	
		--Exceso de potencia
		LEFT JOIN (SELECT IdFacturaVentaCabecera, SUM(ImporteBase) As ImporteBase 
					FROM FacturaVentaLinea WITH (NOLOCK) WHERE FacturaConcepto = 20004 Group By IdFacturaVentaCabecera) 
					As ExcesoPotencia on v.IdFacturaVentaCabecera = ExcesoPotencia.IdFacturaVentaCabecera 

		--Exceso Distribuidora
		LEFT JOIN (SELECT IdFacturaVentaCabecera, SUM(ImporteBase) As ImporteBase
					FROM FacturaVentaLinea WITH (NOLOCK) WHERE FacturaConcepto = 20006 Group By IdFacturaVentaCabecera)
					As ExcesoDistribuidora on v.IdFacturaVentaCabecera = ExcesoDistribuidora.IdFacturaVentaCabecera 

		--Importe Energia Variable
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto=30004
			group by IdFacturaVentaCabecera
		) as ImporteVariable on v.IdFacturaVentaCabecera = ImporteVariable.IdFacturaVentaCabecera
		
		--Importe Productos
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto in (100001,100002, 70020)
			group by IdFacturaVentaCabecera
		) as ImporteProductos on v.IdFacturaVentaCabecera = ImporteProductos.IdFacturaVentaCabecera
		
		--- Importe Potencias
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto in (10001,10002,10003,10005)
			group by IdFacturaVentaCabecera
		) as ImportePotencia on v.IdFacturaVentaCabecera = ImportePotencia.IdFacturaVentaCabecera

		--- Importe Reactiva
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto in (40003)
			group by IdFacturaVentaCabecera
		) as ImporteReactiva on v.IdFacturaVentaCabecera = ImporteReactiva.IdFacturaVentaCabecera

		--cargos
		left join AgrupadaPeriodoCargosEnergia CargosEnergiaP WITH (NOLOCK) ON CargosEnergiaP.idfacturaventacabecera = v.IdFacturaVentaCabecera	
		left join AgrupadaPeriodoCargosPotencia CargosPotenciaP WITH (NOLOCK) ON CargosPotenciaP.idfacturaventacabecera = v.IdFacturaVentaCabecera	

		--Alquiler
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto=50002
			group by IdFacturaVentaCabecera
		) as ImporteAlquiler on v.IdFacturaVentaCabecera = ImporteAlquiler.IdFacturaVentaCabecera

		--IE
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto=60001
			group by IdFacturaVentaCabecera
		) as ImporteIE on v.IdFacturaVentaCabecera = ImporteIE.IdFacturaVentaCabecera

	drop table #TmpId

"

	End Function


	Public Shared Function validacionesScript1() As String
		Return "select c.codigocontrato
,cups.CodigoCUPS
,c.Confirmado
,convert(varchar,c.FechaCreacion, 103) as FechaCreacion
,convert(varchar,c.FechaPrevistaActivacion, 103) as FechaPrevistaActivacion
,stf.TextoFechaEfecto
,c.observaciones 
,cs.textosituacion 
from contrato c 
inner join cups on cups.IdCups = c.idcups
inner join ContratoSituacion cs on c.IdContratoSituacion = cs.IdContratoSituacion
left join SolicitudTipoFechaEfecto stf on stf.IdSolicitudTipoFechaEfecto = c.IdSolicitudTipoFechaEfecto
inner join Solicitud s on s.CodigoContrato = c.CodigoContrato and (s.idsolicitudtipo in (1009
,1010
,1011
,1013
,50103
,50112) or s.IdSolicitudTipo is null)
where c.idcontratosituacion in (4,14) and Confirmado=0
order by c.CodigoContrato"
	End Function
	Public Shared Function validacionesScript2() As String
		Return "select c.codigocontrato
,cups.CodigoCUPS
,c.Confirmado
,convert(varchar,c.FechaCreacion, 103) as FechaCreacion
,convert(varchar,c.FechaPrevistaActivacion, 103) as FechaPrevistaActivacion
,stf.TextoFechaEfecto
,c.observaciones 
,ss.Nombre SituacionSolicitud
,s.FechaApertura FechaAperturaSolicitud
from contrato c 
inner join cups on cups.IdCups = c.idcups
left join SolicitudTipoFechaEfecto stf on stf.IdSolicitudTipoFechaEfecto = c.IdSolicitudTipoFechaEfecto

inner join Solicitud s on s.CodigoContrato = c.CodigoContrato and s.IdUsuario in (1610,
1611,5900,5901,5902,5903,5904,5905,5906,5907,5908,5909,5910,5911,5884,5885,5886,5887,5888,5889,5890,5891,5892,5893,5894,5895,5896,5897,5898,5899) 
left join SolicitudSituacion ss on  s.IdSolicitudSituacion = ss.IdSolicitudSituacion
where c.idcontratosituacion=4 order by s.FechaApertura desc"
	End Function
	Public Shared Function validacionesScript3() As String
		Return ";with 
ContratoTarifaVigenteMaxima as
(    select CodigoContrato,
           max(FechaDesde) as FechaDesde
    from ContratoTarifa
    where GetDate() between FechaDesde and FechaHasta
    group by CodigoContrato)
,ContratoTarifaFechaAjustada as
(    select ct.CodigoContrato,
           ctvm.FechaDesde as fechaCTVM,
           ct.FechaDesde as fechaCT,
           case when ctvm.FechaDesde is not null then ctvm.FechaDesde else ct.FechaDesde end as Fecha
    from ContratoTarifa ct left join ContratoTarifaVigenteMaxima ctvm on ctvm.CodigoContrato = ct.CodigoContrato)
,ContratoTarifaFechaMaxima as
(    select CodigoContrato,
           max(Fecha) as FechaDesde
    from ContratoTarifaFechaAjustada
    group by CodigoContrato)
,ContratoTarifaVigente as
(    select ctfm.Codigocontrato,
           ctfm.FechaDesde,
           Entorno,
           IdTarifaGrupo,
           IdTarifa,
           IdPerfilFacturacion
    from ContratoTarifa ct 
	inner join ContratoTarifaFechaMaxima ctfm on ct.CodigoContrato = ctfm.CodigoContrato and ct.FechaDesde = ctfm.FechaDesde)
select Solicitud.IdSolicitud as Solicitud
,SolicitudTipo.NombreSolicitudTipo as TipoSolicitud
,u.Nombre
,SolicitudSituacion.Nombre as Situacion 
,Cliente.Identidad as Cliente
,Solicitud.CodigoContrato as Contrato
,Contrato.FechaCreacion as 'Fecha creacion contrato'
,CASE
	WHEN (Cliente.Nombre is null) OR (Cliente.Nombre = '')
		THEN Cliente.RazonSocial
		ELSE CONCAT(Cliente.Nombre, ' ' , ISNULL(Cliente.Apellido1, '') , ' ' , ISNULL(Cliente.Apellido2, ''))
		END as Nombre
,CUPS.CodigoCUPS as CUPS
,convert(varchar,Solicitud.FechaApertura,103) as 'Fecha apertura'
,convert(varchar,Solicitud.FechaCierre,103) as 'Fecha cierre'
,convert(varchar,Contrato.FechaAlta,103) as 'Fecha alta'
,convert(varchar,Contrato.FechaPrevistaActivacion,103) as 'F. Prev. Act.'
,SolicitudTipoFechaEfecto.TextoFechaEfecto as 'Texto Fecha Efecto'
,convert(varchar,Contrato.FechaPrevistaBaja,103) as 'F. Prev. Baja'
,convert(varchar,Contrato.FechaBaja,103) as 'Fecha baja'
,Motivobaja.TextoBaja as 'Motivo baja'
,MotivoRechazo.TextoRechazo as 'Motivo Rechazo'
,Solicitud.Observaciones 
,Case
	when Agente.CodigoTipoAgente=2
		then Agente.NombreAgente
    when Agente.CodigoTipoAgente=3
        then Agenteb.NombreAgente
        else null
    End As NombreAgente
,Case
	when Agente.CodigoTipoAgente=3
		then Agente.NombreAgente
        else null
    End As NombreSubAgente
,CASE
	when ClienteContactoTelefono.TipoContacto = 'T'
		then ClienteContactoTelefono.Valor
	when ClienteContactoTelefono.TipoContacto = 'M'
		then ClienteContactoTelefono.Valor
	end as TelefonoAgente
,Tarifa.TextoTarifa As Tarifa
, CONCAT( CallejeroTipoVia.TextoVia,' ',Callejero.NombreCalle, ' ', Cliente.Numero, ' ' ,Cliente.Aclarador) as Direccion
, Ciudad.TextoCiudad as Pobllacion
, ClienteContactoEmail.Valor as EMail
,eq.IdEquipoMedida as 'Nº Equipo Medida'
,ContratoPotenciaMaxima.PotenciaMaxima as 'Potencia Actual'
,Solicitud.ValorTrafo as 'Situación Libre'
,ModoLectura.Descripcion as 'Modo lectura'
from Solicitud with(nolock)
left join Usuario u with(nolock) on u.IdUsuario = Solicitud.IdUsuario
left join SolicitudTipoFechaEfecto on Solicitud.IdSolicitudTipoFechaEfecto=SolicitudTipoFechaEfecto.IdSolicitudTipoFechaEfecto
left join SolicitudTipo with(nolock) on SolicitudTipo.IdSolicitudTipo = Solicitud.IdSolicitudTipo
left join SolicitudSituacion with(nolock) on SolicitudSituacion.IdSolicitudSituacion = Solicitud.IdSolicitudSituacion
left join Contrato with(nolock) on Contrato.CodigoContrato = Solicitud.CodigoContrato
left join Cliente with(nolock) on Cliente.IdCliente = Contrato.IdCliente
left join CUPS with(nolock) on CUPS.IdCups = Contrato.IdCups
left join MotivoBaja with(nolock) on MotivoBaja.IdMotivoBaja = Contrato.IdMotivoBaja
left join MotivoRechazo with(nolock) on MotivoRechazo.IdMotivoRechazo = Solicitud.IdMotivoRechazo
left join Agente with(nolock) on Agente.IdAgente = Contrato.IdAgente
left join (select IdAgente, NombreAgente, IdAgenteNivelAnterior from Agente with(nolock)) as Agenteb on Agenteb.IdAgente = Agente.IdAgenteNivelAnterior
left join ClienteContacto as ClienteContactoTelefono with(nolock) on ClienteContactoTelefono.IdCliente = Cliente.IdCliente AND ClienteContactoTelefono.TipoContacto = 'T' and ClienteContactoTelefono.PorDefecto = 1
left join Tarifa with(nolock) on Tarifa.IdTarifa = Contrato.IdTarifa
left join Callejero with(nolock) on Callejero.IdCallejero = Cliente.IdCallejero
left join CallejeroTipoVia with(nolock) on CallejeroTipoVia.IdCallejeroTipoVia = Callejero.IdCallejeroTipoVia
left join Ciudad with(nolock) on Ciudad.IdCiudad = CUPS.IdCiudad
left join ClienteContacto as ClienteContactoEmail with(nolock) on ClienteContactoEmail.IdCliente = Cliente.IdCliente AND ClienteContactoEmail.TipoContacto = 'E' and ClienteContactoEmail.PorDefecto = 1
LEFT JOIN (Select Top 1 Entorno, CodigoContrato, NumeroSerie, IdEquipoModelo, Min(IdEquipoMedida) as IdEquipoMedida, IsInstalado as IsInstalado from EquipoMedida with(nolock) where isinstalado=1  group by Entorno, CodigoContrato, NumeroSerie, IdEquipoModelo, IsInstalado) as eq ON (Contrato.CodigoContrato = eq.CodigoContrato and eq.Entorno = 'G2')
LEFT JOIN EquipoModelo with(nolock) ON (eq.IdEquipoModelo =EquipoModelo.IdEquipoModelo) 
LEFT JOIN (
			SELECT	IdContrato, MAX(PotenciaContratada)	AS PotenciaMaxima 
				FROM ContratoPotencia with(nolock)
				GROUP BY IdContrato
			  ) AS ContratoPotenciaMaxima 
			  ON ContratoPotenciaMaxima.IdContrato = Contrato.IdContrato
LEFT JOIN ModoLectura with(nolock) ON Solicitud.IdModoLectura = ModoLectura.IdModoLectura
left join ContratoTarifaVigente with (nolock) on ContratoTarifaVigente.CodigoContrato = Contrato.CodigoContrato
left join TarifaGrupo with (nolock) on TarifaGrupo.IdTarifaGrupo = ContratoTarifaVigente.IdTarifaGrupo
where   Solicitud.FechaApertura  >= DATEADD (dd, 0, DATEDIFF (dd, 0, GETDATE() - 1))
order by Solicitud.IdSolicitudTipo, Solicitud.FechaApertura"
	End Function

	Public Shared Function ConsultaTop() As String
		Return "--Erick _Gestor Sige_ ConsultaTop
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
,FacturasEnergiaML.IdFacturaVentaCabecera"
	End Function

	Public Shared Function BuscarFacturaATR(facturasatr As List(Of String)) As String
		Dim facturasatrBD = String.Join(",", facturasatr.Select(Function(c) $"'{c.Trim}'"))
		Return $"select fc.idfacturacompracabecera, idlectura,Perfilar,numerofactura from FacturaCompraCabecera fc
left join lectura l on fc.IdFacturaCompraCabecera = l.IdFacturaCompraCabecera
where numerofactura in ({facturasatrBD})
and facturar=1 and Vigente=1"
	End Function
End Class
