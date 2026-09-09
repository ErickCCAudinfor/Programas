--Erick _Gestor Sige_ FacturasCSVVarios
--
-- Portada LITERAL de PrepararQuery de ActualizaPrecios (ValidacionExcel.vb:435). Son 127 lineas
-- de CTEs sobre potencias contratadas por periodo y no es algo que convenga rehacer de memoria.
--
-- DOS MARCADORES:
--   idsReplace       la lista de IdFacturaVentaCabecera, separados por comas
--   condicionReplace como se une FacturaVentaCabecera con el resto
--
-- La consulta se lanza DOS VECES con condiciones distintas, y esto no es un descuido: las
-- facturas que tienen CodigoContrato se unen por contrato y las que no, por cliente. El
-- ejecutable las junta despues.

--Facturas VA SUEZ -- ErickCC
With 
FacturasVentaConsulta (idfacturaventacabecera,codigocontrato) as
(Select idfacturaventacabecera,Codigocontrato from FacturaVentaCabecera with(nolock) 

where IdFacturaVentaCabecera in (idsReplace
)),

 PotenciaReemplazadaP1 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp1.PotenciaContratada, '.', ',') AS PotContratadaP1
    FROM contrato c
left join ContratoPotencia cp1 with (nolock) on cp1.idcontrato = c.idcontrato and cp1.IdTarifaPeriodo in (20202001,20203001,20206101,20206201,20206301,20206401,20208001,20208101)

)

, PotenciaReemplazadaP2 AS (
    SELECT 
		c.idcontrato,
        REPLACE(cp2.PotenciaContratada, '.', ',') AS PotContratadaP2

    FROM contrato c
left join ContratoPotencia cp2 with (nolock) on cp2.idcontrato = c.idcontrato and cp2.IdTarifaPeriodo in (20202002,20203002,20206102,20206202,20206302,20206402,20208002,20208102)

)
, PotenciaReemplazadaP3 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp3.PotenciaContratada, '.', ',') AS PotContratadaP3
    FROM contrato c
left join ContratoPotencia cp3 with (nolock) on cp3.idcontrato = c.idcontrato and cp3.IdTarifaPeriodo in (20202003,20203003,20206103,20206203,20206303,20206403,20208003,20208103)
)
, PotenciaReemplazadaP4 AS (
    SELECT 
        c.idcontrato,

        REPLACE(cp4.PotenciaContratada, '.', ',') AS PotContratadaP4
    FROM contrato c
left join ContratoPotencia cp4 with (nolock) on cp4.idcontrato = c.idcontrato and cp4.IdTarifaPeriodo in (20203004,20206104,20206204,20206304,20206404,20208004,20208104)

)
, PotenciaReemplazadaP5 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp5.PotenciaContratada, '.', ',') AS PotContratadaP5
    FROM contrato c
left join ContratoPotencia cp5 with (nolock) on cp5.idcontrato = c.idcontrato and cp5.IdTarifaPeriodo in (20203005,20206105,20206205,20206305,20206405,20208005,20208105)

)
, PotenciaReemplazadaP6 AS (
    SELECT 
        c.idcontrato,
        REPLACE(cp6.PotenciaContratada, '.', ',') AS PotContratadaP6
    FROM contrato c
left join ContratoPotencia cp6 with (nolock) on cp6.idcontrato = c.idcontrato and cp6.IdTarifaPeriodo in (20203006,20206106,20206206,20206306,20206406,20208006,20208106)
)

,ProductoClick as (
select IdFacturaVentaCabecera, sum(ImporteBase) ImporteClick from facturaventalinea where
Descripcion = 'Ajuste término productos click'
group by IdFacturaVentaCabecera
)
,ProductoElectrico as (
select IdFacturaVentaCabecera, sum(ImporteBase)  ImporteElectrico from facturaventalinea where
Descripcion = 'Impuesto Electricidad'
group by IdFacturaVentaCabecera
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
, case when c.Entorno ='E1' then 'Electricidad' else 'Gas' end Sector
, pr.PotContratadaP1
, pr2.PotContratadaP2
, pr3.PotContratadaP3
, pr4.PotContratadaP4
, pr5.PotContratadaP5
, pr6.PotContratadaP6
,t.textotarifa as Tarifa
,d.NombreFiscal as Distribuidora
,fvc.FechaFactura
,replace(fvt.ImporteTotal,'.',',') As ImporteTotal
,tip.textotipocobro
,fvc.SerieFactura+' '+cast (fvc.NumeroFactura as varchar) As NumeroFactura
,fvc.idfacturaorigen
,convert(varchar, fvc.FechaLecturaAnteriorXML, 103) as FechaDesde
,convert(varchar,fvc.FechaLecturaActualXML, 103) as FechaHasta
,replace(ProductoElectrico.ImporteElectrico,'.',',') ImporteElectrico
,replace(ProductoClick.ImporteClick,'.',',') ImporteClick
,fvt.PorcentajeImpuesto As PorcentajeIVA
,replace(fvt.ImporteBase,'.',',') As BaseIVA
,replace(fvt.ImporteImpuesto,'.',',') As ImporteIVA


from  contrato c with (nolock)
inner join cliente cl with (nolock) on cl.idcliente = c.idcliente
inner join CUPS with (nolock) on c.idcups = cups.idcups
inner join callejero cll with (nolock) on cll.IdCallejero = cups.IdCallejero
inner join Ciudad ciu with (nolock) on ciu.idciudad = cups.idciudad
inner join provincia pv with (nolock) on ciu.idprovincia=pv.idprovincia
LEFT JOIN PotenciaReemplazadaP1 pr  ON pr.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP2 pr2 ON pr2.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP3 pr3 ON pr3.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP4 pr4 ON pr4.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP5 pr5 ON pr5.idcontrato = c.idcontrato
LEFT JOIN PotenciaReemplazadaP6 pr6 ON pr6.idcontrato = c.idcontrato
LEFT JOIN FacturaVentaCabecera fvc WITH (NOLOCK) ON condicionReplace
left join FacturaVentaTotal fvt with (nolock) on fvc.idfacturaventacabecera = fvt.idfacturaventacabecera
left join ProductoClick on fvc.IdFacturaVentaCabecera = ProductoClick.IdFacturaVentaCabecera
left join ProductoElectrico on fvc.IdFacturaVentaCabecera = ProductoElectrico.IdFacturaVentaCabecera
left join CarteraCobro cc with (nolock) on fvc.idfacturaventacabecera = cc.idfacturaventacabecera
left join clientepago clp on c.idclientepago = clp.idclientepago
left join tipocobro tip on clp.idtipocobro = tip.idtipocobro
left join Tarifa t with (nolock) on c.idtarifa = t.IdTarifa
left join Distribuidora d with (nolock) on CUPS.iddistribuidora = d.iddistribuidora
where fvc.idfacturaventacabecera in (select IdFacturaVentaCabecera from FacturasVentaConsulta)
