-- Contratos con factura unica, por fecha de alta.
--
-- Las fechas van en AAAAMMDD: el servidor esta en español y '2026-01-01' falla o se lee del
-- reves. El original traia la fecha escrita a mano y solo acotaba por abajo; aqui va entre las
-- dos del formulario, que es lo que pide la pantalla.
--
-- Filtro del original: agrupan facturas y su revision es «unica». Se miran las dos
-- formas de escribirlo, con tilde y sin ella, porque en la tabla estan las dos.
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
  AND  ctro.IsAgruparFacturas = 1
  AND  (ctro.TextoRevision LIKE '%unica%' OR ctro.TextoRevision LIKE '%única%')
ORDER BY ctro.FechaAlta, ctro.CodigoContrato;
