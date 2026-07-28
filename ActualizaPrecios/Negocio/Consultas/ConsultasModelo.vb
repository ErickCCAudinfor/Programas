Imports System.Threading

''' <summary>
''' Datos de entrada que necesita una consulta. Al elegirla en el combo, el formulario
''' habilita solo los campos correspondientes y valida que estén informados antes de lanzar.
''' </summary>
<Flags>
Public Enum EntradasConsulta
    Ninguna = 0
    Fechas = 1
    Cups = 2
    Cifs = 4
    Facturas = 8
End Enum

Public Class ProgresoConsulta
    Property Mensaje As String = ""
    Property Contador As String = ""
End Class

''' <summary>
''' Todo lo que una consulta necesita para ejecutarse. Lo construye el formulario en el
''' hilo de UI y se pasa a la tarea de fondo, para que la consulta nunca toque controles.
''' </summary>
Public Class ContextoConsulta

    Property Conexion As String = ""
    Property CarpetaDestino As String = ""
    Property Desde As Date
    Property Hasta As Date

    ''' <summary>CUPS, CIF o números de factura ya troceados y limpios, según lo que pida la consulta.</summary>
    Property Entradas As New List(Of String)

    Property Dividir As Boolean = False
    Property Cancelacion As CancellationToken = CancellationToken.None
    Property Progreso As IProgress(Of ProgresoConsulta) = Nothing

    Public Sub Informar(mensaje As String, Optional contador As String = "")
        If Progreso IsNot Nothing Then
            Progreso.Report(New ProgresoConsulta With {.Mensaje = mensaje, .Contador = contador})
        End If
    End Sub

    Public Sub AbortarSiCancelado()
        Cancelacion.ThrowIfCancellationRequested()
    End Sub

End Class

''' <summary>Qué se ha generado realmente, para poder informar al usuario sin mentirle.</summary>
Public Class ResultadoConsulta

    Property Filas As Integer = 0
    Property Ficheros As New List(Of String)

    Public ReadOnly Property SinDatos As Boolean
        Get
            Return Filas = 0
        End Get
    End Property

    Public Sub Anotar(filasEscritas As Integer, ruta As String)
        Filas += filasEscritas
        If filasEscritas > 0 AndAlso Not Ficheros.Contains(ruta) Then Ficheros.Add(ruta)
    End Sub

End Class

''' <summary>Una hoja dentro de un mismo libro de Excel.</summary>
Public Class HojaConsulta
    Property Nombre As String
    Property Sql As Func(Of String)
End Class

''' <summary>
''' Una consulta del catálogo. Añadir una consulta nueva a la aplicación es añadir una
''' instancia de esto en CatalogoConsultas: no hay que tocar el diseñador ni el formulario.
''' </summary>
Public Class DefinicionConsulta

    Property Nombre As String = ""
    Property Grupo As String = ""
    Property Requiere As EntradasConsulta = EntradasConsulta.Ninguna

    ''' <summary>Si admite la opción "Dividir Excel" (un fichero por CUPS).</summary>
    Property PermiteDividir As Boolean = False

    ''' <summary>True para las que van contra SigeTotalTM en vez de contra SigeTotal.</summary>
    Property UsaConexionTM As Boolean = False

    ''' <summary>Ayuda que se muestra bajo el combo.</summary>
    Property Descripcion As String = ""

    Property Ejecutar As Func(Of ContextoConsulta, ResultadoConsulta)

    Public Overrides Function ToString() As String
        Return $"{Grupo}  ·  {Nombre}"
    End Function

    ''' <summary>Texto de ayuda de los campos que hay que rellenar.</summary>
    Public Function TextoEntradas() As String

        Dim partes As New List(Of String)
        If Requiere.HasFlag(EntradasConsulta.Fechas) Then partes.Add("rango de fechas")
        If Requiere.HasFlag(EntradasConsulta.Cups) Then partes.Add("lista de CUPS")
        If Requiere.HasFlag(EntradasConsulta.Cifs) Then partes.Add("lista de CIF")
        If Requiere.HasFlag(EntradasConsulta.Facturas) Then partes.Add("lista de facturas")

        If partes.Count = 0 Then Return "No necesita ningún dato."
        Return "Necesita: " & String.Join(" + ", partes) & "."

    End Function

End Class
