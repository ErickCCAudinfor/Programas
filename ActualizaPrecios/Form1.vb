Imports System.Collections.ObjectModel
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Microsoft.VisualBasic.Logging

Public Class Form1

    Private ReadOnly Property ipDB As String = "data source=172.31.100.50;"
    'Private ReadOnly Property ipDB As String = "data source=172.31.100.50\TOTALUAT;"
    Private ReadOnly Property nameDB As String = "initial catalog=SigeTotalUAT;"
    'Private ReadOnly Property nameDB As String = "initial catalog=SigeTotalUAT;"
    Private ReadOnly Property userDB As String = "User ID=Sige;"
    Private ReadOnly Property passDB As String = "Password=SigeNew;"

    Private ContratoTarifaSrv As New ContratoTarifaSrv
    Private Funciones As New FuncionesGenericas
    Private Sub Actualizar(sender As Object, e As EventArgs) Handles Button1.Click

        Try
            Dim totalContratos = 0
            Dim ContratoActualizar As New List(Of Long)
            Dim Con As New List(Of Long)
            Dim contratosTexto As String = TextBox2.Text

            'If contratosTexto.Trim.Length > 0 AndAlso TextBox1.Text.Trim.Length > 0 Then
            ' Separar la cadena en una matriz de cadenas utilizando la coma como delimitador
            Dim contratosSeparados As String() = contratosTexto.Split(","c)
            'Convertir los contratos separados a Longs y agregarlos a una lista

            For Each contratoTexto As String In contratosSeparados
                Dim codigosCon As Long
                If Long.TryParse(contratoTexto.Trim(), codigosCon) Then
                    Con.Add(codigosCon)
                End If
            Next

            If CheckBox1.Checked Then 'CUPS
                Dim Cups As New List(Of String)
                'Cups.Add("ES0027700038574004TJ")
                'Contratos = Funciones.BuscarbyCups(Cups, ipDB, nameDB, userDB, passDB)

            End If
            If CheckBox2.Checked Then 'Contrato
                ContratoActualizar = Funciones.BuscarbyCodigocontrato(Con, ipDB, nameDB, userDB, passDB)
                totalContratos = Con.Count
            End If
            If CheckBox3.Checked Then 'Cliente

            End If
            Dim yesorNot As MsgBoxResult
            Dim todoOK As Boolean = False
            If totalContratos <> ContratoActualizar.Count Then
                yesorNot = MsgBox("Los contratos filtratos y los contratos encontrados no coinciden. ¿Actualizar de todas formas?", vbYesNo)
            Else
                todoOK = True
            End If
            If (yesorNot = 6 OrElse yesorNot = 1) OrElse todoOK Then
                Dim ContratosTXT = ActualizarRegistros(ContratoActualizar)

                MessageBox.Show($"{ContratosTXT} / {ContratoActualizar.Count} contratos")
            Else
                MessageBox.Show($"Se ha cancelado la actualización")
            End If

            'Else
            '    MessageBox.Show("No hay datos a modificar")
            'End If



        Catch ex As Exception
            MessageBox.Show("Exception: " + ex.Message)
        End Try

    End Sub

    Private Function ActualizarRegistros(ListaCodigo As List(Of Long)) As String
        Dim Contador = 0I
        Dim ContratosSinActualizar = "Contratos sin actualizarse: "
        Try
            Dim ContratoTra As New List(Of ContratoTarifa)
            Dim ContratoTraMergeado As New List(Of ContratoTarifa)

            'Me busco solo contratos que tengan fechaHasta is null y sea personalizada
            For Each cod In ListaCodigo
                ContratoTra.Add(ContratoTarifaSrv.GetContratoTarifaByCodigoContrato(cod, ipDB, nameDB, userDB, passDB))
            Next

            Dim pepe = 1
            ' Dependiendo de si el PerfilFacturacion del ContratoTarifa es indexado,
            ' cargaremos los precios de IndexadoPrecioSrv o TarifaPrecioSrv.
            ' Estos precios serán los mismos que se muestran en el programa de presupuestos,
            ' los vigentes para una Tarifa, TarifaGrupo y fecha concreta.

            ' Guardamos el nuevo ContratoTarifa, le pasamos la tarifagrupo a asignar
            If ListaCodigo.Count = ContratoTra.Count Then

                For Each elment In ContratoTra
                    'ContratoTraMergeado.Add(ContratoTarifaSrv.UpdateContratoTarifa(elment, TextBox1.Text, False, ipDB, nameDB, userDB, passDB))
                    Dim ContratoActualizar = ContratoTarifaSrv.UpdateContratoTarifa(elment, TextBox1.Text, False, ipDB, nameDB, userDB, passDB)
                    'Dim ContratoTarifaViejo = elment
                    If Not IsNothing(ContratoActualizar) AndAlso ContratoActualizar.IdContratoTarifa > 0 Then
                        ' Dependiendo de si el PerfilFacturacion del ContratoTarifa es indexado,
                        ' cargaremos los precios de IndexadoPrecioSrv o TarifaPrecioSrv.
                        ' Estos precios serán los mismos que se muestran en el programa de presupuestos,
                        ' los vigentes para una Tarifa, TarifaGrupo y fecha concreta.

                        ' Guardamos el nuevo ContratoTarifa.

                        Dim Contrato As Contrato = Funciones.GetContrato(If(elment.CodigoContrato, 0L), ipDB, nameDB, userDB, passDB)
                        Dim FechaPresupuesto As DateTime = DateTime.Today 'New DateTime(Now.Year, Now.Month, Now.Day)
                        If Not IsNothing(Contrato) AndAlso Contrato.IdContrato > 0L Then
                            If Not IsNothing(Contrato.FechaAplicacionPrecios) AndAlso Contrato.FechaAplicacionPrecios > DateTime.MinValue Then
                                FechaPresupuesto = If(Contrato.FechaAplicacionPrecios, DateTime.Now)
                            Else
                                If Not IsNothing(Contrato.FechaContrato) AndAlso Contrato.FechaContrato > DateTime.MinValue Then
                                    FechaPresupuesto = If(Contrato.FechaContrato, DateTime.Now)
                                End If
                            End If

                        End If



                        If Not IsNothing(ContratoActualizar) Then
                            ContratoActualizar.PerfilFacturacion = Funciones.GetPerfilFacturacion(If(ContratoActualizar.IdPerfilFacturacion, 0), ipDB, nameDB, userDB, passDB)

                            If Not IsNothing(ContratoActualizar.PerfilFacturacion) Then
                                Dim tarifasPrecioContratoGuardar As New List(Of TarifaPrecioContrato)
                                'Dim TarifaPerdidaCalculadaContratoGuardar As New ObservableCollection(Of TarifaPerdidaCalculadaContratoDTO)

                                If ContratoActualizar.PerfilFacturacion.isPerfilIndexado() Then

                                    If ContratoActualizar.Entorno = "G1" Then

                                        Dim indexadosPrecios As List(Of IndexadoPrecio) = Funciones.GetDTOAllPeriodosIndx(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto, ipDB, nameDB, userDB, passDB).OrderBy(Function(f) f.IdIndexadoPrecio).ToList
                                        ''Avisar si no hay precios para grabar
                                        If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
                                            For Each eleindexadoPrecio As IndexadoPrecio In indexadosPrecios
                                                Dim tarifaPrecioContrato As New TarifaPrecioContrato

                                                tarifaPrecioContrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
                                                tarifaPrecioContrato.IdIndexadoPrecio = eleindexadoPrecio.IdIndexadoPrecio
                                                tarifaPrecioContrato.TextoTarifaPeriodo = eleindexadoPrecio.TextoTarifaPeriodo
                                                tarifaPrecioContrato.IdTarifaPeriodo = eleindexadoPrecio.IdTarifaPeriodo
                                                tarifaPrecioContrato.Entorno = eleindexadoPrecio.Entorno
                                                tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
                                            Next
                                        Else
                                            ''Avisar si no hay precios para grabar
                                            'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_TARIFA_PRECIO_VIGENTE"))
                                        End If

                                        ''Tarifa Perdida Calculada Contrato
                                        'Dim TPCLista As List(Of TarifaPerdidaCalculadaDTO) = objTarifaPerdidaCalculadaSrv.GetDTOAllPeriodos(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto)
                                        '''Avisar si no hay precios para grabar
                                        'If Not IsNothing(TPCLista) AndAlso TPCLista.Count > 0 Then
                                        '    For Each TPC As TarifaPerdidaCalculadaDTO In TPCLista

                                        '        Dim TPCContrato As New TarifaPerdidaCalculadaContratoDTO

                                        '        TPCContrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
                                        '        TPCContrato.IdTarifaPerdidaCalculada = TPC.IdTarifaPerdidaCalculada

                                        '        TarifaPerdidaCalculadaContratoGuardar.Add(TPCContrato)
                                        '    Next
                                        'Else
                                        '    'TODO Preguntar Carlos si interrumpir proceso o no
                                        '    'Throw New Exception(String.Format(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "TarifaPrecioContratoSrv_FALTA_PERDIDA_CALCULADA_VIGENTE"), ContratoActualizar.CodigoContrato))
                                        'End If
                                    Else
                                        Dim indexadosPrecios As List(Of IndexadoPrecioGas) = Funciones.GetDTOAllPeriodosIndxGas(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto, ipDB, nameDB, userDB, passDB).OrderBy(Function(f) f.IdIndexadoPrecioGas).ToList
                                        ''Avisar si no hay precios para grabar
                                        If Not IsNothing(indexadosPrecios) AndAlso indexadosPrecios.Count > 0 Then
                                            For Each indexadoPrecio As IndexadoPrecioGas In indexadosPrecios
                                                Dim tarifaPrecioContrato As New TarifaPrecioContrato

                                                tarifaPrecioContrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
                                                tarifaPrecioContrato.IdIndexadoPrecioGas = indexadoPrecio.IdIndexadoPrecioGas
                                                tarifaPrecioContrato.IdTarifaPeriodo = indexadoPrecio.IdTarifaPeriodo
                                                tarifaPrecioContrato.TextoTarifaPeriodo = indexadoPrecio.TextoTarifaPeriodo
                                                tarifaPrecioContrato.Entorno = indexadoPrecio.Entorno
                                                tarifasPrecioContratoGuardar.Add(tarifaPrecioContrato)
                                            Next
                                        End If
                                    End If

                                Else
                                    Dim tarifasPrecios As List(Of TarifaPrecio) = Funciones.GetDTOAllPeriodosTarifaPrecio(ContratoActualizar.IdTarifa, ContratoActualizar.IdTarifaGrupo, FechaPresupuesto, ipDB, nameDB, userDB, passDB).OrderBy(Function(f) f.IdTarifaPrecio).ToList

                                    If Not IsNothing(tarifasPrecios) AndAlso tarifasPrecios.Count > 0 Then
                                        For Each tarifaPrecio As TarifaPrecio In tarifasPrecios
                                            Dim tarifapreciocontrato As New TarifaPrecioContrato

                                            tarifapreciocontrato.IdContratoTarifa = ContratoActualizar.IdContratoTarifa
                                            tarifapreciocontrato.IdTarifaPrecio = tarifaPrecio.IdTarifaPrecio
                                            tarifapreciocontrato.IdTarifaPeriodo = tarifaPrecio.IdTarifaPeriodo
                                            tarifapreciocontrato.TextoTarifaPeriodo = tarifaPrecio.TextoTarifaPeriodo
                                            tarifapreciocontrato.Entorno = tarifaPrecio.Entorno
                                            tarifasPrecioContratoGuardar.Add(tarifapreciocontrato)
                                        Next
                                    Else
                                        ''Avisar si no hay precios para grabar
                                        'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_TARIFA_PRECIO_VIGENTE"))
                                    End If

                                End If

                                ' Guardamos todos los registros de TarifaPrecioContrato generados.
                                Dim TarifaPrecioContratoOld = Funciones.GetPrecioContratoTarifa(elment, ipDB, nameDB, userDB, passDB)
                                'Dim Actualizado =
                                'tarifasPrecioContratoGuardar.OrderByDescending(Sub(f) f.IdIndexadoPrecio).ToList
                                Funciones.UpdatePrecioContratoTarifa(tarifasPrecioContratoGuardar, TarifaPrecioContratoOld, ipDB, nameDB, userDB, passDB)
                                'objTarifaPerdidaCalculadaSrv.MergeUpdateTarifaPerdidaCalculadaContratoDTO(TarifaPerdidaCalculadaContratoGuardar, New ObservableCollection(Of TarifaPerdidaCalculadaContratoDTO))
                                'If Actualizado > 0 Then
                                '    Contador = +1
                                'Else
                                '    ContratosSinActualizar += elment.CodigoContrato + " "
                                'End If
                            Else
                                'Throw New Exception(SessionInformationDTO.GetMessage(IdUsuarioEntorno, "ContratoTarifaSrv_FALTA_PERFIL_FACTURACION"))
                            End If
                        End If
                    End If
                Next
            Else
                MessageBox.Show("las listas no coinciden")
            End If

        Catch ex As Exception
            Throw
        End Try
        Return $"Se han actualizado {Contador}. {ContratosSinActualizar}"
    End Function

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            ' Si CheckBox1 está marcado, deshabilitar CheckBox2 y CheckBox3
            CheckBox2.Enabled = False
            CheckBox3.Enabled = False
            TextBox2.Enabled = True
        Else
            ' Si CheckBox1 no está marcado, habilitar CheckBox2 y CheckBox3
            CheckBox2.Enabled = True
            CheckBox3.Enabled = True
            TextBox2.Enabled = False
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        ' Verificar si CheckBox2 está marcado
        If CheckBox2.Checked Then
            ' Si CheckBox2 está marcado, deshabilitar CheckBox1 y CheckBox3
            CheckBox1.Enabled = False
            CheckBox3.Enabled = False
            TextBox2.Enabled = True
        Else
            ' Si CheckBox2 no está marcado, habilitar CheckBox1 y CheckBox3
            CheckBox1.Enabled = True
            CheckBox3.Enabled = True
            TextBox2.Enabled = False
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        ' Verificar si CheckBox3 está marcado
        If CheckBox3.Checked Then
            ' Si CheckBox3 está marcado, deshabilitar CheckBox1 y CheckBox2
            CheckBox1.Enabled = False
            CheckBox2.Enabled = False
            TextBox2.Enabled = True
        Else
            ' Si CheckBox3 no está marcado, habilitar CheckBox1 y CheckBox2
            CheckBox1.Enabled = True
            CheckBox2.Enabled = True
            TextBox2.Enabled = False
        End If
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        If TextBox2.Enabled AndAlso TextBox2.Text.Trim.Length >= 1 Then
            ' Si CheckBox3 está marcado, deshabilitar CheckBox1 y CheckBox2
            TextBox1.Enabled = True
        Else
            ' Si CheckBox3 no está marcado, habilitar CheckBox1 y CheckBox2
            TextBox1.Enabled = False
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim contratosTexto As String = TextBox2.Text
            Dim contratosSeparados As String() = contratosTexto.Split(","c)
            'Convertir los contratos separados a Longs y agregarlos a una lista
            Dim Con As New List(Of Long)
            For Each contratoTexto As String In contratosSeparados
                Dim codigosCon As Long
                If Long.TryParse(contratoTexto.Trim(), codigosCon) Then
                    Con.Add(codigosCon)
                End If
            Next
            Dim Entorno = If(Funciones.GetContrato(Con.FirstOrDefault, ipDB, nameDB, userDB, passDB).Entorno = "E1", "G1", "G2")
            Dim ModiCo As New ProductosAsig(Entorno, ipDB, nameDB, userDB, passDB)
            ModiCo.Show()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Try
            If TextBox1.Text.Trim.Length > 0 Then
                Button1.Enabled = True
            Else
                Button1.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
End Class
