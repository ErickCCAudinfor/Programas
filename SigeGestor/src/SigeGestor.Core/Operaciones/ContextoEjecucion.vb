Imports System.Threading
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

    End Class

End Namespace
