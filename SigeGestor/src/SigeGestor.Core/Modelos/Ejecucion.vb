Namespace Modelos

    ''' <summary>
    ''' OJO AL ORDEN: estos valores se guardan como número en el log, así que los nuevos se
    ''' añaden AL FINAL. Insertar uno en medio reinterpretaría los ficheros ya escritos.
    ''' </summary>
    Public Enum EstadoEjecucion
        EnCurso = 0
        Completada = 1
        ConErrores = 2
        Cancelada = 3

        ''' <summary>
        ''' Terminó sin fallos pero no modificó nada: ninguna entrada procedía. Se distingue
        ''' de Completada porque «Completada» con resultado 0 se lee como si hubiera trabajado.
        ''' </summary>
        SinCambios = 4
    End Enum

    ''' <summary>
    ''' Una ejecución registrada: qué se lanzó, quién, contra qué entorno y cómo acabó.
    '''
    ''' No hay tabla que la respalde: se guarda en ficheros de log junto al ejecutable, que
    ''' es lo que se acordó para no añadir una tabla nueva. Ver Registro.RegistroEjecuciones.
    ''' </summary>
    Public Class Ejecucion

        Public Property Id As Long

        ''' <summary>Nombre de la operación tal como se ve en la interfaz.</summary>
        Public Property Operacion As String = ""

        ''' <summary>Sección a la que pertenece: Precios, Contratos, Consultas…</summary>
        Public Property Grupo As String = ""

        ''' <summary>Detalle corto de lo que se pidió: «216 CUPS», «grupo SL-2026».</summary>
        Public Property Detalle As String = ""

        Public Property NombreUsuario As String = ""

        ''' <summary>Login con el que se lanzó. Es la clave para los accesos frecuentes.</summary>
        Public Property Login As String = ""

        ''' <summary>Máquina desde la que se lanzó.</summary>
        Public Property Equipo As String = ""

        Public Property Entorno As ClaveEntorno

        Public Property Estado As EstadoEjecucion

        ''' <summary>Filas, contratos o ficheros afectados, según la operación.</summary>
        Public Property Registros As Long

        Public Property Errores As Integer

        Public Property Duracion As TimeSpan

        Public Property Momento As DateTime

    End Class

    ''' <summary>
    ''' Operación que un usuario concreto usa a menudo. Alimenta las tarjetas de acceso
    ''' rápido del inicio, que son por persona: quien solo hace curvas ve curvas.
    ''' </summary>
    Public Class OperacionFrecuente

        Public Property Operacion As String = ""

        ''' <summary>Sección. La interfaz lo usa para elegir el icono.</summary>
        Public Property Grupo As String = ""

        ''' <summary>Entorno con el que se lanza habitualmente.</summary>
        Public Property Entorno As ClaveEntorno

        Public Property UltimaVez As DateTime

        ''' <summary>Resumen del último resultado: «216 contratos», «54 ficheros».</summary>
        Public Property UltimoResultado As String = ""

        ''' <summary>Veces que se ha lanzado en la ventana de log conservada.</summary>
        Public Property Veces As Integer

    End Class

End Namespace
