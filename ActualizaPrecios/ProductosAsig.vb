Imports System.IO

Public Class ProductosAsig


    Private ReadOnly Property ipDB As String
    'Private ReadOnly Property ipDB As String = "data source=172.31.100.50\TOTALUAT;"
    Private ReadOnly Property nameDB As String
    'Private ReadOnly Property nameDB As String = "initial catalog=SigeTotalUAT;"
    Private ReadOnly Property userDB As String
    Private ReadOnly Property passDB As String

    Private ReadOnly Property Contratos As List(Of Long)

    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Private Funciones As New FuncionesGenericas(Me.connectionString)

    Public Sub New(Entorno As String, Contratos As List(Of Long), ipDB As String, nameDB As String, userDB As String, passDB As String)
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
            Me.Contratos = Contratos
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
            Dim Contrato As New List(Of Contrato)
            'Contrato.Add(Funciones.GetContrato(12))

            ' Ruta del archivo de texto
            Dim rutaCarpeta As String = $"C:\logP\{Date.Today.ToString("ddMMyyyy")}"
            Dim rutaArchivo As String = Path.Combine(rutaCarpeta, "logProgramas.txt")
            ' Verificar si la carpeta existe, y si no, crearla
            If Not Directory.Exists(rutaCarpeta) Then
                Directory.CreateDirectory(rutaCarpeta)
            End If

            ' Verificar si el archivo existe, y si no, crearlo
            If Not File.Exists(rutaArchivo) Then
                File.Create(rutaArchivo).Close()
            End If

            ' Abrir el archivo para escritura
            Using escritor As New StreamWriter(rutaArchivo, True)
                For Each elemnt In Contrato
                    ' Realiza las acciones que desees hacer dentro del bucle
                    ' Por ejemplo, puedes llamar a una función que escriba en el archivo
                    Funciones.EscribirEnArchivo(escritor, elemnt)
                Next
            End Using

            MessageBox.Show($"Se han escrito todos los datos en el archivo correctamente. {rutaArchivo}")
        Catch ex As Exception
            MessageBox.Show("Error al escribir en el archivo: " & ex.Message)
        End Try
    End Sub
End Class