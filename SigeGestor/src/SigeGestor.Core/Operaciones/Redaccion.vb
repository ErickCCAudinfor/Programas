Namespace Operaciones

    ''' <summary>
    ''' Cómo se redactan los mensajes de resultado.
    '''
    ''' Existe porque los mensajes se veían mal por dos motivos distintos y los dos se
    ''' arreglan aquí:
    '''
    ''' 1. CONCORDANCIA. Se escribían con la plantilla en plural y salía «1 emails
    '''    actualizados», «1 filas», «1 ficheros». Con <see cref="Cuenta"/> el número decide.
    '''
    ''' 2. LAS RUTAS. Media docena de operaciones pegaban la carpeta de salida dentro del
    '''    mensaje —«2 incidencias en C:\Users\ErickCC\Desktop\ConsultasBO\Contactos»— y ese
    '''    mensaje se muestra en tres sitios de una sola línea: la cabecera del progreso, el
    '''    registro y el resumen. En los tres se cortaba, así que la ruta no se leía en
    '''    ninguno, y encima empujaba fuera lo único que importaba: cuántos y de qué.
    '''
    '''    La ruta ya no va en el texto. Va en <see cref="ResultadoEntrada.Salidas"/>, y la
    '''    pantalla la pinta aparte, entera y con un botón para abrir la carpeta. El mensaje se
    '''    queda con lo que se lee de un vistazo.
    ''' </summary>
    Public Module Redaccion

        ''' <summary>
        ''' «1 email», «2 emails». El número elige la forma; se formatea con separador de
        ''' miles, que es lo que hace legible un 41.208.
        ''' </summary>
        Public Function Cuenta(cuantos As Long, singular As String, plural As String) As String
            Return $"{cuantos:N0} {If(cuantos = 1, singular, plural)}"
        End Function

        ''' <summary>
        ''' Para los sustantivos cuyo plural es la palabra más una s, que son casi todos.
        ''' «fichero» -> «1 fichero» / «3 ficheros».
        ''' </summary>
        Public Function Cuenta(cuantos As Long, singular As String) As String
            Return Cuenta(cuantos, singular, singular & "s")
        End Function

        ''' <summary>
        ''' Une trozos de mensaje con el separador de siempre, saltándose los vacíos. Evita el
        ''' « ·  · » que salía al concatenar a mano un trozo que unas veces está y otras no.
        ''' </summary>
        Public Function Unir(ParamArray partes As String()) As String
            Return String.Join(" · ", partes.Where(Function(p) Not String.IsNullOrWhiteSpace(p)))
        End Function

    End Module

End Namespace
