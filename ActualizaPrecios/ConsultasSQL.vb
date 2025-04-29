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
left join TarifaPeajePrecio tpp1a with (nolock) on l.IdTarifaPeaje = tpp1a.IdTarifaPeaje and tpp1a.fechaInicio='01/01/2023' and tpp1a.idtarifapeajeperiodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join TarifaPeajePrecio tpp2a with (nolock) on l.IdTarifaPeaje = tpp2a.IdTarifaPeaje and tpp2a.fechaInicio='01/01/2023' and tpp2a.idtarifapeajeperiodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join TarifaPeajePrecio tpp3a with (nolock) on l.IdTarifaPeaje = tpp3a.IdTarifaPeaje and tpp3a.fechaInicio='01/01/2023' and tpp3a.idtarifapeajeperiodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join TarifaPeajePrecio tpp4a with (nolock) on l.IdTarifaPeaje = tpp4a.IdTarifaPeaje and tpp4a.fechaInicio='01/01/2023' and tpp4a.idtarifapeajeperiodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join TarifaPeajePrecio tpp5a with (nolock) on l.IdTarifaPeaje = tpp5a.IdTarifaPeaje and tpp5a.fechaInicio='01/01/2023' and tpp5a.idtarifapeajeperiodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join TarifaPeajePrecio tpp6a with (nolock) on l.IdTarifaPeaje = tpp6a.IdTarifaPeaje and tpp6a.fechaInicio='01/01/2023' and tpp6a.idtarifapeajeperiodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
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
,replace(c.consumoestimado,'.',',') as ConsumoEstimado
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
				  where Identidad in ('B88425707','A48138051','B33382433','B88586953','B78078904','B88586953','B78078904')) 
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
	Public Shared Function GetCurvaHoraria(DesdeFecha As Date, hastaFecha As Date, Cups As List(Of String)) As String
		Dim joinCups = String.Join(",", Cups.Select(Function(c) $"'{c.Trim}'"))
		Return $"
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

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H
WHERE left(cups,20) IN ({joinCups}) AND FechaMedida BETWEEN '{DesdeFecha:dd-MM-yyyy}' AND '{hastaFecha:dd-MM-yyyy}'

ORDER BY FechaMedida;"
	End Function


	Public Shared Function GetCurvaCuartoHoraria(DesdeFecha As Date, hastaFecha As Date, Cups As List(Of String)) As String
		Dim joinCups = String.Join(",", Cups.Select(Function(c) $"'{c.Trim}'"))

		Return $"SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_032024
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_092024
WHERE left(cups,20) in( {joinCups})  AND FechaMedida between  '{DesdeFecha.ToString("dd/MM/yyyy")}' and '{hastaFecha.ToString("dd/MM/yyyy")}'"
	End Function
End Class
