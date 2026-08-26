Imports System.Data
Imports System.Threading
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Operaciones

Namespace Contratos

    ''' <summary>Lo mínimo de un contrato para decidir si se puede tocar.</summary>
    Public Class ContratoBreve
        Public Property CodigoContrato As Long
        Public Property IdContratoSituacion As Integer

        ''' <summary>
        ''' E1 luz, E2 gas. Hace falta para elegir el agente o el administrador correcto: en
        ''' SIGE están duplicados, uno por entorno, y el contrato apunta al del suyo.
        ''' </summary>
        Public Property Entorno As String = ""

        ''' <summary>Entorno con el que se nombran agentes, administradores y productos.</summary>
        Public ReadOnly Property EntornoMaestros As String
            Get
                Return If(String.Equals(Entorno, "E1", StringComparison.OrdinalIgnoreCase), "G1", "G2")
            End Get
        End Property

        ''' <summary>
        ''' Situación 1 = activo. Es el filtro que aplica ActualizaPrecios en todas las
        ''' operaciones masivas antes de escribir.
        ''' </summary>
        Public ReadOnly Property Activo As Boolean
            Get
                Return IdContratoSituacion = SituacionActiva
            End Get
        End Property

        Public Const SituacionActiva As Integer = 1
    End Class

    ''' <summary>
    ''' Resuelve lo que el usuario pega —código de contrato, CUPS o CIF— a los contratos
    ''' correspondientes.
    '''
    ''' Está en un solo sitio porque lo necesitan todas las operaciones masivas, y porque las
    ''' consultas no son evidentes: la de CUPS es un LIKE por prefijo, no una igualdad.
    ''' </summary>
    Public Class RepositorioContratos

        ''' <summary>
        ''' Los CUPS se comparan por los 20 primeros caracteres. En la base pueden llevar
        ''' sufijo de punto de medida, así que una igualdad exacta dejaría fuera contratos que
        ''' sí existen. ActualizaPrecios recorta a 20 y hace LIKE 'prefijo%'.
        ''' </summary>
        Public Const LongitudCups As Integer = 20

        Public Async Function ResolverAsync(cadenaConexion As String,
                                            entrada As String,
                                            tipo As TipoLista,
                                            Optional ct As CancellationToken = Nothing) _
            As Task(Of IReadOnlyList(Of ContratoBreve))

            Dim consulta As String
            Dim valor As String = entrada.Trim()

            Select Case tipo
                Case TipoLista.Cups
                    ' Igual que GetListContratobyCUPS: CTE sobre CUPS por prefijo y luego los
                    ' contratos de esos IdCups.
                    consulta =
                        ";WITH ids AS (SELECT IdCups FROM CUPS WHERE CodigoCups LIKE @valor + '%') " &
                        "SELECT CodigoContrato, IdContratoSituacion, Entorno FROM Contrato " &
                        "WHERE IdCups IN (SELECT IdCups FROM ids)"
                    If valor.Length > LongitudCups Then valor = valor.Substring(0, LongitudCups)

                Case TipoLista.Cifs
                    consulta =
                        "SELECT c.CodigoContrato, c.IdContratoSituacion, c.Entorno FROM Contrato c " &
                        "INNER JOIN Cliente cl ON c.IdCliente = cl.IdCliente " &
                        "WHERE cl.Identidad = @valor"

                Case Else
                    consulta = "SELECT CodigoContrato, IdContratoSituacion, Entorno FROM Contrato WHERE CodigoContrato = @valor"
            End Select

            Dim encontrados As New List(Of ContratoBreve)

            Using conexion As New SqlConnection(cadenaConexion)
                Await conexion.OpenAsync(ct).ConfigureAwait(False)

                Using comando As New SqlCommand(consulta, conexion)
                    comando.CommandTimeout = 120

                    If tipo = TipoLista.Contratos Then
                        Dim codigo As Long
                        If Not Long.TryParse(valor, codigo) Then
                            Return encontrados
                        End If
                        comando.Parameters.Add("@valor", SqlDbType.BigInt).Value = codigo
                    Else
                        comando.Parameters.Add("@valor", SqlDbType.VarChar, 40).Value = valor
                    End If

                    Using lector = Await comando.ExecuteReaderAsync(ct).ConfigureAwait(False)
                        While Await lector.ReadAsync(ct).ConfigureAwait(False)
                            encontrados.Add(New ContratoBreve With {
                                .CodigoContrato = If(lector.IsDBNull(0), 0L, Convert.ToInt64(lector.GetValue(0))),
                                .IdContratoSituacion = If(lector.IsDBNull(1), 0, Convert.ToInt32(lector.GetValue(1))),
                                .Entorno = If(lector.IsDBNull(2), "", Convert.ToString(lector.GetValue(2))).Trim()
                            })
                        End While
                    End Using
                End Using
            End Using

            Return encontrados

        End Function

        ''' <summary>
        ''' Vuelve a marcar como pendiente de renovación un contrato, poniendo
        ''' IsRenovacionProcesada a NULL. Devuelve las filas afectadas.
        '''
        ''' Parametrizado, al contrario que el original, que concatenaba el código en el SQL.
        ''' </summary>
        Public Async Function VolverARenovarAsync(cadenaConexion As String,
                                                  codigoContrato As Long,
                                                  Optional ct As CancellationToken = Nothing) As Task(Of Integer)

            Const consulta As String =
                "UPDATE Contrato SET IsRenovacionProcesada = NULL WHERE CodigoContrato = @codigo"

            Using conexion As New SqlConnection(cadenaConexion)
                Await conexion.OpenAsync(ct).ConfigureAwait(False)

                Using comando As New SqlCommand(consulta, conexion)
                    comando.CommandTimeout = 120
                    comando.Parameters.Add("@codigo", SqlDbType.BigInt).Value = codigoContrato
                    Return Await comando.ExecuteNonQueryAsync(ct).ConfigureAwait(False)
                End Using
            End Using

        End Function

        ''' <summary>
        ''' Fija los tres códigos DIR de un contrato.
        '''
        ''' Parametrizado. El original los concatenaba en el SQL, así que un apóstrofo en
        ''' cualquiera de los tres rompía la consulta — y encima se tragaba la excepción con un
        ''' Console.WriteLine y devolvía 0, de modo que el fallo se veía como «0 afectados».
        ''' </summary>
        Public Async Function ActualizarCodigosDirAsync(cadenaConexion As String,
                                                        codigoContrato As Long,
                                                        unidadTramitadora As String,
                                                        oficinaContable As String,
                                                        organoGestor As String,
                                                        Optional ct As CancellationToken = Nothing) As Task(Of Integer)

            Const consulta As String =
                "UPDATE Contrato SET CodigoUnidadTramitadora = @unidad, " &
                "CodigoOficinaContable = @oficina, CodigoOrganoGestor = @organo " &
                "WHERE CodigoContrato = @codigo"

            Using conexion As New SqlConnection(cadenaConexion)
                Await conexion.OpenAsync(ct).ConfigureAwait(False)

                Using comando As New SqlCommand(consulta, conexion)
                    comando.CommandTimeout = 120
                    comando.Parameters.Add("@codigo", SqlDbType.BigInt).Value = codigoContrato
                    comando.Parameters.Add("@unidad", SqlDbType.VarChar, 50).Value = unidadTramitadora
                    comando.Parameters.Add("@oficina", SqlDbType.VarChar, 50).Value = oficinaContable
                    comando.Parameters.Add("@organo", SqlDbType.VarChar, 50).Value = organoGestor
                    Return Await comando.ExecuteNonQueryAsync(ct).ConfigureAwait(False)
                End Using
            End Using

        End Function

        ''' <summary>Columnas que se admiten en <see cref="ReasignarAsync"/>.</summary>
        Public Const ColumnaAgente As String = "IdAgente"
        Public Const ColumnaAdministrador As String = "IdAdministrador"

        Private Shared ReadOnly ColumnasPermitidas As String() = {ColumnaAgente, ColumnaAdministrador}

        ''' <summary>
        ''' Reasigna agente o administrador. Con <paramref name="idDestino"/> en Nothing deja
        ''' la columna en NULL, que es el «permitir NULL» del original.
        '''
        ''' El nombre de columna NO puede parametrizarse en SQL, así que se comprueba contra una
        ''' lista blanca antes de interpolarlo. Es la única forma de que no se cuele nada.
        ''' </summary>
        Public Async Function ReasignarAsync(cadenaConexion As String,
                                             columna As String,
                                             codigoContrato As Long,
                                             idDestino As Long?,
                                             Optional ct As CancellationToken = Nothing) As Task(Of Integer)

            If Not ColumnasPermitidas.Contains(columna) Then
                Throw New ArgumentException($"Columna no permitida: '{columna}'.", NameOf(columna))
            End If

            Dim consulta = $"UPDATE Contrato SET {columna} = @destino WHERE CodigoContrato = @codigo"

            Using conexion As New SqlConnection(cadenaConexion)
                Await conexion.OpenAsync(ct).ConfigureAwait(False)

                Using comando As New SqlCommand(consulta, conexion)
                    comando.CommandTimeout = 120
                    comando.Parameters.Add("@codigo", SqlDbType.BigInt).Value = codigoContrato
                    comando.Parameters.Add("@destino", SqlDbType.BigInt).Value =
                        If(idDestino.HasValue, CObj(idDestino.Value), DBNull.Value)
                    Return Await comando.ExecuteNonQueryAsync(ct).ConfigureAwait(False)
                End Using
            End Using

        End Function

    End Class

End Namespace
