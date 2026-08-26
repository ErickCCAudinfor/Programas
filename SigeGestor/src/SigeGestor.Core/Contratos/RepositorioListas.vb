Imports System.Threading
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Operaciones

Namespace Contratos

    ''' <summary>Una opción de desplegable: su Id y lo que se lee.</summary>
    Public Class OpcionLista
        Public Property Id As Long
        Public Property Nombre As String = ""
        Public Property Entorno As String = ""

        ''' <summary>
        ''' Valor que llega al ejecutable cuando no es un Id numérico. Lo usan los orígenes de
        ''' configuración: el tipo de XML no tiene Id, lo que hace falta son su raíz y su nodo.
        ''' Vacío significa «usa el Id».
        ''' </summary>
        Public Property Valor As String = ""

        Public ReadOnly Property Etiqueta As String
            Get
                If String.IsNullOrWhiteSpace(Entorno) Then Return Nombre
                Return $"{Nombre} ({Entorno})"
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Listas de apoyo para los campos de selección: agentes, administradores.
    '''
    ''' Se filtra por entorno G1 y G2 igual que GetAgente y GetAdmind de ActualizaPrecios.
    ''' </summary>
    Public Class RepositorioListas

        Public Async Function CargarAsync(cadenaConexion As String,
                                          origen As String,
                                          Optional ct As CancellationToken = Nothing,
                                          Optional filtro As String = "") _
            As Task(Of IReadOnlyList(Of OpcionLista))

            ' Los orígenes de configuración no van a la base: se resuelven aquí y se sale antes
            ' de abrir conexión, que además permite usarlos sin haber elegido entorno.
            If OrigenLista.EsDeConfiguracion(origen) Then
                Return RepositorioTiposXml.Cargar()
            End If

            ' Los que dependen de otro campo devuelven lista vacía hasta que hay filtro, en vez
            ' de traer la tabla entera: los clientes de pago son decenas de miles.
            If OrigenLista.NecesitaFiltro(origen) AndAlso String.IsNullOrWhiteSpace(filtro) Then
                Return Array.Empty(Of OpcionLista)()
            End If

            Dim consulta As String

            Select Case origen
                Case OrigenLista.Agentes
                    consulta = "SELECT IdAgente, NombreAgente, Entorno FROM agente " &
                               "WHERE Entorno IN ('G1','G2') ORDER BY NombreAgente"

                Case OrigenLista.Administradores
                    ' Ojo: GetAdminAll de ActualizaPrecios consulta la tabla «agente» por lo
                    ' que parece un copia y pega; la buena es GetAdmind, que va a Administrador.
                    consulta = "SELECT IdAdministrador, NombreAdministrador, Entorno FROM Administrador " &
                               "WHERE Entorno IN ('G1','G2') ORDER BY NombreAdministrador"

                Case OrigenLista.Productos
                    ' Los dos entornos a la vez, con la etiqueta detrás. Es la misma consulta de
                    ' GetProductosbyEntorno pero sin fijar el entorno, y el Entorno de cada fila
                    ' se traduce a «luz»/«gas» para que se lea en el desplegable.
                    consulta = "SELECT IdProducto, TextoProducto, " &
                               "       CASE Entorno WHEN 'G1' THEN 'luz' WHEN 'G2' THEN 'gas' ELSE Entorno END " &
                               "FROM Producto WHERE Entorno IN ('G1','G2') ORDER BY Entorno, TextoProducto"

                Case OrigenLista.TiposImpuesto
                    consulta = "SELECT IdTipoImpuesto, TextoImpuesto, Entorno FROM TipoImpuesto " &
                               "ORDER BY TextoImpuesto"

                ' --- Listas de «Masivo contrato». Son las mismas consultas que los
                '     Get… de FuncionesGenericas, con ORDER BY para que el desplegable
                '     salga alfabético: el original los dejaba en el orden de la tabla.
                Case OrigenLista.SituacionesContrato
                    consulta = "SELECT IdContratoSituacion, TextoSituacion, Entorno " &
                               "FROM ContratoSituacion ORDER BY TextoSituacion"

                Case OrigenLista.Cnaes
                    consulta = "SELECT IdCNAE, TextoCNAE, CodigoCNAE FROM cnae ORDER BY TextoCNAE"

                Case OrigenLista.Colectivos
                    consulta = "SELECT IdColectivo, TextoColectivo, Entorno FROM Colectivo " &
                               "ORDER BY TextoColectivo"

                Case OrigenLista.SituacionesScoring
                    ' El contrato guarda el NOMBRE, no el Id: ver SituacionScoring='…' en el
                    ' GetParameters del original. Por eso el nombre va también como Valor.
                    consulta = "SELECT IdSituacionScoring, Nombre, Nombre FROM SituacionScoring " &
                               "ORDER BY Nombre"

                Case OrigenLista.TiposAutoconsumo
                    consulta = "SELECT IdTipoAutoconsumo, TextoAutoconsumo, Entorno " &
                               "FROM TiposAutoconsumo WHERE Entorno = 'U' ORDER BY TextoAutoconsumo"

                Case OrigenLista.ModelosFactura, OrigenLista.ModelosFacturaVarios, OrigenLista.ModelosContrato
                    ' Un solo sitio para los tres: solo cambia CodigoTipoModeloDeImpresion.
                    Dim tipo = If(origen = OrigenLista.ModelosFactura, 1,
                                  If(origen = OrigenLista.ModelosFacturaVarios, 9, 4))
                    consulta = "SELECT idmodelodeimpresion, DescripcionModeloDeImpresion, Entorno " &
                               $"FROM ModeloDeImpresion WHERE CodigoTipoModeloDeImpresion = {tipo} " &
                               "ORDER BY DescripcionModeloDeImpresion"

                Case OrigenLista.ClientesPago
                    ' Misma consulta y mismo texto unificado que GetClientePago, pero con el CIF
                    ' como parámetro en vez de interpolado: aquí el valor lo teclea el usuario.
                    consulta = "SELECT cp.idclientepago, " &
                               "ISNULL(NombreP,'???')+'/'+ISNULL(IdentidadPago,'???')+'/'+" &
                               "ISNULL(TextoColectivo,'???')+'/'+ISNULL(TextoTipoCobro,'???')+'/'+" &
                               "ISNULL(IBAN,'???')+'/'+ISNULL(TextoBanco,'???'), '' " &
                               "FROM clientepago cp " &
                               "LEFT JOIN tipocobro tc ON cp.idtipocobro = tc.idtipocobro " &
                               "LEFT JOIN Colectivo c ON cp.IdColectivo = c.IdColectivo " &
                               "LEFT JOIN Banco b ON cp.IdBanco = b.IdBanco " &
                               "LEFT JOIN cliente cl ON cp.idcliente = cl.idcliente " &
                               "WHERE cl.Identidad = @filtro"

                Case Else
                    Throw New ArgumentException($"Origen de lista desconocido: '{origen}'.", NameOf(origen))
            End Select

            Dim opciones As New List(Of OpcionLista)

            Using conexion As New SqlConnection(cadenaConexion)
                Await conexion.OpenAsync(ct).ConfigureAwait(False)

                Using comando As New SqlCommand(consulta, conexion)
                    comando.CommandTimeout = 60
                    If consulta.Contains("@filtro") Then
                        comando.Parameters.AddWithValue("@filtro", filtro.Trim())
                    End If

                    ' La situación de scoring es la excepción: el contrato guarda el NOMBRE, no
                    ' el Id, así que lo que tiene que viajar es el texto y no el número.
                    Dim valorEsElNombre = (origen = OrigenLista.SituacionesScoring)

                    Using lector = Await comando.ExecuteReaderAsync(ct).ConfigureAwait(False)
                        While Await lector.ReadAsync(ct).ConfigureAwait(False)

                            Dim nombre = If(lector.IsDBNull(1), "", Convert.ToString(lector.GetValue(1))).Trim()

                            opciones.Add(New OpcionLista With {
                                .Id = If(lector.IsDBNull(0), 0L, Convert.ToInt64(lector.GetValue(0))),
                                .Nombre = nombre,
                                .Entorno = If(lector.IsDBNull(2), "", Convert.ToString(lector.GetValue(2))).Trim(),
                                .Valor = If(valorEsElNombre, nombre, "")
                            })

                        End While
                    End Using
                End Using
            End Using

            Return opciones

        End Function

        ''' <summary>
        ''' La pareja del otro entorno de una opción de agente o administrador.
        '''
        ''' En SIGE cada agente y cada administrador está por duplicado, uno con entorno G1 —luz—
        ''' y otro con G2 —gas—, y el nombre lleva delante el prefijo del entorno. El contrato
        ''' apunta al Id del suyo, así que escribir el de luz en un contrato de gas deja el
        ''' contrato apuntando a un agente del otro entorno.
        '''
        ''' Es la misma regla que ComboBox1_SelectedIndexChanged de Agentes y AdministradoresWF:
        ''' se quita el prefijo del nombre y se busca en el otro entorno un nombre que lo
        ''' contenga. Devuelve Nothing si no hay pareja, y entonces esos contratos se dejan.
        ''' </summary>
        Public Shared Function Pareja(opciones As IEnumerable(Of OpcionLista),
                                      elegida As OpcionLista,
                                      entornoBuscado As String) As OpcionLista

            If elegida Is Nothing OrElse opciones Is Nothing Then Return Nothing
            If elegida.Entorno.StartsWith(entornoBuscado, StringComparison.OrdinalIgnoreCase) Then Return elegida

            Dim sinPrefijo = elegida.Nombre
            For Each prefijo In {"G1 ", "G2 "}
                sinPrefijo = sinPrefijo.Replace(prefijo, "")
            Next
            sinPrefijo = sinPrefijo.Trim()

            If sinPrefijo.Length = 0 Then Return Nothing

            Return opciones.FirstOrDefault(
                Function(o) o.Entorno.StartsWith(entornoBuscado, StringComparison.OrdinalIgnoreCase) AndAlso
                            o.Nombre.IndexOf(sinPrefijo, StringComparison.OrdinalIgnoreCase) >= 0)

        End Function

    End Class

End Namespace
