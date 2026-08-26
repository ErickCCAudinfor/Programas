--Erick _Gestor Sige_Consulta_Cogeneracion_Por_Factura
with
SumConsumosGas as
(
SELECT id AS IdFacturaVentaCabeceraGas, 
                           SUM(ConsumoTotal1) AS ConsumoTotal 
                                  FROM (SELECT IdFacturaVentaCabecera as id, 
                                                       ConsumoT1 as ConsumoTotal1 
                                  FROM (SELECT FacturaVentaLinea.IdFacturaVentaCabecera, 
                                                ISNULL(FacturaVentaLinea.TotConsumoGasXML, 0) AS ConsumoT1, 
                                                FacturaVentaLinea.CodigoPeriodoXML as CodPeriodo 
                                                FROM FacturaVentaLinea WITH(NOLOCK)
                                                WHERE  FacturaConcepto in (90031,90001,90002,90038,90048,90062) and Entorno = 'E2') AS bumba --Añado el 90048 jst-5894
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
                                                WHERE  FacturaConcepto in (90012) and Entorno = 'E2') AS bumba 
                                  GROUP BY bumba.CodPeriodo, IdFacturaVentaCabecera, bumba.consumot1) AS FacturaVentaLineaTotalGas 
                                  GROUP BY id
),
FacturaOriginal AS (
    -- Nivel base
    SELECT 
        IdFacturaVentaCabecera,
        CASE WHEN IsAbono = 1 THEN IdFacturaOrigen ELSE IdFacturaVentaCabecera END AS FacturaOriginal,
        IsAbono,
        CodigoContrato,
        SerieFactura,
        NumeroFactura,
        IdFacturaOrigen
    FROM FacturaVentaCabecera

    UNION ALL

    -- Nivel recursivo: seguimos la cadena de abonos
    SELECT 
        f.IdFacturaVentaCabecera,
        fo.FacturaOriginal,
        f.IsAbono,
        f.CodigoContrato,
        f.SerieFactura,
        f.NumeroFactura,
        f.IdFacturaOrigen
    FROM FacturaVentaCabecera f
    INNER JOIN FacturaOriginal fo
        ON f.IdFacturaOrigen = fo.IdFacturaVentaCabecera
    WHERE f.IsAbono = 1
)


,consultaf as(select CodigoCUPS,fvc.codigocontrato,fvc.SerieFactura,fvc.NumeroFactura,fvcc.SerieNumFactura, fvc.IdFacturaVentaCabecera,fvc.IdFacturaOrigen,l.IdFacturaVentaCabeceraSectorC,
isnull(em.NumeroSerie,em2.NumeroSerie) NumeroSerie
,	isnull(em.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeExento)[1]', 'float'),isnull(em2.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeExento)[1]', 'float'),0) )  PorcentajeExento
,	isnull(em.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeNormal)[1]', 'float'),isnull(em2.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeNormal)[1]', 'float'),0))  PorcentajeNormal
,	isnull(em.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeVehicular)[1]', 'float'),isnull(em2.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeVehicular)[1]', 'float'),0))  PorcentajeVehicular
,	 replace( isnull(c.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeExento)[1]', 'decimal(18,2)'),0),'.',',') PorcentajeExentoContrato
,
case when isnull(fvc.IsAbono,0)=1 then
replace(isnull(equipos.InfoLecturaXML.value('(/LecturaInfoDTO/ConsumoGasKwh)[1]', 'decimal(38,6)'), l.InfoLecturaXML.value('(/LecturaInfoDTO/ConsumoGasKwh)[1]', 'decimal(38,6)'))*-1,'.',',')
else
replace(isnull(equipos.InfoLecturaXML.value('(/LecturaInfoDTO/ConsumoGasKwh)[1]', 'decimal(38,6)'), l.InfoLecturaXML.value('(/LecturaInfoDTO/ConsumoGasKwh)[1]', 'decimal(38,6)')),'.',',')
end ConsumoGasContador


    FROM FacturaOriginal fvc
	left join FacturaVentaCabecera fvcc on fvc.IdFacturaVentaCabecera = fvcc.IdFacturaVentaCabecera
 LEFT JOIN Lectura l
    ON 
	(
        (isnull(fvc.IsAbono,0) = 1 AND l.IdLectura = fvcc.InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceralectura/IdLectura)[1]', 'bigint'))
        OR
        (isnull(fvc.IsAbono,0) = 0 AND l.IdFacturaVentaCabeceraSectorC = fvc.IdFacturaVentaCabecera)
       )

	   --l.IdFacturaVentaCabeceraSectorC = fvc.IdFacturaVentaCabecera

left join (
		select em2.idequipomedida, lc.codigocontrato, fc.IdFacturaCompraCabecera, lc.IdLectura,lc.InfoLecturaXML from FacturaCompraCabecera fc
		left join lectura lc on fc.idfacturacompracabecera = lc.idfacturacompracabecera and IdFacturaVentaCabeceraSectorC is null
		left join equipomedida em2 on lc.IdEquipoMedida = em2.IdEquipoMedida
		--where  isnull(em2.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeExento)[1]', 'float'),0)<>0
		  ) equipos on l.IdFacturaCompraCabecera = equipos.IdFacturaCompraCabecera
	left join equipomedida em with(nolock) on equipos.idequipomedida = em.idequipomedida
	left join equipomedida em2 with(nolock) on l.idequipomedida = em2.idequipomedida
	LEFT JOIN SumConsumosGas AS ConsumosGas WITH(NOLOCK)  ON ConsumosGas.IdFacturaVentaCabeceraGas = fvc.IdFacturaVentaCabecera 
LEFT JOIN SumConsumosGas2 AS ConsumosGas2 WITH(NOLOCK)  ON ConsumosGas2.IdFacturaVentaCabeceraGas = fvc.IdFacturaVentaCabecera 
left join contrato c on fvc.CodigoContrato = c.CodigoContrato
left join cups on c.IdCups = cups.IdCups
where (fvc.idfacturaventacabecera in (
IdsFacturasReplace
) ) 
)
--,CONSULTASemi_1 as(
select distinct*
 from consultaf 