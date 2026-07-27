Imports System.Collections.ObjectModel
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports DocumentFormat.OpenXml.Drawing
Imports SigeCom.Repository

Public Class FuncionesGenericas

    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub
    'Private ReadOnly Property URL_API_DOCUMENTOS As String = "http://172.31.100.51:8045/documentos/"
    Private ReadOnly Property URL_API_DOCUMENTOS As String = "http://172.31.100.31:8045/documentos/"

    Private ReadOnly Property API_DOCUMENTOS_ACTIVA As Boolean = True

    'Private HelperSQL As New Helper

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
        Dim ListaContratov2 As New List(Of Integer)
        Try
            For Each CodigoCon In ListaContratos
                Dim query As String = $"SELECT CodigoContrato
                    FROM Contrato
                    WHERE codigocontrato = {CodigoCon}"
                Dim result = Helper.QuerySelect(query, connectionString)
                Dim errores = Helper.GetError(result)
                If errores.HasError Then
                    'Escribir errores en un log'
                Else
                    Dim lista = Helper.FillObjectFromDatatable(result.Tables(0), GetType(Contrato)).Cast(Of Contrato).FirstOrDefault
                    If Not IsNothing(lista) AndAlso lista.CodigoContrato > 0 Then
                        ListaContrato.Add(lista.CodigoContrato)

                    End If
                End If
            Next
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
            Dim query As String = $"SELECT IdContrato,CodigoContrato,FechaAplicacionPrecios,FechaContrato,c.Entorno,idcliente,idcontratosituacion,idcups, c.IdTipoImpuesto, c.fechaalta,c.entorno
                    FROM Contrato c
					left join TipoImpuesto on c.IdTipoImpuesto = TipoImpuesto.IdTipoImpuesto
                    WHERE codigocontrato = {CodContrato}"

            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ContratoBD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(Contrato)).Cast(Of Contrato).FirstOrDefault
                If Not IsNothing(ContratoBD) AndAlso ContratoBD.IdContrato > 0 Then
                    Contrato = ContratoBD

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return Contrato
    End Function

    Public Function GetContratoMasivo(CodContrato As List(Of Long)) As List(Of Contrato)
        Dim Contrato As New List(Of Contrato)

        Try
            Dim joinContrato = String.Join(",", CodContrato)
            Dim query As String = $"SELECT IdContrato,CodigoContrato,FechaAplicacionPrecios,FechaContrato,c.Entorno,idcliente,idcontratosituacion,idcups, c.IdTipoImpuesto, c.fechaalta
                    FROM Contrato c
					left join TipoImpuesto on c.IdTipoImpuesto = TipoImpuesto.IdTipoImpuesto
                    WHERE codigocontrato in ({joinContrato})"

            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ContratoBD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(Contrato)).Cast(Of Contrato).ToList
                If Not IsNothing(ContratoBD) AndAlso ContratoBD.Count > 0 Then
                    Contrato = ContratoBD
                End If
            End If
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
                                                 WHERE IdPerfilFacturacion = {IdPerfilFacturacion} and codigoconcepto is not null"

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

    Public Function GetDTOAllPeriodosIndxByFechaFinPresupuesto(Entorno As String, IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As List(Of IndexadoPrecio)
        Dim IndexadoPrecioGas As New List(Of IndexadoPrecio)
        Dim TarifaPeriodoSrv As New TarifaPeriodoSrv(connectionString)
        Dim ret As New List(Of IndexadoPrecio)
        Dim IndexPrecioGas = GetDTOAllPeriodosIndxbyFechaPresupuesto(IdTarifa, IdTarifaGrupo, FechaPresupuesto)
        Try
            Dim IndexadoPrecioGasBBDD As New List(Of IndexadoPrecio)
            If Not IsNothing(IndexPrecioGas) AndAlso IndexPrecioGas.IdIndexadoPrecio > 0 Then
                Dim Query = $"SELECT *
FROM IndexadoPrecio TP
WHERE TP.Entorno = '{Entorno}'
  AND ISNULL(TP.IdTarifa, 0) = ISNULL({IndexPrecioGas.IdTarifa}, 0)
  AND ISNULL(TP.IdTarifaGrupo, 0) = ISNULL({IndexPrecioGas.IdTarifaGrupo}, 0)
  AND ISNULL(TP.fechafinpresupuesto, '31-12-9999') = ISNULL('{IndexPrecioGas.FechaFinPresupuesto}', GETDATE());

"
                Dim result = Helper.QuerySelect(Query, connectionString)
                Dim errores = Helper.GetError(result)
                If Not errores.HasError Then
                    Dim TIndexadoPrecioGas = Helper.FillObjectFromDatatable(result.Tables(0), GetType(IndexadoPrecio)).Cast(Of IndexadoPrecio).ToList
                    If Not IsNothing(TIndexadoPrecioGas) AndAlso TIndexadoPrecioGas.Count > 0 Then
                        IndexadoPrecioGas = TIndexadoPrecioGas
                    End If
                End If
            End If

            Dim objIndexadoPrecioGas As IndexadoPrecio
            For Each ele In IndexadoPrecioGas
                objIndexadoPrecioGas = ele
                objIndexadoPrecioGas.tarifaperiodo = TarifaPeriodoSrv.GetDTO(If(objIndexadoPrecioGas.IdTarifaPeriodo, 0L))
                ret.Add(objIndexadoPrecioGas)
            Next

        Catch ex As Exception
            Throw
        End Try

        Return ret
    End Function

    Public Function GetDTOAllPeriodosIndx(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As List(Of IndexadoPrecio)
        Dim IndexadoPrecio As New List(Of IndexadoPrecio)

        Try
            Dim top = If(IdTarifa = 202020, 3, 6)
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"SELECT IdIndexadoPrecio,IndexadoPrecio.Entorno,IndexadoPrecio.IdTarifa,IdTarifaGrupo,IdIndexadoConcepto,tp.IdTarifaPeriodo,tp.TextoTarifaPeriodo
                    FROM IndexadoPrecio
                    Inner join TarifaPeriodo tp on IndexadoPrecio.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE IndexadoPrecio.IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo} and FechaFinPresupuesto>='{FechaPresupuesto}' order by idindexadoprecio desc"
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

    Public Function GetDTOAllPeriodosIndxbyFechaPresupuesto(IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As IndexadoPrecio
        Dim IndexadoPrecio As New IndexadoPrecio

        Try
            Dim top = If(IdTarifa = 202020, 3, 6)


            Dim query As String = $"SELECT IdIndexadoPrecio,IndexadoPrecio.Entorno,IndexadoPrecio.IdTarifa,IdTarifaGrupo,IdIndexadoConcepto,tp.IdTarifaPeriodo,tp.TextoTarifaPeriodo,FechaFinPresupuesto
                    FROM IndexadoPrecio
                    Inner join TarifaPeriodo tp on IndexadoPrecio.IdTarifaPeriodo = tp.IdTarifaPeriodo
                    WHERE IndexadoPrecio.IdTarifa = {IdTarifa} AND IdTarifaGrupo = {IdTarifaGrupo} and isnull(FechaFinPresupuesto,'31-12-9999')>=isnull('{FechaPresupuesto}',getdate()) and FechaFinPresupuesto is not null"
            'and FechaFinPresupuesto ='{FechaPresupuesto}' 
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If Not errores.HasError Then
                Dim tp = Helper.FillObjectFromDatatable(result.Tables(0), GetType(IndexadoPrecio)).Cast(Of IndexadoPrecio).ToList
                If Not IsNothing(tp) AndAlso tp.Count > 0 Then
                    IndexadoPrecio = tp.OrderBy(Function(f) f.FechaFinPresupuesto.Value).FirstOrDefault
                End If
            End If
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

    Public Function GetDTOAllPeriodosIndxGasByFechaFinPresupuesto(Entorno As String, IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As List(Of IndexadoPrecioGas)
        Dim IndexadoPrecioGas As New List(Of IndexadoPrecioGas)
        Dim TarifaPeriodoSrv As New TarifaPeriodoSrv(connectionString)
        Dim ret As New List(Of IndexadoPrecioGas)
        Dim IndexPrecioGas = GetDTOIndexadoPreciobyFechaPresupuesto(Entorno, IdTarifa, IdTarifaGrupo, FechaPresupuesto)
        Try
            Dim IndexadoPrecioGasBBDD As New List(Of IndexadoPrecioGas)
            If Not IsNothing(IndexPrecioGas) AndAlso IndexPrecioGas.IdIndexadoPrecioGas > 0 Then
                Dim Query = $"SELECT *
FROM IndexadoPrecioGas TP
WHERE TP.Entorno = '{Entorno}'
  AND ISNULL(TP.IdTarifa, 0) = ISNULL({IdTarifa}, 0)
  AND ISNULL(TP.IdTarifaGrupo, 0) = ISNULL({IdTarifaGrupo}, 0)
  AND ISNULL(TP.FechaVigencia, '31-12-9999') = ISNULL('{IndexPrecioGas.FechaVigencia}', GETDATE());

"
                Dim result = Helper.QuerySelect(Query, connectionString)
                Dim errores = Helper.GetError(result)
                If Not errores.HasError Then
                    Dim TIndexadoPrecioGas = Helper.FillObjectFromDatatable(result.Tables(0), GetType(IndexadoPrecioGas)).Cast(Of IndexadoPrecioGas).ToList
                    If Not IsNothing(TIndexadoPrecioGas) AndAlso TIndexadoPrecioGas.Count > 0 Then
                        IndexadoPrecioGas = TIndexadoPrecioGas
                    End If
                End If
            End If

            Dim objIndexadoPrecioGas As IndexadoPrecioGas
            For Each ele In IndexadoPrecioGas
                objIndexadoPrecioGas = ele
                objIndexadoPrecioGas.TarifaPeriodo = TarifaPeriodoSrv.GetDTO(If(objIndexadoPrecioGas.IdTarifaPeriodo, 0L))
                ret.Add(objIndexadoPrecioGas)
            Next

        Catch ex As Exception
            Throw
        End Try

        Return ret
    End Function

    Public Function GetDTOIndexadoPreciobyFechaPresupuesto(Entorno As String, IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As IndexadoPrecioGas
        Try
            Dim ret As New IndexadoPrecioGas

            Dim fechaSinHora As String = FechaPresupuesto.Value.Date.ToString("dd-MM-yyyy")
            Dim Query = $"SELECT *
FROM IndexadoPrecioGas tp
WHERE tp.Entorno = '{Entorno}'
  AND tp.IdTarifa IS NOT NULL
  AND {IdTarifa} IS NOT NULL
  AND tp.IdTarifa = {IdTarifa}
  AND tp.IdTarifaGrupo IS NOT NULL
  AND {IdTarifaGrupo} IS NOT NULL
  AND tp.IdTarifaGrupo = {IdTarifaGrupo}
  AND tp.FechaVigencia IS NOT NULL
  AND '{fechaSinHora}' IS NOT NULL
  AND tp.FechaVigencia >= '{fechaSinHora}';
"
            Dim result = Helper.QuerySelect(Query, connectionString)
            Dim errores = Helper.GetError(result)
            If Not errores.HasError Then
                Dim Tp = Helper.FillObjectFromDatatable(result.Tables(0), GetType(IndexadoPrecioGas)).Cast(Of IndexadoPrecioGas).ToList
                If Not IsNothing(Tp) AndAlso Tp.Count > 0 Then
                    ret = Tp.OrderBy(Function(f) f.FechaVigencia.Value).FirstOrDefault
                End If
            End If

            Return ret
        Catch ex As Exception
            Throw
        End Try
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

    Public Function GetDTOAllPeriodosTarifaPrecioByFechaPresupuesto(Entorno As String, IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As List(Of TarifaPrecio)
        Dim TarifaPeriodoSrv As New TarifaPeriodoSrv(connectionString)
        Dim TarifaPrecio As New List(Of TarifaPrecio)
        Dim ret As New List(Of TarifaPrecio)
        Dim TarifaPrecioBD As New TarifaPrecio
        TarifaPrecioBD = GetDTOTarifaPreciobyFechaPresupuesto(Entorno, IdTarifa, IdTarifaGrupo, If(FechaPresupuesto, DateAndTime.Now))

        Try
            Dim TarifasPreciosBBDD As New List(Of TarifaPrecio)
            If Not IsNothing(TarifaPrecioBD) AndAlso TarifaPrecioBD.IdTarifaPrecio > 0 Then
                Dim Query = $"SELECT *
FROM TarifaPrecio TP
WHERE TP.Entorno = '{Entorno}'
  AND ISNULL(TP.IdTarifa, 0) = ISNULL({IdTarifa}, 0)
  AND ISNULL(TP.IdTarifaGrupo, 0) = ISNULL({IdTarifaGrupo}, 0)
  AND ISNULL(TP.FechaFinPresupuesto, '31-12-9999') = ISNULL('{TarifaPrecioBD.FechaFinPresupuesto}', GETDATE());

"
                Dim result = Helper.QuerySelect(Query, connectionString)
                Dim errores = Helper.GetError(result)
                If Not errores.HasError Then
                    Dim TTarifasPreciop = Helper.FillObjectFromDatatable(result.Tables(0), GetType(TarifaPrecio)).Cast(Of TarifaPrecio).ToList
                    If Not IsNothing(TTarifasPreciop) AndAlso TTarifasPreciop.Count > 0 Then
                        TarifasPreciosBBDD = TTarifasPreciop
                    End If
                End If

            End If

            Dim objTarifaPrecio As TarifaPrecio
            For Each ele In TarifasPreciosBBDD
                objTarifaPrecio = ele
                objTarifaPrecio.TarifaPeriodo = TarifaPeriodoSrv.GetDTO(If(objTarifaPrecio.IdTarifaPeriodo, 0L))
                ret.Add(objTarifaPrecio)
            Next

        Catch ex As Exception
            Throw
        End Try

        Return ret
    End Function
    Public Function GetDTOTarifaPreciobyFechaPresupuesto(Entorno As String, IdTarifa As Long?, IdTarifaGrupo As Long?, FechaPresupuesto As DateTime?) As TarifaPrecio
        Try
            Dim ret As New TarifaPrecio

            Dim fechaSinHora As String = FechaPresupuesto.Value.Date.ToString("dd-MM-yyyy")

            Dim Query = $"
SELECT *
FROM TarifaPrecio tp
WHERE tp.Entorno = '{Entorno}'
  AND tp.IdTarifa IS NOT NULL
  AND {IdTarifa} IS NOT NULL
  AND tp.IdTarifa = {IdTarifa}
  AND tp.IdTarifaGrupo IS NOT NULL
  AND {IdTarifaGrupo} IS NOT NULL
  AND tp.IdTarifaGrupo = {IdTarifaGrupo}
  AND tp.FechaFinPresupuesto IS NOT NULL
  AND '{fechaSinHora}' IS NOT NULL
  AND tp.FechaFinPresupuesto >= '{fechaSinHora}';"
            Dim result = Helper.QuerySelect(Query, connectionString)
            Dim errores = Helper.GetError(result)
            If Not errores.HasError Then
                Dim Tp = Helper.FillObjectFromDatatable(result.Tables(0), GetType(TarifaPrecio)).Cast(Of TarifaPrecio).ToList
                If Not IsNothing(Tp) AndAlso Tp.Count > 0 Then
                    ret = Tp.OrderBy(Function(f) f.FechaFinPresupuesto.Value).FirstOrDefault
                End If
            End If

            Return ret
        Catch ex As Exception
            Throw
        End Try
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
        Dim top = If(Cont.IdTarifa = 202020, 3, 6)
        top = If(Cont.Entorno = "G2", 1, top)
        Dim TarifaPrecioContrato As New List(Of TarifaPrecioContrato)
        Try
            conexion.Open()
            Dim query = $"select top {top} tarifapreciocontrato.*, t.idtarifa, tg.IdTarifaGrupo,tg.TextoTarifaGrupo, tp.IdTarifaPeriodo,tp.TextoTarifaPeriodo from tarifapreciocontrato 
                        left join Contratotarifa ct  on tarifapreciocontrato.idcontratotarifa = ct.idcontratotarifa
                        left join tarifa t  on ct.idtarifa = t.idtarifa
                        left join TarifaGrupo tg on ct.IdTarifaGrupo = tg.IdTarifaGrupo
                        left join tarifaprecio Inp on TarifaPrecioContrato.idtarifaprecio = Inp.idtarifaprecio
                        left join TarifaPeriodo tp on inp.IdTarifaPeriodo = tp.IdTarifaPeriodo
                        where ct.idcontratotarifa in (
                        {Cont.IdContratoTarifa}
                        ) order by  IdTarifaPrecioContrato, tp.IdTarifaPeriodo desc"
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
                        TarifaPrecioContratoL.IdIndexadoPrecioGas = GetValueOrDefault(readerQuery, 5, 0)
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
                    'Productos.Add(New Producto)
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

    Public Function GetProductosbyTextoProducto(ProductoBuscar As String, Entorno As String) As Producto
        Dim Producto As New Producto

        Try
            Dim query As String = $"select * from Producto where  textoproducto = '{ProductoBuscar}' and entorno='{Entorno}'"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ProductoEncontrado = Helper.FillObjectFromDatatable(result.Tables(0), GetType(Producto)).Cast(Of Producto).FirstOrDefault
                If Not IsNothing(ProductoEncontrado) AndAlso ProductoEncontrado.IdProducto > 0 Then
                    Producto = ProductoEncontrado
                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return Producto
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

    Public Function InsertProductoAsignacion(Entorno As String, IdProductoGrupo As Long, IdProducto As Long, IdContrato As Long, FechaInicial As Date, Importe As Decimal, IdTipoImpuesto As Long, AntesIE As Boolean, AplicarSobreConsumo As Boolean, AplicarPrecioConsumo As Boolean, PrecioSobredia As Boolean, FechaFinal As String) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            'Dim Codigos = String.Join(",", Contratos)
            'Dim query = $"INSERT INTO ProductoAsignacion(Entorno,IdProductoGrupo,IdProducto,TipoAsignacion,IdContrato,FechaInicial,Importe,IdTipoImpuesto,AntesIE,AplicarSobreConsumo,AplicarPrecioConsumo) 
            '                values('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO',{IdContrato},'{FechaInicial}',{Importe.ToString},{IdTipoImpuesto},{If(AntesIE, 1, 0)},{If(AplicarSobreConsumo, 1, 0)},{If(AplicarPrecioConsumo, 1, 0)})"
            Dim Query = ""

            If IdTipoImpuesto = 0 Then
                Query =
$"INSERT INTO ProductoAsignacion (Entorno, IdProductoGrupo, IdProducto, TipoAsignacion, IdCliente, IdContrato, IdTarifa, IdTarifaGrupo, IdTipoCobro, IdTarifaPeaje, Desde, Hasta, IsControlFecha, FechaInicial, FechaFinal, Plazo, PlazoCargado, ImporteTotalPlazo, IsFacturado, Importe, Descuento, AntesIE, IdTipoImpuesto, PrecioDia, IsFacturaProrrateo, IdFacturaProrrateo, PorcentajeIncremento, AplicarSobreConsumo, ImportePlazo, AplicarPrecioConsumo, IsBonificacion, FechaAsignacion)
VALUES ('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO', NULL, {IdContrato}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '{FechaInicial}', {FechaFinal},NULL, NULL, NULL, NULL, {Importe.ToString.Replace(",", ".")}, 0.00, {If(AntesIE, 1, 0)}, null, {If(PrecioSobredia, 1, 0)}, 0, NULL, 0.00, {If(AplicarSobreConsumo, 1, 0)}, NULL, {If(AplicarPrecioConsumo, 1, 0)}, NULL, NULL);"

            Else
                Query =
$"INSERT INTO ProductoAsignacion (Entorno, IdProductoGrupo, IdProducto, TipoAsignacion, IdCliente, IdContrato, IdTarifa, IdTarifaGrupo, IdTipoCobro, IdTarifaPeaje, Desde, Hasta, IsControlFecha, FechaInicial, FechaFinal, Plazo, PlazoCargado, ImporteTotalPlazo, IsFacturado, Importe, Descuento, AntesIE, IdTipoImpuesto, PrecioDia, IsFacturaProrrateo, IdFacturaProrrateo, PorcentajeIncremento, AplicarSobreConsumo, ImportePlazo, AplicarPrecioConsumo, IsBonificacion, FechaAsignacion)
VALUES ('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO', NULL, {IdContrato}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '{FechaInicial}', {FechaFinal}, NULL, NULL, NULL, NULL, {Importe.ToString.Replace(",", ".")}, 0.00, {If(AntesIE, 1, 0)},  {IdTipoImpuesto}, {If(PrecioSobredia, 1, 0)}, 0, NULL, 0.00, {If(AplicarSobreConsumo, 1, 0)}, NULL, {If(AplicarPrecioConsumo, 1, 0)}, NULL, NULL);"

            End If

            Dim comando = New SqlCommand(Query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Throw
        End Try
        Return FilfasAfectadas
    End Function

    Public Function InsertProductoAsignacionV2(Entorno As String, IdProductoGrupo As Long, IdProducto As Long, IdContrato As Long, FechaInicial As String, Importe As String, IdTipoImpuesto As Long, AntesIE As String, AplicarSobreConsumo As Boolean, AplicarPrecioConsumo As Boolean, PrecioSobredia As String, FechaFinal As String, Plazo As String, PlazoCargado As String, importeTotalPlazo As String) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            'Dim Codigos = String.Join(",", Contratos)
            'Dim query = $"INSERT INTO ProductoAsignacion(Entorno,IdProductoGrupo,IdProducto,TipoAsignacion,IdContrato,FechaInicial,Importe,IdTipoImpuesto,AntesIE,AplicarSobreConsumo,AplicarPrecioConsumo) 
            '                values('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO',{IdContrato},'{FechaInicial}',{Importe.ToString},{IdTipoImpuesto},{If(AntesIE, 1, 0)},{If(AplicarSobreConsumo, 1, 0)},{If(AplicarPrecioConsumo, 1, 0)})"
            Dim Query = ""

            If IdTipoImpuesto = 0 Then
                Query =
$"INSERT INTO ProductoAsignacion (Entorno, IdProductoGrupo, IdProducto, TipoAsignacion, IdCliente, IdContrato, IdTarifa, IdTarifaGrupo, IdTipoCobro, IdTarifaPeaje, Desde, Hasta, IsControlFecha, FechaInicial, FechaFinal, Plazo, PlazoCargado, ImporteTotalPlazo, IsFacturado, Importe, Descuento, AntesIE, IdTipoImpuesto, PrecioDia, IsFacturaProrrateo, IdFacturaProrrateo, PorcentajeIncremento, AplicarSobreConsumo, ImportePlazo, AplicarPrecioConsumo, IsBonificacion, FechaAsignacion)
VALUES ('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO', NULL, {IdContrato}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, {FechaInicial}, {FechaFinal}, {Plazo}, {PlazoCargado},  {importeTotalPlazo.Replace(",", ".")}, NULL, {Importe.Replace(",", ".")}, 0.00, {AntesIE}, null, {PrecioSobredia}, 0, NULL, 0.00, {If(AplicarSobreConsumo, 1, 0)}, NULL, {If(AplicarPrecioConsumo, 1, 0)}, NULL, NULL);"

            Else
                Query =
$"INSERT INTO ProductoAsignacion (Entorno, IdProductoGrupo, IdProducto, TipoAsignacion, IdCliente, IdContrato, IdTarifa, IdTarifaGrupo, IdTipoCobro, IdTarifaPeaje, Desde, Hasta, IsControlFecha, FechaInicial, FechaFinal, Plazo, PlazoCargado, ImporteTotalPlazo, IsFacturado, Importe, Descuento, AntesIE, IdTipoImpuesto, PrecioDia, IsFacturaProrrateo, IdFacturaProrrateo, PorcentajeIncremento, AplicarSobreConsumo, ImportePlazo, AplicarPrecioConsumo, IsBonificacion, FechaAsignacion)
VALUES ('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO', NULL, {IdContrato}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, {FechaInicial}, {FechaFinal}, {Plazo}, {PlazoCargado}, {importeTotalPlazo.Replace(",", ".")}, NULL, {Importe.Replace(",", ".")}, 0.00, {AntesIE},  {IdTipoImpuesto}, {PrecioSobredia}, 0, NULL, 0.00, {If(AplicarSobreConsumo, 1, 0)}, NULL, {If(AplicarPrecioConsumo, 1, 0)}, NULL, NULL);"

            End If

            Dim comando = New SqlCommand(Query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Throw
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
            '                values('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO',{IdContrato},'{FechaInicial}',{Importe.ToString},{IdTipoImpuesto},{If(AntesIE, 1, 0)},{If(AplicarSobreConsumo, 1, 0)},{If(AplicarPrecioConsumo, 1, 0)})"

            Dim query =
$"INSERT INTO ProductoAsignacion (Entorno, IdProductoGrupo, IdProducto, TipoAsignacion, IdCliente, IdContrato, IdTarifa, IdTarifaGrupo, IdTipoCobro, IdTarifaPeaje, Desde, Hasta, IsControlFecha, FechaInicial, FechaFinal, Plazo, PlazoCargado, ImporteTotalPlazo, IsFacturado, Importe, Descuento, AntesIE, IdTipoImpuesto, PrecioDia, IsFacturaProrrateo, IdFacturaProrrateo, PorcentajeIncremento, AplicarSobreConsumo, ImportePlazo, AplicarPrecioConsumo, IsBonificacion, FechaAsignacion)
VALUES ('{Entorno}', {IdProductoGrupo}, {IdProducto}, 'CO', NULL, {IdContrato}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '{FechaInicial}', NULL, NULL, NULL, NULL, NULL, {Importe.ToString}, 0.00, {If(AntesIE, 1, 0)}, {IdTipoImpuesto}, 0, 0, NULL, 0.00, {If(AplicarSobreConsumo, 1, 0)}, NULL, {If(AplicarPrecioConsumo, 1, 0)}, NULL, NULL);"
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
            Dim rutaArchivo As String = IO.Path.Combine(rutaCarpeta, $"ContratTarifaAntesActualizacion_{Date.Now.ToString("ddMMyyyyHHmmss")}.xlsx")

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
            Dim rutaArchivo As String = IO.Path.Combine(rutaCarpeta, $"RevisaPreciosPersonalizados_{Date.Now.ToString("ddMMyyyyHHmmss")}.xlsx")
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
            Dim rutaArchivo As String = IO.Path.Combine(rutaCarpeta, $"RevisaPreciosPersonalizadosGas_{Date.Now.ToString("ddMMyyyyHHmmss")}.xlsx")
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
    Public Function GetContratoContactobyCodContratoTlfno(codContrato As Long, IsTlfono As String) As ContratoContacto
        Dim objContratoContacto As New ContratoContacto

        Try

            Using conexion As New SqlConnection(connectionString)
                conexion.Open()

                Dim query As String = $"
select IdContratoContacto,ContratoContacto.Entorno,CodigoContrato,ClienteContacto.IdClienteContacto,IdCliente,valor from ContratoContacto
inner join ClienteContacto on ContratoContacto.IdClienteContacto = ClienteContacto.IdClienteContacto
where (TipoContacto='{IsTlfono}') and CodigoContrato = {codContrato}"

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

    Public Function InsertClienteContactoTlfnoMovil(IdCliente As Long, Email As String, IsTlfono As Boolean) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            Dim query = $"INSERT INTO [dbo].[ClienteContacto]([Entorno],[IdCliente],[TipoContacto],[Valor],[Contacto],[Departamento],[PorDefecto])VALUES('U',{IdCliente},'{If(IsTlfono, "T", "M")}','{Email}','','',0)
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
    Public Function UpdateContratoIdAdmin(Contrato As Long, IdAdministrador As Long, PermitirNULL As Boolean) As Long
        Dim FilfasAfectadas As Long
        Try
            Dim conexion = New SqlConnection(connectionString)
            conexion.Open()
            Dim query = ""
            If PermitirNULL Then
                query = $"update Contrato set IdAdministrador = null where CodigoContrato ={Contrato}"
            Else
                query = $"update Contrato set IdAdministrador = {IdAdministrador} where CodigoContrato ={Contrato}"
            End If

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
        Dim IdDocumento As Long = 0
        Try
            conexion.Open()
            Dim query = $" select  IdFacturaVentaCabecera, d.DocumentoData, d.iddocumento from FacturaVentaCabecera fv
 left join contratodocumento cd on fv.IdContratoDocumento = cd.IdContratoDocumento
 left join Documento d on cd.IdDocumento = d.IdDocumento
 where CONCAT(seriefactura,numerofactura) ='{Facs}'"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Se lee cada fila del SqlDataReader y se crea un objeto Documento
            If readerQuery.Read() Then
                ' Leemos los datos binarios del campo DocumentoData
                If Not readerQuery.IsDBNull(readerQuery.GetOrdinal("DocumentoData")) Then
                    ' Leer los datos binarios del campo DocumentoData
                    FacturaByte = DirectCast(readerQuery("DocumentoData"), Byte())

                End If
                If Not readerQuery.IsDBNull(readerQuery.GetOrdinal("iddocumento")) Then
                    IdDocumento = DirectCast(readerQuery("iddocumento"), Long)
                End If
            End If
            readerQuery.Close()
            conexion.Close()

            'Comprobamos si ha traido el documento de BD
            If FacturaByte Is Nothing AndAlso IdDocumento > 0 Then ' si no lo ha traido, lo buscamos en disco
                DocumentDataFromCopiaAnioSiNulo(FacturaByte, IdDocumento)
            End If
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FacturaByte
    End Function

    Public Sub DocumentDataFromCopiaAnioSiNulo(ByRef ret As Byte(), IdDocumento As Long)
        'data source=172.31.100.12;initial catalog=SigeTotal; User ID=Sige;Password=SigeNew; integrated security=False;Connection Timeout=120;Persist Security Info=True;MultipleActiveResultSets=True;
        If ret Is Nothing AndAlso IdDocumento > 0 AndAlso API_DOCUMENTOS_ACTIVA AndAlso If(URL_API_DOCUMENTOS, "").Trim <> String.Empty Then
            Try
                'urlApiDocumento= "https://localhost:8046/Documentos/"
                Dim urlDocumento = $"{URL_API_DOCUMENTOS}{IdDocumento}"
                Dim token = SysMainControl("TOKEN_DOCUMENTOS_API")
                Using client As New HttpClient()
                    If Not String.IsNullOrEmpty(token) Then
                        client.DefaultRequestHeaders.Add("token", token)
                    End If

                    ' Obtener los datos de manera síncrona
                    Dim response As HttpResponseMessage = client.GetAsync(urlDocumento).Result ' Bloqueo síncrono
                    If response.IsSuccessStatusCode Then
                        Dim binario64 As String = response.Content.ReadAsStringAsync().Result ' Bloqueo síncrono
                        ret = Convert.FromBase64String(binario64)
                    End If
                End Using
            Catch ex As Exception
                Throw
            End Try
        End If
    End Sub

    Public Function SysMainControl(CodigoControlS As String) As String
        Dim conexion = New SqlConnection(connectionString)

        Dim Value As String = 0
        Try
            conexion.Open()
            Dim query = $"select Value from SysMainControl where CodigoControl like '%{CodigoControlS}%'"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Se lee cada fila del SqlDataReader
            If readerQuery.Read() Then
                ' Leemos los datos binarios del campo Value
                If Not readerQuery.IsDBNull(readerQuery.GetOrdinal("Value")) Then
                    ' Leer los datos binarios del campo Value
                    Value = DirectCast(readerQuery("Value"), String)
                End If
            End If
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return Value
    End Function



    Public Async Function UrlGetString(url As String, token As String) As Task(Of String)
        Dim ret As String = String.Empty
        Try
            Using client As New HttpClient()
                ' Si hay token, agrégalo al header
                If Not String.IsNullOrEmpty(token) Then
                    client.DefaultRequestHeaders.Add("token", token)
                End If

                ' Realiza la petición GET a la URL
                Dim response As HttpResponseMessage = Await client.GetAsync(url)

                ' Asegúrate de que la petición fue exitosa
                response.EnsureSuccessStatusCode()

                ' Lee el contenido como string usando la codificación UTF-8
                ret = Await response.Content.ReadAsStringAsync()
            End Using

        Catch ex As Exception
            ' Captura la excepción y devuélvela como string
            ret = ex.ToString()
        End Try

        Return ret
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

    Public Function InsertTarifaPrecioContrato(tarifasPrecioContratoGuardar As List(Of TarifaPrecioContrato)) As Long
        Dim FilfasAfectadas As Long
        Try
            Dim conexion = New SqlConnection(connectionString)
            conexion.Open()

            For Each TPC In tarifasPrecioContratoGuardar
                Dim query = $"INSERT INTO [dbo].[TarifaPrecioContrato]([Entorno]
           ,[IdContratoTarifa]
           ,[IdTarifaPrecio]
           ,[IdIndexadoPrecio]
           ,[IdIndexadoPrecioGas])
     VALUES
           ('{TPC.Entorno}',
           {TPC.IdContratoTarifa},
           {TPC.IdTarifaPrecio},
           {TPC.IdIndexadoPrecio},
           null)"
                Dim comando = New SqlCommand(query, conexion)
                FilfasAfectadas += comando.ExecuteNonQuery
            Next
            'Borro las linea de tarifas personalizadas
            Dim query2 = $"delete TarifaPrecioContrato 
where IdContratoTarifa= {tarifasPrecioContratoGuardar.FirstOrDefault.IdContratoTarifa} and IdIndexadoPrecio in (select ipf.IdIndexadoPrecio from IndexadoPrecio ipf
															inner join tarifagrupo tg on ipf.idtarifagrupo = tg.idtarifagrupo
															and tg.TextoTarifaGrupo like '%personalizada%')"
            Dim comando2 = New SqlCommand(query2, conexion)
            Dim FilfasAfectadas2 = comando2.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Throw
        End Try
        Return FilfasAfectadas
    End Function


    Public Function GetFacClick(Factura As String) As List(Of ClickFac)
        Dim conexion = New SqlConnection(connectionString)

        Dim ListaClickFac As New List(Of ClickFac)
        Try
            conexion.Open()
            Dim query = $"select CONCAT(SerieFactura,NumeroFactura)NFactura,Descripcion, ImporteBase from facturaventalinea fl
inner join FacturaVentaCabecera fv on fl.IdFacturaVentaCabecera = fv.IdFacturaVentaCabecera
where CONCAT(fv.SerieFactura,fv.NumeroFactura) ='{Factura}' and FacturaConcepto = 30006"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Se lee cada fila del SqlDataReader y se crea un objeto Agente
            Do While readerQuery.Read
                Dim AClickFacC = New ClickFac

                ' Controlar valores nulos y convertir al tipo de datos correcto para cada campo

                AClickFacC.NFactura = GetValueOrDefault(Of String)(readerQuery, 0, "")
                AClickFacC.Descripcion = GetValueOrDefault(Of String)(readerQuery, 1, "")
                AClickFacC.ImporteBase = GetValueOrDefault(Of Decimal)(readerQuery, 2, 0.0D)

                ListaClickFac.Add(AClickFacC)
            Loop

            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Throw
        End Try
        Return ListaClickFac
    End Function


    Public Function GetFacVenta(idFacturaVenta As Long) As FacturaVentaCabecera
        Dim FacturaVentaC As New FacturaVentaCabecera
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"SELECT IdFacturaVentaCabecera,CodigoContrato
                    FROM FacturaVentaCabecera
                    WHERE IdFacturaVentaCabecera in ( {idFacturaVenta})"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim Fac = Helper.FillObjectFromDatatable(result.Tables(0), GetType(FacturaVentaCabecera)).Cast(Of FacturaVentaCabecera).FirstOrDefault
                If Not IsNothing(Fac) AndAlso Fac.IdFacturaVentaCabecera > 0 Then
                    FacturaVentaC = Fac

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return FacturaVentaC
    End Function

    Public Function GetFacVentaLista(idFacturaVenta As String) As List(Of FacturaVentaCabecera)
        Dim FacturaVentaC As New List(Of FacturaVentaCabecera)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"SELECT IdFacturaVentaCabecera,CodigoContrato
                    FROM FacturaVentaCabecera
                    WHERE IdFacturaVentaCabecera in ({idFacturaVenta})"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim Fac = Helper.FillObjectFromDatatable(result.Tables(0), GetType(FacturaVentaCabecera)).Cast(Of FacturaVentaCabecera).ToList
                If Not IsNothing(Fac) AndAlso Fac.Count > 0 Then
                    FacturaVentaC = Fac

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return FacturaVentaC
    End Function

    Public Function GetContratoTarifaPersonalizado(CodContrato As List(Of Long)) As List(Of ContratoTarifaPersonalizado)
        Dim ContratoTarifaPersonalizadoC As New List(Of ContratoTarifaPersonalizado)
        Dim JoinContrato = String.Join(",", CodContrato)
        Try

            Dim query As String = $"select ct.idcontratotarifa, ct.codigocontrato,tg.IdTarifaGrupo,tg.textotarifagrupo, pf.TextoPerfilFacturacion, t.IdTarifa,t.TextoTarifa,ct.fechadesde
from contratotarifa ct
left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
left join tarifa t  on ct.idtarifa = t.idtarifa
left join contrato c on ct.CodigoContrato = c.CodigoContrato
where ct.idcontratotarifa in (
{JoinContrato}
)

order by c.CodigoContrato

"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ResultC = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ContratoTarifaPersonalizado)).Cast(Of ContratoTarifaPersonalizado).ToList
                If Not IsNothing(ResultC) AndAlso ResultC.Count > 0 Then
                    ContratoTarifaPersonalizadoC = ResultC.GroupBy(Function(f) f.TextoPerfilFacturacion).SelectMany(Function(grupo) grupo).ToList()
                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ContratoTarifaPersonalizadoC
    End Function

    Public Function GetContratoTarifaExcel(ContratTarifaParam As ContratoTarifa) As ContratoTarifa
        Dim contratoTarifaBD As New ContratoTarifa
        'Dim JoinContrato = String.Join(",", idcontratotarifa)
        Try

            Dim query As String = $"select ct.*
from contratotarifa ct
left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
left join tarifa t  on ct.idtarifa = t.idtarifa
left join contrato c on ct.CodigoContrato = c.CodigoContrato
where ct.idcontratotarifa in (
{ContratTarifaParam.IdContratoTarifa}
) and ct.idtarifagrupo in ({ContratTarifaParam.IdTarifaGrupo})
order by c.CodigoContrato

"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ResultC = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ContratoTarifa)).Cast(Of ContratoTarifa).ToList
                If Not IsNothing(ResultC) AndAlso ResultC.Count > 0 Then
                    For Each r In ResultC
                        If Not r Is Nothing AndAlso r.IdContratoTarifa > 0 Then
                            contratoTarifaBD = r
                        End If
                    Next
                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return contratoTarifaBD
    End Function


    Public Sub AplicarPrecios(CodContrato As Long, FechaVigencia As Date?, Optional IsQ As Boolean = False)
        Try
            Dim ContratoTarifaSrv As New ContratoTarifaSrv(connectionString)
            Dim TarifaPrecioContratoSrv As New TarifaPrecioContratoSrv(connectionString)

            Dim TarifasPrecioContrato = GetDTOAllByCodigosContrato(CodContrato)
            Dim objDatosContratos As New List(Of Contrato)
            Dim IdsContratoTarifa As New List(Of Long)

            'Actualizaba solo los que tenian precio asignado, entonces no aplicaba a nuevos si estos no habian cogido bien el precio de primeras. 

            Dim objContratosTarifa = ContratoTarifaSrv.GetContratoTarifaByCodigoContratoLista(CodContrato)
            IdsContratoTarifa.AddRange(objContratosTarifa.
    Where(Function(f) f.FechaHasta Is Nothing).
    Select(Function(f) f.IdContratoTarifa).ToList())


            If IsNothing(IdsContratoTarifa) Then 'Dejo de todas formas lo de buscar por tarifapreciocontrato por si no encuentra de primeras, abajo ya filtra solo las nuevas. 
                If Not IsNothing(TarifasPrecioContrato) AndAlso TarifasPrecioContrato.Count > 0 Then
                    IdsContratoTarifa.AddRange(TarifasPrecioContrato.Select(Function(f) If(f.IdContratoTarifa, 0L)).Distinct.ToList)
                End If
            End If

            For Each idcontratoT In IdsContratoTarifa
                Dim PreciosOriginales = New List(Of TarifaPrecioContrato)(TarifasPrecioContrato.Where(Function(f) If(f.IdContratoTarifa = idcontratoT, False)))
                Try
                    Dim objContratoTarifa As ContratoTarifa = ContratoTarifaSrv.GetContratoTarifaByIdContratotarifa(idcontratoT)
                    Dim Contrato = GetContrato(objContratoTarifa.CodigoContrato)
                    'Dim fechacontrato = objDatosContratos.Select(Function(f) f.IdContratoTarifa).Distinct.ToList
                    Dim ContratoTarifaEnFechas As Boolean = If(objContratoTarifa.FechaDesde >= Contrato.FechaAlta, False)
                    If ContratoTarifaEnFechas = True Then
                        Dim TarPrecioContrato = TarifaPrecioContratoSrv.ActualizarPreciosVigentes(idcontratoT, PreciosOriginales, If(FechaVigencia, Date.MinValue))
                    End If
                    'Fin de comprobacion 351
                Catch ex As Exception
                    'Dim ContratoTarifa = objContratoTarifaSrv.GetDTO(ID)
                    'ContratosErroneos.Add(If(ContratoTarifa.CodigoContrato, 0L))
                    Throw
                End Try
            Next

            'If ContratosErroneos.Count > 0 Then
            '    Dim Codigos As String = String.Empty
            '    If (Not IsNothing(ContratosErroneos)) Then
            '        Codigos = String.Join(",", ContratosErroneos.Distinct.ToArray())
            '    End If
            '    SigeMessageBox.SigeShowInformation(String.Format("No se han podido actualizar los precios de los siguientes contratos: {0}", Codigos))
            'End If
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Public Sub aplicapreciosFromEcel(contratotarifaParam As ContratoTarifa)
        Try
            Dim ContratoTarifaSrv As New ContratoTarifaSrv(connectionString)
            Dim TarifaPrecioContratoSrv As New TarifaPrecioContratoSrv(connectionString)
            Try
                Dim TarifasPrecioContrato = GetDTOAllByCodigosContratoFromExcel(contratotarifaParam)
                Dim objContratoTarifa As ContratoTarifa = GetContratoTarifaExcel(contratotarifaParam) ' lo busco de nuevo.. por si acaso
                Dim PreciosOriginales = New List(Of TarifaPrecioContrato)(TarifasPrecioContrato.Where(Function(f) If(f.IdContratoTarifa = objContratoTarifa.IdContratoTarifa, False)))
                Dim TarPrecioContrato = TarifaPrecioContratoSrv.ActualizarPreciosVigentes(objContratoTarifa.IdContratoTarifa, PreciosOriginales, contratotarifaParam.FechaDesde)
            Catch ex As Exception
                Throw
            End Try
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Sub AplicarPreciosV2(CodContrato As Long, FechaVigencia As Date?, IsQ As Boolean, fechaAplicar As Date?)
        Try
            Dim objContratoTarifa = GetContratoTarifabyCodContratoFechaVigencia(CodContrato, FechaVigencia)
            Dim UsarFechaAplicar = False
            ' Opción 1: verificar que tiene valor Y no es la fecha mínima
            If fechaAplicar.HasValue AndAlso fechaAplicar.Value > Date.MinValue Then
                ' fechaAplicar tiene una fecha real
                FechaVigencia = fechaAplicar
            End If

            If Not IsNothing(objContratoTarifa.IdPerfilFacturacion) Then
                objContratoTarifa.PerfilFacturacion = GetPerfilFacturacion(If(objContratoTarifa.IdPerfilFacturacion, 0))

                Dim tarifasPrecioContratoGuardar As New List(Of TarifaPrecioContrato)

                Dim isFijoIndex As Boolean = False
                If objContratoTarifa.PerfilFacturacion.isPerfilIndexado() AndAlso Not IsQ Then
                    isFijoIndex = True
                    If objContratoTarifa.Entorno = "G1" Then
                        ' IndexadoPrecioSrv
                        Dim indexadosPrecios As List(Of IndexadoPrecio) = GetDTOAllPeriodosIndx(objContratoTarifa.IdTarifa, objContratoTarifa.IdTarifaGrupo, FechaVigencia)
                        ''Avisar si no hay precios para grabar
                        If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
                            For Each indexadoPrecio As IndexadoPrecio In indexadosPrecios
                                Dim tarifaPrecioContrato As New TarifaPrecioContrato

                                tarifaPrecioContrato.IdContratoTarifa = objContratoTarifa.IdContratoTarifa
                                tarifaPrecioContrato.IdIndexadoPrecio = indexadoPrecio.IdIndexadoPrecio
                                tarifaPrecioContrato.Entorno = indexadoPrecio.Entorno
                                tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
                            Next
                        End If
                    End If
                ElseIf Not objContratoTarifa.PerfilFacturacion.isPerfilIndexado Then
                    AplicarPrecios(CodContrato, FechaVigencia)
                End If
                If tarifasPrecioContratoGuardar.Count > 0 Then
                    For Each tpc In tarifasPrecioContratoGuardar
                        InsertTarifaPrecioContrato(tpc.Entorno, tpc.IdContratoTarifa, tpc.IdTarifaPrecio, tpc.IdIndexadoPrecio, If(tpc.IdIndexadoPrecioGas, 0))
                    Next
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Function InsertTarifaPrecioContrato(entorno As String, idcontratotarifa As Long, idtarifaprecio As Long, idindexadoprecio As Long, idindexadopreciogas As Long) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            Dim query = $"INSERT INTO [dbo].[TarifaPrecioContrato] 
    ([Entorno], [IdContratoTarifa], [IdTarifaPrecio], [IdIndexadoPrecio], [IdIndexadoPrecioGas]) 
VALUES 
    ('{entorno}', {idcontratotarifa}, {idtarifaprecio}, {idindexadoprecio}, {idindexadopreciogas})"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function

    Public Function GetDTOAllByCodigosContrato(CodigosContrato As Long) As List(Of TarifaPrecioContrato)
        Try
            Dim ret As New List(Of TarifaPrecioContrato)

            Dim query As String = $"select *
	from TarifaPrecioContrato
	where IdContratoTarifa in (
		select IdContratoTarifa
		from ContratoTarifa where codigocontrato in (
{CodigosContrato}))

"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ResultC = Helper.FillObjectFromDatatable(result.Tables(0), GetType(TarifaPrecioContrato)).Cast(Of TarifaPrecioContrato).ToList
                If Not IsNothing(ResultC) AndAlso ResultC.Count > 0 Then
                    ret.AddRange(ResultC)
                End If
            End If
            Return ret
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Function GetDTOAllByCodigosContratoFromExcel(contratotarifaparam As ContratoTarifa) As List(Of TarifaPrecioContrato)
        Try
            Dim ret As New List(Of TarifaPrecioContrato)

            Dim query As String = $"select *
	from TarifaPrecioContrato
	where IdContratoTarifa in (
		select IdContratoTarifa
		from ContratoTarifa where codigocontrato in (
{contratotarifaparam.CodigoContrato}) and idtarifagrupo in ({contratotarifaparam.IdTarifaGrupo})
)

"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ResultC = Helper.FillObjectFromDatatable(result.Tables(0), GetType(TarifaPrecioContrato)).Cast(Of TarifaPrecioContrato).ToList
                If Not IsNothing(ResultC) AndAlso ResultC.Count > 0 Then
                    ret.AddRange(ResultC)
                End If
            End If
            Return ret
        Catch ex As Exception
            Throw
        End Try
    End Function


    Public Function VerificarLicitacion(CodigosContrato As List(Of Long)) As List(Of Contrato)
        Try
            Dim ret As New List(Of Contrato)
            Dim CodJoin = String.Join(",", CodigosContrato)
            Dim query As String = $"select codigocontrato  from Contrato where IsLicitacion=1 and CodigoContrato in ({CodJoin})"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ResultC = Helper.FillObjectFromDatatable(result.Tables(0), GetType(Contrato)).Cast(Of Contrato).ToList
                If Not IsNothing(ResultC) AndAlso ResultC.Count > 0 Then
                    ret.AddRange(ResultC)
                End If
            End If
            Return ret
        Catch ex As Exception
            Throw
        End Try
    End Function


    Public Function InsertTarifaGrupoCalendario(Entorno As String, Codigocontrato As Long, IdTarifaGrupo As Long, idtarifa As Long, idperfilfacturacion As Long, fechaDesdeAplicar As Date) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            Dim query = $"INSERT INTO [dbo].[ContratoTarifa] 
    ([Entorno], [CodigoContrato], [IdTarifa], [IdTarifaGrupo], 
     [IdPerfilFacturacion], [FechaDesde], [FechaHasta], [Aviso], 
     [IdContratoTarifaOld], [IsAjusteCAPGas], [FechaRegistroEntraEnRango]) 
VALUES 
    ('{Entorno}', {Codigocontrato}, {idtarifa}, {IdTarifaGrupo}, 
     {idperfilfacturacion}, '{fechaDesdeAplicar}', null, null, 
     null,1, null)"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()


        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function

    Public Function GetCalendarioNuevoTarifa(ListaIdContratoTarifa As Long, TGVIEJO As String, TGNUEVO As String, FechaCierre As Date) As TarifaGrupo
        Dim TarifaGrupoRet As New TarifaGrupo
        Try
            Dim query As String = $"; with GrupoViejo as ( select ct.entorno,idcontratotarifa,ct.idtarifa idtarifaOld,tg.idtarifagrupo idtarifagrupoOld, TextoTarifaGrupo textoGrupoOld, ct.idperfilfacturacion idperfilfacturacionOld from ContratoTarifa ct
inner join tarifagrupo  tg on ct.idtarifagrupo = tg.idtarifagrupo
where idcontratotarifa ={ListaIdContratoTarifa})

select tgN.entorno,tgN.idtarifagrupo,tgN.idtarifa,grupoviejo.idperfilfacturacionold idperfilfacturacion,tgn.IdPerfilFacturacion idperfilfacturacionoNuevo   from TarifaGrupo tgN
inner join GrupoViejo on tgN.idtarifa = GrupoViejo.idtarifaold and textotarifagrupo = replace(GrupoViejo.textoGrupoOld,'{TGVIEJO}','{TGNUEVO}')" 'CAM
            'inner join GrupoViejo on tgN.idtarifa = GrupoViejo.idtarifaold and textotarifagrupo = replace(GrupoViejo.textoGrupoOld,'2024','2025') SUEZ
            'inner join GrupoViejo on tgN.idtarifa = GrupoViejo.idtarifaold and textotarifagrupo = replace(GrupoViejo.textoGrupoOld,'MADRID','MADRID 2025') CAM
            'tgn.entorno,idtarifa,textotarifagrupo textotarifagrupoNuevo,tgn.idperfilfacturacion, textoGrupoOld, idperfilfacturacionOld
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim TGBBDD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(TarifaGrupo)).Cast(Of TarifaGrupo).FirstOrDefault
                If Not IsNothing(TGBBDD) AndAlso TGBBDD.IdTarifaGrupo > 0 Then
                    TarifaGrupoRet = TGBBDD
                    'Cierre el Antiguo Calendario
                    UpdateSetFechaCierreCalendario(ListaIdContratoTarifa, FechaCierre)
                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return TarifaGrupoRet
    End Function


    Public Function GetOnlyCodigoContratobyIdContratoTarifa(idContratoTarifa As Long) As Long
        Dim CodContrato As New Long
        Try
            Dim query As String = $"select codigocontrato from Contratotarifa where idcontratotarifa ={idContratoTarifa} group by codigocontrato"
            'tgn.entorno,idtarifa,textotarifagrupo textotarifagrupoNuevo,tgn.idperfilfacturacion, textoGrupoOld, idperfilfacturacionOld
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim TGBBDD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ContratoTarifa)).Cast(Of ContratoTarifa).FirstOrDefault
                If Not IsNothing(TGBBDD) AndAlso TGBBDD.CodigoContrato > 0 Then
                    CodContrato = TGBBDD.CodigoContrato

                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return CodContrato
    End Function
    Public Function GetContratoTarifabyCodContratoFechaVigencia(codigocontrato As Long, fechaBuscar As Date) As ContratoTarifa
        Dim ContratoTarifaBD As New ContratoTarifa
        Try
            Dim query As String = $"select * from ContratoTarifa where CodigoContrato={codigocontrato} and fechadesde >='{fechaBuscar}'  and FechaHasta is null"
            'tgn.entorno,idtarifa,textotarifagrupo textotarifagrupoNuevo,tgn.idperfilfacturacion, textoGrupoOld, idperfilfacturacionOld
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim TGBBDD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ContratoTarifa)).Cast(Of ContratoTarifa).FirstOrDefault
                If Not IsNothing(TGBBDD) AndAlso TGBBDD.CodigoContrato > 0 Then
                    ContratoTarifaBD = TGBBDD

                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ContratoTarifaBD
    End Function

    Public Function GetContratoTarifabyCodContrato(codigocontrato As Long, tarifagrupo As String) As ContratoTarifa
        Dim ContratoTarifaBD As New ContratoTarifa
        Try
            Dim query As String = $" select ct.* from ContratoTarifa ct
inner join tarifagrupo  tg on ct.idtarifagrupo = tg.idtarifagrupo
where codigocontrato = {codigocontrato} and TextoTarifaGrupo='{tarifagrupo}' and FechaHasta is null"
            'tgn.entorno,idtarifa,textotarifagrupo textotarifagrupoNuevo,tgn.idperfilfacturacion, textoGrupoOld, idperfilfacturacionOld
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim TGBBDD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ContratoTarifa)).Cast(Of ContratoTarifa).FirstOrDefault
                If Not IsNothing(TGBBDD) AndAlso TGBBDD.CodigoContrato > 0 Then
                    ContratoTarifaBD = TGBBDD

                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ContratoTarifaBD
    End Function

    Public Function GetContratoSituacion() As List(Of ContratoSituacion)
        Dim SituacionesContratos As New List(Of ContratoSituacion)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"SELECT * FROM ContratoSituacion"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaContratosSituaciones = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ContratoSituacion)).Cast(Of ContratoSituacion).ToList
                If Not IsNothing(ListaContratosSituaciones) AndAlso ListaContratosSituaciones.Count > 0 Then
                    SituacionesContratos = ListaContratosSituaciones

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return SituacionesContratos
    End Function

    Public Function GetModelosImpresion() As List(Of ModeloDeImpresion)
        Dim ModelosFacs As New List(Of ModeloDeImpresion)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select idmodelodeimpresion, Entorno,DescripcionModeloDeImpresion, CodigoTipoModeloDeImpresion,RptFileName from ModeloDeImpresion where CodigoTipoModeloDeImpresion  in (1,9,4)"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaModelosFacs = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ModeloDeImpresion)).Cast(Of ModeloDeImpresion).ToList
                If Not IsNothing(ListaModelosFacs) AndAlso ListaModelosFacs.Count > 0 Then
                    ModelosFacs = ListaModelosFacs

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ModelosFacs
    End Function
    Public Function GetAllModelosImpresion() As List(Of ModeloDeImpresion)
        Dim ModelosFacs As New List(Of ModeloDeImpresion)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select idmodelodeimpresion, Entorno,DescripcionModeloDeImpresion, CodigoTipoModeloDeImpresion,RptFileName, classname from ModeloDeImpresion"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaModelosFacs = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ModeloDeImpresion)).Cast(Of ModeloDeImpresion).ToList
                If Not IsNothing(ListaModelosFacs) AndAlso ListaModelosFacs.Count > 0 Then
                    ModelosFacs = ListaModelosFacs

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ModelosFacs
    End Function

    Public Function GetModeloImpreisonYbinario(idmodelodeimpresion As Long) As ModeloDeImpresion
        Dim ModelosRe As New ModeloDeImpresion
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select idmodelodeimpresion, Entorno,DescripcionModeloDeImpresion, CodigoTipoModeloDeImpresion,RptFileName, classname,Modelo from ModeloDeImpresion where idmodelodeimpresion={idmodelodeimpresion}"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaModelosFacs = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ModeloDeImpresion)).Cast(Of ModeloDeImpresion).ToList
                If Not IsNothing(ListaModelosFacs) AndAlso ListaModelosFacs.Count > 0 Then
                    ModelosRe = ListaModelosFacs.FirstOrDefault

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ModelosRe
    End Function


    Public Function GetCNAE() As List(Of CNAE)
        Dim CANES As New List(Of CNAE)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select * from cnae"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaCNAE = Helper.FillObjectFromDatatable(result.Tables(0), GetType(CNAE)).Cast(Of CNAE).ToList
                If Not IsNothing(ListaCNAE) AndAlso ListaCNAE.Count > 0 Then
                    CANES = ListaCNAE

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return CANES
    End Function

    Public Function GetColectivos() As List(Of Colectivo)
        Dim Colectivos As New List(Of Colectivo)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select * from Colectivo"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaColectivos = Helper.FillObjectFromDatatable(result.Tables(0), GetType(Colectivo)).Cast(Of Colectivo).ToList
                If Not IsNothing(ListaColectivos) AndAlso ListaColectivos.Count > 0 Then
                    Colectivos = ListaColectivos

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return Colectivos
    End Function

    Public Function UpdateContratosMasivo(QueryContratos As String, CodigosContratos As List(Of Long)) As Long
        Dim FilasAfectadas As Long = 0

        Try
            Using conexion As New SqlConnection(connectionString)
                conexion.Open()
                For Each cod In CodigosContratos
                    Dim query = $"{QueryContratos} WHERE codigocontrato = @cod"
                    Using comando As New SqlCommand(query, conexion)
                        comando.Parameters.AddWithValue("@cod", cod)
                        FilasAfectadas += comando.ExecuteNonQuery()
                    End Using
                Next
            End Using
        Catch ex As Exception
            Console.WriteLine("Error en UpdateContratosMasivo: " & ex.Message)
        End Try

        Return FilasAfectadas
    End Function


    Public Function GetSituacionesScoring() As List(Of SituacionScoring)
        Dim SituacionScoring As New List(Of SituacionScoring)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"SELECT * FROM SituacionScoring"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaSituacionScoring = Helper.FillObjectFromDatatable(result.Tables(0), GetType(SituacionScoring)).Cast(Of SituacionScoring).ToList
                If Not IsNothing(ListaSituacionScoring) AndAlso ListaSituacionScoring.Count > 0 Then
                    SituacionScoring = ListaSituacionScoring

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return SituacionScoring
    End Function

    Public Function GetClientePago(identidad As String) As List(Of ClientePago)
        Dim ClientePago As New List(Of ClientePago)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select idclientepago, cp.idcliente,NombreP,IdentidadPago,TextoColectivo,TextoTipoCobro,IBAN,TextoBanco
,isnull(NombreP,'???')+'/'+isnull(IdentidadPago,'???')+'/'+isnull(TextoColectivo,'???')+'/'+isnull(TextoTipoCobro,'???')+'/'+isnull(IBAN,'???')+'/'+isnull(TextoBanco,'???') ClientePagoUnificado
from clientepago cp
left join tipocobro tc on cp.idtipocobro = tc.idtipocobro
left join Colectivo c on cp.IdColectivo=c.IdColectivo
left join Banco b on cp.IdBanco=b.IdBanco
left join cliente cl on cp.idcliente= cl.idcliente
where cl.Identidad='{identidad}'"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaClientePago = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ClientePago)).Cast(Of ClientePago).ToList
                If Not IsNothing(ListaClientePago) AndAlso ListaClientePago.Count > 0 Then
                    ClientePago = ListaClientePago

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ClientePago
    End Function

    Public Function GetTiposAutoconsumos() As List(Of TiposAutoconsumo)
        Dim ListaTiposAutoconsumos As New List(Of TiposAutoconsumo)
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select * from TiposAutoconsumo where Entorno='U'"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaTiposAutoconsumo = Helper.FillObjectFromDatatable(result.Tables(0), GetType(TiposAutoconsumo)).Cast(Of TiposAutoconsumo).ToList
                If Not IsNothing(ListaTiposAutoconsumo) AndAlso ListaTiposAutoconsumo.Count > 0 Then
                    ListaTiposAutoconsumos = ListaTiposAutoconsumo

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ListaTiposAutoconsumos
    End Function

    Public Function GetClientebyFac(Fac As String) As ClienteBasic
        Dim ClienteB As New ClienteBasic
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select 
identidad 
,dbo.formateardenominacion(nombre,apellido1,Apellido2, RazonSocial) denominacion
from facturaventacabecera fv
inner join cliente cl on fv.idcliente = cl.idcliente
where serienumfactura='{Fac}'"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ClienteBBBDD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ClienteBasic)).Cast(Of ClienteBasic).ToList
                If Not IsNothing(ClienteBBBDD) AndAlso ClienteBBBDD.Count > 0 Then
                    For Each B In ClienteBBBDD
                        If Not String.IsNullOrEmpty(B.Identidad) Then
                            ClienteB = B
                        End If
                    Next

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ClienteB
    End Function
    Public Function GetNumPedidoFacturacionbyFac(Fac As String) As ClienteBasic
        Dim ClienteB As New ClienteBasic
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select 
identidad 
,dbo.formateardenominacion(nombre,apellido1,Apellido2, RazonSocial) denominacion,
NumPedidoFacturacion
from facturaventacabecera fv
inner join cliente cl on fv.idcliente = cl.idcliente
inner join contrato c on fv.codigocontrato= c.codigocontrato
inner join cups on c.idcups = cups.IdCups
where serienumfactura='{Fac}'"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ClienteBBBDD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ClienteBasic)).Cast(Of ClienteBasic).ToList
                If Not IsNothing(ClienteBBBDD) AndAlso ClienteBBBDD.Count > 0 Then
                    For Each B In ClienteBBBDD
                        If Not String.IsNullOrEmpty(B.NumPedidoFacturacion) Then
                            ClienteB = B
                        End If
                    Next

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ClienteB
    End Function


    Public Function UpdateMarcarPerfilarLectura(QueryFacturasATR As String) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try
            Dim IdsLectura = EjecutarConsultaFacturasATR(QueryFacturasATR)
            conexion.Open()
            Dim query = $"update lectura set Perfilar = 1 where IdLectura in ({String.Join(",", IdsLectura)})"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function


    Public Function EjecutarConsultaFacturasATR(QueryFacturasATR As String) As List(Of Long)
        Dim conexion = New SqlConnection(connectionString)
        Dim listaIdLecturas As New List(Of Long)

        Try
            conexion.Open()
            Dim comando = New SqlCommand(QueryFacturasATR, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()

            ' Leer todas las filas
            While readerQuery.Read()
                If Not readerQuery.IsDBNull(readerQuery.GetOrdinal("idlectura")) Then
                    Dim idlectura = CLng(readerQuery("idlectura"))
                    If idlectura > 0 Then
                        listaIdLecturas.Add(idlectura)
                    End If
                End If
            End While

            readerQuery.Close()
            conexion.Close()

        Catch ex As Exception
            Console.WriteLine(ex)
        End Try

        Return listaIdLecturas
    End Function

    Public Function UsuarioValidacion(Login As String, Password As String, ByRef IsLoginReport As Boolean) As UsuarioValidacion
        Dim user As UsuarioValidacion
        'Dim ListaContratov2 As New List(Of Integer)
        Try
            If Password.Equals("ReportSige") AndAlso Login.Equals("ReportSige") Then
                Dim user2 As New UsuarioValidacion
                IsLoginReport = True
                user2.Nombre = "Report"
                user2.login = "REPORT SIGE"
                user2.Password = Password
                user = user2
                Return user
            End If


            Dim query As String = $"select nombre,login,Password from usuario where login='{Login}' and Password='{Password}'"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ListaUsuarioValidacion = Helper.FillObjectFromDatatable(result.Tables(0), GetType(UsuarioValidacion)).Cast(Of UsuarioValidacion).ToList
                If Not IsNothing(ListaUsuarioValidacion) AndAlso ListaUsuarioValidacion.Count > 0 Then
                    user = ListaUsuarioValidacion.Where(Function(f) f.Nombre.Length > 1).FirstOrDefault

                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return user
    End Function

    Public Function GetModeloBinario(idmodelodeimpresion As Long) As ModeloDeImpresion
        Dim ModeloDeImpresionBin As New ModeloDeImpresion
        'Dim ListaContratov2 As New List(Of Integer)
        Try

            Dim query As String = $"select idmodelodeimpresion,modelo from modelodeimpresion where idmodelodeimpresion ={idmodelodeimpresion}"
            Dim result = Helper.QuerySelect(query, connectionString)
            Dim errores = Helper.GetError(result)
            If errores.HasError Then
                'Escribir errores en un log'
            Else
                Dim ModeloBD = Helper.FillObjectFromDatatable(result.Tables(0), GetType(ModeloDeImpresion)).Cast(Of ModeloDeImpresion).FirstOrDefault
                If Not IsNothing(ModeloBD) AndAlso ModeloBD.IdModeloDeImpresion > 0 Then
                    ModeloDeImpresionBin = ModeloBD

                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex)
            Console.WriteLine(ex.StackTrace)
        End Try

        Return ModeloDeImpresionBin
    End Function

    Public Function UpdateModeloImpresion(modelo As ModeloDeImpresion) As Long

        Dim filasAfectadas As Long = 0

        Using conexion As New SqlConnection(connectionString)
            Using comando As New SqlCommand("
            UPDATE modelodeimpresion 
            SET 
                Entorno = @Entorno,
                DescripcionModeloDeImpresion = @Descripcion,
                CodigoTipoModeloDeImpresion = @CodigoTipo,
                ClassName = @ClassName,
                RptFileName = @RptFileName,
                Modelo = @Modelo
            WHERE IdModeloDeImpresion = @Id", conexion)

                comando.Parameters.AddWithValue("@Entorno", modelo.Entorno)
                comando.Parameters.AddWithValue("@Descripcion", modelo.DescripcionModeloDeImpresion)
                comando.Parameters.AddWithValue("@CodigoTipo", modelo.CodigoTipoModeloDeImpresion)
                comando.Parameters.AddWithValue("@ClassName", If(modelo.ClassName, DBNull.Value))
                comando.Parameters.AddWithValue("@RptFileName", If(modelo.RptFileName, DBNull.Value))

                ' BINARIO
                Dim pModelo As New SqlParameter("@Modelo", SqlDbType.VarBinary, -1)
                pModelo.Value = If(modelo.Modelo IsNot Nothing, modelo.Modelo, DBNull.Value)
                comando.Parameters.Add(pModelo)

                comando.Parameters.AddWithValue("@Id", modelo.IdModeloDeImpresion)

                conexion.Open()
                filasAfectadas = comando.ExecuteNonQuery()
            End Using
        End Using

        Return filasAfectadas

    End Function

    Public Function InsertModeloImpresion(modelo As ModeloDeImpresion) As Long

        Dim idInsertado As Long = 0

        Using conexion As New SqlConnection(connectionString)
            Using comando As New SqlCommand("
            INSERT INTO modelodeimpresion
            (
                Entorno,
                DescripcionModeloDeImpresion,
                CodigoTipoModeloDeImpresion,
                ClassName,
                RptFileName,
                Modelo
            )
            VALUES
            (
                @Entorno,
                @Descripcion,
                @CodigoTipo,
                @ClassName,
                @RptFileName,
                @Modelo
            );

            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
        ", conexion)

                comando.Parameters.Add("@Entorno", SqlDbType.VarChar, 10).Value = modelo.Entorno
                comando.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 250).Value = modelo.DescripcionModeloDeImpresion
                comando.Parameters.Add("@CodigoTipo", SqlDbType.Int).Value = modelo.CodigoTipoModeloDeImpresion
                comando.Parameters.Add("@ClassName", SqlDbType.NVarChar, 250).Value = If(modelo.ClassName, DBNull.Value)
                comando.Parameters.Add("@RptFileName", SqlDbType.NVarChar, -1).Value = If(modelo.RptFileName, DBNull.Value)

                ' BINARIO (VARBINARY MAX)
                Dim pModelo As New SqlParameter("@Modelo", SqlDbType.VarBinary, -1)
                pModelo.Value = If(modelo.Modelo IsNot Nothing, modelo.Modelo, DBNull.Value)
                comando.Parameters.Add(pModelo)

                comando.Parameters.Add("@Usuario", SqlDbType.NVarChar, 50).Value =
                Environment.UserName

                conexion.Open()
                idInsertado = CLng(comando.ExecuteScalar())
            End Using
        End Using

        Return idInsertado

    End Function

    Public Function ExisteModeloImpresionPorTipo(codigoTipo As Integer, descripcion As String) As Boolean

        Using conexion As New SqlConnection(connectionString)
            Using comando As New SqlCommand("
            SELECT idmodelodeimpresion
            FROM ModeloDeImpresion
            WHERE CodigoTipoModeloDeImpresion = @CodigoTipo
              AND LEN(modelo) >= 10
              AND DescripcionModeloDeImpresion = @Descripcion
        ", conexion)

                comando.Parameters.Add("@CodigoTipo", SqlDbType.Int).Value = codigoTipo
                comando.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 250).Value = If(descripcion, String.Empty)

                conexion.Open()
                Using reader = comando.ExecuteReader()
                    Return reader.Read()
                End Using
            End Using
        End Using

    End Function

    Public Function UpdateModeloImpresionPorClave(modelo As ModeloDeImpresion) As Long

        Dim filasAfectadas As Long = 0

        Using conexion As New SqlConnection(connectionString)
            Using comando As New SqlCommand("
            UPDATE modelodeimpresion 
            SET 
                ClassName = @ClassName,
                RptFileName = @RptFileName,
                Modelo = @Modelo
            WHERE DescripcionModeloDeImpresion = @Descripcion
              AND Entorno = @Entorno
              AND CodigoTipoModeloDeImpresion = @CodigoTipo", conexion)

                comando.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 250).Value = modelo.DescripcionModeloDeImpresion
                comando.Parameters.Add("@Entorno", SqlDbType.VarChar, 10).Value = modelo.Entorno
                comando.Parameters.Add("@CodigoTipo", SqlDbType.Int).Value = modelo.CodigoTipoModeloDeImpresion
                comando.Parameters.Add("@ClassName", SqlDbType.NVarChar, 250).Value = If(modelo.ClassName, DBNull.Value)
                comando.Parameters.Add("@RptFileName", SqlDbType.NVarChar, -1).Value = If(modelo.RptFileName, DBNull.Value)

                Dim pModelo As New SqlParameter("@Modelo", SqlDbType.VarBinary, -1)
                pModelo.Value = If(modelo.Modelo IsNot Nothing, modelo.Modelo, DBNull.Value)
                comando.Parameters.Add(pModelo)

                conexion.Open()
                filasAfectadas = comando.ExecuteNonQuery()
            End Using
        End Using

        Return filasAfectadas

    End Function

    Public Function DeleteModeloImpresion(idModelo As Long) As Long

        Dim filasAfectadas As Long = 0

        Using conexion As New SqlConnection(connectionString)
            Using comando As New SqlCommand("
            DELETE FROM modelodeimpresion
            WHERE IdModeloDeImpresion = @Id
        ", conexion)

                comando.Parameters.Add("@Id", SqlDbType.BigInt).Value = idModelo

                conexion.Open()
                filasAfectadas = comando.ExecuteNonQuery()
            End Using
        End Using

        Return filasAfectadas
    End Function

    Public Function UpdateSetFechaCierreCalendario(idContratoTarifa As Long, fechaCierre As Date?) As Long

        Dim filasAfectadas As Long = 0

        Using conexion As New SqlConnection(connectionString)
            Using comando As New SqlCommand("
            UPDATE ContratoTarifa
            SET fechahasta = @FechaCierre
            WHERE idContratoTarifa = @Id", conexion)

                comando.Parameters.Add("@Id", SqlDbType.BigInt).Value = idContratoTarifa

                Dim paramFecha = comando.Parameters.Add("@FechaCierre", SqlDbType.DateTime2)

                If fechaCierre.HasValue Then
                    paramFecha.Value = fechaCierre.Value
                Else
                    paramFecha.Value = DBNull.Value
                End If

                conexion.Open()
                filasAfectadas = comando.ExecuteNonQuery()
            End Using
        End Using

        Return filasAfectadas

    End Function


#Region "Controlar valores excel"
    Public Function ToNullableDate(value As Object) As Date?
        If value Is Nothing OrElse String.IsNullOrWhiteSpace(value.ToString) Then
            Return Nothing
        End If
        Dim result As Date
        If Date.TryParse(value.ToString, result) Then
            Return result
        End If
        Return Nothing
    End Function

    Public Function ToNullableDecimal(value As Object) As Decimal?
        If value Is Nothing OrElse String.IsNullOrWhiteSpace(value.ToString) Then
            Return Nothing
        End If
        Dim result As Decimal
        If Decimal.TryParse(value.ToString, result) Then
            Return result
        End If
        Return Nothing
    End Function

    Public Function ToNullableInteger(value As Object) As Integer?
        If value Is Nothing OrElse String.IsNullOrWhiteSpace(value.ToString) Then
            Return Nothing
        End If
        Dim result As Integer
        If Integer.TryParse(value.ToString, result) Then
            Return result
        End If
        Return Nothing
    End Function

    Public Function ToNullableBoolean(value As Object) As Boolean?
        If value Is Nothing OrElse String.IsNullOrWhiteSpace(value.ToString) Then
            Return Nothing
        End If
        Dim result As Boolean
        If Boolean.TryParse(value.ToString, result) Then
            Return result
        End If
        ' Si vienen valores tipo "0/1", "S/N", etc. puedes mapearlos manualmente aquí
        Return Nothing
    End Function

#End Region
End Class

