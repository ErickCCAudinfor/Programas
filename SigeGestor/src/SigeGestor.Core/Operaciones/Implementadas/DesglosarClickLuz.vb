Option Strict Off   ' Usa FuncionesGenericas y ClickFac, portados sin Option Strict.

Imports System.Data
Imports System.Globalization
Imports System.Text.RegularExpressions
Imports ClosedXML.Excel
Imports SigeGestor.Core.Configuracion

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Desglosa la descripción del click de cada factura y lo entrega en un Excel. Es el
    ''' Button15 («Desglosar click Luz») de ActualizaPrecios.
    '''
    ''' QUÉ HACE. Las líneas de factura con concepto 30006 llevan el click descrito en texto,
    ''' con una pinta parecida a esta:
    '''
    '''     ... 35,5% ... (Coste P1 = 1234,5*(0,08+0,012))
    '''
    ''' De ahí se saca el porcentaje, el periodo, el consumo y el precio del click, que sale de
    ''' EVALUAR la expresión aritmética del paréntesis. El original lo hace con
    ''' DataTable.Compute y se mantiene: es la forma más corta de evaluar «0,08+0,012» sin
    ''' escribir un intérprete, y esa cadena la genera SIGE, no el usuario.
    '''
    ''' UNA ENTRADA POR FACTURA, al contrario que el original, que recorría la lista entera
    ''' dentro de un Task.Run. Allí una descripción con una expresión que no evaluase lanzaba y
    ''' se perdía el desglose de TODAS las facturas —el catch de GetClickDesglosado hace Throw—;
    ''' aquí esa factura se marca y las demás siguen.
    ''' </summary>
    Public Class DesglosarClickLuz
        Inherits OperacionPorEntrada

        ''' <summary>
        ''' Del original, tal cual. Captura porcentaje, periodo, consumo y la expresión del
        ''' precio; el «(?:,\d+)?» es porque los decimales vienen con coma.
        ''' </summary>
        Private Shared ReadOnly Patron As New Regex(
            "(\d+(?:,\d+)?)%.*\((Coste P\d+) = (\d+(?:,\d+)?)\*\((.+)\)?",
            RegexOptions.Compiled)

        Private Shared ReadOnly PatronPorcentaje As New Regex(
            "(\d{1,3}(?:,\d{1,3})?)%", RegexOptions.Compiled)

        Private ReadOnly _clicks As New List(Of ClickFac)
        Private _carpeta As String = ""
        Private _ruta As String = ""

        Protected Overrides Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _clicks.Clear()

            ' El original lo dejaba en Escritorio\ClickFacs, fuera de ConsultasBO. Aquí va dentro,
            ' como todo lo que genera la aplicación.
            _carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                          RutasSalida.Asegurar("ClickFacs"),
                          ctx.CarpetaDestino)

            _ruta = ""
            Return Task.CompletedTask

        End Function

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim factura = entrada.Trim()
            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)

            Dim lineas = funciones.GetFacClick(factura)

            If lineas Is Nothing OrElse lineas.Count = 0 Then
                ' El original no distinguía esto: la factura desaparecía del recuento.
                Return Task.FromResult(ResultadoEntrada.SinDatos("sin líneas de click (concepto 30006)"))
            End If

            Dim desglosadas = 0

            For Each linea In lineas

                Dim texto = If(linea.Descripcion, "")
                Dim coincide = Patron.Match(texto)

                If coincide.Success Then
                    Dim periodo = coincide.Groups(2).Value
                    Dim consumo = Convert.ToDecimal(coincide.Groups(3).Value, Cultura)
                    Dim expresion = coincide.Groups(4).Value.Trim()

                    ' Si la expresión no evalúa se deja la línea sin desglosar y se sigue. El
                    ' original relanzaba y se llevaba por delante el desglose de todas.
                    Dim precio As Decimal
                    If Evaluar(expresion, precio) Then
                        linea.ClickCalculado = precio
                        linea.ConsumokWh = consumo
                        linea.Periodo = periodo.Replace("Coste", "")
                        desglosadas += 1
                    End If
                End If

                Dim conPorcentaje = PatronPorcentaje.Match(texto)
                linea.Porcentaje = If(conPorcentaje.Success,
                                      Convert.ToDecimal(conPorcentaje.Groups(1).Value.Replace(",", "."),
                                                        CultureInfo.InvariantCulture),
                                      0D)

                _clicks.Add(linea)
            Next

            If desglosadas = 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos(
                    $"{Redaccion.Cuenta(lineas.Count, "línea", "líneas")} de click, pero " &
                    "ninguna con el formato «(Coste Px = consumo*(precio))»"))
            End If

            Return Task.FromResult(ResultadoEntrada.ConDatos(
                desglosadas, Redaccion.Cuenta(desglosadas, "línea desglosada", "líneas desglosadas")))

        End Function

        Protected Overrides Function CerrarAsync(ctx As ContextoEjecucion,
                                                 resultado As ResultadoOperacion) As Task

            If _clicks.Count = 0 Then
                resultado.Mensaje = "No hay ninguna línea de click que desglosar"
                Return Task.CompletedTask
            End If

            _ruta = IO.Path.Combine(
                _carpeta, $"DesglosadoClick_{Date.Today:ddMMyyyy}_{DateTime.Now:HHmm}.xlsx")

            Escribir(_clicks, _ruta)

            resultado.Mensaje = Redaccion.Unir(
                IO.Path.GetFileName(_ruta),
                Redaccion.Cuenta(_clicks.Count, "línea", "líneas"))

            resultado.AnadirSalidas(_ruta)
            Return Task.CompletedTask

        End Function

        ''' <summary>
        ''' El Excel, con las mismas ocho columnas y en el mismo orden que
        ''' ValidacionExcel.GuardarEnExcelClick del original.
        '''
        ''' La última, ImporteBaseCalculado, es el producto del click por el consumo: es lo que
        ''' se compara con el ImporteBaseSIGE de la columna 3 para ver si cuadra.
        ''' </summary>
        Private Shared Sub Escribir(clicks As List(Of ClickFac), ruta As String)

            Using libro As New XLWorkbook()

                Dim hoja = libro.Worksheets.Add("Datos")

                Dim cabeceras = {"NFactura", "Descripcion", "ImporteBaseSIGE", "Periodo",
                                 "Porcentaje", "ConsumokWh", "ClickCalculado", "ImporteBaseCalculado"}

                For i = 0 To cabeceras.Length - 1
                    hoja.Cell(1, i + 1).Value = cabeceras(i)
                Next
                hoja.Row(1).Style.Font.Bold = True

                Dim fila = 2
                For Each c In clicks
                    hoja.Cell(fila, 1).Value = c.NFactura
                    hoja.Cell(fila, 2).Value = c.Descripcion
                    hoja.Cell(fila, 3).Value = c.ImporteBase
                    hoja.Cell(fila, 4).Value = c.Periodo
                    hoja.Cell(fila, 5).Value = c.Porcentaje
                    hoja.Cell(fila, 6).Value = c.ConsumokWh
                    hoja.Cell(fila, 7).Value = c.ClickCalculado
                    hoja.Cell(fila, 8).Value = c.ClickCalculado * c.ConsumokWh
                    fila += 1
                Next

                hoja.ColumnsUsed.AdjustToContents()
                libro.SaveAs(ruta)

            End Using

        End Sub

        Private Shared ReadOnly Cultura As New CultureInfo("es-ES")

        ''' <summary>
        ''' Evalúa la expresión del precio. Devuelve False si no se puede en vez de lanzar.
        '''
        ''' Es EvaluarExpresion del original: cambia las comas por puntos, quita el paréntesis
        ''' que la expresión regular deja colgando al final y lo pasa a DataTable.Compute, que
        ''' entiende «0,08+0,012» sin más. Con InvariantCulture a propósito: después del Replace
        ''' el separador decimal ya es el punto.
        ''' </summary>
        Private Shared Function Evaluar(expresion As String, ByRef resultado As Decimal) As Boolean

            Try
                Dim limpia = expresion.Replace(",", ".").TrimEnd(")"c)
                Dim valor = New DataTable().Compute(limpia, Nothing)

                resultado = Convert.ToDecimal(valor, CultureInfo.InvariantCulture)
                Return True

            Catch ex As Exception
                resultado = 0D
                Return False
            End Try

        End Function

    End Class

End Namespace
