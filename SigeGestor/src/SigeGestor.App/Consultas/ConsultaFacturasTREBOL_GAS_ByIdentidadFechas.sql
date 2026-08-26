--Erick _Gestor Sige_ TREBOL GAS By Identidad V2
;WITH 

FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,fvc.Codigocontrato from FacturaVentaCabecera fvc with(nolock) 
inner join cliente cl on fvc.idcliente = cl.idcliente
    WHERE 
	fvc.Entorno='E2' and
	fvc.SerieFactura IS NOT NULL AND
	 fvc.FechaFactura >= 'DesdeFechaReplace' and fvc.FechaFactura <= 'HastaFechaReplace' and Identidad ='identidadReplace'

),

LineasFactura as (
select IdFacturaVentaCabecera,IdFacturaVentaLinea,Entorno,FacturaConcepto,InfoLineaXML,ImporteBase,IsAjusteCAPGas,Descripcion
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
),
LineasFacturaClick as (
select IdFacturaVentaCabecera,Replace(SUM(ImporteBase),'.',',') as importebase--,STRING_AGG(FacturaVentaLinea.descripcion, ' | ') AS descripcion
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) and FacturaConcepto=90032
group by IdFacturaVentaCabecera
)
,ImporteClickDesglosado AS (
    Select lf.idfacturaventacabecera,lf.importebase,lf.descripcion,
        ROW_NUMBER() OVER (PARTITION BY lf.idfacturaventacabecera ORDER BY lf.idfacturaventacabecera) AS LineaNumero
    from LineasFactura lf WITH (NOLOCK)
    where 
        lf.IdFacturaVentaCabecera IN (select IdFacturaVentaCabecera from FacturasVentaConsulta)
        AND lf.Facturaconcepto = 90032
)
,FVL_SUM_ImporteBase_IH as
(
	SELECT IdFacturaVentaCabecera,FacturaConcepto,-- sum(ImporteBase) As Importe,
	 isnull(infolineaxml.value('(//ConceptoImpuesto//CoeficienteImpuesto)[1]', 'decimal(18,8)'),0) as PrecioDist
	,isnull(infolineaxml.value('(//ConceptoImpuestoHidrocarburoPropio//PrecioNormal)[1]', 'decimal(18,8)'),0) as PrecioNormal
	,isnull(infolineaxml.value('(//ConceptoImpuestoHidrocarburoPropio//PrecioReducido)[1]', 'decimal(18,8)'),0) as PrecioReducido
	,isnull(infolineaxml.value('(//ConceptoImpuestoHidrocarburoPropio//PrecioVehicular)[1]', 'decimal(18,8)'),0) as PrecioVehicular
	FROM  FacturaVentaLinea WITH(NOLOCK)
	where  FacturaConcepto in (90004,90017,90025,90026,90027,90034) and Entorno='E2'
	--group by IdFacturaVentaCabecera,FacturaConcepto
),

Datos as
(
	SELECT IdFacturaVentaCabecera as Id,
	T.N.value('(PCS)[1]','decimal(18,6)') as PCS,
	T.N.value('(FactorConversion)[1]','decimal(18,6)') as FactorConversion,
	T.N.value('(Presion)[1]','decimal(18,6)') as Presion,
	InfoCabeceraXML.value('(//FechaLecturaAnterior)[1]','date') AS FechaLecturaAnterior,
	InfoCabeceraXML.value('(//FechaLecturaActual)[1]','date') AS FechaLecturaActual	
	FROM FacturaVentaCabecera WITH (NOLOCK)
	CROSS APPLY InfoCabeceraXML.nodes('//FacturaVentaInfoConsumoDTO') as T(N)
	WHERE IdFacturaVentaCabecera in (SELECT idfacturaventacabecera FROM FacturasVentaConsulta)
),
AgrupadaDatos as
(
SELECT Id , AVG(PCS) as PCSMedia,  AVG(FactorConversion) as FactorConversionMedia,  AVG(Presion) as PresionMedia, FechaLecturaAnterior, FechaLecturaActual
	FROM Datos
	group by id, FechaLecturaAnterior, FechaLecturaActual
),
--Consumos
Consumos as
(			
		SELECT FacturaVentaLinea.IdFacturaVentaCabecera as Id,
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo,
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/Consumo)[1]', 'decimal(18,3)'), 0) AS VolumenTotal,
		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo,
		ImporteBase	
		FROM   FacturaVentaLinea WITH (NOLOCK)
		where  FacturaVentaLinea.IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta) And FacturaConcepto in (90001,90002,90012,90032) AND (FacturaConcepto <> 90002) and InfoLineaXML.exist('FacturaConceptosDTO/ConceptoConsumoGas') = 1
),
AgrupadaConsumos as
(
	SELECT Id , SUM(Consumo) As Consumo,  SUM(VolumenTotal) as VolumenTotal, Sum(ImporteBase) as  ImporteConsumo, Sum(Precio) as Precio, CodPeriodo
	FROM Consumos
	WHERE CodPeriodo = 1
	group by id, CodPeriodo
),
Potencia as
(
	SELECT FacturaVentaLinea.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoTerminoFijoGas/QAplicado)[1]', 'decimal(18,3)'), 0) AS PotenciaFacturar,
	      ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoTerminoFijoGas/QContratado)[1]', 'decimal(18,3)'), 0) AS PotenciaContratada,	 	  
		  ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoTerminoFijoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
		  FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   FacturaVentaLinea WITH (NOLOCK)
		where  FacturaConcepto like '90%' and InfoLineaXML.exist('FacturaConceptosDTO/ConceptoTerminoFijoGas') = 1
),
AgrupadaPotencias as
(
	SELECT Id , SUM(PotenciaFacturar) As PotenciaFacturar,  SUM(PotenciaContratada) as PotenciaContratada, MAX(Precio) as Precio, CodPeriodo	
	FROM Potencia
	WHERE CodPeriodo = 1
	group by id, CodPeriodo
),
PreseleccionContratos as (
select IdContrato, CodigoContrato, TipoImprimir, IdGrupoImprimir, IdModeloFactura, IdModeloFacturaGestinel, IdTarifa, IdCliente from Contrato 
where Entorno = 'E2' and Contrato.CodigoContrato in (select codigocontrato from FacturasVentaConsulta)
),
PreseleccionFacturas as (
select IdFacturaVentaCabecera, FacturaVentaCabecera.SerieFactura, NumeroFactura, FacturaCategoria, PreseleccionContratos.CodigoContrato, FechaFactura, FacturaVentaCabecera.IdFacturaTipo,facturaventacabecera.fechalecturaactualxml,facturaventacabecera.fechalecturaanteriorxml
,IdTipoImpuesto, PreseleccionContratos.IdTarifa,
IdFacturaRectificativa, IdFacturaAbono, IdFacturaOrigen, cast(isnull(InfoCabeceraXML.value('(//ConsumoActiva)[1]', 'nvarchar(max)'), '0') as decimal(18,2)) as Consumo, IsGestinel, IdContratoDocumento,
PreseleccionContratos.IdContrato, PreseleccionContratos.IdCliente, IdCanal, PreseleccionContratos.IdModeloFactura, PreseleccionContratos.IdModeloFacturaGestinel, FacturaTipo.TextoFacturaTipo
from PreseleccionContratos with(nolock)
inner join FacturaVentaCabecera on PreseleccionContratos.IdContrato = FacturaVentaCabecera.IdContrato 
left join FacturaTipo on FacturaTipo.IdFacturaTipo = FacturaVentaCabecera.IdFacturaTipo
where FacturaVentaCabecera.Entorno = 'E2' and IsFactura = 1
)
,
Lineas as (
select PreseleccionFacturas.IdFacturaVentaCabecera, FacturaConcepto, ImporteBase,InfoLineaXML  from PreseleccionFacturas
inner join LineasFactura on LineasFactura.IdFacturaVentaCabecera = PreseleccionFacturas.IdFacturaVentaCabecera
)
,
AgrupadaLineas as
(
	SELECT IdFacturaVentaCabecera As Id, 	
	CASE WHEN FacturaConcepto = 90002 THEN  sum(ImporteBase) ELSE 0 END as ImporteATRVariable,
	CASE WHEN FacturaConcepto in (90003, 90018, 90019,90070) THEN  sum(ImporteBase) ELSE 0 END as ImporteATRFijo,
	CASE WHEN  FacturaConcepto in (90005,90013, 90014, 90015) THEN sum(ImporteBase) Else 0 End as Alquiler,
	CASE WHEN FacturaConcepto in (90004,90017, 90025,90026, 90027) THEN sum(ImporteBase) Else 0 End as IH,
	CASE WHEN FacturaConcepto in (90012, 90001, 90031)  THEN sum(ImporteBase) Else 0 End as ImportePG,
	CASE WHEN FacturaConcepto in (70017, 100001)  THEN sum(ImporteBase) Else 0 End as Adicionales01,
	CASE WHEN FacturaConcepto in (90006, 120005,120007)  THEN sum(ImporteBase) Else 0 End as DescuentosEnergia, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90007)  THEN sum(ImporteBase) Else 0 End as DescuentosFijo,
	CASE WHEN FacturaConcepto in (90008)  THEN sum(ImporteBase) Else 0 End as Canon,
	CASE WHEN FacturaConcepto in (90009)  THEN sum(ImporteBase) Else 0 End as OtrosATR,
	CASE WHEN FacturaConcepto in (90010)  THEN sum(ImporteBase) Else 0 End as OtrosEnergia,


		CASE WHEN FacturaConcepto in (90032)  THEN sum(ImporteBase) Else 0 End as TerminoVariablePersonalizado,
	CASE WHEN FacturaConcepto in (90048)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalterminovariableconsumoDistribuidora, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90049)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalterminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90050)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalterminofijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90051)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalCaudaldemandadoDistribuidora,
	CASE WHEN FacturaConcepto in (90052)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteterminovariableconsumoDistribuidora,
	CASE WHEN FacturaConcepto in (90053)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteterminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90054)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteterminoFijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90055)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,
	CASE WHEN FacturaConcepto in (90056)  THEN sum(ImporteBase) Else 0 End as OtroscostesderegasificacipnterminofijoclienteDistribuidora, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90057)  THEN sum(ImporteBase) Else 0 End as OtroscostesderegasificacipnterminoFijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90058)  THEN sum(ImporteBase) Else 0 End as CargoterminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90059)  THEN sum(ImporteBase) Else 0 End as CargoterminofijoCapacidadDistribuidora,

	CASE WHEN FacturaConcepto in (90062)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTerminoVariableConsumo,
	CASE WHEN FacturaConcepto in (90035)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTerminofijoCliente,
	CASE WHEN FacturaConcepto in (90036)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTerminofijoCapacidad, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90037)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalCaudalDemandado,
	CASE WHEN FacturaConcepto in (90038)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTerminoVariableConsmuo,
	CASE WHEN FacturaConcepto in (90039)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTerminoFijoCliente,
	CASE WHEN FacturaConcepto in (90040)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTerminoFijoCapacidad,
	CASE WHEN FacturaConcepto in (90041)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteCaudalDemandado,
	CASE WHEN FacturaConcepto in (90042)  THEN sum(ImporteBase) Else 0 End as Otroscostesderegasificacipnterminofijocliente,
	CASE WHEN FacturaConcepto in (90043)  THEN sum(ImporteBase) Else 0 End as Otroscostesderegasificacipnterminofijocapacidad,
	CASE WHEN FacturaConcepto in (90044)  THEN sum(ImporteBase) Else 0 End as CargoTerminofijocliente, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90045)  THEN sum(ImporteBase) Else 0 End as CargoTerminofijocapacidad,
	CASE WHEN FacturaConcepto in (90046)  THEN sum(ImporteBase) Else 0 End as CuotaDelGTS,
	CASE WHEN FacturaConcepto in (90047)  THEN sum(ImporteBase) Else 0 End as TasaCNMC,




	CASE WHEN FacturaConcepto in (90060)  THEN sum(ImporteBase) Else 0 End as CuotaDelGTSDistribuidora,
	CASE WHEN FacturaConcepto in (90061)  THEN sum(ImporteBase) Else 0 End as TasaCNMCDistribuidora ,
	CASE WHEN FacturaConcepto in (90066) THEN sum(ImporteBase) Else 0 end as AlmacenamientoSubterraneo
	from Lineas where Lineas.IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
	group by IdFacturaVentaCabecera, FacturaConcepto
),
SumaAgrupadaLineas as
(
	SELECT
	Id, sum(ImporteATRVariable) as ImporteATRVariable, sum(ImporteATRFijo) as ImporteATRFijo, sum(Alquiler) as Alquiler, sum(IH) as IH, 
	sum(ImportePG) as ImportePG, sum(Adicionales01) as Adicionales01,	sum(DescuentosEnergia) as DescuentosEnergia, 
	sum(DescuentosFijo) as DescuentosFijo, sum(Canon) as Canon, sum(OtrosATR) as OtrosATR, sum(OtrosEnergia) as OtrosEnergia,



	sum(TerminoVariablePersonalizado) as TerminoVariablePersonalizado,

	sum(PeajeSalidaRedLocalterminovariableconsumoDistribuidora) as PeajeSalidaRedLocalterminovariableconsumoDistribuidora,sum(PeajeSalidaRedLocalterminofijoclienteDistribuidora) as PeajeSalidaRedLocalterminofijoclienteDistribuidora,
	sum(PeajeSalidaRedLocalterminofijoCapacidadDistribuidora) as PeajeSalidaRedLocalterminofijoCapacidadDistribuidora,sum(PeajeSalidaRedLocalCaudaldemandadoDistribuidora) as PeajeSalidaRedLocalCaudaldemandadoDistribuidora,
	sum(PeajeSalidaRedTransporteterminovariableconsumoDistribuidora) as PeajeSalidaRedTransporteterminovariableconsumoDistribuidora,sum(PeajeSalidaRedTransporteterminofijoclienteDistribuidora) as PeajeSalidaRedTransporteterminofijoclienteDistribuidora,
	sum(PeajeSalidaRedTransporteterminoFijoCapacidadDistribuidora) as PeajeSalidaRedTransporteterminoFijoCapacidadDistribuidora,sum(PeajeSalidaRedTransporteCaudaldemandadoDistribuidora) as PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,
	sum(OtroscostesderegasificacipnterminofijoclienteDistribuidora) as OtroscostesderegasificacipnterminofijoclienteDistribuidora,sum(OtroscostesderegasificacipnterminoFijoCapacidadDistribuidora) as OtroscostesderegasificacipnterminoFijoCapacidadDistribuidora,
	sum(CargoterminofijoclienteDistribuidora) as CargoterminofijoclienteDistribuidora,sum(CargoterminofijoCapacidadDistribuidora) as CargoterminofijoCapacidadDistribuidora,
	sum(CuotaDelGTSDistribuidora) as CuotaDelGTSDistribuidora,sum(TasaCNMCDistribuidora) as TasaCNMCDistribuidora,sum(AlmacenamientoSubterraneo) as AlmacenamientoSubterraneo,
	sum(PeajeSalidaRedLocalTerminoVariableConsumo) as PeajeSalidaRedLocalTerminoVariableConsumo,

	sum(PeajeSalidaRedLocalTerminofijoCliente) as PeajeSalidaRedLocalTerminofijoCliente,sum(PeajeSalidaRedLocalTerminofijoCapacidad) as PeajeSalidaRedLocalTerminofijoCapacidad,
	sum(PeajeSalidaRedLocalCaudalDemandado) as PeajeSalidaRedLocalCaudalDemandado,sum(PeajeSalidaRedTransporteTerminoVariableConsmuo) as PeajeSalidaRedTransporteTerminoVariableConsmuo,
	sum(PeajeSalidaRedTransporteTerminoFijoCliente) as PeajeSalidaRedTransporteTerminoFijoCliente,sum(PeajeSalidaRedTransporteTerminoFijoCapacidad) as PeajeSalidaRedTransporteTerminoFijoCapacidad,
	sum(PeajeSalidaRedTransporteCaudalDemandado) as PeajeSalidaRedTransporteCaudalDemandado,sum(Otroscostesderegasificacipnterminofijocliente) as Otroscostesderegasificacipnterminofijocliente,
	sum(Otroscostesderegasificacipnterminofijocapacidad) as Otroscostesderegasificacipnterminofijocapacidad,sum(CargoTerminofijocliente) as CargoTerminofijocliente,
	sum(CargoTerminofijocapacidad) as CargoTerminofijocapacidad,sum(CuotaDelGTS) as CuotaDelGTS,
	sum(TasaCNMC) as TasaCNMC


	FROM AgrupadaLineas
	group by id
),
Lecturas as (
Select Max(IdLectura) as IdLectura, IdFacturaVentaCabeceraSectorC, IdFacturaCompracabecera from Lectura
where  Lectura.IdFacturaVentaCabeceraSectorC in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
group by IdFacturaVentaCabeceraSectorC, IdFacturaCompracabecera
)

,SerieNumeroFacturasOrigen as
(
	select CONCAT( origen.seriefactura, ' ',  origen.numerofactura) as SerieNumeroFacturaOrigen, principal.IdFacturaVentaCabecera
	from FacturaVentaCabecera principal
	inner join FacturaVentaCabecera origen 
	on principal.IdFacturaOrigen = origen.IdFacturaVentaCabecera
),
ImportesProductos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where FacturaConcepto in (100001, 100002, 110001, 90009)
group by Lineas.IdFacturaVentaCabecera
)
,
ImportesProductosDesglosado as (
Select IdFacturaVentaCabecera,IdProducto,sum(ImporteBase) AS Importe
from (Select Lineas.IdFacturaVentaCabecera, Lineas.ImporteBase,       
			CASE 
            WHEN InfoLineaXML.value('(//ConceptoProductos//IdProducto)[1]', 'int') not in (30,32,123,133,113,159,206) THEN 1000
            ELSE InfoLineaXML.value('(//ConceptoProductos//IdProducto)[1]', 'int')
        END AS IdProducto
 from Lineas WHERE FacturaConcepto IN (100001, 100002, 110001, 90009)
) as Subconsulta
group by IdFacturaVentaCabecera,IdProducto
)
, ProductosContrato as (
Select pa.IdContrato,pa.IdProducto,pa.Importe,AplicarPrecioConsumo,producto.TextoProducto from ProductoAsignacion pa
inner join producto on pa.IdProducto =producto.IdProducto
where pa.IdContrato in (select IdContrato from PreseleccionContratos)--and pa.AplicarPrecioConsumo=1
)
select distinct  cliente.RazonSocial
,cliente.identidad
,facturaventacabecera.CodigoContrato
, CodigoCUPS
,cll.NombreCalle +' '+ cups.Aclarador as Direccion
,ciu.TextoCiudad as Poblacion
,cups.CodPostal
,pv.TextoProvincia as Provincia
,FacturaVentaCabecera.SerieFactura as Serie,
	FacturaVentaCabecera.NumeroFactura as Numero,
	cast(FacturaventaCabecera.fechafactura as date) fechafactura
	,tp.textotarifapeaje
	,t.textotarifa as Tarifa
	,tg.textotarifagrupo as Grupo
	,replace(isnull(ImportesProductos.Importe, 0),'.',',') as ImporteProductos
	,fvt.PorcentajeImpuesto As PorcentajeIVA
,replace(fvt.ImporteBase,'.',',') As ImporteBase
,replace(fvt.ImporteImpuesto,'.',',') As ImporteIVA
,replace(fvt.ImporteTotal,'.',',') As ImporteTotal
	,replace(Consumos.Consumo,'.',',') as Consumo,
	replace(Consumos.Precio,'.',',') as EnergiaPrecio,
	replace(COnsumos.ImporteConsumo,'.',',') as ImporteConsumo,
	replace(Consumos.VolumenTotal,'.',',') as Volumentotal,
	AgrupadaDatos.FechaLecturaAnterior As FechaLecturaAnterior,
	AgrupadaDatos.FechaLecturaActual As FechaLecturaActual,
	replace(AgrupadaDatos.FactorConversionMedia,'.',',') As Factorconversion,
	replace(AgrupadaDatos.PCSMedia,'.',',') As PCS,
	replace(AgrupadaDatos.PresionMedia,'.',',') As Presion,
	replace(AgrupadaPotencias.PotenciaContratada,'.',',') as TerminoFijoContratado,
	replace(AgrupadaPotencias.PotenciaFacturar,'.',',') as TerminoFijoFactura,
	replace(AgrupadaPotencias.Precio,'.',',') as PrecioTerminoFijo,
	Agente.NombreAgente as Agente,
	replace(SumaAgrupadaLineas.Alquiler,'.',',') As ImporteAlquiler,	
	replace(SumaAgrupadaLineas.IH,'.',',') As ImpHidrocarburos,
	replace(ConsumoVariable,'.',',') AS KwhVariable,
	replace(PrecioVariable,'.',',') AS PrecioVariable,
	replace(SumaAgrupadaLineas.ImportePG,'.',',') As 'Importe T.Variable',	
	replace(SumaAgrupadaLineas.Adicionales01,'.',',') as Productos,
	replace(SumaAgrupadaLineas.DescuentosEnergia,'.',',') as DescuentosEnergia,
	replace(SumaAgrupadaLineas.DescuentosFijo,'.',',') as DescuentosFijo,
	replace(SumaAgrupadaLineas.Canon,'.',',') as Canon,
	replace(SumaAgrupadaLineas.OtrosATR,'.',',') As OtrosATR,
	replace(SumaAgrupadaLineas.OtrosEnergia,'.',',') As OtrosEnergia,
	replace(SumaAgrupadaLineas.TerminoVariablePersonalizado,'.',',') As TerminoVariablePersonalizado,
	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalterminovariableconsumoDistribuidora,'.',',') As PeajeSalidaRedLocalterminovariableconsumoDistribuidora,
	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalterminofijoclienteDistribuidora,'.',',') As PeajeSalidaRedLocalterminofijoclienteDistribuidora,
	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalterminofijoCapacidadDistribuidora,'.',',') As PeajeSalidaRedLocalterminofijoCapacidadDistribuidora,
	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalCaudaldemandadoDistribuidora,'.',',') As PeajeSalidaRedLocalCaudaldemandadoDistribuidora,
	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteterminovariableconsumoDistribuidora,'.',',') As PeajeSalidaRedTransporteterminovariableconsumoDistribuidora,
	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteterminofijoclienteDistribuidora,'.',',') As PeajeSalidaRedTransporteterminofijoclienteDistribuidora,
	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteterminoFijoCapacidadDistribuidora,'.',',') As PeajeSalidaRedTransporteterminoFijoCapacidadDistribuidora,
	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,'.',',') As PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,
	replace(SumaAgrupadaLineas.OtroscostesderegasificacipnterminofijoclienteDistribuidora,'.',',') As OtroscostesderegasificacipnterminofijoclienteDistribuidora,
	replace(SumaAgrupadaLineas.OtroscostesderegasificacipnterminoFijoCapacidadDistribuidora,'.',',') As OtroscostesderegasificacipnterminoFijoCapacidadDistribuidora,
	replace(SumaAgrupadaLineas.CargoterminofijoclienteDistribuidora,'.',',') As CargoterminofijoclienteDistribuidora,
	replace(SumaAgrupadaLineas.CargoterminofijoCapacidadDistribuidora,'.',',') As CargoterminofijoCapacidadDistribuidora,
	replace(SumaAgrupadaLineas.CuotaDelGTSDistribuidora,'.',',') As CuotaDelGTSDistribuidora,
	replace(SumaAgrupadaLineas.TasaCNMCDistribuidora,'.',',') As TasaCNMCDistribuidora,
	replace(SumaAgrupadaLineas.AlmacenamientoSubterraneo,'.',',') As AlmacenamientoSubterraneo,
	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTerminoVariableConsumo,'.',',') As PeajeSalidaRedLocalTerminoVariableConsumo,
	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTerminofijoCliente,'.',',') As PeajeSalidaRedLocalTerminofijoCliente,
	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTerminofijoCapacidad,'.',',') As PeajeSalidaRedLocalTerminofijoCapacidad,
	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalCaudalDemandado,'.',',') As PeajeSalidaRedLocalCaudalDemandado,
	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoVariableConsmuo,'.',',') As PeajeSalidaRedTransporteTerminoVariableConsmuo,
	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoFijoCliente,'.',',') As PeajeSalidaRedTransporteTerminoFijoCliente,
	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoFijoCapacidad,'.',',') As PeajeSalidaRedTransporteTerminoFijoCapacidad,
	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteCaudalDemandado,'.',',') As PeajeSalidaRedTransporteCaudalDemandado,
	replace(SumaAgrupadaLineas.Otroscostesderegasificacipnterminofijocliente,'.',',') As Otroscostesderegasificacipnterminofijocliente,
	replace(SumaAgrupadaLineas.Otroscostesderegasificacipnterminofijocapacidad,'.',',') As Otroscostesderegasificacipnterminofijocapacidad,
	replace(SumaAgrupadaLineas.CargoTerminofijocliente ,'.',',')As CargoTerminofijocliente,
	replace(SumaAgrupadaLineas.CargoTerminofijocapacidad,'.',',') As CargoTerminofijocapacidad,
	replace(SumaAgrupadaLineas.CuotaDelGTS,'.',',') As CuotaDelGTS,
	replace(SumaAgrupadaLineas.TasaCNMC,'.',',') As TasaCNMC,
	replace(tpp.TransporteFijoCapacidad,'.',',') As TransporteFijoCapacidadTarifaAcceso,
	replace(tpp.Transportefijocliente,'.',',') As TransportefijoclienteTarifaAcceso,
	replace(tpp.TransporteVariable,'.',',') As TransporteVariableTarifaAcceso,
	replace(tpp.LocalFijoCapacidad,'.',',') As LocalFijoCapacidadTarifaAcceso,
	replace(tpp.LocalFijoCliente,'.',',') As LocalFijoClienteTarifaAcceso,
	replace(tpp.LocalVariableCapacidad,'.',',') As LocalVariableCapacidadTarifaAcceso,
	replace(tpp.LocalVariableCliente,'.',',') As LocalVariableClienteTarifaAcceso,
	replace(tpp.CargoFijoCapacidad,'.',',') As CargoFijoCapacidadTarifaAcceso,
	replace(tpp.CargoFijoCliente,'.',',') As CargoFijoClienteTarifaAcceso,
	replace(tpp.RegasificacionFijoTelemedido,'.',',') As RegasificacionFijoTelemedidoTarifaAcceso,
	replace(tpp.RegasificacionFijoNoTelemedido,'.',',') As RegasificacionFijoNoTelemedido,
	replace(pc.PrecioAlmacenamientoSubterraneo,'.',',') As PrecioAlmacenamientoSubterraneo,
	PerfilFacturacion.TextoPerfilFacturacion,
	SerieNumeroFacturasOrigen.SerieNumeroFacturaOrigen,
    replace(ISNULL((select ProductoAsignacion.Importe from ProductoAsignacion where ProductoAsignacion.IdContrato = Contrato.IdContrato and ProductoAsignacion.IdProducto = 45), 0),'.',',') as AjustePeajesTFDia,
    replace(ISNULL((select ProductoAsignacion.Importe from ProductoAsignacion where ProductoAsignacion.IdContrato = Contrato.IdContrato and ProductoAsignacion.IdProducto = 46), 0),'.',',') as AjustePeajesTFQ,
    replace(ISNULL((select ProductoAsignacion.Importe from ProductoAsignacion where ProductoAsignacion.IdContrato = Contrato.IdContrato and ProductoAsignacion.IdProducto = 47), 0),'.',',') as AjustePeajesATR,
	Contrato.NumPedidoFacturacion,
	replace(ContratoPotencia.Caudal,'.',',') as Caudal
	,facturacompracabecera.NumeroFactura NumeroFacturaCompra,
		convert (date,facturacompracabecera.FechaRecepcion) as FechaRecepcionB70
		,
		case 
			when contrato.IsTelemedido = 1 
			then 'S'
			else 'N'
		end as Telemedida
		,	   case 
	    when IH.FacturaConcepto = 90025
		then ISNULL(IH.PrecioReducido,0) 
		else 0 end  as PrecioIHReducido,
		case 
	    when IH.FacturaConcepto = 90004
		then PrecioDist 
		Else  0
		end as PrecioIHDistribuidora,
		case 
	    when IH.FacturaConcepto = 90017
		then ISNULL(IH.PrecioNormal,0) 
		Else 0
		end as PrecioIHNormal,
		case 
	    when IH.FacturaConcepto = 90034
		then ISNULL( IH.PrecioVehicular,0)
		Else 0
		end as PrecioIHVehicular
		,case ProductosContrato1.AplicarprecioConsumo when 1 then replace(ProductosContrato1.importe,'.',',') else replace(isnull(ImportesProductosDesglosado1.Importe,0),'.',',') end  as 'CO'
		,case ProductosContrato4.AplicarprecioConsumo when 1 then replace(ProductosContrato4.importe,'.',',') else replace(isnull(ImportesProductosDesglosado4.Importe,0),'.',',') end  as 'CO interno'
        ,case ProductosContrato2.AplicarprecioConsumo when 1 then replace(ProductosContrato2.importe,'.',',') else replace(isnull(ImportesProductosDesglosado2.Importe,0),'.',',') end  as 'Impresipn en Papel'
        ,case ProductosContrato3.AplicarprecioConsumo when 1 then replace(ProductosContrato3.importe,'.',',') else replace(isnull(ImportesProductosDesglosado3.Importe,0),'.',',') end  as 'Gastos - Devolucion recibo'
        ,case ProductosContrato6.AplicarprecioConsumo when 1 then replace(ProductosContrato6.importe,'.',',') else replace(isnull(ImportesProductosDesglosado6.Importe,0),'.',',') end  as 'Actualizacipn de FNEE Orden TED/268/2024 (0,000477€/kWh x Consumo)'
        ,case ProductosContrato7.AplicarprecioConsumo when 1 then replace(ProductosContrato7.importe,'.',',') else replace(isnull(ImportesProductosDesglosado7.Importe,0),'.',',') end  as 'Actualizacipn de FNEE Orden TED/197/2025 (0,000454€/kWh x Consumo)'
		,case ProductosContrato9.AplicarprecioConsumo when 1 then replace(ProductosContrato9.importe,'.',',') else replace(isnull(ImportesProductosDesglosado9.Importe,0),'.',',') end  as 'Otros Productos'
		,ImporteClickDesglosado1.descripcion as descripcionclick1
		,replace(ImporteClickDesglosado1.importebase,'.',',')  as ImporteClick1
		,ImporteClickDesglosado2.descripcion as descripcionclick2
		,replace(ImporteClickDesglosado2.importebase,'.',',')  as ImporteClick2
		,ImporteClickDesglosado3.descripcion as descripcionclick3
		,replace(ImporteClickDesglosado3.importebase,'.',',')  as ImporteClick3
		,ImporteClickDesglosado4.descripcion as descripcionclick4
		,replace(ImporteClickDesglosado4.importebase,'.',',')  as ImporteClick4
		,ImporteClickDesglosado5.descripcion as descripcionclick5
		,replace(ImporteClickDesglosado5.importebase,'.',',')  as ImporteClick5
		,ImporteClickDesglosado6.descripcion as descripcionclick6
		,replace(ImporteClickDesglosado6.importebase,'.',',')  as ImporteClick6
		,ImporteClickDesglosado7.descripcion as descripcionclick7
		,replace(ImporteClickDesglosado7.importebase,'.',',')  as ImporteClick7
		,ImporteClickDesglosado8.descripcion as descripcionclick8
		,replace(ImporteClickDesglosado8.importebase,'.',',')  as ImporteClick8
		,ImporteClickDesglosado9.descripcion as descripcionclick9
		,replace(ImporteClickDesglosado9.importebase,'.',',')  as ImporteClick9
		,ImporteClickDesglosado10.descripcion as descripcionclick10
		,replace(ImporteClickDesglosado10.importebase,'.',',')  as ImporteClick10
		,ImporteClickDesglosado11.descripcion as descripcionclick11
		,replace(ImporteClickDesglosado11.importebase,'.',',')  as ImporteClick11
		,ImporteClickDesglosado12.descripcion as descripcionclick12
		,replace(ImporteClickDesglosado12.importebase,'.',',')  as ImporteClick12
		,ImporteClickDesglosado13.descripcion as descripcionclick13
		,replace(ImporteClickDesglosado13.importebase,'.',',')  as ImporteClick13
		,ImporteClickDesglosado14.descripcion as descripcionclick14
		,replace(ImporteClickDesglosado14.importebase,'.',',')  as ImporteClick14
		,ImporteClickDesglosado15.descripcion as descripcionclick15
		,replace(ImporteClickDesglosado15.importebase,'.',',')  as ImporteClick15
		--,lfc.Descripcion as DescripcionClick
		,lfc.ImporteBase as ImporteTotalClick
	from facturaventacabecera WITH (NOLOCK)
	inner join FacturasVentaConsulta fvcc on facturaventacabecera.IdFacturaVentaCabecera = fvcc.IdFacturaVentaCabecera
		--Potencias
		left join 		
		AgrupadaPotencias as AgrupadaPotencias on FacturaVentaCabecera.IdFacturaVentaCabecera = AgrupadaPotencias.Id		
		--Lectura
		--left join
		--Lectura on Lectura.IdFacturaVentaCabeceraSectorC = FacturaVentaCabecera.IdFacturaVentaCabecera and Lectura.Facturar=1		
		--Contrato
		left join Contrato WITH (NOLOCK) on Contrato.IdContrato = FacturaventaCabecera.IdContrato
		inner join cups on Contrato.IdCups = CUPS.IdCups
		--Agente
		left join Agente WITH (NOLOCK) on Agente.IdAgente = Contrato.IdAgente
		--Datos
		left join AgrupadaDatos  as AgrupadaDatos on FacturaVentaCabecera.IdFacturaVentaCabecera = AgrupadaDatos.Id
		--AgrupadaConsumos
		left join AgrupadaConsumos  as Consumos on FacturaVentaCabecera.IdFacturaVentaCabecera = Consumos.Id
		--Impuesto Hidrocarburos
		left join SumaAgrupadaLineas as SumaAgrupadaLineas on FacturaVentaCabecera.IdFacturaVentaCabecera = SumaAgrupadaLineas.Id
	    left join Lecturas on Lecturas.IdFacturaVentaCabeceraSectorC = FacturaVentaCabecera.IdFacturaVentaCabecera
		left join Lectura Lecturas2 on Lecturas2.IdFacturaVentaCabeceraSectorC = FacturaVentaCabecera.IdFacturaVentaCabecera	
		left join cliente WITH (NOLOCK) on cliente.IdCliente = FacturaVentaCabecera.IdCliente
		left join SerieNumeroFacturasOrigen on FacturaVentaCabecera.IdFacturaVentaCabecera = SerieNumeroFacturasOrigen.IdFacturaVentaCabecera
		--PerfilFacturacion
		left join PerfilFacturacion WITH (NOLOCK) on PerfilFacturacion.IdPerfilFacturacion=InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceralectura/IdPerfilFacturacion)[1]', 'bigint')
		 left join ContratoPotencia on contrato.idcontrato =ContratoPotencia.IdContrato
		 left join FacturaCompraCabecera on FacturaCompraCabecera.IdFacturaCompraCabecera =lecturas.IdFacturaCompraCabecera
		 left join productoasignacion on productoasignacion.idcontrato =contrato.idcontrato and idproducto = 30
         left join callejero cll with (nolock) on cll.IdCallejero = cups.IdCallejero
         left join Ciudad ciu with (nolock) on ciu.idciudad = cups.idciudad
         left join provincia pv with (nolock) on ciu.idprovincia=pv.idprovincia
		 left join FacturaVentaTotal fvt with (nolock) on FacturaVentaCabecera.idfacturaventacabecera = fvt.idfacturaventacabecera
		 left join Tarifa t with (nolock) on FacturaVentaCabecera.IdTarifaPeajeXML = t.IdTarifa
		 left join TarifaPeaje tp with (nolock) on FacturaVentaCabecera.IdTarifaPeajeXML = tp.IdTarifapeaje
		 left join TarifaPeajePrecio tpp with (nolock) on FacturaVentaCabecera.IdTarifaPeajeXML = tpp.IdTarifapeaje and tpp.FechaFinal is null
		 left join PeajeComercializadora pc on pc.IdTarifa = t.IdTarifa and isnull(pc.FechaFin,'31/12/2045')>=getdate()

		 left join TarifaGrupo tg on tg.IdTarifaGrupo = FacturaVentaCabecera.IdTarifaGrupoXML
		 left join ImportesProductos on ImportesProductos.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
		 left join LineasFacturaClick lfc on lfc.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
		 left join FVL_SUM_ImporteBase_IH as IH on IH.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera 
		 left join ImportesProductosDesglosado as ImportesProductosDesglosado1 with (nolock) on ImportesProductosDesglosado1.IdFacturaVentaCabecera = facturaventacabecera.IdFacturaVentaCabecera and ImportesProductosDesglosado1.idproducto in (30)
         left join ImportesProductosDesglosado as ImportesProductosDesglosado2 with (nolock) on ImportesProductosDesglosado2.IdFacturaVentaCabecera = facturaventacabecera.IdFacturaVentaCabecera and ImportesProductosDesglosado2.idproducto in (32)
         left join ImportesProductosDesglosado as ImportesProductosDesglosado3 with (nolock) on ImportesProductosDesglosado3.IdFacturaVentaCabecera = facturaventacabecera.IdFacturaVentaCabecera and ImportesProductosDesglosado3.idproducto in (123)
         left join ImportesProductosDesglosado as ImportesProductosDesglosado4 with (nolock) on ImportesProductosDesglosado4.IdFacturaVentaCabecera = facturaventacabecera.IdFacturaVentaCabecera and ImportesProductosDesglosado4.idproducto in (133)
         left join ImportesProductosDesglosado as ImportesProductosDesglosado5 with (nolock) on ImportesProductosDesglosado5.IdFacturaVentaCabecera = facturaventacabecera.IdFacturaVentaCabecera and ImportesProductosDesglosado5.idproducto in (113)
         left join ImportesProductosDesglosado as ImportesProductosDesglosado6 with (nolock) on ImportesProductosDesglosado6.IdFacturaVentaCabecera = facturaventacabecera.IdFacturaVentaCabecera and ImportesProductosDesglosado6.idproducto in (159)
         left join ImportesProductosDesglosado as ImportesProductosDesglosado7 with (nolock) on ImportesProductosDesglosado7.IdFacturaVentaCabecera = facturaventacabecera.IdFacturaVentaCabecera and ImportesProductosDesglosado7.idproducto in (206)
		 left join ImportesProductosDesglosado as ImportesProductosDesglosado9 with (nolock) on ImportesProductosDesglosado9.IdFacturaVentaCabecera = facturaventacabecera.IdFacturaVentaCabecera and ImportesProductosDesglosado9.idproducto in (1000)
         left join ProductosContrato as ProductosContrato1 with (nolock) on ProductosContrato1.IdContrato = contrato.idcontrato and ProductosContrato1.idproducto in (30)
         left join ProductosContrato as ProductosContrato2 with (nolock) on ProductosContrato2.IdContrato = contrato.idcontrato and ProductosContrato2.idproducto in (32)
         left join ProductosContrato as ProductosContrato3 with (nolock) on ProductosContrato3.IdContrato = contrato.idcontrato and ProductosContrato3.idproducto in (123)
         left join ProductosContrato as ProductosContrato4 with (nolock) on ProductosContrato4.IdContrato = contrato.idcontrato and ProductosContrato4.idproducto in (133)
         left join ProductosContrato as ProductosContrato6 with (nolock) on ProductosContrato6.IdContrato = contrato.idcontrato and ProductosContrato6.idproducto in (159)
         left join ProductosContrato as ProductosContrato7 with (nolock) on ProductosContrato7.IdContrato = contrato.idcontrato and ProductosContrato7.idproducto in (206)
		 left join ProductosContrato as ProductosContrato9 with (nolock) on ProductosContrato9.IdContrato = contrato.idcontrato and ProductosContrato9.idproducto in (1000)
		 left join ImporteClickDesglosado as ImporteClickDesglosado1 with(nolock) on ImporteClickDesglosado1.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado1.LineaNumero=1
		 left join ImporteClickDesglosado as ImporteClickDesglosado2 with(nolock) on ImporteClickDesglosado2.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado2.LineaNumero=2
		 left join ImporteClickDesglosado as ImporteClickDesglosado3 with(nolock) on ImporteClickDesglosado3.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado3.LineaNumero=3
		 left join ImporteClickDesglosado as ImporteClickDesglosado4 with(nolock) on ImporteClickDesglosado4.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado4.LineaNumero=4
		 left join ImporteClickDesglosado as ImporteClickDesglosado5 with(nolock) on ImporteClickDesglosado5.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado5.LineaNumero=5
		 left join ImporteClickDesglosado as ImporteClickDesglosado6 with(nolock) on ImporteClickDesglosado6.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado6.LineaNumero=6
		 left join ImporteClickDesglosado as ImporteClickDesglosado7 with(nolock) on ImporteClickDesglosado7.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado7.LineaNumero=7
		 left join ImporteClickDesglosado as ImporteClickDesglosado8 with(nolock) on ImporteClickDesglosado8.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado8.LineaNumero=8
		 left join ImporteClickDesglosado as ImporteClickDesglosado9 with(nolock) on ImporteClickDesglosado9.idfacturaventacabecera =   facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado9.LineaNumero=9
		 left join ImporteClickDesglosado as ImporteClickDesglosado10 with(nolock) on ImporteClickDesglosado10.idfacturaventacabecera = facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado10.LineaNumero=10
		 left join ImporteClickDesglosado as ImporteClickDesglosado11 with(nolock) on ImporteClickDesglosado11.idfacturaventacabecera = facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado11.LineaNumero=11
		 left join ImporteClickDesglosado as ImporteClickDesglosado12 with(nolock) on ImporteClickDesglosado12.idfacturaventacabecera = facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado12.LineaNumero=12
		 left join ImporteClickDesglosado as ImporteClickDesglosado13 with(nolock) on ImporteClickDesglosado13.idfacturaventacabecera = facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado13.LineaNumero=13
		 left join ImporteClickDesglosado as ImporteClickDesglosado14 with(nolock) on ImporteClickDesglosado14.idfacturaventacabecera = facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado14.LineaNumero=14
		 left join ImporteClickDesglosado as ImporteClickDesglosado15 with(nolock) on ImporteClickDesglosado15.idfacturaventacabecera = facturaventacabecera.idfacturaventacabecera and ImporteClickDesglosado15.LineaNumero=15
		 left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]', 'decimal(18,8)'), 0)) PrecioVariable
			from LineasFactura with (nolock) 
			where FacturaConcepto = 90012
			group by IdFacturaVentaCabecera) as fvlP on facturaventacabecera.idfacturaventacabecera = fvlP.idfacturaventacabecera
		left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/TotConsumo)[1]', 'decimal(18,6)'), 0)) ConsumoVariable
			from LineasFactura with (nolock) 
			where FacturaConcepto = 90012 
			group by IdFacturaVentaCabecera) as fvlC on facturaventacabecera.idfacturaventacabecera = fvlC.idfacturaventacabecera


