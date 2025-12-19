Imports System.IO
Imports OfficeOpenXml

Public Class ProductosAsig
    Dim LoadingWF As New LoadingWF
    Dim Complementos As New Complementos
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
    Dim NombreUsuarioEquipoP As String = ""

    Public Sub New(Entorno As String, Contratos As List(Of Long), connectionString As String, NombreUsuarioEquipo As String)
        Try
            InitializeComponent()
            Me.connectionString = connectionString
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
            ' Llamo a los productos
            Me.ComboBox1.DataSource = Funciones.GetProductosbyEntorno(Entorno)
            Me.ComboBox1.DisplayMember = "TextoProducto"
            Me.ComboBox1.ValueMember = "IdProducto"
            Me.Contratos = Contratos
            Me.NombreUsuarioEquipoP = NombreUsuarioEquipo
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
                Dim FechaFormateada As String = FechaFinalSeleccionada.Value.ToString("dd/MM/yyyy")
                FechaFinal = $"'{FechaFormateada}'"
            End If
            'Dim plazo = False
            'Dim plazoCargado = False
            'Dim ImporteTotalPlazo = "0"
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

    Private Sub BotonImportarProducto_Click(sender As Object, e As EventArgs) Handles BotonImportarProducto.Click
        Dim ProductoAsigExcelErrores As New List(Of String)
        Dim Contador As Long = 0
        Try
            Dim ListaProductoAsigExcel As New List(Of ProductoAsignacion)
            Dim Funciones As New FuncionesGenericas(Me.connectionString)
#Region "Archivos"
            Dim openFileDialog1 As New OpenFileDialog
            ' Configurar propiedades del diálogo
            openFileDialog1.Title = "Seleccionar archivos"
            openFileDialog1.Multiselect = True ' Permitir la selección múltiple de archivos
            openFileDialog1.Filter = "Todos los archivos (*.*)|*.*" ' Filtro de archivos
            Dim rutaArchivo = ""
            ' Mostrar el diálogo y verificar si el usuario hizo clic en OK
            If openFileDialog1.ShowDialog = DialogResult.OK Then
                ' Obtener la ruta de cada archivo seleccionado y mostrarla en la consola
                For Each filename In openFileDialog1.FileNames
                    rutaArchivo = filename
                Next
            End If
#End Region
#Region "Leer Excel"

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial
            Using package As New ExcelPackage(New FileInfo(rutaArchivo))
                Dim worksheet = package.Workbook.Worksheets(0)
                Dim rowCount = worksheet.Dimension.Rows
                ' Leer códigos de contrato del Excel
                Dim codigosContrato As New List(Of Long)
                For row = 2 To rowCount
                    Dim CodContrato = worksheet.Cells(row, 1).Value?.ToString
                    Dim ContratoBD = Funciones.GetContrato(CLng(CodContrato))
                    If Not ContratoBD Is Nothing AndAlso ContratoBD.IdContrato > 0 Then
                        Dim TextoProducto = worksheet.Cells(row, 2).Value?.ToString
                        Dim ProductoBD = Funciones.GetProductosbyTextoProducto(TextoProducto)
                        If Not ProductoBD Is Nothing AndAlso ProductoBD.IdProducto > 0 Then
                            Dim ProductoInsertar As New ProductoAsignacion
                            ProductoInsertar.IdContrato = ContratoBD.IdContrato
                            ProductoInsertar.IdProducto = ProductoBD.IdProducto
                            ProductoInsertar.IdProductoGrupo = ProductoBD.IdProductoGrupo
                            ProductoInsertar.Entorno = ContratoBD.Entorno
                            ProductoInsertar.IdTipoImpuesto = ContratoBD.IdTipoImpuesto

                            'Viene desde el excel
                            ProductoInsertar.FechaInicial = Funciones.ToNullableDate(worksheet.Cells(row, 3).Value)
                            ProductoInsertar.FechaFinal = Funciones.ToNullableDate(worksheet.Cells(row, 4).Value)
                            ProductoInsertar.Plazo = Funciones.ToNullableInteger(worksheet.Cells(row, 5).Value)
                            ProductoInsertar.PlazoCargado = Funciones.ToNullableInteger(worksheet.Cells(row, 6).Value)
                            Dim tmpImporteTotal = Funciones.ToNullableDecimal(worksheet.Cells(row, 7).Value)
                            If tmpImporteTotal.HasValue Then
                                If CheckRedondear.Checked Then
                                    ProductoInsertar.ImporteTotalPlazo = Math.Round(tmpImporteTotal.Value, 2)
                                Else
                                    ProductoInsertar.ImporteTotalPlazo = tmpImporteTotal
                                End If
                            Else
                                ProductoInsertar.ImporteTotalPlazo = Nothing
                            End If
                            Dim tmpImporte = Funciones.ToNullableDecimal(worksheet.Cells(row, 8).Value)
                            If tmpImporte.HasValue Then
                                If CheckRedondear.Checked Then
                                    ProductoInsertar.Importe = Math.Round(tmpImporte.Value, 2)
                                Else
                                    ProductoInsertar.Importe = tmpImporte
                                End If
                            Else
                                ProductoInsertar.Importe = Nothing
                            End If

                            ProductoInsertar.AntesIE = Funciones.ToNullableBoolean(worksheet.Cells(row, 9).Value)
                            ProductoInsertar.AplicarSobreConsumo = Funciones.ToNullableBoolean(worksheet.Cells(row, 10).Value).GetValueOrDefault(False)
                            ProductoInsertar.PrecioDia = Funciones.ToNullableBoolean(worksheet.Cells(row, 11).Value)
                            ProductoInsertar.AplicarPrecioConsumo = Funciones.ToNullableBoolean(worksheet.Cells(row, 12).Value).GetValueOrDefault(False)

                            ListaProductoAsigExcel.Add(ProductoInsertar)
                        Else
                            ProductoAsigExcelErrores.Add($"El Producto no Existe en la BD: {CodContrato}")
                        End If

                    Else
                        ProductoAsigExcelErrores.Add($"El contrato no Existe en la BD: {CodContrato}")
                    End If

                Next
            End Using
#End Region
#Region "Insertar Productos"
            If ListaProductoAsigExcel.Count > 0 Then
                For Each pae In ListaProductoAsigExcel
#Region "Comprobar NUlos"
                    Dim Plazo = "NULL"
                    Dim PlazoCargado = "NULL"
                    Dim ImporteTotalPlazo = "NULL"

                    Dim FechaInicial = "NULL"
                    Dim FechaFinal = "NULL"
                    Dim Importe = "NULL"

                    Dim AntesIE = "NULL"
                    Dim AplicarSobreConsumo = "NULL"
                    Dim AplicarPrecioConsumo = "NULL"
                    Dim PrecioDia = "NULL"

                    If pae.FechaInicial.HasValue Then
                        Dim FechaFormateada As String = pae.FechaInicial.Value.ToString("dd/MM/yyyy HH:mm:ss")
                        FechaInicial = $"'{FechaFormateada}'"
                    End If
                    If pae.FechaFinal.HasValue Then
                        Dim FechaFormateada As String = pae.FechaFinal.Value.ToString("dd/MM/yyyy HH:mm:ss")
                        FechaFinal = $"'{FechaFormateada}'"
                    End If

                    If Not pae.Plazo Is Nothing Then
                        If pae.Plazo > 0 Then
                            Plazo = pae.Plazo
                        End If
                        If pae.Plazo = 0 Then
                            Plazo = "0"
                        End If
                    End If

                    If Not pae.PlazoCargado Is Nothing Then
                        If pae.PlazoCargado > 0 Then
                            PlazoCargado = pae.PlazoCargado
                        End If
                        If pae.PlazoCargado = 0 Then
                            PlazoCargado = "0"
                        End If
                    End If
                    If pae.ImporteTotalPlazo.HasValue Then
                        ImporteTotalPlazo = If(pae.ImporteTotalPlazo.Value = 0, "0", CStr(pae.ImporteTotalPlazo.Value))
                    Else
                        ImporteTotalPlazo = "NULL"
                    End If
                    If pae.Importe.HasValue Then
                        Importe = If(pae.Importe.Value = 0, "0", CStr(pae.Importe.Value))
                    Else
                        Importe = "NULL"
                    End If

                    AntesIE = If(pae.AntesIE.HasValue, If(pae.AntesIE.Value, "1", "0"), "NULL")
                    PrecioDia = If(pae.PrecioDia, "1", "0")

#End Region
                    Funciones.InsertProductoAsignacionV2(pae.Entorno, pae.IdProductoGrupo, pae.IdProducto, pae.IdContrato, FechaInicial,
                                                       Importe, If(pae.IdTipoImpuesto, 0), AntesIE, pae.AplicarSobreConsumo, pae.AplicarPrecioConsumo, PrecioDia, FechaFinal, Plazo, PlazoCargado, ImporteTotalPlazo)
                    Contador += 1
                Next
            End If
#End Region
        Catch ex As Exception
            Throw
        Finally
            Dim mensajeErrores = ""
            If ProductoAsigExcelErrores.Count > 0 Then
                mensajeErrores = $"Además, se detectaron {ProductoAsigExcelErrores.Count} errores. Revisa el archivo de log."

                Dim archivoResultados = $"C:\Users\{NombreUsuarioEquipoP}\Desktop\ConsultasBO\ErroresImportacion_{Date.Today.Date.ToString("ddMMyyyy")}.txt"

                ' Opcional: limpiar archivo antes de escribir
                File.WriteAllText(archivoResultados, "")

                ' Escribir errores con fecha y salto de línea
                File.AppendAllText(archivoResultados, $"----- Errores encontrados el {DateTime.Now} -----{Environment.NewLine}")
                File.AppendAllText(archivoResultados, String.Join(Environment.NewLine, ProductoAsigExcelErrores))
                File.AppendAllText(archivoResultados, Environment.NewLine & Environment.NewLine)
            End If
            MessageBox.Show($"Se han insertado {Contador} registros. {mensajeErrores}")
        End Try
    End Sub

    Private Sub BotonPlantilla_Click(sender As Object, e As EventArgs) Handles BotonPlantilla.Click
        Try
            GenerarPlantilla($"C:\Users\{NombreUsuarioEquipoP}\Desktop\ConsultasBO\PlantillaProductoAsignacion.xlsx")
        Catch ex As Exception
            Throw
        Finally
            Complementos.MostrarMensajePersonalizado($"plantilla generada en C:\Users\{NombreUsuarioEquipoP}\Desktop\ConsultasBO\PlantillaProductoAsignacion.xlsx")
        End Try
    End Sub
    Public Sub GenerarPlantilla(rutaArchivo As String)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial

        Using package As New ExcelPackage()
            Dim worksheet = package.Workbook.Worksheets.Add("Plantilla")

            ' Cabeceras
            worksheet.Cells(1, 1).Value = "CodContrato"
            worksheet.Cells(1, 2).Value = "TextoProducto"
            worksheet.Cells(1, 3).Value = "FechaInicio"
            worksheet.Cells(1, 4).Value = "FechaFinal"
            worksheet.Cells(1, 5).Value = "Plazo"
            worksheet.Cells(1, 6).Value = "PlazoCargado"
            worksheet.Cells(1, 7).Value = "ImporteTotalPlazo"
            worksheet.Cells(1, 8).Value = "Importe"
            worksheet.Cells(1, 9).Value = "AntesIE(true/false)"
            worksheet.Cells(1, 10).Value = "AplicarSobreConsumo(true/false)"
            worksheet.Cells(1, 11).Value = "PrecioDia(true/false)"
            worksheet.Cells(1, 12).Value = "AplicarPrecioConsumo(true/false)"

            ' Opcional: darle estilo a cabecera
            Using range = worksheet.Cells(1, 1, 1, 12)
                range.Style.Font.Bold = True
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray)
            End Using

            ' Ajustar ancho de columnas
            worksheet.Cells.AutoFitColumns()
            worksheet.Cells("A1:B1").Style.Font.Color.SetColor(Color.Red)
            ' Guardar archivo
            Dim fi As New FileInfo(rutaArchivo)
            package.SaveAs(fi)
        End Using
    End Sub
End Class