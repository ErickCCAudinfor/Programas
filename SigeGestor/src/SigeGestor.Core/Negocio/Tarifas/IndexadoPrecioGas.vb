Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.

Public Class IndexadoPrecioGas
    Public Property IdIndexadoPrecioGas As Long
    Public Property Entorno As String
    Public Property IdTarifa As Nullable(Of Long)
    Public Property IdTarifaGrupo As Nullable(Of Long)
    Public Property IdTarifaPeriodo As Nullable(Of Long)
    Public Property DesdeFecha As Nullable(Of Date)
    Public Property HastaFecha As Nullable(Of Date)
    Public Property A As Nullable(Of Decimal)
    Public Property B As Nullable(Of Decimal)
    Public Property C As Nullable(Of Decimal)
    Public Property FechaVigencia As Nullable(Of Date)
    Public Property FechaInicioAnexos As Nullable(Of Date)
    Public Property FechaFinalAnexos As Nullable(Of Date)
    Public Property Rawenergy As Nullable(Of Decimal)
    Public Property FNEEGas As Nullable(Of Decimal)
    Public Property SpreadPVBTTF As Nullable(Of Decimal)
    Public Property DesviosGas As Nullable(Of Decimal)
    Public Property BalancingTTFMAPF As Nullable(Of Decimal)
    Public Property BalancingTTFDA As Nullable(Of Decimal)
    Public Property ValidityTTF As Nullable(Of Decimal)
    Public Property ValidityMIBGAS As Nullable(Of Decimal)
    Public Property ValidityPFIJO As Nullable(Of Decimal)
    Public Property MermasATR As Nullable(Of Decimal)

    'Public Overridable Property Tarifa As Tarifa
    Public Overridable Property TarifaGrupo As TarifaGrupo
    'Public Overridable Property TarifaPeriodo As TarifaPeriodo

End Class

Partial Class IndexadoPrecioGas
    Public Property TextoTarifaPeriodo As String
    Public Property TarifaPeriodo As TarifaPeriodo
End Class