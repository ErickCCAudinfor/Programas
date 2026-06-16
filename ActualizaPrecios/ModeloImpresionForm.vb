Imports System.ComponentModel
Imports System.Text.Json
Imports System.IO

Public Class ModeloImpresionForm

    Private Empresas As List(Of EmpresaBD)
    Private RutaConfig As String = RutaConfigEmpresas
    Private _complementos As New Complementos
    Private _GlobalConnecString As String
    Private _todosModelos As List(Of ModeloDeImpresion)

    Public Sub New()
        InitializeComponent()
        CargarEmpresas()
    End Sub

    Private Sub ValorCambia(sender As Object, e As EventArgs) Handles BDEmpresaCombo.TextChanged
        Try
            If CheckVPN.Checked Then
                'Con VPN activo no se cargan los modelos automáticamente (solo con RecargaModelos)
                Dim empVPN As EmpresaBD = CType(BDEmpresaCombo.SelectedItem, EmpresaBD)
                If empVPN Is Nothing Then Return
                _GlobalConnecString = GetConnectionString(empVPN)
                _todosModelos = Nothing
                DataModeloImpresionView.DataSource = Nothing
                LabelServidor.Text = $"Servidor: {empVPN.Servidor} - Base Datos: {empVPN.BaseDatos}"
                Return
            End If

            CargarModelos()
            Dim emp As EmpresaBD = CType(BDEmpresaCombo.SelectedItem, EmpresaBD)
            If emp Is Nothing Then Return
            LabelServidor.Text = $"Servidor: {emp.Servidor} - Base Datos: {emp.BaseDatos}"
        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        End Try

    End Sub
    Private Sub CargarModelos()
        Try
            If BDEmpresaCombo.SelectedItem Is Nothing Then Exit Sub

            Dim emp As EmpresaBD = CType(BDEmpresaCombo.SelectedItem, EmpresaBD)
            Dim connStr As String = GetConnectionString(emp)
            _GlobalConnecString = connStr
            Dim funciones As New FuncionesGenericas(connStr)
            _todosModelos = funciones.GetAllModelosImpresion()
            TextFiltro.Text = ""
            AplicarFiltro()
        Catch ex As Exception
            Return
        End Try
    End Sub

    Private Sub AplicarFiltro()
        If _todosModelos Is Nothing Then Return

        Dim filtro As String = TextFiltro.Text.Trim().ToLower()
        Dim lista As List(Of ModeloDeImpresion)

        If filtro.Length > 0 Then
            lista = _todosModelos.Where(Function(m)
                                            Return If(m.DescripcionModeloDeImpresion, "").ToLower().Contains(filtro) OrElse
                       If(m.Entorno, "").ToLower().Contains(filtro) OrElse
                       If(m.ClassName, "").ToLower().Contains(filtro) OrElse
                       If(m.RptFileName, "").ToLower().Contains(filtro)
                                        End Function).ToList()
        Else
            lista = _todosModelos
        End If

        Dim bindingList As New BindingList(Of ModeloDeImpresion)(lista)
        Dim source As New BindingSource(bindingList, Nothing)
        DataModeloImpresionView.DataSource = source

        If DataModeloImpresionView.Columns.Contains("IdModeloDeImpresion") Then
            DataModeloImpresionView.Columns("IdModeloDeImpresion").ReadOnly = True
        End If
        If DataModeloImpresionView.Columns.Contains("Modelo") Then
            DataModeloImpresionView.Columns("Modelo").Visible = False
        End If
    End Sub

    Private Sub TextFiltro_TextChanged(sender As Object, e As EventArgs) Handles TextFiltro.TextChanged
        AplicarFiltro()
    End Sub

    Private Sub DataModeloImpresionView_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles DataModeloImpresionView.DataError

        e.ThrowException = False
    End Sub

    Private Sub CargarEmpresas()

        If Not File.Exists(RutaConfig) Then
            File.WriteAllText(RutaConfig, "[]")
        End If

        Dim json = File.ReadAllText(RutaConfig)

        Empresas = JsonSerializer.Deserialize(Of List(Of EmpresaBD))(json,
        New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True})

        Dim listaCombo As List(Of EmpresaBD) = If(CheckVPN.Checked,
            Empresas.Where(Function(x) x.VPN).ToList(),
            Empresas)

        BDEmpresaCombo.DataSource = Nothing
        BDEmpresaCombo.DataSource = listaCombo
        BDEmpresaCombo.DisplayMember = "Nombre"

        If listaCombo.Count > 0 Then
            LabelServidor.Text = $"Servidor: {listaCombo.First.Servidor} - Base Datos: {listaCombo.First.BaseDatos}"
        Else
            LabelServidor.Text = "Servidor: -"
            _GlobalConnecString = String.Empty
            _todosModelos = Nothing
            DataModeloImpresionView.DataSource = Nothing
        End If
    End Sub

    Private Sub CheckVPN_CheckedChanged(sender As Object, e As EventArgs) Handles CheckVPN.CheckedChanged
        Try
            CargarEmpresas()
        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
    Private Function GetConnectionString(Emp1 As EmpresaBD) As String
        Dim emp As EmpresaBD = Emp1
        Dim user = CryptoHelper.Descifrar(emp.Usuario)
        Dim pass = CryptoHelper.Descifrar(emp.Password)
        Dim cc = $"Data Source={emp.Servidor};Initial Catalog={emp.BaseDatos};User ID={user};Password={pass};"
        Return cc
    End Function

    Private Sub AgregarBDBotton_Click(sender As Object, e As EventArgs) Handles AgregarBDBotton.Click
        Try
            Using VentanaRegistrarBD As New RegistrarBD(Empresas, RutaConfig)
                If VentanaRegistrarBD.ShowDialog = DialogResult.OK Then
                    CargarEmpresas()
                End If
            End Using

        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    Private Sub DataModeloImpresionView_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataModeloImpresionView.CellDoubleClick
        Try
            If e.RowIndex < 0 Then Exit Sub

            Dim modelo As ModeloDeImpresion = CType(DataModeloImpresionView.Rows(e.RowIndex).DataBoundItem, ModeloDeImpresion)


            ' Fila nueva o inválida
            If modelo Is Nothing OrElse modelo.IdModeloDeImpresion = 0 Then
                AbrirNuevoModelo()
                Exit Sub
            End If

            AbrirEdicionModelo(modelo)
        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    Private Sub AbrirEdicionModelo(modelo As ModeloDeImpresion)
        Try
            'Dim emp As EmpresaBD = CType(BDEmpresaCombo.SelectedItem, EmpresaBD)
            Using frm As New EditarModeloImpresionForm(modelo, _GlobalConnecString)
                If frm.ShowDialog() = DialogResult.OK Then
                    CargarModelos() ' recarga el datasource
                End If
            End Using
        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
    Private Sub AbrirNuevoModelo()
        Try
            'Dim emp As EmpresaBD = CType(BDEmpresaCombo.SelectedItem, EmpresaBD)
            Using frm As New EditarModeloImpresionForm(_GlobalConnecString)
                If frm.ShowDialog() = DialogResult.OK Then
                    CargarModelos() ' recarga el datasource
                End If
            End Using
        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    Private Async Sub DataModeloImpresionView_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs) Handles DataModeloImpresionView.UserDeletingRow
        Try
            ' Confirmación
            Dim r = MessageBox.Show(
        "¿Deseas eliminar este modelo?",
        "Confirmar eliminación",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning
    )
            If r = DialogResult.No Then
                e.Cancel = True
                Return
            End If

            ' Obtener el objeto asociado a la fila
            Dim modelo As ModeloDeImpresion = CType(e.Row.DataBoundItem, ModeloDeImpresion)

            ' Evitar borrar filas nuevas
            If modelo Is Nothing OrElse modelo.IdModeloDeImpresion = 0 Then
                e.Cancel = True
                Return
            End If
            'Aqui hare el borrado
            Dim funciones As New FuncionesGenericas(_GlobalConnecString)
            LoadImagen.Show()
            TextConsultando.Visible = True
            TextConsultando.Text = "Eliminando..."
            Dim delete = Await Task.Run(Function() funciones.DeleteModeloImpresion(modelo.IdModeloDeImpresion))

        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            TextConsultando.Visible = False
            TextConsultando.Text = ""
            LoadImagen.Hide()
            CargarModelos()
        End Try
    End Sub

    Private Sub RecargaModelos_Click(sender As Object, e As EventArgs) Handles RecargaModelos.Click
        Try
            CargarModelos()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BotonBackUp_Click(sender As Object, e As EventArgs) Handles BotonBackUp.Click
        Try
            Dim ListaReport As New List(Of ModeloDeImpresion)
            Dim funciones As New FuncionesGenericas(_GlobalConnecString)

            ' Recorremos todas las filas seleccionadas
            For Each row As DataGridViewRow In DataModeloImpresionView.SelectedRows
                Dim modelo As ModeloDeImpresion = CType(row.DataBoundItem, ModeloDeImpresion)
                Dim ReportBin = funciones.GetModeloImpreisonYbinario(modelo.IdModeloDeImpresion)
                ListaReport.Add(ReportBin)
            Next

            ' Pedimos al usuario la carpeta donde guardar
            Using fbd As New FolderBrowserDialog()
                fbd.Description = "Seleccione la carpeta donde guardar los reports"
                If fbd.ShowDialog() = DialogResult.OK Then
                    Dim carpetaDestino As String = fbd.SelectedPath

                    ' Guardamos cada report en un archivo .rpt
                    For Each modelor In ListaReport
                        ' Sanitizamos el nombre para que no tenga caracteres inválidos
                        Dim nombreArchivo As String = String.Concat(modelor.DescripcionModeloDeImpresion.Split(Path.GetInvalidFileNameChars()))
                        Dim rutaCompleta As String = Path.Combine(carpetaDestino, nombreArchivo & ".rpt")

                        ' Guardamos el binario en archivo
                        File.WriteAllBytes(rutaCompleta, modelor.Modelo)
                    Next

                    _complementos.MostrarMensajePersonalizado("BackUp completado con éxito.")
                End If
            End Using

        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub
    Private Sub ButtonGenerarXML_Click(sender As Object, e As EventArgs) Handles ButtonGenerarXML.Click
        If String.IsNullOrEmpty(_GlobalConnecString) Then
            _complementos.MostrarMensajePersonalizado("Selecciona primero una empresa/base de datos.")
            Return
        End If

        Using opciones As New GenerarXMLOpcionesForm(_GlobalConnecString)
            If opciones.ShowDialog(Me) <> DialogResult.OK Then Return

            Using sfd As New SaveFileDialog()
                sfd.Filter = "Archivo XML (*.xml)|*.xml"
                sfd.Title = "Guardar XML de Report"
                sfd.FileName = $"RptFichasFacturaOptENDTO_{opciones.IdFacturaVentaCabecera}.xml"

                If sfd.ShowDialog(Me) <> DialogResult.OK Then Return

                Try
                    ButtonGenerarXML.Enabled = False
                    Dim generador As New GeneradorXMLFactura(_GlobalConnecString)

                    Select Case opciones.TipoSeleccionado
                        Case GenerarXMLOpcionesForm.TipoXML.FacturasGeneral
                            generador.GenerarXMLFacturasGeneral(opciones.IdFacturaVentaCabecera, sfd.FileName)
                    End Select

                    MessageBox.Show($"XML generado correctamente en:{Environment.NewLine}{sfd.FileName}",
                                    "Generar XML", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    _complementos.MostrarMensajePersonalizado($"Error al generar el XML: {ex.Message}")
                Finally
                    ButtonGenerarXML.Enabled = True
                End Try
            End Using
        End Using
    End Sub

    Private Sub BotonAnadirMasivo_Click(sender As Object, e As EventArgs) Handles BotonAnadirMasivo.Click
        Try
            Using frm As New AnadirMasivoEmpresaForm(Empresas)
                frm.ShowDialog(Me)
            End Using
        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        End Try
    End Sub

    'Private Sub CerrarFormModelo(sender As Object, e As EventArgs) Handles Me.FormClosing
    '    MarcarUsuarioDesconectado(SesionActual.UsuarioLogueado)
    'End Sub
End Class