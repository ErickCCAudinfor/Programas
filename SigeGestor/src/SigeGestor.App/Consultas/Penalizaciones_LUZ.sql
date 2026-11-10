--Gestor SIGE _ Penalizaciones LUZ (serie FELEC)
--
--Estaba escrita dentro de ValidacionExcel.vb, en el codigo portado de ActualizaPrecios.
--
--UNA SOLA EJECUCION PARA TODOS LOS CONTRATOS. Antes se lanzaba una consulta por contrato,
--cada una abriendo su propia conexion.
--
--contratosReplace lo rellena la aplicacion con los codigos de contrato separados por comas.
--Son numeros, validados antes por el analizador de entradas.
--
--QUE SE CORRIGIO AL SACARLA AQUI
--
--1. La subconsulta del concepto 30004 NO estaba acotada por el contrato: agrupaba TODAS las
--   lineas de FacturaVentaLinea con ese concepto, evaluando un .value() de XML en cada una, y
--   solo despues se unia por IdFacturaVentaCabecera. Con el GROUP BY por medio, SQL Server no
--   puede empujar el filtro hacia dentro. Eran 2,3 millones de lecturas fisicas POR CONTRATO.
--
--2. Los cuatro JOIN a FacturaVentaLinea (130004, 30001, 30002, 30004) se hacian a la vez y sin
--   correlacionar el periodo, asi que salia el producto cartesiano de todas sus filas y los
--   MAX() de arriba lo deshacian despues. Mismo resultado, el trabajo multiplicado. Aqui se
--   agrega una vez por factura y periodo.
--
--3. El WHERE final volvia a buscar las cabeceras por NumeroFactura SIN el contrato, asi que si
--   un numero de factura se repite entre contratos se colaban facturas ajenas.
--
--LO QUE NO SE HA TOCADO: la "ultima factura" sigue siendo MAX(NumeroFactura) como TEXTO, igual
--que antes, para que los resultados sean los mismos. Ojo: alfabeticamente '99' > '100'.
WITH UltimaFactura AS (
    -- La ultima factura FELEC de cada contrato.
    SELECT  f.CodigoContrato,
            MAX(f.NumeroFactura) AS NumeroFactura
    FROM    FacturaVentaCabecera f WITH (NOLOCK)
    WHERE   f.SerieFactura = 'FELEC'
      AND   f.CodigoContrato IN (contratosReplace)
    GROUP BY f.CodigoContrato
),
Cab AS (
    -- Las cabeceras de esas facturas. Cruzando TAMBIEN por contrato (ver nota 3).
    SELECT  fvc.IdFacturaVentaCabecera,
            fvc.CodigoContrato,
            fvc.SerieFactura,
            fvc.NumeroFactura,
            fvc.IdTarifaGrupoXML
    FROM    FacturaVentaCabecera fvc WITH (NOLOCK)
    INNER JOIN UltimaFactura u
            ON  u.CodigoContrato = fvc.CodigoContrato
            AND u.NumeroFactura  = fvc.NumeroFactura
    WHERE   fvc.SerieFactura = 'FELEC'
),
Lin AS (
    -- UNA sola lectura de las lineas, ya acotada a esas facturas, agregada por factura y
    -- periodo. Aqui estaba el coste.
    SELECT  l.IdFacturaVentaCabecera,
            l.CodigoPeriodoXML AS Periodo,

            MAX(CASE WHEN l.FacturaConcepto = 130004
                     THEN ISNULL(l.InfoLineaXML.value('(FacturaConceptosDTO/FacturaConceptoCargos/PrecioCargo)[1]', 'decimal(18,6)'), 0)
                END) AS PrecioCargo,

            MAX(CASE WHEN l.FacturaConcepto = 30001
                     THEN ISNULL(l.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)
                END) AS Precio30001,

            MAX(CASE WHEN l.FacturaConcepto = 30002
                     THEN ISNULL(l.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)
                END) AS Precio30002,

            MAX(CASE WHEN l.FacturaConcepto = 30002
                     THEN ISNULL(l.TotConsumoEnergiaXML, 0)
                END) AS Consumo30002,

            -- El 30004 va sumado, como en la subconsulta original
            SUM(CASE WHEN l.FacturaConcepto = 30004
                     THEN ISNULL(l.InfoLineaXML.value('(FacturaConceptosDTO/ConceptoEnergia/PrecioMedio)[1]', 'decimal(18,6)'), 0)
                END) AS PrecioVariable,

            MAX(CASE WHEN l.FacturaConcepto = 30004
                     THEN l.TotConsumoEnergiaXML
                END) AS ConsumoVariable

    FROM    FacturaVentaLinea l WITH (NOLOCK)
    INNER JOIN Cab ON Cab.IdFacturaVentaCabecera = l.IdFacturaVentaCabecera
    WHERE   l.FacturaConcepto IN (130004, 30001, 30002, 30004)
    GROUP BY l.IdFacturaVentaCabecera, l.CodigoPeriodoXML
)
SELECT
    Cab.CodigoContrato,
    Cab.SerieFactura,
    Cab.NumeroFactura,

    -- Precio de cada periodo: cargo + energia 30001 + energia 30002 + variable 30004
    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 1 THEN Lin.PrecioCargo     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 1 THEN Lin.Precio30001     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 1 THEN Lin.Precio30002     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 1 THEN Lin.PrecioVariable  END), 0), '.', ',') AS SumaPrecioP1,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 2 THEN Lin.PrecioCargo     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 2 THEN Lin.Precio30001     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 2 THEN Lin.Precio30002     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 2 THEN Lin.PrecioVariable  END), 0), '.', ',') AS SumaPrecioP2,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 3 THEN Lin.PrecioCargo     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 3 THEN Lin.Precio30001     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 3 THEN Lin.Precio30002     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 3 THEN Lin.PrecioVariable  END), 0), '.', ',') AS SumaPrecioP3,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 4 THEN Lin.PrecioCargo     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 4 THEN Lin.Precio30001     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 4 THEN Lin.Precio30002     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 4 THEN Lin.PrecioVariable  END), 0), '.', ',') AS SumaPrecioP4,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 5 THEN Lin.PrecioCargo     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 5 THEN Lin.Precio30001     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 5 THEN Lin.Precio30002     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 5 THEN Lin.PrecioVariable  END), 0), '.', ',') AS SumaPrecioP5,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 6 THEN Lin.PrecioCargo     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 6 THEN Lin.Precio30001     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 6 THEN Lin.Precio30002     END), 0)
          + ISNULL(MAX(CASE WHEN Lin.Periodo = 6 THEN Lin.PrecioVariable  END), 0), '.', ',') AS SumaPrecioP6,

    -- Consumo de cada periodo. En las tarifas «soul» se coge el del 30004.
    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 1
                            THEN CASE WHEN tg.TextoTarifaGrupo LIKE '%soul%'
                                      THEN Lin.ConsumoVariable ELSE Lin.Consumo30002 END
                       END), 0), '.', ',') AS ConsumoP1,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 2
                            THEN CASE WHEN tg.TextoTarifaGrupo LIKE '%soul%'
                                      THEN Lin.ConsumoVariable ELSE Lin.Consumo30002 END
                       END), 0), '.', ',') AS ConsumoP2,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 3
                            THEN CASE WHEN tg.TextoTarifaGrupo LIKE '%soul%'
                                      THEN Lin.ConsumoVariable ELSE Lin.Consumo30002 END
                       END), 0), '.', ',') AS ConsumoP3,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 4
                            THEN CASE WHEN tg.TextoTarifaGrupo LIKE '%soul%'
                                      THEN Lin.ConsumoVariable ELSE Lin.Consumo30002 END
                       END), 0), '.', ',') AS ConsumoP4,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 5
                            THEN CASE WHEN tg.TextoTarifaGrupo LIKE '%soul%'
                                      THEN Lin.ConsumoVariable ELSE Lin.Consumo30002 END
                       END), 0), '.', ',') AS ConsumoP5,

    REPLACE(ISNULL(MAX(CASE WHEN Lin.Periodo = 6
                            THEN CASE WHEN tg.TextoTarifaGrupo LIKE '%soul%'
                                      THEN Lin.ConsumoVariable ELSE Lin.Consumo30002 END
                       END), 0), '.', ',') AS ConsumoP6,

    tg.TextoTarifaGrupo

FROM        Cab
LEFT  JOIN  TarifaGrupo tg WITH (NOLOCK) ON tg.IdTarifaGrupo = Cab.IdTarifaGrupoXML
LEFT  JOIN  Lin                          ON Lin.IdFacturaVentaCabecera = Cab.IdFacturaVentaCabecera
GROUP BY
    Cab.CodigoContrato,
    Cab.SerieFactura,
    Cab.NumeroFactura,
    Cab.IdFacturaVentaCabecera,
    tg.TextoTarifaGrupo
ORDER BY
    Cab.CodigoContrato;
