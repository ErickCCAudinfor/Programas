/* ============================================================================
   DESGLOSADO JC CASTILLA LA MANCHA - Tarifa Grupo "PF AM JC CLM..."
   ABRIL 2025 a SEPTIEMBRE 2026, desglosado por mes
   ----------------------------------------------------------------------------
   Base: Consulta_JC CASTILLA LA MANCHA.sql (se mantiene intacta; solo se
   ANADEN columnas y los bloques que las calculan).

   Lo que pide negocio y de donde sale cada cosa:
     1. Potencia por periodo .......... FacturaVentaLinea 10000-10999
                                        PotenciaContratadaXML / PotenciaAFacturarXML
     2. Consumo por periodo ........... FacturaVentaLinea 30000-30999
                                        TotConsumoEnergiaXML
     3. Maximetro por periodo ......... LecturaLinea.Maximetro / MaximetroExceso
                                        periodo via TarifaPeajePeriodoLectura > PeriodoHorario
     4. Reactiva ...................... FacturaVentaLinea 40000-40999
                                        TotConsumoReactivaXML + lecturas de LecturaLinea
     5. Termino Excesos Distribuidora . FacturaVentaLinea FacturaConcepto = 20006
                                        (20004 = exceso de potencia de comercializadora)

   El periodo de cada linea sale siempre de
   InfoLineaXML > FacturaConceptosDTO/Periodo/CodigoPeriodo,
   que es la columna calculada persistida CodigoPeriodoXML.

   SOLO LECTURA. No modifica nada.

   EN SIGEGESTOR: las fechas y el patron de tarifa grupo los pone la pantalla
   (Consultas > JC Castilla-La Mancha). Las fechas van en AAAAMMDD: el servidor
   esta en español y '2026-10-15' falla o se lee del reves. Los bloques /* C1 */,
   /* C2 */ y /* C3 */ del final son controles para lanzar sueltos en SSMS; estan
   comentados y no se ejecutan desde la aplicacion.
   ============================================================================ */

SET NOCOUNT ON;

DECLARE @Entorno            NVARCHAR(10)  = 'E1',
        @FechaDesde         DATETIME      = 'DesdeFechaReplace',
        @FechaHasta         DATETIME      = 'hastaFechaReplace',   -- incluido
        @PatronTarifaGrupo  NVARCHAR(100) = 'patronTarifaGrupoReplace';
/* El script original filtraba 'PF AM JC CLM 2026 LOTE%'. El jira pide
   "que contengan en el nombre PF AM JC CLM...", asi que se abre el patron.
   Antes de lanzar la consulta grande conviene ver que grupos entran:

       SELECT IdTarifaGrupo, TextoTarifaGrupo
       FROM   TarifaGrupo WITH (NOLOCK)
       WHERE  TextoTarifaGrupo LIKE 'PF AM JC CLM%'
       ORDER  BY TextoTarifaGrupo;
*/

DECLARE @DateTimeMaxValue AS DATETIME = CONVERT(DATETIME, '3000-12-31T00:00:00.000', 126);
DECLARE @DateTimeMinValue AS DATETIME = CONVERT(DATETIME, '1800-01-01T00:00:00.000', 126);

;WITH XMLNAMESPACES ('http://localhost/elegibilidad' AS ns),

/* ============================================================================
   BLOQUE NUEVO A - Facturas del ambito.
   Se calcula una sola vez y acota todos los bloques nuevos, para no volver a
   recorrer FacturaVentaLinea entera en cada uno.
   ============================================================================ */
JC_Facturas AS
(
    SELECT  fvc.IdFacturaVentaCabecera
    FROM    FacturaVentaCabecera AS fvc WITH (NOLOCK)
    WHERE   fvc.Entorno          = @Entorno
      AND   fvc.SerieFactura     IS NOT NULL
      AND   fvc.FacturaCategoria = 'EN'
      AND   fvc.FechaFactura    >= @FechaDesde
      AND   fvc.FechaFactura    <  DATEADD(DAY, 1, @FechaHasta)
      AND   fvc.IdTarifaGrupoXML IN (SELECT tg.IdTarifaGrupo
                                     FROM   TarifaGrupo AS tg WITH (NOLOCK)
                                     WHERE  tg.TextoTarifaGrupo LIKE @PatronTarifaGrupo)
)
, JC_Lineas AS
(
    SELECT  l.IdFacturaVentaCabecera,
            l.FacturaConcepto,
            l.ImporteBase,
            l.CodigoPeriodoXML,
            l.TotConsumoEnergiaXML,
            l.PrecioMedioEnergiaXML,
            l.PotenciaContratadaXML,
            l.PotenciaAFacturarXML,
            l.PrecioMedioPotenciaXML,
            l.TotConsumoReactivaXML,
            l.ConsumoReactivaXML,
            l.PrecioMedioReactivaXML,
            l.InfoLineaXML
    FROM    FacturaVentaLinea AS l WITH (NOLOCK)
    INNER JOIN JC_Facturas AS f ON f.IdFacturaVentaCabecera = l.IdFacturaVentaCabecera
    WHERE   l.Entorno = @Entorno
)

/* ============================================================================
   BLOQUE NUEVO B - POTENCIA POR PERIODO (conceptos 10000-10999)
   PotenciaContratada y PotenciaAFacturar NO son aditivas: son el valor del
   periodo. Si hay varias lineas del mismo periodo (peaje + cargo) se toma MAX,
   no SUM, que es lo que haria duplicar el dato. El importe si se suma.
   ============================================================================ */
, JC_Potencia AS
(
    SELECT  IdFacturaVentaCabecera,
            MAX(CASE WHEN CodigoPeriodoXML = 1 THEN PotenciaContratadaXML END) AS PotContratadaP1,
            MAX(CASE WHEN CodigoPeriodoXML = 2 THEN PotenciaContratadaXML END) AS PotContratadaP2,
            MAX(CASE WHEN CodigoPeriodoXML = 3 THEN PotenciaContratadaXML END) AS PotContratadaP3,
            MAX(CASE WHEN CodigoPeriodoXML = 4 THEN PotenciaContratadaXML END) AS PotContratadaP4,
            MAX(CASE WHEN CodigoPeriodoXML = 5 THEN PotenciaContratadaXML END) AS PotContratadaP5,
            MAX(CASE WHEN CodigoPeriodoXML = 6 THEN PotenciaContratadaXML END) AS PotContratadaP6,

            MAX(CASE WHEN CodigoPeriodoXML = 1 THEN PotenciaAFacturarXML END)  AS PotFacturadaP1,
            MAX(CASE WHEN CodigoPeriodoXML = 2 THEN PotenciaAFacturarXML END)  AS PotFacturadaP2,
            MAX(CASE WHEN CodigoPeriodoXML = 3 THEN PotenciaAFacturarXML END)  AS PotFacturadaP3,
            MAX(CASE WHEN CodigoPeriodoXML = 4 THEN PotenciaAFacturarXML END)  AS PotFacturadaP4,
            MAX(CASE WHEN CodigoPeriodoXML = 5 THEN PotenciaAFacturarXML END)  AS PotFacturadaP5,
            MAX(CASE WHEN CodigoPeriodoXML = 6 THEN PotenciaAFacturarXML END)  AS PotFacturadaP6,

            MAX(CASE WHEN CodigoPeriodoXML = 1 THEN PrecioMedioPotenciaXML END) AS PrecioPotenciaP1,
            MAX(CASE WHEN CodigoPeriodoXML = 2 THEN PrecioMedioPotenciaXML END) AS PrecioPotenciaP2,
            MAX(CASE WHEN CodigoPeriodoXML = 3 THEN PrecioMedioPotenciaXML END) AS PrecioPotenciaP3,
            MAX(CASE WHEN CodigoPeriodoXML = 4 THEN PrecioMedioPotenciaXML END) AS PrecioPotenciaP4,
            MAX(CASE WHEN CodigoPeriodoXML = 5 THEN PrecioMedioPotenciaXML END) AS PrecioPotenciaP5,
            MAX(CASE WHEN CodigoPeriodoXML = 6 THEN PrecioMedioPotenciaXML END) AS PrecioPotenciaP6,

            SUM(CASE WHEN CodigoPeriodoXML = 1 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImportePotenciaP1,
            SUM(CASE WHEN CodigoPeriodoXML = 2 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImportePotenciaP2,
            SUM(CASE WHEN CodigoPeriodoXML = 3 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImportePotenciaP3,
            SUM(CASE WHEN CodigoPeriodoXML = 4 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImportePotenciaP4,
            SUM(CASE WHEN CodigoPeriodoXML = 5 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImportePotenciaP5,
            SUM(CASE WHEN CodigoPeriodoXML = 6 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImportePotenciaP6
    FROM    JC_Lineas
    WHERE   FacturaConcepto BETWEEN 10000 AND 10999
      AND   CodigoPeriodoXML BETWEEN 1 AND 6
    GROUP BY IdFacturaVentaCabecera
)

/* ============================================================================
   BLOQUE NUEVO C - CONSUMO POR PERIODO (conceptos 30000-30999)
   Mismo criterio: el consumo del periodo se toma con MAX (varias lineas del
   mismo periodo repiten el mismo TotConsumo), el importe se suma.
   ============================================================================ */
, JC_Energia AS
(
    SELECT  IdFacturaVentaCabecera,
            MAX(CASE WHEN CodigoPeriodoXML = 1 THEN TotConsumoEnergiaXML END) AS ConsumoP1,
            MAX(CASE WHEN CodigoPeriodoXML = 2 THEN TotConsumoEnergiaXML END) AS ConsumoP2,
            MAX(CASE WHEN CodigoPeriodoXML = 3 THEN TotConsumoEnergiaXML END) AS ConsumoP3,
            MAX(CASE WHEN CodigoPeriodoXML = 4 THEN TotConsumoEnergiaXML END) AS ConsumoP4,
            MAX(CASE WHEN CodigoPeriodoXML = 5 THEN TotConsumoEnergiaXML END) AS ConsumoP5,
            MAX(CASE WHEN CodigoPeriodoXML = 6 THEN TotConsumoEnergiaXML END) AS ConsumoP6,

            MAX(CASE WHEN CodigoPeriodoXML = 1 THEN PrecioMedioEnergiaXML END) AS PrecioEnergiaP1,
            MAX(CASE WHEN CodigoPeriodoXML = 2 THEN PrecioMedioEnergiaXML END) AS PrecioEnergiaP2,
            MAX(CASE WHEN CodigoPeriodoXML = 3 THEN PrecioMedioEnergiaXML END) AS PrecioEnergiaP3,
            MAX(CASE WHEN CodigoPeriodoXML = 4 THEN PrecioMedioEnergiaXML END) AS PrecioEnergiaP4,
            MAX(CASE WHEN CodigoPeriodoXML = 5 THEN PrecioMedioEnergiaXML END) AS PrecioEnergiaP5,
            MAX(CASE WHEN CodigoPeriodoXML = 6 THEN PrecioMedioEnergiaXML END) AS PrecioEnergiaP6,

            SUM(CASE WHEN CodigoPeriodoXML = 1 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteEnergiaP1,
            SUM(CASE WHEN CodigoPeriodoXML = 2 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteEnergiaP2,
            SUM(CASE WHEN CodigoPeriodoXML = 3 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteEnergiaP3,
            SUM(CASE WHEN CodigoPeriodoXML = 4 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteEnergiaP4,
            SUM(CASE WHEN CodigoPeriodoXML = 5 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteEnergiaP5,
            SUM(CASE WHEN CodigoPeriodoXML = 6 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteEnergiaP6
    FROM    JC_Lineas
    WHERE   FacturaConcepto BETWEEN 30000 AND 30999
      AND   CodigoPeriodoXML BETWEEN 1 AND 6
    GROUP BY IdFacturaVentaCabecera
)

/* ============================================================================
   BLOQUE NUEVO D - REACTIVA FACTURADA POR PERIODO (conceptos 40000-40999)
   ============================================================================ */
, JC_Reactiva AS
(
    SELECT  IdFacturaVentaCabecera,
            MAX(CASE WHEN CodigoPeriodoXML = 1 THEN TotConsumoReactivaXML END) AS ConsumoReactivaP1,
            MAX(CASE WHEN CodigoPeriodoXML = 2 THEN TotConsumoReactivaXML END) AS ConsumoReactivaP2,
            MAX(CASE WHEN CodigoPeriodoXML = 3 THEN TotConsumoReactivaXML END) AS ConsumoReactivaP3,
            MAX(CASE WHEN CodigoPeriodoXML = 4 THEN TotConsumoReactivaXML END) AS ConsumoReactivaP4,
            MAX(CASE WHEN CodigoPeriodoXML = 5 THEN TotConsumoReactivaXML END) AS ConsumoReactivaP5,
            MAX(CASE WHEN CodigoPeriodoXML = 6 THEN TotConsumoReactivaXML END) AS ConsumoReactivaP6,

            MAX(CASE WHEN CodigoPeriodoXML = 1 THEN PrecioMedioReactivaXML END) AS PrecioReactivaP1,
            MAX(CASE WHEN CodigoPeriodoXML = 2 THEN PrecioMedioReactivaXML END) AS PrecioReactivaP2,
            MAX(CASE WHEN CodigoPeriodoXML = 3 THEN PrecioMedioReactivaXML END) AS PrecioReactivaP3,
            MAX(CASE WHEN CodigoPeriodoXML = 4 THEN PrecioMedioReactivaXML END) AS PrecioReactivaP4,
            MAX(CASE WHEN CodigoPeriodoXML = 5 THEN PrecioMedioReactivaXML END) AS PrecioReactivaP5,
            MAX(CASE WHEN CodigoPeriodoXML = 6 THEN PrecioMedioReactivaXML END) AS PrecioReactivaP6,

            SUM(CASE WHEN CodigoPeriodoXML = 1 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteReactivaP1,
            SUM(CASE WHEN CodigoPeriodoXML = 2 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteReactivaP2,
            SUM(CASE WHEN CodigoPeriodoXML = 3 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteReactivaP3,
            SUM(CASE WHEN CodigoPeriodoXML = 4 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteReactivaP4,
            SUM(CASE WHEN CodigoPeriodoXML = 5 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteReactivaP5,
            SUM(CASE WHEN CodigoPeriodoXML = 6 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteReactivaP6,
            SUM(ISNULL(ImporteBase, 0))                                              AS ImporteReactivaTotal
    FROM    JC_Lineas
    WHERE   FacturaConcepto BETWEEN 40000 AND 40999
    GROUP BY IdFacturaVentaCabecera
)

/* ============================================================================
   BLOQUE NUEVO E - EXCESOS (conceptos 20000-20999)
     20004 = exceso de potencia que factura la comercializadora
     20006 = TERMINO DE EXCESOS QUE REPERCUTE LA DISTRIBUIDORA  <-- el que piden
   La potencia en exceso de cada periodo esta en
   InfoLineaXML > FacturaConceptosDTO/ConceptoExceso/PotenciaAFacturar
   ============================================================================ */
, JC_Excesos AS
(
    SELECT  IdFacturaVentaCabecera,
            SUM(CASE WHEN FacturaConcepto = 20006 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteExcesoDistribuidora,
            SUM(CASE WHEN FacturaConcepto = 20004 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteExcesoPotencia,
            SUM(ISNULL(ImporteBase, 0))                                                   AS ImporteExcesosTotal,

            MAX(CASE WHEN CodigoPeriodoXML = 1 THEN PotenciaExceso END) AS PotenciaExcesoP1,
            MAX(CASE WHEN CodigoPeriodoXML = 2 THEN PotenciaExceso END) AS PotenciaExcesoP2,
            MAX(CASE WHEN CodigoPeriodoXML = 3 THEN PotenciaExceso END) AS PotenciaExcesoP3,
            MAX(CASE WHEN CodigoPeriodoXML = 4 THEN PotenciaExceso END) AS PotenciaExcesoP4,
            MAX(CASE WHEN CodigoPeriodoXML = 5 THEN PotenciaExceso END) AS PotenciaExcesoP5,
            MAX(CASE WHEN CodigoPeriodoXML = 6 THEN PotenciaExceso END) AS PotenciaExcesoP6,

            SUM(CASE WHEN CodigoPeriodoXML = 1 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteExcesoP1,
            SUM(CASE WHEN CodigoPeriodoXML = 2 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteExcesoP2,
            SUM(CASE WHEN CodigoPeriodoXML = 3 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteExcesoP3,
            SUM(CASE WHEN CodigoPeriodoXML = 4 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteExcesoP4,
            SUM(CASE WHEN CodigoPeriodoXML = 5 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteExcesoP5,
            SUM(CASE WHEN CodigoPeriodoXML = 6 THEN ISNULL(ImporteBase, 0) ELSE 0 END) AS ImporteExcesoP6
    FROM    (
                SELECT  IdFacturaVentaCabecera,
                        FacturaConcepto,
                        ImporteBase,
                        CodigoPeriodoXML,
                        InfoLineaXML.value('(FacturaConceptosDTO/ConceptoExceso/PotenciaAFacturar)[1]', 'decimal(18,3)') AS PotenciaExceso
                FROM    JC_Lineas
                WHERE   FacturaConcepto BETWEEN 20000 AND 20999
            ) AS e
    GROUP BY IdFacturaVentaCabecera
)

/* ============================================================================
   BLOQUE NUEVO F - MAXIMETRO Y LECTURAS DE REACTIVA, POR PERIODO
   El periodo de cada LecturaLinea se resuelve por la via del modelo
   (TarifaPeajePeriodoLectura > PeriodoHorario.CodigoPeriodo) en lugar de
   listar a mano los IdTarifaPeajePeriodoLectura: asi no hay que mantener la
   lista cuando entren tarifas nuevas.
   ============================================================================ */
, JC_LecturasAmbito AS
(
    SELECT DISTINCT lec.IdLectura
    FROM   Lectura AS lec WITH (NOLOCK)
    INNER JOIN FacturaVentaCabecera AS fvc WITH (NOLOCK)
            ON lec.IdFacturaVentaCabeceraSectorC = CASE WHEN fvc.IsAbono = 1
                                                        THEN fvc.IdFacturaRectificativa
                                                        ELSE fvc.IdFacturaVentaCabecera
                                                   END
    INNER JOIN JC_Facturas AS f ON f.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
)
, JC_Maximetro AS
(
    SELECT  ll.IdLectura,
            MAX(CASE WHEN ph.CodigoPeriodo = 1 THEN ll.Maximetro END) AS MaximetroP1,
            MAX(CASE WHEN ph.CodigoPeriodo = 2 THEN ll.Maximetro END) AS MaximetroP2,
            MAX(CASE WHEN ph.CodigoPeriodo = 3 THEN ll.Maximetro END) AS MaximetroP3,
            MAX(CASE WHEN ph.CodigoPeriodo = 4 THEN ll.Maximetro END) AS MaximetroP4,
            MAX(CASE WHEN ph.CodigoPeriodo = 5 THEN ll.Maximetro END) AS MaximetroP5,
            MAX(CASE WHEN ph.CodigoPeriodo = 6 THEN ll.Maximetro END) AS MaximetroP6,

            MAX(CASE WHEN ph.CodigoPeriodo = 1 THEN ll.MaximetroExceso END) AS MaximetroExcesoP1,
            MAX(CASE WHEN ph.CodigoPeriodo = 2 THEN ll.MaximetroExceso END) AS MaximetroExcesoP2,
            MAX(CASE WHEN ph.CodigoPeriodo = 3 THEN ll.MaximetroExceso END) AS MaximetroExcesoP3,
            MAX(CASE WHEN ph.CodigoPeriodo = 4 THEN ll.MaximetroExceso END) AS MaximetroExcesoP4,
            MAX(CASE WHEN ph.CodigoPeriodo = 5 THEN ll.MaximetroExceso END) AS MaximetroExcesoP5,
            MAX(CASE WHEN ph.CodigoPeriodo = 6 THEN ll.MaximetroExceso END) AS MaximetroExcesoP6,

            MAX(CASE WHEN ph.CodigoPeriodo = 1 THEN ll.ReactivaActual END) AS LectReactivaActualP1,
            MAX(CASE WHEN ph.CodigoPeriodo = 2 THEN ll.ReactivaActual END) AS LectReactivaActualP2,
            MAX(CASE WHEN ph.CodigoPeriodo = 3 THEN ll.ReactivaActual END) AS LectReactivaActualP3,
            MAX(CASE WHEN ph.CodigoPeriodo = 4 THEN ll.ReactivaActual END) AS LectReactivaActualP4,
            MAX(CASE WHEN ph.CodigoPeriodo = 5 THEN ll.ReactivaActual END) AS LectReactivaActualP5,
            MAX(CASE WHEN ph.CodigoPeriodo = 6 THEN ll.ReactivaActual END) AS LectReactivaActualP6,

            MAX(CASE WHEN ph.CodigoPeriodo = 1 THEN ll.ReactivaAnterior END) AS LectReactivaAnteriorP1,
            MAX(CASE WHEN ph.CodigoPeriodo = 2 THEN ll.ReactivaAnterior END) AS LectReactivaAnteriorP2,
            MAX(CASE WHEN ph.CodigoPeriodo = 3 THEN ll.ReactivaAnterior END) AS LectReactivaAnteriorP3,
            MAX(CASE WHEN ph.CodigoPeriodo = 4 THEN ll.ReactivaAnterior END) AS LectReactivaAnteriorP4,
            MAX(CASE WHEN ph.CodigoPeriodo = 5 THEN ll.ReactivaAnterior END) AS LectReactivaAnteriorP5,
            MAX(CASE WHEN ph.CodigoPeriodo = 6 THEN ll.ReactivaAnterior END) AS LectReactivaAnteriorP6,

            MAX(CASE WHEN ph.CodigoPeriodo = 1 THEN ll.ConsumoReactiva END) AS LectConsumoReactivaP1,
            MAX(CASE WHEN ph.CodigoPeriodo = 2 THEN ll.ConsumoReactiva END) AS LectConsumoReactivaP2,
            MAX(CASE WHEN ph.CodigoPeriodo = 3 THEN ll.ConsumoReactiva END) AS LectConsumoReactivaP3,
            MAX(CASE WHEN ph.CodigoPeriodo = 4 THEN ll.ConsumoReactiva END) AS LectConsumoReactivaP4,
            MAX(CASE WHEN ph.CodigoPeriodo = 5 THEN ll.ConsumoReactiva END) AS LectConsumoReactivaP5,
            MAX(CASE WHEN ph.CodigoPeriodo = 6 THEN ll.ConsumoReactiva END) AS LectConsumoReactivaP6
    FROM    LecturaLinea AS ll WITH (NOLOCK)
    INNER JOIN JC_LecturasAmbito AS jl ON jl.IdLectura = ll.IdLectura
    LEFT  JOIN TarifaPeajePeriodoLectura AS tppl WITH (NOLOCK)
            ON tppl.IdTarifaPeajePeriodoLectura = ll.IdTarifaPeajePeriodoLectura
    LEFT  JOIN PeriodoHorario AS ph WITH (NOLOCK)
            ON ph.IdPeriodoHorario = tppl.IdPeriodoHorario
    GROUP BY ll.IdLectura
)

/* ============================================================================
   A PARTIR DE AQUI, LA CONSULTA ORIGINAL SIN CAMBIOS
   ============================================================================ */
, FVL_SUM_TotConsumoEnergiaXML_EnergiaAcceso AS
(
    SELECT IdFacturaVentaCabecera,
    SUM(CASE
            WHEN TotConsumoEnergiaXML < 2147483647 AND TotConsumoEnergiaXML > -2147483647 THEN TotConsumoEnergiaXML
            ELSE 0
        END) AS ConsumoTotal
    FROM FacturaVentaLinea WITH(NOLOCK)
    WHERE  FacturaConcepto = 30001 AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, FVL_SUM_TotConsumoEnergiaXML_Otros AS
(
    SELECT IdFacturaVentaCabecera
    , SUM(TotConsumoEnergiaXML) AS ConsumoTotal
    FROM FacturaVentaLinea WITH(NOLOCK)
    WHERE FacturaConcepto BETWEEN 30002 AND 30999 AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, SerieNumeroFacturasOrigen AS
(
    SELECT CONCAT(origen.SerieFactura, ' ', origen.NumeroFactura) AS SerieNumeroFacturaOrigen, principal.IdFacturaVentaCabecera
    FROM FacturaVentaCabecera principal WITH(NOLOCK)
    INNER JOIN FacturaVentaCabecera origen WITH(NOLOCK)
    ON principal.IdFacturaOrigen = origen.IdFacturaVentaCabecera
)
, FVL_SUM_ImporteBase_IH AS
(
    SELECT IdFacturaVentaCabecera, SUM(ImporteBase) AS Importe
    FROM  FacturaVentaLinea WITH(NOLOCK)
    WHERE  FacturaConcepto IN (90004, 90017, 90025) AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, FVT_SUM_Importes AS
(
    SELECT IdFacturaVentaCabecera
    , SUM(ImporteTotal) AS ImporteTotal
    , SUM(ImporteBase) AS ImporteBase
    , SUM(ImporteImpuesto) AS ImporteImpuesto
    , SUM(ISNULL(ImporteAdicional, 0)) AS ImporteAdicional
    FROM FacturaVentaTotal WITH(NOLOCK)
    GROUP BY IdFacturaVentaCabecera
)
, FVL_SUM_Potencia_ConsumoReactiva AS (
    SELECT IdFacturaVentaCabecera
    , CONVERT(DECIMAL(18,3), SUM(ISNULL(FacturaVentaLinea.PotenciaAFacturarXML, 0)))  AS PotenciaAFacturarTotal,
    CONVERT(DECIMAL(18,3), SUM(ISNULL(FacturaVentaLinea.ConsumoReactivaXML, 0))) AS ConsumoReactivaTotal
    FROM FacturaVentaLinea WITH(NOLOCK)
    GROUP BY IdFacturaVentaCabecera
)
, FVL_SUM_ImporteBaseAlquiler AS (
    SELECT IdFacturaVentaCabecera
    , SUM(ImporteBase) AS ImporteBaseAlquiler
    FROM FacturaVentaLinea  WITH(NOLOCK)
    WHERE FacturaConcepto IN (50001, 50002, 50003, 90013, 90005, 90014, 90015) AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, FVL_SUM_ImporteBaseProductos AS
(
    SELECT IdFacturaVentaCabecera
    , SUM(ImporteBase) AS ImporteBaseProductos
    FROM FacturaVentaLinea WITH(NOLOCK)
    WHERE FacturaConcepto IN (100001, 100002, 70020) AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, FVL_SUM_ImporteBaseImpuestoIE AS
(
    SELECT IdFacturaVentaCabecera
    , SUM(ImporteBase) AS ImporteBaseImpuestoIE
    FROM FacturaVentaLinea WITH(NOLOCK)
    WHERE FacturaConcepto IN (60001, 60002, 60010, 60011, 60012) AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, FVL_SUM_ImporteIEElectricidadE AS (
    SELECT IdFacturaVentaCabecera, SUM(ImporteBase) AS ImporteIEElectricidad
    FROM FacturaVentaLinea WITH(NOLOCK)
    WHERE FacturaConcepto IN (60001, 60002) AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, FVL_SUM_ImporteIEGas AS
(
    SELECT IdFacturaVentaCabecera, SUM(ImporteBase) AS ImporteIEGas
    FROM FacturaVentaLinea WITH(NOLOCK)
    WHERE FacturaConcepto IN (90004)  AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, DiferenciaPagosPorCapacidad_Importe AS
(
    SELECT IdFacturaVentaCabecera, SUM(ImporteBase) AS Importe
    FROM FacturaVentaLinea WITH(NOLOCK)
    WHERE  FacturaConcepto = 120009 AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
, AjusteCapGas_Importe AS
(
    SELECT IdFacturaVentaCabecera, SUM(ImporteBase) AS Importe
    FROM FacturaVentaLinea WITH(NOLOCK)
    WHERE FacturaConcepto IN (30004, 30006) AND  IsAjusteCAPGas = 1 AND Entorno = @Entorno
    GROUP BY IdFacturaVentaCabecera
)
--JET-578
, ServiciosAjusteSistema_Importe AS
(
    SELECT linea.IdFacturaVentaCabecera
    , SUM(DISTINCT linea.ImporteBase) AS ImporteProductos
    FROM FacturaVentaLinea linea WITH(NOLOCK)
    INNER JOIN FacturaVentaCabecera cabecera ON cabecera.IdFacturaVentaCabecera = linea.IdFacturaVentaCabecera
    WHERE linea.FacturaConcepto IN (100001) AND linea.Entorno = @Entorno
    AND linea.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoProductos/IdProducto)[1]', 'decimal(6)') IN
    (SELECT IdProducto FROM Producto WHERE IsServiciosAjusteSistema = 1)
    GROUP BY linea.IdFacturaVentaCabecera
)
, SumConsumosGas AS
(
    SELECT id AS IdFacturaVentaCabeceraGas,
           SUM(ConsumoTotal1) AS ConsumoTotal
    FROM (SELECT IdFacturaVentaCabecera AS id,
                 ConsumoT1 AS ConsumoTotal1
          FROM (SELECT FacturaVentaLinea.IdFacturaVentaCabecera,
                       ISNULL(FacturaVentaLinea.TotConsumoGasXML, 0) AS ConsumoT1,
                       FacturaVentaLinea.CodigoPeriodoXML AS CodPeriodo
                FROM FacturaVentaLinea WITH(NOLOCK)
                WHERE  FacturaConcepto IN (90031, 90001, 90002, 90038, 90048, 90062) AND Entorno = @Entorno) AS bumba
          GROUP BY bumba.CodPeriodo, IdFacturaVentaCabecera, bumba.ConsumoT1) AS FacturaVentaLineaTotalGas
    GROUP BY id
)
, SumConsumosGas2 AS
(
    SELECT id AS IdFacturaVentaCabeceraGas,
           SUM(ConsumoTotal1) AS ConsumoTotal
    FROM (SELECT IdFacturaVentaCabecera AS id,
                 ConsumoT1 AS ConsumoTotal1
          FROM (SELECT FacturaVentaLinea.IdFacturaVentaCabecera,
                       ISNULL(FacturaVentaLinea.TotConsumoGasXML, 0) AS ConsumoT1,
                       FacturaVentaLinea.CodigoPeriodoXML AS CodPeriodo
                FROM FacturaVentaLinea WITH(NOLOCK)
                WHERE  FacturaConcepto IN (90012) AND Entorno = @Entorno) AS bumba
          GROUP BY bumba.CodPeriodo, IdFacturaVentaCabecera, bumba.ConsumoT1) AS FacturaVentaLineaTotalGas
    GROUP BY id
)
, fvo(idfvo) AS  (
    SELECT IdFacturaOrigen FROM FacturaVentaCabecera WITH(NOLOCK)
    INNER JOIN Lectura WITH(NOLOCK) ON Lectura.IdFacturaVentaCabeceraSectorC = FacturaVentaCabecera.IdFacturaOrigen
    GROUP BY FacturaVentaCabecera.IdFacturaOrigen)


SELECT
             /* ---- DESGLOSE POR MES (nuevo) ---- */
             YEAR(FacturaVentaCabecera.FechaFactura)                  AS AnyoFactura,
             MONTH(FacturaVentaCabecera.FechaFactura)                 AS MesFactura,
             CONVERT(CHAR(7), FacturaVentaCabecera.FechaFactura, 126) AS AnyoMes,

             Contrato.IdContrato,
        CASE WHEN Contrato.IsFirmaDigitalEnviada = 1
        THEN 'SI'
        ELSE 'NO' END AS isFirmaDigitalEnviada,
        CASE WHEN Contrato.IsFirmadoDigitalmente = 1
        THEN 'SI'
        ELSE 'NO' END AS isFirmadoDigitalmente,
             Contrato.Entorno,
             Contrato.Version,
             Contrato.IdCliente,
             FacturaVentaCabecera.FechaFactura,
             FacturaVentaCabecera.SerieFactura,
             FacturaVentaCabecera.NumeroFactura,
             ca.TextoCanal TipoFactura,
             FacturaVentaCabecera.IdFacturaVentaCabecera,
             FacturaVentaCabecera.IdTipoCobro,
             TipoCobro.TextoTipoCobro,
             Cliente.Identidad AS IdentidadCliente,
        CASE
        WHEN Cliente.Nombre IS NULL OR Cliente.Nombre = ''
            THEN Cliente.RazonSocial
            ELSE Cliente.Nombre + ' ' + ISNULL(Cliente.Apellido1, '') + ' ' + ISNULL(Cliente.Apellido2, '')
        END AS DenominacionCliente,
             Cliente.Apellido1,
             Cliente.Apellido2,
             Cliente.Nombre,
             Cliente.RazonSocial,
             Contrato.CodigoContrato,
             CUPS.CodPostal AS CodigoPostal,
             CUPS.CodigoCUPS,
             CUPS.IdRuta,
             CiudadCUPS.TextoCiudad AS TextoCiudadCUPS,
       CONCAT(ISNULL(CallejeroCUPS.NombreCalle, '') , CASE LEN(ISNULL(CallejeroCUPS.NombreCalle, '')) WHEN 0 THEN '' ELSE ' ' END
        , CONVERT(NVARCHAR(20), CUPS.Numero), CASE ISNULL(CUPS.Numero, 0)  WHEN 0 THEN '' ELSE ' ' END
        , ISNULL(CallejeroTipoNumeroCUPS.TextoNumero,'') , CASE LEN(ISNULL(CallejeroTipoNumeroCUPS.TextoNumero,'')) WHEN 0 THEN '' ELSE  ' ' END
        , ISNULL(CallejeroTipoCalificadorCUPS.TextoCalificador,'') , CASE LEN(ISNULL(CallejeroTipoCalificadorCUPS.TextoCalificador,'')) WHEN  0 THEN '' ELSE ' ' END
        , ISNULL(CallejeroPortalCUPS.TextoPortal,''), CASE LEN(ISNULL(CallejeroPortalCUPS.TextoPortal,'')) WHEN 0 THEN '' ELSE ' ' END
        , ISNULL(CallejeroEscaleraCUPS.TextoEscalera,''), CASE LEN(ISNULL(CallejeroEscaleraCUPS.TextoEscalera,'')) WHEN  0 THEN '' ELSE ' ' END
        , ISNULL(CallejeroPisoCUPS.TextoPiso,''), CASE LEN(ISNULL(CallejeroPisoCUPS.TextoPiso,'')) WHEN 0 THEN '' ELSE ' ' END
        , ISNULL(CallejeroPuertaCUPS.TextoPuerta,''), CASE LEN(ISNULL(CallejeroPuertaCUPS.TextoPuerta,'')) WHEN 0 THEN '' ELSE ' ' END
        , ISNULL(CUPS.Aclarador,'')
        ) AS DireccionCUPS,
             CUPS.IdDistribuidora AS IdDistribuidoraCUPS,
             CUPS.IdCups,
             Distribuidora.NombreFiscal AS DistribuidoraCUPS,
             Comercializadora.NombreFiscal AS Comercializadora,
             TarifaPeaje.TextoTarifaPeaje,
             Tarifa.TextoTarifa,
             Tarifa.IdTarifa,
             TarifaGrupo.IdTarifaGrupo,
             TarifaGrupo.TextoTarifaGrupo,
             REPLACE(CONVERT(VARCHAR(30), CASE
                    WHEN ISNULL(FacturaTotales.ConsumoTotal, 0) <> 0  THEN FacturaTotales.ConsumoTotal
                    ELSE FacturaTotales2.ConsumoTotal
             END), '.', ',') AS ConsumoTotal,
             REPLACE(CONVERT(VARCHAR(30), FacturaVentaLineaTotal.PotenciaAFacturarTotal), '.', ',') AS PotenciaAFacturarTotal,
             REPLACE(CONVERT(VARCHAR(30), FacturaVentaLineaTotal.ConsumoReactivaTotal), '.', ',')   AS ConsumoReactivaTotal,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(ImportesTotales.ImporteTotal, 0)), '.', ',')    AS ImporteTotal,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(ImportesTotales.ImporteBase, 0)), '.', ',')     AS ImporteBase,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(ImportesTotales.ImporteImpuesto, 0)), '.', ',') AS ImporteImpuesto,
             CNAE.CodigoCNAE,
             CNAE.TextoCNAE,
             FacturaVentaCabecera.FechaLecturaActualXML AS FechaLecturaActual,
             FacturaVentaCabecera.FechaLecturaAnteriorXML AS FechaLecturaAnterior,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(FacturaVentaLineaAlq.ImporteBaseAlquiler, 0)), '.', ',')      AS ImporteBaseAlquiler,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(FacturaVentaLineaProd.ImporteBaseProductos, 0)), '.', ',')    AS ImporteBaseProductos,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(FacturaVentaLineaImp.ImporteBaseImpuestoIE, 0)), '.', ',')    AS ImporteBaseImpuestoIE,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(ImportesTotales.ImporteAdicional, 0)), '.', ',')              AS ImporteAdicional,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(ConceptoIEElectricidad.ImporteIEElectricidad, 0)), '.', ',')  AS ImporteIEElectricidad,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(ConceptoIEGas.ImporteIEGas, 0)), '.', ',')                    AS ImporteIEGas,
             REPLACE(CONVERT(VARCHAR(30), ISNULL(DPC.Importe, 0)), '.', ',')                                   AS DiferenciaPagosPorCapacidad,
             REPLACE(CONVERT(VARCHAR(30), CASE
                    WHEN ISNULL(ConsumosGas.ConsumoTotal, 0) <> 0  THEN ConsumosGas.ConsumoTotal
                    ELSE ConsumosGas2.ConsumoTotal
             END), '.', ',') AS ConsumoTotalGas,
             CiudadCUPS.TextoCiudad,
             ProvinciaCUPS.TextoProvincia,
        CarteraCobroCalendario.TextoCalendario AS Calendario,
        REPLACE(CONVERT(VARCHAR(30), ISNULL(IH.Importe, 0)), '.', ',') AS ImpHidrocarburos,
        FacturaVentaCabecera.AsuntosSociales,
        SerieNumeroFacturasOrigen.SerieNumeroFacturaOrigen,
        FacturaVentaCabecera.OrigenMedida,
        REPLACE(CONVERT(VARCHAR(30), ISNULL(AjusteGas.Importe, 0)), '.', ',') AS AjusteCapGAS,

CASE FacturaVentaCabecera.IsAbono
    WHEN 1 THEN
        (SELECT FacturaCompraCabecera.SerieFactura FROM FacturaCompraCabecera WITH(NOLOCK) LEFT JOIN Canal WITH(NOLOCK)
        ON FacturaCompraCabecera.IdCanal = Canal.IdCanal
        WHERE Canal.Serie LIKE '%ABO%' AND
        FacturaVentaCabecera.IsAbono = 1  AND
        FacturaCompraCabecera.IdFacturaOrigen IN(SELECT IdFacturaCompraCabecera FROM FacturaCompraCabecera WHERE IdFacturaCompraCabecera IN (SELECT
        IdFacturaCompraCabecera FROM Lectura WITH(NOLOCK) WHERE Lectura.CodigoContrato = FacturaVentaCabecera.CodigoContrato AND  IdFacturaVentaCabeceraSectorC = fvo.idfvo)))
    ELSE
         FacturaCompraCabecera.SerieFactura
END AS SerieFacturaATR,

CASE FacturaVentaCabecera.IsAbono
    WHEN 1 THEN
        (SELECT FacturaCompraCabecera.NumeroFactura FROM FacturaCompraCabecera WITH(NOLOCK)  LEFT JOIN Canal WITH(NOLOCK)
        ON FacturaCompraCabecera.IdCanal = Canal.IdCanal
        WHERE Canal.Serie LIKE '%ABO%' AND
        FacturaCompraCabecera.IdFacturaOrigen IN (SELECT IdFacturaCompraCabecera FROM FacturaCompraCabecera WITH(NOLOCK) WHERE IdFacturaCompraCabecera IN (SELECT
        IdFacturaCompraCabecera FROM Lectura WHERE Lectura.CodigoContrato = FacturaVentaCabecera.CodigoContrato AND IdFacturaVentaCabeceraSectorC = fvo.idfvo)))
    ELSE
         FacturaCompraCabecera.NumeroFactura
END AS NumFacturaATR,

CASE FacturaVentaCabecera.IsAbono
    WHEN 1 THEN
        (SELECT FacturaCompraCabecera.IdFacturaCompraCabecera FROM FacturaCompraCabecera WITH(NOLOCK)
        LEFT JOIN Canal WITH(NOLOCK)    ON FacturaCompraCabecera.IdCanal = Canal.IdCanal
        WHERE
        Canal.Serie LIKE '%ABO%' AND
        FacturaCompraCabecera.IdCanal = 9  AND
        FacturaCompraCabecera.IdFacturaOrigen IN (SELECT IdFacturaCompraCabecera FROM FacturaCompraCabecera WHERE IdFacturaCompraCabecera IN (SELECT
        IdFacturaCompraCabecera FROM Lectura WHERE Lectura.CodigoContrato = FacturaVentaCabecera.CodigoContrato AND
        IdFacturaVentaCabeceraSectorC = fvo.idfvo)))
    ELSE
         FacturaCompraCabecera.IdFacturaCompraCabecera
END AS IdFacturaATR,

        FacturaCompraCabecera.FechaRecepcion AS FechaRecepcion,
        CASE
            WHEN Contrato.IsTelemedido = 1
            THEN 'S'
            ELSE 'N'
        END AS Telemedida,
        --JET-356 Nueva configuracion IH -GAS
       REPLACE(CONVERT(VARCHAR(30), Contrato.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeReducido)[1]', 'decimal(5,2)')), '.', ',') AS PcIHRedudico,
       REPLACE(CONVERT(VARCHAR(30), Contrato.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeExento)[1]', 'decimal(5,2)')), '.', ',') AS PcIHExento,
       REPLACE(CONVERT(VARCHAR(30), Contrato.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeNormal)[1]', 'decimal(5,2)')), '.', ',') AS PcIHNormal,
       REPLACE(CONVERT(VARCHAR(30), Contrato.ContratoInfoXML.value('(ContratoInfoDTO/ImpuestoHidrocarburoPropioGas/PorcentajeVehicular)[1]', 'decimal(5,2)')), '.', ',') AS PcIHVehicular
       --JET-578
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(ServiciosAjusteSistema.ImporteProductos, 0)), '.', ',') AS ServiciosAjusteSistema
       , FacturaCompraCabecera.FacturaXML.value('(/ns:MensajeFacturacion/ns:Facturas/ns:FacturaATR/ns:DatosGeneralesFacturaATR/ns:DatosFacturaATR/ns:TipoAutoconsumo)[1]', 'VARCHAR(50)') AS TipoAutoconsumo
       , FacturaCompraCabecera.FacturaXML.value('(/ns:MensajeFacturacion/ns:Facturas/ns:FacturaATR/ns:DatosGeneralesFacturaATR/ns:DatosFacturaATR/ns:TipoSubseccion)[1]', 'VARCHAR(50)') AS TipoSubseccion
       , Contrato.Autoconsumo
       , Contrato.IsAutoconsumoNoCompensable

       /* =====================================================================
          COLUMNAS NUEVAS
          ===================================================================== */

       /* --- 1. POTENCIA POR PERIODO --- */
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotContratadaP1), '.', ',') AS PotContratadaP1
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotContratadaP2), '.', ',') AS PotContratadaP2
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotContratadaP3), '.', ',') AS PotContratadaP3
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotContratadaP4), '.', ',') AS PotContratadaP4
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotContratadaP5), '.', ',') AS PotContratadaP5
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotContratadaP6), '.', ',') AS PotContratadaP6
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotFacturadaP1), '.', ',')  AS PotFacturadaP1
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotFacturadaP2), '.', ',')  AS PotFacturadaP2
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotFacturadaP3), '.', ',')  AS PotFacturadaP3
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotFacturadaP4), '.', ',')  AS PotFacturadaP4
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotFacturadaP5), '.', ',')  AS PotFacturadaP5
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PotFacturadaP6), '.', ',')  AS PotFacturadaP6
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PrecioPotenciaP1), '.', ',') AS PrecioPotenciaP1
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PrecioPotenciaP2), '.', ',') AS PrecioPotenciaP2
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PrecioPotenciaP3), '.', ',') AS PrecioPotenciaP3
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PrecioPotenciaP4), '.', ',') AS PrecioPotenciaP4
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PrecioPotenciaP5), '.', ',') AS PrecioPotenciaP5
       , REPLACE(CONVERT(VARCHAR(30), JCPot.PrecioPotenciaP6), '.', ',') AS PrecioPotenciaP6
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCPot.ImportePotenciaP1, 0)), '.', ',') AS ImportePotenciaP1
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCPot.ImportePotenciaP2, 0)), '.', ',') AS ImportePotenciaP2
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCPot.ImportePotenciaP3, 0)), '.', ',') AS ImportePotenciaP3
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCPot.ImportePotenciaP4, 0)), '.', ',') AS ImportePotenciaP4
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCPot.ImportePotenciaP5, 0)), '.', ',') AS ImportePotenciaP5
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCPot.ImportePotenciaP6, 0)), '.', ',') AS ImportePotenciaP6

       /* --- 2. CONSUMO POR PERIODO (kWh) --- */
       , REPLACE(CONVERT(VARCHAR(30), JCEne.ConsumoP1), '.', ',') AS ConsumoP1
       , REPLACE(CONVERT(VARCHAR(30), JCEne.ConsumoP2), '.', ',') AS ConsumoP2
       , REPLACE(CONVERT(VARCHAR(30), JCEne.ConsumoP3), '.', ',') AS ConsumoP3
       , REPLACE(CONVERT(VARCHAR(30), JCEne.ConsumoP4), '.', ',') AS ConsumoP4
       , REPLACE(CONVERT(VARCHAR(30), JCEne.ConsumoP5), '.', ',') AS ConsumoP5
       , REPLACE(CONVERT(VARCHAR(30), JCEne.ConsumoP6), '.', ',') AS ConsumoP6
       , REPLACE(CONVERT(VARCHAR(30), JCEne.PrecioEnergiaP1), '.', ',') AS PrecioEnergiaP1
       , REPLACE(CONVERT(VARCHAR(30), JCEne.PrecioEnergiaP2), '.', ',') AS PrecioEnergiaP2
       , REPLACE(CONVERT(VARCHAR(30), JCEne.PrecioEnergiaP3), '.', ',') AS PrecioEnergiaP3
       , REPLACE(CONVERT(VARCHAR(30), JCEne.PrecioEnergiaP4), '.', ',') AS PrecioEnergiaP4
       , REPLACE(CONVERT(VARCHAR(30), JCEne.PrecioEnergiaP5), '.', ',') AS PrecioEnergiaP5
       , REPLACE(CONVERT(VARCHAR(30), JCEne.PrecioEnergiaP6), '.', ',') AS PrecioEnergiaP6
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCEne.ImporteEnergiaP1, 0)), '.', ',') AS ImporteEnergiaP1
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCEne.ImporteEnergiaP2, 0)), '.', ',') AS ImporteEnergiaP2
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCEne.ImporteEnergiaP3, 0)), '.', ',') AS ImporteEnergiaP3
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCEne.ImporteEnergiaP4, 0)), '.', ',') AS ImporteEnergiaP4
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCEne.ImporteEnergiaP5, 0)), '.', ',') AS ImporteEnergiaP5
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCEne.ImporteEnergiaP6, 0)), '.', ',') AS ImporteEnergiaP6

       /* --- 3. MAXIMETRO POR PERIODO --- */
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroP1), '.', ',') AS MaximetroP1
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroP2), '.', ',') AS MaximetroP2
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroP3), '.', ',') AS MaximetroP3
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroP4), '.', ',') AS MaximetroP4
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroP5), '.', ',') AS MaximetroP5
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroP6), '.', ',') AS MaximetroP6
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroExcesoP1), '.', ',') AS MaximetroExcesoP1
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroExcesoP2), '.', ',') AS MaximetroExcesoP2
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroExcesoP3), '.', ',') AS MaximetroExcesoP3
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroExcesoP4), '.', ',') AS MaximetroExcesoP4
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroExcesoP5), '.', ',') AS MaximetroExcesoP5
       , REPLACE(CONVERT(VARCHAR(30), JCMax.MaximetroExcesoP6), '.', ',') AS MaximetroExcesoP6

       /* --- 4. REACTIVA --- */
       , REPLACE(CONVERT(VARCHAR(30), JCRea.ConsumoReactivaP1), '.', ',') AS ConsumoReactivaP1
       , REPLACE(CONVERT(VARCHAR(30), JCRea.ConsumoReactivaP2), '.', ',') AS ConsumoReactivaP2
       , REPLACE(CONVERT(VARCHAR(30), JCRea.ConsumoReactivaP3), '.', ',') AS ConsumoReactivaP3
       , REPLACE(CONVERT(VARCHAR(30), JCRea.ConsumoReactivaP4), '.', ',') AS ConsumoReactivaP4
       , REPLACE(CONVERT(VARCHAR(30), JCRea.ConsumoReactivaP5), '.', ',') AS ConsumoReactivaP5
       , REPLACE(CONVERT(VARCHAR(30), JCRea.ConsumoReactivaP6), '.', ',') AS ConsumoReactivaP6
       , REPLACE(CONVERT(VARCHAR(30), JCRea.PrecioReactivaP1), '.', ',') AS PrecioReactivaP1
       , REPLACE(CONVERT(VARCHAR(30), JCRea.PrecioReactivaP2), '.', ',') AS PrecioReactivaP2
       , REPLACE(CONVERT(VARCHAR(30), JCRea.PrecioReactivaP3), '.', ',') AS PrecioReactivaP3
       , REPLACE(CONVERT(VARCHAR(30), JCRea.PrecioReactivaP4), '.', ',') AS PrecioReactivaP4
       , REPLACE(CONVERT(VARCHAR(30), JCRea.PrecioReactivaP5), '.', ',') AS PrecioReactivaP5
       , REPLACE(CONVERT(VARCHAR(30), JCRea.PrecioReactivaP6), '.', ',') AS PrecioReactivaP6
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCRea.ImporteReactivaP1, 0)), '.', ',') AS ImporteReactivaP1
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCRea.ImporteReactivaP2, 0)), '.', ',') AS ImporteReactivaP2
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCRea.ImporteReactivaP3, 0)), '.', ',') AS ImporteReactivaP3
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCRea.ImporteReactivaP4, 0)), '.', ',') AS ImporteReactivaP4
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCRea.ImporteReactivaP5, 0)), '.', ',') AS ImporteReactivaP5
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCRea.ImporteReactivaP6, 0)), '.', ',') AS ImporteReactivaP6
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCRea.ImporteReactivaTotal, 0)), '.', ',') AS ImporteReactivaTotal
       -- lecturas de reactiva del contador
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaAnteriorP1), '.', ',') AS LectReactivaAnteriorP1
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaAnteriorP2), '.', ',') AS LectReactivaAnteriorP2
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaAnteriorP3), '.', ',') AS LectReactivaAnteriorP3
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaAnteriorP4), '.', ',') AS LectReactivaAnteriorP4
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaAnteriorP5), '.', ',') AS LectReactivaAnteriorP5
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaAnteriorP6), '.', ',') AS LectReactivaAnteriorP6
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaActualP1), '.', ',') AS LectReactivaActualP1
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaActualP2), '.', ',') AS LectReactivaActualP2
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaActualP3), '.', ',') AS LectReactivaActualP3
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaActualP4), '.', ',') AS LectReactivaActualP4
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaActualP5), '.', ',') AS LectReactivaActualP5
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectReactivaActualP6), '.', ',') AS LectReactivaActualP6
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectConsumoReactivaP1), '.', ',') AS LectConsumoReactivaP1
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectConsumoReactivaP2), '.', ',') AS LectConsumoReactivaP2
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectConsumoReactivaP3), '.', ',') AS LectConsumoReactivaP3
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectConsumoReactivaP4), '.', ',') AS LectConsumoReactivaP4
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectConsumoReactivaP5), '.', ',') AS LectConsumoReactivaP5
       , REPLACE(CONVERT(VARCHAR(30), JCMax.LectConsumoReactivaP6), '.', ',') AS LectConsumoReactivaP6

       /* --- 5. TERMINO EXCESOS DISTRIBUIDORA --- */
       -- concepto 20006, el que pide el cliente
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesoDistribuidora, 0)), '.', ',') AS ImporteExcesoDistribuidora
       -- concepto 20004, exceso de comercializadora
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesoPotencia, 0)), '.', ',')      AS ImporteExcesoPotencia
       -- 20000-20999 completo
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesosTotal, 0)), '.', ',')        AS ImporteExcesosTotal
       , REPLACE(CONVERT(VARCHAR(30), JCExc.PotenciaExcesoP1), '.', ',') AS PotenciaExcesoP1
       , REPLACE(CONVERT(VARCHAR(30), JCExc.PotenciaExcesoP2), '.', ',') AS PotenciaExcesoP2
       , REPLACE(CONVERT(VARCHAR(30), JCExc.PotenciaExcesoP3), '.', ',') AS PotenciaExcesoP3
       , REPLACE(CONVERT(VARCHAR(30), JCExc.PotenciaExcesoP4), '.', ',') AS PotenciaExcesoP4
       , REPLACE(CONVERT(VARCHAR(30), JCExc.PotenciaExcesoP5), '.', ',') AS PotenciaExcesoP5
       , REPLACE(CONVERT(VARCHAR(30), JCExc.PotenciaExcesoP6), '.', ',') AS PotenciaExcesoP6
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesoP1, 0)), '.', ',') AS ImporteExcesoP1
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesoP2, 0)), '.', ',') AS ImporteExcesoP2
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesoP3, 0)), '.', ',') AS ImporteExcesoP3
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesoP4, 0)), '.', ',') AS ImporteExcesoP4
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesoP5, 0)), '.', ',') AS ImporteExcesoP5
       , REPLACE(CONVERT(VARCHAR(30), ISNULL(JCExc.ImporteExcesoP6, 0)), '.', ',') AS ImporteExcesoP6

       FROM FacturaVentaCabecera WITH(NOLOCK)
       LEFT JOIN Contrato WITH(NOLOCK) ON Contrato.IdContrato = FacturaVentaCabecera.IdContrato
       LEFT JOIN CUPS WITH(NOLOCK) ON Contrato.IdCups = CUPS.IdCups
       LEFT JOIN Distribuidora WITH(NOLOCK) ON CUPS.IdDistribuidora = Distribuidora.IdDistribuidora
       LEFT JOIN Cliente WITH(NOLOCK) ON Contrato.IdCliente = Cliente.IdCliente
       LEFT JOIN ClienteCategoria WITH(NOLOCK) ON Cliente.IdClienteCategoria = ClienteCategoria.IdClienteCategoria
       LEFT JOIN CNAE WITH(NOLOCK) ON Contrato.IdCNAE = CNAE.IdCNAE
       LEFT JOIN TipoCobro WITH(NOLOCK) ON FacturaVentaCabecera.IdTipoCobro = TipoCobro.IdTipoCobro
       LEFT JOIN Ciudad AS CiudadCUPS WITH(NOLOCK) ON CUPS.IdCiudad = CiudadCUPS.IdCiudad
       LEFT JOIN Provincia AS ProvinciaCUPS WITH(NOLOCK) ON CiudadCUPS.IdProvincia = ProvinciaCUPS.IdProvincia
       LEFT JOIN Callejero AS CallejeroCUPS WITH(NOLOCK) ON CallejeroCUPS.IdCallejero = CUPS.IdCallejero
       LEFT JOIN CallejeroTipoVia AS CallejeroTipoViaCUPS WITH(NOLOCK) ON CallejeroTipoViaCUPS.IdCallejeroTipoVia = CallejeroCUPS.IdCallejeroTipoVia
       LEFT JOIN CallejeroBloque AS CallejeroBloqueCUPS  WITH(NOLOCK) ON CallejeroBloqueCUPS.IdCallejeroBloque = CUPS.IdCallejeroBloque
       LEFT JOIN CallejeroEscalera AS CallejeroEscaleraCUPS WITH(NOLOCK) ON CallejeroEscaleraCUPS.IdCallejeroEscalera = CUPS.IdCallejeroEscalera
       LEFT JOIN CallejeroPiso AS CallejeroPisoCUPS  WITH(NOLOCK) ON CallejeroPisoCUPS.IdCallejeroPiso = CUPS.IdCallejeroPiso
       LEFT JOIN CallejeroPortal AS CallejeroPortalCUPS WITH(NOLOCK) ON CallejeroPortalCUPS.IdCallejeroPortal = CUPS.IdCallejeroPortal
       LEFT JOIN CallejeroPuerta AS CallejeroPuertaCUPS WITH(NOLOCK) ON CallejeroPuertaCUPS.IdCallejeroPuerta = CUPS.IdCallejeroPuerta
       LEFT JOIN CallejeroTipoCalificador AS CallejeroTipoCalificadorCUPS WITH(NOLOCK) ON CallejeroTipoCalificadorCUPS.IdCallejeroTipoCalificador = CUPS.IdCallejeroTipoCalificador
       LEFT JOIN CallejeroTipoNumero AS CallejeroTipoNumeroCUPS WITH(NOLOCK) ON CallejeroTipoNumeroCUPS.IdCallejeroTipoNumero = CUPS.IdCallejeroTipoNum
       LEFT JOIN Comercializadora AS Comercializadora WITH(NOLOCK) ON Comercializadora.IdComercializadora = Contrato.IdComercializadora
       LEFT JOIN CarteraCobroCalendario WITH(NOLOCK) ON CarteraCobroCalendario.IdCarteraCobroCalendario = Contrato.IdCarteraCobroCalendario
       LEFT JOIN FVL_SUM_ImporteBase_IH AS IH ON IH.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN TarifaPeaje AS TarifaPeaje WITH(NOLOCK) ON TarifaPeaje.IdTarifaPeaje = FacturaVentaCabecera.IdTarifaPeajeXML
       LEFT JOIN Tarifa AS Tarifa WITH(NOLOCK) ON Tarifa.IdTarifa = FacturaVentaCabecera.IdTarifaXML
       LEFT JOIN TarifaGrupo AS TarifaGrupo WITH(NOLOCK) ON TarifaGrupo.IdTarifaGrupo = FacturaVentaCabecera.IdTarifaGrupoXML

       LEFT JOIN FVT_SUM_Importes  AS ImportesTotales ON ImportesTotales.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_Potencia_ConsumoReactiva AS FacturaVentaLineaTotal ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaVentaLineaTotal.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_TotConsumoEnergiaXML_EnergiaAcceso AS FacturaTotales ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaTotales.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_TotConsumoEnergiaXML_Otros AS FacturaTotales2 ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaTotales2.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_ImporteBaseAlquiler AS FacturaVentaLineaAlq ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaVentaLineaAlq.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_ImporteBaseProductos AS FacturaVentaLineaProd ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaVentaLineaProd.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_ImporteBaseImpuestoIE AS FacturaVentaLineaImp ON FacturaVentaCabecera.IdFacturaVentaCabecera = FacturaVentaLineaImp.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_ImporteIEElectricidadE AS ConceptoIEElectricidad ON ConceptoIEElectricidad.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN FVL_SUM_ImporteIEGas AS ConceptoIEGas ON ConceptoIEGas.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN fvo ON fvo.idfvo = FacturaVentaCabecera.IdFacturaOrigen
       LEFT JOIN DiferenciaPagosPorCapacidad_Importe AS DPC ON DPC.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN AjusteCapGas_Importe AS AjusteGas ON AjusteGas.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN SumConsumosGas AS ConsumosGas ON ConsumosGas.IdFacturaVentaCabeceraGas = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN SumConsumosGas2 AS ConsumosGas2 ON ConsumosGas2.IdFacturaVentaCabeceraGas = FacturaVentaCabecera.IdFacturaVentaCabecera
       LEFT JOIN SerieNumeroFacturasOrigen ON FacturaVentaCabecera.IdFacturaVentaCabecera = SerieNumeroFacturasOrigen.IdFacturaVentaCabecera

        LEFT JOIN Lectura ON
            Lectura.IdFacturaVentaCabeceraSectorC =
            CASE
                WHEN (FacturaVentaCabecera.IsAbono = 1) THEN FacturaVentaCabecera.IdFacturaRectificativa
                WHEN (ISNULL(FacturaVentaCabecera.IsAbono, 0) = 0) THEN FacturaVentaCabecera.IdFacturaVentaCabecera
            END
            AND Lectura.CodigoContrato = Contrato.CodigoContrato
        LEFT JOIN FacturaCompraCabecera ON FacturaCompraCabecera.IdFacturaCompraCabecera = Lectura.IdFacturaCompraCabecera
        LEFT JOIN ServiciosAjusteSistema_Importe AS ServiciosAjusteSistema ON ServiciosAjusteSistema.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
        LEFT JOIN Canal ca ON FacturaVentaCabecera.IdCanal = ca.IdCanal

        /* ---- JOINS NUEVOS ---- */
        LEFT JOIN JC_Potencia  AS JCPot ON JCPot.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
        LEFT JOIN JC_Energia   AS JCEne ON JCEne.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
        LEFT JOIN JC_Reactiva  AS JCRea ON JCRea.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
        LEFT JOIN JC_Excesos   AS JCExc ON JCExc.IdFacturaVentaCabecera = FacturaVentaCabecera.IdFacturaVentaCabecera
        LEFT JOIN JC_Maximetro AS JCMax ON JCMax.IdLectura = Lectura.IdLectura

       WHERE FacturaVentaCabecera.Entorno = @Entorno
       AND FacturaVentaCabecera.SerieFactura IS NOT NULL
       AND FacturaVentaCabecera.FacturaCategoria = 'EN'
       AND FacturaVentaCabecera.IdTarifaGrupoXML IN (SELECT IdTarifaGrupo FROM TarifaGrupo WITH (NOLOCK) WHERE TextoTarifaGrupo LIKE @PatronTarifaGrupo)
       AND FacturaVentaCabecera.FechaFactura >= @FechaDesde
       AND FacturaVentaCabecera.FechaFactura <  DATEADD(DAY, 1, @FechaHasta)
       ORDER BY AnyoMes, Contrato.CodigoContrato, FacturaVentaCabecera.FechaFactura;

/* ============================================================================
   CONTROLES. Ejecutar sueltos ANTES de mandar el Excel.
   ============================================================================ */

/* C1. Que grupos tarifarios entran con el patron.
       Si aparece alguno que no toca, estrechar @PatronTarifaGrupo.

SELECT IdTarifaGrupo, TextoTarifaGrupo
FROM   TarifaGrupo WITH (NOLOCK)
WHERE  TextoTarifaGrupo LIKE 'PF AM JC CLM%'
ORDER  BY TextoTarifaGrupo;
*/

/* C2. Cuantas facturas salen por mes. Sirve para ver de un vistazo si falta
       algun mes del rango (abr-25 a sep-26 son 18 meses).

SELECT CONVERT(CHAR(7), fvc.FechaFactura, 126) AS AnyoMes,
       COUNT(*) AS Facturas,
       COUNT(DISTINCT fvc.CodigoContrato) AS Contratos
FROM   FacturaVentaCabecera AS fvc WITH (NOLOCK)
WHERE  fvc.Entorno = 'E1'
  AND  fvc.SerieFactura IS NOT NULL
  AND  fvc.FacturaCategoria = 'EN'
  AND  fvc.FechaFactura >= '20250401'
  AND  fvc.FechaFactura <  '20261001'
  AND  fvc.IdTarifaGrupoXML IN (SELECT IdTarifaGrupo FROM TarifaGrupo WITH (NOLOCK)
                                WHERE TextoTarifaGrupo LIKE 'PF AM JC CLM%')
GROUP BY CONVERT(CHAR(7), fvc.FechaFactura, 126)
ORDER BY AnyoMes;
*/

/* C3. Que conceptos de exceso aparecen de verdad en estas facturas.
       Confirma que el 20006 es el que lleva el termino de la distribuidora
       y si ademas hay 20004 (exceso de la comercializadora).

SELECT l.FacturaConcepto, COUNT(*) AS Lineas,
       COUNT(DISTINCT l.IdFacturaVentaCabecera) AS Facturas,
       SUM(l.ImporteBase) AS ImporteTotal
FROM   FacturaVentaLinea AS l WITH (NOLOCK)
INNER JOIN FacturaVentaCabecera AS fvc WITH (NOLOCK)
        ON fvc.IdFacturaVentaCabecera = l.IdFacturaVentaCabecera
WHERE  l.Entorno = 'E1'
  AND  l.FacturaConcepto BETWEEN 20000 AND 20999
  AND  fvc.FechaFactura >= '20250401' AND fvc.FechaFactura < '20261001'
  AND  fvc.IdTarifaGrupoXML IN (SELECT IdTarifaGrupo FROM TarifaGrupo WITH (NOLOCK)
                                WHERE TextoTarifaGrupo LIKE 'PF AM JC CLM%')
GROUP BY l.FacturaConcepto
ORDER BY l.FacturaConcepto;
*/

/* C4. Cuadre del consumo: la suma de P1..P6 tiene que coincidir con el consumo
       total de la factura. Se mide aqui sobre los valores numericos, porque en
       el SELECT principal ya salen como texto con coma decimal.
       Si devuelve filas, esas facturas tienen varias lineas del mismo periodo
       con consumos distintos (tipicamente regularizaciones) y hay que mirarlas.

SELECT  l.IdFacturaVentaCabecera,
        l.CodigoPeriodoXML                       AS Periodo,
        COUNT(DISTINCT l.TotConsumoEnergiaXML)   AS ValoresDistintos,
        MIN(l.TotConsumoEnergiaXML)              AS ConsumoMin,
        MAX(l.TotConsumoEnergiaXML)              AS ConsumoMax
FROM    FacturaVentaLinea AS l WITH (NOLOCK)
INNER JOIN FacturaVentaCabecera AS fvc WITH (NOLOCK)
        ON fvc.IdFacturaVentaCabecera = l.IdFacturaVentaCabecera
WHERE   l.Entorno = 'E1'
  AND   l.FacturaConcepto BETWEEN 30000 AND 30999
  AND   l.CodigoPeriodoXML BETWEEN 1 AND 6
  AND   fvc.FechaFactura >= '20250401' AND fvc.FechaFactura < '20261001'
  AND   fvc.IdTarifaGrupoXML IN (SELECT IdTarifaGrupo FROM TarifaGrupo WITH (NOLOCK)
                                 WHERE TextoTarifaGrupo LIKE 'PF AM JC CLM%')
GROUP BY l.IdFacturaVentaCabecera, l.CodigoPeriodoXML
HAVING  COUNT(DISTINCT l.TotConsumoEnergiaXML) > 1
ORDER BY l.IdFacturaVentaCabecera, Periodo;

  Lo mismo para la potencia, cambiando el rango a 10000-10999 y la columna a
  PotenciaContratadaXML / PotenciaAFacturarXML.
*/

/* ----------------------------------------------------------------------------
   NOTAS
   ----------------------------------------------------------------------------
   * FechaFactura vs FechaEmision: el script original filtra por FechaFactura y
     aqui se mantiene. Si negocio se refiere a la fecha de emision real, hay que
     cambiar las dos condiciones del WHERE y las de JC_Facturas por FechaEmision.

   * El rango se cierra con "< DATEADD(DAY,1,@FechaHasta)" en lugar de
     "<= @FechaHasta" porque FechaFactura es DATETIME: con <= se perderian las
     facturas del 30/09/2026 que tengan hora distinta de 00:00.

   * Fechas en ISO 'yyyymmdd' para que no dependan del idioma del login.

   * Decimales con coma: todas las columnas numericas salen ya como texto con
     REPLACE(CONVERT(VARCHAR(30), columna), '.', ','), para que Excel no lea los
     puntos como separador de miles.
     - Importes: van envueltos en ISNULL(...,0), asi que si no hay linea de ese
       concepto sale "0,00" en vez de celda vacia.
     - Medidas (potencia, consumo, maximetro, lecturas y precios): SIN ISNULL a
       proposito. Celda vacia significa "ese periodo no existe en la tarifa"
       (en 2.0TD no hay P4-P6), que no es lo mismo que haber consumido 0.
     Las CTE siguen siendo numericas: los controles de abajo cuadran sobre los
     numeros de verdad, no sobre el texto.

   * Rendimiento: las CTE FVL_SUM_* de la consulta original agregan sobre toda
     FacturaVentaLinea sin filtro previo. Con 18 meses eso no empeora (ya era
     asi), pero si tarda demasiado se les puede anadir
         INNER JOIN JC_Facturas AS jf ON jf.IdFacturaVentaCabecera = FacturaVentaLinea.IdFacturaVentaCabecera
     sin que cambie ningun resultado: son agregados por IdFacturaVentaCabecera y
     las facturas que se descartan no se usan en el SELECT final.
   ---------------------------------------------------------------------------- */
