-- Contratos activos cuya ultima lectura facturada es muy vieja.
--
-- Se queda la ULTIMA lectura de cada contrato (ROW_NUMBER por CodigoContrato) y se deja
-- pasar solo si han transcurrido mas dias de los indicados.
--
-- LOS DOS ENTORNOS VAN JUNTOS Y NO SE ELIGEN POR SEPARADO. La lectura usa G1/G2 y el
-- contrato E1/E2 para lo mismo -luz o gas-, asi que la pantalla ofrece «Luz y gas», «Luz» o
-- «Gas» y rellena los dos marcadores de una vez. Cruzarlos no devuelve nada y no avisa.
--
-- Van como IN y no como igual: con «Luz y gas» entran los dos valores de una vez, que es lo
-- que sale por defecto cuando no se acota por tarifa.
--
-- idTarifaPeajeReplace llega como 0 cuando no se elige ninguna, y entonces no filtra.

DECLARE @IdTarifaPeaje AS bigint = idTarifaPeajeReplace;
DECLARE @Dias          AS int    = diasReplace;
DECLARE @Diahoy         AS date     = GETDATE();

;WITH LecturasOrdenadas AS (
    SELECT
        LecturaOrigen.TextoOrigen,
        dbo.FormatearDenominacion(Cliente.Apellido1, Cliente.Apellido2, Cliente.Nombre, Cliente.RazonSocial) AS DenominacionCliente,
        Contrato.CodigoContrato,
        Contrato.FechaAlta,
        Contrato.FechaBaja,
        ContratoSituacion.TextoSituacion,
        CUPS.CodigoCUPS,
        Lectura.IdLectura,
        CONVERT(varchar, Lectura.FechaLecturaAnterior, 103) AS FechaLecturaAnterior,
        CONVERT(varchar, Lectura.FechaLectura, 103)         AS FechaLectura,
        CONVERT(varchar, @Diahoy, 103)                      AS DiaActual,
        CONVERT(varchar, Lectura.FechaLectura, 103) + ' - ' + CONVERT(varchar, @Diahoy, 103) AS Hueco,
        DATEDIFF(day, Lectura.FechaLectura, @Diahoy)        AS DiasDiferencia,
        ROW_NUMBER() OVER (
            PARTITION BY Contrato.CodigoContrato
            ORDER BY Lectura.FechaLectura DESC
        ) AS rn
    FROM Lectura WITH (NOLOCK)
    LEFT JOIN EquipoMedida WITH (NOLOCK) ON EquipoMedida.IdEquipoMedida = Lectura.IdEquipoMedida
    LEFT JOIN TelegestionSituacion WITH (NOLOCK) ON EquipoMedida.IdTelegestionSituacion = TelegestionSituacion.IdTelegestionSituacion
    LEFT JOIN LecturaOrigen WITH (NOLOCK) ON LecturaOrigen.IdLecturaOrigen = Lectura.IdLecturaOrigen
    LEFT JOIN Contrato WITH (NOLOCK) ON Lectura.CodigoContrato = Contrato.CodigoContrato AND Contrato.Entorno IN (entornoContratoReplace)
    LEFT JOIN Cliente WITH (NOLOCK) ON Contrato.IdCliente = Cliente.IdCliente
    LEFT JOIN CUPS WITH (NOLOCK) ON Contrato.IdCups = CUPS.IdCups
    LEFT JOIN ContratoSituacion WITH (NOLOCK) ON Contrato.IdContratoSituacion = ContratoSituacion.IdContratoSituacion
    WHERE Lectura.Entorno IN (entornoLecturaReplace)
      AND CONVERT(date, Lectura.FechaLectura) < CONVERT(date, @Diahoy)
      AND ISNULL(Lectura.Facturado, 0) = 1
      AND (ISNULL(@IdTarifaPeaje, 0) = 0 OR Lectura.IdTarifaPeaje = @IdTarifaPeaje)
      AND Lectura.IdFacturaCompraCabecera IS NOT NULL
      AND Contrato.IdContratoSituacion = 1
)
SELECT *
FROM   LecturasOrdenadas
WHERE  rn = 1
  AND  DiasDiferencia > @Dias
ORDER BY DiasDiferencia DESC, CodigoContrato;
