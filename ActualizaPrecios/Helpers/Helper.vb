Public Class Helper
    Public Shared Function FillObjectFromDatatable(tabla As DataTable, tipo As Type) As List(Of Object)
        Dim lista As New List(Of Object)
        Try
            Dim mapa(tabla.Columns.Count - 1) As Integer
            Dim mapaRelleno As Boolean = False
            Dim propiedades = tipo.GetProperties()

            For i = 0 To mapa.Count - 1
                mapa(i) = -1
            Next

            For Each fila As DataRow In tabla.Rows
                Try
                    Dim asemb = tipo.Assembly  'Instanciar el objeto con new sin parametros
                    Dim objeto = asemb.CreateInstance(tipo.FullName)
                    lista.Add(objeto)
                    For i As Integer = 0 To tabla.Columns.Count - 1
                        Try
                            'Dim nombreColumna As String = tabla.Columns(i).ColumnName.Trim.ToUpper
                            'Dim prop1 = tipo.GetProperty(nombreColumna)'Es case sensitive


                            If mapaRelleno = False Then
                                Dim nombreColumna As String = tabla.Columns(i).ColumnName.Trim.ToUpper

                                Dim indicePropiedad As Integer = -1

                                For Each prop In propiedades ' tipo.GetProperties()
                                    Try
                                        indicePropiedad += 1

                                        Dim nombrePropiedad As String = prop.Name.Trim.ToUpper
                                        If nombrePropiedad = nombreColumna Then
                                            mapa(i) = indicePropiedad

                                            If indicePropiedad > 0 Then
                                                Dim aa As Integer = 0
                                            End If

                                            'Los XML llegan como string
                                            'Las propiedades string no se pueden definir nulables en el lado de puntonet
                                            If Not fila(i) Is DBNull.Value Then
                                                If prop.PropertyType.Equals(GetType(XElement)) Then
                                                    If fila(i).ToString <> String.Empty Then
                                                        prop.SetValue(objeto, XElement.Parse(fila(i).ToString))
                                                    End If
                                                ElseIf prop.PropertyType.Equals(GetType(String)) Then
                                                    prop.SetValue(objeto, fila(i).ToString)
                                                Else
                                                    prop.SetValue(objeto, fila(i))
                                                End If
                                            Else
                                                Try
                                                    'FLN:2021-03-26 si la propiedad es nulable se le debe asignar el valor nothing.
                                                    prop.SetValue(objeto, Nothing)
                                                Catch ex As Exception
                                                    Throw
                                                End Try
                                            End If

                                            Exit For
                                        End If
                                    Catch ex As Exception
                                        Throw
                                    End Try
                                Next
                            Else
                                'Los XML llegan como string
                                'Las propiedades string no se pueden definir nulables en el lado de puntonet
                                If mapa(i) >= 0 Then
                                    If Not fila(i) Is DBNull.Value Then
                                        If propiedades.ElementAt(mapa(i)).PropertyType.Equals(GetType(XElement)) Then
                                            If fila(i).ToString <> String.Empty Then
                                                propiedades.ElementAt(mapa(i)).SetValue(objeto, XElement.Parse(fila(i).ToString))
                                            End If
                                        ElseIf propiedades.ElementAt(mapa(i)).PropertyType.Equals(GetType(String)) Then
                                            propiedades.ElementAt(mapa(i)).SetValue(objeto, fila(i).ToString)
                                        Else
                                            propiedades.ElementAt(mapa(i)).SetValue(objeto, fila(i))
                                        End If
                                    Else
                                        Try
                                            'FLN:2021-03-26 si la propiedad es nulable se le debe asignar el valor nothing.
                                            propiedades.ElementAt(mapa(i)).SetValue(objeto, Nothing)
                                        Catch ex As Exception
                                            Throw
                                        End Try
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            Throw
                        End Try
                    Next

                    mapaRelleno = True
                Catch ex As Exception
                    Throw
                End Try
            Next
        Catch ex As Exception
            Throw
        End Try
        Return lista 'Utilizar  ret = lista.Cast(Of UnDTO)().ToList
    End Function

    Public Shared Function QuerySelect(query As String, Optional connectionString As String = "", Optional commandTimeout As Integer = 0,
                                           Optional ByRef resultado As String = "") As DataSet
        Dim ret As DataSet = Nothing
        Dim t As DataTable = Nothing
        Dim conexion As SqlClient.SqlConnection = Nothing
        Dim comando As SqlClient.SqlCommand = Nothing
        Dim fechaInicio As DateTime = Nothing
        Dim erroresColumnas As String = ""
        Try
            If query Is Nothing Then
                query = ""
            End If



            fechaInicio = Now

            ret = New DataSet
            t = New DataTable
            ret.Tables.Add(t)


            conexion = New SqlClient.SqlConnection(connectionString)


            comando = New SqlClient.SqlCommand(query, conexion)
            If commandTimeout > 30 Then
                comando.CommandTimeout = commandTimeout
            End If

            Dim repeticiones_nombres_columnas As Long = 10

            conexion.Open()
            Dim dr As SqlClient.SqlDataReader = comando.ExecuteReader

            If dr.FieldCount > 0 Then
                For i = 0 To dr.FieldCount - 1
                    Try
                        Dim nombre = dr.GetName(i)
                        Try
                            Dim tipo As Type = dr.GetFieldType(i)
                            If dr.HasRows AndAlso Not tipo Is DBNull.Value Then
                                'dr.Item(i).GetType fallará si es nothing
                                If Not t.Columns.Contains(nombre) Then
                                    t.Columns.Add(New DataColumn(nombre, tipo))
                                Else
                                    Dim n As Long = 0
                                    Do
                                        Try
                                            n += 1
                                            Dim nuevo_nombre As String = $"{nombre}_{n}"
                                            If Not t.Columns.Contains(nuevo_nombre) Then
                                                Dim descripcionErrorColumna As String = $"Exception: Ya existe la columna de nombre ""{nombre}"". Revisar la consulta ""{query}"". Entre tanto se renombrará a {nuevo_nombre}"

                                                t.Columns.Add(New DataColumn(nuevo_nombre, tipo))
                                                n = repeticiones_nombres_columnas
                                            End If
                                        Catch ex As Exception
                                            Throw
                                        End Try
                                    Loop While n < repeticiones_nombres_columnas
                                End If
                            Else
                                If Not t.Columns.Contains(nombre) Then
                                    t.Columns.Add(New DataColumn(nombre))
                                Else
                                    Dim n As Long = 0
                                    Do
                                        Try
                                            n += 1
                                            Dim nuevo_nombre As String = $"{nombre}_{n}"
                                            If Not t.Columns.Contains(nuevo_nombre) Then
                                                Dim descripcionErrorColumna As String = $"Exception: Ya existe la columna de nombre ""{nombre}"". Revisar la consulta ""{query}"". Entre tanto se renombrará a {nuevo_nombre}"

                                                t.Columns.Add(New DataColumn(nuevo_nombre, tipo))
                                                n = repeticiones_nombres_columnas
                                            End If
                                        Catch ex As Exception
                                            Throw
                                        End Try
                                    Loop While n < repeticiones_nombres_columnas
                                End If
                            End If
                        Catch ex As Exception
                            erroresColumnas &= ex.ToString & Chr(13) & Chr(10)
                            t.Columns.Add(New DataColumn(nombre))
                        End Try
                    Catch ex As Exception
                        erroresColumnas &= ex.ToString & Chr(13) & Chr(10)
                    End Try
                Next
            End If

            'Rellenar los datos
            While dr.Read
                Try
                    Dim fila As DataRow = t.NewRow
                    t.Rows.Add(fila)
                    Dim objetos(dr.FieldCount - 1) As Object
                    Dim numero = dr.GetValues(objetos)
                    fila.ItemArray = objetos
                Catch ex As Exception
                    Throw
                End Try
            End While

            dr.Close()

            Dim diferencia = DateDiff(DateInterval.Second, fechaInicio, Now)
            resultado = $"OK;Timeout={diferencia.ToString} segundos;{erroresColumnas}"
        Catch ex As Exception
            'LogHelper.OBJ.TraceLog($"ERROR: {ex.ToString}{Chr(10)}Query:{Chr(10)}{query}", "", NivelSwitch.ERROR_LEVEL)

            Try
                Dim diferencia = DateDiff(DateInterval.Second, fechaInicio, Now)
                resultado = $"ERROR;Timeout={diferencia.ToString} segundos;DescripcionError={ex.ToString};{erroresColumnas}"
            Catch ex1 As Exception

            End Try

            Dim terrores = New DataTable("errores")
            terrores.Columns.Add("DescripcionError", GetType(String))
            terrores.Columns.Add("ErroresColumnas", GetType(String))
            Dim filaError As DataRow = terrores.NewRow
            filaError.Item("DescripcionError") = ex.ToString
            filaError.Item("ErroresColumnas") = erroresColumnas
            terrores.Rows.Add(filaError)
            ret.Tables.Add(terrores)
        Finally
            If Not conexion Is Nothing Then
                conexion.Close()
                conexion.Dispose()
                conexion = Nothing
            End If
        End Try

        Return ret
    End Function

    Public Shared Function GetError(ds As DataSet) As ErrorConsulta
        Dim resultado As New ErrorConsulta
        Try
            If Not ds Is Nothing Then
                For Each tabla As DataTable In ds.Tables
                    Try
                        If tabla.TableName.ToUpper = "errores".ToUpper Then
                            resultado.HasError = True
                            If tabla.Rows.Count > 0 Then
                                resultado.DescripcionError = tabla.Rows(0).Item("DescripcionError").ToString
                            End If
                            Exit For
                        End If
                    Catch ex As Exception

                    End Try
                Next
            End If
        Catch ex As Exception

        End Try

        Return resultado
    End Function

    Public Shared Function LimpiarCups(cups As List(Of String)) As List(Of String)
        Return cups.Select(Function(c) Replace(c, " ", "").Substring(0, Math.Min(20, c.Length))).ToList()
    End Function
End Class
Public Class ErrorConsulta
    Public Property HasError As Boolean = False
    Public Property DescripcionError As String = String.Empty
End Class