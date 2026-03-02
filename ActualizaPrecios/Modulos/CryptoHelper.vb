Imports System.Security.Cryptography
Imports System.Text

Public Module CryptoHelper

    '
    Private ReadOnly Key As Byte() =
        Encoding.UTF8.GetBytes("9FvK7xQ2LmP4aR8TzWc3HsN6JdY1uEoB") ' 32 chars = AES256

    Public Function Cifrar(textoPlano As String) As String
        If String.IsNullOrEmpty(textoPlano) Then Return textoPlano

        Using aes As Aes = Aes.Create()
            aes.Key = Key
            aes.GenerateIV()

            Using encryptor = aes.CreateEncryptor()
                Dim datos = Encoding.UTF8.GetBytes(textoPlano)
                Dim cifrado = encryptor.TransformFinalBlock(datos, 0, datos.Length)

                ' Guardamos IV + datos
                Dim resultado(aes.IV.Length + cifrado.Length - 1) As Byte
                Buffer.BlockCopy(aes.IV, 0, resultado, 0, aes.IV.Length)
                Buffer.BlockCopy(cifrado, 0, resultado, aes.IV.Length, cifrado.Length)

                Return Convert.ToBase64String(resultado)
            End Using
        End Using
    End Function

    Public Function Descifrar(textoCifrado As String) As String
        If String.IsNullOrEmpty(textoCifrado) Then Return textoCifrado

        Dim datos = Convert.FromBase64String(textoCifrado)

        Using aes As Aes = Aes.Create()
            aes.Key = Key

            Dim iv(15) As Byte
            Buffer.BlockCopy(datos, 0, iv, 0, 16)
            aes.IV = iv

            Dim contenido(datos.Length - 17) As Byte
            Buffer.BlockCopy(datos, 16, contenido, 0, contenido.Length)

            Using decryptor = aes.CreateDecryptor()
                Dim descifrado = decryptor.TransformFinalBlock(contenido, 0, contenido.Length)
                Return Encoding.UTF8.GetString(descifrado)
            End Using
        End Using
    End Function

End Module
