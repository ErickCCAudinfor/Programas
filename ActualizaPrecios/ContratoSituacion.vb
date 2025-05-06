Public Class ContratoSituacion
    Public Property IdContratoSituacion As Long
    Public Property Entorno As String
    Public Property TextoSituacion As String
    Public Property ImagenSituacion As String
    Public Property ColorSituacion As String
    Public Property ColorSituacionForeground As String
    Public Property IsEstadoTramite As Boolean
    Public Property IsEstadoContrato As Boolean
    Public Property IsEstadoIncidencia As Boolean
    Public Property IsEstadoAnulado As Boolean
    Public Property IsEstadoBaja As Boolean
    Public Property IsEstadoInspeccionAlta As Boolean
    Public Property IsEstadoCortado As Boolean
    Public Property IsActivo As Boolean
    Public Property IsEstadoPotencial As Nullable(Of Boolean)
    Public Property IsEstadoBajaModificacion As Nullable(Of Boolean)
    Public Property IsEstadoBajaRenovacion As Nullable(Of Boolean)
    Public Property IsClick As Nullable(Of Boolean)
    Public Property isPendienteFirma As Nullable(Of Boolean)
    Public Property isPendienteFirmaRenovacion As Nullable(Of Boolean)
    Public Property IsCaducadofirma As Nullable(Of Boolean)
    Public Property IsDuplicidadCups As Nullable(Of Boolean)
    Public Property IsRenovacion As Nullable(Of Boolean)
    Public Property CodigoControl As String
End Class