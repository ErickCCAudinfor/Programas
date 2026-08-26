Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Revisa qué contratos tienen precios personalizados y entrega dos Excel, uno de luz y
    ''' otro de gas. No cambia nada.
    '''
    ''' Llama a RevisaTarifaPrecioContratoPersonalizada y a su variante de gas, ya portadas.
    ''' No pide entradas: las consultas barren todos los contratos con grupo de tarifa que
    ''' contenga «persona».
    '''
    ''' DÓNDE DEJA LOS FICHEROS: en Escritorio\ConsultasBO\PreciosPersonalizados del usuario
    ''' que ejecuta. Los métodos portados llevaban la ruta fija al escritorio; ahora resuelven
    ''' por RutasSalida, igual que el resto de la aplicación. La ruta exacta se devuelve en el
    ''' resultado para no tener que buscarla.
    ''' </summary>
    Public Class RevisarTarifasPersonalizadas
        Inherits OperacionUnica

        Protected Overrides Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                      avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)
            Dim hechos As New List(Of String)
            Dim fallos As New List(Of String)

            ' Las dos consultas son largas: se avisa de cada fase para que no parezca colgado.
            avisar("Revisando precios personalizados de luz…")
            Try
                ctx.AbortarSiCancelado()
                Dim ruta = funciones.RevisaTarifaPrecioContratoPersonalizada()
                If Not String.IsNullOrWhiteSpace(ruta) Then hechos.Add(IO.Path.GetFileName(ruta))
            Catch ex As OperationCanceledException
                Throw
            Catch ex As Exception
                ' Que falle luz no impide intentar gas: son independientes.
                fallos.Add($"luz: {ex.Message}")
            End Try

            avisar("Revisando precios personalizados de gas…")
            Try
                ctx.AbortarSiCancelado()
                funciones.RevisaTarifaPrecioContratoPersonalizadaGas()
                hechos.Add("el de gas")
            Catch ex As OperationCanceledException
                Throw
            Catch ex As Exception
                fallos.Add($"gas: {ex.Message}")
            End Try

            If hechos.Count = 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo(String.Join(" · ", fallos)))
            End If

            Dim carpeta = Configuracion.RutasSalida.Asegurar("PreciosPersonalizados")

            Dim mensaje = $"Escrito {String.Join(" y ", hechos)} en {carpeta}"
            If fallos.Count > 0 Then mensaje &= $" · falló {String.Join(" · ", fallos)}"

            ' Los métodos portados no devuelven cuántas filas han salido, así que aquí no se
            ' inventa un número: el recuento va en el propio Excel.
            Return Task.FromResult(ResultadoEntrada.ConDatos(0, mensaje))

        End Function

    End Class

End Namespace
