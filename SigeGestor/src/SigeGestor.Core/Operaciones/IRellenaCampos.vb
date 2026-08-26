Namespace Operaciones

    ''' <summary>
    ''' Lo implementa la operación en la que elegir el valor de un campo rellena otros.
    '''
    ''' Existe por «Añadir productos a contratos»: en ActualizaPrecios, al elegir el producto del
    ''' desplegable se rellenaban solos el importe, el tipo de impuesto y tres casillas con los
    ''' valores del propio producto, y el usuario los podía cambiar antes de aplicar. Sin esto
    ''' habría que teclear a mano cinco cosas que la base ya sabe.
    '''
    ''' Se declara aquí y no en la vista para que el formulario genérico siga sirviendo: la
    ''' vista solo pregunta «¿este cambio rellena algo?» y aplica lo que le devuelvan.
    ''' </summary>
    Public Interface IRellenaCampos

        ''' <summary>
        ''' Qué valores poner en otros campos cuando <paramref name="clave"/> pasa a valer
        ''' <paramref name="valor"/>. Diccionario vacío si ese campo no rellena nada.
        '''
        ''' Las claves son las de los campos declarados y los valores van en el mismo formato en
        ''' que los entrega el formulario: los booleanos como «1» o «0», las fechas en ISO.
        ''' </summary>
        Function RellenarAsync(cadenaConexion As String,
                               clave As String,
                               valor As String) As Task(Of IReadOnlyDictionary(Of String, String))

    End Interface

End Namespace
