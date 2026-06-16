Imports System.Collections.ObjectModel
Imports System.Data.SqlClient

Public Class TarifaPrecioContratoSrv
    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub


    Public Function ActualizarPreciosVigentes(IdContratoTarifa As Long, TarifasPrecioContratoOriginales As List(Of TarifaPrecioContrato), FechaVigenciaNueva As Date) As List(Of TarifaPrecioContrato)
        Try
            Dim ret As New List(Of TarifaPrecioContrato)

            Dim ContratoTarifaSrv As New ContratoTarifaSrv(connectionString)
            Dim funciones As New FuncionesGenericas(connectionString)
            ' No se actualizan los precios de ls Tarifas TipoQs
            If Not IsNothing(IdContratoTarifa) AndAlso IdContratoTarifa > 0 Then
                Dim objContratoTarifa As ContratoTarifa = ContratoTarifaSrv.GetContratoTarifaByIdContratotarifa(IdContratoTarifa)

                If Not IsNothing(objContratoTarifa) Then 'AndAlso ContratoTarifaEnFechas Then

                    If Not IsNothing(objContratoTarifa.IdPerfilFacturacion) Then
                        objContratoTarifa.PerfilFacturacion = funciones.GetPerfilFacturacion(If(objContratoTarifa.IdPerfilFacturacion, 0))

                        Dim tarifasPrecioContratoGuardar As New List(Of TarifaPrecioContrato)

                        Dim isFijoIndex As Boolean = False
                        If objContratoTarifa.PerfilFacturacion.isPerfilIndexado() Then
                            isFijoIndex = True
                            If objContratoTarifa.Entorno = "G1" Then
                                ' IndexadoPrecioSrv
                                Dim indexadosPrecios As List(Of IndexadoPrecio) = funciones.GetDTOAllPeriodosIndxByFechaFinPresupuesto(objContratoTarifa.Entorno, objContratoTarifa.IdTarifa, objContratoTarifa.IdTarifaGrupo, FechaVigenciaNueva)
                                ''Avisar si no hay precios para grabar
                                If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
                                    For Each indexadoPrecio As IndexadoPrecio In indexadosPrecios
                                        Dim tarifaPrecioContrato As New TarifaPrecioContrato

                                        tarifaPrecioContrato.IdContratoTarifa = objContratoTarifa.IdContratoTarifa
                                        tarifaPrecioContrato.IdIndexadoPrecio = indexadoPrecio.IdIndexadoPrecio
                                        tarifaPrecioContrato.Entorno = objContratoTarifa.Entorno
                                        tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
                                    Next
                                Else
                                    Throw New Exception(String.Format("FALTA_TARIFA_PRECIO_VIGENTE_LUZ_INDEX", objContratoTarifa.CodigoContrato))
                                End If

                            Else
                                Dim indexadosPrecios As List(Of IndexadoPrecioGas) = funciones.GetDTOAllPeriodosIndxGasByFechaFinPresupuesto(objContratoTarifa.Entorno, objContratoTarifa.IdTarifa, objContratoTarifa.IdTarifaGrupo, FechaVigenciaNueva)
                                ''Avisar si no hay precios para grabar
                                If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
                                    For Each indexadoPrecio As IndexadoPrecioGas In indexadosPrecios
                                        Dim tarifaPrecioContrato As New TarifaPrecioContrato

                                        tarifaPrecioContrato.IdContratoTarifa = objContratoTarifa.IdContratoTarifa
                                        tarifaPrecioContrato.IdIndexadoPrecioGas = indexadoPrecio.IdIndexadoPrecioGas
                                        tarifaPrecioContrato.Entorno = objContratoTarifa.Entorno
                                        tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
                                    Next
                                Else
                                    Throw New Exception(String.Format("FALTA_TARIFA_PRECIO_VIGENTE_GAS_INDEX", objContratoTarifa.CodigoContrato))
                                End If
                            End If
                        Else
                            ' TarifaPrecioSrv
                            Dim tarifasPrecios As List(Of TarifaPrecio) = funciones.GetDTOAllPeriodosTarifaPrecioByFechaPresupuesto(objContratoTarifa.Entorno, objContratoTarifa.IdTarifa, objContratoTarifa.IdTarifaGrupo, FechaVigenciaNueva)

                            If Not IsNothing(tarifasPrecios) AndAlso tarifasPrecios.Count > 0 Then
                                For Each tarifaPrecio As TarifaPrecio In tarifasPrecios
                                    Dim tarifapreciocontrato As New TarifaPrecioContrato

                                    tarifapreciocontrato.IdContratoTarifa = objContratoTarifa.IdContratoTarifa
                                    tarifapreciocontrato.IdTarifaPrecio = tarifaPrecio.IdTarifaPrecio
                                    tarifapreciocontrato.Entorno = objContratoTarifa.Entorno
                                    tarifasPrecioContratoGuardar.Add(tarifapreciocontrato)
                                Next
                            Else
                                ''Avisar si no hay precios para grabar
                                Throw New Exception(String.Format("FALTA_TARIFA_PRECIO_VIGENTE", objContratoTarifa.CodigoContrato))
                            End If
                        End If

                        If Not IsNothing(tarifasPrecioContratoGuardar) AndAlso tarifasPrecioContratoGuardar.Count > 0 AndAlso TarifasPrecioContratoOriginales.Count > 0 Then
                            'Aqui hay que guardar la vigencia nueva
                            'Dim Query = $"update contrato set FechaAplicacionPrecios='{FechaVigenciaNueva.Date}' where codigocontrato = {objContratoTarifa.CodigoContrato};"
                            'If Query <> String.Empty Then
                            '    Dim ds As DataSet = Helper.QuerySelect(Query, connectionString)
                            '    Dim a As Integer = 1
                            'End If
                            funciones.UpdatePrecioContratoTarifa(tarifasPrecioContratoGuardar, TarifasPrecioContratoOriginales, isFijoIndex)
                        ElseIf Not IsNothing(tarifasPrecioContratoGuardar) AndAlso tarifasPrecioContratoGuardar.Count > 0 Then
                            InsertPrecioContratoTarifa(tarifasPrecioContratoGuardar, isFijoIndex)
                        End If

                        ret = tarifasPrecioContratoGuardar.ToList
                    Else
                        Throw New Exception("FALTA_PERFIL_FACTURACION")
                    End If
                End If
            End If

            Return ret
        Catch ex As Exception
            Throw
        End Try
    End Function

    Public Function InsertPrecioContratoTarifa(Cont As List(Of TarifaPrecioContrato), IsFijoIndex As Boolean) As Long
        Dim conexion = New SqlConnection(connectionString)

        Dim FilfasAfectadas As New Long
        Try
            Dim numTarifaPrecioContrato As Integer = Cont.Count
            'Dim numTarifaPrecioContratoOLD As Integer = ContOld.Count
            'Dim minNum As Integer = Math.Min(numTarifaPrecioContrato, numTarifaPrecioContratoOLD)
            conexion.Open()
            For Each insertT In Cont
                Dim query = $"INSERT INTO [dbo].[TarifaPrecioContrato]
([Entorno],[IdContratoTarifa],[IdTarifaPrecio],[IdIndexadoPrecio],[IdIndexadoPrecioGas])
     VALUES 
(
'{insertT.Entorno}',
{If(insertT.IdContratoTarifa, 0)},
{insertT.IdTarifaPrecio},
{insertT.IdIndexadoPrecio},
{If(insertT.IdIndexadoPrecioGas, 0)}
)"
                Dim comando = New SqlCommand(query, conexion)
                FilfasAfectadas = comando.ExecuteNonQuery
            Next
            conexion.Close()
            'TarifaPrecioContrato = Cont
        Catch ex As Exception
            Console.WriteLine(ex)
        End Try
        Return FilfasAfectadas
    End Function






End Class
