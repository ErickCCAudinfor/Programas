--Erick _Gestor Sige_ CurvaHoraria
--
-- UN SOLO BLOQUE. El nombre de la tabla lo pone SigeGestor en «tablaReplace», y repite este
-- bloque unido con UNION ALL una vez por cada historico que encuentre en la base. Antes los
-- historicos venian escritos aqui a mano y se quedaron atras: faltaban _H_082025 y _H_082026.
-- El ORDER BY tambien lo pone el codigo, porque con los bloques unidos solo puede ir una vez.
SELECT * FROM [SigeTotalTM].[dbo].tablaReplace
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'
