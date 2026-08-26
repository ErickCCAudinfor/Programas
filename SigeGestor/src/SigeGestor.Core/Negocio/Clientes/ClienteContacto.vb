Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Public Class ClienteContacto
    Inherits BaseGenericDTO
    Implements IEquatable(Of ClienteContacto)

    Public Sub New()
        _TipoContacto = String.Empty    ' String
        _Valor = String.Empty   ' String
        _IdCliente = 0  ' Long
        _IdClienteContacto = 0  ' Long
        _Entorno = String.Empty ' String
        _Contacto = String.Empty    ' String
        _Departamento = String.Empty    ' String
        _PorDefecto = Nothing   ' Nullable(Of Boolean)
    End Sub

    Public Function Difference(other As ClienteContacto) As DifferenceDTO
        If other Is Nothing Then Throw New ArgumentException("other is null")
        If Not TypeOf other Is ClienteContacto Then Throw New ArgumentException("Type of other invalid")
        Dim otherDTO As ClienteContacto = CType(other, ClienteContacto)
        Dim dif As New List(Of String)

        If Not IsEqual(Me.IdClienteContacto, otherDTO.IdClienteContacto) Then dif.Add("IdClienteContacto")
        If Not IsEqual(Me.Entorno, otherDTO.Entorno) Then dif.Add("Entorno")
        If Not IsEqual(Me.IdCliente, otherDTO.IdCliente) Then dif.Add("IdCliente")
        If Not IsEqual(Me.TipoContacto, otherDTO.TipoContacto) Then dif.Add("TipoContacto")
        If Not IsEqual(Me.Valor, otherDTO.Valor) Then dif.Add("Valor")
        If Not IsEqual(Me.Contacto, otherDTO.Contacto) Then dif.Add("Contacto")
        If Not IsEqual(Me.Departamento, otherDTO.Departamento) Then dif.Add("Departamento")
        If Not IsEqual(Me.PorDefecto, otherDTO.PorDefecto) Then dif.Add("PorDefecto")
        Return New DifferenceDTO(Me.GetType.ToString, Me.IDEntityDTO, dif)
    End Function

    Public Overridable Function GetClone() As ClienteContacto
        Dim clone As New ClienteContacto
        clone.IdClienteContacto = _IdClienteContacto
        clone.Entorno = _Entorno
        clone.IdCliente = _IdCliente
        clone.TipoContacto = _TipoContacto
        clone.Valor = _Valor
        clone.Contacto = _Contacto
        clone.Departamento = _Departamento
        clone.PorDefecto = _PorDefecto
        Return clone
    End Function

    Public Overloads Function Equals(other As ClienteContacto) As Boolean _
     Implements IEquatable(Of ClienteContacto).Equals
        If other Is Nothing Then Return False
        If Object.ReferenceEquals(Me, other) Then Return True
        Dim ret As Boolean = True
        ret = ret AndAlso IsEqual(Me.IdClienteContacto, other.IdClienteContacto)
        ret = ret AndAlso IsEqual(Me.Entorno, other.Entorno)
        ret = ret AndAlso IsEqual(Me.IdCliente, other.IdCliente)
        ret = ret AndAlso IsEqual(Me.TipoContacto, other.TipoContacto)
        ret = ret AndAlso IsEqual(Me.Valor, other.Valor)
        ret = ret AndAlso IsEqual(Me.Contacto, other.Contacto)
        ret = ret AndAlso IsEqual(Me.Departamento, other.Departamento)
        ret = ret AndAlso IsEqual(Me.PorDefecto, other.PorDefecto)
        Return ret
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then Return False
        Dim DTOObj As ClienteContacto = TryCast(obj, ClienteContacto)
        If DTOObj Is Nothing Then
            Return False
        Else
            Return Equals(DTOObj)
        End If
    End Function

    Public Shared Operator <>(DTO1 As ClienteContacto, DTO2 As ClienteContacto) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Object.Equals(DTO1, DTO2)
        End If
        Return DTO1.Equals(DTO2)
    End Operator

    Public Shared Operator =(DTO1 As ClienteContacto, DTO2 As ClienteContacto) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Not Object.Equals(DTO1, DTO2)
        End If
        Return Not DTO1.Equals(DTO2)
    End Operator


    Public Overrides Function GetHashCode() As Integer
        Dim ret As String = String.Empty
        If Not IsNothing(Me.IdClienteContacto) Then ret = ret & Me.IdClienteContacto.GetHashCode().ToString()
        If Not IsNothing(Me.Entorno) Then ret = ret & Me.Entorno.GetHashCode().ToString()
        If Not IsNothing(Me.IdCliente) Then ret = ret & Me.IdCliente.GetHashCode().ToString()
        If Not IsNothing(Me.TipoContacto) Then ret = ret & Me.TipoContacto.GetHashCode().ToString()
        If Not IsNothing(Me.Valor) Then ret = ret & Me.Valor.GetHashCode().ToString()
        If Not IsNothing(Me.Contacto) Then ret = ret & Me.Contacto.GetHashCode().ToString()
        If Not IsNothing(Me.Departamento) Then ret = ret & Me.Departamento.GetHashCode().ToString()
        If Not IsNothing(Me.PorDefecto) Then ret = ret & Me.PorDefecto.GetHashCode().ToString()
        Return ret.GetHashCode()
    End Function

    Public Property IDEntityDTO As Long
        Get
            Return IdClienteContacto
        End Get
        Set(value As Long)
            IdClienteContacto = value
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

    Private _TipoContacto As String
    Public Property TipoContacto As String
        Get
            Return _TipoContacto
        End Get
        Set(ByVal value As String)
            _TipoContacto = value
        End Set
    End Property

    Private _Valor As String
    Public Property Valor As String
        Get
            Return _Valor
        End Get
        Set(ByVal value As String)
            _Valor = value
        End Set
    End Property

    Private _IdCliente As Long
    Public Property IdCliente As Long
        Get
            Return _IdCliente
        End Get
        Set(ByVal value As Long)
            _IdCliente = value
        End Set
    End Property

    Private _IdClienteContacto As Long
    Public Property IdClienteContacto As Long
        Get
            Return _IdClienteContacto
        End Get
        Set(ByVal value As Long)
            _IdClienteContacto = value
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

    Private _Contacto As String
    Public Property Contacto As String
        Get
            Return _Contacto
        End Get
        Set(ByVal value As String)
            _Contacto = value
        End Set
    End Property

    Private _Departamento As String
    Public Property Departamento As String
        Get
            Return _Departamento
        End Get
        Set(ByVal value As String)
            _Departamento = value
        End Set
    End Property

    Private _PorDefecto As Nullable(Of Boolean)
    Public Property PorDefecto As Nullable(Of Boolean)
        Get
            Return _PorDefecto
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _PorDefecto = value
        End Set
    End Property
End Class
