Imports System.Data.SqlClient

Public Class GenerarXMLOpcionesForm

    Public Enum TipoXML
        FacturasGeneral = 1
    End Enum

    Private ReadOnly _connectionString As String

    Public Sub New(connectionString As String)
        InitializeComponent()
        _connectionString = connectionString
    End Sub

    Public Property TipoSeleccionado As TipoXML = TipoXML.FacturasGeneral
    Public Property IdFacturaVentaCabecera As Long = 0

    Private Sub BtnAceptar_Click(sender As Object, e As EventArgs) Handles BtnAceptar.Click
        Dim serie As String = TextSerieFactura.Text.Trim()
        Dim numeroStr As String = TextNumeroFactura.Text.Trim()

        If String.IsNullOrEmpty(serie) Then
            MessageBox.Show("Introduce la serie de la factura.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim numero As Long
        If Not Long.TryParse(numeroStr, numero) OrElse numero <= 0 Then
            MessageBox.Show("Introduce un número de factura válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim idFactura As Long = BuscarIdFactura(serie, numero)
        If idFactura <= 0 Then
            MessageBox.Show($"No se encontró ninguna factura con Serie='{serie}' y Número={numero}.",
                            "Factura no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If RadioFacturasGeneral.Checked Then
            TipoSeleccionado = TipoXML.FacturasGeneral
        End If

        IdFacturaVentaCabecera = idFactura
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Function BuscarIdFactura(serie As String, numero As Long) As Long
        Try
            Dim query = "SELECT IdFacturaVentaCabecera FROM FacturaVentaCabecera " &
                        $"WHERE NumeroFactura = {numero} AND SerieFactura = '{serie.Replace("'", "''")}'"
            Using conn As New SqlConnection(_connectionString)
                Using cmd As New SqlCommand(query, conn)
                    cmd.CommandTimeout = 30
                    conn.Open()
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        Return CLng(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error al buscar la factura: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return 0L
    End Function

    Private Sub BtnCancelar_Click(sender As Object, e As EventArgs) Handles BtnCancelar.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
