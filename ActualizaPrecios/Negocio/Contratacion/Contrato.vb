Partial Public Class Contrato
    Inherits BaseGenericDTO
    Implements IEquatable(Of Contrato)


    Public Sub New()
        _ContratoPDF = Nothing ' Byte()
        _IdTipoAlquiler = Nothing   ' Nullable(Of Long)
        _IdDestinoEnergia = Nothing ' Nullable(Of Long)
        _IdContratoTipo = Nothing   ' Nullable(Of Long)


        _IdContrato = 0 ' Long
        _Entorno = String.Empty ' String
        _CodigoContrato = Nothing   ' Nullable(Of Long)
        _Version = Nothing  ' Nullable(Of Long)
        _Confirmado = Nothing   ' Nullable(Of Boolean)
        _IdCliente = 0  ' Long
        _IdCups = Nothing   ' Nullable(Of Long)
        _IdComercializadora = Nothing   ' Nullable(Of Long)
        _FechaContrato = Nothing    ' Nullable(Of Date)
        _FechaAlta = Nothing    ' Nullable(Of Date)
        _FechaBaja = Nothing    ' Nullable(Of Date)
        _FechaVto = Nothing ' Nullable(Of Date)
        _FechaInstalacion = Nothing ' Nullable(Of Date)
        _IdContratoSituacion = Nothing  ' Nullable(Of Long)
        _Poliza = String.Empty  ' String
        _TipoContrato = String.Empty    ' String
        _Aclarador = String.Empty   ' String
        _DirecionUnica = String.Empty   ' String
        _IdClienteEnvio = 0 ' Long
        _IdClientePago = 0  ' Long
        _NombreInstalacion = String.Empty   ' String
        _IdCiudadInstalacion = Nothing  ' Nullable(Of Long)
        _IdCallejeroInstalacion = Nothing   ' Nullable(Of Long)
        _NumeroInstalacion = Nothing    ' Nullable(Of Integer)
        _AclaradorInstalacion = String.Empty    ' String
        _CodPostalInstalacion = String.Empty    ' String
        _NIFInstalacion = String.Empty  ' String
        _TelefonoInstalacion = String.Empty ' String
        _MovilInstalacion = String.Empty    ' String
        _FaxInstalacion = String.Empty  ' String
        _EmailInstalacion = String.Empty    ' String
        _IdTarifaPeaje = Nothing    ' Nullable(Of Long)
        _IdTarifa = Nothing ' Nullable(Of Long)
        _IdPerfilFacturacionPeaje = Nothing ' Nullable(Of Long)
        _IdModeloFactura = Nothing  ' Nullable(Of Long)
        _RevisionFra = Nothing  ' Nullable(Of Boolean)
        _TextoRevision = String.Empty   ' String
        _Agregacion = String.Empty  ' String
        _IdTipoPuntoMedida = Nothing    ' Nullable(Of Long)

        _NoFacturar = Nothing   ' Nullable(Of Boolean)
        _NoCortable = Nothing   ' Nullable(Of Boolean)
        _IdCarteraCobroCalendario = Nothing ' Nullable(Of Long)
        _ConsumoEstimada = String.Empty ' String

        _FechaObra = Nothing    ' Nullable(Of Date)
        _IdTensionSuministro = Nothing  ' Nullable(Of Long)
        _IdTensionFase = Nothing    ' Nullable(Of Long)

        _IdCNAE = Nothing   ' Nullable(Of Long)
        _RefExt1 = String.Empty ' String
        _RefExt2 = String.Empty ' String
        _IdGrupoImprimir = Nothing  ' Nullable(Of Long)
        _IdTipoCobroGrupo = Nothing ' Nullable(Of Long)
        _TipoContratoTM = Nothing   ' Nullable(Of Integer)
        _CurvaCargaTM = String.Empty    ' String
        _Observaciones = String.Empty   ' String
        _Representante = String.Empty   ' String
        _IdColectivoRep = Nothing   ' Nullable(Of Long)
        _IdentificadorRep = String.Empty    ' String
        _Comentario = String.Empty  ' String
        _IdIdiomaInforme = Nothing  ' Nullable(Of Long)
        _IdTipoImpuesto = Nothing   ' Nullable(Of Long)
        _CodigoContable = String.Empty  ' String
        _IdPeriodoFactura = Nothing ' Nullable(Of Long)
        _TipoImprimir = String.Empty    ' String
        _NoRenovar = Nothing    ' Nullable(Of Boolean)
        _EnviarRenovacion = Nothing ' Nullable(Of Boolean)
        _EnviarPrecios = Nothing    ' Nullable(Of Boolean)
        _Empleado = Nothing ' Nullable(Of Boolean)
        _CondicionesEsp = String.Empty  ' String
        _IdCalendarioTipo = Nothing ' Nullable(Of Long)
        _IdUnidadProgramacion = Nothing ' Nullable(Of Long)
        _FechaPrevistaActivacion = Nothing  ' Nullable(Of Date)
        _FechaPrevistaBaja = Nothing    ' Nullable(Of Date)
        _IdAgente = Nothing ' Nullable(Of Long)
        _IdModeloContrato = Nothing ' Nullable(Of Long)
        _ContratoInfoXML = String.Empty ' String
        _AltaOV = Nothing   ' Nullable(Of Boolean)
        _ModoImportacion = String.Empty ' String
        _ReferenciaBanco = String.Empty ' String
        _ReferenciaB2B = String.Empty   ' String
        _IsFirst = Nothing  ' Nullable(Of Boolean)
        _FechaCambioBanco = Nothing ' Nullable(Of Date)
        _CoreFechaFirma = Nothing   ' Nullable(Of Date)
        _B2BFechaFirma = Nothing    ' Nullable(Of Date)
        _IdSolicitudTipo = Nothing  ' Nullable(Of Long)
        _IsSolicitudUrgente = Nothing   ' Nullable(Of Boolean)
        _IdSolicitudTipoFechaEfecto = Nothing   ' Nullable(Of Long)
        _SinGastoImpago = False ' Boolean
        _IdPresion = Nothing    ' Nullable(Of Long)
        _IdSolicitudCambioTitular = Nothing ' Nullable(Of Long)
        _NoCedeCUPS = False ' Boolean
        _NoEnviarInformacion = False    ' Boolean
        _IdClientePagoDerecho = Nothing ' Nullable(Of Long)
        _CedulaHabitabilidad = String.Empty ' String
        _FechaCedulaHabitabilidad = Nothing ' Nullable(Of Date)
        _IdAdministrador = Nothing  ' Nullable(Of Long)
        _NumeroFinca = String.Empty ' String
        _ConformidadCliente = Nothing   ' Nullable(Of Boolean)
        _IncondicionalPS = Nothing  ' Nullable(Of Boolean)
        _CodigoOficinaContable = String.Empty   ' String
        _CodigoOrganoGestor = String.Empty  ' String
        _CodigoUnidadTramitadora = String.Empty ' String
        _CodigoTipoEstimacion = Nothing ' Nullable(Of Integer)
        _CodigoEscaladoConsumo = Nothing    ' Nullable(Of Integer)
        _ConsumoEstimado = Nothing  ' Nullable(Of Decimal)
        _IdContratoTipoEsencial = Nothing   ' Nullable(Of Long)
        _IdTipoCicloHorario = Nothing   ' Nullable(Of Long)
        _IdTipoEquipamiento = Nothing   ' Nullable(Of Long)
        _TiempoAutonomiaEquipo = String.Empty   ' String
        _ConfidencialidadDatosRPE = Nothing ' Nullable(Of Boolean)
        _ConfidencialidadDatosTarifaSocial = Nothing    ' Nullable(Of Boolean)
        _LecturaExtraordinaria = Nothing    ' Nullable(Of Boolean)
        _IdTipoDocumentacion = Nothing  ' Nullable(Of Long)
        _UrlDocumento = String.Empty    ' String
        _PrecioCapacidadEntrada = Nothing   ' Nullable(Of Decimal)
        _PrecioCapacidadSalida = Nothing    ' Nullable(Of Decimal)
        _IdSolicitudMotivoModificacion = Nothing    ' Nullable(Of Long)
        _IdModeloFacturaVarios = Nothing    ' Nullable(Of Long)
        _SituacionScoring = String.Empty    ' String
        _IdTipoAutoconsumo = Nothing    ' Nullable(Of Long)
        _BloqueoSistemaComercial = Nothing  ' Nullable(Of Boolean)
        _BloqueoPublicidadEmpresa = Nothing ' Nullable(Of Boolean)
        _BloqueoUsoTercero = Nothing    ' Nullable(Of Boolean)
        _IdOfertaLote = Nothing ' Nullable(Of Long)
        _IsBonoSocial = Nothing ' Nullable(Of Boolean)
        _IsContratoBonificado = Nothing ' Nullable(Of Boolean)
        _IdCanal = Nothing  ' Nullable(Of Long)
        _IsBajaAnticipada = Nothing ' Nullable(Of Boolean)
        _FechaBajaAnticipada = Nothing  ' Nullable(Of Date)
        _IdMotivoBaja = Nothing ' Nullable(Of Long)
        _CodigoOrigenBaja = Nothing ' Nullable(Of Integer)
        _IdTarifaPeajeAnual = Nothing   ' Nullable(Of Long)
        _TipoLecturaContrato = Nothing  ' Nullable(Of Integer)
        _TipoTension = Nothing  ' Nullable(Of Boolean)
        _NumPedidoFacturacion = String.Empty    ' String
        _IdAplicacionIH = Nothing   ' Nullable(Of Long)
        _AplicarRegasificacion = Nothing    ' Nullable(Of Boolean)
        _AplicarReajusteTarifa = Nothing    ' Nullable(Of Boolean)
        _TarifarPorB70 = Nothing    ' Nullable(Of Boolean)
        _IdModoControlPotencia = Nothing    ' Nullable(Of Long)
        _OrdenLibreFacturas = Nothing   ' Nullable(Of Long)
        _PermitirFacturarA7 = Nothing   ' Nullable(Of Boolean)
        _CodigoPromocional = String.Empty   ' String
        _IsNoInformarASNEF = Nothing    ' Nullable(Of Boolean)
        _NumProveedor = String.Empty    ' String
        _IsRenovacionProcesada = Nothing    ' Nullable(Of Boolean)
        _FechaAnulacion = Nothing   ' Nullable(Of Date)
        _ImporteCargoCuenta = Nothing   ' Nullable(Of Decimal)
        _TextoLibreFacturas = String.Empty  ' String
        _IsAgruparFacturas = Nothing    ' Nullable(Of Boolean)
        _IdMarca = Nothing  ' Nullable(Of Long)
        _IsUnificarCuentaBancaria = Nothing ' Nullable(Of Boolean)
        _IsTelemedido = Nothing ' Nullable(Of Boolean)
        _FechaAplicacionPrecios = Nothing   ' Nullable(Of Date)
        _Procedencia = String.Empty ' String
        _Subprocedencia = String.Empty  ' String
        _ATRDirecto = Nothing   ' Nullable(Of Boolean)
        _PorcentajePerdidasPropio = Nothing ' Nullable(Of Decimal)
        _EmailRep = String.Empty    ' String
        _SMSRep = String.Empty  ' String
        _ObtenerLiquidacion = Nothing   ' Nullable(Of Boolean)
        _OrderId = String.Empty ' String
        _IsContratoRenovado = Nothing   ' Nullable(Of Boolean)
        _Vulnerabilidad = Nothing   ' Nullable(Of Long)
        _FechaInicioVulnerabilidad = Nothing    ' Nullable(Of Date)
        _FechaFinVulnerabilidad = Nothing   ' Nullable(Of Date)
        _EnvioMinisterio = Nothing  ' Nullable(Of Boolean)
        _NumMenores = Nothing   ' Nullable(Of Long)
        _FechaXML = Nothing ' Nullable(Of Date)
        _SegmentoBS = Nothing   ' Nullable(Of Long)
        _FechaNacimientoMenor = Nothing ' Nullable(Of Date)
        _AyudaBS = Nothing  ' Nullable(Of Boolean)
        _IdSwitchDocumentSalesforce = String.Empty  ' String
        _IdOrderSalesforce = String.Empty   ' String
        _DiasVencimiento = Nothing  ' Nullable(Of Integer)
        _IsContratoRevisado = Nothing   ' Nullable(Of Boolean)
        _FechaCreacion = Nothing    ' Nullable(Of Date)
        _IsDual = Nothing   ' Nullable(Of Boolean)
        _IsProductoClick = Nothing  ' Nullable(Of Boolean)
        _IsFirmaDigitalEnviada = Nothing    ' Nullable(Of Boolean)
        _IsFirmadoDigitalmente = Nothing    ' Nullable(Of Boolean)
        _Reqqd = Nothing    ' Nullable(Of Decimal)
        _Reqqh = Nothing    ' Nullable(Of Long)
        _IsContratoAutonomo = Nothing   ' Nullable(Of Boolean)
        _IdCanalCodage = Nothing    ' Nullable(Of Long)
        _Version = Nothing  ' Nullable(Of Long)
        _AutorizarFidelizacion = Nothing ' Nullable(Of Boolean)
        _CederDatosEmpresasGrupoTotal = Nothing ' Nullable(Of Boolean)

    End Sub

    Public Function Difference(other As Contrato) As DifferenceDTO
        If other Is Nothing Then Throw New ArgumentException("other is null")
        If Not TypeOf other Is Contrato Then Throw New ArgumentException("Type of other invalid")
        Dim otherDTO As Contrato = CType(other, Contrato)
        Dim dif As New List(Of String)

        If Not IsEqual(Me.IdContrato, otherDTO.IdContrato) Then dif.Add("IdContrato")
        If Not IsEqual(Me.Entorno, otherDTO.Entorno) Then dif.Add("Entorno")
        If Not IsEqual(Me.CodigoContrato, otherDTO.CodigoContrato) Then dif.Add("CodigoContrato")
        If Not IsEqual(Me.Version, otherDTO.Version) Then dif.Add("Version")
        If Not IsEqual(Me.Confirmado, otherDTO.Confirmado) Then dif.Add("Confirmado")
        If Not IsEqual(Me.IdCliente, otherDTO.IdCliente) Then dif.Add("IdCliente")
        If Not IsEqual(Me.IdCups, otherDTO.IdCups) Then dif.Add("IdCups")
        If Not IsEqual(Me.IdComercializadora, otherDTO.IdComercializadora) Then dif.Add("IdComercializadora")
        If Not IsEqual(Me.FechaContrato, otherDTO.FechaContrato) Then dif.Add("FechaContrato")
        If Not IsEqual(Me.FechaAlta, otherDTO.FechaAlta) Then dif.Add("FechaAlta")
        If Not IsEqual(Me.FechaBaja, otherDTO.FechaBaja) Then dif.Add("FechaBaja")
        If Not IsEqual(Me.FechaVto, otherDTO.FechaVto) Then dif.Add("FechaVto")
        If Not IsEqual(Me.FechaInstalacion, otherDTO.FechaInstalacion) Then dif.Add("FechaInstalacion")
        If Not IsEqual(Me.IdContratoSituacion, otherDTO.IdContratoSituacion) Then dif.Add("IdContratoSituacion")
        If Not IsEqual(Me.Poliza, otherDTO.Poliza) Then dif.Add("Poliza")
        If Not IsEqual(Me.TipoContrato, otherDTO.TipoContrato) Then dif.Add("TipoContrato")
        If Not IsEqual(Me.Aclarador, otherDTO.Aclarador) Then dif.Add("Aclarador")
        If Not IsEqual(Me.DirecionUnica, otherDTO.DirecionUnica) Then dif.Add("DirecionUnica")
        If Not IsEqual(Me.IdClienteEnvio, otherDTO.IdClienteEnvio) Then dif.Add("IdClienteEnvio")
        If Not IsEqual(Me.IdClientePago, otherDTO.IdClientePago) Then dif.Add("IdClientePago")
        If Not IsEqual(Me.NombreInstalacion, otherDTO.NombreInstalacion) Then dif.Add("NombreInstalacion")
        If Not IsEqual(Me.IdCiudadInstalacion, otherDTO.IdCiudadInstalacion) Then dif.Add("IdCiudadInstalacion")
        If Not IsEqual(Me.IdCallejeroInstalacion, otherDTO.IdCallejeroInstalacion) Then dif.Add("IdCallejeroInstalacion")
        If Not IsEqual(Me.NumeroInstalacion, otherDTO.NumeroInstalacion) Then dif.Add("NumeroInstalacion")
        If Not IsEqual(Me.AclaradorInstalacion, otherDTO.AclaradorInstalacion) Then dif.Add("AclaradorInstalacion")
        If Not IsEqual(Me.CodPostalInstalacion, otherDTO.CodPostalInstalacion) Then dif.Add("CodPostalInstalacion")
        If Not IsEqual(Me.NIFInstalacion, otherDTO.NIFInstalacion) Then dif.Add("NIFInstalacion")
        If Not IsEqual(Me.TelefonoInstalacion, otherDTO.TelefonoInstalacion) Then dif.Add("TelefonoInstalacion")
        If Not IsEqual(Me.MovilInstalacion, otherDTO.MovilInstalacion) Then dif.Add("MovilInstalacion")
        If Not IsEqual(Me.FaxInstalacion, otherDTO.FaxInstalacion) Then dif.Add("FaxInstalacion")
        If Not IsEqual(Me.EmailInstalacion, otherDTO.EmailInstalacion) Then dif.Add("EmailInstalacion")
        If Not IsEqual(Me.IdTarifaPeaje, otherDTO.IdTarifaPeaje) Then dif.Add("IdTarifaPeaje")
        If Not IsEqual(Me.IdTarifa, otherDTO.IdTarifa) Then dif.Add("IdTarifa")
        If Not IsEqual(Me.IdPerfilFacturacionPeaje, otherDTO.IdPerfilFacturacionPeaje) Then dif.Add("IdPerfilFacturacionPeaje")
        If Not IsEqual(Me.IdModeloFactura, otherDTO.IdModeloFactura) Then dif.Add("IdModeloFactura")
        If Not IsEqual(Me.RevisionFra, otherDTO.RevisionFra) Then dif.Add("RevisionFra")
        If Not IsEqual(Me.TextoRevision, otherDTO.TextoRevision) Then dif.Add("TextoRevision")
        If Not IsEqual(Me.Agregacion, otherDTO.Agregacion) Then dif.Add("Agregacion")
        If Not IsEqual(Me.IdTipoPuntoMedida, otherDTO.IdTipoPuntoMedida) Then dif.Add("IdTipoPuntoMedida")
        If Not IsEqual(Me.IdTipoAlquiler, otherDTO.IdTipoAlquiler) Then dif.Add("IdTipoAlquiler")
        If Not IsEqual(Me.NoFacturar, otherDTO.NoFacturar) Then dif.Add("NoFacturar")
        If Not IsEqual(Me.NoCortable, otherDTO.NoCortable) Then dif.Add("NoCortable")
        If Not IsEqual(Me.IdCarteraCobroCalendario, otherDTO.IdCarteraCobroCalendario) Then dif.Add("IdCarteraCobroCalendario")
        If Not IsEqual(Me.ConsumoEstimada, otherDTO.ConsumoEstimada) Then dif.Add("ConsumoEstimada")
        If Not IsEqual(Me.IdContratoTipo, otherDTO.IdContratoTipo) Then dif.Add("IdContratoTipo")
        If Not IsEqual(Me.FechaObra, otherDTO.FechaObra) Then dif.Add("FechaObra")
        If Not IsEqual(Me.IdTensionSuministro, otherDTO.IdTensionSuministro) Then dif.Add("IdTensionSuministro")
        If Not IsEqual(Me.IdTensionFase, otherDTO.IdTensionFase) Then dif.Add("IdTensionFase")
        If Not IsEqual(Me.IdDestinoEnergia, otherDTO.IdDestinoEnergia) Then dif.Add("IdDestinoEnergia")
        If Not IsEqual(Me.IdCNAE, otherDTO.IdCNAE) Then dif.Add("IdCNAE")
        If Not IsEqual(Me.RefExt1, otherDTO.RefExt1) Then dif.Add("RefExt1")
        If Not IsEqual(Me.RefExt2, otherDTO.RefExt2) Then dif.Add("RefExt2")
        If Not IsEqual(Me.IdGrupoImprimir, otherDTO.IdGrupoImprimir) Then dif.Add("IdGrupoImprimir")
        If Not IsEqual(Me.IdTipoCobroGrupo, otherDTO.IdTipoCobroGrupo) Then dif.Add("IdTipoCobroGrupo")
        If Not IsEqual(Me.TipoContratoTM, otherDTO.TipoContratoTM) Then dif.Add("TipoContratoTM")
        If Not IsEqual(Me.CurvaCargaTM, otherDTO.CurvaCargaTM) Then dif.Add("CurvaCargaTM")
        If Not IsEqual(Me.Observaciones, otherDTO.Observaciones) Then dif.Add("Observaciones")
        If Not IsEqual(Me.Representante, otherDTO.Representante) Then dif.Add("Representante")
        If Not IsEqual(Me.IdColectivoRep, otherDTO.IdColectivoRep) Then dif.Add("IdColectivoRep")
        If Not IsEqual(Me.IdentificadorRep, otherDTO.IdentificadorRep) Then dif.Add("IdentificadorRep")
        If Not IsEqual(Me.Comentario, otherDTO.Comentario) Then dif.Add("Comentario")
        If Not IsEqual(Me.IdIdiomaInforme, otherDTO.IdIdiomaInforme) Then dif.Add("IdIdiomaInforme")
        If Not IsEqual(Me.IdTipoImpuesto, otherDTO.IdTipoImpuesto) Then dif.Add("IdTipoImpuesto")
        If Not IsEqual(Me.CodigoContable, otherDTO.CodigoContable) Then dif.Add("CodigoContable")
        If Not IsEqual(Me.IdPeriodoFactura, otherDTO.IdPeriodoFactura) Then dif.Add("IdPeriodoFactura")
        If Not IsEqual(Me.TipoImprimir, otherDTO.TipoImprimir) Then dif.Add("TipoImprimir")
        If Not IsEqual(Me.NoRenovar, otherDTO.NoRenovar) Then dif.Add("NoRenovar")
        If Not IsEqual(Me.EnviarRenovacion, otherDTO.EnviarRenovacion) Then dif.Add("EnviarRenovacion")
        If Not IsEqual(Me.EnviarPrecios, otherDTO.EnviarPrecios) Then dif.Add("EnviarPrecios")
        If Not IsEqual(Me.Empleado, otherDTO.Empleado) Then dif.Add("Empleado")
        If Not IsEqual(Me.CondicionesEsp, otherDTO.CondicionesEsp) Then dif.Add("CondicionesEsp")
        If Not IsEqual(Me.IdCalendarioTipo, otherDTO.IdCalendarioTipo) Then dif.Add("IdCalendarioTipo")
        If Not IsEqual(Me.IdUnidadProgramacion, otherDTO.IdUnidadProgramacion) Then dif.Add("IdUnidadProgramacion")
        If Not IsEqual(Me.FechaPrevistaActivacion, otherDTO.FechaPrevistaActivacion) Then dif.Add("FechaPrevistaActivacion")
        If Not IsEqual(Me.FechaPrevistaBaja, otherDTO.FechaPrevistaBaja) Then dif.Add("FechaPrevistaBaja")
        If Not IsEqual(Me.IdAgente, otherDTO.IdAgente) Then dif.Add("IdAgente")
        If Not IsEqual(Me.IdModeloContrato, otherDTO.IdModeloContrato) Then dif.Add("IdModeloContrato")
        If Not IsEqual(Me.ContratoInfoXML, otherDTO.ContratoInfoXML) Then dif.Add("ContratoInfoXML")
        If Not IsEqual(Me.AltaOV, otherDTO.AltaOV) Then dif.Add("AltaOV")
        If Not IsEqual(Me.ModoImportacion, otherDTO.ModoImportacion) Then dif.Add("ModoImportacion")
        If Not IsEqual(Me.ReferenciaBanco, otherDTO.ReferenciaBanco) Then dif.Add("ReferenciaBanco")
        If Not IsEqual(Me.ReferenciaB2B, otherDTO.ReferenciaB2B) Then dif.Add("ReferenciaB2B")
        If Not IsEqual(Me.IsFirst, otherDTO.IsFirst) Then dif.Add("IsFirst")
        If Not IsEqual(Me.FechaCambioBanco, otherDTO.FechaCambioBanco) Then dif.Add("FechaCambioBanco")
        If Not IsEqual(Me.CoreFechaFirma, otherDTO.CoreFechaFirma) Then dif.Add("CoreFechaFirma")
        If Not IsEqual(Me.B2BFechaFirma, otherDTO.B2BFechaFirma) Then dif.Add("B2BFechaFirma")
        If Not IsEqual(Me.IdSolicitudTipo, otherDTO.IdSolicitudTipo) Then dif.Add("IdSolicitudTipo")
        If Not IsEqual(Me.IsSolicitudUrgente, otherDTO.IsSolicitudUrgente) Then dif.Add("IsSolicitudUrgente")
        If Not IsEqual(Me.IdSolicitudTipoFechaEfecto, otherDTO.IdSolicitudTipoFechaEfecto) Then dif.Add("IdSolicitudTipoFechaEfecto")
        If Not IsEqual(Me.SinGastoImpago, otherDTO.SinGastoImpago) Then dif.Add("SinGastoImpago")
        If Not IsEqual(Me.IdPresion, otherDTO.IdPresion) Then dif.Add("IdPresion")
        If Not IsEqual(Me.IdSolicitudCambioTitular, otherDTO.IdSolicitudCambioTitular) Then dif.Add("IdSolicitudCambioTitular")
        If Not IsEqual(Me.NoCedeCUPS, otherDTO.NoCedeCUPS) Then dif.Add("NoCedeCUPS")
        If Not IsEqual(Me.NoEnviarInformacion, otherDTO.NoEnviarInformacion) Then dif.Add("NoEnviarInformacion")
        If Not IsEqual(Me.IdClientePagoDerecho, otherDTO.IdClientePagoDerecho) Then dif.Add("IdClientePagoDerecho")
        If Not IsEqual(Me.CedulaHabitabilidad, otherDTO.CedulaHabitabilidad) Then dif.Add("CedulaHabitabilidad")
        If Not IsEqual(Me.FechaCedulaHabitabilidad, otherDTO.FechaCedulaHabitabilidad) Then dif.Add("FechaCedulaHabitabilidad")
        If Not IsEqual(Me.IdAdministrador, otherDTO.IdAdministrador) Then dif.Add("IdAdministrador")
        If Not IsEqual(Me.NumeroFinca, otherDTO.NumeroFinca) Then dif.Add("NumeroFinca")
        If Not IsEqual(Me.ConformidadCliente, otherDTO.ConformidadCliente) Then dif.Add("ConformidadCliente")
        If Not IsEqual(Me.IncondicionalPS, otherDTO.IncondicionalPS) Then dif.Add("IncondicionalPS")
        If Not IsEqual(Me.CodigoOficinaContable, otherDTO.CodigoOficinaContable) Then dif.Add("CodigoOficinaContable")
        If Not IsEqual(Me.CodigoOrganoGestor, otherDTO.CodigoOrganoGestor) Then dif.Add("CodigoOrganoGestor")
        If Not IsEqual(Me.CodigoUnidadTramitadora, otherDTO.CodigoUnidadTramitadora) Then dif.Add("CodigoUnidadTramitadora")
        If Not IsEqual(Me.CodigoTipoEstimacion, otherDTO.CodigoTipoEstimacion) Then dif.Add("CodigoTipoEstimacion")
        If Not IsEqual(Me.CodigoEscaladoConsumo, otherDTO.CodigoEscaladoConsumo) Then dif.Add("CodigoEscaladoConsumo")
        If Not IsEqual(Me.ConsumoEstimado, otherDTO.ConsumoEstimado) Then dif.Add("ConsumoEstimado")
        If Not IsEqual(Me.IdContratoTipoEsencial, otherDTO.IdContratoTipoEsencial) Then dif.Add("IdContratoTipoEsencial")
        If Not IsEqual(Me.IdTipoCicloHorario, otherDTO.IdTipoCicloHorario) Then dif.Add("IdTipoCicloHorario")
        If Not IsEqual(Me.IdTipoEquipamiento, otherDTO.IdTipoEquipamiento) Then dif.Add("IdTipoEquipamiento")
        If Not IsEqual(Me.TiempoAutonomiaEquipo, otherDTO.TiempoAutonomiaEquipo) Then dif.Add("TiempoAutonomiaEquipo")
        If Not IsEqual(Me.ConfidencialidadDatosRPE, otherDTO.ConfidencialidadDatosRPE) Then dif.Add("ConfidencialidadDatosRPE")
        If Not IsEqual(Me.ConfidencialidadDatosTarifaSocial, otherDTO.ConfidencialidadDatosTarifaSocial) Then dif.Add("ConfidencialidadDatosTarifaSocial")
        If Not IsEqual(Me.LecturaExtraordinaria, otherDTO.LecturaExtraordinaria) Then dif.Add("LecturaExtraordinaria")
        If Not IsEqual(Me.IdTipoDocumentacion, otherDTO.IdTipoDocumentacion) Then dif.Add("IdTipoDocumentacion")
        If Not IsEqual(Me.UrlDocumento, otherDTO.UrlDocumento) Then dif.Add("UrlDocumento")
        If Not IsEqual(Me.PrecioCapacidadEntrada, otherDTO.PrecioCapacidadEntrada) Then dif.Add("PrecioCapacidadEntrada")
        If Not IsEqual(Me.PrecioCapacidadSalida, otherDTO.PrecioCapacidadSalida) Then dif.Add("PrecioCapacidadSalida")
        If Not IsEqual(Me.IdSolicitudMotivoModificacion, otherDTO.IdSolicitudMotivoModificacion) Then dif.Add("IdSolicitudMotivoModificacion")
        If Not IsEqual(Me.IdModeloFacturaVarios, otherDTO.IdModeloFacturaVarios) Then dif.Add("IdModeloFacturaVarios")
        If Not IsEqual(Me.SituacionScoring, otherDTO.SituacionScoring) Then dif.Add("SituacionScoring")
        If Not IsEqual(Me.IdTipoAutoconsumo, otherDTO.IdTipoAutoconsumo) Then dif.Add("IdTipoAutoconsumo")
        If Not IsEqual(Me.BloqueoSistemaComercial, otherDTO.BloqueoSistemaComercial) Then dif.Add("BloqueoSistemaComercial")
        If Not IsEqual(Me.BloqueoPublicidadEmpresa, otherDTO.BloqueoPublicidadEmpresa) Then dif.Add("BloqueoPublicidadEmpresa")
        If Not IsEqual(Me.BloqueoUsoTercero, otherDTO.BloqueoUsoTercero) Then dif.Add("BloqueoUsoTercero")
        If Not IsEqual(Me.IdOfertaLote, otherDTO.IdOfertaLote) Then dif.Add("IdOfertaLote")
        If Not IsEqual(Me.IsBonoSocial, otherDTO.IsBonoSocial) Then dif.Add("IsBonoSocial")
        If Not IsEqual(Me.IsContratoBonificado, otherDTO.IsContratoBonificado) Then dif.Add("IsContratoBonificado")
        If Not IsEqual(Me.IdCanal, otherDTO.IdCanal) Then dif.Add("IdCanal")
        If Not IsEqual(Me.IsBajaAnticipada, otherDTO.IsBajaAnticipada) Then dif.Add("IsBajaAnticipada")
        If Not IsEqual(Me.FechaBajaAnticipada, otherDTO.FechaBajaAnticipada) Then dif.Add("FechaBajaAnticipada")
        If Not IsEqual(Me.IdMotivoBaja, otherDTO.IdMotivoBaja) Then dif.Add("IdMotivoBaja")
        If Not IsEqual(Me.CodigoOrigenBaja, otherDTO.CodigoOrigenBaja) Then dif.Add("CodigoOrigenBaja")
        If Not IsEqual(Me.IdTarifaPeajeAnual, otherDTO.IdTarifaPeajeAnual) Then dif.Add("IdTarifaPeajeAnual")
        If Not IsEqual(Me.TipoLecturaContrato, otherDTO.TipoLecturaContrato) Then dif.Add("TipoLecturaContrato")
        If Not IsEqual(Me.TipoTension, otherDTO.TipoTension) Then dif.Add("TipoTension")
        If Not IsEqual(Me.NumPedidoFacturacion, otherDTO.NumPedidoFacturacion) Then dif.Add("NumPedidoFacturacion")
        If Not IsEqual(Me.IdAplicacionIH, otherDTO.IdAplicacionIH) Then dif.Add("IdAplicacionIH")
        If Not IsEqual(Me.AplicarRegasificacion, otherDTO.AplicarRegasificacion) Then dif.Add("AplicarRegasificacion")
        If Not IsEqual(Me.AplicarReajusteTarifa, otherDTO.AplicarReajusteTarifa) Then dif.Add("AplicarReajusteTarifa")
        If Not IsEqual(Me.TarifarPorB70, otherDTO.TarifarPorB70) Then dif.Add("TarifarPorB70")
        If Not IsEqual(Me.IdModoControlPotencia, otherDTO.IdModoControlPotencia) Then dif.Add("IdModoControlPotencia")
        If Not IsEqual(Me.OrdenLibreFacturas, otherDTO.OrdenLibreFacturas) Then dif.Add("OrdenLibreFacturas")
        If Not IsEqual(Me.PermitirFacturarA7, otherDTO.PermitirFacturarA7) Then dif.Add("PermitirFacturarA7")
        If Not IsEqual(Me.CodigoPromocional, otherDTO.CodigoPromocional) Then dif.Add("CodigoPromocional")
        If Not IsEqual(Me.IsNoInformarASNEF, otherDTO.IsNoInformarASNEF) Then dif.Add("IsNoInformarASNEF")
        If Not IsEqual(Me.NumProveedor, otherDTO.NumProveedor) Then dif.Add("NumProveedor")
        If Not IsEqual(Me.IsRenovacionProcesada, otherDTO.IsRenovacionProcesada) Then dif.Add("IsRenovacionProcesada")
        If Not IsEqual(Me.FechaAnulacion, otherDTO.FechaAnulacion) Then dif.Add("FechaAnulacion")
        If Not IsEqual(Me.ImporteCargoCuenta, otherDTO.ImporteCargoCuenta) Then dif.Add("ImporteCargoCuenta")
        If Not IsEqual(Me.TextoLibreFacturas, otherDTO.TextoLibreFacturas) Then dif.Add("TextoLibreFacturas")
        If Not IsEqual(Me.IsAgruparFacturas, otherDTO.IsAgruparFacturas) Then dif.Add("IsAgruparFacturas")
        If Not IsEqual(Me.IdMarca, otherDTO.IdMarca) Then dif.Add("IdMarca")
        If Not IsEqual(Me.IsUnificarCuentaBancaria, otherDTO.IsUnificarCuentaBancaria) Then dif.Add("IsUnificarCuentaBancaria")
        If Not IsEqual(Me.IsTelemedido, otherDTO.IsTelemedido) Then dif.Add("IsTelemedido")
        If Not IsEqual(Me.FechaAplicacionPrecios, otherDTO.FechaAplicacionPrecios) Then dif.Add("FechaAplicacionPrecios")
        If Not IsEqual(Me.Procedencia, otherDTO.Procedencia) Then dif.Add("Procedencia")
        If Not IsEqual(Me.Subprocedencia, otherDTO.Subprocedencia) Then dif.Add("Subprocedencia")
        If Not IsEqual(Me.ATRDirecto, otherDTO.ATRDirecto) Then dif.Add("ATRDirecto")
        If Not IsEqual(Me.PorcentajePerdidasPropio, otherDTO.PorcentajePerdidasPropio) Then dif.Add("PorcentajePerdidasPropio")
        If Not IsEqual(Me.EmailRep, otherDTO.EmailRep) Then dif.Add("EmailRep")
        If Not IsEqual(Me.SMSRep, otherDTO.SMSRep) Then dif.Add("SMSRep")
        If Not IsEqual(Me.ObtenerLiquidacion, otherDTO.ObtenerLiquidacion) Then dif.Add("ObtenerLiquidacion")
        If Not IsEqual(Me.OrderId, otherDTO.OrderId) Then dif.Add("OrderId")
        If Not IsEqual(Me.IsContratoRenovado, otherDTO.IsContratoRenovado) Then dif.Add("IsContratoRenovado")
        If Not IsEqual(Me.Vulnerabilidad, otherDTO.Vulnerabilidad) Then dif.Add("Vulnerabilidad")
        If Not IsEqual(Me.FechaInicioVulnerabilidad, otherDTO.FechaInicioVulnerabilidad) Then dif.Add("FechaInicioVulnerabilidad")
        If Not IsEqual(Me.FechaFinVulnerabilidad, otherDTO.FechaFinVulnerabilidad) Then dif.Add("FechaFinVulnerabilidad")
        If Not IsEqual(Me.EnvioMinisterio, otherDTO.EnvioMinisterio) Then dif.Add("EnvioMinisterio")
        If Not IsEqual(Me.NumMenores, otherDTO.NumMenores) Then dif.Add("NumMenores")
        If Not IsEqual(Me.FechaXML, otherDTO.FechaXML) Then dif.Add("FechaXML")
        If Not IsEqual(Me.SegmentoBS, otherDTO.SegmentoBS) Then dif.Add("SegmentoBS")
        If Not IsEqual(Me.FechaNacimientoMenor, otherDTO.FechaNacimientoMenor) Then dif.Add("FechaNacimientoMenor")
        If Not IsEqual(Me.AyudaBS, otherDTO.AyudaBS) Then dif.Add("AyudaBS")
        If Not IsEqual(Me.IdSwitchDocumentSalesforce, otherDTO.IdSwitchDocumentSalesforce) Then dif.Add("IdSwitchDocumentSalesforce")
        If Not IsEqual(Me.IdOrderSalesforce, otherDTO.IdOrderSalesforce) Then dif.Add("IdOrderSalesforce")
        If Not IsEqual(Me.DiasVencimiento, otherDTO.DiasVencimiento) Then dif.Add("DiasVencimiento")
        If Not IsEqual(Me.IsContratoRevisado, otherDTO.IsContratoRevisado) Then dif.Add("IsContratoRevisado")
        If Not IsEqual(Me.FechaCreacion, otherDTO.FechaCreacion) Then dif.Add("FechaCreacion")
        If Not IsEqual(Me.IsDual, otherDTO.IsDual) Then dif.Add("IsDual")
        If Not IsEqual(Me.IsProductoClick, otherDTO.IsProductoClick) Then dif.Add("IsProductoClick")
        If Not IsEqual(Me.IsFirmaDigitalEnviada, otherDTO.IsFirmaDigitalEnviada) Then dif.Add("IsFirmaDigitalEnviada")
        If Not IsEqual(Me.IsFirmadoDigitalmente, otherDTO.IsFirmadoDigitalmente) Then dif.Add("IsFirmadoDigitalmente")
        If Not IsEqual(Me.Reqqd, otherDTO.Reqqd) Then dif.Add("Reqqd")
        If Not IsEqual(Me.Reqqh, otherDTO.Reqqh) Then dif.Add("Reqqh")
        If Not IsEqual(Me.IsContratoAutonomo, otherDTO.IsContratoAutonomo) Then dif.Add("IsContratoAutonomo")
        If Not IsEqual(Me.IdCanalCodage, otherDTO.IdCanalCodage) Then dif.Add("IdCanalCodage")

        If Not IsEqual(Me.AutorizarFidelizacion, otherDTO.AutorizarFidelizacion) Then dif.Add("AutorizarFidelizacion ")
        If Not IsEqual(Me.CederDatosEmpresasGrupoTotal, otherDTO.CederDatosEmpresasGrupoTotal) Then dif.Add("CederDatosEmpresasGrupoTotal")

        Return New DifferenceDTO(Me.GetType.ToString, Me.IDEntityDTO, dif)
    End Function

    Public Overridable Function GetClone() As Contrato
        Dim clone As New Contrato
        clone.IdContrato = _IdContrato
        clone.Entorno = _Entorno
        clone.CodigoContrato = _CodigoContrato
        clone.Version = _Version
        clone.Confirmado = _Confirmado
        clone.IdCliente = _IdCliente
        clone.IdCups = _IdCups
        clone.IdComercializadora = _IdComercializadora
        clone.FechaContrato = _FechaContrato
        clone.FechaAlta = _FechaAlta
        clone.FechaBaja = _FechaBaja
        clone.FechaVto = _FechaVto
        clone.FechaInstalacion = _FechaInstalacion
        clone.IdContratoSituacion = _IdContratoSituacion
        clone.Poliza = _Poliza
        clone.TipoContrato = _TipoContrato
        clone.Aclarador = _Aclarador
        clone.DirecionUnica = _DirecionUnica
        clone.IdClienteEnvio = _IdClienteEnvio
        clone.IdClientePago = _IdClientePago
        clone.NombreInstalacion = _NombreInstalacion
        clone.IdCiudadInstalacion = _IdCiudadInstalacion
        clone.IdCallejeroInstalacion = _IdCallejeroInstalacion
        clone.NumeroInstalacion = _NumeroInstalacion
        clone.AclaradorInstalacion = _AclaradorInstalacion
        clone.CodPostalInstalacion = _CodPostalInstalacion
        clone.NIFInstalacion = _NIFInstalacion
        clone.TelefonoInstalacion = _TelefonoInstalacion
        clone.MovilInstalacion = _MovilInstalacion
        clone.FaxInstalacion = _FaxInstalacion
        clone.EmailInstalacion = _EmailInstalacion
        clone.IdTarifaPeaje = _IdTarifaPeaje
        clone.IdTarifa = _IdTarifa
        clone.IdPerfilFacturacionPeaje = _IdPerfilFacturacionPeaje
        clone.IdModeloFactura = _IdModeloFactura
        clone.RevisionFra = _RevisionFra
        clone.TextoRevision = _TextoRevision
        clone.Agregacion = _Agregacion
        clone.IdTipoPuntoMedida = _IdTipoPuntoMedida
        clone.IdTipoAlquiler = _IdTipoAlquiler
        clone.NoFacturar = _NoFacturar
        clone.NoCortable = _NoCortable
        clone.IdCarteraCobroCalendario = _IdCarteraCobroCalendario
        clone.ConsumoEstimada = _ConsumoEstimada
        clone.IdContratoTipo = _IdContratoTipo
        clone.FechaObra = _FechaObra
        clone.IdTensionSuministro = _IdTensionSuministro
        clone.IdTensionFase = _IdTensionFase
        clone.IdDestinoEnergia = _IdDestinoEnergia
        clone.IdCNAE = _IdCNAE
        clone.RefExt1 = _RefExt1
        clone.RefExt2 = _RefExt2
        clone.IdGrupoImprimir = _IdGrupoImprimir
        clone.IdTipoCobroGrupo = _IdTipoCobroGrupo
        clone.TipoContratoTM = _TipoContratoTM
        clone.CurvaCargaTM = _CurvaCargaTM
        clone.Observaciones = _Observaciones
        clone.Representante = _Representante
        clone.IdColectivoRep = _IdColectivoRep
        clone.IdentificadorRep = _IdentificadorRep
        clone.Comentario = _Comentario
        clone.IdIdiomaInforme = _IdIdiomaInforme
        clone.IdTipoImpuesto = _IdTipoImpuesto
        clone.CodigoContable = _CodigoContable
        clone.IdPeriodoFactura = _IdPeriodoFactura
        clone.TipoImprimir = _TipoImprimir
        clone.NoRenovar = _NoRenovar
        clone.EnviarRenovacion = _EnviarRenovacion
        clone.EnviarPrecios = _EnviarPrecios
        clone.Empleado = _Empleado
        clone.CondicionesEsp = _CondicionesEsp
        clone.IdCalendarioTipo = _IdCalendarioTipo
        clone.IdUnidadProgramacion = _IdUnidadProgramacion
        clone.FechaPrevistaActivacion = _FechaPrevistaActivacion
        clone.FechaPrevistaBaja = _FechaPrevistaBaja
        clone.IdAgente = _IdAgente
        clone.IdModeloContrato = _IdModeloContrato
        clone.ContratoInfoXML = _ContratoInfoXML
        clone.AltaOV = _AltaOV
        clone.ModoImportacion = _ModoImportacion
        clone.ReferenciaBanco = _ReferenciaBanco
        clone.ReferenciaB2B = _ReferenciaB2B
        clone.IsFirst = _IsFirst
        clone.FechaCambioBanco = _FechaCambioBanco
        clone.CoreFechaFirma = _CoreFechaFirma
        clone.B2BFechaFirma = _B2BFechaFirma
        clone.IdSolicitudTipo = _IdSolicitudTipo
        clone.IsSolicitudUrgente = _IsSolicitudUrgente
        clone.IdSolicitudTipoFechaEfecto = _IdSolicitudTipoFechaEfecto
        clone.SinGastoImpago = _SinGastoImpago
        clone.IdPresion = _IdPresion
        clone.IdSolicitudCambioTitular = _IdSolicitudCambioTitular
        clone.NoCedeCUPS = _NoCedeCUPS
        clone.NoEnviarInformacion = _NoEnviarInformacion
        clone.IdClientePagoDerecho = _IdClientePagoDerecho
        clone.CedulaHabitabilidad = _CedulaHabitabilidad
        clone.FechaCedulaHabitabilidad = _FechaCedulaHabitabilidad
        clone.IdAdministrador = _IdAdministrador
        clone.NumeroFinca = _NumeroFinca
        clone.ConformidadCliente = _ConformidadCliente
        clone.IncondicionalPS = _IncondicionalPS
        clone.CodigoOficinaContable = _CodigoOficinaContable
        clone.CodigoOrganoGestor = _CodigoOrganoGestor
        clone.CodigoUnidadTramitadora = _CodigoUnidadTramitadora
        clone.CodigoTipoEstimacion = _CodigoTipoEstimacion
        clone.CodigoEscaladoConsumo = _CodigoEscaladoConsumo
        clone.ConsumoEstimado = _ConsumoEstimado
        clone.IdContratoTipoEsencial = _IdContratoTipoEsencial
        clone.IdTipoCicloHorario = _IdTipoCicloHorario
        clone.IdTipoEquipamiento = _IdTipoEquipamiento
        clone.TiempoAutonomiaEquipo = _TiempoAutonomiaEquipo
        clone.ConfidencialidadDatosRPE = _ConfidencialidadDatosRPE
        clone.ConfidencialidadDatosTarifaSocial = _ConfidencialidadDatosTarifaSocial
        clone.LecturaExtraordinaria = _LecturaExtraordinaria
        clone.IdTipoDocumentacion = _IdTipoDocumentacion
        clone.UrlDocumento = _UrlDocumento
        clone.PrecioCapacidadEntrada = _PrecioCapacidadEntrada
        clone.PrecioCapacidadSalida = _PrecioCapacidadSalida
        clone.IdSolicitudMotivoModificacion = _IdSolicitudMotivoModificacion
        clone.IdModeloFacturaVarios = _IdModeloFacturaVarios
        clone.SituacionScoring = _SituacionScoring
        clone.IdTipoAutoconsumo = _IdTipoAutoconsumo
        clone.BloqueoSistemaComercial = _BloqueoSistemaComercial
        clone.BloqueoPublicidadEmpresa = _BloqueoPublicidadEmpresa
        clone.BloqueoUsoTercero = _BloqueoUsoTercero
        clone.IdOfertaLote = _IdOfertaLote
        clone.IsBonoSocial = _IsBonoSocial
        clone.IsContratoBonificado = _IsContratoBonificado
        clone.IdCanal = _IdCanal
        clone.IsBajaAnticipada = _IsBajaAnticipada
        clone.FechaBajaAnticipada = _FechaBajaAnticipada
        clone.IdMotivoBaja = _IdMotivoBaja
        clone.CodigoOrigenBaja = _CodigoOrigenBaja
        clone.IdTarifaPeajeAnual = _IdTarifaPeajeAnual
        clone.TipoLecturaContrato = _TipoLecturaContrato
        clone.TipoTension = _TipoTension
        clone.NumPedidoFacturacion = _NumPedidoFacturacion
        clone.IdAplicacionIH = _IdAplicacionIH
        clone.AplicarRegasificacion = _AplicarRegasificacion
        clone.AplicarReajusteTarifa = _AplicarReajusteTarifa
        clone.TarifarPorB70 = _TarifarPorB70
        clone.IdModoControlPotencia = _IdModoControlPotencia
        clone.OrdenLibreFacturas = _OrdenLibreFacturas
        clone.PermitirFacturarA7 = _PermitirFacturarA7
        clone.CodigoPromocional = _CodigoPromocional
        clone.IsNoInformarASNEF = _IsNoInformarASNEF
        clone.NumProveedor = _NumProveedor
        clone.IsRenovacionProcesada = _IsRenovacionProcesada
        clone.FechaAnulacion = _FechaAnulacion
        clone.ImporteCargoCuenta = _ImporteCargoCuenta
        clone.TextoLibreFacturas = _TextoLibreFacturas
        clone.IsAgruparFacturas = _IsAgruparFacturas
        clone.IdMarca = _IdMarca
        clone.IsUnificarCuentaBancaria = _IsUnificarCuentaBancaria
        clone.IsTelemedido = _IsTelemedido
        clone.FechaAplicacionPrecios = _FechaAplicacionPrecios
        clone.Procedencia = _Procedencia
        clone.Subprocedencia = _Subprocedencia
        clone.ATRDirecto = _ATRDirecto
        clone.PorcentajePerdidasPropio = _PorcentajePerdidasPropio
        clone.EmailRep = _EmailRep
        clone.SMSRep = _SMSRep
        clone.ObtenerLiquidacion = _ObtenerLiquidacion
        clone.OrderId = _OrderId
        clone.IsContratoRenovado = _IsContratoRenovado
        clone.Vulnerabilidad = _Vulnerabilidad
        clone.FechaInicioVulnerabilidad = _FechaInicioVulnerabilidad
        clone.FechaFinVulnerabilidad = _FechaFinVulnerabilidad
        clone.EnvioMinisterio = _EnvioMinisterio
        clone.NumMenores = _NumMenores
        clone.FechaXML = _FechaXML
        clone.SegmentoBS = _SegmentoBS
        clone.FechaNacimientoMenor = _FechaNacimientoMenor
        clone.AyudaBS = _AyudaBS
        clone.IdSwitchDocumentSalesforce = _IdSwitchDocumentSalesforce
        clone.IdOrderSalesforce = _IdOrderSalesforce
        clone.DiasVencimiento = _DiasVencimiento
        clone.IsContratoRevisado = _IsContratoRevisado
        clone.FechaCreacion = _FechaCreacion
        clone.IsDual = _IsDual
        clone.IsProductoClick = _IsProductoClick
        clone.IsFirmaDigitalEnviada = _IsFirmaDigitalEnviada
        clone.IsFirmadoDigitalmente = _IsFirmadoDigitalmente
        clone.Reqqd = _Reqqd
        clone.Reqqh = _Reqqh
        clone.IsContratoAutonomo = _IsContratoAutonomo
        clone.IdCanalCodage = _IdCanalCodage

        clone.AutorizarFidelizacion = _AutorizarFidelizacion
        clone.CederDatosEmpresasGrupoTotal = _CederDatosEmpresasGrupoTotal

        Return clone
    End Function

    Public Overloads Function Equals(other As Contrato) As Boolean _
     Implements IEquatable(Of Contrato).Equals
        If other Is Nothing Then Return False
        If Object.ReferenceEquals(Me, other) Then Return True
        Dim ret As Boolean = True
        ret = ret AndAlso IsEqual(Me.IdContrato, other.IdContrato)
        ret = ret AndAlso IsEqual(Me.Entorno, other.Entorno)
        ret = ret AndAlso IsEqual(Me.CodigoContrato, other.CodigoContrato)
        ret = ret AndAlso IsEqual(Me.Version, other.Version)
        ret = ret AndAlso IsEqual(Me.Confirmado, other.Confirmado)
        ret = ret AndAlso IsEqual(Me.IdCliente, other.IdCliente)
        ret = ret AndAlso IsEqual(Me.IdCups, other.IdCups)
        ret = ret AndAlso IsEqual(Me.IdComercializadora, other.IdComercializadora)
        ret = ret AndAlso IsEqual(Me.FechaContrato, other.FechaContrato)
        ret = ret AndAlso IsEqual(Me.FechaAlta, other.FechaAlta)
        ret = ret AndAlso IsEqual(Me.FechaBaja, other.FechaBaja)
        ret = ret AndAlso IsEqual(Me.FechaVto, other.FechaVto)
        ret = ret AndAlso IsEqual(Me.FechaInstalacion, other.FechaInstalacion)
        ret = ret AndAlso IsEqual(Me.IdContratoSituacion, other.IdContratoSituacion)
        ret = ret AndAlso IsEqual(Me.Poliza, other.Poliza)
        ret = ret AndAlso IsEqual(Me.TipoContrato, other.TipoContrato)
        ret = ret AndAlso IsEqual(Me.Aclarador, other.Aclarador)
        ret = ret AndAlso IsEqual(Me.DirecionUnica, other.DirecionUnica)
        ret = ret AndAlso IsEqual(Me.IdClienteEnvio, other.IdClienteEnvio)
        ret = ret AndAlso IsEqual(Me.IdClientePago, other.IdClientePago)
        ret = ret AndAlso IsEqual(Me.NombreInstalacion, other.NombreInstalacion)
        ret = ret AndAlso IsEqual(Me.IdCiudadInstalacion, other.IdCiudadInstalacion)
        ret = ret AndAlso IsEqual(Me.IdCallejeroInstalacion, other.IdCallejeroInstalacion)
        ret = ret AndAlso IsEqual(Me.NumeroInstalacion, other.NumeroInstalacion)
        ret = ret AndAlso IsEqual(Me.AclaradorInstalacion, other.AclaradorInstalacion)
        ret = ret AndAlso IsEqual(Me.CodPostalInstalacion, other.CodPostalInstalacion)
        ret = ret AndAlso IsEqual(Me.NIFInstalacion, other.NIFInstalacion)
        ret = ret AndAlso IsEqual(Me.TelefonoInstalacion, other.TelefonoInstalacion)
        ret = ret AndAlso IsEqual(Me.MovilInstalacion, other.MovilInstalacion)
        ret = ret AndAlso IsEqual(Me.FaxInstalacion, other.FaxInstalacion)
        ret = ret AndAlso IsEqual(Me.EmailInstalacion, other.EmailInstalacion)
        ret = ret AndAlso IsEqual(Me.IdTarifaPeaje, other.IdTarifaPeaje)
        ret = ret AndAlso IsEqual(Me.IdTarifa, other.IdTarifa)
        ret = ret AndAlso IsEqual(Me.IdPerfilFacturacionPeaje, other.IdPerfilFacturacionPeaje)
        ret = ret AndAlso IsEqual(Me.IdModeloFactura, other.IdModeloFactura)
        ret = ret AndAlso IsEqual(Me.RevisionFra, other.RevisionFra)
        ret = ret AndAlso IsEqual(Me.TextoRevision, other.TextoRevision)
        ret = ret AndAlso IsEqual(Me.Agregacion, other.Agregacion)
        ret = ret AndAlso IsEqual(Me.IdTipoPuntoMedida, other.IdTipoPuntoMedida)
        ret = ret AndAlso IsEqual(Me.IdTipoAlquiler, other.IdTipoAlquiler)
        ret = ret AndAlso IsEqual(Me.NoFacturar, other.NoFacturar)
        ret = ret AndAlso IsEqual(Me.NoCortable, other.NoCortable)
        ret = ret AndAlso IsEqual(Me.IdCarteraCobroCalendario, other.IdCarteraCobroCalendario)
        ret = ret AndAlso IsEqual(Me.ConsumoEstimada, other.ConsumoEstimada)
        ret = ret AndAlso IsEqual(Me.IdContratoTipo, other.IdContratoTipo)
        ret = ret AndAlso IsEqual(Me.FechaObra, other.FechaObra)
        ret = ret AndAlso IsEqual(Me.IdTensionSuministro, other.IdTensionSuministro)
        ret = ret AndAlso IsEqual(Me.IdTensionFase, other.IdTensionFase)
        ret = ret AndAlso IsEqual(Me.IdDestinoEnergia, other.IdDestinoEnergia)
        ret = ret AndAlso IsEqual(Me.IdCNAE, other.IdCNAE)
        ret = ret AndAlso IsEqual(Me.RefExt1, other.RefExt1)
        ret = ret AndAlso IsEqual(Me.RefExt2, other.RefExt2)
        ret = ret AndAlso IsEqual(Me.IdGrupoImprimir, other.IdGrupoImprimir)
        ret = ret AndAlso IsEqual(Me.IdTipoCobroGrupo, other.IdTipoCobroGrupo)
        ret = ret AndAlso IsEqual(Me.TipoContratoTM, other.TipoContratoTM)
        ret = ret AndAlso IsEqual(Me.CurvaCargaTM, other.CurvaCargaTM)
        ret = ret AndAlso IsEqual(Me.Observaciones, other.Observaciones)
        ret = ret AndAlso IsEqual(Me.Representante, other.Representante)
        ret = ret AndAlso IsEqual(Me.IdColectivoRep, other.IdColectivoRep)
        ret = ret AndAlso IsEqual(Me.IdentificadorRep, other.IdentificadorRep)
        ret = ret AndAlso IsEqual(Me.Comentario, other.Comentario)
        ret = ret AndAlso IsEqual(Me.IdIdiomaInforme, other.IdIdiomaInforme)
        ret = ret AndAlso IsEqual(Me.IdTipoImpuesto, other.IdTipoImpuesto)
        ret = ret AndAlso IsEqual(Me.CodigoContable, other.CodigoContable)
        ret = ret AndAlso IsEqual(Me.IdPeriodoFactura, other.IdPeriodoFactura)
        ret = ret AndAlso IsEqual(Me.TipoImprimir, other.TipoImprimir)
        ret = ret AndAlso IsEqual(Me.NoRenovar, other.NoRenovar)
        ret = ret AndAlso IsEqual(Me.EnviarRenovacion, other.EnviarRenovacion)
        ret = ret AndAlso IsEqual(Me.EnviarPrecios, other.EnviarPrecios)
        ret = ret AndAlso IsEqual(Me.Empleado, other.Empleado)
        ret = ret AndAlso IsEqual(Me.CondicionesEsp, other.CondicionesEsp)
        ret = ret AndAlso IsEqual(Me.IdCalendarioTipo, other.IdCalendarioTipo)
        ret = ret AndAlso IsEqual(Me.IdUnidadProgramacion, other.IdUnidadProgramacion)
        ret = ret AndAlso IsEqual(Me.FechaPrevistaActivacion, other.FechaPrevistaActivacion)
        ret = ret AndAlso IsEqual(Me.FechaPrevistaBaja, other.FechaPrevistaBaja)
        ret = ret AndAlso IsEqual(Me.IdAgente, other.IdAgente)
        ret = ret AndAlso IsEqual(Me.IdModeloContrato, other.IdModeloContrato)
        ret = ret AndAlso IsEqual(Me.ContratoInfoXML, other.ContratoInfoXML)
        ret = ret AndAlso IsEqual(Me.AltaOV, other.AltaOV)
        ret = ret AndAlso IsEqual(Me.ModoImportacion, other.ModoImportacion)
        ret = ret AndAlso IsEqual(Me.ReferenciaBanco, other.ReferenciaBanco)
        ret = ret AndAlso IsEqual(Me.ReferenciaB2B, other.ReferenciaB2B)
        ret = ret AndAlso IsEqual(Me.IsFirst, other.IsFirst)
        ret = ret AndAlso IsEqual(Me.FechaCambioBanco, other.FechaCambioBanco)
        ret = ret AndAlso IsEqual(Me.CoreFechaFirma, other.CoreFechaFirma)
        ret = ret AndAlso IsEqual(Me.B2BFechaFirma, other.B2BFechaFirma)
        ret = ret AndAlso IsEqual(Me.IdSolicitudTipo, other.IdSolicitudTipo)
        ret = ret AndAlso IsEqual(Me.IsSolicitudUrgente, other.IsSolicitudUrgente)
        ret = ret AndAlso IsEqual(Me.IdSolicitudTipoFechaEfecto, other.IdSolicitudTipoFechaEfecto)
        ret = ret AndAlso IsEqual(Me.SinGastoImpago, other.SinGastoImpago)
        ret = ret AndAlso IsEqual(Me.IdPresion, other.IdPresion)
        ret = ret AndAlso IsEqual(Me.IdSolicitudCambioTitular, other.IdSolicitudCambioTitular)
        ret = ret AndAlso IsEqual(Me.NoCedeCUPS, other.NoCedeCUPS)
        ret = ret AndAlso IsEqual(Me.NoEnviarInformacion, other.NoEnviarInformacion)
        ret = ret AndAlso IsEqual(Me.IdClientePagoDerecho, other.IdClientePagoDerecho)
        ret = ret AndAlso IsEqual(Me.CedulaHabitabilidad, other.CedulaHabitabilidad)
        ret = ret AndAlso IsEqual(Me.FechaCedulaHabitabilidad, other.FechaCedulaHabitabilidad)
        ret = ret AndAlso IsEqual(Me.IdAdministrador, other.IdAdministrador)
        ret = ret AndAlso IsEqual(Me.NumeroFinca, other.NumeroFinca)
        ret = ret AndAlso IsEqual(Me.ConformidadCliente, other.ConformidadCliente)
        ret = ret AndAlso IsEqual(Me.IncondicionalPS, other.IncondicionalPS)
        ret = ret AndAlso IsEqual(Me.CodigoOficinaContable, other.CodigoOficinaContable)
        ret = ret AndAlso IsEqual(Me.CodigoOrganoGestor, other.CodigoOrganoGestor)
        ret = ret AndAlso IsEqual(Me.CodigoUnidadTramitadora, other.CodigoUnidadTramitadora)
        ret = ret AndAlso IsEqual(Me.CodigoTipoEstimacion, other.CodigoTipoEstimacion)
        ret = ret AndAlso IsEqual(Me.CodigoEscaladoConsumo, other.CodigoEscaladoConsumo)
        ret = ret AndAlso IsEqual(Me.ConsumoEstimado, other.ConsumoEstimado)
        ret = ret AndAlso IsEqual(Me.IdContratoTipoEsencial, other.IdContratoTipoEsencial)
        ret = ret AndAlso IsEqual(Me.IdTipoCicloHorario, other.IdTipoCicloHorario)
        ret = ret AndAlso IsEqual(Me.IdTipoEquipamiento, other.IdTipoEquipamiento)
        ret = ret AndAlso IsEqual(Me.TiempoAutonomiaEquipo, other.TiempoAutonomiaEquipo)
        ret = ret AndAlso IsEqual(Me.ConfidencialidadDatosRPE, other.ConfidencialidadDatosRPE)
        ret = ret AndAlso IsEqual(Me.ConfidencialidadDatosTarifaSocial, other.ConfidencialidadDatosTarifaSocial)
        ret = ret AndAlso IsEqual(Me.LecturaExtraordinaria, other.LecturaExtraordinaria)
        ret = ret AndAlso IsEqual(Me.IdTipoDocumentacion, other.IdTipoDocumentacion)
        ret = ret AndAlso IsEqual(Me.UrlDocumento, other.UrlDocumento)
        ret = ret AndAlso IsEqual(Me.PrecioCapacidadEntrada, other.PrecioCapacidadEntrada)
        ret = ret AndAlso IsEqual(Me.PrecioCapacidadSalida, other.PrecioCapacidadSalida)
        ret = ret AndAlso IsEqual(Me.IdSolicitudMotivoModificacion, other.IdSolicitudMotivoModificacion)
        ret = ret AndAlso IsEqual(Me.IdModeloFacturaVarios, other.IdModeloFacturaVarios)
        ret = ret AndAlso IsEqual(Me.SituacionScoring, other.SituacionScoring)
        ret = ret AndAlso IsEqual(Me.IdTipoAutoconsumo, other.IdTipoAutoconsumo)
        ret = ret AndAlso IsEqual(Me.BloqueoSistemaComercial, other.BloqueoSistemaComercial)
        ret = ret AndAlso IsEqual(Me.BloqueoPublicidadEmpresa, other.BloqueoPublicidadEmpresa)
        ret = ret AndAlso IsEqual(Me.BloqueoUsoTercero, other.BloqueoUsoTercero)
        ret = ret AndAlso IsEqual(Me.IdOfertaLote, other.IdOfertaLote)
        ret = ret AndAlso IsEqual(Me.IsBonoSocial, other.IsBonoSocial)
        ret = ret AndAlso IsEqual(Me.IsContratoBonificado, other.IsContratoBonificado)
        ret = ret AndAlso IsEqual(Me.IdCanal, other.IdCanal)
        ret = ret AndAlso IsEqual(Me.IsBajaAnticipada, other.IsBajaAnticipada)
        ret = ret AndAlso IsEqual(Me.FechaBajaAnticipada, other.FechaBajaAnticipada)
        ret = ret AndAlso IsEqual(Me.IdMotivoBaja, other.IdMotivoBaja)
        ret = ret AndAlso IsEqual(Me.CodigoOrigenBaja, other.CodigoOrigenBaja)
        ret = ret AndAlso IsEqual(Me.IdTarifaPeajeAnual, other.IdTarifaPeajeAnual)
        ret = ret AndAlso IsEqual(Me.TipoLecturaContrato, other.TipoLecturaContrato)
        ret = ret AndAlso IsEqual(Me.TipoTension, other.TipoTension)
        ret = ret AndAlso IsEqual(Me.NumPedidoFacturacion, other.NumPedidoFacturacion)
        ret = ret AndAlso IsEqual(Me.IdAplicacionIH, other.IdAplicacionIH)
        ret = ret AndAlso IsEqual(Me.AplicarRegasificacion, other.AplicarRegasificacion)
        ret = ret AndAlso IsEqual(Me.AplicarReajusteTarifa, other.AplicarReajusteTarifa)
        ret = ret AndAlso IsEqual(Me.TarifarPorB70, other.TarifarPorB70)
        ret = ret AndAlso IsEqual(Me.IdModoControlPotencia, other.IdModoControlPotencia)
        ret = ret AndAlso IsEqual(Me.OrdenLibreFacturas, other.OrdenLibreFacturas)
        ret = ret AndAlso IsEqual(Me.PermitirFacturarA7, other.PermitirFacturarA7)
        ret = ret AndAlso IsEqual(Me.CodigoPromocional, other.CodigoPromocional)
        ret = ret AndAlso IsEqual(Me.IsNoInformarASNEF, other.IsNoInformarASNEF)
        ret = ret AndAlso IsEqual(Me.NumProveedor, other.NumProveedor)
        ret = ret AndAlso IsEqual(Me.IsRenovacionProcesada, other.IsRenovacionProcesada)
        ret = ret AndAlso IsEqual(Me.FechaAnulacion, other.FechaAnulacion)
        ret = ret AndAlso IsEqual(Me.ImporteCargoCuenta, other.ImporteCargoCuenta)
        ret = ret AndAlso IsEqual(Me.TextoLibreFacturas, other.TextoLibreFacturas)
        ret = ret AndAlso IsEqual(Me.IsAgruparFacturas, other.IsAgruparFacturas)
        ret = ret AndAlso IsEqual(Me.IdMarca, other.IdMarca)
        ret = ret AndAlso IsEqual(Me.IsUnificarCuentaBancaria, other.IsUnificarCuentaBancaria)
        ret = ret AndAlso IsEqual(Me.IsTelemedido, other.IsTelemedido)
        ret = ret AndAlso IsEqual(Me.FechaAplicacionPrecios, other.FechaAplicacionPrecios)
        ret = ret AndAlso IsEqual(Me.Procedencia, other.Procedencia)
        ret = ret AndAlso IsEqual(Me.Subprocedencia, other.Subprocedencia)
        ret = ret AndAlso IsEqual(Me.ATRDirecto, other.ATRDirecto)
        ret = ret AndAlso IsEqual(Me.PorcentajePerdidasPropio, other.PorcentajePerdidasPropio)
        ret = ret AndAlso IsEqual(Me.EmailRep, other.EmailRep)
        ret = ret AndAlso IsEqual(Me.SMSRep, other.SMSRep)
        ret = ret AndAlso IsEqual(Me.ObtenerLiquidacion, other.ObtenerLiquidacion)
        ret = ret AndAlso IsEqual(Me.OrderId, other.OrderId)
        ret = ret AndAlso IsEqual(Me.IsContratoRenovado, other.IsContratoRenovado)
        ret = ret AndAlso IsEqual(Me.Vulnerabilidad, other.Vulnerabilidad)
        ret = ret AndAlso IsEqual(Me.FechaInicioVulnerabilidad, other.FechaInicioVulnerabilidad)
        ret = ret AndAlso IsEqual(Me.FechaFinVulnerabilidad, other.FechaFinVulnerabilidad)
        ret = ret AndAlso IsEqual(Me.EnvioMinisterio, other.EnvioMinisterio)
        ret = ret AndAlso IsEqual(Me.NumMenores, other.NumMenores)
        ret = ret AndAlso IsEqual(Me.FechaXML, other.FechaXML)
        ret = ret AndAlso IsEqual(Me.SegmentoBS, other.SegmentoBS)
        ret = ret AndAlso IsEqual(Me.FechaNacimientoMenor, other.FechaNacimientoMenor)
        ret = ret AndAlso IsEqual(Me.AyudaBS, other.AyudaBS)
        ret = ret AndAlso IsEqual(Me.IdSwitchDocumentSalesforce, other.IdSwitchDocumentSalesforce)
        ret = ret AndAlso IsEqual(Me.IdOrderSalesforce, other.IdOrderSalesforce)
        ret = ret AndAlso IsEqual(Me.DiasVencimiento, other.DiasVencimiento)
        ret = ret AndAlso IsEqual(Me.IsContratoRevisado, other.IsContratoRevisado)
        ret = ret AndAlso IsEqual(Me.FechaCreacion, other.FechaCreacion)
        ret = ret AndAlso IsEqual(Me.IsDual, other.IsDual)
        ret = ret AndAlso IsEqual(Me.IsProductoClick, other.IsProductoClick)
        ret = ret AndAlso IsEqual(Me.IsFirmaDigitalEnviada, other.IsFirmaDigitalEnviada)
        ret = ret AndAlso IsEqual(Me.IsFirmadoDigitalmente, other.IsFirmadoDigitalmente)
        ret = ret AndAlso IsEqual(Me.Reqqd, other.Reqqd)
        ret = ret AndAlso IsEqual(Me.Reqqh, other.Reqqh)
        ret = ret AndAlso IsEqual(Me.IsContratoAutonomo, other.IsContratoAutonomo)
        ret = ret AndAlso IsEqual(Me.IdCanalCodage, other.IdCanalCodage)
        ret = ret AndAlso IsEqual(Me.AutorizarFidelizacion, other.AutorizarFidelizacion)
        ret = ret AndAlso IsEqual(Me.CederDatosEmpresasGrupoTotal, other.CederDatosEmpresasGrupoTotal)

        Return ret
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then Return False
        Dim DTOObj As Contrato = TryCast(obj, Contrato)
        If DTOObj Is Nothing Then
            Return False
        Else
            Return Equals(DTOObj)
        End If
    End Function

    Public Shared Operator <>(DTO1 As Contrato, DTO2 As Contrato) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Object.Equals(DTO1, DTO2)
        End If
        Return DTO1.Equals(DTO2)
    End Operator

    Public Shared Operator =(DTO1 As Contrato, DTO2 As Contrato) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Not Object.Equals(DTO1, DTO2)
        End If
        Return Not DTO1.Equals(DTO2)
    End Operator


    Public Overrides Function GetHashCode() As Integer
        Dim ret As String = String.Empty
        If Not IsNothing(Me.IdContrato) Then ret = ret & Me.IdContrato.GetHashCode().ToString()
        If Not IsNothing(Me.Entorno) Then ret = ret & Me.Entorno.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoContrato) Then ret = ret & Me.CodigoContrato.GetHashCode().ToString()
        If Not IsNothing(Me.Version) Then ret = ret & Me.Version.GetHashCode().ToString()
        If Not IsNothing(Me.Confirmado) Then ret = ret & Me.Confirmado.GetHashCode().ToString()
        If Not IsNothing(Me.IdCliente) Then ret = ret & Me.IdCliente.GetHashCode().ToString()
        If Not IsNothing(Me.IdCups) Then ret = ret & Me.IdCups.GetHashCode().ToString()
        If Not IsNothing(Me.IdComercializadora) Then ret = ret & Me.IdComercializadora.GetHashCode().ToString()
        If Not IsNothing(Me.FechaContrato) Then ret = ret & Me.FechaContrato.GetHashCode().ToString()
        If Not IsNothing(Me.FechaAlta) Then ret = ret & Me.FechaAlta.GetHashCode().ToString()
        If Not IsNothing(Me.FechaBaja) Then ret = ret & Me.FechaBaja.GetHashCode().ToString()
        If Not IsNothing(Me.FechaVto) Then ret = ret & Me.FechaVto.GetHashCode().ToString()
        If Not IsNothing(Me.FechaInstalacion) Then ret = ret & Me.FechaInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.IdContratoSituacion) Then ret = ret & Me.IdContratoSituacion.GetHashCode().ToString()
        If Not IsNothing(Me.Poliza) Then ret = ret & Me.Poliza.GetHashCode().ToString()
        If Not IsNothing(Me.TipoContrato) Then ret = ret & Me.TipoContrato.GetHashCode().ToString()
        If Not IsNothing(Me.Aclarador) Then ret = ret & Me.Aclarador.GetHashCode().ToString()
        If Not IsNothing(Me.DirecionUnica) Then ret = ret & Me.DirecionUnica.GetHashCode().ToString()
        If Not IsNothing(Me.IdClienteEnvio) Then ret = ret & Me.IdClienteEnvio.GetHashCode().ToString()
        If Not IsNothing(Me.IdClientePago) Then ret = ret & Me.IdClientePago.GetHashCode().ToString()
        If Not IsNothing(Me.NombreInstalacion) Then ret = ret & Me.NombreInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.IdCiudadInstalacion) Then ret = ret & Me.IdCiudadInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.IdCallejeroInstalacion) Then ret = ret & Me.IdCallejeroInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.NumeroInstalacion) Then ret = ret & Me.NumeroInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.AclaradorInstalacion) Then ret = ret & Me.AclaradorInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.CodPostalInstalacion) Then ret = ret & Me.CodPostalInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.NIFInstalacion) Then ret = ret & Me.NIFInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.TelefonoInstalacion) Then ret = ret & Me.TelefonoInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.MovilInstalacion) Then ret = ret & Me.MovilInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.FaxInstalacion) Then ret = ret & Me.FaxInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.EmailInstalacion) Then ret = ret & Me.EmailInstalacion.GetHashCode().ToString()
        If Not IsNothing(Me.IdTarifaPeaje) Then ret = ret & Me.IdTarifaPeaje.GetHashCode().ToString()
        If Not IsNothing(Me.IdTarifa) Then ret = ret & Me.IdTarifa.GetHashCode().ToString()
        If Not IsNothing(Me.IdPerfilFacturacionPeaje) Then ret = ret & Me.IdPerfilFacturacionPeaje.GetHashCode().ToString()
        If Not IsNothing(Me.IdModeloFactura) Then ret = ret & Me.IdModeloFactura.GetHashCode().ToString()
        If Not IsNothing(Me.RevisionFra) Then ret = ret & Me.RevisionFra.GetHashCode().ToString()
        If Not IsNothing(Me.TextoRevision) Then ret = ret & Me.TextoRevision.GetHashCode().ToString()
        If Not IsNothing(Me.Agregacion) Then ret = ret & Me.Agregacion.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoPuntoMedida) Then ret = ret & Me.IdTipoPuntoMedida.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoAlquiler) Then ret = ret & Me.IdTipoAlquiler.GetHashCode().ToString()
        If Not IsNothing(Me.NoFacturar) Then ret = ret & Me.NoFacturar.GetHashCode().ToString()
        If Not IsNothing(Me.NoCortable) Then ret = ret & Me.NoCortable.GetHashCode().ToString()
        If Not IsNothing(Me.IdCarteraCobroCalendario) Then ret = ret & Me.IdCarteraCobroCalendario.GetHashCode().ToString()
        If Not IsNothing(Me.ConsumoEstimada) Then ret = ret & Me.ConsumoEstimada.GetHashCode().ToString()
        If Not IsNothing(Me.IdContratoTipo) Then ret = ret & Me.IdContratoTipo.GetHashCode().ToString()
        If Not IsNothing(Me.FechaObra) Then ret = ret & Me.FechaObra.GetHashCode().ToString()
        If Not IsNothing(Me.IdTensionSuministro) Then ret = ret & Me.IdTensionSuministro.GetHashCode().ToString()
        If Not IsNothing(Me.IdTensionFase) Then ret = ret & Me.IdTensionFase.GetHashCode().ToString()
        If Not IsNothing(Me.IdDestinoEnergia) Then ret = ret & Me.IdDestinoEnergia.GetHashCode().ToString()
        If Not IsNothing(Me.IdCNAE) Then ret = ret & Me.IdCNAE.GetHashCode().ToString()
        If Not IsNothing(Me.RefExt1) Then ret = ret & Me.RefExt1.GetHashCode().ToString()
        If Not IsNothing(Me.RefExt2) Then ret = ret & Me.RefExt2.GetHashCode().ToString()
        If Not IsNothing(Me.IdGrupoImprimir) Then ret = ret & Me.IdGrupoImprimir.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoCobroGrupo) Then ret = ret & Me.IdTipoCobroGrupo.GetHashCode().ToString()
        If Not IsNothing(Me.TipoContratoTM) Then ret = ret & Me.TipoContratoTM.GetHashCode().ToString()
        If Not IsNothing(Me.CurvaCargaTM) Then ret = ret & Me.CurvaCargaTM.GetHashCode().ToString()
        If Not IsNothing(Me.Observaciones) Then ret = ret & Me.Observaciones.GetHashCode().ToString()
        If Not IsNothing(Me.Representante) Then ret = ret & Me.Representante.GetHashCode().ToString()
        If Not IsNothing(Me.IdColectivoRep) Then ret = ret & Me.IdColectivoRep.GetHashCode().ToString()
        If Not IsNothing(Me.IdentificadorRep) Then ret = ret & Me.IdentificadorRep.GetHashCode().ToString()
        If Not IsNothing(Me.Comentario) Then ret = ret & Me.Comentario.GetHashCode().ToString()
        If Not IsNothing(Me.IdIdiomaInforme) Then ret = ret & Me.IdIdiomaInforme.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoImpuesto) Then ret = ret & Me.IdTipoImpuesto.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoContable) Then ret = ret & Me.CodigoContable.GetHashCode().ToString()
        If Not IsNothing(Me.IdPeriodoFactura) Then ret = ret & Me.IdPeriodoFactura.GetHashCode().ToString()
        If Not IsNothing(Me.TipoImprimir) Then ret = ret & Me.TipoImprimir.GetHashCode().ToString()
        If Not IsNothing(Me.NoRenovar) Then ret = ret & Me.NoRenovar.GetHashCode().ToString()
        If Not IsNothing(Me.EnviarRenovacion) Then ret = ret & Me.EnviarRenovacion.GetHashCode().ToString()
        If Not IsNothing(Me.EnviarPrecios) Then ret = ret & Me.EnviarPrecios.GetHashCode().ToString()
        If Not IsNothing(Me.Empleado) Then ret = ret & Me.Empleado.GetHashCode().ToString()
        If Not IsNothing(Me.CondicionesEsp) Then ret = ret & Me.CondicionesEsp.GetHashCode().ToString()
        If Not IsNothing(Me.IdCalendarioTipo) Then ret = ret & Me.IdCalendarioTipo.GetHashCode().ToString()
        If Not IsNothing(Me.IdUnidadProgramacion) Then ret = ret & Me.IdUnidadProgramacion.GetHashCode().ToString()
        If Not IsNothing(Me.FechaPrevistaActivacion) Then ret = ret & Me.FechaPrevistaActivacion.GetHashCode().ToString()
        If Not IsNothing(Me.FechaPrevistaBaja) Then ret = ret & Me.FechaPrevistaBaja.GetHashCode().ToString()
        If Not IsNothing(Me.IdAgente) Then ret = ret & Me.IdAgente.GetHashCode().ToString()
        If Not IsNothing(Me.IdModeloContrato) Then ret = ret & Me.IdModeloContrato.GetHashCode().ToString()
        If Not IsNothing(Me.ContratoInfoXML) Then ret = ret & Me.ContratoInfoXML.GetHashCode().ToString()
        If Not IsNothing(Me.AltaOV) Then ret = ret & Me.AltaOV.GetHashCode().ToString()
        If Not IsNothing(Me.ModoImportacion) Then ret = ret & Me.ModoImportacion.GetHashCode().ToString()
        If Not IsNothing(Me.ReferenciaBanco) Then ret = ret & Me.ReferenciaBanco.GetHashCode().ToString()
        If Not IsNothing(Me.ReferenciaB2B) Then ret = ret & Me.ReferenciaB2B.GetHashCode().ToString()
        If Not IsNothing(Me.IsFirst) Then ret = ret & Me.IsFirst.GetHashCode().ToString()
        If Not IsNothing(Me.FechaCambioBanco) Then ret = ret & Me.FechaCambioBanco.GetHashCode().ToString()
        If Not IsNothing(Me.CoreFechaFirma) Then ret = ret & Me.CoreFechaFirma.GetHashCode().ToString()
        If Not IsNothing(Me.B2BFechaFirma) Then ret = ret & Me.B2BFechaFirma.GetHashCode().ToString()
        If Not IsNothing(Me.IdSolicitudTipo) Then ret = ret & Me.IdSolicitudTipo.GetHashCode().ToString()
        If Not IsNothing(Me.IsSolicitudUrgente) Then ret = ret & Me.IsSolicitudUrgente.GetHashCode().ToString()
        If Not IsNothing(Me.IdSolicitudTipoFechaEfecto) Then ret = ret & Me.IdSolicitudTipoFechaEfecto.GetHashCode().ToString()
        If Not IsNothing(Me.SinGastoImpago) Then ret = ret & Me.SinGastoImpago.GetHashCode().ToString()
        If Not IsNothing(Me.IdPresion) Then ret = ret & Me.IdPresion.GetHashCode().ToString()
        If Not IsNothing(Me.IdSolicitudCambioTitular) Then ret = ret & Me.IdSolicitudCambioTitular.GetHashCode().ToString()
        If Not IsNothing(Me.NoCedeCUPS) Then ret = ret & Me.NoCedeCUPS.GetHashCode().ToString()
        If Not IsNothing(Me.NoEnviarInformacion) Then ret = ret & Me.NoEnviarInformacion.GetHashCode().ToString()
        If Not IsNothing(Me.IdClientePagoDerecho) Then ret = ret & Me.IdClientePagoDerecho.GetHashCode().ToString()
        If Not IsNothing(Me.CedulaHabitabilidad) Then ret = ret & Me.CedulaHabitabilidad.GetHashCode().ToString()
        If Not IsNothing(Me.FechaCedulaHabitabilidad) Then ret = ret & Me.FechaCedulaHabitabilidad.GetHashCode().ToString()
        If Not IsNothing(Me.IdAdministrador) Then ret = ret & Me.IdAdministrador.GetHashCode().ToString()
        If Not IsNothing(Me.NumeroFinca) Then ret = ret & Me.NumeroFinca.GetHashCode().ToString()
        If Not IsNothing(Me.ConformidadCliente) Then ret = ret & Me.ConformidadCliente.GetHashCode().ToString()
        If Not IsNothing(Me.IncondicionalPS) Then ret = ret & Me.IncondicionalPS.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoOficinaContable) Then ret = ret & Me.CodigoOficinaContable.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoOrganoGestor) Then ret = ret & Me.CodigoOrganoGestor.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoUnidadTramitadora) Then ret = ret & Me.CodigoUnidadTramitadora.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoTipoEstimacion) Then ret = ret & Me.CodigoTipoEstimacion.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoEscaladoConsumo) Then ret = ret & Me.CodigoEscaladoConsumo.GetHashCode().ToString()
        If Not IsNothing(Me.ConsumoEstimado) Then ret = ret & Me.ConsumoEstimado.GetHashCode().ToString()
        If Not IsNothing(Me.IdContratoTipoEsencial) Then ret = ret & Me.IdContratoTipoEsencial.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoCicloHorario) Then ret = ret & Me.IdTipoCicloHorario.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoEquipamiento) Then ret = ret & Me.IdTipoEquipamiento.GetHashCode().ToString()
        If Not IsNothing(Me.TiempoAutonomiaEquipo) Then ret = ret & Me.TiempoAutonomiaEquipo.GetHashCode().ToString()
        If Not IsNothing(Me.ConfidencialidadDatosRPE) Then ret = ret & Me.ConfidencialidadDatosRPE.GetHashCode().ToString()
        If Not IsNothing(Me.ConfidencialidadDatosTarifaSocial) Then ret = ret & Me.ConfidencialidadDatosTarifaSocial.GetHashCode().ToString()
        If Not IsNothing(Me.LecturaExtraordinaria) Then ret = ret & Me.LecturaExtraordinaria.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoDocumentacion) Then ret = ret & Me.IdTipoDocumentacion.GetHashCode().ToString()
        If Not IsNothing(Me.UrlDocumento) Then ret = ret & Me.UrlDocumento.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioCapacidadEntrada) Then ret = ret & Me.PrecioCapacidadEntrada.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioCapacidadSalida) Then ret = ret & Me.PrecioCapacidadSalida.GetHashCode().ToString()
        If Not IsNothing(Me.IdSolicitudMotivoModificacion) Then ret = ret & Me.IdSolicitudMotivoModificacion.GetHashCode().ToString()
        If Not IsNothing(Me.IdModeloFacturaVarios) Then ret = ret & Me.IdModeloFacturaVarios.GetHashCode().ToString()
        If Not IsNothing(Me.SituacionScoring) Then ret = ret & Me.SituacionScoring.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoAutoconsumo) Then ret = ret & Me.IdTipoAutoconsumo.GetHashCode().ToString()
        If Not IsNothing(Me.BloqueoSistemaComercial) Then ret = ret & Me.BloqueoSistemaComercial.GetHashCode().ToString()
        If Not IsNothing(Me.BloqueoPublicidadEmpresa) Then ret = ret & Me.BloqueoPublicidadEmpresa.GetHashCode().ToString()
        If Not IsNothing(Me.BloqueoUsoTercero) Then ret = ret & Me.BloqueoUsoTercero.GetHashCode().ToString()
        If Not IsNothing(Me.IdOfertaLote) Then ret = ret & Me.IdOfertaLote.GetHashCode().ToString()
        If Not IsNothing(Me.IsBonoSocial) Then ret = ret & Me.IsBonoSocial.GetHashCode().ToString()
        If Not IsNothing(Me.IsContratoBonificado) Then ret = ret & Me.IsContratoBonificado.GetHashCode().ToString()
        If Not IsNothing(Me.IdCanal) Then ret = ret & Me.IdCanal.GetHashCode().ToString()
        If Not IsNothing(Me.IsBajaAnticipada) Then ret = ret & Me.IsBajaAnticipada.GetHashCode().ToString()
        If Not IsNothing(Me.FechaBajaAnticipada) Then ret = ret & Me.FechaBajaAnticipada.GetHashCode().ToString()
        If Not IsNothing(Me.IdMotivoBaja) Then ret = ret & Me.IdMotivoBaja.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoOrigenBaja) Then ret = ret & Me.CodigoOrigenBaja.GetHashCode().ToString()
        If Not IsNothing(Me.IdTarifaPeajeAnual) Then ret = ret & Me.IdTarifaPeajeAnual.GetHashCode().ToString()
        If Not IsNothing(Me.TipoLecturaContrato) Then ret = ret & Me.TipoLecturaContrato.GetHashCode().ToString()
        If Not IsNothing(Me.TipoTension) Then ret = ret & Me.TipoTension.GetHashCode().ToString()
        If Not IsNothing(Me.NumPedidoFacturacion) Then ret = ret & Me.NumPedidoFacturacion.GetHashCode().ToString()
        If Not IsNothing(Me.IdAplicacionIH) Then ret = ret & Me.IdAplicacionIH.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarRegasificacion) Then ret = ret & Me.AplicarRegasificacion.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarReajusteTarifa) Then ret = ret & Me.AplicarReajusteTarifa.GetHashCode().ToString()
        If Not IsNothing(Me.TarifarPorB70) Then ret = ret & Me.TarifarPorB70.GetHashCode().ToString()
        If Not IsNothing(Me.IdModoControlPotencia) Then ret = ret & Me.IdModoControlPotencia.GetHashCode().ToString()
        If Not IsNothing(Me.OrdenLibreFacturas) Then ret = ret & Me.OrdenLibreFacturas.GetHashCode().ToString()
        If Not IsNothing(Me.PermitirFacturarA7) Then ret = ret & Me.PermitirFacturarA7.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoPromocional) Then ret = ret & Me.CodigoPromocional.GetHashCode().ToString()
        If Not IsNothing(Me.IsNoInformarASNEF) Then ret = ret & Me.IsNoInformarASNEF.GetHashCode().ToString()
        If Not IsNothing(Me.NumProveedor) Then ret = ret & Me.NumProveedor.GetHashCode().ToString()
        If Not IsNothing(Me.IsRenovacionProcesada) Then ret = ret & Me.IsRenovacionProcesada.GetHashCode().ToString()
        If Not IsNothing(Me.FechaAnulacion) Then ret = ret & Me.FechaAnulacion.GetHashCode().ToString()
        If Not IsNothing(Me.ImporteCargoCuenta) Then ret = ret & Me.ImporteCargoCuenta.GetHashCode().ToString()
        If Not IsNothing(Me.TextoLibreFacturas) Then ret = ret & Me.TextoLibreFacturas.GetHashCode().ToString()
        If Not IsNothing(Me.IsAgruparFacturas) Then ret = ret & Me.IsAgruparFacturas.GetHashCode().ToString()
        If Not IsNothing(Me.IdMarca) Then ret = ret & Me.IdMarca.GetHashCode().ToString()
        If Not IsNothing(Me.IsUnificarCuentaBancaria) Then ret = ret & Me.IsUnificarCuentaBancaria.GetHashCode().ToString()
        If Not IsNothing(Me.IsTelemedido) Then ret = ret & Me.IsTelemedido.GetHashCode().ToString()
        If Not IsNothing(Me.FechaAplicacionPrecios) Then ret = ret & Me.FechaAplicacionPrecios.GetHashCode().ToString()
        If Not IsNothing(Me.Procedencia) Then ret = ret & Me.Procedencia.GetHashCode().ToString()
        If Not IsNothing(Me.Subprocedencia) Then ret = ret & Me.Subprocedencia.GetHashCode().ToString()
        If Not IsNothing(Me.ATRDirecto) Then ret = ret & Me.ATRDirecto.GetHashCode().ToString()
        If Not IsNothing(Me.PorcentajePerdidasPropio) Then ret = ret & Me.PorcentajePerdidasPropio.GetHashCode().ToString()
        If Not IsNothing(Me.EmailRep) Then ret = ret & Me.EmailRep.GetHashCode().ToString()
        If Not IsNothing(Me.SMSRep) Then ret = ret & Me.SMSRep.GetHashCode().ToString()
        If Not IsNothing(Me.ObtenerLiquidacion) Then ret = ret & Me.ObtenerLiquidacion.GetHashCode().ToString()
        If Not IsNothing(Me.OrderId) Then ret = ret & Me.OrderId.GetHashCode().ToString()
        If Not IsNothing(Me.IsContratoRenovado) Then ret = ret & Me.IsContratoRenovado.GetHashCode().ToString()
        If Not IsNothing(Me.Vulnerabilidad) Then ret = ret & Me.Vulnerabilidad.GetHashCode().ToString()
        If Not IsNothing(Me.FechaInicioVulnerabilidad) Then ret = ret & Me.FechaInicioVulnerabilidad.GetHashCode().ToString()
        If Not IsNothing(Me.FechaFinVulnerabilidad) Then ret = ret & Me.FechaFinVulnerabilidad.GetHashCode().ToString()
        If Not IsNothing(Me.EnvioMinisterio) Then ret = ret & Me.EnvioMinisterio.GetHashCode().ToString()
        If Not IsNothing(Me.NumMenores) Then ret = ret & Me.NumMenores.GetHashCode().ToString()
        If Not IsNothing(Me.FechaXML) Then ret = ret & Me.FechaXML.GetHashCode().ToString()
        If Not IsNothing(Me.SegmentoBS) Then ret = ret & Me.SegmentoBS.GetHashCode().ToString()
        If Not IsNothing(Me.FechaNacimientoMenor) Then ret = ret & Me.FechaNacimientoMenor.GetHashCode().ToString()
        If Not IsNothing(Me.AyudaBS) Then ret = ret & Me.AyudaBS.GetHashCode().ToString()
        If Not IsNothing(Me.IdSwitchDocumentSalesforce) Then ret = ret & Me.IdSwitchDocumentSalesforce.GetHashCode().ToString()
        If Not IsNothing(Me.IdOrderSalesforce) Then ret = ret & Me.IdOrderSalesforce.GetHashCode().ToString()
        If Not IsNothing(Me.DiasVencimiento) Then ret = ret & Me.DiasVencimiento.GetHashCode().ToString()
        If Not IsNothing(Me.IsContratoRevisado) Then ret = ret & Me.IsContratoRevisado.GetHashCode().ToString()
        If Not IsNothing(Me.FechaCreacion) Then ret = ret & Me.FechaCreacion.GetHashCode().ToString()
        If Not IsNothing(Me.IsDual) Then ret = ret & Me.IsDual.GetHashCode().ToString()
        If Not IsNothing(Me.IsProductoClick) Then ret = ret & Me.IsProductoClick.GetHashCode().ToString()
        If Not IsNothing(Me.IsFirmaDigitalEnviada) Then ret = ret & Me.IsFirmaDigitalEnviada.GetHashCode().ToString()
        If Not IsNothing(Me.IsFirmadoDigitalmente) Then ret = ret & Me.IsFirmadoDigitalmente.GetHashCode().ToString()
        If Not IsNothing(Me.Reqqd) Then ret = ret & Me.Reqqd.GetHashCode().ToString()
        If Not IsNothing(Me.Reqqh) Then ret = ret & Me.Reqqh.GetHashCode().ToString()
        If Not IsNothing(Me.IsContratoAutonomo) Then ret = ret & Me.IsContratoAutonomo.GetHashCode().ToString()
        If Not IsNothing(Me.IdCanalCodage) Then ret = ret & Me.IdCanalCodage.GetHashCode().ToString()
        If Not IsNothing(Me.AutorizarFidelizacion) Then ret = ret & Me.AutorizarFidelizacion.GetHashCode().ToString()
        If Not IsNothing(Me.CederDatosEmpresasGrupoTotal) Then ret = ret & Me.CederDatosEmpresasGrupoTotal.GetHashCode().ToString()

        Return ret.GetHashCode()
    End Function

    Public Property IDEntityDTO As Long
        Get
            Return IdContrato
        End Get
        Set(value As Long)
            IdContrato = value
        End Set
    End Property

    Protected Function IsEqual(a As Object, b As Object) As Boolean
        Dim ret As Boolean = False

        Dim tipo As System.Type = Nothing
        If Not IsNothing(a) Then
            tipo = a.GetType()
        End If
        If IsNothing(tipo) Then
            If IsNothing(b) Then
                Return True
            Else
                tipo = b.GetType()
            End If
        Else
            If Not IsNothing(b) AndAlso Not (tipo Is b.GetType) Then
                Return False
            End If
        End If

        Select Case tipo
            Case GetType(Int16), GetType(Int32), GetType(Int64), GetType(Decimal)
                If IsNothing(a) Then
                    ret = (Convert.ToDecimal(b) = 0)
                Else
                    If IsNothing(b) Then
                        ret = (Convert.ToDecimal(a) = 0)
                    Else
                        ret = (Convert.ToDecimal(a) = Convert.ToDecimal(b))
                    End If
                End If
            Case GetType(String)
                If IsNothing(a) Then
                    ret = b.Equals(String.Empty)
                Else
                    If IsNothing(b) Then
                        ret = a.Equals(String.Empty)
                    Else
                        ret = a.Equals(b)
                    End If
                End If
            Case GetType(Boolean)
                If IsNothing(a) Then
                    ret = b.Equals(False)
                Else
                    If IsNothing(b) Then
                        ret = a.Equals(False)
                    Else
                        ret = a.Equals(b)
                    End If
                End If
            Case GetType(Byte())
                If IsNothing(a) Then
                    ret = b.Equals(Nothing)
                Else
                    If IsNothing(b) Then
                        ret = a.Equals(Nothing)
                    Else
                        ret = a.Equals(b)
                    End If
                End If
            Case GetType(Date)
                If IsNothing(a) Then
                    ret = b.Equals(Nothing)
                Else
                    If IsNothing(b) Then
                        ret = a.Equals(Nothing)
                    Else
                        ret = a.Equals(b)
                    End If
                End If
            Case Else
                ret = False
        End Select
        Return ret
    End Function

    Private _Entorno As String
    Public Property Entorno As String
        Get
            Return _Entorno
        End Get
        Set(ByVal value As String)
            _Entorno = value
        End Set
    End Property

    Private _CodigoContrato As Nullable(Of Long)
    Public Property CodigoContrato As Nullable(Of Long)
        Get
            Return _CodigoContrato
        End Get
        Set(ByVal value As Nullable(Of Long))
            _CodigoContrato = value
        End Set
    End Property

    Private _IdContrato As Long
    Public Property IdContrato As Long
        Get
            Return _IdContrato
        End Get
        Set(ByVal value As Long)
            _IdContrato = value
        End Set
    End Property

    Private _FechaAlta As Nullable(Of Date)
    Public Property FechaAlta As Nullable(Of Date)
        Get
            Return _FechaAlta
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaAlta = value
        End Set
    End Property

    Private _IdCliente As Long
    Public Property IdCliente As Long
        Get
            Return _IdCliente
        End Get
        Set(ByVal value As Long)
            _IdCliente = value
        End Set
    End Property

    Private _Representante As String
    Public Property Representante As String
        Get
            Return _Representante
        End Get
        Set(ByVal value As String)
            _Representante = value
        End Set
    End Property

    Private _IdentificadorRep As String
    Public Property IdentificadorRep As String
        Get
            Return _IdentificadorRep
        End Get
        Set(ByVal value As String)
            _IdentificadorRep = value
        End Set
    End Property

    Private _EmailRep As String
    Public Property EmailRep As String
        Get
            Return _EmailRep
        End Get
        Set(ByVal value As String)
            _EmailRep = value
        End Set
    End Property

    Private _SMSRep As String
    Public Property SMSRep As String
        Get
            Return _SMSRep
        End Get
        Set(ByVal value As String)
            _SMSRep = value
        End Set
    End Property

    Private _ContratoPDF As Byte()
    Public Property ContratoPDF As Byte()
        Get
            Return _ContratoPDF
        End Get
        Set(ByVal value As Byte())
            _ContratoPDF = value
        End Set
    End Property

    Private _IdModeloContrato As Nullable(Of Long)
    Public Property IdModeloContrato As Nullable(Of Long)
        Get
            Return _IdModeloContrato
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdModeloContrato = value
        End Set
    End Property

    Private _IdIdiomaInforme As Nullable(Of Long)
    Public Property IdIdiomaInforme As Nullable(Of Long)
        Get
            Return _IdIdiomaInforme
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdIdiomaInforme = value
        End Set
    End Property

    Private _FechaVto As Nullable(Of Date)
    Public Property FechaVto As Nullable(Of Date)
        Get
            Return _FechaVto
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaVto = value
        End Set
    End Property

    Private _ConsumoAnualEstimado As Nullable(Of Decimal)
    Public Property ConsumoAnualEstimado As Nullable(Of Decimal)
        Get
            Return _ConsumoAnualEstimado
        End Get
        Set(value As Nullable(Of Decimal))
            _ConsumoAnualEstimado = value
        End Set
    End Property

    Private _ConsumoAnualEstimadoP1 As Nullable(Of Decimal)
    Public Property ConsumoAnualEstimadoP1 As Nullable(Of Decimal)
        Get
            Return _ConsumoAnualEstimadoP1
        End Get
        Set(value As Nullable(Of Decimal))
            _ConsumoAnualEstimadoP1 = value
        End Set
    End Property

    Private _ConsumoAnualEstimadoP2 As Nullable(Of Decimal)
    Public Property ConsumoAnualEstimadoP2 As Nullable(Of Decimal)
        Get
            Return _ConsumoAnualEstimadoP2
        End Get
        Set(value As Nullable(Of Decimal))
            _ConsumoAnualEstimadoP2 = value
        End Set
    End Property

    Private _ConsumoAnualEstimadoP3 As Nullable(Of Decimal)
    Public Property ConsumoAnualEstimadoP3 As Nullable(Of Decimal)
        Get
            Return _ConsumoAnualEstimadoP3
        End Get
        Set(value As Nullable(Of Decimal))
            _ConsumoAnualEstimadoP3 = value
        End Set
    End Property

    Private _ConsumoAnualEstimadoP4 As Nullable(Of Decimal)
    Public Property ConsumoAnualEstimadoP4 As Nullable(Of Decimal)
        Get
            Return _ConsumoAnualEstimadoP4
        End Get
        Set(value As Nullable(Of Decimal))
            _ConsumoAnualEstimadoP4 = value
        End Set
    End Property

    Private _ConsumoAnualEstimadoP5 As Nullable(Of Decimal)
    Public Property ConsumoAnualEstimadoP5 As Nullable(Of Decimal)
        Get
            Return _ConsumoAnualEstimadoP5
        End Get
        Set(value As Nullable(Of Decimal))
            _ConsumoAnualEstimadoP5 = value

        End Set
    End Property

    Private _ConsumoAnualEstimadoP6 As Nullable(Of Decimal)
    Public Property ConsumoAnualEstimadoP6 As Nullable(Of Decimal)
        Get
            Return _ConsumoAnualEstimadoP6
        End Get
        Set(value As Nullable(Of Decimal))
            _ConsumoAnualEstimadoP6 = value
        End Set
    End Property

    Private _IdClientePago As Long
    Public Property IdClientePago As Long
        Get
            Return _IdClientePago
        End Get
        Set(ByVal value As Long)
            _IdClientePago = value
        End Set
    End Property

    Private _IdTarifa As Nullable(Of Long)
    Public Property IdTarifa As Nullable(Of Long)
        Get
            Return _IdTarifa
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTarifa = value
        End Set
    End Property

    'Private _Cliente As Cliente = New Cliente
    'Public Property Cliente As Cliente
    '    Get
    '        Return _Cliente
    '    End Get
    '    Set(value As Cliente)
    '        _Cliente = value
    '    End Set
    'End Property

    'Private Property _ContratoInfo As ContratoInfoDTO
    'Public Property ContratoInfo As ContratoInfoDTO
    '    Get
    '        Return _ContratoInfo
    '    End Get
    '    Set(ByVal value As ContratoInfoDTO)
    '        _ContratoInfo = value
    '    End Set
    'End Property

    'Private Property _CUPS As CUPS = New CUPS
    'Public Property CUPS As CUPS
    '    Get
    '        Return _CUPS
    '    End Get
    '    Set(value As CUPS)
    '        _CUPS = value
    '    End Set
    'End Property

    'Private _Comercializadora As Comercializadora

    'Public Property Comercializadora As Comercializadora
    '    Get
    '        Return _Comercializadora
    '    End Get
    '    Set(value As Comercializadora)
    '        _Comercializadora = value
    '    End Set
    'End Property

    'Private _ContratoPotencia As List(Of ContratoPotencia)

    'Public Property ContratoPotencia As List(Of ContratoPotencia)
    '    Get
    '        Return _ContratoPotencia
    '    End Get
    '    Set(value As List(Of ContratoPotencia))
    '        _ContratoPotencia = value
    '    End Set
    'End Property

    Private _IdCalendarioTipo As Nullable(Of Long)
    Public Property IdCalendarioTipo As Nullable(Of Long)
        Get
            Return _IdCalendarioTipo
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCalendarioTipo = value
        End Set
    End Property

    Private _IdTarifaPeaje As Nullable(Of Long)
    Public Property IdTarifaPeaje As Nullable(Of Long)
        Get
            Return _IdTarifaPeaje
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTarifaPeaje = value
        End Set
    End Property

    Private _IdTipoAlquiler As Nullable(Of Long)
    Public Property IdTipoAlquiler As Nullable(Of Long)
        Get
            Return _IdTipoAlquiler
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoAlquiler = value
        End Set
    End Property

    Private _IdContratoTipo As Nullable(Of Long)
    Public Property IdContratoTipo As Nullable(Of Long)
        Get
            Return _IdContratoTipo
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdContratoTipo = value
        End Set
    End Property



    Private _Version As Nullable(Of Long)

    Public Property Version As Nullable(Of Long)
        Get
            Return _Version
        End Get
        Set(ByVal value As Nullable(Of Long))
            _Version = value
        End Set
    End Property

    Private _Confirmado As Nullable(Of Boolean)
    Public Property Confirmado As Nullable(Of Boolean)
        Get
            Return _Confirmado
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _Confirmado = value
        End Set
    End Property



    Private _IdCups As Nullable(Of Long)
    Public Property IdCups As Nullable(Of Long)
        Get
            Return _IdCups
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCups = value
        End Set
    End Property

    Private _IdComercializadora As Nullable(Of Long)
    Public Property IdComercializadora As Nullable(Of Long)
        Get
            Return _IdComercializadora
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdComercializadora = value
        End Set
    End Property

    Private _FechaContrato As Nullable(Of Date)
    Public Property FechaContrato As Nullable(Of Date)
        Get
            Return _FechaContrato
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaContrato = value
        End Set
    End Property


    Private _FechaBaja As Nullable(Of Date)
    Public Property FechaBaja As Nullable(Of Date)
        Get
            Return _FechaBaja
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaBaja = value
        End Set
    End Property


    Private _FechaInstalacion As Nullable(Of Date)
    Public Property FechaInstalacion As Nullable(Of Date)
        Get
            Return _FechaInstalacion
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaInstalacion = value
        End Set
    End Property

    Private _IdContratoSituacion As Nullable(Of Long)
    Public Property IdContratoSituacion As Nullable(Of Long)
        Get
            Return _IdContratoSituacion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdContratoSituacion = value
        End Set
    End Property

    Private _Poliza As String
    Public Property Poliza As String
        Get
            Return _Poliza
        End Get
        Set(ByVal value As String)
            _Poliza = value
        End Set
    End Property

    Private _TipoContrato As String
    Public Property TipoContrato As String
        Get
            Return _TipoContrato
        End Get
        Set(ByVal value As String)
            _TipoContrato = value
        End Set
    End Property

    Private _Aclarador As String
    Public Property Aclarador As String
        Get
            Return _Aclarador
        End Get
        Set(ByVal value As String)
            _Aclarador = value
        End Set
    End Property

    Private _DirecionUnica As String
    Public Property DirecionUnica As String
        Get
            Return _DirecionUnica
        End Get
        Set(ByVal value As String)
            _DirecionUnica = value
        End Set
    End Property

    Private _IdClienteEnvio As Long
    Public Property IdClienteEnvio As Long
        Get
            Return _IdClienteEnvio
        End Get
        Set(ByVal value As Long)
            _IdClienteEnvio = value
        End Set
    End Property

    Private _IdDestinoEnergia As Nullable(Of Long)
    Public Property IdDestinoEnergia As Nullable(Of Long)
        Get
            Return _IdDestinoEnergia
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdDestinoEnergia = value
        End Set
    End Property

    Private _IdTensionSuministro As Nullable(Of Long)
    Public Property IdTensionSuministro As Nullable(Of Long)
        Get
            Return _IdTensionSuministro
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTensionSuministro = value
        End Set
    End Property

    Private _IdTensionFase As Nullable(Of Long)
    Public Property IdTensionFase As Nullable(Of Long)
        Get
            Return _IdTensionFase
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTensionFase = value
        End Set
    End Property

    Private _ContratoInfoXML As String
    Public Property ContratoInfoXML As String
        Get
            Return _ContratoInfoXML
        End Get
        Set(ByVal value As String)
            _ContratoInfoXML = value
        End Set
    End Property



    Private _NombreInstalacion As String
    Public Property NombreInstalacion As String
        Get
            Return _NombreInstalacion
        End Get
        Set(ByVal value As String)
            _NombreInstalacion = value
        End Set
    End Property

    Private _IdCiudadInstalacion As Nullable(Of Long)
    Public Property IdCiudadInstalacion As Nullable(Of Long)
        Get
            Return _IdCiudadInstalacion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCiudadInstalacion = value
        End Set
    End Property

    Private _IdCallejeroInstalacion As Nullable(Of Long)
    Public Property IdCallejeroInstalacion As Nullable(Of Long)
        Get
            Return _IdCallejeroInstalacion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCallejeroInstalacion = value
        End Set
    End Property

    Private _NumeroInstalacion As Nullable(Of Integer)
    Public Property NumeroInstalacion As Nullable(Of Integer)
        Get
            Return _NumeroInstalacion
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _NumeroInstalacion = value
        End Set
    End Property

    Private _AclaradorInstalacion As String
    Public Property AclaradorInstalacion As String
        Get
            Return _AclaradorInstalacion
        End Get
        Set(ByVal value As String)
            _AclaradorInstalacion = value
        End Set
    End Property

    Private _CodPostalInstalacion As String
    Public Property CodPostalInstalacion As String
        Get
            Return _CodPostalInstalacion
        End Get
        Set(ByVal value As String)
            _CodPostalInstalacion = value
        End Set
    End Property

    Private _NIFInstalacion As String
    Public Property NIFInstalacion As String
        Get
            Return _NIFInstalacion
        End Get
        Set(ByVal value As String)
            _NIFInstalacion = value
        End Set
    End Property

    Private _TelefonoInstalacion As String
    Public Property TelefonoInstalacion As String
        Get
            Return _TelefonoInstalacion
        End Get
        Set(ByVal value As String)
            _TelefonoInstalacion = value
        End Set
    End Property

    Private _MovilInstalacion As String
    Public Property MovilInstalacion As String
        Get
            Return _MovilInstalacion
        End Get
        Set(ByVal value As String)
            _MovilInstalacion = value
        End Set
    End Property

    Private _FaxInstalacion As String
    Public Property FaxInstalacion As String
        Get
            Return _FaxInstalacion
        End Get
        Set(ByVal value As String)
            _FaxInstalacion = value
        End Set
    End Property

    Private _EmailInstalacion As String
    Public Property EmailInstalacion As String
        Get
            Return _EmailInstalacion
        End Get
        Set(ByVal value As String)
            _EmailInstalacion = value
        End Set
    End Property

    Private _IdPerfilFacturacionPeaje As Nullable(Of Long)
    Public Property IdPerfilFacturacionPeaje As Nullable(Of Long)
        Get
            Return _IdPerfilFacturacionPeaje
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdPerfilFacturacionPeaje = value
        End Set
    End Property

    Private _IdModeloFactura As Nullable(Of Long)
    Public Property IdModeloFactura As Nullable(Of Long)
        Get
            Return _IdModeloFactura
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdModeloFactura = value
        End Set
    End Property

    Private _RevisionFra As Nullable(Of Boolean)
    Public Property RevisionFra As Nullable(Of Boolean)
        Get
            Return _RevisionFra
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _RevisionFra = value
        End Set
    End Property

    Private _TextoRevision As String
    Public Property TextoRevision As String
        Get
            Return _TextoRevision
        End Get
        Set(ByVal value As String)
            _TextoRevision = value
        End Set
    End Property

    Private _Agregacion As String
    Public Property Agregacion As String
        Get
            Return _Agregacion
        End Get
        Set(ByVal value As String)
            _Agregacion = value
        End Set
    End Property

    Private _IdTipoPuntoMedida As Nullable(Of Long)
    Public Property IdTipoPuntoMedida As Nullable(Of Long)
        Get
            Return _IdTipoPuntoMedida
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoPuntoMedida = value
        End Set
    End Property

    Private _NoFacturar As Nullable(Of Boolean)
    Public Property NoFacturar As Nullable(Of Boolean)
        Get
            Return _NoFacturar
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _NoFacturar = value
        End Set
    End Property

    Private _NoCortable As Nullable(Of Boolean)
    Public Property NoCortable As Nullable(Of Boolean)
        Get
            Return _NoCortable
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _NoCortable = value
        End Set
    End Property

    Private _IdCarteraCobroCalendario As Nullable(Of Long)
    Public Property IdCarteraCobroCalendario As Nullable(Of Long)
        Get
            Return _IdCarteraCobroCalendario
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCarteraCobroCalendario = value
        End Set
    End Property

    Private _ConsumoEstimada As String
    Public Property ConsumoEstimada As String
        Get
            Return _ConsumoEstimada
        End Get
        Set(ByVal value As String)
            _ConsumoEstimada = value
        End Set
    End Property

    Private _FechaObra As Nullable(Of Date)
    Public Property FechaObra As Nullable(Of Date)
        Get
            Return _FechaObra
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaObra = value
        End Set
    End Property

    Private _IdCNAE As Nullable(Of Long)
    Public Property IdCNAE As Nullable(Of Long)
        Get
            Return _IdCNAE
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCNAE = value
        End Set
    End Property

    Private _RefExt1 As String
    Public Property RefExt1 As String
        Get
            Return _RefExt1
        End Get
        Set(ByVal value As String)
            _RefExt1 = value
        End Set
    End Property

    Private _RefExt2 As String
    Public Property RefExt2 As String
        Get
            Return _RefExt2
        End Get
        Set(ByVal value As String)
            _RefExt2 = value
        End Set
    End Property

    Private _IdGrupoImprimir As Nullable(Of Long)
    Public Property IdGrupoImprimir As Nullable(Of Long)
        Get
            Return _IdGrupoImprimir
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdGrupoImprimir = value
        End Set
    End Property

    Private _IdTipoCobroGrupo As Nullable(Of Long)
    Public Property IdTipoCobroGrupo As Nullable(Of Long)
        Get
            Return _IdTipoCobroGrupo
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoCobroGrupo = value
        End Set
    End Property

    Private _TipoContratoTM As Nullable(Of Integer)
    Public Property TipoContratoTM As Nullable(Of Integer)
        Get
            Return _TipoContratoTM
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _TipoContratoTM = value
        End Set
    End Property

    Private _CurvaCargaTM As String
    Public Property CurvaCargaTM As String
        Get
            Return _CurvaCargaTM
        End Get
        Set(ByVal value As String)
            _CurvaCargaTM = value
        End Set
    End Property

    Private _Observaciones As String
    Public Property Observaciones As String
        Get
            Return _Observaciones
        End Get
        Set(ByVal value As String)
            _Observaciones = value
        End Set
    End Property

    Private _IdColectivoRep As Nullable(Of Long)
    Public Property IdColectivoRep As Nullable(Of Long)
        Get
            Return _IdColectivoRep
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdColectivoRep = value
        End Set
    End Property

    Private _Comentario As String
    Public Property Comentario As String
        Get
            Return _Comentario
        End Get
        Set(ByVal value As String)
            _Comentario = value
        End Set
    End Property

    Private _IdTipoImpuesto As Nullable(Of Long)
    Public Property IdTipoImpuesto As Nullable(Of Long)
        Get
            Return _IdTipoImpuesto
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoImpuesto = value
        End Set
    End Property

    Private _CodigoContable As String
    Public Property CodigoContable As String
        Get
            Return _CodigoContable
        End Get
        Set(ByVal value As String)
            _CodigoContable = value
        End Set
    End Property

    Private _IdPeriodoFactura As Nullable(Of Long)
    Public Property IdPeriodoFactura As Nullable(Of Long)
        Get
            Return _IdPeriodoFactura
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdPeriodoFactura = value
        End Set
    End Property

    Private _TipoImprimir As String
    Public Property TipoImprimir As String
        Get
            Return _TipoImprimir
        End Get
        Set(ByVal value As String)
            _TipoImprimir = value
        End Set
    End Property

    Private _NoRenovar As Nullable(Of Boolean)
    Public Property NoRenovar As Nullable(Of Boolean)
        Get
            Return _NoRenovar
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _NoRenovar = value
        End Set
    End Property

    Private _EnviarRenovacion As Nullable(Of Boolean)
    Public Property EnviarRenovacion As Nullable(Of Boolean)
        Get
            Return _EnviarRenovacion
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _EnviarRenovacion = value
        End Set
    End Property

    Private _EnviarPrecios As Nullable(Of Boolean)
    Public Property EnviarPrecios As Nullable(Of Boolean)
        Get
            Return _EnviarPrecios
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _EnviarPrecios = value
        End Set
    End Property

    Private _Empleado As Nullable(Of Boolean)
    Public Property Empleado As Nullable(Of Boolean)
        Get
            Return _Empleado
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _Empleado = value
        End Set
    End Property

    Private _CondicionesEsp As String
    Public Property CondicionesEsp As String
        Get
            Return _CondicionesEsp
        End Get
        Set(ByVal value As String)
            _CondicionesEsp = value
        End Set
    End Property

    Private _IdUnidadProgramacion As Nullable(Of Long)
    Public Property IdUnidadProgramacion As Nullable(Of Long)
        Get
            Return _IdUnidadProgramacion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdUnidadProgramacion = value
        End Set
    End Property

    Private _FechaPrevistaActivacion As Nullable(Of Date)
    Public Property FechaPrevistaActivacion As Nullable(Of Date)
        Get
            Return _FechaPrevistaActivacion
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaPrevistaActivacion = value
        End Set
    End Property

    Private _FechaPrevistaBaja As Nullable(Of Date)
    Public Property FechaPrevistaBaja As Nullable(Of Date)
        Get
            Return _FechaPrevistaBaja
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaPrevistaBaja = value
        End Set
    End Property

    Private _IdAgente As Nullable(Of Long)
    Public Property IdAgente As Nullable(Of Long)
        Get
            Return _IdAgente
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdAgente = value
        End Set
    End Property

    Private _AltaOV As Nullable(Of Boolean)
    Public Property AltaOV As Nullable(Of Boolean)
        Get
            Return _AltaOV
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AltaOV = value
        End Set
    End Property

    Private _ModoImportacion As String
    Public Property ModoImportacion As String
        Get
            Return _ModoImportacion
        End Get
        Set(ByVal value As String)
            _ModoImportacion = value
        End Set
    End Property

    Private _ReferenciaBanco As String
    Public Property ReferenciaBanco As String
        Get
            Return _ReferenciaBanco
        End Get
        Set(ByVal value As String)
            _ReferenciaBanco = value
        End Set
    End Property

    Private _ReferenciaB2B As String
    Public Property ReferenciaB2B As String
        Get
            Return _ReferenciaB2B
        End Get
        Set(ByVal value As String)
            _ReferenciaB2B = value
        End Set
    End Property

    Private _IsFirst As Nullable(Of Boolean)
    Public Property IsFirst As Nullable(Of Boolean)
        Get
            Return _IsFirst
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsFirst = value
        End Set
    End Property

    Private _FechaCambioBanco As Nullable(Of Date)
    Public Property FechaCambioBanco As Nullable(Of Date)
        Get
            Return _FechaCambioBanco
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaCambioBanco = value
        End Set
    End Property

    Private _CoreFechaFirma As Nullable(Of Date)
    Public Property CoreFechaFirma As Nullable(Of Date)
        Get
            Return _CoreFechaFirma
        End Get
        Set(ByVal value As Nullable(Of Date))
            _CoreFechaFirma = value
        End Set
    End Property

    Private _B2BFechaFirma As Nullable(Of Date)
    Public Property B2BFechaFirma As Nullable(Of Date)
        Get
            Return _B2BFechaFirma
        End Get
        Set(ByVal value As Nullable(Of Date))
            _B2BFechaFirma = value
        End Set
    End Property

    Private _IdSolicitudTipo As Nullable(Of Long)
    Public Property IdSolicitudTipo As Nullable(Of Long)
        Get
            Return _IdSolicitudTipo
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdSolicitudTipo = value
        End Set
    End Property

    Private _IsSolicitudUrgente As Nullable(Of Boolean)
    Public Property IsSolicitudUrgente As Nullable(Of Boolean)
        Get
            Return _IsSolicitudUrgente
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsSolicitudUrgente = value
        End Set
    End Property

    Private _IdSolicitudTipoFechaEfecto As Nullable(Of Long)
    Public Property IdSolicitudTipoFechaEfecto As Nullable(Of Long)
        Get
            Return _IdSolicitudTipoFechaEfecto
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdSolicitudTipoFechaEfecto = value
        End Set
    End Property

    Private _SinGastoImpago As Boolean
    Public Property SinGastoImpago As Boolean
        Get
            Return _SinGastoImpago
        End Get
        Set(ByVal value As Boolean)
            _SinGastoImpago = value
        End Set
    End Property

    Private _IdPresion As Nullable(Of Long)
    Public Property IdPresion As Nullable(Of Long)
        Get
            Return _IdPresion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdPresion = value
        End Set
    End Property

    Private _IdSolicitudCambioTitular As Nullable(Of Long)
    Public Property IdSolicitudCambioTitular As Nullable(Of Long)
        Get
            Return _IdSolicitudCambioTitular
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdSolicitudCambioTitular = value
        End Set
    End Property

    Private _NoCedeCUPS As Boolean
    Public Property NoCedeCUPS As Boolean
        Get
            Return _NoCedeCUPS
        End Get
        Set(ByVal value As Boolean)
            _NoCedeCUPS = value
        End Set
    End Property

    Private _NoEnviarInformacion As Boolean
    Public Property NoEnviarInformacion As Boolean
        Get
            Return _NoEnviarInformacion
        End Get
        Set(ByVal value As Boolean)
            _NoEnviarInformacion = value
        End Set
    End Property

    Private _IdClientePagoDerecho As Nullable(Of Long)
    Public Property IdClientePagoDerecho As Nullable(Of Long)
        Get
            Return _IdClientePagoDerecho
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdClientePagoDerecho = value
        End Set
    End Property

    Private _CedulaHabitabilidad As String
    Public Property CedulaHabitabilidad As String
        Get
            Return _CedulaHabitabilidad
        End Get
        Set(ByVal value As String)
            _CedulaHabitabilidad = value
        End Set
    End Property

    Private _FechaCedulaHabitabilidad As Nullable(Of Date)
    Public Property FechaCedulaHabitabilidad As Nullable(Of Date)
        Get
            Return _FechaCedulaHabitabilidad
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaCedulaHabitabilidad = value
        End Set
    End Property

    Private _IdAdministrador As Nullable(Of Long)
    Public Property IdAdministrador As Nullable(Of Long)
        Get
            Return _IdAdministrador
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdAdministrador = value
        End Set
    End Property

    Private _NumeroFinca As String
    Public Property NumeroFinca As String
        Get
            Return _NumeroFinca
        End Get
        Set(ByVal value As String)
            _NumeroFinca = value
        End Set
    End Property

    Private _ConformidadCliente As Nullable(Of Boolean)
    Public Property ConformidadCliente As Nullable(Of Boolean)
        Get
            Return _ConformidadCliente
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ConformidadCliente = value
        End Set
    End Property

    Private _IncondicionalPS As Nullable(Of Boolean)
    Public Property IncondicionalPS As Nullable(Of Boolean)
        Get
            Return _IncondicionalPS
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IncondicionalPS = value
        End Set
    End Property

    Private _CodigoOficinaContable As String
    Public Property CodigoOficinaContable As String
        Get
            Return _CodigoOficinaContable
        End Get
        Set(ByVal value As String)
            _CodigoOficinaContable = value
        End Set
    End Property

    Private _CodigoOrganoGestor As String
    Public Property CodigoOrganoGestor As String
        Get
            Return _CodigoOrganoGestor
        End Get
        Set(ByVal value As String)
            _CodigoOrganoGestor = value
        End Set
    End Property

    Private _CodigoUnidadTramitadora As String
    Public Property CodigoUnidadTramitadora As String
        Get
            Return _CodigoUnidadTramitadora
        End Get
        Set(ByVal value As String)
            _CodigoUnidadTramitadora = value
        End Set
    End Property

    Private _CodigoTipoEstimacion As Nullable(Of Integer)
    Public Property CodigoTipoEstimacion As Nullable(Of Integer)
        Get
            Return _CodigoTipoEstimacion
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _CodigoTipoEstimacion = value
        End Set
    End Property

    Private _CodigoEscaladoConsumo As Nullable(Of Integer)
    Public Property CodigoEscaladoConsumo As Nullable(Of Integer)
        Get
            Return _CodigoEscaladoConsumo
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _CodigoEscaladoConsumo = value
        End Set
    End Property

    Private _ConsumoEstimado As Nullable(Of Decimal)
    Public Property ConsumoEstimado As Nullable(Of Decimal)
        Get
            Return _ConsumoEstimado
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _ConsumoEstimado = value
        End Set
    End Property

    Private _IdContratoTipoEsencial As Nullable(Of Long)
    Public Property IdContratoTipoEsencial As Nullable(Of Long)
        Get
            Return _IdContratoTipoEsencial
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdContratoTipoEsencial = value
        End Set
    End Property

    Private _IdTipoCicloHorario As Nullable(Of Long)
    Public Property IdTipoCicloHorario As Nullable(Of Long)
        Get
            Return _IdTipoCicloHorario
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoCicloHorario = value
        End Set
    End Property

    Private _IdTipoEquipamiento As Nullable(Of Long)
    Public Property IdTipoEquipamiento As Nullable(Of Long)
        Get
            Return _IdTipoEquipamiento
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoEquipamiento = value
        End Set
    End Property

    Private _TiempoAutonomiaEquipo As String
    Public Property TiempoAutonomiaEquipo As String
        Get
            Return _TiempoAutonomiaEquipo
        End Get
        Set(ByVal value As String)
            _TiempoAutonomiaEquipo = value
        End Set
    End Property

    Private _ConfidencialidadDatosRPE As Nullable(Of Boolean)
    Public Property ConfidencialidadDatosRPE As Nullable(Of Boolean)
        Get
            Return _ConfidencialidadDatosRPE
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ConfidencialidadDatosRPE = value
        End Set
    End Property

    Private _ConfidencialidadDatosTarifaSocial As Nullable(Of Boolean)
    Public Property ConfidencialidadDatosTarifaSocial As Nullable(Of Boolean)
        Get
            Return _ConfidencialidadDatosTarifaSocial
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ConfidencialidadDatosTarifaSocial = value
        End Set
    End Property

    Private _LecturaExtraordinaria As Nullable(Of Boolean)
    Public Property LecturaExtraordinaria As Nullable(Of Boolean)
        Get
            Return _LecturaExtraordinaria
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _LecturaExtraordinaria = value
        End Set
    End Property

    Private _IdTipoDocumentacion As Nullable(Of Long)
    Public Property IdTipoDocumentacion As Nullable(Of Long)
        Get
            Return _IdTipoDocumentacion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoDocumentacion = value
        End Set
    End Property

    Private _UrlDocumento As String
    Public Property UrlDocumento As String
        Get
            Return _UrlDocumento
        End Get
        Set(ByVal value As String)
            _UrlDocumento = value
        End Set
    End Property

    Private _PrecioCapacidadEntrada As Nullable(Of Decimal)
    Public Property PrecioCapacidadEntrada As Nullable(Of Decimal)
        Get
            Return _PrecioCapacidadEntrada
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioCapacidadEntrada = value
        End Set
    End Property

    Private _PrecioCapacidadSalida As Nullable(Of Decimal)
    Public Property PrecioCapacidadSalida As Nullable(Of Decimal)
        Get
            Return _PrecioCapacidadSalida
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioCapacidadSalida = value
        End Set
    End Property

    Private _IdSolicitudMotivoModificacion As Nullable(Of Long)
    Public Property IdSolicitudMotivoModificacion As Nullable(Of Long)
        Get
            Return _IdSolicitudMotivoModificacion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdSolicitudMotivoModificacion = value
        End Set
    End Property

    Private _IdModeloFacturaVarios As Nullable(Of Long)
    Public Property IdModeloFacturaVarios As Nullable(Of Long)
        Get
            Return _IdModeloFacturaVarios
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdModeloFacturaVarios = value
        End Set
    End Property

    Private _SituacionScoring As String
    Public Property SituacionScoring As String
        Get
            Return _SituacionScoring
        End Get
        Set(ByVal value As String)
            _SituacionScoring = value
        End Set
    End Property

    Private _IdTipoAutoconsumo As Nullable(Of Long)
    Public Property IdTipoAutoconsumo As Nullable(Of Long)
        Get
            Return _IdTipoAutoconsumo
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoAutoconsumo = value
        End Set
    End Property

    Private _BloqueoSistemaComercial As Nullable(Of Boolean)
    Public Property BloqueoSistemaComercial As Nullable(Of Boolean)
        Get
            Return _BloqueoSistemaComercial
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _BloqueoSistemaComercial = value
        End Set
    End Property

    Private _BloqueoPublicidadEmpresa As Nullable(Of Boolean)
    Public Property BloqueoPublicidadEmpresa As Nullable(Of Boolean)
        Get
            Return _BloqueoPublicidadEmpresa
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _BloqueoPublicidadEmpresa = value
        End Set
    End Property

    Private _BloqueoUsoTercero As Nullable(Of Boolean)
    Public Property BloqueoUsoTercero As Nullable(Of Boolean)
        Get
            Return _BloqueoUsoTercero
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _BloqueoUsoTercero = value
        End Set
    End Property

    Private _IdOfertaLote As Nullable(Of Long)
    Public Property IdOfertaLote As Nullable(Of Long)
        Get
            Return _IdOfertaLote
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdOfertaLote = value
        End Set
    End Property

    Private _IsBonoSocial As Nullable(Of Boolean)
    Public Property IsBonoSocial As Nullable(Of Boolean)
        Get
            Return _IsBonoSocial
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsBonoSocial = value
        End Set
    End Property

    Private _IsContratoBonificado As Nullable(Of Boolean)
    Public Property IsContratoBonificado As Nullable(Of Boolean)
        Get
            Return _IsContratoBonificado
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsContratoBonificado = value
        End Set
    End Property

    Private _IdCanal As Nullable(Of Long)
    Public Property IdCanal As Nullable(Of Long)
        Get
            Return _IdCanal
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCanal = value
        End Set
    End Property

    Private _IsBajaAnticipada As Nullable(Of Boolean)
    Public Property IsBajaAnticipada As Nullable(Of Boolean)
        Get
            Return _IsBajaAnticipada
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsBajaAnticipada = value
        End Set
    End Property

    Private _FechaBajaAnticipada As Nullable(Of Date)
    Public Property FechaBajaAnticipada As Nullable(Of Date)
        Get
            Return _FechaBajaAnticipada
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaBajaAnticipada = value
        End Set
    End Property

    Private _IdMotivoBaja As Nullable(Of Long)
    Public Property IdMotivoBaja As Nullable(Of Long)
        Get
            Return _IdMotivoBaja
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdMotivoBaja = value
        End Set
    End Property

    Private _CodigoOrigenBaja As Nullable(Of Integer)
    Public Property CodigoOrigenBaja As Nullable(Of Integer)
        Get
            Return _CodigoOrigenBaja
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _CodigoOrigenBaja = value
        End Set
    End Property

    Private _IdTarifaPeajeAnual As Nullable(Of Long)
    Public Property IdTarifaPeajeAnual As Nullable(Of Long)
        Get
            Return _IdTarifaPeajeAnual
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTarifaPeajeAnual = value
        End Set
    End Property

    Private _TipoLecturaContrato As Nullable(Of Integer)
    Public Property TipoLecturaContrato As Nullable(Of Integer)
        Get
            Return _TipoLecturaContrato
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _TipoLecturaContrato = value
        End Set
    End Property

    Private _TipoTension As Nullable(Of Boolean)
    Public Property TipoTension As Nullable(Of Boolean)
        Get
            Return _TipoTension
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _TipoTension = value
        End Set
    End Property

    Private _NumPedidoFacturacion As String
    Public Property NumPedidoFacturacion As String
        Get
            Return _NumPedidoFacturacion
        End Get
        Set(ByVal value As String)
            _NumPedidoFacturacion = value
        End Set
    End Property

    Private _IdAplicacionIH As Nullable(Of Long)
    Public Property IdAplicacionIH As Nullable(Of Long)
        Get
            Return _IdAplicacionIH
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdAplicacionIH = value
        End Set
    End Property

    Private _AplicarRegasificacion As Nullable(Of Boolean)
    Public Property AplicarRegasificacion As Nullable(Of Boolean)
        Get
            Return _AplicarRegasificacion
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AplicarRegasificacion = value
        End Set
    End Property

    Private _AplicarReajusteTarifa As Nullable(Of Boolean)
    Public Property AplicarReajusteTarifa As Nullable(Of Boolean)
        Get
            Return _AplicarReajusteTarifa
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AplicarReajusteTarifa = value
        End Set
    End Property

    Private _TarifarPorB70 As Nullable(Of Boolean)
    Public Property TarifarPorB70 As Nullable(Of Boolean)
        Get
            Return _TarifarPorB70
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _TarifarPorB70 = value
        End Set
    End Property

    Private _IdModoControlPotencia As Nullable(Of Long)
    Public Property IdModoControlPotencia As Nullable(Of Long)
        Get
            Return _IdModoControlPotencia
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdModoControlPotencia = value
        End Set
    End Property

    Private _OrdenLibreFacturas As Nullable(Of Long)
    Public Property OrdenLibreFacturas As Nullable(Of Long)
        Get
            Return _OrdenLibreFacturas
        End Get
        Set(ByVal value As Nullable(Of Long))
            _OrdenLibreFacturas = value
        End Set
    End Property

    Private _PermitirFacturarA7 As Nullable(Of Boolean)
    Public Property PermitirFacturarA7 As Nullable(Of Boolean)
        Get
            Return _PermitirFacturarA7
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _PermitirFacturarA7 = value
        End Set
    End Property

    Private _CodigoPromocional As String
    Public Property CodigoPromocional As String
        Get
            Return _CodigoPromocional
        End Get
        Set(ByVal value As String)
            _CodigoPromocional = value
        End Set
    End Property

    Private _IsNoInformarASNEF As Nullable(Of Boolean)
    Public Property IsNoInformarASNEF As Nullable(Of Boolean)
        Get
            Return _IsNoInformarASNEF
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsNoInformarASNEF = value
        End Set
    End Property

    Private _NumProveedor As String
    Public Property NumProveedor As String
        Get
            Return _NumProveedor
        End Get
        Set(ByVal value As String)
            _NumProveedor = value
        End Set
    End Property

    Private _IsRenovacionProcesada As Nullable(Of Boolean)
    Public Property IsRenovacionProcesada As Nullable(Of Boolean)
        Get
            Return _IsRenovacionProcesada
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsRenovacionProcesada = value
        End Set
    End Property

    Private _FechaAnulacion As Nullable(Of Date)
    Public Property FechaAnulacion As Nullable(Of Date)
        Get
            Return _FechaAnulacion
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaAnulacion = value
        End Set
    End Property

    Private _ImporteCargoCuenta As Nullable(Of Decimal)
    Public Property ImporteCargoCuenta As Nullable(Of Decimal)
        Get
            Return _ImporteCargoCuenta
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _ImporteCargoCuenta = value
        End Set
    End Property

    Private _TextoLibreFacturas As String
    Public Property TextoLibreFacturas As String
        Get
            Return _TextoLibreFacturas
        End Get
        Set(ByVal value As String)
            _TextoLibreFacturas = value
        End Set
    End Property

    Private _IsAgruparFacturas As Nullable(Of Boolean)
    Public Property IsAgruparFacturas As Nullable(Of Boolean)
        Get
            Return _IsAgruparFacturas
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsAgruparFacturas = value
        End Set
    End Property

    Private _IdMarca As Nullable(Of Long)
    Public Property IdMarca As Nullable(Of Long)
        Get
            Return _IdMarca
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdMarca = value
        End Set
    End Property

    Private _IsUnificarCuentaBancaria As Nullable(Of Boolean)
    Public Property IsUnificarCuentaBancaria As Nullable(Of Boolean)
        Get
            Return _IsUnificarCuentaBancaria
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsUnificarCuentaBancaria = value
        End Set
    End Property

    Private _IsTelemedido As Nullable(Of Boolean)
    Public Property IsTelemedido As Nullable(Of Boolean)
        Get
            Return _IsTelemedido
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsTelemedido = value
        End Set
    End Property

    Private _FechaAplicacionPrecios As Nullable(Of Date)
    Public Property FechaAplicacionPrecios As Nullable(Of Date)
        Get
            Return _FechaAplicacionPrecios
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaAplicacionPrecios = value
        End Set
    End Property

    Private _Procedencia As String
    Public Property Procedencia As String
        Get
            Return _Procedencia
        End Get
        Set(ByVal value As String)
            _Procedencia = value
        End Set
    End Property

    Private _Subprocedencia As String
    Public Property Subprocedencia As String
        Get
            Return _Subprocedencia
        End Get
        Set(ByVal value As String)
            _Subprocedencia = value
        End Set
    End Property

    Private _ATRDirecto As Nullable(Of Boolean)
    Public Property ATRDirecto As Nullable(Of Boolean)
        Get
            Return _ATRDirecto
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ATRDirecto = value
        End Set
    End Property

    Private _PorcentajePerdidasPropio As Nullable(Of Decimal)
    Public Property PorcentajePerdidasPropio As Nullable(Of Decimal)
        Get
            Return _PorcentajePerdidasPropio
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PorcentajePerdidasPropio = value
        End Set
    End Property

    Private _ObtenerLiquidacion As Nullable(Of Boolean)
    Public Property ObtenerLiquidacion As Nullable(Of Boolean)
        Get
            Return _ObtenerLiquidacion
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _ObtenerLiquidacion = value
        End Set
    End Property

    Private _OrderId As String
    Public Property OrderId As String
        Get
            Return _OrderId
        End Get
        Set(ByVal value As String)
            _OrderId = value
        End Set
    End Property

    Private _IsContratoRenovado As Nullable(Of Boolean)
    Public Property IsContratoRenovado As Nullable(Of Boolean)
        Get
            Return _IsContratoRenovado
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsContratoRenovado = value
        End Set
    End Property

    Private _Vulnerabilidad As Nullable(Of Long)
    Public Property Vulnerabilidad As Nullable(Of Long)
        Get
            Return _Vulnerabilidad
        End Get
        Set(ByVal value As Nullable(Of Long))
            _Vulnerabilidad = value
        End Set
    End Property

    Private _FechaInicioVulnerabilidad As Nullable(Of Date)
    Public Property FechaInicioVulnerabilidad As Nullable(Of Date)
        Get
            Return _FechaInicioVulnerabilidad
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaInicioVulnerabilidad = value
        End Set
    End Property

    Private _FechaFinVulnerabilidad As Nullable(Of Date)
    Public Property FechaFinVulnerabilidad As Nullable(Of Date)
        Get
            Return _FechaFinVulnerabilidad
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaFinVulnerabilidad = value
        End Set
    End Property

    Private _EnvioMinisterio As Nullable(Of Boolean)
    Public Property EnvioMinisterio As Nullable(Of Boolean)
        Get
            Return _EnvioMinisterio
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _EnvioMinisterio = value
        End Set
    End Property

    Private _NumMenores As Nullable(Of Long)
    Public Property NumMenores As Nullable(Of Long)
        Get
            Return _NumMenores
        End Get
        Set(ByVal value As Nullable(Of Long))
            _NumMenores = value
        End Set
    End Property

    Private _FechaXML As Nullable(Of Date)
    Public Property FechaXML As Nullable(Of Date)
        Get
            Return _FechaXML
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaXML = value
        End Set
    End Property

    Private _SegmentoBS As Nullable(Of Long)
    Public Property SegmentoBS As Nullable(Of Long)
        Get
            Return _SegmentoBS
        End Get
        Set(ByVal value As Nullable(Of Long))
            _SegmentoBS = value
        End Set
    End Property

    Private _FechaNacimientoMenor As Nullable(Of Date)
    Public Property FechaNacimientoMenor As Nullable(Of Date)
        Get
            Return _FechaNacimientoMenor
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaNacimientoMenor = value
        End Set
    End Property

    Private _AyudaBS As Nullable(Of Boolean)
    Public Property AyudaBS As Nullable(Of Boolean)
        Get
            Return _AyudaBS
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AyudaBS = value
        End Set
    End Property

    Private _IdSwitchDocumentSalesforce As String
    Public Property IdSwitchDocumentSalesforce As String
        Get
            Return _IdSwitchDocumentSalesforce
        End Get
        Set(ByVal value As String)
            _IdSwitchDocumentSalesforce = value
        End Set
    End Property

    Private _IdOrderSalesforce As String
    Public Property IdOrderSalesforce As String
        Get
            Return _IdOrderSalesforce
        End Get
        Set(ByVal value As String)
            _IdOrderSalesforce = value
        End Set
    End Property

    Private _DiasVencimiento As Nullable(Of Integer)
    Public Property DiasVencimiento As Nullable(Of Integer)
        Get
            Return _DiasVencimiento
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _DiasVencimiento = value
        End Set
    End Property

    Private _IsContratoRevisado As Nullable(Of Boolean)
    Public Property IsContratoRevisado As Nullable(Of Boolean)
        Get
            Return _IsContratoRevisado
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsContratoRevisado = value
        End Set
    End Property

    Private _FechaCreacion As Nullable(Of Date)
    Public Property FechaCreacion As Nullable(Of Date)
        Get
            Return _FechaCreacion
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaCreacion = value
        End Set
    End Property

    Private _IsDual As Nullable(Of Boolean)
    Public Property IsDual As Nullable(Of Boolean)
        Get
            Return _IsDual
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsDual = value
        End Set
    End Property

    Private _IsProductoClick As Nullable(Of Boolean)
    Public Property IsProductoClick As Nullable(Of Boolean)
        Get
            Return _IsProductoClick
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsProductoClick = value
        End Set
    End Property

    Private _IsFirmaDigitalEnviada As Nullable(Of Boolean)
    Public Property IsFirmaDigitalEnviada As Nullable(Of Boolean)
        Get
            Return _IsFirmaDigitalEnviada
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsFirmaDigitalEnviada = value
        End Set
    End Property

    Private _IsFirmadoDigitalmente As Nullable(Of Boolean)
    Public Property IsFirmadoDigitalmente As Nullable(Of Boolean)
        Get
            Return _IsFirmadoDigitalmente
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsFirmadoDigitalmente = value
        End Set
    End Property

    Private _Reqqd As Nullable(Of Decimal)
    Public Property Reqqd As Nullable(Of Decimal)
        Get
            Return _Reqqd
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _Reqqd = value
        End Set
    End Property

    Private _Reqqh As Nullable(Of Long)
    Public Property Reqqh As Nullable(Of Long)
        Get
            Return _Reqqh
        End Get
        Set(ByVal value As Nullable(Of Long))
            _Reqqh = value
        End Set
    End Property

    Private _IsContratoAutonomo As Nullable(Of Boolean)
    Public Property IsContratoAutonomo As Nullable(Of Boolean)
        Get
            Return _IsContratoAutonomo
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsContratoAutonomo = value
        End Set
    End Property

    Private _AutorizarFidelizacion As Nullable(Of Boolean)
    Public Property AutorizarFidelizacion As Nullable(Of Boolean)
        Get
            Return _AutorizarFidelizacion
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AutorizarFidelizacion = value
        End Set
    End Property
    Private _CederDatosEmpresasGrupoTotal As Nullable(Of Boolean)
    Public Property CederDatosEmpresasGrupoTotal As Nullable(Of Boolean)
        Get
            Return _CederDatosEmpresasGrupoTotal
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _CederDatosEmpresasGrupoTotal = value
        End Set
    End Property

    Private _IdCanalCodage As Nullable(Of Long)
    Public Property IdCanalCodage As Nullable(Of Long)
        Get
            Return _IdCanalCodage
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCanalCodage = value
        End Set
    End Property

    'Public ReadOnly Property DenominacionCliente As String
    '    Get
    '        If Not IsNothing(_Cliente) Then
    '            Return If(_Cliente.Denominacion, String.Empty)
    '        Else
    '            Return String.Empty
    '        End If
    '    End Get
    'End Property

    Private _Telefono1 As String = ""

#Region "Columnas Relacionadas"
    Public Property Telefono1 As String
        Get
            Return _Telefono1
        End Get
        Set(value As String)
            _Telefono1 = value
        End Set
    End Property

    Private _Telefono2 As String = ""
    Public Property Telefono2 As String
        Get
            Return _Telefono2
        End Get
        Set(value As String)
            _Telefono2 = value
        End Set
    End Property

    Private _Fax As String = ""
    Public Property Fax As String
        Get
            Return _Fax
        End Get
        Set(value As String)
            _Fax = value
        End Set
    End Property

    Private _Email As String
    Public Property Email As String
        Get
            Return _Email
        End Get
        Set(value As String)
            _Email = value
        End Set
    End Property

    Private _TieneMaximetro As String = ""

    Public Property TieneMaximetro As String
        Get
            Return _TieneMaximetro
        End Get
        Set(value As String)
            _TieneMaximetro = value
        End Set
    End Property
#End Region
    Private _ImpuestoHidrocarburo As String

    Public Property ImpuestoHidrocarburo As String
        Get
            Return _ImpuestoHidrocarburo
        End Get
        Set(value As String)
            _ImpuestoHidrocarburo = value
        End Set
    End Property

End Class

