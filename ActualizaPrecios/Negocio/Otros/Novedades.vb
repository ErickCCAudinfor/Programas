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
            .Version = "2.4",
            .Fecha = New Date(2026, 7, 29),
            .Titulo = "Aviso de novedades y consultas rediseñadas",
            .Cambios = New List(Of String) From {
                "Nueva campana de novedades: parpadea cuando hay cambios sin leer y se apaga al abrirla.",
                "Cada usuario guarda por separado qué versión de novedades ha leído.",
                "Se ha corregido un bug al momento de importar productos con la plantilla. Faltaba filtrar por entorno el producto",
                "Se han mejorado las consultas de Curvas, son un 80% más rápidas",
                "Las consultas generales y de pool se eligen ahora en un desplegable, una cada vez, en lugar de marcar casillas.",
                "Al elegir una consulta se habilitan solo los campos que necesita, y avisa si falta alguno en vez de no hacer nada.",
                "Las consultas se pueden cancelar: mientras se ejecuta, el botón Consultar pasa a ser Cancelar.",
                "Al terminar se indica cuántas filas y qué ficheros se han generado. Si la consulta no devuelve datos, ahora lo dice.",
                "Corregido: la consulta de Trébol dejaba la aplicación bloqueada mientras se ejecutaba.",
                "La consulta de Trébol se divide en dos: ""Desglosado Trébol Luz by CIF"" y ""Desglosado Trébol Gas by CIF"". Ambas filtran por CIF y por el rango de fechas elegido.",
                "Las consultas por CIF permiten ahora elegir entre un Excel por cada CIF o todo junto en un único fichero, igual que las de pool.",
                "Las consultas ya no se cortan por tiempo: tardan lo que necesiten y se paran solo si pulsas Cancelar.",
                "La ventana principal ya no se puede maximizar, porque el contenido quedaba descolocado.",
                "Nueva consulta ""Cuentas LB2B"": pide el nombre del agente y un rango de fechas. Basta con escribir parte del nombre.",
                "Nueva consulta ""Santa Lucía Trébol Gas"": facturas de gas del grupo de tarifa Santa Lucía por rango de fechas.",
                "La consulta ""Energía Activa y Reactiva"" ya se puede usar: antes estaba deshabilitada. Pide una lista de facturas.",
                "Corregido: varias consultas perdían las tildes al leerse y eso hacía fallar las que llevan acentos en los nombres de columna.",
                "Nueva consulta ""Cogeneración"", en dos pasos: primero genera el Excel por fechas y después te pide los IDs de factura de la columna A para sacar el detalle por equipo. Puedes abrir el Excel desde el propio aviso, y omitir el segundo paso si no lo necesitas."
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
