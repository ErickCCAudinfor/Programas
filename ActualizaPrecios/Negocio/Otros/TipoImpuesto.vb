Public Class TipoImpuesto
    Inherits BaseGenericDTO
    Implements IEquatable(Of TipoImpuesto)

    Public Sub New()
        _IdTipoImpuesto = 0 ' Long
        _Entorno = String.Empty ' String
        _TextoImpuesto = String.Empty   ' String
        _Porcentaje = Nothing   ' Nullable(Of Decimal)
        _RecargoEquiv = Nothing ' Nullable(Of Decimal)
        _PorcentajeAlquiler = Nothing   ' Nullable(Of Decimal)
        _PorcentajeServicios = Nothing  ' Nullable(Of Decimal)
        _CodigoComunicacion = String.Empty  ' String
    End Sub

    Public Function Difference(other As TipoImpuesto) As DifferenceDTO
        If other Is Nothing Then Throw New ArgumentException("other is null")
        If Not TypeOf other Is TipoImpuesto Then Throw New ArgumentException("Type of other invalid")
        Dim otherDTO As TipoImpuesto = CType(other, TipoImpuesto)
        Dim dif As New List(Of String)

        If Not IsEqual(Me.IdTipoImpuesto, otherDTO.IdTipoImpuesto) Then dif.Add("IdTipoImpuesto")
        If Not IsEqual(Me.Entorno, otherDTO.Entorno) Then dif.Add("Entorno")
        If Not IsEqual(Me.TextoImpuesto, otherDTO.TextoImpuesto) Then dif.Add("TextoImpuesto")
        If Not IsEqual(Me.Porcentaje, otherDTO.Porcentaje) Then dif.Add("Porcentaje")
        If Not IsEqual(Me.RecargoEquiv, otherDTO.RecargoEquiv) Then dif.Add("RecargoEquiv")
        If Not IsEqual(Me.PorcentajeAlquiler, otherDTO.PorcentajeAlquiler) Then dif.Add("PorcentajeAlquiler")
        If Not IsEqual(Me.PorcentajeServicios, otherDTO.PorcentajeServicios) Then dif.Add("PorcentajeServicios")
        If Not IsEqual(Me.CodigoComunicacion, otherDTO.CodigoComunicacion) Then dif.Add("CodigoComunicacion")
        Return New DifferenceDTO(Me.GetType.ToString, Me.IDEntityDTO, dif)
    End Function

    Public Overridable Function GetClone() As TipoImpuesto
        Dim clone As New TipoImpuesto
        clone.IdTipoImpuesto = _IdTipoImpuesto
        clone.Entorno = _Entorno
        clone.TextoImpuesto = _TextoImpuesto
        clone.Porcentaje = _Porcentaje
        clone.RecargoEquiv = _RecargoEquiv
        clone.PorcentajeAlquiler = _PorcentajeAlquiler
        clone.PorcentajeServicios = _PorcentajeServicios
        clone.CodigoComunicacion = _CodigoComunicacion
        Return clone
    End Function

    Public Overloads Function Equals(other As TipoImpuesto) As Boolean _
     Implements IEquatable(Of TipoImpuesto).Equals
        If other Is Nothing Then Return False
        If Object.ReferenceEquals(Me, other) Then Return True
        Dim ret As Boolean = True
        ret = ret AndAlso IsEqual(Me.IdTipoImpuesto, other.IdTipoImpuesto)
        ret = ret AndAlso IsEqual(Me.Entorno, other.Entorno)
        ret = ret AndAlso IsEqual(Me.TextoImpuesto, other.TextoImpuesto)
        ret = ret AndAlso IsEqual(Me.Porcentaje, other.Porcentaje)
        ret = ret AndAlso IsEqual(Me.RecargoEquiv, other.RecargoEquiv)
        ret = ret AndAlso IsEqual(Me.PorcentajeAlquiler, other.PorcentajeAlquiler)
        ret = ret AndAlso IsEqual(Me.PorcentajeServicios, other.PorcentajeServicios)
        ret = ret AndAlso IsEqual(Me.CodigoComunicacion, other.CodigoComunicacion)
        Return ret
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then Return False
        Dim DTOObj As TipoImpuesto = TryCast(obj, TipoImpuesto)
        If DTOObj Is Nothing Then
            Return False
        Else
            Return Equals(DTOObj)
        End If
    End Function

    Public Shared Operator <>(DTO1 As TipoImpuesto, DTO2 As TipoImpuesto) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Object.Equals(DTO1, DTO2)
        End If
        Return DTO1.Equals(DTO2)
    End Operator

    Public Shared Operator =(DTO1 As TipoImpuesto, DTO2 As TipoImpuesto) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Not Object.Equals(DTO1, DTO2)
        End If
        Return Not DTO1.Equals(DTO2)
    End Operator


    Public Overrides Function GetHashCode() As Integer
        Dim ret As String = String.Empty
        If Not IsNothing(Me.IdTipoImpuesto) Then ret = ret & Me.IdTipoImpuesto.GetHashCode().ToString()
        If Not IsNothing(Me.Entorno) Then ret = ret & Me.Entorno.GetHashCode().ToString()
        If Not IsNothing(Me.TextoImpuesto) Then ret = ret & Me.TextoImpuesto.GetHashCode().ToString()
        If Not IsNothing(Me.Porcentaje) Then ret = ret & Me.Porcentaje.GetHashCode().ToString()
        If Not IsNothing(Me.RecargoEquiv) Then ret = ret & Me.RecargoEquiv.GetHashCode().ToString()
        If Not IsNothing(Me.PorcentajeAlquiler) Then ret = ret & Me.PorcentajeAlquiler.GetHashCode().ToString()
        If Not IsNothing(Me.PorcentajeServicios) Then ret = ret & Me.PorcentajeServicios.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoComunicacion) Then ret = ret & Me.CodigoComunicacion.GetHashCode().ToString()
        Return ret.GetHashCode()
    End Function

    Public Property IDEntityDTO As Long
        Get
            Return IdTipoImpuesto
        End Get
        Set(value As Long)
            IdTipoImpuesto = value
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

    Private _IdTipoImpuesto As Long
    Public Property IdTipoImpuesto As Long
        Get
            Return _IdTipoImpuesto
        End Get
        Set(ByVal value As Long)
            _IdTipoImpuesto = value
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

    Private _TextoImpuesto As String
    Public Property TextoImpuesto As String
        Get
            Return _TextoImpuesto
        End Get
        Set(ByVal value As String)
            _TextoImpuesto = value
        End Set
    End Property

    Private _Porcentaje As Nullable(Of Decimal)
    Public Property Porcentaje As Nullable(Of Decimal)
        Get
            Return _Porcentaje
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _Porcentaje = value
        End Set
    End Property

    Private _RecargoEquiv As Nullable(Of Decimal)
    Public Property RecargoEquiv As Nullable(Of Decimal)
        Get
            Return _RecargoEquiv
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _RecargoEquiv = value
        End Set
    End Property

    Private _PorcentajeAlquiler As Nullable(Of Decimal)
    Public Property PorcentajeAlquiler As Nullable(Of Decimal)
        Get
            Return _PorcentajeAlquiler
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PorcentajeAlquiler = value
        End Set
    End Property

    Private _PorcentajeServicios As Nullable(Of Decimal)
    Public Property PorcentajeServicios As Nullable(Of Decimal)
        Get
            Return _PorcentajeServicios
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PorcentajeServicios = value
        End Set
    End Property

    Private _CodigoComunicacion As String
    Public Property CodigoComunicacion As String
        Get
            Return _CodigoComunicacion
        End Get
        Set(ByVal value As String)
            _CodigoComunicacion = value
        End Set
    End Property
End Class
