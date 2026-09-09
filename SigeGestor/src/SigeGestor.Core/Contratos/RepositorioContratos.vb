Imports System.Data
Imports System.Threading
Imports Microsoft.Data.SqlClient
Imports SigeGestor.Core.Operaciones

Namespace Contratos

    ''' <summary>
    ''' Qué suministro tiene la lista que se ha pegado.
    '''
    ''' Hace falta porque los maestros de SIGE están duplicados por entorno: un producto de luz
    ''' vive en G1 y uno de gas en G2, y no se pueden cruzar. Sabiéndolo antes de elegir, el
    ''' desplegable puede traer solo los que valen.
    ''' </summary>
    Public Enum TipoSuministro

        ''' <summary>No se ha podido averiguar: lista vacía, contratos que no existen o fallo
        ''' de conexión. No se filtra nada y la comprobación por contrato hace su trabajo.</summary>
        SinResolver

        Luz
        Gas

        ''' <summary>Hay contratos de los dos. No se puede asignar un solo producto.</summary>
        Mezclado

    End Enum

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

        ''' <summary>Entradas por consulta. SQL Server no admite más de 2.100 parámetros.</summary>
        Private Const PorLote As Integer = 500

        ''' <summary>
        ''' Si la lista pegada es de luz, de gas, de las dos cosas o no se sabe.
        '''
        ''' UNA CONSULTA POR LOTE Y NO UNA POR ENTRADA: ResolverAsync resuelve de una en una, y
        ''' para 200 contratos serían 200 viajes cada vez que se toca el cuadro de texto. Aquí
        ''' se pregunta por 500 a la vez con un DISTINCT, así que vuelven dos filas como mucho.
        '''
        ''' Y se corta en cuanto aparecen los dos entornos: si la lista está mezclada, con el
        ''' primer lote ya está dicho y no hace falta recorrer los 5.000 restantes.
        '''
        ''' Devuelve SinResolver si algo falla. Es a propósito: esto sirve para ayudar a elegir,
        ''' no para autorizar nada. Quien decide de verdad es la comprobación que hace cada
        ''' operación contrato a contrato antes de escribir.
        ''' </summary>
        Public Async Function SuministroDeAsync(cadenaConexion As String,
                                                entradas As IReadOnlyList(Of String),
                                                tipo As TipoLista,
                                                Optional ct As CancellationToken = Nothing) _
            As Task(Of TipoSuministro)

            If entradas Is Nothing OrElse entradas.Count = 0 Then Return TipoSuministro.SinResolver
            If String.IsNullOrWhiteSpace(cadenaConexion) Then Return TipoSuministro.SinResolver

            ' Se normaliza y se quitan repetidos antes de preguntar: una lista pegada suele
            ' traer el mismo CIF muchas veces.
            Dim claves = Normalizar(entradas, tipo)
            If claves.Count = 0 Then Return TipoSuministro.SinResolver

            Dim vistos As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            Using conexion As New SqlConnection(cadenaConexion)
                Await conexion.OpenAsync(ct).ConfigureAwait(False)

                For inicio = 0 To claves.Count - 1 Step PorLote

                    ct.ThrowIfCancellationRequested()

                    Dim lote = claves.Skip(inicio).Take(PorLote).ToList()

                    Using comando As New SqlCommand(ConsultaSuministro(tipo, lote.Count), conexion)
                        ' Corto a propósito: esto corre mientras se escribe, y si tarda más de
                        ' unos segundos no interesa esperarlo.
                        comando.CommandTimeout = 20

                        For i = 0 To lote.Count - 1
                            If tipo = TipoLista.Contratos Then
                                comando.Parameters.Add($"@v{i}", SqlDbType.BigInt).Value = CLng(lote(i))
                            Else
                                comando.Parameters.Add($"@v{i}", SqlDbType.VarChar, 40).Value = lote(i)
                            End If
                        Next

                        Using lector = Await comando.ExecuteReaderAsync(ct).ConfigureAwait(False)
                            While Await lector.ReadAsync(ct).ConfigureAwait(False)
                                If lector.IsDBNull(0) Then Continue While
                                Dim e = Convert.ToString(lector.GetValue(0)).Trim()
                                If e = "E1" OrElse e = "E2" Then vistos.Add(e)
                            End While
                        End Using
                    End Using

                    ' Ya están los dos: no hay nada más que averiguar.
                    If vistos.Count >= 2 Then Exit For

                Next
            End Using

            If vistos.Count >= 2 Then Return TipoSuministro.Mezclado
            If vistos.Contains("E1") Then Return TipoSuministro.Luz
            If vistos.Contains("E2") Then Return TipoSuministro.Gas
            Return TipoSuministro.SinResolver

        End Function

        ''' <summary>
        ''' La consulta del suministro para un lote de <paramref name="cuantos"/> entradas.
        '''
        ''' Está en su propia función y no incrustada en el bucle para poder leerla y probarla
        ''' sin base de datos: los huecos @v0..@vN-1 los tiene que poner exactamente igual que
        ''' los parámetros que se añaden después, y un desajuste ahí no se ve hasta que SQL
        ''' Server se queja en tiempo de ejecución.
        ''' </summary>
        Friend Shared Function ConsultaSuministro(tipo As TipoLista, cuantos As Integer) As String

            Dim marcas = String.Join(",", Enumerable.Range(0, cuantos).Select(Function(i) $"@v{i}"))

            Select Case tipo
                Case TipoLista.Cups
                    ' LEFT(...,20) IN (...) es lo mismo que el LIKE 'prefijo%' de ResolverAsync
                    ' —el prefijo son justo 20 caracteres— y así entran todas de golpe en vez de
                    ' un OR por cada una.
                    Return "SELECT DISTINCT c.Entorno FROM Contrato c " &
                           "INNER JOIN CUPS u ON c.IdCups = u.IdCups " &
                           $"WHERE LEFT(u.CodigoCups, {LongitudCups}) IN ({marcas})"

                Case TipoLista.Cifs
                    Return "SELECT DISTINCT c.Entorno FROM Contrato c " &
                           "INNER JOIN Cliente cl ON c.IdCliente = cl.IdCliente " &
                           $"WHERE cl.Identidad IN ({marcas})"

                Case Else
                    Return $"SELECT DISTINCT Entorno FROM Contrato WHERE CodigoContrato IN ({marcas})"
            End Select

        End Function

        ''' <summary>
        ''' Deja las entradas como las espera la consulta: los códigos de contrato solo si son
        ''' números, los CUPS recortados a 20 y los CIF tal cual. Sin repetidos.
        ''' </summary>
        Friend Shared Function Normalizar(entradas As IReadOnlyList(Of String),
                                           tipo As TipoLista) As List(Of String)

            Dim claves As New List(Of String)
            Dim vistas As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each cruda In entradas
                If String.IsNullOrWhiteSpace(cruda) Then Continue For

                Dim valor = cruda.Trim()

                Select Case tipo
                    Case TipoLista.Contratos
                        Dim codigo As Long
                        If Not Long.TryParse(valor, codigo) Then Continue For
                        valor = codigo.ToString()

                    Case TipoLista.Cups
                        If valor.Length > LongitudCups Then valor = valor.Substring(0, LongitudCups)
                End Select

                If vistas.Add(valor) Then claves.Add(valor)
            Next

            Return claves

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
