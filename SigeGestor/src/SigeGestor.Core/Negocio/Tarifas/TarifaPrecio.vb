Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Imports System.Runtime.Serialization

Public Class TarifaPrecio
    Public Property IdTarifaPrecio As Long
    Public Property Entorno As String
    Public Property IdTarifa As Nullable(Of Long)
    Public Property IdTarifaGrupo As Nullable(Of Long)
    Public Property IdTarifaPeriodo As Nullable(Of Long)
    Public Property FechaInicio As Nullable(Of Date)
    Public Property FechaFinal As Nullable(Of Date)
    Public Property PotenciaPrecio As Nullable(Of Decimal)
    Public Property PotenciaDto As Nullable(Of Decimal)
    Public Property PotenciaExtra As Nullable(Of Decimal)
    Public Property EnergiaPrecio As Nullable(Of Decimal)
    Public Property EnergiaDto As Nullable(Of Decimal)
    Public Property EnergiaExtra As Nullable(Of Decimal)
    Public Property MaximetroPrecio As Nullable(Of Decimal)
    Public Property ReactivaPrecio As Nullable(Of Decimal)
    Public Property FechaFinPresupuesto As Nullable(Of Date)
    Public Property DesdePotencia As Nullable(Of Decimal)
    Public Property HastaPotencia As Nullable(Of Decimal)
    Public Property FechaInicioAnexos As Nullable(Of Date)
    Public Property FechaFinalAnexos As Nullable(Of Date)
    Public Property PrecioExcedente As Nullable(Of Decimal)
    Public Property IdTarifaPrecioOld As Nullable(Of Long)
    Public Property Mercado1 As Nullable(Of Decimal)
    Public Property SAS As Nullable(Of Decimal)
    Public Property K As Nullable(Of Decimal)
    Public Property Desvios As Nullable(Of Decimal)
    Public Property IntElec As Nullable(Of Decimal)
    Public Property C2C5 As Nullable(Of Decimal)
    Public Property FNEE As Nullable(Of Decimal)
    Public Property BSocial As Nullable(Of Decimal)
    Public Property GdO As Nullable(Of Decimal)
    Public Property Balancing As Nullable(Of Decimal)
    Public Property Validity As Nullable(Of Decimal)
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


Partial Class TarifaPrecio
    Public Property TextoTarifaPeriodo As String

    Public Property TarifaPeriodo As TarifaPeriodo

End Class
