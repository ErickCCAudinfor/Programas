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

        BDEmpresaCombo.DataSource = Nothing
        BDEmpresaCombo.DataSource = Empresas
        BDEmpresaCombo.DisplayMember = "Nombre"
        LabelServidor.Text = $"Servidor: {Empresas.FirstOrDefault.Servidor} - Base Datos: {Empresas.FirstOrDefault.BaseDatos}"
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
    'Private Sub CerrarFormModelo(sender As Object, e As EventArgs) Handles Me.FormClosing
    '    MarcarUsuarioDesconectado(SesionActual.UsuarioLogueado)
    'End Sub
End Class