Option Strict Off   ' Usa FuncionesGenericas y ContratoTarifa, portados sin Option Strict.

Imports System.IO
Imports ClosedXML.Excel
Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Añade en bloque un calendario de tarifa a los contratos de un Excel: cierra el que
    ''' tienen y abre el nuevo desde la fecha indicada, aplicando después los precios. Es el
    ''' Button21 («Aña. Masv. Calendario Tarifa») de ActualizaPrecios.
    '''
    ''' ESCRIBE, Y NO SE DESHACE SOLO. Por cada contrato se hace un InsertTarifaGrupoCalendario
    ''' y un AplicarPreciosV2, los dos portados tal cual de FuncionesGenericas.
    '''
    ''' UNA ENTRADA POR CONTRATO, al contrario que el original, que recorría la lista dentro de
    ''' un Task.Run y volcaba los fallos a un fichero de texto al terminar
    ''' («ErroresCalendarioMasivo_...»). Con eso, a mitad de un masivo sobre Producción no había
    ''' forma de saber por dónde iba ni qué contrato había fallado hasta abrir el fichero. Aquí
    ''' cada contrato es una entrada con su motivo, y se pueden copiar los que no salieron.
    '''
    ''' LA REGLA QUE PROTEGE: no se crea un calendario con fecha ANTERIOR a la del calendario
    ''' actual. Es CambioCalendarioValido del original, y el motivo está en su comentario: al
    ''' cerrar el actual quedaría un intervalo inválido.
    ''' </summary>
    Public Class AnadirCalendarioTarifa
        Inherits OperacionPorEntrada
        Implements IEntradasDesdeExcel
        Implements IEsquemaExcel

        Public ReadOnly Property Esquema As EsquemaExcel Implements IEsquemaExcel.Esquema
            Get
                Return New EsquemaExcel(
                    {
                        New ColumnaExcel("CodContrato", "5048104", "Solo el número"),
                        New ColumnaExcel("FechaAplicarNueva", "01/01/2026",
                                         "Desde cuándo vale el calendario nuevo. No puede ser anterior a la del actual"),
                        New ColumnaExcel("FechaCierreAnterior", "31/12/2025",
                                         "Con qué fecha se cierra el calendario que tiene ahora"),
                        New ColumnaExcel("GrupoTarifaViejo", "IND_2025_S1",
                                         "El que tiene ahora. Se usa para encontrar su calendario"),
                        New ColumnaExcel("GrupoTarifaNuevo", "IND_2026_S1",
                                         "El que se le pone"),
                        New ColumnaExcel("EsQ", "false",
                                         "true o false. Con true se usa el perfil nuevo y los precios se aplican desde la fecha del calendario actual",
                                         obligatoria:=False),
                        New ColumnaExcel("MantenerPerfil", "true",
                                         "true o false. Con true se conserva el perfil de facturación que ya tenía",
                                         obligatoria:=False),
                        New ColumnaExcel("FechaAplicarPrecios", "01/01/2026",
                                         "La que recibe AplicarPreciosV2. Puede ir vacía",
                                         obligatoria:=False)
                    },
                    aviso:="Las columnas F y G admiten true/false, 1/0 y sí/no. El original solo " &
                           "entendía true/false y lo demás lo leía como false SIN AVISAR, que en " &
                           "una operación de escritura es de las cosas que peor sientan.")
            End Get
        End Property

        ''' <summary>Una etiqueta por fila, y de paso se cachea lo leído.</summary>
        Public Function LeerEntradas(rutaExcel As String) As IReadOnlyList(Of String) _
            Implements IEntradasDesdeExcel.LeerEntradas

            Return Leer(rutaExcel).Select(Function(f) f.Etiqueta).ToList()

        End Function

        ''' <summary>Una fila del Excel, ya interpretada.</summary>
        Private Class Fila
            Public Property Numero As Integer
            Public Property Datos As ContratoTarifa

            Public ReadOnly Property Etiqueta As String
                Get
                    Return $"fila {Numero}: contrato {Datos.CodigoContrato}"
                End Get
            End Property
        End Class

        Private _filas As Dictionary(Of String, Fila)

        ''' <summary>
        ''' Lee las ocho columnas. Es LeerContratosDesdeExcel de Helpers\Excel.vb del original,
        ''' con la misma correspondencia de columnas anotada al lado.
        ''' </summary>
        Private Shared Function Leer(rutaExcel As String) As List(Of Fila)

            Dim filas As New List(Of Fila)

            Using flujo As New FileStream(rutaExcel, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Dim libro As New XLWorkbook(flujo)
                Dim hoja = libro.Worksheets.First()

                Dim usado = hoja.RangeUsed()
                If usado Is Nothing Then Return filas

                For n = 2 To usado.LastRow().RowNumber()

                    Dim codigo As Long
                    If Not Long.TryParse(Texto(hoja, n, 1), codigo) OrElse codigo <= 0 Then Continue For

                    Dim aplicarNueva As Date
                    Date.TryParse(Texto(hoja, n, 2), aplicarNueva)

                    Dim cierreAnterior As Date
                    Date.TryParse(Texto(hoja, n, 3), cierreAnterior)

                    Dim aplicarPrecios As Date
                    Date.TryParse(Texto(hoja, n, 8), aplicarPrecios)

                    filas.Add(New Fila With {
                        .Numero = n,
                        .Datos = New ContratoTarifa With {
                            .CodigoContrato = codigo,                              ' 1
                            .FechaDesde = aplicarNueva,                            ' 2
                            .FechaHasta = cierreAnterior,                          ' 3
                            .textotarifagrupoViejo = Texto(hoja, n, 4),            ' 4
                            .textotarifagrupoNuevo = Texto(hoja, n, 5),            ' 5
                            .IsQ = Booleano(Texto(hoja, n, 6)),                    ' 6
                            .MantenerPerfil = Booleano(Texto(hoja, n, 7)),         ' 7
                            .FechaAplicar = aplicarPrecios                         ' 8
                        }
                    })
                Next
            End Using

            Return filas

        End Function

        Private Shared Function Texto(hoja As IXLWorksheet, fila As Integer, columna As Integer) As String
            Dim celda = hoja.Cell(fila, columna)
            If celda.IsEmpty() Then Return ""
            Return celda.Value.ToString().Trim()
        End Function

        ''' <summary>
        ''' true/false, 1/0 y sí/no. El original usaba Boolean.TryParse a secas, que solo
        ''' entiende «true» y «false»: un 1 en la columna se leía como False y el contrato se
        ''' procesaba con el perfil equivocado sin que nada lo dijera.
        ''' </summary>
        Private Shared Function Booleano(valor As String) As Boolean

            Select Case valor.Trim().ToLowerInvariant()
                Case "true", "1", "si", "sí", "s", "verdadero", "x" : Return True
                Case Else : Return False
            End Select

        End Function

        Protected Overrides Async Function PrepararAsync(ctx As ContextoEjecucion) As Task

            If Not File.Exists(ctx.RutaExcel) Then
                Throw New FileNotFoundException($"No se encuentra el Excel: {ctx.RutaExcel}")
            End If

            ' Antes de tocar nada: si la base no responde, GetContratoTarifabyCodContrato se come
            ' el error y devuelve Nothing, así que los 200 contratos saldrían como «no tiene ese
            ' grupo de tarifa» y quien lo lea acabará revisando el Excel para nada.
            Dim problema = Await ctx.ProbarConexionAsync().ConfigureAwait(False)
            If problema.Length > 0 Then
                Throw New InvalidOperationException(
                    $"No se puede conectar a {ctx.Entorno?.Nombre}: {problema}")
            End If

            _filas = New Dictionary(Of String, Fila)
            For Each f In Leer(ctx.RutaExcel)
                _filas(f.Etiqueta) = f
            Next

        End Function

        Protected Overrides Function ProcesarAsync(entrada As String,
                                                   ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim fila As Fila = Nothing
            If _filas Is Nothing OrElse Not _filas.TryGetValue(entrada, fila) Then
                Return Task.FromResult(ResultadoEntrada.Fallo("no se ha podido leer la fila del Excel"))
            End If

            Dim c = fila.Datos

            If c.textotarifagrupoViejo.Length = 0 OrElse c.textotarifagrupoNuevo.Length = 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo(
                    "faltan el grupo de tarifa viejo o el nuevo (columnas D y E)"))
            End If

            Dim funciones As New FuncionesGenericas(ctx.CadenaConexion)

            ' --- El calendario que tiene ahora ---
            Dim actual = funciones.GetContratoTarifabyCodContrato(c.CodigoContrato, c.textotarifagrupoViejo)

            If actual Is Nothing OrElse actual.IdContratoTarifa <= 0 Then
                ' El original lo metía en «noRealizados» y lo volcaba a un fichero al final.
                Return Task.FromResult(ResultadoEntrada.SinDatos(
                    $"no tiene un calendario con el grupo '{c.textotarifagrupoViejo}'"))
            End If

            ' --- La regla que evita dejar un intervalo inválido ---
            If Not CambioValido(actual.FechaDesde, c.FechaDesde) Then
                Return Task.FromResult(ResultadoEntrada.Fallo(
                    $"la fecha nueva ({c.FechaDesde:dd/MM/yyyy}) es anterior a la del calendario " &
                    $"actual ({actual.FechaDesde:dd/MM/yyyy})"))
            End If

            ctx.AbortarSiCancelado()

            ' --- El calendario nuevo ---
            Dim nuevo = funciones.GetCalendarioNuevoTarifa(
                actual.IdContratoTarifa, c.textotarifagrupoViejo, c.textotarifagrupoNuevo, c.FechaHasta)

            Dim codigo = funciones.GetOnlyCodigoContratobyIdContratoTarifa(actual.IdContratoTarifa)

            If codigo <= 0 OrElse nuevo Is Nothing OrElse nuevo.IdTarifaGrupo = 0 Then
                Return Task.FromResult(ResultadoEntrada.SinDatos(
                    $"no se ha podido resolver el grupo de tarifa nuevo '{c.textotarifagrupoNuevo}'"))
            End If

            ' --- Qué perfil de facturación se usa ---
            '
            ' Regla del original: el nuevo si es «Q» o si NO hay que mantener el anterior. Las dos
            ' columnas del Excel son opcionales y, vacías, valen False; así que sin rellenarlas se
            ' usa el perfil NUEVO, que es lo que hacía el original.
            Dim esQ = (c.IsQ = True)
            Dim mantener = (c.MantenerPerfil = True)
            Dim idPerfil As Integer = If(esQ OrElse Not mantener,
                                         nuevo.idperfilfacturacionoNuevo,
                                         nuevo.IdPerfilFacturacion)

            ctx.AbortarSiCancelado()

            funciones.InsertTarifaGrupoCalendario(
                nuevo.Entorno, codigo, nuevo.IdTarifaGrupo, nuevo.IdTarifa, idPerfil, c.FechaDesde)

            ' --- Y los precios ---
            '
            ' Con «Q» los precios se aplican desde la fecha del calendario ACTUAL y no desde la
            ' nueva. Es del original y se mantiene.
            Dim fechaAplicacion = If(Not esQ, c.FechaDesde, actual.FechaDesde)
            funciones.AplicarPreciosV2(codigo, fechaAplicacion, esQ, c.FechaAplicar)

            Return Task.FromResult(ResultadoEntrada.ConDatos(
                1,
                Redaccion.Unir($"{c.textotarifagrupoViejo} → {c.textotarifagrupoNuevo}",
                               $"desde {c.FechaDesde:dd/MM/yyyy}",
                               If(esQ, "perfil nuevo (Q)",
                                  If(mantener, "perfil conservado", "perfil nuevo")))))

        End Function

        ''' <summary>
        ''' No se puede crear un calendario con fecha anterior a la del actual: al cerrar el
        ''' actual quedaría un intervalo inválido. Es CambioCalendarioValido del original, con
        ''' su mismo comentario.
        ''' </summary>
        Friend Shared Function CambioValido(desdeActual As Date, nuevaDesde As Date) As Boolean
            Return nuevaDesde >= desdeActual
        End Function

    End Class

End Namespace
