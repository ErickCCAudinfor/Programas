Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Imports System.Xml.Serialization

Public Class BaseGenericDTO


    Partial Public Class BaseGenericDTO

        Private _IdEmpresaEntorno As Long = 0L
        Public Property IdEmpresaEntorno As Long
            Get
                If _IdEmpresaEntorno = 0L Then
                    Dim bumbaStop As Long = 0
                End If
                Return _IdEmpresaEntorno
            End Get
            Set(value As Long)
                _IdEmpresaEntorno = value
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

        Public Overridable Function Difference(other As BaseGenericDTO) As DifferenceDTO
            Return New DifferenceDTO
        End Function



#Region "IDataErrorInfo"


        Private _Incidencias As List(Of Incidencia) = Nothing
        Public Property Incidencias As List(Of Incidencia)
            Get
                If IsNothing(_Incidencias) Then
                    _Incidencias = New List(Of Incidencia)
                End If
                Return _Incidencias
            End Get
            Set(value As List(Of Incidencia))
                _Incidencias = value
            End Set
        End Property

        Public Sub AddExcepcion(ByVal propertyName As String, Mensaje As String, ExIn As Exception)
            Try
                Incidencias.Add(New Incidencia(propertyName, Mensaje, ExIn))
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Public Sub AddAllIncidencias(propertyNamePrefix As String, IncidenciasIn As List(Of Incidencia))
            Try
                RemoveError(propertyNamePrefix)
                For Each ele As Incidencia In IncidenciasIn
                    Dim newIncidencia As Incidencia = ele.GetClone
                    newIncidencia.Propiedad = String.Format("{0}.{1}", propertyNamePrefix, ele.Propiedad)
                    Incidencias.Add(newIncidencia)
                Next
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Public Sub AddAllIncidencias(propertyNamePrefix As String, MsgPrefix As String, IncidenciasIn As List(Of Incidencia))
            Try
                RemoveError(propertyNamePrefix)
                For Each ele As Incidencia In IncidenciasIn
                    Dim newIncidencia As Incidencia = ele.GetClone
                    newIncidencia.Propiedad = String.Format("{0}.{1}", propertyNamePrefix, ele.Propiedad)
                    newIncidencia.Mensaje = String.Format("{0} - {1}", MsgPrefix, ele.Mensaje)
                    Incidencias.Add(newIncidencia)
                Next
            Catch ex As Exception
                Throw
            End Try
        End Sub



        'Public Sub AddXMLIncidencias(propertyNamePrefix As String, XML As String)
        '    Try
        '        RemoveError(propertyNamePrefix)
        '        Dim IncidenciasIn As List(Of Incidencia) = CType(New XmlHelper().Deserialize(New XmlSerializer(Incidencias.GetType()), XML), List(Of Incidencia))
        '        AddAllIncidencias(propertyNamePrefix, IncidenciasIn)
        '    Catch ex As Exception
        '        Throw
        '    End Try
        'End Sub

        'Public Sub AddXMLIncidenciasPorCodigo(propertyNamePrefix As String, XML As String, code As Integer)
        '    Try
        '        RemoveError(propertyNamePrefix)
        '        Dim IncidenciasIn As List(Of Incidencia) = CType(New XmlHelper().Deserialize(New XmlSerializer(Incidencias.GetType()), XML), List(Of Incidencia))
        '        AddAllIncidencias(propertyNamePrefix, IncidenciasIn.Where(Function(f) f.Code = code).ToList)
        '    Catch ex As Exception
        '        Throw
        '    End Try
        'End Sub

        Public Sub AddIncidencia(IncidenciaIn As Incidencia)
            Try
                Incidencias.RemoveAll(Function(f) f.Propiedad = IncidenciaIn.Propiedad _
                                          AndAlso f.Code = IncidenciaIn.Code _
                                          AndAlso f.CodeAlfa = IncidenciaIn.CodeAlfa _
                                          AndAlso (If(f.Mensaje, String.Empty).StartsWith("@MsgCode@=") OrElse f.Mensaje = IncidenciaIn.Mensaje) _
                                          AndAlso f.IsAdvertencia = IncidenciaIn.IsAdvertencia _
                                          AndAlso f.IsError = IncidenciaIn.IsError _
                                          AndAlso f.IsExcepcion = IncidenciaIn.IsExcepcion)
                Incidencias.Add(IncidenciaIn)
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Public Sub RemoveError(ByVal propertyName As String, ByVal MsgErrWar As String, ByVal isWarning As Boolean)
            Try
                Dim IsAdvertencia As Boolean = isWarning
                Dim IsError As Boolean = Not isWarning
                If If(MsgErrWar, String.Empty).StartsWith("@MsgCode@=") Then
                    Dim CodeAlfa As String = MsgErrWar.Substring(10)
                    Incidencias.RemoveAll(Function(f) f.Propiedad = propertyName AndAlso f.IsAdvertencia = IsAdvertencia AndAlso f.IsError = IsError AndAlso If(f.CodeAlfa, String.Empty) = CodeAlfa)
                Else
                    Incidencias.RemoveAll(Function(f) f.Propiedad = propertyName AndAlso f.IsAdvertencia = IsAdvertencia AndAlso f.IsError = IsError AndAlso If(f.Mensaje, String.Empty) = MsgErrWar)
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Public Sub RemoveError(ByVal propertyNamePrefix As String)
            Try
                Incidencias.RemoveAll(Function(f) f.Propiedad.StartsWith(propertyNamePrefix))
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Public Sub ClearErrores()
            Incidencias = Nothing
        End Sub

        ReadOnly Property ItemeError(ByVal propertyName As String) As String
            Get
                If New String() {"Item", "Error", "Incidencias", "IsOnError", "IsOnWarning", "GetIncidencias", "Warnings", "IncidenciasToString", "ErroresToString", "ErroresToString", "WarningsToString", "HayErrores", "HayErroresYOExcepciones", "ErroresCount", "WarningsCount", "ExcepcionesCount", "ItemeError", "Errores"}.Contains(propertyName) Then
                    Return String.Empty
                Else

                    Dim ret As String = String.Empty
                    For Each ele As Incidencia In Incidencias.Where(Function(f) f.Propiedad.StartsWith(propertyName))

                        ret += IIf(ret.Length > 0, Environment.NewLine, "").ToString + If(ele.Mensaje, String.Empty)

                    Next
                    Return ret
                End If
            End Get
        End Property
#End Region

    End Class

#Region "Clases relacionadas con el DTO"

    Public Class DifferenceDTO
        Public Property DtoName As String = String.Empty
        Public Property ID As Long = 0
        Public Property PropertiesDifference As List(Of String) = Nothing

        Public Sub New()
            DtoName = String.Empty
            ID = 0L
            PropertiesDifference = New List(Of String)
        End Sub

        Public Sub New(DtoNameIn As String, IDIn As Long, PropertiesDifferenceIn As List(Of String))
            DtoName = DtoNameIn
            ID = IDIn
            PropertiesDifference = PropertiesDifferenceIn
        End Sub
    End Class


    <Serializable()>
    Public Class Incidencia


        Public Sub New()
            _Propiedad = String.Empty
            _Code = 0
            _CodeAlfa = String.Empty
            _Mensaje = String.Empty
            _IsAdvertencia = False
            _IsError = False
            _IsExcepcion = False
            _ExceptionTimestamp = Nothing
            _ExceptionMessage = String.Empty
            _ExceptionStackTrace = String.Empty
            _InnerException = String.Empty
            _ImageSource = String.Empty
            _InfoExtra = String.Empty
        End Sub

        Public Sub New(PropiedadIn As String, MensajeIn As String, IsErrorIn As Boolean, IsAdvertenciaIn As Boolean)
            _Propiedad = PropiedadIn
            _Code = 0
            _CodeAlfa = GetCodAlfa(MensajeIn)
            _Mensaje = MensajeIn
            _IsAdvertencia = IsAdvertenciaIn
            _IsError = IsErrorIn
            _IsExcepcion = False
            _ExceptionTimestamp = Nothing
            _ExceptionMessage = String.Empty
            _ExceptionStackTrace = String.Empty
            _InnerException = String.Empty
            _ImageSource = String.Empty
            _InfoExtra = String.Empty
        End Sub

        Public Sub New(PropiedadIn As String, MensajeIn As String, CodeIn As Integer, IsErrorIn As Boolean, IsAdvertenciaIn As Boolean, IsValidable As Boolean)
            _Propiedad = PropiedadIn
            _Code = CodeIn
            _CodeAlfa = GetCodAlfa(MensajeIn)
            _Mensaje = MensajeIn
            _IsAdvertencia = IsAdvertenciaIn
            _IsError = IsErrorIn
            _IsExcepcion = False
            _IsValidable = IsValidable
            _ExceptionTimestamp = Nothing
            _ExceptionMessage = String.Empty
            _ExceptionStackTrace = String.Empty
            _InnerException = String.Empty
            _ImageSource = String.Empty
            _InfoExtra = String.Empty
        End Sub

        Public Sub New(PropiedadIn As String, MensajeIn As String, CodeIn As Integer, IsErrorIn As Boolean, IsAdvertenciaIn As Boolean, ValorNumero As Decimal?, ValorTexto As String)
            Me.New(PropiedadIn, MensajeIn, CodeIn, IsErrorIn, IsAdvertenciaIn, ValorNumero, ValorTexto, String.Empty)
        End Sub

        Public Sub New(PropiedadIn As String, MensajeIn As String, CodeIn As Integer, IsErrorIn As Boolean, IsAdvertenciaIn As Boolean, ValorNumero As Decimal?, ValorTexto As String, InfoExtraIn As String)
            _Propiedad = PropiedadIn
            _Code = CodeIn
            _CodeAlfa = GetCodAlfa(MensajeIn)
            _Mensaje = MensajeIn
            _IsAdvertencia = IsAdvertenciaIn
            _IsError = IsErrorIn
            _IsExcepcion = False
            _ValorNumero = ValorNumero
            _ValorTexto = ValorTexto
            _ExceptionTimestamp = Nothing
            _ExceptionMessage = String.Empty
            _ExceptionStackTrace = String.Empty
            _InnerException = String.Empty
            _ImageSource = String.Empty
            _InfoExtra = InfoExtraIn
        End Sub

        Public Sub New(PropiedadIn As String, MensajeIn As String, ExcepcionIn As Exception)
            _Propiedad = PropiedadIn
            _Code = 0
            _CodeAlfa = GetCodAlfa(MensajeIn)
            _Mensaje = MensajeIn
            _IsAdvertencia = False
            _IsError = True
            _IsExcepcion = True
            If Not IsNothing(ExcepcionIn) Then
                _ExceptionTimestamp = DateTime.Now
                _ExceptionMessage = ExcepcionIn.Message
                _ExceptionStackTrace = ExcepcionIn.StackTrace
                If Not IsNothing(ExcepcionIn.InnerException) Then
                    _InnerException = ExcepcionIn.InnerException.Message
                End If
            End If
            _ImageSource = String.Empty
            _InfoExtra = String.Empty
        End Sub

        Public Sub New(MensajeIn As String, ExcepcionIn As Exception)
            Me.New(String.Empty, MensajeIn, ExcepcionIn)
        End Sub

        Public Sub New(MensajeIn As String, InfoExtraIn As String)
            Me.New(String.Empty, MensajeIn, InfoExtraIn)
        End Sub

        Public Sub New(PropiedadIn As String, MensajeIn As String, InfoExtraIn As String)
            _Propiedad = PropiedadIn
            _Mensaje = MensajeIn
            _InfoExtra = InfoExtraIn
        End Sub

        Private Function GetCodAlfa(MensajeIn As String) As String
            Try
                Dim ret As String = String.Empty
                If If(MensajeIn, String.Empty).StartsWith("@MsgCode@=") Then
                    ret = MensajeIn.Substring(10)
                End If
                Return ret
            Catch ex As Exception
                Throw
            End Try
        End Function

        Private _Propiedad As String

        Public Property Propiedad As String
            Get
                Return _Propiedad
            End Get
            Set(value As String)
                _Propiedad = value
            End Set
        End Property

        Private _Code As Integer

        Public Property Code As Integer
            Get
                Return _Code
            End Get
            Set(value As Integer)
                _Code = value
            End Set
        End Property

        Private _CodeAlfa As String

        Public Property CodeAlfa As String
            Get
                Return _CodeAlfa
            End Get
            Set(value As String)
                _CodeAlfa = value
            End Set
        End Property

        Private _Mensaje As String

        Public Property Mensaje As String
            Get
                Return _Mensaje
            End Get
            Set(value As String)
                _Mensaje = value
            End Set
        End Property

        Private _IsAdvertencia As Boolean

        Public Property IsAdvertencia As Boolean
            Get
                Return _IsAdvertencia
            End Get
            Set(value As Boolean)
                _IsAdvertencia = value
            End Set
        End Property

        Private _IsError As Boolean

        Public Property IsError As Boolean
            Get
                Return _IsError
            End Get
            Set(value As Boolean)
                _IsError = value
            End Set
        End Property

        Private _IsExcepcion As Boolean

        Public Property IsExcepcion As Boolean
            Get
                Return _IsExcepcion
            End Get
            Set(value As Boolean)
                _IsExcepcion = value
            End Set
        End Property

        Public ReadOnly Property IsErrorOrExcepcion() As Boolean
            Get
                Return IsError OrElse IsExcepcion
            End Get
        End Property

        Private _IsValidable As Boolean
        Public Property IsValidable As Boolean
            Get
                Return _IsValidable
            End Get
            Set(value As Boolean)
                _IsValidable = value
            End Set
        End Property

        Private _ImageSource As String
        Public Property ImageSource As String
            Get
                Return _ImageSource
            End Get
            Set(value As String)
                _ImageSource = value
            End Set
        End Property

        Private _ValorNumero As Decimal?
        Public Property ValorNumero As Decimal?
            Get
                Return _ValorNumero
            End Get
            Set(value As Decimal?)
                _ValorNumero = value
            End Set
        End Property

        Private _ValorTexto As String

        Public Property ValorTexto As String
            Get
                Return _ValorTexto
            End Get
            Set(value As String)
                _ValorTexto = value
            End Set
        End Property

        Private _InfoExtra As String

        Public Property InfoExtra As String
            Get
                Return _InfoExtra
            End Get
            Set(value As String)
                _InfoExtra = value
            End Set
        End Property

        Private _MensajeControlIncidencia As String
        Public ReadOnly Property MensajeControlIncidencia As String
            Get
                If IsNothing(_MensajeControlIncidencia) Then
                    _MensajeControlIncidencia = String.Format("{0}{1}{2}", Mensaje, IIf(IsNothing(ValorNumero), String.Empty, " - " + ValorNumero.ToString), IIf(IsNothing(ValorTexto) OrElse ValorTexto = String.Empty, String.Empty, " - " + ValorTexto))
                End If
                Return _MensajeControlIncidencia
            End Get
        End Property


#Region "Datos de Exception"
        Private _ExceptionTimestamp As DateTime?

        Public Property ExceptionTimestamp As DateTime?
            Get
                Return _ExceptionTimestamp
            End Get
            Set(value As DateTime?)
                _ExceptionTimestamp = value
            End Set
        End Property

        Private _ExceptionMessage As String

        Public Property ExceptionMessage As String
            Get
                Return _ExceptionMessage
            End Get
            Set(value As String)
                _ExceptionMessage = value
            End Set
        End Property

        Private _ExceptionStackTrace As String

        Public Property ExceptionStackTrace As String
            Get
                Return _ExceptionStackTrace
            End Get
            Set(value As String)
                _ExceptionStackTrace = value
            End Set
        End Property

        Private _InnerException As String

        Public Property InnerException As String
            Get
                Return _InnerException
            End Get
            Set(value As String)
                _InnerException = value
            End Set
        End Property
#End Region


        Public Function GetClone() As Incidencia
            Dim clone As New Incidencia
            clone.Propiedad = Propiedad
            clone.Code = Code
            clone.CodeAlfa = CodeAlfa
            clone.Mensaje = Mensaje
            clone.IsAdvertencia = IsAdvertencia
            clone.IsError = IsError
            clone.IsExcepcion = IsExcepcion
            clone.ValorNumero = ValorNumero
            clone.ValorTexto = ValorTexto
            clone.ExceptionTimestamp = ExceptionTimestamp
            clone.ExceptionMessage = ExceptionMessage
            clone.ExceptionStackTrace = ExceptionStackTrace
            clone.InnerException = InnerException
            clone.ImageSource = ImageSource
            clone.InfoExtra = InfoExtra
            Return clone
        End Function

    End Class


    Public Enum TipoEntorno As Integer
        Ninguno = 0
        Universal = 1
        Grupo = 2
        Empresa = 3
    End Enum

#End Region


End Class
