Namespace Operaciones

    ''' <summary>
    ''' Una columna que la operación espera en el Excel, en su posición.
    '''
    ''' Estas operaciones leen POR POSICIÓN, no por el nombre de la cabecera: la columna 1 es el
    ''' contrato pase lo que pase, y da igual que ponga «CodContrato» o «C». Así que lo único
    ''' que importa es el ORDEN, y es justo lo que no se veía en ninguna parte: quien preparaba
    ''' el fichero tenía que adivinarlo o preguntar.
    ''' </summary>
    Public Class ColumnaExcel

        Public Sub New(titulo As String,
                       ejemplo As String,
                       Optional nota As String = "",
                       Optional obligatoria As Boolean = True)

            Me.Titulo = titulo
            Me.Ejemplo = ejemplo
            Me.Nota = nota
            Me.Obligatoria = obligatoria

        End Sub

        ''' <summary>Lo que conviene poner en la cabecera. Orientativo: no se lee.</summary>
        Public ReadOnly Property Titulo As String

        ''' <summary>Un valor de muestra, con el formato que espera la operación.</summary>
        Public ReadOnly Property Ejemplo As String

        ''' <summary>Aclaración corta, si el formato tiene alguna regla que no se ve sola.</summary>
        Public ReadOnly Property Nota As String

        ''' <summary>Si va vacía, la fila no sirve.</summary>
        Public ReadOnly Property Obligatoria As Boolean

    End Class

    ''' <summary>
    ''' Cómo tiene que estar montado el Excel de una operación, para poder enseñárselo a quien
    ''' lo prepara.
    ''' </summary>
    Public Class EsquemaExcel

        Public Sub New(columnas As IReadOnlyList(Of ColumnaExcel),
                       Optional hoja As String = "",
                       Optional filasDeCabecera As Integer = 1,
                       Optional aviso As String = "")

            Me.Columnas = columnas
            Me.Hoja = hoja
            Me.FilasDeCabecera = filasDeCabecera
            Me.Aviso = aviso

        End Sub

        Public ReadOnly Property Columnas As IReadOnlyList(Of ColumnaExcel)

        ''' <summary>
        ''' Nombre exacto que tiene que tener la hoja, o cadena vacía si se lee la primera y da
        ''' igual cómo se llame. Solo «Aplicar precios desde Excel» lo exige, y el original
        ''' lanzaba NullReferenceException sin explicar nada cuando no coincidía.
        ''' </summary>
        Public ReadOnly Property Hoja As String

        ''' <summary>
        ''' Filas que la operación se salta al principio. Es 1 en las tres que leen Excel, pero
        ''' va aquí y no fijo en la pantalla: si algún día se porta una que lea desde la fila 1,
        ''' lo que se muestre tiene que cambiar con ella. Enseñar una cabecera que la operación
        ''' no salta haría perder la primera fila de datos sin avisar.
        ''' </summary>
        Public ReadOnly Property FilasDeCabecera As Integer

        ''' <summary>Lo que no encaja en la nota de una columna concreta.</summary>
        Public ReadOnly Property Aviso As String

        ''' <summary>Número de la primera fila con datos.</summary>
        Public ReadOnly Property PrimeraFilaConDatos As Integer
            Get
                Return FilasDeCabecera + 1
            End Get
        End Property

    End Class

    ''' <summary>
    ''' La implementa la operación que lee un Excel, para que el formulario pueda dibujar cómo
    ''' hay que montarlo antes de elegir el fichero.
    '''
    ''' Se declara en la propia operación y no en el catálogo a propósito: las columnas y su
    ''' orden son lo que lee su código, y teniéndolo al lado no se puede desincronizar sin que
    ''' se vea.
    ''' </summary>
    Public Interface IEsquemaExcel

        ReadOnly Property Esquema As EsquemaExcel

    End Interface

End Namespace
