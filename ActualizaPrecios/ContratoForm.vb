Imports System.Reflection.Emit

Public Class ContratoForm
    Dim complementos As New Complementos()
    Private ReadOnly Property Contratos As List(Of Long)

    Private ReadOnly Property connectionString As String
    Private ReadOnly Property Funciones As FuncionesGenericas

    Private ReadOnly Property NombreUsuarioEquipo As String
    Dim LoadingWF As New LoadingWF

    Public Sub New(Contratos As List(Of Long), connectionString As String, nombreUser As String)
        Try
            InitializeComponent()
            Me.connectionString = connectionString
            Me.NombreUsuarioEquipo = nombreUser
            Me.Contratos = Contratos
            Label2.Text = $"Para realizar una modificación masiva hay que marcar la opción que se desea actualizar en los contratos, es decir, si desea actualizar la situacion del {vbCr}contrato, debes marcar el check situado al lado izquierdo de la opción, posteriormente elegir la situación del contrato y por último darle al botón de actualizar.{vbCr}¡Ojo!, si no se marca el check situado al lado izquierdo la actualización del campo deseado no se llevara  acabo.{vbCr}Cada modificación que se realizara la APP generara una consulta antes y despues, con la finalidad para saber si se ha hecho correctamente la modificacion."
            InicializarCombos()
        Catch ex As Exception
            Throw
        End Try
        ' Esta llamada es exigida por el diseñador.
    End Sub

    Private Sub InicializarCombos()
        Try
            Dim funciones2 As New FuncionesGenericas(Me.connectionString)
            'Situaciones Contratos
            Me.ComboBox1.DataSource = funciones2.GetContratoSituacion()
            Me.ComboBox1.DisplayMember = "TextoSituacion"
            Me.ComboBox1.ValueMember = "IdContratoSituacion"
            'TipoImpuesto
            Me.ComboBox2.DataSource = funciones2.GetTipoImpuestoBy()
            Me.ComboBox2.DisplayMember = "TextoImpuesto"
            Me.ComboBox2.ValueMember = "IdTipoImpuesto"
            Dim ModelosImpresion = funciones2.GetModelosImpresion()
            'Energia
            Me.ComboBox6.DataSource = ModelosImpresion.Where(Function(f) f.CodigoTipoModeloDeImpresion = 1).ToList
            Me.ComboBox6.DisplayMember = "DescripcionModeloDeImpresion"
            Me.ComboBox6.ValueMember = "idmodelodeimpresion"
            'Varios
            Me.ComboBox7.DataSource = ModelosImpresion.Where(Function(f) f.CodigoTipoModeloDeImpresion = 9).ToList
            Me.ComboBox7.DisplayMember = "DescripcionModeloDeImpresion"
            Me.ComboBox7.ValueMember = "idmodelodeimpresion"

            'CNAE
            Me.ComboBox3.DataSource = funciones2.GetCNAE()
            Me.ComboBox3.DisplayMember = "TextoCNAE"
            Me.ComboBox3.ValueMember = "IdCNAE"

            'TipoImpresion
            Me.ComboBox4.DataSource = TipoImpresion()
            Me.ComboBox4.DisplayMember = "TextoTipoImpresion"
            Me.ComboBox4.ValueMember = "Key"
            'ModeloContrato
            Me.ComboBox5.DataSource = ModelosImpresion.Where(Function(f) f.CodigoTipoModeloDeImpresion = 4).ToList
            Me.ComboBox5.DisplayMember = "DescripcionModeloDeImpresion"
            Me.ComboBox5.ValueMember = "idmodelodeimpresion"

            'Colectivos
            Me.ComboBox8.DataSource = funciones2.GetColectivos
            Me.ComboBox8.DisplayMember = "TextoColectivo"
            Me.ComboBox8.ValueMember = "IdColectivo"

            'Colectivos
            Me.ComboBox9.DataSource = funciones2.GetSituacionesScoring
            Me.ComboBox9.DisplayMember = "Nombre"
            Me.ComboBox9.ValueMember = "IdSituacionScoring"

            'TipoAutoconsumo
            Me.ComboTipoAutoconsumo.DataSource = funciones2.GetTiposAutoconsumos
            Me.ComboTipoAutoconsumo.DisplayMember = "TextoAutoconsumo"
            Me.ComboTipoAutoconsumo.ValueMember = "IdTipoAutoconsumo"

        Catch ex As Exception

        End Try
    End Sub

    Private Function TipoImpresion() As List(Of TipoImpresionItem)
        Dim Lista As New List(Of TipoImpresionItem) From {
            New TipoImpresionItem With {.Key = "P", .TextoTipoImpresion = "Papel y Email"},
            New TipoImpresionItem With {.Key = "E", .TextoTipoImpresion = "Email"},
            New TipoImpresionItem With {.Key = "W", .TextoTipoImpresion = "Web"},
            New TipoImpresionItem With {.Key = "Q", .TextoTipoImpresion = "Papel"},
            New TipoImpresionItem With {.Key = "R", .TextoTipoImpresion = "Recibo Bancario"},
            New TipoImpresionItem With {.Key = "F", .TextoTipoImpresion = "FACE"}
        }
        Return Lista
    End Function

    'Actualizamos de forma masiva los contratos
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim rutaCarpeta = $"C:\Users\{NombreUsuarioEquipo}\Desktop\ConsultasBO\LogContratos"
            Dim JoinContratos = String.Join(",", Contratos)
            Dim UpdateContrato = "Update contrato set"
            Dim WhereContrato = $"Where codigocontrato in({JoinContratos})"
            Dim ContratosModificados = 0
            ' Lista para las tareas
            Dim tasks As New List(Of Task)
            Dim ValoresF = GetParameters()

            If ValoresF.Length > 0 Then

                Dim yesorNot1 = MsgBox($"Hay un total de {Contratos.Count} contratos, ¿Seguir con la actualización?", vbYesNo)
                If yesorNot1 <> 6 Then
                    MsgBox("operación cancelada")
                    Exit Sub
                End If

                ''Antes de la actualizacion
                Dim rutaArchivoAntesModificacion = IO.Path.Combine(rutaCarpeta, $"ContratosAntesActualizacion_{Now.ToString("ddMMyyyy_HHmmss")}.xlsx")
                    Dim rutaArchivoDespuesModificacion = IO.Path.Combine(rutaCarpeta, $"ContratosDespuesActualizacion_{Now.ToString("ddMMyyyy_HHmmss")}.xlsx")
                    'Obtengo la consulta antes de la modificacion
                    Dim consultaAntes = ConsultasSQL.GetConsultaContrato(Contratos)
                    'Exporto los contratos antes de la modificacion
                    PictureBox2.Visible = True
                    tasks.Add(Task.Run(Sub()
                                           ExportarConsultaAExcel(connectionString, consultaAntes, rutaArchivoAntesModificacion, "Contratos")
                                           'Unificamos la query
                                           Dim QueryFinal = $"{UpdateContrato} {ValoresF} {WhereContrato}"
                                           'Ejecutamos la query
                                           Dim funciones2 As New FuncionesGenericas(connectionString)
                                           ContratosModificados = funciones2.UpdateContratosMasivo(QueryFinal)

                                           Dim consultaDespues = ConsultasSQL.GetConsultaContrato(Contratos)
                                           ExportarConsultaAExcel(connectionString, consultaAntes, rutaArchivoDespuesModificacion, "Contratos")
                                       End Sub))
                    Dim pepe = 0
                    Await Task.WhenAll(tasks)
                    PictureBox2.Visible = False
                    If ContratosModificados > 0 Then
                        complementos.MostrarMensajePersonalizado($"Compare los resultados en los excels generados.{rutaArchivoAntesModificacion} y {rutaArchivoDespuesModificacion}")
                    Else
                        complementos.MostrarMensajePersonalizado($"No se ha actualizado ningún contrato.")
                    End If
                    'Vuelvo a consultar los contratos esten o no esten modificados
                End If
        Catch ex As Exception
            PictureBox2.Visible = False
            complementos.MostrarMensajePersonalizado($"{ex.Message}")
        End Try
    End Sub
    Private Function getDiasVencimiento() As String
        Dim DiasVencimiento As String = ""
        Try
            If CheckBox2.Checked AndAlso CheckBox3.Checked Then ' si el alta y vto se van a modificar, calculamos los dias
                DiasVencimiento = DateDiff(DateInterval.Day, DateTimePicker3.Value.Date, DateTimePicker1.Value.Date)
            End If
            If CheckBox2.Checked AndAlso CheckBox3.Checked = False Then 'si solo se modifica el alta, calculo los dias con el alta nuevo
                DiasVencimiento = $"datediff(d,'{DateTimePicker3.Value.Date.ToString("dd/MM/yyyy")}',Fechavto) "
            End If
            If CheckBox2.Checked = False AndAlso CheckBox3.Checked Then  'si solo se modifica el vto, calculo los dias con el vto nuevo
                DiasVencimiento = $"datediff(d,fechaAlta,'{DateTimePicker1.Value.Date.ToString("dd/MM/yyyy")}') "
            End If
        Catch ex As Exception
            Throw
        End Try
        Return DiasVencimiento
    End Function

    Private Function GetParameters() As String
        Dim ValuesF As String

        Try
            Dim camposUpdate As New List(Of String)

            'SituacionContrato
            If CheckBox1.Checked Then
                Dim ContratoSituacionSeleccionado As ContratoSituacion = TryCast(ComboBox1.SelectedItem, ContratoSituacion)
                camposUpdate.Add($"IdContratoSituacion={ContratoSituacionSeleccionado.IdContratoSituacion}")
            End If

            'Alta
            If CheckBox2.Checked Then
                Dim Alta = DateTimePicker3.Value.Date.ToString("dd/MM/yyyy")
                camposUpdate.Add($"FechaAlta='{Alta}'")
            End If

            'Vto
            If CheckBox3.Checked Then
                Dim VTO = DateTimePicker1.Value.Date.ToString("dd/MM/yyyy")
                camposUpdate.Add($"FechaVto='{VTO}'")
            End If
            'Observaciones Contrato
            If CheckBox5.Checked Then
                If ChckBlancoObservacion.Checked Then
                    camposUpdate.Add($"Observaciones=''")
                Else
                    camposUpdate.Add($"Observaciones=Observaciones+' | {TextBox1.Text}'")
                End If

            End If
            'Tipo Impuesto
            If CheckBox6.Checked Then
                'ComboBox2
                Dim TipoImpuestoSeleccionado As TipoImpuesto = TryCast(ComboBox2.SelectedItem, TipoImpuesto)
                camposUpdate.Add($"IdTipoImpuesto={TipoImpuestoSeleccionado.IdTipoImpuesto}")
            End If
            'Unificar Facturas
            If CheckBox7.Checked Then
                Dim Sino = "0"
                If CheckBox22.Checked Then
                    Sino = "1"
                End If
                camposUpdate.Add($"IsAgruparFacturas={Sino}")
            End If
            'Revision
            If CheckBox8.Checked Then
                Dim Sino = "0"
                If CheckBox23.Checked Then
                    Sino = "1"
                End If
                camposUpdate.Add($"RevisionFra={Sino}")
            End If

            'Texto Revision Facturas
            If CheckBox9.Checked Then
                If CheckBlancoRevision.Checked Then
                    camposUpdate.Add($"textorevision=''")
                Else
                    camposUpdate.Add($"textorevision=textorevision+' | {TextBox2.Text}'")
                End If

            End If
            'Modelo Factura
            If CheckBox17.Checked Then
                Dim TipoModeloFac As ModeloDeImpresion = TryCast(ComboBox6.SelectedItem, ModeloDeImpresion)
                camposUpdate.Add($"IdModeloFactura={TipoModeloFac.IdModeloDeImpresion}")
            End If
            'Modelo Factura Varios
            If CheckBox18.Checked Then
                Dim TipoModeloFac As ModeloDeImpresion = TryCast(ComboBox7.SelectedItem, ModeloDeImpresion)
                camposUpdate.Add($"IdModeloFacturaVarios={TipoModeloFac.IdModeloDeImpresion}")
            End If
            'CNAE
            If CheckBox10.Checked Then
                Dim CNASEseleccionado As CNAE = TryCast(ComboBox3.SelectedItem, CNAE)
                camposUpdate.Add($"idCNAE={CNASEseleccionado.IdCNAE}")
            End If
            'TipoImprimir
            If CheckBox11.Checked Then
                Dim TipoImpresionSelec As TipoImpresionItem = TryCast(ComboBox4.SelectedItem, TipoImpresionItem)
                camposUpdate.Add($"TipoImprimir='{TipoImpresionSelec.Key}'")
            End If
            'Modelo Factura Varios
            If CheckBox16.Checked Then
                Dim TipoModeloCon As ModeloDeImpresion = TryCast(ComboBox5.SelectedItem, ModeloDeImpresion)
                camposUpdate.Add($"IdModeloContrato={TipoModeloCon.IdModeloDeImpresion}")
            End If

            'Renovacion Procesada
            If CheckBox15.Checked Then
                Dim Sino = "0"
                If CheckBox25.Checked Then
                    Sino = "1"
                End If
                camposUpdate.Add($"IsRenovacionProcesada={Sino}")
            End If

            'No Renovar
            If CheckBox14.Checked Then
                Dim Sino = "0"
                If CheckBox24.Checked Then
                    Sino = "1"
                End If
                camposUpdate.Add($"NoRenovar={Sino}")
            End If

            'Nombre Representante
            If CheckBox13.Checked Then
                camposUpdate.Add($"Representante='{TextBox3.Text}'")
            End If
            'Colectivo Representante
            If CheckBox12.Checked Then
                Dim ColectivoSelect As Colectivo = TryCast(ComboBox8.SelectedItem, Colectivo)
                camposUpdate.Add($"IdColectivoRep={ColectivoSelect.IdColectivo}")
            End If
            'Nombre Representante
            If CheckBox19.Checked Then
                camposUpdate.Add($"IdentificadorRep='{TextBox4.Text}'")
            End If
            'Email Representante
            If CheckBox21.Checked Then
                camposUpdate.Add($"EmailRep='{TextBox6.Text}'")
            End If
            'Movil Representante
            If CheckBox20.Checked Then
                camposUpdate.Add($"SMSRep='{TextBox5.Text}'")
            End If

            'Scoring
            If CheckBox4.Checked Then
                Dim SituacionScoringSelect As SituacionScoring = TryCast(ComboBox9.SelectedItem, SituacionScoring)
                camposUpdate.Add($"SituacionScoring='{SituacionScoringSelect.Nombre}'")
            End If

            'ClientePago
            If CheckBox26.Checked Then
                Dim ClientePagoSelect As ClientePago = TryCast(ComboBox10.SelectedItem, ClientePago)
                camposUpdate.Add($"idclientepago={ClientePagoSelect.IdClientePago}")
            End If


            If chkAutoconsumo.Checked Then
                Dim Sino = "0"
                If chkAutoconsumoSINO.Checked Then
                    Sino = "1"
                End If
                camposUpdate.Add($"autoconsumo={Sino}")
            End If

            If chkAutoconsumoNOCompesable.Checked Then
                Dim Sino = "0"
                If chkAutoconsumoNOCompesableSINO.Checked Then
                    Sino = "1"
                End If
                camposUpdate.Add($"IsAutoconsumoNoCompensable={Sino}")
            End If

            If chkLicitacion.Checked Then
                Dim Sino = "0"
                If chkLicitacionSINO.Checked Then
                    Sino = "1"
                End If
                camposUpdate.Add($"islicitacion={Sino}")
            End If

            If ChkExencionIE.Checked Then
                Dim Sino = "0"
                If ChkExencionIESINO.Checked Then
                    Sino = "1"
                End If
                camposUpdate.Add($"ExencionIE={Sino}")
            End If
            'TiposAutoconsumo
            If chktipoautoconsumo.Checked Then
                Dim TiposAutoconsumoSelect As TiposAutoconsumo = TryCast(ComboTipoAutoconsumo.SelectedItem, TiposAutoconsumo)
                camposUpdate.Add($"idtipoautoconsumo={TiposAutoconsumoSelect.IdTipoAutoconsumo}")
            End If

            'DiasVencimiento
            If getDiasVencimiento.Length > 0 Then
                camposUpdate.Add($"diasvencimiento={getDiasVencimiento()}")
            End If

            ValuesF = String.Join(", ", camposUpdate)
        Catch ex As Exception
            Throw
        End Try
        Return ValuesF
    End Function

    Private Sub TextBox7_TextChanged(sender As Object, e As EventArgs) Handles TextBox7.TextChanged
        If TextBox7.Text.Length > 0 Then
            CheckBox26.Enabled = True
        Else
            CheckBox26.Enabled = False
            CheckBox26.Checked = False
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If TextBox7.Text.Length > 0 Then
            'ClientePago
            Dim funciones2 As New FuncionesGenericas(Me.connectionString)
            Me.ComboBox10.DataSource = funciones2.GetClientePago(TextBox7.Text)
            Me.ComboBox10.DisplayMember = "ClientePagoUnificado"
            Me.ComboBox10.ValueMember = "IdClientePago"
        End If
    End Sub
End Class
Public Class TipoImpresionItem
    Public Property Key As String
    Public Property TextoTipoImpresion As String
End Class
