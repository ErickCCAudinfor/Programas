Public Class CNAE
    Inherits BaseGenericDTO
    Implements IEquatable(Of CNAE)

    Public Sub New()
        _IdCNAE = 0 ' Long
        _Entorno = String.Empty ' String
        _CodigoCNAE = String.Empty  ' String
        _TextoCNAE = String.Empty   ' String
        _CodigoAgrupacion = String.Empty    ' String
        _DescripcionCodigoAgrupacion = String.Empty ' String
    End Sub

    Public Function Difference(other As CNAE) As DifferenceDTO
        If other Is Nothing Then Throw New ArgumentException("other is null")
        If Not TypeOf other Is CNAE Then Throw New ArgumentException("Type of other invalid")
        Dim otherDTO As CNAE = CType(other, CNAE)
        Dim dif As New List(Of String)

        If Not IsEqual(Me.IdCNAE, otherDTO.IdCNAE) Then dif.Add("IdCNAE")
        If Not IsEqual(Me.Entorno, otherDTO.Entorno) Then dif.Add("Entorno")
        If Not IsEqual(Me.CodigoCNAE, otherDTO.CodigoCNAE) Then dif.Add("CodigoCNAE")
        If Not IsEqual(Me.TextoCNAE, otherDTO.TextoCNAE) Then dif.Add("TextoCNAE")
        If Not IsEqual(Me.CodigoAgrupacion, otherDTO.CodigoAgrupacion) Then dif.Add("CodigoAgrupacion")
        If Not IsEqual(Me.DescripcionCodigoAgrupacion, otherDTO.DescripcionCodigoAgrupacion) Then dif.Add("DescripcionCodigoAgrupacion")
        Return New DifferenceDTO(Me.GetType.ToString, Me.IDEntityDTO, dif)
    End Function

    Public Overridable Function GetClone() As CNAE
        Dim clone As New CNAE
        clone.IdCNAE = _IdCNAE
        clone.Entorno = _Entorno
        clone.CodigoCNAE = _CodigoCNAE
        clone.TextoCNAE = _TextoCNAE
        clone.CodigoAgrupacion = _CodigoAgrupacion
        clone.DescripcionCodigoAgrupacion = _DescripcionCodigoAgrupacion
        Return clone
    End Function

    Public Overloads Function Equals(other As CNAE) As Boolean _
     Implements IEquatable(Of CNAE).Equals
        If other Is Nothing Then Return False
        If Object.ReferenceEquals(Me, other) Then Return True
        Dim ret As Boolean = True
        ret = ret AndAlso IsEqual(Me.IdCNAE, other.IdCNAE)
        ret = ret AndAlso IsEqual(Me.Entorno, other.Entorno)
        ret = ret AndAlso IsEqual(Me.CodigoCNAE, other.CodigoCNAE)
        ret = ret AndAlso IsEqual(Me.TextoCNAE, other.TextoCNAE)
        ret = ret AndAlso IsEqual(Me.CodigoAgrupacion, other.CodigoAgrupacion)
        ret = ret AndAlso IsEqual(Me.DescripcionCodigoAgrupacion, other.DescripcionCodigoAgrupacion)
        Return ret
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then Return False
        Dim DTOObj As CNAE = TryCast(obj, CNAE)
        If DTOObj Is Nothing Then
            Return False
        Else
            Return Equals(DTOObj)
        End If
    End Function

    Public Shared Operator <>(DTO1 As CNAE, DTO2 As CNAE) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Object.Equals(DTO1, DTO2)
        End If
        Return DTO1.Equals(DTO2)
    End Operator

    Public Shared Operator =(DTO1 As CNAE, DTO2 As CNAE) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Not Object.Equals(DTO1, DTO2)
        End If
        Return Not DTO1.Equals(DTO2)
    End Operator


    Public Overrides Function GetHashCode() As Integer
        Dim ret As String = String.Empty
        If Not IsNothing(Me.IdCNAE) Then ret = ret & Me.IdCNAE.GetHashCode().ToString()
        If Not IsNothing(Me.Entorno) Then ret = ret & Me.Entorno.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoCNAE) Then ret = ret & Me.CodigoCNAE.GetHashCode().ToString()
        If Not IsNothing(Me.TextoCNAE) Then ret = ret & Me.TextoCNAE.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoAgrupacion) Then ret = ret & Me.CodigoAgrupacion.GetHashCode().ToString()
        If Not IsNothing(Me.DescripcionCodigoAgrupacion) Then ret = ret & Me.DescripcionCodigoAgrupacion.GetHashCode().ToString()
        Return ret.GetHashCode()
    End Function

    Public Property IDEntityDTO As Long
        Get
            Return IdCNAE
        End Get
        Set(value As Long)
            IdCNAE = value
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

    Private _IdCNAE As Long
    Public Property IdCNAE As Long
        Get
            Return _IdCNAE
        End Get
        Set(ByVal value As Long)
            _IdCNAE = value
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

    Private _CodigoCNAE As String
    Public Property CodigoCNAE As String
        Get
            Return _CodigoCNAE
        End Get
        Set(ByVal value As String)
            _CodigoCNAE = value
        End Set
    End Property

    Private _TextoCNAE As String
    Public Property TextoCNAE As String
        Get
            Return _TextoCNAE
        End Get
        Set(ByVal value As String)
            _TextoCNAE = value
        End Set
    End Property

    Private _CodigoAgrupacion As String
    Public Property CodigoAgrupacion As String
        Get
            Return _CodigoAgrupacion
        End Get
        Set(ByVal value As String)
            _CodigoAgrupacion = value
        End Set
    End Property

    Private _DescripcionCodigoAgrupacion As String
    Public Property DescripcionCodigoAgrupacion As String
        Get
            Return _DescripcionCodigoAgrupacion
        End Get
        Set(ByVal value As String)
            _DescripcionCodigoAgrupacion = value
        End Set
    End Property



End Class
