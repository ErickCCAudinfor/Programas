<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LoadingWF
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LoadingWF))
        Load = New PictureBox()
        CType(Load, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' Load
        ' 
        Load.BackgroundImageLayout = ImageLayout.Center
        Load.Image = CType(resources.GetObject("Load.Image"), Image)
        Load.Location = New Point(12, -5)
        Load.Margin = New Padding(3, 4, 3, 4)
        Load.Name = "Load"
        Load.Size = New Size(131, 96)
        Load.TabIndex = 0
        Load.TabStop = False
        ' 
        ' LoadingWF
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = SystemColors.Window
        ClientSize = New Size(142, 85)
        Controls.Add(Load)
        FormBorderStyle = FormBorderStyle.None
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(3, 4, 3, 4)
        Name = "LoadingWF"
        Opacity = 0.7R
        StartPosition = FormStartPosition.CenterScreen
        Text = "Loading"
        TopMost = True
        CType(Load, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents Load As PictureBox
End Class
