Option Strict Off

Imports System.IO
Imports ClosedXML.Excel

''' <summary>
''' ADAPTADOR DE COMPATIBILIDAD con EPPlus, montado sobre ClosedXML.
'''
''' Existe para poder traer PenalizacionesLuz y PenalizacionesGas de ActualizaPrecios
''' LITERALMENTE, sin reescribir sus 346 líneas de lógica de penalizaciones. De EPPlus solo
''' usaban seis miembros —Cells, Row, Workbook, Save, el constructor y LicenseContext— así que
''' reproducirlos aquí sale mucho más barato y seguro que traducir el cálculo a mano.
'''
''' Y evita la dependencia de EPPlus, que es de pago desde la versión 5.
'''
''' NO es un EPPlus completo ni pretende serlo: si al portar otra cosa falta un miembro, el
''' compilador lo dirá y se añade. Mejor eso que dar por hecho que está.
''' </summary>
Public Enum LicenseContext
    NonCommercial
    Commercial
End Enum

''' <summary>Celda: valor y estilo, que es lo único que se usa.</summary>
Public Class CeldaCompat

    Private ReadOnly _celda As IXLCell

    Friend Sub New(celda As IXLCell)
        _celda = celda
    End Sub

    ''' <summary>
    ''' Se devuelve el IXLStyle de ClosedXML tal cual, sin envolverlo. El código portado escribe
    ''' «Style.Numberformat.Format = ...» —así, con f minúscula, que es como lo llama EPPlus— y
    ''' en ClosedXML la propiedad es NumberFormat. Compila igual porque VB no distingue
    ''' mayúsculas en los nombres de miembro.
    ''' </summary>
    Public ReadOnly Property Style As IXLStyle
        Get
            Return _celda.Style
        End Get
    End Property

    ''' <summary>
    ''' El valor de la celda CON LA SEMÁNTICA DE EPPlus: Nothing si la celda está vacía.
    '''
    ''' Y esto no es un detalle. ClosedXML devuelve un XLCellValue, que es una ESTRUCTURA: al
    ''' salir por una propiedad declarada As Object se empaqueta, y una estructura empaquetada
    ''' nunca es Nothing. El código portado está lleno de comprobaciones del estilo
    ''' «If Cells(fila, 2).Value IsNot Nothing», que devolviendo el XLCellValue tal cual daban
    ''' verdadero SIEMPRE, incluso en una celda vacía.
    '''
    ''' Casi siempre daba igual, porque XLCellValue.ToString() de una celda vacía es "" y el
    ''' código acaba mirando String.IsNullOrEmpty. Pero no siempre: en ActualizarEmailFromExcel
    ''' una fila con email y sin código de contrato hacía CLng("") y eso lanza
    ''' InvalidCastException, que allí no está capturada y tumbaba la operación entera. Con
    ''' EPPlus el valor era Nothing, CLng(Nothing) da 0, y la fila salía como «No existe en BD».
    ''' </summary>
    Public Property Value As Object
        Get
            If _celda.IsEmpty() Then Return Nothing
            Return _celda.Value
        End Get
        Set(v As Object)
            If v Is Nothing OrElse Convert.IsDBNull(v) Then
                _celda.Clear(XLClearOptions.Contents)
            Else
                _celda.Value = XLCellValue.FromObject(v)
            End If
        End Set
    End Property

End Class

''' <summary>Fila: se usa solo para poner la cabecera en negrita.</summary>
Public Class FilaCompat

    Private ReadOnly _fila As IXLRow

    Friend Sub New(fila As IXLRow)
        _fila = fila
    End Sub

    Public ReadOnly Property Style As IXLStyle
        Get
            Return _fila.Style
        End Get
    End Property

End Class

''' <summary>Lo que devuelve worksheet.Dimension: solo se usa Rows.</summary>
Public Class DimensionCompat

    Public ReadOnly Property Rows As Integer
    Public ReadOnly Property Columns As Integer

    Friend Sub New(filas As Integer, columnas As Integer)
        Rows = filas
        Columns = columnas
    End Sub

End Class

''' <summary>
''' La hoja. Se llama ExcelWorksheet, con el nombre de EPPlus y no HojaCompat, PARA QUE EL
''' CÓDIGO PORTADO PUEDA DECLARARLA: ActualizarEmailFromExcel hace
''' «Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(0)», tal cual venía.
'''
''' AQUÍ HUBO UN FALLO, y conviene dejarlo escrito para no repetirlo: estaba como HojaCompat y
''' ExcelWorksheet era una clase aparte que HEREDABA de ella. Esa línea quedaba entonces
''' asignando la base a la derivada —un downcast— y lanzaba en tiempo de ejecución
''' «Unable to cast object of type HojaCompat to type ExcelWorksheet» SIEMPRE, en la primera
''' línea de la operación. Compilaba en silencio solo por el Option Strict Off de arriba.
'''
''' Así que en este adaptador los tipos llevan el nombre de EPPlus y no hay herencia de por
''' medio: si falta un miembro, el compilador lo dice; si falta un tipo, también. Lo que no
''' puede volver a pasar es que compile y falle al ejecutarse.
''' </summary>
Public Class ExcelWorksheet

    Friend ReadOnly Hoja As IXLWorksheet

    Friend Sub New(hoja As IXLWorksheet)
        Me.Hoja = hoja
    End Sub

    Public ReadOnly Property Name As String
        Get
            Return Hoja.Name
        End Get
    End Property

    ''' <summary>Cells(fila, columna), con los índices en base 1 como en EPPlus.</summary>
    Public ReadOnly Property Cells(fila As Integer, columna As Integer) As CeldaCompat
        Get
            Return New CeldaCompat(Hoja.Cell(fila, columna))
        End Get
    End Property

    Public ReadOnly Property Row(numero As Integer) As FilaCompat
        Get
            Return New FilaCompat(Hoja.Row(numero))
        End Get
    End Property

    ''' <summary>
    ''' Rango usado de la hoja. En EPPlus es Nothing cuando la hoja está vacía y el código
    ''' portado hace «worksheet.Dimension.Rows» sin comprobarlo: eso lanzaba
    ''' NullReferenceException con un fichero vacío. Aquí se devuelve una dimensión de cero
    ''' filas, que hace que el bucle «For row = 2 To rowCount» simplemente no entre.
    ''' </summary>
    Public ReadOnly Property Dimension As DimensionCompat
        Get
            Dim usado = Hoja.RangeUsed()
            If usado Is Nothing Then Return New DimensionCompat(0, 0)
            Return New DimensionCompat(usado.LastRow().RowNumber(),
                                       usado.LastColumn().ColumnNumber())
        End Get
    End Property

End Class

Public Class HojasCompat

    Private ReadOnly _libro As XLWorkbook

    Friend Sub New(libro As XLWorkbook)
        _libro = libro
    End Sub

    ''' <summary>
    ''' Excel no admite más de 31 caracteres ni los caracteres : \ / ? * [ ] en el nombre de
    ''' hoja. EPPlus lo dejaba pasar y fallaba al guardar; aquí se sanea.
    ''' </summary>
    Public Function Add(nombre As String) As ExcelWorksheet

        Dim limpio = If(String.IsNullOrWhiteSpace(nombre), "Hoja1", nombre)
        For Each c In ":\/?*[]"
            limpio = limpio.Replace(c, "_"c)
        Next
        If limpio.Length > 31 Then limpio = limpio.Substring(0, 31)

        Return New ExcelWorksheet(_libro.Worksheets.Add(limpio))

    End Function

    ''' <summary>
    ''' Worksheets(0) — en EPPlus el índice es base 0; en ClosedXML, base 1. Se traduce aquí,
    ''' que es justo la clase de detalle que hace falta para no tocar el código portado.
    ''' </summary>
    Default Public ReadOnly Property Item(indice As Integer) As ExcelWorksheet
        Get
            Return New ExcelWorksheet(_libro.Worksheet(indice + 1))
        End Get
    End Property

    ''' <summary>Worksheets("Hoja1") — por nombre el índice es el mismo en las dos.</summary>
    Default Public ReadOnly Property Item(nombre As String) As ExcelWorksheet
        Get
            Return New ExcelWorksheet(_libro.Worksheet(nombre))
        End Get
    End Property

End Class

Public Class LibroCompat

    Public ReadOnly Property Worksheets As HojasCompat

    Friend Sub New(libro As XLWorkbook)
        Worksheets = New HojasCompat(libro)
    End Sub

End Class

''' <summary>
''' Sustituto de OfficeOpenXml.ExcelPackage para lo que usaban las penalizaciones.
''' </summary>
Public Class ExcelPackage
    Implements IDisposable

    ''' <summary>
    ''' Solo existe para que compile «ExcelPackage.LicenseContext = ...». ClosedXML no tiene
    ''' licencia que aceptar, así que no hace nada.
    ''' </summary>
    Public Shared Property LicenseContext As LicenseContext

    Private ReadOnly _libro As XLWorkbook
    Private ReadOnly _ruta As String
    Private _cerrado As Boolean

    Public ReadOnly Property Workbook As LibroCompat

    Public Sub New(fichero As FileInfo)
        Me.New(fichero.FullName)
    End Sub

    ''' <summary>
    ''' Igual que EPPlus: si el fichero existe se ABRE, y si no, se crea vacío. Esto es
    ''' imprescindible para el código portado que LEE un Excel —«Aplicar precios desde Excel»,
    ''' «Actualizar email y móvil»—, que hace New ExcelPackage(fichero) y luego lee celdas.
    '''
    ''' Se abre en copia de solo lectura para no bloquear el fichero: estos Excel los suele
    ''' tener abierto quien los ha preparado, y con bloqueo exclusivo fallaría al leerlos.
    ''' </summary>
    Public Sub New(ruta As String)

        _ruta = ruta
        Dim carpeta = Path.GetDirectoryName(ruta)
        If Not String.IsNullOrEmpty(carpeta) Then Directory.CreateDirectory(carpeta)

        If File.Exists(ruta) Then
            Using flujo As New FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                _libro = New XLWorkbook(flujo)
            End Using
        Else
            _libro = New XLWorkbook()
        End If

        Workbook = New LibroCompat(_libro)

    End Sub

    Public Sub Save()
        _libro.SaveAs(_ruta)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If _cerrado Then Exit Sub
        _cerrado = True
        _libro.Dispose()
    End Sub

End Class
