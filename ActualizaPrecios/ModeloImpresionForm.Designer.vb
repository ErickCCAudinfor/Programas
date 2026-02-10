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
        CType(DataModeloImpresionView, ComponentModel.ISupportInitialize).BeginInit()
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
        BDLabel.Location = New Point(21, 15)
        BDLabel.Name = "BDLabel"
        BDLabel.Size = New Size(70, 15)
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
        DataGridViewCellStyle1.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        DataGridViewCellStyle1.ForeColor = Color.White
        DataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(CByte(0), CByte(122), CByte(204))
        DataGridViewCellStyle1.SelectionForeColor = Color.White
        DataGridViewCellStyle1.WrapMode = DataGridViewTriState.True
        DataModeloImpresionView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        DataModeloImpresionView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataModeloImpresionView.EnableHeadersVisualStyles = False
        DataModeloImpresionView.Location = New Point(21, 41)
        DataModeloImpresionView.MultiSelect = False
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
        ' ModeloImpresionForm
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(213), CByte(220), CByte(227))
        ClientSize = New Size(1321, 552)
        Controls.Add(AgregarBDBotton)
        Controls.Add(DataModeloImpresionView)
        Controls.Add(BDLabel)
        Controls.Add(BDEmpresaCombo)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "ModeloImpresionForm"
        Text = "ModeloImpresion"
        CType(DataModeloImpresionView, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents BDEmpresaCombo As ComboBox
    Friend WithEvents BDLabel As Label
    Friend WithEvents DataModeloImpresionView As DataGridView
    Friend WithEvents AgregarBDBotton As Button
End Class
