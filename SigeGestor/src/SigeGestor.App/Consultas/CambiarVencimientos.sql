/* =============================================================================
   jiraReplace - Cambiar el vencimiento de contratosReplace contratos (Excel ficheroExcelReplace).

   En cada contrato:
     - FechaVto        = fecha del Excel (VENCIMIENTO CORRECTO)
     - DiasVencimiento = dias entre FechaAlta y la nueva FechaVto (DATEDIFF, como lo calcula SIGE)
     - IsFijarFechaVto = 1, solo si @FijarFechaVto = 1. Lo elige quien genera el script, y
       tambien se puede cambiar aqui antes de lanzarlo.

   Se actualiza de uno en uno: el trigger de Contrato (CuantificacionDevoluciones)
   falla con un UPDATE de varias filas.

   @Paso = 'VER'      lo que cambiaria (no toca nada)
   @Paso = 'CAMBIAR'  con @Aplicar = 1: copia los valores actuales en
                      dbo.jiraSinGuionReplace_Contrato_fechaTablaReplace y actualiza. Relanzable.
   @Paso = 'DESHACER' con @Aplicar = 1: devuelve los valores originales.

   Al acabar, pedir SELECT @@TRANCOUNT (tiene que ser 0).
   PROD. Erick / fechaCabeceraReplace
   ============================================================================= */

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Paso    varchar(10) = 'VER';     -- 'VER' | 'CAMBIAR' | 'DESHACER'
DECLARE @Aplicar bit         = 0;

-- Poner IsFijarFechaVto = 1 en los contratos que se cambien. Con 0 se deja como este.
-- DESHACER devuelve siempre el valor original, mire lo que mire esto.
DECLARE @FijarFechaVto bit   = fijarFechaVtoReplace;

IF OBJECT_ID('dbo.jiraSinGuionReplace_Contrato_fechaTablaReplace') IS NULL
    CREATE TABLE dbo.jiraSinGuionReplace_Contrato_fechaTablaReplace (
        CodigoContrato          bigint   NOT NULL PRIMARY KEY,
        FechaVtoOriginal        datetime NULL,
        DiasVencimientoOriginal int      NULL,
        IsFijarFechaVtoOriginal bit      NULL,
        FechaVtoNuevo           datetime NULL,
        DiasVencimientoNuevo    int      NULL,
        FechaCambio             datetime NOT NULL DEFAULT GETDATE(),
        FechaDeshecho           datetime NULL
    );

DECLARE @Cod bigint, @FechaVto datetime, @Dias int, @IsFijar bit, @n int = 0;

/* ---------------------------------------------------------------- DESHACER */
IF @Paso = 'DESHACER'
BEGIN
    SELECT APendiente = COUNT(*) FROM dbo.jiraSinGuionReplace_Contrato_fechaTablaReplace WHERE FechaDeshecho IS NULL;
    IF @Aplicar = 0 BEGIN PRINT 'SIMULACION: no se ha tocado nada.'; RETURN; END

    DECLARE cDeshacer CURSOR LOCAL FAST_FORWARD FOR
        SELECT CodigoContrato, FechaVtoOriginal, DiasVencimientoOriginal, IsFijarFechaVtoOriginal
        FROM   dbo.jiraSinGuionReplace_Contrato_fechaTablaReplace WHERE FechaDeshecho IS NULL;
    OPEN cDeshacer;
    FETCH NEXT FROM cDeshacer INTO @Cod, @FechaVto, @Dias, @IsFijar;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRANSACTION;
        UPDATE dbo.Contrato SET FechaVto = @FechaVto, DiasVencimiento = @Dias, IsFijarFechaVto = @IsFijar
        WHERE  CodigoContrato = @Cod;
        UPDATE dbo.jiraSinGuionReplace_Contrato_fechaTablaReplace SET FechaDeshecho = GETDATE() WHERE CodigoContrato = @Cod;
        COMMIT TRANSACTION;
        SET @n += 1;
        FETCH NEXT FROM cDeshacer INTO @Cod, @FechaVto, @Dias, @IsFijar;
    END
    CLOSE cDeshacer; DEALLOCATE cDeshacer;

    SELECT Deshechos = @n, Trancount = @@TRANCOUNT;
    RETURN;
END

/* ------------------------------------------------------------- datos Excel */
IF OBJECT_ID('tempdb..#jiraSinGuionReplace_Excel') IS NOT NULL DROP TABLE #jiraSinGuionReplace_Excel;
CREATE TABLE #jiraSinGuionReplace_Excel (CodigoContrato bigint PRIMARY KEY, CUPS varchar(22), FechaVto datetime);

INSERT INTO #jiraSinGuionReplace_Excel (CodigoContrato, CUPS, FechaVto) VALUES
filasExcelReplace

/* ------------------------------------------------------------- plan */
IF OBJECT_ID('tempdb..#jiraSinGuionReplace_Plan') IS NOT NULL DROP TABLE #jiraSinGuionReplace_Plan;

SELECT  e.CodigoContrato, CUPSExcel = e.CUPS, CUPSContrato = cu.CodigoCUPS, c.FechaAlta,
        FechaVtoOriginal        = c.FechaVto,
        DiasVencimientoOriginal = c.DiasVencimiento,
        IsFijarFechaVtoOriginal = c.IsFijarFechaVto,
        FechaVtoNuevo           = e.FechaVto,
        DiasVencimientoNuevo    = DATEDIFF(DAY, c.FechaAlta, e.FechaVto),
        Aviso = CASE WHEN c.CodigoContrato IS NULL                                  THEN 'NO EXISTE EL CONTRATO'
                     WHEN LEFT(cu.CodigoCUPS, 20) <> LEFT(e.CUPS, 20)               THEN 'CUPS DISTINTO AL DEL EXCEL'
                     WHEN c.FechaAlta IS NULL                                       THEN 'SIN FECHA DE ALTA'
                     WHEN e.FechaVto <= c.FechaAlta                                 THEN 'VENCIMIENTO ANTERIOR AL ALTA'
                     WHEN c.FechaVto = e.FechaVto
                      AND c.DiasVencimiento = DATEDIFF(DAY, c.FechaAlta, e.FechaVto)
                      AND (@FijarFechaVto = 0 OR c.IsFijarFechaVto = 1)               THEN 'YA CAMBIADO'
                     ELSE 'CAMBIAR' END
INTO    #jiraSinGuionReplace_Plan
FROM    #jiraSinGuionReplace_Excel e
LEFT JOIN dbo.Contrato c ON c.CodigoContrato = e.CodigoContrato
LEFT JOIN dbo.CUPS cu    ON cu.IdCups = c.IdCups;

SELECT Aviso, Contratos = COUNT(*) FROM #jiraSinGuionReplace_Plan GROUP BY Aviso;
SELECT * FROM #jiraSinGuionReplace_Plan ORDER BY CASE WHEN Aviso = 'CAMBIAR' THEN 1 ELSE 0 END, CodigoContrato;

/* ---------------------------------------------------------------- CAMBIAR */
IF @Paso = 'CAMBIAR'
BEGIN
    IF @Aplicar = 0 BEGIN PRINT 'SIMULACION: no se ha tocado nada.'; RETURN; END

    DECLARE cCambiar CURSOR LOCAL FAST_FORWARD FOR
        SELECT CodigoContrato, FechaVtoNuevo, DiasVencimientoNuevo
        FROM   #jiraSinGuionReplace_Plan WHERE Aviso = 'CAMBIAR';
    OPEN cCambiar;
    FETCH NEXT FROM cCambiar INTO @Cod, @FechaVto, @Dias;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRANSACTION;

        MERGE dbo.jiraSinGuionReplace_Contrato_fechaTablaReplace AS d
        USING (SELECT * FROM #jiraSinGuionReplace_Plan WHERE CodigoContrato = @Cod) AS s ON d.CodigoContrato = s.CodigoContrato
        WHEN MATCHED THEN UPDATE SET FechaVtoOriginal = s.FechaVtoOriginal, DiasVencimientoOriginal = s.DiasVencimientoOriginal,
                                     IsFijarFechaVtoOriginal = s.IsFijarFechaVtoOriginal, FechaVtoNuevo = s.FechaVtoNuevo,
                                     DiasVencimientoNuevo = s.DiasVencimientoNuevo, FechaCambio = GETDATE(), FechaDeshecho = NULL
        WHEN NOT MATCHED THEN INSERT (CodigoContrato, FechaVtoOriginal, DiasVencimientoOriginal, IsFijarFechaVtoOriginal,
                                      FechaVtoNuevo, DiasVencimientoNuevo)
                              VALUES (s.CodigoContrato, s.FechaVtoOriginal, s.DiasVencimientoOriginal, s.IsFijarFechaVtoOriginal,
                                      s.FechaVtoNuevo, s.DiasVencimientoNuevo);

        UPDATE dbo.Contrato
        SET    FechaVto        = @FechaVto,
               DiasVencimiento = @Dias,
               IsFijarFechaVto = CASE WHEN @FijarFechaVto = 1 THEN 1 ELSE IsFijarFechaVto END
        WHERE  CodigoContrato = @Cod;

        COMMIT TRANSACTION;
        SET @n += 1;
        FETCH NEXT FROM cCambiar INTO @Cod, @FechaVto, @Dias;
    END
    CLOSE cCambiar; DEALLOCATE cCambiar;

    SELECT Cambiados = @n, Trancount = @@TRANCOUNT;
END
