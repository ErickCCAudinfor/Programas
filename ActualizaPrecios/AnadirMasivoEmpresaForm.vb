Imports System.IO
Imports ClosedXML.Excel

Public Class AnadirMasivoEmpresaForm
    Private _complementos As New Complementos
    Private _Empresas As List(Of EmpresaBD)
    Private _Modelo As New ModeloDeImpresion()

    Public Sub New(Empresas As List(Of EmpresaBD))
        InitializeComponent()
        Me._Empresas = Empresas
    End Sub

    Private Sub AnadirMasivoEmpresaForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InicializarComboEntorno()
        InicializarComboTipoModelo()
        CargarGridEmpresas()
    End Sub

    Private Sub CargarGridEmpresas()
        GridEmpresas.Rows.Clear()
        If _Empresas Is Nothing Then Return

        For Each emp In _Empresas
            Dim idx = GridEmpresas.Rows.Add(False, emp.Nombre, If(emp.VPN, "Sí", "No"), GetConnectionString(emp))
            GridEmpresas.Rows(idx).Tag = emp
        Next
    End Sub

    Private Function GetConnectionString(Emp1 As EmpresaBD) As String
        Dim emp As EmpresaBD = Emp1
        Dim user = CryptoHelper.Descifrar(emp.Usuario)
        Dim pass = CryptoHelper.Descifrar(emp.Password)
        Dim cc = $"Data Source={emp.Servidor};Initial Catalog={emp.BaseDatos};User ID={user};Password={pass};"
        Return cc
    End Function

    Private Sub InicializarComboEntorno()
        ComboEntorno.Items.Clear()
        ComboEntorno.Items.Add("Electricidad (G1)")
        ComboEntorno.Items.Add("Gas (G2)")
        ComboEntorno.SelectedIndex = -1
    End Sub

    Private Sub InicializarComboTipoModelo()
        ComboTipoModelo.DataSource = EnumHelper.EnumToComboBoxList(Of TipoModeloImpresionGeneral)()
        ComboTipoModelo.DisplayMember = "Text"
        ComboTipoModelo.ValueMember = "Value"
        ComboTipoModelo.SelectedIndex = -1
    End Sub

    Private Sub CheckSeleccionarTodo_CheckedChanged(sender As Object, e As EventArgs) Handles CheckSeleccionarTodo.CheckedChanged
        For Each row As DataGridViewRow In GridEmpresas.Rows
            row.Cells("ColSeleccion").Value = CheckSeleccionarTodo.Checked
        Next
        ActualizarEstadoBotones()
    End Sub

    Private Sub GridEmpresas_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles GridEmpresas.CurrentCellDirtyStateChanged
        If GridEmpresas.IsCurrentCellDirty Then
            GridEmpresas.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub GridEmpresas_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles GridEmpresas.CellValueChanged
        If e.RowIndex < 0 Then Return
        If GridEmpresas.Columns(e.ColumnIndex).Name = "ColSeleccion" Then
            ActualizarEstadoBotones()
        End If
    End Sub

    Private Function GetEmpresasSeleccionadas() As List(Of EmpresaBD)
        Dim seleccionadas As New List(Of EmpresaBD)
        For Each row As DataGridViewRow In GridEmpresas.Rows
            If CBool(If(row.Cells("ColSeleccion").Value, False)) Then
                seleccionadas.Add(CType(row.Tag, EmpresaBD))
            End If
        Next
        Return seleccionadas
    End Function

    Private Sub ActualizarEstadoBotones()
        Dim haySeleccion = GetEmpresasSeleccionadas().Count > 0
        BotonElegirReport.Enabled = haySeleccion
        BotonSubirMasivo.Enabled = haySeleccion AndAlso _Modelo.Modelo IsNot Nothing
        BotonComprobarModelo.Enabled = haySeleccion
    End Sub

    Private Sub BotonElegirReport_Click(sender As Object, e As EventArgs) Handles BotonElegirReport.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Crystal Reports (*.rpt)|*.rpt"
            ofd.Title = "Seleccionar modelo de impresión"

            If ofd.ShowDialog() = DialogResult.OK Then
                CargarModeloDesdeArchivo(ofd.FileName)
            End If
        End Using
    End Sub

    Private Sub CargarModeloDesdeArchivo(ruta As String)
        _Modelo.Modelo = File.ReadAllBytes(ruta)
        _Modelo.RptFileName = Path.GetFileName(ruta)
        LabelReport.Text = $"{_Modelo.RptFileName} ({_Modelo.Modelo.Length} bytes)"
        ActualizarEstadoBotones()
    End Sub

    Private Async Sub BotonSubirMasivo_Click(sender As Object, e As EventArgs) Handles BotonSubirMasivo.Click
        Dim seleccionadas = GetEmpresasSeleccionadas()

        If seleccionadas.Count = 0 Then
            _complementos.MostrarMensajePersonalizado("No hay empresas seleccionadas.")
            Return
        End If

        If _Modelo.Modelo Is Nothing Then
            _complementos.MostrarMensajePersonalizado("No hay ningún report cargado.")
            Return
        End If

        If ComboTipoModelo.SelectedValue Is Nothing Then
            _complementos.MostrarMensajePersonalizado("Selecciona el tipo de modelo.")
            Return
        End If

        Dim exitos As Integer = 0
        Dim fallidos As New List(Of (Empresa As String, Detalle As String))

        Try
            PrepararModeloDesdeUI()
            MostrarEstadoGuardando(True)

            For Each emp In seleccionadas
                Try
                    LabelEstado.Text = $"Subiendo a {emp.Nombre}..."
                    Dim funciones As New FuncionesGenericas(GetConnectionString(emp))
                    Dim numFilas = Await Task.Run(Function() funciones.InsertModeloImpresion(_Modelo))

                    If numFilas > 0 Then
                        exitos += 1
                    Else
                        fallidos.Add((emp.Nombre, "No se insertó el modelo."))
                    End If

                Catch ex As Exception
                    fallidos.Add((emp.Nombre, ex.Message))
                End Try
            Next

            Dim mensaje = $"Subida masiva finalizada. Éxitos: {exitos} de {seleccionadas.Count}."
            If fallidos.Count > 0 Then
                Dim rutaExcel = ExportarFallidosExcel(fallidos)
                mensaje &= Environment.NewLine & "Errores:" & Environment.NewLine &
                    String.Join(Environment.NewLine, fallidos.Select(Function(f) $"{f.Empresa}: {f.Detalle}")) &
                    Environment.NewLine & $"Listado de fallos generado en: {rutaExcel}"
            End If
            _complementos.MostrarMensajePersonalizado(mensaje)

        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)

        Finally
            MostrarEstadoGuardando(False)
        End Try
    End Sub

    Private Function ExportarFallidosExcel(fallidos As List(Of (Empresa As String, Detalle As String))) As String
        Dim escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        Dim ruta = Path.Combine(escritorio, $"FallosSubidaMasiva_{Date.Now:yyyyMMdd_HHmmss}.xlsx")

        Using wb As New XLWorkbook()
            Dim ws = wb.Worksheets.Add("Fallos")
            ws.Cell(1, 1).Value = "Empresa"
            ws.Cell(1, 2).Value = "Report"
            ws.Cell(1, 3).Value = "Error"
            ws.Range("A1:C1").Style.Font.Bold = True

            Dim fila As Integer = 2
            For Each f In fallidos
                ws.Cell(fila, 1).Value = f.Empresa
                ws.Cell(fila, 2).Value = _Modelo.RptFileName
                ws.Cell(fila, 3).Value = f.Detalle
                fila += 1
            Next

            ws.Columns().AdjustToContents()
            wb.SaveAs(ruta)
        End Using

        Return ruta
    End Function

    Private Async Sub BotonComprobarModelo_Click(sender As Object, e As EventArgs) Handles BotonComprobarModelo.Click
        Dim seleccionadas = GetEmpresasSeleccionadas()

        If seleccionadas.Count = 0 Then
            _complementos.MostrarMensajePersonalizado("No hay empresas seleccionadas.")
            Return
        End If

        If ComboTipoModelo.SelectedValue Is Nothing Then
            _complementos.MostrarMensajePersonalizado("Selecciona el tipo de modelo a comprobar.")
            Return
        End If

        If String.IsNullOrWhiteSpace(TextDescripcion.Text) Then
            _complementos.MostrarMensajePersonalizado("Indica la descripción del modelo a comprobar.")
            Return
        End If

        Dim codigoTipo = CInt(ComboTipoModelo.SelectedValue)
        Dim descripcion = TextDescripcion.Text
        Dim faltan As New List(Of String)
        Dim erroresConexion As New List(Of String)

        Try
            MostrarEstadoGuardando(True)

            For Each emp In seleccionadas
                Try
                    LabelEstado.Text = $"Comprobando {emp.Nombre}..."
                    Dim funciones As New FuncionesGenericas(GetConnectionString(emp))
                    Dim existe = Await Task.Run(Function() funciones.ExisteModeloImpresionPorTipo(codigoTipo, descripcion))

                    If Not existe Then
                        faltan.Add(emp.Nombre)
                    End If

                Catch ex As Exception
                    erroresConexion.Add($"{emp.Nombre}: {ex.Message}")
                End Try
            Next

            Dim mensaje As String
            If faltan.Count = 0 AndAlso erroresConexion.Count = 0 Then
                mensaje = $"Comprobación finalizada. Todas las empresas ({seleccionadas.Count}) tienen el modelo correcto."
            Else
                mensaje = "Comprobación finalizada."
                If faltan.Count > 0 Then
                    mensaje &= Environment.NewLine & "Empresas a las que les falta el modelo:" &
                        Environment.NewLine & String.Join(Environment.NewLine, faltan)
                End If
                If erroresConexion.Count > 0 Then
                    mensaje &= Environment.NewLine & "Errores de conexión:" &
                        Environment.NewLine & String.Join(Environment.NewLine, erroresConexion)
                End If
            End If
            _complementos.MostrarMensajePersonalizado(mensaje)

        Catch ex As Exception
            _complementos.MostrarMensajePersonalizado(ex.Message)

        Finally
            MostrarEstadoGuardando(False)
        End Try
    End Sub

    Private Sub PrepararModeloDesdeUI()
        Select Case ComboEntorno.SelectedIndex
            Case 0
                _Modelo.Entorno = "G1"
            Case 1
                _Modelo.Entorno = "G2"
            Case Else
                _Modelo.Entorno = String.Empty
        End Select
        _Modelo.DescripcionModeloDeImpresion = TextDescripcion.Text
        _Modelo.CodigoTipoModeloDeImpresion = CInt(ComboTipoModelo.SelectedValue)
        _Modelo.ClassName = TextClassName.Text
    End Sub

    Private Sub MostrarEstadoGuardando(mostrar As Boolean)
        BotonSubirMasivo.Enabled = Not mostrar
        BotonComprobarModelo.Enabled = Not mostrar
        BotonElegirReport.Enabled = Not mostrar
        GridEmpresas.Enabled = Not mostrar
        CheckSeleccionarTodo.Enabled = Not mostrar
        LabelEstado.Text = If(mostrar, "Subiendo...", String.Empty)
        If Not mostrar Then ActualizarEstadoBotones()
    End Sub

End Class
