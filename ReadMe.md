## Nukepayload2.ApplicationServices.Wpf
Provides single instance application base for WPF.
- Nuget package: https://www.nuget.org/packages/Nukepayload2.ApplicationServices.Wpf
- Some codes are copied from http://github.com/dotnet/winforms .

### How to use it:
1. Use Nukepayload2.ApplicationServices.Wpf.WpfApplicationBase as the base class of your WPF application.
2. Set IsSingleInstance="True"
```xml
<appsrv:WpfApplicationBase x:Class="Application"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:DemoApp"
             StartupUri="MainWindow.xaml"
             IsSingleInstance="True"
             xmlns:appsrv="clr-namespace:Nukepayload2.ApplicationServices.Wpf;assembly=Nukepayload2.ApplicationServices.Wpf">
    <Application.Resources>
         
    </Application.Resources>
</appsrv:WpfApplicationBase>
```

3. Optionally handle the StartupNextInstance event.
```vbnet
Imports Nukepayload2.ApplicationServices.Wpf

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Private Sub Application_StartupNextInstance(sender As Object, e As StartupNextInstanceEventArgs) Handles Me.StartupNextInstance
        MsgBox("Started a new instance.", vbInformation, "Singleton app")
    End Sub

End Class
```
