select distinct Codigocontrato,Codigocups,
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
left join cups on cups.IdCups = c.idcups