Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Imports System.Runtime.Serialization

Public Class ContratoTarifa
    Inherits BaseGenericDTO
    Implements IEquatable(Of ContratoTarifa)

    Public Sub New()
        _IdContratoTarifa = 0   ' Long
        _Entorno = String.Empty ' String
        _CodigoContrato = Nothing   ' Nullable(Of Long)
        _IdTarifa = Nothing ' Nullable(Of Long)
        _IdTarifaGrupo = Nothing    ' Nullable(Of Long)
        _IdPerfilFacturacion = Nothing  ' Nullable(Of Long)
        _FechaDesde = Nothing   ' Nullable(Of Date)
        _FechaHasta = Nothing   ' Nullable(Of Date)
        _Aviso = Nothing    ' Nullable(Of Boolean)
        _IdContratoOnlyReport = Nothing   ' Long?
    End Sub

    Public Function Difference(other As ContratoTarifa) As DifferenceDTO
        If other Is Nothing Then Throw New ArgumentException("other is null")
        If Not TypeOf other Is ContratoTarifa Then Throw New ArgumentException("Type of other invalid")
        Dim otherDTO As ContratoTarifa = CType(other, ContratoTarifa)
        Dim dif As New List(Of String)

        If Not IsEqual(Me.IdContratoTarifa, otherDTO.IdContratoTarifa) Then dif.Add("IdContratoTarifa")
        If Not IsEqual(Me.Entorno, otherDTO.Entorno) Then dif.Add("Entorno")
        If Not IsEqual(Me.CodigoContrato, otherDTO.CodigoContrato) Then dif.Add("CodigoContrato")
        If Not IsEqual(Me.IdTarifa, otherDTO.IdTarifa) Then dif.Add("IdTarifa")
        If Not IsEqual(Me.IdTarifaGrupo, otherDTO.IdTarifaGrupo) Then dif.Add("IdTarifaGrupo")
        If Not IsEqual(Me.IdPerfilFacturacion, otherDTO.IdPerfilFacturacion) Then dif.Add("IdPerfilFacturacion")
        If Not IsEqual(Me.FechaDesde, otherDTO.FechaDesde) Then dif.Add("FechaDesde")
        If Not IsEqual(Me.FechaHasta, otherDTO.FechaHasta) Then dif.Add("FechaHasta")
        If Not IsEqual(Me.Aviso, otherDTO.Aviso) Then dif.Add("Aviso")
        Return New DifferenceDTO(Me.GetType.ToString, Me.IDEntityDTO, dif)
    End Function

    Public Overridable Function GetClone() As ContratoTarifa
        Dim clone As New ContratoTarifa
        clone.IdContratoTarifa = _IdContratoTarifa
        clone.Entorno = _Entorno
        clone.CodigoContrato = _CodigoContrato
        clone.IdTarifa = _IdTarifa
        clone.IdTarifaGrupo = _IdTarifaGrupo
        clone.IdPerfilFacturacion = _IdPerfilFacturacion
        clone.FechaDesde = _FechaDesde
        clone.FechaHasta = _FechaHasta
        clone.Aviso = _Aviso
        Return clone
    End Function

    Public Overloads Function Equals(other As ContratoTarifa) As Boolean _
     Implements IEquatable(Of ContratoTarifa).Equals
        If other Is Nothing Then Return False
        If Object.ReferenceEquals(Me, other) Then Return True
        Dim ret As Boolean = True
        ret = ret AndAlso IsEqual(Me.IdContratoTarifa, other.IdContratoTarifa)
        ret = ret AndAlso IsEqual(Me.Entorno, other.Entorno)
        ret = ret AndAlso IsEqual(Me.CodigoContrato, other.CodigoContrato)
        ret = ret AndAlso IsEqual(Me.IdTarifa, other.IdTarifa)
        ret = ret AndAlso IsEqual(Me.IdTarifaGrupo, other.IdTarifaGrupo)
        ret = ret AndAlso IsEqual(Me.IdPerfilFacturacion, other.IdPerfilFacturacion)
        ret = ret AndAlso IsEqual(Me.FechaDesde, other.FechaDesde)
        ret = ret AndAlso IsEqual(Me.FechaHasta, other.FechaHasta)
        ret = ret AndAlso IsEqual(Me.Aviso, other.Aviso)
        Return ret
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then Return False
        Dim DTOObj As ContratoTarifa = TryCast(obj, ContratoTarifa)
        If DTOObj Is Nothing Then
            Return False
        Else
            Return Equals(DTOObj)
        End If
    End Function

    Public Shared Operator <>(DTO1 As ContratoTarifa, DTO2 As ContratoTarifa) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Object.Equals(DTO1, DTO2)
        End If
        Return DTO1.Equals(DTO2)
    End Operator

    Public Shared Operator =(DTO1 As ContratoTarifa, DTO2 As ContratoTarifa) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Not Object.Equals(DTO1, DTO2)
        End If
        Return Not DTO1.Equals(DTO2)
    End Operator


    Public Overrides Function GetHashCode() As Integer
        Dim ret As String = String.Empty
        If Not IsNothing(Me.IdContratoTarifa) Then ret = ret & Me.IdContratoTarifa.GetHashCode().ToString()
        If Not IsNothing(Me.Entorno) Then ret = ret & Me.Entorno.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoContrato) Then ret = ret & Me.CodigoContrato.GetHashCode().ToString()
        If Not IsNothing(Me.IdTarifa) Then ret = ret & Me.IdTarifa.GetHashCode().ToString()
        If Not IsNothing(Me.IdTarifaGrupo) Then ret = ret & Me.IdTarifaGrupo.GetHashCode().ToString()
        If Not IsNothing(Me.IdPerfilFacturacion) Then ret = ret & Me.IdPerfilFacturacion.GetHashCode().ToString()
        If Not IsNothing(Me.FechaDesde) Then ret = ret & Me.FechaDesde.GetHashCode().ToString()
        If Not IsNothing(Me.FechaHasta) Then ret = ret & Me.FechaHasta.GetHashCode().ToString()
        If Not IsNothing(Me.Aviso) Then ret = ret & Me.Aviso.GetHashCode().ToString()
        Return ret.GetHashCode()
    End Function

    Public Property IDEntityDTO As Long
        Get
            Return IdContratoTarifa
        End Get
        Set(value As Long)
            IdContratoTarifa = value
        End Set
    End Property

    Protected Function IsEqual(a As Object, b As Object) As Boolean
        Dim ret As Boolean = False

        Dim tipo As System.Type = Nothing
        If Not IsNothing(a) Then
            tipo = a.GetType()
        End If
        If IsNothing(tipo) Then
            If IsNothing(b) Then
                Return True
            Else
                tipo = b.GetType()
            End If
        Else
            If Not IsNothing(b) AndAlso Not (tipo Is b.GetType) Then
                Return False
            End If
        End If

        Select Case tipo
            Case GetType(Int16), GetType(Int32), GetType(Int64), GetType(Decimal)
                If IsNothing(a) Then
                    ret = (Convert.ToDecimal(b) = 0)
                Else
                    If IsNothing(b) Then
                        ret = (Convert.ToDecimal(a) = 0)
                    Else
                        ret = (Convert.ToDecimal(a) = Convert.ToDecimal(b))
                    End If
                End If
            Case GetType(String)
                If IsNothing(a) Then
                    ret = b.Equals(String.Empty)
                Else
                    If IsNothing(b) Then
                        ret = a.Equals(String.Empty)
                    Else
                        ret = a.Equals(b)
                    End If
                End If
            Case GetType(Boolean)
                If IsNothing(a) Then
                    ret = b.Equals(False)
                Else
                    If IsNothing(b) Then
                        ret = a.Equals(False)
                    Else
                        ret = a.Equals(b)
                    End If
                End If
            Case GetType(Byte())
                If IsNothing(a) Then
                    ret = b.Equals(Nothing)
                Else
                    If IsNothing(b) Then
                        ret = a.Equals(Nothing)
                    Else
                        ret = a.Equals(b)
                    End If
                End If
            Case GetType(Date)
                If IsNothing(a) Then
                    ret = b.Equals(Nothing)
                Else
                    If IsNothing(b) Then
                        ret = a.Equals(Nothing)
                    Else
                        ret = a.Equals(b)
                    End If
                End If
            Case Else
                ret = False
        End Select
        Return ret
    End Function

    Private _IdContratoTarifa As Long
    Public Property IdContratoTarifa As Long
        Get
            Return _IdContratoTarifa
        End Get
        Set(ByVal value As Long)
            _IdContratoTarifa = value
        End Set
    End Property

    Private _Entorno As String
    Public Property Entorno As String
        Get
            Return _Entorno
        End Get
        Set(ByVal value As String)
            _Entorno = value
        End Set
    End Property

    Private _CodigoContrato As Nullable(Of Long)
    Public Property CodigoContrato As Nullable(Of Long)
        Get
            Return _CodigoContrato
        End Get
        Set(ByVal value As Nullable(Of Long))
            _CodigoContrato = value
        End Set
    End Property




    Private _IdTarifa As Nullable(Of Long)
    Public Property IdTarifa As Nullable(Of Long)
        Get
            Return _IdTarifa
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTarifa = value
        End Set
    End Property

    Private _IdTarifaGrupo As Nullable(Of Long)
    Public Property IdTarifaGrupo As Nullable(Of Long)
        Get
            Return _IdTarifaGrupo
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTarifaGrupo = value
        End Set
    End Property

    Private _IdPerfilFacturacion As Nullable(Of Long)
    Public Property IdPerfilFacturacion As Nullable(Of Long)
        Get
            Return _IdPerfilFacturacion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdPerfilFacturacion = value
        End Set
    End Property

    Private _FechaDesde As Nullable(Of Date)

    Public Property FechaDesde As Nullable(Of Date)
        Get
            Return _FechaDesde
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaDesde = value
        End Set
    End Property

    Private _FechaHasta As Nullable(Of Date)
    Public Property FechaHasta As Nullable(Of Date)
        Get
            Return _FechaHasta
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaHasta = value
        End Set
    End Property

    Private _Aviso As Nullable(Of Boolean)
    Public Property Aviso As Nullable(Of Boolean)
        Get
            Return _Aviso
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _Aviso = value
        End Set
    End Property

    Private _IdContratoOnlyReport As Long?
    Public Property IdContratoOnlyReport As Long?
        Get
            Return _IdContratoOnlyReport
        End Get
        Set(ByVal value As Long?)
            _IdContratoOnlyReport = value
        End Set
    End Property
End Class
Partial Class ContratoTarifa
    Private _TextoTarifa As String
    Public Property TextoTarifa As String
        Get
            Return _TextoTarifa
        End Get
        Set(ByVal value As String)
            _TextoTarifa = value
        End Set
    End Property

    Private _textotarifagrupo As String
    Public Property textotarifagrupo As String
        Get
            Return _textotarifagrupo
        End Get
        Set(ByVal value As String)
            _textotarifagrupo = value
        End Set
    End Property
    Private _TextoPerfilFacturacion As String
    Public Property TextoPerfilFacturacion As String
        Get
            Return _TextoPerfilFacturacion
        End Get
        Set(ByVal value As String)
            _TextoPerfilFacturacion = value
        End Set
    End Property
    Private _PerfilFacturacion As PerfilFacturacion
    <DataMember()> Public Property PerfilFacturacion As PerfilFacturacion
        Get
            Return _PerfilFacturacion
        End Get
        Set(value As PerfilFacturacion)
            _PerfilFacturacion = value
        End Set
    End Property

    Private _textotarifagrupoNuevo As String
    Public Property textotarifagrupoNuevo As String
        Get
            Return _textotarifagrupoNuevo
        End Get
        Set(ByVal value As String)
            _textotarifagrupoNuevo = value
        End Set
    End Property
    Private _textotarifagrupoViejo As String
    Public Property textotarifagrupoViejo As String
        Get
            Return _textotarifagrupoViejo
        End Get
        Set(ByVal value As String)
            _textotarifagrupoViejo = value
        End Set
    End Property

    Private _IsQ As Boolean?
    Public Property IsQ As Boolean?
        Get
            Return _IsQ
        End Get
        Set(ByVal value As Boolean?)
            _IsQ = value
        End Set
    End Property

    Private _MantenerPerfil As Boolean?
    Public Property MantenerPerfil As Boolean?
        Get
            Return _MantenerPerfil
        End Get
        Set(ByVal value As Boolean?)
            _MantenerPerfil = value
        End Set
    End Property

    Private _FechaAplicar As Nullable(Of Date)
    Public Property FechaAplicar As Nullable(Of Date)
        Get
            Return _FechaAplicar
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaAplicar = value
        End Set
    End Property
End Class