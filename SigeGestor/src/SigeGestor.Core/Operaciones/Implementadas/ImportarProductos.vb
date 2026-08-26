Option Strict Off   ' Usa los DTO portados.

Imports System.IO
Imports ClosedXML.Excel

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Carga asignaciones de producto en bloque desde la plantilla de Excel. Es el botón
    ''' «Importar Producto» de ProductosAsig.
    '''
    ''' El insert es el mismo de siempre: FuncionesGenericas.InsertProductoAsignacionV2, con los
    ''' mismos quince argumentos y la misma conversión a «NULL» de los que vienen vacíos. Eso NO
    ''' se ha tocado, porque ese método construye el SQL a mano y espera exactamente esas
    ''' cadenas.
    '''
    ''' UNA FILA POR ENTRADA: cada fila del Excel es una entrada del ejecutor, así que se ve el
    ''' avance y una fila mala no tira las demás. El original recorría todo dentro de un Try y
    ''' cualquier excepción abortaba la importación entera dejándola a medias, sin decir en qué
    ''' fila se había quedado.
    ''' </summary>
    Public Class ImportarProductos
        Inherits OperacionPorEntrada
        Implements IEntradasDesdeExcel

        Public Const ClaveRedondear As String = "redondear"

        ''' <summary>
        ''' Las columnas de la plantilla, en orden. Es la única definición: la genera
        ''' <see cref="GenerarPlantillaProductos"/> y la lee esta clase.
        ''' </summary>
        Public Shared ReadOnly Columnas As String() = {
            "CodContrato",
            "TextoProducto",
            "FechaInicio",
            "FechaFinal",
            "Plazo",
            "PlazoCargado",
            "ImporteTotalPlazo",
            "Importe",
            "AntesIE(true/false)",
            "AplicarSobreConsumo(true/false)",
            "PrecioDia(true/false)",
            "AplicarPrecioConsumo(true/false)"
        }

        ''' <summary>
        ''' Filas leídas del Excel, por su etiqueta de entrada. Se llena en PrepararAsync porque
        ''' el Excel se lee una sola vez y las entradas ya vienen dadas por quien lanza.
        ''' </summary>
        Private _filas As Dictionary(Of String, Object())

        ''' <summary>
        ''' Lee la plantilla y devuelve una etiqueta por fila con datos. Lo llama la interfaz
        ''' antes de lanzar, para saber cuántas filas hay y poder pintar la tira de estados.
        ''' </summary>
        Public Function LeerEntradas(rutaExcel As String) As IReadOnlyList(Of String) _
            Implements IEntradasDesdeExcel.LeerEntradas

            Dim etiquetas As New List(Of String)

            For Each fila In Leer(rutaExcel)
                etiquetas.Add(Etiqueta(fila.Key, fila.Value))
            Next

            Return etiquetas

        End Function

        Private Shared Function Etiqueta(numeroFila As Integer, valores As Object()) As String
            Dim contrato = Texto(valores, 0)
            Dim producto = Texto(valores, 1)
            Return $"fila {numeroFila}: {contrato} · {producto}"
        End Function

        ''' <summary>
        ''' Lee la primera hoja a partir de la fila 2. Devuelve el número de fila real del Excel
        ''' junto a sus valores, para poder decir «fila 47» y que se encuentre.
        ''' </summary>
        Private Shared Function Leer(rutaExcel As String) As List(Of KeyValuePair(Of Integer, Object()))

            Dim filas As New List(Of KeyValuePair(Of Integer, Object()))

            Using libro As New XLWorkbook(rutaExcel)

                Dim hoja = libro.Worksheets.First()
                Dim usado = hoja.RangeUsed()
                If usado Is Nothing Then Return filas

                For Each fila In usado.Rows()

                    Dim n = fila.RowNumber()
                    If n = 1 Then Continue For   ' cabecera

                    Dim valores(Columnas.Length - 1) As Object
                    Dim algo = False

                    For c = 0 To Columnas.Length - 1
                        Dim celda = hoja.Cell(n, c + 1)
                        If celda.IsEmpty() Then Continue For
                        valores(c) = celda.Value.ToString()
                        algo = True
                    Next

                    ' Una fila entera vacía en medio no es un error: se salta. El original
                    ' llegaba a CLng(Nothing) y lanzaba InvalidCastException, que abortaba todo.
                    If algo Then filas.Add(New KeyValuePair(Of Integer, Object())(n, valores))

                Next

            End Using

            Return filas

        End Function

        Protected Overrides Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _filas = New Dictionary(Of String, Object())

            If Not File.Exists(ctx.RutaExcel) Then
                Throw New FileNotFoundException($"No se encuentra el Excel: {ctx.RutaExcel}")
            End If

            For Each fila In Leer(ctx.RutaExcel)
                _filas(Etiqueta(fila.Key, fila.Value)) = fila.Value
            Next

            Return Task.CompletedTask

        End Function

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim valores As Object() = Nothing
            If _filas Is Nothing OrElse Not _filas.TryGetValue(entrada, valores) Then
                Return Task.FromResult(ResultadoEntrada.Fallo("no se ha podido leer la fila del Excel"))
            End If

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)
            Dim redondear = ctx.Campo(ClaveRedondear) = "1"

            ' --- Contrato ---
            Dim codContrato As Long
            If Not Long.TryParse(Texto(valores, 0), codContrato) OrElse codContrato <= 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("el código de contrato no es un número"))
            End If

            Dim contrato = funciones.GetContrato(codContrato)
            If contrato Is Nothing OrElse contrato.IdContrato <= 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos("el contrato no existe en la base"))
            End If

            ' --- Producto, buscado por texto en el entorno del contrato ---
            Dim textoProducto = Texto(valores, 1)
            If textoProducto.Length = 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("falta el texto del producto"))
            End If

            Dim entornoProducto = If(String.Equals(contrato.Entorno, "E1"), "G1", "G2")
            Dim producto = funciones.GetProductosbyTextoProducto(textoProducto, entornoProducto)

            If producto Is Nothing OrElse producto.IdProducto <= 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos(
                    $"el producto '{textoProducto}' no existe en {entornoProducto}"))
            End If

            ' --- Resto de columnas, con la misma conversión a NULL del original ---
            Dim fechaInicial = ComoFechaSql(funciones.ToNullableDate(valores(2)))
            Dim fechaFinal = ComoFechaSql(funciones.ToNullableDate(valores(3)))

            Dim plazo = ComoEnteroSql(funciones.ToNullableInteger(valores(4)))
            Dim plazoCargado = ComoEnteroSql(funciones.ToNullableInteger(valores(5)))

            Dim importeTotal = ComoDecimalSql(funciones.ToNullableDecimal(valores(6)), redondear)
            Dim importe = ComoDecimalSql(funciones.ToNullableDecimal(valores(7)), redondear)

            Dim antesIe = ComoBooleanoSql(funciones.ToNullableBoolean(valores(8)))
            Dim sobreConsumo = funciones.ToNullableBoolean(valores(9)).GetValueOrDefault(False)
            Dim precioDia = If(funciones.ToNullableBoolean(valores(10)).GetValueOrDefault(False), "1", "0")
            Dim precioConsumo = funciones.ToNullableBoolean(valores(11)).GetValueOrDefault(False)

            ctx.AbortarSiCancelado()

            Dim filas = funciones.InsertProductoAsignacionV2(
                contrato.Entorno,
                producto.IdProductoGrupo,
                producto.IdProducto,
                contrato.IdContrato,
                fechaInicial,
                importe,
                If(contrato.IdTipoImpuesto, 0),
                antesIe,
                sobreConsumo,
                precioConsumo,
                precioDia,
                fechaFinal,
                plazo,
                plazoCargado,
                importeTotal)

            If filas <= 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("el insert no ha creado ninguna fila"))
            End If

            Return Task.FromResult(ResultadoEntrada.ConDatos(
                filas, $"{producto.TextoProducto} asignado al {codContrato}"))

        End Function

        ' ------------------------------------------------------------------
        ' Conversiones al formato de cadena que espera InsertProductoAsignacionV2.
        ' Ese método interpola directamente en el SQL, así que «NULL» tiene que llegar
        ' como la palabra NULL sin comillas y las fechas con comillas y en dd/MM/yyyy.
        ' ------------------------------------------------------------------

        Private Shared Function Texto(valores As Object(), indice As Integer) As String
            If valores Is Nothing OrElse indice >= valores.Length OrElse valores(indice) Is Nothing Then Return ""
            Return valores(indice).ToString().Trim()
        End Function

        Private Shared Function ComoFechaSql(fecha As Date?) As String
            If Not fecha.HasValue Then Return "NULL"
            Return $"'{fecha.Value:dd/MM/yyyy HH:mm:ss}'"
        End Function

        Private Shared Function ComoEnteroSql(valor As Integer?) As String
            If Not valor.HasValue Then Return "NULL"
            Return valor.Value.ToString()
        End Function

        ''' <summary>
        ''' Se fuerza el punto decimal con InvariantCulture. El original usaba CStr, que en una
        ''' máquina española escribe «12,5» y eso dentro del SQL se lee como dos argumentos.
        ''' </summary>
        Private Shared Function ComoDecimalSql(valor As Decimal?, redondear As Boolean) As String

            If Not valor.HasValue Then Return "NULL"

            Dim v = If(redondear, Math.Round(valor.Value, 2), valor.Value)
            Return v.ToString(Globalization.CultureInfo.InvariantCulture)

        End Function

        Private Shared Function ComoBooleanoSql(valor As Boolean?) As String
            If Not valor.HasValue Then Return "NULL"
            Return If(valor.Value, "1", "0")
        End Function

    End Class

End Namespace
