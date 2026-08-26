--Erick _Gestor Sige_ TREBOL Luz By Identidad V2
;WITH 
FacturasVentaConsulta AS (
    SELECT fvc.IdFacturaVentaCabecera, fvc.CodigoContrato, fvc.IdContrato, fvc.SerieFactura, fvc.FechaFactura, IdFacturaOrigen,IsAbono
    FROM FacturaVentaCabecera fvc WITH (NOLOCK)
	inner join cliente cl on fvc.idcliente = cl.idcliente
    WHERE 
	fvc.Entorno='E1' and
	fvc.SerieFactura IS NOT NULL AND
	 fvc.FechaFactura >= 'DesdeFechaReplace' and fvc.FechaFactura <= 'HastaFechaReplace' and Identidad ='identidadReplace'
)
,
ContratoPot as(
select 
c.codigocontrato,
 replace(isnull(max(case when cp.IdTarifaPeajePeriodo in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then cp.PotenciaContratada end),0),'.',',') Cp1
,replace(isnull(max(case when cp.IdTarifaPeajePeriodo in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then cp.PotenciaContratada end),0),'.',',') Cp2
,replace(isnull(max(case when cp.IdTarifaPeajePeriodo in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then cp.PotenciaContratada end),0),'.',',') Cp3
,replace(isnull(max(case when cp.IdTarifaPeajePeriodo in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then cp.PotenciaContratada end),0),'.',',') Cp4
,replace(isnull(max(case when cp.IdTarifaPeajePeriodo in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then cp.PotenciaContratada end),0),'.',',') Cp5
,replace(isnull(max(case when cp.IdTarifaPeajePeriodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then cp.PotenciaContratada end),0),'.',',') Cp6
 from contrato c with (nolock)
 inner join  FacturasVentaConsulta on c.codigocontrato=FacturasVentaConsulta.codigocontrato
left join ContratoPotencia cp with (nolock) on cp.idcontrato = c.idcontrato
--where c.idcontrato=159564
group by c.CodigoContrato
)
,
LecturasV1 as (
select idlectura,fvc.idfacturaventacabecera,l.idtarifapeaje,
l.PotenciaMaxP1 PotenciaMaxPeriodo1,
l.PotenciaMaxP2 PotenciaMaxPeriodo2,
l.PotenciaMaxP3 PotenciaMaxPeriodo3,
l.PotenciaMaxP4 PotenciaMaxPeriodo4,
l.PotenciaMaxP5 PotenciaMaxPeriodo5,
l.PotenciaMaxP6 PotenciaMaxPeriodo6, l.IdLecturaOrigen
from  lectura l  
inner join FacturasVentaConsulta fvc WITH (NOLOCK) on fvc.idfacturaventacabecera = l.IdFacturaVentaCabeceraSectorC 
UNION ALL
-- Parte 2: join por IdFacturaOrigen solo si es abono
SELECT 
    l.IdLectura,
    fvc.IdFacturaVentaCabecera,
    l.IdTarifaPeaje,
    l.PotenciaMaxP1 AS PotenciaMaxPeriodo1,
    l.PotenciaMaxP2 AS PotenciaMaxPeriodo2,
    l.PotenciaMaxP3 AS PotenciaMaxPeriodo3,
    l.PotenciaMaxP4 AS PotenciaMaxPeriodo4,
    l.PotenciaMaxP5 AS PotenciaMaxPeriodo5,
    l.PotenciaMaxP6 AS PotenciaMaxPeriodo6, l.IdLecturaOrigen
FROM Lectura l
INNER JOIN FacturasVentaConsulta fvc WITH (NOLOCK) 
    ON fvc.IdFacturaOrigen = l.IdFacturaVentaCabeceraSectorC
   AND fvc.IsAbono = 1
)
,LecturaLineasV1 as (
select ll.IdLectura
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then ll.Maximetro end),0),'.',',') LectMaximetroP1
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then ll.Maximetro end),0),'.',',') LectMaximetroP2
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then ll.Maximetro end),0),'.',',') LectMaximetroP3
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then ll.Maximetro end),0),'.',',') LectMaximetroP4
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then ll.Maximetro end),0),'.',',') LectMaximetroP5
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then ll.Maximetro end),0),'.',',') LectMaximetroP6

, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then ll.ConsumoActiva end),0),'.',',') ConsumoActivaP1
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then ll.ConsumoActiva end),0),'.',',') ConsumoActivaP2
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then ll.ConsumoActiva end),0),'.',',') ConsumoActivaP3
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then ll.ConsumoActiva end),0),'.',',') ConsumoActivaP4
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then ll.ConsumoActiva end),0),'.',',') ConsumoActivaP5
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then ll.ConsumoActiva end),0),'.',',') ConsumoActivaP6

, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then ll.ActivaExtra end),0),'.',',') ActivaExtraP1
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then ll.ActivaExtra end),0),'.',',') ActivaExtraP2
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then ll.ActivaExtra end),0),'.',',') ActivaExtraP3
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then ll.ActivaExtra end),0),'.',',') ActivaExtraP4
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then ll.ActivaExtra end),0),'.',',') ActivaExtraP5
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then ll.ActivaExtra end),0),'.',',') ActivaExtraP6

, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then ll.activaActual end),0),'.',',') LectActivaP1
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then ll.activaActual end),0),'.',',') LectActivaP2
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then ll.activaActual end),0),'.',',') LectActivaP3
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then ll.activaActual end),0),'.',',') LectActivaP4
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then ll.activaActual end),0),'.',',') LectActivaP5
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then ll.activaActual end),0),'.',',') LectActivaP6

, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then ll.ActivaAnterior end),0),'.',',') LectAntActivaP1
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then ll.ActivaAnterior end),0),'.',',') LectAntActivaP2
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then ll.ActivaAnterior end),0),'.',',') LectAntActivaP3
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then ll.ActivaAnterior end),0),'.',',') LectAntActivaP4
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then ll.ActivaAnterior end),0),'.',',') LectAntActivaP5
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then ll.ActivaAnterior end),0),'.',',') LectAntActivaP6

, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then ll.ReactivaActual end),0),'.',',') LectReActivaP1
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then ll.ReactivaActual end),0),'.',',') LectReActivaP2
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then ll.ReactivaActual end),0),'.',',') LectReActivaP3
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then ll.ReactivaActual end),0),'.',',') LectReActivaP4
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then ll.ReactivaActual end),0),'.',',') LectReActivaP5
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then ll.ReactivaActual end),0),'.',',') LectReActivaP6

, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then ll.ReActivaAnterior end),0),'.',',') LectAntReActivaP1
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then ll.ReActivaAnterior end),0),'.',',') LectAntReActivaP2
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then ll.ReActivaAnterior end),0),'.',',') LectAntReActivaP3
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then ll.ReActivaAnterior end),0),'.',',') LectAntReActivaP4
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then ll.ReActivaAnterior end),0),'.',',') LectAntReActivaP5
, replace(isnull(max(case when ll.idtarifapeajeperiodolectura in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then ll.ReActivaAnterior end),0),'.',',') LectAntReActivaP6
from lecturalinea ll
inner join LecturasV1 on ll.idlectura =LecturasV1.idlectura 
group by ll.IdLectura
),
PeajesPrecios as (
select lecturasv1.IdTarifaPeaje
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then tpp.PotenciaPrecio end),0),'.',',') PrecioPotenciaP1
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then tpp.PotenciaPrecio end),0),'.',',') PrecioPotenciaP2
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then tpp.PotenciaPrecio end),0),'.',',') PrecioPotenciaP3
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then tpp.PotenciaPrecio end),0),'.',',') PrecioPotenciaP4
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then tpp.PotenciaPrecio end),0),'.',',') PrecioPotenciaP5
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then tpp.PotenciaPrecio end),0),'.',',') PrecioPotenciaP6

, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then tpp.CargoPotenciaPrecio end),0),'.',',') CargoPotenciaPrecioP1
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then tpp.CargoPotenciaPrecio end),0),'.',',')  CargoPotenciaPrecioP2
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then tpp.CargoPotenciaPrecio end),0),'.',',') CargoPotenciaPrecioP3
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then tpp.CargoPotenciaPrecio end),0),'.',',')  CargoPotenciaPrecioP4
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then tpp.CargoPotenciaPrecio end),0),'.',',')  CargoPotenciaPrecioP5
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then tpp.CargoPotenciaPrecio end),0),'.',',')  CargoPotenciaPrecioP6

, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then tpp.EnergiaPrecio end),0),'.',',') PrecioEnergiaP1
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then tpp.EnergiaPrecio end),0),'.',',') PrecioEnergiaP2
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then tpp.EnergiaPrecio end),0),'.',',') PrecioEnergiaP3
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then tpp.EnergiaPrecio end),0),'.',',') PrecioEnergiaP4
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then tpp.EnergiaPrecio end),0),'.',',') PrecioEnergiaP5
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then tpp.EnergiaPrecio end),0),'.',',') PrecioEnergiaP6

, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101) then tpp.CargoEnergiaPrecio end),0),'.',',') CargoEnergiaPrecioP1
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102) then tpp.CargoEnergiaPrecio end),0),'.',',') CargoEnergiaPrecioP2
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103) then tpp.CargoEnergiaPrecio end),0),'.',',') CargoEnergiaPrecioP3
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203004,20206104,20206204,20206304,20206404,20208004,20208104) then tpp.CargoEnergiaPrecio end),0),'.',',') CargoEnergiaPrecioP4
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203005,20206105,20206205,20206305,20206405,20208005,20208105) then tpp.CargoEnergiaPrecio end),0),'.',',') CargoEnergiaPrecioP5
, replace(isnull(max(case when tpp.idtarifapeajeperiodo in  (20203006,20206106,20206206,20206306,20206406,20208006,20208106) then tpp.CargoEnergiaPrecio end),0),'.',',') CargoEnergiaPrecioP6
from TarifaPeajePrecio tpp
inner join LecturasV1 on tpp.IdTarifaPeaje =LecturasV1.IdTarifaPeaje 
where tpp.fechafinal is null
group by LecturasV1.IdTarifaPeaje
)
,
LineasFactura as (
select FacturaVentaLinea.IdFacturaVentaCabecera,IdFacturaVentaLinea,Entorno,FacturaConcepto,InfoLineaXML,ImporteBase,IsAjusteCAPGas,Descripcion
from FacturaVentaLinea WITH (NOLOCK)
inner join FacturasVentaConsulta WITH (NOLOCK) on FacturasVentaConsulta.IdFacturaVentaCabecera = FacturaVentaLinea.IdFacturaVentaCabecera 
--where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta WITH (NOLOCK))
)
,ImporteClick (idfacturaventacabecera,ImporteClickTotal) as(
Select idfacturaventacabecera,replace(sum(importebase),'.',',')ImporteClickTotal from LineasFactura with(nolock) where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta WITH (NOLOCK))
and Facturaconcepto=30006 and (isajustecapgas=0 or isajustecapgas is null)
group by IdFacturaVentaCabecera
)
,ImporteClickDesglosado AS (
    Select lf.idfacturaventacabecera,replace(lf.importebase,'.',',') importebase,lf.descripcion,
        ROW_NUMBER() OVER (PARTITION BY lf.idfacturaventacabecera ORDER BY lf.idfacturaventacabecera) AS LineaNumero
    from LineasFactura lf WITH (NOLOCK)
    where 
        lf.IdFacturaVentaCabecera IN (select IdFacturaVentaCabecera from FacturasVentaConsulta WITH (NOLOCK))
        AND lf.Facturaconcepto = 30006 AND (lf.isajustecapgas = 0 OR lf.isajustecapgas IS NULL)
)
,
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
	WHERE facturaventacabecera.IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta))
	--,PreseleccionContratos as (
--select IdContrato, CodigoContrato from Contrato 
--where Entorno = 'E1' and Contrato.CodigoContrato in (select codigocontrato from FacturasVentaConsulta)
--)
,PreseleccionFacturas as (
select FacturaVentaCabecera.IdFacturaVentaCabecera, FacturaVentaCabecera.SerieFactura, NumeroFactura, FacturaCategoria, FacturasVentaConsulta.CodigoContrato, FacturaVentaCabecera.FechaFactura, FacturaVentaCabecera.IdFacturaTipo,facturaventacabecera.fechalecturaactualxml,facturaventacabecera.fechalecturaanteriorxml
,IdTipoImpuesto,
IdFacturaRectificativa, IdFacturaAbono, FacturaVentaCabecera.IdFacturaOrigen, cast(isnull(InfoCabeceraXML.value('(//ConsumoActiva)[1]', 'nvarchar(max)'), '0') as decimal(18,2)) as Consumo, IsGestinel, IdContratoDocumento,
FacturasVentaConsulta.IdContrato, IdCanal, FacturaTipo.TextoFacturaTipo
from FacturaVentaCabecera with(nolock)
inner join FacturasVentaConsulta WITH (NOLOCK) on FacturasVentaConsulta.idfacturaventacabecera = FacturaVentaCabecera.IdFacturaVentaCabecera 
left join FacturaTipo WITH (NOLOCK) on FacturaTipo.IdFacturaTipo = FacturaVentaCabecera.IdFacturaTipo
where FacturaVentaCabecera.Entorno = 'E1' and IsFactura = 1
),
Lineas as (
select PreseleccionFacturas.IdFacturaVentaCabecera, FacturaConcepto, ImporteBase,InfoLineaXML  from PreseleccionFacturas WITH (NOLOCK)
inner join LineasFactura WITH (NOLOCK) on LineasFactura.IdFacturaVentaCabecera = PreseleccionFacturas.IdFacturaVentaCabecera
),
ImportesPotencia as (
select Lineas.IdFacturaVentaCabecera, sum(Lineas.ImporteBase) as Importe from Lineas WITH (NOLOCK)
where (FacturaConcepto between 10000 and 19999 or FacturaConcepto in (130001,130002, 131001)) or 
(FacturaConcepto in (90003, 90018, 90035,90036,90039,90040, 90042, 90043, 90044, 90045, 90049, 90050, 90053, 90054, 90056, 90057, 90058, 90059, 90070))
group by Lineas.IdFacturaVentaCabecera
),
ImportesEnergia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas WITH (NOLOCK)
where (FacturaConcepto between 30000 and 39999 or FacturaConcepto in (130003,130004, 131003)) or 
(FacturaConcepto in (90001,90002,90012,90031,90032,90062,90038,90048,90052))
group by Lineas.IdFacturaVentaCabecera
),
ImportesReactiva as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas WITH (NOLOCK)
where FacturaConcepto between 40000 and 49999 
group by Lineas.IdFacturaVentaCabecera
),
ImportesExcesos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas WITH (NOLOCK)
where (FacturaConcepto between 20000 and 29999) or (FacturaConcepto in (90037,90041,90051,90055))
group by Lineas.IdFacturaVentaCabecera
),
DescuentosPotencia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas WITH (NOLOCK)
where (FacturaConcepto in (120001, 120004)) or (FacturaConcepto in (120006, 90007))
group by Lineas.IdFacturaVentaCabecera
),
DescuentosEnergia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas WITH (NOLOCK)
where (FacturaConcepto in (120002, 120005)) or (FacturaConcepto in (120007, 90006))
group by Lineas.IdFacturaVentaCabecera
),
ImportesAlquileres as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas WITH (NOLOCK)
where (FacturaConcepto between 50000 and 59999) or (FacturaConcepto in (120007, 90006))
group by Lineas.IdFacturaVentaCabecera
),
ImportesProductos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas WITH (NOLOCK)
where FacturaConcepto in (100001, 100002, 110001, 90009)
group by Lineas.IdFacturaVentaCabecera
),
ImportesProductosDesglosado as (
Select IdFacturaVentaCabecera,IdProducto,sum(ImporteBase) AS Importe
from (Select Lineas.IdFacturaVentaCabecera, Lineas.ImporteBase
,       
			CASE 
            WHEN InfoLineaXML.value('(//ConceptoProductos//IdProducto)[1]', 'int') not in (4,5,27,28,18,90,113,122,158,202,207,251,248) THEN 1000
            ELSE InfoLineaXML.value('(//ConceptoProductos//IdProducto)[1]', 'int')
        END AS IdProducto
 from Lineas WITH (NOLOCK) WHERE FacturaConcepto IN (100001, 100002, 110001, 90009)
) as Subconsulta
group by IdFacturaVentaCabecera,IdProducto
)
, ProductosContrato as (
Select pa.IdContrato,pa.IdProducto,pa.Importe,AplicarPrecioConsumo,producto.TextoProducto from ProductoAsignacion pa WITH (NOLOCK)
inner join producto WITH (NOLOCK) on pa.IdProducto =producto.IdProducto
where pa.IdContrato in (select IdContrato from FacturasVentaConsulta)--and pa.AplicarPrecioConsumo=1
)
,FacturaOrigen as (
select FacturaVentaCabecera.IdFacturaVentaCabecera,SerieNumFactura from FacturaVentaCabecera WITH (NOLOCK) where IdFacturaVentaCabecera in 
(select IdFacturaOrigen from FacturaVentaCabecera where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta WITH (NOLOCK)))
)


,PreConsulta as(
Select distinct
fvc.idfacturaventacabecera idf,
fvc.idcontrato,
cl.Identidad as CIFDNI
,dbo.formateardenominacion(cl.nombre,cl.apellido1,cl.apellido2,cl.razonsocial) as RazonSocial
,cll.NombreCalle +' '+ cups.Aclarador as Direccion
,CUPS.CodigoCUPS
,cups.CodPostal
,ciu.TextoCiudad as Poblacion
,pv.TextoProvincia as Provincia
,c.codigocontrato
,c.NumPedidoFacturacion
,replace(cp1,'.',',') as PotContratadaP1
,replace(cp2,'.',',') as PotContratadaP2
,replace(cp3,'.',',') as PotContratadaP3
,replace(cp4,'.',',') as PotContratadaP4
,replace(cp5,'.',',') as PotContratadaP5
,replace(cp6,'.',',') as PotContratadaP6
,t.textotarifa as Tarifa
,d.NombreFiscal as Distribuidora
,tg.TextoTarifaGrupo as Grupo
,convert(varchar,fvc.FechaFactura, 103) as FechaFactura
,replace(fvt3.ImporteTotal,'.',',') As 'ImporteTotalIGIC3%(Canarias)'
,replace(fvt7.ImporteTotal,'.',',') As 'ImporteTotalIGIC7%(Canarias)'
,replace(isnull(fvt.ImporteTotal,0)+ isnull(fvt3.ImporteTotal,0) + isnull(fvt7.ImporteTotal,0),'.',',') As ImporteTotal
,fvc.SerieFactura+' '+cast (fvc.NumeroFactura as varchar) As NumeroFactura
,fo.SerieNumFactura as FacturaOrigen
,convert(varchar, fvc.FechaLecturaAnteriorXML, 103) as FechaDesde
,convert(varchar,fvc.FechaLecturaActualXML, 103) as FechaHasta,
Lorigenes.TextoOrigen Origen_Lectura,
replace(isnull(ConsumoVariableP1,0)+
isnull(ConsumoVariableP2,0)+
isnull(ConsumoVariableP3,0)+
isnull(ConsumoVariableP4,0)+
isnull(ConsumoVariableP5,0)+
isnull(ConsumoVariableP6,0),'.',',')
As ConsumoTotalKwh
,replace(ISNULL(fvlIE.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoImpuesto/PorcentajeAplicado)[1]', 'decimal(18,6)'), 0),'.',',') PorcentajeAplicadoImpuestoElectrico
,replace(ISNULL(fvlIE.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoImpuesto/BaseLiquidable)[1]', 'decimal(18,6)'), 0),'.',',') BaseLiquidableImpuestoElectrico
,replace(fvlIE.ImporteBase,'.',',') As ImpuestoElectrico
,replace(fvlCON.ImporteBase,'.',',') As AlquilerContador
,fvt3.PorcentajeImpuesto As 'PorcentajeIGIC3%(Canarias)'
,fvt7.PorcentajeImpuesto As 'PorcentajeIGIC7%(Canarias)'
,fvt.PorcentajeImpuesto As PorcentajeIVA
,replace(fvt3.ImporteBase,'.',',') As 'BaseIGIC3%(Canarias)'
,replace(fvt7.ImporteBase,'.',',') As 'BaseIGIC7%(Canarias)'
,replace(fvt.ImporteBase,'.',',') As BaseIVA
,replace(fvt3.ImporteImpuesto,'.',',') As 'ImporteIGIC3%(Canarias)'
,replace(fvt7.ImporteImpuesto,'.',',') As 'ImporteIGIC7%(Canarias)'
,replace(fvt.ImporteImpuesto,'.',',') As ImporteIVA
,PotenciaMaxPeriodo1
,PotenciaMaxPeriodo2
,PotenciaMaxPeriodo3
,PotenciaMaxPeriodo4
,PotenciaMaxPeriodo5
,PotenciaMaxPeriodo6
,peajesP.PrecioPotenciaP1
,peajesP.CargoPotenciaPrecioP1
,peajesP.PrecioPotenciaP2
,peajesP.CargoPotenciaPrecioP2
,peajesP.PrecioPotenciaP3
,peajesP.CargoPotenciaPrecioP3
,peajesP.PrecioPotenciaP4
,peajesP.CargoPotenciaPrecioP4
,peajesP.PrecioPotenciaP5
,peajesP.CargoPotenciaPrecioP5
,peajesP.PrecioPotenciaP6
,peajesP.CargoPotenciaPrecioP6
,peajesP.PrecioEnergiaP1
,peajesP.CargoEnergiaPrecioP1
,peajesP.PrecioEnergiaP2
,peajesP.CargoEnergiaPrecioP2
,peajesP.PrecioEnergiaP3
,peajesP.CargoEnergiaPrecioP3
,peajesP.PrecioEnergiaP4
,peajesP.CargoEnergiaPrecioP4
,peajesP.PrecioEnergiaP5
,peajesP.CargoEnergiaPrecioP5
,peajesP.PrecioEnergiaP6
,peajesP.CargoEnergiaPrecioP6
,replace(fvlMAX.ImporteBase,'.',',') as ImporteMaximetro
,ll.ConsumoActivaP1
,ll.ActivaExtraP1
,ll.ConsumoActivaP2
,ll.ActivaExtraP2
,ll.ConsumoActivaP3
,ll.ActivaExtraP3
,ll.ConsumoActivaP4
,ll.ActivaExtraP4
,ll.ConsumoActivaP5
,ll.ActivaExtraP5
,ll.ConsumoActivaP6
,ll.ActivaExtraP6
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
,replace(isnull(Importesreactiva.Importe, 0),'.',',')as 'CosteReactiva'
,ll.LectActivaP1
,ll.LectActivaP2
,ll.LectActivaP3
,ll.LectActivaP4
,ll.LectActivaP5
,ll.LectActivaP6
,ll.LectAntActivaP1
,ll.LectAntActivaP2
,ll.LectAntActivaP3
,ll.LectAntActivaP4
,ll.LectAntActivaP5
,ll.LectAntActivaP6
,ll.LectReActivaP1
,ll.LectReActivaP2
,ll.LectReActivaP3
,ll.LectReActivaP4
,ll.LectReActivaP5
,ll.LectReActivaP6
,ll.LectAntReActivaP1
,ll.LectAntReActivaP2
,ll.LectAntReActivaP3
,ll.LectAntReActivaP4
,ll.LectAntReActivaP5
,ll.LectAntReActivaP6
,ll.LectMaximetroP1
,ll.LectMaximetroP2
,ll.LectMaximetroP3
,ll.LectMaximetroP4
,ll.LectMaximetroP5
,ll.LectMaximetroP6
,replace(isnull(ImportesPotencia.Importe, 0),'.',',') as ImportePotenciaContratacion
,replace(isnull(ImportesEnergia.Importe, 0),'.',',') as ImporteEnergiaContratacion
,replace(isnull(ImportesReactiva.Importe, 0),'.',',') as ImporteReactivaContratacion
,replace(isnull(ImportesExcesos.Importe, 0),'.',',') as ImporteExcesosContratacion
,replace(isnull(DescuentosPotencia.Importe, 0),'.',',') as DescuentoPotenciaContratacion
,replace(isnull(DescuentosEnergia.Importe, 0),'.',',') as DescuentoEnergiaContratacion
,replace(isnull(ImportesAlquileres.Importe, 0),'.',',') as ImporteAlquilerContratacion
,replace(isnull(ImportesProductos.Importe, 0),'.',',') as ImporteProductosContratacion
,replace(fvl_Energia.ImporteBaseV_P1,'.',',') as ImporteVariableP1
,replace(fvl_Energia.ImporteBaseV_P2,'.',',') as ImporteVariableP2
,replace(fvl_Energia.ImporteBaseV_P3,'.',',') as ImporteVariableP3
,replace(fvl_Energia.ImporteBaseV_P4,'.',',') as ImporteVariableP4
,replace(fvl_Energia.ImporteBaseV_P5,'.',',') as ImporteVariableP5
,replace(fvl_Energia.ImporteBaseV_P6,'.',',') as ImporteVariableP6
,replace(fvlCAP.importebase,'.',',') as ImporteCAPGAS
,replace(ConsumoVariableP1,'.',',') AS ConsumoVariableP1
,replace(PrecioVariableP1,'.',',') AS PrecioVariableP1
,replace(ConsumoVariableP2,'.',',') AS ConsumoVariableP2
,replace(PrecioVariableP2,'.',',') AS PrecioVariableP2
,replace(ConsumoVariableP3,'.',',') AS ConsumoVariableP3
,replace(PrecioVariableP3,'.',',') AS PrecioVariableP3
,replace(ConsumoVariableP4,'.',',') AS ConsumoVariableP4
,replace(PrecioVariableP4,'.',',') AS PrecioVariableP4
,replace(ConsumoVariableP5,'.',',') AS ConsumoVariableP5
,replace(PrecioVariableP5,'.',',') AS PrecioVariableP5
,replace(ConsumoVariableP6,'.',',') AS ConsumoVariableP6
,replace(PrecioVariableP6,'.',',') AS PrecioVariableP6
,replace(fvlp_Potencia.PrecioCargoPotenciaP1,'.',',')  PrecioCargoPontenciaP1
,replace(fvlp_Potencia.PrecioCargoPotenciaP2,'.',',')PrecioCargoPontenciaP2
,replace(fvlp_Potencia.PrecioCargoPotenciaP3,'.',',')PrecioCargoPontenciaP3
,replace(fvlp_Potencia.PrecioCargoPotenciaP4,'.',',')PrecioCargoPontenciaP4
,replace(fvlp_Potencia.PrecioCargoPotenciaP5,'.',',')PrecioCargoPontenciaP5
,replace(fvlp_Potencia.PrecioCargoPotenciaP6,'.',',') PrecioCargoPontenciaP6
,replace(PrecioCargoEnergiaP1,'.',',') AS PrecioCargoEnergiaP1
,replace(PrecioCargoEnergiaP2,'.',',') AS PrecioCargoEnergiaP2
,replace(PrecioCargoEnergiaP3,'.',',') AS PrecioCargoEnergiaP3
,replace(PrecioCargoEnergiaP4,'.',',') AS PrecioCargoEnergiaP4
,replace(PrecioCargoEnergiaP5,'.',',') AS PrecioCargoEnergiaP5
,replace(PrecioCargoEnergiaP6,'.',',') AS PrecioCargoEnergiaP6
,(ConsumoAutoconsumoP1
+ConsumoAutoconsumoP2
+ConsumoAutoconsumoP3
+ConsumoAutoconsumoP4
+ConsumoAutoconsumoP5
+ConsumoAutoconsumoP6
+ConsumoAutoconsumoUnLinea) ConsumoAutoConsumo
,(PrecioAutoConsumoP1
+PrecioAutoConsumoP2
+PrecioAutoConsumoP3
+PrecioAutoConsumoP4
+PrecioAutoConsumoP5
+PrecioAutoConsumoP6
+PrecioAutoconsumoUnLinea) PrecioAutoconsumo
,(ImporteBaseAutoconsumoP1
+ImporteBaseAutoconsumoP2
+ImporteBaseAutoconsumoP3
+ImporteBaseAutoconsumoP4
+ImporteBaseAutoconsumoP5
+ImporteBaseAutoconsumoP6
+ImporteAutoconsumoUnLinea) ImporteAutoconsumo
----,fvlClickAjuste.descripcion As DescripcionAjusteClick
----,replace(fvlClickAjuste.ImporteBase,'.',',') As ImporteAjusteClick

from contrato c with (nolock)
inner join cliente cl with (nolock) on cl.idcliente = c.idcliente
inner join CUPS with (nolock) on c.idcups = cups.idcups
inner join callejero cll with (nolock) on cll.IdCallejero = cups.IdCallejero
inner join Ciudad ciu with (nolock) on ciu.idciudad = cups.idciudad
inner join provincia pv with (nolock) on ciu.idprovincia=pv.idprovincia
left join ContratoPot cp with (nolock) on cp.CodigoContrato = c.CodigoContrato
inner join FacturaVentaCabecera fvc with (nolock) on fvc.codigocontrato = c.codigocontrato
inner join FacturasVentaConsulta fvcc on fvc.IdFacturaVentaCabecera = fvcc.IdFacturaVentaCabecera
left join FacturaOrigen fo with(nolock) on fvc.IdFacturaOrigen = fo.IdFacturaVentaCabecera
left join FacturaVentaTotal fvt with (nolock) on fvc.idfacturaventacabecera = fvt.idfacturaventacabecera and fvt.PorcentajeImpuesto not in (3.00,7.00)
left join FacturaVentaTotal fvt3 with (nolock) on fvc.idfacturaventacabecera = fvt3.idfacturaventacabecera and fvt3.PorcentajeImpuesto=3.00
left join FacturaVentaTotal fvt7 with (nolock) on fvc.idfacturaventacabecera = fvt7.idfacturaventacabecera and fvt7.PorcentajeImpuesto=7.00
inner join CarteraCobro cc with (nolock) on fvc.idfacturaventacabecera = cc.idfacturaventacabecera
inner join Tarifa t with (nolock) on fvc.IdTarifaPeajeXML = t.IdTarifa
left join TarifaGrupo tg on tg.IdTarifaGrupo = fvc.IdTarifaGrupoXML
inner join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
left join LineasFactura fvlIE with (nolock) on fvc.idfacturaventacabecera = fvlIE.idfacturaventacabecera and fvlIE.Facturaconcepto in (60001,60002)
left join (select IdFacturaVentaCabecera, sum(ImporteBase) importebase 
     from LineasFactura with (nolock) 
     where FacturaConcepto = 50002
     group by IdFacturaVentaCabecera
    ) as fvlCON on fvc.idfacturaventacabecera = fvlCON.idfacturaventacabecera
left join (
	select IdFacturaVentaCabecera, sum(ImporteBase) ImporteBase from LineasFactura 
	where  LineasFactura.Facturaconcepto=20006
	group by IdFacturaVentaCabecera
	)as fvlmax on fvc.idfacturaventacabecera = fvlMAX.idfacturaventacabecera
inner join LecturasV1 l  with (nolock) on fvc.idfacturaventacabecera = l.idfacturaventacabecera
left join LecturaLineasV1  ll with (nolock) on l.IdLectura = ll.IdLectura
left join LecturaOrigen Lorigenes with(nolock) on l.IdLecturaOrigen = Lorigenes.IdLecturaOrigen
left join PeajesPrecios peajesP with (nolock) on l.IdTarifaPeaje = peajesP.IdTarifaPeaje


left join ConsumosReactiva cr with (nolock) ON cr.id = fvc.IdFacturaVentaCabecera	
left join ImportesPotencia WITH (NOLOCK) on ImportesPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesEnergia WITH (NOLOCK) on ImportesEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesReactiva WITH (NOLOCK) on ImportesReactiva.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesExcesos WITH (NOLOCK) on ImportesExcesos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join DescuentosPotencia WITH (NOLOCK) on DescuentosPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join DescuentosEnergia WITH (NOLOCK) on DescuentosEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join ImportesAlquileres WITH (NOLOCK) on ImportesAlquileres.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join ImportesProductos WITH (NOLOCK) on ImportesProductos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
--- BLOQUE 1: Concepto 30004 (Precios, Consumos e Importes de Energía)
LEFT JOIN (
    SELECT 
        IdFacturaVentaCabecera,
        -- Importes Base
        SUM(CASE WHEN CodigoPeriodo = 1 THEN ImporteBase ELSE 0 END) AS ImporteBaseV_P1,
        SUM(CASE WHEN CodigoPeriodo = 2 THEN ImporteBase ELSE 0 END) AS ImporteBaseV_P2,
        SUM(CASE WHEN CodigoPeriodo = 3 THEN ImporteBase ELSE 0 END) AS ImporteBaseV_P3,
        SUM(CASE WHEN CodigoPeriodo = 4 THEN ImporteBase ELSE 0 END) AS ImporteBaseV_P4,
        SUM(CASE WHEN CodigoPeriodo = 5 THEN ImporteBase ELSE 0 END) AS ImporteBaseV_P5,
        SUM(CASE WHEN CodigoPeriodo = 6 THEN ImporteBase ELSE 0 END) AS ImporteBaseV_P6,
        -- Precios Variables (PrecioMedio)
        SUM(CASE WHEN CodigoPeriodo = 1 THEN PrecioMedio ELSE 0 END) AS PrecioVariableP1,
        SUM(CASE WHEN CodigoPeriodo = 2 THEN PrecioMedio ELSE 0 END) AS PrecioVariableP2,
        SUM(CASE WHEN CodigoPeriodo = 3 THEN PrecioMedio ELSE 0 END) AS PrecioVariableP3,
        SUM(CASE WHEN CodigoPeriodo = 4 THEN PrecioMedio ELSE 0 END) AS PrecioVariableP4,
        SUM(CASE WHEN CodigoPeriodo = 5 THEN PrecioMedio ELSE 0 END) AS PrecioVariableP5,
        SUM(CASE WHEN CodigoPeriodo = 6 THEN PrecioMedio ELSE 0 END) AS PrecioVariableP6,
        -- Consumos (TotConsumo)
        SUM(CASE WHEN CodigoPeriodo = 1 THEN TotConsumo ELSE 0 END) AS ConsumoVariableP1,
        SUM(CASE WHEN CodigoPeriodo = 2 THEN TotConsumo ELSE 0 END) AS ConsumoVariableP2,
        SUM(CASE WHEN CodigoPeriodo = 3 THEN TotConsumo ELSE 0 END) AS ConsumoVariableP3,
        SUM(CASE WHEN CodigoPeriodo = 4 THEN TotConsumo ELSE 0 END) AS ConsumoVariableP4,
        SUM(CASE WHEN CodigoPeriodo = 5 THEN TotConsumo ELSE 0 END) AS ConsumoVariableP5,
        SUM(CASE WHEN CodigoPeriodo = 6 THEN TotConsumo ELSE 0 END) AS ConsumoVariableP6
    FROM (
        SELECT 
            IdFacturaVentaCabecera,
            ImporteBase,
            InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') AS CodigoPeriodo,
            ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS PrecioMedio,
            ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/TotConsumo)[1]', 'decimal(18,6)'), 0) AS TotConsumo
        FROM LineasFactura WITH (NOLOCK)
        WHERE FacturaConcepto = 30004
    ) AS Sub30004
    WHERE CodigoPeriodo BETWEEN 1 AND 6
    GROUP BY IdFacturaVentaCabecera
) AS fvl_Energia ON fvc.idfacturaventacabecera = fvl_Energia.idfacturaventacabecera

--- BLOQUE 2: Concepto 130002 (Cargos de Potencia)
LEFT JOIN (
    SELECT 
        IdFacturaVentaCabecera,
        SUM(CASE WHEN CodigoPeriodo = 1 THEN PrecioCargo ELSE 0 END) AS PrecioCargoPotenciaP1,
        SUM(CASE WHEN CodigoPeriodo = 2 THEN PrecioCargo ELSE 0 END) AS PrecioCargoPotenciaP2,
        SUM(CASE WHEN CodigoPeriodo = 3 THEN PrecioCargo ELSE 0 END) AS PrecioCargoPotenciaP3,
        SUM(CASE WHEN CodigoPeriodo = 4 THEN PrecioCargo ELSE 0 END) AS PrecioCargoPotenciaP4,
        SUM(CASE WHEN CodigoPeriodo = 5 THEN PrecioCargo ELSE 0 END) AS PrecioCargoPotenciaP5,
        SUM(CASE WHEN CodigoPeriodo = 6 THEN PrecioCargo ELSE 0 END) AS PrecioCargoPotenciaP6
    FROM (
        SELECT 
            IdFacturaVentaCabecera,
            InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') AS CodigoPeriodo,
            ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) AS PrecioCargo
        FROM LineasFactura WITH (NOLOCK)
        WHERE FacturaConcepto = 130002
    ) AS Sub130002
    WHERE CodigoPeriodo BETWEEN 1 AND 6
    GROUP BY IdFacturaVentaCabecera
) AS fvlp_Potencia ON fvc.idfacturaventacabecera = fvlp_Potencia.idfacturaventacabecera

--- BLOQUE 3: Ajuste CAP Gas (Simple join)
LEFT JOIN LineasFactura fvlCAP WITH (NOLOCK) 
    ON fvc.idfacturaventacabecera = fvlCAP.idfacturaventacabecera 
    AND fvlCAP.Facturaconcepto = 30004 
    AND fvlCAP.IsAjusteCAPGas = 1
	
	LEFT JOIN (
    SELECT 
        IdFacturaVentaCabecera,
        -- Importes base por periodo
        SUM(CASE WHEN CodigoPeriodo = 1 THEN ImporteBase ELSE 0 END) AS ImporteBaseP1,
        SUM(CASE WHEN CodigoPeriodo = 2 THEN ImporteBase ELSE 0 END) AS ImporteBaseP2,
        SUM(CASE WHEN CodigoPeriodo = 3 THEN ImporteBase ELSE 0 END) AS ImporteBaseP3,
        SUM(CASE WHEN CodigoPeriodo = 4 THEN ImporteBase ELSE 0 END) AS ImporteBaseP4,
        SUM(CASE WHEN CodigoPeriodo = 5 THEN ImporteBase ELSE 0 END) AS ImporteBaseP5,
        SUM(CASE WHEN CodigoPeriodo = 6 THEN ImporteBase ELSE 0 END) AS ImporteBaseP6,
        -- Precios Cargo por periodo
        SUM(CASE WHEN CodigoPeriodo = 1 THEN PrecioCargo ELSE 0 END) AS PrecioCargoEnergiaP1,
        SUM(CASE WHEN CodigoPeriodo = 2 THEN PrecioCargo ELSE 0 END) AS PrecioCargoEnergiaP2,
        SUM(CASE WHEN CodigoPeriodo = 3 THEN PrecioCargo ELSE 0 END) AS PrecioCargoEnergiaP3,
        SUM(CASE WHEN CodigoPeriodo = 4 THEN PrecioCargo ELSE 0 END) AS PrecioCargoEnergiaP4,
        SUM(CASE WHEN CodigoPeriodo = 5 THEN PrecioCargo ELSE 0 END) AS PrecioCargoEnergiaP5,
        SUM(CASE WHEN CodigoPeriodo = 6 THEN PrecioCargo ELSE 0 END) AS PrecioCargoEnergiaP6
    FROM (
        SELECT 
            IdFacturaVentaCabecera,
            ImporteBase,
            -- Extraemos los valores del XML una sola vez
            InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') AS CodigoPeriodo,
            ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0) AS PrecioCargo
        FROM LineasFactura WITH (NOLOCK)
        WHERE FacturaConcepto = 130004
    ) AS T
    WHERE CodigoPeriodo BETWEEN 1 AND 6
    GROUP BY IdFacturaVentaCabecera
) AS fvle ON fvc.idfacturaventacabecera = fvle.idfacturaventacabecera
--Autoconsumo
LEFT JOIN (
    SELECT 
        IdFacturaVentaCabecera,
        -- Importes base por periodo
        SUM(CASE WHEN CodigoPeriodo = 20001 THEN ImporteBase ELSE 0 END) AS ImporteBaseAutoconsumoP1,
        SUM(CASE WHEN CodigoPeriodo = 20002 THEN ImporteBase ELSE 0 END) AS ImporteBaseAutoconsumoP2,
        SUM(CASE WHEN CodigoPeriodo = 20003 THEN ImporteBase ELSE 0 END) AS ImporteBaseAutoconsumoP3,
        SUM(CASE WHEN CodigoPeriodo = 20004 THEN ImporteBase ELSE 0 END) AS ImporteBaseAutoconsumoP4,
        SUM(CASE WHEN CodigoPeriodo = 20005 THEN ImporteBase ELSE 0 END) AS ImporteBaseAutoconsumoP5,
        SUM(CASE WHEN CodigoPeriodo = 20006 THEN ImporteBase ELSE 0 END) AS ImporteBaseAutoconsumoP6,
		 SUM( Importebase) AS ImporteAutoconsumoUnLinea,
        -- Precios Cargo por periodo
        SUM(CASE WHEN CodigoPeriodo = 20001 THEN PrecioAutoconsumo ELSE 0 END) AS PrecioAutoConsumoP1,
        SUM(CASE WHEN CodigoPeriodo = 20002 THEN PrecioAutoconsumo ELSE 0 END) AS PrecioAutoConsumoP2,
        SUM(CASE WHEN CodigoPeriodo = 20003 THEN PrecioAutoconsumo ELSE 0 END) AS PrecioAutoConsumoP3,
        SUM(CASE WHEN CodigoPeriodo = 20004 THEN PrecioAutoconsumo ELSE 0 END) AS PrecioAutoConsumoP4,
        SUM(CASE WHEN CodigoPeriodo = 20005 THEN PrecioAutoconsumo ELSE 0 END) AS PrecioAutoConsumoP5,
        SUM(CASE WHEN CodigoPeriodo = 20006 THEN PrecioAutoconsumo ELSE 0 END) AS PrecioAutoConsumoP6,
		 SUM( PrecioAutoconsumo) AS PrecioAutoconsumoUnLinea,
		-- Consumo
		 SUM(CASE WHEN CodigoPeriodo = 20001 THEN ConsumoAutoconsumo ELSE 0 END) AS ConsumoAutoconsumoP1,
		 SUM(CASE WHEN CodigoPeriodo = 20002 THEN ConsumoAutoconsumo ELSE 0 END) AS ConsumoAutoconsumoP2,
		 SUM(CASE WHEN CodigoPeriodo = 20003 THEN ConsumoAutoconsumo ELSE 0 END) AS ConsumoAutoconsumoP3,
		 SUM(CASE WHEN CodigoPeriodo = 20004 THEN ConsumoAutoconsumo ELSE 0 END) AS ConsumoAutoconsumoP4,
		 SUM(CASE WHEN CodigoPeriodo = 20005 THEN ConsumoAutoconsumo ELSE 0 END) AS ConsumoAutoconsumoP5,
		 SUM(CASE WHEN CodigoPeriodo = 20006 THEN ConsumoAutoconsumo ELSE 0 END) AS ConsumoAutoconsumoP6,
		  SUM( ConsumoAutoconsumo) AS ConsumoAutoconsumoUnLinea
    FROM (
        SELECT 
            IdFacturaVentaCabecera,
            ImporteBase,
            -- Extraemos los valores del XML una sola vez
            InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') AS CodigoPeriodo,
            ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/Precios/FacturaConceptoEnergiaPreciosDTO/Precio)[1]', 'decimal(18,6)'), 0) AS PrecioAutoconsumo,
			ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/TotConsumo)[1]', 'decimal(18,2)'), 0) AS ConsumoAutoconsumo
        FROM FacturaVentaLinea WITH (NOLOCK) 
        WHERE FacturaConcepto = 30008 
    ) AS T
    --WHERE CodigoPeriodo BETWEEN 20001 AND 20006
	--where 
    GROUP BY IdFacturaVentaCabecera
) AS fvlAuto ON fvc.idfacturaventacabecera = fvlAuto.idfacturaventacabecera	

)

,PreconsultaFinal as (select distinct fvc.*
,case ProductosContrato1.AplicarprecioConsumo when 1 then replace(ProductosContrato1.importe,'.',',') else replace(isnull(ImportesProductosDesglosado1.Importe,0),'.',',') end  as 'CO'
,case ProductosContrato4.AplicarprecioConsumo when 1 then replace(ProductosContrato4.importe,'.',',') else replace(isnull(ImportesProductosDesglosado4.Importe,0),'.',',') end  as 'CO interno'
,case ProductosContrato2.AplicarprecioConsumo when 1 then replace(ProductosContrato2.importe,'.',',') else replace(isnull(ImportesProductosDesglosado2.Importe,0),'.',',') end  as 'Energía Verde'
,case ProductosContrato3.AplicarprecioConsumo when 1 then replace(ProductosContrato3.importe,'.',',') else replace(isnull(ImportesProductosDesglosado3.Importe,0),'.',',') end  as 'Impresión en Papel'
,case ProductosContrato5.AplicarprecioConsumo when 1 then replace(ProductosContrato5.importe,'.',',') else replace(isnull(ImportesProductosDesglosado5.Importe,0),'.',',') end  as 'Servicio RAD + Diferencial de SAS'
,case ProductosContrato6.AplicarprecioConsumo when 1 then replace(ProductosContrato6.importe,'.',',') else replace(isnull(ImportesProductosDesglosado6.Importe,0),'.',',') end  as 'Gastos - Devolucion recibo'
,case ProductosContrato7.AplicarprecioConsumo when 1 then replace(ProductosContrato7.importe,'.',',') else replace(isnull(ImportesProductosDesglosado7.Importe,0),'.',',') end  as 'Actualización de FNEE Orden TED/268/2024 (0,000477€/kWh x Consumo)'
,case ProductosContrato8.AplicarprecioConsumo when 1 then replace(ProductosContrato8.importe,'.',',') else replace(isnull(ImportesProductosDesglosado8.Importe,0),'.',',') end  as 'Actualización de FNEE Orden TED/197/2025 (0,000454€/kWh x Consumo)'
,case ProductosContrato10.AplicarprecioConsumo when 1 then replace(ProductosContrato10.importe,'.',',') else replace(isnull(ImportesProductosDesglosado10.Importe,0),'.',',') end  as 'Coste equivalente al FNEE del año 2025 (1,4293 x 1,015)'
,case ProductosContrato11.AplicarprecioConsumo when 1 then replace(ProductosContrato11.importe,'.',',') else replace(isnull(ImportesProductosDesglosado11.Importe,0),'.',',') end  as 'Actualización de FNEE Orden TED/133/2026 (0,001247€/kWh x Consumo)'
,case ProductosContrato12.AplicarprecioConsumo when 1 then replace(ProductosContrato12.importe,'.',',') else replace(isnull(ImportesProductosDesglosado12.Importe,0),'.',',') end  as 'Coste equivalente al FNEE del año 2026 (2,6577 x 1,015)'

,case ProductosContrato9.AplicarprecioConsumo when 1 then replace(ProductosContrato9.importe,'.',',') else replace(isnull(ImportesProductosDesglosado9.Importe,0),'.',',') end  as 'Otros Productos'
from preconsulta fvc
left join facturaventalinea fvlClickAjuste with (nolock) on fvc.idf = fvlClickAjuste.idfacturaventacabecera and fvlClickAjuste.Facturaconcepto=30006 and fvlClickAjuste.isajustecapgas=1
left join ImportesProductosDesglosado  as ImportesProductosDesglosado1 with (nolock) on ImportesProductosDesglosado1.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado1.idproducto in (4)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado2 with (nolock) on ImportesProductosDesglosado2.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado2.idproducto in (5,27,28)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado3 with (nolock) on ImportesProductosDesglosado3.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado3.idproducto in (18)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado4 with (nolock) on ImportesProductosDesglosado4.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado4.idproducto in (90)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado5 with (nolock) on ImportesProductosDesglosado5.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado5.idproducto in (113)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado6 with (nolock) on ImportesProductosDesglosado6.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado6.idproducto in (122)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado7 with (nolock) on ImportesProductosDesglosado7.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado7.idproducto in (158)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado8 with (nolock) on ImportesProductosDesglosado8.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado8.idproducto in (202)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado9 with (nolock) on ImportesProductosDesglosado9.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado9.idproducto in (1000)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado10 with (nolock) on ImportesProductosDesglosado10.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado10.idproducto in (207)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado11 with (nolock) on ImportesProductosDesglosado11.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado11.idproducto in (251)
left join ImportesProductosDesglosado  as ImportesProductosDesglosado12 with (nolock) on ImportesProductosDesglosado12.IdFacturaVentaCabecera = fvc.idf and ImportesProductosDesglosado12.idproducto in (248)

left join ProductosContrato as ProductosContrato1 with (nolock) on ProductosContrato1.IdContrato = fvc.idcontrato and ProductosContrato1.idproducto in (4)
left join (
select IdContrato, sum(importe)importe, AplicarPrecioConsumo from ProductosContrato
where idproducto in (5,27,28) and isnull(importe,0) <>0
group by IdContrato,AplicarPrecioConsumo 
) ProductosContrato2 on ProductosContrato2.IdContrato = fvc.idcontrato
--left join ProductosContrato as ProductosContrato2 with (nolock) on ProductosContrato2.IdContrato = fvc.idcontrato and ProductosContrato2.idproducto in (5,27,28)
left join ProductosContrato as ProductosContrato3 with (nolock) on ProductosContrato3.IdContrato = fvc.idcontrato and ProductosContrato3.idproducto in (18)
left join ProductosContrato as ProductosContrato4 with (nolock) on ProductosContrato4.IdContrato = fvc.idcontrato and ProductosContrato4.idproducto in (90)
left join ProductosContrato as ProductosContrato5 with (nolock) on ProductosContrato5.IdContrato = fvc.idcontrato and ProductosContrato5.idproducto in (113)
left join ProductosContrato as ProductosContrato6 with (nolock) on ProductosContrato6.IdContrato = fvc.idcontrato and ProductosContrato6.idproducto in (122)
left join ProductosContrato as ProductosContrato7 with (nolock) on ProductosContrato7.IdContrato = fvc.idcontrato and ProductosContrato7.idproducto in (158)
left join ProductosContrato as ProductosContrato8 with (nolock) on ProductosContrato8.IdContrato = fvc.idcontrato and ProductosContrato8.idproducto in (202)
left join ProductosContrato as ProductosContrato9 with (nolock) on ProductosContrato9.IdContrato = fvc.idcontrato and ProductosContrato9.idproducto in (1000)
left join ProductosContrato as ProductosContrato10 with (nolock) on ProductosContrato10.IdContrato = fvc.idcontrato and ProductosContrato10.idproducto in (207)
left join ProductosContrato as ProductosContrato11 with (nolock) on ProductosContrato11.IdContrato = fvc.idcontrato and ProductosContrato11.idproducto in (251)
left join ProductosContrato as ProductosContrato12 with (nolock) on ProductosContrato12.IdContrato = fvc.idcontrato and ProductosContrato12.idproducto in (248)
)

select distinct fvc.*
,ImporteClickDesglosado1.descripcion as descripcionclick1
,ImporteClickDesglosado1.importebase as ImporteClick1
,ImporteClickDesglosado2.descripcion as descripcionclick2
,ImporteClickDesglosado2.importebase as ImporteClick2
,ImporteClickDesglosado3.descripcion as descripcionclick3
,ImporteClickDesglosado3.importebase as ImporteClick3
,ImporteClickDesglosado4.descripcion as descripcionclick4
,ImporteClickDesglosado4.importebase as ImporteClick4
,ImporteClickDesglosado5.descripcion as descripcionclick5
,ImporteClickDesglosado5.importebase as ImporteClick5
,ImporteClickDesglosado6.descripcion as descripcionclick6
,ImporteClickDesglosado6.importebase as ImporteClick6
,ImporteClickDesglosado7.descripcion as descripcionclick7
,ImporteClickDesglosado7.importebase as ImporteClick7
,ImporteClickDesglosado8.descripcion as descripcionclick8
,ImporteClickDesglosado8.importebase as ImporteClick8
,ImporteClickDesglosado9.descripcion as descripcionclick9
,ImporteClickDesglosado9.importebase as ImporteClick9
,ImporteClickDesglosado10.descripcion as descripcionclick10
,ImporteClickDesglosado10.importebase as ImporteClick10
,ImporteClickDesglosado11.descripcion as descripcionclick11
,ImporteClickDesglosado11.importebase as ImporteClick11
,ImporteClickDesglosado12.descripcion as descripcionclick12
,ImporteClickDesglosado12.importebase as ImporteClick12
,ImporteClickDesglosado13.descripcion as descripcionclick13
,ImporteClickDesglosado13.importebase as ImporteClick13
,ImporteClickDesglosado14.descripcion as descripcionclick14
,ImporteClickDesglosado14.importebase as ImporteClick14
,ImporteClickDesglosado15.descripcion as descripcionclick15
,ImporteClickDesglosado15.importebase as ImporteClick15
,ImporteClickDesglosado16.descripcion as descripcionclick16
,ImporteClickDesglosado16.importebase as ImporteClick16
,ImporteClickDesglosado17.descripcion as descripcionclick17
,ImporteClickDesglosado17.importebase as ImporteClick17
,ImporteClickDesglosado18.descripcion as descripcionclick18
,ImporteClickDesglosado18.importebase as ImporteClick18
,ImporteClickDesglosado19.descripcion as descripcionclick19
,ImporteClickDesglosado19.importebase as ImporteClick19
,ImporteClickDesglosado20.descripcion as descripcionclick20
,ImporteClickDesglosado20.importebase as ImporteClick20
,ImporteClickDesglosado21.descripcion as descripcionclick21
,ImporteClickDesglosado21.importebase as ImporteClick21
,ImporteClickDesglosado22.descripcion as descripcionclick22
,ImporteClickDesglosado22.importebase as ImporteClick22
,ImporteClickDesglosado23.descripcion as descripcionclick23
,ImporteClickDesglosado23.importebase as ImporteClick23
,ImporteClickDesglosado24.descripcion as descripcionclick24
,ImporteClickDesglosado24.importebase as ImporteClick24
,ImporteClickDesglosado25.descripcion as descripcionclick25
,ImporteClickDesglosado25.importebase as ImporteClick25
,ImporteClickDesglosado26.descripcion as descripcionclick26
,ImporteClickDesglosado26.importebase as ImporteClick26
,ImporteClickDesglosado27.descripcion as descripcionclick27
,ImporteClickDesglosado27.importebase as ImporteClick27
,ImporteClickDesglosado28.descripcion as descripcionclick28
,ImporteClickDesglosado28.importebase as ImporteClick28
,ImporteClickDesglosado29.descripcion as descripcionclick29
,ImporteClickDesglosado29.importebase as ImporteClick29
,ImporteClickDesglosado30.descripcion as descripcionclick30
,ImporteClickDesglosado30.importebase as ImporteClick30

, fvlClick.ImporteClickTotal
from PreconsultaFinal fvc
left join ImporteClick fvlClick with (nolock) on fvc.idf = fvlClick.idfacturaventacabecera
left join ImporteClickDesglosado as ImporteClickDesglosado1 with(nolock) on ImporteClickDesglosado1.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado1.LineaNumero=1
left join ImporteClickDesglosado as ImporteClickDesglosado2 with(nolock) on ImporteClickDesglosado2.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado2.LineaNumero=2
left join ImporteClickDesglosado as ImporteClickDesglosado3 with(nolock) on ImporteClickDesglosado3.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado3.LineaNumero=3
left join ImporteClickDesglosado as ImporteClickDesglosado4 with(nolock) on ImporteClickDesglosado4.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado4.LineaNumero=4
left join ImporteClickDesglosado as ImporteClickDesglosado5 with(nolock) on ImporteClickDesglosado5.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado5.LineaNumero=5
left join ImporteClickDesglosado as ImporteClickDesglosado6 with(nolock) on ImporteClickDesglosado6.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado6.LineaNumero=6
left join ImporteClickDesglosado as ImporteClickDesglosado7 with(nolock) on ImporteClickDesglosado7.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado7.LineaNumero=7
left join ImporteClickDesglosado as ImporteClickDesglosado8 with(nolock) on ImporteClickDesglosado8.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado8.LineaNumero=8
left join ImporteClickDesglosado as ImporteClickDesglosado9 with(nolock) on ImporteClickDesglosado9.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado9.LineaNumero=9
left join ImporteClickDesglosado as ImporteClickDesglosado10 with(nolock) on ImporteClickDesglosado10.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado10.LineaNumero=10
left join ImporteClickDesglosado as ImporteClickDesglosado11 with(nolock) on ImporteClickDesglosado11.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado11.LineaNumero=11
left join ImporteClickDesglosado as ImporteClickDesglosado12 with(nolock) on ImporteClickDesglosado12.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado12.LineaNumero=12
left join ImporteClickDesglosado as ImporteClickDesglosado13 with(nolock) on ImporteClickDesglosado13.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado13.LineaNumero=13
left join ImporteClickDesglosado as ImporteClickDesglosado14 with(nolock) on ImporteClickDesglosado14.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado14.LineaNumero=14
left join ImporteClickDesglosado as ImporteClickDesglosado15 with(nolock) on ImporteClickDesglosado15.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado15.LineaNumero=15
left join ImporteClickDesglosado as ImporteClickDesglosado16 with(nolock) on ImporteClickDesglosado16.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado16.LineaNumero=16
left join ImporteClickDesglosado as ImporteClickDesglosado17 with(nolock) on ImporteClickDesglosado17.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado17.LineaNumero=17
left join ImporteClickDesglosado as ImporteClickDesglosado18 with(nolock) on ImporteClickDesglosado18.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado18.LineaNumero=18
left join ImporteClickDesglosado as ImporteClickDesglosado19 with(nolock) on ImporteClickDesglosado19.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado19.LineaNumero=19
left join ImporteClickDesglosado as ImporteClickDesglosado20 with(nolock) on ImporteClickDesglosado20.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado20.LineaNumero=20
left join ImporteClickDesglosado as ImporteClickDesglosado21 with(nolock) on ImporteClickDesglosado21.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado21.LineaNumero=21
left join ImporteClickDesglosado as ImporteClickDesglosado22 with(nolock) on ImporteClickDesglosado22.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado22.LineaNumero=22
left join ImporteClickDesglosado as ImporteClickDesglosado23 with(nolock) on ImporteClickDesglosado23.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado23.LineaNumero=23
left join ImporteClickDesglosado as ImporteClickDesglosado24 with(nolock) on ImporteClickDesglosado24.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado24.LineaNumero=24
left join ImporteClickDesglosado as ImporteClickDesglosado25 with(nolock) on ImporteClickDesglosado25.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado25.LineaNumero=25
left join ImporteClickDesglosado as ImporteClickDesglosado26 with(nolock) on ImporteClickDesglosado26.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado26.LineaNumero=26
left join ImporteClickDesglosado as ImporteClickDesglosado27 with(nolock) on ImporteClickDesglosado27.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado27.LineaNumero=27
left join ImporteClickDesglosado as ImporteClickDesglosado28 with(nolock) on ImporteClickDesglosado28.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado28.LineaNumero=28
left join ImporteClickDesglosado as ImporteClickDesglosado29 with(nolock) on ImporteClickDesglosado29.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado29.LineaNumero=29
left join ImporteClickDesglosado as ImporteClickDesglosado30 with(nolock) on ImporteClickDesglosado30.idfacturaventacabecera = fvc.idf and ImporteClickDesglosado30.LineaNumero=30