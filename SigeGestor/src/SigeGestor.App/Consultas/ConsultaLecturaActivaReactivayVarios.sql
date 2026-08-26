with Fo as (
select IdFacturaVentaCabecera,SerieFactura,NumeroFactura from FacturaVentaCabecera f
)

select 
f.idfacturaventacabecera
,cl.RazonSocial
,cl.Identidad
,cps.CodigoCUPS
,f.CodigoContrato
,t.TextoTarifa
,tg.TextoTarifaGrupo
,CONCAT(f.SerieFactura, ' ',f.NumeroFactura) NFactura
,datepart(YYYY,f.FechaFactura) Anio
,datepart(MONTH,f.FechaFactura) Mes
,cast(f.FechaFactura as date)  FechaFactura
,f.IdFacturaOrigen
,cast(l.FechaLecturaAnterior as date) FechaLecturaAnterior
,cast(l.FechaLectura as date) FechaLecturaas
,cast(fc.FechaRecepcion as date) FechaRecepcionCompra
,fc.NumeroFactura NumeroFacturaCompra
,tpl.TextoTarifaPeajePeriodoLectura
,replace(ll.ActivaAnterior,'.',',') ActivaAnterior
,replace(ll.ActivaActual,'.',',') ActivaActual
,replace(ll.ActivaExtra,'.',',') ActivaExtra
,replace(ll.ConsumoActiva,'.',',') ConsumoActiva
,replace(ll.ReactivaAnterior,'.',',') ReactivaAnterior
,replace(ll.ReactivaActual,'.',',') ReactivaActual
,replace(ll.ReactivaExtra,'.',',') ReactivaExtra
,replace(ll.ConsumoReactiva,'.',',') ConsumoReactiva
,replace(ll.Maximetro,'.',',') Maximetro
,replace(ll.MaximetroExceso,'.',',') MaximetroExceso
from facturaventacabecera f
left join Lectura l on l.IdFacturaVentaCabeceraSectorC = 
	case 
		when f.IsAbono = 1 
			then f.IdFacturaOrigen
		else f.IdFacturaVentaCabecera
	End

left join LecturaLinea ll on l.IdLectura = ll.IdLectura
left join Cliente cl on f.IdCliente = cl.IdCliente
left join contrato c on f.codigocontrato = c.codigocontrato

inner join CUPS cps on c.IdCups = cps.IdCups
left join Tarifa t on f.IdTarifaPeajeXML = t.IdTarifa
left join TarifaGrupo tg on f.IdTarifaGrupoXML = tg.IdTarifaGrupo
left join TarifaPeajePeriodoLectura tpl on ll.IdTarifaPeajePeriodoLectura = tpl.IdTarifaPeajePeriodoLectura
left join FacturaCompraCabecera fc on l.IdFacturaCompraCabecera= fc.IdFacturaCompraCabecera
 where f.serienumfactura in  (ListaFacturasParam)
 order by  NFactura
