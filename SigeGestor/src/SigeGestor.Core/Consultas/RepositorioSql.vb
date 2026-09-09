Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Namespace Consultas

    ''' <summary>
    ''' Carga las plantillas .sql que hay junto al ejecutable y sustituye sus marcadores.
    '''
    ''' A DIFERENCIA DE ACTUALIZAPRECIOS, los .sql sí van dentro del proyecto y se copian al
    ''' compilar. Allí vivían solo en la carpeta de salida y había que subirlos a mano al
    ''' servidor: si faltaba uno, la consulta reventaba con FileNotFoundException al pulsar
    ''' Consultar. Aquí eso no puede pasar.
    ''' </summary>
    Public Class RepositorioSql

        Public Const NombreCarpeta As String = "Consultas"

        Private ReadOnly _cache As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        Public ReadOnly Property Carpeta As String
            Get
                Return Path.Combine(AppContext.BaseDirectory, NombreCarpeta)
            End Get
        End Property

        ''' <summary>
        ''' Texto de una plantilla. Se cachea: un .sql no cambia mientras la aplicación está
        ''' abierta, y las curvas lo piden una vez por CUPS.
        ''' </summary>
        Public Function Obtener(nombre As String) As String

            Dim cacheado As String = Nothing
            If _cache.TryGetValue(nombre, cacheado) Then Return cacheado

            Dim ruta = Path.Combine(Carpeta, $"{nombre}.sql")
            If Not File.Exists(ruta) Then
                Throw New FileNotFoundException(
                    $"No se encuentra la plantilla de consulta '{nombre}.sql' en {NombreCarpeta}\.", ruta)
            End If

            Dim texto = Leer(ruta)
            _cache(nombre) = texto
            Return texto

        End Function

        ''' <summary>
        ''' Lee el .sql detectando su codificación.
        '''
        ''' Portado de ConsultasSQLDirectorio. Hace falta porque SSMS guarda por defecto en
        ''' ANSI (Windows-1252) y leer eso como UTF-8 convierte cada tilde en U+FFFD. No es
        ''' cosmético: varias consultas llevan tildes en alias de columna sin entrecomillar
        ''' (as Otroscostesderegasificacióntérminofijocliente) y U+FFFD no es válido en un
        ''' identificador de SQL Server, así que la consulta fallaba con error de sintaxis.
        '''
        ''' Ahora mismo los 28 ficheros ya están en UTF-8, pero el respaldo se mantiene: el
        ''' siguiente que salga de SSMS vendrá otra vez en ANSI.
        ''' </summary>
        Private Shared Function Leer(ruta As String) As String

            Dim bytes = File.ReadAllBytes(ruta)

            ' UTF-16, que sí lleva BOM obligatorio
            If bytes.Length >= 2 Then
                If bytes(0) = &HFF AndAlso bytes(1) = &HFE Then
                    Return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2)
                End If
                If bytes(0) = &HFE AndAlso bytes(1) = &HFF Then
                    Return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2)
                End If
            End If

            ' UTF-8 con BOM: hay que saltarlo o queda un carácter invisible delante del SQL
            Dim desde = 0
            If bytes.Length >= 3 AndAlso bytes(0) = &HEF AndAlso bytes(1) = &HBB AndAlso bytes(2) = &HBF Then
                desde = 3
            End If

            ' Sin BOM: si descodifica como UTF-8 válido, lo es; si no, es ANSI
            Try
                Return New UTF8Encoding(False, True).GetString(bytes, desde, bytes.Length - desde)
            Catch ex As DecoderFallbackException
                Return DecodificarWindows1252(bytes)
            End Try

        End Function

        ''' <summary>
        ''' Windows-1252 a mano. Coincide con Latin-1 salvo en el rango 80-9F, donde 1252 mete
        ''' el euro y las comillas tipográficas. Se hace así para no depender del paquete
        ''' System.Text.Encoding.CodePages, que en .NET moderno hay que registrar antes de
        ''' poder pedir Encoding.GetEncoding(1252).
        ''' </summary>
        Private Shared Function DecodificarWindows1252(bytes As Byte()) As String

            ' Posiciones 0-31 de esta cadena = bytes 80-9F. U+FFFD marca los no definidos.
            Const Altos As String = "€" & ChrW(&HFFFD) & "‚ƒ„…†‡ˆ‰Š‹Œ" & ChrW(&HFFFD) & "Ž" &
                                    ChrW(&HFFFD) & ChrW(&HFFFD) & "‘’“”•–—˜™š›œ" & ChrW(&HFFFD) & "žŸ"

            Dim sb As New StringBuilder(bytes.Length)
            For Each b In bytes
                If b >= &H80 AndAlso b <= &H9F Then
                    sb.Append(Altos(b - &H80))
                Else
                    sb.Append(ChrW(b))
                End If
            Next
            Return sb.ToString()

        End Function

    End Class

    ''' <summary>
    ''' Una plantilla con sus marcadores por sustituir.
    '''
    ''' La sustitución es SIN DISTINGUIR MAYÚSCULAS a propósito. Los marcadores de los .sql no
    ''' son consistentes —unos ponen «DesdeFechaReplace» y otros «hastaFechaReplace»— y
    ''' String.Replace sí distingue, así que en ActualizaPrecios bastaba con que alguien
    ''' escribiera el marcador con otra caja para que la fecha se quedara literal dentro del
    ''' SQL y la consulta fallara o devolviera cualquier cosa.
    '''
    ''' OJO CON LA INYECCIÓN: estas plantillas concatenan valores en el SQL, no admiten
    ''' parámetros. Las fechas son seguras porque se formatean desde un Date, y los CUPS y CIF
    ''' pasan antes por AnalizadorEntradas, que descarta lo que no cumpla su patrón. Aun así,
    ''' cualquier marcador nuevo que reciba texto libre hay que mirarlo con lupa.
    ''' </summary>
    Public Class PlantillaSql

        Private _texto As String

        Public Sub New(texto As String)
            _texto = texto
        End Sub

        Public Function Poner(marcador As String, valor As String) As PlantillaSql
            _texto = Regex.Replace(_texto, Regex.Escape(marcador), Function(m) valor, RegexOptions.IgnoreCase)
            Return Me
        End Function

        Public Function PonerFecha(marcador As String, fecha As Date, formato As String) As PlantillaSql
            Return Poner(marcador, fecha.ToString(formato, Globalization.CultureInfo.InvariantCulture))
        End Function

        ''' <summary>
        ''' Marcadores «algoReplace» que han quedado sin sustituir.
        '''
        ''' El patrón exige AL MENOS UN CARÁCTER antes de «Replace» y palabra completa. Sin
        ''' eso caza la función REPLACE() de T-SQL, que varias plantillas usan, y toda consulta
        ''' con un REPLACE dentro se abortaría diciendo que falta un marcador.
        '''
        ''' NO SE MIRA DENTRO DE LOS COMENTARIOS. Un comentario que explique qué marcador lleva
        ''' la plantilla —«la tabla va en tablaReplace»— contaba como marcador sin resolver y la
        ''' operación abortaba antes de consultar nada, siempre, aunque el SQL estuviese
        ''' perfecto. Y el sitio natural para documentar eso es justo el comentario del .sql.
        '''
        ''' Ojo: no detecta marcadores que no acaben en «Replace», como el ListaFacturasParam
        ''' de Activa y Reactiva. Ahí el respaldo es el error de SQL Server.
        ''' </summary>
        Public Function MarcadoresPendientes() As IReadOnlyList(Of String)
            Return Regex.Matches(SinComentarios(_texto), "\b\w+Replace\b", RegexOptions.IgnoreCase) _
                        .Select(Function(m) m.Value) _
                        .Distinct(StringComparer.OrdinalIgnoreCase) _
                        .ToList()
        End Function

        ''' <summary>
        ''' Quita los comentarios de línea, de «--» hasta el fin de la línea.
        '''
        ''' Se corta por línea y no solo las líneas que EMPIEZAN por «--», para que valga
        ''' también el comentario puesto detrás de código.
        '''
        ''' Es un corte a lo bruto: un «--» dentro de una cadena literal se llevaría por delante
        ''' el resto de la línea. No importa para lo que se usa —solo buscar marcadores— y lo
        ''' peor que puede pasar es dejar de avisar de uno, cosa que SQL Server dirá de todas
        ''' formas. Para nada más vale esta función.
        ''' </summary>
        Private Shared Function SinComentarios(texto As String) As String
            Return Regex.Replace(texto, "--[^\r\n]*", "")
        End Function

        Public Overrides Function ToString() As String
            Return _texto
        End Function

    End Class

End Namespace
