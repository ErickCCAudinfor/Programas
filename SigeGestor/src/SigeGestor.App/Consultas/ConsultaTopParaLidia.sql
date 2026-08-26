--Erick _Gestor Sige_ ConsultaTop
with XMLNAMESPACES('http://localhost/elegibilidad' as "XS") 

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
,FacturasEnergiaML.IdFacturaVentaCabecera