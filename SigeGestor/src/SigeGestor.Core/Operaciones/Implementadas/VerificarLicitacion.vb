Option Strict Off   ' Usa los DTO portados.

Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Dice si cada contrato tiene marcado el campo IsLicitacion. No cambia nada.
    '''
    ''' Se apoya en FuncionesGenericas.VerificarLicitacion, que ya venía portada y hace
    ''' exactamente esa consulta: los contratos de la lista con IsLicitacion = 1.
    '''
    ''' Nota sobre la semántica del resultado: «con datos» significa que SÍ está marcado como
    ''' licitación; «sin cambios» que existe pero no lo está. No es un fallo — es la respuesta
    ''' a la pregunta, y por eso no se cuenta como error.
    ''' </summary>
    Public Class VerificarLicitacion
        Inherits OperacionPorEntrada

        Private ReadOnly _contratos As New RepositorioContratos()

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            ' La consulta portada trabaja con códigos de contrato, así que primero se resuelve
            ' lo pegado —que puede ser CUPS o CIF— a códigos.
            Dim encontrados = Await _contratos.ResolverAsync(
                ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

            Dim codigos = encontrados.Where(Function(c) c.CodigoContrato > 0) _
                                     .Select(Function(c) c.CodigoContrato) _
                                     .Distinct().ToList()

            If codigos.Count = 0 Then
                Return ResultadoEntrada.SinDatos("no existe")
            End If

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)
            Dim conLicitacion = funciones.VerificarLicitacion(codigos)

            Dim marcados = If(conLicitacion Is Nothing, 0, conLicitacion.Count)

            If marcados = 0 Then
                Return ResultadoEntrada.SinDatos(
                    If(codigos.Count = 1, "no está marcado como licitación",
                                          $"ninguno de los {codigos.Count} está marcado"))
            End If

            If marcados = codigos.Count Then
                Return ResultadoEntrada.ConDatos(
                    marcados, If(marcados = 1, "es licitación", $"los {marcados} son licitación"))
            End If

            Return ResultadoEntrada.ConDatos(marcados, $"{marcados} de {codigos.Count} son licitación")

        End Function

    End Class

End Namespace
