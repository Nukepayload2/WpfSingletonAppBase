Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO.Pipes
Imports System.Reflection
Imports System.Threading
Imports System.Windows

''' <summary>
''' Application base for VB WPF application model.
''' </summary>
''' <remarks>This class replaces <see cref="System.Windows.Application"/>.</remarks>
Public Class WpfApplicationBase
    Inherits System.Windows.Application

    Public Event StartupNextInstance As StartupNextInstanceEventHandler

    Private Const SECOND_INSTANCE_TIMEOUT As Integer = 2500 'milliseconds.  How long a subsequent instance will wait for the original instance to get on its feet.

    Protected Overrides Sub OnStartup(e As System.Windows.StartupEventArgs)
        Dim continueStartup As Boolean
        RunImpl(e.Args, continueStartup)
        If continueStartup Then
            MyBase.OnStartup(e)
        Else
            Shutdown()
        End If
    End Sub

    Private _firstInstanceTokenSource As CancellationTokenSource

    ''' <summary>
    ''' Entry point to kick off the VB Startup/Shutdown Application model
    ''' </summary>
    ''' <param name="commandLine">The command line from Main()</param>
    Public Sub RunImpl(commandLine As String(), ByRef continueStartup As Boolean)
        If Not IsSingleInstance Then
            continueStartup = True
            Return
        End If
        Dim ApplicationInstanceID As String = GetApplicationInstanceID(Assembly.GetCallingAssembly) 'Note: Must pass the calling assembly from here so we can get the running app.  Otherwise, can break single instance.
        Dim pipeServer As NamedPipeServerStream = Nothing
        If TryCreatePipeServer(ApplicationInstanceID, pipeServer) Then
            '--- This is the first instance of a single-instance application to run.  This is the instance that subsequent instances will attach to.
            Dim tokenSource = New CancellationTokenSource()
            _firstInstanceTokenSource = tokenSource
            Dim tsk = WaitForClientConnectionsAsync(pipeServer, AddressOf OnStartupNextInstanceMarshallingAdaptor, cancellationToken:=tokenSource.Token)
            continueStartup = True
        Else '--- We are launching a subsequent instance.
            Dim tokenSource = New CancellationTokenSource()
            tokenSource.CancelAfter(SECOND_INSTANCE_TIMEOUT)
            Try
                Dim awaitable = SendSecondInstanceArgsAsync(
                    ApplicationInstanceID, commandLine,
                    cancellationToken:=tokenSource.Token).ConfigureAwait(False)
                awaitable.GetAwaiter().GetResult()
            Catch ex As Exception
                Throw New CantStartSingleInstanceException()
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Extensibility point which raises the StartupNextInstance
    ''' </summary>
    ''' <param name="eventArgs"></param>
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Protected Overridable Sub OnStartupNextInstance(eventArgs As StartupNextInstanceEventArgs)
        RaiseEvent StartupNextInstance(Me, eventArgs)
        'Activate the original instance
        If eventArgs.BringToForeground AndAlso MainWindow IsNot Nothing Then
            If MainWindow.Visibility <> System.Windows.Visibility.Visible Then
                MainWindow.Show()
            End If
            If MainWindow.WindowState = System.Windows.WindowState.Minimized Then
                MainWindow.WindowState = System.Windows.WindowState.Normal
            End If
            MainWindow.Activate()
        End If
    End Sub

    Private Sub OnStartupNextInstanceMarshallingAdaptor(args As String())
        If Not Dispatcher.CheckAccess Then
            Dispatcher.BeginInvoke(
                Sub() OnStartupNextInstanceMarshallingAdaptor(args))
            Return
        End If
        If MainWindow Is Nothing Then
            Return
        End If
        Dim invoked = False
        Try
            invoked = True
            OnStartupNextInstance(New StartupNextInstanceEventArgs(New ReadOnlyCollection(Of String)(args), bringToForegroundFlag:=True))
        Catch ex As Exception When Not invoked
            ' Only catch exceptions thrown when the UI thread is not available, before
            ' the UI thread has been created or after it has been terminated. Exceptions
            ' thrown from OnStartupNextInstance() should be allowed to propagate.
        End Try
    End Sub

    ''' <summary>
    ''' Generates the name for the remote singleton that we use to channel multiple instances
    ''' to the same application model thread.
    ''' </summary>
    Private Shared Function GetApplicationInstanceID(Entry As Assembly) As String
        Return Entry.ManifestModule.ModuleVersionId.ToString()
    End Function

    ''' <summary>
    ''' Indicates whether this application is singleton.
    ''' </summary>
    Public Property IsSingleInstance As Boolean

    Protected Overrides Sub OnExit(e As ExitEventArgs)
        _firstInstanceTokenSource?.Cancel()
        MyBase.OnExit(e)
    End Sub
End Class