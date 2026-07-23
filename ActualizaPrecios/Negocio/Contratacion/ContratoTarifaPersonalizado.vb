Public Class ContratoTarifaPersonalizado
    Implements IEquatable(Of ContratoTarifaPersonalizado)

    Private _IdContratoTarifa As Long
    Private _CodigoContrato As Long
    Private _IdTarifaGrupo As Long
    Private _TextoTarifaGrupo As String
    Private _TextoPerfilFacturacion As String
    Private _IdTarifa As Long
    Private _TextoTarifa As String
    Private _FechaDesde As Date?
    Public Sub New()
        _IdContratoTarifa = 0 ' Long
        _CodigoContrato = 0 ' Long
        _IdTarifaGrupo = 0 ' Long
        _TextoTarifaGrupo = String.Empty ' String
        _TextoPerfilFacturacion = String.Empty ' String
        _IdTarifa = 0 ' Long
        _TextoTarifa = String.Empty ' String
        _FechaDesde = Nothing
    End Sub

    Public Property IdContratoTarifa As Long
        Get
            Return _IdContratoTarifa
        End Get
        Set(value As Long)
            _IdContratoTarifa = value
        End Set
    End Property

    Public Property CodigoContrato As Long
        Get
            Return _CodigoContrato
        End Get
        Set(value As Long)
            _CodigoContrato = value
        End Set
    End Property

    Public Property IdTarifaGrupo As Long
        Get
            Return _IdTarifaGrupo
        End Get
        Set(value As Long)
            _IdTarifaGrupo = value
        End Set
    End Property

    Public Property TextoTarifaGrupo As String
        Get
            Return _TextoTarifaGrupo
        End Get
        Set(value As String)
            _TextoTarifaGrupo = value
        End Set
    End Property

    Public Property TextoPerfilFacturacion As String
        Get
            Return _TextoPerfilFacturacion
        End Get
        Set(value As String)
            _TextoPerfilFacturacion = value
        End Set
    End Property

    Public Property IdTarifa As Long
        Get
            Return _IdTarifa
        End Get
        Set(value As Long)
            _IdTarifa = value
        End Set
    End Property

    Public Property TextoTarifa As String
        Get
            Return _TextoTarifa
        End Get
        Set(value As String)
            _TextoTarifa = value
        End Set
    End Property
    Public Property FechaDesde As String
        Get
            Return _FechaDesde
        End Get
        Set(value As String)
            _FechaDesde = value
        End Set
    End Property
    Public Overloads Function Equals(other As ContratoTarifaPersonalizado) As Boolean Implements IEquatable(Of ContratoTarifaPersonalizado).Equals
        Return Me.IdContratoTarifa = other.IdContratoTarifa AndAlso
               Me.CodigoContrato = other.CodigoContrato AndAlso
               Me.IdTarifaGrupo = other.IdTarifaGrupo AndAlso
               Me.TextoTarifaGrupo = other.TextoTarifaGrupo AndAlso
               Me.TextoPerfilFacturacion = other.TextoPerfilFacturacion AndAlso
               Me.IdTarifa = other.IdTarifa
        Me.TextoTarifa = other.TextoTarifa
        Me.FechaDesde = other.FechaDesde
    End Function

End Class
