--Erick _Gestor Sige_Consulta_Cogeneracion
with 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato,IsAbono, idfacturaorigen) as
(Select idfacturaventacabecera,Codigocontrato,IsAbono,idfacturaorigen from FacturaVentaCabecera with(nolock) where  IdFacturaVentaCabecera in (
select IdFacturaVentaCabecera from FacturaVentaCabecera 
where codigocontrato in (select codigocontrato from contrato c 
left join cups on c.IdCups = cups.IdCups
where c.Entorno='E2' and
((c.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeExento)[1]', 'decimal(5,2)')>0  )
or 
(
c.CodigoContrato in (select CodigoContrato from EquipoMedida where ImpuestoHidrocarburoXML.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeExento)[1]','decimal(5,2)')>0 ) 
))
) and
SerieFactura is not null and FacturaCategoria='EN' and 
FechaLecturaAnteriorXML<='hastaFechaReplace' and FechaLecturaActualXML>='DesdeFechaReplace' 
))
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
                                                WHERE  FacturaConcepto in (90031,90001,90002,90038,90048,90062) and Entorno = 'E2') AS bumba --Añado el 90048 jst-5894
                                  GROUP BY bumba.CodPeriodo, IdFacturaVentaCabecera, bumba.consumot1) AS FacturaVentaLineaTotalGas 
                                  GROUP BY id
)
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
InfoCabeceraXMLLectura as
(
	--SELECT Id , SUM(Consumo) As Consumo,  SUM(VolumenTotal) as VolumenTotal, Sum(ImporteBase) as  ImporteConsumo, Sum(Precio) as Precio, CodPeriodo
	--FROM Consumos
	--WHERE CodPeriodo = 1
	--group by id, CodPeriodo
	select idFacturaVentaCabecera,InfoCabeceraXML, InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceralectura/IdLectura)[1]', 'bigint') idlectura from FacturaVentaCabecera where idfacturaventacabecera in (SELECT idfacturaventacabecera FROM FacturasVentaConsulta) 
),
AgrupadaConsumos as
(
	--SELECT Id , SUM(Consumo) As Consumo,  SUM(VolumenTotal) as VolumenTotal, Sum(ImporteBase) as  ImporteConsumo, Sum(Precio) as Precio, CodPeriodo
	--FROM Consumos
	--WHERE CodPeriodo = 1
	--group by id, CodPeriodo
	select  IdFacturaVentaCabeceraSectorC id,infolecturaxml.value('(LecturaInfoDTO/ConsumoGasKwh)[1]', 'as decimal(18,6)')  Consumo  from lectura where IdFacturaVentaCabeceraSectorC in (select IdFacturaVentaCabecera from InfoCabeceraXMLLectura) 
)
,Lineas as
(
	SELECT IdFacturaVentaCabecera, sum(ImporteBase) As Importe, FacturaConcepto
	FROM  FacturaVentaLinea WITH (NOLOCK)
		where  IdFacturaVentaCabecera in ( select IdFacturaVentaCabecera from FacturasVentaConsulta)
		group by IdFacturaVentaCabecera, FacturaConcepto
),
AgrupadaLineas as
(
	SELECT IdFacturaVentaCabecera As Id, 	
	CASE WHEN FacturaConcepto = 90002 THEN  Importe ELSE 0 END as ImporteATRVariable,
	CASE WHEN FacturaConcepto in (90003, 90018, 90019,90070) THEN  Importe ELSE 0 END as ImporteATRFijo,
	CASE WHEN  FacturaConcepto in (90005,90013, 90014, 90015) THEN Importe Else 0 End as Alquiler,
	CASE WHEN FacturaConcepto in (90004,90017, 90025,90026, 90027) THEN Importe Else 0 End as IH,
	CASE WHEN FacturaConcepto in (90012, 90001, 90031)  THEN Importe Else 0 End as ImportePG,
	CASE WHEN FacturaConcepto in (70017, 100001)  THEN Importe Else 0 End as Adicionales01,
	CASE WHEN FacturaConcepto in (120001, 120002)  THEN Importe Else 0 End as Adicionales02,
	CASE WHEN FacturaConcepto in (90006, 120005,120007)  THEN Importe Else 0 End as DescuentosEnergia, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90007)  THEN Importe Else 0 End as DescuentosFijo,
	CASE WHEN FacturaConcepto in (90008)  THEN Importe Else 0 End as Canon,
	CASE WHEN FacturaConcepto in (90009)  THEN Importe Else 0 End as OtrosATR,
	CASE WHEN FacturaConcepto in (90010)  THEN Importe Else 0 End as OtrosEnergia,
	CASE WHEN FacturaConcepto in (90032)  THEN Importe Else 0 End as TerminoVariablePersonalizado,
	CASE WHEN FacturaConcepto in (90048)  THEN Importe Else 0 End as PeajeSalidaRedLocalTérminovariableconsumoDistribuidora, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90049)  THEN Importe Else 0 End as PeajeSalidaRedLocalTérminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90050)  THEN Importe Else 0 End as PeajeSalidaRedLocalTérminofijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90051)  THEN Importe Else 0 End as PeajeSalidaRedLocalCaudaldemandadoDistribuidora,
	CASE WHEN FacturaConcepto in (90052)  THEN Importe Else 0 End as PeajeSalidaRedTransporteTérminovariableconsumoDistribuidora,
	CASE WHEN FacturaConcepto in (90053)  THEN Importe Else 0 End as PeajeSalidaRedTransporteTérminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90054)  THEN Importe Else 0 End as PeajeSalidaRedTransporteTérminoFijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90055)  THEN Importe Else 0 End as PeajeSalidaRedTransporteCaudaldemandadoDistribuidora,
	CASE WHEN FacturaConcepto in (90056)  THEN Importe Else 0 End as OtroscostesderegasificacióntérminofijoclienteDistribuidora, --(90006, 120005,120007)
	CASE WHEN FacturaConcepto in (90057)  THEN Importe Else 0 End as OtroscostesderegasificacióntérminoFijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90058)  THEN Importe Else 0 End as CargoTérminofijoclienteDistribuidora,
	CASE WHEN FacturaConcepto in (90059)  THEN Importe Else 0 End as CargoTérminofijoCapacidadDistribuidora,
	CASE WHEN FacturaConcepto in (90062,90048)  THEN Importe Else 0 End as PeajeSalidaRedLocalTerminoVariableConsumo,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90035,90049)  THEN Importe Else 0 End as PeajeSalidaRedLocalTerminofijoCliente,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90036,90050)  THEN Importe Else 0 End as PeajeSalidaRedLocalTerminofijoCapacidad, --(90006, 120005,120007)--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90037,90051)  THEN Importe Else 0 End as PeajeSalidaRedLocalCaudalDemandado,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90038,90052)  THEN Importe Else 0 End as PeajeSalidaRedTransporteTerminoVariableConsmuo,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90039,90053)  THEN Importe Else 0 End as PeajeSalidaRedTransporteTerminoFijoCliente,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90040,90054)  THEN Importe Else 0 End as PeajeSalidaRedTransporteTerminoFijoCapacidad,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90041,90055)  THEN Importe Else 0 End as PeajeSalidaRedTransporteCaudalDemandado,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90042,90056)  THEN Importe Else 0 End as Otroscostesderegasificacióntérminofijocliente,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90043,90057)  THEN Importe Else 0 End as Otroscostesderegasificacióntérminofijocapacidad,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-4227 (se deberia crear, pero mientras quieren que salga dato..)
	CASE WHEN FacturaConcepto in (90044,90058)  THEN Importe Else 0 End as CargoTerminofijocliente, --Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-3842
	CASE WHEN FacturaConcepto in (90045,90059)  THEN Importe Else 0 End as CargoTerminofijocapacidad,--Añado los de distribuidora en el normal para que les salga porque no tienen la columna de distribuidora creada todavia. JST-3842
	CASE WHEN FacturaConcepto in (90046)  THEN Importe Else 0 End as CuotaDelGTS,
	CASE WHEN FacturaConcepto in (90047)  THEN Importe Else 0 End as TasaCNMC,
	CASE WHEN FacturaConcepto in (90060)  THEN Importe Else 0 End as CuotaDelGTSDistribuidora,
	CASE WHEN FacturaConcepto in (90061)  THEN Importe Else 0 End as TasaCNMCDistribuidora ,
	CASE WHEN FacturaConcepto in (90066) THEN Importe Else 0 end as AlmacenamientoSubterraneo
	from Lineas where Lineas.IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
	group by IdFacturaVentaCabecera, FacturaConcepto, Importe
),
SumaAgrupadaLineas as
(
	SELECT
	Id, sum(ImporteATRVariable) as ImporteATRVariable, sum(ImporteATRFijo) as ImporteATRFijo, sum(Alquiler) as Alquiler, sum(IH) as IH, 
	sum(ImportePG) as ImportePG, sum(Adicionales01) as Adicionales01,  sum(Adicionales02) as Adicionales02,	sum(DescuentosEnergia) as DescuentosEnergia, 
	sum(DescuentosFijo) as DescuentosFijo, sum(Canon) as Canon, sum(OtrosATR) as OtrosATR, sum(OtrosEnergia) as OtrosEnergia,



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


	FROM AgrupadaLineas with (nolock)
	group by id
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
        IdFacturaOrigen, FechaLecturaActualXML,FechaLecturaAnteriorXML,FechaFactura,IdTarifaPeajeXML,IdTarifaXML, SerieNumFactura,IdCliente
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
        f.IdFacturaOrigen, f.FechaLecturaActualXML,f.FechaLecturaAnteriorXML,f.FechaFactura,f.IdTarifaPeajeXML,f.IdTarifaXML,f.SerieNumFactura,f.IdCliente
    FROM FacturaVentaCabecera f
    INNER JOIN FacturaOriginal fo
        ON f.IdFacturaOrigen = fo.IdFacturaVentaCabecera
    WHERE f.IsAbono = 1
)
,ConsultaFinal as(
select FVC.IdFacturaVentaCabecera,fvc.SerieNumFactura
,fvc.CodigoContrato
,cups.codigocups
,tp.textotarifapeaje
,p.IdProvincia
,p.TextoProvincia
,fvc.FechaFactura
,FORMAT(fvc.FechaLecturaActualXML, 'yyyyMM') AS Periodo
,fvc.FechaLecturaAnteriorXML as FechaLecturaAnterior
,fvc.FechaLecturaActualXML as FechaLecturaActual
,replace(cp.Caudal/1000,'.',',') as CaudalContratadoMWh
--,CASE WHEN isnull(ConsumosGas.ConsumoTotal,0) <> 0  THEN replace(ConsumosGas.ConsumoTotal,'.',',')
--        ELSE replace(ConsumosGas2.ConsumoTotal,'.',',')
-- END AS
 
 --,Consumos.Consumo ConsumoTotalGas
 ,case when isnull(fvc.IsAbono,0)=1 then
	Consumos.Consumo*-1
	else
	Consumos.Consumo
	end ConsumoTotalGas


,c.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeReducido)[1]', 'decimal(5,2)') as PcIHReducido
,case when isnull(em.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeExento)[1]', 'float'),0) =0 then
	 	  isnull(c.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeExento)[1]', 'decimal(18,2)'),0)
	   else
	   	isnull(em.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeExento)[1]', 'float'),0)  end
  AS PcIHExento,
c.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeNormal)[1]', 'decimal(5,2)') as PcIHNormal,
c.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeVehicular)[1]', 'decimal(5,2)')as PcIHVehicular,

		isnull(em.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeExento)[1]', 'float'),0) PIexentocontador,
	SumaAgrupadaLineas.ImporteATRFijo ,
	SumaAgrupadaLineas.PeajeSalidaRedLocalTerminofijoCliente ,
	SumaAgrupadaLineas.PeajeSalidaRedLocalTerminofijoCapacidad ,
	SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoFijoCliente ,
	SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoFijoCapacidad ,
	SumaAgrupadaLineas.Otroscostesderegasificacióntérminofijocliente ,
	SumaAgrupadaLineas.Otroscostesderegasificacióntérminofijocapacidad ,
	SumaAgrupadaLineas.CargoTerminofijocliente ,
	SumaAgrupadaLineas.CargoTerminofijocapacidad	,
	--, c.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeExento)[1]', 'decimal(5,2)') as TerminoFijoC,

	SumaAgrupadaLineas.ImportePG ,
	SumaAgrupadaLineas.TerminoVariablePersonalizado ,
	SumaAgrupadaLineas.PeajeSalidaRedLocalTerminoVariableConsumo ,
	SumaAgrupadaLineas.PeajeSalidaRedTransporteTerminoVariableConsmuo , isnull(c.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeExento)[1]', 'decimal(18,6)'),0)/100 as PorcentajeExentoContrato
	,
	case when isnull(fvc.IsAbono,0)=1 then
		 case when ConsumosGas.ConsumoTotal <0 
		 then ConsumosGas.ConsumoTotal
		 else ConsumosGas.ConsumoTotal*-1
		 end
	else
	ConsumosGas.ConsumoTotal
	end ConsumoTotal
	
	,
	case when isnull(fvc.IsAbono,0)=1 then
		case when ConsumosGas2.ConsumoTotal <0
			then ConsumosGas2.ConsumoTotal
			else ConsumosGas2.ConsumoTotal*-1
		 end
	else
	ConsumosGas2.ConsumoTotal 
	end consumototal2

	,l.IdFacturaCompraCabecera

	,case when isnull(fvc.IsAbono,0)=1 then
isnull(equipos.InfoLecturaXML.value('(/LecturaInfoDTO/ConsumoGasKwh)[1]', 'decimal(38,6)'), l.InfoLecturaXML.value('(/LecturaInfoDTO/ConsumoGasKwh)[1]', 'decimal(38,6)'))*-1
else
isnull(equipos.InfoLecturaXML.value('(/LecturaInfoDTO/ConsumoGasKwh)[1]', 'decimal(38,6)'), l.InfoLecturaXML.value('(/LecturaInfoDTO/ConsumoGasKwh)[1]', 'decimal(38,6)'))
end ConsumoGasContador

from FacturaOriginal fvc
inner join FacturasVentaConsulta fvcc on fvc.IdFacturaVentaCabecera = fvcc.idfacturaventacabecera
left join contrato c on c.codigocontrato = fvc.codigocontrato
left join cups on cups.idcups = c.idcups
left join ciudad ci on ci.IdCiudad = cups.IdCiudad
left join Provincia p on p.IdProvincia = ci.IdProvincia
left join cliente cl on cl.idcliente = fvc.idcliente
left join TarifaPeaje tp on tp.IdTarifaPeaje = fvc.IdTarifaPeajeXML
left join ContratoPotencia cp on cp.IdContrato = c.IdContrato
left join Lectura l with(nolock) on  (case when isnull(fvcc.IsAbono,0)=1 then fvcc.idfacturaorigen else fvcc.idfacturaventacabecera end = l.IdFacturaVentaCabeceraSectorC)
LEFT JOIN SumConsumosGas AS ConsumosGas WITH(NOLOCK)  ON ConsumosGas.IdFacturaVentaCabeceraGas = fvc.IdFacturaVentaCabecera 
LEFT JOIN SumConsumosGas2 AS ConsumosGas2 WITH(NOLOCK)  ON ConsumosGas2.IdFacturaVentaCabeceraGas = fvc.IdFacturaVentaCabecera 
left join AgrupadaConsumos  as Consumos on (case when fvc.isabono =1 then fvc.IdFacturaOrigen else fvc.IdFacturaVentaCabecera end) = Consumos.Id
left join SumaAgrupadaLineas as SumaAgrupadaLineas with (nolock) on fvc.IdFacturaVentaCabecera = SumaAgrupadaLineas.Id
left join (
		select em2.idequipomedida, lc.codigocontrato, fc.IdFacturaCompraCabecera,lc.InfoLecturaXML from FacturaCompraCabecera fc
		left join lectura lc on fc.idfacturacompracabecera = lc.idfacturacompracabecera and IdFacturaVentaCabeceraSectorC is null
		left join equipomedida em2 on lc.IdEquipoMedida = em2.IdEquipoMedida
		where  isnull(em2.impuestohidrocarburoxml.value('(ImpuestoHidrocarburoPropioGasDTO/PorcentajeExento)[1]', 'float'),0)<>0
	) equipos on l.IdFacturaCompraCabecera = equipos.IdFacturaCompraCabecera
	left join equipomedida em with(nolock) on equipos.idequipomedida = em.idequipomedida

) 

,cs1 as(select distinct 
IdFacturaVentaCabecera, SerieNumFactura, CodigoContrato, codigocups,
textotarifapeaje, IdProvincia, TextoProvincia, FechaFactura, Periodo, FechaLecturaAnterior, FechaLecturaActual, 
CaudalContratadoMWh,

case when isnull(ConsumoTotalGas,0)= 0then case when isnull(ConsumoTotal,0)=0 then consumototal2 else ConsumoTotal end else ConsumoTotalGas end ConsumoTotalGas

, replace(PcIHReducido,'.',',') PcIHReducido, replace(PcIHExento,'.',',')PcIHExento, replace(PcIHNormal,'.',',') PcIHNormal, replace(PcIHVehicular,'.',',')PcIHVehicular, PIexentocontador
,
    ImporteATRFijo ,
	PeajeSalidaRedLocalTerminofijoCliente ,
	PeajeSalidaRedLocalTerminofijoCapacidad ,
	PeajeSalidaRedTransporteTerminoFijoCliente ,
	PeajeSalidaRedTransporteTerminoFijoCapacidad ,
	Otroscostesderegasificacióntérminofijocliente ,
	Otroscostesderegasificacióntérminofijocapacidad ,
	CargoTerminofijocliente ,
	CargoTerminofijocapacidad	,
	ImportePG ,
	TerminoVariablePersonalizado ,
	PeajeSalidaRedLocalTerminoVariableConsumo ,PeajeSalidaRedTransporteTerminoVariableConsmuo,
	 PorcentajeExentoContrato,ConsumoTotal,consumototal2

,ConsumoGasContador
from ConsultaFinal
where (len(ConsumoTotal)<>0 or len(ConsumoTotal2)<>0 )  
--year(FechaLecturaActual) not in ( 2025)
and isnull(ConsumoGasContador,0) <>0
group by 
IdFacturaVentaCabecera, SerieNumFactura, CodigoContrato, codigocups,
textotarifapeaje, IdProvincia, TextoProvincia, FechaFactura, Periodo, FechaLecturaAnterior, FechaLecturaActual, 
CaudalContratadoMWh, ConsumoTotalGas, ImporteATRFijo,
PeajeSalidaRedLocalTerminofijoCliente, PeajeSalidaRedLocalTerminofijoCapacidad, PeajeSalidaRedTransporteTerminoFijoCliente, 
PeajeSalidaRedTransporteTerminoFijoCapacidad, Otroscostesderegasificacióntérminofijocliente, Otroscostesderegasificacióntérminofijocapacidad, 
CargoTerminofijocliente, CargoTerminofijocapacidad
, ImportePG, TerminoVariablePersonalizado, PeajeSalidaRedLocalTerminoVariableConsumo,
PeajeSalidaRedTransporteTerminoVariableConsmuo, PorcentajeExentoContrato, ConsumoTotal,consumototal2,PcIHReducido , PcIHExento, PcIHNormal , PcIHVehicular,PorcentajeExentoContrato, PcIHExento,ConsumoGasContador,PIexentocontador
--,consumototal2,consumototal
)

select 
IdFacturaVentaCabecera, SerieNumFactura, CodigoContrato, codigocups,
textotarifapeaje, IdProvincia, TextoProvincia, FechaFactura, Periodo, FechaLecturaAnterior, FechaLecturaActual, 
CaudalContratadoMWh
,replace(ConsumoTotalGas,'.',',')ConsumoTotalGas_Factura,
replace(sum(ConsumoGasContador) ,'.',',')ConsumoTotalGas_Contador,
replace(CAST(
isnull(sum(ConsumoGasContador),0)/1000 * CAST(replace(PcIHExento,',','.') AS decimal(18,6))   /100 
AS decimal(18,6)) ,'.',',')AS CogeneracionMWh
,PcIHReducido	,PcIHExento,	PcIHNormal, replace(PorcentajeExentoContrato*100,'.',',')PorcentajeExentoContrato


 	,replace((	ImporteATRFijo 
	+PeajeSalidaRedLocalTerminofijoCliente 
	+PeajeSalidaRedLocalTerminofijoCapacidad 
	+PeajeSalidaRedTransporteTerminoFijoCliente 
	+PeajeSalidaRedTransporteTerminoFijoCapacidad 
	+Otroscostesderegasificacióntérminofijocliente 
	+Otroscostesderegasificacióntérminofijocapacidad 
	+CargoTerminofijocliente 
	+CargoTerminofijocapacidad)	*
	case when PorcentajeExentoContrato =0 then PcIHExento/100  else  PorcentajeExentoContrato end,'.',',') as TerminoFijo,

	replace((ImportePG +
	TerminoVariablePersonalizado +
	PeajeSalidaRedLocalTerminoVariableConsumo +
	PeajeSalidaRedTransporteTerminoVariableConsmuo )* case when PorcentajeExentoContrato =0 then PcIHExento/100  else  PorcentajeExentoContrato end ,'.',',') as TerminoVariable

from cs1
group by 
IdFacturaVentaCabecera, SerieNumFactura, CodigoContrato, codigocups,
textotarifapeaje, IdProvincia, TextoProvincia, FechaFactura, Periodo, FechaLecturaAnterior, FechaLecturaActual, 
CaudalContratadoMWh, ConsumoTotalGas, ImporteATRFijo,
PeajeSalidaRedLocalTerminofijoCliente, PeajeSalidaRedLocalTerminofijoCapacidad, PeajeSalidaRedTransporteTerminoFijoCliente, 
PeajeSalidaRedTransporteTerminoFijoCapacidad, Otroscostesderegasificacióntérminofijocliente, Otroscostesderegasificacióntérminofijocapacidad, 
CargoTerminofijocliente, CargoTerminofijocapacidad
, ImportePG, TerminoVariablePersonalizado, PeajeSalidaRedLocalTerminoVariableConsumo,
PeajeSalidaRedTransporteTerminoVariableConsmuo, PorcentajeExentoContrato, ConsumoTotal,consumototal2,PcIHReducido , PcIHExento, PcIHNormal , PcIHVehicular,PorcentajeExentoContrato, PcIHExento

order by SerieNumFactura