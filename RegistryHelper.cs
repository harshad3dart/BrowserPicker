using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace BrowserPicker;

public static class RegistryHelper
{
    private const string AppName = "BrowserPicker";
    private const string AppDisplayName = "Browser Picker";
    private const string UrlProgId = "BrowserPickerURL";
    private const string HtmlProgId = "BrowserPickerHTML";

    public static bool Register()
    {
        try
        {
            string exePath = Process.GetCurrentProcess().MainModule?.FileName 
                ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BrowserPicker.exe");

            // 1. HKCU\Software\Clients\StartMenuInternet\BrowserPicker
            string clientKeyPath = $@"Software\Clients\StartMenuInternet\{AppName}";
            using (var key = Registry.CurrentUser.CreateSubKey(clientKeyPath))
            {
                key.SetValue("", AppDisplayName);

                using (var iconKey = key.CreateSubKey("DefaultIcon"))
                {
                    iconKey.SetValue("", $"{exePath},0");
                }

                using (var cmdKey = key.CreateSubKey(@"shell\open\command"))
                {
                    cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }

                using (var capKey = key.CreateSubKey("Capabilities"))
                {
                    capKey.SetValue("ApplicationDescription", "Choose which browser or profile to open links and OAuth requests in.");
                    capKey.SetValue("ApplicationIcon", $"{exePath},0");
                    capKey.SetValue("ApplicationName", AppDisplayName);

                    using (var startMenu = capKey.CreateSubKey("StartMenu"))
                    {
                        startMenu.SetValue("StartMenuInternet", AppName);
                    }

                    using (var fileAssoc = capKey.CreateSubKey("FileAssociations"))
                    {
                        fileAssoc.SetValue(".htm", HtmlProgId);
                        fileAssoc.SetValue(".html", HtmlProgId);
                        fileAssoc.SetValue(".shtml", HtmlProgId);
                        fileAssoc.SetValue(".xhtml", HtmlProgId);
                    }

                    using (var urlAssoc = capKey.CreateSubKey("URLAssociations"))
                    {
                        urlAssoc.SetValue("http", UrlProgId);
                        urlAssoc.SetValue("https", UrlProgId);
                    }
                }
            }

            // 2. HKCU\Software\RegisteredApplications
            using (var regAppsKey = Registry.CurrentUser.CreateSubKey(@"Software\RegisteredApplications"))
            {
                regAppsKey.SetValue(AppName, $@"Software\Clients\StartMenuInternet\{AppName}\Capabilities");
            }

            // 3. HKCU\Software\Classes\BrowserPickerURL
            using (var urlKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{UrlProgId}"))
            {
                urlKey.SetValue("", "Browser Picker URL");
                urlKey.SetValue("URL Protocol", "");
                urlKey.SetValue("FriendlyTypeName", "Browser Picker Web Link");

                using (var iconKey = urlKey.CreateSubKey("DefaultIcon"))
                {
                    iconKey.SetValue("", $"{exePath},0");
                }

                using (var cmdKey = urlKey.CreateSubKey(@"shell\open\command"))
                {
                    cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }

            // 4. HKCU\Software\Classes\BrowserPickerHTML
            using (var htmlKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{HtmlProgId}"))
            {
                htmlKey.SetValue("", "Browser Picker HTML Document");
                htmlKey.SetValue("FriendlyTypeName", "Browser Picker HTML Document");

                using (var iconKey = htmlKey.CreateSubKey("DefaultIcon"))
                {
                    iconKey.SetValue("", $"{exePath},0");
                }

                using (var cmdKey = htmlKey.CreateSubKey(@"shell\open\command"))
                {
                    cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Registration failed: {ex.Message}");
            return false;
        }
    }

    public static void OpenDefaultAppsSettings()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-settings:defaultapps",
                UseShellExecute = true
            });
        }
        catch { }
    }
}
