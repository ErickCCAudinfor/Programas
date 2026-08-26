Option Strict Off   ' Usa los DTO portados.

Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Reasigna el grupo de tarifa de una lista de contratos SIN recalcular precios.
    '''
    ''' AVISO DE PROCEDENCIA: en ActualizaPrecios no existe un botón para esto. Es el paso 2 de
    ''' «Actualizar precios de tarifa» aislado — el UPDATE de contratotarifa que hace
    ''' ContratoTarifaSrv.UpdateContratoTarifaV2 — sin los pasos de precios que vienen después.
    ''' Se implementa reutilizando ese mismo método, así que el efecto en la base es idéntico
    ''' al del original en ese punto.
    '''
    ''' CONSECUENCIA IMPORTANTE: al no recalcular, los contratos se quedan con el grupo nuevo y
    ''' los precios del viejo. Eso es lo que se ha pedido, pero conviene tenerlo presente: si
    ''' lo que se busca es cambiar grupo Y precios, la operación es «Actualizar precios de
    ''' tarifa», no esta.
    ''' </summary>
    Public Class CambiarGrupoTarifa
        Inherits OperacionPorEntrada

        Private ReadOnly _contratos As New RepositorioContratos()

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            If String.IsNullOrWhiteSpace(ctx.GrupoTarifa) Then
                Return ResultadoEntrada.Fallo("falta el grupo de tarifa destino")
            End If

            Dim encontrados = Await _contratos.ResolverAsync(
                ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

            Dim activos = encontrados.Where(Function(c) c.Activo AndAlso c.CodigoContrato > 0).ToList()
            If encontrados.Count = 0 Then Return ResultadoEntrada.SinDatos("no existe")
            If activos.Count = 0 Then Return ResultadoEntrada.SinDatos("no está activo")

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)
            Dim srv As New ContratoTarifaSrv(ctx.CadenaConexion)

            ' Los grupos de tarifa se buscan por texto y salen varios: uno por IdTarifa. Hay
            ' que quedarse con el que corresponde a la tarifa del contrato, igual que hace
            ' UpdateContratoTarifa en el original.
            Dim candidatos = funciones.GetTarifaGrupo(ctx.GrupoTarifa)
            If candidatos Is Nothing OrElse candidatos.Count = 0 Then
                Return ResultadoEntrada.Fallo($"el grupo de tarifa '{ctx.GrupoTarifa}' no existe")
            End If

            Dim cambiados = 0
            Dim motivos As New List(Of String)

            For Each contrato In activos
                ctx.AbortarSiCancelado()

                Dim ct = srv.GetContratoTarifaPersonalizadaByCodigoContrato(
                    contrato.CodigoContrato, ctx.GrupoTarifaActual, ctx.SoloPersonalizadas)

                If ct Is Nothing OrElse ct.IdContratoTarifa <= 0 Then
                    motivos.Add($"{contrato.CodigoContrato}: sin ContratoTarifa que cumpla el filtro")
                    Continue For
                End If

                Dim destino = candidatos.Where(Function(g) g.IdTarifa = ct.IdTarifa).FirstOrDefault()
                If destino Is Nothing OrElse destino.IdTarifaGrupo <= 0 Then
                    motivos.Add($"{contrato.CodigoContrato}: el grupo no existe para su tarifa")
                    Continue For
                End If

                Dim resultado = srv.UpdateContratoTarifaV2(ct, destino)
                If resultado Is Nothing OrElse resultado.IdContratoTarifa <= 0 Then
                    motivos.Add($"{contrato.CodigoContrato}: el update no afectó a ninguna fila")
                    Continue For
                End If

                cambiados += 1
            Next

            If cambiados = 0 Then
                Return ResultadoEntrada.Fallo(String.Join(" · ", motivos))
            End If

            If motivos.Count > 0 Then
                Return ResultadoEntrada.ConDatos(
                    cambiados, $"{cambiados} cambiados, {motivos.Count} no: {String.Join(" · ", motivos)}")
            End If

            Return ResultadoEntrada.ConDatos(
                cambiados, If(cambiados = 1, "grupo cambiado", $"{cambiados} grupos cambiados"))

        End Function

    End Class

End Namespace
