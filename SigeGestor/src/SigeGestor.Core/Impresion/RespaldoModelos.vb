Imports System.IO
Imports System.Text
Imports System.Text.Json
Imports SigeGestor.Core.Configuracion

Namespace Impresion

    ''' <summary>Lo que se guardó, para poder decirlo.</summary>
    Public Class ResultadoRespaldo
        ''' <summary>Ruta del .rpt guardado. Vacía si el modelo no tenía binario.</summary>
        Public Property Ruta As String = ""

        ''' <summary>Ruta de la ficha con los datos para volver a subirlo.</summary>
        Public Property RutaFicha As String = ""

        ''' <summary>False cuando la fila existía pero no tenía report que guardar.</summary>
        Public Property TeniaBinario As Boolean

        Public ReadOnly Property Resumen As String
            Get
                If Not TeniaBinario Then Return "la fila no tenía report que guardar"
                Return Path.GetFileName(Ruta)
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Copia de seguridad de un modelo de impresión ANTES de pisarlo o borrarlo.
    '''
    ''' POR QUE EXISTE. El 17/11/2026 se piso un report en Producción que no era el que tocaba,
    ''' y no había copia de ninguna clase: el binario solo vivía en la base del cliente y el
    ''' UPDATE lo sustituyó. No había forma de volver atrás.
    '''
    ''' Se guarda en disco y no en la base a propósito. Una fila más en modelodeimpresion
    ''' saldría en la lista de modelos de la empresa y alguien acabaría imprimiendo con ella;
    ''' y además el .rpt en un fichero es lo que de verdad hace falta para restaurarlo, porque
    ''' se puede abrir con Crystal y volver a subir desde la propia pantalla.
    '''
    ''' Junto al .rpt se deja una ficha .json con lo que hay que reponer —descripción, entorno,
    ''' tipo y clase—, que es lo que no se puede deducir mirando el report.
    ''' </summary>
    Public Module RespaldoModelos

        ''' <summary>Carpeta raiz, en el escritorio. Aparte de ConsultasBO: esto no es una
        ''' salida de una consulta, es una copia de seguridad.</summary>
        Public Const NombreRaiz As String = "ReportsBack"

        Public Const NombreCarpeta As String = "ModelosImpresion_Respaldo"

        ''' <summary>Escritorio\ReportsBack\ModelosImpresion_Respaldo</summary>
        Public ReadOnly Property Carpeta As String
            Get
                Return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    NombreRaiz, NombreCarpeta)
            End Get
        End Property

        Private ReadOnly OpcionesJson As New JsonSerializerOptions With {
            .WriteIndented = True,
            .Encoder = Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }

        ''' <summary>
        ''' Guarda el modelo tal como está ahora mismo en la base.
        '''
        ''' LANZA si no puede escribir. Es deliberado: quien llama tiene que abortar el cambio.
        ''' Seguir adelante creyendo que hay copia cuando no la hay es peor que no tener la
        ''' casilla, porque da una confianza que no existe.
        ''' </summary>
        Public Function Guardar(nombreEmpresa As String,
                                modelo As ModeloDeImpresion) As ResultadoRespaldo

            If modelo Is Nothing Then
                Throw New ArgumentNullException(NameOf(modelo), "No hay modelo que respaldar.")
            End If

            ' Una carpeta por día: así no se acumulan cientos de ficheros sueltos y se
            ' encuentra por la fecha, que es como se recuerda («lo pisé el martes»).
            Dim carpetaDelDia = Path.Combine(Carpeta, Date.Today.ToString("yyyy-MM-dd"))
            Directory.CreateDirectory(carpetaDelDia)

            ' EL NOMBRE QUE TIENE EN LA BASE, mas la fecha en que se hizo la copia. Asi el
            ' fichero se reconoce de un vistazo y es el mismo que habria que volver a subir.
            '
            ' Lleva tambien la hora, y no solo el dia, porque la carpeta ya va por dia: sin
            ' ella, dos copias del mismo report en la misma jornada -o el masivo, que hace una
            ' por empresa en el mismo segundo- saldrian con el mismo nombre.
            Dim sello = Date.Now.ToString("yyyyMMdd_HHmmss")
            Dim baseNombre = Sanear(NombreDeFichero(modelo) & "_" & sello)

            Dim resultado As New ResultadoRespaldo With {
                .TeniaBinario = modelo.Modelo IsNot Nothing AndAlso modelo.Modelo.Length > 0
            }

            If resultado.TeniaBinario Then
                resultado.Ruta = RutaLibre(carpetaDelDia, baseNombre, ".rpt")
                File.WriteAllBytes(resultado.Ruta, modelo.Modelo)
            End If

            ' La ficha se escribe SIEMPRE, aunque no hubiera binario: deja constancia de qué
            ' fila se tocó y con qué datos, que es la mitad de lo que hace falta para reponer.
            resultado.RutaFicha = RutaLibre(carpetaDelDia, baseNombre, ".json")
            File.WriteAllText(resultado.RutaFicha, JsonSerializer.Serialize(New With {
                .Empresa = nombreEmpresa,
                .Guardado = Date.Now,
                .IdModeloDeImpresion = modelo.IdModeloDeImpresion,
                .Descripcion = modelo.DescripcionModeloDeImpresion,
                .Entorno = modelo.Entorno,
                .CodigoTipo = modelo.CodigoTipoModeloDeImpresion,
                .ClassName = modelo.ClassName,
                .RptFileName = modelo.RptFileName,
                .Bytes = If(modelo.Modelo Is Nothing, 0, modelo.Modelo.Length),
                .Report = If(resultado.TeniaBinario, Path.GetFileName(resultado.Ruta), "")
            }, OpcionesJson), New UTF8Encoding(True))

            Return resultado

        End Function

        ''' <summary>
        ''' De donde sale el nombre: del RptFileName que tenga en la base, sin extension.
        '''
        ''' Muchas filas lo tienen VACIO -en la lista de modelos la columna FICHERO sale en
        ''' blanco en la mayoria- y entonces se tira de la clase, que es lo que si suelen
        ''' llevar. Y si tampoco, del Id, que siempre esta.
        ''' </summary>
        Private Function NombreDeFichero(modelo As ModeloDeImpresion) As String

            Dim enBase = If(modelo.RptFileName, "").Trim()
            If enBase.Length > 0 Then Return Path.GetFileNameWithoutExtension(enBase)

            Dim clase = If(modelo.ClassName, "").Trim()
            If clase.Length > 0 Then Return clase

            Return "modelo_" & modelo.IdModeloDeImpresion

        End Function

        ''' <summary>Un nombre que no exista ya: no se pisa un respaldo anterior nunca.</summary>
        Private Function RutaLibre(carpetaDelDia As String, baseNombre As String, extension As String) As String

            Dim candidata = Path.Combine(carpetaDelDia, baseNombre & extension)

            Dim n = 2
            While File.Exists(candidata)
                candidata = Path.Combine(carpetaDelDia, $"{baseNombre}_{n}{extension}")
                n += 1
            End While

            Return candidata

        End Function

        ''' <summary>
        ''' Deja el texto en algo que valga como nombre de fichero. Las descripciones llevan
        ''' barras y dos puntos con normalidad («Factura Luz 2.0 / General»).
        ''' </summary>
        Private Function Sanear(texto As String) As String

            Dim limpio As New StringBuilder(texto.Length)

            For Each c In If(texto, "")
                limpio.Append(If(Array.IndexOf(Path.GetInvalidFileNameChars(), c) >= 0, "_"c, c))
            Next

            Dim resultado = limpio.ToString().Trim()
            If resultado.Length > 120 Then resultado = resultado.Substring(0, 120)

            Return If(resultado.Length = 0, "modelo", resultado)

        End Function

    End Module

End Namespace
