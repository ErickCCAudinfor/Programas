Imports System.Reflection.Emit

Public Class AdministradoresWF
    Private ReadOnly Property Contratos As List(Of Long)

    Private ReadOnly Property connectionString As String
    Private ReadOnly Property Funciones As FuncionesGenericas

    Public Sub New(Contratos As List(Of Long), connectionString As String)
        Try
            InitializeComponent()
            Me.connectionString = connectionString
            Me.Contratos = Contratos
            Dim funciones2 As New FuncionesGenericas(Me.connectionString)
            Funciones = funciones2
            Dim admins = Funciones.GetAdmind()
            Me.ComboBox1.DataSource = admins
            Me.ComboBox1.DisplayMember = "NombreAdministrador"
            Me.ComboBox1.ValueMember = "IdAdministrador"
            Label3.Text = admins.Count
        Catch ex As Exception
            Throw
        End Try
        ' Esta llamada es exigida por el diseñador.
    End Sub

    'Actualizo el idadministrador del contrato, para luz o gas según lo que se haya seleccionado en los combos
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            Dim AdministradorSeleccionado As Administrador = TryCast(ComboBox1.SelectedItem, Administrador)
            Dim AdministradorSeleccionadoGas As Administrador = TryCast(ComboBox2.SelectedItem, Administrador)
            Dim ListaOk As New List(Of Contrato)
            For Each elemnt In Contratos
                Dim Contrato = Funciones.GetContrato(elemnt)
                'Contrato de luz
                If Not IsNothing(Contrato) AndAlso Contrato.IdContrato > 0 AndAlso Contrato.Entorno = "E1" AndAlso Not IsNothing(AdministradorSeleccionado) AndAlso AdministradorSeleccionado.IdAdministrador > 0 Then
                    Dim ok = Await Task.Run(Function() Funciones.UpdateContratoIdAdmin(Contrato.CodigoContrato, AdministradorSeleccionado.IdAdministrador))
                    If ok > 0 Then
                        ListaOk.Add(Contrato)
                    End If
                End If
                'Contrato de Gas
                If Not IsNothing(Contrato) AndAlso Contrato.IdContrato > 0 AndAlso Contrato.Entorno = "E2" AndAlso Not IsNothing(AdministradorSeleccionadoGas) AndAlso AdministradorSeleccionadoGas.IdAdministrador > 0 Then
                    Dim ok = Await Task.Run(Function() Funciones.UpdateContratoIdAdmin(Contrato.CodigoContrato, AdministradorSeleccionadoGas.IdAdministrador))
                    If ok > 0 Then
                        ListaOk.Add(Contrato)
                    End If
                End If
            Next
            If ListaOk.Count > 0 Then
                MessageBox.Show($"Contratos Actualizados")
            Else
                MessageBox.Show($"No se ha actualizado ningún contrato")
            End If

        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            Dim AdministradorSeleccionado As Administrador = TryCast(ComboBox1.SelectedItem, Administrador)

            If Not IsNothing(AdministradorSeleccionado) AndAlso AdministradorSeleccionado.IdAdministrador > 0 Then
                Label7.Text = AdministradorSeleccionado.IdAdministrador 'Añado el id del admin de luz

                ' Asumiendo que admin es el tipo de objeto que contiene la lista
                Dim listaAdministradorres As List(Of Administrador) = TryCast(ComboBox1.DataSource, List(Of Administrador))

                ' Buscar el admin de gas
                Dim AdminGas = listaAdministradorres.FirstOrDefault(Function(f) f.Entorno.StartsWith("G2") AndAlso f.NombreAdministrador.Contains(Replace(AdministradorSeleccionado.NombreAdministrador, "G1 ", "")))

                If Not IsNothing(AdminGas) AndAlso AdminGas.IdAdministrador > 0 Then
                    ' Mostrar solo el admin de gas en ComboBox2
                    Me.ComboBox2.DataSource = New List(Of Administrador) From {AdminGas}
                    Me.ComboBox2.DisplayMember = "NombreAdministrador"
                    Me.ComboBox2.ValueMember = "IdAdministrador"
                    Me.ComboBox2.SelectedIndex = 0 ' Seleccionar el único elemento en el ComboBox2
                    Label8.Text = AdminGas.IdAdministrador
                Else
                    ' No se encontró admin de gas, dejar ComboBox2 en blanco
                    Me.ComboBox2.DataSource = Nothing
                    Me.ComboBox2.Items.Clear()
                    Label8.Text = "" 'Quito el id del admin de gas
                End If
            Else
                ' No se seleccionó ningún admin en ComboBox1, dejar ComboBox2 en blanco
                Me.ComboBox2.DataSource = Nothing
                Me.ComboBox2.Items.Clear()
                Label8.Text = "" 'Quito el id del admin de gas
                Label7.Text = "" 'Quito el id del admin de luz
            End If
        Catch ex As Exception
            ' Manejar excepciones según sea necesario
        End Try
    End Sub
End Class