Imports System.Threading
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Modelos

Namespace Operaciones

    ''' <summary>Cómo ha acabado una entrada. Es lo que colorea su celda en la tira.</summary>
    Public Enum EstadoEntrada
        Pendiente
        EnCurso
        ConDatos
        SinDatos
        Fallo
    End Enum

    ''' <summary>Lo que devuelve el procesado de una sola entrada.</summary>
    Public Class ResultadoEntrada

        Public Property Estado As EstadoEntrada
        Public Property Registros As Long
        Public Property Mensaje As String = ""
        Public Property Duracion As TimeSpan

        ''' <summary>
        ''' Ficheros o carpetas que ha generado esta entrada, con la ruta completa.
        '''
        ''' VA APARTE DEL MENSAJE A PROPÓSITO. El mensaje se pinta en tres sitios de una línea
        ''' —cabecera, registro y resumen— y una ruta ahí dentro se corta en los tres. Estando
        ''' aquí, la pantalla la muestra entera y con un botón para abrir la carpeta, y el
        ''' mensaje se queda con el recuento, que es lo que se lee de un vistazo.
        '''
        ''' Se admiten las dos cosas, ficheros y carpetas: hay operaciones que escriben un
        ''' fichero con nombre —el Excel de incidencias— y otras que llenan una carpeta con
        ''' cientos —los PDF, los trozos de XML—, donde el nombre de cada uno no dice nada.
        ''' Quien lo pinta distingue una de otra mirando el disco.
        ''' </summary>
        Public Property Salidas As IReadOnlyList(Of String) = Array.Empty(Of String)()

        ''' <summary>
        ''' Apunta lo que se ha generado y se devuelve a sí mismo, para poder escribir
        ''' «Return ResultadoEntrada.ConDatos(n, mensaje).Genera(ruta)» de una sola vez.
        ''' Las entradas vacías se descartan.
        ''' </summary>
        Public Function Genera(ParamArray rutas As String()) As ResultadoEntrada
            Salidas = rutas.Where(Function(r) Not String.IsNullOrWhiteSpace(r)).ToArray()
            Return Me
        End Function

        Public Shared Function ConDatos(registros As Long, Optional mensaje As String = "") As ResultadoEntrada
            Return New ResultadoEntrada With {
                .Estado = EstadoEntrada.ConDatos, .Registros = registros, .Mensaje = mensaje}
        End Function

        Public Shared Function SinDatos(Optional mensaje As String = "sin datos") As ResultadoEntrada
            Return New ResultadoEntrada With {.Estado = EstadoEntrada.SinDatos, .Mensaje = mensaje}
        End Function

        Public Shared Function Fallo(mensaje As String) As ResultadoEntrada
            Return New ResultadoEntrada With {.Estado = EstadoEntrada.Fallo, .Mensaje = mensaje}
        End Function

    End Class

    ''' <summary>
    ''' Foto del progreso. Se manda a la interfaz en cada entrada procesada.
    '''
    ''' Lleva la tira de estados completa y no solo el contador porque es lo que permite
    ''' pintar de un vistazo dónde ha fallado algo, que es la información que en
    ''' ActualizaPrecios se perdía.
    ''' </summary>
    Public Class ProgresoOperacion

        Public Property Procesados As Integer
        Public Property Total As Integer
        Public Property EntradaActual As String = ""
        Public Property Registros As Long
        Public Property Errores As Integer
        Public Property SinDatos As Integer
        Public Property Transcurrido As TimeSpan
        Public Property Estados As IReadOnlyList(Of EstadoEntrada) = Array.Empty(Of EstadoEntrada)()

        ''' <summary>Últimas entradas procesadas, para el registro en vivo.</summary>
        Public Property Ultimas As IReadOnlyList(Of LineaProgreso) = Array.Empty(Of LineaProgreso)()

        Public ReadOnly Property Fraccion As Double
            Get
                If Total <= 0 Then Return 0
                Return Math.Min(1, Procesados / CDbl(Total))
            End Get
        End Property

        ''' <summary>
        ''' Estimación de lo que queda, a partir del ritmo medio. Nothing hasta que haya al
        ''' menos una entrada hecha: antes de eso cualquier cifra sería inventada.
        ''' </summary>
        Public ReadOnly Property Restante As TimeSpan?
            Get
                If Procesados <= 0 OrElse Procesados >= Total Then Return Nothing
                Dim porEntrada = Transcurrido.TotalSeconds / Procesados
                Return TimeSpan.FromSeconds(porEntrada * (Total - Procesados))
            End Get
        End Property

    End Class

    ''' <summary>Una línea del registro en vivo.</summary>
    Public Class LineaProgreso
        Public Property Momento As DateTime
        Public Property Entrada As String = ""
        Public Property Estado As EstadoEntrada
        Public Property Registros As Long
        Public Property Mensaje As String = ""
        Public Property Duracion As TimeSpan
    End Class

    ''' <summary>Resumen final.</summary>
    Public Class ResultadoOperacion

        Public Property Total As Integer
        Public Property ConDatos As Integer
        Public Property SinDatos As Integer
        Public Property Errores As Integer
        Public Property Registros As Long
        Public Property Duracion As TimeSpan
        Public Property Cancelada As Boolean

        ''' <summary>Entradas que fallaron. Es lo que permite reintentar solo eso.</summary>
        Public Property Fallidas As IReadOnlyList(Of String) = Array.Empty(Of String)()

        ''' <summary>
        ''' Toda entrada que NO salió con datos: las que fallaron y las que no tenían nada.
        ''' Con su mensaje, para poder copiarlas y saber por qué.
        '''
        ''' SE RECOGE APARTE DEL REGISTRO EN VIVO A PROPÓSITO: aquel está limitado a las
        ''' últimas 40 líneas —si no, una ejecución de 5.000 entradas llenaría la pantalla de
        ''' filas que nadie va a leer—, así que en una tanda de 70 facturas las primeras 30 ya
        ''' no están. Esta lista no se recorta: es la que se copia al portapapeles.
        ''' </summary>
        Public Property Incidencias As IReadOnlyList(Of LineaProgreso) = Array.Empty(Of LineaProgreso)()

        ''' <summary>
        ''' Todo lo que se ha generado, juntando lo de cada entrada y sin repetidos. Es lo que
        ''' permite a la pantalla decir dónde han quedado los ficheros en vez de meter la ruta
        ''' en un mensaje que se corta. Ver <see cref="ResultadoEntrada.Salidas"/>.
        ''' </summary>
        Public Property Salidas As IReadOnlyList(Of String) = Array.Empty(Of String)()

        ''' <summary>
        ''' Añade a lo ya recogido, sin repetir.
        '''
        ''' HAY QUE USAR ESTO Y NO ASIGNAR SALIDAS desde un CerrarAsync: el ejecutor rellena
        ''' Salidas con lo de cada entrada ANTES de llamar a CerrarAsync, así que una asignación
        ''' ahí borraría los ficheros del bucle y solo quedarían los del cierre.
        ''' </summary>
        Public Sub AnadirSalidas(ParamArray rutas As String())

            Dim juntas As New List(Of String)(Salidas)
            Dim vistas As New HashSet(Of String)(Salidas, StringComparer.OrdinalIgnoreCase)

            For Each r In rutas
                If Not String.IsNullOrWhiteSpace(r) AndAlso vistas.Add(r) Then juntas.Add(r)
            Next

            Salidas = juntas

        End Sub

        Public Property Mensaje As String = ""

        Public ReadOnly Property Estado As EstadoEjecucion
            Get
                If Cancelada Then Return EstadoEjecucion.Cancelada
                If Errores > 0 Then Return EstadoEjecucion.ConErrores

                ' Fue bien pero no tocó nada. Se distingue a propósito: «Completada» con
                ' resultado 0 hace pensar que sí trabajó, y deja al usuario preguntándose
                ' si tiene que hacer algo más.
                If ConDatos = 0 AndAlso Total > 0 Then Return EstadoEjecucion.SinCambios

                Return EstadoEjecucion.Completada
            End Get
        End Property

        ''' <summary>
        ''' Desglose para el registro y el resumen: «1 aplicado, 3 sin cambios, 1 con error».
        ''' Es lo que permite entender un resultado 0 sin volver a abrir la operación.
        ''' </summary>
        Public ReadOnly Property Desglose As String
            Get
                Dim partes As New List(Of String)
                If ConDatos > 0 Then partes.Add($"{ConDatos:N0} con resultado")
                If SinDatos > 0 Then partes.Add($"{SinDatos:N0} sin cambios")
                If Errores > 0 Then partes.Add($"{Errores:N0} con error")
                If partes.Count = 0 Then Return "nada procesado"
                Return String.Join(", ", partes)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Todo lo que una operación necesita para ejecutarse: las entradas, el entorno con su
    ''' cadena de conexión, los parámetros del formulario y el canal por el que informar.
    '''
    ''' El ejecutable no sabe de dónde viene nada de esto: recibe el contexto y trabaja.
    ''' </summary>
    Public Class ContextoEjecucion

        Public Property Definicion As DefinicionOperacion
        Public Property Entorno As EntornoBD
        Public Property CadenaConexion As String = ""

        Public Property Entradas As IReadOnlyList(Of String) = Array.Empty(Of String)()
        Public Property TipoLista As TipoLista

        Public Property Desde As Date
        Public Property Hasta As Date
        Public Property Texto As String = ""
        Public Property GrupoTarifa As String = ""
        Public Property GrupoTarifaActual As String = ""
        Public Property SoloPersonalizadas As Boolean = True
        Public Property CarpetaDestino As String = ""
        Public Property RutaExcel As String = ""
        Public Property Dividir As Boolean

        ''' <summary>
        ''' Valores de los campos declarados en la definición, por su clave. En los de
        ''' selección viene el Id como texto; vacío significa «ninguno».
        ''' </summary>
        Public Property Campos As IReadOnlyDictionary(Of String, String) =
            New Dictionary(Of String, String)()

        ''' <summary>Valor de un campo, o cadena vacía si no está.</summary>
        Public Function Campo(clave As String) As String
            Dim v As String = Nothing
            If Campos IsNot Nothing AndAlso Campos.TryGetValue(clave, v) Then Return If(v, "")
            Return ""
        End Function

        Public Property Cancelacion As CancellationToken

        ''' <summary>Quién lanza. Va al registro.</summary>
        Public Property Usuario As Usuario

        Public Sub AbortarSiCancelado()
            Cancelacion.ThrowIfCancellationRequested()
        End Sub

        ''' <summary>
        ''' Abre y cierra la conexión. Devuelve cadena vacía si va, y el motivo si no.
        '''
        ''' POR QUÉ HACE FALTA: FuncionesGenericas se traga los errores de conexión. GetContrato,
        ''' por ejemplo, tiene el «If errores.HasError» con un comentario vacío dentro y un
        ''' Catch que solo escribe en la consola, así que devuelve un Contrato en blanco. Con la
        ''' base caída o el entorno equivocado, la operación no falla: informa de «No existe en
        ''' BD» de los 200 contratos, y quien lo lee acaba pensando que el Excel está mal.
        '''
        ''' Arreglarlo dentro de FuncionesGenericas obligaría a tocar los cientos de métodos que
        ''' comparten las 45 operaciones. Comprobarlo una vez antes de empezar cuesta una
        ''' conexión y convierte un resultado engañoso en un mensaje claro.
        ''' </summary>
        Public Async Function ProbarConexionAsync() As Task(Of String)

            If String.IsNullOrWhiteSpace(CadenaConexion) Then Return "no hay entorno seleccionado"

            Try
                Using conexion As New SqlConnection(CadenaConexion)
                    Await conexion.OpenAsync(Cancelacion).ConfigureAwait(False)
                End Using
                Return ""

            Catch ex As OperationCanceledException
                Throw

            Catch ex As Exception
                ' El error de red de SqlClient son cinco líneas y solo la primera dice algo.
                Dim motivo = ex.Message
                Dim corte = motivo.IndexOf(vbLf)
                If corte > 0 Then motivo = motivo.Substring(0, corte).Trim()
                Return motivo
            End Try

        End Function

    End Class

End Namespace
