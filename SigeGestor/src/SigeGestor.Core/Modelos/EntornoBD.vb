Namespace Modelos

    ''' <summary>
    ''' Los tres entornos contra los que trabaja el equipo. El orden importa: es el que se
    ''' usa para ordenar el selector de la interfaz.
    ''' </summary>
    Public Enum ClaveEntorno
        Produccion = 0
        Replica = 1
        Uat = 2
    End Enum

    ''' <summary>
    ''' Un entorno de base de datos. Usuario y Password vienen cifrados en el JSON, igual que
    ''' en el EmpresasBD.json de ActualizaPrecios; se descifran solo al construir la cadena
    ''' de conexión.
    ''' </summary>
    Public Class EntornoBD

        Public Property Clave As ClaveEntorno
        Public Property Nombre As String = ""
        Public Property Servidor As String = ""
        Public Property BaseDatos As String = ""

        ''' <summary>Cifrado. Ver <see cref="Seguridad.Cifrado"/>.</summary>
        Public Property Usuario As String = ""

        ''' <summary>Cifrado. Ver <see cref="Seguridad.Cifrado"/>.</summary>
        Public Property Password As String = ""

        ''' <summary>
        ''' Cuando es True, las operaciones de escritura no ofrecen este entorno. Es lo que
        ''' hace que Replica no aparezca como destino en Precios, Contratos o Productos.
        ''' </summary>
        Public Property SoloLectura As Boolean

        ''' <summary>Requiere VPN para llegar al servidor.</summary>
        Public Property RequiereVpn As Boolean

        ''' <summary>Etiqueta corta para la interfaz: PRO, REP, UAT.</summary>
        Public ReadOnly Property Abreviatura As String
            Get
                Select Case Clave
                    Case ClaveEntorno.Produccion : Return "PRO"
                    Case ClaveEntorno.Replica : Return "REP"
                    Case Else : Return "UAT"
                End Select
            End Get
        End Property

        ''' <summary>Lo que se muestra junto al nombre: «172.31.100.12 · SigeTotal».</summary>
        Public ReadOnly Property Descripcion As String
            Get
                Return $"{Servidor} · {BaseDatos}"
            End Get
        End Property

    End Class

End Namespace
