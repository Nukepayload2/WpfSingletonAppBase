Imports System.ComponentModel

''' <summary>
''' Signature for the StartupNextInstance event handler
''' </summary>
<EditorBrowsable(EditorBrowsableState.Advanced)>
Public Delegate Sub StartupNextInstanceEventHandler(sender As Object, e As StartupNextInstanceEventArgs)

''' <summary>
''' Signature for the Shutdown event handler
''' </summary>
<EditorBrowsable(EditorBrowsableState.Advanced)>
Public Delegate Sub ShutdownEventHandler(sender As Object, e As EventArgs)

''' <summary>
''' Signature for the UnhandledException event handler
''' </summary>
<EditorBrowsable(EditorBrowsableState.Advanced)>
Public Delegate Sub UnhandledExceptionEventHandler(sender As Object, e As UnhandledExceptionEventArgs)
