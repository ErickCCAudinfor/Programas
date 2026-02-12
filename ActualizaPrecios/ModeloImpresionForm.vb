Imports System.ComponentModel
Imports System.Text.Json
Imports System.IO

Public Class ModeloImpresionForm

    Private Empresas As List(Of EmpresaBD)
    Private RutaConfig As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Json", "EmpresasBD.json")
    Private _complementos As New Complementos
    Private _GlobalConnecString As String

    Public Sub New()
        InitializeComponent()
        CargarEmpresas()
    End Sub

    Private Sub ValorCambia(sender As Object, e As EventArgs) Handles BDEmpresaCombo.TextChanged
        Try
            CargarModelos()
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
            Dim report = funciones.GetAllModelosImpresion()

            ' Convertir a BindingList para que sea editable y permita añadir filas
            Dim bindingList As New BindingList(Of ModeloDeImpresion)(report)
            Dim source As New BindingSource(bindingList, Nothing)

            DataModeloImpresionView.DataSource = source

            ' Configurar columnas ReadOnly si hace falta
            DataModeloImpresionView.Columns("IdModeloDeImpresion").ReadOnly = True
            If DataModeloImpresionView.Columns.Contains("Modelo") Then
                DataModeloImpresionView.Columns("Modelo").Visible = False
            End If
        Catch ex As Exception
            Return
        End Try

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

    Private Sub DataModeloImpresionView_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs) Handles DataModeloImpresionView.UserDeletingRow
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
            Dim delete = funciones.DeleteModeloImpresion(modelo.IdModeloDeImpresion)

        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)
        Finally
            CargarModelos()
        End Try
    End Sub

    Private Sub RecargaModelos_Click(sender As Object, e As EventArgs) Handles RecargaModelos.Click
        Try
            CargarModelos()
        Catch ex As Exception

        End Try
    End Sub
End Class