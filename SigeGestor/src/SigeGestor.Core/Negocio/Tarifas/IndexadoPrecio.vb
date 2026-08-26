Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Public Class IndexadoPrecio
    Public Property IdIndexadoPrecio As Long
    Public Property Entorno As String
    Public Property IdTarifa As Nullable(Of Long)
    Public Property IdTarifaGrupo As Nullable(Of Long)
    Public Property IdIndexadoConcepto As Nullable(Of Long)
    Public Property IdTarifaPeriodo As Nullable(Of Long)
    Public Property FechaInicio As Nullable(Of Date)
    Public Property FechaFinal As Nullable(Of Date)
    Public Property PorcentajePotencia As Nullable(Of Decimal)
    Public Property ImportePotencia As Nullable(Of Decimal)
    Public Property PorcentajeEnergia As Nullable(Of Decimal)
    Public Property ImporteEnergia As Nullable(Of Decimal)
    Public Property FechaFinPresupuesto As Nullable(Of Date)
    Public Property FechaInicioAnexos As Nullable(Of Date)
    Public Property FechaFinalAnexos As Nullable(Of Date)
    Public Property IdIndexadoPrecioOld As Nullable(Of Long)
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

    'Public Overridable Property IndexadoConcepto As IndexadoConcepto
    'Public Overridable Property Tarifa As Tarifa
    Public Overridable Property TarifaGrupo As TarifaGrupo
    'Public Overridable Property TarifaPeriodo As TarifaPeriodo
End Class

Partial Class IndexadoPrecio
    Public Property TextoTarifaPeriodo As String
    Public Property TarifaPeriodo As TarifaPeriodo

End Class