Public Class Agentes
    Private ReadOnly Property Contratos As List(Of Long)

    Private ReadOnly Property connectionString As String
    Private ReadOnly Property Funciones As FuncionesGenericas
    Dim LoadingWF As New LoadingWF
    Public Sub New(Contratos As List(Of Long), connectionString As String)
        Try
            InitializeComponent()
            Me.connectionString = connectionString
            Me.Contratos = Contratos
            Dim funciones2 As New FuncionesGenericas(Me.connectionString)
            Funciones = funciones2
            Dim agentes = Funciones.GetAgente()
            Me.ComboBox1.DataSource = agentes
            Me.ComboBox1.DisplayMember = "NombreAgente"
            Me.ComboBox1.ValueMember = "IdAgente"
            Label3.Text = agentes.Count
            Label9.Text = Contratos.Count
        Catch ex As Exception
            Throw
        End Try
        ' Esta llamada es exigida por el diseñador.

    End Sub

    'Actual el idAgente del contrato, para luz o gasm según lo que se haya seleccionado en los combos
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            Dim AgenteSeleccionado As Agente = TryCast(ComboBox1.SelectedItem, Agente)
            Dim AgenteSeleccionadoGas As Agente = TryCast(ComboBox2.SelectedItem, Agente)
            Dim ListaOk As New List(Of Contrato)
            LoadingWF.Show()
            For Each elemnt In Contratos

                Dim Contrato = Funciones.GetContrato(elemnt)
                'Contrato de luz
                If Not IsNothing(Contrato) AndAlso Contrato.IdContrato > 0 AndAlso Contrato.Entorno = "E1" AndAlso Not IsNothing(AgenteSeleccionado) AndAlso AgenteSeleccionado.IdAgente > 0 Then
                    Dim ok = Await Task.Run(Function() Funciones.UpdateContratoIdAgente(Contrato.CodigoContrato, AgenteSeleccionado.IdAgente))
                    If ok > 0 Then
                        ListaOk.Add(Contrato)
                    End If
                End If
                'Contrato de Gas
                If Not IsNothing(Contrato) AndAlso Contrato.IdContrato > 0 AndAlso Contrato.Entorno = "E2" AndAlso Not IsNothing(AgenteSeleccionadoGas) AndAlso AgenteSeleccionadoGas.IdAgente > 0 Then
                    Dim ok = Await Task.Run(Function() Funciones.UpdateContratoIdAgente(Contrato.CodigoContrato, AgenteSeleccionadoGas.IdAgente))
                    If ok > 0 Then
                        ListaOk.Add(Contrato)
                    End If
                End If
            Next
            LoadingWF.Hide()

            If ListaOk.Count > 0 Then
                MessageBox.Show($"Contratos Actualizados")
            Else
                MessageBox.Show($"No se ha actualizado ningún contrato")
            End If

        Catch ex As Exception
            LoadingWF.Hide()
            Throw
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            Dim AgenteSeleccionado As Agente = TryCast(ComboBox1.SelectedItem, Agente)

            If Not IsNothing(AgenteSeleccionado) AndAlso AgenteSeleccionado.IdAgente > 0 Then
                Label7.Text = AgenteSeleccionado.IdAgente 'Añado el id del agente de luz

                ' Asumiendo que Agente es el tipo de objeto que contiene tu lista
                Dim listaAgentes As List(Of Agente) = TryCast(ComboBox1.DataSource, List(Of Agente))

                ' Buscar el agente de gas
                Dim AgenteGas = listaAgentes.FirstOrDefault(Function(f) f.Entorno.StartsWith("G2") AndAlso f.NombreAgente.Contains(Replace(AgenteSeleccionado.NombreAgente, "G1 ", "")))

                If Not IsNothing(AgenteGas) AndAlso AgenteGas.IdAgente > 0 Then
                    ' Mostrar solo el agente de gas en ComboBox2
                    Me.ComboBox2.DataSource = New List(Of Agente) From {AgenteGas}
                    Me.ComboBox2.DisplayMember = "NombreAgente"
                    Me.ComboBox2.ValueMember = "IdAgente"
                    Me.ComboBox2.SelectedIndex = 0 ' Seleccionar el único elemento en el ComboBox2
                    Label8.Text = AgenteGas.IdAgente
                Else
                    ' No se encontró agente de gas, dejar ComboBox2 en blanco
                    Me.ComboBox2.DataSource = Nothing
                    Me.ComboBox2.Items.Clear()
                    Label8.Text = "" 'Quito el id del agente de gas
                End If
            Else
                ' No se seleccionó ningún agente en ComboBox1, dejar ComboBox2 en blanco
                Me.ComboBox2.DataSource = Nothing
                Me.ComboBox2.Items.Clear()
                Label8.Text = "" 'Quito el id del agente de gas
                Label7.Text = "" 'Quito el id del agente de luz
            End If
        Catch ex As Exception
            ' Manejar excepciones según sea necesario
        End Try
    End Sub

End Class