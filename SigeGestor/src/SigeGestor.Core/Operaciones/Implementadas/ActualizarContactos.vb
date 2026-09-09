Option Strict Off   ' Usa la clase portada.

Namespace Operaciones.Implementadas

    ''' <summary>
    ''' Actualiza email y teléfono o móvil de los contratos de un Excel de tres columnas:
    ''' CodContrato, Emails —varios separados por punto y coma— y TlfnoMovil.
    '''
    ''' El trabajo lo hace ActualizarEmailFromExcel, portada literal de ActualizaPrecios.
    '''
    ''' ES UNA SOLA OPERACIÓN, NO UNA POR FILA: el método portado recorre el Excel por dentro y
    ''' devuelve un contador. Partirlo por filas obligaría a reescribir sus 200 líneas de
    ''' casuística de contactos, que es justo lo que no interesa tocar. Lo que sí se hace es
    ''' recoger las incidencias que antes solo iban a un fichero y decirlas aquí.
    ''' </summary>
    Public Class ActualizarContactos
        Inherits OperacionUnica
        Implements IEsquemaExcel

        ''' <summary>
        ''' Las tres columnas, en el orden en que las lee ActualizarEmailFromExcel: Cells(fila,1),
        ''' Cells(fila,2) y Cells(fila,3). Se lee POR POSICIÓN, así que el título de la cabecera
        ''' da igual; lo que importa es que cada cosa esté en su columna.
        ''' </summary>
        Public ReadOnly Property Esquema As EsquemaExcel Implements IEsquemaExcel.Esquema
            Get
                Return New EsquemaExcel(
                    {
                        New ColumnaExcel("CodContrato", "5048104",
                                         "Solo el número, sin letras ni espacios"),
                        New ColumnaExcel("Emails", "uno@empresa.es;dos@empresa.es",
                                         "Varios, separados por punto y coma",
                                         obligatoria:=False),
                        New ColumnaExcel("TlfnoMovil", "637669376",
                                         "9 dígitos. Si empieza por 6 o 7 se guarda como móvil; por 8 o 9, como fijo",
                                         obligatoria:=False)
                    },
                    aviso:="Cada fila necesita al menos el email o el teléfono. Si faltan los dos, " &
                           "la fila se salta y queda anotada en las incidencias.")
            End Get
        End Property

        Protected Overrides Async Function EjecutarUnaAsync(ctx As ContextoEjecucion,
                                                            avisar As Action(Of String)) As Task(Of ResultadoEntrada)

            If Not IO.File.Exists(ctx.RutaExcel) Then
                Return ResultadoEntrada.Fallo("elige el fichero de Excel")
            End If

            ' Se comprueba la conexión ANTES de recorrer el Excel. Sin esto, con la base caída
            ' el método portado no falla: GetContrato se traga el error y devuelve un contrato
            ' vacío, así que el fichero de incidencias sale con «No existe en BD» de todas las
            ' filas y no hay forma de distinguirlo de un Excel con contratos inventados.
            avisar("Comprobando la conexión…")
            Dim problema = Await ctx.ProbarConexionAsync().ConfigureAwait(False)
            If problema.Length > 0 Then
                Return ResultadoEntrada.Fallo($"no se puede conectar a {ctx.Entorno?.Nombre}: {problema}")
            End If

            avisar("Leyendo el Excel y actualizando contactos…")

            Dim actualizador As New ActualizarEmailFromExcel(ctx.CadenaConexion) With {
                .RutaExcel = ctx.RutaExcel
            }

            Dim actualizados = Await actualizador.ActualizarEmailFromExcelAsync().ConfigureAwait(False)

            Dim incidencias = actualizador.Incidencias.Count

            ' El contador que devuelve el método portado solo cuenta los EMAILS actualizados: el
            ' de teléfonos se cuenta aparte en una variable local que no sale del método. Así que
            ' aquí no se puede afirmar cuántos teléfonos se han tocado, y no se inventa: para eso
            ' está el fichero de incidencias, que sí lista línea a línea lo que ha pasado.
            '
            ' La RUTA de ese fichero ya no se pega al mensaje: va en Salidas y la pantalla la
            ' pinta entera, con un botón para abrirla. Antes se cortaba en «...\ConsultasBO\Con…»
            ' y no servía de nada.
            If actualizados = 0 AndAlso incidencias = 0 Then
                Return ResultadoEntrada.SinDatos("el Excel no tenía filas con datos")
            End If

            Dim conIncidencias = If(incidencias = 0, "",
                                    Redaccion.Cuenta(incidencias, "incidencia anotada",
                                                                  "incidencias anotadas"))

            If actualizados = 0 Then
                Return ResultadoEntrada.SinDatos(
                    Redaccion.Unir("ningún email actualizado", conIncidencias)) _
                    .Genera(actualizador.RutaIncidencias)
            End If

            Return ResultadoEntrada.ConDatos(actualizados,
                Redaccion.Unir(Redaccion.Cuenta(actualizados, "email actualizado",
                                                              "emails actualizados"),
                               conIncidencias)) _
                .Genera(actualizador.RutaIncidencias)

        End Function

    End Class

End Namespace
