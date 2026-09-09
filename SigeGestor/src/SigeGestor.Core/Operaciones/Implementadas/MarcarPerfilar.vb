Option Strict Off   ' Usa los DTO portados.

Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Marca Perfilar = 1 en las lecturas de las facturas ATR indicadas.
    '''
    ''' EL NOMBRE DEL CATÁLOGO ENGAÑA, y viene del botón original («Checkear perfilar»). No
    ''' comprueba nada: hace un UPDATE sobre la tabla lectura. Está corregido en el catálogo
    ''' —marcada como escritura y con la descripción real— porque catalogada como lectura se
    ''' habría podido lanzar contra Réplica creyendo que no tocaba nada.
    '''
    ''' El original resolvía la lista con ConsultasSQL.BuscarFacturaATR, que sustituye
    ''' facturasatrBDReplace en la plantilla BuscarFacturaATR.sql. Aquí se hace lo mismo con
    ''' RepositorioSql, que es la misma mecánica de plantillas.
    '''
    ''' ES UNA SOLA OPERACIÓN, NO UNA POR FACTURA: el original lanza una única consulta con
    ''' todas las facturas y un único UPDATE con todos los IdLectura. Partirlo por factura
    ''' cambiaría el número de viajes a la base y el comportamiento.
    ''' </summary>
    Public Class MarcarPerfilar
        Inherits OperacionUnica

        Private ReadOnly _sql As New RepositorioSql()

        Protected Overrides Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                      avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim facturas = ctx.Entradas.Select(Function(f) f.Trim()) _
                                       .Where(Function(f) f.Length > 0) _
                                       .Distinct() _
                                       .ToList()

            If facturas.Count = 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("no hay facturas en la lista"))
            End If

            avisar($"Buscando las lecturas de {Redaccion.Cuenta(facturas.Count, "factura")}…")

            Dim plantilla As New PlantillaSql(_sql.Obtener("BuscarFacturaATR"))
            plantilla.Poner("facturasatrBDReplace",
                            String.Join(",", facturas.Select(Function(f) $"'{f}'")))

            Dim pendientes = plantilla.MarcadoresPendientes()
            If pendientes.Count > 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo(
                    $"la plantilla BuscarFacturaATR tiene marcadores sin sustituir: {String.Join(", ", pendientes)}"))
            End If

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)

            ' Se piden los IdLectura por separado, y no directamente UpdateMarcarPerfilarLectura,
            ' para poder distinguir «no había lecturas» de «el update no afectó a nada». El
            ' método original hace las dos cosas y devuelve 0 en ambos casos: además se come la
            ' excepción con Console.WriteLine, así que un fallo real también salía como 0.
            Dim ids = funciones.EjecutarConsultaFacturasATR(plantilla.ToString())

            If ids Is Nothing OrElse ids.Count = 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos(
                    If(facturas.Count = 1,
                   "la factura no tiene lecturas que marcar",
                   $"ninguna de las {facturas.Count:N0} facturas tiene lecturas que marcar")))
            End If

            ctx.AbortarSiCancelado()
            avisar($"Marcando perfilar en {ids.Count:N0} lecturas…")

            Dim filas = funciones.UpdateMarcarPerfilarLectura(plantilla.ToString())

            If filas <= 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo(
                    $"se encontraron {ids.Count:N0} lecturas pero el update no afectó a ninguna fila"))
            End If

            Return Task.FromResult(ResultadoEntrada.ConDatos(
                filas, $"{Redaccion.Cuenta(filas, "lectura marcada", "lecturas marcadas")} de " &
                       $"{Redaccion.Cuenta(facturas.Count, "factura")}"))

        End Function

    End Class

End Namespace
