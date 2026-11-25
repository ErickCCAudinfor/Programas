--Gestor SIGE _ Ciclo de impagos
--
--Viene de Consulta_Ciclo_Impagos_Ver.sql (Erick, 25/11/2026). SOLO LECTURA: SELECT y tablas
--temporales, no toca ni una fila.
--
--LO UNICO QUE CAMBIA respecto al original es el DECLARE de @Lista: ahora lo manda la
--aplicacion como PARAMETRO, no se concatena. El cuadro de facturas ya parte por lineas,
--quita comillas, comas y espacios, y descarta vacias y duplicadas, asi que aqui llega
--limpia y unida por «;». El STRING_SPLIT se queda igualmente: no estorba y permite pegar
--esta consulta en SSMS tal cual, poniendo el DECLARE a mano.
--
--La logica de negocio NO se ha tocado: los textos de diagnostico son los del original,
--porque los lee el BPO.

/* =====================================================================================
   Consulta_Ciclo_Impagos_Ver.sql  -  SOLO LECTURA
   Para una lista de facturas: en que punto del ciclo de impagos esta cada cartera
   (OPEN ITEMS -> devoluciones -> carta de aviso -> carta de corte -> remesado/cobrado),
   si saldria en Comunicados Impago para crear la carta de aviso o la de corte (y si no,
   por que) y que le pasara con el proximo OPEN ITEMS.

   Como usarla: pegar las facturas en @Lista, una por linea (tal cual de Excel) o con
   comillas y comas como en Consulta_Comunicados.sql. Acepta los dos formatos.
   Erick / 25-11-2026
   ===================================================================================== */
SET NOCOUNT ON;

DECLARE @Hoy date = CAST(GETDATE() AS date);

/* ---------- 1. Facturas de la lista ---------- */
IF OBJECT_ID('tempdb..#Consulta_Ciclo_Impagos_Lista') IS NOT NULL DROP TABLE #Consulta_Ciclo_Impagos_Lista;
SELECT DISTINCT Factura = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(s.value, CHAR(13), ''), CHAR(9), ''), '''', ''), ',', ''), '"', '')))
INTO   #Consulta_Ciclo_Impagos_Lista
FROM   STRING_SPLIT(REPLACE(@Lista, CHAR(10), ';'), ';') s;
DELETE FROM #Consulta_Ciclo_Impagos_Lista WHERE Factura = '';

/* ---------- 2. Datos de cada cartera ---------- */
IF OBJECT_ID('tempdb..#Consulta_Ciclo_Impagos_Datos') IS NOT NULL DROP TABLE #Consulta_Ciclo_Impagos_Datos;
SELECT
    l.Factura,
    fv.Entorno,
    cc.IdCarteraCobro,
    co.CodigoContrato,
    Cliente           = dbo.formateardenominacion(cl.Nombre, cl.Apellido1, cl.Apellido2, cl.RazonSocial),
    cl.Identidad,
    FechaFactura      = CAST(fv.FechaFactura AS date),
    FechaVencimiento  = CAST(cc.FechaVencimiento AS date),
    cc.ImporteTotal,
    cc.ImporteCobro,
    FechaCobro        = CAST(cc.FechaCobro AS date),
    MedioCobro        = tc.TextoTipoCobro,
    EsRemesa          = tc.Remesa,
    cc.CodigoSituacionCobro,
    FechaImpagado     = CAST(cc.FechaImpagado AS date),
    cc.NumDevoluciones,
    AvisoPrevisto     = CAST(cc.AvisoPrevisto AS date),
    AvisoReal         = CAST(cc.AvisoReal AS date),
    FechaEnvioAviso   = CAST(cc.FechaEnvioImpago AS date),
    PdfAviso          = da.FileName,
    FalloEnvioAviso   = NULLIF(LTRIM(RTRIM(cc.Falloenvioavisoimpago)), ''),
    AvisoCortePrevisto= CAST(cc.AvisoCortePrevisto AS date),
    AvisoCorteReal    = CAST(cc.AvisoCorteReal AS date),
    FechaEnvioCorte   = CAST(cc.FechaEnvioCorte AS date),
    PdfCorte          = dc.FileName,
    FalloEnvioCorte   = NULLIF(LTRIM(RTRIM(cc.Falloenviocartacorte)), ''),
    CortePrevisto     = CAST(cc.CortePrevisto AS date),
    NoCortable        = ISNULL(co.NoCortable, 0),
    Calendario        = cal.TextoCalendario,
    FechaCartera      = CAST(cc.Fecha AS date),
    TieneClientePago  = CASE WHEN cc.IdClientePago IS NULL THEN 0 ELSE 1 END,
    UltimaDevolucion  = dev.UltimaDevolucion,
    DevolucionesFichero = ISNULL(dev.NumDev, 0),
    ContratoOmitidoDevoluciones = CASE WHEN om.CodigoContrato IS NULL THEN 0 ELSE 1 END,
    UltimoPagoParcialOI = pp.FechaImportacion,
    ImporteAbiertoOI    = pp.ImporteAbiertoOpenItem
INTO   #Consulta_Ciclo_Impagos_Datos
FROM   #Consulta_Ciclo_Impagos_Lista l
LEFT JOIN dbo.FacturaVentaCabecera fv ON fv.SerieNumFactura = l.Factura
LEFT JOIN dbo.CarteraCobro cc         ON cc.IdFacturaVentaCabecera = fv.IdFacturaVentaCabecera
LEFT JOIN dbo.Contrato co             ON co.IdContrato = ISNULL(cc.IdContrato, fv.IdContrato)
LEFT JOIN dbo.Cliente cl              ON cl.IdCliente = fv.IdCliente
LEFT JOIN dbo.TipoCobro tc            ON tc.IdTipoCobro = cc.IdTipoCobro
LEFT JOIN dbo.CarteraCobroCalendario cal ON cal.IdCarteraCobroCalendario = co.IdCarteraCobroCalendario
LEFT JOIN dbo.ContratoDocumento da    ON da.IdContratoDocumento = cc.IdContratoDocumentoAviso
LEFT JOIN dbo.ContratoDocumento dc    ON dc.IdContratoDocumento = cc.IdContratoDocumentoCorte
OUTER APPLY (SELECT UltimaDevolucion = CAST(MAX(d.FechaDevolucion) AS date), NumDev = COUNT(*)
             FROM dbo.CarteraCobroDevolucion d
             WHERE d.IdFacturaVentaCabecera = fv.IdFacturaVentaCabecera) dev
OUTER APPLY (SELECT TOP 1 o.CodigoContrato
             FROM dbo.CarteraCobroDevolucionOmision o
             WHERE o.CodigoContrato = co.CodigoContrato) om
OUTER APPLY (SELECT TOP 1 FechaImportacion = CAST(p.FechaImportacion AS date), p.ImporteAbiertoOpenItem
             FROM dbo.PagoParcial p
             WHERE p.IdCarteraCobro = cc.IdCarteraCobro
             ORDER BY p.FechaImportacion DESC, p.IdPagoParcial DESC) pp;

/* ---------- 3. Resultado con el diagnostico ---------- */
SELECT
    d.Entorno, d.Factura, d.CodigoContrato, d.Cliente, d.Identidad,
    Situacion = CASE d.CodigoSituacionCobro
                  WHEN 1 THEN 'PENDIENTE' WHEN 2 THEN 'COBRADO' WHEN 3 THEN 'IMPAGADO' WHEN 4 THEN 'REMESADO'
                  WHEN 5 THEN 'CARTA_AVISO' WHEN 6 THEN 'CARTA_CORTE' WHEN 7 THEN 'CORTADO' WHEN 8 THEN 'COBRADO_NORMA_57'
                  WHEN 9 THEN 'CANCELADO_POR_ABONO' WHEN 10 THEN 'CARTA_CORTE1' WHEN 11 THEN 'CARTA_DEUDA'
                  WHEN 12 THEN 'CARTA_NO_RENOVACION' WHEN 13 THEN 'AVISO_JURIDICO' WHEN 16 THEN 'RESCISION_ANTICIPADA'
                  ELSE CASE WHEN d.IdCarteraCobro IS NULL THEN 'SIN CARTERA' ELSE 'Desconocida' END END,

    /* Donde esta en el ciclo */
    PuntoDelCiclo = CASE
        WHEN d.Entorno IS NULL THEN 'Factura no encontrada'
        WHEN d.IdCarteraCobro IS NULL THEN 'La factura no tiene cartera de cobro'
        WHEN d.CodigoSituacionCobro IN (2, 4, 8) AND d.FechaImpagado IS NULL THEN 'Pagada: nunca ha estado impagada'
        WHEN d.CodigoSituacionCobro IN (2, 4, 8) THEN 'Pagada despues de un impago (fin del ciclo)'
        WHEN d.CodigoSituacionCobro = 1 THEN 'Pendiente: en el OPEN ITEMS y aun no vencida'
        WHEN d.CodigoSituacionCobro = 3 THEN 'Impagada: falta la carta de aviso'
        WHEN d.CodigoSituacionCobro = 5 AND d.FalloEnvioAviso IS NOT NULL THEN 'Carta de aviso: el envio fallo'
        WHEN d.CodigoSituacionCobro = 5 AND d.PdfAviso IS NULL THEN 'Carta de aviso: sin PDF creado'
        WHEN d.CodigoSituacionCobro = 5 AND d.FechaEnvioAviso IS NULL THEN 'Carta de aviso: PDF creado, sin enviar'
        WHEN d.CodigoSituacionCobro = 5 AND d.AvisoCortePrevisto > @Hoy THEN 'Carta de aviso enviada: el corte toca el ' + CONVERT(varchar(10), d.AvisoCortePrevisto, 103)
        WHEN d.CodigoSituacionCobro = 5 THEN 'Carta de aviso enviada: ya toca la carta de corte'
        WHEN d.CodigoSituacionCobro = 6 AND d.FalloEnvioCorte IS NOT NULL THEN 'Carta de corte: el envio fallo'
        WHEN d.CodigoSituacionCobro = 6 AND d.FechaEnvioCorte IS NULL THEN 'Carta de corte: sin enviar'
        WHEN d.CodigoSituacionCobro = 6 THEN 'Carta de corte enviada'
        ELSE 'Otra situacion: revisar a mano' END,

    /* Crear Comunicado -> CARTA AVISO */
    SaleCartaAviso = CASE
        WHEN d.IdCarteraCobro IS NULL THEN 'No: sin cartera'
        WHEN d.CodigoSituacionCobro <> 3 THEN 'No: no esta en IMPAGADO'
        WHEN d.ImporteTotal < 0 THEN 'No: importe negativo'
        ELSE 'Si' END,

    /* Crear Comunicado -> CARTA CORTE (sin bono social) */
    SaleCartaCorte = CASE
        WHEN d.IdCarteraCobro IS NULL THEN 'No: sin cartera'
        WHEN d.CodigoSituacionCobro <> 5 THEN 'No: no esta en CARTA_AVISO'
        WHEN d.AvisoCorteReal IS NOT NULL THEN 'No: ya tiene carta de corte (Aviso Corte, Fecha Real)'
        WHEN d.NoCortable = 1 THEN 'No: contrato marcado como No cortable'
        WHEN d.ImporteTotal < 0 THEN 'No: importe negativo'
        WHEN d.TieneClientePago = 0 THEN 'Revisar: sin cliente pagador'
        WHEN d.AvisoCortePrevisto > @Hoy THEN 'Si, pero solo con Filtrar grid por facturas (el corte previsto es el ' + CONVERT(varchar(10), d.AvisoCortePrevisto, 103) + ')'
        ELSE 'Si' END,

    /* Aviso para quien lo fuerce a mano */
    AvisoDatos = CASE
        WHEN d.CodigoSituacionCobro IN (3, 5, 6) AND ISNULL(d.ImporteCobro, 0) <> 0 THEN 'Importe Cobro distinto de 0: la carta saldria sin importe pendiente'
        WHEN d.CodigoSituacionCobro IN (3, 5, 6) AND d.FechaCobro IS NOT NULL THEN 'Tiene Fecha Cobro estando impagada'
        WHEN d.CodigoSituacionCobro IN (3, 5, 6) AND d.FechaImpagado IS NULL THEN 'Sin fecha de impago'
        WHEN d.CodigoSituacionCobro = 5 AND d.AvisoReal IS NULL THEN 'En CARTA_AVISO sin Aviso Impago, Fecha Real: si vuelve en el OPEN ITEMS pasara a IMPAGADO'
        ELSE '' END,

    /* Proximo OPEN ITEMS */
    SiVieneEnOpenItems = CASE
        WHEN d.IdCarteraCobro IS NULL THEN ''
        WHEN d.FechaVencimiento > @Hoy THEN 'PENDIENTE (aun no vencida; IMPAGADO si viene con codigo de devolucion)'
        WHEN d.ImporteTotal < 0 THEN 'PENDIENTE (importe negativo)'
        WHEN d.AvisoCorteReal IS NOT NULL AND d.FechaImpagado IS NOT NULL THEN 'CARTA_CORTE'
        WHEN d.AvisoReal IS NOT NULL AND d.FechaImpagado IS NOT NULL THEN 'CARTA_AVISO'
        ELSE 'IMPAGADO' END,
    SiNoVieneEnOpenItems = CASE
        WHEN d.IdCarteraCobro IS NULL THEN ''
        WHEN d.CodigoSituacionCobro NOT IN (1, 3, 5, 6, 9) THEN 'Sin cambios'
        WHEN d.FechaCartera > DATEADD(DAY, -3, @Hoy) THEN 'Sin cambios (factura de menos de 3 dias)'
        WHEN d.EsRemesa = 1 THEN 'REMESADO (se da por pagada)'
        WHEN d.EsRemesa = 0 THEN 'COBRADO (se da por pagada)'
        ELSE 'Sin cambios (sin medio de cobro)' END,

    /* Datos */
    d.FechaFactura, d.FechaVencimiento, d.ImporteTotal, d.ImporteCobro, d.FechaCobro, d.MedioCobro,
    d.FechaImpagado, d.NumDevoluciones, d.DevolucionesFichero, d.UltimaDevolucion, d.ContratoOmitidoDevoluciones,
    d.AvisoPrevisto, d.AvisoReal, d.FechaEnvioAviso, d.PdfAviso, d.FalloEnvioAviso,
    d.AvisoCortePrevisto, d.AvisoCorteReal, d.FechaEnvioCorte, d.PdfCorte, d.FalloEnvioCorte,
    d.CortePrevisto, d.NoCortable, d.Calendario,
    d.UltimoPagoParcialOI, d.ImporteAbiertoOI,
    d.IdCarteraCobro
FROM #Consulta_Ciclo_Impagos_Datos d
ORDER BY CASE WHEN d.Entorno IS NULL THEN 0 ELSE 1 END, d.CodigoSituacionCobro, d.Factura;

/* Resumen por situacion.
   Cambio sobre el original: ademas del codigo va el nombre. En el .sql salia «3», «5»,
   y quien lee el Excel no tiene por que saberse la tabla de memoria. */
SELECT Codigo    = ISNULL(CAST(CodigoSituacionCobro AS varchar(5)), '-'),
       Situacion = CASE CodigoSituacionCobro
                     WHEN 1 THEN 'PENDIENTE' WHEN 2 THEN 'COBRADO' WHEN 3 THEN 'IMPAGADO' WHEN 4 THEN 'REMESADO'
                     WHEN 5 THEN 'CARTA_AVISO' WHEN 6 THEN 'CARTA_CORTE' WHEN 7 THEN 'CORTADO' WHEN 8 THEN 'COBRADO_NORMA_57'
                     WHEN 9 THEN 'CANCELADO_POR_ABONO' WHEN 10 THEN 'CARTA_CORTE1' WHEN 11 THEN 'CARTA_DEUDA'
                     WHEN 12 THEN 'CARTA_NO_RENOVACION' WHEN 13 THEN 'AVISO_JURIDICO' WHEN 16 THEN 'RESCISION_ANTICIPADA'
                     ELSE 'SIN CARTERA O NO ENCONTRADA' END,
       Carteras  = COUNT(*)
FROM   #Consulta_Ciclo_Impagos_Datos
GROUP BY CodigoSituacionCobro
ORDER BY CodigoSituacionCobro;

DROP TABLE #Consulta_Ciclo_Impagos_Lista;
DROP TABLE #Consulta_Ciclo_Impagos_Datos;
