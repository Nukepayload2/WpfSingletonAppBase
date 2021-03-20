Imports Nukepayload2.ApplicationServices.Wpf

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Sub Application_StartupNextInstance(sender As Object, e As StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
        MsgBox("Started a new instance.", vbInformation, "Singleton app")
    End Sub

End Class
