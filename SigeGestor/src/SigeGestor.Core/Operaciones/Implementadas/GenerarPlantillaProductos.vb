Imports System.IO
Imports ClosedXML.Excel
Imports SigeGestor.Core.Configuracion

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Crea el Excel vacío con las columnas que espera «Importar productos desde Excel».
    ''' Es el botón «Generar Plantilla» de ProductosAsig.
    '''
    ''' Aquí sí se ha reescrito, y con motivo: el original son veinte líneas de cabeceras y
    ''' estilos, no hay lógica de negocio que preservar, y las columnas se toman de una única
    ''' constante compartida con la importación —<see cref="ImportarProductos.Columnas"/>— para
    ''' que no puedan volver a desincronizarse la plantilla y el lector.
    ''' </summary>
    Public Class GenerarPlantillaProductos
        Inherits OperacionUnica

        Protected Overrides Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                      avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            Dim carpeta = If(String.IsNullOrWhiteSpace(ctx.CarpetaDestino),
                             RutasSalida.Predeterminada,
                             ctx.CarpetaDestino)
            Directory.CreateDirectory(carpeta)

            ' El original escribía siempre PlantillaProductoAsignacion.xlsx y machacaba la
            ' anterior. Da igual porque va vacía, pero si estaba abierta en Excel el guardado
            ' fallaba; con sello no se pisa nunca.
            Dim ruta = Path.Combine(carpeta,
                                    $"PlantillaProductoAsignacion_{DateTime.Now:yyyyMMdd_HHmm}.xlsx")

            avisar("Generando la plantilla…")

            Using libro As New XLWorkbook()

                Dim hoja = libro.Worksheets.Add("Plantilla")

                For i = 0 To ImportarProductos.Columnas.Length - 1
                    hoja.Cell(1, i + 1).Value = ImportarProductos.Columnas(i)
                Next

                Dim cabecera = hoja.Range(1, 1, 1, ImportarProductos.Columnas.Length)
                cabecera.Style.Font.Bold = True
                cabecera.Style.Fill.BackgroundColor = XLColor.LightGray

                ' Las dos primeras en rojo, como en el original: son las obligatorias.
                hoja.Range(1, 1, 1, 2).Style.Font.FontColor = XLColor.Red

                hoja.ColumnsUsed.AdjustToContents()

                libro.SaveAs(ruta)

            End Using

            Return Task.FromResult(ResultadoEntrada.ConDatos(
                ImportarProductos.Columnas.Length,
                $"{Path.GetFileName(ruta)} · las dos primeras columnas son obligatorias") _
                .Genera(ruta))

        End Function

    End Class

End Namespace
