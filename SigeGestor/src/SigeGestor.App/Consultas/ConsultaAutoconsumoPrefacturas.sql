-- Se ejecuta TAL CUAL: las fechas van escritas aqui dentro y no las pone el usuario.
-- Esta declarada con pideFechas:=False en CatalogoOperaciones.
--
-- Si algun dia hay que poder cambiarlas desde la aplicacion, se sustituyen los valores de
-- @DesdeFechaFactura y @HastaFechaFactura por 'DesdeFechaReplace' y 'hastaFechaReplace',
-- se le quita el pideFechas:=False y se le pasa el FormatoCompacto: el SP las espera como
-- 'AAAAMMDD', no en el formato de barras que usan casi todas las demas plantillas.
--DATO, EN EL EXCEL MODIFICAR LA COLUMNA BI EN FORMATO TEXTO, SI NO EL NUMERO DE SERIE DE LAS FACS DE COMPRA NO SE PEGA BIEN.
exec [dbo].[Query_ConsultasPreFacturas_AutoConsumos] @DesdeFechaFactura='20250301',@HastaFechaFactura='20250415',@HastaRecepcion=NULL,@DesdeRecepcion=NULL,
													@IdCliente=NULL,@IdCiudad=NULL,@IdCups=NULL,@IdContrato=NULL,@IdAgrupacionContrato=NULL,@IdDistribuidora=NULL,@IsDistribuidoraExterna=NULL,
													@IsComercializadoraExterna=NULL,@IdComercializadora=NULL,@IdRuta=NULL,@CodigoCategoria=N'EN',@IdCNAE=NULL,@IsCalculo=1,@IsCierre=0,
													@IsIncluirNegativos=0,@IdTipoCobro=NULL,@IdProvincia=NULL,@IdAgente=NULL,@IdModeloContrato=NULL,@CodSectorEmpresarial=2,@Entorno=N'E1',
													@EntornoContratoGrupo=N'G1',@EntornoTarifaGrupo=N'G1',@IdClienteCategoria=NULL,@SerieFactura=NULL,@NumeroFactura=NULL,@IdMarca=NULL,
													@MaxFinderItem=500000,@IsGestinel=NULL