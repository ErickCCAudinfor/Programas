 --Erick _Gestor Sige_ RechazosVeolia
select  distinct 
case c.Entorno 
when 'E1' then 'Electricidad'
Else 'GAS' End as Negocio
,cups.CodigoCUPS
,c.codigocontrato
,t.textotarifa
,cl.Identidad
,dbo.formateardenominacion(cl.nombre,cl.apellido1,cl.apellido2,cl.razonsocial) as Sociedad
,c.fechaalta
,c.fechabaja
,cs.textosituacion
,s.IdSolicitud
,mr.TextoRechazo
,consumoestimado
,c.FechaVto 
from contrato c
inner join cups on cups.IdCups = c.IdCups
inner join cliente cl on cl.IdCliente = c.IdCliente
left join Tarifa t on t.idtarifa = c.IdTarifa
inner join ContratoSituacion cs on cs.IdContratoSituacion = c.IdContratoSituacion
left join Solicitud s on s.CodigoContrato = c.CodigoContrato
left join MotivoRechazo mr on s.IdMotivoRechazo = mr.IdMotivoRechazo
where cl.Identidad in (
'A28233922'
,'A15208408'
,'A58295031'
,'A20071429'
,'V20681623'
,'U09964313')
and c.idcontratosituacion not in  (2,18,19,22,3,9,17,28,29,34,43,48)