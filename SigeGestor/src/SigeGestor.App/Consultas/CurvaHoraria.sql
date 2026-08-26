--Erick _Gestor Sige_ CurvaHoraria
SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H_082024
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H_092024
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H_032024
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H_022025
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'

UNION ALL

SELECT * FROM [SigeTotalTM].[dbo].CurvaHoraria_H
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'

ORDER BY FechaMedida;