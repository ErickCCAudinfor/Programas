Imports System.Data
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.Data.SqlClient

Namespace Consultas

    ''' <summary>
    ''' Los históricos de una tabla de curva, preguntados a la base en el momento de consultar.
    '''
    ''' POR QUÉ NO VAN ESCRITOS EN EL .sql, QUE ES COMO ESTABAN
    ''' =======================================================
    ''' Cuando una tabla de curva se llena, alguien la parte y crea CurvaHoraria_H_082025 con lo
    ''' viejo. Los .sql traían la lista de históricos escrita a mano y con UNION ALL, así que
    ''' cada vez que se partía una tabla había que acordarse de editar el .sql.
    '''
    ''' No se acordó nadie: al portar esto, CurvaHoraria unía cinco históricos cuando en la base
    ''' había siete —le faltaban _H_082025 y _H_082026— y CurvaFacturable seis de siete. Esas
    ''' consultas devolvían de menos SIN DECIR NADA, que es la peor forma de fallar en una
    ''' consulta que alguien va a mandar a un cliente.
    '''
    ''' Preguntando a sys.tables eso no puede volver a pasar: la lista es la que hay.
    ''' </summary>
    Public Class TablasHistoricas

        ''' <summary>
        ''' El sufijo de los históricos es MMAAAA: _H_082025 es agosto de 2025. Hay también un
        ''' _H sin fecha, que es el primero que se hizo.
        ''' </summary>
        Private Const PatronSufijo As String = "^{0}_H(_[0-9]{{6}})?$"

        Public Class Resultado

            ''' <summary>La tabla principal. Vacío si no existe, que es un fallo de verdad.</summary>
            Public Property Base As String = ""

            ''' <summary>Los históricos que se van a consultar, en orden de nombre.</summary>
            Public Property Historicas As IReadOnlyList(Of String) = Array.Empty(Of String)()

            ''' <summary>
            ''' Tablas que empiezan por «base_H» pero cuyo sufijo no es MMAAAA: un _H_OLD, un
            ''' _H_pruebas. No se consultan —no se sabe qué son— pero SE DICEN, para que nadie
            ''' se quede pensando que se han mirado.
            ''' </summary>
            Public Property Raras As IReadOnlyList(Of String) = Array.Empty(Of String)()

            ''' <summary>
            ''' Históricos cuyas columnas no coinciden con las de la tabla principal. Se dejan
            ''' fuera: ver el comentario de <see cref="BuscarAsync"/>.
            ''' </summary>
            Public Property Incompatibles As IReadOnlyList(Of String) = Array.Empty(Of String)()

            ''' <summary>Principal más históricas, que es lo que hay que recorrer.</summary>
            Public ReadOnly Property Todas As IReadOnlyList(Of String)
                Get
                    Dim lista As New List(Of String)
                    If Base.Length > 0 Then lista.Add(Base)
                    lista.AddRange(Historicas)
                    Return lista
                End Get
            End Property

            ''' <summary>Lo que hay que contarle al usuario, o cadena vacía si no hay nada raro.</summary>
            Public ReadOnly Property Aviso As String
                Get
                    Dim partes As New List(Of String)

                    If Incompatibles.Count > 0 Then
                        partes.Add($"sin consultar por no cuadrar las columnas: {String.Join(", ", Incompatibles)}")
                    End If

                    If Raras.Count > 0 Then
                        partes.Add($"sin consultar por tener un nombre que no es _H_MMAAAA: {String.Join(", ", Raras)}")
                    End If

                    Return String.Join(" · ", partes)
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Busca la tabla principal y sus históricos.
        '''
        ''' UNA SOLA CONSULTA para nombres y columnas, y las columnas hacen falta: los .sql unen
        ''' con UNION ALL y uno de ellos hace SELECT *. Si un histórico tuviera las columnas en
        ''' otro orden, el UNION ALL no fallaría necesariamente —bastaría con que los tipos
        ''' encajen— y las columnas saldrían cruzadas en el Excel sin que nada avisara. Mientras
        ''' la lista se mantenía a mano eso lo cubría quien la escribía; automatizándola hay que
        ''' comprobarlo aquí.
        ''' </summary>
        Public Async Function BuscarAsync(cadenaConexion As String,
                                          tablaBase As String,
                                          Optional ct As CancellationToken = Nothing) _
            As Task(Of Resultado)

            ' El nombre viene del catálogo, no del usuario, pero se comprueba igual: acaba
            ' dentro de un LIKE y del propio SQL de la consulta.
            If Not Regex.IsMatch(tablaBase, "^[A-Za-z][A-Za-z0-9_]{0,120}$") Then
                Throw New ArgumentException(
                    $"'{tablaBase}' no es un nombre de tabla admisible.", NameOf(tablaBase))
            End If

            ' Columnas por tabla, en el orden en que están declaradas.
            Const consulta As String =
                "SELECT t.name AS Tabla, c.name AS Columna, c.column_id " &
                "FROM sys.tables t " &
                "INNER JOIN sys.schemas s ON t.schema_id = s.schema_id " &
                "INNER JOIN sys.columns c ON c.object_id = t.object_id " &
                "WHERE s.name = 'dbo' AND t.name LIKE @patron " &
                "ORDER BY t.name, c.column_id"

            Dim columnas As New Dictionary(Of String, List(Of String))(StringComparer.OrdinalIgnoreCase)

            Using conexion As New SqlConnection(cadenaConexion)
                Await conexion.OpenAsync(ct).ConfigureAwait(False)

                Using comando As New SqlCommand(consulta, conexion)
                    comando.CommandTimeout = 30

                    ' tablaBase ya está validado, así que no lleva comodines de LIKE dentro.
                    comando.Parameters.Add("@patron", SqlDbType.NVarChar, 160).Value = tablaBase & "%"

                    Using lector = Await comando.ExecuteReaderAsync(ct).ConfigureAwait(False)
                        While Await lector.ReadAsync(ct).ConfigureAwait(False)
                            Dim tabla = lector.GetString(0)
                            Dim columna = lector.GetString(1)

                            Dim suyas As List(Of String) = Nothing
                            If Not columnas.TryGetValue(tabla, suyas) Then
                                suyas = New List(Of String)
                                columnas(tabla) = suyas
                            End If
                            suyas.Add(columna)
                        End While
                    End Using
                End Using
            End Using

            Return Clasificar(tablaBase, columnas)

        End Function

        ''' <summary>
        ''' Reparte lo que ha devuelto la base entre principal, históricos, raros e
        ''' incompatibles. Aparte de BuscarAsync para poder probarlo sin base de datos.
        ''' </summary>
        Friend Shared Function Clasificar(tablaBase As String,
                                          columnas As Dictionary(Of String, List(Of String))) As Resultado

            Dim r As New Resultado

            Dim deLaBase As List(Of String) = Nothing
            If Not columnas.TryGetValue(tablaBase, deLaBase) Then Return r   ' Base vacío: no existe
            r.Base = tablaBase

            Dim patron As New Regex(String.Format(PatronSufijo, Regex.Escape(tablaBase)),
                                    RegexOptions.IgnoreCase)

            ' Lo que empieza por «base_H» y por tanto pretende ser un histórico. Se compara con
            ' «_H» y no solo con «base», porque CurvaValidadaDesvioTM también empieza por
            ' CurvaValidada y no es un histórico de nada.
            Dim prefijoHistorico = tablaBase & "_H"

            Dim buenas As New List(Of String)
            Dim raras As New List(Of String)
            Dim incompatibles As New List(Of String)

            For Each nombre In columnas.Keys.OrderBy(Function(n) n, StringComparer.OrdinalIgnoreCase)

                If String.Equals(nombre, tablaBase, StringComparison.OrdinalIgnoreCase) Then Continue For
                If Not nombre.StartsWith(prefijoHistorico, StringComparison.OrdinalIgnoreCase) Then Continue For

                If Not patron.IsMatch(nombre) Then
                    raras.Add(nombre)
                    Continue For
                End If

                ' Mismas columnas y en el mismo orden. Se compara el orden y no solo el
                ' conjunto: el UNION ALL empareja por posición.
                If Not columnas(nombre).SequenceEqual(deLaBase, StringComparer.OrdinalIgnoreCase) Then
                    incompatibles.Add(nombre)
                    Continue For
                End If

                buenas.Add(nombre)
            Next

            r.Historicas = buenas
            r.Raras = raras
            r.Incompatibles = incompatibles

            Return r

        End Function

    End Class

End Namespace
