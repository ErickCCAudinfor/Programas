Imports System.Runtime.InteropServices

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    ' Constantes para establecer el estilo del borde
    Private Const GWL_STYLE As Integer = -16
    Private Const WS_BORDER As Integer = &H800000
    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        CheckBox1 = New CheckBox()
        CheckBox2 = New CheckBox()
        CheckBox3 = New CheckBox()
        TextBox2 = New TextBox()
        Label2 = New Label()
        Label5 = New Label()
        Panel1 = New Panel()
        TextConsultando = New Label()
        LblContadorCups = New Label()
        PictureBox2 = New PictureBox()
        RadioButton2 = New RadioButton()
        RadioButton1 = New RadioButton()
        Button15 = New Button()
        Button10 = New Button()
        ToolTop1 = New ToolTip(components)
        PictureBox1 = New PictureBox()
        Timer1 = New Timer(components)
        RadioButton3 = New RadioButton()
        Button17 = New Button()
        BotonConsultar = New Button()
        DateTimePicker2 = New DateTimePicker()
        DateTimePicker3 = New DateTimePicker()
        Label8 = New Label()
        Label9 = New Label()
        lblConsultaTitulo = New Label()
        cmbConsulta = New ComboBox()
        lblNotaReplica = New Label()
        lblRequiere = New Label()
        lblParametro = New Label()
        txtParametro = New TextBox()
        lblResumenConsulta = New Label()
        lblEnCursoTitulo = New Label()
        pnlEnCurso = New Panel()
        Label4 = New Label()
        CheckBox4 = New CheckBox()
        Button7 = New Button()
        TextViejoTarifaGrupo = New TextBox()
        TextTarifaGrupo = New TextBox()
        BotonActualizar = New Button()
        Label1 = New Label()
        DateTimePicker1 = New DateTimePicker()
        Label6 = New Label()
        Button25 = New Button()
        Button6 = New Button()
        Button5 = New Button()
        Button8 = New Button()
        Button4 = New Button()
        Button3 = New Button()
        Button2 = New Button()
        Label3 = New Label()
        Button9 = New Button()
        Button11 = New Button()
        Button12 = New Button()
        Button16 = New Button()
        Button13 = New Button()
        Button14 = New Button()
        Button18 = New Button()
        Button19 = New Button()
        Button20 = New Button()
        Button21 = New Button()
        Button22 = New Button()
        Panel2 = New Panel()
        PDFBotonAgrupado = New Button()
        CheckearPerfilar = New Button()
        btnExpandir = New Button()
        Button29 = New Button()
        Button30 = New Button()
        Button28 = New Button()
        Button27 = New Button()
        BuscarFButton = New Button()
        Button24 = New Button()
        PanelLateral = New Panel()
        TrocearXMLButton = New Button()
        AplicarPreciosExcelButton = New Button()
        TimerPanel = New Timer(components)
        DividirChck = New CheckBox()
        Separador = New Panel()
        pblBorde = New Panel()
        pnlContenido = New Panel()
        Panel3 = New Panel()
        Panel4 = New Panel()
        btnNovedades = New Button()
        TimerNovedades = New Timer(components)
        Panel1.SuspendLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        Panel2.SuspendLayout()
        PanelLateral.SuspendLayout()
        pblBorde.SuspendLayout()
        pnlContenido.SuspendLayout()
        Panel3.SuspendLayout()
        Panel4.SuspendLayout()
        SuspendLayout()
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoSize = True
        CheckBox1.BackColor = Color.Transparent
        CheckBox1.Location = New Point(83, 32)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(55, 19)
        CheckBox1.TabIndex = 1
        CheckBox1.Text = "CUPS"
        CheckBox1.UseVisualStyleBackColor = False
        ' 
        ' CheckBox2
        ' 
        CheckBox2.AutoSize = True
        CheckBox2.BackColor = Color.Transparent
        CheckBox2.Location = New Point(144, 32)
        CheckBox2.Name = "CheckBox2"
        CheckBox2.Size = New Size(73, 19)
        CheckBox2.TabIndex = 2
        CheckBox2.Text = "Contrato"
        CheckBox2.UseVisualStyleBackColor = False
        ' 
        ' CheckBox3
        ' 
        CheckBox3.AutoSize = True
        CheckBox3.BackColor = Color.Transparent
        CheckBox3.Location = New Point(17, 32)
        CheckBox3.Name = "CheckBox3"
        CheckBox3.Size = New Size(63, 19)
        CheckBox3.TabIndex = 3
        CheckBox3.Text = "Cliente"
        CheckBox3.UseVisualStyleBackColor = False
        ' 
        ' TextBox2
        ' 
        TextBox2.Enabled = False
        TextBox2.HideSelection = False
        TextBox2.Location = New Point(11, 55)
        TextBox2.Multiline = True
        TextBox2.Name = "TextBox2"
        TextBox2.PlaceholderText = "Selecciona un filtro y escriba aqui para buscar: 216, 456789,231,24,...etc."
        TextBox2.ScrollBars = ScrollBars.Vertical
        TextBox2.Size = New Size(295, 119)
        TextBox2.TabIndex = 5
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.BackColor = Color.Transparent
        Label2.Font = New Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label2.Location = New Point(14, 5)
        Label2.Name = "Label2"
        Label2.Size = New Size(42, 15)
        Label2.TabIndex = 7
        Label2.Text = "Filtros"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.BorderStyle = BorderStyle.FixedSingle
        Label5.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label5.ForeColor = Color.Black
        Label5.Location = New Point(400, 20)
        Label5.Name = "Label5"
        Label5.Size = New Size(23, 17)
        Label5.TabIndex = 17
        Label5.Text = "Ip:"
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.FromArgb(CByte(232), CByte(240), CByte(252))
        Panel1.Controls.Add(TextConsultando)
        Panel1.Controls.Add(LblContadorCups)
        Panel1.Controls.Add(PictureBox2)
        Panel1.Controls.Add(Label5)
        Panel1.Controls.Add(RadioButton2)
        Panel1.Controls.Add(RadioButton1)
        Panel1.Location = New Point(21, 598)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(616, 56)
        Panel1.TabIndex = 21
        ' 
        ' TextConsultando
        ' 
        TextConsultando.Location = New Point(68, 1)
        TextConsultando.MaximumSize = New Size(400, 0)
        TextConsultando.Name = "TextConsultando"
        TextConsultando.Size = New Size(296, 35)
        TextConsultando.TabIndex = 44
        TextConsultando.Text = "Consultando..."
        TextConsultando.Visible = False
        ' 
        ' LblContadorCups
        ' 
        LblContadorCups.AutoSize = True
        LblContadorCups.ForeColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        LblContadorCups.Location = New Point(68, 38)
        LblContadorCups.Name = "LblContadorCups"
        LblContadorCups.Size = New Size(96, 15)
        LblContadorCups.TabIndex = 55
        LblContadorCups.Text = "Procesados: 0 / 0"
        LblContadorCups.Visible = False
        ' 
        ' PictureBox2
        ' 
        PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), Image)
        PictureBox2.Location = New Point(13, 0)
        PictureBox2.Name = "PictureBox2"
        PictureBox2.Size = New Size(54, 36)
        PictureBox2.TabIndex = 43
        PictureBox2.TabStop = False
        PictureBox2.Visible = False
        ' 
        ' RadioButton2
        ' 
        RadioButton2.AutoSize = True
        RadioButton2.Location = New Point(497, 1)
        RadioButton2.Name = "RadioButton2"
        RadioButton2.Size = New Size(63, 19)
        RadioButton2.TabIndex = 27
        RadioButton2.TabStop = True
        RadioButton2.Text = "Replica"
        RadioButton2.UseVisualStyleBackColor = True
        ' 
        ' RadioButton1
        ' 
        RadioButton1.AutoSize = True
        RadioButton1.Checked = True
        RadioButton1.Location = New Point(405, 1)
        RadioButton1.Name = "RadioButton1"
        RadioButton1.Size = New Size(86, 19)
        RadioButton1.TabIndex = 26
        RadioButton1.TabStop = True
        RadioButton1.Text = "Producción"
        RadioButton1.UseVisualStyleBackColor = True
        ' 
        ' Button15
        ' 
        Button15.Location = New Point(19, 204)
        Button15.Name = "Button15"
        Button15.Size = New Size(180, 23)
        Button15.TabIndex = 26
        Button15.Text = "Desglosar click Luz"
        Button15.UseVisualStyleBackColor = True
        ' 
        ' Button10
        ' 
        Button10.Location = New Point(192, 180)
        Button10.Name = "Button10"
        Button10.Size = New Size(106, 25)
        Button10.TabIndex = 25
        Button10.Text = "Limpiar"
        Button10.UseVisualStyleBackColor = True
        ' 
        ' ToolTop1
        ' 
        ToolTop1.ToolTipTitle = "Ayuda"
        ' 
        ' PictureBox1
        ' 
        PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), Image)
        PictureBox1.Location = New Point(539, 12)
        PictureBox1.Name = "PictureBox1"
        PictureBox1.Size = New Size(22, 21)
        PictureBox1.TabIndex = 20
        PictureBox1.TabStop = False
        ToolTop1.SetToolTip(PictureBox1, resources.GetString("PictureBox1.ToolTip"))
        ' 
        ' RadioButton3
        ' 
        RadioButton3.AutoSize = True
        RadioButton3.BackColor = Color.Transparent
        RadioButton3.Location = New Point(588, 599)
        RadioButton3.Name = "RadioButton3"
        RadioButton3.Size = New Size(47, 19)
        RadioButton3.TabIndex = 28
        RadioButton3.TabStop = True
        RadioButton3.Text = "UAT"
        RadioButton3.UseVisualStyleBackColor = False
        ' 
        ' Button17
        ' 
        Button17.Enabled = False
        Button17.Location = New Point(129, 346)
        Button17.Name = "Button17"
        Button17.Size = New Size(96, 23)
        Button17.TabIndex = 21
        Button17.Text = "Aplicar Precios"
        Button17.UseVisualStyleBackColor = True
        ' 
        ' BotonConsultar
        ' 
        BotonConsultar.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonConsultar.Cursor = Cursors.Hand
        BotonConsultar.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(150), CByte(230))
        BotonConsultar.FlatStyle = FlatStyle.Flat
        BotonConsultar.ForeColor = Color.White
        BotonConsultar.Location = New Point(3, 266)
        BotonConsultar.Name = "BotonConsultar"
        BotonConsultar.Size = New Size(311, 28)
        BotonConsultar.TabIndex = 29
        BotonConsultar.Text = "Consultar"
        BotonConsultar.UseVisualStyleBackColor = False
        '
        ' DateTimePicker2
        ' 
        DateTimePicker2.CustomFormat = "dd/MM/yyyy"
        DateTimePicker2.Font = New Font("Arial Narrow", 9F, FontStyle.Regular, GraphicsUnit.Point)
        DateTimePicker2.Format = DateTimePickerFormat.Custom
        DateTimePicker2.Location = New Point(177, 242)
        DateTimePicker2.Name = "DateTimePicker2"
        DateTimePicker2.Size = New Size(137, 21)
        DateTimePicker2.TabIndex = 23
        ' 
        ' DateTimePicker3
        ' 
        DateTimePicker3.CalendarMonthBackground = Color.LightBlue
        DateTimePicker3.CustomFormat = "dd/MM/yyyy"
        DateTimePicker3.Font = New Font("Arial Narrow", 9F, FontStyle.Regular, GraphicsUnit.Point)
        DateTimePicker3.Format = DateTimePickerFormat.Custom
        DateTimePicker3.Location = New Point(8, 242)
        DateTimePicker3.Name = "DateTimePicker3"
        DateTimePicker3.Size = New Size(144, 21)
        DateTimePicker3.TabIndex = 32
        ' 
        ' Label8
        ' 
        Label8.AutoSize = True
        Label8.BackColor = Color.Transparent
        Label8.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label8.Location = New Point(8, 224)
        Label8.Name = "Label8"
        Label8.Size = New Size(77, 15)
        Label8.TabIndex = 33
        Label8.Text = "Desde Fecha"
        ' 
        ' Label9
        ' 
        Label9.AutoSize = True
        Label9.BackColor = Color.Transparent
        Label9.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label9.Location = New Point(176, 223)
        Label9.Name = "Label9"
        Label9.Size = New Size(73, 15)
        Label9.TabIndex = 34
        Label9.Text = "Hasta Fecha"
        '
        ' lblConsultaTitulo
        '
        lblConsultaTitulo.AutoSize = True
        lblConsultaTitulo.BackColor = Color.Transparent
        lblConsultaTitulo.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        lblConsultaTitulo.ForeColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        lblConsultaTitulo.Location = New Point(8, 8)
        lblConsultaTitulo.Name = "lblConsultaTitulo"
        lblConsultaTitulo.Size = New Size(60, 15)
        lblConsultaTitulo.TabIndex = 55
        lblConsultaTitulo.Text = "Consulta"
        '
        ' cmbConsulta
        '
        cmbConsulta.DropDownStyle = ComboBoxStyle.DropDownList
        cmbConsulta.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        cmbConsulta.FormattingEnabled = True
        cmbConsulta.Location = New Point(8, 27)
        cmbConsulta.MaxDropDownItems = 15
        cmbConsulta.Name = "cmbConsulta"
        cmbConsulta.Size = New Size(300, 23)
        cmbConsulta.TabIndex = 56
        '
        ' lblRequiere
        '
        lblRequiere.BackColor = Color.Transparent
        lblRequiere.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point)
        lblRequiere.ForeColor = Color.FromArgb(CByte(90), CByte(115), CByte(155))
        lblRequiere.Location = New Point(8, 88)
        lblRequiere.Name = "lblRequiere"
        ' Alto para 4 líneas: la ayuda más larga es descripción + entradas + lista + dividir.
        lblRequiere.Size = New Size(300, 62)
        lblRequiere.TabIndex = 57
        '
        ' lblNotaReplica
        '
        lblNotaReplica.BackColor = Color.FromArgb(CByte(255), CByte(247), CByte(224))
        lblNotaReplica.BorderStyle = BorderStyle.FixedSingle
        lblNotaReplica.Font = New Font("Segoe UI Semibold", 8.25F, FontStyle.Bold, GraphicsUnit.Point)
        lblNotaReplica.ForeColor = Color.FromArgb(CByte(140), CByte(90), CByte(10))
        lblNotaReplica.Location = New Point(8, 54)
        lblNotaReplica.Name = "lblNotaReplica"
        lblNotaReplica.Padding = New Padding(4, 2, 2, 2)
        lblNotaReplica.Size = New Size(300, 30)
        lblNotaReplica.TabIndex = 61
        lblNotaReplica.Text = "Se recomienda lanzar las consultas contra Réplica (abajo)."
        lblNotaReplica.TextAlign = ContentAlignment.MiddleLeft
        '
        ' lblEnCursoTitulo
        '
        lblEnCursoTitulo.AutoSize = True
        lblEnCursoTitulo.BackColor = Color.Transparent
        lblEnCursoTitulo.Font = New Font("Segoe UI Semibold", 8.25F, FontStyle.Bold, GraphicsUnit.Point)
        lblEnCursoTitulo.ForeColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        lblEnCursoTitulo.Location = New Point(8, 314)
        lblEnCursoTitulo.Name = "lblEnCursoTitulo"
        lblEnCursoTitulo.Size = New Size(100, 13)
        lblEnCursoTitulo.TabIndex = 62
        lblEnCursoTitulo.Text = "En curso"
        '
        ' pnlEnCurso
        '
        pnlEnCurso.BackColor = Color.FromArgb(CByte(248), CByte(251), CByte(255))
        pnlEnCurso.Location = New Point(8, 329)
        pnlEnCurso.Name = "pnlEnCurso"
        pnlEnCurso.Size = New Size(300, 44)
        pnlEnCurso.TabIndex = 63
        '
        ' lblParametro
        '
        lblParametro.AutoSize = True
        lblParametro.BackColor = Color.Transparent
        lblParametro.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        lblParametro.Location = New Point(8, 154)
        lblParametro.Name = "lblParametro"
        lblParametro.Size = New Size(100, 15)
        lblParametro.TabIndex = 59
        lblParametro.Text = "Valor"
        lblParametro.Visible = False
        '
        ' txtParametro
        '
        txtParametro.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        txtParametro.Location = New Point(8, 171)
        txtParametro.Name = "txtParametro"
        txtParametro.Size = New Size(300, 23)
        txtParametro.TabIndex = 60
        txtParametro.Visible = False
        '
        ' lblResumenConsulta
        '
        lblResumenConsulta.BackColor = Color.Transparent
        lblResumenConsulta.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point)
        lblResumenConsulta.ForeColor = Color.FromArgb(CByte(110), CByte(135), CByte(175))
        lblResumenConsulta.Location = New Point(8, 297)
        lblResumenConsulta.Name = "lblResumenConsulta"
        lblResumenConsulta.Size = New Size(300, 14)
        lblResumenConsulta.TabIndex = 58
        '
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label4.Location = New Point(10, 271)
        Label4.Name = "Label4"
        Label4.Size = New Size(198, 15)
        Label4.TabIndex = 16
        Label4.Text = "Revisar Tarifa Precio Personalizada"
        ' 
        ' CheckBox4
        ' 
        CheckBox4.AutoSize = True
        CheckBox4.Checked = True
        CheckBox4.CheckState = CheckState.Checked
        CheckBox4.Location = New Point(208, 269)
        CheckBox4.Name = "CheckBox4"
        CheckBox4.Size = New Size(98, 19)
        CheckBox4.TabIndex = 19
        CheckBox4.Text = "Personalizada"
        CheckBox4.UseVisualStyleBackColor = True
        ' 
        ' Button7
        ' 
        Button7.Location = New Point(16, 293)
        Button7.Name = "Button7"
        Button7.Size = New Size(103, 23)
        Button7.TabIndex = 15
        Button7.Text = "Revisar"
        Button7.UseVisualStyleBackColor = True
        ' 
        ' TextViejoTarifaGrupo
        ' 
        TextViejoTarifaGrupo.Enabled = False
        TextViejoTarifaGrupo.Location = New Point(10, 245)
        TextViejoTarifaGrupo.Name = "TextViejoTarifaGrupo"
        TextViejoTarifaGrupo.PlaceholderText = "Tarifa grupo actual..."
        TextViejoTarifaGrupo.Size = New Size(299, 23)
        TextViejoTarifaGrupo.TabIndex = 20
        ' 
        ' TextTarifaGrupo
        ' 
        TextTarifaGrupo.Enabled = False
        TextTarifaGrupo.Location = New Point(10, 216)
        TextTarifaGrupo.Name = "TextTarifaGrupo"
        TextTarifaGrupo.PlaceholderText = "Ingrese la nueva tarifa grupo"
        TextTarifaGrupo.Size = New Size(299, 23)
        TextTarifaGrupo.TabIndex = 4
        ' 
        ' BotonActualizar
        ' 
        BotonActualizar.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonActualizar.Cursor = Cursors.Hand
        BotonActualizar.Enabled = False
        BotonActualizar.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(150), CByte(230))
        BotonActualizar.FlatStyle = FlatStyle.Flat
        BotonActualizar.ForeColor = Color.White
        BotonActualizar.Location = New Point(125, 293)
        BotonActualizar.Name = "BotonActualizar"
        BotonActualizar.Size = New Size(181, 23)
        BotonActualizar.TabIndex = 0
        BotonActualizar.Text = "Actualizar"
        BotonActualizar.UseVisualStyleBackColor = False
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label1.Location = New Point(11, 202)
        Label1.Name = "Label1"
        Label1.Size = New Size(76, 15)
        Label1.TabIndex = 6
        Label1.Text = "Tarifa Grupo"
        ' 
        ' DateTimePicker1
        ' 
        DateTimePicker1.CustomFormat = "dd/MM/yyyy"
        DateTimePicker1.Format = DateTimePickerFormat.Custom
        DateTimePicker1.Location = New Point(15, 346)
        DateTimePicker1.Name = "DateTimePicker1"
        DateTimePicker1.Size = New Size(109, 23)
        DateTimePicker1.TabIndex = 21
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        Label6.Location = New Point(11, 328)
        Label6.Name = "Label6"
        Label6.Size = New Size(263, 15)
        Label6.TabIndex = 22
        Label6.Text = "Se necesita el IdContratoTarifa para ésta parte"
        ' 
        ' Button25
        ' 
        Button25.Image = CType(resources.GetObject("Button25.Image"), Image)
        Button25.ImageAlign = ContentAlignment.BottomLeft
        Button25.Location = New Point(429, 38)
        Button25.Name = "Button25"
        Button25.Padding = New Padding(10, 0, 35, 0)
        Button25.Size = New Size(180, 23)
        Button25.TabIndex = 33
        Button25.Text = "Masivo Contrato"
        Button25.TextAlign = ContentAlignment.BottomRight
        Button25.UseVisualStyleBackColor = True
        ' 
        ' Button6
        ' 
        Button6.Location = New Point(19, 175)
        Button6.Name = "Button6"
        Button6.Size = New Size(180, 23)
        Button6.TabIndex = 13
        Button6.Text = "Actualizar CNAE Excel"
        Button6.UseVisualStyleBackColor = True
        ' 
        ' Button5
        ' 
        Button5.Image = CType(resources.GetObject("Button5.Image"), Image)
        Button5.ImageAlign = ContentAlignment.BottomLeft
        Button5.Location = New Point(13, 96)
        Button5.Name = "Button5"
        Button5.Padding = New Padding(10, 0, 35, 0)
        Button5.Size = New Size(180, 23)
        Button5.TabIndex = 12
        Button5.Text = "Añadir CodigosDIR"
        Button5.TextAlign = ContentAlignment.MiddleRight
        Button5.UseVisualStyleBackColor = True
        ' 
        ' Button8
        ' 
        Button8.Image = CType(resources.GetObject("Button8.Image"), Image)
        Button8.ImageAlign = ContentAlignment.BottomLeft
        Button8.Location = New Point(229, 97)
        Button8.Name = "Button8"
        Button8.Size = New Size(180, 23)
        Button8.TabIndex = 18
        Button8.Text = "Act. Email/Movil/tlfno  Excel"
        Button8.TextAlign = ContentAlignment.MiddleRight
        Button8.UseVisualStyleBackColor = True
        ' 
        ' Button4
        ' 
        Button4.Image = CType(resources.GetObject("Button4.Image"), Image)
        Button4.ImageAlign = ContentAlignment.BottomLeft
        Button4.Location = New Point(429, 96)
        Button4.Name = "Button4"
        Button4.Padding = New Padding(15, 0, 45, 0)
        Button4.Size = New Size(180, 23)
        Button4.TabIndex = 11
        Button4.Text = "Validaciones"
        Button4.TextAlign = ContentAlignment.MiddleRight
        Button4.UseVisualStyleBackColor = True
        ' 
        ' Button3
        ' 
        Button3.Image = CType(resources.GetObject("Button3.Image"), Image)
        Button3.ImageAlign = ContentAlignment.BottomLeft
        Button3.Location = New Point(13, 67)
        Button3.Name = "Button3"
        Button3.Padding = New Padding(10, 0, 55, 0)
        Button3.Size = New Size(180, 23)
        Button3.TabIndex = 10
        Button3.Text = "Volver a renovar"
        Button3.TextAlign = ContentAlignment.MiddleRight
        Button3.UseVisualStyleBackColor = True
        ' 
        ' Button2
        ' 
        Button2.Font = New Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point)
        Button2.Image = CType(resources.GetObject("Button2.Image"), Image)
        Button2.ImageAlign = ContentAlignment.BottomLeft
        Button2.Location = New Point(14, 38)
        Button2.Name = "Button2"
        Button2.Padding = New Padding(10, 0, 0, 0)
        Button2.Size = New Size(181, 23)
        Button2.TabIndex = 9
        Button2.Text = "Añadir Productos Contratos"
        Button2.TextAlign = ContentAlignment.BottomRight
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(13, 18)
        Label3.Name = "Label3"
        Label3.Size = New Size(86, 15)
        Label3.TabIndex = 8
        Label3.Text = "Otras opciones"
        ' 
        ' Button9
        ' 
        Button9.Image = CType(resources.GetObject("Button9.Image"), Image)
        Button9.ImageAlign = ContentAlignment.BottomLeft
        Button9.Location = New Point(13, 125)
        Button9.Name = "Button9"
        Button9.Padding = New Padding(10, 0, 0, 0)
        Button9.Size = New Size(180, 23)
        Button9.TabIndex = 21
        Button9.Text = "Cambiar Agente Contrato"
        Button9.TextAlign = ContentAlignment.MiddleRight
        Button9.UseVisualStyleBackColor = True
        ' 
        ' Button11
        ' 
        Button11.Image = CType(resources.GetObject("Button11.Image"), Image)
        Button11.ImageAlign = ContentAlignment.BottomLeft
        Button11.Location = New Point(13, 153)
        Button11.Name = "Button11"
        Button11.Padding = New Padding(10, 0, 0, 0)
        Button11.Size = New Size(180, 23)
        Button11.TabIndex = 22
        Button11.Text = "Cambiar Administrador Contrato"
        Button11.UseVisualStyleBackColor = True
        ' 
        ' Button12
        ' 
        Button12.Font = New Font("Segoe UI", 7F, FontStyle.Regular, GraphicsUnit.Point)
        Button12.Image = CType(resources.GetObject("Button12.Image"), Image)
        Button12.ImageAlign = ContentAlignment.BottomLeft
        Button12.Location = New Point(229, 126)
        Button12.Name = "Button12"
        Button12.Size = New Size(81, 23)
        Button12.TabIndex = 23
        Button12.Text = "Extraer PDF Facs"
        Button12.TextAlign = ContentAlignment.MiddleRight
        Button12.UseVisualStyleBackColor = True
        ' 
        ' Button16
        ' 
        Button16.Image = CType(resources.GetObject("Button16.Image"), Image)
        Button16.ImageAlign = ContentAlignment.BottomLeft
        Button16.Location = New Point(429, 67)
        Button16.Name = "Button16"
        Button16.Padding = New Padding(10, 0, 10, 0)
        Button16.Size = New Size(180, 23)
        Button16.TabIndex = 26
        Button16.Text = "Penalizaciones"
        Button16.UseVisualStyleBackColor = True
        ' 
        ' Button13
        ' 
        Button13.Location = New Point(20, 9)
        Button13.Name = "Button13"
        Button13.Size = New Size(180, 23)
        Button13.TabIndex = 24
        Button13.Text = "OpenItems"
        Button13.UseVisualStyleBackColor = True
        ' 
        ' Button14
        ' 
        Button14.Location = New Point(18, 323)
        Button14.Name = "Button14"
        Button14.Size = New Size(180, 23)
        Button14.TabIndex = 25
        Button14.Text = "Extraer CSV Varios"
        Button14.UseVisualStyleBackColor = True
        ' 
        ' Button18
        ' 
        Button18.Image = CType(resources.GetObject("Button18.Image"), Image)
        Button18.ImageAlign = ContentAlignment.BottomLeft
        Button18.Location = New Point(229, 40)
        Button18.Name = "Button18"
        Button18.Padding = New Padding(10, 0, 55, 0)
        Button18.Size = New Size(180, 23)
        Button18.TabIndex = 27
        Button18.Text = "Consulta TOP"
        Button18.TextAlign = ContentAlignment.MiddleRight
        Button18.UseVisualStyleBackColor = True
        ' 
        ' Button19
        ' 
        Button19.Image = CType(resources.GetObject("Button19.Image"), Image)
        Button19.ImageAlign = ContentAlignment.BottomLeft
        Button19.Location = New Point(229, 67)
        Button19.Name = "Button19"
        Button19.Padding = New Padding(10, 0, 35, 0)
        Button19.Size = New Size(180, 23)
        Button19.TabIndex = 29
        Button19.Text = "Verificar Licitacion"
        Button19.TextAlign = ContentAlignment.MiddleRight
        Button19.UseVisualStyleBackColor = True
        ' 
        ' Button20
        ' 
        Button20.Enabled = False
        Button20.Location = New Point(19, 62)
        Button20.Name = "Button20"
        Button20.Size = New Size(180, 23)
        Button20.TabIndex = 30
        Button20.Text = "ConsultaCAE"
        Button20.UseVisualStyleBackColor = True
        ' 
        ' Button21
        ' 
        Button21.Location = New Point(20, 88)
        Button21.Name = "Button21"
        Button21.Size = New Size(180, 23)
        Button21.TabIndex = 31
        Button21.Text = "Aña. Masv. Calendario Tarifa"
        Button21.UseVisualStyleBackColor = True
        ' 
        ' Button22
        ' 
        Button22.Enabled = False
        Button22.Location = New Point(20, 117)
        Button22.Name = "Button22"
        Button22.Size = New Size(180, 23)
        Button22.TabIndex = 32
        Button22.Text = "GenerarXML"
        Button22.UseVisualStyleBackColor = True
        ' 
        ' Panel2
        ' 
        Panel2.BackColor = Color.FromArgb(CByte(232), CByte(240), CByte(252))
        Panel2.Controls.Add(PDFBotonAgrupado)
        Panel2.Controls.Add(CheckearPerfilar)
        Panel2.Controls.Add(Button2)
        Panel2.Controls.Add(btnExpandir)
        Panel2.Controls.Add(Button29)
        Panel2.Controls.Add(Button25)
        Panel2.Controls.Add(Button19)
        Panel2.Controls.Add(Button18)
        Panel2.Controls.Add(Button16)
        Panel2.Controls.Add(Button12)
        Panel2.Controls.Add(Button11)
        Panel2.Controls.Add(Button9)
        Panel2.Controls.Add(PictureBox1)
        Panel2.Controls.Add(Label3)
        Panel2.Controls.Add(Button3)
        Panel2.Controls.Add(Button4)
        Panel2.Controls.Add(Button8)
        Panel2.Controls.Add(Button5)
        Panel2.Location = New Point(20, 400)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(617, 185)
        Panel2.TabIndex = 22
        ' 
        ' PDFBotonAgrupado
        ' 
        PDFBotonAgrupado.Font = New Font("Segoe UI", 6F, FontStyle.Regular, GraphicsUnit.Point)
        PDFBotonAgrupado.Image = CType(resources.GetObject("PDFBotonAgrupado.Image"), Image)
        PDFBotonAgrupado.ImageAlign = ContentAlignment.BottomLeft
        PDFBotonAgrupado.Location = New Point(229, 152)
        PDFBotonAgrupado.Name = "PDFBotonAgrupado"
        PDFBotonAgrupado.Size = New Size(180, 23)
        PDFBotonAgrupado.TabIndex = 47
        PDFBotonAgrupado.Text = "Extraer PDF Facs Agrupado NumPedido"
        PDFBotonAgrupado.UseVisualStyleBackColor = True
        ' 
        ' CheckearPerfilar
        ' 
        CheckearPerfilar.Image = CType(resources.GetObject("CheckearPerfilar.Image"), Image)
        CheckearPerfilar.ImageAlign = ContentAlignment.BottomLeft
        CheckearPerfilar.Location = New Point(430, 123)
        CheckearPerfilar.Name = "CheckearPerfilar"
        CheckearPerfilar.Padding = New Padding(10, 0, 45, 0)
        CheckearPerfilar.Size = New Size(180, 23)
        CheckearPerfilar.TabIndex = 46
        CheckearPerfilar.Text = "Checkear Perfilar"
        CheckearPerfilar.TextAlign = ContentAlignment.MiddleRight
        CheckearPerfilar.UseVisualStyleBackColor = True
        ' 
        ' btnExpandir
        ' 
        btnExpandir.Image = CType(resources.GetObject("btnExpandir.Image"), Image)
        btnExpandir.ImageAlign = ContentAlignment.BottomLeft
        btnExpandir.Location = New Point(430, 152)
        btnExpandir.Name = "btnExpandir"
        btnExpandir.Padding = New Padding(10, 0, 45, 0)
        btnExpandir.Size = New Size(180, 23)
        btnExpandir.TabIndex = 45
        btnExpandir.Text = "Otras opciones"
        btnExpandir.TextAlign = ContentAlignment.MiddleRight
        btnExpandir.UseVisualStyleBackColor = True
        ' 
        ' Button29
        ' 
        Button29.Font = New Font("Segoe UI", 7.25F, FontStyle.Regular, GraphicsUnit.Point)
        Button29.Location = New Point(311, 125)
        Button29.Name = "Button29"
        Button29.Size = New Size(99, 23)
        Button29.TabIndex = 38
        Button29.Text = "Extr. PDF Cliente"
        Button29.UseVisualStyleBackColor = True
        ' 
        ' Button30
        ' 
        Button30.Location = New Point(19, 236)
        Button30.Name = "Button30"
        Button30.Size = New Size(180, 23)
        Button30.TabIndex = 46
        Button30.Text = "Extraer Nomb Fac FichDev"
        Button30.UseVisualStyleBackColor = True
        ' 
        ' Button28
        ' 
        Button28.Location = New Point(92, 35)
        Button28.Name = "Button28"
        Button28.Size = New Size(58, 23)
        Button28.TabIndex = 37
        Button28.Text = "S. Items"
        Button28.UseVisualStyleBackColor = True
        ' 
        ' Button27
        ' 
        Button27.Location = New Point(147, 35)
        Button27.Name = "Button27"
        Button27.Size = New Size(53, 23)
        Button27.TabIndex = 36
        Button27.Text = "Copiar"
        Button27.UseVisualStyleBackColor = True
        ' 
        ' BuscarFButton
        ' 
        BuscarFButton.Location = New Point(20, 35)
        BuscarFButton.Name = "BuscarFButton"
        BuscarFButton.Size = New Size(70, 23)
        BuscarFButton.TabIndex = 35
        BuscarFButton.Text = "B.  DEVOL"
        BuscarFButton.UseVisualStyleBackColor = True
        ' 
        ' Button24
        ' 
        Button24.Location = New Point(19, 146)
        Button24.Name = "Button24"
        Button24.Size = New Size(180, 23)
        Button24.TabIndex = 34
        Button24.Text = "Extraer Documentos Generales"
        Button24.UseVisualStyleBackColor = True
        ' 
        ' PanelLateral
        ' 
        PanelLateral.BackColor = Color.FromArgb(CByte(20), CByte(55), CByte(110))
        PanelLateral.Controls.Add(TrocearXMLButton)
        PanelLateral.Controls.Add(AplicarPreciosExcelButton)
        PanelLateral.Controls.Add(Button30)
        PanelLateral.Controls.Add(Button13)
        PanelLateral.Controls.Add(Button28)
        PanelLateral.Controls.Add(BuscarFButton)
        PanelLateral.Controls.Add(Button24)
        PanelLateral.Controls.Add(Button14)
        PanelLateral.Controls.Add(Button27)
        PanelLateral.Controls.Add(Button20)
        PanelLateral.Controls.Add(Button22)
        PanelLateral.Controls.Add(Button21)
        PanelLateral.Controls.Add(Button6)
        PanelLateral.Controls.Add(Button15)
        PanelLateral.Dock = DockStyle.Left
        PanelLateral.Location = New Point(0, 0)
        PanelLateral.Name = "PanelLateral"
        PanelLateral.Size = New Size(0, 657)
        PanelLateral.TabIndex = 45
        ' 
        ' TrocearXMLButton
        ' 
        TrocearXMLButton.ImageAlign = ContentAlignment.BottomCenter
        TrocearXMLButton.Location = New Point(18, 294)
        TrocearXMLButton.Name = "TrocearXMLButton"
        TrocearXMLButton.Padding = New Padding(10, 0, 35, 0)
        TrocearXMLButton.Size = New Size(180, 23)
        TrocearXMLButton.TabIndex = 47
        TrocearXMLButton.Text = "Trocear XML"
        TrocearXMLButton.UseVisualStyleBackColor = True
        ' 
        ' AplicarPreciosExcelButton
        ' 
        AplicarPreciosExcelButton.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        AplicarPreciosExcelButton.ForeColor = Color.Black
        AplicarPreciosExcelButton.Location = New Point(18, 265)
        AplicarPreciosExcelButton.Name = "AplicarPreciosExcelButton"
        AplicarPreciosExcelButton.Size = New Size(181, 23)
        AplicarPreciosExcelButton.TabIndex = 26
        AplicarPreciosExcelButton.Text = "Aplicar Precios Excel"
        AplicarPreciosExcelButton.UseVisualStyleBackColor = True
        ' 
        ' TimerPanel
        ' 
        TimerPanel.Interval = 10
        ' 
        ' DividirChck
        ' 
        DividirChck.AutoSize = True
        DividirChck.BackColor = Color.Transparent
        DividirChck.Font = New Font("Segoe UI Semibold", 8.25F, FontStyle.Bold, GraphicsUnit.Point)
        DividirChck.ForeColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        DividirChck.Location = New Point(8, 200)
        DividirChck.Name = "DividirChck"
        DividirChck.Size = New Size(160, 17)
        DividirChck.TabIndex = 47
        DividirChck.Text = "Dividir Excel"
        DividirChck.UseVisualStyleBackColor = False
        '
        ' Separador
        ' 
        Separador.BackColor = Color.FromArgb(CByte(218), CByte(220), CByte(224))
        Separador.BorderStyle = BorderStyle.FixedSingle
        Separador.Location = New Point(3, 26)
        Separador.Name = "Separador"
        Separador.Size = New Size(310, 1)
        Separador.TabIndex = 0
        ' 
        ' pblBorde
        ' 
        pblBorde.BackColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        pblBorde.Controls.Add(pnlContenido)
        pblBorde.Location = New Point(18, 9)
        pblBorde.Name = "pblBorde"
        pblBorde.Padding = New Padding(1)
        pblBorde.Size = New Size(319, 381)
        pblBorde.TabIndex = 51
        ' 
        ' pnlContenido
        ' 
        pnlContenido.BackColor = Color.White
        pnlContenido.Controls.Add(Label6)
        pnlContenido.Controls.Add(Label2)
        pnlContenido.Controls.Add(TextBox2)
        pnlContenido.Controls.Add(Button17)
        pnlContenido.Controls.Add(DateTimePicker1)
        pnlContenido.Controls.Add(Label4)
        pnlContenido.Controls.Add(Button10)
        pnlContenido.Controls.Add(CheckBox2)
        pnlContenido.Controls.Add(Label1)
        pnlContenido.Controls.Add(CheckBox4)
        pnlContenido.Controls.Add(BotonActualizar)
        pnlContenido.Controls.Add(CheckBox1)
        pnlContenido.Controls.Add(Separador)
        pnlContenido.Controls.Add(Button7)
        pnlContenido.Controls.Add(TextTarifaGrupo)
        pnlContenido.Controls.Add(CheckBox3)
        pnlContenido.Controls.Add(TextViejoTarifaGrupo)
        pnlContenido.Dock = DockStyle.Fill
        pnlContenido.Location = New Point(1, 1)
        pnlContenido.Name = "pnlContenido"
        pnlContenido.Size = New Size(317, 379)
        pnlContenido.TabIndex = 0
        ' 
        ' Panel3
        ' 
        Panel3.BackColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        Panel3.Controls.Add(Panel4)
        Panel3.Location = New Point(340, 15)
        Panel3.Name = "Panel3"
        Panel3.Padding = New Padding(1)
        Panel3.Size = New Size(319, 375)
        Panel3.TabIndex = 52
        ' 
        ' Panel4
        ' 
        Panel4.BackColor = Color.White
        Panel4.Controls.Add(btnNovedades)
        Panel4.Controls.Add(lblConsultaTitulo)
        Panel4.Controls.Add(cmbConsulta)
        Panel4.Controls.Add(lblNotaReplica)
        Panel4.Controls.Add(lblRequiere)
        Panel4.Controls.Add(lblParametro)
        Panel4.Controls.Add(txtParametro)
        Panel4.Controls.Add(DividirChck)
        Panel4.Controls.Add(lblResumenConsulta)
        Panel4.Controls.Add(lblEnCursoTitulo)
        Panel4.Controls.Add(pnlEnCurso)
        Panel4.Controls.Add(BotonConsultar)
        Panel4.Controls.Add(Label9)
        Panel4.Controls.Add(Label8)
        Panel4.Controls.Add(DateTimePicker3)
        Panel4.Controls.Add(DateTimePicker2)
        Panel4.Dock = DockStyle.Fill
        Panel4.Location = New Point(1, 1)
        Panel4.Name = "Panel4"
        Panel4.Size = New Size(317, 373)
        Panel4.TabIndex = 0
        '
        ' btnNovedades
        ' 
        btnNovedades.Cursor = Cursors.Hand
        btnNovedades.FlatStyle = FlatStyle.Flat
        btnNovedades.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        btnNovedades.Location = New Point(168, 3)
        btnNovedades.Name = "btnNovedades"
        btnNovedades.Size = New Size(140, 22)
        btnNovedades.TabIndex = 55
        btnNovedades.Text = "Novedades"
        btnNovedades.UseVisualStyleBackColor = False
        ' 
        ' TimerNovedades
        ' 
        TimerNovedades.Interval = 550
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(245), CByte(248), CByte(252))
        ClientSize = New Size(659, 657)
        Controls.Add(Panel3)
        Controls.Add(pblBorde)
        Controls.Add(PanelLateral)
        Controls.Add(RadioButton3)
        Controls.Add(Panel2)
        Controls.Add(Panel1)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        ' Sin maximizar: el contenido tiene posiciones fijas y al expandir queda descolocado.
        MaximizeBox = False
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Gestor de Datos SIGE"
        TransparencyKey = Color.YellowGreen
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        CType(PictureBox2, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureBox1, ComponentModel.ISupportInitialize).EndInit()
        Panel2.ResumeLayout(False)
        Panel2.PerformLayout()
        PanelLateral.ResumeLayout(False)
        pblBorde.ResumeLayout(False)
        pnlContenido.ResumeLayout(False)
        pnlContenido.PerformLayout()
        Panel3.ResumeLayout(False)
        Panel4.ResumeLayout(False)
        Panel4.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents CheckBox2 As CheckBox
    Friend WithEvents CheckBox3 As CheckBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Panel1 As Panel

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        ' Dibujar el borde personalizado
        Dim borderColor As Color = Color.FromArgb(191, 24, 27)
        Dim borderThickness As Integer = 2
        Dim rect As New Rectangle(0, 0, Me.ClientSize.Width - 1, Me.ClientSize.Height - 1)
        Dim borderPen As New Pen(borderColor, borderThickness)
        e.Graphics.DrawRectangle(borderPen, rect)
    End Sub
    Friend WithEvents ToolTop1 As ToolTip
    Friend WithEvents Timer1 As Timer
    Friend WithEvents Button10 As Button
    Friend WithEvents Button15 As Button
    Friend WithEvents RadioButton2 As RadioButton
    Friend WithEvents RadioButton1 As RadioButton
    Friend WithEvents RadioButton3 As RadioButton
    Friend WithEvents Button17 As Button
    Friend WithEvents BotonConsultar As Button
    Friend WithEvents DateTimePicker2 As DateTimePicker
    Friend WithEvents DateTimePicker3 As DateTimePicker
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents lblConsultaTitulo As Label
    Friend WithEvents cmbConsulta As ComboBox
    Friend WithEvents lblRequiere As Label
    Friend WithEvents lblNotaReplica As Label
    Friend WithEvents lblParametro As Label
    Friend WithEvents txtParametro As TextBox
    Friend WithEvents lblResumenConsulta As Label
    Friend WithEvents lblEnCursoTitulo As Label
    Friend WithEvents pnlEnCurso As Panel
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents Label4 As Label
    Friend WithEvents CheckBox4 As CheckBox
    Friend WithEvents Button7 As Button
    Friend WithEvents TextViejoTarifaGrupo As TextBox
    Friend WithEvents TextTarifaGrupo As TextBox
    Friend WithEvents BotonActualizar As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents DateTimePicker1 As DateTimePicker
    Friend WithEvents Label6 As Label
    Friend WithEvents Button25 As Button
    Friend WithEvents Button6 As Button
    Friend WithEvents Button5 As Button
    Friend WithEvents Button8 As Button
    Friend WithEvents Button4 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Button9 As Button
    Friend WithEvents Button11 As Button
    Friend WithEvents Button12 As Button
    Friend WithEvents Button16 As Button
    Friend WithEvents Button13 As Button
    Friend WithEvents Button14 As Button
    Friend WithEvents Button18 As Button
    Friend WithEvents Button19 As Button
    Friend WithEvents Button20 As Button
    Friend WithEvents Button21 As Button
    Friend WithEvents Button22 As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Button24 As Button
    Friend WithEvents BuscarFButton As Button
    Friend WithEvents Button27 As Button
    Friend WithEvents Button28 As Button
    Friend WithEvents Button29 As Button
    Friend WithEvents btnExpandir As Button
    Friend WithEvents PanelLateral As Panel
    Friend WithEvents TimerPanel As Timer
    Friend WithEvents Button30 As Button
    Friend WithEvents DividirChck As CheckBox
    Friend WithEvents TextConsultando As Label
    Friend WithEvents LblContadorCups As Label
    Friend WithEvents CheckearPerfilar As Button
    Friend WithEvents Separador As Panel
    Friend WithEvents pblBorde As Panel
    Friend WithEvents pnlContenido As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Panel4 As Panel
    Friend WithEvents btnNovedades As Button
    Friend WithEvents TimerNovedades As Timer
    Friend WithEvents AplicarPreciosExcelButton As Button
    Friend WithEvents TrocearXMLButton As Button
    Friend WithEvents PDFBotonAgrupado As Button
End Class
