--Erick _Gestor Sige_ Norauto
	create table #TmpId (LongParameter BIGINT)--FLN: 2023-08-06 Es mucho más rápido en una tabla temporal que en una variable de tabla.

		INSERT INTO #TmpId select idfacturaventacabecera from facturaventacabecera where  facturacategoria='EN' and (fechafactura between 'fechaReplace' and 'fechahastaReplace') and idcliente in(select idcliente from cliente where 
	identidad in (
'CIFReplace'

	) and seriefactura is not null)

	;WITH  FacturasFiltradas as
	(
		select IdFacturaVentaCabecera, f.Entorno, f.SerieFactura, f.NumeroFactura, f.IsFactura, f.IsIncidencia, f.IdCliente, f.IdClientePago, f.IdClienteEnvio, f.IdContrato, f.CodigoContrato
			, f.VersionContrato, f.FechaFactura, IdTipoCobro, f.IBAN, f.FechaVencimiento, f.IdCanal, f.IdFacturaTipo, f.FacturaCategoria, f.IdTipoImpuesto, f.IsFacturaConIE, DescuentoBase, DescuentoTotal
			, f.Imprimida, FechaEmision, FechaEnvio, VersionReport, f.IsCorrecta, f.IsProcesada, f.IsEnlaceContable, f.IdSCSAsientoCabecera, f.IdFacturaAbono, f.IdFacturaRectificativa, f.IdFacturaOrigen
			, f.IdTarifaPeajeXML, f.IdTarifaGrupoXML, f.IdTarifaXML, f.FechaLecturaActualXML, f.FechaLecturaAnteriorXML
			, f.FechaRecepcion
			, f.IdPerfilFacturacionXML as IdPerfilFacturacion
			, Lectura.IdLectura
			, Cliente.RazonSocial
			, Cliente.Identidad
			, contrato.Version
			, cups.CodigoCUPS
			, callejero.NombreCalle + ' ' + ISNULL(convert(varchar(10), cups.Numero),'') as NombreCalle
			, ciudad.TextoCiudad
			, cups.CodPostal
			, Provincia.TextoProvincia
			, f.OrigenMedida
			, tarifapeaje.TextoTarifaPeaje
			, tarifa.TextoTarifa
			, lectura.FechaLecturaAnterior
			, lectura.FechaLectura
			, tarifagrupo.TextoTarifaGrupo
			, distribuidora.NombreFiscal
			, FacturaCompraCabecera.NumeroFactura as NumfacturaATR
		from #TmpId as s 
		inner join FacturaVentaCabecera  as f WITH (NOLOCK)
		on s.LongParameter = f.IdFacturaVentaCabecera
		left join Lectura WITH (NOLOCK) on Lectura.IdFacturaVentaCabeceraSectorC = f.IdFacturaVentaCabecera and Lectura.Facturar = 1
		left join Cliente WITH(NOLOCK) on Cliente.IdCliente = f.IdCliente
		left join contrato WITH(NOLOCK) on contrato.codigocontrato = f.codigocontrato
		left join cups WITH(NOLOCK) on cups.IdCups = contrato.IdCups
		left join callejero with(nolock) on callejero.IdCallejero=cups.IdCallejero
		left join Ciudad with(nolock) on ciudad.idciudad = callejero.IdCiudad
		left join Provincia with(nolock) on Provincia.IdProvincia = ciudad.IdProvincia
		left join TarifaPeaje with(nolock) on tarifapeaje.IdTarifaPeaje=lectura.IdTarifaPeaje
		left join Tarifa with(nolock) on tarifa.IdTarifa=lectura.IdTarifa
		left join tarifagrupo with(nolock) on tarifagrupo.idtarifagrupo = f.IdTarifaGrupoXML
		left join Distribuidora with(nolock) on distribuidora.IdDistribuidora = cups.iddistribuidora
		left join FacturaCompraCabecera with(nolock) on FacturaCompraCabecera.IdFacturaCompraCabecera = lectura.IdFacturaCompraCabecera
	)
	, LecturasSeleccionadas as 
	(
			select LecturaLinea.IdLectura, 
			CASE periodohorario.codigoperiodo WHEN 1 then Maximetro ELSE 0 END as MaximetroP1,
			CASE periodohorario.codigoperiodo WHEN 2 then Maximetro ELSE 0 END as MaximetroP2,
			CASE periodohorario.codigoperiodo WHEN 3 then Maximetro ELSE 0 END as MaximetroP3,
			CASE periodohorario.codigoperiodo WHEN 4 then Maximetro ELSE 0 END as MaximetroP4,
			CASE periodohorario.codigoperiodo WHEN 5 then Maximetro ELSE 0 END as MaximetroP5,
			CASE periodohorario.codigoperiodo WHEN 6 then Maximetro ELSE 0 END as MaximetroP6
			from lecturalinea WITH (NOLOCK) 
			inner join FacturasFiltradas WITH (NOLOCK) on FacturasFiltradas.IdLectura = LecturaLinea.IdLectura
			left join TarifaPeajePeriodoLectura WITH (NOLOCK) on TarifaPeajePeriodoLectura.IdTarifaPeajePeriodoLectura=lecturalinea.IdTarifaPeajePeriodoLectura  
			left join PeriodoHorario WITH (NOLOCK) on TarifaPeajePeriodoLectura.IdPeriodoHorario = PeriodoHorario.IdPeriodoHorario 
	)
	, LecturasAgrupadas as 
	(
	SELECT IdLectura, MAX(MaximetroP1) as MaximetroP1, MAX(MaximetroP2) as MaximetroP2, MAX(MaximetroP3) as MaximetroP3, MAX(MaximetroP4) as MaximetroP4, MAX(MaximetroP5) as MaximetroP5, MAX(MaximetroP6) as MaximetroP6 
	FROM LecturasSeleccionadas WITH (NOLOCK) group by IdLectura
	)
	, ConsumosEnergiaTarifaAcceso as
	(
			SELECT FacturaVentaLinea.IdFacturaVentaCabecera, ISNULL(TotConsumoEnergiaXML, 0) AS Consumo, 
			ISNULL(PrecioMedioEnergiaXML, 0) AS Precio,
			CodigoPeriodoXML as CodPeriodo 
			, ImporteBase
			FROM FacturaVentaLinea WITH (NOLOCK) inner join #TmpId  as FacturasFiltradas  on FacturaVentaLinea.IdFacturaVentaCabecera = FacturasFiltradas.LongParameter
			where (FacturaConcepto = 30001)
	)
	, PeriodoConsumosEnergiaTarifaAcceso as
	(
		select IdFacturaVentaCabecera, ImporteBase,
		CASE codperiodo WHEN 1 then Consumo ELSE 0 END as ConsumoRP1,
		CASE codperiodo WHEN 2 then Consumo ELSE 0 END as ConsumoRP2,
		CASE codperiodo WHEN 3 then Consumo ELSE 0 END as ConsumoRP3,
		CASE codperiodo WHEN 4 then Consumo ELSE 0 END as ConsumoRP4,
		CASE codperiodo WHEN 5 then Consumo ELSE 0 END as ConsumoRP5,
		CASE codperiodo WHEN 6 then Consumo ELSE 0 END as ConsumoRP6,

		CASE codperiodo WHEN 1 then Precio ELSE 0 END as PrecioRP1,
		CASE codperiodo WHEN 2 then Precio ELSE 0 END as PrecioRP2,
		CASE codperiodo WHEN 3 then Precio ELSE 0 END as PrecioRP3,
		CASE codperiodo WHEN 4 then Precio ELSE 0 END as PrecioRP4,
		CASE codperiodo WHEN 5 then Precio ELSE 0 END as PrecioRP5,
		CASE codperiodo WHEN 6 then Precio ELSE 0 END as PrecioRP6,
		codperiodo
		from ConsumosEnergiaTarifaAcceso  WITH (NOLOCK)
	)
	, AgrupadaPeriodoConsumosEnergiaTarifaAcceso as
	(
		SELECT IdFacturaVentaCabecera , SUM(ConsumoRP1) as ConsumoP1,  SUM(ConsumoRP2) as ConsumoP2,  SUM(ConsumoRP3) as ConsumoP3, 
		SUM(ConsumoRP4) as ConsumoP4,  SUM(ConsumoRP5) as ConsumoP5,  SUM(ConsumoRP6) as ConsumoP6
		, SUM(PrecioRP1) as EnergiaPrecioP1, SUM(PrecioRP2) as EnergiaPrecioP2, SUM(PrecioRP3) as EnergiaPrecioP3, SUM(PrecioRP4) as EnergiaPrecioP4,SUM(PrecioRP5) as EnergiaPrecioP5,SUM(PrecioRP6) as EnergiaPrecioP6
		, SUM(ImporteBase) as ImporteTarifaAcceso
		FROM PeriodoConsumosEnergiaTarifaAcceso WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)
	, Consumos as
	(
			SELECT FacturaVentaLinea.IdFacturaVentaCabecera, ISNULL(TotConsumoEnergiaXML, 0) AS Consumo, 
			case FacturaConcepto when 30006 then
			ISNULL(PrecioClick, 0)
			else
			ISNULL(PrecioMedioEnergiaXML, 0) END AS Precio,
			CodigoPeriodoXML as CodPeriodo 
			FROM   FacturaVentaLinea WITH (NOLOCK) inner join #TmpId as FacturasFiltradas on FacturaVentaLinea.IdFacturaVentaCabecera = FacturasFiltradas.LongParameter
			where  (FacturaConcepto between 30000 and 30999) AND (FacturaConcepto <> 30001)
	)
	, PeriodoConsumos as
	(
		select IdFacturaVentaCabecera, 		
		CASE codperiodo WHEN 1 then Consumo ELSE 0 END as ConsumoRP1,
		CASE codperiodo WHEN 2 then Consumo ELSE 0 END as ConsumoRP2,
		CASE codperiodo WHEN 3 then Consumo ELSE 0 END as ConsumoRP3,
		CASE codperiodo WHEN 4 then Consumo ELSE 0 END as ConsumoRP4,
		CASE codperiodo WHEN 5 then Consumo ELSE 0 END as ConsumoRP5,
		CASE codperiodo WHEN 6 then Consumo ELSE 0 END as ConsumoRP6,

		CASE codperiodo WHEN 1 then Precio ELSE 0 END as PrecioRP1,
		CASE codperiodo WHEN 2 then Precio ELSE 0 END as PrecioRP2,
		CASE codperiodo WHEN 3 then Precio ELSE 0 END as PrecioRP3,
		CASE codperiodo WHEN 4 then Precio ELSE 0 END as PrecioRP4,
		CASE codperiodo WHEN 5 then Precio ELSE 0 END as PrecioRP5,
		CASE codperiodo WHEN 6 then Precio ELSE 0 END as PrecioRP6,
		codperiodo
		from Consumos WITH (NOLOCK)
	)
	, AgrupadaPeriodoConsumos as
	(
		SELECT IdFacturaVentaCabecera , SUM(ConsumoRP1) as ConsumoP1,  SUM(ConsumoRP2) as ConsumoP2,  SUM(ConsumoRP3) as ConsumoP3, 
		SUM(ConsumoRP4) as ConsumoP4,  SUM(ConsumoRP5) as ConsumoP5,  SUM(ConsumoRP6) as ConsumoP6
		, SUM(PrecioRP1) as EnergiaPrecioP1, SUM(PrecioRP2) as EnergiaPrecioP2, SUM(PrecioRP3) as EnergiaPrecioP3, SUM(PrecioRP4) as EnergiaPrecioP4,SUM(PrecioRP5) as EnergiaPrecioP5,SUM(PrecioRP6) as EnergiaPrecioP6
		FROM PeriodoConsumos WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)
	, ConsumosReactiva AS
	(
			SELECT fvl.idfacturaventacabecera
			,ISNULL(ll.ConsumoReactiva, 0) as Consumo
			,ISNULL(PrecioMedioReactivaXML, 0) AS Precio
			, CodigoPeriodoXML as CodPeriodo 
			, tppl.TextoTarifaPeajePeriodoLectura as CodPeriodoLectura
			, ImporteBase 
			from LecturaLinea ll
			INNER join Lectura l on l.IdLectura = ll.idlectura
			LEFT join FacturaVentaLinea fvl WITH (NOLOCK)  on fvl.IdFacturaVentaCabecera = l.IdFacturaVentaCabeceraSectorC and (FacturaConcepto between 40000 and 40999)
			INNER join tarifapeajeperiodolectura tppl on tppl.idtarifapeajeperiodolectura = ll.idtarifapeajeperiodolectura
	)
	, PeriodoConsumosReactiva as
	(
		select IdFacturaVentaCabecera, ImporteBase,		
		CASE codperiodolectura WHEN 'P1' then Consumo ELSE 0 END as ConsumoReactivaP1,
		CASE codperiodolectura WHEN 'P2' then Consumo ELSE 0 END as ConsumoReactivaP2,
		CASE codperiodolectura WHEN 'P3' then Consumo ELSE 0 END as ConsumoReactivaP3,
		CASE codperiodolectura WHEN 'P4' then Consumo ELSE 0 END as ConsumoReactivaP4,
		CASE codperiodolectura WHEN 'P5' then Consumo ELSE 0 END as ConsumoReactivaP5,
		CASE codperiodolectura WHEN 'P6' then Consumo ELSE 0 END as ConsumoReactivaP6,

		CASE codperiodo WHEN 1 then Precio ELSE 0 END as PrecioReactivaP1,
		CASE codperiodo WHEN 2 then Precio ELSE 0 END as PrecioReactivaP2,
		CASE codperiodo WHEN 3 then Precio ELSE 0 END as PrecioReactivaP3,
		CASE codperiodo WHEN 4 then Precio ELSE 0 END as PrecioReactivaP4,
		CASE codperiodo WHEN 5 then Precio ELSE 0 END as PrecioReactivaP5,
		CASE codperiodo WHEN 6 then Precio ELSE 0 END as PrecioReactivaP6
		from ConsumosReactiva  WITH (NOLOCK)
	)
	, AgrupadaPeriodoReactiva as
	(
		SELECT IdFacturaVentaCabecera , Max(ConsumoReactivaP1) as ConsumoReactivaP1,  Max(ConsumoReactivaP2) as ConsumoReactivaP2,  Max(ConsumoReactivaP3) as ConsumoReactivaP3, 
		Max(ConsumoReactivaP4) as ConsumoReactivaP4,  Max(ConsumoReactivaP5) as ConsumoReactivaP5,  Max(ConsumoReactivaP6) as ConsumoReactivaP6
		, Max(PrecioReactivaP1) as PrecioReactivaP1, Max(PrecioReactivaP2) as PrecioReactivaP2, Max(PrecioReactivaP3) as PrecioReactivaP3, Max(PrecioReactivaP4) as PrecioReactivaP4
		,Max(PrecioReactivaP5) as PrecioReactivaP5,Max(PrecioReactivaP6) as PrecioReactivaP6
		, SUM(ImporteBase) as ImportePeriodoReactiva
		FROM PeriodoConsumosReactiva WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)
	, Potencia as
	(
		SELECT l.IdFacturaVentaCabecera as IdFacturaVentaCabecera, ISNULL(PotenciaAFacturarXML, 0) AS PotenciaFacturar
		, ISNULL(PotenciaContratadaXML, 0) AS PotenciaContratada,
		l.CodigoPeriodoXML as CodPeriodo
		, ISNULL(PrecioMedioPotenciaXML, 0) AS Precio 
		FROM FacturaVentaLinea as l WITH (NOLOCK)
			inner join FacturasFiltradas as f WITH (NOLOCK)
		on l.IdFacturaVentaCabecera = f.IdFacturaVentaCabecera
		where (FacturaConcepto between 10000 and 10999)
	)
	, PeriodoPotencia as
	(
		select IdFacturaVentaCabecera, 	
		CASE codperiodo WHEN 1 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP1,
		CASE codperiodo WHEN 2 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP2,
		CASE codperiodo WHEN 3 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP3,
		CASE codperiodo WHEN 4 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP4,
		CASE codperiodo WHEN 5 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP5,
		CASE codperiodo WHEN 6 then PotenciaFacturar ELSE 0 END as PotenciaFacturarP6,
	
		CASE codperiodo WHEN 1 then PotenciaContratada ELSE 0 END as PotenciaContratadaP1,
		CASE codperiodo WHEN 2 then PotenciaContratada ELSE 0 END as PotenciaContratadaP2,
		CASE codperiodo WHEN 3 then PotenciaContratada ELSE 0 END as PotenciaContratadaP3,
		CASE codperiodo WHEN 4 then PotenciaContratada ELSE 0 END as PotenciaContratadaP4,
		CASE codperiodo WHEN 5 then PotenciaContratada ELSE 0 END as PotenciaContratadaP5,
		CASE codperiodo WHEN 6 then PotenciaContratada ELSE 0 END as PotenciaContratadaP6,

		CASE codperiodo WHEN 1 then Precio ELSE 0 END as PrecioP1,
		CASE codperiodo WHEN 2 then Precio ELSE 0 END as PrecioP2,
		CASE codperiodo WHEN 3 then Precio ELSE 0 END as PrecioP3,
		CASE codperiodo WHEN 4 then Precio ELSE 0 END as PrecioP4,
		CASE codperiodo WHEN 5 then Precio ELSE 0 END as PrecioP5,
		CASE codperiodo WHEN 6 then Precio ELSE 0 END as PrecioP6
		from Potencia  WITH (NOLOCK)
	)
	, AgrupadaPeriodoPotencia as
	(
		SELECT IdFacturaVentaCabecera , 
		MAX(PotenciaFacturarP1) as PotenciaFacturaP1,  MAX(PotenciaFacturarP2) as PotenciaFacturaP2,  MAX(PotenciaFacturarP3) as PotenciaFacturaP3, 
		MAX(PotenciaFacturarP4) as PotenciaFacturaP4,  MAX(PotenciaFacturarP5) as PotenciaFacturaP5,  MAX(PotenciaFacturarP6) as PotenciaFacturaP6,
		MAX(PotenciaContratadaP1) as PotenciaContratadaP1,  MAX(PotenciaContratadaP2) as PotenciaContratadaP2,  MAX(PotenciaContratadaP3) as PotenciaContratadaP3, 
		MAX(PotenciaContratadaP4) as PotenciaContratadaP4,  MAX(PotenciaContratadaP5) as PotenciaContratadaP5,  MAX(PotenciaContratadaP6) as PotenciaContratadaP6
		, MAX(PrecioP1) as PotenciaPrecioP1, MAX(PrecioP2) as PotenciaPrecioP2, MAX(PrecioP3) as PotenciaPrecioP3, MAX(PrecioP4) as PotenciaPrecioP4,MAX(PrecioP5) as PotenciaPrecioP5,MAX(PrecioP6) as PotenciaPrecioP6
		FROM PeriodoPotencia WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)
	, SerieNumeroFacturasOrigen as
	(
		select CONCAT( origen.seriefactura, ' ',  origen.numerofactura) as SerieNumeroFacturaOrigen, principal.IdFacturaVentaCabecera
		from FacturaVentaCabecera principal WITH (NOLOCK)
		inner join FacturaVentaCabecera origen  WITH (NOLOCK)
		on principal.IdFacturaOrigen = origen.IdFacturaVentaCabecera
	),
	--cargos
	--Cargos Energia
	 CargosEnergia AS
	(
			SELECT FacturaVentaLinea.IdFacturaVentaCabecera as IdFacturaVentaCabecera
			, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/Energia)[1]', 'decimal'), 0) AS ConsumoCargoEnergia
			,		ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,9)'), 0) AS PrecioCargoEnergia
			,ImporteBase
			,FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo
		
			FROM   FacturaVentaLinea WITH (NOLOCK) inner join #TmpId as f
			on FacturaVentaLinea.IdFacturaVentaCabecera = f.LongParameter
			where  (FacturaConcepto IN (130004,130003))
	),
	PeriodoCargosEnergia as (
	select IdFacturaVentaCabecera, 	
		CASE codperiodo WHEN 1 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP1,
		CASE codperiodo WHEN 2 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP2,
		CASE codperiodo WHEN 3 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP3,
		CASE codperiodo WHEN 4 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP4,
		CASE codperiodo WHEN 5 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP5,
		CASE codperiodo WHEN 6 then ConsumoCargoEnergia ELSE 0 END as CargosEnergiaP6,

		CASE codperiodo WHEN 1 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP1,
		CASE codperiodo WHEN 2 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP2,
		CASE codperiodo WHEN 3 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP3,
		CASE codperiodo WHEN 4 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP4,
		CASE codperiodo WHEN 5 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP5,
		CASE codperiodo WHEN 6 then PrecioCargoEnergia ELSE 0 END as CargosPrecioEnergiaP6,

		CASE codperiodo WHEN 1 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP1,
		CASE codperiodo WHEN 2 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP2,
		CASE codperiodo WHEN 3 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP3,
		CASE codperiodo WHEN 4 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP4,
		CASE codperiodo WHEN 5 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP5,
		CASE codperiodo WHEN 6 then ImporteBase ELSE 0 END as CargosTotalPrecioEnergiaP6

		from CargosEnergia WITH (NOLOCK)
	),
	AgrupadaPeriodoCargosEnergia as (
	SELECT IdFacturaVentaCabecera , SUM(CargosEnergiaP1) as CargosEnergiaP1,  SUM(CargosEnergiaP2) as CargosEnergiaP2,  SUM(CargosEnergiaP3) as CargosEnergiaP3, 
		SUM(CargosEnergiaP4) as CargosEnergiaP4,  SUM(CargosEnergiaP5) as CargosEnergiaP5,  SUM(CargosEnergiaP6) as CargosEnergiaP6,
		MAX(CargosPrecioEnergiaP1) as CargosPrecioEnergiaP1, MAX(CargosPrecioEnergiaP2) as CargosPrecioEnergiaP2, MAX(CargosPrecioEnergiaP3) as CargosPrecioEnergiaP3,
		MAX(CargosPrecioEnergiaP4) as CargosPrecioEnergiaP4,MAX(CargosPrecioEnergiaP5) as CargosPrecioEnergiaP5,MAX(CargosPrecioEnergiaP6) as CargosPrecioEnergiaP6,

		MAX(CargosTotalPrecioEnergiaP1) as CargosTotalPrecioEnergiaP1, MAX(CargosTotalPrecioEnergiaP2) as CargosTotalPrecioEnergiaP2, MAX(CargosTotalPrecioEnergiaP3) as CargosTotalPrecioEnergiaP3,
		MAX(CargosTotalPrecioEnergiaP4) as CargosTotalPrecioEnergiaP4,MAX(CargosTotalPrecioEnergiaP5) as CargosTotalPrecioEnergiaP5,MAX(CargosTotalPrecioEnergiaP6) as CargosTotalPrecioEnergiaP6
	
		FROM PeriodoCargosEnergia WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	),
	--Cargos Potencia
	CargosPotencia AS
	(
			SELECT FacturaVentaLinea.IdFacturaVentaCabecera as IdFacturaVentaCabecera
			, ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/Potencia)[1]', 'decimal'), 0) AS ConsumoCargoPotencia
			,ISNULL(InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,9)'), 0) AS PrecioCargoPotencia
			,FacturaVentaLinea.ImporteBase
			,FacturaVentaLinea.InfoLineaXML.value('(FacturaConceptosDTO/Periodo/CodigoPeriodo)[1]', 'integer') as CodPeriodo
		
			FROM   FacturaVentaLinea WITH (NOLOCK) inner join #TmpId as f
			on FacturaVentaLinea.IdFacturaVentaCabecera = f.LongParameter
			where  (FacturaConcepto IN (130001,130002))
	),
	PeriodoCargosPotencia as (
	select IdFacturaVentaCabecera, 	
		CASE codperiodo WHEN 1 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP1,
		CASE codperiodo WHEN 2 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP2,
		CASE codperiodo WHEN 3 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP3,
		CASE codperiodo WHEN 4 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP4,
		CASE codperiodo WHEN 5 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP5,
		CASE codperiodo WHEN 6 then ConsumoCargoPotencia ELSE 0 END as CargosPotenciaP6,

		CASE codperiodo WHEN 1 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP1,
		CASE codperiodo WHEN 2 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP2,
		CASE codperiodo WHEN 3 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP3,
		CASE codperiodo WHEN 4 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP4,
		CASE codperiodo WHEN 5 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP5,
		CASE codperiodo WHEN 6 then PrecioCargoPotencia ELSE 0 END as CargosPrecioPotenciaP6,

		CASE codperiodo WHEN 1 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP1,
		CASE codperiodo WHEN 2 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP2,
		CASE codperiodo WHEN 3 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP3,
		CASE codperiodo WHEN 4 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP4,
		CASE codperiodo WHEN 5 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP5,
		CASE codperiodo WHEN 6 then ImporteBase ELSE 0 END as CargosTotalPrecioPotenciaP6
	   
		from CargosPotencia WITH (NOLOCK)
	),
	AgrupadaPeriodoCargosPotencia as (
	SELECT IdFacturaVentaCabecera , SUM(CargosPotenciaP1) as CargosPotenciaP1,  SUM(CargosPotenciaP2) as CargosPotenciaP2,  SUM(CargosPotenciaP3) as CargosPotenciaP3, 
		SUM(CargosPotenciaP4) as CargosPotenciaP4,  SUM(CargosPotenciaP5) as CargosPotenciaP5,  SUM(CargosPotenciaP6) as CargosPotenciaP6,

			MAX(CargosPrecioPotenciaP1) as CargosPrecioPotenciaP1, MAX(CargosPrecioPotenciaP2) as CargosPrecioPotenciaP2, MAX(CargosPrecioPotenciaP3) as CargosPrecioPotenciaP3,
		MAX(CargosPrecioPotenciaP4) as CargosPrecioPotenciaP4,MAX(CargosPrecioPotenciaP5) as CargosPrecioPotenciaP5,MAX(CargosPrecioPotenciaP6) as CargosPrecioPotenciaP6,

			MAX(CargosTotalPrecioPotenciaP1) as CargosTotalPrecioPotenciaP1, MAX(CargosTotalPrecioPotenciaP2) as CargosTotalPrecioPotenciaP2, MAX(CargosTotalPrecioPotenciaP3) as CargosTotalPrecioPotenciaP3,
		MAX(CargosTotalPrecioPotenciaP4) as CargosTotalPrecioPotenciaP4,MAX(CargosTotalPrecioPotenciaP5) as CargosTotalPrecioPotenciaP5,MAX(CargosTotalPrecioPotenciaP6) as CargosTotalPrecioPotenciaP6
	
		FROM PeriodoCargosPotencia WITH (NOLOCK)
		group by IdFacturaVentaCabecera
	)

	--genera consulta
	select	v.Identidad,v.RazonSocial as Cliente,v.CodigoContrato as contrato,v.Version as V,v.CodigoCUPS as cups,v.NombreCalle as direccion,v.TextoCiudad as poblacion,v.OrigenMedida as OrigenCurva,v.CodPostal as CodigoPostal,
			v.TextoProvincia as Provincia,v.SerieFactura + ' ' + convert(varchar(20),v.NumeroFactura) as 'Serie/Numero',CONVERT(VARCHAR, v.fechafactura, 103) as FechaFactura,v.NumfacturaATR as NroFacturaATR, 
			CONVERT(VARCHAR, v.fecharecepcion, 103) as FechaRecepcion,v.TextoTarifaPeaje as TarifaPeaje,v.TextoTarifa as Tarifa,v.TextoTarifaGrupo as Grupo, CONVERT(VARCHAR, v.FechaLecturaAnterior, 103) as LecturaAnterior,
			CONVERT(VARCHAR, v.FechaLectura, 103) as FechaLectura, ImporteIE.Importe as IE, ImporteAlquiler.Importe as Alquiler,'' as ImporteProductos, 


			(select sum(ImporteBase) from FacturaVentaTotal where IdFacturaVentaCabecera in (v.IdFacturaVentaCabecera)) as ImporteBase,
			(select sum(ImporteImpuesto) from FacturaVentaTotal where IdFacturaVentaCabecera in (v.IdFacturaVentaCabecera)) as ImporteImpuesto,
			(select sum(ImporteTotal) from FacturaVentaTotal where IdFacturaVentaCabecera in (v.IdFacturaVentaCabecera)) as ImporteTotal,

			cast(Round(CASE WHEN ISNULL(ConsumoP.ConsumoP1,0) <> 0 THEN ConsumoP.ConsumoP1 ELSE ConsumoPTA.ConsumoP1 END + 
					CASE WHEN ISNULL(ConsumoP.ConsumoP2,0) <> 0 THEN ConsumoP.ConsumoP2 ELSE ConsumoPTA.ConsumoP2 END +
					CASE WHEN ISNULL(ConsumoP.ConsumoP3,0) <> 0 THEN ConsumoP.ConsumoP3 ELSE ConsumoPTA.ConsumoP3 END +
					CASE WHEN ISNULL(ConsumoP.ConsumoP4,0) <> 0 THEN ConsumoP.ConsumoP4 ELSE ConsumoPTA.ConsumoP4 END +
					CASE WHEN ISNULL(ConsumoP.ConsumoP5,0) <> 0 THEN ConsumoP.ConsumoP5 ELSE ConsumoPTA.ConsumoP5 END +
					CASE WHEN ISNULL(ConsumoP.ConsumoP6,0) <> 0 THEN ConsumoP.ConsumoP6 ELSE ConsumoPTA.ConsumoP6 END,0) as decimal(10,2)) as Consumo,

			0 as reactiva,

			cast(round(PotenciaP.PotenciaContratadaP1 +
						PotenciaP.PotenciaContratadaP2 +
						PotenciaP.PotenciaContratadaP3 +
						PotenciaP.PotenciaContratadaP4 +
						PotenciaP.PotenciaContratadaP5 +
						PotenciaP.PotenciaContratadaP6,2) as decimal(10,2)) as Potencia,

			v.NombreFiscal as Distribuidora,
			0 as ImporteAdicional,
			v.SerieFactura + ' ' + convert(varchar(20),v.NumeroFactura) as NumeroFactura,
			PotenciaP.PotenciaContratadaP1 as PotenciaContratadaP1,
			PotenciaP.PotenciaContratadaP2 as PotenciaContratadaP2,
			PotenciaP.PotenciaContratadaP3 as PotenciaContratadaP3,
			PotenciaP.PotenciaContratadaP4 as PotenciaContratadaP4,
			PotenciaP.PotenciaContratadaP5 as PotenciaContratadaP5,
			PotenciaP.PotenciaContratadaP6 as PotenciaContratadaP6,

			PotenciaP.PotenciaFacturaP1 as PotenciafacturadaP1,
			PotenciaP.PotenciaFacturaP2 as PotenciafacturadaP2,
			PotenciaP.PotenciaFacturaP3 as PotenciafacturadaP3,
			PotenciaP.PotenciaFacturaP4 as PotenciafacturadaP4,
			PotenciaP.PotenciaFacturaP5 as PotenciafacturadaP5,
			PotenciaP.PotenciaFacturaP6 as PotenciafacturadaP6,

			PotenciaP.PotenciaPrecioP1 as PrecioPotenciaP1,
			PotenciaP.PotenciaPrecioP2 as PrecioPotenciaP2,
			PotenciaP.PotenciaPrecioP3 as PrecioPotenciaP3,
			PotenciaP.PotenciaPrecioP4 as PrecioPotenciaP4,
			PotenciaP.PotenciaPrecioP5 as PrecioPotenciaP5,
			PotenciaP.PotenciaPrecioP6 as PrecioPotenciaP6,

			round(CASE WHEN ISNULL(ConsumoP.ConsumoP1,0) <> 0 THEN ConsumoP.ConsumoP1 ELSE ConsumoPTA.ConsumoP1 END,2) as ConsumoP1, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP2,0) <> 0 THEN ConsumoP.ConsumoP2 ELSE ConsumoPTA.ConsumoP2 END,2) as ConsumoP2, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP3,0) <> 0 THEN ConsumoP.ConsumoP3 ELSE ConsumoPTA.ConsumoP3 END,2) as ConsumoP3, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP4,0) <> 0 THEN ConsumoP.ConsumoP4 ELSE ConsumoPTA.ConsumoP4 END,2) as ConsumoP4, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP5,0) <> 0 THEN ConsumoP.ConsumoP5 ELSE ConsumoPTA.ConsumoP5 END,2) as ConsumoP5, 
			round(CASE WHEN ISNULL(ConsumoP.ConsumoP6,0) <> 0 THEN ConsumoP.ConsumoP6 ELSE ConsumoPTA.ConsumoP6 END,2) as ConsumoP6,

			ConsumoP.EnergiaPrecioP1 as EnergiaPrecioP1, 
			ConsumoP.EnergiaPrecioP2 as EnergiaPrecioP2,
			ConsumoP.EnergiaPrecioP3 as EnergiaPrecioP3, 
			ConsumoP.EnergiaPrecioP4 as EnergiaPrecioP4, 
			ConsumoP.EnergiaPrecioP5 as EnergiaPrecioP5, 
			ConsumoP.EnergiaPrecioP6 as EnergiaPrecioP6,
	 
			( ISNULL(ConsumoP.ConsumoP1, ISNULL(ConsumoPTA.ConsumoP1,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP1, ISNULL(ConsumoPTA.EnergiaPrecioP1,0.0))
			+ ISNULL(ConsumoP.ConsumoP2, ISNULL(ConsumoPTA.ConsumoP2,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP2, ISNULL(ConsumoPTA.EnergiaPrecioP2,0.0))
			+ ISNULL(ConsumoP.ConsumoP3, ISNULL(ConsumoPTA.ConsumoP3,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP3, ISNULL(ConsumoPTA.EnergiaPrecioP3,0.0))
			+ ISNULL(ConsumoP.ConsumoP4, ISNULL(ConsumoPTA.ConsumoP4,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP4, ISNULL(ConsumoPTA.EnergiaPrecioP4,0.0))
			+ ISNULL(ConsumoP.ConsumoP5, ISNULL(ConsumoPTA.ConsumoP5,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP5, ISNULL(ConsumoPTA.EnergiaPrecioP5,0.0))
			+ ISNULL(ConsumoP.ConsumoP6, ISNULL(ConsumoPTA.ConsumoP6,0.0)) * ISNULL(ConsumoP.EnergiaPrecioP6, ISNULL(ConsumoPTA.EnergiaPrecioP6,0.0))) as CosteActiva,

			 ISNULL(f.ConsumoReactivaP1,0) as ConsumoRP1
			,ISNULL(f.ConsumoReactivaP2,0) as ConsumoRP2
			,ISNULL(f.ConsumoReactivaP3,0) as ConsumoRP3
			,ISNULL(f.ConsumoReactivaP4,0) as ConsumoRP4
			,ISNULL(f.ConsumoReactivaP5,0) as ConsumoRP5
			,ISNULL(f.ConsumoReactivaP6,0) as ConsumoRP6,

			 ISNULL(f.PrecioReactivaP1,0) as PrecioEnergiaRP1
			,ISNULL(f.PrecioReactivaP2,0) as PrecioEnergiaRP2
			,ISNULL(f.PrecioReactivaP3,0) as PrecioEnergiaRP3
			,ISNULL(f.PrecioReactivaP4,0) as PrecioEnergiaRP4
			,ISNULL(f.PrecioReactivaP5,0) as PrecioEnergiaRP5
			,ISNULL(f.PrecioReactivaP6,0) as PrecioEnergiaRP6,

			CASE WHEN ISNULL(ImporteReactiva.Importe,0) <> 0 THEN ImporteReactiva.Importe ELSE  
			(ISNULL(f.ConsumoReactivaP1,0.0) * ISNULL(f.PrecioReactivaP1, 0.0) + ISNULL(f.ConsumoReactivaP2,0.0) * ISNULL(f.PrecioReactivaP2, 0.0) 
			+ ISNULL(f.ConsumoReactivaP3,0.0) * ISNULL(f.PrecioReactivaP3, 0.0) + ISNULL(f.ConsumoReactivaP4,0.0) * ISNULL(f.PrecioReactivaP4, 0.0) 
			+ ISNULL(f.ConsumoReactivaP5,0.0) * ISNULL(f.PrecioReactivaP5, 0.0) + ISNULL(f.ConsumoReactivaP6,0.0) * ISNULL(f.PrecioReactivaP6, 0.0)) END as CosteReactiva,

			  LecturaP.MaximetroP1 as MaximetroP1
			, LecturaP.MaximetroP2 as MaximetroP2
			, LecturaP.MaximetroP3 as MaximetroP3
			, LecturaP.MaximetroP4 as MaximetroP4
			, LecturaP.MaximetroP5 as MaximetroP5
			, LecturaP.MaximetroP6 as MaximetroP6, 

			 Agente.NombreAgente as Comercial,
			 PerfilFacturacion.TextoPerfilFacturacion as PerfilFacturacion, 
			 ConsumoPTA.ImporteTarifaAcceso as ImporteEnergiaAcceso,
			 isnull(ImporteVariable.Importe,0) AS ImporteEnergiaVariable,
			 ImportePotencia.Importe as ImportePotencia,
			 isnull(ExcesoDistribuidora.ImporteBase,0) As ImporteExcesoDistribuidora,
			 '' as SerieNumeroFacturaOrigen,

			CargosPotenciaP.CargosPotenciaP1 as CargoPotenciaP1, 
			CargosPotenciaP.CargosPrecioPotenciaP1 as CargoPotenciaPrecioP1,
			CargospotenciaP.CargosTotalPrecioPotenciaP1,
			CargosPotenciaP.CargosPotenciaP2 as CargoPotenciaP2, 
			CargosPotenciaP.CargosPrecioPotenciaP2 as CargoPotenciaPrecioP2,
			CargospotenciaP.CargosTotalPrecioPotenciaP2,
			CargosPotenciaP.CargosPotenciaP3 as CargoPotenciaP3,
			CargosPotenciaP.CargosPrecioPotenciaP3 as CargoPotenciaPrecioP3,
			CargospotenciaP.CargosTotalPrecioPotenciaP3,
			CargosPotenciaP.CargosPotenciaP4 as CargoPotenciaP4, 
			CargosPotenciaP.CargosPrecioPotenciaP4 as CargoPotenciaPrecioP4,
			CargospotenciaP.CargosTotalPrecioPotenciaP4,
			CargosPotenciaP.CargosPotenciaP5 as CargoPotenciaP5, 
			CargosPotenciaP.CargosPrecioPotenciaP5 as CargoPotenciaPrecioP5,
			CargospotenciaP.CargosTotalPrecioPotenciaP5,
			CargosPotenciaP.CargosPotenciaP6 as CargoPotenciaP6,
			CargosPotenciaP.CargosPrecioPotenciaP6 as CargoPotenciaPrecioP6,
			CargospotenciaP.CargosTotalPrecioPotenciaP6,
			(CargospotenciaP.CargosTotalPrecioPotenciaP1+CargospotenciaP.CargosTotalPrecioPotenciaP2+CargospotenciaP.CargosTotalPrecioPotenciaP3+CargospotenciaP.CargosTotalPrecioPotenciaP4+CargospotenciaP.CargosTotalPrecioPotenciaP5+CargospotenciaP.CargosTotalPrecioPotenciaP6) as CargosTotalPrecioPotencia,
		
			CargosEnergiaP.CargosEnergiaP1 as CargoEnergiaP1,
			CargosEnergiap.CargosPrecioEnergiaP1 as CargoEnergiaPrecioP1,
			CargosEnergiap.CargosTotalPrecioEnergiaP1 as CargosTotalPrecioEnergiaP1,
			CargosEnergiaP.CargosEnergiaP2 as CargoEnergiaP2,
			CargosEnergiap.CargosPrecioEnergiaP2 as CargoEnergiaPrecioP2,
			CargosEnergiap.CargosTotalPrecioEnergiaP2 as CargosTotalPrecioEnergiaP2,
			CargosEnergiaP.CargosEnergiaP3 as CargoEnergiaP3,
			CargosEnergiap.CargosPrecioEnergiaP3 as CargoEnergiaPrecioP3,
			CargosEnergiap.CargosTotalPrecioEnergiaP3 as CargosTotalPrecioEnergiaP3,
			CargosEnergiaP.CargosEnergiaP4 as CargoEnergiaP4,
			CargosEnergiap.CargosPrecioEnergiaP4 as CargoEnergiaPrecioP4,
			CargosEnergiap.CargosTotalPrecioEnergiaP4 as CargosTotalPrecioEnergiaP4,
			CargosEnergiaP.CargosEnergiaP5 as CargoEnergiaP5,
			CargosEnergiap.CargosPrecioEnergiaP5 as CargoEnergiaPrecioP5,
			CargosEnergiap.CargosTotalPrecioEnergiaP5 as CargosTotalPrecioEnergiaP5,
			CargosEnergiaP.CargosEnergiaP6 as CargoEnergiaP6,
			CargosEnergiap.CargosPrecioEnergiaP6 as CargoEnergiaPrecioP6,
			CargosEnergiap.CargosTotalPrecioEnergiaP6 as CargosTotalPrecioEnergiaP6,
			(CargosEnergiap.CargosTotalPrecioEnergiaP1 + CargosEnergiap.CargosTotalPrecioEnergiaP2 + CargosEnergiap.CargosTotalPrecioEnergiaP3 + CargosEnergiap.CargosTotalPrecioEnergiaP4 +CargosEnergiap.CargosTotalPrecioEnergiaP5+ CargosEnergiap.CargosTotalPrecioEnergiaP6)as CargosTotalPrecioEnergia,

			(select sum(importebase) from FacturaVentaLinea where idfacturaventacabecera in (v.IdFacturaVentaCabecera) and facturaconcepto=30006 group by idfacturaventacabecera) as TotalTerminoProductosClick,
		
			(select sum(importebase) from FacturaVentaLinea where idfacturaventacabecera in (v.IdFacturaVentaCabecera) and facturaconcepto=30008 group by idfacturaventacabecera) as AutoConsumo,

			'' as DiferenciaPagosPorCapacidad,
			0 as AjusteCapGas,
			0 as ServicioAjusteSistema

		 FROM FacturasFiltradas as v WITH (NOLOCK)	

		 -- Reactiva
		 left join AgrupadaPeriodoReactiva  as f WITH (NOLOCK) on v.IdFacturaVentaCabecera = f.IdFacturaVentaCabecera

		 -- Consumos
		 left join AgrupadaPeriodoConsumos as ConsumoP WITH (NOLOCK) on v.IdFacturaVentaCabecera = ConsumoP.IdFacturaVentaCabecera

		 -- Consumos Energia Tarifa Acceso
		 left join AgrupadaPeriodoConsumosEnergiaTarifaAcceso as ConsumoPTA WITH (NOLOCK) on v.IdFacturaVentaCabecera = ConsumoPTA.IdFacturaVentaCabecera

		--Potencias
		left join AgrupadaPeriodoPotencia as PotenciaP WITH (NOLOCK) on v.IdFacturaVentaCabecera = PotenciaP.IdFacturaVentaCabecera

		--Maximetros
		left join LecturasAgrupadas as LecturaP WITH (NOLOCK) on v.IdLectura = LecturaP.IdLectura 
		
		--Contrato
		left join Contrato WITH (NOLOCK) on Contrato.IdContrato = v.IdContrato
		
		--Agente
		left join Agente WITH (NOLOCK) on Agente.IdAgente = Contrato.IdAgente
	
		--PerfilFacturacion
		left join PerfilFacturacion WITH (NOLOCK) on v.IdPerfilFacturacion = PerfilFacturacion.IdPerfilFacturacion
	
		--Exceso de potencia
		LEFT JOIN (SELECT IdFacturaVentaCabecera, SUM(ImporteBase) As ImporteBase 
					FROM FacturaVentaLinea WITH (NOLOCK) WHERE FacturaConcepto = 20004 Group By IdFacturaVentaCabecera) 
					As ExcesoPotencia on v.IdFacturaVentaCabecera = ExcesoPotencia.IdFacturaVentaCabecera 

		--Exceso Distribuidora
		LEFT JOIN (SELECT IdFacturaVentaCabecera, SUM(ImporteBase) As ImporteBase
					FROM FacturaVentaLinea WITH (NOLOCK) WHERE FacturaConcepto = 20006 Group By IdFacturaVentaCabecera)
					As ExcesoDistribuidora on v.IdFacturaVentaCabecera = ExcesoDistribuidora.IdFacturaVentaCabecera 

		--Importe Energia Variable
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto=30004
			group by IdFacturaVentaCabecera
		) as ImporteVariable on v.IdFacturaVentaCabecera = ImporteVariable.IdFacturaVentaCabecera
		
		--Importe Productos
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto in (100001,100002, 70020)
			group by IdFacturaVentaCabecera
		) as ImporteProductos on v.IdFacturaVentaCabecera = ImporteProductos.IdFacturaVentaCabecera
		
		--- Importe Potencias
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto in (10001,10002,10003,10005)
			group by IdFacturaVentaCabecera
		) as ImportePotencia on v.IdFacturaVentaCabecera = ImportePotencia.IdFacturaVentaCabecera

		--- Importe Reactiva
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto in (40003)
			group by IdFacturaVentaCabecera
		) as ImporteReactiva on v.IdFacturaVentaCabecera = ImporteReactiva.IdFacturaVentaCabecera

		--cargos
		left join AgrupadaPeriodoCargosEnergia CargosEnergiaP WITH (NOLOCK) ON CargosEnergiaP.idfacturaventacabecera = v.IdFacturaVentaCabecera	
		left join AgrupadaPeriodoCargosPotencia CargosPotenciaP WITH (NOLOCK) ON CargosPotenciaP.idfacturaventacabecera = v.IdFacturaVentaCabecera	

		--Alquiler
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto=50002
			group by IdFacturaVentaCabecera
		) as ImporteAlquiler on v.IdFacturaVentaCabecera = ImporteAlquiler.IdFacturaVentaCabecera

		--IE
		left join (
			select IdFacturaventaCabecera, sum(ImporteBase) as Importe
			from FacturaVentaLinea WITH (NOLOCK)
			where FacturaConcepto=60001
			group by IdFacturaVentaCabecera
		) as ImporteIE on v.IdFacturaVentaCabecera = ImporteIE.IdFacturaVentaCabecera

	drop table #TmpId