Public Class ProductosAsig

    Private Funciones As New FuncionesGenericas

    Private ReadOnly Property ipDB As String
    'Private ReadOnly Property ipDB As String = "data source=172.31.100.50\TOTALUAT;"
    Private ReadOnly Property nameDB As String
    'Private ReadOnly Property nameDB As String = "initial catalog=SigeTotalUAT;"
    Private ReadOnly Property userDB As String
    Private ReadOnly Property passDB As String

    Public Sub New(Entorno As String, ipDB As String, nameDB As String, userDB As String, passDB As String)
        Try
            InitializeComponent()
            Me.ipDB = ipDB
            Me.nameDB = nameDB
            Me.userDB = userDB
            Me.passDB = passDB
            ' Llamo a los productos
            Me.ComboBox1.DataSource = Funciones.GetProductosbyEntorno(Entorno, ipDB, nameDB, userDB, passDB)
            Me.ComboBox1.DisplayMember = "TextoProducto"
            Me.ComboBox1.ValueMember = "IdProducto"
        Catch ex As Exception
            Throw
        End Try
        ' Esta llamada es exigida por el diseñador.

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            ' Obtener el objeto Producto seleccionado
            Dim productoSeleccionado As Producto = TryCast(ComboBox1.SelectedItem, Producto)

            ' Verificar si se seleccionó un producto válido
            If Not IsNothing(productoSeleccionado) AndAlso productoSeleccionado.IdProducto > 0 Then
                ' Cambios los valores según se seleccione
                Dim produc = Funciones.GetProductoGrupobyById(productoSeleccionado.IdProductoGrupo, ipDB, nameDB, userDB, passDB)
                Dim ImpuestosTipos = Funciones.GetTipoImpuestoBy(ipDB, nameDB, userDB, passDB)
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

        Catch ex As Exception
            Throw
        End Try
    End Sub
End Class