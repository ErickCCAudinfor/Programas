--Erick _Gestor Sige_ ErroresSigeJob
--
-- Portada de ConsultaExtraerErroresSigeJobbyId.sql, con dos cambios:
--
--  1) El Id fijo (211429) pasa a un IN con el marcador idsSigeJobReplace, para poder pedir
--     varios trabajos de una sola vez.
--
--  2) Se selecciona TAMBIEN el idsigejob. La consulta original devolvia solo MensajeError, y
--     eso vale cuando se pregunta por un trabajo, pero pidiendo varios no habria forma de
--     saber de que trabajo es cada error. Y peor: el SELECT DISTINCT habria fusionado en una
--     sola fila el mismo mensaje de trabajos distintos.
--
-- SE LANZA UNA SOLA VEZ para todos los trabajos, no una por trabajo. El CROSS APPLY sobre el
-- XML es la parte cara: pidiendolos juntos se paga un recorrido en vez de uno por trabajo.
-- El reparto por trabajo lo hace despues el codigo, con la columna IdSigeJob, asi que se
-- sigue viendo cual no tiene ningun error.
SELECT DISTINCT
    idsigejob AS IdSigeJob,
    i.x.value('(Mensaje/text())[1]', 'NVARCHAR(MAX)') AS MensajeError
FROM SigeStep
CROSS APPLY ResultadoStepXML.nodes('/SigeResultDTO/IncidenciasList/Incidencia') AS i(x)
WHERE idsigejob IN (idsSigeJobReplace)
  AND i.x.value('(IsError/text())[1]', 'BIT') = 1
