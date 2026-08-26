--Erick _Gestor Sige_ Consulta CAM v8
With XMLNAMESPACES('http://localhost/elegibilidad' as "XS") 
,FacturasVentaConsulta (idfacturaventacabecera,idcontrato,codigocontrato) as
(Select FacturaVentaCabecera.idfacturaventacabecera,idcontrato,Codigocontrato from FacturaVentaCabecera with(nolock) 
Where Entorno='E1' and IdTarifaGrupoXML in (select idtarifagrupo from TarifaGrupo where TextoTarifaGrupo like '%PP CA%Madrid%' ) 
and FechaFactura>='DesdeFechaReplace' and FechaFactura<='hastaFechaReplace' and SerieFactura is not null
)
,PrecioOmie as (
SELECT    IdFacturaVentaCabecera,    AVG(Precio) AS PrecioMedioFinal
FROM (    SELECT        fvc.IdFacturaVentaCabecera,Cd.Fecha,AVG(pm.Precio) AS Precio
    FROM contrato c with (nolock)
        LEFT JOIN CalendarioDetalle CD WITH (NOLOCK) ON CD.IdCalendario = c.IdCalendarioTipo      
        LEFT JOIN PrecioMercado PM WITH (NOLOCK) ON CD.Fecha = PM.FECHA AND CD.Hora = PM.Hora AND PM.IdPoolConceptoSistema IN (7)
        inner join FacturasVentaConsulta with (nolock) on FacturasVentaConsulta.codigocontrato = c.CodigoContrato
        INNER JOIN FacturaVentaCabecera fvc WITH (NOLOCK) ON fvc.IdFacturaVentaCabecera =FacturasVentaConsulta.idfacturaventacabecera
    WHERE Cd.IdCalendario = c.idcalendariotipo
        AND Cd.IdTarifaPeaje = c.idtarifapeaje
        AND CD.Fecha >= InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaInicioFactura)[1]', 'date')
        AND CD.Fecha <= InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaFinalFactura)[1]', 'date')
    GROUP BY CD.Fecha,fvc.IdFacturaVentaCabecera
) AS Subconsulta
GROUP BY IdFacturaVentaCabecera
)
,LineasFactura as (
select Facturaventalinea.IdFacturaVentaCabecera,IdFacturaVentaLinea,Entorno,FacturaConcepto,InfoLineaXML,ImporteBase,IsAjusteCAPGas,TotConsumoEnergiaXML,CodigoPeriodoXML,Descripcion
from FacturaVentaLinea with (nolock)
where IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta with (nolock)))
--inner join FacturasVentaConsulta on FacturasVentaConsulta.idfacturaventacabecera = FacturaVentaLinea.IdFacturaVentaCabecera)

--,ConsumosReactiva(id, r1,r2,r3,r4,r5,r6,pr1,pr2,pr3,pr4,pr5,pr6)
--AS
--(
--	select facturaventacabecera.IdFacturaventaCabecera,
--	ConsumoRP1.Consumo as ConsumoP1, ConsumoRP2.Consumo as ConsumoP2, ConsumoRP3.Consumo as ConsumoP3, ConsumoRP4.Consumo as ConsumoP4, ConsumoRP5.Consumo as ConsumoP5, ConsumoRP6.Consumo as ConsumoP6, 
--	ConsumoRP1.Precio as EnergiaPrecioP1, ConsumoRP2.Precio as EnergiaPrecioP2, ConsumoRP3.Precio as EnergiaPrecioP3, ConsumoRP4.Precio as EnergiaPrecioP4, ConsumoRP5.Precio as EnergiaPrecioP5, ConsumoRP6.Precio as EnergiaPrecioP6	
--	from facturaventacabecera WITH (NOLOCK)
--	--Consumos reactiva
--		left join 		
--		(Select Id, Consumo, Max(Precio) as Precio from(
--		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
--		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
--		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
--		where  FacturaConcepto like '40%')  as l where CodPeriodo=1 group by l.Id, Consumo) as ConsumoRP1 
--		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP1.Id
--		left join 		
--		(Select Id, Consumo, Max(Precio) as Precio from(
--		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
--		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
--		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
--		where  FacturaConcepto like '40%')  as l where CodPeriodo=2 group by l.Id, Consumo) as ConsumoRP2 
--		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP2.Id
--		left join 		
--		(Select Id, Consumo, Max(Precio) as Precio from(
--		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
--		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
--		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
--		where  FacturaConcepto like '40%')  as l where CodPeriodo=3 group by l.Id, Consumo) as ConsumoRP3 
--		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP3.Id
--		left join 		
--		(Select Id, Consumo, Max(Precio) as Precio from(
--		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
--		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
--		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
--		where  FacturaConcepto like '40%')  as l where CodPeriodo=4 group by l.Id, Consumo) as ConsumoRP4 
--		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP4.Id
--		left join 		
--		(Select Id, Consumo, Max(Precio) as Precio from(
--		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
--		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
--		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
--		where  FacturaConcepto like '40%')  as l where CodPeriodo=5 group by l.Id, Consumo) as ConsumoRP5 
--		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP5.Id
--		left join 		
--		(Select Id, Consumo, Max(Precio) as Precio from(
--		SELECT LineasFactura.IdFacturaVentaCabecera as Id, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/TotConsumo)[1]', 'decimal(18,3)'), 0) AS Consumo, 
--		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/ConceptoReactiva/PrecioMedio)[1]', 'decimal(18,6)'), 0) AS Precio,
--		LineasFactura.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo FROM   LineasFactura WITH (NOLOCK)
--		where  FacturaConcepto like '40%')  as l where CodPeriodo=6 group by l.Id, Consumo) as ConsumoRP6 
--		on facturaventacabecera.IdFacturaVentaCabecera = ConsumoRP6.Id
--	WHERE facturaventacabecera.IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta))
,
Lineas as (
select FacturasVentaConsulta.IdFacturaVentaCabecera, FacturaConcepto, ImporteBase,descripcion  from FacturasVentaConsulta with (nolock)
inner join LineasFactura with (nolock) on LineasFactura.IdFacturaVentaCabecera = FacturasVentaConsulta.IdFacturaVentaCabecera

)
,
ImportesPotencia as (
select Lineas.IdFacturaVentaCabecera, sum(Lineas.ImporteBase) as Importe from Lineas with (nolock)
where (FacturaConcepto between 10000 and 19999 or FacturaConcepto in (130001,130002, 131001)) or 
(FacturaConcepto in (90003, 90018, 90035,90036,90039,90040, 90042, 90043, 90044, 90045, 90049, 90050, 90053, 90054, 90056, 90057, 90058, 90059, 90070))
group by Lineas.IdFacturaVentaCabecera
),
ImportesEnergia as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with (nolock)
where (FacturaConcepto between 30000 and 39999 or FacturaConcepto in (130003,130004, 131003)) or 
(FacturaConcepto in (90001,90002,90012,90031,90032,90062,90038,90048,90052))
group by Lineas.IdFacturaVentaCabecera
),
ImportesReactiva as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with (nolock)
where FacturaConcepto between 40000 and 49999 
group by Lineas.IdFacturaVentaCabecera
),
ImportesExcesos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe from Lineas with (nolock) 
where (FacturaConcepto between 20000 and 29999) or (FacturaConcepto in (90037,90041,90051,90055))
group by Lineas.IdFacturaVentaCabecera
),
ImportesProductos as (
Select Lineas.IdFacturaVentacabecera, sum(Lineas.ImporteBase) as Importe,STRING_AGG(descripcion+' Importe:'+CAST(Lineas.ImporteBase AS VARCHAR(50))+' ', '; ') as Descripcion from Lineas with (nolock)
where FacturaConcepto in (100001, 100002, 110001, 90009)
group by Lineas.IdFacturaVentaCabecera
), LineasLectura as (
select ll.idlectura,ll.IdTarifaPeajePeriodoLectura,sum(ll.maximetro) as maximetro,sum(ll.ConsumoActiva) as ConsumoActiva
,sum(ll.ConsumoReactiva) as ConsumoReactiva,sum(ll.ActivaExtra) as ActivaExtra from LecturaLinea ll with (nolock)
inner join Lectura l with (nolock) on l.IdLectura = ll.IdLectura
where l.IdFacturaVentaCabeceraSectorC in (select idfacturaventacabecera from FacturasVentaConsulta) group by ll.idlectura,ll.IdTarifaPeajePeriodoLectura
)


Select distinct
cl.Identidad as NIF
,dbo.formateardenominacion(cl.nombre,cl.apellido1,cl.apellido2,cl.razonsocial) as Razon_Social
,c.codigocontrato as NIS
,cll.NombreCalle +' '+ cups.Aclarador as Direccion_NIS
,CUPS.CodigoCUPS as CUPS
,em.NumeroSerie as N_Contador
,t.textotarifa as Tarifa
,convert(varchar,fvc.FechaFactura, 103) as Fecha_Factura
,fvc.SerieFactura+' '+cast (fvc.NumeroFactura as varchar) As N_Factura
,ft.TextoFacturaTipo as TipoFactura
,case  when fvc.idfacturaorigen is not  null 
THEN (select fv.SerieFactura+''+cast (fv.NumeroFactura as varchar) from FacturaVentaCabecera fv where fv.IdFacturaVentaCabecera=fvc.idfacturaorigen)
Else ' '
End as N_Factura_Corregida
,ISNULL(CONVERT(VARCHAR,fvc.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaInicioFactura)[1]', 'datetimeoffset'), 103), ' ') as Fecha_Desde
,ISNULL(CONVERT(VARCHAR,fvc.InfocabeceraXML.value('(FacturaInfoCabeceraDTO/FechaFinalFactura)[1]', 'datetimeoffset'), 103), ' ') as Fecha_Hasta
,replace(CAST(po.PrecioMedioFinal AS DECIMAL(18,2)),'.',',')+'€' as OMIE
,replace(fvt.ImporteTotal,'.',',')+'€' As ImporteTotal
,replace(fvt.ImporteBase,'.',',')+'€' As Base_IVA
,replace(fvt.ImporteImpuesto,'.',',')+'€' As IVA
,replace(ISNULL(fvlIE.InfoLineaXML.value('(//ConceptoImpuesto/ImporteBaseImpuesto)[1]', 'decimal(18,6)'),0),'.',',')+'€' AS BASE_IE
,replace(fvlIE.ImporteBase,'.',',')+'€' As Impuesto_Electrico
,replace(isnull(ImportesPotencia.Importe, 0),'.',',')+'€' as Importe_TP
,replace(isnull(ImportesEnergia.Importe, 0),'.',',')+'€' as Importe_TE
,replace(fvlCON.ImporteBase,'.',',')+'€' As Importe_EM
,replace(isnull(ImportesReactiva.Importe, 0),'.',',')+'€' as Importe_Reactiva
,replace(isnull(ImportesExcesos.Importe, 0),'.',',')+'€' as Importe_Exceso_Potencia
,'' as Importe_Bono_Social
,replace(fvlCAP.importebase,'.',',')+'€' as Importe_Coste_Tope_Gas
,replace(cp1.PotenciaContratada,'.',',') as Potencia_Contratada_1
,replace(cp2.PotenciaContratada,'.',',') as Potencia_Contratada_2
,replace(cp3.PotenciaContratada,'.',',') as Potencia_Contratada_3
,replace(cp4.PotenciaContratada,'.',',') as Potencia_Contratada_4
,replace(cp5.PotenciaContratada,'.',',') as Potencia_Contratada_5
,replace(cp6.PotenciaContratada,'.',',') as Potencia_Contratada_6
,replace((ISNULL(ll1.ConsumoActiva,0.0) + ISNULL(ll1.ActivaExtra,0.0)),'.',',') as Consumo_Activa_1
,replace((ISNULL(ll2.ConsumoActiva,0.0) + ISNULL(ll2.ActivaExtra,0.0)),'.',',') as Consumo_Activa_2
,replace((ISNULL(ll3.ConsumoActiva,0.0) + ISNULL(ll3.ActivaExtra,0.0)),'.',',') as Consumo_Activa_3
,replace((ISNULL(ll4.ConsumoActiva,0.0) + ISNULL(ll4.ActivaExtra,0.0)),'.',',') as Consumo_Activa_4
,replace((ISNULL(ll5.ConsumoActiva,0.0) + ISNULL(ll5.ActivaExtra,0.0)),'.',',') as Consumo_Activa_5
,replace((ISNULL(ll6.ConsumoActiva,0.0) + ISNULL(ll6.ActivaExtra,0.0)),'.',',') as Consumo_Activa_6
,Replace(ISNULL(fvc.InfoCabeceraXML.value('(FacturaInfoCabeceraDTO/FacturaInfoCabeceraHistorialConsumos/PeriodoConsumo/FacturaInfoCabeceraHistorialPeriodoConsumoDTO/ConsumoActiva)[1]', 'decimal(18,3)'), 0),'.',',') as Consumo_Activa_Total
--,replace(cr.r1,'.',',') as Consumo_Reactiva_1
--,replace(cr.r2,'.',',') as Consumo_Reactiva_2
--,replace(cr.r3,'.',',') as Consumo_Reactiva_3
--,replace(cr.r4,'.',',') as Consumo_Reactiva_4
--,replace(cr.r5,'.',',') as Consumo_Reactiva_5
--,replace(cr.r6,'.',',') as Consumo_Reactiva_6
--,replace((ISNULL(cr.r1,0.0)+ ISNULL(cr.r2,0.0)+ ISNULL(cr.r3,0.0)+ ISNULL(cr.r4,0.0)+ ISNULL(cr.r5,0.0)+ ISNULL(cr.r6,0.0)),'.',',') as Consumo_Reactiva_Total
,replace(ll1.ConsumoReactiva,'.',',') as 'Consumo_Reactiva_1'
,replace(ll2.ConsumoReactiva,'.',',') as 'Consumo_Reactiva_2'
,replace(ll3.ConsumoReactiva,'.',',') as 'Consumo_Reactiva_3'
,replace(ll4.ConsumoReactiva,'.',',') as 'Consumo_Reactiva_4'
,replace(ll5.ConsumoReactiva,'.',',') as 'Consumo_Reactiva_5'
,replace(ll6.ConsumoReactiva,'.',',') as 'Consumo_Reactiva_6' 
,convert(float,isnull(ll1.ConsumoReactiva,0))+convert(float,isnull(ll2.ConsumoReactiva,0))+convert(float,isnull(ll3.ConsumoReactiva,0))+convert(float,isnull(ll4.ConsumoReactiva,0))+convert(float,isnull(ll5.ConsumoReactiva,0))+convert(float,isnull(ll6.ConsumoReactiva,0)) As 'Consumo_Reactiva_Total'																																						
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:TerminoEnergiaReactiva/XS:Periodo/XS:ValorEnergiaReactiva)[1]', 'nvarchar(max)'),0),'.',',') as Exceso_Reactiva1
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:TerminoEnergiaReactiva/XS:Periodo/XS:ValorEnergiaReactiva)[2]', 'nvarchar(max)'),0),'.',',') as Exceso_Reactiva2
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:TerminoEnergiaReactiva/XS:Periodo/XS:ValorEnergiaReactiva)[3]', 'nvarchar(max)'),0),'.',',') as Exceso_Reactiva3
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:TerminoEnergiaReactiva/XS:Periodo/XS:ValorEnergiaReactiva)[4]', 'nvarchar(max)'),0),'.',',') as Exceso_Reactiva4
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:TerminoEnergiaReactiva/XS:Periodo/XS:ValorEnergiaReactiva)[5]', 'nvarchar(max)'),0),'.',',') as Exceso_Reactiva5
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:TerminoEnergiaReactiva/XS:Periodo/XS:ValorEnergiaReactiva)[6]', 'nvarchar(max)'),0),'.',',') as Exceso_Reactiva6
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:Autoconsumo/XS:EnergiaExcedentaria/XS:ValorTotalEnergiaExcedentaria)[1]', 'nvarchar(max)'),0),'.',',') as Energia_Vertida
,replace(ll1.maximetro,'.',',') as Potencia_Maxima1
,replace(ll2.maximetro,'.',',') as Potencia_Maxima2
,replace(ll3.maximetro,'.',',') as Potencia_Maxima3
,replace(ll4.maximetro,'.',',') as Potencia_Maxima4
,replace(ll5.maximetro,'.',',') as Potencia_Maxima5
,replace(ll6.maximetro,'.',',') as Potencia_Maxima6
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:ExcesoPotencia/XS:Periodo/XS:ValorExcesoPotencia)[1]', 'nvarchar(max)'),0),'.',',') as Ac1
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:ExcesoPotencia/XS:Periodo/XS:ValorExcesoPotencia)[2]', 'nvarchar(max)'),0),'.',',') as Ac2
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:ExcesoPotencia/XS:Periodo/XS:ValorExcesoPotencia)[3]', 'nvarchar(max)'),0),'.',',') as Ac3
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:ExcesoPotencia/XS:Periodo/XS:ValorExcesoPotencia)[4]', 'nvarchar(max)'),0),'.',',') as Ac4
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:ExcesoPotencia/XS:Periodo/XS:ValorExcesoPotencia)[5]', 'nvarchar(max)'),0),'.',',') as Ac5
,Replace(ISNULL(fcc.FacturaXML.value('(//XS:ExcesoPotencia/XS:Periodo/XS:ValorExcesoPotencia)[6]', 'nvarchar(max)'),0),'.',',') as Ac6
,d.NombreFiscal as Distribuidora
,'' as Cambio_Precio
,replace(ISNULL(fvlP1.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') as Precio_TE1
,replace(ISNULL(fvlP2.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') as Precio_TE2
,replace(ISNULL(fvlP3.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') as Precio_TE3
,replace(ISNULL(fvlP4.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') as Precio_TE4
,replace(ISNULL(fvlP5.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') as Precio_TE5
,replace(ISNULL(fvlP6.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0),'.',',') as Precio_TE6
,replace(tpp1b.PotenciaPrecio,'.',',') as Precio_TP1
,replace(tpp2b.PotenciaPrecio,'.',',') as Precio_TP2
,replace(tpp3b.PotenciaPrecio,'.',',') as Precio_TP3
,replace(tpp4b.PotenciaPrecio,'.',',') as Precio_TP4
,replace(tpp5b.PotenciaPrecio,'.',',') as Precio_TP5
,replace(tpp6b.PotenciaPrecio,'.',',') as Precio_TP6
,CASE 
   WHEN fvlAutoConsumo.TotconsumoEnergiaxml <> 0 
   THEN Replace(CAST(fvlAutoConsumo.Importebase / fvlAutoConsumo.TotconsumoEnergiaxml AS DECIMAL(18,6)),'.',',')
   ELSE NULL -- O cualquier otro valor predeterminado que desees asignar
END AS Precio_E_Vertida
,replace(isnull(ImportesProductos.Importe, 0),'.',',')+'€' as Imp_Otros_Conceptos
,ImportesProductos.descripcion as Conceptos
,replace(fvlp1.totconsumoEnergiaxml,'.',',') as ConsumoFacturadoP1
,replace(fvlp2.totconsumoEnergiaxml,'.',',') as ConsumoFacturadoP2
,replace(fvlp3.totconsumoEnergiaxml,'.',',') as ConsumoFacturadoP3
,replace(fvlp4.totconsumoEnergiaxml,'.',',') as ConsumoFacturadoP4
,replace(fvlp5.totconsumoEnergiaxml,'.',',') as ConsumoFacturadoP5
,replace(fvlp6.totconsumoEnergiaxml,'.',',') as ConsumoFacturadoP6
from contrato c with (nolock)
inner join cliente cl with (nolock) on cl.idcliente = c.idcliente
inner join CUPS with (nolock) on c.idcups = cups.idcups
inner join callejero cll with (nolock) on cll.IdCallejero = cups.IdCallejero
left join ContratoPotencia cp1 with (nolock) on cp1.idcontrato = c.idcontrato and cp1.IdTarifaPeriodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join ContratoPotencia cp2 with (nolock) on cp2.idcontrato = c.idcontrato and cp2.IdTarifaPeriodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join ContratoPotencia cp3 with (nolock) on cp3.idcontrato = c.idcontrato and cp3.IdTarifaPeriodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join ContratoPotencia cp4 with (nolock) on cp4.idcontrato = c.idcontrato and cp4.IdTarifaPeriodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join ContratoPotencia cp5 with (nolock) on cp5.idcontrato = c.idcontrato and cp5.IdTarifaPeriodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join ContratoPotencia cp6 with (nolock) on cp6.idcontrato = c.idcontrato and cp6.IdTarifaPeriodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
inner join FacturaVentaCabecera fvc with (nolock) on fvc.codigocontrato = c.codigocontrato
inner join FacturaVentaTotal fvt with (nolock) on fvc.idfacturaventacabecera = fvt.idfacturaventacabecera
inner join Tarifa t with (nolock) on fvc.IdTarifaPeajeXML = t.IdTarifa
inner join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
left join LineasFactura fvlIE with (nolock) on fvc.idfacturaventacabecera = fvlIE.idfacturaventacabecera and fvlIE.Facturaconcepto in (60001,60002)
left join LineasFactura fvlCON with (nolock) on fvc.idfacturaventacabecera = fvlCON.idfacturaventacabecera and fvlCON.Facturaconcepto=50002
inner join lectura l  with (nolock) on fvc.idfacturaventacabecera = l.IdFacturaVentaCabeceraSectorC or (fvc.IdFacturaOrigen = l.IdFacturaVentaCabeceraSectorC and fvc.SerieFactura like '%ABO%')
left join FacturaCompraCabecera fcc with (nolock) on fcc.IdFacturaCompraCabecera = l.IdFacturaCompraCabecera
left join EquipoMedida em with (nolock) on em.IdEquipoMedida = l.IdEquipoMedida
inner join LineasLectura ll1 with (nolock) on l.IdLectura = ll1.IdLectura and ll1.idtarifapeajeperiodolectura in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
inner join LineasLectura ll2 with (nolock) on l.IdLectura = ll2.IdLectura	and ll2.idtarifapeajeperiodolectura in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
inner join LineasLectura ll3 with (nolock) on l.IdLectura = ll3.IdLectura	and ll3.idtarifapeajeperiodolectura in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
inner join LineasLectura ll4 with (nolock) on l.IdLectura = ll4.IdLectura	and ll4.idtarifapeajeperiodolectura in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
inner join LineasLectura ll5 with (nolock) on l.IdLectura = ll5.IdLectura	and ll5.idtarifapeajeperiodolectura in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
inner join LineasLectura ll6 with (nolock) on l.IdLectura = ll6.IdLectura	and ll6.idtarifapeajeperiodolectura in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
left join TarifaPeajePrecio tpp1b with (nolock) on l.IdTarifaPeaje = tpp1b.IdTarifaPeaje and tpp1b.fechafinal is null and tpp1b.idtarifapeajeperiodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)
left join TarifaPeajePrecio tpp2b with (nolock) on l.IdTarifaPeaje = tpp2b.IdTarifaPeaje and tpp2b.fechafinal is null and tpp2b.idtarifapeajeperiodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)
left join TarifaPeajePrecio tpp3b with (nolock) on l.IdTarifaPeaje = tpp3b.IdTarifaPeaje and tpp3b.fechafinal is null and tpp3b.idtarifapeajeperiodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
left join TarifaPeajePrecio tpp4b with (nolock) on l.IdTarifaPeaje = tpp4b.IdTarifaPeaje and tpp4b.fechafinal is null and tpp4b.idtarifapeajeperiodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)
left join TarifaPeajePrecio tpp5b with (nolock) on l.IdTarifaPeaje = tpp5b.IdTarifaPeaje and tpp5b.fechafinal is null and tpp5b.idtarifapeajeperiodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)
left join TarifaPeajePrecio tpp6b with (nolock) on l.IdTarifaPeaje = tpp6b.IdTarifaPeaje and tpp6b.fechafinal is null and tpp6b.idtarifapeajeperiodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
--left join ConsumosReactiva cr with (nolock) ON cr.id = fvc.IdFacturaVentaCabecera	
left join ImportesPotencia with (nolock) on ImportesPotencia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesEnergia with (nolock) on ImportesEnergia.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesReactiva with (nolock) on ImportesReactiva.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesExcesos with (nolock) on ImportesExcesos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera 
left join ImportesProductos with (nolock) on ImportesProductos.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
left join LineasFactura fvlP1 with (nolock) on fvc.idfacturaventacabecera = fvlP1.idfacturaventacabecera and fvlP1.Facturaconcepto=30004 and fvlP1.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=1
left join LineasFactura fvlP2 with (nolock) on fvc.idfacturaventacabecera = fvlP2.idfacturaventacabecera and fvlP2.Facturaconcepto=30004 and fvlP2.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=2
left join LineasFactura fvlP3 with (nolock) on fvc.idfacturaventacabecera = fvlP3.idfacturaventacabecera and fvlP3.Facturaconcepto=30004 and fvlP3.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=3
left join LineasFactura fvlP4 with (nolock) on fvc.idfacturaventacabecera = fvlP4.idfacturaventacabecera and fvlP4.Facturaconcepto=30004 and fvlP4.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=4
left join LineasFactura fvlP5 with (nolock) on fvc.idfacturaventacabecera = fvlP5.idfacturaventacabecera and fvlP5.Facturaconcepto=30004 and fvlP5.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=5
left join LineasFactura fvlP6 with (nolock) on fvc.idfacturaventacabecera = fvlP6.idfacturaventacabecera and fvlP6.Facturaconcepto=30004 and fvlP6.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer')=6
left join LineasFactura fvlCAP with (nolock) on fvc.idfacturaventacabecera = fvlCAP.idfacturaventacabecera and fvlCAP.Facturaconcepto=30004 and fvlCAP.IsAjusteCAPGas=1
left join LineasFactura fvlAutoConsumo with (nolock) on fvc.idfacturaventacabecera = fvlAutoConsumo.idfacturaventacabecera and fvlAutoConsumo.Facturaconcepto in (30008,30009)
left join facturatipo ft with (nolock) on ft.IdFacturaTipo = fvc.IdFacturaTipo
left join PrecioOmie Po with (nolock) on po.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
where fvc.IdFacturaVentaCabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta with (nolock)) and c.CodigoContrato in (select CodigoContrato from FacturasVentaConsulta with (nolock))
--inner join FacturasVentaConsulta with (nolock) on FacturasVentaConsulta.idfacturaventacabecera = fvc.IdFacturaVentaCabecera


