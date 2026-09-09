--Erick _Gestor Sige_ CurvaValidada
--
-- Igual que las otras tres curvas de Pool: un solo bloque, la tabla la pone SigeGestor en
-- «tablaReplace» y la repite por cada historico que encuentre (CurvaValidada_H,
-- CurvaValidada_H_012026, _H_022025, ...).
--
-- SELECT * y no la lista de columnas: es la que trae CurvaHoraria, y aqui no se conocen las
-- columnas de CurvaValidada como para escribirlas. SigeGestor comprueba antes de unir que cada
-- historico tenga las mismas columnas y en el mismo orden que la tabla principal, y deja fuera
-- —diciendolo— el que no cuadre; sin eso, un UNION ALL con SELECT * puede cruzar columnas sin
-- dar error.
SELECT * FROM [SigeTotalTM].[dbo].tablaReplace
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'
