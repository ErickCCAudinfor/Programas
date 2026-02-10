Imports System.Security.Cryptography
Imports System.Text

Public Module CryptoHelper

    Public Function Cifrar(textoPlano As String) As String
        If String.IsNullOrEmpty(textoPlano) Then Return textoPlano

        Dim datos = Encoding.UTF8.GetBytes(textoPlano)

        Dim cifrado = ProtectedData.Protect(
            datos,
            Nothing,
            DataProtectionScope.CurrentUser
        )

        Return Convert.ToBase64String(cifrado)
    End Function

    Public Function Descifrar(textoCifrado As String) As String
        If String.IsNullOrEmpty(textoCifrado) Then Return textoCifrado

        Dim datos = Convert.FromBase64String(textoCifrado)

        Dim descifrado = ProtectedData.Unprotect(
            datos,
            Nothing,
            DataProtectionScope.CurrentUser
        )

        Return Encoding.UTF8.GetString(descifrado)
    End Function

End Module
