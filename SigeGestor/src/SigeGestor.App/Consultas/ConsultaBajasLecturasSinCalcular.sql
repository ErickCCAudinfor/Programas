-- Bajas con lecturas sin calcular: lecturas por facturar de contratos ya dados de baja.
--
-- El original acotaba solo por abajo, con c.fechabaja>='01/01/2025' escrito a mano. Aqui va
-- entre las dos fechas del formulario, que es lo que esperan los dos campos que pide la
-- pantalla; poniendo la de hasta en hoy se obtiene lo mismo que daba el original.
select c.codigocontrato,cups.CodigoCUPS,cs.TextoSituacion,c.fechabaja,fcc.FechaRecepcion,l.FechaLecturaAnterior,l.fechalectura,l.Facturar,l.Facturado,l.Vigente,d.nombrefiscal,fcc.NumeroFactura from lectura l
inner join contrato c on c.codigocontrato = l.codigocontrato
left join cups  on cups.IdCups = c.IdCups
left join Distribuidora d on cups.IdDistribuidora = d.IdDistribuidora
inner join ContratoSituacion cs on cs.IdContratoSituacion = c.IdContratoSituacion
left join FacturaCompraCabecera fcc on fcc.IdFacturaCompraCabecera = l.IdFacturaCompraCabecera
where l.FechaLectura>c.FechaBaja and Facturar=1 and Facturado=0 and Vigente=1 and c.fechabaja >= 'DesdeFechaReplace' and c.fechabaja <= 'hastaFechaReplace'
