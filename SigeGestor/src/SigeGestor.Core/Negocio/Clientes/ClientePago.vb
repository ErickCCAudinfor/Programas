Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Public Class ClientePago
    Public Property IdClientePago As Long
    Public Property idcliente As String
    Public Property NombreP As String
    Public Property IdentidadPago As String
    Public Property TextoColectivo As String
    Public Property TextoTipoCobro As String
    Public Property IBAN As String
    Public Property TextoBanco As String
    Public Property ClientePagoUnificado As String

End Class
