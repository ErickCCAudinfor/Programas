Option Strict Off   ' Usa los DTO portados.

Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Asigna un producto a una lista de contratos, con su importe, sus fechas y su forma de
    ''' aplicación. Es el formulario ProductosAsig de ActualizaPrecios, con el botón «Insertar».
    '''
    ''' El insert es el mismo: FuncionesGenericas.InsertProductoAsignacion, con los doce
    ''' argumentos en el mismo orden y la fecha final como «NULL» o entre comillas, que es lo que
    ''' espera porque construye el SQL a mano.
    '''
    ''' DOS DIFERENCIAS CON EL ORIGINAL, las dos por corregir algo:
    '''
    ''' 1) El original miraba el entorno del PRIMER contrato de la lista, cargaba solo los
    '''    productos de ese entorno y los aplicaba a todos. Con una lista que mezclase luz y gas
    '''    metía un producto de luz en contratos de gas sin avisar. Aquí el desplegable trae los
    '''    de los dos entornos y cada contrato se comprueba: si no cuadra, esa entrada se marca y
    '''    las demás siguen.
    '''
    ''' 2) El original solo insertaba si la casilla «Insertar» estaba marcada, y si no, no hacía
    '''    nada en absoluto —el botón cambiaba a «Actualizar» pero no había código de update—.
    '''    Aquí siempre inserta, que es lo único que llegaba a hacer.
    ''' </summary>
    Public Class AnadirProductos
        Inherits OperacionPorEntrada
        Implements IRellenaCampos

        Public Const ClaveProducto As String = "producto"
        Public Const ClaveImporte As String = "importe"
        Public Const ClaveImpuesto As String = "impuesto"
        Public Const ClaveDesde As String = "fechaInicial"
        Public Const ClaveHasta As String = "fechaFinal"
        Public Const ClaveAntesIe As String = "antesIe"
        Public Const ClaveSobreConsumo As String = "sobreConsumo"
        Public Const ClavePrecioSobreConsumo As String = "precioSobreConsumo"
        Public Const ClavePrecioDia As String = "precioDia"

        Private ReadOnly _contratos As New RepositorioContratos()

        ' ==================================================================
        ' Relleno automático al elegir producto
        ' ==================================================================

        Public Function RellenarAsync(cadenaConexion As String,
                                      clave As String,
                                      valor As String) As Task(Of IReadOnlyDictionary(Of String, String)) _
            Implements IRellenaCampos.RellenarAsync

            Dim vacio As IReadOnlyDictionary(Of String, String) = New Dictionary(Of String, String)()

            If clave <> ClaveProducto Then Return Task.FromResult(vacio)

            Dim idProducto As Long
            If Not Long.TryParse(valor, idProducto) OrElse idProducto <= 0 Then
                Return Task.FromResult(vacio)
            End If

            Return Task.Run(
                Function() As IReadOnlyDictionary(Of String, String)

                    Dim producto = BuscarProducto(cadenaConexion, idProducto)
                    If producto Is Nothing Then Return vacio

                    Dim valores As New Dictionary(Of String, String)

                    ' Lo mismo que hacía ComboBox1_SelectedIndexChanged del original.
                    If producto.Importe.HasValue Then
                        valores(ClaveImporte) = producto.Importe.Value.ToString(
                            Globalization.CultureInfo.CurrentCulture)
                    End If

                    valores(ClaveAntesIe) = SiNo(producto.AntesIE)
                    valores(ClaveSobreConsumo) = SiNo(producto.SobreConsumo)
                    valores(ClavePrecioSobreConsumo) = SiNo(producto.PrecioSobreConsumo)

                    If producto.IdTipoImpuesto.HasValue AndAlso producto.IdTipoImpuesto.Value > 0 Then
                        valores(ClaveImpuesto) = producto.IdTipoImpuesto.Value.ToString()
                    End If

                    Return valores

                End Function)

        End Function

        Private Shared Function SiNo(valor As Boolean?) As String
            Return If(valor.GetValueOrDefault(False), "1", "0")
        End Function

        ''' <summary>
        ''' Busca el producto por Id entre los de los dos entornos. Se usa GetProductosbyEntorno,
        ''' que es lo portado, en vez de escribir una consulta nueva por un Id.
        ''' </summary>
        Private Shared Function BuscarProducto(cadenaConexion As String, idProducto As Long) As Producto

            Dim funciones As New FuncionesGenericas(cadenaConexion)

            For Each entorno In {"G1", "G2"}
                Dim lista = funciones.GetProductosbyEntorno(entorno)
                If lista Is Nothing Then Continue For

                Dim p = lista.Where(Function(x) x.IdProducto = idProducto).FirstOrDefault()
                If p IsNot Nothing Then Return p
            Next

            Return Nothing

        End Function

        ' ==================================================================
        ' Ejecución
        ' ==================================================================

        Private _producto As Producto

        Protected Overrides Function PrepararAsync(ctx As ContextoEjecucion) As Task

            Dim idProducto As Long
            If Not Long.TryParse(ctx.Campo(ClaveProducto), idProducto) OrElse idProducto <= 0 Then
                Throw New InvalidOperationException("Elige el producto que se va a asignar.")
            End If

            _producto = BuscarProducto(ctx.CadenaConexion, idProducto)
            If _producto Is Nothing Then
                Throw New InvalidOperationException(
                    $"El producto {idProducto} ya no existe en este entorno. Vuelve a elegirlo.")
            End If

            Return Task.CompletedTask

        End Function

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim encontrados = Await _contratos.ResolverAsync(
                ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

            Dim codigos = encontrados.Where(Function(c) c.CodigoContrato > 0) _
                                     .Select(Function(c) c.CodigoContrato) _
                                     .Distinct().ToList()

            If codigos.Count = 0 Then Return ResultadoEntrada.SinDatos("no existe")

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)

            ' Fecha inicial: el original la mandaba como Date, no como cadena, así que aquí
            ' también. Sin fecha se usa hoy, que es lo que daba el DateTimePicker por defecto.
            Dim desde = FechaDe(ctx.Campo(ClaveDesde), Date.Today)

            ' Fecha final: cadena «NULL» o entre comillas en dd/MM/yyyy, como en el original.
            Dim hasta = "NULL"
            Dim hastaFecha = FechaDe(ctx.Campo(ClaveHasta), Nothing)
            If hastaFecha.HasValue Then hasta = $"'{hastaFecha.Value:dd/MM/yyyy}'"

            If hastaFecha.HasValue AndAlso hastaFecha.Value < desde Then
                Return ResultadoEntrada.Fallo("la fecha final es anterior a la inicial")
            End If

            Dim importe = ImporteDe(ctx.Campo(ClaveImporte), _producto)

            Dim antesIe = ctx.Campo(ClaveAntesIe) = "1"
            Dim sobreConsumo = ctx.Campo(ClaveSobreConsumo) = "1"
            Dim precioSobreConsumo = ctx.Campo(ClavePrecioSobreConsumo) = "1"
            Dim precioDia = ctx.Campo(ClavePrecioDia) = "1"

            Dim insertados = 0
            Dim motivos As New List(Of String)

            For Each codigo In codigos
                ctx.AbortarSiCancelado()

                Dim contrato = funciones.GetContrato(codigo)
                If contrato Is Nothing OrElse contrato.IdContrato <= 0 Then
                    motivos.Add($"{codigo}: no existe")
                    Continue For
                End If

                ' El entorno del contrato es E1/E2; el del producto, G1/G2.
                Dim entornoContrato = If(String.Equals(contrato.Entorno, "E1"), "G1", "G2")
                If Not String.Equals(entornoContrato, _producto.Entorno) Then
                    motivos.Add($"{codigo}: es de {Tipo(contrato.Entorno)} y el producto es de {Tipo(_producto.Entorno)}")
                    Continue For
                End If

                ' Igual que el original: si el contrato tiene su propio tipo de impuesto y se ha
                ' elegido uno, manda el del contrato.
                Dim idImpuesto = ImpuestoDe(ctx.Campo(ClaveImpuesto))
                If If(contrato.IdTipoImpuesto, 0) <> 0 AndAlso idImpuesto <> 0 Then
                    idImpuesto = contrato.IdTipoImpuesto
                End If

                Dim filas = funciones.InsertProductoAsignacion(
                    contrato.Entorno,
                    _producto.IdProductoGrupo,
                    _producto.IdProducto,
                    contrato.IdContrato,
                    desde,
                    importe,
                    idImpuesto,
                    antesIe,
                    sobreConsumo,
                    precioSobreConsumo,
                    precioDia,
                    hasta)

                If filas > 0 Then
                    insertados += 1
                Else
                    motivos.Add($"{codigo}: el insert no creó ninguna fila")
                End If
            Next

            If insertados = 0 Then
                Return ResultadoEntrada.Fallo(String.Join(" · ", motivos))
            End If

            If motivos.Count > 0 Then
                Return ResultadoEntrada.ConDatos(
                    insertados, $"{insertados} asignados, {motivos.Count} no: {String.Join(" · ", motivos)}")
            End If

            Return ResultadoEntrada.ConDatos(
                insertados, Redaccion.Cuenta(insertados, "producto asignado",
                                                          "productos asignados"))

        End Function

        Private Shared Function Tipo(entorno As String) As String
            Select Case entorno
                Case "E1", "G1" : Return "luz"
                Case "E2", "G2" : Return "gas"
                Case Else : Return entorno
            End Select
        End Function

        ''' <summary>Las fechas llegan del formulario en ISO.</summary>
        Private Shared Function FechaDe(texto As String, porDefecto As Date?) As Date?
            Dim f As Date
            If Date.TryParse(texto, Globalization.CultureInfo.InvariantCulture,
                             Globalization.DateTimeStyles.None, f) Then Return f
            Return porDefecto
        End Function

        ''' <summary>
        ''' Importe escrito, y si está vacío el del propio producto: es lo que el original dejaba
        ''' puesto en el NumericUpDown al elegirlo.
        ''' </summary>
        Private Shared Function ImporteDe(texto As String, producto As Producto) As Decimal

            Dim d As Decimal
            If Decimal.TryParse(texto, Globalization.NumberStyles.Number,
                                Globalization.CultureInfo.CurrentCulture, d) Then Return d

            Return producto.Importe.GetValueOrDefault(0D)

        End Function

        Private Shared Function ImpuestoDe(texto As String) As Long
            Dim n As Long
            If Long.TryParse(texto, n) AndAlso n > 0 Then Return n
            Return 0
        End Function

    End Class

End Namespace
