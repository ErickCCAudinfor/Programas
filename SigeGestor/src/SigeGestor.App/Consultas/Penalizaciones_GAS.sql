--Gestor SIGE _ Penalizaciones GAS (serie FGAS)
--
--Estaba escrita dentro de ValidacionExcel.vb, en el codigo portado de ActualizaPrecios.
--
--UNA SOLA EJECUCION PARA TODOS LOS CONTRATOS. Antes se lanzaba una consulta por contrato.
--
--contratosReplace lo rellena la aplicacion con los codigos de contrato separados por comas.
--
--Esta pesa mucho menos que la de luz: no tenia la subconsulta sin acotar. Lo que cambia es
--que el ROW_NUMBER() ya iba PARTITION BY CodigoContrato, asi que vale tal cual para una
--lista, y que el cruce lleva ahora tambien el CodigoContrato: antes se hacia SOLO por
--NumeroFactura, y si un numero se repite entre contratos se colaban facturas ajenas.
--
--La "ultima factura" no se toca: sigue siendo el NumeroFactura mas alto como TEXTO.
WITH UltimaFactura AS (
    SELECT  f.NumeroFactura,
            f.CodigoContrato,
            ROW_NUMBER() OVER (PARTITION BY f.CodigoContrato
                               ORDER BY f.NumeroFactura DESC) AS rn
    FROM    FacturaVentaCabecera f WITH (NOLOCK)
    WHERE   f.SerieFactura = 'FGAS'
      AND   f.CodigoContrato IN (contratosReplace)
)
SELECT
    fvc.CodigoContrato,
    fvc.SerieFactura,
    fvc.NumeroFactura,
    REPLACE(
        ISNULL(SUM(
            CASE
                WHEN fvl.FacturaConcepto = 90066
                THEN ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoTerminoFijoGas/PrecioMedio)[1]', 'decimal(18,6)'), 0)
                ELSE ISNULL(fvl.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoConsumoGas/PrecioMedio)[1]',   'decimal(18,6)'), 0)
            END
        ), 0),
        '.', ','
    ) AS PrecioMedioTotal

FROM        FacturaVentaCabecera fvc WITH (NOLOCK)
INNER JOIN  UltimaFactura u
        ON  u.CodigoContrato = fvc.CodigoContrato
       AND  u.NumeroFactura  = fvc.NumeroFactura
       AND  u.rn             = 1
LEFT  JOIN  FacturaVentaLinea fvl WITH (NOLOCK)
        ON  fvl.IdFacturaVentaCabecera = fvc.IdFacturaVentaCabecera
       AND  fvl.FacturaConcepto IN (90012, 90038, 90062, 90066, 90001)
       AND  fvl.CodigoPeriodoXML = 1
WHERE       fvc.SerieFactura = 'FGAS'
GROUP BY
    fvc.CodigoContrato,
    fvc.SerieFactura,
    fvc.NumeroFactura
ORDER BY
    fvc.CodigoContrato;
