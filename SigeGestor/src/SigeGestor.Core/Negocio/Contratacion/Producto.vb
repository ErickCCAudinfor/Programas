Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.

Public Class Producto
    Inherits BaseGenericDTO
    Implements IEquatable(Of Producto)

    Public Sub New()
        _IdProducto = 0 ' Long
        _Entorno = String.Empty ' String
        _IdProductoGrupo = Nothing  ' Nullable(Of Long)
        _TextoProducto = String.Empty   ' String
        _Importe = Nothing  ' Nullable(Of Decimal)
        _Descuento = Nothing    ' Nullable(Of Decimal)
        _AntesIE = Nothing  ' Nullable(Of Boolean)
        _IdTipoImpuesto = Nothing   ' Nullable(Of Long)
        _IdSCSCuenta = Nothing  ' Nullable(Of Long)
        _RefExternaExport = String.Empty    ' String
        _RefExternaImport = String.Empty    ' String
        _CodigoComunicacion = String.Empty  ' String
        _SobreConsumo = Nothing ' Nullable(Of Boolean)
        _PrecioDia = Nothing    ' Nullable(Of Boolean)
        _PrecioSobreConsumo = Nothing   ' Nullable(Of Boolean)
        _AplicarRenovacion = Nothing    ' Nullable(Of Boolean)
        _IsRecalcular = Nothing ' Nullable(Of Boolean)
        _IsAplicarFecha = Nothing   ' Nullable(Of Boolean)
    End Sub

    Public Function Difference(other As Producto) As DifferenceDTO
        If other Is Nothing Then Throw New ArgumentException("other is null")
        If Not TypeOf other Is Producto Then Throw New ArgumentException("Type of other invalid")
        Dim otherDTO As Producto = CType(other, Producto)
        Dim dif As New List(Of String)

        If Not IsEqual(Me.IdProducto, otherDTO.IdProducto) Then dif.Add("IdProducto")
        If Not IsEqual(Me.Entorno, otherDTO.Entorno) Then dif.Add("Entorno")
        If Not IsEqual(Me.IdProductoGrupo, otherDTO.IdProductoGrupo) Then dif.Add("IdProductoGrupo")
        If Not IsEqual(Me.TextoProducto, otherDTO.TextoProducto) Then dif.Add("TextoProducto")
        If Not IsEqual(Me.Importe, otherDTO.Importe) Then dif.Add("Importe")
        If Not IsEqual(Me.Descuento, otherDTO.Descuento) Then dif.Add("Descuento")
        If Not IsEqual(Me.AntesIE, otherDTO.AntesIE) Then dif.Add("AntesIE")
        If Not IsEqual(Me.IdTipoImpuesto, otherDTO.IdTipoImpuesto) Then dif.Add("IdTipoImpuesto")
        If Not IsEqual(Me.IdSCSCuenta, otherDTO.IdSCSCuenta) Then dif.Add("IdSCSCuenta")
        If Not IsEqual(Me.RefExternaExport, otherDTO.RefExternaExport) Then dif.Add("RefExternaExport")
        If Not IsEqual(Me.RefExternaImport, otherDTO.RefExternaImport) Then dif.Add("RefExternaImport")
        If Not IsEqual(Me.CodigoComunicacion, otherDTO.CodigoComunicacion) Then dif.Add("CodigoComunicacion")
        If Not IsEqual(Me.SobreConsumo, otherDTO.SobreConsumo) Then dif.Add("SobreConsumo")
        If Not IsEqual(Me.PrecioDia, otherDTO.PrecioDia) Then dif.Add("PrecioDia")
        If Not IsEqual(Me.PrecioSobreConsumo, otherDTO.PrecioSobreConsumo) Then dif.Add("PrecioSobreConsumo")
        If Not IsEqual(Me.AplicarRenovacion, otherDTO.AplicarRenovacion) Then dif.Add("AplicarRenovacion")
        If Not IsEqual(Me.IsRecalcular, otherDTO.IsRecalcular) Then dif.Add("IsRecalcular")
        If Not IsEqual(Me.IsAplicarFecha, otherDTO.IsAplicarFecha) Then dif.Add("IsAplicarFecha")
        Return New DifferenceDTO(Me.GetType.ToString, Me.IDEntityDTO, dif)
    End Function

    Public Overridable Function GetClone() As Producto
        Dim clone As New Producto
        clone.IdProducto = _IdProducto
        clone.Entorno = _Entorno
        clone.IdProductoGrupo = _IdProductoGrupo
        clone.TextoProducto = _TextoProducto
        clone.Importe = _Importe
        clone.Descuento = _Descuento
        clone.AntesIE = _AntesIE
        clone.IdTipoImpuesto = _IdTipoImpuesto
        clone.IdSCSCuenta = _IdSCSCuenta
        clone.RefExternaExport = _RefExternaExport
        clone.RefExternaImport = _RefExternaImport
        clone.CodigoComunicacion = _CodigoComunicacion
        clone.SobreConsumo = _SobreConsumo
        clone.PrecioDia = _PrecioDia
        clone.PrecioSobreConsumo = _PrecioSobreConsumo
        clone.AplicarRenovacion = _AplicarRenovacion
        clone.IsRecalcular = _IsRecalcular
        clone.IsAplicarFecha = _IsAplicarFecha
        Return clone
    End Function

    Public Overloads Function Equals(other As Producto) As Boolean _
     Implements IEquatable(Of Producto).Equals
        If other Is Nothing Then Return False
        If Object.ReferenceEquals(Me, other) Then Return True
        Dim ret As Boolean = True
        ret = ret AndAlso IsEqual(Me.IdProducto, other.IdProducto)
        ret = ret AndAlso IsEqual(Me.Entorno, other.Entorno)
        ret = ret AndAlso IsEqual(Me.IdProductoGrupo, other.IdProductoGrupo)
        ret = ret AndAlso IsEqual(Me.TextoProducto, other.TextoProducto)
        ret = ret AndAlso IsEqual(Me.Importe, other.Importe)
        ret = ret AndAlso IsEqual(Me.Descuento, other.Descuento)
        ret = ret AndAlso IsEqual(Me.AntesIE, other.AntesIE)
        ret = ret AndAlso IsEqual(Me.IdTipoImpuesto, other.IdTipoImpuesto)
        ret = ret AndAlso IsEqual(Me.IdSCSCuenta, other.IdSCSCuenta)
        ret = ret AndAlso IsEqual(Me.RefExternaExport, other.RefExternaExport)
        ret = ret AndAlso IsEqual(Me.RefExternaImport, other.RefExternaImport)
        ret = ret AndAlso IsEqual(Me.CodigoComunicacion, other.CodigoComunicacion)
        ret = ret AndAlso IsEqual(Me.SobreConsumo, other.SobreConsumo)
        ret = ret AndAlso IsEqual(Me.PrecioDia, other.PrecioDia)
        ret = ret AndAlso IsEqual(Me.PrecioSobreConsumo, other.PrecioSobreConsumo)
        ret = ret AndAlso IsEqual(Me.AplicarRenovacion, other.AplicarRenovacion)
        ret = ret AndAlso IsEqual(Me.IsRecalcular, other.IsRecalcular)
        ret = ret AndAlso IsEqual(Me.IsAplicarFecha, other.IsAplicarFecha)
        Return ret
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then Return False
        Dim DTOObj As Producto = TryCast(obj, Producto)
        If DTOObj Is Nothing Then
            Return False
        Else
            Return Equals(DTOObj)
        End If
    End Function

    Public Shared Operator <>(DTO1 As Producto, DTO2 As Producto) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Object.Equals(DTO1, DTO2)
        End If
        Return DTO1.Equals(DTO2)
    End Operator

    Public Shared Operator =(DTO1 As Producto, DTO2 As Producto) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Not Object.Equals(DTO1, DTO2)
        End If
        Return Not DTO1.Equals(DTO2)
    End Operator


    Public Overrides Function GetHashCode() As Integer
        Dim ret As String = String.Empty
        If Not IsNothing(Me.IdProducto) Then ret = ret & Me.IdProducto.GetHashCode().ToString()
        If Not IsNothing(Me.Entorno) Then ret = ret & Me.Entorno.GetHashCode().ToString()
        If Not IsNothing(Me.IdProductoGrupo) Then ret = ret & Me.IdProductoGrupo.GetHashCode().ToString()
        If Not IsNothing(Me.TextoProducto) Then ret = ret & Me.TextoProducto.GetHashCode().ToString()
        If Not IsNothing(Me.Importe) Then ret = ret & Me.Importe.GetHashCode().ToString()
        If Not IsNothing(Me.Descuento) Then ret = ret & Me.Descuento.GetHashCode().ToString()
        If Not IsNothing(Me.AntesIE) Then ret = ret & Me.AntesIE.GetHashCode().ToString()
        If Not IsNothing(Me.IdTipoImpuesto) Then ret = ret & Me.IdTipoImpuesto.GetHashCode().ToString()
        If Not IsNothing(Me.IdSCSCuenta) Then ret = ret & Me.IdSCSCuenta.GetHashCode().ToString()
        If Not IsNothing(Me.RefExternaExport) Then ret = ret & Me.RefExternaExport.GetHashCode().ToString()
        If Not IsNothing(Me.RefExternaImport) Then ret = ret & Me.RefExternaImport.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoComunicacion) Then ret = ret & Me.CodigoComunicacion.GetHashCode().ToString()
        If Not IsNothing(Me.SobreConsumo) Then ret = ret & Me.SobreConsumo.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioDia) Then ret = ret & Me.PrecioDia.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioSobreConsumo) Then ret = ret & Me.PrecioSobreConsumo.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarRenovacion) Then ret = ret & Me.AplicarRenovacion.GetHashCode().ToString()
        If Not IsNothing(Me.IsRecalcular) Then ret = ret & Me.IsRecalcular.GetHashCode().ToString()
        If Not IsNothing(Me.IsAplicarFecha) Then ret = ret & Me.IsAplicarFecha.GetHashCode().ToString()
        Return ret.GetHashCode()
    End Function

    Public Property IDEntityDTO As Long
        Get
            Return IdProducto
        End Get
        Set(value As Long)
            IdProducto = value
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

    Private _IdProducto As Long
    Public Property IdProducto As Long
        Get
            Return _IdProducto
        End Get
        Set(ByVal value As Long)
            _IdProducto = value
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

    Private _IdProductoGrupo As Nullable(Of Long)
    Public Property IdProductoGrupo As Nullable(Of Long)
        Get
            Return _IdProductoGrupo
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdProductoGrupo = value
        End Set
    End Property

    Private _TextoProducto As String
    Public Property TextoProducto As String
        Get
            Return _TextoProducto
        End Get
        Set(ByVal value As String)
            _TextoProducto = value
        End Set
    End Property

    Private _Importe As Nullable(Of Decimal)
    Public Property Importe As Nullable(Of Decimal)
        Get
            Return _Importe
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _Importe = value
        End Set
    End Property

    Private _Descuento As Nullable(Of Decimal)
    Public Property Descuento As Nullable(Of Decimal)
        Get
            Return _Descuento
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _Descuento = value
        End Set
    End Property

    Private _AntesIE As Nullable(Of Boolean)
    Public Property AntesIE As Nullable(Of Boolean)
        Get
            Return _AntesIE
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AntesIE = value
        End Set
    End Property

    Private _IdTipoImpuesto As Nullable(Of Long)
    Public Property IdTipoImpuesto As Nullable(Of Long)
        Get
            Return _IdTipoImpuesto
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoImpuesto = value
        End Set
    End Property

    Private _IdSCSCuenta As Nullable(Of Long)
    Public Property IdSCSCuenta As Nullable(Of Long)
        Get
            Return _IdSCSCuenta
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdSCSCuenta = value
        End Set
    End Property

    Private _RefExternaExport As String
    Public Property RefExternaExport As String
        Get
            Return _RefExternaExport
        End Get
        Set(ByVal value As String)
            _RefExternaExport = value
        End Set
    End Property

    Private _RefExternaImport As String
    Public Property RefExternaImport As String
        Get
            Return _RefExternaImport
        End Get
        Set(ByVal value As String)
            _RefExternaImport = value
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

    Private _SobreConsumo As Nullable(Of Boolean)
    Public Property SobreConsumo As Nullable(Of Boolean)
        Get
            Return _SobreConsumo
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _SobreConsumo = value
        End Set
    End Property

    Private _PrecioDia As Nullable(Of Boolean)
    Public Property PrecioDia As Nullable(Of Boolean)
        Get
            Return _PrecioDia
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _PrecioDia = value
        End Set
    End Property

    Private _PrecioSobreConsumo As Nullable(Of Boolean)
    Public Property PrecioSobreConsumo As Nullable(Of Boolean)
        Get
            Return _PrecioSobreConsumo
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _PrecioSobreConsumo = value
        End Set
    End Property

    Private _AplicarRenovacion As Nullable(Of Boolean)
    Public Property AplicarRenovacion As Nullable(Of Boolean)
        Get
            Return _AplicarRenovacion
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AplicarRenovacion = value
        End Set
    End Property

    Private _IsRecalcular As Nullable(Of Boolean)
    Public Property IsRecalcular As Nullable(Of Boolean)
        Get
            Return _IsRecalcular
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsRecalcular = value
        End Set
    End Property

    Private _IsAplicarFecha As Nullable(Of Boolean)
    Public Property IsAplicarFecha As Nullable(Of Boolean)
        Get
            Return _IsAplicarFecha
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsAplicarFecha = value
        End Set
    End Property

    Private _ProductoGrupo As ProductoGrupo
    Public Property ProductoGrupo As ProductoGrupo
        Get
            Return _ProductoGrupo
        End Get
        Set(value As ProductoGrupo)
            _ProductoGrupo = value
        End Set
    End Property

End Class
