Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Public Class UsuarioValidacion
    Property Nombre As String
    Property login As String
    Property Password As String
    Property Servidor As String
    Property logeado As Boolean
    ''' <summary>
    ''' Última versión de novedades que este usuario ha leído. Vacío = no ha leído ninguna.
    ''' Se compara con NovedadesApp.VersionActual para saber si hay cambios pendientes.
    ''' </summary>
    Property VersionNovedadesLeida As String = ""
End Class
