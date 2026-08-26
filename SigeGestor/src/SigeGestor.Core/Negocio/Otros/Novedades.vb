Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
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
            .Version = "3.0",
            .Fecha = New Date(2026, 8, 25),
            .Titulo = "Aplicación nueva: SigeGestor",
            .Cambios = New List(Of String) From {
                "La aplicación se ha reescrito de cero. Lo que hacía la 2.5 lo sigue haciendo, pero ahora todo está agrupado por secciones en la barra de la izquierda en lugar de repartido en treinta y un botones.",
                "Cada operación dice contra qué entorno se va a lanzar y no deja elegir Réplica si va a escribir. Las de consulta arrancan en Réplica, para no provocar bloqueos a los usuarios de SIGE.",
                "Las operaciones que recorren una lista muestran el avance elemento a elemento: se ve cuál va, cuáles han acabado y por qué ha fallado cada una. Antes solo había un GIF girando.",
                "Un fallo en un elemento ya no tumba el resto del lote: se marca ese y se sigue.",
                "Se pueden cancelar sin cerrar la aplicación.",
                "Todos los ficheros que se generan van a Escritorio\ConsultasBO, cada uno en su subcarpeta, y nunca se sobrescribe uno anterior: llevan fecha y hora.",
                "Hay un histórico de ejecuciones del equipo: quién lanzó qué, contra qué entorno y cómo acabó. Se conserva tres días.",
                "El inicio muestra accesos rápidos a lo que cada uno usa más, por usuario.",
                "Mantenimiento de empresas y bases de datos, con prueba de conexión antes de guardar.",
                "Mantenimiento de modelos de impresión: rejilla por empresa con filtro, y subir o actualizar un .rpt en varias empresas de golpe.",
                "Corregido: al editar un modelo de impresión sin cambiar el fichero, se borraba el report guardado.",
                "Corregido: en el masivo de contratos, el Excel de ""después"" se generaba con la consulta de ""antes"", así que los dos ficheros salían iguales y la comparación no servía.",
                "Corregido: en el masivo de contratos, cada campo tenía dos casillas y marcar la segunda sin la primera no hacía nada, en silencio. Ahora es un desplegable de tres opciones.",
                "Corregido: en el masivo de contratos, una observación con un apóstrofo rompía la actualización de todo el lote.",
                "Corregido: al cambiar el agente o el administrador de una lista con contratos de luz y de gas, se escribía el mismo identificador en todos. Ahora se elige la pareja del entorno de cada contrato.",
                "Corregido: en penalizaciones, cuando la lista tenía luz y gas, el fichero de gas machacaba el de luz.",
                "Corregido: al extraer PDF, las facturas que no tenían PDF desaparecían del recuento sin avisar."
            }
        },
        New Novedad With {
            .Version = "2.5",
            .Fecha = New Date(2026, 7, 29),
            .Titulo = "Varias consultas a la vez",
            .Cambios = New List(Of String) From {
                "Ya se pueden lanzar hasta 3 consultas a la vez. El panel ""En curso"" muestra en qué va cada una y permite cancelarlas por separado.",
                "No se puede lanzar dos veces la misma consulta a la vez, porque las dos escribirían el mismo Excel.",
                "Aviso bajo el desplegable recordando que conviene lanzar las consultas contra Réplica.",
                "Las novedades se reparten en dos pestañas: los cambios de esta versión y, aparte, las versiones anteriores.",
                "Tres consultas nuevas por rango de fechas: ""Trébol Luz"", ""Trébol Gas"" y ""JC Castilla-La Mancha"". Las dos de Trébol sacan todas las facturas del periodo; las de ""Desglosado Trébol ... by CIF"" siguen ahí para filtrar por CIF."
            }
        },
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

    ''' <summary>
    ''' Cuántas versiones del historial son posteriores a la que esa persona ya vio. Es el
    ''' número del aviso de la barra lateral.
    '''
    ''' Con la cadena vacía —alguien que entra por primera vez— NO se devuelven todas: se
    ''' devuelve solo la actual. Un aviso con el historial entero no dice nada útil.
    ''' </summary>
    Public Function SinLeer(versionLeida As String) As Integer

        If Historial Is Nothing OrElse Historial.Count = 0 Then Return 0
        If String.IsNullOrWhiteSpace(versionLeida) Then Return 1

        Return Historial.Where(Function(n) EsPosterior(n.Version, versionLeida)).Count()

    End Function

    ''' <summary>
    ''' Compara dos versiones por sus números, no como texto.
    '''
    ''' Con String.Compare, "2.5" es mayor que "3.0" —porque "2" &lt; "3" es lo único que
    ''' habría mirado bien, pero "10.0" sería menor que "9.0"— así que se comparan los
    ''' componentes uno a uno como enteros.
    ''' </summary>
    Public Function EsPosterior(version As String, respectoA As String) As Boolean

        Dim a = Componentes(version)
        Dim b = Componentes(respectoA)

        For i = 0 To Math.Max(a.Length, b.Length) - 1
            Dim va = If(i < a.Length, a(i), 0)
            Dim vb = If(i < b.Length, b(i), 0)
            If va <> vb Then Return va > vb
        Next

        Return False

    End Function

    Private Function Componentes(version As String) As Integer()

        If String.IsNullOrWhiteSpace(version) Then Return New Integer() {}

        Return version.Split("."c) _
                      .Select(Function(t)
                                  Dim n As Integer
                                  Integer.TryParse(t.Trim(), n)
                                  Return n
                              End Function) _
                      .ToArray()

    End Function

End Module
