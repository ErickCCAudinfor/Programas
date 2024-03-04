Public Class ContratoContacto
    Public Property IdContratoContacto As Long
    Public Property Entorno As String
    Public Property CodigoContrato As Long
    Public Property IdClienteContacto As Nullable(Of Long)
    Public Property Bloque As String
    Public Property PorDefecto As Nullable(Of Boolean)

    Public Overridable Property ClienteContacto As ClienteContacto

End Class

Partial Class ContratoContacto
    Public Property IdCliente As Nullable(Of Long)
    Public Property Valor As String
End Class
