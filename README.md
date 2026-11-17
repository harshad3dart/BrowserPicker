# Browser Picker for Windows

A custom, lightweight, keyboard-friendly browser interceptor built for developers and multi-account workflows.

Whenever any app triggers an **OAuth link** or **browser redirect**, this interceptor pops up and lets you choose which browser and profile to use.

---

## ⚡ Quick Start

### 1. Set as Default Browser
Run `Register.ps1` (or use the built-in option in the UI):
- In Windows Settings under **Default Apps** > **Web browser**, select **Browser Picker**.

### 2. When an OAuth Link Opens
The picker window will appear instantly:
- **`1`** or **`F`** &rarr; **Firefox** (Default browser)
- **`2`** or **`W`** &rarr; **Chrome · Work** (`work@example.com`)
- **`3`** or **`P`** &rarr; **Chrome · Personal** (`personal@example.com`)
- **`4`** or **`E`** &rarr; **Microsoft Edge**
- **`C`** &rarr; Copy URL to clipboard
- **`Esc`** &rarr; Cancel / dismiss

---

## ⚙ Customizing Profiles

Open `config.json` next to `BrowserPicker.exe` to customize names, email labels, colors, or Chrome profiles:

```json
{
  "Browsers": [
    {
      "Id": "firefox",
      "Name": "Firefox",
      "Subtitle": "Default Browser",
      "ExecutablePath": "C:\\Program Files\\Mozilla Firefox\\firefox.exe",
      "Arguments": "",
      "ShortcutKey": "1",
      "AltKey": "F",
      "BadgeText": "FF",
      "BadgeColor": "#FF5500"
    },
    {
      "Id": "chrome_work",
      "Name": "Chrome · Work",
      "Subtitle": "work@example.com",
      "ExecutablePath": "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
      "Arguments": "--profile-directory=\"Profile 1\"",
      "ShortcutKey": "2",
      "AltKey": "W",
      "BadgeText": "H",
      "BadgeColor": "#6D4C41"
    },
    {
      "Id": "chrome_personal",
      "Name": "Chrome · Personal",
      "Subtitle": "personal@example.com",
      "ExecutablePath": "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
      "Arguments": "--profile-directory=\"Profile 2\"",
      "ShortcutKey": "3",
      "AltKey": "P",
      "BadgeText": "H",
      "BadgeColor": "#5C6BC0"
    }
  ]
}
```

---

## 🔄 Reverting / Unregistering
To revert back to Firefox as your sole default browser:
Run `Unregister.ps1` and select Firefox in Windows Settings.
