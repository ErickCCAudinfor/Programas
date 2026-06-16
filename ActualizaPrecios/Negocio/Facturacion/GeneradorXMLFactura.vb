Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml.Linq

Public Class GeneradorXMLFactura

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        _connectionString = connectionString
    End Sub

    Public Sub GenerarXMLFacturasGeneral(idFactura As Long, rutaDestino As String)
        Dim idsStr As String = idFactura.ToString()
        Dim ds As New DataSet("RptFichasFacturaOptENDTO")

        ds.Tables.Add(BuildFacturasDatos(idsStr))
        ds.Tables.Add(BuildFacturasTotales(idsStr))
        ds.Tables.Add(BuildFacturasDetalles(idsStr))
        ds.Tables.Add(BuildFacturasLecturas(idsStr))
        ds.Tables.Add(BuildFacturasATR(idsStr))
        ds.Tables.Add(BuildFacturasConsumos(idsStr))
        ds.Tables.Add(BuildEtiquetas(idsStr))
        ds.Tables.Add(BuildAgentes(idsStr))
        ds.Tables.Add(BuildEmptyRecargas())
        ds.Tables.Add(BuildEmptyDescuentosRecarga())
        ds.Tables.Add(BuildEmptyFacturasLecturasAutoconsumo())
        ds.Tables.Add(BuildEmptyHistoricoExcedentes())
        ds.Tables.Add(BuildEmptySaldoDescuentos())
        ds.Tables.Add(BuildEmptyPeriodoConsumo())
        ds.Tables.Add(BuildEmptyInfraestructuras())
        ds.Tables.Add(BuildEmptyPassPoolDesglose())

        ds.WriteXml(rutaDestino, XmlWriteMode.WriteSchema)
    End Sub

    Private Function EjecutarSP(spName As String, idsFacturas As String) As DataTable
        Dim dt As New DataTable()
        Using conn As New SqlConnection(_connectionString)
            Dim cmd As New SqlCommand(spName, conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 3600
            cmd.Parameters.AddWithValue("@IdFacturas", idsFacturas)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        End Using
        Return dt
    End Function

    Private Function EjecutarQuery(query As String) As DataTable
        Dim dt As New DataTable()
        Using conn As New SqlConnection(_connectionString)
            Dim cmd As New SqlCommand(query, conn)
            cmd.CommandTimeout = 3600
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(dt)
        End Using
        Return dt
    End Function

    Private Function GetStr(row As DataRow, columnName As String) As String
        Try
            If row.Table.Columns.Contains(columnName) AndAlso Not row.IsNull(columnName) Then
                Return row(columnName).ToString()
            End If
        Catch
        End Try
        Return String.Empty
    End Function

    Private Function GetDec(row As DataRow, columnName As String) As Decimal
        Try
            If row.Table.Columns.Contains(columnName) AndAlso Not row.IsNull(columnName) Then
                Return CDec(row(columnName))
            End If
        Catch
        End Try
        Return 0D
    End Function

    Private Function GetBool(row As DataRow, columnName As String) As Boolean
        Try
            If row.Table.Columns.Contains(columnName) AndAlso Not row.IsNull(columnName) Then
                Return CBool(row(columnName))
            End If
        Catch
        End Try
        Return False
    End Function

    Private Function GetDate(row As DataRow, columnName As String) As Object
        Try
            If row.Table.Columns.Contains(columnName) AndAlso Not row.IsNull(columnName) Then
                Return CDate(row(columnName))
            End If
        Catch
        End Try
        Return DBNull.Value
    End Function

    Private Function GetLong(row As DataRow, columnName As String) As Long
        Try
            If row.Table.Columns.Contains(columnName) AndAlso Not row.IsNull(columnName) Then
                Return CLng(row(columnName))
            End If
        Catch
        End Try
        Return 0L
    End Function

    Private Function GetDbl(row As DataRow, columnName As String) As Double
        Try
            If row.Table.Columns.Contains(columnName) AndAlso Not row.IsNull(columnName) Then
                Return CDbl(row(columnName))
            End If
        Catch
        End Try
        Return 0.0
    End Function

    Private Function GetXmlChild(xmlStr As String, elementName As String) As String
        If String.IsNullOrEmpty(xmlStr) Then Return String.Empty
        Try
            Dim xdoc = XElement.Parse(xmlStr)
            Dim el = xdoc.Descendants(elementName).FirstOrDefault()
            If el IsNot Nothing Then Return el.Value
        Catch
        End Try
        Return String.Empty
    End Function

    Private Function GetXmlDec(xmlStr As String, elementName As String) As Decimal
        Dim v = GetXmlChild(xmlStr, elementName)
        If String.IsNullOrEmpty(v) Then Return 0D
        Try : Return CDec(v) : Catch : Return 0D : End Try
    End Function

    Private Function GetXmlDbl(xmlStr As String, elementName As String) As Double
        Dim v = GetXmlChild(xmlStr, elementName)
        If String.IsNullOrEmpty(v) Then Return 0.0
        Try : Return CDbl(v) : Catch : Return 0.0 : End Try
    End Function

    Private Function GetXmlDate(xmlStr As String, elementName As String) As Object
        Dim v = GetXmlChild(xmlStr, elementName)
        If String.IsNullOrEmpty(v) Then Return DBNull.Value
        Try : Return CDate(v) : Catch : Return DBNull.Value : End Try
    End Function

    Private Sub AddCols(dt As DataTable, defs As (String, Type)())
        For Each def In defs
            dt.Columns.Add(def.Item1, def.Item2)
        Next
    End Sub

    Private Function BuildDireccion(tipoVia As String, calle As String, numero As String, aclarador As String) As String
        Return String.Join(" ", {tipoVia, calle, numero, aclarador}.Select(Function(s) s.Trim()).Where(Function(s) Not String.IsNullOrEmpty(s)))
    End Function

    Private Class ATRPeriodoData
        Public EnergiaImporte As Decimal = 0D
        Public EnergiaPrecio As Decimal = 0D
        Public EnergiaDescripcion As String = String.Empty
        Public EnergiaCargoImporte As Decimal = 0D
        Public EnergiaCargoPrecio As Decimal = 0D
        Public EnergiaCargoDescripcion As String = String.Empty
        Public PotenciaImporte As Decimal = 0D
        Public PotenciaPrecio As Decimal = 0D
        Public PotenciaContratada As Decimal = 0D
        Public PotenciaDescripcion As String = String.Empty
        Public PotenciaCargoImporte As Decimal = 0D
        Public PotenciaCargoPrecio As Decimal = 0D
        Public PotenciaCargoDescripcion As String = String.Empty
    End Class

    Private Function BuildFacturasDatos(idsStr As String) As DataTable
        Dim dtSP = EjecutarSP("GetInfoFacturasReport", idsStr)
        Dim dtPot = EjecutarSP("GetInfoPotenciasReport", idsStr)
        Dim dtLineas = EjecutarSP("GetInfoFacturasLineasReport", idsStr)

        Dim dtExtra As New DataTable()
        Try
            Dim sqlExtra = "SELECT fvc.IdFacturaVentaCabecera, " &
                           "ISNULL(CAST(fvc.CodigoBarras AS nvarchar(max)),'') AS CodigoBarras, " &
                           "ISNULL(com.RegistroMercantil,'') AS RegistroMercantil, " &
                           "ISNULL(com.Email,'') AS EmailComercializadora, " &
                           "ISNULL(cont.IdIdiomaInforme,0) AS IdIdiomaInforme, " &
                           "ISNULL(ii.TextoIdioma,'') AS TextoIdioma " &
                           "FROM FacturaVentaCabecera fvc " &
                           "INNER JOIN Contrato cont ON cont.CodigoContrato = fvc.CodigoContrato AND cont.Entorno = fvc.Entorno " &
                           "INNER JOIN Comercializadora com ON com.IdComercializadora = cont.IdComercializadora " &
                           "LEFT JOIN IdiomaInforme ii ON ii.IdIdiomaInforme = cont.IdIdiomaInforme " &
                           $"INNER JOIN dbo.StringSplit('{idsStr}',',') F ON F.value = fvc.IdFacturaVentaCabecera"
            dtExtra = EjecutarQuery(sqlExtra)
        Catch
        End Try

        Dim dt As New DataTable("FacturasDatos")
        AddCols(dt, {
            ("CodigoContrato", GetType(String)),
            ("FechaEmision", GetType(DateTime)),
            ("FechaInicioFactura", GetType(DateTime)),
            ("FechaFinFactura", GetType(DateTime)),
            ("FechaInicioPotencia", GetType(DateTime)),
            ("FechaFinPotencia", GetType(DateTime)),
            ("DesdeFechaEmision", GetType(DateTime)),
            ("HastaFechaEmision", GetType(DateTime)),
            ("NumFactura", GetType(String)),
            ("Importe", GetType(Decimal)),
            ("FechaFinContrato", GetType(DateTime)),
            ("NombreEnvio", GetType(String)),
            ("NombreDistribuidora", GetType(String)),
            ("WebDistribuidora", GetType(String)),
            ("Unipres", GetType(String)),
            ("DireccionEnvio", GetType(String)),
            ("CPEnvio", GetType(String)),
            ("RegitroMercantil", GetType(String)),
            ("PoblacionEnvio", GetType(String)),
            ("NombreTitular", GetType(String)),
            ("DireccionCUPS", GetType(String)),
            ("PoblacionCUPS", GetType(String)),
            ("CPCUPS", GetType(String)),
            ("DireccionCliente", GetType(String)),
            ("ProvinciaCliente", GetType(String)),
            ("PoblacionCliente", GetType(String)),
            ("CPCliente", GetType(String)),
            ("NIF", GetType(String)),
            ("ReferenciaCatastral", GetType(String)),
            ("CUPS", GetType(String)),
            ("CNAE", GetType(String)),
            ("PotenciaContratadaPeriodo", GetType(String)),
            ("PotenciaContratadaCantidad", GetType(String)),
            ("Tension", GetType(String)),
            ("TarifaAcceso", GetType(String)),
            ("NumeroContador", GetType(String)),
            ("TelefonoAverias", GetType(String)),
            ("FormaPago", GetType(String)),
            ("EntidadBancaria", GetType(String)),
            ("NumeroCuenta", GetType(String)),
            ("FechaPago", GetType(DateTime)),
            ("FechaFinVulnerabilidad", GetType(DateTime)),
            ("BaseImponible", GetType(Decimal)),
            ("IVA", GetType(Decimal)),
            ("IdCliente", GetType(Long)),
            ("ImporteCosteRegulado", GetType(Double)),
            ("ImporteCosteEnergia", GetType(Double)),
            ("ImporteCostePotencia", GetType(Double)),
            ("ImporteCargosEnergia", GetType(Double)),
            ("ImporteCargosPotencia", GetType(Double)),
            ("ImporteCosteProduccion", GetType(Double)),
            ("ImporteCosteCargos", GetType(Double)),
            ("ImporteImpuesto", GetType(Double)),
            ("ImporteAlquiler", GetType(Double)),
            ("CosteIncentivo", GetType(Double)),
            ("CosteRedes", GetType(Double)),
            ("CosteOtros", GetType(Double)),
            ("CosteRecoreCargos", GetType(Double)),
            ("CosteDeficitCargos", GetType(Double)),
            ("CosteTnpCargos", GetType(Double)),
            ("CosteOtrosCargos", GetType(Double)),
            ("CodigoBarras", GetType(String)),
            ("Comentarios", GetType(String)),
            ("IdFacturaCabeceraVenta", GetType(Long)),
            ("IdIdiomaInforme", GetType(Long)),
            ("TextoIdioma", GetType(String)),
            ("TextoTarifaPeaje", GetType(String)),
            ("ProvinciaCUPS", GetType(String)),
            ("ProvinciaEnvio", GetType(String)),
            ("CodigoExterno1", GetType(String)),
            ("Cultura", GetType(String)),
            ("P1", GetType(Decimal)),
            ("P2", GetType(Decimal)),
            ("P3", GetType(Decimal)),
            ("P4", GetType(Decimal)),
            ("P5", GetType(Decimal)),
            ("P6", GetType(Decimal)),
            ("PML1", GetType(Decimal)),
            ("PML2", GetType(Decimal)),
            ("PML3", GetType(Decimal)),
            ("PML4", GetType(Decimal)),
            ("PML5", GetType(Decimal)),
            ("PML6", GetType(Decimal)),
            ("PPML1", GetType(Decimal)),
            ("PPML2", GetType(Decimal)),
            ("PPML3", GetType(Decimal)),
            ("PPML4", GetType(Decimal)),
            ("PPML5", GetType(Decimal)),
            ("PPML6", GetType(Decimal)),
            ("TotalImporteTotal", GetType(Decimal)),
            ("SerieFactura", GetType(String)),
            ("NombreFiscal", GetType(String)),
            ("IsDuplicadoFactura", GetType(Boolean)),
            ("Remesa", GetType(Boolean)),
            ("ATREnergiaP1Descripcion", GetType(String)),
            ("ATREnergiaP2Descripcion", GetType(String)),
            ("ATREnergiaP3Descripcion", GetType(String)),
            ("ATREnergiaP4Descripcion", GetType(String)),
            ("ATREnergiaP5Descripcion", GetType(String)),
            ("ATREnergiaP6Descripcion", GetType(String)),
            ("ATREnergiaCargoP1Descripcion", GetType(String)),
            ("ATREnergiaCargoP2Descripcion", GetType(String)),
            ("ATREnergiaCargoP3Descripcion", GetType(String)),
            ("ATREnergiaCargoP4Descripcion", GetType(String)),
            ("ATREnergiaCargoP5Descripcion", GetType(String)),
            ("ATREnergiaCargoP6Descripcion", GetType(String)),
            ("ATRPotenciaCargoP1Descripcion", GetType(String)),
            ("ATRPotenciaCargoP2Descripcion", GetType(String)),
            ("ATRPotenciaCargoP3Descripcion", GetType(String)),
            ("ATRPotenciaCargoP4Descripcion", GetType(String)),
            ("ATRPotenciaCargoP5Descripcion", GetType(String)),
            ("ATRPotenciaCargoP6Descripcion", GetType(String)),
            ("PotenciaFacturadaP1", GetType(Decimal)),
            ("PotenciaFacturadaP2", GetType(Decimal)),
            ("PotenciaFacturadaP3", GetType(Decimal)),
            ("PotenciaFacturadaP4", GetType(Decimal)),
            ("PotenciaFacturadaP5", GetType(Decimal)),
            ("PotenciaFacturadaP6", GetType(Decimal)),
            ("ATREnergiaP1Importe", GetType(Decimal)),
            ("ATREnergiaP2Importe", GetType(Decimal)),
            ("ATREnergiaP3Importe", GetType(Decimal)),
            ("ATREnergiaP4Importe", GetType(Decimal)),
            ("ATREnergiaP5Importe", GetType(Decimal)),
            ("ATREnergiaP6Importe", GetType(Decimal)),
            ("ATREnergiaCargoP1Importe", GetType(Decimal)),
            ("ATREnergiaCargoP2Importe", GetType(Decimal)),
            ("ATREnergiaCargoP3Importe", GetType(Decimal)),
            ("ATREnergiaCargoP4Importe", GetType(Decimal)),
            ("ATREnergiaCargoP5Importe", GetType(Decimal)),
            ("ATREnergiaCargoP6Importe", GetType(Decimal)),
            ("ATRPotenciaCargoP1Importe", GetType(Decimal)),
            ("ATRPotenciaCargoP2Importe", GetType(Decimal)),
            ("ATRPotenciaCargoP3Importe", GetType(Decimal)),
            ("ATRPotenciaCargoP4Importe", GetType(Decimal)),
            ("ATRPotenciaCargoP5Importe", GetType(Decimal)),
            ("ATRPotenciaCargoP6Importe", GetType(Decimal)),
            ("ATREnergiaP1Precio", GetType(Decimal)),
            ("ATREnergiaP2Precio", GetType(Decimal)),
            ("ATREnergiaP3Precio", GetType(Decimal)),
            ("ATREnergiaP4Precio", GetType(Decimal)),
            ("ATREnergiaP5Precio", GetType(Decimal)),
            ("ATREnergiaP6Precio", GetType(Decimal)),
            ("ATREnergiaCargoP1Precio", GetType(Decimal)),
            ("ATREnergiaCargoP2Precio", GetType(Decimal)),
            ("ATREnergiaCargoP3Precio", GetType(Decimal)),
            ("ATREnergiaCargoP4Precio", GetType(Decimal)),
            ("ATREnergiaCargoP5Precio", GetType(Decimal)),
            ("ATREnergiaCargoP6Precio", GetType(Decimal)),
            ("ATRPotenciaCargoP1Precio", GetType(Decimal)),
            ("ATRPotenciaCargoP2Precio", GetType(Decimal)),
            ("ATRPotenciaCargoP3Precio", GetType(Decimal)),
            ("ATRPotenciaCargoP4Precio", GetType(Decimal)),
            ("ATRPotenciaCargoP5Precio", GetType(Decimal)),
            ("ATRPotenciaCargoP6Precio", GetType(Decimal)),
            ("ATREnergiaCargoDescuentoP1Precio", GetType(Decimal)),
            ("ATREnergiaCargoDescuentoP2Precio", GetType(Decimal)),
            ("ATREnergiaCargoDescuentoP3Precio", GetType(Decimal)),
            ("ATREnergiaCargoDescuentoP4Precio", GetType(Decimal)),
            ("ATREnergiaCargoDescuentoP5Precio", GetType(Decimal)),
            ("ATREnergiaCargoDescuentoP6Precio", GetType(Decimal)),
            ("ATRPotenciaCargoDescuentoP1Precio", GetType(Decimal)),
            ("ATRPotenciaCargoDescuentoP2Precio", GetType(Decimal)),
            ("ATRPotenciaCargoDescuentoP3Precio", GetType(Decimal)),
            ("ATRPotenciaCargoDescuentoP4Precio", GetType(Decimal)),
            ("ATRPotenciaCargoDescuentoP5Precio", GetType(Decimal)),
            ("ATRPotenciaCargoDescuentoP6Precio", GetType(Decimal)),
            ("ATRPotenciaP1Descripcion", GetType(String)),
            ("ATRPotenciaP2Descripcion", GetType(String)),
            ("ATRPotenciaP3Descripcion", GetType(String)),
            ("ATRPotenciaP4Descripcion", GetType(String)),
            ("ATRPotenciaP5Descripcion", GetType(String)),
            ("ATRPotenciaP6Descripcion", GetType(String)),
            ("ATRPotenciaP1Importe", GetType(Decimal)),
            ("ATRPotenciaP2Importe", GetType(Decimal)),
            ("ATRPotenciaP3Importe", GetType(Decimal)),
            ("ATRPotenciaP4Importe", GetType(Decimal)),
            ("ATRPotenciaP5Importe", GetType(Decimal)),
            ("ATRPotenciaP6Importe", GetType(Decimal)),
            ("ATRPotenciaP1Precio", GetType(Decimal)),
            ("ATRPotenciaP2Precio", GetType(Decimal)),
            ("ATRPotenciaP3Precio", GetType(Decimal)),
            ("ATRPotenciaP4Precio", GetType(Decimal)),
            ("ATRPotenciaP5Precio", GetType(Decimal)),
            ("ATRPotenciaP6Precio", GetType(Decimal)),
            ("Emisora", GetType(String)),
            ("Referencia", GetType(String)),
            ("Identidad", GetType(String)),
            ("ImporteAlquilerEquipo", GetType(Decimal)),
            ("ConsumoAcumuladoUltimoAnio", GetType(Decimal)),
            ("ConsumoMedioDiarioPeriodo", GetType(Decimal)),
            ("ConsumoMedioDiarioUltimos14Meses", GetType(Decimal)),
            ("TfnoAtCliente", GetType(String)),
            ("RefExt1", GetType(String)),
            ("RefExt2", GetType(String)),
            ("TipoContrato", GetType(String)),
            ("SupConsumoP1", GetType(Decimal)), ("SupConsumoP2", GetType(Decimal)), ("SupConsumoP3", GetType(Decimal)),
            ("SupConsumoP4", GetType(Decimal)), ("SupConsumoP5", GetType(Decimal)), ("SupConsumoP6", GetType(Decimal)),
            ("SupPotenciaP11", GetType(Decimal)), ("SupPotenciaP12", GetType(Decimal)), ("SupPotenciaP13", GetType(Decimal)),
            ("SupPotenciaP14", GetType(Decimal)), ("SupPotenciaP15", GetType(Decimal)), ("SupPotenciaP16", GetType(Decimal)),
            ("SupPotenciaP21", GetType(Decimal)), ("SupPotenciaP22", GetType(Decimal)), ("SupPotenciaP23", GetType(Decimal)),
            ("SupPotenciaP24", GetType(Decimal)), ("SupPotenciaP25", GetType(Decimal)), ("SupPotenciaP26", GetType(Decimal)),
            ("SupPotenciaP31", GetType(Decimal)), ("SupPotenciaP32", GetType(Decimal)), ("SupPotenciaP33", GetType(Decimal)),
            ("SupPotenciaP34", GetType(Decimal)), ("SupPotenciaP35", GetType(Decimal)), ("SupPotenciaP36", GetType(Decimal)),
            ("SupCambioFecha1", GetType(DateTime)), ("SupCambioFecha2", GetType(DateTime)),
            ("SupImporteRegularizacion", GetType(Decimal)), ("SupImporteFraccion", GetType(Decimal)),
            ("SupNumeroFraccion", GetType(Integer)), ("SupNumeroTotalFracciones", GetType(Integer)),
            ("FacturaCategoria", GetType(String)),
            ("IsAbono", GetType(Boolean)),
            ("TipoEquipoMedida", GetType(String)),
            ("TextoTarifa", GetType(String)),
            ("IsPVPCBS", GetType(Boolean)),
            ("CodigoTipoTelegestion", GetType(String)),
            ("CadenaOrdenEmision", GetType(String)),
            ("NumPedidoFacturacion", GetType(String)),
            ("SerieFacturaOrigen", GetType(String)),
            ("NumFacturaOrigen", GetType(String)),
            ("SerieFacturaUnica", GetType(String)),
            ("NumFacturaUnica", GetType(String)),
            ("CeutaSegundaTarifa", GetType(String)), ("CeutaSegundaTarifaImporte", GetType(Decimal)),
            ("CeutaTerceraTarifa", GetType(String)), ("CeutaTerceraTarifaImporte", GetType(Decimal)),
            ("IsEmpleado", GetType(Boolean)),
            ("IsBateriaVirtual", GetType(Boolean)),
            ("URLQR", GetType(String)),
            ("TextoBOE", GetType(String)),
            ("FechaFinPenalizacion", GetType(DateTime)),
            ("FechaEmisionOrigen", GetType(DateTime)),
            ("ExcedenteAutoconsumoP1", GetType(Decimal)), ("ExcedenteAutoconsumoP2", GetType(Decimal)),
            ("ExcedenteAutoconsumoP3", GetType(Decimal)), ("ExcedenteAutoconsumoP4", GetType(Decimal)),
            ("ExcedenteAutoconsumoP5", GetType(Decimal)), ("ExcedenteAutoconsumoP6", GetType(Decimal)),
            ("KwhCompensadosP1", GetType(Decimal)), ("KwhCompensadosP2", GetType(Decimal)),
            ("KwhCompensadosP3", GetType(Decimal)), ("KwhCompensadosP4", GetType(Decimal)),
            ("KwhCompensadosP5", GetType(Decimal)), ("KwhCompensadosP6", GetType(Decimal)),
            ("BolsaAutoconsumoP1", GetType(Decimal)), ("BolsaAutoconsumoP2", GetType(Decimal)),
            ("BolsaAutoconsumoP3", GetType(Decimal)), ("BolsaAutoconsumoP4", GetType(Decimal)),
            ("BolsaAutoconsumoP5", GetType(Decimal)), ("BolsaAutoconsumoP6", GetType(Decimal)),
            ("PerdidasP1", GetType(Decimal)), ("PerdidasP2", GetType(Decimal)), ("PerdidasP3", GetType(Decimal)),
            ("PerdidasP4", GetType(Decimal)), ("PerdidasP5", GetType(Decimal)), ("PerdidasP6", GetType(Decimal)),
            ("CantRecP1", GetType(Decimal)), ("CantRecP2", GetType(Decimal)), ("CantRecP3", GetType(Decimal)),
            ("CantRecP4", GetType(Decimal)), ("CantRecP5", GetType(Decimal)), ("CantRecP6", GetType(Decimal)),
            ("TotConsumoP1", GetType(Decimal)), ("TotConsumoP2", GetType(Decimal)), ("TotConsumoP3", GetType(Decimal)),
            ("TotConsumoP4", GetType(Decimal)), ("TotConsumoP5", GetType(Decimal)), ("TotConsumoP6", GetType(Decimal)),
            ("TextoTarifaGrupo", GetType(String)),
            ("TlfnoReclamaciones", GetType(String)),
            ("WebReclamaciones", GetType(String)),
            ("EmailReclamaciones", GetType(String)),
            ("TlfnoLecturas", GetType(String)),
            ("NombreTitularCups", GetType(String)),
            ("NombreAsesor", GetType(String)),
            ("EmailAsesor", GetType(String)),
            ("IsPermanenciaContrato", GetType(Boolean)),
            ("ConsumoMedioDiarioEuros", GetType(Decimal)),
            ("ConsumoMedioUltimos14MesesEuros", GetType(Decimal)),
            ("ConsumoMedioUltimos12MesesEuros", GetType(Decimal)),
            ("Mibgas", GetType(Decimal)), ("TechoGas", GetType(Decimal)), ("CapGas", GetType(Decimal)),
            ("OMIE", GetType(Decimal)), ("DivisorGas", GetType(Decimal)),
            ("Articulo3", GetType(Decimal)), ("Articulo7", GetType(Decimal)),
            ("OmieYArticulo3", GetType(Decimal)), ("OmieYArticulo7", GetType(Decimal)),
            ("SaldoAnteriorBateriaVirtual", GetType(Decimal)), ("SaldoGeneradoBateriaVirtual", GetType(Decimal)),
            ("SaldoCompensacionSimplificadaBateriaVirtual", GetType(Decimal)),
            ("SaldoDescuentoFinalBateriaVirtual", GetType(Decimal)), ("SaldoFinalBateriaVirtual", GetType(Decimal)),
            ("CodTicketBAI", GetType(String)), ("UrlQRTicketBAI", GetType(String)),
            ("CodigoQRImagen", GetType(Byte())),
            ("DescripcionPotyCargAcceso", GetType(String)), ("DescripcionENyCargAcceso", GetType(String)),
            ("ImporteENyCargAcceso", GetType(String)), ("ImportePotyCargAcceso", GetType(String)),
            ("FactorEmisionCO2", GetType(Decimal)),
            ("ATCUD", GetType(String)), ("HashDocument", GetType(String)),
            ("IdAgente", GetType(String)), ("NombreAgente", GetType(String)),
            ("IsBateriaVirtualMultiple", GetType(Boolean)),
            ("SaldoAnteriorMultiple", GetType(Decimal)), ("SaldoGeneradoMultiple", GetType(Decimal)),
            ("SaldoCompensacionSimplificadaMultiple", GetType(Decimal)),
            ("SaldoDescuentoMultiple", GetType(Decimal)), ("SaldoFinalMultiple", GetType(Decimal)),
            ("ReferenciaB2B", GetType(String)), ("ReferenciaBanco", GetType(String)),
            ("TipoCompensacionAutoConsumoBateriaVirtual", GetType(Integer)),
            ("UrlQRVerifactu", GetType(String)), ("HashVerifactu", GetType(String)),
            ("PorcentajeIncrementoEnergia", GetType(Decimal)),
            ("MotivoFacturacion", GetType(String)), ("NumeroExpediente", GetType(String)),
            ("CodigoOficinaContable", GetType(String)), ("CodigoOrganoGestor", GetType(String)),
            ("CodigoUnidadTramitadora", GetType(String)),
            ("IdInfraestructura", GetType(Long))
        })

        ' Potencias por factura: clave=IdFacturaVentaCabecera, lista ordenada por fila (P1=row0, P2=row1...)
        Dim potenciasPorFactura As New Dictionary(Of Long, List(Of (Decimal, String)))
        For Each potRow As DataRow In dtPot.Rows
            Dim idFact As Long = GetLong(potRow, "IdFacturaVentaCabecera")
            Dim pot As Decimal = GetDec(potRow, "PotenciaContratada")
            Dim texto As String = GetStr(potRow, "TextoTarifaPeriodo")
            If Not potenciasPorFactura.ContainsKey(idFact) Then
                potenciasPorFactura(idFact) = New List(Of (Decimal, String))
            End If
            potenciasPorFactura(idFact).Add((pot, texto))
        Next

        ' ATR por factura y periodo (primera ocurrencia = ATR base; segunda = cargo)
        Dim atrPorFactura As New Dictionary(Of Long, Dictionary(Of Integer, ATRPeriodoData))
        For Each linRow As DataRow In dtLineas.Rows
            Dim idFact As Long = GetLong(linRow, "IdFacturaVentaCabecera")
            Dim periodo As Integer = 0
            Try
                If linRow.Table.Columns.Contains("CodigoPeriodo") AndAlso Not linRow.IsNull("CodigoPeriodo") Then
                    periodo = CInt(linRow("CodigoPeriodo"))
                End If
            Catch
            End Try
            If periodo < 1 OrElse periodo > 6 Then Continue For

            If Not atrPorFactura.ContainsKey(idFact) Then
                atrPorFactura(idFact) = New Dictionary(Of Integer, ATRPeriodoData)
            End If
            If Not atrPorFactura(idFact).ContainsKey(periodo) Then
                atrPorFactura(idFact)(periodo) = New ATRPeriodoData()
            End If

            Dim atr = atrPorFactura(idFact)(periodo)
            Dim importe As Decimal = GetDec(linRow, "ImporteBase")
            Dim precio As Decimal = GetDec(linRow, "PrecioMedio")
            Dim potencia As Decimal = GetDec(linRow, "PotenciaContratada")
            Dim descripcion As String = GetStr(linRow, "Descripcion")

            If potencia > 0 Then
                If atr.PotenciaImporte = 0D Then
                    atr.PotenciaImporte = importe
                    atr.PotenciaPrecio = precio
                    atr.PotenciaContratada = potencia
                    atr.PotenciaDescripcion = descripcion
                ElseIf atr.PotenciaCargoImporte = 0D Then
                    atr.PotenciaCargoImporte = importe
                    atr.PotenciaCargoPrecio = precio
                    atr.PotenciaCargoDescripcion = descripcion
                End If
            Else
                If atr.EnergiaImporte = 0D Then
                    atr.EnergiaImporte = importe
                    atr.EnergiaPrecio = precio
                    atr.EnergiaDescripcion = descripcion
                ElseIf atr.EnergiaCargoImporte = 0D Then
                    atr.EnergiaCargoImporte = importe
                    atr.EnergiaCargoPrecio = precio
                    atr.EnergiaCargoDescripcion = descripcion
                End If
            End If
        Next

        ' Índice dtExtra por IdFacturaVentaCabecera
        Dim extraPorFactura As New Dictionary(Of Long, DataRow)
        For Each exRow As DataRow In dtExtra.Rows
            Dim exId As Long = GetLong(exRow, "IdFacturaVentaCabecera")
            If Not extraPorFactura.ContainsKey(exId) Then
                extraPorFactura(exId) = exRow
            End If
        Next

        ' Construir fila por cada factura del SP principal
        For Each row As DataRow In dtSP.Rows
            Dim newRow = dt.NewRow()
            Dim idFact As Long = GetLong(row, "IdFacturaVentaCabecera")

            newRow("CodigoContrato") = GetStr(row, "CodigoContrato")
            newRow("FechaEmision") = GetDate(row, "FechaFactura")
            newRow("NumFactura") = GetStr(row, "NumeroFactura")
            newRow("Importe") = GetDec(row, "ImporteTotal")
            newRow("TotalImporteTotal") = GetDec(row, "ImporteTotal")
            newRow("FechaFinContrato") = GetDate(row, "FechaVto")
            newRow("NombreEnvio") = GetStr(row, "NombreE")
            newRow("DireccionEnvio") = BuildDireccion(GetStr(row, "TextoImp"), GetStr(row, "NombreCalle"), GetStr(row, "NumeroE"), GetStr(row, "AclaradorE"))
            newRow("CPEnvio") = GetStr(row, "CodPostalE")
            newRow("PoblacionEnvio") = GetStr(row, "TextoCiudad")
            newRow("ProvinciaEnvio") = GetStr(row, "ProvinciaEnvio")
            newRow("NombreDistribuidora") = GetStr(row, "NombreDistribuidora")
            newRow("WebDistribuidora") = GetStr(row, "WebDistribuidora")

            Dim razonSocial = GetStr(row, "RazonSocial")
            If String.IsNullOrEmpty(razonSocial) Then
                newRow("NombreTitular") = String.Join(" ", {GetStr(row, "Nombre"), GetStr(row, "Apellido1"), GetStr(row, "Apellido2")}.Where(Function(s) Not String.IsNullOrEmpty(s)))
            Else
                newRow("NombreTitular") = razonSocial
            End If

            newRow("DireccionCUPS") = BuildDireccion(GetStr(row, "TextoImpCUPS"), GetStr(row, "NombreCalleCUPS"), "", GetStr(row, "Aclarador"))
            newRow("PoblacionCUPS") = GetStr(row, "TextoCiudadCUPS")
            newRow("CPCUPS") = GetStr(row, "CPCUPS")
            newRow("ProvinciaCUPS") = GetStr(row, "CUPSProvincia")
            newRow("DireccionCliente") = BuildDireccion(GetStr(row, "TextoImpCliente"), GetStr(row, "NombreCalleCliente"), GetStr(row, "NumeroCliente"), GetStr(row, "ClienteAclarador"))
            newRow("ProvinciaCliente") = GetStr(row, "TextoProvinciaCliente")
            newRow("PoblacionCliente") = GetStr(row, "TextoCiudadCliente")
            newRow("CPCliente") = GetStr(row, "CPCliente")
            newRow("NIF") = GetStr(row, "Identidad")
            newRow("Identidad") = GetStr(row, "Identidad")
            newRow("ReferenciaCatastral") = GetStr(row, "RefCatastral")
            newRow("CUPS") = GetStr(row, "CodigoCUPS")
            newRow("CNAE") = GetStr(row, "CodigoCNAE")
            newRow("Tension") = GetStr(row, "TextoTension")
            newRow("TarifaAcceso") = GetStr(row, "TextoTarifaPeaje")
            newRow("TextoTarifaPeaje") = GetStr(row, "TextoTarifaPeaje")
            newRow("NumeroContador") = GetStr(row, "NumeroContador")
            newRow("TelefonoAverias") = GetStr(row, "TelefonoAveria")
            newRow("FormaPago") = GetStr(row, "TextoTipoCobro")
            newRow("EntidadBancaria") = GetStr(row, "TextoBanco")
            newRow("NumeroCuenta") = GetStr(row, "IBAN")
            newRow("FechaPago") = GetDate(row, "FechaVencimiento")
            newRow("BaseImponible") = GetDec(row, "ImporteBase")
            newRow("IVA") = GetDec(row, "PorcentajeImpuesto")
            newRow("IdCliente") = GetLong(row, "IdCliente")
            newRow("IdFacturaCabeceraVenta") = idFact
            newRow("Cultura") = GetStr(row, "Cultura")
            newRow("SerieFactura") = GetStr(row, "SerieFactura")
            newRow("NombreFiscal") = GetStr(row, "NombreFiscal")
            newRow("Remesa") = GetBool(row, "Remesa")
            newRow("IsDuplicadoFactura") = False
            newRow("CodigoExterno1") = GetStr(row, "CodigoExterno1")
            newRow("RefExt1") = GetStr(row, "RefExt1")
            newRow("RefExt2") = GetStr(row, "RefExt2")
            newRow("TipoContrato") = GetStr(row, "TextoContratoTipo")
            newRow("FacturaCategoria") = GetStr(row, "FacturaCategoria")
            newRow("IsAbono") = GetBool(row, "IsAbono")
            newRow("TipoEquipoMedida") = GetStr(row, "TipoEquipo")
            newRow("TextoTarifa") = GetStr(row, "TextoTarifa")
            newRow("IsPVPCBS") = GetBool(row, "IsPVPCBS")
            newRow("CodigoTipoTelegestion") = GetStr(row, "CodigoTipoTelegestion")
            newRow("IsEmpleado") = GetBool(row, "IsEmpleado")
            newRow("URLQR") = GetStr(row, "URLQR")
            newRow("CadenaOrdenEmision") = GetStr(row, "CodigoContrato")
            newRow("NumPedidoFacturacion") = GetStr(row, "NumPedidoFacturacion")
            newRow("SerieFacturaOrigen") = GetStr(row, "SerieFacturaOrigen")
            newRow("NumFacturaOrigen") = GetStr(row, "NumeroFacturaOrigen")
            newRow("SerieFacturaUnica") = GetStr(row, "SerieFacturaUnica")
            newRow("NumFacturaUnica") = GetStr(row, "NumeroFacturaUnica")
            newRow("ReferenciaB2B") = GetStr(row, "ReferenciaB2B")
            newRow("ReferenciaBanco") = GetStr(row, "ReferenciaBanco")
            newRow("IsBateriaVirtual") = GetBool(row, "IsBateriaVirtual")
            newRow("IsBateriaVirtualMultiple") = GetBool(row, "IsBateriaVirtualMultiple")
            newRow("SaldoAnteriorBateriaVirtual") = GetDec(row, "SaldoAnterior")
            newRow("SaldoGeneradoBateriaVirtual") = GetDec(row, "SaldoGenerado")
            newRow("SaldoCompensacionSimplificadaBateriaVirtual") = GetDec(row, "SaldoCompensacionSimplificada")
            newRow("SaldoDescuentoFinalBateriaVirtual") = GetDec(row, "SaldoDescuentoFinal")
            newRow("SaldoFinalBateriaVirtual") = GetDec(row, "SaldoFinal")
            newRow("SaldoAnteriorMultiple") = GetDec(row, "SaldoAnteriorMultiple")
            newRow("SaldoGeneradoMultiple") = GetDec(row, "SaldoGeneradoMultiple")
            newRow("SaldoCompensacionSimplificadaMultiple") = GetDec(row, "SaldoCompensacionSimplificadaMultiple")
            newRow("SaldoDescuentoMultiple") = GetDec(row, "SaldoDescuentoMultiple")
            newRow("SaldoFinalMultiple") = GetDec(row, "SaldoFinalMultiple")
            newRow("CodTicketBAI") = GetStr(row, "CodTicketBAI")
            newRow("UrlQRTicketBAI") = GetStr(row, "UrlQRTicketBAI")
            newRow("CodigoQRImagen") = New Byte() {}
            newRow("ATCUD") = GetStr(row, "ATCUD")
            newRow("HashDocument") = GetStr(row, "HashDocument")
            newRow("IdAgente") = GetStr(row, "IdAgente")
            newRow("NombreAgente") = GetStr(row, "NombreAgente")
            newRow("TipoCompensacionAutoConsumoBateriaVirtual") = GetLong(row, "TipoCompensacionAutoConsumoBateriaVirtual")
            newRow("UrlQRVerifactu") = GetStr(row, "UrlQRVerifactu")
            newRow("HashVerifactu") = GetStr(row, "hashVerifactu")
            newRow("PorcentajeIncrementoEnergia") = GetDec(row, "PorcentajeIncrementoEnergia")
            newRow("IdInfraestructura") = GetLong(row, "IdInfraestructura")
            newRow("TfnoAtCliente") = GetStr(row, "TfnoAtCliente")
            newRow("TlfnoReclamaciones") = GetStr(row, "TfnoReclamaciones")
            newRow("WebReclamaciones") = GetStr(row, "WebReclamaciones")
            newRow("TlfnoLecturas") = GetStr(row, "TlfnoLecturas")
            newRow("TextoTarifaGrupo") = GetStr(row, "TextoTarifaGrupo")
            newRow("NombreTitularCups") = GetStr(row, "NombreTitularCUPS")
            newRow("NombreAsesor") = GetStr(row, "NombreAsesor")
            newRow("EmailAsesor") = GetStr(row, "EmailAsesor")
            newRow("IsPermanenciaContrato") = GetBool(row, "IsPermanenciaContrato")
            newRow("Emisora") = GetStr(row, "Identidad").Replace("-", "").Replace(" ", "")
            newRow("Referencia") = GetStr(row, "CodigoContrato")
            newRow("ExcedenteAutoconsumoP1") = GetDec(row, "ExcedenteAutoconsumoP1")
            newRow("ExcedenteAutoconsumoP2") = GetDec(row, "ExcedenteAutoconsumoP2")
            newRow("ExcedenteAutoconsumoP3") = GetDec(row, "ExcedenteAutoconsumoP3")
            newRow("ExcedenteAutoconsumoP4") = GetDec(row, "ExcedenteAutoconsumoP4")
            newRow("ExcedenteAutoconsumoP5") = GetDec(row, "ExcedenteAutoconsumoP5")
            newRow("ExcedenteAutoconsumoP6") = GetDec(row, "ExcedenteAutoconsumoP6")
            newRow("KwhCompensadosP1") = GetDec(row, "KwhCompensadosP1")
            newRow("KwhCompensadosP2") = GetDec(row, "KwhCompensadosP2")
            newRow("KwhCompensadosP3") = GetDec(row, "KwhCompensadosP3")
            newRow("KwhCompensadosP4") = GetDec(row, "KwhCompensadosP4")
            newRow("KwhCompensadosP5") = GetDec(row, "KwhCompensadosP5")
            newRow("KwhCompensadosP6") = GetDec(row, "KwhCompensadosP6")
            newRow("BolsaAutoconsumoP1") = GetDec(row, "BolsaAutoconsumoP1")
            newRow("BolsaAutoconsumoP2") = GetDec(row, "BolsaAutoconsumoP2")
            newRow("BolsaAutoconsumoP3") = GetDec(row, "BolsaAutoconsumoP3")
            newRow("BolsaAutoconsumoP4") = GetDec(row, "BolsaAutoconsumoP4")
            newRow("BolsaAutoconsumoP5") = GetDec(row, "BolsaAutoconsumoP5")
            newRow("BolsaAutoconsumoP6") = GetDec(row, "BolsaAutoconsumoP6")

            ' InfoCabeceraXML - parsear campos derivados
            Dim infoXml As String = GetStr(row, "InfoCabeceraXML")
            newRow("FechaInicioFactura") = GetXmlDate(infoXml, "FechaInicio")
            newRow("FechaFinFactura") = GetXmlDate(infoXml, "FechaFin")
            newRow("DesdeFechaEmision") = GetXmlDate(infoXml, "FechaInicio")
            newRow("HastaFechaEmision") = GetXmlDate(infoXml, "FechaFin")
            newRow("FechaFinVulnerabilidad") = GetXmlDate(infoXml, "FechaFinVulnerabilidad")
            newRow("FechaFinPenalizacion") = GetXmlDate(infoXml, "FechaFinPenalizacion")
            newRow("ImporteCosteRegulado") = GetXmlDbl(infoXml, "ImporteCosteRegulado")
            newRow("ImporteCosteEnergia") = GetXmlDbl(infoXml, "ImporteCosteEnergia")
            newRow("ImporteCostePotencia") = GetXmlDbl(infoXml, "ImporteCostePotencia")
            newRow("ImporteCargosEnergia") = GetXmlDbl(infoXml, "ImporteCargosEnergia")
            newRow("ImporteCargosPotencia") = GetXmlDbl(infoXml, "ImporteCargosPotencia")
            newRow("ImporteCosteProduccion") = GetXmlDbl(infoXml, "ImporteCosteProduccion")
            newRow("ImporteCosteCargos") = GetXmlDbl(infoXml, "ImporteCosteCargos")
            newRow("ImporteImpuesto") = GetXmlDbl(infoXml, "ImporteImpuesto")
            newRow("ImporteAlquiler") = GetXmlDbl(infoXml, "ImporteAlquiler")
            newRow("ImporteAlquilerEquipo") = GetXmlDec(infoXml, "ImporteAlquilerEquipo")
            newRow("CosteIncentivo") = GetXmlDbl(infoXml, "CosteIncentivo")
            newRow("CosteRedes") = GetXmlDbl(infoXml, "CosteRedes")
            newRow("CosteOtros") = GetXmlDbl(infoXml, "CosteOtros")
            newRow("CosteRecoreCargos") = GetXmlDbl(infoXml, "CosteRecoreCargos")
            newRow("CosteDeficitCargos") = GetXmlDbl(infoXml, "CosteDeficitCargos")
            newRow("CosteTnpCargos") = GetXmlDbl(infoXml, "CosteTnpCargos")
            newRow("CosteOtrosCargos") = GetXmlDbl(infoXml, "CosteOtrosCargos")
            newRow("TextoBOE") = GetXmlChild(infoXml, "TextoBOE")
            newRow("Comentarios") = GetXmlChild(infoXml, "Comentarios")
            newRow("MotivoFacturacion") = GetXmlChild(infoXml, "MotivoFacturacion")
            newRow("NumeroExpediente") = GetXmlChild(infoXml, "NumeroExpediente")
            newRow("CodigoOficinaContable") = GetXmlChild(infoXml, "CodigoOficinaContable")
            newRow("CodigoOrganoGestor") = GetXmlChild(infoXml, "CodigoOrganoGestor")
            newRow("CodigoUnidadTramitadora") = GetXmlChild(infoXml, "CodigoUnidadTramitadora")
            newRow("ConsumoAcumuladoUltimoAnio") = GetXmlDec(infoXml, "ConsumoAcumuladoUltimoAnio")
            newRow("ConsumoMedioDiarioPeriodo") = GetXmlDec(infoXml, "ConsumoMedioDiarioPeriodo")
            newRow("ConsumoMedioDiarioUltimos14Meses") = GetXmlDec(infoXml, "ConsumoMedioDiarioUltimos14Meses")
            newRow("ConsumoMedioDiarioEuros") = GetXmlDec(infoXml, "ConsumoMedioDiarioEuros")
            newRow("ConsumoMedioUltimos14MesesEuros") = GetXmlDec(infoXml, "ConsumoMedioUltimos14MesesEuros")
            newRow("ConsumoMedioUltimos12MesesEuros") = GetXmlDec(infoXml, "ConsumoMedioUltimos12MesesEuros")
            newRow("DescripcionPotyCargAcceso") = GetXmlChild(infoXml, "DescripcionPotyCargAcceso")
            newRow("DescripcionENyCargAcceso") = GetXmlChild(infoXml, "DescripcionENyCargAcceso")
            newRow("ImporteENyCargAcceso") = GetXmlChild(infoXml, "ImporteENyCargAcceso")
            newRow("ImportePotyCargAcceso") = GetXmlChild(infoXml, "ImportePotyCargAcceso")
            newRow("FactorEmisionCO2") = GetXmlDec(infoXml, "FactorEmisionCO2")
            For i = 1 To 6
                newRow($"PerdidasP{i}") = GetXmlDec(infoXml, $"PerdidasP{i}")
                newRow($"CantRecP{i}") = GetXmlDec(infoXml, $"CantRecP{i}")
                newRow($"TotConsumoP{i}") = GetXmlDec(infoXml, $"TotConsumoP{i}")
                newRow($"PML{i}") = GetXmlDec(infoXml, $"PML{i}")
                newRow($"PPML{i}") = GetXmlDec(infoXml, $"PPML{i}")
                newRow($"SupConsumoP{i}") = GetXmlDec(infoXml, $"SupConsumoP{i}")
            Next

            ' Extra row (CodigoBarras, RegistroMercantil, IdIdiomaInforme, TextoIdioma, Email)
            Dim exRow As DataRow = Nothing
            If extraPorFactura.TryGetValue(idFact, exRow) Then
                newRow("CodigoBarras") = GetStr(exRow, "CodigoBarras")
                newRow("RegitroMercantil") = GetStr(exRow, "RegistroMercantil")
                newRow("IdIdiomaInforme") = GetLong(exRow, "IdIdiomaInforme")
                newRow("TextoIdioma") = GetStr(exRow, "TextoIdioma")
                newRow("EmailReclamaciones") = GetStr(exRow, "EmailComercializadora")
            End If

            ' FechaEmisionOrigen desde SP (factura origen)
            newRow("FechaEmisionOrigen") = GetDate(row, "FechaFacturaOrigen")

            ' P1-P6 potencias contratadas (pivotadas por orden de fila del SP de potencias)
            If potenciasPorFactura.ContainsKey(idFact) Then
                Dim potList = potenciasPorFactura(idFact)
                Dim periodoTextos = New List(Of String)
                For i = 0 To Math.Min(potList.Count - 1, 5)
                    newRow($"P{i + 1}") = potList(i).Item1
                    newRow($"PotenciaFacturadaP{i + 1}") = potList(i).Item1
                    periodoTextos.Add(potList(i).Item2)
                Next
                newRow("PotenciaContratadaPeriodo") = String.Join(" / ", periodoTextos)
                If potList.Count > 0 Then
                    newRow("PotenciaContratadaCantidad") = potList(0).Item1.ToString("0.000")
                End If
            End If

            ' ATR campos de energía y potencia (base + cargo) por periodo
            If atrPorFactura.ContainsKey(idFact) Then
                For Each kvp In atrPorFactura(idFact)
                    Dim p = kvp.Key
                    Dim atr = kvp.Value
                    If atr.EnergiaImporte <> 0D Then
                        newRow($"ATREnergiaP{p}Importe") = atr.EnergiaImporte
                        newRow($"ATREnergiaP{p}Precio") = atr.EnergiaPrecio
                        newRow($"ATREnergiaP{p}Descripcion") = atr.EnergiaDescripcion
                    End If
                    If atr.EnergiaCargoImporte <> 0D Then
                        newRow($"ATREnergiaCargoP{p}Importe") = atr.EnergiaCargoImporte
                        newRow($"ATREnergiaCargoP{p}Precio") = atr.EnergiaCargoPrecio
                        newRow($"ATREnergiaCargoP{p}Descripcion") = atr.EnergiaCargoDescripcion
                    End If
                    If atr.PotenciaImporte <> 0D Then
                        newRow($"ATRPotenciaP{p}Importe") = atr.PotenciaImporte
                        newRow($"ATRPotenciaP{p}Precio") = atr.PotenciaPrecio
                        newRow($"ATRPotenciaP{p}Descripcion") = atr.PotenciaDescripcion
                    End If
                    If atr.PotenciaCargoImporte <> 0D Then
                        newRow($"ATRPotenciaCargoP{p}Importe") = atr.PotenciaCargoImporte
                        newRow($"ATRPotenciaCargoP{p}Precio") = atr.PotenciaCargoPrecio
                        newRow($"ATRPotenciaCargoP{p}Descripcion") = atr.PotenciaCargoDescripcion
                    End If
                Next
            End If

            dt.Rows.Add(newRow)
        Next

        Return dt
    End Function

    Private Function BuildFacturasTotales(idsStr As String) As DataTable
        Dim dt As New DataTable("FacturasTotales")
        AddCols(dt, {
            ("IdFacturaCabeceraVenta", GetType(Long)),
            ("TotalImporteBase", GetType(Decimal)),
            ("TotalImporteTotal", GetType(Decimal)),
            ("PorcentajeImpuesto", GetType(Decimal)),
            ("ImporteImpuesto", GetType(Decimal)),
            ("Cultura", GetType(String)),
            ("TextoImpuesto", GetType(String)),
            ("ImporteAdicional", GetType(Decimal)),
            ("TextoImporteAdicional", GetType(String)),
            ("ImporteImpuestoCalculoEmpleadoCeuta", GetType(Decimal))
        })

        Dim query = "SELECT fvt.IdFacturaVentaCabecera, fvt.ImporteBase, fvt.ImporteTotal, " &
                    "fvt.PorcentajeImpuesto, fvt.ImporteImpuesto, " &
                    "ISNULL(ti.TextoImpuesto,'') AS TextoImpuesto, " &
                    "ISNULL(ii.Cultura,'ES-ES') AS Cultura " &
                    "FROM FacturaVentaTotal fvt " &
                    "INNER JOIN FacturaVentaCabecera fvc ON fvc.IdFacturaVentaCabecera = fvt.IdFacturaVentaCabecera " &
                    "INNER JOIN Contrato c ON c.CodigoContrato = fvc.CodigoContrato AND c.Entorno = fvc.Entorno " &
                    "LEFT JOIN TipoImpuesto ti ON ti.IdTipoImpuesto = fvt.IdTipoImpuesto " &
                    "LEFT JOIN IdiomaInforme ii ON ii.IdIdiomaInforme = c.IdIdiomaInforme " &
                    $"INNER JOIN dbo.StringSplit('{idsStr}', ',') F ON F.value = fvt.IdFacturaVentaCabecera"

        Try
            Dim dtSrc = EjecutarQuery(query)
            For Each row As DataRow In dtSrc.Rows
                Dim newRow = dt.NewRow()
                newRow("IdFacturaCabeceraVenta") = GetLong(row, "IdFacturaVentaCabecera")
                newRow("TotalImporteBase") = GetDec(row, "ImporteBase")
                newRow("TotalImporteTotal") = GetDec(row, "ImporteTotal")
                newRow("PorcentajeImpuesto") = GetDec(row, "PorcentajeImpuesto")
                newRow("ImporteImpuesto") = GetDec(row, "ImporteImpuesto")
                newRow("Cultura") = GetStr(row, "Cultura")
                newRow("TextoImpuesto") = GetStr(row, "TextoImpuesto")
                newRow("ImporteAdicional") = 0D
                newRow("TextoImporteAdicional") = String.Empty
                newRow("ImporteImpuestoCalculoEmpleadoCeuta") = GetDec(row, "ImporteBase")
                dt.Rows.Add(newRow)
            Next
        Catch
        End Try

        Return dt
    End Function

    Private Function BuildFacturasDetalles(idsStr As String) As DataTable
        Dim dtSP = EjecutarSP("GetInfoFacturasLineasReport", idsStr)

        Dim dt As New DataTable("FacturasDetalles")
        AddCols(dt, {
            ("IdFacturaCabeceraVenta", GetType(Long)),
            ("FacturaConcepto", GetType(Long)),
            ("TextoGrupo", GetType(String)),
            ("Descripcion", GetType(String)),
            ("TextoFacturaConcepto", GetType(String)),
            ("ImporteBase", GetType(Decimal)),
            ("OrdenFactura", GetType(Long)),
            ("Cultura", GetType(String)),
            ("DescripcionAmpliada", GetType(String)),
            ("PotenciaContratada", GetType(Decimal)),
            ("IsAjusteCAPGas", GetType(Boolean)),
            ("CodigoCUPS", GetType(String)),
            ("CambioNombreConcepto", GetType(Boolean)),
            ("AplicaGDOS", GetType(Boolean))
        })

        Dim orden As Long = 0
        For Each row As DataRow In dtSP.Rows
            orden += 1
            Dim newRow = dt.NewRow()
            newRow("IdFacturaCabeceraVenta") = GetLong(row, "IdFacturaVentaCabecera")
            newRow("FacturaConcepto") = GetLong(row, "FacturaConcepto")
            newRow("TextoGrupo") = String.Empty
            newRow("Descripcion") = GetStr(row, "Descripcion")
            newRow("TextoFacturaConcepto") = GetStr(row, "Descripcion")
            newRow("ImporteBase") = GetDec(row, "ImporteBase")
            newRow("OrdenFactura") = orden
            newRow("Cultura") = GetStr(row, "Cultura")
            newRow("DescripcionAmpliada") = GetStr(row, "DescripcionAmpliada")
            newRow("PotenciaContratada") = GetDec(row, "PotenciaContratada")
            newRow("IsAjusteCAPGas") = GetBool(row, "IsAjusteCAPGas")
            newRow("CodigoCUPS") = GetStr(row, "CodigoCUPS")
            newRow("CambioNombreConcepto") = False
            newRow("AplicaGDOS") = GetBool(row, "AplicaGDOS")
            dt.Rows.Add(newRow)
        Next

        Return dt
    End Function

    Private Function BuildFacturasLecturas(idsStr As String) As DataTable
        Dim dt As New DataTable("FacturasLecturas")
        AddCols(dt, {
            ("IdFacturaCabeceraVenta", GetType(Long)),
            ("IdLectura", GetType(Long)),
            ("ActivaAnterior", GetType(Decimal)),
            ("ActivaActual", GetType(Decimal)),
            ("ReactivaAnterior", GetType(Decimal)),
            ("ReactivaActual", GetType(Decimal)),
            ("Maximetro", GetType(Decimal)),
            ("MaximetroExceso", GetType(Decimal)),
            ("FechaLectura", GetType(DateTime)),
            ("FechaLecturaAnterior", GetType(DateTime)),
            ("FechaAnteriorPotencia", GetType(DateTime)),
            ("FechaActualPotencia", GetType(DateTime)),
            ("FechaAnioMovil", GetType(DateTime)),
            ("ConsumoActiva", GetType(Decimal)),
            ("ConsumoReactiva", GetType(Decimal)),
            ("PotenciaMaxP1", GetType(Decimal)), ("PotenciaMaxP2", GetType(Decimal)), ("PotenciaMaxP3", GetType(Decimal)),
            ("PotenciaMaxP4", GetType(Decimal)), ("PotenciaMaxP5", GetType(Decimal)), ("PotenciaMaxP6", GetType(Decimal)),
            ("ValorEnergiaCP", GetType(Decimal)),
            ("TextoOrigen", GetType(String)),
            ("Cultura", GetType(String)),
            ("TextoTarifaPeriodoLectura", GetType(String)),
            ("IsLecturaDobleContador", GetType(Boolean)),
            ("ActivaExtra", GetType(Decimal)),
            ("ReactivaExtra", GetType(Decimal)),
            ("IsRegularizadora", GetType(Boolean)),
            ("Rectificativa", GetType(Boolean)),
            ("IsInspeccion", GetType(Boolean)),
            ("TextoTipoTelegestion", GetType(String)),
            ("TextoTelegestion", GetType(String))
        })

        Dim query = "SELECT l.IdLectura, l.IdFacturaVentaCabeceraSectorC AS IdFacturaVentaCabecera, " &
                    "ISNULL(l.LecturaActualAnterior, 0) AS ActivaAnterior, " &
                    "ISNULL(l.LecturaActual, 0) AS ActivaActual, " &
                    "ISNULL(l.LecturaReactivaAnterior, 0) AS ReactivaAnterior, " &
                    "ISNULL(l.LecturaReactivaActual, 0) AS ReactivaActual, " &
                    "ISNULL(l.Maximetre, 0) AS Maximetro, " &
                    "ISNULL(l.MaximetreExces, 0) AS MaximetroExceso, " &
                    "l.FechaLectura, l.FechaLecturaAnterior, " &
                    "l.FechaLecturaAnterior AS FechaAnteriorPotencia, " &
                    "l.FechaLectura AS FechaActualPotencia, " &
                    "ISNULL(l.LecturaActual, 0) - ISNULL(l.LecturaActualAnterior, 0) AS ConsumoActiva, " &
                    "ISNULL(l.LecturaReactivaActual, 0) - ISNULL(l.LecturaReactivaAnterior, 0) AS ConsumoReactiva, " &
                    "ISNULL(l.Maximetre, 0) AS PotenciaMaxP1, " &
                    "ISNULL(l.IsRegularizadora, 0) AS IsRegularizadora, " &
                    "ISNULL(ii.Cultura,'ES-ES') AS Cultura " &
                    "FROM Lectura l " &
                    "INNER JOIN FacturaVentaCabecera fvc ON fvc.IdFacturaVentaCabecera = l.IdFacturaVentaCabeceraSectorC " &
                    "INNER JOIN Contrato c ON c.CodigoContrato = fvc.CodigoContrato AND c.Entorno = fvc.Entorno " &
                    "LEFT JOIN IdiomaInforme ii ON ii.IdIdiomaInforme = c.IdIdiomaInforme " &
                    $"INNER JOIN dbo.StringSplit('{idsStr}', ',') F ON F.value = l.IdFacturaVentaCabeceraSectorC"

        Try
            Dim dtSrc = EjecutarQuery(query)
            For Each row As DataRow In dtSrc.Rows
                Dim newRow = dt.NewRow()
                newRow("IdFacturaCabeceraVenta") = GetLong(row, "IdFacturaVentaCabecera")
                newRow("IdLectura") = GetLong(row, "IdLectura")
                newRow("ActivaAnterior") = GetDec(row, "ActivaAnterior")
                newRow("ActivaActual") = GetDec(row, "ActivaActual")
                newRow("ReactivaAnterior") = GetDec(row, "ReactivaAnterior")
                newRow("ReactivaActual") = GetDec(row, "ReactivaActual")
                newRow("Maximetro") = GetDec(row, "Maximetro")
                newRow("MaximetroExceso") = GetDec(row, "MaximetroExceso")
                newRow("FechaLectura") = GetDate(row, "FechaLectura")
                newRow("FechaLecturaAnterior") = GetDate(row, "FechaLecturaAnterior")
                newRow("FechaAnteriorPotencia") = GetDate(row, "FechaAnteriorPotencia")
                newRow("FechaActualPotencia") = GetDate(row, "FechaActualPotencia")
                newRow("ConsumoActiva") = GetDec(row, "ConsumoActiva")
                newRow("ConsumoReactiva") = GetDec(row, "ConsumoReactiva")
                newRow("PotenciaMaxP1") = GetDec(row, "PotenciaMaxP1")
                newRow("IsRegularizadora") = GetBool(row, "IsRegularizadora")
                newRow("Cultura") = GetStr(row, "Cultura")
                newRow("TextoTipoTelegestion") = GetStr(row, "TextoTipoTelegestion")
                newRow("TextoOrigen") = String.Empty
                newRow("TextoTarifaPeriodoLectura") = String.Empty
                newRow("IsLecturaDobleContador") = False
                newRow("ActivaExtra") = 0D
                newRow("ReactivaExtra") = 0D
                newRow("Rectificativa") = False
                newRow("IsInspeccion") = False
                newRow("TextoTelegestion") = String.Empty
                newRow("ValorEnergiaCP") = 0D
                dt.Rows.Add(newRow)
            Next
        Catch
        End Try

        Return dt
    End Function

    Private Function BuildFacturasATR(idsStr As String) As DataTable
        Dim dtLineas = EjecutarSP("GetInfoFacturasLineasReport", idsStr)

        Dim dt As New DataTable("FacturasATR")
        AddCols(dt, {
            ("IdFacturaCabeceraVenta", GetType(Long)),
            ("Periodo", GetType(String)),
            ("Potencia", GetType(Decimal)),
            ("Consumo", GetType(Decimal)),
            ("Precio", GetType(Decimal)),
            ("Descripcion", GetType(String)),
            ("Importe", GetType(Decimal)),
            ("Cultura", GetType(String)),
            ("Tipo", GetType(String))
        })

        For Each row As DataRow In dtLineas.Rows
            Dim periodo As Integer = 0
            Try
                If row.Table.Columns.Contains("CodigoPeriodo") AndAlso Not row.IsNull("CodigoPeriodo") Then
                    periodo = CInt(row("CodigoPeriodo"))
                End If
            Catch
            End Try
            If periodo < 1 OrElse periodo > 6 Then Continue For

            Dim potencia As Decimal = GetDec(row, "PotenciaContratada")
            Dim consumo As Decimal = GetDec(row, "ConsumoXML")
            If potencia = 0D AndAlso consumo = 0D Then Continue For

            Dim newRow = dt.NewRow()
            newRow("IdFacturaCabeceraVenta") = GetLong(row, "IdFacturaVentaCabecera")
            newRow("Periodo") = $"P{periodo}"
            newRow("Potencia") = potencia
            newRow("Consumo") = consumo
            newRow("Precio") = GetDec(row, "PrecioMedio")
            newRow("Descripcion") = GetStr(row, "Descripcion")
            newRow("Importe") = GetDec(row, "ImporteBase")
            newRow("Cultura") = GetStr(row, "Cultura")
            newRow("Tipo") = If(potencia > 0, "P", "E")
            dt.Rows.Add(newRow)
        Next

        Return dt
    End Function

    Private Function BuildFacturasConsumos(idsStr As String) As DataTable
        Dim dt As New DataTable("FacturasConsumos")
        AddCols(dt, {
            ("IdFacturaCabeceraVenta", GetType(Long)),
            ("TextoTarifaPeajePeriodo", GetType(String)),
            ("Consumo", GetType(Double)),
            ("ConsumoActivaP1", GetType(Decimal)), ("ConsumoActivaP2", GetType(Decimal)),
            ("ConsumoActivaP3", GetType(Decimal)), ("ConsumoActivaP4", GetType(Decimal)),
            ("ConsumoActivaP5", GetType(Decimal)), ("ConsumoActivaP6", GetType(Decimal)),
            ("Ano", GetType(Long)),
            ("FechaInicio", GetType(DateTime)), ("FechaFin", GetType(DateTime)),
            ("Cultura", GetType(String))
        })

        Dim query = "SELECT l.IdFacturaVentaCabeceraSectorC AS IdFacturaVentaCabecera, " &
                    "YEAR(l.FechaLectura) AS Ano, l.FechaLecturaAnterior AS FechaInicio, " &
                    "l.FechaLectura AS FechaFin, " &
                    "ISNULL(l.LecturaActual,0) - ISNULL(l.LecturaActualAnterior,0) AS ConsumoTotal, " &
                    "ISNULL(ii.Cultura,'ES-ES') AS Cultura " &
                    "FROM Lectura l " &
                    "INNER JOIN FacturaVentaCabecera fvc ON fvc.IdFacturaVentaCabecera = l.IdFacturaVentaCabeceraSectorC " &
                    "INNER JOIN Contrato c ON c.CodigoContrato = fvc.CodigoContrato AND c.Entorno = fvc.Entorno " &
                    "LEFT JOIN IdiomaInforme ii ON ii.IdIdiomaInforme = c.IdIdiomaInforme " &
                    $"INNER JOIN dbo.StringSplit('{idsStr}',',') F ON F.value = l.IdFacturaVentaCabeceraSectorC"
        Try
            Dim dtSrc = EjecutarQuery(query)
            For Each row As DataRow In dtSrc.Rows
                Dim newRow = dt.NewRow()
                newRow("IdFacturaCabeceraVenta") = GetLong(row, "IdFacturaVentaCabecera")
                newRow("Ano") = GetLong(row, "Ano")
                newRow("FechaInicio") = GetDate(row, "FechaInicio")
                newRow("FechaFin") = GetDate(row, "FechaFin")
                newRow("Consumo") = GetDbl(row, "ConsumoTotal")
                newRow("Cultura") = GetStr(row, "Cultura")
                newRow("TextoTarifaPeajePeriodo") = String.Empty
                dt.Rows.Add(newRow)
            Next
        Catch
        End Try

        Return dt
    End Function

    Private Function BuildEtiquetas(idsStr As String) As DataTable
        Dim dt As New DataTable("Etiquetas")
        AddCols(dt, {
            ("Cultura", GetType(String)),
            ("Etiquetas", GetType(String))
        })

        Dim query = "SELECT DISTINCT ISNULL(ii.Cultura,'ES-ES') AS Cultura " &
                    "FROM FacturaVentaCabecera fvc " &
                    "INNER JOIN Contrato c ON c.CodigoContrato = fvc.CodigoContrato AND c.Entorno = fvc.Entorno " &
                    "LEFT JOIN IdiomaInforme ii ON ii.IdIdiomaInforme = c.IdIdiomaInforme " &
                    $"INNER JOIN dbo.StringSplit('{idsStr}',',') F ON F.value = fvc.IdFacturaVentaCabecera"
        Try
            Dim dtSrc = EjecutarQuery(query)
            For Each row As DataRow In dtSrc.Rows
                Dim newRow = dt.NewRow()
                newRow("Cultura") = GetStr(row, "Cultura")
                newRow("Etiquetas") = String.Empty
                dt.Rows.Add(newRow)
            Next
        Catch
        End Try

        Return dt
    End Function

    Private Function BuildAgentes(idsStr As String) As DataTable
        Dim dt As New DataTable("Agentes")
        AddCols(dt, {
            ("IDEntityDTO", GetType(Long)), ("IdAgente", GetType(Long)),
            ("Entorno", GetType(String)), ("NombreAgente", GetType(String)),
            ("Direccion", GetType(String)), ("CodigoPostal", GetType(String)),
            ("Ciudad", GetType(String)), ("Telefono", GetType(String)),
            ("Movil", GetType(String)), ("email", GetType(String)),
            ("Web", GetType(String)), ("IdProveedor", GetType(Long)),
            ("IdAgenteGrupo", GetType(Long)), ("Notas", GetType(String)),
            ("CodigoTipoAgente", GetType(Integer)), ("IdAgenteNivelAnterior", GetType(Long)),
            ("EmailSolicitud", GetType(String)), ("CodigoVendedor", GetType(String)),
            ("IdPerfilCanal", GetType(Long)), ("IdAgenteTipoVenta", GetType(Long)),
            ("CodigoTipoVenta", GetType(Integer)), ("IdModeloFactura", GetType(Long)),
            ("IdTipoDistribuidor", GetType(Long)), ("IsOcultarComisiones", GetType(Boolean)),
            ("IsFirmaEmail", GetType(Boolean)), ("IsFirmaSMS", GetType(Boolean)),
            ("IsFirmaPapel", GetType(Boolean)), ("IdZonaVenta", GetType(Long)),
            ("IsOcultarTickets", GetType(Boolean)),
            ("IncluirLogoEnFactura", GetType(Boolean)), ("IsSeleccionado", GetType(Boolean)),
            ("IdAgenteOrigen", GetType(Long)), ("IdAgenteSubLogo", GetType(String))
        })

        Dim query = "SELECT DISTINCT a.IdAgente, a.NombreAgente, " &
                    "ISNULL(a.Email,'') AS email, ISNULL(a.Web,'') AS Web, " &
                    "ISNULL(a.Telefono,'') AS Telefono, ISNULL(a.Movil,'') AS Movil, " &
                    "ISNULL(a.CodigoVendedor,'') AS CodigoVendedor " &
                    "FROM FacturaVentaCabecera fvc " &
                    "INNER JOIN Contrato c ON c.CodigoContrato = fvc.CodigoContrato AND c.Entorno = fvc.Entorno " &
                    "INNER JOIN Agente a ON a.IdAgente = c.IdAgente " &
                    $"INNER JOIN dbo.StringSplit('{idsStr}',',') F ON F.value = fvc.IdFacturaVentaCabecera"
        Try
            Dim dtSrc = EjecutarQuery(query)
            For Each row As DataRow In dtSrc.Rows
                Dim newRow = dt.NewRow()
                newRow("IdAgente") = GetLong(row, "IdAgente")
                newRow("NombreAgente") = GetStr(row, "NombreAgente")
                newRow("email") = GetStr(row, "email")
                newRow("Web") = GetStr(row, "Web")
                newRow("Telefono") = GetStr(row, "Telefono")
                newRow("Movil") = GetStr(row, "Movil")
                newRow("CodigoVendedor") = GetStr(row, "CodigoVendedor")
                newRow("IsSeleccionado") = True
                dt.Rows.Add(newRow)
            Next
        Catch
        End Try

        Return dt
    End Function

    Private Function BuildEmptyRecargas() As DataTable
        Dim dt As New DataTable("Recargas")
        AddCols(dt, {
            ("IdFacturaVentaCabecera", GetType(Long)), ("IdLectura", GetType(Long)),
            ("Recarga", GetType(Decimal)), ("MesesFactura", GetType(String)),
            ("IdsRecarga", GetType(String)), ("FechaRecarga", GetType(DateTime)),
            ("FechaFactura", GetType(DateTime)), ("Importe", GetType(Decimal)),
            ("Factura", GetType(String)), ("Consumo", GetType(Decimal)),
            ("FechaLecturaAnterior", GetType(DateTime)), ("FechaLectura", GetType(DateTime)),
            ("TextoPeriodo", GetType(String)), ("ValorMaximo", GetType(Decimal)),
            ("Mes", GetType(Integer)), ("Anio", GetType(Integer))
        })
        Return dt
    End Function

    Private Function BuildEmptyDescuentosRecarga() As DataTable
        Dim dt As New DataTable("DescuentosRecarga")
        AddCols(dt, {
            ("IdFacturaVentaCabecera", GetType(Long)), ("TipoCargoDescuento", GetType(Integer)),
            ("TextoFactura", GetType(String)), ("CargoDescuentoEnergia", GetType(Decimal))
        })
        Return dt
    End Function

    Private Function BuildEmptyFacturasLecturasAutoconsumo() As DataTable
        Dim dt As New DataTable("FacturasLecturasAutoconsumo")
        AddCols(dt, {
            ("IdFacturaCabeceraVenta", GetType(Long)), ("Observaciones", GetType(String)),
            ("FechaLectura", GetType(DateTime)), ("FechaLecturaAnterior", GetType(DateTime)),
            ("PorcentajeReparto", GetType(Decimal)), ("TextoTarifaPeajePeriodoLectura", GetType(String)),
            ("ActivaAnterior", GetType(Decimal)), ("ActivaActual", GetType(Decimal)),
            ("ConsumoActiva", GetType(Decimal)), ("GeneracionNetakWh", GetType(Decimal)),
            ("AutoconsumokWh", GetType(Decimal)), ("ExcedenteskWh", GetType(Decimal)),
            ("IdLecturaAutoconsumo", GetType(Long))
        })
        Return dt
    End Function

    Private Function BuildEmptyHistoricoExcedentes() As DataTable
        Dim dt As New DataTable("HistoricoExcedentes")
        AddCols(dt, {
            ("IdFacturaVentaCabecera", GetType(Long)), ("FechaInicio", GetType(DateTime)),
            ("FechaFin", GetType(DateTime)), ("Excendentes", GetType(Decimal)),
            ("Cultura", GetType(String))
        })
        Return dt
    End Function

    Private Function BuildEmptySaldoDescuentos() As DataTable
        Dim dt As New DataTable("SaldoDescuentos")
        AddCols(dt, {
            ("IdFacturaVentaCabecera", GetType(Long)), ("ImportePendienteTotal", GetType(Decimal)),
            ("ImporteDescuentoTotal", GetType(Decimal)), ("ImporteAplicadoTotal", GetType(Decimal)),
            ("TextoDescuento", GetType(String)), ("ImporteFactura", GetType(Decimal))
        })
        Return dt
    End Function

    Private Function BuildEmptyPeriodoConsumo() As DataTable
        Dim dt As New DataTable("PeriodoConsumo")
        AddCols(dt, {
            ("Año", GetType(Integer)), ("Periodo", GetType(Integer)),
            ("TextoPeriodo", GetType(String)), ("ConsumoActiva", GetType(Decimal)),
            ("ConsumoActivaP1", GetType(Decimal)), ("ConsumoActivaP2", GetType(Decimal)),
            ("ConsumoActivaP3", GetType(Decimal)), ("ConsumoActivaP4", GetType(Decimal)),
            ("ConsumoActivaP5", GetType(Decimal)), ("ConsumoActivaP6", GetType(Decimal)),
            ("ConsumoReactiva", GetType(Decimal)), ("DiasPeriodo", GetType(Integer)),
            ("FechaInicio", GetType(DateTime)), ("FechaFin", GetType(DateTime))
        })
        Return dt
    End Function

    Private Function BuildEmptyInfraestructuras() As DataTable
        Dim dt As New DataTable("Infraestructuras")
        AddCols(dt, {
            ("IDEntityDTO", GetType(Long)), ("IdInfraestructura", GetType(Long)),
            ("Entorno", GetType(String)), ("Codigo", GetType(String)),
            ("Descripcion", GetType(String)), ("EstadoGas", GetType(String)),
            ("CodigoEIC", GetType(String)), ("IsSociedadPT", GetType(Boolean)),
            ("Pais", GetType(String))
        })
        Return dt
    End Function

    Private Function BuildEmptyPassPoolDesglose() As DataTable
        Dim dt As New DataTable("PassPoolDesglose")
        AddCols(dt, {
            ("IdFacturaVentaCabecera", GetType(Long)), ("Periodo", GetType(String)),
            ("A", GetType(Decimal)), ("OMIE", GetType(Decimal)),
            ("B", GetType(Decimal)), ("Resultado", GetType(Decimal))
        })
        Return dt
    End Function

End Class
