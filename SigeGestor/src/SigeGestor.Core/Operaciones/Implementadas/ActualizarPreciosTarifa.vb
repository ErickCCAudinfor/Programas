Option Strict Off   ' Se apoya en los DTO portados, que se escribieron sin Option Strict.

Imports System.Data
Imports System.IO
Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Consultas
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
    '''
    ''' ==================================================================================
    ''' TODO O NADA, igual que el original. Lo primero que se hace es resolver el
    ''' ContratoTarifa de TODOS los contratos de la lista; si a alguno le falta, NO SE TOCA
    ''' NINGUNO y se dice cuáles faltan.
    '''
    ''' Es la puerta que en el original está en ActualizarRegistros:
    '''     If contratosTarifa.Count &lt;&gt; ListaCodigo.Count Then
    '''         MostrarMensajePersonalizado("Las listas no coinciden") : Return
    '''
    ''' En una operación que reescribe precios en Producción no da igual: si de 200
    ''' contratos uno no cumple el filtro, conviene enterarse SIN haber actualizado los 199
    ''' otros. Se conserva a propósito. La única mejora es que aquí se dice QUÉ contratos
    ''' faltan, que el mensaje del original no lo decía.
    ''' ==================================================================================
    '''
    ''' DIFERENCIA CONSCIENTE con el original: aquí se aceptan CUPS y CIF además de códigos
    ''' de contrato. En ActualizaPrecios solo funcionaba con el check «Contrato» marcado —con
    ''' CUPS o Cliente, ObtenerContratos devolvía lista vacía y la operación no hacía nada—.
    ''' Se amplía porque el resto de operaciones ya resuelve las tres formas, pero OJO: un
    ''' CUPS o un CIF pueden traer más de un contrato, y todos entran en la puerta de arriba.
    '''
    ''' Y NO se filtra por contrato activo, también igual que el original: su
    ''' BuscarbyCodigocontrato es un SELECT sin condición de situación. En otras operaciones
    ''' sí se filtra; aquí filtrar sería inventarse una restricción que no existía.
    ''' </summary>
    Public Class ActualizarPreciosTarifa
        Inherits OperacionPorEntrada

        Private ReadOnly _contratos As New RepositorioContratos()

        ' Códigos de contrato por entrada pegada.
        Private _porEntrada As Dictionary(Of String, List(Of Long))

        ' ContratoTarifa ya resuelto por código. Se resuelven todos en PrepararAsync, igual
        ' que ObtenerContratosTarifaValidos del original, y desde aquí se reutilizan: así no
        ' se consulta dos veces la misma fila.
        Private _tarifaPorContrato As Dictionary(Of Long, ContratoTarifa)

        ''' <summary>Contrato y motivo de cada uno que no se ha podido actualizar.</summary>
        Private ReadOnly _incidencias As New List(Of KeyValuePair(Of Long, String))

        Private _carpetaIncidencias As String = ""

        ' ==================================================================
        ' PREPARACIÓN: resolver todo y cerrar la puerta antes de tocar nada
        ' ==================================================================

        Protected Overrides Async Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _porEntrada = New Dictionary(Of String, List(Of Long))(StringComparer.OrdinalIgnoreCase)
            _tarifaPorContrato = New Dictionary(Of Long, ContratoTarifa)
            _incidencias.Clear()

            Dim todos As New List(Of Long)
            Dim sinResolver As New List(Of String)

            For Each entrada In ctx.Entradas
                ctx.AbortarSiCancelado()

                Dim encontrados = Await _contratos.ResolverAsync(
                    ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

                ' SIN filtro de activo: el original tampoco lo aplica en esta operación.
                Dim codigos = encontrados _
                    .Where(Function(c) c.CodigoContrato > 0) _
                    .Select(Function(c) c.CodigoContrato) _
                    .Distinct() _
                    .ToList()

                _porEntrada(entrada) = codigos
                If codigos.Count = 0 Then sinResolver.Add(entrada)
                todos.AddRange(codigos)
            Next

            todos = todos.Distinct().ToList()

            If todos.Count = 0 Then
                Throw New InvalidOperationException(
                    "Ninguna de las entradas corresponde a un contrato que exista. No se ha tocado nada.")
            End If

            ' --- La puerta de todo o nada ---
            Dim srv As New ContratoTarifaSrv(ctx.CadenaConexion)
            Dim sinTarifa As New List(Of Long)

            For Each codigo In todos
                ctx.AbortarSiCancelado()

                Dim ct = srv.GetContratoTarifaPersonalizadaByCodigoContrato(
                    codigo, ctx.GrupoTarifaActual, ctx.SoloPersonalizadas)

                If ct Is Nothing OrElse ct.IdContratoTarifa <= 0 Then
                    sinTarifa.Add(codigo)
                Else
                    _tarifaPorContrato(codigo) = ct
                End If
            Next

            If sinTarifa.Count > 0 Then
                Throw New InvalidOperationException(MensajePuerta(sinTarifa, todos.Count, ctx))
            End If

            If sinResolver.Count > 0 Then
                Throw New InvalidOperationException(
                    $"{sinResolver.Count} de las entradas no corresponden a ningún contrato " &
                    $"({Muestra(sinResolver)}). No se ha tocado nada: quítalas de la lista o corrígelas.")
            End If

            ' Copia de seguridad del estado anterior, una sola vez y antes de tocar nada.
            ' Va a Escritorio\ConsultasBO\Precios del usuario que ejecuta.
            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)
            funciones.EscribirContratoTarifaAntesCambios(todos)

        End Function

        ''' <summary>
        ''' El mensaje de la puerta. Dice cuántos y cuáles, y recuerda con qué filtro se ha
        ''' buscado: casi siempre el problema es el filtro, no el contrato.
        ''' </summary>
        Private Shared Function MensajePuerta(sinTarifa As List(Of Long),
                                              total As Integer,
                                              ctx As ContextoEjecucion) As String

            Dim filtro = If(ctx.SoloPersonalizadas,
                            "solo tarifas personalizadas",
                            $"grupo de tarifa actual = «{ctx.GrupoTarifaActual}»")

            Return $"NO SE HA ACTUALIZADO NADA. {sinTarifa.Count} de {total} contratos no tienen " &
                   $"ContratoTarifa que cumpla el filtro ({filtro}): {Muestra(sinTarifa.Select(Function(c) c.ToString()))}. " &
                   "Se aplica todo o nada, así que revisa el filtro o quita esos contratos de la lista."

        End Function

        ''' <summary>Hasta diez, y el resto contado. Un mensaje con 200 códigos no se lee.</summary>
        Private Shared Function Muestra(valores As IEnumerable(Of String)) As String

            Dim lista = valores.ToList()
            If lista.Count <= 10 Then Return String.Join(", ", lista)

            Return String.Join(", ", lista.Take(10)) & $" y {lista.Count - 10} más"

        End Function

        ' ==================================================================
        ' EJECUCIÓN
        ' ==================================================================

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim codigos As List(Of Long) = Nothing
            If _porEntrada Is Nothing OrElse Not _porEntrada.TryGetValue(entrada, codigos) Then
                Return Task.FromResult(ResultadoEntrada.Fallo("no se pudo resolver la entrada"))
            End If

            ' No puede estar vacío: PrepararAsync habría abortado. Se comprueba por si acaso.
            If codigos.Count = 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos("no existe"))
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
                    _incidencias.Add(New KeyValuePair(Of Long, String)(codigo, motivo))
                End If
            Next

            If actualizados = 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo(String.Join(" · ", motivos)))
            End If

            If motivos.Count > 0 Then
                ' Parcial: se avisa, pero cuenta como hecho lo que se hizo. Aquí ya no aplica
                ' el todo o nada: el cambio del grupo ya está deshecho contrato a contrato con
                ' UpdateContratoTarifaSiError, que es lo que hace el original.
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

            ' 1. ContratoTarifa: ya resuelto en PrepararAsync, no se vuelve a consultar.
            Dim ct As ContratoTarifa = Nothing
            If Not _tarifaPorContrato.TryGetValue(codigo, ct) Then
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

        ' ==================================================================
        ' CIERRE: el Excel de incidencias
        ' ==================================================================

        ''' <summary>
        ''' Vuelca las incidencias a un Excel, como hacía ExportarErrores del original
        ''' («PreciosErrores»). Va a ConsultasBO\Precios en vez de al escritorio raso, y con
        ''' dos columnas —contrato y motivo— en vez de una cadena por fila: así se puede
        ''' filtrar y ordenar, que es para lo que se abre este fichero.
        '''
        ''' El mensaje del resultado también lleva los motivos, pero con muchos fallos se
        ''' vuelve ilegible; para eso está el fichero.
        ''' </summary>
        Protected Overrides Function CerrarAsync(ctx As ContextoEjecucion,
                                                 resultado As ResultadoOperacion) As Task

            _carpetaIncidencias = RutasSalida.Asegurar("Precios")

            If _incidencias.Count = 0 Then
                If resultado.ConDatos > 0 Then
                    ' La copia del estado anterior la escribe EscribirContratoTarifaAntesCambios
                    ' en esta misma carpeta. La ruta va en Salidas, no en el texto.
                    resultado.Mensaje = "Se ha guardado copia del estado anterior"
                    resultado.AnadirSalidas(_carpetaIncidencias)
                End If
                Return Task.CompletedTask
            End If

            Try
                Dim tabla As New DataTable("Incidencias")
                tabla.Columns.Add("CodigoContrato", GetType(Long))
                tabla.Columns.Add("Motivo", GetType(String))

                For Each i In _incidencias
                    tabla.Rows.Add(i.Key, i.Value)
                Next

                Dim escrito = EscritorExcel.Escribir(
                    tabla, _carpetaIncidencias, "PreciosIncidencias", "Incidencias")

                Dim nombre = If(escrito.Ficheros.Count > 0,
                                Path.GetFileName(escrito.Ficheros(0)),
                                "(no se pudo escribir)")

                resultado.Mensaje = Redaccion.Unir(
                    Redaccion.Cuenta(_incidencias.Count, "contrato sin actualizar",
                                                         "contratos sin actualizar"),
                    $"detalle en {nombre}")

                resultado.AnadirSalidas(_carpetaIncidencias)

            Catch ex As Exception
                ' Que falle el Excel no invalida lo ya actualizado, y los motivos siguen
                ' estando en el registro de la ejecución.
                resultado.Mensaje = Redaccion.Unir(
                    Redaccion.Cuenta(_incidencias.Count, "contrato sin actualizar",
                                                         "contratos sin actualizar"),
                    $"no se ha podido escribir el Excel de incidencias: {ex.Message}")
            End Try

            Return Task.CompletedTask

        End Function

        ' ==================================================================
        ' PRECIOS
        ' ==================================================================

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
