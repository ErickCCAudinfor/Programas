Option Strict Off   ' Usa los DTO portados.

Imports System.Data
Imports System.IO
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Consultas
Imports SigeGestor.Core.Contratos

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Modificación masiva de contratos: se eligen los campos que se quieren cambiar y se
    ''' aplican a toda la lista. Es el ContratoForm de ActualizaPrecios.
    '''
    ''' El UPDATE se compone igual que en el original —un «campo=valor» por cada cosa marcada,
    ''' unidos por comas, y un UPDATE por contrato con el código como parámetro— y se sigue
    ''' generando el Excel de antes y el de después para poder comparar, con la misma consulta
    ''' ConsultaContrato.sql.
    '''
    ''' TRES DIFERENCIAS CON EL ORIGINAL, las tres por corregir algo:
    '''
    ''' 1) Allí cada campo llevaba DOS casillas: una para «tocar esto» y otra para el sí o el no.
    '''    Marcar la segunda sin la primera no hacía nada, en silencio. Aquí los booleanos son un
    '''    desplegable de tres opciones —no tocar / Sí / No— y eso no puede pasar.
    '''
    ''' 2) El Excel de DESPUÉS se generaba con la consulta de ANTES: «ExportarConsultaAExcel(...,
    '''    consultaAntes, rutaArchivoDespuesModificacion, ...)». Los dos ficheros salían con los
    '''    mismos datos, así que la comparación que promete la pantalla no servía de nada. Aquí
    '''    el de después se consulta después.
    '''
    ''' 3) UpdateContratosMasivo se come las excepciones con Console.WriteLine y devuelve lo que
    '''    llevara acumulado, así que un fallo a mitad de un masivo sobre Producción salía como
    '''    «se han actualizado N». Aquí cada contrato es una entrada con su propio resultado.
    ''' </summary>
    Public Class MasivoContrato
        Inherits OperacionPorEntrada

        ' Claves de los campos. Se agrupan por bloque igual que en la pantalla original.
        Public Const ClaveSituacion As String = "situacion"
        Public Const ClaveFechaAlta As String = "fechaAlta"
        Public Const ClaveFechaVto As String = "fechaVto"
        Public Const ClaveSumarAnoVto As String = "sumarAnoVto"
        Public Const ClaveObservaciones As String = "observaciones"
        Public Const ClaveModoObservaciones As String = "modoObservaciones"
        Public Const ClaveTipoImpuesto As String = "tipoImpuesto"
        Public Const ClaveAgruparFacturas As String = "agruparFacturas"
        Public Const ClaveRevisionFra As String = "revisionFra"
        Public Const ClaveTextoRevision As String = "textoRevision"
        Public Const ClaveModoTextoRevision As String = "modoTextoRevision"
        Public Const ClaveModeloFactura As String = "modeloFactura"
        Public Const ClaveModeloFacturaVarios As String = "modeloFacturaVarios"
        Public Const ClaveModeloContrato As String = "modeloContrato"
        Public Const ClaveCnae As String = "cnae"
        Public Const ClaveTipoImprimir As String = "tipoImprimir"
        Public Const ClaveRenovacionProcesada As String = "renovacionProcesada"
        Public Const ClaveNoRenovar As String = "noRenovar"
        Public Const ClaveRepresentante As String = "representante"
        Public Const ClaveColectivoRep As String = "colectivoRep"
        Public Const ClaveIdentificadorRep As String = "identificadorRep"
        Public Const ClaveEmailRep As String = "emailRep"
        Public Const ClaveMovilRep As String = "movilRep"
        Public Const ClaveScoring As String = "scoring"
        Public Const ClaveCifClientePago As String = "cifClientePago"
        Public Const ClaveClientePago As String = "clientePago"
        Public Const ClaveAutoconsumo As String = "autoconsumo"
        Public Const ClaveAutoconsumoNoCompensable As String = "autoconsumoNoCompensable"
        Public Const ClaveTipoAutoconsumo As String = "tipoAutoconsumo"
        Public Const ClaveLicitacion As String = "licitacion"
        Public Const ClaveExencionIe As String = "exencionIe"

        ''' <summary>Modos de escritura de los campos de texto acumulativos.</summary>
        Public Const ModoAnadir As String = "anadir"
        Public Const ModoBorrar As String = "borrar"

        Public Shared Function OpcionesModoTexto() As IReadOnlyList(Of OpcionFija)
            Return {
                New OpcionFija(ModoAnadir, "Añadir al final, tras « | »"),
                New OpcionFija(ModoBorrar, "Dejarlo en blanco")
            }
        End Function

        ''' <summary>Los mismos seis de TipoImpresion() del original, con sus claves.</summary>
        Public Shared Function OpcionesTipoImprimir() As IReadOnlyList(Of OpcionFija)
            Return {
                New OpcionFija("", "— no tocar —"),
                New OpcionFija("P", "Papel y Email"),
                New OpcionFija("E", "Email"),
                New OpcionFija("W", "Web"),
                New OpcionFija("Q", "Papel"),
                New OpcionFija("R", "Recibo Bancario"),
                New OpcionFija("F", "FACE")
            }
        End Function

        Private ReadOnly _contratos As New RepositorioContratos()
        Private ReadOnly _sql As New RepositorioSql()

        Private _asignaciones As String = ""
        Private _todos As New List(Of Long)
        Private _rutaAntes As String = ""
        Private _rutaDespues As String = ""
        Private _carpeta As String = ""

        ' ==================================================================
        ' Composición del SET
        ' ==================================================================

        ''' <summary>
        ''' Los «campo=valor» de todo lo que el usuario haya rellenado. Vacío si no ha marcado
        ''' nada, y entonces la operación no se lanza.
        '''
        ''' Es la traducción de GetParameters del original, campo por campo y en el mismo orden.
        ''' </summary>
        Public Shared Function Asignaciones(ctx As ContextoEjecucion) As String

            Dim campos As New List(Of String)

            ' --- Situación ---
            Numerico(campos, ctx, ClaveSituacion, "IdContratoSituacion")

            ' --- Fechas ---
            Dim alta = Fecha(ctx.Campo(ClaveFechaAlta))
            If alta.HasValue Then campos.Add($"FechaAlta='{alta.Value:dd/MM/yyyy}'")

            Dim sumarAno = ctx.Campo(ClaveSumarAnoVto) = "1"
            Dim vto = Fecha(ctx.Campo(ClaveFechaVto))

            ' En el original, marcar «sumar un año» deshabilitaba el selector de vencimiento.
            ' Aquí se respeta la misma exclusión: si se suma un año, la fecha escrita se ignora.
            If sumarAno Then
                campos.Add("FechaVto = DateAdd(Year, 1, FechaVto)")
            ElseIf vto.HasValue Then
                campos.Add($"FechaVto='{vto.Value:dd/MM/yyyy}'")
            End If

            ' --- Textos acumulativos ---
            Acumulativo(campos, ctx, ClaveObservaciones, ClaveModoObservaciones, "Observaciones")
            Acumulativo(campos, ctx, ClaveTextoRevision, ClaveModoTextoRevision, "textorevision")

            ' --- Listas ---
            Numerico(campos, ctx, ClaveTipoImpuesto, "IdTipoImpuesto")
            Numerico(campos, ctx, ClaveModeloFactura, "IdModeloFactura")
            Numerico(campos, ctx, ClaveModeloFacturaVarios, "IdModeloFacturaVarios")
            Numerico(campos, ctx, ClaveModeloContrato, "IdModeloContrato")
            Numerico(campos, ctx, ClaveCnae, "idCNAE")
            Numerico(campos, ctx, ClaveColectivoRep, "IdColectivoRep")
            Numerico(campos, ctx, ClaveClientePago, "idclientepago")
            Numerico(campos, ctx, ClaveTipoAutoconsumo, "idtipoautoconsumo")

            ' --- Booleanos de tres estados ---
            Booleano(campos, ctx, ClaveAgruparFacturas, "IsAgruparFacturas")
            Booleano(campos, ctx, ClaveRevisionFra, "RevisionFra")
            Booleano(campos, ctx, ClaveRenovacionProcesada, "IsRenovacionProcesada")
            Booleano(campos, ctx, ClaveNoRenovar, "NoRenovar")
            Booleano(campos, ctx, ClaveAutoconsumo, "autoconsumo")
            Booleano(campos, ctx, ClaveAutoconsumoNoCompensable, "IsAutoconsumoNoCompensable")
            Booleano(campos, ctx, ClaveLicitacion, "islicitacion")
            Booleano(campos, ctx, ClaveExencionIe, "ExencionIE")

            ' --- Textos simples ---
            Texto(campos, ctx, ClaveRepresentante, "Representante")
            Texto(campos, ctx, ClaveIdentificadorRep, "IdentificadorRep")
            Texto(campos, ctx, ClaveEmailRep, "EmailRep")
            Texto(campos, ctx, ClaveMovilRep, "SMSRep")
            Texto(campos, ctx, ClaveScoring, "SituacionScoring")

            ' --- Tipo de impresión: clave de una letra ---
            Texto(campos, ctx, ClaveTipoImprimir, "TipoImprimir")

            ' --- Días de vencimiento, derivados de qué fechas se hayan cambiado ---
            Dim dias = DiasVencimiento(alta, vto, sumarAno)
            If dias.Length > 0 Then campos.Add($"diasvencimiento={dias}")

            Return String.Join(", ", campos)

        End Function

        ''' <summary>
        ''' Misma regla que getDiasVencimiento del original: se recalculan los días solo si se
        ''' toca alguna de las dos fechas, y con la que no se toca leída de la propia fila.
        '''
        ''' Con «sumar un año» no se recalcula: el original tampoco lo hacía, porque su cálculo
        ''' miraba el DateTimePicker y ese caso lo dejaba deshabilitado.
        ''' </summary>
        Private Shared Function DiasVencimiento(alta As Date?, vto As Date?, sumarAno As Boolean) As String

            If sumarAno Then Return ""

            If alta.HasValue AndAlso vto.HasValue Then
                Return CInt((vto.Value - alta.Value).TotalDays).ToString()
            End If

            If alta.HasValue Then Return $"datediff(d,'{alta.Value:dd/MM/yyyy}',FechaVto)"
            If vto.HasValue Then Return $"datediff(d,FechaAlta,'{vto.Value:dd/MM/yyyy}')"

            Return ""

        End Function

        Private Shared Sub Numerico(campos As List(Of String),
                                    ctx As ContextoEjecucion,
                                    clave As String,
                                    columna As String)

            Dim n As Long
            Dim v = ctx.Campo(clave)
            If v.Length = 0 Then Exit Sub
            If Not Long.TryParse(v, n) OrElse n <= 0 Then Exit Sub

            campos.Add($"{columna}={n}")

        End Sub

        Private Shared Sub Booleano(campos As List(Of String),
                                    ctx As ContextoEjecucion,
                                    clave As String,
                                    columna As String)

            Dim v = ctx.Campo(clave)
            If v <> SiNoSinTocar.Si AndAlso v <> SiNoSinTocar.No Then Exit Sub

            campos.Add($"{columna}={v}")

        End Sub

        Private Shared Sub Texto(campos As List(Of String),
                                 ctx As ContextoEjecucion,
                                 clave As String,
                                 columna As String)

            Dim v = ctx.Campo(clave)
            If v.Length = 0 Then Exit Sub

            campos.Add($"{columna}='{Escapar(v)}'")

        End Sub

        ''' <summary>
        ''' Observaciones y texto de revisión: o se añade al final tras « | », o se deja en
        ''' blanco. Es lo que hacían las parejas CheckBox5/ChckBlancoObservacion y
        ''' CheckBox9/CheckBlancoRevision.
        ''' </summary>
        Private Shared Sub Acumulativo(campos As List(Of String),
                                       ctx As ContextoEjecucion,
                                       claveTexto As String,
                                       claveModo As String,
                                       columna As String)

            Dim modo = ctx.Campo(claveModo)
            Dim texto = ctx.Campo(claveTexto)

            If modo = ModoBorrar Then
                campos.Add($"{columna}=''")
                Exit Sub
            End If

            If modo <> ModoAnadir OrElse texto.Length = 0 Then Exit Sub

            ' El original hace «Observaciones=Observaciones+' | texto'». Si el campo está a NULL
            ' eso da NULL y se pierde el texto, así que se protege con ISNULL. Es el mismo
            ' resultado cuando hay contenido y deja de perderlo cuando no.
            campos.Add($"{columna}=ISNULL({columna},'')+' | {Escapar(texto)}'")

        End Sub

        ''' <summary>
        ''' Comilla simple duplicada. El original interpolaba el texto del cuadro tal cual, así
        ''' que una observación con un apóstrofo —«C/ O'Donnell»— rompía el UPDATE de todo el
        ''' lote. Y con mala intención se podía escribir cualquier cosa.
        ''' </summary>
        Private Shared Function Escapar(valor As String) As String
            Return valor.Replace("'", "''")
        End Function

        Private Shared Function Fecha(texto As String) As Date?
            Dim f As Date
            If Date.TryParse(texto, Globalization.CultureInfo.InvariantCulture,
                             Globalization.DateTimeStyles.None, f) Then Return f
            Return Nothing
        End Function

        ' ==================================================================
        ' Ejecución
        ' ==================================================================

        Protected Overrides Async Function PrepararAsync(ctx As ContextoEjecucion) As Task

            _asignaciones = Asignaciones(ctx)
            If _asignaciones.Length = 0 Then
                Throw New InvalidOperationException(
                    "No has indicado ningún cambio. Rellena al menos un campo de los de abajo.")
            End If

            ' Se resuelve la lista entera antes de tocar nada, para poder sacar el Excel de antes
            ' con los mismos contratos que se van a modificar.
            _todos = New List(Of Long)
            For Each entrada In ctx.Entradas
                Dim encontrados = Await _contratos.ResolverAsync(
                    ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

                _todos.AddRange(encontrados.Where(Function(c) c.CodigoContrato > 0) _
                                           .Select(Function(c) c.CodigoContrato))
            Next
            _todos = _todos.Distinct().ToList()

            _carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                          RutasSalida.Asegurar("LogContratos"),
                          ctx.CarpetaDestino)
            Directory.CreateDirectory(_carpeta)

            _rutaAntes = ""
            _rutaDespues = ""

            If _todos.Count > 0 Then
                _rutaAntes = Await VolcarAsync(ctx, "ContratosAntesActualizacion").ConfigureAwait(False)
            End If

        End Function

        Protected Overrides Async Function ProcesarAsync(entrada As String,
                                                         ctx As ContextoEjecucion) As Task(Of ResultadoEntrada)

            Dim encontrados = Await _contratos.ResolverAsync(
                ctx.CadenaConexion, entrada, ctx.TipoLista, ctx.Cancelacion).ConfigureAwait(False)

            Dim codigos = encontrados.Where(Function(c) c.CodigoContrato > 0) _
                                     .Select(Function(c) c.CodigoContrato) _
                                     .Distinct().ToList()

            If codigos.Count = 0 Then Return ResultadoEntrada.SinDatos("no existe")

            ' Un UPDATE por contrato con el código como parámetro, igual que
            ' UpdateContratosMasivo. Se ejecuta aquí en vez de llamar a ese método porque ese se
            ' come las excepciones y devuelve el acumulado: en un masivo sobre Producción hay
            ' que poder decir qué contrato ha fallado y por qué.
            Dim afectadas As Long = 0

            Using conexion As New SqlConnection(ctx.CadenaConexion)
                Await conexion.OpenAsync(ctx.Cancelacion).ConfigureAwait(False)

                For Each codigo In codigos
                    ctx.AbortarSiCancelado()

                    Using comando As New SqlCommand(
                        $"UPDATE contrato SET {_asignaciones} WHERE codigocontrato = @cod", conexion)

                        comando.CommandTimeout = 300
                        comando.Parameters.Add("@cod", SqlDbType.BigInt).Value = codigo

                        afectadas += Await comando.ExecuteNonQueryAsync(ctx.Cancelacion).ConfigureAwait(False)

                    End Using
                Next
            End Using

            If afectadas = 0 Then
                Return ResultadoEntrada.SinDatos("el update no ha afectado a ninguna fila")
            End If

            Return ResultadoEntrada.ConDatos(
                afectadas, If(codigos.Count = 1, "actualizado", $"{afectadas} contratos actualizados"))

        End Function

        Protected Overrides Async Function CerrarAsync(ctx As ContextoEjecucion,
                                                      resultado As ResultadoOperacion) As Task

            Dim partes As New List(Of String)

            If _rutaAntes.Length > 0 Then partes.Add(Path.GetFileName(_rutaAntes))

            ' El de después se consulta AHORA, ya modificado. El original reutilizaba la consulta
            ' de antes y los dos ficheros salían iguales.
            If resultado.ConDatos > 0 Then
                Try
                    _rutaDespues = Await VolcarAsync(ctx, "ContratosDespuesActualizacion").ConfigureAwait(False)
                    partes.Add(Path.GetFileName(_rutaDespues))
                Catch ex As Exception
                    partes.Add($"no se ha podido sacar el Excel de después: {ex.Message}")
                End Try
            End If

            resultado.Mensaje = $"Cambios aplicados: {_asignaciones}"
            If partes.Count > 0 Then
                resultado.Mensaje &= $" · {String.Join(" y ", partes)} en {_carpeta}"
            End If

        End Function

        ''' <summary>
        ''' Vuelca el estado de los contratos con ConsultaContrato.sql, la misma plantilla que
        ''' usaba GetConsultaContrato.
        ''' </summary>
        Private Async Function VolcarAsync(ctx As ContextoEjecucion, nombre As String) As Task(Of String)

            Dim plantilla As New PlantillaSql(_sql.Obtener("ConsultaContrato"))
            plantilla.Poner("codCntratojoinReplace", String.Join(",", _todos))

            Dim tabla As New DataTable()

            Using conexion As New SqlConnection(ctx.CadenaConexion)
                Await conexion.OpenAsync(ctx.Cancelacion).ConfigureAwait(False)

                Using comando As New SqlCommand(plantilla.ToString(), conexion)
                    comando.CommandTimeout = 600
                    Using lector = Await comando.ExecuteReaderAsync(ctx.Cancelacion).ConfigureAwait(False)
                        tabla.Load(lector)
                    End Using
                End Using
            End Using

            Dim escrito = EscritorExcel.Escribir(tabla, _carpeta, nombre, "Contratos")

            If escrito.Ficheros.Count = 0 Then
                Throw New InvalidOperationException(
                    "la consulta de contratos no ha devuelto ninguna fila")
            End If

            ' Con más de un millón de filas sale partido; se nombra el primero, que es el que
            ' hay que abrir, y el resto está al lado.
            Return escrito.Ficheros(0)

        End Function

    End Class

End Namespace
