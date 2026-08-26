Option Strict Off   ' Usa los DTO portados.

Imports System.IO
Imports ClosedXML.Excel

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Aplica precios ya calculados que vienen en un Excel. Es el botón «Aplicar Precios Excel»
    ''' de ActualizaPrecios.
    '''
    ''' El Excel lleva cuatro columnas y la hoja se llama «Hoja1»:
    ''' 1 FechaContrato · 2 IdContratoTarifa · 3 IdTarifaGrupo · 4 CodigoContrato.
    '''
    ''' Lo que hace por contrato es exactamente lo del original: comprobar con
    ''' GetContratoTarifaExcel que ese ContratoTarifa existe y, si existe, llamar a
    ''' aplicapreciosFromEcel. Ninguno de los dos se ha tocado.
    '''
    ''' La operación NO usa fechas del formulario: la fecha va en la columna 1 del Excel. En el
    ''' catálogo pedía «Fechas», y era arrastre —el original tiene comentada la línea que la
    ''' usaba—, así que se ha quitado.
    ''' </summary>
    Public Class AplicarPreciosDesdeExcel
        Inherits OperacionPorEntrada
        Implements IEntradasDesdeExcel

        Public Const NombreHoja As String = "Hoja1"

        ''' <summary>Una fila del Excel, ya leída.</summary>
        Private Class Fila
            Public Property Numero As Integer
            Public Property Fecha As Date?
            Public Property IdContratoTarifa As String = ""
            Public Property IdTarifaGrupo As String = ""
            Public Property CodigoContrato As String = ""

            Public ReadOnly Property Etiqueta As String
                Get
                    Return $"fila {Numero}: contrato {CodigoContrato}"
                End Get
            End Property
        End Class

        Private _filas As Dictionary(Of String, Fila)

        Public Function LeerEntradas(rutaExcel As String) As IReadOnlyList(Of String) _
            Implements IEntradasDesdeExcel.LeerEntradas

            Return Leer(rutaExcel).Select(Function(f) f.Etiqueta).ToList()

        End Function

        Private Shared Function Leer(rutaExcel As String) As List(Of Fila)

            Dim filas As New List(Of Fila)

            Using libro As New XLWorkbook(rutaExcel)

                ' El original hacía Worksheets("Hoja1") sin comprobar: si la hoja se llamaba de
                ' otra manera, NullReferenceException sin explicación. Aquí se dice.
                Dim hoja As IXLWorksheet = Nothing
                If Not libro.TryGetWorksheet(NombreHoja, hoja) Then
                    Throw New InvalidOperationException(
                        $"El libro no tiene una hoja llamada «{NombreHoja}». Tiene: " &
                        String.Join(", ", libro.Worksheets.Select(Function(h) h.Name)))
                End If

                Dim usado = hoja.RangeUsed()
                If usado Is Nothing Then Return filas

                For n = 2 To usado.LastRow().RowNumber()

                    Dim fila As New Fila With {
                        .Numero = n,
                        .IdContratoTarifa = Celda(hoja, n, 2),
                        .IdTarifaGrupo = Celda(hoja, n, 3),
                        .CodigoContrato = Celda(hoja, n, 4)
                    }

                    ' Fila vacía: se salta, no es un error.
                    If fila.IdContratoTarifa.Length = 0 AndAlso
                       fila.IdTarifaGrupo.Length = 0 AndAlso
                       fila.CodigoContrato.Length = 0 Then Continue For

                    Dim textoFecha = Celda(hoja, n, 1)
                    Dim f As Date
                    If Date.TryParse(textoFecha, f) Then fila.Fecha = f

                    filas.Add(fila)

                Next

            End Using

            Return filas

        End Function

        Private Shared Function Celda(hoja As IXLWorksheet, fila As Integer, columna As Integer) As String
            Dim c = hoja.Cell(fila, columna)
            If c.IsEmpty() Then Return ""
            Return c.Value.ToString().Trim()
        End Function

        Protected Overrides Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _filas = New Dictionary(Of String, Fila)

            If Not File.Exists(ctx.RutaExcel) Then
                Throw New FileNotFoundException($"No se encuentra el Excel: {ctx.RutaExcel}")
            End If

            For Each f In Leer(ctx.RutaExcel)
                _filas(f.Etiqueta) = f
            Next

            Return Task.CompletedTask

        End Function

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim fila As Fila = Nothing
            If _filas Is Nothing OrElse Not _filas.TryGetValue(entrada, fila) Then
                Return Task.FromResult(ResultadoEntrada.Fallo("no se ha podido leer la fila del Excel"))
            End If

            If fila.CodigoContrato.Length = 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("falta el código de contrato"))
            End If

            ' Se rellena el mismo DTO que montaba el original a partir de las cuatro columnas.
            Dim datos As New ContratoTarifa With {
                .IdContratoTarifa = fila.IdContratoTarifa,
                .CodigoContrato = fila.CodigoContrato,
                .FechaDesde = fila.Fecha,
                .IdTarifaGrupo = fila.IdTarifaGrupo
            }

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)

            Dim existente = funciones.GetContratoTarifaExcel(datos)
            If existente Is Nothing OrElse existente.IdContratoTarifa <= 0 Then
                ' Mismo motivo que escribía el original en su fichero de errores.
                Return Task.FromResult(ResultadoEntrada.SinDatos(
                    "no se ha encontrado el tarifagrupo al que aplicar precios"))
            End If

            ctx.AbortarSiCancelado()

            funciones.aplicapreciosFromEcel(datos)

            Return Task.FromResult(ResultadoEntrada.ConDatos(1, "precios aplicados"))

        End Function

    End Class

End Namespace
