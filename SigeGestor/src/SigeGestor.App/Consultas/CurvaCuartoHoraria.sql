--Erick _Gestor Sige_ CurvaCuartoHoraria
--
-- UN SOLO BLOQUE: ver el comentario de CurvaHoraria.sql. La tabla la pone SigeGestor en
-- «tablaReplace» y repite el bloque por cada historico que haya en la base.
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].tablaReplace
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'
