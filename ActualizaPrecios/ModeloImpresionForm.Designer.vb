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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ModeloImpresionForm))
        BDEmpresaCombo = New ComboBox()
        BDLabel = New Label()
        DataModeloImpresionView = New DataGridView()
        AgregarBDBotton = New Button()
        RecargaModelos = New Button()
        LabelServidor = New Label()
        LoadImagen = New PictureBox()
        TextConsultando = New Label()
        BotonBackUp = New Button()
        CType(DataModeloImpresionView, ComponentModel.ISupportInitialize).BeginInit()
        CType(LoadImagen, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' BDEmpresaCombo
        ' 
        BDEmpresaCombo.FormattingEnabled = True
        BDEmpresaCombo.Location = New Point(97, 12)
        BDEmpresaCombo.Name = "BDEmpresaCombo"
        BDEmpresaCombo.Size = New Size(229, 23)
        BDEmpresaCombo.TabIndex = 0
        ' 
        ' BDLabel
        ' 
        BDLabel.AutoSize = True
        BDLabel.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        BDLabel.Location = New Point(21, 15)
        BDLabel.Name = "BDLabel"
        BDLabel.Size = New Size(71, 15)
        BDLabel.TabIndex = 1
        BDLabel.Text = "BD Empresa"
        ' 
        ' DataModeloImpresionView
        ' 
        DataModeloImpresionView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataModeloImpresionView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        DataModeloImpresionView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        DataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = Color.FromArgb(CByte(45), CByte(45), CByte(48))
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        DataGridViewCellStyle1.ForeColor = Color.White
        DataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(CByte(0), CByte(122), CByte(204))
        DataGridViewCellStyle1.SelectionForeColor = Color.White
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        DataModeloImpresionView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        DataModeloImpresionView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataModeloImpresionView.EnableHeadersVisualStyles = False
        DataModeloImpresionView.Location = New Point(21, 41)
        DataModeloImpresionView.Name = "DataModeloImpresionView"
        DataModeloImpresionView.RowHeadersVisible = False
        DataModeloImpresionView.RowTemplate.Height = 25
        DataModeloImpresionView.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        DataModeloImpresionView.Size = New Size(1288, 491)
        DataModeloImpresionView.TabIndex = 3
        ' 
        ' AgregarBDBotton
        ' 
        AgregarBDBotton.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        AgregarBDBotton.FlatAppearance.BorderColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        AgregarBDBotton.FlatStyle = FlatStyle.Popup
        AgregarBDBotton.ForeColor = SystemColors.Control
        AgregarBDBotton.Location = New Point(1197, 12)
        AgregarBDBotton.Name = "AgregarBDBotton"
        AgregarBDBotton.Size = New Size(112, 23)
        AgregarBDBotton.TabIndex = 4
        AgregarBDBotton.Text = "Agregar BD"
        AgregarBDBotton.UseVisualStyleBackColor = False
        ' 
        ' RecargaModelos
        ' 
        RecargaModelos.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(220))
        RecargaModelos.FlatAppearance.BorderColor = Color.FromArgb(CByte(25), CByte(118), CByte(220))
        RecargaModelos.FlatStyle = FlatStyle.Popup
        RecargaModelos.ForeColor = SystemColors.Control
        RecargaModelos.Location = New Point(1067, 12)
        RecargaModelos.Name = "RecargaModelos"
        RecargaModelos.Size = New Size(124, 23)
        RecargaModelos.TabIndex = 5
        RecargaModelos.Text = "Recargar Modelos"
        RecargaModelos.UseVisualStyleBackColor = False
        ' 
        ' LabelServidor
        ' 
        LabelServidor.AutoSize = True
        LabelServidor.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        LabelServidor.Location = New Point(345, 15)
        LabelServidor.Name = "LabelServidor"
        LabelServidor.Size = New Size(51, 15)
        LabelServidor.TabIndex = 6
        LabelServidor.Text = "Servidor"
        ' 
        ' LoadImagen
        ' 
        LoadImagen.Image = CType(resources.GetObject("LoadImagen.Image"), Image)
        LoadImagen.Location = New Point(988, 1)
        LoadImagen.Name = "LoadImagen"
        LoadImagen.Size = New Size(54, 36)
        LoadImagen.TabIndex = 44
        LoadImagen.TabStop = False
        LoadImagen.Visible = False
        ' 
        ' TextConsultando
        ' 
        TextConsultando.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        TextConsultando.Location = New Point(903, 12)
        TextConsultando.MaximumSize = New Size(400, 0)
        TextConsultando.Name = "TextConsultando"
        TextConsultando.Size = New Size(78, 18)
        TextConsultando.TabIndex = 45
        TextConsultando.Text = "Eliminando..."
        TextConsultando.Visible = False
        ' 
        ' BotonBackUp
        ' 
        BotonBackUp.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonBackUp.FlatAppearance.BorderColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BotonBackUp.FlatStyle = FlatStyle.Popup
        BotonBackUp.ForeColor = SystemColors.Control
        BotonBackUp.Location = New Point(21, 538)
        BotonBackUp.Name = "BotonBackUp"
        BotonBackUp.Size = New Size(112, 23)
        BotonBackUp.TabIndex = 46
        BotonBackUp.Text = "BackUp"
        BotonBackUp.UseVisualStyleBackColor = False
        ' 
        ' ModeloImpresionForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(213), CByte(220), CByte(227))
        ClientSize = New Size(1321, 569)
        Controls.Add(BotonBackUp)
        Controls.Add(TextConsultando)
        Controls.Add(LoadImagen)
        Controls.Add(LabelServidor)
        Controls.Add(RecargaModelos)
        Controls.Add(AgregarBDBotton)
        Controls.Add(DataModeloImpresionView)
        Controls.Add(BDLabel)
        Controls.Add(BDEmpresaCombo)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "ModeloImpresionForm"
        Text = "Modelo Impresion"
        CType(DataModeloImpresionView, ComponentModel.ISupportInitialize).EndInit()
        CType(LoadImagen, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

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
