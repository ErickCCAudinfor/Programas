Imports System.IO

Public Class ProductosAsig


    'Private ReadOnly Property ipDB As String
    ''Private ReadOnly Property ipDB As String = "data source=172.31.100.50\TOTALUAT;"
    'Private ReadOnly Property nameDB As String
    ''Private ReadOnly Property nameDB As String = "initial catalog=SigeTotalUAT;"
    'Private ReadOnly Property userDB As String
    'Private ReadOnly Property passDB As String

    Private ReadOnly Property Contratos As List(Of Long)

    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub


    Public Sub New(Entorno As String, Contratos As List(Of Long), connectionString As String)
        Try
            InitializeComponent()
            Me.connectionString = connectionString
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            ' Llamo a los productos
            Me.ComboBox1.DataSource = Funciones.GetProductosbyEntorno(Entorno)
            Me.ComboBox1.DisplayMember = "TextoProducto"
            Me.ComboBox1.ValueMember = "IdProducto"
            Me.Contratos = Contratos
        Catch ex As Exception
            Throw
        End Try
        ' Esta llamada es exigida por el diseñador.

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            ' Obtener el objeto Producto seleccionado
            Dim productoSeleccionado As Producto = TryCast(ComboBox1.SelectedItem, Producto)

            ' Verificar si se seleccionó un producto válido
            If Not IsNothing(productoSeleccionado) AndAlso productoSeleccionado.IdProducto > 0 Then
                ' Cambios los valores según se seleccione
                Dim produc = Funciones.GetProductoGrupobyById(productoSeleccionado.IdProductoGrupo)
                Dim ImpuestosTipos = Funciones.GetTipoImpuestoBy()
                TextBox1.Text = produc.TextoProductoGrupo
                NumericUpDown1.Value = productoSeleccionado.Importe
                CheckBox1.Checked = productoSeleccionado.AntesIE
                CheckBox4.Checked = productoSeleccionado.SobreConsumo
                CheckBox5.Checked = productoSeleccionado.PrecioSobreConsumo
                ComboBox2.DataSource = ImpuestosTipos
                ComboBox2.DisplayMember = "TextoImpuesto"
                ComboBox2.ValueMember = "IdTipoImpuesto"

                ' Ahora, establece el tipo de impuesto seleccionado
                If productoSeleccionado IsNot Nothing AndAlso productoSeleccionado.IdTipoImpuesto > 0 Then
                    ComboBox2.SelectedValue = productoSeleccionado.IdTipoImpuesto
                Else
                    ' Si no hay un tipo de impuesto asociado al producto seleccionado, deselecciona cualquier elemento en el ComboBox de tipos de impuesto
                    ComboBox2.SelectedIndex = -1
                End If
            End If
        Catch ex As Exception
            Throw
        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            Dim productoSeleccionado As Producto = TryCast(ComboBox1.SelectedItem, Producto)
            Dim TipoImpuesto As TipoImpuesto = TryCast(ComboBox2.SelectedItem, TipoImpuesto)
            Dim importe = NumericUpDown1.Value
            Dim Fecha = DateTimePicker1.Value.ToString("yyyy-MM-dd")
            Dim IdTipoImpuesto = TipoImpuesto.IdTipoImpuesto
            Dim AntesIe = CheckBox1.Checked
            Dim SobreConsumo = CheckBox4.Checked
            Dim PrecioSobreConsumo = CheckBox5.Checked
            For Each elemnt In Contratos
                Dim Contrato = Funciones.GetContrato(elemnt)
                If CheckBox2.Checked Then 'Insertar
                    Funciones.InsertProductoAsignacion(Contrato.Entorno, productoSeleccionado.IdProductoGrupo, productoSeleccionado.IdProducto, Contrato.IdContrato, Fecha, importe, IdTipoImpuesto, AntesIe, SobreConsumo, PrecioSobreConsumo)
                End If
                'If Not CheckBox2.Checked Then 'Insertar
                '    Funciones.UpdateProductoAsignacion(Contrato.Entorno, productoSeleccionado.IdProductoGrupo, productoSeleccionado.IdProducto, Contrato.IdContrato, Fecha, importe, IdTipoImpuesto, AntesIe, SobreConsumo, PrecioSobreConsumo)
                'End If

            Next
            MessageBox.Show($"Se han escrito todos los datos en el archivo correctamente.")
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        Try
            If CheckBox2.Checked Then
                Button1.Text = "Insertar"
            Else
                Button1.Text = "Actualizar"
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class