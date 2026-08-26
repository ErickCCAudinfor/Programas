Option Strict Off   ' Usa los DTO portados.

Imports System.IO
Imports System.Threading
Imports SigeGestor.Core.Configuracion
Imports SigeGestor.Core.Modelos

Namespace Impresion

    ''' <summary>Resultado de una operación masiva sobre una empresa.</summary>
    Public Class ResultadoEmpresa

        Public Property Empresa As String = ""
        Public Property Bien As Boolean
        Public Property Detalle As String = ""

        Public Shared Function Ok(empresa As String, detalle As String) As ResultadoEmpresa
            Return New ResultadoEmpresa With {.Empresa = empresa, .Bien = True, .Detalle = detalle}
        End Function

        Public Shared Function Mal(empresa As String, detalle As String) As ResultadoEmpresa
            Return New ResultadoEmpresa With {.Empresa = empresa, .Bien = False, .Detalle = detalle}
        End Function

    End Class

    ''' <summary>
    ''' Mantenimiento de los modelos de impresión: los de una empresa, y las tres operaciones
    ''' masivas sobre varias a la vez.
    '''
    ''' Todo el SQL es el ya portado en FuncionesGenericas —GetAllModelosImpresion,
    ''' GetModeloBinario, InsertModeloImpresion, UpdateModeloImpresion,
    ''' UpdateModeloImpresionPorClave, ExisteModeloImpresionPorTipo y DeleteModeloImpresion—.
    ''' Esta clase solo lo saca del hilo de interfaz y recoge lo que ha pasado con cada empresa.
    '''
    ''' POR QUÉ NO ES UNA OPERACIÓN DEL CATÁLOGO: las demás recorren una lista pegada y
    ''' escriben. Esto es una rejilla de mantenimiento contra N bases distintas, con un binario
    ''' que se sube por fichero. No cabe en el formulario genérico, así que tiene página propia.
    ''' </summary>
    Public Class ServicioModelosImpresion

        Private ReadOnly _empresas As New RepositorioEmpresas()

        ''' <summary>Los modelos de una empresa, sin el binario: son megas por fila.</summary>
        Public Function Listar(empresa As EmpresaBD) As Task(Of IReadOnlyList(Of ModeloDeImpresion))

            Return Task.Run(
                Function() As IReadOnlyList(Of ModeloDeImpresion)
                    Dim funciones As New FuncionesGenericas(_empresas.CadenaConexion(empresa, 30))
                    Dim lista = funciones.GetAllModelosImpresion()
                    Return If(lista, New List(Of ModeloDeImpresion)())
                End Function)

        End Function

        ''' <summary>El binario de un modelo. Se pide solo cuando se va a editar o descargar.</summary>
        Public Function Binario(empresa As EmpresaBD, idModelo As Long) As Task(Of Byte())

            Return Task.Run(
                Function() As Byte()
                    Dim funciones As New FuncionesGenericas(_empresas.CadenaConexion(empresa, 60))
                    Dim m = funciones.GetModeloBinario(idModelo)
                    Return If(m Is Nothing, Nothing, m.Modelo)
                End Function)

        End Function

        Public Function Guardar(empresa As EmpresaBD,
                                modelo As ModeloDeImpresion,
                                esNuevo As Boolean) As Task(Of Long)

            Return Task.Run(
                Function() As Long
                    Dim funciones As New FuncionesGenericas(_empresas.CadenaConexion(empresa, 60))
                    If esNuevo Then Return funciones.InsertModeloImpresion(modelo)
                    Return funciones.UpdateModeloImpresion(modelo)
                End Function)

        End Function

        Public Function Borrar(empresa As EmpresaBD, idModelo As Long) As Task(Of Long)

            Return Task.Run(
                Function() As Long
                    Dim funciones As New FuncionesGenericas(_empresas.CadenaConexion(empresa, 30))
                    Return funciones.DeleteModeloImpresion(idModelo)
                End Function)

        End Function

        ''' <summary>Lee un .rpt del disco a la forma que espera el modelo.</summary>
        Public Shared Sub CargarReport(modelo As ModeloDeImpresion, ruta As String)
            modelo.Modelo = File.ReadAllBytes(ruta)
            modelo.RptFileName = Path.GetFileName(ruta)
        End Sub

        ' ==================================================================
        ' Masivo sobre varias empresas
        ' ==================================================================

        ''' <summary>
        ''' Recorre las empresas aplicando <paramref name="hacer"/> a cada una, informando por
        ''' cuál va y sin que el fallo de una detenga las demás.
        '''
        ''' Es lo que hacían por triplicado BotonSubirMasivo, BotonComprobarModelo y
        ''' BotonActualizarMasivo, con el mismo bucle copiado tres veces.
        ''' </summary>
        Private Async Function RecorrerAsync(empresas As IEnumerable(Of EmpresaBD),
                                             hacer As Func(Of FuncionesGenericas, EmpresaBD, ResultadoEmpresa),
                                             avisar As Action(Of String),
                                             ct As CancellationToken) _
            As Task(Of IReadOnlyList(Of ResultadoEmpresa))

            Dim resultados As New List(Of ResultadoEmpresa)

            For Each empresa In empresas
                ct.ThrowIfCancellationRequested()
                avisar($"{empresa.Nombre}…")

                Try
                    Dim cadena = _empresas.CadenaConexion(empresa, 60)
                    Dim r = Await Task.Run(
                        Function() hacer(New FuncionesGenericas(cadena), empresa), ct).ConfigureAwait(False)
                    resultados.Add(r)

                Catch ex As OperationCanceledException
                    Throw
                Catch ex As Exception
                    Dim motivo = ex.Message
                    If empresa.VPN Then motivo &= " · requiere VPN"
                    resultados.Add(ResultadoEmpresa.Mal(empresa.Nombre, motivo))
                End Try
            Next

            Return resultados

        End Function

        ''' <summary>Inserta el modelo en cada empresa.</summary>
        Public Function SubirAsync(empresas As IEnumerable(Of EmpresaBD),
                                   modelo As ModeloDeImpresion,
                                   avisar As Action(Of String),
                                   ct As CancellationToken) As Task(Of IReadOnlyList(Of ResultadoEmpresa))

            Return RecorrerAsync(empresas,
                Function(funciones, empresa)
                    Dim id = funciones.InsertModeloImpresion(modelo)
                    If id > 0 Then Return ResultadoEmpresa.Ok(empresa.Nombre, $"insertado con Id {id}")
                    Return ResultadoEmpresa.Mal(empresa.Nombre, "no se insertó el modelo")
                End Function, avisar, ct)

        End Function

        ''' <summary>
        ''' Actualiza por clave —descripción, entorno y tipo— en cada empresa. Que no afecte a
        ''' ninguna fila no es un fallo: es que ahí no existe ese modelo.
        ''' </summary>
        Public Function ActualizarAsync(empresas As IEnumerable(Of EmpresaBD),
                                        modelo As ModeloDeImpresion,
                                        avisar As Action(Of String),
                                        ct As CancellationToken) As Task(Of IReadOnlyList(Of ResultadoEmpresa))

            Return RecorrerAsync(empresas,
                Function(funciones, empresa)
                    Dim filas = funciones.UpdateModeloImpresionPorClave(modelo)
                    If filas > 0 Then Return ResultadoEmpresa.Ok(empresa.Nombre, $"{filas} actualizado")
                    Return ResultadoEmpresa.Mal(empresa.Nombre, "no existe ese modelo aquí")
                End Function, avisar, ct)

        End Function

        ''' <summary>
        ''' Comprueba en cada empresa si existe el modelo de ese tipo y descripción con binario
        ''' de verdad. La condición «LEN(modelo) >= 10» del original es lo que distingue un
        ''' modelo subido de una fila creada con el binario vacío.
        ''' </summary>
        Public Function ComprobarAsync(empresas As IEnumerable(Of EmpresaBD),
                                       codigoTipo As Integer,
                                       descripcion As String,
                                       avisar As Action(Of String),
                                       ct As CancellationToken) As Task(Of IReadOnlyList(Of ResultadoEmpresa))

            Return RecorrerAsync(empresas,
                Function(funciones, empresa)
                    If funciones.ExisteModeloImpresionPorTipo(codigoTipo, descripcion) Then
                        Return ResultadoEmpresa.Ok(empresa.Nombre, "lo tiene")
                    End If
                    Return ResultadoEmpresa.Mal(empresa.Nombre, "le falta")
                End Function, avisar, ct)

        End Function

    End Class

End Namespace
