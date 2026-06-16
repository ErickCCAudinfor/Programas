<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GenerarXMLOpcionesForm
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        PanelHeader = New Panel()
        LabelTitulo = New Label()
        LabelTipoInforme = New Label()
        RadioContrato = New RadioButton()
        RadioCartas = New RadioButton()
        LabelFacturasGrupo = New Label()
        RadioFacturasTotal = New RadioButton()
        RadioFacturasEleia = New RadioButton()
        RadioFacturasGeneral = New RadioButton()
        PanelSeparador = New Panel()
        LabelSerieFactura = New Label()
        TextSerieFactura = New TextBox()
        LabelNumeroFactura = New Label()
        TextNumeroFactura = New TextBox()
        PanelFooter = New Panel()
        BtnAceptar = New Button()
        BtnCancelar = New Button()
        PanelHeader.SuspendLayout()
        PanelFooter.SuspendLayout()
        SuspendLayout()
        '
        ' PanelHeader
        '
        PanelHeader.BackColor = Color.FromArgb(CByte(20), CByte(55), CByte(110))
        PanelHeader.Controls.Add(LabelTitulo)
        PanelHeader.Location = New Point(0, 0)
        PanelHeader.Name = "PanelHeader"
        PanelHeader.Size = New Size(380, 55)
        PanelHeader.TabIndex = 100
        '
        ' LabelTitulo
        '
        LabelTitulo.AutoSize = True
        LabelTitulo.Font = New Font("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point)
        LabelTitulo.ForeColor = Color.White
        LabelTitulo.Location = New Point(16, 15)
        LabelTitulo.Name = "LabelTitulo"
        LabelTitulo.TabIndex = 200
        LabelTitulo.Text = "Generar XML de Report"
        '
        ' LabelTipoInforme
        '
        LabelTipoInforme.AutoSize = True
        LabelTipoInforme.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        LabelTipoInforme.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        LabelTipoInforme.Location = New Point(20, 72)
        LabelTipoInforme.Name = "LabelTipoInforme"
        LabelTipoInforme.TabIndex = 1
        LabelTipoInforme.Text = "Tipo de informe:"
        '
        ' RadioContrato
        '
        RadioContrato.AutoSize = True
        RadioContrato.Enabled = False
        RadioContrato.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        RadioContrato.ForeColor = Color.FromArgb(CByte(160), CByte(165), CByte(175))
        RadioContrato.Location = New Point(30, 98)
        RadioContrato.Name = "RadioContrato"
        RadioContrato.TabIndex = 2
        RadioContrato.Text = "Contrato"
        '
        ' RadioCartas
        '
        RadioCartas.AutoSize = True
        RadioCartas.Enabled = False
        RadioCartas.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        RadioCartas.ForeColor = Color.FromArgb(CByte(160), CByte(165), CByte(175))
        RadioCartas.Location = New Point(30, 124)
        RadioCartas.Name = "RadioCartas"
        RadioCartas.TabIndex = 3
        RadioCartas.Text = "Cartas"
        '
        ' LabelFacturasGrupo
        '
        LabelFacturasGrupo.AutoSize = True
        LabelFacturasGrupo.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        LabelFacturasGrupo.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        LabelFacturasGrupo.Location = New Point(30, 154)
        LabelFacturasGrupo.Name = "LabelFacturasGrupo"
        LabelFacturasGrupo.TabIndex = 4
        LabelFacturasGrupo.Text = "Facturas"
        '
        ' RadioFacturasTotal
        '
        RadioFacturasTotal.AutoSize = True
        RadioFacturasTotal.Enabled = False
        RadioFacturasTotal.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        RadioFacturasTotal.ForeColor = Color.FromArgb(CByte(160), CByte(165), CByte(175))
        RadioFacturasTotal.Location = New Point(50, 176)
        RadioFacturasTotal.Name = "RadioFacturasTotal"
        RadioFacturasTotal.TabIndex = 5
        RadioFacturasTotal.Text = "Total (EN)"
        '
        ' RadioFacturasEleia
        '
        RadioFacturasEleia.AutoSize = True
        RadioFacturasEleia.Enabled = False
        RadioFacturasEleia.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        RadioFacturasEleia.ForeColor = Color.FromArgb(CByte(160), CByte(165), CByte(175))
        RadioFacturasEleia.Location = New Point(50, 202)
        RadioFacturasEleia.Name = "RadioFacturasEleia"
        RadioFacturasEleia.TabIndex = 6
        RadioFacturasEleia.Text = "Eleia"
        '
        ' RadioFacturasGeneral
        '
        RadioFacturasGeneral.AutoSize = True
        RadioFacturasGeneral.Checked = True
        RadioFacturasGeneral.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        RadioFacturasGeneral.ForeColor = Color.FromArgb(CByte(25), CByte(100), CByte(200))
        RadioFacturasGeneral.Location = New Point(50, 228)
        RadioFacturasGeneral.Name = "RadioFacturasGeneral"
        RadioFacturasGeneral.TabIndex = 7
        RadioFacturasGeneral.TabStop = True
        RadioFacturasGeneral.Text = "General"
        '
        ' PanelSeparador
        '
        PanelSeparador.BackColor = Color.FromArgb(CByte(200), CByte(210), CByte(225))
        PanelSeparador.Location = New Point(20, 262)
        PanelSeparador.Name = "PanelSeparador"
        PanelSeparador.Size = New Size(340, 1)
        PanelSeparador.TabIndex = 8
        '
        ' LabelSerieFactura
        '
        LabelSerieFactura.AutoSize = True
        LabelSerieFactura.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        LabelSerieFactura.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        LabelSerieFactura.Location = New Point(20, 278)
        LabelSerieFactura.Name = "LabelSerieFactura"
        LabelSerieFactura.TabIndex = 9
        LabelSerieFactura.Text = "Serie:"
        '
        ' TextSerieFactura
        '
        TextSerieFactura.BorderStyle = BorderStyle.FixedSingle
        TextSerieFactura.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        TextSerieFactura.Location = New Point(190, 274)
        TextSerieFactura.Name = "TextSerieFactura"
        TextSerieFactura.Size = New Size(170, 23)
        TextSerieFactura.TabIndex = 10
        '
        ' LabelNumeroFactura
        '
        LabelNumeroFactura.AutoSize = True
        LabelNumeroFactura.Font = New Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point)
        LabelNumeroFactura.ForeColor = Color.FromArgb(CByte(65), CByte(80), CByte(105))
        LabelNumeroFactura.Location = New Point(20, 308)
        LabelNumeroFactura.Name = "LabelNumeroFactura"
        LabelNumeroFactura.TabIndex = 11
        LabelNumeroFactura.Text = "Número Factura:"
        '
        ' TextNumeroFactura
        '
        TextNumeroFactura.BorderStyle = BorderStyle.FixedSingle
        TextNumeroFactura.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        TextNumeroFactura.Location = New Point(190, 304)
        TextNumeroFactura.Name = "TextNumeroFactura"
        TextNumeroFactura.Size = New Size(170, 23)
        TextNumeroFactura.TabIndex = 12
        '
        ' PanelFooter
        '
        PanelFooter.BackColor = Color.FromArgb(CByte(236), CByte(242), CByte(250))
        PanelFooter.Controls.Add(BtnAceptar)
        PanelFooter.Controls.Add(BtnCancelar)
        PanelFooter.Location = New Point(0, 348)
        PanelFooter.Name = "PanelFooter"
        PanelFooter.Size = New Size(380, 46)
        PanelFooter.TabIndex = 101
        '
        ' BtnAceptar
        '
        BtnAceptar.BackColor = Color.FromArgb(CByte(25), CByte(118), CByte(210))
        BtnAceptar.Cursor = Cursors.Hand
        BtnAceptar.FlatAppearance.BorderColor = Color.FromArgb(CByte(70), CByte(150), CByte(230))
        BtnAceptar.FlatAppearance.BorderSize = 1
        BtnAceptar.FlatStyle = FlatStyle.Flat
        BtnAceptar.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        BtnAceptar.ForeColor = Color.White
        BtnAceptar.Location = New Point(195, 9)
        BtnAceptar.Name = "BtnAceptar"
        BtnAceptar.Size = New Size(85, 28)
        BtnAceptar.TabIndex = 13
        BtnAceptar.Text = "Aceptar"
        BtnAceptar.UseVisualStyleBackColor = False
        '
        ' BtnCancelar
        '
        BtnCancelar.BackColor = Color.FromArgb(CByte(100), CByte(110), CByte(130))
        BtnCancelar.Cursor = Cursors.Hand
        BtnCancelar.FlatAppearance.BorderColor = Color.FromArgb(CByte(140), CByte(150), CByte(170))
        BtnCancelar.FlatAppearance.BorderSize = 1
        BtnCancelar.FlatStyle = FlatStyle.Flat
        BtnCancelar.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        BtnCancelar.ForeColor = Color.White
        BtnCancelar.Location = New Point(288, 9)
        BtnCancelar.Name = "BtnCancelar"
        BtnCancelar.Size = New Size(85, 28)
        BtnCancelar.TabIndex = 14
        BtnCancelar.Text = "Cancelar"
        BtnCancelar.UseVisualStyleBackColor = False
        '
        ' GenerarXMLOpcionesForm
        '
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(248), CByte(250), CByte(254))
        ClientSize = New Size(380, 394)
        Controls.Add(LabelTipoInforme)
        Controls.Add(RadioContrato)
        Controls.Add(RadioCartas)
        Controls.Add(LabelFacturasGrupo)
        Controls.Add(RadioFacturasTotal)
        Controls.Add(RadioFacturasEleia)
        Controls.Add(RadioFacturasGeneral)
        Controls.Add(PanelSeparador)
        Controls.Add(LabelSerieFactura)
        Controls.Add(TextSerieFactura)
        Controls.Add(LabelNumeroFactura)
        Controls.Add(TextNumeroFactura)
        Controls.Add(PanelHeader)
        Controls.Add(PanelFooter)
        FormBorderStyle = FormBorderStyle.FixedSingle
        MaximizeBox = False
        Name = "GenerarXMLOpcionesForm"
        StartPosition = FormStartPosition.CenterParent
        Text = "Generar XML"
        PanelHeader.ResumeLayout(False)
        PanelHeader.PerformLayout()
        PanelFooter.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents PanelHeader As Panel
    Friend WithEvents LabelTitulo As Label
    Friend WithEvents LabelTipoInforme As Label
    Friend WithEvents RadioContrato As RadioButton
    Friend WithEvents RadioCartas As RadioButton
    Friend WithEvents LabelFacturasGrupo As Label
    Friend WithEvents RadioFacturasTotal As RadioButton
    Friend WithEvents RadioFacturasEleia As RadioButton
    Friend WithEvents RadioFacturasGeneral As RadioButton
    Friend WithEvents PanelSeparador As Panel
    Friend WithEvents LabelSerieFactura As Label
    Friend WithEvents TextSerieFactura As TextBox
    Friend WithEvents LabelNumeroFactura As Label
    Friend WithEvents TextNumeroFactura As TextBox
    Friend WithEvents PanelFooter As Panel
    Friend WithEvents BtnAceptar As Button
    Friend WithEvents BtnCancelar As Button

End Class
