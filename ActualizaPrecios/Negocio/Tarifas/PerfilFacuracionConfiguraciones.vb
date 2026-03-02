Partial Public Class PerfilFacturacionConfiguracion
    Public Property IdPerfilFacturacionConfiguracion As Long
    Public Property Entorno As String
    Public Property IdPerfilFacturacion As Long
    Public Property CodigoConcepto As Nullable(Of Integer)
    Public Property Clave As Nullable(Of Integer)

    Public Overridable Property PerfilFacturacion As PerfilFacturacion

End Class
Partial Class PerfilFacturacionConfiguracion
    Public Function IsConceptoIndexado() As Boolean
        Try
            Return If(Me.CodigoConcepto, 0) = 30004 OrElse If(Me.CodigoConcepto, 0) = 90012
        Catch ex As Exception
            Throw
        End Try
    End Function
End Class