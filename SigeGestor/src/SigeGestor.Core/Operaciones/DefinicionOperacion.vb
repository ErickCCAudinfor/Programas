Imports SigeGestor.Core.Modelos

Namespace Operaciones

    ''' <summary>
    ''' Secciones de la aplicación. Coinciden con la barra lateral.
    ''' </summary>
    Public Enum SeccionOperacion
        Precios
        Contratos
        Productos
        Consultas
        Facturas
        Ajustes
    End Enum

    ''' <summary>
    ''' Qué le pide una operación al usuario. Es el mismo concepto que EntradasConsulta de
    ''' ActualizaPrecios, generalizado a todas las operaciones y no solo a las consultas.
    '''
    ''' Con esto el formulario se genera solo: no hay que tocar ningún diseñador para añadir
    ''' una operación nueva.
    ''' </summary>
    <Flags>
    Public Enum EntradasOperacion
        Ninguna = 0
        Contratos = 1
        Cups = 2
        Cifs = 4
        Facturas = 8
        Fechas = 16
        Texto = 32
        GrupoTarifa = 64
        Carpeta = 128
        Excel = 256

        ''' <summary>
        ''' Filtro por el grupo de tarifa que el contrato tiene AHORA. No es informativo: en
        ''' ActualizaPrecios es la condición que decide qué contratos se tocan.
        ''' O solo los que tienen tarifa personalizada, o solo los que tienen exactamente
        ''' este grupo actual. Ver UpdateContratoTarifa en ContratoTarifaSrv.
        ''' </summary>
        FiltroTarifaActual = 512
    End Enum

    ''' <summary>
    ''' Una operación del catálogo.
    '''
    ''' Es declarativa a propósito. En ActualizaPrecios cada función era un Button del
    ''' diseñador con su Click, y por eso Form1 acabó con 4.000 líneas y treinta y un
    ''' controles llamados Button1…Button31. Aquí añadir una operación es añadir una entrada
    ''' a una lista: la sección, el formulario y la validación salen de aquí.
    ''' </summary>
    Public Class DefinicionOperacion

        Public Property Nombre As String = ""

        Public Property Seccion As SeccionOperacion

        ''' <summary>Subgrupo dentro de la sección: «Generales», «Pool (CUPS)». Opcional.</summary>
        Public Property Subgrupo As String = ""

        ''' <summary>Una línea explicando qué hace. Se muestra bajo el título.</summary>
        Public Property Descripcion As String = ""

        Public Property Requiere As EntradasOperacion = EntradasOperacion.Ninguna

        ''' <summary>
        ''' True si la operación escribe en la base de datos. Determina que Replica no se
        ''' ofrezca como destino y que el botón de acción herede el color del entorno.
        ''' </summary>
        Public Property EsEscritura As Boolean

        ''' <summary>
        ''' Entorno con el que se abre. Las de lectura arrancan en Replica para no bloquear
        ''' a los usuarios de SIGE; las de escritura, en Producción.
        ''' </summary>
        Public ReadOnly Property EntornoPorDefecto As ClaveEntorno
            Get
                Return If(EsEscritura, ClaveEntorno.Produccion, ClaveEntorno.Replica)
            End Get
        End Property

        ''' <summary>
        ''' Tiene sentido ofrecer «un fichero por entrada». Solo en las que iteran una lista.
        ''' </summary>
        Public Property PermiteDividir As Boolean

        ''' <summary>
        ''' Qué hace la operación. Nothing mientras no esté portada desde ActualizaPrecios.
        ''' </summary>
        Public Property Ejecutable As IOperacionEjecutable

        ''' <summary>
        ''' Clave de una página propia, para lo que no cabe en el formulario generado.
        '''
        ''' Lo necesitan las dos pantallas de mantenimiento: una rejilla con filtro, edición
        ''' fila a fila y subida de un binario no se puede describir con campos declarados. El
        ''' resto de operaciones deja esto vacío, y eso es lo normal: si una operación nueva
        ''' pide página propia, casi siempre significa que falta un tipo de campo.
        ''' </summary>
        Public Property PaginaPropia As String = ""

        ''' <summary>
        ''' Se deduce de tener ejecutable o página propia, en vez de ser una marca que hay que
        ''' acordarse de cambiar. La página lo usa para decirlo en vez de dejar un botón que no
        ''' hace nada.
        ''' </summary>
        Public ReadOnly Property Implementada As Boolean
            Get
                Return Ejecutable IsNot Nothing OrElse PaginaPropia.Length > 0
            End Get
        End Property

        ''' <summary>
        ''' Base de datos distinta a la del entorno. Las curvas van contra SigeTotalTM y no
        ''' contra SigeTotal, aunque el servidor sea el mismo.
        '''
        ''' En ActualizaPrecios esto era una cadena de conexión fija a 172.31.100.30, así que
        ''' las curvas ignoraban el selector de entorno por completo. Aquí se toma el servidor
        ''' y las credenciales del entorno elegido y solo se cambia el catálogo.
        ''' </summary>
        Public Property BaseDatosAlternativa As String = ""

        ''' <summary>
        ''' Campos propios de esta operación, más allá de los flags de Requiere. Los pinta y
        ''' valida el formulario genérico.
        ''' </summary>
        Public Property Campos As IReadOnlyList(Of CampoOperacion) = Array.Empty(Of CampoOperacion)()

        ''' <summary>Etiqueta del campo de texto libre, cuando la operación lo pide.</summary>
        Public Property EtiquetaTexto As String = ""

        ''' <summary>Texto del botón de acción. Si está vacío se usa uno genérico.</summary>
        Public Property EtiquetaAccion As String = ""

        Public Function Pide(entrada As EntradasOperacion) As Boolean
            Return (Requiere And entrada) = entrada
        End Function

        ''' <summary>Pide una lista pegada (contratos, CUPS, CIF o facturas).</summary>
        Public ReadOnly Property PideLista As Boolean
            Get
                Dim listas = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or
                             EntradasOperacion.Cifs Or EntradasOperacion.Facturas
                Return (Requiere And listas) <> 0
            End Get
        End Property

    End Class

    ''' <summary>Claves de las páginas propias. Las resuelve la ventana principal.</summary>
    Public Module PaginasPropias
        Public Const Empresas As String = "empresas"
        Public Const ModelosImpresion As String = "modelosimpresion"
    End Module

End Namespace
