Namespace Modelos

    ''' <summary>
    ''' Usuario autenticado. A diferencia de la UsuarioValidacion de ActualizaPrecios, aquí
    ''' NO se arrastra la contraseña: una vez validada no hace falta para nada y tenerla en
    ''' memoria durante toda la sesión solo añade superficie de riesgo.
    ''' </summary>
    Public Class Usuario

        Public Property Nombre As String = ""
        Public Property Login As String = ""

        ''' <summary>
        ''' True cuando se entra con el acceso especial de informes. Equivale al
        ''' IsLoginReport que ActualizaPrecios pasaba por ByRef.
        ''' </summary>
        Public Property EsAccesoInformes As Boolean

        ''' <summary>Máquina desde la que se ha iniciado sesión.</summary>
        Public Property Equipo As String = Environment.MachineName

        ''' <summary>Última versión de novedades que este usuario ha leído.</summary>
        Public Property VersionNovedadesLeida As String = ""

        Public ReadOnly Property Iniciales As String
            Get
                Dim limpio = If(Nombre, "").Trim()
                If limpio.Length = 0 Then Return "?"

                Dim partes = limpio.Split(" "c, StringSplitOptions.RemoveEmptyEntries)
                If partes.Length = 1 Then
                    Return partes(0).Substring(0, Math.Min(2, partes(0).Length)).ToUpperInvariant()
                End If

                Return (partes(0).Substring(0, 1) & partes(partes.Length - 1).Substring(0, 1)).ToUpperInvariant()
            End Get
        End Property

    End Class

End Namespace
