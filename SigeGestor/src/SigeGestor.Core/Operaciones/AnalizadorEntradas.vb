Imports System.Text.RegularExpressions

Namespace Operaciones

    ''' <summary>Qué clase de lista se ha pegado. Determina cómo se valida cada elemento.</summary>
    Public Enum TipoLista
        Contratos
        Cups
        Cifs
        Facturas
    End Enum

    ''' <summary>
    ''' Resultado de analizar lo que el usuario ha pegado. Nunca falla: separa lo que vale de
    ''' lo que no y deja que la interfaz lo cuente.
    ''' </summary>
    Public Class ResultadoAnalisis

        Public Property Validos As IReadOnlyList(Of String) = Array.Empty(Of String)()
        Public Property Descartados As IReadOnlyList(Of String) = Array.Empty(Of String)()
        Public Property Repetidos As Integer

        Public ReadOnly Property HayAlgo As Boolean
            Get
                Return Validos.Count > 0
            End Get
        End Property

    End Class

    ''' <summary>
    ''' Convierte el texto pegado en una lista limpia.
    '''
    ''' En ActualizaPrecios esto era SplitEntrada, dentro de Form1 y con el separador a mano
    ''' en cada llamada. Aquí se separa por comas, saltos de línea, tabuladores y punto y coma
    ''' a la vez, porque lo que se pega viene de Excel, de un correo o de SSMS y cada sitio
    ''' usa el suyo.
    '''
    ''' Lo que no encaja NO se rechaza en bloque: se aparta y se informa de cuántos han sido.
    ''' Bloquear toda la operación porque una línea traía un encabezado pegado por error es
    ''' peor que decir «216 reconocidos, 1 descartado».
    ''' </summary>
    Public Module AnalizadorEntradas

        ' ControlChars y no vbCr/vbLf/vbTab: esas constantes son String y con Option Strict On
        ' no convierten a Char.
        Private ReadOnly Separadores As Char() =
            {","c, ";"c, ControlChars.Cr, ControlChars.Lf, ControlChars.Tab, "|"c}

        ' CUPS español: ES + 16 dígitos + 2 letras, y a veces un sufijo de 1-2 caracteres.
        Private ReadOnly PatronCups As New Regex("^ES\d{16}[A-Z]{2}[0-9A-Z]{0,2}$",
                                                RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        ' CIF/NIF: 9 caracteres alfanuméricos. Deliberadamente laxo: aquí no se valida el
        ' dígito de control, solo se descarta lo que no puede ser un identificador.
        Private ReadOnly PatronCif As New Regex("^[A-Z0-9][0-9]{7}[A-Z0-9]$",
                                                RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        Private ReadOnly PatronSoloDigitos As New Regex("^\d{1,18}$", RegexOptions.Compiled)

        Public Function Analizar(texto As String, tipo As TipoLista) As ResultadoAnalisis

            Dim resultado As New ResultadoAnalisis
            If String.IsNullOrWhiteSpace(texto) Then Return resultado

            Dim validos As New List(Of String)
            Dim descartados As New List(Of String)
            Dim vistos As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim repetidos = 0

            For Each bruto In texto.Split(Separadores, StringSplitOptions.RemoveEmptyEntries)
                Dim pieza = Limpiar(bruto)
                If pieza.Length = 0 Then Continue For

                If Not EsValido(pieza, tipo) Then
                    If Not descartados.Contains(pieza, StringComparer.OrdinalIgnoreCase) Then
                        descartados.Add(pieza)
                    End If
                    Continue For
                End If

                If Not vistos.Add(pieza) Then
                    repetidos += 1
                    Continue For
                End If

                validos.Add(pieza)
            Next

            resultado.Validos = validos
            resultado.Descartados = descartados
            resultado.Repetidos = repetidos
            Return resultado

        End Function

        ''' <summary>
        ''' Quita espacios, comillas y el apóstrofo que Excel cuela delante de los números
        ''' para forzarlos a texto.
        ''' </summary>
        Private Function Limpiar(pieza As String) As String
            Return pieza.Trim().Trim(""""c, "'"c, " "c)
        End Function

        Private Function EsValido(pieza As String, tipo As TipoLista) As Boolean
            Select Case tipo
                Case TipoLista.Contratos
                    ' El código de contrato es numérico: si no, no se puede consultar.
                    Return PatronSoloDigitos.IsMatch(pieza)

                Case TipoLista.Cups
                    Return PatronCups.IsMatch(pieza)

                Case TipoLista.Cifs
                    Return PatronCif.IsMatch(pieza)

                Case Else
                    ' Facturas: el identificador no siempre es numérico, así que basta con
                    ' que no traiga espacios ni sea absurdamente largo.
                    Return pieza.Length <= 40 AndAlso Not pieza.Contains(" ")
            End Select
        End Function

        ''' <summary>Cómo se llama esta lista en la interfaz.</summary>
        Public Function Etiqueta(tipo As TipoLista) As String
            Select Case tipo
                Case TipoLista.Contratos : Return "Contratos"
                Case TipoLista.Cups : Return "CUPS"
                Case TipoLista.Cifs : Return "CIF"
                Case Else : Return "Facturas"
            End Select
        End Function

        ''' <summary>Singular y plural para los mensajes de recuento.</summary>
        Public Function Unidad(tipo As TipoLista, cuantos As Integer) As String
            Select Case tipo
                Case TipoLista.Contratos : Return If(cuantos = 1, "contrato", "contratos")
                Case TipoLista.Cups : Return "CUPS"
                Case TipoLista.Cifs : Return "CIF"
                Case Else : Return If(cuantos = 1, "factura", "facturas")
            End Select
        End Function

        ''' <summary>Qué se espera, para poder explicar un descarte.</summary>
        Public Function FormatoEsperado(tipo As TipoLista) As String
            Select Case tipo
                Case TipoLista.Contratos : Return "solo números"
                Case TipoLista.Cups : Return "ES + 16 dígitos + 2 letras"
                Case TipoLista.Cifs : Return "9 caracteres, como B12345678"
                Case Else : Return "sin espacios"
            End Select
        End Function

        ''' <summary>Tipos de lista que admite una operación, según sus flags.</summary>
        Public Function TiposDe(definicion As DefinicionOperacion) As IReadOnlyList(Of TipoLista)
            Dim tipos As New List(Of TipoLista)
            If definicion.Pide(EntradasOperacion.Contratos) Then tipos.Add(TipoLista.Contratos)
            If definicion.Pide(EntradasOperacion.Cups) Then tipos.Add(TipoLista.Cups)
            If definicion.Pide(EntradasOperacion.Cifs) Then tipos.Add(TipoLista.Cifs)
            If definicion.Pide(EntradasOperacion.Facturas) Then tipos.Add(TipoLista.Facturas)
            Return tipos
        End Function

    End Module

End Namespace
