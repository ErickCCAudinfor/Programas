Public Class ProductoAsignacion
    Public Sub New()
        _IdProductoAsignacion = 0   ' Long
        _Entorno = String.Empty ' String
        _IdProductoGrupo = Nothing  ' Nullable(Of Long)
        _IdProducto = Nothing   ' Nullable(Of Long)
        _TipoAsignacion = String.Empty  ' String
        _IdCliente = Nothing    ' Nullable(Of Long)
        _IdContrato = Nothing   ' Nullable(Of Long)
        _IdTarifa = Nothing ' Nullable(Of Long)
        _IdTarifaGrupo = Nothing    ' Nullable(Of Long)
        _IdTipoCobro = Nothing  ' Nullable(Of Long)
        _IdTarifaPeaje = Nothing    ' Nullable(Of Long)
        _Desde = Nothing    ' Nullable(Of Decimal)
        _Hasta = Nothing    ' Nullable(Of Decimal)
        _IsControlFecha = Nothing   ' Nullable(Of Boolean)
        _FechaInicial = Nothing ' Nullable(Of Date)
        _FechaFinal = Nothing   ' Nullable(Of Date)
        _Plazo = Nothing    ' Nullable(Of Integer)
        _PlazoCargado = Nothing ' Nullable(Of Integer)
        _ImporteTotalPlazo = Nothing    ' Nullable(Of Decimal)
        _IsFacturado = Nothing  ' Nullable(Of Boolean)
        _Importe = Nothing  ' Nullable(Of Decimal)
        _Descuento = Nothing    ' Nullable(Of Decimal)
        _AntesIE = Nothing  ' Nullable(Of Boolean)
        _IdTipoImpuesto = Nothing   ' Nullable(Of Long)
        _PrecioDia = Nothing    ' Nullable(Of Boolean)
        _IsFacturaProrrateo = False ' Boolean
        _IdFacturaProrrateo = Nothing   ' Nullable(Of Long)
        _PorcentajeIncremento = Nothing ' Nullable(Of Decimal)
        _AplicarSobreConsumo = False    ' Boolean
        _ImportePlazo = String.Empty    ' String
        _AplicarPrecioConsumo = False   ' Boolean
        _IsBonificacion = Nothing   ' Nullable(Of Boolean)
        _FechaAsignacion = Nothing  ' Nullable(Of Date)
    End Sub


    Private _IdProductoAsignacion As Long
    Public Property IdProductoAsignacion As Long
        Get
            Return _IdProductoAsignacion
        End Get
        Set(ByVal value As Long)
            _IdProductoAsignacion = value
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

    Private _IdProducto As Nullable(Of Long)
    Public Property IdProducto As Nullable(Of Long)
        Get
            Return _IdProducto
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdProducto = value
        End Set
    End Property

    Private _TipoAsignacion As String
    Public Property TipoAsignacion As String
        Get
            Return _TipoAsignacion
        End Get
        Set(ByVal value As String)
            _TipoAsignacion = value
        End Set
    End Property

    Private _IdCliente As Nullable(Of Long)
    Public Property IdCliente As Nullable(Of Long)
        Get
            Return _IdCliente
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdCliente = value
        End Set
    End Property

    Private _IdContrato As Nullable(Of Long)
    Public Property IdContrato As Nullable(Of Long)
        Get
            Return _IdContrato
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdContrato = value
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

    Private _IdTipoCobro As Nullable(Of Long)
    Public Property IdTipoCobro As Nullable(Of Long)
        Get
            Return _IdTipoCobro
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTipoCobro = value
        End Set
    End Property

    Private _IdTarifaPeaje As Nullable(Of Long)
    Public Property IdTarifaPeaje As Nullable(Of Long)
        Get
            Return _IdTarifaPeaje
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdTarifaPeaje = value
        End Set
    End Property

    Private _Desde As Nullable(Of Decimal)
    Public Property Desde As Nullable(Of Decimal)
        Get
            Return _Desde
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _Desde = value
        End Set
    End Property

    Private _Hasta As Nullable(Of Decimal)
    Public Property Hasta As Nullable(Of Decimal)
        Get
            Return _Hasta
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _Hasta = value
        End Set
    End Property

    Private _IsControlFecha As Nullable(Of Boolean)
    Public Property IsControlFecha As Nullable(Of Boolean)
        Get
            Return _IsControlFecha
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsControlFecha = value
        End Set
    End Property

    Private _FechaInicial As Nullable(Of Date)
    Public Property FechaInicial As Nullable(Of Date)
        Get
            Return _FechaInicial
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaInicial = value
        End Set
    End Property

    Private _FechaFinal As Nullable(Of Date)
    Public Property FechaFinal As Nullable(Of Date)
        Get
            Return _FechaFinal
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaFinal = value
        End Set
    End Property

    Private _Plazo As Nullable(Of Integer)
    Public Property Plazo As Nullable(Of Integer)
        Get
            Return _Plazo
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _Plazo = value
        End Set
    End Property

    Private _PlazoCargado As Nullable(Of Integer)
    Public Property PlazoCargado As Nullable(Of Integer)
        Get
            Return _PlazoCargado
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _PlazoCargado = value
        End Set
    End Property

    Private _ImporteTotalPlazo As Nullable(Of Decimal)
    Public Property ImporteTotalPlazo As Nullable(Of Decimal)
        Get
            Return _ImporteTotalPlazo
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _ImporteTotalPlazo = value
        End Set
    End Property

    Private _IsFacturado As Nullable(Of Boolean)
    Public Property IsFacturado As Nullable(Of Boolean)
        Get
            Return _IsFacturado
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsFacturado = value
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

    Private _PrecioDia As Nullable(Of Boolean)
    Public Property PrecioDia As Nullable(Of Boolean)
        Get
            Return _PrecioDia
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _PrecioDia = value
        End Set
    End Property

    Private _IsFacturaProrrateo As Boolean
    Public Property IsFacturaProrrateo As Boolean
        Get
            Return _IsFacturaProrrateo
        End Get
        Set(ByVal value As Boolean)
            _IsFacturaProrrateo = value
        End Set
    End Property

    Private _IdFacturaProrrateo As Nullable(Of Long)
    Public Property IdFacturaProrrateo As Nullable(Of Long)
        Get
            Return _IdFacturaProrrateo
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdFacturaProrrateo = value
        End Set
    End Property

    Private _PorcentajeIncremento As Nullable(Of Decimal)
    Public Property PorcentajeIncremento As Nullable(Of Decimal)
        Get
            Return _PorcentajeIncremento
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PorcentajeIncremento = value
        End Set
    End Property

    Private _AplicarSobreConsumo As Boolean
    Public Property AplicarSobreConsumo As Boolean
        Get
            Return _AplicarSobreConsumo
        End Get
        Set(ByVal value As Boolean)
            _AplicarSobreConsumo = value
        End Set
    End Property

    Private _ImportePlazo As String
    Public Property ImportePlazo As String
        Get
            Return _ImportePlazo
        End Get
        Set(ByVal value As String)
            _ImportePlazo = value
        End Set
    End Property

    Private _AplicarPrecioConsumo As Boolean
    Public Property AplicarPrecioConsumo As Boolean
        Get
            Return _AplicarPrecioConsumo
        End Get
        Set(ByVal value As Boolean)
            _AplicarPrecioConsumo = value
        End Set
    End Property

    Private _IsBonificacion As Nullable(Of Boolean)
    Public Property IsBonificacion As Nullable(Of Boolean)
        Get
            Return _IsBonificacion
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsBonificacion = value
        End Set
    End Property

    Private _FechaAsignacion As Nullable(Of Date)
    Public Property FechaAsignacion As Nullable(Of Date)
        Get
            Return _FechaAsignacion
        End Get
        Set(ByVal value As Nullable(Of Date))
            _FechaAsignacion = value
        End Set
    End Property

    Private _IsCliente As Boolean = False
    Public Property IsCliente As Boolean
        Get
            Return _IsCliente
        End Get
        Set(value As Boolean)
            _IsCliente = value
        End Set
    End Property

    Private _IsContrato As Boolean = False
    Public Property IsContrato As Boolean
        Get
            Return _IsContrato
        End Get
        Set(value As Boolean)
            _IsContrato = value
        End Set
    End Property

    Public ReadOnly Property Intervalos As String
        Get
            Dim ret As String = String.Empty
            ret += IIf(ret.Length > 0, " ", "").ToString + If(Desde.ToString, String.Empty)
            ret += IIf(ret.Length > 0, "-", "").ToString + If(Hasta.ToString(), String.Empty)
            Return ret
        End Get
    End Property


    Private _IsFacturaProrrateoFiltro As Boolean?
    Public Property IsFacturaProrrateoFiltro As Boolean?
        Get
            Return _IsFacturaProrrateoFiltro
        End Get
        Set(value As Boolean?)
            _IsFacturaProrrateoFiltro = value
        End Set
    End Property

    Private _IdCarteraCobro As Long = 0L
    Public Property IdCarteraCobro As Long
        Get
            Return _IdCarteraCobro
        End Get
        Set(value As Long)
            _IdCarteraCobro = value
        End Set
    End Property

    Private _IdSituacionCobroLibre As Long = 0L
    Public Property IdSituacionCobroLibre As Long
        Get
            Return _IdSituacionCobroLibre
        End Get
        Set(value As Long)
            _IdSituacionCobroLibre = value
        End Set
    End Property


    Private _IsRecalcular As Boolean
    Public Property IsRecalcular() As Boolean
        Get
            Return _IsRecalcular
        End Get
        Set(ByVal value As Boolean)
            _IsRecalcular = value
        End Set
    End Property

    Private _CodigoComunicacion As String
    Public Property CodigoComunicacion() As String
        Get
            Return _CodigoComunicacion
        End Get
        Set(ByVal value As String)
            _CodigoComunicacion = value
        End Set
    End Property


#Region "Properties"
    Private _Producto As Producto
    Public Property Producto As Producto
        Get
            Return _Producto
        End Get
        Set(value As Producto)
            _Producto = value
        End Set
    End Property

#End Region

End Class
