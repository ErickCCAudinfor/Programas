
declare
       @Entorno nvarchar(10)='E1'

SET NOCOUNT ON

 
DECLARE @DateTimeMaxValue as datetime = CONVERT(Datetime, '3000-12-31T00:00:00.000', 126)
DECLARE @DateTimeMinValue as datetime = CONVERT(Datetime, '1800-01-01T00:00:00.000', 126)

;with XMLNAMESPACES ('http://localhost/elegibilidad' AS ns),
FVL_SUM_TotConsumoEnergiaXML_EnergiaAcceso as
(
	SELECT IdFacturaVentaCabecera,
    SUM(CASE
            WHEN TotConsumoEnergiaXML < 2147483647 and TotConsumoEnergiaXML > -2147483647 THEN TotConsumoEnergiaXML
			
            ELSE 0
        END) AS ConsumoTotal
	FROM FacturaVentaLinea WITH(NOLOCK)
	where  FacturaConcepto = 30001 and Entorno=@Entorno
	group by IdFacturaVentaCabecera
)
,FVL_SUM_TotConsumoEnergiaXML_Otros as
(
	SELECT IdFacturaVentaCabecera
	,SUM(TotConsumoEnergiaXML) as ConsumoTotal
	FROM FacturaVentaLinea WITH(NOLOCK)
	where FacturaConcepto between 30002 and 30999 and Entorno=@Entorno
	group by IdFacturaVentaCabecera
)
,SerieNumeroFacturasOrigen as
(
	select CONCAT( origen.seriefactura, ' ',  origen.numerofactura) as SerieNumeroFacturaOrigen, principal.IdFacturaVentaCabecera
	from FacturaVentaCabecera principal WITH(NOLOCK)
	inner join FacturaVentaCabecera origen WITH(NOLOCK)
	on principal.IdFacturaOrigen = origen.IdFacturaVentaCabecera
)
,FVL_SUM_ImporteBase_IH as
(
	SELECT IdFacturaVentaCabecera, sum(ImporteBase) As Importe
	FROM  FacturaVentaLinea WITH(NOLOCK)
	where  FacturaConcepto in (90004,90017, 90025) and Entorno=@Entorno
	group by IdFacturaVentaCabecera
)
,FVT_SUM_Importes as
(
	Select IdFacturaVentaCabecera
	, sum(ImporteTotal) as ImporteTotal
	, sum(ImporteBase) as ImporteBase
	, sum(ImporteImpuesto) as ImporteImpuesto
	, sum(ISNULL(ImporteAdicional,0)) as ImporteAdicional
    from FacturaVentaTotal WITH(NOLOCK)
	group by IdFacturaVentaCabecera
)
, FVL_SUM_Potencia_ConsumoReactiva as (
	Select IdFacturaVentaCabecera
	,convert(decimal(18,3),sum(isnull(facturaventalinea.PotenciaAFacturarXML,0)))  as PotenciaAFacturarTotal,
	convert(decimal(18,3),SUM(ISNULL(FacturaVentaLinea.ConsumoReactivaXML, 0)))as ConsumoReactivaTotal                      
	from FacturaVentaLinea WITH(NOLOCK)
	group by IdFacturaVentaCabecera
)
, FVL_SUM_ImporteBaseAlquiler as (
	Select IdFacturaVentaCabecera
	,SUM(ImporteBase) AS ImporteBaseAlquiler
	from FacturaVentaLinea  WITH(NOLOCK)                
	where FacturaConcepto in(50001,50002,50003, 90013,90005,90014,90015) and Entorno=@Entorno
	group by IdFacturaVentaCabecera
)
, FVL_SUM_ImporteBaseProductos as 
(
	Select IdFacturaVentaCabecera
	,SUM(ImporteBase) AS ImporteBaseProductos
	from FacturaVentaLinea WITH(NOLOCK)                 
	where FacturaConcepto in(100001,100002, 70020) and Entorno=@Entorno
	group by IdFacturaVentaCabecera
)
, FVL_SUM_ImporteBaseImpuestoIE as
(
	Select IdFacturaVentaCabecera
	,SUM(ImporteBase) AS ImporteBaseImpuestoIE
	from FacturaVentaLinea WITH(NOLOCK)                 
	where FacturaConcepto in(60001,60002,60010,60011,60012) and Entorno=@Entorno
	group by IdFacturaVentaCabecera
)
, FVL_SUM_ImporteIEElectricidadE as (
	SELECT IdFacturaVentaCabecera,sum(ImporteBase) AS ImporteIEElectricidad 
	FROM FacturaVentaLinea WITH(NOLOCK)
	WHERE FacturaConcepto in (60001,60002) and Entorno=@Entorno
	GROUP BY IdFacturaVentaCabecera
)
, FVL_SUM_ImporteIEGas as
(
	SELECT IdFacturaVentaCabecera,sum(ImporteBase) AS ImporteIEGas 
	FROM FacturaVentaLinea WITH(NOLOCK)
	WHERE FacturaConcepto in (90004)  and Entorno=@Entorno
	GROUP BY IdFacturaVentaCabecera
)
,DiferenciaPagosPorCapacidad_Importe as
(
    SELECT IdFacturaVentaCabecera, sum(ImporteBase) As Importe
	FROM FacturaVentaLinea WITH(NOLOCK)
	where  FacturaConcepto = 120009 and Entorno=@Entorno
	group by IdFacturaVentaCabecera
)
,AjusteCapGas_Importe as
(
    SELECT IdFacturaVentaCabecera, sum(ImporteBase) As Importe
	FROM FacturaVentaLinea WITH(NOLOCK)
	where facturaconcepto in (30004,30006) and  IsAjusteCAPGas = 1 and Entorno=@Entorno
	group by IdFacturaVentaCabecera
)
--JET-578
,ServiciosAjusteSistema_Importe as
(
    Select linea.IdFacturaVentaCabecera
	,SUM(distinct linea.ImporteBase) AS ImporteProductos
	from FacturaVentaLinea linea WITH(NOLOCK)
	INNER JOIN FacturaVentaCabecera cabecera ON cabecera.IdFacturaVentaCabecera = linea.IdFacturaVentaCabecera
	where linea.FacturaConcepto in(100001) and linea.Entorno= @Entorno
	and linea.Infolineaxml.value('(FacturaConceptosDTO/ConceptoProductos/IdProducto)[1]', 'decimal(6)') in
	(select idproducto from producto where IsServiciosAjusteSistema = 1)
	GROUP BY linea.IdFacturaVentaCabecera
)
, SumConsumosGas as
(
SELECT id AS IdFacturaVentaCabeceraGas, 
                           SUM(ConsumoTotal1) AS ConsumoTotal 
                                  FROM (SELECT IdFacturaVentaCabecera as id, 
                                                       ConsumoT1 as ConsumoTotal1 
                                  FROM (SELECT FacturaVentaLinea.IdFacturaVentaCabecera, 
                                                ISNULL(FacturaVentaLinea.TotConsumoGasXML, 0) AS ConsumoT1, 
                                                FacturaVentaLinea.CodigoPeriodoXML as CodPeriodo 
                                                FROM FacturaVentaLinea WITH(NOLOCK)
                                                WHERE  FacturaConcepto in (90031,90001,90002,90038,90048,90062) and Entorno = @Entorno) AS bumba --Añado el 90048 jst-5894
                                  GROUP BY bumba.CodPeriodo, IdFacturaVentaCabecera, bumba.consumot1) AS FacturaVentaLineaTotalGas 
                                  GROUP BY id
)
--Se saca el consuymo de las lineas con concepto 90012 por si el Consumo anterior mirado es Null
, SumConsumosGas2 as
(
SELECT id AS IdFacturaVentaCabeceraGas, 
                           SUM(ConsumoTotal1) AS ConsumoTotal 
                                  FROM (SELECT IdFacturaVentaCabecera as id, 
                                                       ConsumoT1 as ConsumoTotal1 
                                  FROM (SELECT FacturaVentaLinea.IdFacturaVentaCabecera, 
                                                ISNULL(FacturaVentaLinea.TotConsumoGasXML, 0) AS ConsumoT1, 
                                                FacturaVentaLinea.CodigoPeriodoXML as CodPeriodo 
                                                FROM FacturaVentaLinea WITH(NOLOCK)
                                                WHERE  FacturaConcepto in (90012) and Entorno = @Entorno) AS bumba 
                                  GROUP BY bumba.CodPeriodo, IdFacturaVentaCabecera, bumba.consumot1) AS FacturaVentaLineaTotalGas 
                                  GROUP BY id
)
,fvo(idfvo) as  (

select idfacturaorigen from FacturaVentaCabecera WITH(NOLOCK)
inner join lectura WITH(NOLOCK) on lectura.IdFacturaVentaCabeceraSectorC=FacturaVentaCabecera.IdFacturaOrigen
group by facturaventacabecera.idfacturaorigen)



SELECT 
             Contrato.IdContrato,		 
		CASE WHEN Contrato.isFirmaDigitalEnviada = 1 
		THEN 'SI'
		ELSE 'NO' END as isFirmaDigitalEnviada,
		CASE WHEN Contrato.IsFirmadoDigitalmente = 1 
		THEN 'SI'
		ELSE 'NO' END as isFirmadoDigitalmente,
             Contrato.Entorno,                 
             Contrato.Version,   
             Contrato.IdCliente,
             FacturaVentaCabecera.FechaFactura,             
             FacturaVentaCabecera.SerieFactura,
             FacturaVentaCabecera.NumeroFactura,
			 ca.TextoCanal TipoFactura, 
             FacturaVentaCabecera.IdFacturaVentaCabecera,                       
             FacturaVentaCabecera.IdTipoCobro,
             TipoCobro.TextoTipoCobro,
             Cliente.Identidad AS IdentidadCliente,
			CASE
			WHEN Cliente.Nombre is null OR Cliente.Nombre = ''
				THEN Cliente.RazonSocial
				ELSE Cliente.Nombre + ' ' + ISNULL(Cliente.Apellido1, '') + ' ' + ISNULL(Cliente.Apellido2, '')
			END as DenominacionCliente,
            -- dbo.FormatearDenominacion(Cliente.Apellido1, Cliente.Apellido2, Cliente.Nombre, Cliente.RazonSocial) AS DenominacionCliente,
             Cliente.Apellido1, 
             Cliente.Apellido2, 
             Cliente.Nombre, 
             Cliente.RazonSocial,                     
             Contrato.CodigoContrato,
			 CUPS.CodPostal as CodigoPostal,
             CUPS.CodigoCUPS,
             CUPS.IdRuta,
             CiudadCUPS.TextoCiudad AS TextoCiudadCUPS,
	   CONCAT(ISNULL(CallejeroCUPS.NombreCalle, '') , CASE LEN(ISNULL(CallejeroCUPS.NombreCalle, '')) WHEN 0 THEN '' ELSE ' ' END  
		, CONVERT(nvarchar(20), CUPS.Numero), CASE ISNULL(CUPS.Numero, 0)  WHEN 0 THEN '' ELSE ' ' END
		, ISNULL(CallejeroTipoNumeroCUPS.TextoNumero,'') , CASE LEN(ISNULL(CallejeroTipoNumeroCUPS.TextoNumero,'')) WHEN 0 THEN '' ELSE  ' ' END
		, ISNULL(CallejeroTipoCalificadorCUPS.TextoCalificador,'') , CASE LEN(ISNULL(CallejeroTipoCalificadorCUPS.TextoCalificador,'')) WHEN  0 THEN '' ELSE ' ' END
		, ISNULL(CallejeroPortalCUPS.TextoPortal,''), CASE LEN(ISNULL(CallejeroPortalCUPS.TextoPortal,'')) WHEN 0 THEN '' ELSE ' ' END
		, ISNULL(CallejeroEscaleraCUPS.TextoEscalera,''), CASE LEN(ISNULL(CallejeroEscaleraCUPS.TextoEscalera,'')) WHEN  0 THEN '' ELSE ' ' END
		, ISNULL(CallejeroPisoCUPS.TextoPiso,''), CASE LEN(ISNULL(CallejeroPisoCUPS.TextoPiso,'')) WHEN 0 THEN '' ELSE ' ' END
		, ISNULL(CallejeroPuertaCUPS.TextoPuerta,''), CASE LEN(ISNULL(CallejeroPuertaCUPS.TextoPuerta,'')) WHEN 0 THEN '' ELSE ' ' END
		, ISNULL(CUPS.Aclarador,'')
		) as DireccionCUPS,
             --       CUPS.Aclarador) AS DireccionCUPS,
             CUPS.IdDistribuidora AS IdDistribuidoraCUPS,
             CUPS.IdCups,
             Distribuidora.NombreFiscal AS DistribuidoraCUPS,             
             Comercializadora.NombreFiscal as Comercializadora,
             TarifaPeaje.TextoTarifaPeaje,                         
             Tarifa.TextoTarifa,
             Tarifa.IdTarifa,
             TarifaGrupo.IdTarifaGrupo,
             TarifaGrupo.TextoTarifaGrupo,
             --AgrupacionContrato.TextoAgrupacion,
             CASE
                    WHEN isnull(FacturaTotales.ConsumoTotal,0) <> 0  THEN FacturaTotales.ConsumoTotal
                    ELSE FacturaTotales2.ConsumoTotal
             END AS ConsumoTotal,
             FacturaVentaLineaTotal.PotenciaAFacturarTotal,
             FacturaVentaLineaTotal.ConsumoReactivaTotal,
             ImportesTotales.ImporteTotal AS ImporteTotal,
             ImportesTotales.ImporteBase AS ImporteBase,
             ImportesTotales.ImporteImpuesto AS ImporteImpuesto,
             CNAE.CodigoCNAE,
             CNAE.TextoCNAE,
			 FacturaVentaCabecera.FechaLecturaActualXML AS FechaLecturaActual,
			 FacturaVentaCabecera.FechaLecturaAnteriorXML AS FechaLecturaAnterior,             
             FacturaVentaLineaAlq.ImporteBaseAlquiler,
             FacturaVentaLineaProd.ImporteBaseProductos,
             FacturaVentaLineaImp.ImporteBaseImpuestoIE,
             ImportesTotales.ImporteAdicional,
             ConceptoIEElectricidad.ImporteIEElectricidad,
             ConceptoIEGas.ImporteIEGas,
			 DPC.Importe As DiferenciaPagosPorCapacidad,
			 CASE
                    WHEN isnull(ConsumosGas.ConsumoTotal,0) <> 0  THEN ConsumosGas.ConsumoTotal
                    ELSE ConsumosGas2.ConsumoTotal
             END AS ConsumoTotalGas,
             CiudadCUPS.TextoCiudad,
             ProvinciaCUPS.TextoProvincia,
		CarteraCobroCalendario.TextoCalendario as Calendario,
		IH.Importe As ImpHidrocarburos,
		FacturaVentaCabecera.AsuntosSociales,
		SerieNumeroFacturasOrigen.SerieNumeroFacturaOrigen,
		FacturaVentaCabecera.OrigenMedida,
		AjusteGas.Importe As AjusteCapGAS,

case facturaventacabecera.IsAbono
	when 1 then
		(select facturacompracabecera.SerieFactura from facturacompracabecera WITH(NOLOCK) left join canal WITH(NOLOCK)
		on facturacompracabecera.idcanal = canal.idcanal
		where canal.serie like'%ABO%' and
		FacturaVentaCabecera.IsAbono=1  and
		facturacompracabecera.idfacturaorigen in(select idfacturacompracabecera from facturacompracabecera where idfacturacompracabecera in (select
		IdFacturaCompraCabecera from lectura WITH(NOLOCK) where lectura.codigocontrato =facturaventacabecera.codigocontrato and  IdFacturaVentaCabeceraSectorC = fvo.idfvo)))
	else
		 FacturaCompraCabecera.SerieFactura 
end AS SerieFacturaATR,

case facturaventacabecera.IsAbono
	when 1 then
		(select facturacompracabecera.NumeroFactura from facturacompracabecera WITH(NOLOCK)  left join canal WITH(NOLOCK)
		on facturacompracabecera.idcanal = canal.idcanal
		where canal.serie like'%ABO%' and
		facturacompracabecera.idfacturaorigen in (select idfacturacompracabecera from facturacompracabecera WITH(NOLOCK) where idfacturacompracabecera in (select
		IdFacturaCompraCabecera from lectura where lectura.codigocontrato =facturaventacabecera.codigocontrato and IdFacturaVentaCabeceraSectorC = fvo.idfvo)))
	else
		 FacturaCompraCabecera.NumeroFactura
end AS NumFacturaATR,

case facturaventacabecera.IsAbono
	when 1 then
		(select facturacompracabecera.IdFacturaCompraCabecera from facturacompracabecera WITH(NOLOCK)
		left join canal WITH(NOLOCK)	on facturacompracabecera.idcanal = canal.idcanal
		where 
		canal.serie like'%ABO%' and
		FacturaCompraCabecera.IdCanal=9  and
		facturacompracabecera.idfacturaorigen in (select idfacturacompracabecera from facturacompracabecera where idfacturacompracabecera in (select
		IdFacturaCompraCabecera from lectura where lectura.codigocontrato =facturaventacabecera.codigocontrato and
		IdFacturaVentaCabeceraSectorC = fvo.idfvo)))
	else
		 FacturaCompraCabecera.IdFacturaCompraCabecera 
end AS IdFacturaATR,

		FacturaCompraCabecera.FechaRecepcion AS FechaRecepcion,
		case 
			when contrato.IsTelemedido = 1 
			then 'S'
			else 'N'
		end as Telemedida,
		--JET-356 Nueva configuración IH -GAS 
	   Contrato.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeReducido)[1]', 'decimal(5,2)') as PcIHRedudico,
	   Contrato.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeExento)[1]', 'decimal(5,2)') as PcIHExento,
	   Contrato.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeNormal)[1]', 'decimal(5,2)') as PcIHNormal,
	   Contrato.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeVehicular)[1]', 'decimal(5,2)') as PcIHVehicular
	   --JET-578
	   , ServiciosAjusteSistema.ImporteProductos as ServiciosAjusteSistema
	   ,FacturaCompraCabecera.facturaxml.value('(/ns:MensajeFacturacion/ns:Facturas/ns:FacturaATR/ns:DatosGeneralesFacturaATR/ns:DatosFacturaATR/ns:TipoAutoconsumo)[1]', 'VARCHAR(50)') AS TipoAutoconsumo
	   ,FacturaCompraCabecera.facturaxml.value('(/ns:MensajeFacturacion/ns:Facturas/ns:FacturaATR/ns:DatosGeneralesFacturaATR/ns:DatosFacturaATR/ns:TipoSubseccion)[1]', 'VARCHAR(50)') AS TipoSubseccion
	   ,contrato.Autoconsumo
	   ,contrato.IsAutoconsumoNoCompensable
       FROM FacturaVentaCabecera WITH(NOLOCK)
       LEFT JOIN Contrato WITH(NOLOCK) ON Contrato.IdContrato = FacturaVentaCabecera.IdContrato             
       LEFT JOIN CUPS WITH(NOLOCK) ON Contrato.IdCups = CUPS.IdCups
       LEFT JOIN Distribuidora WITH(NOLOCK) ON CUPS.IdDistribuidora = Distribuidora.IdDistribuidora
       LEFT JOIN Cliente WITH(NOLOCK) ON Contrato.IdCliente = Cliente.IdCliente  
       LEFT JOIN ClienteCategoria WITH(NOLOCK) on Cliente.IdClienteCategoria=ClienteCategoria.IdClienteCategoria
       LEFT JOIN CNAE WITH(NOLOCK) ON Contrato.IdCNAE = CNAE.IdCNAE
       LEFT JOIN TipoCobro WITH(NOLOCK) ON FacturaVentaCabecera.IdTipoCobro = TipoCobro.IdTipoCobro
       LEFT JOIN Ciudad AS CiudadCUPS WITH(NOLOCK) ON CUPS.IdCiudad = CiudadCUPS.IdCiudad
       Left Join Provincia as ProvinciaCUPS WITH(NOLOCK) on CiudadCUPS.IdProvincia = ProvinciaCUPS.IdProvincia
       LEFT JOIN Callejero AS CallejeroCUPS WITH(NOLOCK) ON CallejeroCUPS.IdCallejero = CUPS.IdCallejero 
       LEFT JOIN CallejeroTipoVia AS CallejeroTipoViaCUPS WITH(NOLOCK) ON CallejeroTipoViaCUPS.IdCallejeroTipoVia = CallejeroCUPS.IdCallejeroTipoVia
       LEFT JOIN CallejeroBloque AS CallejeroBloqueCUPS  WITH(NOLOCK) ON CallejeroBloqueCUPS.IdCallejeroBloque = CUPS.IdCallejeroBloque     
       LEFT JOIN CallejeroEscalera AS CallejeroEscaleraCUPS WITH(NOLOCK) ON CallejeroEscaleraCUPS.IdCallejeroEscalera = CUPS.IdCallejeroEscalera      
       LEFT JOIN CallejeroPiso AS CallejeroPisoCUPS  WITH(NOLOCK) ON CallejeroPisoCUPS.IdCallejeroPiso = CUPS.IdCallejeroPiso
       LEFT JOIN CallejeroPortal AS CallejeroPortalCUPS WITH(NOLOCK) ON CallejeroPortalCUPS.IdCallejeroPortal = CUPS.IdCallejeroPortal
       LEFT JOIN CallejeroPuerta AS CallejeroPuertaCUPS WITH(NOLOCK) ON CallejeroPuertaCUPS.IdCallejeroPuerta = CUPS.IdCallejeroPuerta     
       LEFT JOIN CallejeroTipoCalificador AS CallejeroTipoCalificadorCUPS WITH(NOLOCK) ON CallejeroTipoCalificadorCUPS.IdCallejeroTipoCalificador = CUPS.IdCallejeroTipoCalificador
       LEFT JOIN CallejeroTipoNumero AS CallejeroTipoNumeroCUPS WITH(NOLOCK) ON CallejeroTipoNumeroCUPS.IdCallejeroTipoNumero = CUPS.IdCallejeroTipoNum
       LEFT JOIN Comercializadora AS Comercializadora WITH(NOLOCK) ON Comercializadora.IdComercializadora = Contrato.IdComercializadora       
	   LEFT JOIN CarteraCobroCalendario WITH(NOLOCK) ON CarteraCobroCalendario.IdCarteraCobroCalendario = Contrato.IdCarteraCobroCalendario
		left join FVL_SUM_ImporteBase_IH as IH on IH.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera 
       LEFT JOIN TarifaPeaje AS TarifaPeaje WITH(NOLOCK) ON TarifaPeaje.IdTarifaPeaje = FacturaVentaCabecera.IdTarifaPeajeXML --CAST(FacturaVentaCabecera.InfoCabeceraXML.query('FacturaInfoCabeceraDTO/FacturaInfoCabeceralectura/IdTarifaPeaje/text()') as nvarchar(20))
       LEFT JOIN Tarifa AS Tarifa WITH(NOLOCK) ON Tarifa.IdTarifa = FacturaVentaCabecera.IdTarifaXML --CAST(FacturaVentaCabecera.InfoCabeceraXML.query('FacturaInfoCabeceraDTO/FacturaInfoCabeceralectura/IdTarifa/text()') as nvarchar(20))
       LEFT JOIN TarifaGrupo AS TarifaGrupo WITH(NOLOCK) ON TarifaGrupo.IdTarifaGrupo = FacturaVentaCabecera.IdTarifaGrupoXML --CAST(FacturaVentaCabecera.InfoCabeceraXML.query('FacturaInfoCabeceraDTO/FacturaInfoCabeceralectura/IdTarifaGrupo/text()') as nvarchar(20))
 
       LEFT JOIN FVT_SUM_Importes  AS ImportesTotales WITH(NOLOCK) ON ImportesTotales.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_Potencia_ConsumoReactiva AS FacturaVentaLineaTotal WITH(NOLOCK) ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaVentaLineaTotal.IdFacturaVentaCabecera 
 	  LEFT JOIN FVL_SUM_TotConsumoEnergiaXML_EnergiaAcceso AS FacturaTotales WITH(NOLOCK) ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaTotales.IdFacturaVentaCabecera 
 	  LEFT JOIN FVL_SUM_TotConsumoEnergiaXML_Otros AS FacturaTotales2 WITH(NOLOCK)  ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaTotales2.IdFacturaVentaCabecera
      LEFT JOIN FVL_SUM_ImporteBaseAlquiler AS FacturaVentaLineaAlq  WITH(NOLOCK) ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaVentaLineaAlq.IdFacturaVentaCabecera
      LEFT JOIN FVL_SUM_ImporteBaseProductos AS FacturaVentaLineaProd WITH(NOLOCK) ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaVentaLineaProd.IdFacturaVentaCabecera           
      LEFT JOIN FVL_SUM_ImporteBaseImpuestoIE AS FacturaVentaLineaImp WITH(NOLOCK) ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaVentaLineaImp.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_ImporteIEElectricidadE AS ConceptoIEElectricidad WITH(NOLOCK) ON ConceptoIEElectricidad.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera 
       LEFT JOIN FVL_SUM_ImporteIEGas AS ConceptoIEGas WITH(NOLOCK) ON ConceptoIEGas.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera 
		left join fvo on fvo.idfvo=facturaventacabecera.idfacturaorigen 
	   left join DiferenciaPagosPorCapacidad_Importe as DPC WITH(NOLOCK) on DPC.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
	   left join AjusteCapGas_Importe as AjusteGas WITH(NOLOCK) on AjusteGas.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN SumConsumosGas AS ConsumosGas WITH(NOLOCK)  ON ConsumosGas.IdFacturaVentaCabeceraGas = FacturaVentaCabecera.IdFacturaVentaCabecera 
	   LEFT JOIN SumConsumosGas2 AS ConsumosGas2 WITH(NOLOCK)  ON ConsumosGas2.IdFacturaVentaCabeceraGas = FacturaVentaCabecera.IdFacturaVentaCabecera 
       left join SerieNumeroFacturasOrigen WITH(NOLOCK) on FacturaVentaCabecera.IdFacturaVentaCabecera = SerieNumeroFacturasOrigen.IdFacturaVentaCabecera

		LEFT JOIN Lectura ON
			Lectura.IdFacturaVentaCabeceraSectorC =
			CASE 
				WHEN (FacturaVentaCabecera.IsAbono = 1) THEN FacturaVentaCabecera.IdFacturaRectificativa
				WHEN (ISNULL(FacturaVentaCabecera.IsAbono,0)= 0) THEN FacturaVentaCabecera.IdFacturaVentaCabecera 
			END
			AND Lectura.CodigoContrato = Contrato.CodigoContrato
		LEFT JOIN FacturaCompraCabecera ON FacturaCompraCabecera.IdFacturaCompraCabecera=Lectura.IdFacturaCompraCabecera
		LEFT JOIN ServiciosAjusteSistema_Importe as ServiciosAjusteSistema WITH(NOLOCK) on ServiciosAjusteSistema.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
		left join canal ca on FacturaVentaCabecera.IdCanal = ca.IdCanal
       WHERE FacturaVentaCabecera.Entorno = @Entorno
       AND FacturaVentaCabecera.seriefactura is not null  and FacturaVentaCabecera.FacturaCategoria='EN'	   
	   and IdTarifaGrupoXML in (select idtarifagrupo from TarifaGrupo where TextoTarifaGrupo like 'PF AM JC CLM 2026 LOTE%' ) 
and FacturaVentaCabecera.FechaFactura>='DesdeFechaReplace' and FacturaVentaCabecera.FechaFactura<='hastaFechaReplace'