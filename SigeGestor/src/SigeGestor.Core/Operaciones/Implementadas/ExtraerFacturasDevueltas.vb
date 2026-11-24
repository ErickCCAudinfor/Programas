Imports System.Data
Imports System.IO
Imports System.Xml.Linq
Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Consultas

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Saca los números de factura de los ficheros DEVOL_*.xml de las carpetas de importación.
    '''
    ''' Es el «Extraer Nomb Fac FichDev» de ActualizaPrecios (Button30 de su Form1). Las
    ''' carpetas de \\172.31.100.13\Total\FicherosExport\Import se llaman por el día —20261123—
    ''' y dentro están los DEVOL_20261120_170000.XML con las facturas devueltas.
    '''
    ''' QUE CAMBIA RESPECTO AL ORIGINAL
    '''
    ''' 1. Allí la fecha se escribía a mano en una caja como «20261123» y si te equivocabas de
    '''    formato te lo decía después de haber elegido la carpeta. Aquí van los dos selectores
    '''    de fecha del formulario, y además se puede acotar por arriba: antes siempre era
    '''    «desde esa fecha hasta hoy» y no había forma de mirar un tramo del mes pasado.
    '''
    ''' 2. El resultado salía en un cuadro de diálogo con un botón COPIAR, todo en una línea
    '''    separada por comas. Aquí sale un Excel con una fila por factura y de qué fichero y
    '''    de qué día viene cada una: con cientos de números pegados no había forma de saber de
    '''    dónde salía ninguno.
    '''
    ''' 3. Se avisa de los repetidos en vez de contarlos dos veces. Una misma factura puede
    '''    aparecer en dos DEVOL distintos.
    '''
    ''' 4. El invoiceNumber se busca POR NOMBRE, sin exigir el espacio de nombres. El original
    '''    lo exigía exactamente igual a «http://localhost/elegibilidad»; si un fichero viene
    '''    con otro —o sin ninguno—, allí salían cero facturas y nada lo explicaba.
    ''' </summary>
    Public Class ExtraerFacturasDevueltas
        Inherits OperacionUnica

        Public Const ClaveCarpeta As String = "carpetaImport"

        ''' <summary>Los ficheros que se miran dentro de cada carpeta del día.</summary>
        Private Const Patron As String = "DEVOL_*.xml"

        Protected Overrides Async Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                            avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim origen = ctx.Campo(ClaveCarpeta)
            If String.IsNullOrWhiteSpace(origen) OrElse Not Directory.Exists(origen) Then
                Return ResultadoEntrada.Fallo($"no existe la carpeta '{origen}'")
            End If

            Dim desde = ctx.Desde.Date
            Dim hasta = ctx.Hasta.Date

            ' Las carpetas se llaman yyyyMMdd. Se comparan como fechas y no como texto: asi
            ' «20260901» no se cuela por ser mayor que «20251231» en orden alfabetico.
            Dim delRango = Directory.GetDirectories(origen).
                Select(Function(d) New With {.Ruta = d, .Dia = DiaDe(Path.GetFileName(d))}).
                Where(Function(x) x.Dia.HasValue AndAlso
                                  x.Dia.Value >= desde AndAlso x.Dia.Value <= hasta).
                OrderBy(Function(x) x.Dia.Value).
                ToList()

            If delRango.Count = 0 Then
                Return ResultadoEntrada.SinDatos(
                    $"no hay ninguna carpeta con nombre de día entre {desde:dd/MM/yyyy} y " &
                    $"{hasta:dd/MM/yyyy} dentro de '{origen}'")
            End If

            avisar($"{Redaccion.Cuenta(delRango.Count, "carpeta")} en el rango…")

            Dim tabla = NuevaTabla()
            Dim vistas As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Dim repetidas = 0
            Dim ficheros = 0
            Dim ilegibles As New List(Of String)

            For Each carpeta In delRango
                ctx.Cancelacion.ThrowIfCancellationRequested()

                For Each fichero In Directory.GetFiles(carpeta.Ruta, Patron)
                    ficheros += 1
                    Dim soloNombre = Path.GetFileName(fichero)

                    Dim numeros As IReadOnlyList(Of String)
                    Try
                        numeros = Await Task.Run(Function() Leer(fichero)).ConfigureAwait(False)
                    Catch ex As Exception
                        ' Un XML roto no puede tumbar el recorrido: se anota y se sigue.
                        ilegibles.Add($"{soloNombre}: {ex.Message}")
                        Continue For
                    End Try

                    For Each numero In numeros
                        Dim fila = tabla.NewRow()
                        fila("NumeroFactura") = numero
                        fila("Dia") = carpeta.Dia.Value.ToString("dd/MM/yyyy")
                        fila("Fichero") = soloNombre

                        If vistas.ContainsKey(numero) Then
                            repetidas += 1
                            fila("Repetida") = $"ya estaba en {vistas(numero)}"
                        Else
                            vistas(numero) = soloNombre
                            fila("Repetida") = ""
                        End If

                        tabla.Rows.Add(fila)
                    Next

                    avisar($"{Redaccion.Cuenta(ficheros, "fichero")} · " &
                           $"{vistas.Count:N0} facturas distintas")
                Next
            Next

            If tabla.Rows.Count = 0 Then
                Dim porque = If(ficheros = 0,
                                $"no hay ningún {Patron} en esas carpetas",
                                $"los {ficheros} ficheros {Patron} no traen ningún invoiceNumber")
                Return ResultadoEntrada.SinDatos(porque)
            End If

            Dim destino = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                             RutasSalida.Asegurar(),
                             ctx.CarpetaDestino)

            Dim escritura = EscritorExcel.Escribir(tabla, destino, "FacturasDevueltas", "Devueltas")

            ' El detalle cuenta las tres cifras que interesan: cuántas hay, cuántas distintas
            ' y de cuántos ficheros salen.
            Dim detalle = $"{vistas.Count:N0} facturas de " &
                          $"{Redaccion.Cuenta(ficheros, "fichero")} en " &
                          $"{Redaccion.Cuenta(delRango.Count, "carpeta")}"

            If repetidas > 0 Then detalle &= $" · {repetidas:N0} repetidas"
            If ilegibles.Count > 0 Then detalle &= $" · {ilegibles.Count} ilegibles"

            Dim resultado = ResultadoEntrada.ConDatos(vistas.Count, detalle).
                Genera(escritura.Ficheros.ToArray())

            ' Los ficheros que no se han podido leer se dicen, no se callan: si faltan
            ' facturas hay que poder saber por qué.
            If ilegibles.Count > 0 Then
                avisar("No se han podido leer: " & String.Join(" · ", ilegibles.Take(5)))
            End If

            Return resultado

        End Function

        ''' <summary>
        ''' Los invoiceNumber de un DEVOL. Se buscan por nombre de elemento, sin exigir el
        ''' espacio de nombres: ver la nota 4 de la clase.
        ''' </summary>
        Private Shared Function Leer(ruta As String) As IReadOnlyList(Of String)

            Dim doc = XDocument.Load(ruta)

            Return doc.Descendants().
                Where(Function(e) e.Name.LocalName = "invoiceNumber").
                Select(Function(e) e.Value.Trim()).
                Where(Function(v) v.Length > 0).
                ToList()

        End Function

        ''' <summary>
        ''' El día de una carpeta llamada yyyyMMdd. Nothing si no lo es: dentro de Import hay
        ''' también carpetas con otros nombres y no son de un día.
        ''' </summary>
        Private Shared Function DiaDe(nombre As String) As Date?

            Dim dia As Date
            If nombre IsNot Nothing AndAlso nombre.Length = 8 AndAlso
               Date.TryParseExact(nombre, "yyyyMMdd", Globalization.CultureInfo.InvariantCulture,
                                  Globalization.DateTimeStyles.None, dia) Then
                Return dia
            End If

            Return Nothing

        End Function

        Private Shared Function NuevaTabla() As DataTable

            Dim t As New DataTable("Devueltas")
            t.Columns.Add("NumeroFactura", GetType(String))
            t.Columns.Add("Dia", GetType(String))
            t.Columns.Add("Fichero", GetType(String))
            t.Columns.Add("Repetida", GetType(String))
            Return t

        End Function

    End Class

End Namespace
