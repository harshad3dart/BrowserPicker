using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrowserPicker;

public partial class MainWindow : Window
{
    private readonly string _targetUrl;
    private readonly AppConfig _config;

    public MainWindow(string targetUrl)
    {
        InitializeComponent();
        _targetUrl = targetUrl?.Trim() ?? "";
        _config = AppConfig.LoadOrCreateDefault();
        BrowsersList.ItemsSource = _config.Browsers;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Focus window so keyboard shortcuts work immediately
        Activate();
        Focus();

        if (string.IsNullOrWhiteSpace(_targetUrl))
        {
            TxtDomain.Text = "No URL provided";
            TxtFullUrl.Text = "Launch with a URL or choose a browser to test.";
        }
        else
        {
            try
            {
                if (Uri.TryCreate(_targetUrl, UriKind.Absolute, out var uri))
                {
                    TxtDomain.Text = uri.Host;
                    TxtFullUrl.Text = _targetUrl;
                }
                else
                {
                    TxtDomain.Text = "Opening Link";
                    TxtFullUrl.Text = _targetUrl;
                }
            }
            catch
            {
                TxtDomain.Text = "Web Link";
                TxtFullUrl.Text = _targetUrl;
            }
        }
    }

    private void Header_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void BrowserCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BrowserOption browser)
        {
            LaunchBrowser(browser);
        }
    }

    private void LaunchBrowser(BrowserOption browser)
    {
        try
        {
            string urlToOpen = string.IsNullOrWhiteSpace(_targetUrl) ? "https://google.com" : _targetUrl;

            var startInfo = new ProcessStartInfo
            {
                FileName = browser.ExecutablePath,
                UseShellExecute = true
            };

            if (!string.IsNullOrWhiteSpace(browser.Arguments))
            {
                startInfo.Arguments = $"{browser.Arguments} \"{urlToOpen}\"";
            }
            else
            {
                startInfo.Arguments = $"\"{urlToOpen}\"";
            }

            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to launch {browser.Name}:\n{ex.Message}", "Browser Picker Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Application.Current.Shutdown();
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Application.Current.Shutdown();
            return;
        }

        if (e.Key == Key.C && (Keyboard.Modifiers == ModifierKeys.None || Keyboard.Modifiers == ModifierKeys.Control))
        {
            CopyUrlToClipboard();
            return;
        }

        // Check 1-9
        int numberKey = -1;
        if (e.Key >= Key.D1 && e.Key <= Key.D9)
        {
            numberKey = e.Key - Key.D0;
        }
        else if (e.Key >= Key.NumPad1 && e.Key <= Key.NumPad9)
        {
            numberKey = e.Key - Key.NumPad0;
        }

        if (numberKey > 0)
        {
            var match = _config.Browsers.FirstOrDefault(b => b.ShortcutKey == numberKey.ToString());
            if (match != null)
            {
                LaunchBrowser(match);
                return;
            }
        }

        // Check AltKeys (e.g. F, W, P, E)
        string keyStr = e.Key.ToString().ToUpperInvariant();
        var altMatch = _config.Browsers.FirstOrDefault(b => 
            !string.IsNullOrEmpty(b.AltKey) && b.AltKey.Equals(keyStr, StringComparison.OrdinalIgnoreCase));
        if (altMatch != null)
        {
            LaunchBrowser(altMatch);
            return;
        }
    }

    private void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
        CopyUrlToClipboard();
    }

    private void CopyUrlToClipboard()
    {
        if (!string.IsNullOrEmpty(_targetUrl))
        {
            try
            {
                Clipboard.SetText(_targetUrl);
                TxtDomain.Text = "Copied to clipboard!";
            }
            catch { }
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void BtnSettings_Click(object sender, RoutedEventArgs e)
    {
        var menu = new ContextMenu();

        var itemDefaultApps = new MenuItem { Header = "Set as Default Browser (Windows Settings)" };
        itemDefaultApps.Click += (s, args) =>
        {
            RegistryHelper.Register();
            RegistryHelper.OpenDefaultAppsSettings();
        };
        menu.Items.Add(itemDefaultApps);

        var itemRegister = new MenuItem { Header = "Re-register Registry associations" };
        itemRegister.Click += (s, args) =>
        {
            bool ok = RegistryHelper.Register();
            MessageBox.Show(ok ? "Registered successfully in Windows!" : "Registration failed.", "Browser Picker", MessageBoxButton.OK, MessageBoxImage.Information);
        };
        menu.Items.Add(itemRegister);

        var itemConfig = new MenuItem { Header = "Open config.json (Edit browsers/profiles)" };
        itemConfig.Click += (s, args) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = AppConfig.ConfigPath,
                    UseShellExecute = true
                });
            }
            catch { }
        };
        menu.Items.Add(itemConfig);

        menu.IsOpen = true;
    }
}
