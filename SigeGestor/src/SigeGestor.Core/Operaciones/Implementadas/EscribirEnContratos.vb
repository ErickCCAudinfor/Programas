Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Base de las operaciones que resuelven la lista pegada a contratos activos y escriben en
    ''' cada uno.
    '''
    ''' Se lleva lo que se repite: resolver por contrato, CUPS o CIF; saltarse los que no están
    ''' activos informando de ello en lugar de en silencio; y contar bien las filas afectadas.
    '''
    ''' UNA DIFERENCIA DE CRITERIO CON EL ORIGINAL: aquí se hace un UPDATE por contrato, no uno
    ''' solo con «WHERE CodigoContrato IN (...)». Son más viajes a la base —para 400 contratos,
    ''' unos segundos más—, pero a cambio se sabe qué pasó con cada uno, se ve en la tira y se
    ''' puede reintentar solo lo que falló. Antes, un IN masivo devolvía un número y nada más.
    ''' </summary>
    Public MustInherit Class EscribirEnContratos
        Inherits OperacionPorEntrada

        Protected ReadOnly Contratos As New RepositorioContratos()

        ''' <summary>
        ''' Comprobaciones previas sobre el formulario, una sola vez antes de empezar. Se
        ''' devuelve el motivo si algo no cuadra, o cadena vacía si todo bien.
        ''' </summary>
        Protected Overridable Function Revisar(ctx As ContextoEjecucion) As String
            Return ""
        End Function

        ''' <summary>
        ''' Escribe en un contrato concreto. Devuelve las filas afectadas.
        '''
        ''' Llega el contrato entero y no solo su código porque hay operaciones que necesitan su
        ''' entorno: el agente y el administrador están duplicados por entorno en SIGE.
        ''' </summary>
        Protected MustOverride Function EscribirAsync(contrato As ContratoBreve,
                                                      ctx As ContextoEjecucion) As Task(Of Integer)

        ''' <summary>Cómo se resume lo hecho: «3 renovados», «2 reasignados».</summary>
        Protected MustOverride Function Resumen(afectadas As Integer) As String

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim problema = Revisar(ctx)
            If problema.Length > 0 Then Return ResultadoEntrada.Fallo(problema)

            Dim encontrados = Await Contratos.ResolverAsync(
                ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

            If encontrados.Count = 0 Then Return ResultadoEntrada.SinDatos("no existe")

            Dim activos = encontrados.Where(Function(c) c.Activo AndAlso c.CodigoContrato > 0).ToList()
            If activos.Count = 0 Then Return ResultadoEntrada.SinDatos("no está activo")

            Dim afectadas = 0
            For Each contrato In activos
                ctx.AbortarSiCancelado()
                afectadas += Await EscribirAsync(contrato, ctx).ConfigureAwait(False)
            Next

            If afectadas = 0 Then
                Return ResultadoEntrada.Fallo("activo, pero el update no afectó a ninguna fila")
            End If

            Return ResultadoEntrada.ConDatos(afectadas, Resumen(afectadas))

        End Function

    End Class

    ''' <summary>
    ''' Fija unidad tramitadora, oficina contable y órgano gestor en los contratos indicados.
    ''' </summary>
    Public Class ActualizarCodigosDir
        Inherits EscribirEnContratos

        Public Const CampoUnidad As String = "unidadTramitadora"
        Public Const CampoOficina As String = "oficinaContable"
        Public Const CampoOrgano As String = "organoGestor"

        Protected Overrides Function Revisar(ctx As ContextoEjecucion) As String
            ' Los tres van juntos: dejar uno vacío borraría ese código en todos los contratos
            ' sin que nadie lo hubiera pedido.
            If ctx.Campo(CampoUnidad).Length = 0 OrElse
               ctx.Campo(CampoOficina).Length = 0 OrElse
               ctx.Campo(CampoOrgano).Length = 0 Then
                Return "Faltan códigos DIR: hay que rellenar los tres."
            End If
            Return ""
        End Function

        Protected Overrides Function EscribirAsync(contrato As ContratoBreve,
                                                   ctx As ContextoEjecucion) As Task(Of Integer)
            Return Contratos.ActualizarCodigosDirAsync(
                ctx.CadenaConexion, contrato.CodigoContrato,
                ctx.Campo(CampoUnidad), ctx.Campo(CampoOficina), ctx.Campo(CampoOrgano),
                ctx.Cancelacion)
        End Function

        Protected Overrides Function Resumen(afectadas As Integer) As String
            Return If(afectadas = 1, "códigos fijados", $"códigos fijados en {afectadas}")
        End Function

    End Class

    ''' <summary>
    ''' Reasigna el agente o el administrador de los contratos indicados. Una sola clase para
    ''' las dos operaciones: solo cambian la columna y la lista de la que se elige.
    ''' </summary>
    Public Class ReasignarContrato
        Inherits EscribirEnContratos

        Public Const CampoDestino As String = "destino"

        Private ReadOnly _columna As String
        Private ReadOnly _admiteVacio As Boolean
        Private ReadOnly _queEs As String
        Private ReadOnly _origenLista As String

        ''' <param name="origenLista">
        ''' Lista de la que sale el destino. Se necesita para buscar la pareja del otro entorno;
        ''' vacío desactiva el emparejado.
        ''' </param>
        Public Sub New(columna As String,
                       queEs As String,
                       Optional admiteVacio As Boolean = False,
                       Optional origenLista As String = "")
            _columna = columna
            _queEs = queEs
            _admiteVacio = admiteVacio
            _origenLista = origenLista
        End Sub

        Protected Overrides Function Revisar(ctx As ContextoEjecucion) As String
            If ctx.Campo(CampoDestino).Length = 0 AndAlso Not _admiteVacio Then
                Return $"Elige el {_queEs} destino."
            End If
            Return ""
        End Function

        ''' <summary>
        ''' Todas las opciones de la lista, cargadas una vez. Hacen falta para emparejar luz con
        ''' gas, y no se piden por contrato porque la lista es la misma para todos.
        ''' </summary>
        Private _opciones As IReadOnlyList(Of OpcionLista)

        Protected Overrides Async Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _opciones = Nothing
            If _origenLista.Length = 0 Then Return

            Try
                Dim listas As New RepositorioListas()
                _opciones = Await listas.CargarAsync(
                    ctx.CadenaConexion, _origenLista, ctx.Cancelacion).ConfigureAwait(False)
            Catch ex As Exception
                ' Sin la lista no se puede emparejar, pero sí escribir lo elegido. Se sigue.
                _opciones = Nothing
            End Try

        End Function

        Protected Overrides Function EscribirAsync(contrato As ContratoBreve,
                                                   ctx As ContextoEjecucion) As Task(Of Integer)

            Dim crudo = ctx.Campo(CampoDestino)
            Dim destino As Long? = Nothing

            If crudo.Length > 0 Then
                Dim id As Long
                If Long.TryParse(crudo, id) Then destino = id
            End If

            ' EMPAREJADO LUZ / GAS. En SIGE cada agente y cada administrador está duplicado, uno
            ' con entorno G1 y otro con G2, y el contrato apunta al de su entorno. Si el elegido
            ' es de luz y el contrato es de gas, hay que escribir su pareja de gas: eso es lo
            ' que hacían los formularios Agentes y AdministradoresWF con sus dos desplegables.
            '
            ' Sin esto, una lista con contratos de los dos tipos acabaría con la mitad
            ' apuntando a un agente del entorno equivocado, y sin ningún aviso.
            If destino.HasValue AndAlso _opciones IsNot Nothing Then

                Dim elegida = _opciones.FirstOrDefault(Function(o) o.Id = destino.Value)
                If elegida IsNot Nothing Then

                    Dim pareja = RepositorioListas.Pareja(_opciones, elegida, contrato.EntornoMaestros)

                    If pareja Is Nothing Then
                        ' No hay pareja en el entorno del contrato: escribir el del otro entorno
                        ' sería peor que no hacer nada, así que se deja y se cuenta como 0.
                        Return Task.FromResult(0)
                    End If

                    destino = pareja.Id

                End If
            End If

            Return Contratos.ReasignarAsync(ctx.CadenaConexion, _columna, contrato.CodigoContrato, destino, ctx.Cancelacion)
        End Function

        Protected Overrides Function Resumen(afectadas As Integer) As String
            Return If(afectadas = 1, $"{_queEs} reasignado", $"{_queEs} reasignado en {afectadas}")
        End Function

    End Class

End Namespace
