Imports System.Globalization
Imports System.Text

Namespace Operaciones

    ''' <summary>Una operación encontrada, con lo que la ha hecho encajar.</summary>
    Public Class ResultadoBusqueda

        Public Property Definicion As DefinicionOperacion

        ''' <summary>Cuanto más alto, más arriba sale.</summary>
        Public Property Puntos As Integer

        ''' <summary>Por qué ha salido: «Contratos», «en la descripción»…</summary>
        Public Property Motivo As String = ""

    End Class

    ''' <summary>
    ''' Busca operaciones por nombre, sección o descripción.
    '''
    ''' Es lo que hace funcionar la caja de la barra lateral, que hasta ahora era un botón con
    ''' un «(en construcción)» en el tooltip. Con 45 operaciones repartidas en seis secciones,
    ''' escribir dos letras es más rápido que recordar dónde estaba cada cosa.
    '''
    ''' SIN TILDES Y SIN MAYÚSCULAS: se comparan las cadenas normalizadas, así «facturacion»
    ''' encuentra «facturación» y «nº» no obliga a escribir el símbolo. Escribiendo en español
    ''' esto no es un lujo: nadie va a teclear las tildes en un buscador.
    ''' </summary>
    Public Module Buscador

        ''' <summary>Puntuación por dónde encaja. El nombre pesa más que la descripción.</summary>
        Private Const PuntosNombreEmpieza As Integer = 100
        Private Const PuntosNombrePalabra As Integer = 80
        Private Const PuntosNombreContiene As Integer = 60
        Private Const PuntosSeccion As Integer = 40
        Private Const PuntosDescripcion As Integer = 20

        Public Function Buscar(texto As String,
                              Optional maximo As Integer = 12) As IReadOnlyList(Of ResultadoBusqueda)

            Dim aguja = Normalizar(texto)
            If aguja.Length = 0 Then Return Array.Empty(Of ResultadoBusqueda)()

            Dim encontrados As New List(Of ResultadoBusqueda)

            For Each op In CatalogoOperaciones.Todas

                Dim nombre = Normalizar(op.Nombre)
                Dim seccion = Normalizar(op.Seccion.ToString())
                Dim descripcion = Normalizar(op.Descripcion)

                Dim puntos = 0
                Dim motivo = op.Seccion.ToString()

                If nombre.StartsWith(aguja, StringComparison.Ordinal) Then
                    puntos = PuntosNombreEmpieza
                ElseIf EmpiezaAlgunaPalabra(nombre, aguja) Then
                    puntos = PuntosNombrePalabra
                ElseIf nombre.Contains(aguja) Then
                    puntos = PuntosNombreContiene
                ElseIf seccion.Contains(aguja) Then
                    puntos = PuntosSeccion
                ElseIf descripcion.Contains(aguja) Then
                    puntos = PuntosDescripcion
                    motivo = $"{op.Seccion} · en la descripción"
                End If

                If puntos = 0 Then Continue For

                ' A igualdad de encaje, primero lo corto: «Masivo contrato» antes que
                ' «Actualizar email, móvil y teléfono» cuando se busca «contrato».
                puntos -= Math.Min(15, op.Nombre.Length \ 4)

                encontrados.Add(New ResultadoBusqueda With {
                    .Definicion = op,
                    .Puntos = puntos,
                    .Motivo = motivo
                })
            Next

            Return encontrados _
                .OrderByDescending(Function(r) r.Puntos) _
                .ThenBy(Function(r) r.Definicion.Nombre, StringComparer.CurrentCulture) _
                .Take(Math.Max(1, maximo)) _
                .ToList()

        End Function

        ''' <summary>
        ''' True si alguna palabra del nombre empieza por lo buscado. Es lo que hace que
        ''' «pdf» encuentre «Extraer PDF de facturas» sin bajar al mismo nivel que un encaje
        ''' a mitad de palabra.
        ''' </summary>
        Private Function EmpiezaAlgunaPalabra(nombre As String, aguja As String) As Boolean

            For Each palabra In nombre.Split(" "c)
                If palabra.StartsWith(aguja, StringComparison.Ordinal) Then Return True
            Next

            Return False

        End Function

        ''' <summary>
        ''' A minúsculas y sin marcas diacríticas. Se descompone en forma D y se descartan las
        ''' marcas, que es la única manera fiable de que «á» y «a» acaben iguales.
        ''' </summary>
        Public Function Normalizar(texto As String) As String

            If String.IsNullOrWhiteSpace(texto) Then Return ""

            Dim descompuesto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD)
            Dim limpio As New StringBuilder(descompuesto.Length)

            For Each c In descompuesto
                If CharUnicodeInfo.GetUnicodeCategory(c) <> UnicodeCategory.NonSpacingMark Then
                    limpio.Append(c)
                End If
            Next

            Return limpio.ToString().Normalize(NormalizationForm.FormC)

        End Function

    End Module

End Namespace
