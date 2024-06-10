Imports System.Collections.ObjectModel
Imports System.Data.SqlClient
Imports System.IO

Public Class FuncionesGenericas

    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Function ObtenerValor(ByVal strNombreColumna As String,
                                 ByRef oReader As SqlDataReader) As Object
        Dim oValor As Object = Nothing
        Dim intIndiceColumna As Integer
        Dim strTipoColumna As String = Nothing

        Try
            intIndiceColumna = oReader.GetOrdinal(strNombreColumna)
            strTipoColumna = oReader.GetFieldType(intIndiceColumna).Name

            If strTipoColumna = "Int64" Then
                oValor = If(oReader.IsDBNull(intIndiceColumna), DirectCast(Nothing, Nullable(Of Long)), oReader.GetInt64(intIndiceColumna))
            ElseIf strTipoColumna = "Boolean" Then
                oValor = If(oReader.IsDBNull(intIndiceColumna), DirectCast(Nothing, Nullable(Of Boolean)), oReader.GetBoolean(intIndiceColumna))
            ElseIf strTipoColumna = "DateTime" Then
                oValor = If(oReader.IsDBNull(intIndiceColumna), DirectCast(Nothing, Nullable(Of Date)), oReader.GetDateTime(intIndiceColumna))
            ElseIf strTipoColumna = "Decimal" Then
                oValor = If(oReader.IsDBNull(intIndiceColumna), DirectCast(Nothing, Nullable(Of Decimal)), oReader.GetDecimal(intIndiceColumna))
            ElseIf strTipoColumna = "Byte[]" Then
                oValor = If(oReader.IsDBNull(intIndiceColumna), Nothing, CType(oReader(intIndiceColumna), Byte()))
            ElseIf strTipoColumna = "Byte" Then
                oValor = If(oReader.IsDBNull(intIndiceColumna), Nothing, CType(oReader(intIndiceColumna), Byte()))
            ElseIf strTipoColumna = "Int32" Then
                oValor = If(oReader.IsDBNull(intIndiceColumna), DirectCast(Nothing, Nullable(Of Integer)), oReader.GetInt32(intIndiceColumna))
            ElseIf strTipoColumna = "String" Then
                oValor = If(oReader.IsDBNull(intIndiceColumna), Nothing, oReader.GetString(intIndiceColumna))
            Else
                Throw New Exception("TIPO DE DATOS NO IDENTIFICADO - HAY QUE AÑADIRLO")
            End If

            'Console.WriteLine(oReader.GetName(intIndiceColumna) & " -> {0}", oValor)

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try

        Return oValor
    End Function

    Private Sub EscribirDatosQuery(query As String, num As Long)
        If num = 0 Then
            'Console.WriteLine("Ejecuto la query / procedimiento -> " + query)
        Else
            'Console.WriteLine("He ejecutado la query / procedimiento -> " + query)
        End If
    End Sub

    Public Function BuscarbyCups(listaCups As List(Of String), ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of Long)
        Dim ListaContrato As New List(Of Long)

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                For Each codigoCups In listaCups
                    Dim query As String = $"SELECT CodigoContrato
                    FROM Contrato
                    LEFT JOIN CUPS ON Contrato.IdCups = CUPS.IdCups
                    WHERE codigocups LIKE '{codigoCups}%' AND IdContratoSituacion = 1"

                    Dim comando As New SqlCommand(query, conexion)
                    comando.CommandTimeout = 3600
                    Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                    If readerQuery.HasRows Then
                        Do While readerQuery.Read
                            Dim pepe = 0
                            Dim CodigoContrato As Long
                            CodigoContrato = readerQuery.GetValue(0).ToString
                            'CodigoContrato = funciones.ObtenerValor("CodigoContrato", readerQuery)
                            ListaContrato.Add(CodigoContrato)
                        Loop
                    End If

                    readerQuery.Close()
                Next
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ListaContrato
    End Function
    Public Function BuscarbyCodigocontrato(ListaContratos As List(Of Long)) As List(Of Long)
        Dim ListaContrato As New List(Of Long)

        Try
            'Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(Me.connectionString)
                conexion.Open()

                For Each CodigoCon In ListaContratos
                    Dim query As String = $"SELECT CodigoContrato
                    FROM Contrato
                    WHERE codigocontrato = {CodigoCon}"

                    Dim comando As New SqlCommand(query, conexion)
                    comando.CommandTimeout = 3600
                    Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                    If readerQuery.HasRows Then
                        Do While readerQuery.Read
                            Dim pepe = 0
                            Dim CodigoContrato As Long
                            CodigoContrato = readerQuery.GetValue(0).ToString
                            'CodigoContrato = funciones.ObtenerValor("CodigoContrato", readerQuery)
                            ListaContrato.Add(CodigoContrato)
                        Loop
                    End If

                    readerQuery.Close()
                Next
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ListaContrato
    End Function


    Public Function GetTarifaGrupo(TextoTarifaGrupo As String) As List(Of TarifaGrupo)
        Dim conexion = New SqlConnection(connectionString)
        Dim ret As New TarifaGrupo
        Dim TarifaGrupob As New List(Of TarifaGrupo)
        Try
            conexion.Open()
            Dim query = $"select IdTarifaGrupo,Entorno,IdTarifa,TextoTarifaGrupo,IdPerfilFacturacion from TarifaGrupo where textotarifagrupo = '{TextoTarifaGrupo}'"
            Dim comando = New SqlCommand(query, conexion)
            EscribirDatosQuery(query, 0)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            EscribirDatosQuery(query, 1)
            'Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            Do While readerQuery.Read

                Dim TarifaGrupoc = New TarifaGrupo
                'Dim algo = readerQuery.GetName(0).ToString
                TarifaGrupoc.IdTarifaGrupo = readerQuery.GetValue(0).ToString
                TarifaGrupoc.Entorno = readerQuery.GetValue(1).ToString
                TarifaGrupoc.IdTarifa = readerQuery.GetValue(2).ToString
                TarifaGrupoc.TextoTarifaGrupo = readerQuery.GetValue(3).ToString
                TarifaGrupoc.IdPerfilFacturacion = readerQuery.GetValue(4).ToString
                TarifaGrupob.Add(TarifaGrupoc)
            Loop
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return TarifaGrupob
    End Function
    Public Function GetContrato(CodContrato As Long) As Contrato
        Dim Contrato As New Contrato

        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"SELECT IdContrato,CodigoContrato,FechaAplicacionPrecios,FechaContrato,Entorno,idcliente,idcontratosituacion
                    FROM Contrato
                    WHERE codigocontrato = {CodContrato}"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Contrato.IdContrato = readerQuery.GetValue(0).ToString
                        Contrato.CodigoContrato = readerQuery.GetValue(1).ToString
                        If Not readerQuery.IsDBNull(2) Then
                            Contrato.FechaAplicacionPrecios = readerQuery.GetValue(2).ToString()
                        End If
                        Contrato.FechaContrato = readerQuery.GetValue(3).ToString
                        Contrato.Entorno = readerQuery.GetValue(4).ToString
                        Contrato.IdCliente = readerQuery.GetValue(5).ToString
                        Contrato.IdContratoSituacion = readerQuery.GetValue(6).ToString
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return Contrato
    End Function

    Public Function GetListContratobyCIF(Cif As String) As List(Of Contrato)
        Dim listaContratos As New List(Of Contrato)
        Try
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select idcontrato,CodigoContrato, FechaContrato,cl.IdCliente, c.Entorno, IdContratoSituacion from Contrato c
                                        inner join Cliente cl on c.IdCliente = cl.IdCliente
                                        where cl.Identidad = '{Cif}'"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim Contrato As New Contrato
                        Contrato.IdContrato = readerQuery.GetValue(0).ToString
                        Contrato.CodigoContrato = readerQuery.GetValue(1).ToString
                        Contrato.FechaContrato = readerQuery.GetValue(2).ToString
                        Contrato.IdCliente = readerQuery.GetValue(3).ToString
                        Contrato.Entorno = readerQuery.GetValue(4).ToString
                        Contrato.IdContratoSituacion = readerQuery.GetValue(5).ToString
                        listaContratos.Add(Contrato)
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Throw
        End Try

        Return listaContratos
    End Function


    Public Function GetListContratobyCUPS(CUPS As String) As List(Of Contrato)
        Dim listaContratos As New List(Of Contrato)
        Try
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $";with iddc as (select idcups from CUPS where codigocups like '{CUPS}%')
select idcontrato,codigocontrato,fechacontrato,idcliente,entorno,idcontratosituacion,isrenovacionprocesada 
from contrato where idcups in (select idcups from iddc)"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim Contrato As New Contrato
                        Contrato.IdContrato = readerQuery.GetValue(0).ToString
                        Contrato.CodigoContrato = readerQuery.GetValue(1).ToString
                        Contrato.FechaContrato = readerQuery.GetValue(2).ToString
                        Contrato.IdCliente = readerQuery.GetValue(3).ToString
                        Contrato.Entorno = readerQuery.GetValue(4).ToString
                        Contrato.IdContratoSituacion = readerQuery.GetValue(5).ToString
                        Dim isRenovacionProcesadaString As String = readerQuery.GetValue(6).ToString()
                        Dim isRenovacionProcesadaBoolean As Boolean
                        If Boolean.TryParse(isRenovacionProcesadaString, isRenovacionProcesadaBoolean) Then
                            Contrato.IsRenovacionProcesada = isRenovacionProcesadaBoolean
                        Else
                            Contrato.IsRenovacionProcesada = False
                        End If

                        listaContratos.Add(Contrato)
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Throw
        End Try

        Return listaContratos
    End Function


    Public Function GetPerfilFacturacion(IdPerfilFacturacion As Long) As PerfilFacturacion
        Dim PerfilFacturacion As New PerfilFacturacion

        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"SELECT *
                    FROM PerfilFacturacion
                    WHERE IdPerfilFacturacion = {IdPerfilFacturacion}"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        PerfilFacturacion.IdPerfilFacturacion = readerQuery.GetValue(0).ToString
                        PerfilFacturacion.Entorno = readerQuery.GetValue(1).ToString
                        PerfilFacturacion.TextoPerfilFacturacion = readerQuery.GetValue(2).ToString
                        PerfilFacturacion.CodSectorEmpresarial = readerQuery.GetValue(3).ToString
                        PerfilFacturacion.LineaNegocio = readerQuery.GetValue(4).ToString
                    Loop
                End If
                readerQuery.Close()
                If PerfilFacturacion.IdPerfilFacturacion Then
                    PerfilFacturacion.PerfilFacturacionConfiguraciones = GetPerfilFacturacionConfiguracion(IdPerfilFacturacion)
                End If

            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return PerfilFacturacion
    End Function


    Public Function GetPerfilFacturacionConfiguracion(IdPerfilFacturacion As Long) As ObservableCollection(Of PerfilFacturacionConfiguracion)
        Dim PerfilFacturacionConfiguracion As New ObservableCollection(Of PerfilFacturacionConfiguracion)

        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim queryPerfil As String = $"SELECT *
                                                 FROM PerfilFacturacionConfiguracion
                                                 WHERE IdPerfilFacturacion = {IdPerfilFacturacion}"

                Dim comandoPerfil As New SqlCommand(queryPerfil, conexion)
                comandoPerfil.CommandTimeout = 3600
                Dim readerQueryPerfil As SqlDataReader = comandoPerfil.ExecuteReader()

                If readerQueryPerfil.HasRows Then
                    Do While readerQueryPerfil.Read
                        Dim Perfil As New PerfilFacturacionConfiguracion
                        Perfil.IdPerfilFacturacionConfiguracion = readerQueryPerfil.GetValue(0).ToString
                        Perfil.Entorno = readerQueryPerfil.GetValue(1).ToString
                        Perfil.IdPerfilFacturacion = readerQueryPerfil.GetValue(2).ToString
                        Perfil.CodigoConcepto = readerQueryPerfil.GetValue(3).ToString
                        Perfil.Clave = readerQueryPerfil.GetValue(4).ToString
                        PerfilFacturacionConfiguracion.Add(Perfil)
                    Loop
                End If
                readerQueryPerfil.Close()

            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return PerfilFacturacionConfiguracion
    End Function

    Public Function GetDTOAllPeriodosIndx(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As List(Of IndexadoPrecio)
        Dim IndexadoPrecio As New List(Of IndexadoPrecio)

        Try
            Dim top = If(IdTarifa = 202020, 3, 6)
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"SELECT top {top} IdIndexadoPrecio,IndexadoPrecio.Entorno,IndexadoPrecio.IdTarifa,IdTarifaGrupo,IdIndexadoConcepto,tp.IdTarifaPeriodo,tp.TextoTarifaPeriodo
                    FROM IndexadoPrecio
                    Inner join TarifaPeriodo tp on IndexadoPrecio.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE IndexadoPrecio.IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo} order by IdIndexadoPrecio desc"
                'and FechaFinPresupuesto ='{FechaPresupuesto}' 
                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim Index As New IndexadoPrecio
                        Index.IdIndexadoPrecio = readerQuery.GetValue(0).ToString
                        Index.Entorno = readerQuery.GetValue(1).ToString
                        Index.IdTarifa = readerQuery.GetValue(2).ToString
                        Index.IdTarifaGrupo = readerQuery.GetValue(3).ToString
                        Index.IdIndexadoConcepto = readerQuery.GetValue(4).ToString
                        Index.IdTarifaPeriodo = readerQuery.GetValue(5).ToString
                        Index.TextoTarifaPeriodo = readerQuery.GetValue(6).ToString
                        IndexadoPrecio.Add(Index)
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return IndexadoPrecio
    End Function

    Public Function GetDTOAllPeriodosIndxGas(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As List(Of IndexadoPrecioGas)
        Dim IndexadoPrecioGas As New List(Of IndexadoPrecioGas)

        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select IdIndexadoPrecioGas,IndexadoPrecioGas.Entorno,IndexadoPrecioGas.IdTarifa,IdTarifaGrupo,IndexadoPrecioGas.IdTarifaPeriodo,tp.TextoTarifaPeriodo
                    FROM IndexadoPrecioGas
                    Inner join TarifaPeriodo tp on IndexadoPrecioGas.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE IndexadoPrecioGas.IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo} order by IdIndexadoPrecioGas desc"
                ' and FechaFinPresupuesto ='{FechaPresupuesto}' 
                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim Index As New IndexadoPrecioGas
                        Index.IdIndexadoPrecioGas = readerQuery.GetValue(0).ToString
                        Index.Entorno = readerQuery.GetValue(1).ToString
                        Index.IdTarifa = readerQuery.GetValue(2).ToString
                        Index.IdTarifaGrupo = readerQuery.GetValue(3).ToString
                        Index.IdTarifaPeriodo = readerQuery.GetValue(4).ToString
                        Index.TextoTarifaPeriodo = readerQuery.GetValue(5).ToString
                        IndexadoPrecioGas.Add(Index)
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return IndexadoPrecioGas
    End Function

    Public Function GetDTOAllPeriodosTarifaPrecio(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As List(Of TarifaPrecio)
        Dim TarifaPrecio As New List(Of TarifaPrecio)

        Try
            Dim top = If(IdTarifa = 202020, 3, 6)
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select top {top} IdTarifaPrecio,TarifaPrecio.Entorno,TarifaPrecio.IdTarifa,IdTarifaGrupo,TarifaPrecio.IdTarifaPeriodo,tp.TextoTarifaPeriodo
                    FROM TarifaPrecio
                    Inner join TarifaPeriodo tp on TarifaPrecio.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE TarifaPrecio.IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo}  order by IdTarifaPrecio desc"
                'and FechaFinPresupuesto ='{FechaPresupuesto}'
                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim Tari As New TarifaPrecio
                        Tari.IdTarifaPrecio = readerQuery.GetValue(0).ToString
                        Tari.Entorno = readerQuery.GetValue(1).ToString
                        Tari.IdTarifa = readerQuery.GetValue(2).ToString
                        Tari.IdTarifaGrupo = readerQuery.GetValue(3).ToString
                        Tari.IdTarifaPeriodo = readerQuery.GetValue(4).ToString
                        Tari.TextoTarifaPeriodo = readerQuery.GetValue(5).ToString
                        TarifaPrecio.Add(Tari)
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return TarifaPrecio
    End Function


    Public Function UpdatePrecioContratoTarifa(Cont As List(Of TarifaPrecioContrato), ContOld As List(Of TarifaPrecioContrato), IsFijoIndex As Boolean) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As New Long
        Try
            Dim numTarifaPrecioContrato As Integer = Cont.Count
            Dim numTarifaPrecioContratoOLD As Integer = ContOld.Count
            Dim minNum As Integer = Math.Min(numTarifaPrecioContrato, numTarifaPrecioContratoOLD)
            conexion.Open()

            For i As Integer = 0 To minNum - 1
                Dim ContNuevo As TarifaPrecioContrato = Cont(i)
                Dim ContViejo As TarifaPrecioContrato = ContOld(i)

                If ContNuevo.IdTarifaPeriodo = ContViejo.IdTarifaPeriodo Then
                    'Si es indexado entra aqui
                    If IsFijoIndex Then
                        If ContNuevo.Entorno = "G1" Then
                            Dim query = $"UPDATE tarifapreciocontrato
                                set IdIndexadoPrecio = {ContNuevo.IdIndexadoPrecio}, idtarifaprecio= 0 where IdTarifaPrecioContrato in ({ContViejo.IdTarifaPrecioContrato})"
                            Dim comando = New SqlCommand(query, conexion)
                            FilfasAfectadas = comando.ExecuteNonQuery
                        ElseIf ContNuevo.Entorno = "G2" Then
                            Dim query = $"UPDATE tarifapreciocontrato
                                    set IdIndexadoPrecioGas = {ContNuevo.IdIndexadoPrecioGas}, idtarifaprecio= 0 where IdTarifaPrecioContrato in ({ContViejo.IdTarifaPrecioContrato})"
                            Dim comando = New SqlCommand(query, conexion)
                            FilfasAfectadas = comando.ExecuteNonQuery
                        End If
                    Else
                        Dim query = $"UPDATE tarifapreciocontrato
                                set IdTarifaPrecio = {ContNuevo.IdTarifaPrecio}, IdIndexadoPrecio=0,IdIndexadoPrecioGas=0   where IdTarifaPrecioContrato in ({ContViejo.IdTarifaPrecioContrato})"
                        Dim comando = New SqlCommand(query, conexion)
                        FilfasAfectadas = comando.ExecuteNonQuery
                    End If

                End If
            Next
            'conexion.Open()
            'Dim query = $"UPDATE tarifapreciocontrato set IdTarifaGrupo = {TarifaGrupoNueva.IdTarifaGrupo}
            '            , IdPerfilFacturacion =  {TarifaGrupoNueva.IdPerfilFacturacion}                          
            '            where IdContratoTarifa in ({Cont.IdContratoTarifa})"
            'Dim comando = New SqlCommand(query, conexion)
            'Dim readerQuery As SqlDataReader = comando.ExecuteReader

            conexion.Close()
            'TarifaPrecioContrato = Cont
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function


    Public Function GetDTOAllPeriodosOld(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?, ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of IndexadoPrecio)
        Dim IndexadoPrecio As New List(Of IndexadoPrecio)

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"SELECT IdIndexadoPrecio,Entorno,IdTarifa,IdTarifaGrupo,IdIndexadoConcepto,IdTarifaPeriodo,tp.TextoTarifaPeriodo
                    FROM IndexadoPrecio
                    Inner join TarifaPeriodo tp on IndexadoPrecio.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo} and FechaFinPresupuesto ='{FechaPresupuesto}' "

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim Index As New IndexadoPrecio
                        Index.IdIndexadoPrecio = readerQuery.GetValue(0).ToString
                        Index.Entorno = readerQuery.GetValue(1).ToString
                        Index.IdTarifa = readerQuery.GetValue(2).ToString
                        Index.IdTarifaGrupo = readerQuery.GetValue(3).ToString
                        Index.IdIndexadoConcepto = readerQuery.GetValue(4).ToString
                        Index.IdTarifaPeriodo = readerQuery.GetValue(5).ToString
                        Index.TextoTarifaPeriodo = readerQuery.GetValue(6).ToString
                        IndexadoPrecio.Add(Index)
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return IndexadoPrecio
    End Function



    Public Function GetPrecioContratoTarifaIndex(Cont As ContratoTarifa) As List(Of TarifaPrecioContrato)
        Dim conexion = New SqlConnection(connectionString)

        Dim TarifaPrecioContrato As New List(Of TarifaPrecioContrato)
        Try
            conexion.Open()
            Dim query = $"select tarifapreciocontrato.*, t.idtarifa, tg.IdTarifaGrupo,tg.TextoTarifaGrupo, tp.IdTarifaPeriodo,tp.TextoTarifaPeriodo from tarifapreciocontrato 
                        left join Contratotarifa ct  on tarifapreciocontrato.idcontratotarifa = ct.idcontratotarifa
                        left join tarifa t  on ct.idtarifa = t.idtarifa
                        left join TarifaGrupo tg on ct.IdTarifaGrupo = tg.IdTarifaGrupo
                        left join IndexadoPrecio Inp on TarifaPrecioContrato.IdIndexadoPrecio = Inp.IdIndexadoPrecio
                        left join TarifaPeriodo tp on inp.IdTarifaPeriodo = tp.IdTarifaPeriodo
                        where ct.idcontratotarifa in (
                        {Cont.IdContratoTarifa}
                        ) order by tp.IdTarifaPeriodo desc"
            Dim comando As New SqlCommand(query, conexion)
            comando.CommandTimeout = 3600
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            If readerQuery.HasRows Then
                Do While readerQuery.Read
                    Dim TarifaPrecioContratoL As New TarifaPrecioContrato
                    TarifaPrecioContratoL.IdTarifaPrecioContrato = readerQuery.GetValue(0).ToString
                    TarifaPrecioContratoL.Entorno = readerQuery.GetValue(1).ToString
                    TarifaPrecioContratoL.IdContratoTarifa = readerQuery.GetValue(2).ToString
                    TarifaPrecioContratoL.IdTarifaPrecio = readerQuery.GetValue(3).ToString
                    If TarifaPrecioContratoL.Entorno = "G1" Then
                        TarifaPrecioContratoL.IdIndexadoPrecio = readerQuery.GetValue(4).ToString
                    Else
                        TarifaPrecioContratoL.IdIndexadoPrecioGas = readerQuery.GetValue(5).ToString
                    End If
                    TarifaPrecioContratoL.IdTarifa = readerQuery.GetValue(6).ToString
                    TarifaPrecioContratoL.IdTarifaGrupo = readerQuery.GetValue(7).ToString
                    TarifaPrecioContratoL.TextoTarifaGrupo = readerQuery.GetValue(8).ToString
                    TarifaPrecioContratoL.IdTarifaPeriodo = readerQuery.GetValue(9).ToString
                    TarifaPrecioContratoL.TextoTarifaPeriodo = readerQuery.GetValue(10).ToString
                    TarifaPrecioContrato.Add(TarifaPrecioContratoL)
                Loop
            End If
            readerQuery.Close()
            conexion.Close()

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return TarifaPrecioContrato
    End Function
    Public Function GetPrecioContratoTarifa(Cont As ContratoTarifa) As List(Of TarifaPrecioContrato)
        Dim conexion = New SqlConnection(connectionString)

        Dim TarifaPrecioContrato As New List(Of TarifaPrecioContrato)
        Try
            conexion.Open()
            Dim query = $"select tarifapreciocontrato.*, t.idtarifa, tg.IdTarifaGrupo,tg.TextoTarifaGrupo, tp.IdTarifaPeriodo,tp.TextoTarifaPeriodo from tarifapreciocontrato 
                        left join Contratotarifa ct  on tarifapreciocontrato.idcontratotarifa = ct.idcontratotarifa
                        left join tarifa t  on ct.idtarifa = t.idtarifa
                        left join TarifaGrupo tg on ct.IdTarifaGrupo = tg.IdTarifaGrupo
                        left join tarifaprecio Inp on TarifaPrecioContrato.idtarifaprecio = Inp.idtarifaprecio
                        left join TarifaPeriodo tp on inp.IdTarifaPeriodo = tp.IdTarifaPeriodo
                        where ct.idcontratotarifa in (
                        {Cont.IdContratoTarifa}
                        ) order by tp.IdTarifaPeriodo desc"
            Dim comando As New SqlCommand(query, conexion)
            comando.CommandTimeout = 3600
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            If readerQuery.HasRows Then
                Do While readerQuery.Read
                    Dim TarifaPrecioContratoL As New TarifaPrecioContrato
                    TarifaPrecioContratoL.IdTarifaPrecioContrato = readerQuery.GetValue(0).ToString
                    TarifaPrecioContratoL.Entorno = readerQuery.GetValue(1).ToString
                    TarifaPrecioContratoL.IdContratoTarifa = readerQuery.GetValue(2).ToString
                    TarifaPrecioContratoL.IdTarifaPrecio = readerQuery.GetValue(3).ToString
                    If TarifaPrecioContratoL.Entorno = "G1" Then
                        TarifaPrecioContratoL.IdIndexadoPrecio = readerQuery.GetValue(4).ToString
                    Else
                        TarifaPrecioContratoL.IdIndexadoPrecioGas = readerQuery.GetValue(5).ToString
                    End If
                    TarifaPrecioContratoL.IdTarifa = readerQuery.GetValue(6).ToString
                    TarifaPrecioContratoL.IdTarifaGrupo = readerQuery.GetValue(7).ToString
                    TarifaPrecioContratoL.TextoTarifaGrupo = readerQuery.GetValue(8).ToString
                    TarifaPrecioContratoL.IdTarifaPeriodo = readerQuery.GetValue(9).ToString
                    TarifaPrecioContratoL.TextoTarifaPeriodo = readerQuery.GetValue(10).ToString
                    TarifaPrecioContrato.Add(TarifaPrecioContratoL)
                Loop
            End If
            readerQuery.Close()
            conexion.Close()

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return TarifaPrecioContrato
    End Function

    Public Function GetPrecioContratoTarifaV2(Cont As ContratoTarifa) As TarifaPrecioContrato
        Dim conexion = New SqlConnection(connectionString)

        Dim TarifaPrecioContrato As New TarifaPrecioContrato
        Try
            conexion.Open()
            Dim query = $" select top 1 tarifapreciocontrato.* from tarifapreciocontrato 
                        left join Contratotarifa ct  on tarifapreciocontrato.idcontratotarifa = ct.idcontratotarifa
                        where ct.idcontratotarifa in (
                        {Cont.IdContratoTarifa}
                        )"
            Dim comando As New SqlCommand(query, conexion)
            comando.CommandTimeout = 3600
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            If readerQuery.HasRows Then
                Do While readerQuery.Read
                    TarifaPrecioContrato.IdTarifaPrecioContrato = readerQuery.GetValue(0).ToString
                    TarifaPrecioContrato.Entorno = readerQuery.GetValue(1).ToString
                    TarifaPrecioContrato.IdContratoTarifa = readerQuery.GetValue(2).ToString
                    TarifaPrecioContrato.IdTarifaPrecio = readerQuery.GetValue(3).ToString
                    If TarifaPrecioContrato.Entorno = "G1" Then
                        TarifaPrecioContrato.IdIndexadoPrecio = readerQuery.GetValue(4).ToString
                    Else
                        TarifaPrecioContrato.IdIndexadoPrecioGas = readerQuery.GetValue(5).ToString
                    End If

                Loop
            End If
            readerQuery.Close()
            conexion.Close()

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return TarifaPrecioContrato
    End Function

    Public Function GetPrecioContratoTarifaIndexGas(Cont As ContratoTarifa) As List(Of TarifaPrecioContrato)
        Dim conexion = New SqlConnection(connectionString)

        Dim TarifaPrecioContrato As New List(Of TarifaPrecioContrato)
        Try
            conexion.Open()
            Dim query = $"select tarifapreciocontrato.*, t.idtarifa, tg.IdTarifaGrupo,tg.TextoTarifaGrupo, tp.IdTarifaPeriodo,tp.TextoTarifaPeriodo from tarifapreciocontrato 
                        left join Contratotarifa ct  on tarifapreciocontrato.idcontratotarifa = ct.idcontratotarifa
                        left join tarifa t  on ct.idtarifa = t.idtarifa
                        left join TarifaGrupo tg on ct.IdTarifaGrupo = tg.IdTarifaGrupo
                        left join IndexadoPrecioGas Inp on TarifaPrecioContrato.IdIndexadoPrecioGas = Inp.IdIndexadoPrecioGas
                        left join TarifaPeriodo tp on inp.IdTarifaPeriodo = tp.IdTarifaPeriodo
                        where ct.idcontratotarifa in (
                        {Cont.IdContratoTarifa}
                        ) order by tp.IdTarifaPeriodo desc"
            Dim comando As New SqlCommand(query, conexion)
            comando.CommandTimeout = 3600
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            If readerQuery.HasRows Then
                Do While readerQuery.Read
                    Dim TarifaPrecioContratoL As New TarifaPrecioContrato
                    TarifaPrecioContratoL.IdTarifaPrecioContrato = readerQuery.GetValue(0).ToString
                    TarifaPrecioContratoL.Entorno = readerQuery.GetValue(1).ToString
                    TarifaPrecioContratoL.IdContratoTarifa = readerQuery.GetValue(2).ToString
                    TarifaPrecioContratoL.IdTarifaPrecio = readerQuery.GetValue(3).ToString
                    If TarifaPrecioContratoL.Entorno = "G1" Then
                        TarifaPrecioContratoL.IdIndexadoPrecio = readerQuery.GetValue(4).ToString
                    Else
                        TarifaPrecioContratoL.IdIndexadoPrecioGas = readerQuery.GetValue(5).ToString
                    End If
                    TarifaPrecioContratoL.IdTarifa = readerQuery.GetValue(6).ToString
                    TarifaPrecioContratoL.IdTarifaGrupo = readerQuery.GetValue(7).ToString
                    TarifaPrecioContratoL.TextoTarifaGrupo = readerQuery.GetValue(8).ToString
                    TarifaPrecioContratoL.IdTarifaPeriodo = readerQuery.GetValue(9).ToString
                    TarifaPrecioContratoL.TextoTarifaPeriodo = readerQuery.GetValue(10).ToString
                    TarifaPrecioContrato.Add(TarifaPrecioContratoL)
                Loop
            End If
            readerQuery.Close()
            conexion.Close()

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return TarifaPrecioContrato
    End Function

    Public Function GetProductosbyEntorno(Entorno As String) As List(Of Producto)
        Dim Productos As New List(Of Producto)

        Try
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select IdProducto,Entorno,IdProductoGrupo,TextoProducto,isnull(Importe,0),AntesIE,isnull(IdTipoImpuesto,0) 
                ,isnull(SobreConsumo,0),isnull(PrecioSobreConsumo,0)
                from Producto where Entorno = '{Entorno}' "

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim Pro As New Producto
                        Pro.IdProducto = readerQuery.GetValue(0).ToString
                        Pro.Entorno = readerQuery.GetValue(1).ToString
                        Pro.IdProductoGrupo = readerQuery.GetValue(2).ToString
                        Pro.TextoProducto = readerQuery.GetValue(3).ToString
                        Pro.Importe = readerQuery.GetValue(4).ToString
                        Pro.AntesIE = readerQuery.GetValue(5).ToString
                        Pro.IdTipoImpuesto = readerQuery.GetValue(6).ToString
                        Pro.SobreConsumo = readerQuery.GetValue(7).ToString
                        Pro.PrecioSobreConsumo = readerQuery.GetValue(8).ToString
                        Productos.Add(Pro)
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return Productos
    End Function

    Public Function GetProductoGrupobyById(IdproductoGrupo As Long) As ProductoGrupo
        Dim ProductoGr As New ProductoGrupo

        Try
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select * from Productogrupo where IdProductoGrupo = {IdproductoGrupo} "

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read

                        ProductoGr.IdProductoGrupo = readerQuery.GetValue(0).ToString
                        ProductoGr.Entorno = readerQuery.GetValue(1).ToString
                        ProductoGr.TextoProductoGrupo = readerQuery.GetValue(2).ToString
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ProductoGr
    End Function

    Public Function GetTipoImpuestoBy() As List(Of TipoImpuesto)
        Dim ListaImpuestoTipo As New List(Of TipoImpuesto)

        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select * from TipoImpuesto"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim ImpuestoTipo As New TipoImpuesto
                        ImpuestoTipo.IdTipoImpuesto = readerQuery.GetValue(0).ToString
                        ImpuestoTipo.Entorno = readerQuery.GetValue(1).ToString
                        ImpuestoTipo.TextoImpuesto = readerQuery.GetValue(2).ToString
                        ImpuestoTipo.Porcentaje = readerQuery.GetValue(3).ToString
                        ListaImpuestoTipo.Add(ImpuestoTipo)
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ListaImpuestoTipo
    End Function



    Public Sub EscribirEnArchivo(escritor As StreamWriter, contrato As Contrato)
        Try
            ' Genera el contenido que deseas escribir en el archivo
            Dim contenido As String = "Información del contrato: " & contrato.ToString()

            ' Escribir el contenido en el archivo
            escritor.WriteLine(contenido)
        Catch ex As Exception
            Throw
        End Try

    End Sub


    Public Function UpdateCodigosDir(CodigoUnidadTramitadora As String, CodigoOficinaContable As String, CodigoOrganoGestor As String, Contratos As List(Of Long)) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            Dim Codigos = String.Join(",", Contratos)
            Dim query = $"update Contrato set CodigoUnidadTramitadora='{CodigoUnidadTramitadora}',
                        CodigoOficinaContable='{CodigoOficinaContable}',
                        CodigoOrganoGestor='{CodigoOrganoGestor}'
                        where CodigoContrato in ({Codigos})"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function

    Public Function InsertProductoAsignacion(Entorno As String, IdProductoGrupo As Long, IdProducto As Long, IdContrato As Long, FechaInicial As Date, Importe As Decimal, IdTipoImpuesto As Long, AntesIE As Boolean, AplicarSobreConsumo As Boolean, AplicarPrecioConsumo As Boolean) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            'Dim Codigos = String.Join(",", Contratos)
            'Dim query = $"INSERT INTO ProductoAsignacion(Entorno,IdProductoGrupo,IdProducto,TipoAsignacion,IdContrato,FechaInicial,Importe,IdTipoImpuesto,AntesIE,AplicarSobreConsumo,AplicarPrecioConsumo) 
            '                values('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO',{IdContrato},'{FechaInicial}',{Importe.ToString.Replace(",", ".")},{IdTipoImpuesto},{If(AntesIE, 1, 0)},{If(AplicarSobreConsumo, 1, 0)},{If(AplicarPrecioConsumo, 1, 0)})"

            Dim query =
$"INSERT INTO ProductoAsignacion (Entorno, IdProductoGrupo, IdProducto, TipoAsignacion, IdCliente, IdContrato, IdTarifa, IdTarifaGrupo, IdTipoCobro, IdTarifaPeaje, Desde, Hasta, IsControlFecha, FechaInicial, FechaFinal, Plazo, PlazoCargado, ImporteTotalPlazo, IsFacturado, Importe, Descuento, AntesIE, IdTipoImpuesto, PrecioDia, IsFacturaProrrateo, IdFacturaProrrateo, PorcentajeIncremento, AplicarSobreConsumo, ImportePlazo, AplicarPrecioConsumo, IsBonificacion, FechaAsignacion)
VALUES ('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO', NULL, {IdContrato}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '{FechaInicial}', NULL, NULL, NULL, NULL, NULL, {Importe.ToString.Replace(",", ".")}, 0.00, {If(AntesIE, 1, 0)}, {IdTipoImpuesto}, 0, 0, NULL, 0.00, {If(AplicarSobreConsumo, 1, 0)}, NULL, {If(AplicarPrecioConsumo, 1, 0)}, NULL, NULL);"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function



    Public Function UpdateProductoAsignacion(Entorno As String, IdProductoGrupo As Long, IdProducto As Long, IdContrato As Long, FechaInicial As Date, Importe As Decimal, IdTipoImpuesto As Long, AntesIE As Boolean, AplicarSobreConsumo As Boolean, AplicarPrecioConsumo As Boolean) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            'Dim Codigos = String.Join(",", Contratos)
            'Dim query = $"INSERT INTO ProductoAsignacion(Entorno,IdProductoGrupo,IdProducto,TipoAsignacion,IdContrato,FechaInicial,Importe,IdTipoImpuesto,AntesIE,AplicarSobreConsumo,AplicarPrecioConsumo) 
            '                values('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO',{IdContrato},'{FechaInicial}',{Importe.ToString.Replace(",", ".")},{IdTipoImpuesto},{If(AntesIE, 1, 0)},{If(AplicarSobreConsumo, 1, 0)},{If(AplicarPrecioConsumo, 1, 0)})"

            Dim query =
$"INSERT INTO ProductoAsignacion (Entorno, IdProductoGrupo, IdProducto, TipoAsignacion, IdCliente, IdContrato, IdTarifa, IdTarifaGrupo, IdTipoCobro, IdTarifaPeaje, Desde, Hasta, IsControlFecha, FechaInicial, FechaFinal, Plazo, PlazoCargado, ImporteTotalPlazo, IsFacturado, Importe, Descuento, AntesIE, IdTipoImpuesto, PrecioDia, IsFacturaProrrateo, IdFacturaProrrateo, PorcentajeIncremento, AplicarSobreConsumo, ImportePlazo, AplicarPrecioConsumo, IsBonificacion, FechaAsignacion)
VALUES ('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO', NULL, {IdContrato}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '{FechaInicial}', NULL, NULL, NULL, NULL, NULL, {Importe.ToString.Replace(",", ".")}, 0.00, {If(AntesIE, 1, 0)}, {IdTipoImpuesto}, 0, 0, NULL, 0.00, {If(AplicarSobreConsumo, 1, 0)}, NULL, {If(AplicarPrecioConsumo, 1, 0)}, NULL, NULL);"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function



    Public Function getCNAEbyCodigo(CodigoCNAE As String) As CNAE

        Dim ObjCNAE As New CNAE
        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select * from CNAE where CodigoCNAE = '{CodigoCNAE}'"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        ObjCNAE.IdCNAE = readerQuery.GetValue(0).ToString
                        ObjCNAE.Entorno = readerQuery.GetValue(1).ToString
                        ObjCNAE.CodigoCNAE = readerQuery.GetValue(2).ToString
                        ObjCNAE.TextoCNAE = readerQuery.GetValue(3).ToString
                        ObjCNAE.CodigoAgrupacion = readerQuery.GetValue(4).ToString
                        ObjCNAE.DescripcionCodigoAgrupacion = readerQuery.GetValue(5).ToString
                    Loop
                End If
                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ObjCNAE
    End Function

    Public Function UpdateContratoCNAE(Codigocontrato As Long, IdCNAE As Long) As Long
        Dim FilfasAfectadas As Long
        Try
            Dim conexion = New SqlConnection(connectionString)
            conexion.Open()
            Dim query = $"update Contrato set IdCNAE={IdCNAE} where CodigoContrato = {Codigocontrato}"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Throw
        End Try
        Return FilfasAfectadas
    End Function

    Public Function VolverARenovar(Contrato As Long) As Long
        Dim FilfasAfectadas As Long
        Try
            Dim conexion = New SqlConnection(connectionString)
            conexion.Open()
            Dim query = $"update contrato set isrenovacionprocesada = null where CodigoContrato in ({Contrato})"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Throw
        End Try
        Return FilfasAfectadas
    End Function

    Public Sub EscribirContratoTarifaAntesCambios(ListaCodContrato As List(Of Long))
        Try
            Dim listasQuerys As New List(Of String)
            Dim ContratoTarifaSrv As New ContratoTarifaSrv(connectionString)
            Dim EscribeExcel As New ValidacionExcel(connectionString)
            Dim ListidContratoTarifa As New List(Of Long)
            Dim CodJoin = String.Join(",", ListaCodContrato)
            Dim query = $"select ct.idcontratotarifa, ct.codigocontrato,tg.IdTarifaGrupo,tg.textotarifagrupo, pf.TextoPerfilFacturacion, t.IdTarifa,t.TextoTarifa,ct.FechaDesde,ct.FechaHasta from contratotarifa ct
left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
left join tarifa t  on ct.idtarifa = t.idtarifa
where codigocontrato in ({CodJoin})and ct.FechaHasta is null
order by ct.IdTarifa"

            For Each elment In ListaCodContrato
                Dim ContratTa = ContratoTarifaSrv.GetContratoTarifaByCodigoContrato(elment)
                If Not IsNothing(ContratTa) AndAlso ContratTa.IdContratoTarifa > 0 AndAlso IsNothing(ContratTa.FechaHasta) Then
                    ListidContratoTarifa.Add(ContratTa.IdContratoTarifa)
                End If
            Next
            Dim CodTJoin = String.Join(",", ListidContratoTarifa)
            Dim query2 = $"select tarifapreciocontrato.*, t.idtarifa, tg.IdTarifaGrupo,tg.TextoTarifaGrupo from tarifapreciocontrato 
left join Contratotarifa ct  on tarifapreciocontrato.idcontratotarifa = ct.idcontratotarifa
left join tarifa t  on ct.idtarifa = t.idtarifa
left join TarifaGrupo tg on ct.IdTarifaGrupo = tg.IdTarifaGrupo
where ct.idcontratotarifa in ({CodTJoin}) order by t.IdTarifa"

            listasQuerys.Add(query)
            listasQuerys.Add(query2)

            Dim rutaCarpeta As String = $"C:\Users\{Environment.UserName}\Desktop\Precios"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, $"ContratTarifaAntesActualizacion_{Date.Now.ToString("ddMMyyyyHHmmss")}.xlsx")

            ' Verificar si la carpeta existe, y si no, crearla
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If

            ' Verificar si el archivo existe, y si no, crearlo
            If Not File.Exists(rutaArchivo) Then
                File.Create(rutaArchivo).Close()
            End If

            EscribeExcel.EjecutarConsultasYGuardarEnExcel(listasQuerys, rutaArchivo)
        Catch ex As Exception
            Throw
        End Try
    End Sub


    Public Function RevisaTarifaPrecioContratoPersonalizada() As String
        Try
            Dim listasQuerys As New List(Of String)
            Dim EscribeExcel As New ValidacionExcel(connectionString)
            Dim query = $";with idcl as (select ct.IdContratoTarifa,ct.IdTarifaGrupo from tarifapreciocontrato tf
inner join ContratoTarifa ct on ct.idcontratotarifa = tf.idcontratotarifa
where tf.IdIndexadoPrecio in (select IdIndexadoPrecio from IndexadoPrecio where idtarifagrupo in (
select IdTarifaGrupo from TarifaGrupo where textotarifagrupo like '%persona%' and IdTarifa>=202020)
) and ct.idtarifagrupo not in (select IdTarifaGrupo from TarifaGrupo where textotarifagrupo like '%persona%' and IdTarifa>=202020))

select ct.idcontratotarifa, ct.codigocontrato,tg.IdTarifaGrupo,tg.textotarifagrupo, pf.TextoPerfilFacturacion, t.IdTarifa,t.TextoTarifa,ct.FechaDesde,ct.FechaHasta,Per.TextoTarifaGrupo,Per.IdTarifaGrupo,Per.IdContratoTarifa  from ContratoTarifa ct
left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
left join tarifa t  on ct.idtarifa = t.idtarifa
inner join (
select IdContratoTarifa,tg.TextoTarifaGrupo,tg.IdTarifaGrupo from  tarifapreciocontrato tpc
left join IndexadoPrecio inp on tpc.IdIndexadoPrecio = inp.IdIndexadoPrecio
left join tarifagrupo tg on inp.idtarifagrupo = tg.idtarifagrupo 
where  tpc.idcontratotarifa in (select IdContratoTarifa from idcl)
group by IdContratoTarifa,tg.TextoTarifaGrupo,tg.IdTarifaGrupo
) Per on ct.IdContratoTarifa = Per.IdContratoTarifa"
            listasQuerys.Add(query)

            Dim rutaCarpeta As String = $"C:\Users\{Environment.UserName}\Desktop\PreciosPersonalizados"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, $"RevisaPreciosPersonalizados_{Date.Now.ToString("ddMMyyyyHHmmss")}.xlsx")
            ' Verificar si la carpeta existe, y si no, crearla
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If

            ' Verificar si el archivo existe, y si no, crearlo
            If Not File.Exists(rutaArchivo) Then
                File.Create(rutaArchivo).Close()
            End If

            EscribeExcel.EjecutarConsultasYGuardarEnExcel(listasQuerys, rutaArchivo)
            Return rutaArchivo
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Sub RevisaTarifaPrecioContratoPersonalizadaGas()
        Try
            Dim listasQuerys As New List(Of String)
            Dim EscribeExcel As New ValidacionExcel(connectionString)
            Dim query = $";with idcl as (select ct.IdContratoTarifa from tarifapreciocontrato tf
inner join ContratoTarifa ct on ct.idcontratotarifa = tf.idcontratotarifa
where tf.IdIndexadoPrecio in (select IdIndexadoPrecio from IndexadoPrecio where idtarifagrupo in (
select IdTarifaGrupo from TarifaGrupo where textotarifagrupo like '%persona%' and IdTarifa>=300001)
) and ct.idtarifagrupo not in (select IdTarifaGrupo from TarifaGrupo where textotarifagrupo like '%persona%' and IdTarifa>=300001))

select ct.idcontratotarifa, ct.codigocontrato,tg.IdTarifaGrupo,tg.textotarifagrupo, pf.TextoPerfilFacturacion, t.IdTarifa,t.TextoTarifa,ct.FechaDesde,ct.FechaHasta  from ContratoTarifa ct
left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
left join tarifa t  on ct.idtarifa = t.idtarifa
where idcontratotarifa in (select IdContratoTarifa from idcl) order by tg.TextoTarifaGrupo"
            listasQuerys.Add(query)

            Dim rutaCarpeta As String = $"C:\Users\{Environment.UserName}\Desktop\PreciosPersonalizados"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, $"RevisaPreciosPersonalizadosGas_{Date.Now.ToString("ddMMyyyyHHmmss")}.xlsx")
            ' Verificar si la carpeta existe, y si no, crearla
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If

            ' Verificar si el archivo existe, y si no, crearlo
            If Not File.Exists(rutaArchivo) Then
                File.Create(rutaArchivo).Close()
            End If

            EscribeExcel.EjecutarConsultasYGuardarEnExcel(listasQuerys, rutaArchivo)
        Catch ex As Exception
            Throw
        End Try
    End Sub


    Public Function GetClienteContacto(idCliente As Long, txtemail As String) As ClienteContacto
        Dim objClienteContacto As New ClienteContacto

        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select * from ClienteContacto where idcliente = {idCliente} and valor = '{txtemail}'"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read

                        objClienteContacto.IdClienteContacto = readerQuery.GetValue(0).ToString
                        objClienteContacto.Entorno = readerQuery.GetValue(1).ToString
                        objClienteContacto.IdCliente = readerQuery.GetValue(2).ToString
                        objClienteContacto.TipoContacto = readerQuery.GetValue(3).ToString
                        objClienteContacto.Valor = readerQuery.GetValue(4).ToString
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return objClienteContacto
    End Function

    Public Function GetContratoContactobyCodContratoEmail(codContrato As Long) As ContratoContacto
        Dim objContratoContacto As New ContratoContacto

        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"
select IdContratoContacto,ContratoContacto.Entorno,CodigoContrato,ClienteContacto.IdClienteContacto,IdCliente,valor from ContratoContacto
inner join ClienteContacto on ContratoContacto.IdClienteContacto = ClienteContacto.IdClienteContacto
where TipoContacto = 'E' and CodigoContrato = {codContrato}"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read

                        objContratoContacto.IdContratoContacto = readerQuery.GetValue(0).ToString
                        objContratoContacto.Entorno = readerQuery.GetValue(1).ToString
                        objContratoContacto.CodigoContrato = readerQuery.GetValue(2).ToString
                        objContratoContacto.IdClienteContacto = readerQuery.GetValue(3).ToString
                        objContratoContacto.IdCliente = readerQuery.GetValue(4).ToString
                        objContratoContacto.Valor = readerQuery.GetValue(5).ToString
                    Loop
                End If

                readerQuery.Close()
            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return objContratoContacto
    End Function


    Public Function DeleteContratoContactobyIdContratoContacto(IdContratoContacto As Long) As Long
        Dim FilfasAfectadas As Long
        Try
            Dim conexion = New SqlConnection(connectionString)
            conexion.Open()
            Dim query = $"delete ContratoContacto where IdContratoContacto = {IdContratoContacto}"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Throw
        End Try
        Return FilfasAfectadas
    End Function

    Public Function InsertContractoContacto(Entorno As String, Codigocontrato As Long, IdClienteContacto As Long) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            Dim query = $"INSERT INTO [dbo].[ContratoContacto]([Entorno],[CodigoContrato],[IdClienteContacto],[Bloque],[PorDefecto])VALUES('{Entorno}',{Codigocontrato},{IdClienteContacto},null,1)"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function

    Public Function InsertClienteContactoEmail(IdCliente As Long, Email As String) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            Dim query = $"INSERT INTO [dbo].[ClienteContacto]([Entorno],[IdCliente],[TipoContacto],[Valor],[Contacto],[Departamento],[PorDefecto])VALUES('U',{IdCliente},'E','{Email}','','',0)
"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function


    Public Function GetAgente() As List(Of Agente)
        Dim conexion = New SqlConnection(connectionString)

        Dim ListaAgentes As New List(Of Agente)
        Try
            conexion.Open()
            Dim query = $"Select IdAgente,Entorno,NombreAgente from agente where entorno = 'G1' or entorno = 'G2'"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Se lee cada fila del SqlDataReader y se crea un objeto Agente
            Do While readerQuery.Read
                Dim AgenteC = New Agente

                ' Controlar valores nulos y convertir al tipo de datos correcto para cada campo
                AgenteC.IdAgente = GetValueOrDefault(Of Long)(readerQuery, 0, 0)
                AgenteC.Entorno = GetValueOrDefault(Of String)(readerQuery, 1, "")
                AgenteC.NombreAgente = $"{AgenteC.Entorno} {GetValueOrDefault(Of String)(readerQuery, 2, "")}"
                ListaAgentes.Add(AgenteC)
            Loop
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ListaAgentes
    End Function

    Public Function GetAgenteAll() As List(Of Agente)
        Dim conexion = New SqlConnection(connectionString)

        Dim ListaAgentes As New List(Of Agente)
        Try
            conexion.Open()
            Dim query = $"Select * from agente where entorno = 'G1' or entorno = 'G2'"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Se lee cada fila del SqlDataReader y se crea un objeto Agente
            Do While readerQuery.Read
                Dim AgenteC = New Agente

                ' Controlar valores nulos y convertir al tipo de datos correcto para cada campo
                AgenteC.IdAgente = GetValueOrDefault(Of Long)(readerQuery, 0, 0)
                AgenteC.Entorno = GetValueOrDefault(Of String)(readerQuery, 1, "")
                AgenteC.NombreAgente = $"{GetValueOrDefault(Of String)(readerQuery, 2, "")} {AgenteC.Entorno}"
                AgenteC.Direccion = GetValueOrDefault(Of String)(readerQuery, 3, "")
                AgenteC.CodigoPostal = GetValueOrDefault(Of String)(readerQuery, 4, "")
                AgenteC.Ciudad = GetValueOrDefault(Of String)(readerQuery, 5, "")
                AgenteC.Telefono = GetValueOrDefault(Of String)(readerQuery, 6, "")
                AgenteC.Movil = GetValueOrDefault(Of String)(readerQuery, 7, "")
                AgenteC.email = GetValueOrDefault(Of String)(readerQuery, 8, "")
                AgenteC.Web = GetValueOrDefault(Of String)(readerQuery, 9, "")
                AgenteC.IdProveedor = GetValueOrDefault(Of Long)(readerQuery, 10, 0)
                AgenteC.IdAgenteGrupo = GetValueOrDefault(Of Long)(readerQuery, 11, 0)
                AgenteC.Notas = GetValueOrDefault(Of String)(readerQuery, 12, "")
                AgenteC.CodigoTipoAgente = GetValueOrDefault(Of Long)(readerQuery, 13, 0)
                AgenteC.IdAgenteNivelAnterior = GetValueOrDefault(Of Long)(readerQuery, 14, 0)
                AgenteC.EmailSolicitud = GetValueOrDefault(Of String)(readerQuery, 15, "")
                AgenteC.CodigoVendedor = GetValueOrDefault(Of Long)(readerQuery, 16, 0)
                AgenteC.IdPerfilCanal = GetValueOrDefault(Of Long)(readerQuery, 17, 0)
                AgenteC.IdAgenteTipoVenta = GetValueOrDefault(Of Long)(readerQuery, 18, 0)
                AgenteC.CodigoTipoVenta = GetValueOrDefault(Of Long)(readerQuery, 19, 0)
                AgenteC.NotificacionAutomaticas = GetValueOrDefault(Of Long)(readerQuery, 20, 0)
                AgenteC.NotificacionAutomaticasJerarquia = GetValueOrDefault(Of Long)(readerQuery, 21, 0)

                ListaAgentes.Add(AgenteC)
            Loop

            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ListaAgentes
    End Function

    Public Function GetAdmind() As List(Of Administrador)
        Dim conexion = New SqlConnection(connectionString)

        Dim ListaAdministrador As New List(Of Administrador)
        Try
            conexion.Open()
            Dim query = $"Select IdAdministrador,Entorno,NombreAdministrador from Administrador where entorno = 'G1' or entorno = 'G2'"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Se lee cada fila del SqlDataReader y se crea un objeto Agente
            Do While readerQuery.Read
                Dim AdminC = New Administrador

                ' Controlar valores nulos y convertir al tipo de datos correcto para cada campo
                AdminC.IdAdministrador = GetValueOrDefault(Of Long)(readerQuery, 0, 0)
                AdminC.Entorno = GetValueOrDefault(Of String)(readerQuery, 1, "")
                AdminC.NombreAdministrador = $"{AdminC.Entorno} {GetValueOrDefault(Of String)(readerQuery, 2, "")}"
                ListaAdministrador.Add(AdminC)
            Loop
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ListaAdministrador
    End Function

    Public Function GetAdminAll() As List(Of Administrador)
        Dim conexion = New SqlConnection(connectionString)

        Dim ListaAdministrador As New List(Of Administrador)
        Try
            conexion.Open()
            Dim query = $"Select * from agente where entorno = 'G1' or entorno = 'G2'"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Se lee cada fila del SqlDataReader y se crea un objeto Agente
            Do While readerQuery.Read
                Dim AdminC = New Administrador

                ' Controlar valores nulos y convertir al tipo de datos correcto para cada campo
                AdminC.IdAdministrador = GetValueOrDefault(Of Long)(readerQuery, 0, 0)
                AdminC.Entorno = GetValueOrDefault(Of String)(readerQuery, 1, "")
                AdminC.NombreAdministrador = $"{AdminC.Entorno} {GetValueOrDefault(Of String)(readerQuery, 2, "")}"
                AdminC.Direccion = GetValueOrDefault(Of String)(readerQuery, 3, "")
                AdminC.CodigoPostal = GetValueOrDefault(Of String)(readerQuery, 4, "")
                AdminC.Ciudad = GetValueOrDefault(Of String)(readerQuery, 5, "")
                AdminC.Telefono = GetValueOrDefault(Of String)(readerQuery, 6, "")
                AdminC.Movil = GetValueOrDefault(Of String)(readerQuery, 7, "")
                AdminC.email = GetValueOrDefault(Of String)(readerQuery, 8, "")
                AdminC.Web = GetValueOrDefault(Of String)(readerQuery, 9, "")
                AdminC.Notas = GetValueOrDefault(Of Long)(readerQuery, 10, 0)
                AdminC.UsuarioWeb = GetValueOrDefault(Of Long)(readerQuery, 11, 0)
                AdminC.PasswordWeb = GetValueOrDefault(Of String)(readerQuery, 12, "")
                AdminC.FincasPlus = GetValueOrDefault(Of Boolean)(readerQuery, 13, False)
                AdminC.TuComunidad = GetValueOrDefault(Of Boolean)(readerQuery, 14, False)
                AdminC.TAAF = GetValueOrDefault(Of Boolean)(readerQuery, 15, False)
                AdminC.IsPasswordEncriptado = GetValueOrDefault(Of Boolean)(readerQuery, 16, False)


                ListaAdministrador.Add(AdminC)
            Loop

            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ListaAdministrador
    End Function

    Public Function GetValueOrDefault(Of T)(reader As SqlDataReader, columnIndex As Integer, defaultValue As T) As T
        If reader.IsDBNull(columnIndex) Then
            Return defaultValue
        Else
            Dim value As Object = reader.GetValue(columnIndex)
            If GetType(T) Is GetType(String) Then
                Return CType(CObj(value.ToString()), T)
            ElseIf GetType(T) Is GetType(Long) Then
                If Long.TryParse(value.ToString(), Nothing) Then
                    Return CType(CObj(value), T)
                Else
                    Return defaultValue
                End If
            ElseIf GetType(T) Is GetType(Integer) Then
                If Integer.TryParse(value.ToString(), Nothing) Then
                    Return CType(CObj(value), T)
                Else
                    Return defaultValue
                End If
            ElseIf GetType(T) Is GetType(Decimal) Then
                If Decimal.TryParse(value.ToString(), Nothing) Then
                    Return CType(CObj(value), T)
                Else
                    Return defaultValue
                End If
            ElseIf GetType(T) Is GetType(Byte) Then
                If Byte.TryParse(value.ToString(), Nothing) Then
                    Return CType(CObj(value), T)
                Else
                    Return defaultValue
                End If
            ElseIf GetType(T) Is GetType(Boolean) Then
                If Boolean.TryParse(value.ToString(), Nothing) Then
                    Return CType(CObj(value), T)
                Else
                    Return defaultValue
                End If
                ' Agrega otros tipos de datos según sea necesario
            Else
                Return CType(CObj(value), T)
            End If
        End If
    End Function

    Public Function UpdateContratoIdAgente(Contrato As Long, IdAgente As Long) As Long
        Dim FilfasAfectadas As Long
        Try
            Dim conexion = New SqlConnection(connectionString)
            conexion.Open()
            Dim query = $"update Contrato set IdAgente = {IdAgente} where CodigoContrato ={Contrato}"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Throw
        End Try
        Return FilfasAfectadas
    End Function
    Public Function UpdateContratoIdAdmin(Contrato As Long, IdAdministrador As Long) As Long
        Dim FilfasAfectadas As Long
        Try
            Dim conexion = New SqlConnection(connectionString)
            conexion.Open()
            Dim query = $"update Contrato set IdAdministrador = {IdAdministrador} where CodigoContrato ={Contrato}"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Throw
        End Try
        Return FilfasAfectadas
    End Function

    Public Function ExtraerPDFFactura(Facs As String) As Byte()
        Dim conexion = New SqlConnection(connectionString)

        Dim FacturaByte As Byte() = Nothing
        Try
            conexion.Open()
            Dim query = $" select  IdFacturaVentaCabecera, d.DocumentoData from FacturaVentaCabecera fv
 left join contratodocumento cd on fv.IdContratoDocumento = cd.IdContratoDocumento
 left join Documento d on cd.IdDocumento = d.IdDocumento
 where CONCAT(seriefactura,numerofactura) ='{Facs}'"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

             ' Se lee cada fila del SqlDataReader y se crea un objeto Agente
        If readerQuery.Read() Then
                ' Leemos los datos binarios del campo DocumentoData
                If Not readerQuery.IsDBNull(readerQuery.GetOrdinal("DocumentoData")) Then
                    ' Leer los datos binarios del campo DocumentoData
                    FacturaByte = DirectCast(readerQuery("DocumentoData"), Byte())
                End If
            End If
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FacturaByte
    End Function


    Public Function CodPostalCups(IdCups As Long) As String
        Dim conexion = New SqlConnection(connectionString)

        Dim IdCupss As String = 0
        Try
            conexion.Open()
            Dim query = $"select codpostal from CUPS where IdCups = {IdCups}"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Se lee cada fila del SqlDataReader y se crea un objeto Agente
            If readerQuery.Read() Then
                ' Leemos los datos binarios del campo DocumentoData
                If Not readerQuery.IsDBNull(readerQuery.GetOrdinal("codpostal")) Then
                    ' Leer los datos binarios del campo DocumentoData
                    IdCupss = DirectCast(readerQuery("codpostal"), String)
                End If
            End If
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return IdCupss
    End Function
End Class

