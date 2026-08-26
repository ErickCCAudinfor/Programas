--Erick _Gestor Sige_ CurvaFacturable
SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable
WHERE left(cups,20) IN ( joinCupsReplace)   AND FechaMedida BETWEEN 'DesdeFechaReplace' and 'hastaFechaReplace'

UNION ALL

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H
WHERE left(cups,20) IN ( joinCupsReplace)   AND FechaMedida BETWEEN 'DesdeFechaReplace' and 'hastaFechaReplace'

UNION ALL

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_022025
WHERE left(cups,20) IN ( joinCupsReplace)   AND FechaMedida BETWEEN 'DesdeFechaReplace' and 'hastaFechaReplace'

UNION ALL

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_032024
WHERE left(cups,20) IN ( joinCupsReplace)  AND FechaMedida BETWEEN 'DesdeFechaReplace' and 'hastaFechaReplace'

UNION ALL

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_082024
WHERE left(cups,20) IN ( joinCupsReplace)   AND FechaMedida BETWEEN 'DesdeFechaReplace' and 'hastaFechaReplace'
UNION ALL 

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_082025
WHERE left(cups,20) IN ( joinCupsReplace)   AND FechaMedida BETWEEN 'DesdeFechaReplace' and 'hastaFechaReplace'
UNION ALL 

SELECT Entorno,CUPS,FechaMedida,Epoca,ActivaEntrante,ActivaSaliente,ReactivaQ1,ReactivaQ2,ReactivaQ3,ReactivaQ4,Flags,FechaRegistro,IndicadorObtencion,Prelacion,NumFactura
 FROM [SigeTotalTM].[dbo].CurvaFacturable_H_092024
WHERE left(cups,20) IN ( joinCupsReplace)   AND FechaMedida BETWEEN 'DesdeFechaReplace' and 'hastaFechaReplace'

ORDER BY FechaMedida;