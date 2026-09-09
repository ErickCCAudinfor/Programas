--Erick _Gestor Sige_ CurvaFacturable
--
-- UN SOLO BLOQUE: ver el comentario de CurvaHoraria.sql. La tabla la pone SigeGestor en
-- «tablaReplace». Antes los historicos venian escritos aqui y faltaba _H_082026.
SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
FROM [SigeTotalTM].[dbo].tablaReplace
WHERE left(cups,20) IN (joinCupsReplace) AND FechaMedida BETWEEN 'DesdeFechaReplace' AND 'hastaFechaReplace'
