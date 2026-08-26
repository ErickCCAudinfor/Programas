Imports System.IO

Namespace Configuracion

    ''' <summary>
    ''' Dónde van los ficheros que genera la aplicación.
    '''
    ''' Todo a Escritorio\ConsultasBO del usuario que ejecuta. Como el .exe vive en el
    ''' servidor .13 y cada persona lo abre desde el suyo, la ruta se resuelve por usuario y
    ''' por máquina sin configurar nada: cada uno encuentra lo suyo en su propio escritorio.
    '''
    ''' SE USA SpecialFolder.DesktopDirectory, no $"C:\Users\{Environment.UserName}\Desktop"
    ''' como hacía ActualizaPrecios. Esa cadena fija falla en cuanto el perfil no está en C:,
    ''' el escritorio está redirigido por directiva de grupo o lo sincroniza OneDrive — que es
    ''' justo lo habitual en un entorno con servidores y perfiles de dominio.
    ''' </summary>
    Public Module RutasSalida

        Public Const NombreCarpeta As String = "ConsultasBO"

        ''' <summary>
        ''' Carpeta base: Escritorio\ConsultasBO. No se crea al consultarla, solo al usarla,
        ''' para que abrir la aplicación no deje carpetas por ahí sin haber hecho nada.
        ''' </summary>
        Public ReadOnly Property Predeterminada As String
            Get
                Return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    NombreCarpeta)
            End Get
        End Property

        ''' <summary>
        ''' Subcarpeta dentro de la base, ya creada. Con nombre vacío devuelve la base.
        ''' Se usa para mantener separados los volcados por tipo — «Precios»,
        ''' «PreciosPersonalizados»— tal como los tenía ActualizaPrecios, pero todos dentro
        ''' de ConsultasBO en vez de sueltos por el escritorio.
        ''' </summary>
        Public Function Asegurar(Optional subcarpeta As String = "") As String

            Dim ruta = If(String.IsNullOrWhiteSpace(subcarpeta),
                          Predeterminada,
                          Path.Combine(Predeterminada, subcarpeta))

            Try
                Directory.CreateDirectory(ruta)
            Catch ex As Exception
                ' Si no se puede crear —escritorio redirigido sin permisos, disco lleno— se
                ' devuelve la ruta igualmente y que falle quien vaya a escribir, con su
                ' mensaje concreto. Aquí no hay nada útil que decir todavía.
            End Try

            Return ruta

        End Function

    End Module

End Namespace
