Imports System.Data
Imports System.Threading
Imports Microsoft.Data.SqlClient

Namespace Lecturas

    ''' <summary>Una lectura de una prefactura, con el equipo que tenga puesto.</summary>
    Public Class LecturaPrefactura
        Public Property IdLectura As Long
        Public Property CodigoContrato As Long
        Public Property Entorno As String = ""
        Public Property NumeroFactura As String = ""
        Public Property FechaRecepcion As Date?
        Public Property FechaLecturaAnterior As Date?
        Public Property FechaLectura As Date?
        Public Property Facturar As Boolean
        Public Property Facturado As Boolean
        Public Property Vigente As Boolean
        Public Property IdEquipoMedida As Long?
        Public Property NumeroSerie As String = ""
    End Class

    ''' <summary>
    ''' Asignar el equipo de medida a las lecturas de una prefactura.
    '''
    ''' Cuando las lecturas se graban a mano no se les pone el equipo, y luego la prefactura
    ''' sale sin número de contador. Esto busca las lecturas de una factura de compra y deja
    ''' ponérselo.
    '''
    ''' LA COLUMNA SE LLAMA IdEquipoMedida, no IdEquipo. La tabla es EquipoMedida y cuelga del
    ''' contrato por CodigoContrato, que es lo que permite ofrecer los equipos en una lista en
    ''' lugar de pedir un Id a ciegas.
    ''' </summary>
    Public Class ServicioEquipoLectura

        Private ReadOnly _cadenaConexion As String

        Public Sub New(cadenaConexion As String)
            _cadenaConexion = cadenaConexion
        End Sub

        ''' <summary>
        ''' Las lecturas de las facturas de compra de un contrato. El número de factura es
        ''' opcional: sin él salen las de todas sus facturas.
        ''' </summary>
        Public Async Function BuscarAsync(contratos As IReadOnlyList(Of Long),
                                          numeroFactura As String,
                                          soloSinEquipo As Boolean,
                                          Optional ct As CancellationToken = Nothing) _
                                          As Task(Of IReadOnlyList(Of LecturaPrefactura))

            If contratos Is Nothing OrElse contratos.Count = 0 Then
                Return Array.Empty(Of LecturaPrefactura)()
            End If

            ' Un parametro por contrato (@c0, @c1...) y no los numeros pegados al texto del
            ' IN: asi el plan se reutiliza y no hay nada que escapar.
            Dim marcas = String.Join(", ", contratos.Select(Function(c, i) "@c" & i))

            Dim sql =
"SELECT  l.IdLectura, l.CodigoContrato, l.Entorno,
        fcc.NumeroFactura, fcc.FechaRecepcion,
        l.FechaLecturaAnterior, l.FechaLectura,
        l.Facturar, l.Facturado, l.Vigente,
        l.IdEquipoMedida, em.NumeroSerie
FROM    lectura l WITH (NOLOCK)
INNER JOIN FacturaCompraCabecera fcc WITH (NOLOCK)
        ON fcc.IdFacturaCompraCabecera = l.IdFacturaCompraCabecera
LEFT JOIN EquipoMedida em WITH (NOLOCK)
        ON em.IdEquipoMedida = l.IdEquipoMedida
WHERE   fcc.CodigoContrato IN (" & marcas & ")
  AND   (@numero = '' OR fcc.NumeroFactura = @numero)"

            If soloSinEquipo Then sql &= Environment.NewLine & "  AND   l.IdEquipoMedida IS NULL"

            sql &= Environment.NewLine & "ORDER BY l.CodigoContrato, fcc.NumeroFactura, l.FechaLectura, l.IdLectura;"

            Dim lista As New List(Of LecturaPrefactura)()

            Using cn As New SqlConnection(_cadenaConexion)
                Await cn.OpenAsync(ct).ConfigureAwait(False)

                Using cmd As New SqlCommand(sql, cn)
                    cmd.CommandTimeout = 120
                    For i = 0 To contratos.Count - 1
                        cmd.Parameters.Add("@c" & i, SqlDbType.BigInt).Value = contratos(i)
                    Next
                    cmd.Parameters.Add("@numero", SqlDbType.NVarChar, 50).Value =
                        If(numeroFactura, "").Trim()

                    Using lector = Await cmd.ExecuteReaderAsync(ct).ConfigureAwait(False)
                        While Await lector.ReadAsync(ct).ConfigureAwait(False)
                            lista.Add(New LecturaPrefactura With {
                                .IdLectura = Leer(Of Long)(lector, "IdLectura"),
                                .CodigoContrato = Leer(Of Long)(lector, "CodigoContrato"),
                                .Entorno = LeerTexto(lector, "Entorno"),
                                .NumeroFactura = LeerTexto(lector, "NumeroFactura"),
                                .FechaRecepcion = LeerFecha(lector, "FechaRecepcion"),
                                .FechaLecturaAnterior = LeerFecha(lector, "FechaLecturaAnterior"),
                                .FechaLectura = LeerFecha(lector, "FechaLectura"),
                                .Facturar = LeerBool(lector, "Facturar"),
                                .Facturado = LeerBool(lector, "Facturado"),
                                .Vigente = LeerBool(lector, "Vigente"),
                                .IdEquipoMedida = LeerLongNulo(lector, "IdEquipoMedida"),
                                .NumeroSerie = LeerTexto(lector, "NumeroSerie")})
                        End While
                    End Using
                End Using
            End Using

            Return lista

        End Function

        ''' <summary>
        ''' Pone el equipo en una lectura. Devuelve las filas tocadas: 0 significa que ya
        ''' tenía equipo, y entonces no se pisa.
        '''
        ''' Va por IdLectura, que es la clave, y no por contrato más factura como el UPDATE
        ''' que se hacía a mano: así se toca exactamente la fila que se ve en la pantalla y no
        ''' todas las de esa factura a la vez. La condición IdEquipoMedida IS NULL se queda
        ''' igualmente de red, por si alguien lo asignó entre la búsqueda y el guardado.
        ''' </summary>
        Public Async Function AsignarAsync(idLectura As Long,
                                           idEquipoMedida As Long,
                                           Optional ct As CancellationToken = Nothing) As Task(Of Integer)

            Const sql As String =
"UPDATE lectura
SET    IdEquipoMedida = @equipo
WHERE  IdLectura      = @lectura
  AND  IdEquipoMedida IS NULL;"

            Using cn As New SqlConnection(_cadenaConexion)
                Await cn.OpenAsync(ct).ConfigureAwait(False)

                Using cmd As New SqlCommand(sql, cn)
                    cmd.CommandTimeout = 60
                    cmd.Parameters.Add("@lectura", SqlDbType.BigInt).Value = idLectura
                    cmd.Parameters.Add("@equipo", SqlDbType.BigInt).Value = idEquipoMedida

                    Return Await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(False)
                End Using
            End Using

        End Function

        ' ============================================================
        ' LECTURA DEL READER
        '
        ' Con nombre y no con indice: la consulta puede crecer y un indice movido devuelve
        ' otra columna sin avisar.
        ' ============================================================

        Private Shared Function Leer(Of T)(lector As SqlDataReader, columna As String) As T
            Dim i = lector.GetOrdinal(columna)
            If lector.IsDBNull(i) Then Return Nothing
            Return CType(Convert.ChangeType(lector.GetValue(i), GetType(T)), T)
        End Function

        Private Shared Function LeerTexto(lector As SqlDataReader, columna As String) As String
            Dim i = lector.GetOrdinal(columna)
            Return If(lector.IsDBNull(i), "", Convert.ToString(lector.GetValue(i)))
        End Function

        Private Shared Function LeerFecha(lector As SqlDataReader, columna As String) As Date?
            Dim i = lector.GetOrdinal(columna)
            If lector.IsDBNull(i) Then Return Nothing
            Return Convert.ToDateTime(lector.GetValue(i))
        End Function

        Private Shared Function LeerBool(lector As SqlDataReader, columna As String) As Boolean
            Dim i = lector.GetOrdinal(columna)
            Return Not lector.IsDBNull(i) AndAlso Convert.ToBoolean(lector.GetValue(i))
        End Function

        Private Shared Function LeerLongNulo(lector As SqlDataReader, columna As String) As Long?
            Dim i = lector.GetOrdinal(columna)
            If lector.IsDBNull(i) Then Return Nothing
            Return Convert.ToInt64(lector.GetValue(i))
        End Function

    End Class

End Namespace
