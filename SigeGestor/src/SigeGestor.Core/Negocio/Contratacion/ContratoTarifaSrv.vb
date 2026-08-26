Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
Imports Microsoft.Data.SqlClient
Imports ActualizaPrecios.FuncionesGenericas
Public Class ContratoTarifaSrv
    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub


    Public Function UpdateContratoTarifa(ContratoTarifa As ContratoTarifa, TarifaGrupoNew As String, TarifaGrupoNoPersonalizada As String, IsTarifaPersonalizada As Boolean) As ContratoTarifa
        Dim conexion = New SqlConnection(connectionString)
        Dim ContratoTarifaAux As New ContratoTarifa
        Dim funciones As New FuncionesGenericas(Me.connectionString)
        Try
            Dim ContratoTarifaBBDD As ContratoTarifa = Nothing

            'Vuelvo a buscar el contratotarifa
            If ContratoTarifa.IDEntityDTO > 0 Then
                ContratoTarifaBBDD = GetContratoTarifaByIdContratotarifa(ContratoTarifa.IdContratoTarifa)
                Dim Tari = funciones.GetTarifaGrupo(TarifaGrupoNew)
                'Si existe hacemos el update
                If IsNothing(Tari) OrElse Tari.Count = 0 Then
                    Throw New Exception($"La tarifa grupo ** {TarifaGrupoNew} ** no existe")
                End If
                'Habria que ver la tarifagrupo sea personalizada, si no F
                '************************************************************************
                'Dim ExisteTarifa = Tari.Where(Function(f) f.IdTarifa = If(ContratoTarifaBBDD.IdTarifa, 0) AndAlso f.IdTarifaGrupo = If(ContratoTarifaBBDD.IdTarifaGrupo, 0)).FirstOrDefault
                'Update Si exsite la tarifa personazliada
                'f ContratoTarifa.textotarifagrupo.Contains("personalizada") Then
                If IsTarifaPersonalizada AndAlso ContratoTarifa.textotarifagrupo.ToLower.Contains("personalizada") Then
                    'Busco solo la tarifagrupop a actualizar
                    Dim TariaBuena = Tari.Where(Function(f) f.IdTarifa = ContratoTarifa.IdTarifa).FirstOrDefault
                    If Not IsNothing(TariaBuena) AndAlso TariaBuena.IdTarifaGrupo > 0 Then
                        ContratoTarifaAux = UpdateContratoTarifaV2(ContratoTarifaBBDD, TariaBuena)
                    Else
                        Throw New Exception($"No se ha encontrato ninguna tarifa para el contrato {ContratoTarifaBBDD.CodigoContrato} ")
                    End If
                ElseIf IsTarifaPersonalizada = False AndAlso ContratoTarifa.textotarifagrupo = TarifaGrupoNoPersonalizada Then
                    'Busco solo la tarifagrupop a actualizar
                    Dim TariaBuena = Tari.Where(Function(f) f.IdTarifa = ContratoTarifa.IdTarifa).FirstOrDefault
                    If Not IsNothing(TariaBuena) AndAlso TariaBuena.IdTarifaGrupo > 0 Then
                        ContratoTarifaAux = UpdateContratoTarifaV2(ContratoTarifaBBDD, TariaBuena)
                    Else
                        Throw New Exception($"No se ha encontrato ninguna tarifa para el contrato {ContratoTarifaBBDD.CodigoContrato} ")
                    End If
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
        Return ContratoTarifaAux
    End Function

    Public Function GetContratoTarifaPersonalizadaByCodigoContrato(Cod As Long, TarifaGrupoNoPersonalizada As String, IsTarifaPersonalizada As Boolean) As ContratoTarifa
        Dim conexion = New SqlConnection(connectionString)
        Dim funciones As New FuncionesGenericas(Me.connectionString)
        Dim ret As New ContratoTarifa
        Dim ContratoTarifaB As New ContratoTarifa
        Dim personalizadaorFija = If(IsTarifaPersonalizada, "like '%personalizada%'", $"= '{TarifaGrupoNoPersonalizada}'")

        Try
            conexion.Open()
            Dim query = $"select ct.*,tg.textotarifagrupo, pf.TextoPerfilFacturacion,t.TextoTarifa from contratotarifa ct
            left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
            left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
            left join tarifa t  on ct.idtarifa = t.idtarifa
            where codigocontrato={Cod} and FechaDesde is not null and fechaHasta is null
            and tg.TextoTarifaGrupo {personalizadaorFija}"
            '    like '%personalizada%'"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            'Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            Do While readerQuery.Read

                'Dim ContratoTarifaB = New ContratoTarifa
                ContratoTarifaB.IdContratoTarifa = funciones.ObtenerValor("IdContratoTarifa", readerQuery)
                ContratoTarifaB.Entorno = funciones.ObtenerValor("Entorno", readerQuery)
                ContratoTarifaB.CodigoContrato = funciones.ObtenerValor("CodigoContrato", readerQuery)
                ContratoTarifaB.IdTarifa = funciones.ObtenerValor("IdTarifa", readerQuery)
                ContratoTarifaB.IdTarifaGrupo = funciones.ObtenerValor("IdTarifaGrupo", readerQuery)
                ContratoTarifaB.IdPerfilFacturacion = funciones.ObtenerValor("IdPerfilFacturacion", readerQuery)
                ContratoTarifaB.FechaDesde = funciones.ObtenerValor("FechaDesde", readerQuery)
                ContratoTarifaB.FechaHasta = funciones.ObtenerValor("FechaHasta", readerQuery)
                ContratoTarifaB.TextoTarifa = funciones.ObtenerValor("TextoTarifa", readerQuery)
                ContratoTarifaB.textotarifagrupo = funciones.ObtenerValor("textotarifagrupo", readerQuery)
                ContratoTarifaB.TextoPerfilFacturacion = funciones.ObtenerValor("TextoPerfilFacturacion", readerQuery)

                'ListaContratoTipo.Add(contratoTipo)
                'ListaContratoTarifa.Add(ContratoTarifaB)
            Loop
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ContratoTarifaB
    End Function


    Public Function GetContratoTarifaByCodigoContrato(Cod As Long) As ContratoTarifa
        Dim conexion = New SqlConnection(connectionString)
        Dim funciones As New FuncionesGenericas(Me.connectionString)
        Dim ret As New ContratoTarifa
        Dim ContratoTarifaB As New ContratoTarifa
        Try
            conexion.Open()
            Dim query = $"select ct.*,tg.textotarifagrupo, pf.TextoPerfilFacturacion,t.TextoTarifa from contratotarifa ct
            left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
            left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
            left join tarifa t  on ct.idtarifa = t.idtarifa
            where codigocontrato={Cod}"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            'Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            Do While readerQuery.Read

                'Dim ContratoTarifaB = New ContratoTarifa
                ContratoTarifaB.IdContratoTarifa = funciones.ObtenerValor("IdContratoTarifa", readerQuery)
                ContratoTarifaB.Entorno = funciones.ObtenerValor("Entorno", readerQuery)
                ContratoTarifaB.CodigoContrato = funciones.ObtenerValor("CodigoContrato", readerQuery)
                ContratoTarifaB.IdTarifa = funciones.ObtenerValor("IdTarifa", readerQuery)
                ContratoTarifaB.IdTarifaGrupo = funciones.ObtenerValor("IdTarifaGrupo", readerQuery)
                ContratoTarifaB.IdPerfilFacturacion = funciones.ObtenerValor("IdPerfilFacturacion", readerQuery)
                ContratoTarifaB.FechaDesde = funciones.ObtenerValor("FechaDesde", readerQuery)
                ContratoTarifaB.FechaHasta = funciones.ObtenerValor("FechaHasta", readerQuery)
                ContratoTarifaB.TextoTarifa = funciones.ObtenerValor("TextoTarifa", readerQuery)
                ContratoTarifaB.textotarifagrupo = funciones.ObtenerValor("textotarifagrupo", readerQuery)
                ContratoTarifaB.TextoPerfilFacturacion = funciones.ObtenerValor("TextoPerfilFacturacion", readerQuery)

                'ListaContratoTipo.Add(contratoTipo)
                'ListaContratoTarifa.Add(ContratoTarifaB)
            Loop
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ContratoTarifaB
    End Function

    Public Function GetContratoTarifaByIdContratotarifa(id As Long) As ContratoTarifa
        Dim conexion = New SqlConnection(connectionString)
        Dim funciones As New FuncionesGenericas(Me.connectionString)
        Dim ret As New ContratoTarifa
        Dim ContratoTarifaB As New ContratoTarifa
        Try
            conexion.Open()
            Dim query = $"select * from contratotarifa where IdContratoTarifa={id}"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            'Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            Do While readerQuery.Read

                ContratoTarifaB = New ContratoTarifa
                ContratoTarifaB.IdContratoTarifa = funciones.ObtenerValor("IdContratoTarifa", readerQuery)
                ContratoTarifaB.Entorno = funciones.ObtenerValor("Entorno", readerQuery)
                ContratoTarifaB.CodigoContrato = funciones.ObtenerValor("CodigoContrato", readerQuery)
                ContratoTarifaB.IdTarifa = funciones.ObtenerValor("IdTarifa", readerQuery)
                ContratoTarifaB.IdTarifaGrupo = funciones.ObtenerValor("IdTarifaGrupo", readerQuery)
                ContratoTarifaB.IdPerfilFacturacion = funciones.ObtenerValor("IdPerfilFacturacion", readerQuery)
                ContratoTarifaB.FechaDesde = funciones.ObtenerValor("FechaDesde", readerQuery)
                ContratoTarifaB.FechaHasta = funciones.ObtenerValor("FechaHasta", readerQuery)

            Loop
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ContratoTarifaB
    End Function

    Public Function InsertContratoTarifa(Cont As ContratoTarifa, TarifaGrupoNueva As TarifaGrupo, ipDB As String, nameDB As String, userDB As String, passDB As String) As ContratoTarifa
        Dim conexion = New SqlConnection(ipDB + nameDB + userDB + passDB)
        Dim ret As New ContratoTarifa
        Dim ContratoTarifaB As New ContratoTarifa
        Try
            conexion.Open()
            Dim query = $"INSERT INTO [dbo].[ContratoTarifa] ([Entorno],[CodigoContrato],[IdTarifa],[IdTarifaGrupo],[IdPerfilFacturacion],[FechaDesde],[FechaHasta],[Aviso],[IdContratoTarifaOld],[IsAjusteCAPGas])
                    VALUES ('{Cont.Entorno}',{Cont.CodigoContrato},{TarifaGrupoNueva.IdTarifa},{TarifaGrupoNueva.IdTarifaGrupo},{TarifaGrupoNueva.IdPerfilFacturacion},'{ If(Cont.FechaDesde, DateTime.Today).AddDays(+1)}',null,null,null,null)"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader
            readerQuery.Close()
            conexion.Close()
            'Modifico el anterior tarifagrupo
            UpdateContratoTarifaViejaFechaHasta(Cont, ipDB, nameDB, userDB, passDB)
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ContratoTarifaB
    End Function




    Public Function UpdateContratoTarifaV2(Cont As ContratoTarifa, TarifaGrupoNueva As TarifaGrupo) As ContratoTarifa
        Dim conexion = New SqlConnection(connectionString)
        Dim ret As New ContratoTarifa
        Dim ContratoTarifaB As New ContratoTarifa
        Try
            conexion.Open()
            Dim query = $"update contratotarifa set IdTarifaGrupo = {TarifaGrupoNueva.IdTarifaGrupo}
                        , IdPerfilFacturacion =  {TarifaGrupoNueva.IdPerfilFacturacion}                          
                        where IdContratoTarifa in ({Cont.IdContratoTarifa})"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader
            readerQuery.Close()
            conexion.Close()
            ContratoTarifaB = GetContratoTarifaByIdContratotarifa(Cont.IdContratoTarifa)
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ContratoTarifaB
    End Function


    Public Sub UpdateContratoTarifaViejaFechaHasta(Cont As ContratoTarifa, ipDB As String, nameDB As String, userDB As String, passDB As String)
        Dim conexion = New SqlConnection(ipDB + nameDB + userDB + passDB)
        Try
            conexion.Open()
            Dim query = $"update contratotarifa set FechaHasta = '{Cont.FechaDesde}'                   
                        where IdContratoTarifa in ({Cont.IdContratoTarifa})"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
    End Sub


    Public Function GetContratoTarifaByCodigoContratoLista(Cod As Long) As List(Of ContratoTarifa)
        Dim conexion = New SqlConnection(connectionString)
        Dim funciones As New FuncionesGenericas(Me.connectionString)
        Dim ret As New ContratoTarifa
        Dim ContratoTarifaB As New List(Of ContratoTarifa)
        Try
            conexion.Open()
            Dim query = $"select ct.*,tg.textotarifagrupo, pf.TextoPerfilFacturacion,t.TextoTarifa from contratotarifa ct
            left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
            left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
            left join tarifa t  on ct.idtarifa = t.idtarifa
            where codigocontrato={Cod}"
            Dim comando = New SqlCommand(query, conexion)
            Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            'Dim readerQuery As SqlDataReader = comando.ExecuteReader()
            Do While readerQuery.Read
                Dim ContratoTarifaA As New ContratoTarifa
                'Dim ContratoTarifaB = New ContratoTarifa
                ContratoTarifaA.IdContratoTarifa = funciones.ObtenerValor("IdContratoTarifa", readerQuery)
                ContratoTarifaA.Entorno = funciones.ObtenerValor("Entorno", readerQuery)
                ContratoTarifaA.CodigoContrato = funciones.ObtenerValor("CodigoContrato", readerQuery)
                ContratoTarifaA.IdTarifa = funciones.ObtenerValor("IdTarifa", readerQuery)
                ContratoTarifaA.IdTarifaGrupo = funciones.ObtenerValor("IdTarifaGrupo", readerQuery)
                ContratoTarifaA.IdPerfilFacturacion = funciones.ObtenerValor("IdPerfilFacturacion", readerQuery)
                ContratoTarifaA.FechaDesde = funciones.ObtenerValor("FechaDesde", readerQuery)
                ContratoTarifaA.FechaHasta = funciones.ObtenerValor("FechaHasta", readerQuery)
                ContratoTarifaA.TextoTarifa = funciones.ObtenerValor("TextoTarifa", readerQuery)
                ContratoTarifaA.textotarifagrupo = funciones.ObtenerValor("textotarifagrupo", readerQuery)
                ContratoTarifaA.TextoPerfilFacturacion = funciones.ObtenerValor("TextoPerfilFacturacion", readerQuery)
                ContratoTarifaB.Add(ContratoTarifaA)
                'ListaContratoTipo.Add(contratoTipo)
                'ListaContratoTarifa.Add(ContratoTarifaB)
            Loop
            readerQuery.Close()
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return ContratoTarifaB
    End Function

    Public Function UpdateContratoTarifaSiError(contratoT As ContratoTarifa)
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As Long
        Try

            conexion.Open()
            Dim query = $"update ContratoTarifa set IdTarifaGrupo={contratoT.IdTarifaGrupo}, IdPerfilFacturacion={contratoT.IdPerfilFacturacion} where IdContratoTarifa={contratoT.IdContratoTarifa}"
            Dim comando = New SqlCommand(query, conexion)
            FilfasAfectadas = comando.ExecuteNonQuery
            conexion.Close()
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function
End Class
