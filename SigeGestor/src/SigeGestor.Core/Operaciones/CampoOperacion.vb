Namespace Operaciones

    Public Enum TipoCampo
        ''' <summary>Texto libre.</summary>
        Texto
        ''' <summary>Desplegable con opciones traídas de la base de datos o de configuración.</summary>
        Seleccion
        ''' <summary>Número entero. Se valida al escribir y se acota con Minimo y Maximo.</summary>
        Numero
        ''' <summary>Ruta de un fichero de entrada, con su selector. Ver FiltroFichero.</summary>
        Fichero
        ''' <summary>Importe con decimales. Cadena vacía significa NULL, no cero.</summary>
        Importe
        ''' <summary>Casilla. Llega al ejecutable como «1» o «0».</summary>
        Booleano
        ''' <summary>Fecha suelta. Vacío significa sin fecha cuando no es requerida.</summary>
        Fecha
    End Enum

    ''' <summary>
    ''' De dónde salen las opciones de un campo de selección. Es una clave y no una consulta
    ''' porque el SQL vive en el Core, no en el catálogo.
    ''' </summary>
    Public Module OrigenLista
        Public Const Agentes As String = "agentes"
        Public Const Administradores As String = "administradores"

        ''' <summary>
        ''' Tipos de XML para trocear, de Config\TipoXml.json. No es de base de datos: viene del
        ''' mismo fichero que usaba ActualizaPrecios, así que se puede añadir un tipo nuevo sin
        ''' recompilar, que es de lo que se trataba.
        ''' </summary>
        Public Const TiposXml As String = "tiposxml"

        ''' <summary>
        ''' Productos de los dos entornos, con «(luz)» o «(gas)» detrás. ActualizaPrecios solo
        ''' mostraba los del entorno del PRIMER contrato de la lista y los aplicaba a todos, así
        ''' que con una lista mezclada metía productos de luz en contratos de gas. Aquí se ven
        ''' los dos y la operación comprueba contrato a contrato que el entorno cuadre.
        ''' </summary>
        Public Const Productos As String = "productos"

        ''' <summary>Tipos de impuesto. Admite vaciar: entonces manda el del contrato.</summary>
        Public Const TiposImpuesto As String = "tiposimpuesto"

        ' --- Listas de «Masivo contrato» ---
        Public Const SituacionesContrato As String = "situacionescontrato"
        Public Const Cnaes As String = "cnaes"
        Public Const Colectivos As String = "colectivos"
        Public Const SituacionesScoring As String = "situacionesscoring"
        Public Const TiposAutoconsumo As String = "tiposautoconsumo"

        ''' <summary>Modelos de impresión, cada uno por su tipo.</summary>
        Public Const ModelosFactura As String = "modelosfactura"          ' tipo 1
        Public Const ModelosFacturaVarios As String = "modelosfacturavarios"  ' tipo 9
        Public Const ModelosContrato As String = "modeloscontrato"        ' tipo 4

        ''' <summary>
        ''' Clientes de pago de un CIF. Necesita filtro: sin él no se trae nada, porque la tabla
        ''' entera son decenas de miles de filas.
        ''' </summary>
        Public Const ClientesPago As String = "clientespago"

        ''' <summary>True si el origen no necesita conexión a la base.</summary>
        Public Function EsDeConfiguracion(origen As String) As Boolean
            Return origen = TiposXml
        End Function

        ''' <summary>True si el origen no devuelve nada hasta que se le da un filtro.</summary>
        Public Function NecesitaFiltro(origen As String) As Boolean
            Return origen = ClientesPago
        End Function

    End Module

    ''' <summary>
    ''' Un campo propio de una operación, declarado en el catálogo.
    '''
    ''' Existe porque los flags de <see cref="EntradasOperacion"/> solo llegan hasta cierto
    ''' punto: «Añadir códigos DIR» pide tres textos distintos y «Cambiar agente» pide elegir
    ''' de una lista de la base. Con esto el formulario los pinta y los valida sin que haya que
    ''' tocar la vista, igual que con el resto.
    ''' </summary>
    Public Class CampoOperacion

        ''' <summary>Con qué nombre llega el valor al ejecutable.</summary>
        Public Property Clave As String = ""

        Public Property Etiqueta As String = ""

        Public Property Ayuda As String = ""

        Public Property Tipo As TipoCampo = TipoCampo.Texto

        Public Property Requerido As Boolean = True

        ''' <summary>Longitud máxima, para los de texto. 0 = sin límite.</summary>
        Public Property LongitudMaxima As Integer

        ''' <summary>Para los de selección: <see cref="OrigenLista"/>.</summary>
        Public Property Origen As String = ""

        ''' <summary>
        ''' Texto de la opción que significa «ninguno». Solo en los de selección que admiten
        ''' vaciar el valor, como el administrador, que se puede dejar en NULL.
        ''' </summary>
        Public Property EtiquetaVacio As String = ""

        Public ReadOnly Property AdmiteVacio As Boolean
            Get
                Return Not String.IsNullOrEmpty(EtiquetaVacio)
            End Get
        End Property

        ''' <summary>Para los numéricos. Si los dos son 0 no se acota.</summary>
        Public Property Minimo As Integer
        Public Property Maximo As Integer

        ''' <summary>
        ''' Filtro del selector, para los de fichero. Con el formato de OpenFileDialog:
        ''' «Ficheros XML (*.xml)|*.xml|Todos los ficheros (*.*)|*.*».
        ''' </summary>
        Public Property FiltroFichero As String = ""

        ''' <summary>Valor con el que arranca el campo. Vacío para empezar en blanco.</summary>
        Public Property ValorInicial As String = ""

        ''' <summary>
        ''' Opciones escritas aquí mismo, para los desplegables que no salen de la base: «Sí /
        ''' No / no tocar», el tipo de impresión, el modo de escribir en un campo de texto.
        '''
        ''' Es lo que permite expresar los tres estados que necesita «Masivo contrato»: en
        ''' ActualizaPrecios cada campo llevaba DOS casillas —una para «tocar esto» y otra para
        ''' el sí o el no—, y marcar la segunda sin la primera no hacía nada. Con un desplegable
        ''' de tres opciones eso no puede pasar.
        ''' </summary>
        Public Property OpcionesFijas As IReadOnlyList(Of OpcionFija) = Array.Empty(Of OpcionFija)()

        ''' <summary>
        ''' Clave de otro campo del que depende esta lista. Cuando ese campo cambia, las
        ''' opciones se recargan pasando su valor como filtro.
        '''
        ''' Lo necesita el cliente de pago: en el original se escribía un CIF, se pulsaba
        ''' «Buscar Cliente Pago» y el desplegable se llenaba con los de ese CIF.
        ''' </summary>
        Public Property DependeDe As String = ""

        ''' <summary>
        ''' Cuando es True, el desplegable solo muestra las opciones del suministro que tenga la
        ''' lista pegada: si los contratos son de luz, solo las de luz.
        '''
        ''' Es para los maestros que SIGE tiene duplicados por entorno —los productos viven en
        ''' G1 los de luz y en G2 los de gas— y que no se pueden cruzar. Sin esto salían los de
        ''' los dos entornos y era fácil elegir uno que no valía para ningún contrato de la
        ''' lista, y no enterarse hasta ver el resultado.
        '''
        ''' No sustituye a la comprobación por contrato que hace la operación antes de escribir:
        ''' esto ayuda a elegir bien, aquello impide escribir mal. Si la base no responde o los
        ''' contratos no existen, el suministro queda sin resolver y se muestran todas.
        ''' </summary>
        Public Property FiltraPorSuministro As Boolean

    End Class

    ''' <summary>Una opción declarada en el catálogo, con el valor que viaja y lo que se lee.</summary>
    Public Class OpcionFija

        Public Property Valor As String = ""
        Public Property Etiqueta As String = ""

        Public Sub New()
        End Sub

        Public Sub New(valor As String, etiqueta As String)
            Me.Valor = valor
            Me.Etiqueta = etiqueta
        End Sub

    End Class

    ''' <summary>
    ''' Las tres opciones de un campo booleano que se puede dejar sin tocar. Se usan tanto en el
    ''' catálogo como en las operaciones, así que viven aquí y no repetidas en cada sitio.
    ''' </summary>
    Public Module SiNoSinTocar

        Public Const SinTocar As String = ""
        Public Const Si As String = "1"
        Public Const No As String = "0"

        Public Function Opciones() As IReadOnlyList(Of OpcionFija)
            Return {
                New OpcionFija(SinTocar, "— no tocar —"),
                New OpcionFija(Si, "Sí"),
                New OpcionFija(No, "No")
            }
        End Function

    End Module

End Namespace
