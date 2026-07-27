''' <summary>
''' Identidad de la aplicación (nombre y versión) en un único sitio.
'''
''' La versión NO se escribe aquí a mano: sale de la entrada más reciente de
''' NovedadesApp.Historial. Así, publicar una versión nueva es un único gesto
''' —añadir su entrada en Negocio\Otros\Novedades.vb— y con eso se actualizan a la
''' vez el título del Form1, el pie del Login y el aviso de novedades sin leer.
'''
''' Cualquier pantalla que quiera mostrar la versión debe usar AppInfo, nunca un
''' literal en el diseñador.
''' </summary>
Public Module AppInfo

    Public Const Nombre As String = "Gestor de Datos SIGE"

    ''' <summary>Versión publicada, p. ej. "2.2". Cadena vacía si no hay historial.</summary>
    Public ReadOnly Property Version As String
        Get
            Return NovedadesApp.VersionActual
        End Get
    End Property

    ''' <summary>Etiqueta corta para pies de pantalla, p. ej. "version 2.2".</summary>
    Public ReadOnly Property VersionEtiqueta As String
        Get
            If String.IsNullOrEmpty(Version) Then Return ""
            Return "version " & Version
        End Get
    End Property

    ''' <summary>Título de ventana, p. ej. "Gestor de Datos SIGE (versión 2.2)".</summary>
    Public ReadOnly Property TituloVentana As String
        Get
            If String.IsNullOrEmpty(Version) Then Return Nombre
            Return $"{Nombre} (versión {Version})"
        End Get
    End Property

End Module
