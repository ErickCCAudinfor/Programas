Imports System.Runtime.Serialization

Public Class ModeloDeImpresion
    Public Property IdModeloDeImpresion As Long
    Public Property Entorno As String
    Public Property DescripcionModeloDeImpresion As String
    Public Property CodigoTipoModeloDeImpresion As Integer
    Public Property ClassName As String
    Public Property RptFileName As String
    Public Property Modelo As Byte()

End Class
Public Enum TipoModeloImpresionGeneral As Integer
    <EnumMember> NoDefinido = 0
    <EnumMember> Factura = 1
    <EnumMember> AvisoImpagado = 2
    <EnumMember> AvisoCorte = 3
    <EnumMember> Contrato = 4
    <EnumMember> Solicitud = 5
    <EnumMember> Presupuestos = 6
    <EnumMember> Pre_Contrato = 7
    <EnumMember> ComunicadoCRM = 8
    <EnumMember> Varios = 9
    <EnumMember> AvisoDeuda = 10
    <EnumMember> Ofertas = 11
    <EnumMember> ImpagoCanalDual = 12
    <EnumMember> CartaAvisoJuridico = 13
    <EnumMember> ContratoSwap = 14
    <EnumMember> FacturaCompra = 15
    <EnumMember> ContratoResumen = 16
End Enum

Public Enum TipoModeloImpresionTotal As Integer
    <EnumMember> NoDefinido = 0
    <EnumMember> Factura = 1
    <EnumMember> AvisoImpagado = 2
    <EnumMember> AvisoCorte = 3
    <EnumMember> Contrato = 4
    <EnumMember> Solicitud = 5
    <EnumMember> Presupuestos = 6
    <EnumMember> Pre_Contrato = 7
    <EnumMember> ComunicadoCRM = 8
    <EnumMember> Varios = 9
    <EnumMember> AvisoDeuda = 10
    <EnumMember> Ofertas = 11
    <EnumMember> ImpagoCanalDual = 12
    <EnumMember> CartaAvisoJuridico = 13
    <EnumMember> CartaNoRenovacion = 14
    <EnumMember> RealDecreto = 15
    <EnumMember> AvisoRescision = 16
    <EnumMember> PreVencimiento = 17
    <EnumMember> CartaReclamacion = 18
    <EnumMember> ContratoResumen = 19
End Enum