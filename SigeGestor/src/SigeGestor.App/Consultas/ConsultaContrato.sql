select c.Entorno,codigocontrato,c.idcontratosituacion, cs.textosituacion, c.SituacionScoring
,cast(FechaAlta as date) fechaalta, cast(FechaVto as date) fechavto
,DiasVencimiento
,Observaciones
,c.IdTipoImpuesto
, TextoImpuesto
, IsAgruparFacturas UnificarFacturas
,RevisionFra Revision
,TextoRevision TextoRevisionFra
, c.IdModeloFactura
,mifactura.DescripcionModeloDeImpresion
,c.IdModeloFacturaVarios
,mivarios.DescripcionModeloDeImpresion
,c.IdCNAE
,CNAE.TextoCNAE
,c.IdModeloContrato
,TipoImprimir
,case
when TipoImprimir ='P' then 'Papel y Email'
when TipoImprimir ='E' then 'Email'
when TipoImprimir ='W' then 'Web'
when TipoImprimir ='Q' then 'Papel'
when TipoImprimir ='R' then 'Recibo'
when TipoImprimir ='F' then 'FACE' end TipoImprimirTexto
,micontrato.DescripcionModeloDeImpresion
,IsRenovacionProcesada
,NoRenovar
,Representante
,c.IdColectivoRep
,Colectivo.TextoColectivo
,IdentificadorRep
,EmailRep
,SMSRep
,c.IdClientePago
,isnull(NombreP,'???')+'/'+isnull(IdentidadPago,'???')+'/'+isnull(colecClientePago.TextoColectivo,'???')+'/'+isnull(TextoTipoCobro,'???')+'/'+isnull(IBAN,'???')+'/'+isnull(TextoBanco,'???') ClientePagoUnificado
,Autoconsumo
,IsAutoconsumoNoCompensable
,ExencionIE
,IsLicitacion
,IdTipoAutoconsumo
from contrato c
inner join contratosituacion cs on c.idcontratosituacion = cs.idcontratosituacion
left join SituacionScoring scg on c.SituacionScoring=scg.Nombre
left join TipoImpuesto ti on c.IdTipoImpuesto = ti.IdTipoImpuesto
left join ModeloDeImpresion mifactura on (c.IdModeloFactura = mifactura.IdModeloDeImpresion )
left join ModeloDeImpresion mivarios on (c.IdModeloFacturaVarios = mivarios.IdModeloDeImpresion)
left join ModeloDeImpresion micontrato on (c.IdModeloContrato = micontrato.IdModeloDeImpresion)
left join CNAE on c.IdCNAE = CNAE.IdCNAE
left join Colectivo on c.IdColectivoRep = Colectivo.IdColectivo
left join ClientePago cp on c.IdClientePago  = cp.IdClientePago
left join TipoCobro tp on cp.IdTipoCobro = tp.IdTipoCobro
left join Colectivo colecClientePago on cp.IdColectivo = colecClientePago.IdColectivo
left join Banco b on cp.IdBanco=b.IdBanco
where codigocontrato in(codCntratojoinReplace)