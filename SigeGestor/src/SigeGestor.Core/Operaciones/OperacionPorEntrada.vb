Imports System.Diagnostics

Namespace Operaciones

    ''' <summary>Lo que sabe hacer una operación. Si es Nothing, no está portada todavía.</summary>
    Public Interface IOperacionEjecutable

        Function EjecutarAsync(ctx As ContextoEjecucion,
                               informar As Action(Of ProgresoOperacion)) As Task(Of ResultadoOperacion)

    End Interface

    ''' <summary>
    ''' La implementa la operación cuyas entradas salen del Excel y no de la lista pegada.
    '''
    ''' Con esto la interfaz puede decir «34 filas» antes de lanzar y pintar la tira de estados
    ''' con el tamaño correcto, en vez de tratar la importación como un solo paso opaco. El
    ''' formulario sigue sin saber nada del contenido: solo pide las etiquetas y las pasa.
    ''' </summary>
    Public Interface IEntradasDesdeExcel

        ''' <summary>
        ''' Una etiqueta por fila con datos, en el orden del fichero. Puede lanzar: quien llama
        ''' lo muestra como el problema que impide ejecutar.
        ''' </summary>
        Function LeerEntradas(rutaExcel As String) As IReadOnlyList(Of String)

    End Interface

    ''' <summary>
    ''' Maquinaria compartida de las operaciones que recorren una lista.
    '''
    ''' Es el equivalente de EjecutorConsultas en ActualizaPrecios, y aquí se lleva todo lo
    ''' que se repetía en cada bucle: contar, medir, capturar el error de una entrada sin
    ''' tumbar el resto, atender la cancelación y mantener la tira de estados.
    '''
    ''' Una operación concreta solo escribe ProcesarAsync para una entrada. Eso es lo que hace
    ''' que portar las 45 no haya sido portar 45 bucles.
    ''' </summary>
    Public MustInherit Class OperacionPorEntrada
        Implements IOperacionEjecutable

        ''' <summary>Cuántas líneas del registro se mantienen para la vista en vivo.</summary>
        Private Const LineasEnVivo As Integer = 40

        ''' <summary>
        ''' Procesa una entrada. Si lanza, el ejecutor lo recoge como fallo de esa entrada y
        ''' continúa: una operación de 216 elementos no se cae porque uno vaya mal.
        ''' </summary>
        Protected MustOverride Function ProcesarAsync(entrada As String,
                                                      ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

        ''' <summary>Se llama una vez antes de empezar. Para abrir lo que haga falta.</summary>
        Protected Overridable Function PrepararAsync(ctx As ContextoEjecucion) As Task
            Return Task.CompletedTask
        End Function

        ''' <summary>Se llama al final, incluso si se ha cancelado.</summary>
        Protected Overridable Function CerrarAsync(ctx As ContextoEjecucion,
                                                   resultado As ResultadoOperacion) As Task
            Return Task.CompletedTask
        End Function

        Public Async Function EjecutarAsync(ctx As ContextoEjecucion,
                                            informar As Action(Of ProgresoOperacion)) _
            As Task(Of ResultadoOperacion) Implements IOperacionEjecutable.EjecutarAsync

            Dim total = ctx.Entradas.Count
            Dim estados(Math.Max(0, total - 1)) As EstadoEntrada
            Dim vivo As New List(Of LineaProgreso)
            Dim fallidas As New List(Of String)

            ' Sin recortar, al contrario que «vivo»: de aquí sale lo que se copia al final.
            Dim incidencias As New List(Of LineaProgreso)

            ' Lo generado, sin repetir: 200 PDF de la misma carpeta son una sola línea en la
            ' pantalla. Se conserva el orden de aparición, que es el de la lista.
            Dim salidas As New List(Of String)
            Dim vistas As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            Dim conDatos = 0, sinDatos = 0, errores = 0
            Dim registros As Long = 0
            Dim cancelada = False

            Dim reloj = Stopwatch.StartNew()

            Await PrepararAsync(ctx).ConfigureAwait(False)

            Dim resultado As New ResultadoOperacion With {.Total = total}

            Try
                For i = 0 To total - 1
                    ctx.AbortarSiCancelado()

                    Dim entrada = ctx.Entradas(i)
                    estados(i) = EstadoEntrada.EnCurso

                    ' Se informa antes de empezar la entrada para que se vea cuál está en
                    ' marcha, no solo cuáles han acabado.
                    informar(Foto(i, total, entrada, registros, errores, sinDatos, reloj, estados, vivo))

                    Dim relojEntrada = Stopwatch.StartNew()
                    Dim r As ResultadoEntrada
                    Try
                        r = Await ProcesarAsync(entrada, ctx).ConfigureAwait(False)
                    Catch ex As OperationCanceledException
                        Throw
                    Catch ex As Exception
                        r = ResultadoEntrada.Fallo(ex.Message)
                    End Try
                    relojEntrada.Stop()
                    r.Duracion = relojEntrada.Elapsed

                    estados(i) = r.Estado
                    registros += r.Registros

                    Select Case r.Estado
                        Case EstadoEntrada.ConDatos : conDatos += 1
                        Case EstadoEntrada.SinDatos : sinDatos += 1
                        Case EstadoEntrada.Fallo
                            errores += 1
                            fallidas.Add(entrada)
                    End Select

                    Dim linea As New LineaProgreso With {
                        .Momento = DateTime.Now,
                        .Entrada = entrada,
                        .Estado = r.Estado,
                        .Registros = r.Registros,
                        .Mensaje = r.Mensaje,
                        .Duracion = r.Duracion
                    }

                    ' Las operaciones que llenan una carpeta informan de la CARPETA y no de cada
                    ' fichero, así que aquí 200 facturas dejan una entrada, no doscientas.
                    For Each s In r.Salidas
                        If vistas.Add(s) Then salidas.Add(s)
                    Next

                    vivo.Add(linea)
                    If vivo.Count > LineasEnVivo Then vivo.RemoveAt(0)

                    ' Todo lo que no salió con datos se guarda entero, para poder copiarlo.
                    If r.Estado <> EstadoEntrada.ConDatos Then incidencias.Add(linea)

                    informar(Foto(i + 1, total, "", registros, errores, sinDatos, reloj, estados, vivo))
                Next

            Catch ex As OperationCanceledException
                cancelada = True
            End Try

            reloj.Stop()

            resultado.ConDatos = conDatos
            resultado.SinDatos = sinDatos
            resultado.Errores = errores
            resultado.Registros = registros
            resultado.Duracion = reloj.Elapsed
            resultado.Cancelada = cancelada
            resultado.Fallidas = fallidas
            resultado.Incidencias = incidencias
            resultado.Salidas = salidas

            Await CerrarAsync(ctx, resultado).ConfigureAwait(False)

            Return resultado

        End Function

        ''' <summary>
        ''' Copia del progreso. Se copian los arrays porque el consumidor está en el hilo de
        ''' interfaz y no debe leer estructuras que aquí se siguen modificando.
        ''' </summary>
        Private Shared Function Foto(procesados As Integer,
                                     total As Integer,
                                     actual As String,
                                     registros As Long,
                                     errores As Integer,
                                     sinDatos As Integer,
                                     reloj As Stopwatch,
                                     estados As EstadoEntrada(),
                                     vivo As List(Of LineaProgreso)) As ProgresoOperacion

            Return New ProgresoOperacion With {
                .Procesados = procesados,
                .Total = total,
                .EntradaActual = actual,
                .Registros = registros,
                .Errores = errores,
                .SinDatos = sinDatos,
                .Transcurrido = reloj.Elapsed,
                .Estados = DirectCast(estados.Clone(), EstadoEntrada()),
                .Ultimas = vivo.ToList()
            }
        End Function

    End Class

End Namespace
