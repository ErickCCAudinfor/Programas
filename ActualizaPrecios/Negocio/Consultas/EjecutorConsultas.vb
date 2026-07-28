Imports System.IO

''' <summary>
''' Formas de ejecución que comparten las consultas del catálogo. Cada definición de
''' CatalogoConsultas se apoya en una de estas en vez de repetir el bucle y el volcado a Excel.
''' </summary>
Public Module EjecutorConsultas

    ''' <summary>
    ''' Segundos que se le dan a cada comando SQL. 0 = sin límite: hay consultas que tienen
    ''' que tardar lo que tengan que tardar y no deben abortarse solas.
    ''' Para cortar una ejecución está el botón Cancelar, que llama a SqlCommand.Cancel().
    ''' </summary>
    Public Const TimeoutConsultaSegundos As Integer = 0

    Private Function RutaFechada(ctx As ContextoConsulta, nombre As String) As String
        Return Path.Combine(ctx.CarpetaDestino, $"{nombre}_{Date.Today:ddMMyyyy}.xlsx")
    End Function

    Private Function Consultar(ctx As ContextoConsulta, sql As String) As DataTable
        Return FetchDataTableCancelable(ctx.Conexion, sql, ctx.Cancelacion, TimeoutConsultaSegundos)
    End Function

    ''' <summary>Una consulta, un fichero, una hoja.</summary>
    Public Function EjecutarSimple(ctx As ContextoConsulta, nombre As String, hoja As String, sql As Func(Of String)) As ResultadoConsulta

        Dim resultado As New ResultadoConsulta
        ctx.AbortarSiCancelado()
        ctx.Informar($"Consultando {nombre}...")

        Using dt = Consultar(ctx, sql())
            If dt.Rows.Count = 0 Then Return resultado

            Dim ruta = RutaFechada(ctx, nombre)
            ctx.Informar($"Escribiendo {dt.Rows.Count:N0} filas...")
            resultado.Anotar(EscribirDataTableAExcel(dt, ruta, hoja), ruta)
        End Using

        Return resultado

    End Function

    ''' <summary>Varias consultas dentro del mismo libro, cada una en su hoja.</summary>
    Public Function EjecutarHojas(ctx As ContextoConsulta, nombre As String, hojas As IEnumerable(Of HojaConsulta)) As ResultadoConsulta

        Dim resultado As New ResultadoConsulta
        Dim ruta = RutaFechada(ctx, nombre)

        For Each h In hojas
            ctx.AbortarSiCancelado()
            ctx.Informar($"Consultando {nombre} - {h.Nombre}...")

            Using dt = Consultar(ctx, h.Sql.Invoke())
                If dt.Rows.Count = 0 Then Continue For
                resultado.Anotar(EscribirDataTableAExcel(dt, ruta, h.Nombre), ruta)
            End Using
        Next

        Return resultado

    End Function

    ''' <summary>
    ''' Una consulta por cada entrada (CIF, normalmente) y un fichero independiente por cada una.
    ''' </summary>
    Public Function EjecutarPorEntrada(ctx As ContextoConsulta, prefijo As String, sql As Func(Of String, String)) As ResultadoConsulta

        Dim resultado As New ResultadoConsulta
        Dim procesados = 0

        For Each entrada In ctx.Entradas
            ctx.AbortarSiCancelado()
            procesados += 1
            ctx.Informar($"Consultando {entrada}", $"Procesados: {procesados} / {ctx.Entradas.Count}")

            Using dt = Consultar(ctx, sql(entrada))
                If dt.Rows.Count = 0 Then Continue For

                ' El mismo nombre para fichero y hoja, como venía haciéndose.
                Dim nombre = If(String.IsNullOrEmpty(prefijo), entrada, $"{prefijo}_{entrada}")
                Dim ruta = RutaFechada(ctx, nombre)
                resultado.Anotar(EscribirDataTableAExcel(dt, ruta, nombre), ruta)
            End Using
        Next

        Return resultado

    End Function

    ''' <summary>
    ''' Curvas: una consulta por CUPS. Con "Dividir Excel" genera un fichero por CUPS; sin
    ''' dividir acumula todo y lo vuelca de una vez en streaming, troceando por EXCEL_MAX_ROWS.
    ''' </summary>
    Public Function EjecutarCurva(ctx As ContextoConsulta, nombre As String, sql As Func(Of String, String)) As ResultadoConsulta

        If ctx.Dividir Then Return CurvaDividida(ctx, nombre, sql)
        Return CurvaAgrupada(ctx, nombre, sql)

    End Function

    Private Function CurvaDividida(ctx As ContextoConsulta, nombre As String, sql As Func(Of String, String)) As ResultadoConsulta

        Dim resultado As New ResultadoConsulta
        Dim procesados = 0

        For Each c In ctx.Entradas
            ctx.AbortarSiCancelado()
            procesados += 1
            ctx.Informar($"Consultando Cups: {c}", $"Procesados: {procesados} / {ctx.Entradas.Count}")

            Dim dt = Consultar(ctx, sql(c))
            If dt.Rows.Count = 0 Then
                dt.Dispose()
                Continue For
            End If

            Dim ruta = Path.Combine(ctx.CarpetaDestino, $"{nombre}_{c}.xlsx")

            If dt.Rows.Count <= EXCEL_MAX_ROWS Then
                resultado.Anotar(EscribirDataTableAExcel(dt, ruta, c), ruta)
            Else
                Dim sheetIdx = 1
                Dim offset = 0
                While offset < dt.Rows.Count
                    ctx.AbortarSiCancelado()
                    Dim batch = Math.Min(EXCEL_MAX_ROWS, dt.Rows.Count - offset)
                    Dim subDt = dt.Clone()
                    For i = offset To offset + batch - 1
                        subDt.ImportRow(dt.Rows(i))
                    Next
                    resultado.Anotar(EscribirDataTableAExcel(subDt, ruta, $"{c}_{sheetIdx}"), ruta)
                    subDt.Dispose()
                    offset += batch
                    sheetIdx += 1
                End While
            End If

            dt.Dispose()
        Next

        Return resultado

    End Function

    Private Function CurvaAgrupada(ctx As ContextoConsulta, nombre As String, sql As Func(Of String, String)) As ResultadoConsulta

        Dim resultado As New ResultadoConsulta

        ' Se acumula en un DataTable maestro (más ligero en RAM que ClosedXML) y se escribe
        ' el Excel una sola vez al final, para evitar OutOfMemoryException.
        Dim masterDt As DataTable = Nothing
        Dim procesados = 0

        For Each c In ctx.Entradas
            ctx.AbortarSiCancelado()
            procesados += 1
            ctx.Informar($"Consultando Cups: {c}", $"Procesados: {procesados} / {ctx.Entradas.Count}")

            Dim dt = Consultar(ctx, sql(c))
            If dt.Rows.Count = 0 Then
                dt.Dispose()
                Continue For
            End If

            If masterDt Is Nothing Then masterDt = dt.Clone()
            For Each row As DataRow In dt.Rows
                masterDt.ImportRow(row)
            Next

            dt.Dispose()
            GC.Collect()
        Next

        If masterDt Is Nothing OrElse masterDt.Rows.Count = 0 Then
            If masterDt IsNot Nothing Then masterDt.Dispose()
            Return resultado
        End If

        ctx.Informar($"Escribiendo Excel: {masterDt.Rows.Count:N0} filas...")

        Dim allRows = masterDt.Rows.Cast(Of DataRow)().ToList()
        Dim columnas = allRows(0).Table.Columns
        masterDt.Dispose()
        GC.Collect()

        Dim fileIdx = 1
        Dim offset = 0
        While offset < allRows.Count
            ctx.AbortarSiCancelado()
            Dim ruta = If(allRows.Count <= EXCEL_MAX_ROWS,
                          RutaFechada(ctx, nombre),
                          RutaFechada(ctx, $"{nombre}_{fileIdx:D2}"))
            Dim batch = Math.Min(EXCEL_MAX_ROWS, allRows.Count - offset)
            EscribirStreamingAExcel(allRows.Skip(offset).Take(batch), columnas, ruta, nombre)
            resultado.Anotar(batch, ruta)
            offset += batch
            fileIdx += 1
        End While

        Return resultado

    End Function

End Module
