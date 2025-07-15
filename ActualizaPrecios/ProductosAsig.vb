Imports System.IO

Public Class ProductosAsig
    Dim LoadingWF As New LoadingWF
    Private ReadOnly Property Contratos As List(Of Long)

    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Dim FechaFinalSeleccionada As Nullable(Of DateTime) = Nothing


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
                Dim ImpuestosTipos As New List(Of TipoImpuesto)
                ImpuestosTipos.Add(New TipoImpuesto)
                ImpuestosTipos.AddRange(Funciones.GetTipoImpuestoBy())

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

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim NFilasAfectadas = 0L
            Dim TotalFilasAfectas = 0L
            LoadingWF.Show()
            Button1.Enabled = False 'Desactivo el boton de actualizar
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            Dim productoSeleccionado As Producto = TryCast(ComboBox1.SelectedItem, Producto)
            Dim TipoImpuesto As TipoImpuesto = TryCast(ComboBox2.SelectedItem, TipoImpuesto)
            Dim importe = NumericUpDown1.Value
            Dim Fecha = FechaInicialPicker.Value.ToString("yyyy-MM-dd")
            Dim IdTipoImpuesto = TipoImpuesto.IdTipoImpuesto
            Dim AntesIe = CheckBox1.Checked
            Dim SobreConsumo = CheckBox4.Checked
            Dim PrecioSobreConsumo = CheckBox5.Checked
            Dim PrecioSobredia = CheckBox3.Checked
            Dim FechaFinal = "NULL"
            If FechaFinalSeleccionada.HasValue Then
                Dim FechaFormateada As String = FechaFinalSeleccionada.Value.ToString("dd/MM/yyyy HH:mm:ss")

                FechaFinal = $"'{FechaFormateada}'"
            End If
            For Each elemnt In Contratos
                Dim Contrato = Funciones.GetContrato(elemnt)
                If If(Contrato.IdTipoImpuesto, 0) <> 0 AndAlso IdTipoImpuesto <> 0 Then
                    IdTipoImpuesto = Contrato.IdTipoImpuesto
                End If
                If CheckBox2.Checked Then 'Insertar
                    NFilasAfectadas = Await Task.Run(Function() Funciones.InsertProductoAsignacion(Contrato.Entorno, productoSeleccionado.IdProductoGrupo, productoSeleccionado.IdProducto, Contrato.IdContrato, Fecha, importe, IdTipoImpuesto, AntesIe, SobreConsumo, PrecioSobreConsumo, PrecioSobredia, FechaFinal))
                    TotalFilasAfectas += NFilasAfectadas
                End If
            Next
            LoadingWF.Hide()
            If NFilasAfectadas > 0 Then
                MessageBox.Show($"Se han insertado {TotalFilasAfectas} registros.")
            End If
        Catch ex As Exception
            LoadingWF.Hide()
            Throw
        Finally
            Button1.Enabled = True 'Activo el boton de actualizar
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


    Public Function getImpuestoCanariasbyCodPostal(codPostal As String) As Long
        Dim IdImpuesto As Long = 0

        Try
            Dim primerosDos As String = codPostal.Substring(0, 2) ' Obtener los primeros dos caracteres del código postal

            Select Case primerosDos
                Case "35", "38"
                    IdImpuesto = 5
                Case Else
                    ' Código postal no válido
                    IdImpuesto = 1 ' O cualquier otro valor que desees para indicar que el código postal no es válido
            End Select
        Catch ex As Exception
            Throw
        End Try

        Return IdImpuesto
    End Function

    Private Sub FechaFinalPicker_ValueChanged(sender As Object, e As EventArgs) Handles FechaFinalPicker.ValueChanged
        FechaFinalPicker.Format = DateTimePickerFormat.Short
        FechaFinalSeleccionada = FechaFinalPicker.Value
    End Sub
    Private Sub DateTimePicker1_KeyDown(sender As Object, e As KeyEventArgs) Handles FechaFinalPicker.KeyDown
        If e.KeyCode = Keys.Back OrElse e.KeyCode = Keys.Delete Then
            FechaFinalPicker.Format = DateTimePickerFormat.Custom
            FechaFinalPicker.CustomFormat = " "
            FechaFinalSeleccionada = Nothing
        End If
    End Sub

End Class