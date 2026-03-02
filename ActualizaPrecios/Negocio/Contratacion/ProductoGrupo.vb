Public Class ProductoGrupo
    Public Sub New()
        _IdProductoGrupo = 0    ' Long
        _Entorno = String.Empty    ' String
        _TextoProductoGrupo = String.Empty  ' String
        _Texto = String.Empty   ' String
    End Sub

    Private _IdProductoGrupo As Long
    Public Property IdProductoGrupo As Long
        Get
            Return _IdProductoGrupo
        End Get
        Set(ByVal value As Long)
            _IdProductoGrupo = value
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

    Private _TextoProductoGrupo As String
    Public Property TextoProductoGrupo As String
        Get
            Return _TextoProductoGrupo
        End Get
        Set(ByVal value As String)
            _TextoProductoGrupo = value
        End Set
    End Property

    Private _Texto As String
    Public Property Texto As String
        Get
            Return _Texto
        End Get
        Set(ByVal value As String)
            _Texto = value
        End Set
    End Property
End Class
