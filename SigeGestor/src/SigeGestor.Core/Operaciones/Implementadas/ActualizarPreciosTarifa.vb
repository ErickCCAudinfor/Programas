Option Strict Off   ' Se apoya en los DTO portados, que se escribieron sin Option Strict.

Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Actualizar precios de tarifa. La operación que da nombre a ActualizaPrecios.
    '''
    ''' NO REIMPLEMENTA NADA: orquesta los métodos portados de FuncionesGenericas y
    ''' ContratoTarifaSrv en el mismo orden que ProcesarContratoTarifa del Form1 original.
    ''' Reescribir estas reglas a mano habría sido la peor forma de portarlas.
    '''
    ''' El orden importa y cada paso puede abortar:
    '''   1. Resolver el ContratoTarifa según el filtro de grupo actual.
    '''   2. Cambiar el grupo de tarifa (UpdateContratoTarifa).
    '''   3. Cargar el perfil de facturación, que decide si es indexado o fijo.
    '''   4. Calcular los precios nuevos: cuatro ramas, indexado/fijo por G1/G2.
    '''   5. TDVE inserta en vez de sustituir.
    '''   6. Calcular los precios viejos y sustituir.
    ''' Si algo falla después del paso 2, se DESHACE con UpdateContratoTarifaSiError. Esa
    ''' vuelta atrás es la razón por la que esto no puede ser un UPDATE suelto.
    ''' </summary>
    Public Class ActualizarPreciosTarifa
        Inherits OperacionPorEntrada

        Private ReadOnly _contratos As New RepositorioContratos()

        ' Códigos de contrato por entrada pegada. Se resuelve todo antes de empezar porque la
        ' copia de seguridad previa necesita la lista completa.
        Private _porEntrada As Dictionary(Of String, List(Of Long))

        Protected Overrides Async Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _porEntrada = New Dictionary(Of String, List(Of Long))(StringComparer.OrdinalIgnoreCase)
            Dim todos As New List(Of Long)

            For Each entrada In ctx.Entradas
                ctx.AbortarSiCancelado()

                Dim encontrados = Await _contratos.ResolverAsync(
                    ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

                Dim codigos = encontrados _
                    .Where(Function(c) c.Activo AndAlso c.CodigoContrato > 0) _
                    .Select(Function(c) c.CodigoContrato) _
                    .Distinct() _
                    .ToList()

                _porEntrada(entrada) = codigos
                todos.AddRange(codigos)
            Next

            ' Copia de seguridad del estado anterior, una sola vez y antes de tocar nada.
            ' Va a Escritorio\ConsultasBO\Precios del usuario que ejecuta.
            If todos.Count > 0 Then
                Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)
                funciones.EscribirContratoTarifaAntesCambios(todos.Distinct().ToList())
            End If

        End Function

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim codigos As List(Of Long) = Nothing
            If _porEntrada Is Nothing OrElse Not _porEntrada.TryGetValue(entrada, codigos) Then
                Return Task.FromResult(ResultadoEntrada.Fallo("no se pudo resolver la entrada"))
            End If

            If codigos.Count = 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos("no existe o no está activo"))
            End If

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)
            Dim srv As New ContratoTarifaSrv(ctx.CadenaConexion)

            Dim actualizados = 0
            Dim motivos As New List(Of String)

            For Each codigo In codigos
                ctx.AbortarSiCancelado()

                Dim motivo = ProcesarUnContrato(codigo, ctx, funciones, srv)
                If motivo.Length = 0 Then
                    actualizados += 1
                Else
                    motivos.Add($"{codigo}: {motivo}")
                End If
            Next

            If actualizados = 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo(String.Join(" · ", motivos)))
            End If

            If motivos.Count > 0 Then
                ' Parcial: se avisa, pero cuenta como hecho lo que se hizo.
                Return Task.FromResult(ResultadoEntrada.ConDatos(
                    actualizados, $"{actualizados} actualizados, {motivos.Count} no: {String.Join(" · ", motivos)}"))
            End If

            Return Task.FromResult(ResultadoEntrada.ConDatos(
                actualizados, If(actualizados = 1, "precios actualizados", $"{actualizados} actualizados")))

        End Function

        ''' <summary>
        ''' Un contrato. Devuelve cadena vacía si ha ido bien, o el motivo del fallo.
        ''' Transcripción de ProcesarContratoTarifa del Form1 original.
        ''' </summary>
        Private Function ProcesarUnContrato(codigo As Long,
                                            ctx As ContextoEjecucion,
                                            funciones As FuncionesGenericas,
                                            srv As ContratoTarifaSrv) As String

            ' 1. ContratoTarifa según el filtro: o los personalizados, o los que tienen
            '    exactamente el grupo actual indicado.
            Dim ct = srv.GetContratoTarifaPersonalizadaByCodigoContrato(
                codigo, ctx.GrupoTarifaActual, ctx.SoloPersonalizadas)

            If ct Is Nothing OrElse ct.IdContratoTarifa <= 0 Then
                Return "sin ContratoTarifa que cumpla el filtro"
            End If

            ' 2. Cambiar el grupo de tarifa. A partir de aquí hay que poder deshacer.
            Dim contratoAct = srv.UpdateContratoTarifa(
                ct, ctx.GrupoTarifa, ctx.GrupoTarifaActual, ctx.SoloPersonalizadas)

            If contratoAct Is Nothing OrElse contratoAct.IdContratoTarifa <= 0 Then
                Return "no se pudo cambiar el grupo de tarifa"
            End If

            ' 3. Perfil de facturación: decide indexado o fijo.
            contratoAct.PerfilFacturacion = funciones.GetPerfilFacturacion(contratoAct.IdPerfilFacturacion)
            If contratoAct.PerfilFacturacion Is Nothing Then
                srv.UpdateContratoTarifaSiError(ct)
                Return "sin perfil de facturación"
            End If

            ' 4. Precios nuevos.
            Dim preciosNuevos = ObtenerPreciosNuevos(contratoAct, funciones)
            If preciosNuevos.Count = 0 Then
                srv.UpdateContratoTarifaSiError(ct)
                Return "sin precios nuevos para ese grupo y fecha"
            End If

            ' 5. TDVE no sustituye: inserta.
            Dim textoTarifa As String = If(ct.TextoTarifa, "")
            If textoTarifa.Contains("TDVE") Then
                funciones.InsertTarifaPrecioContrato(preciosNuevos)
                Return ""
            End If

            ' 6. Precios viejos y sustitución.
            Dim preciosViejos = ObtenerPreciosAntiguos(ct, funciones)
            If preciosViejos.Count = 0 Then
                srv.UpdateContratoTarifaSiError(ct)
                Return "sin precios antiguos que sustituir"
            End If

            funciones.UpdatePrecioContratoTarifa(
                preciosNuevos, preciosViejos, contratoAct.PerfilFacturacion.isPerfilIndexado())

            Return ""

        End Function

        ''' <summary>
        ''' Precios nuevos. Cuatro ramas: indexado o fijo, y dentro del indexado G1 (luz) o
        ''' G2 (gas), que usan tablas distintas. Transcrito de ObtenerPreciosNuevos.
        ''' </summary>
        Private Function ObtenerPreciosNuevos(contrato As ContratoTarifa,
                                              funciones As FuncionesGenericas) As List(Of TarifaPrecioContrato)

            Dim lista As New List(Of TarifaPrecioContrato)
            Dim contratoBD As Contrato = funciones.GetContrato(If(contrato.CodigoContrato, 0L))
            Dim fecha As DateTime = FechaPresupuesto(contratoBD)

            If contrato.PerfilFacturacion.isPerfilIndexado() Then

                If contrato.Entorno = "G1" Then
                    For Each p In funciones.GetDTOAllPeriodosIndx(contrato.IdTarifa, contrato.IdTarifaGrupo, fecha) _
                                           .OrderBy(Function(f) f.IdIndexadoPrecio).ToList()
                        lista.Add(New TarifaPrecioContrato With {
                            .IdContratoTarifa = contrato.IdContratoTarifa,
                            .IdIndexadoPrecio = p.IdIndexadoPrecio,
                            .IdTarifaPeriodo = p.IdTarifaPeriodo,
                            .TextoTarifaPeriodo = p.TextoTarifaPeriodo,
                            .Entorno = p.Entorno})
                    Next
                Else
                    For Each p In funciones.GetDTOAllPeriodosIndxGasByFechaFinPresupuesto(
                                      contrato.Entorno, contrato.IdTarifa, contrato.IdTarifaGrupo, fecha)
                        lista.Add(New TarifaPrecioContrato With {
                            .IdContratoTarifa = contrato.IdContratoTarifa,
                            .IdIndexadoPrecioGas = p.IdIndexadoPrecioGas,
                            .IdTarifaPeriodo = p.IdTarifaPeriodo,
                            .TextoTarifaPeriodo = p.TextoTarifaPeriodo,
                            .Entorno = p.Entorno})
                    Next
                End If

            Else
                For Each p In funciones.GetDTOAllPeriodosTarifaPrecio(contrato.IdTarifa, contrato.IdTarifaGrupo, fecha) _
                                       .OrderBy(Function(f) f.IdTarifaPrecio).ToList()
                    lista.Add(New TarifaPrecioContrato With {
                        .IdContratoTarifa = contrato.IdContratoTarifa,
                        .IdTarifaPrecio = p.IdTarifaPrecio,
                        .IdTarifaPeriodo = p.IdTarifaPeriodo,
                        .TextoTarifaPeriodo = p.TextoTarifaPeriodo,
                        .Entorno = p.Entorno})
                Next
            End If

            Return lista

        End Function

        ''' <summary>
        ''' Precios actuales, para saber qué se sustituye. Se mira primero uno cualquiera para
        ''' deducir de qué tipo son. Transcrito de ObtenerPreciosAntiguos.
        ''' </summary>
        Private Function ObtenerPreciosAntiguos(contrato As ContratoTarifa,
                                                funciones As FuncionesGenericas) As List(Of TarifaPrecioContrato)

            Dim lista As New List(Of TarifaPrecioContrato)
            Dim precio = funciones.GetPrecioContratoTarifaV2(contrato)

            If precio Is Nothing OrElse precio.IdContratoTarifa <= 0 Then Return lista

            If contrato.Entorno = "G1" AndAlso precio.IdIndexadoPrecio > 0 Then
                lista = funciones.GetPrecioContratoTarifaIndex(contrato) _
                                 .OrderBy(Function(f) f.IdTarifaPeriodo).ToList()

            ElseIf (contrato.Entorno = "G1" OrElse contrato.Entorno = "G2") AndAlso precio.IdTarifaPrecio > 0 Then
                lista = funciones.GetPrecioContratoTarifa(contrato) _
                                 .OrderBy(Function(f) f.IdTarifaPeriodo).ToList()

            ElseIf contrato.Entorno = "G2" AndAlso precio.IdIndexadoPrecioGas > 0 Then
                lista = funciones.GetPrecioContratoTarifaIndexGas(contrato) _
                                 .OrderBy(Function(f) f.IdTarifaPeriodo).ToList()
            End If

            Return lista

        End Function

        ''' <summary>
        ''' Fecha con la que se buscan los precios: la de aplicación si la hay, si no la del
        ''' contrato, y si no hoy. Transcrito de ObtenerFechaPresupuesto.
        '''
        ''' Nota: la fecha del formulario NO se usa aquí, igual que en el original. Cada
        ''' contrato aporta la suya.
        ''' </summary>
        Private Shared Function FechaPresupuesto(contrato As Contrato) As DateTime

            If contrato Is Nothing Then Return DateTime.Today

            If contrato.FechaAplicacionPrecios IsNot Nothing AndAlso
               contrato.FechaAplicacionPrecios > Date.MinValue Then
                Return contrato.FechaAplicacionPrecios
            End If

            If contrato.FechaContrato IsNot Nothing AndAlso
               contrato.FechaContrato > Date.MinValue Then
                Return contrato.FechaContrato
            End If

            Return DateTime.Today

        End Function

    End Class

End Namespace
