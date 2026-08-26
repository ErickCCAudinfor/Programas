Option Strict Off   ' Portado de ActualizaPrecios, que se escribió sin Option Strict.
                    ' Se deja tal cual a propósito: reescribirlo para satisfacer al
                    ' compilador sería tocar lógica de negocio sin necesidad.
''' <summary>
''' Una base de datos de cliente: nombre, servidor, catálogo y credenciales cifradas.
'''
''' NO ES LO MISMO QUE EntornoBD, y conviene tenerlo claro. Los entornos son tres y fijos
''' —Producción, Réplica, UAT— y son contra dónde se lanzan las operaciones. Las empresas son
''' una lista abierta de bases de clientes —Sige PROD, Adelfas, TOTAL UAT…— y solo se usan
''' para el mantenimiento de modelos de impresión, que hay que subir a cada una por separado.
'''
''' Portada de ActualizaPrecios sin tocar los campos, para que Config\Empresas.json pueda ser
''' una copia literal de su Json\EmpresasBD.json, cifrado incluido. Solo se le ha añadido lo
''' que necesita la pantalla nueva.
''' </summary>
Public Class EmpresaBD
    Public Property Nombre As String
    Public Property Servidor As String
    Public Property BaseDatos As String
    Public Property Usuario As String
    Public Property Password As String
    Public Property VPN As Boolean

    ''' <summary>Lo que se muestra junto al nombre: «SRVSQL1 · SigeAdelfas».</summary>
    Public ReadOnly Property Descripcion As String
        Get
            Return $"{Servidor} · {BaseDatos}"
        End Get
    End Property

    ''' <summary>
    ''' Copia para editar sin tocar la de la lista: así cancelar el diálogo no deja a medias
    ''' los cambios que se hubieran escrito en los campos.
    ''' </summary>
    Public Function Copiar() As EmpresaBD
        Return New EmpresaBD With {
            .Nombre = Nombre,
            .Servidor = Servidor,
            .BaseDatos = BaseDatos,
            .Usuario = Usuario,
            .Password = Password,
            .VPN = VPN
        }
    End Function

End Class
