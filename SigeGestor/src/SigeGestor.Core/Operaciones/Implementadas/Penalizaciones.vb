Option Strict Off   ' Usa los DTO portados.

Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Vuelca a Excel los precios y consumos de la última factura de cada contrato: un libro
    ''' para luz (P_LUZ, con los seis periodos) y otro para gas (P_GAS). No cambia nada.
    '''
    ''' El cálculo es el de ActualizaPrecios sin tocar una línea —PenalizacionesLuz y
    ''' PenalizacionesGas, 346 líneas de SQL sobre InfoLineaXML—, traído tal cual sobre un
    ''' adaptador de EPPlus (ver CompatEPPlus.vb). Se ha portado literal a propósito: ese SQL
    ''' con XPath sobre los conceptos de factura no es algo que convenga reescribir de memoria.
    '''
    ''' ES UNA SOLA OPERACIÓN, NO UNA POR ENTRADA: los dos métodos portados recorren la lista
    ''' entera por dentro y escriben un único libro por tipo. Partirlo por entrada obligaría a
    ''' reescribirlos, y daría un fichero por contrato en vez de uno con todo. Por eso el avance
    ''' se cuenta por fases y no por contrato: hasta que la consulta vuelve no hay nada que
    ''' contar.
    ''' </summary>
    Public Class Penalizaciones
        Inherits OperacionUnica

        Private ReadOnly _contratos As New RepositorioContratos()

        Protected Overrides Async Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                            avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            ' Los métodos portados trabajan con códigos de contrato, así que primero se resuelve
            ' lo pegado —que puede ser CUPS o CIF— igual que en el resto de operaciones.
            avisar("Resolviendo la lista…")

            Dim codigos As New List(Of Long)
            Dim sinResolver = 0

            For Each entrada In ctx.Entradas
                ctx.AbortarSiCancelado()

                Dim encontrados = Await _contratos.ResolverAsync(
                    ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

                Dim deEsta = encontrados.Where(Function(c) c.CodigoContrato > 0) _
                                        .Select(Function(c) c.CodigoContrato) _
                                        .ToList()

                If deEsta.Count = 0 Then
                    sinResolver += 1
                Else
                    codigos.AddRange(deEsta)
                End If
            Next

            codigos = codigos.Distinct().ToList()

            If codigos.Count = 0 Then
                Return ResultadoEntrada.SinDatos(
                    If(sinResolver = 1, "no existe", $"ninguna de las {sinResolver} entradas existe"))
            End If

            Dim carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                             RutasSalida.Asegurar("Penalizaciones"),
                             ctx.CarpetaDestino)

            avisar($"Calculando penalizaciones de {codigos.Count:N0} contratos…")

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)
            Dim validaciones As New ValidacionExcel(ctx.CadenaConexion, carpeta, codigos, funciones)

            ' La clasificación por entorno (E1 luz / E2 gas) y las dos consultas van dentro.
            ' Puede tardar: es una consulta por contrato y sobre XML.
            Dim algo = Await validaciones.GenerarPenalizacionesAsync().ConfigureAwait(False)

            If Not algo Then
                ' Los contratos existen pero ninguno es E1 ni E2, así que no hay hoja donde
                ' meterlos. No es un fallo: es que esos contratos no aplican.
                Return ResultadoEntrada.SinDatos(
                    $"los {codigos.Count:N0} contratos existen, pero ninguno es de luz (E1) ni de gas (E2)")
            End If

            Dim ficheros = validaciones.Generados.Select(Function(f) IO.Path.GetFileName(f)).ToList()

            Dim mensaje = $"{String.Join(" y ", ficheros)} en {carpeta}"
            If sinResolver > 0 Then mensaje &= $" · {sinResolver} entradas sin resolver"

            ' Los métodos portados no devuelven el número de filas escritas, así que aquí no se
            ' inventa: el recuento está en el propio Excel.
            Return ResultadoEntrada.ConDatos(codigos.Count, mensaje)

        End Function

    End Class

End Namespace
