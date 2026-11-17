using System;
using System.IO;
using System.Windows;

namespace BrowserPicker;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        string targetUrl = "";

        if (e.Args.Length > 0)
        {
            string firstArg = e.Args[0].Trim();

            if (firstArg.Equals("--register", StringComparison.OrdinalIgnoreCase))
            {
                RegistryHelper.Register();
                RegistryHelper.OpenDefaultAppsSettings();
                Shutdown();
                return;
            }

            if (firstArg.Equals("--open-settings", StringComparison.OrdinalIgnoreCase))
            {
                RegistryHelper.OpenDefaultAppsSettings();
                Shutdown();
                return;
            }

            targetUrl = string.Join(" ", e.Args).Trim().Trim('"', '\'');
        }

        var mainWindow = new MainWindow(targetUrl);
        mainWindow.Show();
    }
}
