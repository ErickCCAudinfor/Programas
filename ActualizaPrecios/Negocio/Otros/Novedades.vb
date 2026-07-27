''' <summary>
''' Una entrada del historial de cambios de la aplicación.
''' </summary>
Public Class Novedad
    Property Version As String
    Property Fecha As Date
    Property Titulo As String
    Property Cambios As List(Of String)
End Class

''' <summary>
''' Historial de novedades de la aplicación.
'''
''' PARA PUBLICAR NOVEDADES NUEVAS: añade tu entrada AL PRINCIPIO de Historial.
''' El resto es automático: VersionActual pasa a ser la de esa entrada, deja de
''' coincidir con la VersionNovedadesLeida guardada en Usuarios.json y la campana
''' del Form1 vuelve a parpadear para todos los usuarios.
'''
''' IMPORTANTE: el historial va ordenado de más reciente a más antiguo.
''' </summary>
Public Module NovedadesApp

    Public ReadOnly Historial As New List(Of Novedad) From {
        New Novedad With {
            .Version = "2.2",
            .Fecha = New Date(2026, 7, 27),
            .Titulo = "Aviso de novedades",
            .Cambios = New List(Of String) From {
                "Nueva campana de novedades: parpadea cuando hay cambios sin leer y se apaga al abrirla.",
                "Cada usuario guarda por separado qué versión de novedades ha leído.",
                "Se ha corregido un bug al momento de importar productos con la plantilla. Faltaba filtrar por entorno el producto",
                "Se han mejorado las consultas de Curvas, son un 80% más rápidas"
            }
        }
    }

    ''' <summary>
    ''' Versión de novedades que trae este ejecutable (la más reciente del historial).
    ''' </summary>
    Public ReadOnly Property VersionActual As String
        Get
            If Historial Is Nothing OrElse Historial.Count = 0 Then Return ""
            Return Historial(0).Version
        End Get
    End Property

End Module
