-- Contratos por fecha de alta y CIF.
--
-- Las fechas van en AAAAMMDD: el servidor esta en español y '2026-01-01' falla o se lee del
-- reves. El original traia la fecha escrita a mano y solo acotaba por abajo; aqui va entre las
-- dos del formulario, que es lo que pide la pantalla.
--
-- identidadesReplace lo rellena la aplicacion con los CIF pegados, ya entrecomillados
-- y separados por comas.
SELECT
    c.CodigoCUPS,
    ctro.CodigoContrato,
    cl.Identidad,
    CASE ISNULL(cl.RazonSocial, '') WHEN '' THEN cl.Nombre + ' ' + cl.Apellido1 + ' ' + cl.Apellido2
         ELSE cl.RazonSocial END                AS DenominacionCliente,
    cs.TextoSituacion                           AS SituacionContrato,
    ctro.IsAgruparFacturas,
    ctro.RevisionFra,
    ctro.TextoRevision,
    ctro.FechaAlta,
    ctro.FechaVto,
    ctro.FechaBaja
FROM       Contrato ctro WITH (NOLOCK)
LEFT  JOIN ContratoSituacion cs WITH (NOLOCK) ON ctro.IdContratoSituacion = cs.IdContratoSituacion
LEFT  JOIN Cliente cl           WITH (NOLOCK) ON ctro.IdCliente = cl.IdCliente
INNER JOIN Cups c               WITH (NOLOCK) ON ctro.IdCups = c.IdCups
WHERE  ctro.FechaAlta >= 'DesdeFechaReplace'
  AND  ctro.FechaAlta <= 'hastaFechaReplace'
  AND  cl.Identidad IN (identidadesReplace)
ORDER BY cl.Identidad, ctro.FechaAlta, ctro.CodigoContrato;
