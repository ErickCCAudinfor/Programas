Public Class Agentes
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
            Dim agentes = Funciones.GetAgente()
            Me.ComboBox1.DataSource = agentes
            Me.ComboBox1.DisplayMember = "NombreAgente"
            Me.ComboBox1.ValueMember = "IdAgente"
            Label3.Text = agentes.Count
        Catch ex As Exception
            Throw
        End Try
        ' Esta llamada es exigida por el diseñador.

    End Sub

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            Dim AgenteSeleccionado As Agente = TryCast(ComboBox1.SelectedItem, Agente)

            For Each elemnt In Contratos
                Dim Contrato = Funciones.GetContrato(elemnt)
                If Not IsNothing(Contrato) AndAlso Contrato.IdContrato > 0 AndAlso AgenteSeleccionado.IdAgente > 0 Then
                    Dim ok = Await Task.Run(Function() Funciones.UpdateContratoIdAgente(Contrato.CodigoContrato, AgenteSeleccionado.IdAgente))
                End If
            Next
            MessageBox.Show($"Se han escrito todos los datos en el archivo correctamente.")
        Catch ex As Exception
            Throw
        End Try
    End Sub
End Class