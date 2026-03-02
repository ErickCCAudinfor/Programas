
Partial Public Class TarifaPrecioContrato
    Public Property IdTarifaPrecioContrato As Long
    Public Property Entorno As String
    Public Property IdContratoTarifa As Nullable(Of Long)
    Public Property IdTarifaPrecio As Long
    Public Property IdIndexadoPrecio As Long
    Public Property IdIndexadoPrecioGas As Nullable(Of Long)

End Class

Partial Class TarifaPrecioContrato
    Public Property IdTarifa As Nullable(Of Long)
    Public Property IdTarifaGrupo As Nullable(Of Long)
    Public Property TextoTarifaGrupo As String
    Public Property IdTarifaPeriodo As Long
    Public Property TextoTarifaPeriodo As String
End Class