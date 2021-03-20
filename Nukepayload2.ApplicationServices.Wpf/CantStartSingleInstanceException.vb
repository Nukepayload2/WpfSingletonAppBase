Imports System.ComponentModel

''' <summary>
''' Exception for when we launch a single-instance application and it can't connect with the 
''' original instance.
''' </summary>
<EditorBrowsable(EditorBrowsableState.Never)>
<Serializable>
Public Class CantStartSingleInstanceException
    Inherits Exception

    ''' <summary>
    '''  Creates a new exception
    ''' </summary>
    Public Sub New()
        MyBase.New("Unable to connect to the started instance.")
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As System.Exception)
        MyBase.New(message, inner)
    End Sub

    ' Deserialization constructor must be defined since we are serializable
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Protected Sub New(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class
