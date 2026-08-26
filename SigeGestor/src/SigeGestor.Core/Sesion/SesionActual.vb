Imports SigeGestor.Core.Modelos

Namespace Sesion

    ''' <summary>
    ''' Estado de la sesión en curso.
    '''
    ''' En ActualizaPrecios esto era un Module con una variable pública, así que cualquier
    ''' fichero podía escribir en ella. Aquí el usuario solo se puede fijar una vez, al iniciar
    ''' sesión, y a partir de ahí es de lectura.
    ''' </summary>
    Public NotInheritable Class SesionActual

        Private Shared ReadOnly Candado As New Object()
        Private Shared _usuario As Usuario
        Private Shared _entornoActivo As EntornoBD

        Private Sub New()
        End Sub

        Public Shared ReadOnly Property Usuario As Usuario
            Get
                Return _usuario
            End Get
        End Property

        Public Shared ReadOnly Property Autenticado As Boolean
            Get
                Return _usuario IsNot Nothing
            End Get
        End Property

        ''' <summary>
        ''' Entorno con el que se está trabajando ahora mismo. Es informativo: cada operación
        ''' decide el suyo. Sirve para que la barra superior sepa de qué color pintarse.
        ''' </summary>
        Public Shared Property EntornoActivo As EntornoBD
            Get
                Return _entornoActivo
            End Get
            Set(value As EntornoBD)
                _entornoActivo = value
            End Set
        End Property

        Public Shared Sub Iniciar(usuario As Usuario)
            If usuario Is Nothing Then Throw New ArgumentNullException(NameOf(usuario))

            SyncLock Candado
                If _usuario IsNot Nothing Then
                    Throw New InvalidOperationException("La sesión ya está iniciada.")
                End If
                _usuario = usuario
            End SyncLock
        End Sub

        Public Shared Sub Cerrar()
            SyncLock Candado
                _usuario = Nothing
                _entornoActivo = Nothing
            End SyncLock
        End Sub

    End Class

End Namespace
