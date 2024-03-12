Imports System.Data.SqlClient
Imports System.IO

Public Class TablaRevisaPreciosPersonalizados
    Private ReadOnly Property connectionString As String
    Public Sub New(connectionString)
        Try
            InitializeComponent()
            Me.connectionString = connectionString
        Catch ex As Exception
            Throw
        End Try
    End Sub
    Public Sub cargar()
        Try
            Dim dataTable As New DataTable()
            Dim listasQuerys As New List(Of String)
            Dim EscribeExcel As New ValidacionExcel(connectionString)
            Dim query = $";with idcl as (select ct.IdContratoTarifa,ct.IdTarifaGrupo from tarifapreciocontrato tf
inner join ContratoTarifa ct on ct.idcontratotarifa = tf.idcontratotarifa
where tf.IdIndexadoPrecio in (select IdIndexadoPrecio from IndexadoPrecio where idtarifagrupo in (
select IdTarifaGrupo from TarifaGrupo where textotarifagrupo like '%persona%' and IdTarifa>=202020)
) and ct.idtarifagrupo not in (select IdTarifaGrupo from TarifaGrupo where textotarifagrupo like '%persona%' and IdTarifa>=202020))

select ct.idcontratotarifa, ct.codigocontrato,tg.IdTarifaGrupo,tg.textotarifagrupo, pf.TextoPerfilFacturacion, t.IdTarifa,t.TextoTarifa,ct.FechaDesde,ct.FechaHasta,Per.TextoTarifaGrupo,Per.IdTarifaGrupo,Per.IdContratoTarifa  from ContratoTarifa ct
left join TarifaGrupo tg on ct.idtarifagrupo = tg.idtarifagrupo
left join perfilfacturacion pf on ct.idperfilfacturacion = pf.idperfilfacturacion
left join tarifa t  on ct.idtarifa = t.idtarifa
inner join (
select IdContratoTarifa,tg.TextoTarifaGrupo,tg.IdTarifaGrupo from  tarifapreciocontrato tpc
left join IndexadoPrecio inp on tpc.IdIndexadoPrecio = inp.IdIndexadoPrecio
left join tarifagrupo tg on inp.idtarifagrupo = tg.idtarifagrupo 
where  tpc.idcontratotarifa in (select IdContratoTarifa from idcl)
group by IdContratoTarifa,tg.TextoTarifaGrupo,tg.IdTarifaGrupo
) Per on ct.IdContratoTarifa = Per.IdContratoTarifa"
            listasQuerys.Add(query)

            Using connection As New SqlConnection(connectionString)
                ' Abrir la conexión
                connection.Open()

                ' Crear un comando SQL para ejecutar la consulta
                Using command As New SqlCommand(query, connection)
                    Using adapter As New SqlDataAdapter(command)
                        ' Llenar el DataTable con los resultados de la consulta
                        adapter.Fill(dataTable)
                    End Using
                End Using
            End Using
            DataGridView1.DataSource = dataTable
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Try
            ' Tu código para recuperar los datos aquí...
            ' Ajustar automáticamente el tamaño de las columnas
            DataGridView1.AutoResizeColumns()

            ' Evitar que el usuario pueda agregar nuevas filas
            DataGridView1.AllowUserToAddRows = False

            ' Evitar que el usuario haga cambios en cada fila o celda
            DataGridView1.ReadOnly = True

            ' Habilitar el modo de copia en el portapapeles
            DataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText


        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        Try
            ' Verificar si se presionó Ctrl + C
            If e.Control AndAlso e.KeyCode = Keys.C Then
                ' Copiar el contenido seleccionado solo si hay celdas seleccionadas
                If DataGridView1.SelectedCells.Count > 0 Then
                    Clipboard.SetDataObject(DataGridView1.GetClipboardContent())
                    ' Indicar que se ha manejado la combinación de teclas Ctrl + C
                    e.Handled = True
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub
End Class