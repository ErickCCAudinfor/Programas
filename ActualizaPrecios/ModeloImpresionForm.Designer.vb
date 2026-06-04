<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ModeloImpresionForm
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim DataGridViewCellStyle1 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As DataGridViewCellStyle = New DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ModeloImpresionForm))
        PanelHeader = New Panel()
        PanelFooter = New Panel()
        BDEmpresaCombo = New ComboBox()
        BDLabel = New Label()
        DataModeloImpresionView = New DataGridView()
        AgregarBDBotton = New Button()
        RecargaModelos = New Button()
        LabelServidor = New Label()
        LoadImagen = New PictureBox()
        TextConsultando = New Label()
        BotonBackUp = New Button()
        PanelHeader.SuspendLayout()
        PanelFooter.SuspendLayout()
        CType(DataModeloImpresionView, ComponentModel.ISupportInitialize).BeginInit()
        CType(LoadImagen, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' PanelHeader
        ' 
        PanelHeader.BackColor = Color.FromArgb(CByte(20), CByte(55), CByte(110))
        PanelHeader.Controls.Add(AgregarBDBotton)
        PanelHeader.Controls.Add(RecargaModelos)
        PanelHeader.Controls.Add(LoadImagen)
        PanelHeader.Controls.Add(TextConsultando)
        PanelHeader.Controls.Add(LabelServidor)
        PanelHeader.Controls.Add(BDEmpresaCombo)
        PanelHeader.Controls.Add(BDLabel)
        PanelHeader.Location = New Point(0, 0)
        PanelHeader.Name = "PanelHeader"
        PanelHeader.Size = New Size(1321, 60)
        PanelHeader.TabIndex = 100
        ' 
        ' BDLabel
        ' 
        BDLabel.AutoSize = True
        BDLabel.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        BDLabel.ForeColor = Color.FromArgb(CByte(200), CByte(220), CByte(255))
        BDLabel.Location = New Point(16, 22)
        BDLabel.Name = "BDLabel"
        BDLabel.Size = New Size(71, 15)
        BDLabel.TabIndex = 1
        BDLabel.Text = "BD Empresa"
        ' 
        ' BDEmpresaCombo
        ' 
        BDEmpresaCombo.FormattingEnabled = True
        BDEmpresaCombo.Location = New Point(102, 17)
        BDEmpresaCombo.Name = "BDEmpresaCombo"
        BDEmpresaCombo.Size = New Size(240, 23)
        BDEmpresaCombo.TabIndex = 0
        ' 
        ' LabelServidor
        ' 
        LabelServidor.AutoSize = True
        LabelServidor.Font = New Font("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point)
        LabelServidor.ForeColor = Color.FromArgb(CByte(175), CByte(210), CByte(255))
        LabelServidor.Location = New Point(358, 22)
        LabelServidor.Name = "LabelServidor"
        LabelServidor.Size = New Size(51, 15)
        LabelServidor.TabIndex = 6
        LabelServidor.Text = "Servidor"
        ' 
        ' TextConsultando
        ' 
        TextConsultando.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        TextConsultando.ForeColor = Color.FromArgb(CByte(255), CByte(220), CByte(80))
        TextConsultando.Location = New Point(900, 22)
        TextConsultando.MaximumSize = New Size(400, 0)
        TextConsultando.Name = "TextConsultando"
        TextConsultando.Size = New Size(90, 18)
        TextConsultando.TabIndex = 45
        TextConsultando.Text = "Eliminando..."
        TextConsultando.Visible = False
        ' 
        ' LoadImagen
        ' 
        LoadImagen.Image = CType(resources.GetObject("LoadImagen.Image"), Image)
        LoadImagen.Location = New Point(995, 12)
        LoadImagen.Name = "LoadImagen"
        LoadImagen.Size = New Size(36, 36)
        LoadImagen.SizeMode = PictureBoxSizeMode.Zoom
        LoadImagen.TabIndex = 44
        LoadImagen.TabStop = False
        LoadImagen.Visible = False
        ' 
        ' RecargaModelos
        ' 
        RecargaModelos.BackColor = Color.FromArgb(CByte(35), CByte(85), CByte(155))
        RecargaModelos.Cursor = Cursors.Hand
        RecargaModelos.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(120), CByte(190))
        RecargaModelos.FlatAppearance.BorderSize = 1
        RecargaModelos.FlatStyle = FlatStyle.Flat
        RecargaModelos.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        RecargaModelos.ForeColor = Color.White
        RecargaModelos.Location = New Point(1050, 16)
        RecargaModelos.Name = "RecargaModelos"
        RecargaModelos.Size = New Size(130, 28)
        RecargaModelos.TabIndex = 5
        RecargaModelos.Text = "Recargar Modelos"
        RecargaModelos.UseVisualStyleBackColor = False
        ' 
        ' AgregarBDBotton
        ' 
        AgregarBDBotton.BackColor = Color.FromArgb(CByte(48), CByte(125), CByte(210))
        AgregarBDBotton.Cursor = Cursors.Hand
        AgregarBDBotton.FlatAppearance.BorderColor = Color.FromArgb(CByte(90), CByte(160), CByte(235))
        AgregarBDBotton.FlatAppearance.BorderSize = 1
        AgregarBDBotton.FlatStyle = FlatStyle.Flat
        AgregarBDBotton.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        AgregarBDBotton.ForeColor = Color.White
        AgregarBDBotton.Location = New Point(1190, 16)
        AgregarBDBotton.Name = "AgregarBDBotton"
        AgregarBDBotton.Size = New Size(115, 28)
        AgregarBDBotton.TabIndex = 4
        AgregarBDBotton.Text = "Agregar BD"
        AgregarBDBotton.UseVisualStyleBackColor = False
        ' 
        ' DataModeloImpresionView
        ' 
        DataModeloImpresionView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataModeloImpresionView.BackgroundColor = Color.FromArgb(CByte(248), CByte(250), CByte(254))
        DataModeloImpresionView.BorderStyle = BorderStyle.None
        DataModeloImpresionView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        DataModeloImpresionView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataModeloImpresionView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataModeloImpresionView.EnableHeadersVisualStyles = False
        DataModeloImpresionView.GridColor = Color.FromArgb(CByte(218), CByte(228), CByte(242))
        DataModeloImpresionView.Location = New Point(0, 60)
        DataModeloImpresionView.Name = "DataModeloImpresionView"
        DataModeloImpresionView.RowHeadersVisible = False
        DataModeloImpresionView.RowTemplate.Height = 28
        DataModeloImpresionView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataModeloImpresionView.Size = New Size(1321, 454)
        DataModeloImpresionView.TabIndex = 3
        ' Column header style
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(25), CByte(65), CByte(120))
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        DataGridViewCellStyle1.ForeColor = Color.White
        DataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(CByte(0), CByte(110), CByte(195))
        DataGridViewCellStyle1.SelectionForeColor = Color.White
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        DataModeloImpresionView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        ' Default cell style
        DataGridViewCellStyle2.BackColor = Color.White
        DataGridViewCellStyle2.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        DataGridViewCellStyle2.ForeColor = Color.FromArgb(CByte(45), CByte(52), CByte(68))
        DataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        DataGridViewCellStyle2.SelectionForeColor = Color.White
        DataModeloImpresionView.DefaultCellStyle = DataGridViewCellStyle2
        ' Alternating row style
        DataGridViewCellStyle3.BackColor = Color.FromArgb(CByte(242), CByte(247), CByte(255))
        DataGridViewCellStyle3.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        DataGridViewCellStyle3.ForeColor = Color.FromArgb(CByte(45), CByte(52), CByte(68))
        DataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        DataGridViewCellStyle3.SelectionForeColor = Color.White
        DataModeloImpresionView.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle3
        ' 
        ' PanelFooter
        ' 
        PanelFooter.BackColor = Color.FromArgb(CByte(236), CByte(242), CByte(250))
        PanelFooter.Controls.Add(BotonBackUp)
        PanelFooter.Location = New Point(0, 514)
        PanelFooter.Name = "PanelFooter"
        PanelFooter.Size = New Size(1321, 55)
        PanelFooter.TabIndex = 101
        ' 
        ' BotonBackUp
        ' 
        BotonBackUp.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonBackUp.Cursor = Cursors.Hand
        BotonBackUp.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(150), CByte(230))
        BotonBackUp.FlatAppearance.BorderSize = 1
        BotonBackUp.FlatStyle = FlatStyle.Flat
        BotonBackUp.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        BotonBackUp.ForeColor = Color.White
        BotonBackUp.Location = New Point(14, 12)
        BotonBackUp.Name = "BotonBackUp"
        BotonBackUp.Size = New Size(120, 30)
        BotonBackUp.TabIndex = 46
        BotonBackUp.Text = "BackUp"
        BotonBackUp.UseVisualStyleBackColor = False
        ' 
        ' ModeloImpresionForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(248), CByte(250), CByte(254))
        ClientSize = New Size(1321, 569)
        Controls.Add(PanelHeader)
        Controls.Add(DataModeloImpresionView)
        Controls.Add(PanelFooter)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "ModeloImpresionForm"
        Text = "Modelo Impresion"
        PanelHeader.ResumeLayout(False)
        PanelHeader.PerformLayout()
        PanelFooter.ResumeLayout(False)
        PanelFooter.PerformLayout()
        CType(DataModeloImpresionView, ComponentModel.ISupportInitialize).EndInit()
        CType(LoadImagen, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents PanelHeader As Panel
    Friend WithEvents PanelFooter As Panel
    Friend WithEvents BDEmpresaCombo As ComboBox
    Friend WithEvents BDLabel As Label
    Friend WithEvents DataModeloImpresionView As DataGridView
    Friend WithEvents AgregarBDBotton As Button
    Friend WithEvents RecargaModelos As Button
    Friend WithEvents LabelServidor As Label
    Friend WithEvents LoadImagen As PictureBox
    Friend WithEvents TextConsultando As Label
    Friend WithEvents BotonBackUp As Button
End Class
