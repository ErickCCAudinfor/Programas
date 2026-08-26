select fc.idfacturacompracabecera, idlectura,Perfilar,numerofactura from FacturaCompraCabecera fc
left join lectura l on fc.IdFacturaCompraCabecera = l.IdFacturaCompraCabecera
where numerofactura in (facturasatrBDReplace)
and facturar=1 and Vigente=1