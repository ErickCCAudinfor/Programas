select c.codigocontrato
,cups.CodigoCUPS
,c.Confirmado
,convert(varchar,c.FechaCreacion, 103) as FechaCreacion
,convert(varchar,c.FechaPrevistaActivacion, 103) as FechaPrevistaActivacion
,stf.TextoFechaEfecto
,c.observaciones 
,ss.Nombre SituacionSolicitud
,s.FechaApertura FechaAperturaSolicitud
from contrato c 
inner join cups on cups.IdCups = c.idcups
left join SolicitudTipoFechaEfecto stf on stf.IdSolicitudTipoFechaEfecto = c.IdSolicitudTipoFechaEfecto

inner join Solicitud s on s.CodigoContrato = c.CodigoContrato and s.IdUsuario in (1610,
1611,5900,5901,5902,5903,5904,5905,5906,5907,5908,5909,5910,5911,5884,5885,5886,5887,5888,5889,5890,5891,5892,5893,5894,5895,5896,5897,5898,5899) 
left join SolicitudSituacion ss on  s.IdSolicitudSituacion = ss.IdSolicitudSituacion
where c.idcontratosituacion=4 order by s.FechaApertura desc