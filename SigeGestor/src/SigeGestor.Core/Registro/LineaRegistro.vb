Imports System.Text.Json.Serialization

Namespace Registro

    ''' <summary>
    ''' Cómo se escribe una ejecución en el fichero de log.
    '''
    ''' Va aparte del modelo de dominio a propósito: el formato del log es un contrato que
    ''' sobrevive a los cambios internos. Si mañana <see cref="Modelos.Ejecucion"/> se
    ''' reorganiza, los logs de ayer siguen leyéndose.
    '''
    ''' Nombres cortos porque hay una línea por ejecución y el fichero se lee entero.
    ''' </summary>
    Public Class LineaRegistro

        <JsonPropertyName("v")>
        Public Property Version As Integer = 1

        <JsonPropertyName("id")>
        Public Property Id As Long

        <JsonPropertyName("ts")>
        Public Property Momento As DateTime

        <JsonPropertyName("op")>
        Public Property Operacion As String = ""

        <JsonPropertyName("gr")>
        Public Property Grupo As String = ""

        <JsonPropertyName("det")>
        Public Property Detalle As String = ""

        <JsonPropertyName("usr")>
        Public Property NombreUsuario As String = ""

        <JsonPropertyName("log")>
        Public Property Login As String = ""

        <JsonPropertyName("env")>
        Public Property Entorno As Integer

        <JsonPropertyName("est")>
        Public Property Estado As Integer

        <JsonPropertyName("reg")>
        Public Property Registros As Long

        <JsonPropertyName("err")>
        Public Property Errores As Integer

        <JsonPropertyName("seg")>
        Public Property Segundos As Double

        <JsonPropertyName("eq")>
        Public Property Equipo As String = ""

    End Class

End Namespace
