Imports System.Reflection
Imports System.Runtime.Serialization

Public Module EnumHelper

    ' Obtiene el valor del EnumMember si está, sino el nombre del enum
    Public Function GetEnumDisplayName(value As [Enum]) As String
        Dim field As FieldInfo = value.GetType().GetField(value.ToString())
        Dim attr = TryCast(field.GetCustomAttribute(GetType(EnumMemberAttribute)), EnumMemberAttribute)
        If attr IsNot Nothing Then
            Return value.ToString() ' EnumMember no tiene Name, usamos ToString
        End If
        Return value.ToString()
    End Function

    ' Convierte cualquier enum a lista de objetos para ComboBox
    Public Function EnumToComboBoxList(Of T As Structure)() As List(Of Object)
        Dim enumValues = [Enum].GetValues(GetType(T)).Cast(Of [Enum])()
        Dim list As New List(Of Object)

        For Each Valu In enumValues
            list.Add(New With {
                         Key .Value = Convert.ToInt32(Valu),
                         Key .Text = GetEnumDisplayName(Valu)
                     })
        Next

        Return list
    End Function

End Module
