Public Class TarifaPeriodoSrv
    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub


    Public Function GetDTO(IDEntityDTO As Long) As TarifaPeriodo
        Dim ReturnTarifaPeriodor As New TarifaPeriodo
        Try

            Dim Query = $"select * from tarifaperiodo where idtarifaperiodo = {IDEntityDTO}"
            Dim result = Helper.QuerySelect(Query, connectionString)
            Dim errores = Helper.GetError(result)
            If Not errores.HasError Then
                Dim TarifaPeriodore = Helper.FillObjectFromDatatable(result.Tables(0), GetType(TarifaPeriodo)).Cast(Of TarifaPeriodo).FirstOrDefault
                If Not IsNothing(TarifaPeriodore) AndAlso TarifaPeriodore.IdTarifaPeriodo > 0 Then
                    ReturnTarifaPeriodor = TarifaPeriodore
                End If
            End If

        Catch ex As Exception
            Throw
        Finally

        End Try
        Return ReturnTarifaPeriodor
    End Function
End Class
