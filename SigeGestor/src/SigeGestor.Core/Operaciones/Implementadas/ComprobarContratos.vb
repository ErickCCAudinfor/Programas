Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Comprueba, uno a uno, si los códigos pegados existen como contrato y si están activos.
    '''
    ''' Solo lee. En ActualizaPrecios este paso estaba escondido dentro de Actualizar, en
    ''' BuscarbyCodigocontrato: se filtraban los que no existían y el usuario nunca llegaba a
    ''' saber cuáles se habían quedado fuera. Aquí es una operación en sí misma.
    ''' </summary>
    Public Class ComprobarContratos
        Inherits OperacionPorEntrada

        Private ReadOnly _contratos As New RepositorioContratos()

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim encontrados = Await _contratos.ResolverAsync(
                ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

            If encontrados.Count = 0 Then
                Return ResultadoEntrada.SinDatos("no existe")
            End If

            Dim activos = encontrados.Where(Function(c) c.Activo).Count()

            ' Que exista pero no esté activo importa: es la razón por la que una operación
            ' masiva se lo salta después sin decir nada.
            If activos = 0 Then
                Return ResultadoEntrada.ConDatos(
                    encontrados.Count,
                    If(encontrados.Count = 1, "existe, no activo", $"{encontrados.Count} existen, ninguno activo"))
            End If

            If activos = encontrados.Count Then
                Return ResultadoEntrada.ConDatos(
                    activos, If(activos = 1, "1 activo", $"{activos} activos"))
            End If

            Return ResultadoEntrada.ConDatos(
                encontrados.Count, $"{activos} de {encontrados.Count} activos")

        End Function

    End Class

End Namespace
