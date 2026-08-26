Namespace Modelos

    ''' <summary>
    ''' Una cifra del panel de inicio con su comparación y su serie para el sparkline.
    ''' El formato (miles, unidades, minutos) lo pone la interfaz, no el Core.
    ''' </summary>
    Public Class SerieMetrica

        Public Property Valor As Double

        ''' <summary>Valor del periodo anterior, para calcular el delta.</summary>
        Public Property ValorAnterior As Double

        ''' <summary>Últimos días, en orden. Se dibuja como sparkline.</summary>
        Public Property Serie As IReadOnlyList(Of Double) = Array.Empty(Of Double)()

        ''' <summary>
        ''' Diferencia absoluta con el periodo anterior. Positiva = ha subido.
        ''' </summary>
        Public ReadOnly Property Delta As Double
            Get
                Return Valor - ValorAnterior
            End Get
        End Property

        ''' <summary>Variación relativa, o Nothing si no hay base con la que comparar.</summary>
        Public ReadOnly Property DeltaPorcentual As Double?
            Get
                If ValorAnterior = 0 Then Return Nothing
                Return (Valor - ValorAnterior) / ValorAnterior * 100
            End Get
        End Property

    End Class

    ''' <summary>Las cuatro cifras de la cabecera del inicio.</summary>
    Public Class MetricasInicio

        Public Property Ejecuciones As New SerieMetrica

        Public Property Filas As New SerieMetrica

        Public Property ConError As New SerieMetrica

        ''' <summary>Duración media en segundos.</summary>
        Public Property DuracionMedia As New SerieMetrica

    End Class

End Namespace
