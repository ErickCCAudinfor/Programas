Imports System.IO
Imports System.Text.Json

Namespace Configuracion

    ''' <summary>
    ''' Lo que la aplicación recuerda de una persona entre sesiones.
    '''
    ''' El orden de las propiedades ES el orden en que salen en el JSON, y está puesto para que
    ''' el fichero se lea de un vistazo con el Bloc de notas: primero quién, luego dónde está y
    ''' si sigue dentro. Es para lo que se consulta —saber a quién hay que avisar antes de
    ''' actualizar la aplicación en el .13—, así que tiene que entenderse sin abrir nada más.
    ''' </summary>
    Public Class EstadoUsuario

        Public Property Login As String = ""

        ''' <summary>Nombre legible. Al lado del login, que a veces es un número.</summary>
        Public Property Nombre As String = ""

        ''' <summary>
        ''' Máquina desde la que entró. Se llama Servidor y no Equipo porque es como se llama
        ''' en ActualizaPrecios y porque en la práctica son los servidores de sesión —SRVXEN03,
        ''' SRVXEN05…—, no el portátil de nadie.
        ''' </summary>
        Public Property Servidor As String = ""

        ''' <summary>
        ''' Si tiene la aplicación abierta.
        '''
        ''' NO BASTA CON ESTE CAMPO, y por eso está UltimoLatido al lado. En ActualizaPrecios
        ''' este booleano es lo único que hay, y se queda pegado en True para siempre cuando la
        ''' aplicación no se cierra por las buenas: un cuelgue, cerrar la sesión de Windows o
        ''' reiniciar el servidor. Para saber si alguien está DE VERDAD hay que mirar también
        ''' que el latido sea reciente. Eso es lo que hace Conectados().
        ''' </summary>
        Public Property Logeado As Boolean

        ''' <summary>Cuándo entró. Nothing si nunca ha entrado.</summary>
        Public Property UltimoAcceso As Date?

        ''' <summary>
        ''' Última señal de vida. La aplicación la refresca cada pocos minutos mientras está
        ''' abierta, así que una marca vieja con Logeado=True significa que esa sesión murió
        ''' sin avisar, no que la persona siga dentro.
        ''' </summary>
        Public Property UltimoLatido As Date?

        ''' <summary>Cuándo salió. Nothing si nunca ha salido por las buenas.</summary>
        Public Property UltimaSalida As Date?

        ''' <summary>Última versión de novedades que ha visto. Vacío = no ha visto ninguna.</summary>
        Public Property VersionNovedadesLeida As String = ""

    End Class

    ''' <summary>
    ''' Estado por usuario, en Config\Usuarios.json junto al ejecutable.
    '''
    ''' Es el equivalente del Usuarios.json de ActualizaPrecios, y funciona por el mismo
    ''' mecanismo: el .exe vive en el .13 y las ocho personas lo abren desde ahí, así que
    ''' AppContext.BaseDirectory apunta al .13 para todas y el fichero queda compartido.
    '''
    ''' AQUÍ SÍ HAY ESCRITURA CONCURRENTE DE VERDAD, al contrario que en el log de ejecuciones:
    ''' es un único fichero que ocho procesos pueden reescribir. Por eso se relee justo antes
    ''' de guardar y solo se toca la entrada propia — así dos personas marcando las novedades
    ''' como leídas a la vez no se borran la una a la otra—, y se escribe por reemplazo para
    ''' que un corte no deje el fichero a medias.
    '''
    ''' Y por eso mismo NO se guarda aquí nada que importe: si una escritura se pierde, lo
    ''' único que pasa es que a alguien le vuelve a salir el aviso de novedades una vez más.
    ''' </summary>
    Public Class RepositorioEstadoUsuario

        Public Const NombreFichero As String = "Usuarios.json"

        Private Shared ReadOnly OpcionesLectura As New JsonSerializerOptions With {
            .PropertyNameCaseInsensitive = True,
            .ReadCommentHandling = JsonCommentHandling.Skip,
            .AllowTrailingCommas = True
        }

        Private Shared ReadOnly OpcionesEscritura As New JsonSerializerOptions With {
            .WriteIndented = True
        }

        Public ReadOnly Property Ruta As String
            Get
                Return Path.Combine(AppContext.BaseDirectory,
                                    RepositorioEntornos.NombreCarpeta,
                                    NombreFichero)
            End Get
        End Property

        ''' <summary>
        ''' El estado de un login. Nunca devuelve Nothing: si no hay fichero o no hay entrada,
        ''' devuelve uno vacío, que se interpreta como «no ha visto ninguna novedad».
        ''' </summary>
        Public Function Obtener(login As String) As EstadoUsuario

            Dim todos = Leer()

            Dim mio = todos.FirstOrDefault(
                Function(e) String.Equals(e.Login, login, StringComparison.OrdinalIgnoreCase))

            Return If(mio, New EstadoUsuario With {.Login = login})

        End Function

        ''' <summary>
        ''' Guarda el estado de un login dejando intacto el del resto. Si no se puede escribir
        ''' no se lanza nada: es estado de comodidad, no datos.
        ''' </summary>
        Public Sub Guardar(estado As EstadoUsuario)

            If estado Is Nothing OrElse String.IsNullOrWhiteSpace(estado.Login) Then Exit Sub

            Try
                ' Se relee AHORA, no se usa una lista cargada antes: entre medias otra persona
                ' puede haber guardado lo suyo.
                Dim todos = Leer().ToList()

                todos.RemoveAll(
                    Function(e) String.Equals(e.Login, estado.Login, StringComparison.OrdinalIgnoreCase))
                todos.Add(estado)

                Directory.CreateDirectory(Path.GetDirectoryName(Ruta))

                Dim temporal = Ruta & ".tmp"
                File.WriteAllText(temporal, JsonSerializer.Serialize(todos, OpcionesEscritura))

                If File.Exists(Ruta) Then
                    File.Replace(temporal, Ruta, Nothing, ignoreMetadataErrors:=True)
                Else
                    File.Move(temporal, Ruta)
                End If

            Catch ex As Exception
                ' Fichero bloqueado por otro proceso, o sin permisos. Se pierde la marca y a
                ' esa persona le volverá a salir el aviso. No merece molestar a nadie.
            End Try

        End Sub

        ' ============================================================
        ' QUIÉN ESTÁ DENTRO
        '
        ' Para poder avisar antes de actualizar la aplicación en el .13: la copia se hace con
        ' robocopy sobre el .exe, y si alguien lo tiene abierto el fichero está bloqueado y la
        ' publicación falla a medias. Saber quién es y en qué servidor está permite pedirle que
        ' salga un momento en vez de ir preguntando a los ocho.
        ' ============================================================

        ''' <summary>Minutos entre latidos. El shell refresca con esta cadencia.</summary>
        Public Const MinutosEntreLatidos As Integer = 2

        ''' <summary>
        ''' A partir de cuántos minutos sin latido se deja de contar a alguien como conectado.
        ''' Da margen para tres latidos perdidos: por red lenta o por el fichero ocupado en ese
        ''' instante, que se descarta sin reintentar.
        ''' </summary>
        Public Const MinutosParaDarPorIdo As Integer = 7

        ''' <summary>Apunta que alguien acaba de entrar, desde qué máquina y cuándo.</summary>
        Public Sub MarcarEntrada(login As String, nombre As String)

            If String.IsNullOrWhiteSpace(login) Then Exit Sub

            Dim estado = Obtener(login)
            estado.Nombre = nombre
            estado.Servidor = Environment.MachineName
            estado.Logeado = True
            estado.UltimoAcceso = Date.Now
            estado.UltimoLatido = Date.Now

            Guardar(estado)

        End Sub

        ''' <summary>Señal de vida. Si falla no pasa nada: se recupera en el siguiente.</summary>
        Public Sub Latido(login As String)

            If String.IsNullOrWhiteSpace(login) Then Exit Sub

            Dim estado = Obtener(login)

            ' Si otra sesión de la misma persona entró después desde otra máquina, el latido de
            ' esta no debe devolver el servidor al anterior: manda la entrada más reciente.
            If Not String.Equals(estado.Servidor, Environment.MachineName,
                                 StringComparison.OrdinalIgnoreCase) Then Exit Sub

            estado.Logeado = True
            estado.UltimoLatido = Date.Now

            Guardar(estado)

        End Sub

        ''' <summary>Apunta que alguien ha salido por las buenas.</summary>
        Public Sub MarcarSalida(login As String)

            If String.IsNullOrWhiteSpace(login) Then Exit Sub

            Dim estado = Obtener(login)

            ' Mismo caso que en Latido: no se cierra la sesión que otra máquina tenga abierta.
            If estado.Logeado AndAlso
               Not String.Equals(estado.Servidor, Environment.MachineName,
                                 StringComparison.OrdinalIgnoreCase) Then Exit Sub

            estado.Logeado = False
            estado.UltimaSalida = Date.Now

            Guardar(estado)

        End Sub

        ''' <summary>
        ''' Quién está dentro ahora mismo: marcado como conectado Y con latido reciente. Lo
        ''' segundo es lo que evita contar sesiones que murieron sin avisar.
        ''' </summary>
        Public Function Conectados() As IReadOnlyList(Of EstadoUsuario)

            Dim limite = Date.Now.AddMinutes(-MinutosParaDarPorIdo)

            Return Leer().
                Where(Function(e) e.Logeado AndAlso
                                  e.UltimoLatido.HasValue AndAlso
                                  e.UltimoLatido.Value >= limite).
                OrderBy(Function(e) e.Nombre).
                ToList()

        End Function

        ''' <summary>Todos, para poder listarlos tal cual están en el fichero.</summary>
        Public Function Todos() As IReadOnlyList(Of EstadoUsuario)
            Return Leer()
        End Function

        Private Function Leer() As IReadOnlyList(Of EstadoUsuario)

            If Not File.Exists(Ruta) Then Return Array.Empty(Of EstadoUsuario)()

            Try
                Dim leidos = JsonSerializer.Deserialize(Of List(Of EstadoUsuario))(
                    File.ReadAllText(Ruta), OpcionesLectura)

                If leidos Is Nothing Then Return Array.Empty(Of EstadoUsuario)()

                Return leidos.Where(Function(e) Not String.IsNullOrWhiteSpace(e.Login)).ToList()

            Catch ex As Exception
                ' JSON roto: se empieza de cero en vez de impedir entrar en la aplicación.
                Return Array.Empty(Of EstadoUsuario)()
            End Try

        End Function

    End Class

End Namespace
