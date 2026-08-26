Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.


Imports System.Collections.ObjectModel
Imports System.Runtime.Serialization

Partial Public Class PerfilFacturacion
    Public Property IdPerfilFacturacion As Long
    Public Property Entorno As String
    Public Property TextoPerfilFacturacion As String
    Public Property CodSectorEmpresarial As Nullable(Of Integer)
    Public Property LineaNegocio As Nullable(Of Integer)

    Public Overridable Property Contrato As ICollection(Of Contrato) = New HashSet(Of Contrato)
    Public Overridable Property ContratoTarifa As ICollection(Of ContratoTarifa) = New HashSet(Of ContratoTarifa)
    'Public Overridable Property FacturaTipo As ICollection(Of FacturaTipo) = New HashSet(Of FacturaTipo)
    'Public Overridable Property FacturaTipo1 As ICollection(Of FacturaTipo) = New HashSet(Of FacturaTipo)
    'Public Overridable Property Lectura As ICollection(Of Lectura) = New HashSet(Of Lectura)
    'Public Overridable Property Lectura1 As ICollection(Of Lectura) = New HashSet(Of Lectura)
    'Public Overridable Property LecturaGestinel As ICollection(Of LecturaGestinel) = New HashSet(Of LecturaGestinel)
    'Public Overridable Property LecturaGestinel1 As ICollection(Of LecturaGestinel) = New HashSet(Of LecturaGestinel)
    'Public Overridable Property PerfilFacturacionConfiguracion As ICollection(Of PerfilFacturacionConfiguracion) = New HashSet(Of PerfilFacturacionConfiguracion)
    'Public Overridable Property TarifaGrupo As ICollection(Of TarifaGrupo) = New HashSet(Of TarifaGrupo)
    'Public Overridable Property TarifaPeaje As ICollection(Of TarifaPeaje) = New HashSet(Of TarifaPeaje)


End Class

Partial Class PerfilFacturacion
    Private _PerfilFacturacionConfiguraciones As ObservableCollection(Of PerfilFacturacionConfiguracion)
    <DataMember()> Public Property PerfilFacturacionConfiguraciones As ObservableCollection(Of PerfilFacturacionConfiguracion)
        Get
            Return _PerfilFacturacionConfiguraciones
        End Get
        Set(value As ObservableCollection(Of PerfilFacturacionConfiguracion))
            _PerfilFacturacionConfiguraciones = value
        End Set
    End Property

    Public Function isPerfilIndexado() As Boolean
        Try
            Dim ret As Boolean = False

            If Not IsNothing(PerfilFacturacionConfiguraciones) AndAlso PerfilFacturacionConfiguraciones.Count > 0 Then
                For Each perfilFacturacionConfiguracion As PerfilFacturacionConfiguracion In PerfilFacturacionConfiguraciones
                    If Not IsNothing(perfilFacturacionConfiguracion) Then
                        ' Si el PerfilFacturacion está asociado a por lo menos un concepto indexado, todo el PerfilFacturacion es considerado indexado.
                        If perfilFacturacionConfiguracion.IsConceptoIndexado Then
                            Return True
                        End If
                    End If
                Next
            End If

            Return ret
        Catch ex As Exception
            Throw ex
        End Try
    End Function
End Class