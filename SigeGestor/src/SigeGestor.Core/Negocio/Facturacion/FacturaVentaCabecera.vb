Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Public Class FacturaVentaCabecera
    Public Property IdFacturaVentaCabecera As Long
    Public Property Entorno As String
    Public Property SerieFactura As String
    Public Property NumeroFactura As Nullable(Of Long)
    Public Property IsFactura As Nullable(Of Boolean)
    Public Property IsIncidencia As Nullable(Of Boolean)
    Public Property IdCliente As Nullable(Of Long)
    Public Property IdClientePago As Nullable(Of Long)
    Public Property IdClienteEnvio As Nullable(Of Long)
    Public Property IdContrato As Nullable(Of Long)
    Public Property CodigoContrato As Nullable(Of Long)
    Public Property VersionContrato As Nullable(Of Long)
    Public Property FechaFactura As Nullable(Of Date)
    Public Property IdTipoCobro As Nullable(Of Long)
    Public Property IBAN As String
    Public Property FechaVencimiento As Nullable(Of Date)
    Public Property IdCanal As Nullable(Of Long)
    Public Property IdFacturaTipo As Nullable(Of Long)
    Public Property FacturaCategoria As String
    Public Property IdTipoImpuesto As Nullable(Of Long)
    Public Property IsFacturaConIE As Boolean
    Public Property DescuentoBase As Nullable(Of Decimal)
    Public Property DescuentoTotal As Nullable(Of Decimal)
    Public Property Imprimida As Nullable(Of Boolean)
    Public Property FechaEmision As Nullable(Of Date)
    Public Property FechaEnvio As Nullable(Of Date)
    Public Property VersionReport As String
    Public Property IsCorrecta As Nullable(Of Boolean)
    Public Property IsProcesada As Nullable(Of Boolean)
    Public Property IsEnlaceContable As Nullable(Of Boolean)
    Public Property IdSCSAsientoCabecera As Nullable(Of Long)
    Public Property IdFacturaAbono As Nullable(Of Long)
    Public Property IdFacturaRectificativa As Nullable(Of Long)
    Public Property IdFacturaOrigen As Nullable(Of Long)
    Public Property IdFacturaAsociada As Nullable(Of Long)
    Public Property InfoCabeceraXML As String
    Public Property NombreInfoCabeceraXML As String
    Public Property FacturaXML As String
    Public Property NombreFacturaXML As String
    Public Property IncidenciaXML As String
    Public Property NombreIncidenciaXML As String
    Public Property IdContratoDocumento As Nullable(Of Long)
    Public Property EfacturaXML As String
    Public Property NombreEfacturaXML As String
    Public Property IdModeloFactura As Nullable(Of Long)
    Public Property InfoCurvaHorariaXML As String
    Public Property NumeroRegistro As Nullable(Of Long)
    Public Property FechaRegistroSII As Nullable(Of Date)
    Public Property FechaEnvioSII As Nullable(Of Date)
    Public Property FechaContableSII As Nullable(Of Date)
    Public Property TipoComunicacionSII As String
    Public Property TipoFacturaXMLSII As String
    Public Property ClaveRegimenEspecialSII As String
    Public Property TipoNoExentaSII As String
    Public Property IsYaUnificada As Nullable(Of Boolean)
    Public Property CertificadoFactura As String
    Public Property CertificadoFacturaAnterior As String
    Public Property CertificadoFacturaEncriptado As String
    Public Property CertificadoFacturaAnteriorEncriptado As String
    Public Property CertificadoCodigo As String
    Public Property FacturaReguladaXML As String
    Public Property IsFacturaVariosPerfiles As Nullable(Of Boolean)
    Public Property IsAbono As Nullable(Of Boolean)
    Public Property CodigoExternoNAV As String
    Public Property ErrorNAV As String
    Public Property IsExportado As Nullable(Of Boolean)
    Public Property IsComisionCalculada As Nullable(Of Boolean)
    Public Property AvisoRegularizacionProxima As Nullable(Of Boolean)
    Public Property AvisoRegularizacionFueraRango As Nullable(Of Boolean)
    Public Property IsRegularizadaTarifaPlana As Nullable(Of Boolean)
    Public Property IsCuotaFija As Nullable(Of Boolean)
    Public Property IdDestinoEnergia As Nullable(Of Long)
    Public Property ContadorFacturas As Nullable(Of Long)
    Public Property CodigoProceso As Nullable(Of Long)
    Public Property SerieFacturaUnica As String
    Public Property NumeroFacturaUnica As Nullable(Of Long)
    Public Property EstadoCobra As Nullable(Of Long)
    Public Property MotivoErrorCobra As String
    Public Property NumeroFacturaOriginal As String
    Public Property FechaRecepcion As Nullable(Of Date)
    Public Property AsuntosSociales As Nullable(Of Boolean)
    Public Property IsFacturaClick As Nullable(Of Boolean)
    Public Property FechaEmisionXML As Nullable(Of Date)
    Public Property RutaFicheroXML As String
    Public Property FechaLecturaAnteriorXML As Nullable(Of Date)
    Public Property FechaLecturaActualXML As Nullable(Of Date)
    Public Property IdTarifaGrupoXML As String
    Public Property IdTarifaPeajeXML As String
    Public Property IdTarifaXML As String
    Public Property IdPerfilFacturacionPeajeXML As String
    Public Property IdPerfilFacturacionXML As String
    Public Property ImporteFijoXML As String
    Public Property PotenciaMaximaContratadaXML As Nullable(Of Integer)
    Public Property IsSubidaFTP As Nullable(Of Boolean)
    Public Property IsGestinel As Boolean
    Public Property OrigenMedida As String
    Public Property URLQR As String
    Public Property AnioSerie As Nullable(Of Integer)
    Public Property IsFacturaNoEnergia As Nullable(Of Boolean)
    Public Property IsPerfiladoPool As Nullable(Of Boolean)
    Public Property IdFacturaAbonoBateriaVirtual As Nullable(Of Long)
    Public Property IdFacturaOrigenBateriaVirtual As Nullable(Of Long)
    Public Property DescuentoBateriaVirtual As Nullable(Of Decimal)
    Public Property ImporteTotalPagarBateriaVirtual As Nullable(Of Decimal)
End Class
