Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Vuelve a dejar pendientes de renovación los contratos indicados, poniendo
    ''' IsRenovacionProcesada a NULL.
    '''
    ''' PRIMERA OPERACIÓN DE ESCRITURA PORTADA. Solo toca los contratos ACTIVOS
    ''' (IdContratoSituacion = 1), igual que RenovarContratosActivos en ActualizaPrecios.
    '''
    ''' DOS FALLOS DEL ORIGINAL QUE AQUÍ NO ESTÁN:
    '''
    ''' 1) El total estaba mal contado. RenovarContratosActivos hacía
    '''    «totalRenovados = Await ...» dentro del bucle en lugar de «+=», así que el número
    '''    que se enseñaba al final era el de la última iteración —1, o 0— y no el total.
    '''
    ''' 2) Con un CUPS con espacios reventaba. ObtenerContratosActivosPorCUPS quitaba los
    '''    espacios y luego hacía Substring(0, Math.Min(20, cps.Length)) usando la longitud
    '''    del CUPS ORIGINAL, no la del recortado: si sobraban caracteres, salía
    '''    ArgumentOutOfRangeException. Aquí no puede pasar porque AnalizadorEntradas ya
    '''    descarta cualquier CUPS con espacios antes de llegar.
    '''
    ''' Los contratos que existen pero no están activos se cuentan como «sin datos», no como
    ''' error: no es un fallo, es que no procedía tocarlos, y conviene que se vea.
    ''' </summary>
    Public Class VolverARenovar
        Inherits OperacionPorEntrada

        Private ReadOnly _contratos As New RepositorioContratos()

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim encontrados = Await _contratos.ResolverAsync(
                ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

            If encontrados.Count = 0 Then
                Return ResultadoEntrada.SinDatos("no existe")
            End If

            Dim activos = encontrados.Where(Function(c) c.Activo AndAlso c.CodigoContrato > 0).ToList()
            If activos.Count = 0 Then
                Return ResultadoEntrada.SinDatos("no está activo")
            End If

            ' Se acumula de verdad, al contrario que el original.
            Dim afectadas = 0
            For Each contrato In activos
                ctx.AbortarSiCancelado()
                afectadas += Await _contratos.VolverARenovarAsync(
                    ctx.CadenaConexion, contrato.CodigoContrato, ctx.Cancelacion).ConfigureAwait(False)
            Next

            If afectadas = 0 Then
                ' Estaba activo pero el UPDATE no tocó nada: raro, y conviene enterarse.
                Return ResultadoEntrada.Fallo("activo, pero el update no afectó a ninguna fila")
            End If

            Return ResultadoEntrada.ConDatos(
                afectadas,
                If(afectadas = 1, "1 renovado", $"{afectadas} renovados"))

        End Function

    End Class

End Namespace
