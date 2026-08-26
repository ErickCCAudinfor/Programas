--Erick _Gestor Sige_ CurvaCuartoHoraria
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria
WHERE left(cups,20) in( joinCupsReplace)  AND FechaMedida between  'DesdeFechaReplace' and 'hastaFechaReplace'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_032024
WHERE left(cups,20) in( joinCupsReplace)  AND FechaMedida between  'DesdeFechaReplace' and 'hastaFechaReplace'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_022025
WHERE left(cups,20) in( joinCupsReplace)  AND FechaMedida between  'DesdeFechaReplace' and 'hastaFechaReplace'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_092024
WHERE left(cups,20) in( joinCupsReplace)  AND FechaMedida between  'DesdeFechaReplace' and 'hastaFechaReplace'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H_082025
WHERE left(cups,20) in( joinCupsReplace)  AND FechaMedida between  'DesdeFechaReplace' and 'hastaFechaReplace'
UNION ALL
SELECT [Entorno],[CUPS],[FechaMedida],[Epoca],[ActivaEntrante],[ActivaSaliente],[ReactivaQ1],[ReactivaQ2],[ReactivaQ3],[ReactivaQ4],[Flags],[FechaRegistro]
FROM [SigeTotalTM].[dbo].CurvaCuartoHoraria_H
WHERE left(cups,20) in( joinCupsReplace)  AND FechaMedida between  'DesdeFechaReplace' and 'hastaFechaReplace'