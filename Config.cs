using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BrowserPicker;

public class BrowserOption
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string ExecutablePath { get; set; } = "";
    public string Arguments { get; set; } = "";
    public string ShortcutKey { get; set; } = "";
    public string AltKey { get; set; } = "";
    public string BadgeText { get; set; } = "";
    public string BadgeColor { get; set; } = "#4F46E5";
    public string ShortcutDisplay => $"[{ShortcutKey}]";
}

public class AppConfig
{
    public List<BrowserOption> Browsers { get; set; } = new();

    public static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    public static AppConfig LoadOrCreateDefault()
    {
        try
        {
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                var config = JsonSerializer.Deserialize<AppConfig>(json);
                if (config != null && config.Browsers.Count > 0)
                {
                    return config;
                }
            }
        }
        catch { }

        var defaultConfig = CreateDefault();
        defaultConfig.Save();
        return defaultConfig;
    }

    public void Save()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(ConfigPath, json);
        }
        catch { }
    }

    public static AppConfig CreateDefault()
    {
        string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
        if (!File.Exists(chromePath))
        {
            string localChrome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\Application\chrome.exe");
            if (File.Exists(localChrome)) chromePath = localChrome;
        }

        string firefoxPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
        if (!File.Exists(firefoxPath))
        {
            string x86Firefox = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            if (File.Exists(x86Firefox)) firefoxPath = x86Firefox;
        }

        string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";

        return new AppConfig
        {
            Browsers = new List<BrowserOption>
            {
                new BrowserOption
                {
                    Id = "firefox",
                    Name = "Firefox",
                    Subtitle = "Default Browser",
                    ExecutablePath = firefoxPath,
                    Arguments = "",
                    ShortcutKey = "1",
                    AltKey = "F",
                    BadgeText = "FF",
                    BadgeColor = "#FF6611"
                },
                new BrowserOption
                {
                    Id = "chrome_work",
                    Name = "Chrome · Work",
                    Subtitle = "work@example.com",
                    ExecutablePath = chromePath,
                    Arguments = "--profile-directory=\"Profile 1\"",
                    ShortcutKey = "2",
                    AltKey = "W",
                    BadgeText = "H",
                    BadgeColor = "#6D4C41" // Brown badge matching user's profile
                },
                new BrowserOption
                {
                    Id = "chrome_personal",
                    Name = "Chrome · Personal",
                    Subtitle = "personal@example.com",
                    ExecutablePath = chromePath,
                    Arguments = "--profile-directory=\"Profile 2\"",
                    ShortcutKey = "3",
                    AltKey = "P",
                    BadgeText = "H",
                    BadgeColor = "#5C6BC0" // Indigo/purple badge matching user's profile
                },
                new BrowserOption
                {
                    Id = "edge",
                    Name = "Microsoft Edge",
                    Subtitle = "System Browser",
                    ExecutablePath = edgePath,
                    Arguments = "",
                    ShortcutKey = "4",
                    AltKey = "E",
                    BadgeText = "e",
                    BadgeColor = "#0078D7"
                }
            }
        };
    }
}
