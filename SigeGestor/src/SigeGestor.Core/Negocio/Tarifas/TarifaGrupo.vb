Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.

Public Class TarifaGrupo
    Inherits BaseGenericDTO
    Implements IEquatable(Of TarifaGrupo)

    Public Sub New()
        _IdTarifaGrupo = 0  ' Long
        _Entorno = String.Empty ' String
        _IdTarifa = 0   ' Long
        _TextoTarifaGrupo = String.Empty    ' String
        _IdModeloFactura = Nothing  ' Nullable(Of Long)
        _IdPerfilFacturacion = Nothing  ' Nullable(Of Long)
        _IsBase = Nothing   ' Nullable(Of Boolean)
        _DetalleFactura = False ' Boolean
        _TextoDetalleDesglosado = String.Empty  ' String
        _TextoDetalleSinDesglose = String.Empty ' String
        _IncluidoIE = False ' Boolean
        _TipoOrigenVariable = Nothing   ' Nullable(Of Integer)
        _AplicarPerdidas = False    ' Boolean
        _AplicarPerdidasMargen = False  ' Boolean
        _TipoPagoCapacidad = Nothing    ' Nullable(Of Integer)
        _PrecioTop = Nothing    ' Nullable(Of Decimal)
        _TipoRemuneracionOperadores = Nothing   ' Nullable(Of Integer)
        _ObservacionFactura = String.Empty  ' String
        _IsPresupuesto = False  ' Boolean
        _SoloPerfilesDefinitivos = False    ' Boolean
        _UnificarTerminoEnergia = False ' Boolean
        _UnificarTerminoVariable = False    ' Boolean
        _AplicarImpuestoSobreAcceso = False ' Boolean
        _PorcentajeImpuestoSobreAcceso = Nothing    ' Nullable(Of Decimal)
        _AplicarImpuestoSobreVariable = False   ' Boolean
        _PorcentajeImpuestoSobreVariable = Nothing  ' Nullable(Of Decimal)
        _DetallarImpuestoFactura = False    ' Boolean
        _AplicarCargosIndexadoAcceso = False    ' Boolean
        _AplicarPerdidasCalculadas = False  ' Boolean
        _PrecioTop1 = Nothing   ' Nullable(Of Decimal)
        _PrecioTop2 = Nothing   ' Nullable(Of Decimal)
        _PrecioTop3 = Nothing   ' Nullable(Of Decimal)
        _PrecioTop4 = Nothing   ' Nullable(Of Decimal)
        _PrecioTop5 = Nothing   ' Nullable(Of Decimal)
        _PrecioTop6 = Nothing   ' Nullable(Of Decimal)
        _PorcentajeTasas = Nothing  ' Nullable(Of Decimal)
        _IsTasasDesglosado = False  ' Boolean
        _IdAgenteTipoComision = Nothing ' Nullable(Of Long)
        _PorcentajeIncrementoEnergia = Nothing  ' Nullable(Of Decimal)
        _IsIncrementoEnergia = Nothing  ' Nullable(Of Boolean)
        _QTrimestre = Nothing   ' Nullable(Of Integer)
        _QActivos = Nothing ' Nullable(Of Boolean)
        _MesesVencimiento = Nothing ' Nullable(Of Long)
        _AplicarValoresTarifaGrupo = Nothing    ' Nullable(Of Boolean)
        _FormulaGas = Nothing   ' Nullable(Of Integer)
        _Referencia = String.Empty  ' String
        _Click = Nothing    ' Nullable(Of Boolean)
        _CodigoTipoClick = Nothing  ' Nullable(Of Integer)
        _IsPrecioTechoOMIE = Nothing    ' Nullable(Of Boolean)
        _AplicarImpuestoSobreDetalle = Nothing  ' Nullable(Of Boolean)
        _GrupoPrecioPropio = Nothing    ' Nullable(Of Boolean)
        _IsVigente = Nothing    ' Nullable(Of Boolean)
        _IdModeloImpresion = Nothing    ' Nullable(Of Long)
        _IsProductoEstandar = Nothing   ' Nullable(Of Boolean)
        _IsTarifaPlana = Nothing    ' Nullable(Of Boolean)
        _LimiteConsumo = Nothing    ' Nullable(Of Decimal)
    End Sub

    Public Function Difference(other As TarifaGrupo) As DifferenceDTO
        If other Is Nothing Then Throw New ArgumentException("other is null")
        If Not TypeOf other Is TarifaGrupo Then Throw New ArgumentException("Type of other invalid")
        Dim otherDTO As TarifaGrupo = CType(other, TarifaGrupo)
        Dim dif As New List(Of String)

        If Not IsEqual(Me.IdTarifaGrupo, otherDTO.IdTarifaGrupo) Then dif.Add("IdTarifaGrupo")
        If Not IsEqual(Me.Entorno, otherDTO.Entorno) Then dif.Add("Entorno")
        If Not IsEqual(Me.IdTarifa, otherDTO.IdTarifa) Then dif.Add("IdTarifa")
        If Not IsEqual(Me.TextoTarifaGrupo, otherDTO.TextoTarifaGrupo) Then dif.Add("TextoTarifaGrupo")
        If Not IsEqual(Me.IdModeloFactura, otherDTO.IdModeloFactura) Then dif.Add("IdModeloFactura")
        If Not IsEqual(Me.IdPerfilFacturacion, otherDTO.IdPerfilFacturacion) Then dif.Add("IdPerfilFacturacion")
        If Not IsEqual(Me.IsBase, otherDTO.IsBase) Then dif.Add("IsBase")
        If Not IsEqual(Me.DetalleFactura, otherDTO.DetalleFactura) Then dif.Add("DetalleFactura")
        If Not IsEqual(Me.TextoDetalleDesglosado, otherDTO.TextoDetalleDesglosado) Then dif.Add("TextoDetalleDesglosado")
        If Not IsEqual(Me.TextoDetalleSinDesglose, otherDTO.TextoDetalleSinDesglose) Then dif.Add("TextoDetalleSinDesglose")
        If Not IsEqual(Me.IncluidoIE, otherDTO.IncluidoIE) Then dif.Add("IncluidoIE")
        If Not IsEqual(Me.TipoOrigenVariable, otherDTO.TipoOrigenVariable) Then dif.Add("TipoOrigenVariable")
        If Not IsEqual(Me.AplicarPerdidas, otherDTO.AplicarPerdidas) Then dif.Add("AplicarPerdidas")
        If Not IsEqual(Me.AplicarPerdidasMargen, otherDTO.AplicarPerdidasMargen) Then dif.Add("AplicarPerdidasMargen")
        If Not IsEqual(Me.TipoPagoCapacidad, otherDTO.TipoPagoCapacidad) Then dif.Add("TipoPagoCapacidad")
        If Not IsEqual(Me.PrecioTop, otherDTO.PrecioTop) Then dif.Add("PrecioTop")
        If Not IsEqual(Me.TipoRemuneracionOperadores, otherDTO.TipoRemuneracionOperadores) Then dif.Add("TipoRemuneracionOperadores")
        If Not IsEqual(Me.ObservacionFactura, otherDTO.ObservacionFactura) Then dif.Add("ObservacionFactura")
        If Not IsEqual(Me.IsPresupuesto, otherDTO.IsPresupuesto) Then dif.Add("IsPresupuesto")
        If Not IsEqual(Me.SoloPerfilesDefinitivos, otherDTO.SoloPerfilesDefinitivos) Then dif.Add("SoloPerfilesDefinitivos")
        If Not IsEqual(Me.UnificarTerminoEnergia, otherDTO.UnificarTerminoEnergia) Then dif.Add("UnificarTerminoEnergia")
        If Not IsEqual(Me.UnificarTerminoVariable, otherDTO.UnificarTerminoVariable) Then dif.Add("UnificarTerminoVariable")
        If Not IsEqual(Me.AplicarImpuestoSobreAcceso, otherDTO.AplicarImpuestoSobreAcceso) Then dif.Add("AplicarImpuestoSobreAcceso")
        If Not IsEqual(Me.PorcentajeImpuestoSobreAcceso, otherDTO.PorcentajeImpuestoSobreAcceso) Then dif.Add("PorcentajeImpuestoSobreAcceso")
        If Not IsEqual(Me.AplicarImpuestoSobreVariable, otherDTO.AplicarImpuestoSobreVariable) Then dif.Add("AplicarImpuestoSobreVariable")
        If Not IsEqual(Me.PorcentajeImpuestoSobreVariable, otherDTO.PorcentajeImpuestoSobreVariable) Then dif.Add("PorcentajeImpuestoSobreVariable")
        If Not IsEqual(Me.DetallarImpuestoFactura, otherDTO.DetallarImpuestoFactura) Then dif.Add("DetallarImpuestoFactura")
        If Not IsEqual(Me.AplicarCargosIndexadoAcceso, otherDTO.AplicarCargosIndexadoAcceso) Then dif.Add("AplicarCargosIndexadoAcceso")
        If Not IsEqual(Me.AplicarPerdidasCalculadas, otherDTO.AplicarPerdidasCalculadas) Then dif.Add("AplicarPerdidasCalculadas")
        If Not IsEqual(Me.PrecioTop1, otherDTO.PrecioTop1) Then dif.Add("PrecioTop1")
        If Not IsEqual(Me.PrecioTop2, otherDTO.PrecioTop2) Then dif.Add("PrecioTop2")
        If Not IsEqual(Me.PrecioTop3, otherDTO.PrecioTop3) Then dif.Add("PrecioTop3")
        If Not IsEqual(Me.PrecioTop4, otherDTO.PrecioTop4) Then dif.Add("PrecioTop4")
        If Not IsEqual(Me.PrecioTop5, otherDTO.PrecioTop5) Then dif.Add("PrecioTop5")
        If Not IsEqual(Me.PrecioTop6, otherDTO.PrecioTop6) Then dif.Add("PrecioTop6")
        If Not IsEqual(Me.PorcentajeTasas, otherDTO.PorcentajeTasas) Then dif.Add("PorcentajeTasas")
        If Not IsEqual(Me.IsTasasDesglosado, otherDTO.IsTasasDesglosado) Then dif.Add("IsTasasDesglosado")
        If Not IsEqual(Me.IdAgenteTipoComision, otherDTO.IdAgenteTipoComision) Then dif.Add("IdAgenteTipoComision")
        If Not IsEqual(Me.PorcentajeIncrementoEnergia, otherDTO.PorcentajeIncrementoEnergia) Then dif.Add("PorcentajeIncrementoEnergia")
        If Not IsEqual(Me.IsIncrementoEnergia, otherDTO.IsIncrementoEnergia) Then dif.Add("IsIncrementoEnergia")
        If Not IsEqual(Me.QTrimestre, otherDTO.QTrimestre) Then dif.Add("QTrimestre")
        If Not IsEqual(Me.QActivos, otherDTO.QActivos) Then dif.Add("QActivos")
        If Not IsEqual(Me.MesesVencimiento, otherDTO.MesesVencimiento) Then dif.Add("MesesVencimiento")
        If Not IsEqual(Me.AplicarValoresTarifaGrupo, otherDTO.AplicarValoresTarifaGrupo) Then dif.Add("AplicarValoresTarifaGrupo")
        If Not IsEqual(Me.FormulaGas, otherDTO.FormulaGas) Then dif.Add("FormulaGas")
        If Not IsEqual(Me.Referencia, otherDTO.Referencia) Then dif.Add("Referencia")
        If Not IsEqual(Me.Click, otherDTO.Click) Then dif.Add("Click")
        If Not IsEqual(Me.CodigoTipoClick, otherDTO.CodigoTipoClick) Then dif.Add("CodigoTipoClick")
        If Not IsEqual(Me.IsPrecioTechoOMIE, otherDTO.IsPrecioTechoOMIE) Then dif.Add("IsPrecioTechoOMIE")
        If Not IsEqual(Me.AplicarImpuestoSobreDetalle, otherDTO.AplicarImpuestoSobreDetalle) Then dif.Add("AplicarImpuestoSobreDetalle")
        If Not IsEqual(Me.GrupoPrecioPropio, otherDTO.GrupoPrecioPropio) Then dif.Add("GrupoPrecioPropio")
        If Not IsEqual(Me.IsVigente, otherDTO.IsVigente) Then dif.Add("IsVigente")
        If Not IsEqual(Me.IdModeloImpresion, otherDTO.IdModeloImpresion) Then dif.Add("IdModeloImpresion")
        If Not IsEqual(Me.IsProductoEstandar, otherDTO.IsProductoEstandar) Then dif.Add("IsProductoEstandar")
        If Not IsEqual(Me.IsTarifaPlana, otherDTO.IsTarifaPlana) Then dif.Add("IsTarifaPlana")
        If Not IsEqual(Me.LimiteConsumo, otherDTO.LimiteConsumo) Then dif.Add("LimiteConsumo")
        Return New DifferenceDTO(Me.GetType.ToString, Me.IDEntityDTO, dif)
    End Function

    Public Overridable Function GetClone() As TarifaGrupo
        Dim clone As New TarifaGrupo
        clone.IdTarifaGrupo = _IdTarifaGrupo
        clone.Entorno = _Entorno
        clone.IdTarifa = _IdTarifa
        clone.TextoTarifaGrupo = _TextoTarifaGrupo
        clone.IdModeloFactura = _IdModeloFactura
        clone.IdPerfilFacturacion = _IdPerfilFacturacion
        clone.IsBase = _IsBase
        clone.DetalleFactura = _DetalleFactura
        clone.TextoDetalleDesglosado = _TextoDetalleDesglosado
        clone.TextoDetalleSinDesglose = _TextoDetalleSinDesglose
        clone.IncluidoIE = _IncluidoIE
        clone.TipoOrigenVariable = _TipoOrigenVariable
        clone.AplicarPerdidas = _AplicarPerdidas
        clone.AplicarPerdidasMargen = _AplicarPerdidasMargen
        clone.TipoPagoCapacidad = _TipoPagoCapacidad
        clone.PrecioTop = _PrecioTop
        clone.TipoRemuneracionOperadores = _TipoRemuneracionOperadores
        clone.ObservacionFactura = _ObservacionFactura
        clone.IsPresupuesto = _IsPresupuesto
        clone.SoloPerfilesDefinitivos = _SoloPerfilesDefinitivos
        clone.UnificarTerminoEnergia = _UnificarTerminoEnergia
        clone.UnificarTerminoVariable = _UnificarTerminoVariable
        clone.AplicarImpuestoSobreAcceso = _AplicarImpuestoSobreAcceso
        clone.PorcentajeImpuestoSobreAcceso = _PorcentajeImpuestoSobreAcceso
        clone.AplicarImpuestoSobreVariable = _AplicarImpuestoSobreVariable
        clone.PorcentajeImpuestoSobreVariable = _PorcentajeImpuestoSobreVariable
        clone.DetallarImpuestoFactura = _DetallarImpuestoFactura
        clone.AplicarCargosIndexadoAcceso = _AplicarCargosIndexadoAcceso
        clone.AplicarPerdidasCalculadas = _AplicarPerdidasCalculadas
        clone.PrecioTop1 = _PrecioTop1
        clone.PrecioTop2 = _PrecioTop2
        clone.PrecioTop3 = _PrecioTop3
        clone.PrecioTop4 = _PrecioTop4
        clone.PrecioTop5 = _PrecioTop5
        clone.PrecioTop6 = _PrecioTop6
        clone.PorcentajeTasas = _PorcentajeTasas
        clone.IsTasasDesglosado = _IsTasasDesglosado
        clone.IdAgenteTipoComision = _IdAgenteTipoComision
        clone.PorcentajeIncrementoEnergia = _PorcentajeIncrementoEnergia
        clone.IsIncrementoEnergia = _IsIncrementoEnergia
        clone.QTrimestre = _QTrimestre
        clone.QActivos = _QActivos
        clone.MesesVencimiento = _MesesVencimiento
        clone.AplicarValoresTarifaGrupo = _AplicarValoresTarifaGrupo
        clone.FormulaGas = _FormulaGas
        clone.Referencia = _Referencia
        clone.Click = _Click
        clone.CodigoTipoClick = _CodigoTipoClick
        clone.IsPrecioTechoOMIE = _IsPrecioTechoOMIE
        clone.AplicarImpuestoSobreDetalle = _AplicarImpuestoSobreDetalle
        clone.GrupoPrecioPropio = _GrupoPrecioPropio
        clone.IsVigente = _IsVigente
        clone.IdModeloImpresion = _IdModeloImpresion
        clone.IsProductoEstandar = _IsProductoEstandar
        clone.IsTarifaPlana = _IsTarifaPlana
        clone.LimiteConsumo = _LimiteConsumo
        Return clone
    End Function

    Public Overloads Function Equals(other As TarifaGrupo) As Boolean _
     Implements IEquatable(Of TarifaGrupo).Equals
        If other Is Nothing Then Return False
        If Object.ReferenceEquals(Me, other) Then Return True
        Dim ret As Boolean = True
        ret = ret AndAlso IsEqual(Me.IdTarifaGrupo, other.IdTarifaGrupo)
        ret = ret AndAlso IsEqual(Me.Entorno, other.Entorno)
        ret = ret AndAlso IsEqual(Me.IdTarifa, other.IdTarifa)
        ret = ret AndAlso IsEqual(Me.TextoTarifaGrupo, other.TextoTarifaGrupo)
        ret = ret AndAlso IsEqual(Me.IdModeloFactura, other.IdModeloFactura)
        ret = ret AndAlso IsEqual(Me.IdPerfilFacturacion, other.IdPerfilFacturacion)
        ret = ret AndAlso IsEqual(Me.IsBase, other.IsBase)
        ret = ret AndAlso IsEqual(Me.DetalleFactura, other.DetalleFactura)
        ret = ret AndAlso IsEqual(Me.TextoDetalleDesglosado, other.TextoDetalleDesglosado)
        ret = ret AndAlso IsEqual(Me.TextoDetalleSinDesglose, other.TextoDetalleSinDesglose)
        ret = ret AndAlso IsEqual(Me.IncluidoIE, other.IncluidoIE)
        ret = ret AndAlso IsEqual(Me.TipoOrigenVariable, other.TipoOrigenVariable)
        ret = ret AndAlso IsEqual(Me.AplicarPerdidas, other.AplicarPerdidas)
        ret = ret AndAlso IsEqual(Me.AplicarPerdidasMargen, other.AplicarPerdidasMargen)
        ret = ret AndAlso IsEqual(Me.TipoPagoCapacidad, other.TipoPagoCapacidad)
        ret = ret AndAlso IsEqual(Me.PrecioTop, other.PrecioTop)
        ret = ret AndAlso IsEqual(Me.TipoRemuneracionOperadores, other.TipoRemuneracionOperadores)
        ret = ret AndAlso IsEqual(Me.ObservacionFactura, other.ObservacionFactura)
        ret = ret AndAlso IsEqual(Me.IsPresupuesto, other.IsPresupuesto)
        ret = ret AndAlso IsEqual(Me.SoloPerfilesDefinitivos, other.SoloPerfilesDefinitivos)
        ret = ret AndAlso IsEqual(Me.UnificarTerminoEnergia, other.UnificarTerminoEnergia)
        ret = ret AndAlso IsEqual(Me.UnificarTerminoVariable, other.UnificarTerminoVariable)
        ret = ret AndAlso IsEqual(Me.AplicarImpuestoSobreAcceso, other.AplicarImpuestoSobreAcceso)
        ret = ret AndAlso IsEqual(Me.PorcentajeImpuestoSobreAcceso, other.PorcentajeImpuestoSobreAcceso)
        ret = ret AndAlso IsEqual(Me.AplicarImpuestoSobreVariable, other.AplicarImpuestoSobreVariable)
        ret = ret AndAlso IsEqual(Me.PorcentajeImpuestoSobreVariable, other.PorcentajeImpuestoSobreVariable)
        ret = ret AndAlso IsEqual(Me.DetallarImpuestoFactura, other.DetallarImpuestoFactura)
        ret = ret AndAlso IsEqual(Me.AplicarCargosIndexadoAcceso, other.AplicarCargosIndexadoAcceso)
        ret = ret AndAlso IsEqual(Me.AplicarPerdidasCalculadas, other.AplicarPerdidasCalculadas)
        ret = ret AndAlso IsEqual(Me.PrecioTop1, other.PrecioTop1)
        ret = ret AndAlso IsEqual(Me.PrecioTop2, other.PrecioTop2)
        ret = ret AndAlso IsEqual(Me.PrecioTop3, other.PrecioTop3)
        ret = ret AndAlso IsEqual(Me.PrecioTop4, other.PrecioTop4)
        ret = ret AndAlso IsEqual(Me.PrecioTop5, other.PrecioTop5)
        ret = ret AndAlso IsEqual(Me.PrecioTop6, other.PrecioTop6)
        ret = ret AndAlso IsEqual(Me.PorcentajeTasas, other.PorcentajeTasas)
        ret = ret AndAlso IsEqual(Me.IsTasasDesglosado, other.IsTasasDesglosado)
        ret = ret AndAlso IsEqual(Me.IdAgenteTipoComision, other.IdAgenteTipoComision)
        ret = ret AndAlso IsEqual(Me.PorcentajeIncrementoEnergia, other.PorcentajeIncrementoEnergia)
        ret = ret AndAlso IsEqual(Me.IsIncrementoEnergia, other.IsIncrementoEnergia)
        ret = ret AndAlso IsEqual(Me.QTrimestre, other.QTrimestre)
        ret = ret AndAlso IsEqual(Me.QActivos, other.QActivos)
        ret = ret AndAlso IsEqual(Me.MesesVencimiento, other.MesesVencimiento)
        ret = ret AndAlso IsEqual(Me.AplicarValoresTarifaGrupo, other.AplicarValoresTarifaGrupo)
        ret = ret AndAlso IsEqual(Me.FormulaGas, other.FormulaGas)
        ret = ret AndAlso IsEqual(Me.Referencia, other.Referencia)
        ret = ret AndAlso IsEqual(Me.Click, other.Click)
        ret = ret AndAlso IsEqual(Me.CodigoTipoClick, other.CodigoTipoClick)
        ret = ret AndAlso IsEqual(Me.IsPrecioTechoOMIE, other.IsPrecioTechoOMIE)
        ret = ret AndAlso IsEqual(Me.AplicarImpuestoSobreDetalle, other.AplicarImpuestoSobreDetalle)
        ret = ret AndAlso IsEqual(Me.GrupoPrecioPropio, other.GrupoPrecioPropio)
        ret = ret AndAlso IsEqual(Me.IsVigente, other.IsVigente)
        ret = ret AndAlso IsEqual(Me.IdModeloImpresion, other.IdModeloImpresion)
        ret = ret AndAlso IsEqual(Me.IsProductoEstandar, other.IsProductoEstandar)
        ret = ret AndAlso IsEqual(Me.IsTarifaPlana, other.IsTarifaPlana)
        ret = ret AndAlso IsEqual(Me.LimiteConsumo, other.LimiteConsumo)
        Return ret
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj Is Nothing Then Return False
        Dim DTOObj As TarifaGrupo = TryCast(obj, TarifaGrupo)
        If DTOObj Is Nothing Then
            Return False
        Else
            Return Equals(DTOObj)
        End If
    End Function

    Public Shared Operator <>(DTO1 As TarifaGrupo, DTO2 As TarifaGrupo) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Object.Equals(DTO1, DTO2)
        End If
        Return DTO1.Equals(DTO2)
    End Operator

    Public Shared Operator =(DTO1 As TarifaGrupo, DTO2 As TarifaGrupo) As Boolean
        If DTO1 Is Nothing OrElse DTO2 Is Nothing Then
            Return Not Object.Equals(DTO1, DTO2)
        End If
        Return Not DTO1.Equals(DTO2)
    End Operator


    Public Overrides Function GetHashCode() As Integer
        Dim ret As String = String.Empty
        If Not IsNothing(Me.IdTarifaGrupo) Then ret = ret & Me.IdTarifaGrupo.GetHashCode().ToString()
        If Not IsNothing(Me.Entorno) Then ret = ret & Me.Entorno.GetHashCode().ToString()
        If Not IsNothing(Me.IdTarifa) Then ret = ret & Me.IdTarifa.GetHashCode().ToString()
        If Not IsNothing(Me.TextoTarifaGrupo) Then ret = ret & Me.TextoTarifaGrupo.GetHashCode().ToString()
        If Not IsNothing(Me.IdModeloFactura) Then ret = ret & Me.IdModeloFactura.GetHashCode().ToString()
        If Not IsNothing(Me.IdPerfilFacturacion) Then ret = ret & Me.IdPerfilFacturacion.GetHashCode().ToString()
        If Not IsNothing(Me.IsBase) Then ret = ret & Me.IsBase.GetHashCode().ToString()
        If Not IsNothing(Me.DetalleFactura) Then ret = ret & Me.DetalleFactura.GetHashCode().ToString()
        If Not IsNothing(Me.TextoDetalleDesglosado) Then ret = ret & Me.TextoDetalleDesglosado.GetHashCode().ToString()
        If Not IsNothing(Me.TextoDetalleSinDesglose) Then ret = ret & Me.TextoDetalleSinDesglose.GetHashCode().ToString()
        If Not IsNothing(Me.IncluidoIE) Then ret = ret & Me.IncluidoIE.GetHashCode().ToString()
        If Not IsNothing(Me.TipoOrigenVariable) Then ret = ret & Me.TipoOrigenVariable.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarPerdidas) Then ret = ret & Me.AplicarPerdidas.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarPerdidasMargen) Then ret = ret & Me.AplicarPerdidasMargen.GetHashCode().ToString()
        If Not IsNothing(Me.TipoPagoCapacidad) Then ret = ret & Me.TipoPagoCapacidad.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioTop) Then ret = ret & Me.PrecioTop.GetHashCode().ToString()
        If Not IsNothing(Me.TipoRemuneracionOperadores) Then ret = ret & Me.TipoRemuneracionOperadores.GetHashCode().ToString()
        If Not IsNothing(Me.ObservacionFactura) Then ret = ret & Me.ObservacionFactura.GetHashCode().ToString()
        If Not IsNothing(Me.IsPresupuesto) Then ret = ret & Me.IsPresupuesto.GetHashCode().ToString()
        If Not IsNothing(Me.SoloPerfilesDefinitivos) Then ret = ret & Me.SoloPerfilesDefinitivos.GetHashCode().ToString()
        If Not IsNothing(Me.UnificarTerminoEnergia) Then ret = ret & Me.UnificarTerminoEnergia.GetHashCode().ToString()
        If Not IsNothing(Me.UnificarTerminoVariable) Then ret = ret & Me.UnificarTerminoVariable.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarImpuestoSobreAcceso) Then ret = ret & Me.AplicarImpuestoSobreAcceso.GetHashCode().ToString()
        If Not IsNothing(Me.PorcentajeImpuestoSobreAcceso) Then ret = ret & Me.PorcentajeImpuestoSobreAcceso.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarImpuestoSobreVariable) Then ret = ret & Me.AplicarImpuestoSobreVariable.GetHashCode().ToString()
        If Not IsNothing(Me.PorcentajeImpuestoSobreVariable) Then ret = ret & Me.PorcentajeImpuestoSobreVariable.GetHashCode().ToString()
        If Not IsNothing(Me.DetallarImpuestoFactura) Then ret = ret & Me.DetallarImpuestoFactura.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarCargosIndexadoAcceso) Then ret = ret & Me.AplicarCargosIndexadoAcceso.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarPerdidasCalculadas) Then ret = ret & Me.AplicarPerdidasCalculadas.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioTop1) Then ret = ret & Me.PrecioTop1.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioTop2) Then ret = ret & Me.PrecioTop2.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioTop3) Then ret = ret & Me.PrecioTop3.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioTop4) Then ret = ret & Me.PrecioTop4.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioTop5) Then ret = ret & Me.PrecioTop5.GetHashCode().ToString()
        If Not IsNothing(Me.PrecioTop6) Then ret = ret & Me.PrecioTop6.GetHashCode().ToString()
        If Not IsNothing(Me.PorcentajeTasas) Then ret = ret & Me.PorcentajeTasas.GetHashCode().ToString()
        If Not IsNothing(Me.IsTasasDesglosado) Then ret = ret & Me.IsTasasDesglosado.GetHashCode().ToString()
        If Not IsNothing(Me.IdAgenteTipoComision) Then ret = ret & Me.IdAgenteTipoComision.GetHashCode().ToString()
        If Not IsNothing(Me.PorcentajeIncrementoEnergia) Then ret = ret & Me.PorcentajeIncrementoEnergia.GetHashCode().ToString()
        If Not IsNothing(Me.IsIncrementoEnergia) Then ret = ret & Me.IsIncrementoEnergia.GetHashCode().ToString()
        If Not IsNothing(Me.QTrimestre) Then ret = ret & Me.QTrimestre.GetHashCode().ToString()
        If Not IsNothing(Me.QActivos) Then ret = ret & Me.QActivos.GetHashCode().ToString()
        If Not IsNothing(Me.MesesVencimiento) Then ret = ret & Me.MesesVencimiento.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarValoresTarifaGrupo) Then ret = ret & Me.AplicarValoresTarifaGrupo.GetHashCode().ToString()
        If Not IsNothing(Me.FormulaGas) Then ret = ret & Me.FormulaGas.GetHashCode().ToString()
        If Not IsNothing(Me.Referencia) Then ret = ret & Me.Referencia.GetHashCode().ToString()
        If Not IsNothing(Me.Click) Then ret = ret & Me.Click.GetHashCode().ToString()
        If Not IsNothing(Me.CodigoTipoClick) Then ret = ret & Me.CodigoTipoClick.GetHashCode().ToString()
        If Not IsNothing(Me.IsPrecioTechoOMIE) Then ret = ret & Me.IsPrecioTechoOMIE.GetHashCode().ToString()
        If Not IsNothing(Me.AplicarImpuestoSobreDetalle) Then ret = ret & Me.AplicarImpuestoSobreDetalle.GetHashCode().ToString()
        If Not IsNothing(Me.GrupoPrecioPropio) Then ret = ret & Me.GrupoPrecioPropio.GetHashCode().ToString()
        If Not IsNothing(Me.IsVigente) Then ret = ret & Me.IsVigente.GetHashCode().ToString()
        If Not IsNothing(Me.IdModeloImpresion) Then ret = ret & Me.IdModeloImpresion.GetHashCode().ToString()
        If Not IsNothing(Me.IsProductoEstandar) Then ret = ret & Me.IsProductoEstandar.GetHashCode().ToString()
        If Not IsNothing(Me.IsTarifaPlana) Then ret = ret & Me.IsTarifaPlana.GetHashCode().ToString()
        If Not IsNothing(Me.LimiteConsumo) Then ret = ret & Me.LimiteConsumo.GetHashCode().ToString()
        Return ret.GetHashCode()
    End Function

    Public Property IDEntityDTO As Long
        Get
            Return IdTarifaGrupo
        End Get
        Set(value As Long)
            IdTarifaGrupo = value
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



    Private _IdTarifaGrupo As Long
    Public Property IdTarifaGrupo As Long
        Get
            Return _IdTarifaGrupo
        End Get
        Set(ByVal value As Long)
            _IdTarifaGrupo = value
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

    Private _IdTarifa As Long
    Public Property IdTarifa As Long
        Get
            Return _IdTarifa
        End Get
        Set(ByVal value As Long)
            _IdTarifa = value
        End Set
    End Property

    Private _TextoTarifaGrupo As String
    Public Property TextoTarifaGrupo As String
        Get
            Return _TextoTarifaGrupo
        End Get
        Set(ByVal value As String)
            _TextoTarifaGrupo = value
        End Set
    End Property

    Private _IdModeloFactura As Nullable(Of Long)
    Public Property IdModeloFactura As Nullable(Of Long)
        Get
            Return _IdModeloFactura
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdModeloFactura = value
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

    Private _IsBase As Nullable(Of Boolean)
    Public Property IsBase As Nullable(Of Boolean)
        Get
            Return _IsBase
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsBase = value
        End Set
    End Property

    Private _DetalleFactura As Boolean
    Public Property DetalleFactura As Boolean
        Get
            Return _DetalleFactura
        End Get
        Set(ByVal value As Boolean)
            _DetalleFactura = value
        End Set
    End Property

    Private _TextoDetalleDesglosado As String
    Public Property TextoDetalleDesglosado As String
        Get
            Return _TextoDetalleDesglosado
        End Get
        Set(ByVal value As String)
            _TextoDetalleDesglosado = value
        End Set
    End Property

    Private _TextoDetalleSinDesglose As String
    Public Property TextoDetalleSinDesglose As String
        Get
            Return _TextoDetalleSinDesglose
        End Get
        Set(ByVal value As String)
            _TextoDetalleSinDesglose = value
        End Set
    End Property

    Private _IncluidoIE As Boolean
    Public Property IncluidoIE As Boolean
        Get
            Return _IncluidoIE
        End Get
        Set(ByVal value As Boolean)
            _IncluidoIE = value
        End Set
    End Property

    Private _TipoOrigenVariable As Nullable(Of Integer)
    Public Property TipoOrigenVariable As Nullable(Of Integer)
        Get
            Return _TipoOrigenVariable
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _TipoOrigenVariable = value
        End Set
    End Property

    Private _AplicarPerdidas As Boolean
    Public Property AplicarPerdidas As Boolean
        Get
            Return _AplicarPerdidas
        End Get
        Set(ByVal value As Boolean)
            _AplicarPerdidas = value
        End Set
    End Property

    Private _AplicarPerdidasMargen As Boolean
    Public Property AplicarPerdidasMargen As Boolean
        Get
            Return _AplicarPerdidasMargen
        End Get
        Set(ByVal value As Boolean)
            _AplicarPerdidasMargen = value
        End Set
    End Property

    Private _TipoPagoCapacidad As Nullable(Of Integer)
    Public Property TipoPagoCapacidad As Nullable(Of Integer)
        Get
            Return _TipoPagoCapacidad
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _TipoPagoCapacidad = value
        End Set
    End Property

    Private _PrecioTop As Nullable(Of Decimal)
    Public Property PrecioTop As Nullable(Of Decimal)
        Get
            Return _PrecioTop
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioTop = value
        End Set
    End Property

    Private _TipoRemuneracionOperadores As Nullable(Of Integer)
    Public Property TipoRemuneracionOperadores As Nullable(Of Integer)
        Get
            Return _TipoRemuneracionOperadores
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _TipoRemuneracionOperadores = value
        End Set
    End Property

    Private _ObservacionFactura As String
    Public Property ObservacionFactura As String
        Get
            Return _ObservacionFactura
        End Get
        Set(ByVal value As String)
            _ObservacionFactura = value
        End Set
    End Property

    Private _IsPresupuesto As Boolean
    Public Property IsPresupuesto As Boolean
        Get
            Return _IsPresupuesto
        End Get
        Set(ByVal value As Boolean)
            _IsPresupuesto = value
        End Set
    End Property

    Private _SoloPerfilesDefinitivos As Boolean
    Public Property SoloPerfilesDefinitivos As Boolean
        Get
            Return _SoloPerfilesDefinitivos
        End Get
        Set(ByVal value As Boolean)
            _SoloPerfilesDefinitivos = value
        End Set
    End Property

    Private _UnificarTerminoEnergia As Boolean
    Public Property UnificarTerminoEnergia As Boolean
        Get
            Return _UnificarTerminoEnergia
        End Get
        Set(ByVal value As Boolean)
            _UnificarTerminoEnergia = value
        End Set
    End Property

    Private _UnificarTerminoVariable As Boolean
    Public Property UnificarTerminoVariable As Boolean
        Get
            Return _UnificarTerminoVariable
        End Get
        Set(ByVal value As Boolean)
            _UnificarTerminoVariable = value
        End Set
    End Property

    Private _AplicarImpuestoSobreAcceso As Boolean
    Public Property AplicarImpuestoSobreAcceso As Boolean
        Get
            Return _AplicarImpuestoSobreAcceso
        End Get
        Set(ByVal value As Boolean)
            _AplicarImpuestoSobreAcceso = value
        End Set
    End Property

    Private _PorcentajeImpuestoSobreAcceso As Nullable(Of Decimal)
    Public Property PorcentajeImpuestoSobreAcceso As Nullable(Of Decimal)
        Get
            Return _PorcentajeImpuestoSobreAcceso
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PorcentajeImpuestoSobreAcceso = value
        End Set
    End Property

    Private _AplicarImpuestoSobreVariable As Boolean
    Public Property AplicarImpuestoSobreVariable As Boolean
        Get
            Return _AplicarImpuestoSobreVariable
        End Get
        Set(ByVal value As Boolean)
            _AplicarImpuestoSobreVariable = value
        End Set
    End Property

    Private _PorcentajeImpuestoSobreVariable As Nullable(Of Decimal)
    Public Property PorcentajeImpuestoSobreVariable As Nullable(Of Decimal)
        Get
            Return _PorcentajeImpuestoSobreVariable
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PorcentajeImpuestoSobreVariable = value
        End Set
    End Property

    Private _DetallarImpuestoFactura As Boolean
    Public Property DetallarImpuestoFactura As Boolean
        Get
            Return _DetallarImpuestoFactura
        End Get
        Set(ByVal value As Boolean)
            _DetallarImpuestoFactura = value
        End Set
    End Property

    Private _AplicarCargosIndexadoAcceso As Boolean
    Public Property AplicarCargosIndexadoAcceso As Boolean
        Get
            Return _AplicarCargosIndexadoAcceso
        End Get
        Set(ByVal value As Boolean)
            _AplicarCargosIndexadoAcceso = value
        End Set
    End Property

    Private _AplicarPerdidasCalculadas As Boolean
    Public Property AplicarPerdidasCalculadas As Boolean
        Get
            Return _AplicarPerdidasCalculadas
        End Get
        Set(ByVal value As Boolean)
            _AplicarPerdidasCalculadas = value
        End Set
    End Property

    Private _PrecioTop1 As Nullable(Of Decimal)
    Public Property PrecioTop1 As Nullable(Of Decimal)
        Get
            Return _PrecioTop1
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioTop1 = value
        End Set
    End Property

    Private _PrecioTop2 As Nullable(Of Decimal)
    Public Property PrecioTop2 As Nullable(Of Decimal)
        Get
            Return _PrecioTop2
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioTop2 = value
        End Set
    End Property

    Private _PrecioTop3 As Nullable(Of Decimal)
    Public Property PrecioTop3 As Nullable(Of Decimal)
        Get
            Return _PrecioTop3
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioTop3 = value
        End Set
    End Property

    Private _PrecioTop4 As Nullable(Of Decimal)
    Public Property PrecioTop4 As Nullable(Of Decimal)
        Get
            Return _PrecioTop4
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioTop4 = value
        End Set
    End Property

    Private _PrecioTop5 As Nullable(Of Decimal)
    Public Property PrecioTop5 As Nullable(Of Decimal)
        Get
            Return _PrecioTop5
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioTop5 = value
        End Set
    End Property

    Private _PrecioTop6 As Nullable(Of Decimal)
    Public Property PrecioTop6 As Nullable(Of Decimal)
        Get
            Return _PrecioTop6
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PrecioTop6 = value
        End Set
    End Property

    Private _PorcentajeTasas As Nullable(Of Decimal)
    Public Property PorcentajeTasas As Nullable(Of Decimal)
        Get
            Return _PorcentajeTasas
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PorcentajeTasas = value
        End Set
    End Property

    Private _IsTasasDesglosado As Boolean
    Public Property IsTasasDesglosado As Boolean
        Get
            Return _IsTasasDesglosado
        End Get
        Set(ByVal value As Boolean)
            _IsTasasDesglosado = value
        End Set
    End Property

    Private _IdAgenteTipoComision As Nullable(Of Long)
    Public Property IdAgenteTipoComision As Nullable(Of Long)
        Get
            Return _IdAgenteTipoComision
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdAgenteTipoComision = value
        End Set
    End Property

    Private _PorcentajeIncrementoEnergia As Nullable(Of Decimal)
    Public Property PorcentajeIncrementoEnergia As Nullable(Of Decimal)
        Get
            Return _PorcentajeIncrementoEnergia
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _PorcentajeIncrementoEnergia = value
        End Set
    End Property

    Private _IsIncrementoEnergia As Nullable(Of Boolean)
    Public Property IsIncrementoEnergia As Nullable(Of Boolean)
        Get
            Return _IsIncrementoEnergia
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsIncrementoEnergia = value
        End Set
    End Property

    Private _QTrimestre As Nullable(Of Integer)
    Public Property QTrimestre As Nullable(Of Integer)
        Get
            Return _QTrimestre
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _QTrimestre = value
        End Set
    End Property

    Private _QActivos As Nullable(Of Boolean)
    Public Property QActivos As Nullable(Of Boolean)
        Get
            Return _QActivos
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _QActivos = value
        End Set
    End Property

    Private _MesesVencimiento As Nullable(Of Long)
    Public Property MesesVencimiento As Nullable(Of Long)
        Get
            Return _MesesVencimiento
        End Get
        Set(ByVal value As Nullable(Of Long))
            _MesesVencimiento = value
        End Set
    End Property

    Private _AplicarValoresTarifaGrupo As Nullable(Of Boolean)
    Public Property AplicarValoresTarifaGrupo As Nullable(Of Boolean)
        Get
            Return _AplicarValoresTarifaGrupo
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AplicarValoresTarifaGrupo = value
        End Set
    End Property

    Private _FormulaGas As Nullable(Of Integer)
    Public Property FormulaGas As Nullable(Of Integer)
        Get
            Return _FormulaGas
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _FormulaGas = value
        End Set
    End Property

    Private _Referencia As String
    Public Property Referencia As String
        Get
            Return _Referencia
        End Get
        Set(ByVal value As String)
            _Referencia = value
        End Set
    End Property

    Private _Click As Nullable(Of Boolean)
    Public Property Click As Nullable(Of Boolean)
        Get
            Return _Click
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _Click = value
        End Set
    End Property

    Private _CodigoTipoClick As Nullable(Of Integer)
    Public Property CodigoTipoClick As Nullable(Of Integer)
        Get
            Return _CodigoTipoClick
        End Get
        Set(ByVal value As Nullable(Of Integer))
            _CodigoTipoClick = value
        End Set
    End Property

    Private _IsPrecioTechoOMIE As Nullable(Of Boolean)
    Public Property IsPrecioTechoOMIE As Nullable(Of Boolean)
        Get
            Return _IsPrecioTechoOMIE
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsPrecioTechoOMIE = value
        End Set
    End Property

    Private _AplicarImpuestoSobreDetalle As Nullable(Of Boolean)
    Public Property AplicarImpuestoSobreDetalle As Nullable(Of Boolean)
        Get
            Return _AplicarImpuestoSobreDetalle
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _AplicarImpuestoSobreDetalle = value
        End Set
    End Property

    Private _GrupoPrecioPropio As Nullable(Of Boolean)
    Public Property GrupoPrecioPropio As Nullable(Of Boolean)
        Get
            Return _GrupoPrecioPropio
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _GrupoPrecioPropio = value
        End Set
    End Property

    Private _IsVigente As Nullable(Of Boolean)
    Public Property IsVigente As Nullable(Of Boolean)
        Get
            Return _IsVigente
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsVigente = value
        End Set
    End Property

    Private _IdModeloImpresion As Nullable(Of Long)
    Public Property IdModeloImpresion As Nullable(Of Long)
        Get
            Return _IdModeloImpresion
        End Get
        Set(ByVal value As Nullable(Of Long))
            _IdModeloImpresion = value
        End Set
    End Property

    Private _IsProductoEstandar As Nullable(Of Boolean)
    Public Property IsProductoEstandar As Nullable(Of Boolean)
        Get
            Return _IsProductoEstandar
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsProductoEstandar = value
        End Set
    End Property

    Private _IsTarifaPlana As Nullable(Of Boolean)
    Public Property IsTarifaPlana As Nullable(Of Boolean)
        Get
            Return _IsTarifaPlana
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            _IsTarifaPlana = value
        End Set
    End Property

    Private _LimiteConsumo As Nullable(Of Decimal)
    Public Property LimiteConsumo As Nullable(Of Decimal)
        Get
            Return _LimiteConsumo
        End Get
        Set(ByVal value As Nullable(Of Decimal))
            _LimiteConsumo = value
        End Set
    End Property
End Class
Partial Class TarifaGrupo
    Private _idperfilfacturacionoNuevo As Long
    Public Property idperfilfacturacionoNuevo As Long
        Get
            Return _idperfilfacturacionoNuevo
        End Get
        Set(ByVal value As Long)
            _idperfilfacturacionoNuevo = value
        End Set
    End Property
End Class