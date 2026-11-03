--Gestor SIGE _ CAM Gas (Trebol). Viene de ConsultaFacturas_MIBGAS_MARP_CAM_TREBOL_GAS_V3.sql.
--
--Unico cambio sobre el original: las dos fechas, que estaban escritas a mano
--('01/04/2026' y '30/04/2026'), las pone ahora el formulario. Van en DD/MM/AAAA.
;With 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) where  IdTarifaGrupoXML in 
(select idtarifagrupo from TarifaGrupo where 
textotarifagrupo like '%MIBGAS_MARP CAM%') 
and FechaFactura>='DesdeFechaReplace' and FechaFactura<='hastaFechaReplace' 
and SerieFactura is not null 
),

LineasFactura as (
select IdFacturaVentaCabecera,IdFacturaVentaLinea,Entorno,FacturaConcepto,InfoLineaXML,ImporteBase,Descripcion
from FacturaVentaLinea where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
)
,
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
select PreseleccionFacturas.IdFacturaVentaCabecera, FacturaConcepto, ImporteBase,InfoLineaXML,
Case when Facturaconcepto in (90046,90060) then 'Cuota del GTS'
 when Facturaconcepto in (90047,90061) then 'Tasa CNMC'
Else Descripcion
end as Descripcion  from PreseleccionFacturas
inner join LineasFactura on LineasFactura.IdFacturaVentaCabecera = PreseleccionFacturas.IdFacturaVentaCabecera
)
,
AgrupadaLineas as
(
	SELECT IdFacturaVentaCabecera As Id, 	
	CASE WHEN FacturaConcepto = 90002 THEN  sum(ImporteBase) ELSE 0 END as ImporteATRVariable,
	CASE WHEN FacturaConcepto in (90003, 90018, 90019,90035,90036,90039,90040,90070,90042,90043,90044,90045,90049,90050,90053,90054,90056,90057,90058,90059) THEN  sum(ImporteBase) ELSE 0 END as ImporteFijo,
	CASE WHEN  FacturaConcepto in (90005,90013, 90014, 90015) THEN sum(ImporteBase) Else 0 End as Alquiler,
	CASE WHEN FacturaConcepto in (90004,90017, 90025,90026, 90027) THEN sum(ImporteBase) Else 0 End as IH,
	CASE WHEN FacturaConcepto in (90012, 90001, 90031,90048)  THEN sum(ImporteBase) Else 0 End as ImportePG,
	CASE WHEN FacturaConcepto in (70017, 100001)  THEN sum(ImporteBase) Else 0 End as Adicionales01,
	CASE WHEN FacturaConcepto in (90006, 120005,120007)  THEN sum(ImporteBase) Else 0 End as DescuentosEnergia, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90007)  THEN sum(ImporteBase) Else 0 End as DescuentosFijo,
	CASE WHEN FacturaConcepto in (90008)  THEN sum(ImporteBase) Else 0 End as Canon,
	CASE WHEN FacturaConcepto in (90009)  THEN sum(ImporteBase) Else 0 End as OtrosATR,
	CASE WHEN FacturaConcepto in (90010)  THEN sum(ImporteBase) Else 0 End as OtrosEnergia,
	CASE WHEN FacturaConcepto in (90037,90041,90051,90055)  THEN sum(ImporteBase) Else 0 End as caudaldemandado,

		CASE WHEN FacturaConcepto in (90032)  THEN sum(ImporteBase) Else 0 End as TerminoVariablePersonalizado,
	CASE WHEN FacturaConcepto in (90048)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTérminovariableconsumoDistribuidora, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90049)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTérminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90050)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTérminofijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90051)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalCaudaldemandadoDistribuidora,
	CASE WHEN FacturaConcepto in (90052)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTérminovariableconsumoDistribuidora,
	CASE WHEN FacturaConcepto in (90053)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTérminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90054)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTérminoFijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90055)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,
	CASE WHEN FacturaConcepto in (90056)  THEN sum(ImporteBase) Else 0 End as OtroscostesderegasificacióntérminofijoclienteDistribuidora, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90057)  THEN sum(ImporteBase) Else 0 End as OtroscostesderegasificacióntérminoFijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90058)  THEN sum(ImporteBase) Else 0 End as CargoTérminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90059)  THEN sum(ImporteBase) Else 0 End as CargoTérminofijoCapacidadDistribuidora,

	CASE WHEN FacturaConcepto in (90062)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTerminoVariableConsumo,
	CASE WHEN FacturaConcepto in (90035)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTerminofijoCliente,
	CASE WHEN FacturaConcepto in (90036)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalTerminofijoCapacidad, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90037)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedLocalCaudalDemandado,
	CASE WHEN FacturaConcepto in (90038)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTerminoVariableConsmuo,
	CASE WHEN FacturaConcepto in (90039)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTerminoFijoCliente,
	CASE WHEN FacturaConcepto in (90040)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteTerminoFijoCapacidad,
	CASE WHEN FacturaConcepto in (90041)  THEN sum(ImporteBase) Else 0 End as PeajeSalidaRedTransporteCaudalDemandado,
	CASE WHEN FacturaConcepto in (90042)  THEN sum(ImporteBase) Else 0 End as Otroscostesderegasificacióntérminofijocliente,
	CASE WHEN FacturaConcepto in (90043)  THEN sum(ImporteBase) Else 0 End as Otroscostesderegasificacióntérminofijocapacidad,
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
	Id, sum(ImporteATRVariable) as ImporteATRVariable, sum(ImporteFijo) as ImporteFijo, sum(Alquiler) as Alquiler, sum(IH) as IH, 
	sum(ImportePG) as ImportePG, sum(Adicionales01) as Adicionales01,	sum(DescuentosEnergia) as DescuentosEnergia, 
	sum(DescuentosFijo) as DescuentosFijo, sum(Canon) as Canon, sum(OtrosATR) as OtrosATR, sum(OtrosEnergia) as OtrosEnergia,
	sum (caudaldemandado) as caudaldemandado,


	sum(TerminoVariablePersonalizado) as TerminoVariablePersonalizado,

	sum(PeajeSalidaRedLocalTérminovariableconsumoDistribuidora) as PeajeSalidaRedLocalTérminovariableconsumoDistribuidora,sum(PeajeSalidaRedLocalTérminofijoclienteDistribuidora) as PeajeSalidaRedLocalTérminofijoclienteDistribuidora,
	sum(PeajeSalidaRedLocalTérminofijoCapacidadDistribuidora) as PeajeSalidaRedLocalTérminofijoCapacidadDistribuidora,sum(PeajeSalidaRedLocalCaudaldemandadoDistribuidora) as PeajeSalidaRedLocalCaudaldemandadoDistribuidora,
	sum(PeajeSalidaRedTransporteTérminovariableconsumoDistribuidora) as PeajeSalidaRedTransporteTérminovariableconsumoDistribuidora,sum(PeajeSalidaRedTransporteTérminofijoclienteDistribuidora) as PeajeSalidaRedTransporteTérminofijoclienteDistribuidora,
	sum(PeajeSalidaRedTransporteTérminoFijoCapacidadDistribuidora) as PeajeSalidaRedTransporteTérminoFijoCapacidadDistribuidora,sum(PeajeSalidaRedTransporteCaudaldemandadoDistribuidora) as PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,
	sum(OtroscostesderegasificacióntérminofijoclienteDistribuidora) as OtroscostesderegasificacióntérminofijoclienteDistribuidora,sum(OtroscostesderegasificacióntérminoFijoCapacidadDistribuidora) as OtroscostesderegasificacióntérminoFijoCapacidadDistribuidora,
	sum(CargoTérminofijoclienteDistribuidora) as CargoTérminofijoclienteDistribuidora,sum(CargoTérminofijoCapacidadDistribuidora) as CargoTérminofijoCapacidadDistribuidora,
	sum(CuotaDelGTSDistribuidora) as CuotaDelGTSDistribuidora,sum(TasaCNMCDistribuidora) as TasaCNMCDistribuidora,sum(AlmacenamientoSubterraneo) as AlmacenamientoSubterraneo,
	sum(PeajeSalidaRedLocalTerminoVariableConsumo) as PeajeSalidaRedLocalTerminoVariableConsumo,

	sum(PeajeSalidaRedLocalTerminofijoCliente) as PeajeSalidaRedLocalTerminofijoCliente,sum(PeajeSalidaRedLocalTerminofijoCapacidad) as PeajeSalidaRedLocalTerminofijoCapacidad,
	sum(PeajeSalidaRedLocalCaudalDemandado) as PeajeSalidaRedLocalCaudalDemandado,sum(PeajeSalidaRedTransporteTerminoVariableConsmuo) as PeajeSalidaRedTransporteTerminoVariableConsmuo,
	sum(PeajeSalidaRedTransporteTerminoFijoCliente) as PeajeSalidaRedTransporteTerminoFijoCliente,sum(PeajeSalidaRedTransporteTerminoFijoCapacidad) as PeajeSalidaRedTransporteTerminoFijoCapacidad,
	sum(PeajeSalidaRedTransporteCaudalDemandado) as PeajeSalidaRedTransporteCaudalDemandado,sum(Otroscostesderegasificacióntérminofijocliente) as Otroscostesderegasificacióntérminofijocliente,
	sum(Otroscostesderegasificacióntérminofijocapacidad) as Otroscostesderegasificacióntérminofijocapacidad,sum(CargoTerminofijocliente) as CargoTerminofijocliente,
	sum(CargoTerminofijocapacidad) as CargoTerminofijocapacidad,sum(CuotaDelGTS) as CuotaDelGTS,
	sum(TasaCNMC) as TasaCNMC


	FROM AgrupadaLineas
	group by id
)
--,
--Lecturas as (
--Select Max(IdLectura) as IdLectura, IdFacturaVentaCabeceraSectorC, IdFacturaCompracabecera from Lectura
--where  Lectura.IdFacturaVentaCabeceraSectorC in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
--group by IdFacturaVentaCabeceraSectorC, IdFacturaCompracabecera
--)
,
ImportesProductos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas 
where FacturaConcepto in (100001, 100002, 110001, 90009,90046,90047,90060,90061,90008)--Añadimos cuotagts , tasa cnmc y canon aqui a peticion del cliente
group by Lineas.IdFacturaVentaCabecera
),
DescripcionProductos as (
Select Lineas.IdFacturaVentacabecera, STRING_AGG(descripcion, ' / ') AS Descripciones from Lineas 
where FacturaConcepto in (100001, 100002, 110001, 90009, 90009,90046,90047,90060,90061,90008)--Añadimos cuotagts , tasa cnmc y canon aqui a peticion del cliente
group by Lineas.IdFacturaVentaCabecera
)
, ProductosContrato as (
Select pa.IdContrato,pa.IdProducto,pa.Importe,AplicarPrecioConsumo,producto.TextoProducto from ProductoAsignacion pa
inner join producto on pa.IdProducto =producto.IdProducto
where pa.IdContrato in (select IdContrato from PreseleccionContratos)--and pa.AplicarPrecioConsumo=1
)
select  
cliente.identidad as NIF
,cliente.RazonSocial As Razon_Social
,facturaventacabecera.CodigoContrato as NIS
,contrato.NumPedidoFacturacion
,cll.NombreCalle +' '+ REPLACE(cups.aclarador, 'GAS', '') as Direccion 
, CodigoCUPS as CUPS
,REPLACE(t.textotarifa, ' ', '')as Tarifa
,convert(varchar,facturaventacabecera.FechaFactura, 103) as Fecha_Factura
,FacturaVentaCabecera.SerieNumFactura as N_Factura
,lo.TextoOrigen Origen_Lectura
,ft.TextoFacturaTipo as Tipo_Factura
,case  when facturaventacabecera.idfacturaorigen is not  null 
THEN (select fv.SerieFactura+''+cast (fv.NumeroFactura as varchar) from FacturaVentaCabecera fv where fv.IdFacturaVentaCabecera=facturaventacabecera.idfacturaorigen)
Else ' '
End as N_Factura_Corregida
,ISNULL(CONVERT(VARCHAR,facturaventacabecera.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaInicioFactura)[1]', 'datetimeoffset'), 103), ' ') as Fecha_Desde
,ISNULL(CONVERT(VARCHAR,facturaventacabecera.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaFinalFactura)[1]', 'datetimeoffset'), 103), ' ') as Fecha_Hasta
,replace(fvt.ImporteTotal,'.',',')+'€' As ImporteTotal
,replace(fvt.ImporteBase,'.',',')+'€' As Base_IVA
,replace(fvt.ImporteImpuesto,'.',',')+'€' As IVA
,replace(SumaAgrupadaLineas.IH,'.',',')+'€' As IEH
,replace(SumaAgrupadaLineas.ImporteFijo,'.',',')+'€' As Importe_TF
,replace(SumaAgrupadaLineas.caudaldemandado,'.',',')+'€' As Importe_Penalizacion_TF
,replace(SumaAgrupadaLineas.ImportePG,'.',',')+'€' As Importe_TE
,replace(SumaAgrupadaLineas.Alquiler,'.',',')+'€' As Importe_EM
	,replace(Consumos.Consumo,'.',',') as Consumo_Total
	,replace(ContratoPotencia.Caudal,'.',',') as Caudal_Contratado
	,replace(ContratoPotencia.Caudal,'.',',') as Caudal_Maximo
	,replace(ContratoPotencia.Caudal,'.',',') as Caudal_Facturado
	,replace(ipg.A,'.',',') as A
	,replace(ipg.B,'.',',') as B
	,replace(Convert(decimal(18,3),(select avg(preciooperadorgas) as PrecioMedioMibgas from PrecioOperadorGas 
	where FechaOperadorGas>=CONVERT(VARCHAR,facturaventacabecera.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaInicioFactura)[1]', 'datetimeoffset'), 103) 
	and FechaOperadorGas<=CONVERT(VARCHAR,facturaventacabecera.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaFinalFactura)[1]', 'datetimeoffset'), 103))),'.',',') as MIBGAS

	,
		replace(case when  Consumos.Precio=PrecioVariable then PrecioVariable else --JST-7052 Para no dañar la consultar de Consumos
Consumos.Precio+PrecioVariable end ,'.',',') as Precio_1 -- comparemos si son iguales.. de ser asi solo mostramos el precio variable y si no la suma de ambos.
	,consumos.Precio
	,PrecioVariable
	,DATEDIFF(DAY,
    facturaventacabecera.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaInicioFactura)[1]', 'datetimeoffset'),
    facturaventacabecera.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaFinalFactura)[1]', 'datetimeoffset'))+1 AS Dias_1
	,'0' as 'MIBGAS(*)'
	,'0' as 'Precio_2(*)'
	,'0' as 'Dias_2(*)'
	,distribuidora.nombrefiscal as Distribuidora
	,'' as Cambio_Precio
	,replace(isnull(ImportesProductos.Importe, 0),'.',',') as Imp_Otros_Conceptos
	,DescripcionProductos.Descripciones as Conceptos

	,fvt.PorcentajeImpuesto As PorcentajeIVA


--	,replace(Consumos.Precio,'.',',') as EnergiaPrecio,
--	replace(COnsumos.ImporteConsumo,'.',',') as ImporteConsumo,
--	replace(Consumos.VolumenTotal,'.',',') as Volumentotal,

--	replace(AgrupadaPotencias.PotenciaContratada,'.',',') as TerminoFijoContratado,
--	replace(AgrupadaPotencias.PotenciaFacturar,'.',',') as TerminoFijoFactura,
--	replace(AgrupadaPotencias.Precio,'.',',') as PrecioTerminoFijo,
		

--	replace(ConsumoVariable,'.',',') AS KwhVariable,
--	replace(PrecioVariable,'.',',') AS PrecioVariable,
--	replace(SumaAgrupadaLineas.Adicionales01,'.',',') as Productos,
--	replace(SumaAgrupadaLineas.DescuentosEnergia,'.',',') as DescuentosEnergia,
--	replace(SumaAgrupadaLineas.DescuentosFijo,'.',',') as DescuentosFijo,
--	replace(SumaAgrupadaLineas.Canon,'.',',') as Canon,
--	replace(SumaAgrupadaLineas.OtrosATR,'.',',') As OtrosATR,
--	replace(SumaAgrupadaLineas.OtrosEnergia,'.',',') As OtrosEnergia,
--	replace(SumaAgrupadaLineas.TerminoVariablePersonalizado,'.',',') As TerminoVariablePersonalizado,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTérminovariableconsumoDistribuidora,'.',',') As PeajeSalidaRedLocalTérminovariableconsumoDistribuidora,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTérminofijoclienteDistribuidora,'.',',') As PeajeSalidaRedLocalTérminofijoclienteDistribuidora,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTérminofijoCapacidadDistribuidora,'.',',') As PeajeSalidaRedLocalTérminofijoCapacidadDistribuidora,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalCaudaldemandadoDistribuidora,'.',',') As PeajeSalidaRedLocalCaudaldemandadoDistribuidora,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTérminovariableconsumoDistribuidora,'.',',') As PeajeSalidaRedTransporteTérminovariableconsumoDistribuidora,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTérminofijoclienteDistribuidora,'.',',') As PeajeSalidaRedTransporteTérminofijoclienteDistribuidora,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTérminoFijoCapacidadDistribuidora,'.',',') As PeajeSalidaRedTransporteTérminoFijoCapacidadDistribuidora,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,'.',',') As PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,
--	replace(SumaAgrupadaLineas.OtroscostesderegasificacióntérminofijoclienteDistribuidora,'.',',') As OtroscostesderegasificacióntérminofijoclienteDistribuidora,
--	replace(SumaAgrupadaLineas.OtroscostesderegasificacióntérminoFijoCapacidadDistribuidora,'.',',') As OtroscostesderegasificacióntérminoFijoCapacidadDistribuidora,
--	replace(SumaAgrupadaLineas.CargoTérminofijoclienteDistribuidora,'.',',') As CargoTérminofijoclienteDistribuidora,
--	replace(SumaAgrupadaLineas.CargoTérminofijoCapacidadDistribuidora,'.',',') As CargoTérminofijoCapacidadDistribuidora,
--	replace(SumaAgrupadaLineas.CuotaDelGTSDistribuidora,'.',',') As CuotaDelGTSDistribuidora,
--	replace(SumaAgrupadaLineas.TasaCNMCDistribuidora,'.',',') As TasaCNMCDistribuidora,
--	replace(SumaAgrupadaLineas.AlmacenamientoSubterraneo,'.',',') As AlmacenamientoSubterraneo,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTerminoVariableConsumo,'.',',') As PeajeSalidaRedLocalTerminoVariableConsumo,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTerminofijoCliente,'.',',') As PeajeSalidaRedLocalTerminofijoCliente,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalTerminofijoCapacidad,'.',',') As PeajeSalidaRedLocalTerminofijoCapacidad,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedLocalCaudalDemandado,'.',',') As PeajeSalidaRedLocalCaudalDemandado,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoVariableConsmuo,'.',',') As PeajeSalidaRedTransporteTerminoVariableConsmuo,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoFijoCliente,'.',',') As PeajeSalidaRedTransporteTerminoFijoCliente,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoFijoCapacidad,'.',',') As PeajeSalidaRedTransporteTerminoFijoCapacidad,
--	replace(SumaAgrupadaLineas.PeajeSalidaRedTransporteCaudalDemandado,'.',',') As PeajeSalidaRedTransporteCaudalDemandado,
--	replace(SumaAgrupadaLineas.Otroscostesderegasificacióntérminofijocliente,'.',',') As Otroscostesderegasificacióntérminofijocliente,
--	replace(SumaAgrupadaLineas.Otroscostesderegasificacióntérminofijocapacidad,'.',',') As Otroscostesderegasificacióntérminofijocapacidad,
--	replace(SumaAgrupadaLineas.CargoTerminofijocliente ,'.',',')As CargoTerminofijocliente,
--	replace(SumaAgrupadaLineas.CargoTerminofijocapacidad,'.',',') As CargoTerminofijocapacidad,
--	replace(SumaAgrupadaLineas.CuotaDelGTS,'.',',') As CuotaDelGTS,
--	replace(SumaAgrupadaLineas.TasaCNMC,'.',',') As TasaCNMC,
--	replace(tpp.TransporteFijoCapacidad,'.',',') As TransporteFijoCapacidadTarifaAcceso,
--	replace(tpp.Transportefijocliente,'.',',') As TransportefijoclienteTarifaAcceso,
--	replace(tpp.TransporteVariable,'.',',') As TransporteVariableTarifaAcceso,
--	replace(tpp.LocalFijoCapacidad,'.',',') As LocalFijoCapacidadTarifaAcceso,
--	replace(tpp.LocalFijoCliente,'.',',') As LocalFijoClienteTarifaAcceso,
--	replace(tpp.LocalVariableCapacidad,'.',',') As LocalVariableCapacidadTarifaAcceso,
--	replace(tpp.LocalVariableCliente,'.',',') As LocalVariableClienteTarifaAcceso,
--	replace(tpp.CargoFijoCapacidad,'.',',') As CargoFijoCapacidadTarifaAcceso,
--	replace(tpp.CargoFijoCliente,'.',',') As CargoFijoClienteTarifaAcceso,
--	replace(tpp.RegasificacionFijoTelemedido,'.',',') As RegasificacionFijoTelemedidoTarifaAcceso,
--	replace(tpp.RegasificacionFijoNoTelemedido,'.',',') As RegasificacionFijoNoTelemedido,
--	replace(pc.PrecioAlmacenamientoSubterraneo,'.',',') As PrecioAlmacenamientoSubterraneo
--	--PerfilFacturacion.TextoPerfilFacturacion

--    ,replace(ISNULL((select ProductoAsignacion.Importe from ProductoAsignacion where ProductoAsignacion.IdContrato = Contrato.IdContrato and ProductoAsignacion.IdProducto = 45), 0),'.',',') as AjustePeajesTFDia
--    ,replace(ISNULL((select ProductoAsignacion.Importe from ProductoAsignacion where ProductoAsignacion.IdContrato = Contrato.IdContrato and ProductoAsignacion.IdProducto = 46), 0),'.',',') as AjustePeajesTFQ
--    ,replace(ISNULL((select ProductoAsignacion.Importe from ProductoAsignacion where ProductoAsignacion.IdContrato = Contrato.IdContrato and ProductoAsignacion.IdProducto = 47), 0),'.',',') as AjustePeajesATR
--	--,Contrato.NumPedidoFacturacion
----	,facturacompracabecera.NumeroFactura NumeroFacturaCompra,
----		convert (date,facturacompracabecera.FechaRecepcion) as FechaRecepcionB70
--	--	,
--		--case 
--		--	when contrato.IsTelemedido = 1 
--		--	then 'S'
--		--	else 'N'
--		--end as Telemedida

--		,case ProductosContrato1.AplicarprecioConsumo when 1 then replace(ProductosContrato1.importe,'.',',') else replace(isnull(ImportesProductosDesglosado1.Importe,0),'.',',') end  as 'CO'
--		,case ProductosContrato4.AplicarprecioConsumo when 1 then replace(ProductosContrato4.importe,'.',',') else replace(isnull(ImportesProductosDesglosado4.Importe,0),'.',',') end  as 'CO interno'
--        ,case ProductosContrato2.AplicarprecioConsumo when 1 then replace(ProductosContrato2.importe,'.',',') else replace(isnull(ImportesProductosDesglosado2.Importe,0),'.',',') end  as 'Impresión en Papel'
--        ,case ProductosContrato3.AplicarprecioConsumo when 1 then replace(ProductosContrato3.importe,'.',',') else replace(isnull(ImportesProductosDesglosado3.Importe,0),'.',',') end  as 'Gastos - Devolucion recibo'
--        ,case ProductosContrato6.AplicarprecioConsumo when 1 then replace(ProductosContrato6.importe,'.',',') else replace(isnull(ImportesProductosDesglosado6.Importe,0),'.',',') end  as 'Actualización de FNEE Orden TED/268/2024 (0,000477€/kWh x Consumo)'
--        ,case ProductosContrato7.AplicarprecioConsumo when 1 then replace(ProductosContrato7.importe,'.',',') else replace(isnull(ImportesProductosDesglosado7.Importe,0),'.',',') end  as 'Actualización de FNEE Orden TED/197/2025 (0,000454€/kWh x Consumo)'
--		,case ProductosContrato9.AplicarprecioConsumo when 1 then replace(ProductosContrato9.importe,'.',',') else replace(isnull(ImportesProductosDesglosado9.Importe,0),'.',',') end  as 'Otros Productos'
		
	from facturaventacabecera WITH (NOLOCK)
		--Potencias
		left join 		
		AgrupadaPotencias as AgrupadaPotencias on FacturaVentaCabecera.IdFacturaVentaCabecera = AgrupadaPotencias.Id		
		--Lectura
		--left join
		--Lectura on Lectura.IdFacturaVentaCabeceraSectorC = FacturaVentaCabecera.IdFacturaVentaCabecera and Lectura.Facturar=1		
		--Contrato
		left join Contrato WITH (NOLOCK) on Contrato.IdContrato = FacturaventaCabecera.IdContrato
		inner join cups on Contrato.IdCups = CUPS.IdCups
		left join distribuidora on distribuidora.iddistribuidora = cups.iddistribuidora
		--AgrupadaConsumos
		left join AgrupadaConsumos  as Consumos on FacturaVentaCabecera.IdFacturaVentaCabecera = Consumos.Id
		--Impuesto Hidrocarburos
		left join SumaAgrupadaLineas as SumaAgrupadaLineas on FacturaVentaCabecera.IdFacturaVentaCabecera = SumaAgrupadaLineas.Id
	 --   left join Lecturas on Lecturas.IdFacturaVentaCabeceraSectorC = FacturaVentaCabecera.IdFacturaVentaCabecera
		--left join Lectura Lecturas2 on Lecturas2.IdFacturaVentaCabeceraSectorC = FacturaVentaCabecera.IdFacturaVentaCabecera	
		left join cliente WITH (NOLOCK) on cliente.IdCliente = FacturaVentaCabecera.IdCliente
		--PerfilFacturacion
		--left join PerfilFacturacion WITH (NOLOCK) on PerfilFacturacion.IdPerfilFacturacion=InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceralectura/IdPerfilFacturacion)[1]', 'bigint')
		 left join ContratoPotencia on contrato.idcontrato =ContratoPotencia.IdContrato
		 --left join FacturaCompraCabecera on FacturaCompraCabecera.IdFacturaCompraCabecera =lecturas.IdFacturaCompraCabecera
		 left join productoasignacion on productoasignacion.idcontrato =contrato.idcontrato and idproducto = 30
         left join callejero cll with (nolock) on cll.IdCallejero = cups.IdCallejero
		 left join FacturaVentaTotal fvt with (nolock) on FacturaVentaCabecera.idfacturaventacabecera = fvt.idfacturaventacabecera
		 left join Tarifa t with (nolock) on FacturaVentaCabecera.IdTarifaPeajeXML = t.IdTarifa
		 left join TarifaPeaje tp with (nolock) on FacturaVentaCabecera.IdTarifaPeajeXML = tp.IdTarifapeaje
		 left join TarifaPeajePrecio tpp with (nolock) on FacturaVentaCabecera.IdTarifaPeajeXML = tpp.IdTarifapeaje and tpp.FechaFinal is null
		 left join PeajeComercializadora pc on pc.IdTarifa = t.IdTarifa and isnull(pc.FechaFin,'31/12/2045')>=getdate()
		 left join ContratoTarifa ct on ct.CodigoContrato=contrato.CodigoContrato and ct.IdTarifaGrupo = FacturaVentaCabecera.IdTarifaGrupoXML and (ct.fechahasta is null or (ct.FechaHasta>=facturaventacabecera.FechaLecturaActualXML and ct.FechaDesde<=facturaventacabecera.FechaLecturaActualXML))
		 left join TarifaPreciocontrato tpc on ct.IdContratoTarifa = tpc.IdContratoTarifa
		 left join IndexadoPrecioGas ipg on ipg.IdIndexadoPrecioGas = tpc.IdIndexadoPrecioGas
		 left join ImportesProductos on ImportesProductos.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
		 left join DescripcionProductos on DescripcionProductos.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
		 left join lectura l on case when isnull(FacturaVentaCabecera.IsAbono,0)=1 then FacturaVentaCabecera.IdFacturaOrigen else FacturaVentaCabecera.IdFacturaVentaCabecera end = l.IdFacturaVentaCabeceraSectorC
		 left join LecturaOrigen lo on l.IdLecturaOrigen = lo.IdLecturaOrigen
		 left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]', 'decimal(18,8)'), 0)) PrecioVariable
			from LineasFactura with (nolock) 
			where FacturaConcepto = 90012
			group by IdFacturaVentaCabecera) as fvlP on facturaventacabecera.idfacturaventacabecera = fvlP.idfacturaventacabecera
		--left join  (select IdFacturaVentaCabecera, sum(ImporteBase) importebase ,sum(ISNULL(LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/TotConsumo)[1]', 'decimal(18,6)'), 0)) ConsumoVariable
		--	from LineasFactura with (nolock) 
		--	where FacturaConcepto = 90012 
		--	group by IdFacturaVentaCabecera) as fvlC on facturaventacabecera.idfacturaventacabecera = fvlC.idfacturaventacabecera
		left join facturatipo ft with (nolock) on ft.IdFacturaTipo = facturaventacabecera.IdFacturaTipo

		 WHERE facturaventacabecera.IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)


		 