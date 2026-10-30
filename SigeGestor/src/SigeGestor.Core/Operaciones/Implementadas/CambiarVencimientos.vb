Imports System.Globalization
Imports System.IO
Imports System.Text
Imports ClosedXML.Excel
Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Genera el script de cambio de vencimientos de un Jira tipo SOPTOT-13245.
    '''
    ''' NO EJECUTA NADA. Deja un .sql para lanzar a mano en SSMS, con sus pasos VER / CAMBIAR /
    ''' DESHACER. Es a propósito: son cambios en Producción sobre cientos de contratos, se
    ''' revisan con el paso VER antes de aplicar y se quiere poder deshacerlos. La aplicación
    ''' prepara el trabajo; quién y cuándo lo aplica es decisión de quien lo lanza.
    '''
    ''' Lo único que cambia de un Jira al siguiente son el número del Jira, la fecha del día y
    ''' las filas del Excel, así que el cuerpo del script vive en Consultas\CambiarVencimientos.sql
    ''' —el mismo que ya se usó en PROD— y aquí solo se rellenan sus marcadores.
    ''' </summary>
    Public Class CambiarVencimientos
        Inherits OperacionUnica
        Implements IEntradasDesdeExcel

        Public Const NombrePlantilla As String = "CambiarVencimientos"

        ''' <summary>Clave de la casilla «Fijar la fecha de vencimiento».</summary>
        Public Const ClaveFijar As String = "fijarFechaVto"

        ''' <summary>
        ''' Las tres columnas del Excel. Se buscan POR NOMBRE en la fila de cabecera, no por
        ''' posición: estos ficheros los monta cada uno a su manera y el orden cambia.
        ''' </summary>
        Public Shared ReadOnly ColumnaContrato As String = "Contrato"
        Public Shared ReadOnly ColumnaCups As String = "CUPS"
        Public Shared ReadOnly ColumnaVencimiento As String = "VENCIMIENTO CORRECTO"

        Private ReadOnly _sql As RepositorioSql

        Public Sub New(sql As RepositorioSql)
            _sql = sql
        End Sub

        ''' <summary>Una fila del Excel, ya validada.</summary>
        Private Structure Fila
            Public Contrato As Long
            Public Cups As String
            Public Vencimiento As Date
        End Structure

        ' ============================================================
        ' LECTURA DEL EXCEL
        ' ============================================================

        ''' <summary>
        ''' Para que el formulario pueda decir cuántos contratos hay ANTES de generar nada. Si
        ''' el Excel no vale, lanza: eso es lo que impide ejecutar y lo que se ve en pantalla.
        ''' </summary>
        Public Function LeerEntradas(rutaExcel As String) As IReadOnlyList(Of String) _
            Implements IEntradasDesdeExcel.LeerEntradas

            Return Leer(rutaExcel).Select(Function(f) f.Contrato.ToString()).ToList()

        End Function

        ''' <summary>
        ''' Lee y valida el Excel entero. Lanza con el motivo al primer problema que haga
        ''' inservible el fichero: es preferible a generar un script a medias que alguien
        ''' lanzaría contra Producción.
        ''' </summary>
        Private Shared Function Leer(rutaExcel As String) As List(Of Fila)

            If String.IsNullOrWhiteSpace(rutaExcel) OrElse Not File.Exists(rutaExcel) Then
                Throw New InvalidOperationException("No se encuentra el Excel.")
            End If

            Dim filas As New List(Of Fila)()
            Dim vistos As New HashSet(Of Long)()
            Dim repetidos As New List(Of Long)()
            Dim malas As New List(Of String)()

            Using libro As New XLWorkbook(rutaExcel)

                Dim hoja = libro.Worksheets.First()

                ' En ClosedXML no hay Dimension —eso es de EPPlus—: la hoja vacía se reconoce
                ' porque LastRowUsed devuelve Nothing.
                Dim filaFinal = hoja.LastRowUsed()
                Dim columnaFinal = hoja.LastColumnUsed()
                If filaFinal Is Nothing OrElse columnaFinal Is Nothing Then
                    Throw New InvalidOperationException("El Excel está vacío.")
                End If

                Dim ultimaFila = filaFinal.RowNumber()
                Dim ultimaCol = columnaFinal.ColumnNumber()

                ' --- cabecera: se localizan las tres columnas por su nombre ---
                Dim donde As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                For c = 1 To ultimaCol
                    Dim titulo = Normalizar(hoja.Cell(1, c).GetString())
                    If titulo.Length > 0 AndAlso Not donde.ContainsKey(titulo) Then donde(titulo) = c
                Next

                Dim faltan = New String() {ColumnaContrato, ColumnaCups, ColumnaVencimiento}.
                    Where(Function(n) Not donde.ContainsKey(Normalizar(n))).ToList()

                If faltan.Count > 0 Then
                    Throw New InvalidOperationException(
                        "Al Excel le faltan columnas: " & String.Join(", ", faltan) &
                        ". La primera fila tiene que ser la cabecera, con Contrato, CUPS y VENCIMIENTO CORRECTO.")
                End If

                Dim colContrato = donde(Normalizar(ColumnaContrato))
                Dim colCups = donde(Normalizar(ColumnaCups))
                Dim colVto = donde(Normalizar(ColumnaVencimiento))

                ' --- filas ---
                For n = 2 To ultimaFila

                    Dim textoContrato = hoja.Cell(n, colContrato).GetString().Trim()
                    Dim textoCups = hoja.Cell(n, colCups).GetString().Trim()
                    Dim celdaVto = hoja.Cell(n, colVto)

                    ' Fila entera en blanco: se salta sin decir nada. Es lo que deja Excel
                    ' debajo de los datos al borrar contenido.
                    If textoContrato.Length = 0 AndAlso textoCups.Length = 0 AndAlso celdaVto.IsEmpty() Then
                        Continue For
                    End If

                    Dim contrato As Long
                    If Not Long.TryParse(textoContrato, contrato) OrElse contrato <= 0 Then
                        malas.Add($"fila {n}: el contrato «{textoContrato}» no es un número")
                        Continue For
                    End If

                    Dim vencimiento As Date
                    If Not LeerFecha(celdaVto, vencimiento) Then
                        malas.Add($"fila {n} (contrato {contrato}): el vencimiento «{celdaVto.GetString().Trim()}» no es una fecha")
                        Continue For
                    End If

                    If Not vistos.Add(contrato) Then
                        repetidos.Add(contrato)
                        Continue For
                    End If

                    filas.Add(New Fila With {
                        .Contrato = contrato,
                        .Cups = textoCups.ToUpperInvariant(),
                        .Vencimiento = vencimiento})
                Next
            End Using

            ' Se juntan TODOS los problemas en un mensaje. Decirlos de uno en uno obliga a
            ' corregir, volver a elegir el fichero y descubrir el siguiente.
            Dim problemas As New List(Of String)()
            If repetidos.Count > 0 Then
                problemas.Add($"contratos repetidos: {String.Join(", ", repetidos.Distinct().Take(10))}" &
                              If(repetidos.Distinct().Count() > 10, " y más", ""))
            End If
            If malas.Count > 0 Then
                problemas.AddRange(malas.Take(10))
                If malas.Count > 10 Then problemas.Add($"y {malas.Count - 10} filas más con problemas")
            End If

            If problemas.Count > 0 Then
                Throw New InvalidOperationException(
                    "El Excel no se puede usar:" & Environment.NewLine & " · " &
                    String.Join(Environment.NewLine & " · ", problemas))
            End If

            If filas.Count = 0 Then
                Throw New InvalidOperationException("El Excel no tiene ninguna fila con datos.")
            End If

            Return filas

        End Function

        ''' <summary>
        ''' La fecha puede venir como fecha de verdad o como texto. En texto se lee en español
        ''' —dd/MM/yyyy— y también en AAAAMMDD, que es como la escriben algunos.
        ''' </summary>
        Private Shared Function LeerFecha(celda As IXLCell, ByRef valor As Date) As Boolean

            If celda.DataType = XLDataType.DateTime Then
                valor = celda.GetDateTime()
                Return True
            End If

            Dim texto = celda.GetString().Trim()
            If texto.Length = 0 Then Return False

            Dim formatos = {"dd/MM/yyyy", "d/M/yyyy", "yyyyMMdd", "yyyy-MM-dd", "dd-MM-yyyy"}
            If Date.TryParseExact(texto, formatos, CultureInfo.InvariantCulture,
                                  DateTimeStyles.None, valor) Then Return True

            Return Date.TryParse(texto, New CultureInfo("es-ES"), DateTimeStyles.None, valor)

        End Function

        ''' <summary>Compara cabeceras sin que estorben los espacios de más ni las mayúsculas.</summary>
        Private Shared Function Normalizar(texto As String) As String
            Return String.Join(" ", (If(texto, "")).Split(" "c, StringSplitOptions.RemoveEmptyEntries)).Trim()
        End Function

        ' ============================================================
        ' GENERACIÓN DEL SCRIPT
        ' ============================================================

        Protected Overrides Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                      avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim jira = ctx.Texto.Trim()
            If jira.Length = 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo("Falta el número del Jira."))
            End If

            ' El Jira acaba en nombres de tabla, así que no puede traer nada raro.
            If jira.Any(Function(c) Not (Char.IsLetterOrDigit(c) OrElse c = "-"c OrElse c = "_"c)) Then
                Return Task.FromResult(ResultadoEntrada.Fallo(
                    $"El número del Jira «{jira}» tiene caracteres que no valen para un nombre de tabla. " &
                    "Solo letras, números, guion y guion bajo."))
            End If

            Dim filas As List(Of Fila)
            Try
                avisar("Leyendo el Excel…")
                filas = Leer(ctx.RutaExcel)
            Catch ex As Exception
                Return Task.FromResult(ResultadoEntrada.Fallo(ex.Message))
            End Try

            avisar($"Componiendo el script de {filas.Count:N0} contratos…")

            ' Sin guion para los nombres de tabla: SOPTOT-13245 no vale como identificador.
            Dim jiraSinGuion = jira.Replace("-", "").Replace("_", "")
            Dim hoy = Date.Now
            Dim fijar = If(EsVerdadero(ctx.Campo(ClaveFijar)), "1", "0")

            Dim plantilla As New PlantillaSql(_sql.Obtener(NombrePlantilla))
            plantilla.Poner("jiraSinGuionReplace", jiraSinGuion)
            plantilla.Poner("jiraReplace", jira)
            plantilla.Poner("fechaTablaReplace", hoy.ToString("ddMMyyyy"))
            plantilla.Poner("fechaCabeceraReplace", hoy.ToString("dd-MM-yyyy"))
            plantilla.Poner("contratosReplace", filas.Count.ToString("N0", CultureInfo.InvariantCulture))
            plantilla.Poner("ficheroExcelReplace", """" & Path.GetFileName(ctx.RutaExcel) & """")
            plantilla.Poner("fijarFechaVtoReplace", fijar)
            plantilla.Poner("filasExcelReplace", ComponerFilas(filas))

            Dim pendientes = plantilla.MarcadoresPendientes()
            If pendientes.Count > 0 Then
                Return Task.FromResult(ResultadoEntrada.Fallo(
                    $"La plantilla {NombrePlantilla}.sql tiene marcadores sin resolver: {String.Join(", ", pendientes)}"))
            End If

            Dim carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                             RutasSalida.Asegurar("Vencimientos"),
                             ctx.CarpetaDestino)
            Directory.CreateDirectory(carpeta)

            Dim ruta = RutaLibre(carpeta, $"{jira}_PROD_Cambiar_Vencimientos")

            ' UTF-8 CON BOM: SSMS abre sin BOM como ANSI y las tildes de los comentarios salen
            ' rotas. Mismo motivo por el que los .sql del proyecto llevan BOM.
            File.WriteAllText(ruta, plantilla.ToString(), New UTF8Encoding(True))

            Return Task.FromResult(
                ResultadoEntrada.ConDatos(filas.Count, Resumen(filas, jira, fijar = "1")).Genera(ruta))

        End Function

        ''' <summary>
        ''' Las filas del INSERT. Las fechas van en 'AAAAMMDD' SIEMPRE: el servidor está en
        ''' español y '2026-10-15' da el error 242 o se lee como el 10 de marzo.
        ''' </summary>
        Private Shared Function ComponerFilas(filas As List(Of Fila)) As String

            Dim sb As New StringBuilder()

            For i = 0 To filas.Count - 1
                Dim f = filas(i)
                sb.Append("    (").
                   Append(f.Contrato).
                   Append(", '").Append(f.Cups.Replace("'", "''")).
                   Append("', '").Append(f.Vencimiento.ToString("yyyyMMdd")).
                   Append("')").
                   Append(If(i = filas.Count - 1, ";", ","c)).
                   Append(Environment.NewLine)
            Next

            Return sb.ToString().TrimEnd()

        End Function

        ''' <summary>Lo que se lee al terminar, sin tener que abrir el fichero.</summary>
        Private Shared Function Resumen(filas As List(Of Fila), jira As String, fijar As Boolean) As String

            Dim menor = filas.Min(Function(f) f.Vencimiento)
            Dim mayor = filas.Max(Function(f) f.Vencimiento)

            Return $"{jira} · {filas.Count:N0} contratos · vencimientos del {menor:dd/MM/yyyy} al {mayor:dd/MM/yyyy}" &
                   If(fijar, " · fija IsFijarFechaVto", " · NO toca IsFijarFechaVto") &
                   " · el script NO se ha ejecutado, hay que lanzarlo en SSMS empezando por VER"

        End Function

        ''' <summary>
        ''' Un nombre que no exista ya. La de EscritorExcel no vale: es privada y le pone
        ''' extensión .xlsx. Nunca se sobrescribe un fichero anterior, que es la regla de toda
        ''' la aplicación, y aquí además el de ayer puede estar a medio aplicar.
        ''' </summary>
        Private Shared Function RutaLibre(carpeta As String, nombreBase As String) As String

            Dim ruta = Path.Combine(carpeta, nombreBase & ".sql")

            Dim intento = 2
            While File.Exists(ruta)
                ruta = Path.Combine(carpeta, $"{nombreBase}_{intento}.sql")
                intento += 1
            End While

            Return ruta

        End Function

        Private Shared Function EsVerdadero(valor As String) As Boolean
            Dim v = (If(valor, "")).Trim()
            Return v.Equals("True", StringComparison.OrdinalIgnoreCase) OrElse
                   v = "1" OrElse v.Equals("si", StringComparison.OrdinalIgnoreCase) OrElse
                   v.Equals("sí", StringComparison.OrdinalIgnoreCase)
        End Function

    End Class

End Namespace
