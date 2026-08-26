Imports System.Data
Imports System.Threading
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Modelos

Namespace Servicios

    Public Enum ResultadoLogin
        Correcto
        CredencialesIncorrectas
        SinConexion
        ConfiguracionIncompleta
        Cancelado
    End Enum

    Public Class RespuestaLogin

        Public Property Resultado As ResultadoLogin
        Public Property Usuario As Usuario
        Public Property Mensaje As String = ""

        Public ReadOnly Property Correcto As Boolean
            Get
                Return Resultado = ResultadoLogin.Correcto
            End Get
        End Property

        Public Shared Function Ok(usuario As Usuario) As RespuestaLogin
            Return New RespuestaLogin With {.Resultado = ResultadoLogin.Correcto, .Usuario = usuario}
        End Function

        Public Shared Function Fallo(resultado As ResultadoLogin, mensaje As String) As RespuestaLogin
            Return New RespuestaLogin With {.Resultado = resultado, .Mensaje = mensaje}
        End Function

    End Class

    ''' <summary>
    ''' Autenticación contra la tabla usuario de SigeTotal.
    '''
    ''' Diferencias respecto a FuncionesGenericas.UsuarioValidacion de ActualizaPrecios:
    '''  · La consulta va parametrizada. La original concatenaba login y contraseña en el SQL,
    '''    así que un usuario con una comilla en la clave podía alterar la consulta.
    '''  · Distingue «credenciales incorrectas» de «no llego al servidor», que antes se
    '''    confundían porque la excepción se tragaba en un Catch y se devolvía Nothing.
    '''  · No devuelve la contraseña en el objeto de usuario.
    '''
    ''' Lo que NO cambia, y conviene tener presente: las contraseñas se guardan y comparan en
    ''' claro en la columna Password. Arreglarlo exige migrar la tabla a un hash con sal
    ''' (PBKDF2 o similar) y coordinarlo con el resto de aplicaciones que leen esa tabla.
    ''' </summary>
    Public Class ServicioAutenticacion

        ''' <summary>
        ''' Acceso especial de informes que ya existía en ActualizaPrecios. Es una credencial
        ''' fija en el binario: no es autenticación real, solo un atajo para el usuario de
        ''' informes. Se mantiene porque está en uso.
        ''' </summary>
        Private Const LoginInformes As String = "ReportSige"

        Private ReadOnly _entornos As RepositorioEntornos

        Public Sub New(entornos As RepositorioEntornos)
            _entornos = entornos
        End Sub

        Public Async Function AutenticarAsync(login As String,
                                              clave As String,
                                              Optional ct As CancellationToken = Nothing) As Task(Of RespuestaLogin)

            If String.IsNullOrWhiteSpace(login) OrElse String.IsNullOrWhiteSpace(clave) Then
                Return RespuestaLogin.Fallo(ResultadoLogin.CredencialesIncorrectas, "Falta el usuario o la clave.")
            End If

            login = login.Trim()

            If String.Equals(login, LoginInformes, StringComparison.Ordinal) AndAlso
               String.Equals(clave, LoginInformes, StringComparison.Ordinal) Then

                Return RespuestaLogin.Ok(New Usuario With {
                    .Nombre = "Report",
                    .Login = "REPORT SIGE",
                    .EsAccesoInformes = True
                })
            End If

            Dim cadena As String
            Try
                ' El login siempre valida contra Producción: es la única base donde los
                ' usuarios son la fuente de verdad. El entorno de trabajo se elige después,
                ' por operación.
                cadena = _entornos.CadenaConexion(ClaveEntorno.Produccion)
            Catch ex As ConfiguracionNoDisponibleException
                Return RespuestaLogin.Fallo(ResultadoLogin.ConfiguracionIncompleta, ex.Message)
            End Try

            Const consulta As String =
                "SELECT TOP 1 nombre, login " &
                "FROM usuario " &
                "WHERE login = @login AND Password = @clave AND LEN(LTRIM(RTRIM(nombre))) > 1 " &
                "ORDER BY nombre"

            Try
                Using conexion As New SqlConnection(cadena)
                    Await conexion.OpenAsync(ct).ConfigureAwait(False)

                    Using comando As New SqlCommand(consulta, conexion)
                        comando.CommandTimeout = 20
                        comando.Parameters.Add("@login", SqlDbType.VarChar, 100).Value = login
                        comando.Parameters.Add("@clave", SqlDbType.VarChar, 100).Value = clave

                        Using lector = Await comando.ExecuteReaderAsync(ct).ConfigureAwait(False)
                            If Not Await lector.ReadAsync(ct).ConfigureAwait(False) Then
                                Return RespuestaLogin.Fallo(
                                    ResultadoLogin.CredencialesIncorrectas, "Usuario o clave incorrectos.")
                            End If

                            Return RespuestaLogin.Ok(New Usuario With {
                                .Nombre = If(lector.IsDBNull(0), "", lector.GetString(0)).Trim(),
                                .Login = If(lector.IsDBNull(1), "", lector.GetString(1)).Trim()
                            })
                        End Using
                    End Using
                End Using

            Catch ex As OperationCanceledException
                Return RespuestaLogin.Fallo(ResultadoLogin.Cancelado, "Validación cancelada.")

            Catch ex As SqlException
                Return RespuestaLogin.Fallo(
                    ResultadoLogin.SinConexion,
                    $"No se puede conectar con Producción. Comprueba la VPN.{Environment.NewLine}{ex.Message}")

            Catch ex As Exception
                Return RespuestaLogin.Fallo(ResultadoLogin.SinConexion, ex.Message)
            End Try

        End Function

    End Class

End Namespace
