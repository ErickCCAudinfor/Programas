Public Class TarifaPeriodo

    ' Declaración de propiedades privadas con sus tipos
    Private _IdTarifaPeriodo As Long
    Private _Entorno As String
    Private _IdTarifa As Long
    Private _IdPeriodoHorario As Long
    Private _TextoTarifaPeriodo As String

    ' Constructor por defecto
    Public Sub New()
        _IdTarifaPeriodo = 0   ' Long
        _Entorno = String.Empty ' String
        _IdTarifa = 0           ' Long
        _IdPeriodoHorario = 0   ' Long
        _TextoTarifaPeriodo = String.Empty ' String
    End Sub

    ' Propiedades públicas con getters y setters
    Public Property IdTarifaPeriodo As Long
        Get
            Return _IdTarifaPeriodo
        End Get
        Set(value As Long)
            _IdTarifaPeriodo = value
        End Set
    End Property

    Public Property Entorno As String
        Get
            Return _Entorno
        End Get
        Set(value As String)
            _Entorno = value
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

    Public Property IdPeriodoHorario As Long
        Get
            Return _IdPeriodoHorario
        End Get
        Set(value As Long)
            _IdPeriodoHorario = value
        End Set
    End Property

    Public Property TextoTarifaPeriodo As String
        Get
            Return _TextoTarifaPeriodo
        End Get
        Set(value As String)
            _TextoTarifaPeriodo = value
        End Set
    End Property

End Class
