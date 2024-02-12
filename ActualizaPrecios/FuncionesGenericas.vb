Imports System.Collections.ObjectModel
Imports System.Data.SqlClient


Public Class FuncionesGenericas

    Private funciones As FuncionesGenericas
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
    Public Function BuscarbyCodigocontrato(ListaContratos As List(Of Long), ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of Long)
        Dim ListaContrato As New List(Of Long)

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(connectionString)
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


    Public Function GetTarifaGrupo(TextoTarifaGrupo As String, ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of TarifaGrupo)
        Dim conexion = New SqlConnection(ipDB + nameDB + userDB + passDB)
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
    Public Function GetContrato(CodContrato As Long, ipDB As String, nameDB As String, userDB As String, passDB As String) As Contrato
        Dim Contrato As New Contrato

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"SELECT IdContrato,CodigoContrato,FechaAplicacionPrecios,FechaContrato,Entorno
                    FROM Contrato
                    WHERE codigocontrato = {CodContrato}"

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Contrato.IdContrato = readerQuery.GetValue(0).ToString
                        Contrato.CodigoContrato = readerQuery.GetValue(1).ToString
                        Contrato.FechaAplicacionPrecios = readerQuery.GetValue(2).ToString
                        Contrato.FechaContrato = readerQuery.GetValue(3).ToString
                        Contrato.Entorno = readerQuery.GetValue(4).ToString
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

    Public Function GetPerfilFacturacion(IdPerfilFacturacion As Long, ipDB As String, nameDB As String, userDB As String, passDB As String) As PerfilFacturacion
        Dim PerfilFacturacion As New PerfilFacturacion

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
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
                    PerfilFacturacion.PerfilFacturacionConfiguraciones = GetPerfilFacturacionConfiguracion(IdPerfilFacturacion, ipDB, nameDB, userDB, passDB)
                End If

            End Using
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return PerfilFacturacion
    End Function


    Public Function GetPerfilFacturacionConfiguracion(IdPerfilFacturacion As Long, ipDB As String, nameDB As String, userDB As String, passDB As String) As ObservableCollection(Of PerfilFacturacionConfiguracion)
        Dim PerfilFacturacionConfiguracion As New ObservableCollection(Of PerfilFacturacionConfiguracion)

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
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

    Public Function GetDTOAllPeriodosIndx(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?, ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of IndexadoPrecio)
        Dim IndexadoPrecio As New List(Of IndexadoPrecio)

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"SELECT top 6 IdIndexadoPrecio,IndexadoPrecio.Entorno,IndexadoPrecio.IdTarifa,IdTarifaGrupo,IdIndexadoConcepto,tp.IdTarifaPeriodo,tp.TextoTarifaPeriodo
                    FROM IndexadoPrecio
                    Inner join TarifaPeriodo tp on IndexadoPrecio.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE IndexadoPrecio.IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo} and FechaFinPresupuesto ='{FechaPresupuesto}' order by IdIndexadoPrecio desc"

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

    Public Function GetDTOAllPeriodosIndxGas(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?, ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of IndexadoPrecioGas)
        Dim IndexadoPrecioGas As New List(Of IndexadoPrecioGas)

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select IdIndexadoPrecioGas,Entorno,IdTarifa,IdTarifaGrupo,IdTarifaPeriodo,tp.TextoTarifaPeriodo
                    FROM IndexadoPrecioGas
                    Inner join TarifaPeriodo tp on IndexadoPrecioGas.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo} and FechaFinPresupuesto ='{FechaPresupuesto}' order by IdIndexadoPrecioGas desc"

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

    Public Function GetDTOAllPeriodosTarifaPrecio(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?, ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of TarifaPrecio)
        Dim TarifaPrecio As New List(Of TarifaPrecio)

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select top 6 IdTarifaPrecio,Entorno,IdTarifa,IdTarifaGrupo,IdTarifaPeriodo,tp.TextoTarifaPeriodo
                    FROM TarifaPrecio
                    Inner join TarifaPeriodo tp on TarifaPrecio.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo} and FechaFinPresupuesto ='{FechaPresupuesto}' order by IdTarifaPrecio desc"

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


    Public Function UpdatePrecioContratoTarifa(Cont As List(Of TarifaPrecioContrato), ContOld As List(Of TarifaPrecioContrato), ipDB As String, nameDB As String, userDB As String, passDB As String) As Long
        Dim conexion = New SqlConnection(ipDB + nameDB + userDB + passDB)

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
                    If ContNuevo.Entorno = "G1" Then
                        Dim query = $"UPDATE tarifapreciocontrato
                                set IdIndexadoPrecio = {ContNuevo.IdIndexadoPrecio}   where IdTarifaPrecioContrato in ({ContViejo.IdTarifaPrecioContrato})"
                        Dim comando = New SqlCommand(query, conexion)
                        FilfasAfectadas = comando.ExecuteNonQuery

                    ElseIf ContNuevo.Entorno = "G2" Then
                        Dim query = $"UPDATE tarifapreciocontrato
                                    set IdIndexadoPrecioGas = {ContNuevo.IdIndexadoPrecioGas}   where IdTarifaPrecioContrato in ({ContViejo.IdTarifaPrecioContrato})"
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



    Public Function GetPrecioContratoTarifa(Cont As ContratoTarifa, ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of TarifaPrecioContrato)
        Dim conexion = New SqlConnection(ipDB + nameDB + userDB + passDB)

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
                        ) order by tp.IdTarifaPeriodo"
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


    Public Function GetProductosbyEntorno(Entorno As String, ipDB As String, nameDB As String, userDB As String, passDB As String) As List(Of Producto)
        Dim Productos As New List(Of Producto)

        Try
            Dim connectionString As String = $"{ipDB}{nameDB}{userDB}{passDB}"
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"select IdProducto,Entorno,IdProductoGrupo,TextoProducto,isnull(Importe,0),AntesIE,isnull(IdTipoImpuesto,0) from Producto where Entorno = '{Entorno}' "

                Dim comando As New SqlCommand(query, conexion)
                comando.CommandTimeout = 3600
                Dim readerQuery As SqlDataReader = comando.ExecuteReader()

                If readerQuery.HasRows Then
                    Do While readerQuery.Read
                        Dim Pro As New Producto
                        Pro.IdProducto = readerQuery.GetValue(0).ToString
                        Pro.Entorno = readerQuery.GetValue(1).ToString
                        Pro.IdProducto = readerQuery.GetValue(2).ToString
                        Pro.TextoProducto = readerQuery.GetValue(3).ToString
                        Pro.Importe = readerQuery.GetValue(4).ToString
                        Pro.AntesIE = readerQuery.GetValue(5).ToString
                        Pro.IdTipoImpuesto = readerQuery.GetValue(6).ToString
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
End Class
