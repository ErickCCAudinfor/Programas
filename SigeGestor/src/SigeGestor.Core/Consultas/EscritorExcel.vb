Imports System.Data
Imports System.IO
Imports ClosedXML.Excel

Namespace Consultas

    ''' <summary>
    ''' Volcado de resultados a Excel.
    '''
    ''' Solo ClosedXML. ActualizaPrecios arrastraba ClosedXML y EPPlus a la vez —incluso
    ''' mezcladas en el mismo fichero— y EPPlus además es de pago desde la versión 5.
    ''' </summary>
    Public Module EscritorExcel

        ''' <summary>
        ''' Tope de filas de una hoja de Excel menos la de cabecera. Pasarse de aquí no da un
        ''' error claro, da un fichero corrupto, así que se trocea antes.
        ''' </summary>
        Public Const MaxFilasHoja As Integer = 1_048_575

        Public Class ResultadoEscritura
            Public Property Ficheros As IReadOnlyList(Of String) = Array.Empty(Of String)()
            Public Property Filas As Long
        End Class

        ''' <summary>
        ''' Escribe una tabla. Si no cabe en una hoja, se parte en varios ficheros numerados.
        ''' Devuelve las rutas escritas.
        ''' </summary>
        Public Function Escribir(datos As DataTable,
                                 carpeta As String,
                                 nombreBase As String,
                                 nombreHoja As String) As ResultadoEscritura

            Directory.CreateDirectory(carpeta)

            If datos Is Nothing OrElse datos.Rows.Count = 0 Then
                Return New ResultadoEscritura
            End If

            Dim ficheros As New List(Of String)
            Dim escritas As Long = 0
            Dim trozos = CInt(Math.Ceiling(datos.Rows.Count / CDbl(MaxFilasHoja)))

            For trozo = 0 To trozos - 1
                Dim desde = trozo * MaxFilasHoja
                Dim cuantas = Math.Min(MaxFilasHoja, datos.Rows.Count - desde)

                Dim nombre = If(trozos = 1, nombreBase, $"{nombreBase}_{trozo + 1:D2}")
                Dim ruta = RutaLibre(carpeta, nombre)

                Using libro As New XLWorkbook()
                    Dim hoja = libro.Worksheets.Add(SanearHoja(nombreHoja))

                    ' Cabeceras
                    For c = 0 To datos.Columns.Count - 1
                        hoja.Cell(1, c + 1).Value = datos.Columns(c).ColumnName
                    Next
                    hoja.Row(1).Style.Font.Bold = True
                    hoja.SheetView.FreezeRows(1)

                    ' Datos
                    For f = 0 To cuantas - 1
                        Dim fila = datos.Rows(desde + f)
                        For c = 0 To datos.Columns.Count - 1
                            Dim v = fila(c)
                            If v Is Nothing OrElse Convert.IsDBNull(v) Then Continue For
                            hoja.Cell(f + 2, c + 1).Value = XLCellValue.FromObject(v)
                        Next
                    Next

                    ' Ajustar el ancho solo con unas cuantas filas: hacerlo con un millón
                    ' tarda más que la propia consulta.
                    hoja.Columns().AdjustToContents(1UI, 200UI)

                    libro.SaveAs(ruta)
                End Using

                ficheros.Add(ruta)
                escritas += cuantas
            Next

            Return New ResultadoEscritura With {.Ficheros = ficheros, .Filas = escritas}

        End Function

        ''' <summary>
        ''' Un libro con varias hojas. Devuelve la ruta escrita.
        '''
        ''' Las hojas vacías se crean igualmente, solo con las cabeceras: si Gas no ha devuelto
        ''' nada, quien recibe el Excel tiene que poder ver que la hoja existe y está vacía, no
        ''' preguntarse si se ha perdido.
        ''' </summary>
        Public Function EscribirVariasHojas(hojas As IReadOnlyList(Of KeyValuePair(Of String, DataTable)),
                                            carpeta As String,
                                            nombreBase As String) As String

            Directory.CreateDirectory(carpeta)
            Dim ruta = RutaLibre(carpeta, nombreBase)

            Using libro As New XLWorkbook()
                For Each par In hojas
                    Dim datos = par.Value
                    Dim hoja = libro.Worksheets.Add(SanearHoja(par.Key))

                    If datos Is Nothing OrElse datos.Columns.Count = 0 Then Continue For

                    For c = 0 To datos.Columns.Count - 1
                        hoja.Cell(1, c + 1).Value = datos.Columns(c).ColumnName
                    Next
                    hoja.Row(1).Style.Font.Bold = True
                    hoja.SheetView.FreezeRows(1)

                    Dim tope = Math.Min(datos.Rows.Count, MaxFilasHoja)
                    For f = 0 To tope - 1
                        Dim fila = datos.Rows(f)
                        For c = 0 To datos.Columns.Count - 1
                            Dim v = fila(c)
                            If v Is Nothing OrElse Convert.IsDBNull(v) Then Continue For
                            hoja.Cell(f + 2, c + 1).Value = XLCellValue.FromObject(v)
                        Next
                    Next

                    hoja.Columns().AdjustToContents(1UI, 200UI)
                Next

                libro.SaveAs(ruta)
            End Using

            Return ruta

        End Function

        ''' <summary>
        ''' Añade la fecha al nombre y, si ya existe, un sufijo. Nunca se sobrescribe un Excel
        ''' anterior: en ActualizaPrecios el nombre llevaba solo la fecha, así que lanzar dos
        ''' veces la misma consulta el mismo día pisaba el primero sin avisar.
        ''' </summary>
        Private Function RutaLibre(carpeta As String, nombreBase As String) As String

            Dim conFecha = $"{nombreBase}_{Date.Today:ddMMyyyy}"
            Dim ruta = Path.Combine(carpeta, $"{conFecha}.xlsx")

            Dim intento = 2
            While File.Exists(ruta)
                ruta = Path.Combine(carpeta, $"{conFecha}_{intento}.xlsx")
                intento += 1
            End While

            Return ruta

        End Function

        ''' <summary>
        ''' Excel no admite más de 31 caracteres en el nombre de hoja ni los caracteres
        ''' : \ / ? * [ ]. Un CUPS o un CIF caben, pero un nombre de consulta largo no.
        ''' </summary>
        Private Function SanearHoja(nombre As String) As String

            If String.IsNullOrWhiteSpace(nombre) Then Return "Datos"

            Dim limpio = nombre
            For Each c In ":\/?*[]"
                limpio = limpio.Replace(c, "_"c)
            Next

            limpio = limpio.Trim()
            If limpio.Length > 31 Then limpio = limpio.Substring(0, 31)
            If limpio.Length = 0 Then Return "Datos"

            Return limpio

        End Function

    End Module

End Namespace
