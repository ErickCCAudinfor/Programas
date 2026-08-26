Imports System.Security.Cryptography
Imports System.Text

Namespace Seguridad

    ''' <summary>
    ''' AES-256 compatible byte a byte con el CryptoHelper de ActualizaPrecios, para poder
    ''' reutilizar los ficheros de configuración que ya existen sin volver a cifrarlos.
    '''
    ''' PENDIENTE DE MEJORA: la clave está en el binario, así que cualquiera que tenga el .exe
    ''' puede descifrar el JSON. Lo correcto sería DPAPI (ProtectedData, ligada a la máquina o
    ''' al usuario) o el Almacén de credenciales de Windows. No se cambia ahora porque rompería
    ''' la compatibilidad con EmpresasBD.json; queda como tarea aparte.
    ''' </summary>
    Public Module Cifrado

        Private ReadOnly Clave As Byte() =
            Encoding.UTF8.GetBytes("9FvK7xQ2LmP4aR8TzWc3HsN6JdY1uEoB")   ' 32 chars = AES-256

        Private Const TamanoIv As Integer = 16

        Public Function Cifrar(textoPlano As String) As String
            If String.IsNullOrEmpty(textoPlano) Then Return textoPlano

            Using aes As Aes = Aes.Create()
                aes.Key = Clave
                aes.GenerateIV()

                Using cifrador = aes.CreateEncryptor()
                    Dim datos = Encoding.UTF8.GetBytes(textoPlano)
                    Dim cifrado = cifrador.TransformFinalBlock(datos, 0, datos.Length)

                    ' Se guarda IV + datos, igual que en ActualizaPrecios.
                    Dim resultado(aes.IV.Length + cifrado.Length - 1) As Byte
                    Buffer.BlockCopy(aes.IV, 0, resultado, 0, aes.IV.Length)
                    Buffer.BlockCopy(cifrado, 0, resultado, aes.IV.Length, cifrado.Length)

                    Return Convert.ToBase64String(resultado)
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Descifra un valor del JSON. Si el texto no está cifrado (o está corrupto) devuelve
        ''' el original en lugar de reventar: así un fichero editado a mano da un error de
        ''' conexión comprensible en vez de una excepción de criptografía.
        ''' </summary>
        Public Function Descifrar(textoCifrado As String) As String
            If String.IsNullOrEmpty(textoCifrado) Then Return textoCifrado

            Try
                Dim datos = Convert.FromBase64String(textoCifrado)
                If datos.Length <= TamanoIv Then Return textoCifrado

                Using aes As Aes = Aes.Create()
                    aes.Key = Clave

                    Dim iv(TamanoIv - 1) As Byte
                    Buffer.BlockCopy(datos, 0, iv, 0, TamanoIv)
                    aes.IV = iv

                    Dim contenido(datos.Length - TamanoIv - 1) As Byte
                    Buffer.BlockCopy(datos, TamanoIv, contenido, 0, contenido.Length)

                    Using descifrador = aes.CreateDecryptor()
                        Dim descifrado = descifrador.TransformFinalBlock(contenido, 0, contenido.Length)
                        Return Encoding.UTF8.GetString(descifrado)
                    End Using
                End Using

            Catch ex As FormatException
                Return textoCifrado
            Catch ex As CryptographicException
                Return textoCifrado
            End Try
        End Function

    End Module

End Namespace
