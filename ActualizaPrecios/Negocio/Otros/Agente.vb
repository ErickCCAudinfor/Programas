
Public Class Agente
    Public Property IdAgente As Long
    Public Property Entorno As String
    Public Property NombreAgente As String
    Public Property Direccion As String
    Public Property CodigoPostal As String
    Public Property Ciudad As String
    Public Property Telefono As String
    Public Property Movil As String
    Public Property email As String
    Public Property Web As String
    Public Property IdProveedor As Nullable(Of Long)
    Public Property IdAgenteGrupo As Nullable(Of Long)
    Public Property Notas As String
    Public Property CodigoTipoAgente As Nullable(Of Integer)
    Public Property IdAgenteNivelAnterior As Nullable(Of Long)
    Public Property EmailSolicitud As String
    Public Property CodigoVendedor As String
    Public Property IdPerfilCanal As Nullable(Of Long)
    Public Property IdAgenteTipoVenta As Nullable(Of Long)
    Public Property CodigoTipoVenta As Nullable(Of Integer)
    Public Property NotificacionAutomaticas As Nullable(Of Integer)
    Public Property NotificacionAutomaticasJerarquia As Nullable(Of Integer)

End Class
