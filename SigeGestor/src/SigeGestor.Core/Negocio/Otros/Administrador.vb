Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Public Class Administrador
    Public Property IdAdministrador As Long
    Public Property Entorno As String
    Public Property NombreAdministrador As String
    Public Property Direccion As String
    Public Property CodigoPostal As String
    Public Property Ciudad As String
    Public Property Telefono As String
    Public Property Movil As String
    Public Property email As String
    Public Property Web As String
    Public Property Notas As String
    Public Property UsuarioWeb As String
    Public Property PasswordWeb As String
    Public Property FincasPlus As Nullable(Of Boolean)
    Public Property TuComunidad As Nullable(Of Boolean)
    Public Property TAAF As Nullable(Of Boolean)
    Public Property IsPasswordEncriptado As Nullable(Of Boolean)
End Class
