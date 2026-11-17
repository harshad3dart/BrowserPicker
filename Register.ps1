# Register BrowserPicker as an HTTP/HTTPS handler and Windows Web Browser for the current user (No Admin required)
$ErrorActionPreference = "Stop"

$PickerPath = Join-Path $PSScriptRoot "bin\Release\BrowserPicker.exe"
if (-not (Test-Path $PickerPath)) {
    $PickerPath = Join-Path $PSScriptRoot "BrowserPicker.exe"
}

if (-not (Test-Path $PickerPath)) {
    Write-Error "BrowserPicker.exe not found at $PickerPath. Please build it first."
    exit 1
}

$AppName = "BrowserPicker"
$AppTitle = "Browser Picker"
$UrlProgId = "BrowserPickerURL"
$HtmlProgId = "BrowserPickerHTML"

Write-Host "Registering $AppTitle in Windows registry for current user..." -ForegroundColor Cyan

# 1. StartMenuInternet registration
$ClientKey = "HKCU:\Software\Clients\StartMenuInternet\$AppName"
New-Item -Path $ClientKey -Force | Out-Null
Set-ItemProperty -Path $ClientKey -Name "(Default)" -Value $AppTitle

New-Item -Path "$ClientKey\DefaultIcon" -Force | Out-Null
Set-ItemProperty -Path "$ClientKey\DefaultIcon" -Name "(Default)" -Value "$PickerPath,0"

New-Item -Path "$ClientKey\shell\open\command" -Force | Out-Null
Set-ItemProperty -Path "$ClientKey\shell\open\command" -Name "(Default)" -Value "`"$PickerPath`" `"%1`""

# Capabilities
$CapKey = "$ClientKey\Capabilities"
New-Item -Path $CapKey -Force | Out-Null
Set-ItemProperty -Path $CapKey -Name "ApplicationDescription" -Value "Select browser or Chrome profile for OAuth and external links."
Set-ItemProperty -Path $CapKey -Name "ApplicationIcon" -Value "$PickerPath,0"
Set-ItemProperty -Path $CapKey -Name "ApplicationName" -Value $AppTitle

New-Item -Path "$CapKey\StartMenu" -Force | Out-Null
Set-ItemProperty -Path "$CapKey\StartMenu" -Name "StartMenuInternet" -Value $AppName

New-Item -Path "$CapKey\FileAssociations" -Force | Out-Null
Set-ItemProperty -Path "$CapKey\FileAssociations" -Name ".htm" -Value $HtmlProgId
Set-ItemProperty -Path "$CapKey\FileAssociations" -Name ".html" -Value $HtmlProgId
Set-ItemProperty -Path "$CapKey\FileAssociations" -Name ".shtml" -Value $HtmlProgId
Set-ItemProperty -Path "$CapKey\FileAssociations" -Name ".xhtml" -Value $HtmlProgId

New-Item -Path "$CapKey\URLAssociations" -Force | Out-Null
Set-ItemProperty -Path "$CapKey\URLAssociations" -Name "http" -Value $UrlProgId
Set-ItemProperty -Path "$CapKey\URLAssociations" -Name "https" -Value $UrlProgId

# 2. RegisteredApplications
$RegAppsKey = "HKCU:\Software\RegisteredApplications"
if (-not (Test-Path $RegAppsKey)) { New-Item -Path $RegAppsKey -Force | Out-Null }
Set-ItemProperty -Path $RegAppsKey -Name $AppName -Value "Software\Clients\StartMenuInternet\$AppName\Capabilities"

# 3. URL ProgId
$ProgIdKey = "HKCU:\Software\Classes\$UrlProgId"
New-Item -Path $ProgIdKey -Force | Out-Null
Set-ItemProperty -Path $ProgIdKey -Name "(Default)" -Value "Browser Picker URL"
Set-ItemProperty -Path $ProgIdKey -Name "URL Protocol" -Value ""
Set-ItemProperty -Path $ProgIdKey -Name "FriendlyTypeName" -Value "Browser Picker Link"

New-Item -Path "$ProgIdKey\DefaultIcon" -Force | Out-Null
Set-ItemProperty -Path "$ProgIdKey\DefaultIcon" -Name "(Default)" -Value "$PickerPath,0"

New-Item -Path "$ProgIdKey\shell\open\command" -Force | Out-Null
Set-ItemProperty -Path "$ProgIdKey\shell\open\command" -Name "(Default)" -Value "`"$PickerPath`" `"%1`""

# 4. HTML ProgId
$HtmlIdKey = "HKCU:\Software\Classes\$HtmlProgId"
New-Item -Path $HtmlIdKey -Force | Out-Null
Set-ItemProperty -Path $HtmlIdKey -Name "(Default)" -Value "Browser Picker HTML Document"
Set-ItemProperty -Path $HtmlIdKey -Name "FriendlyTypeName" -Value "Browser Picker HTML Document"

New-Item -Path "$HtmlIdKey\DefaultIcon" -Force | Out-Null
Set-ItemProperty -Path "$HtmlIdKey\DefaultIcon" -Name "(Default)" -Value "$PickerPath,0"

New-Item -Path "$HtmlIdKey\shell\open\command" -Force | Out-Null
Set-ItemProperty -Path "$HtmlIdKey\shell\open\command" -Name "(Default)" -Value "`"$PickerPath`" `"%1`""

Write-Host "Registration complete!" -ForegroundColor Green
Write-Host "Opening Windows Default Apps settings so you can set Browser Picker as your default web browser..." -ForegroundColor Yellow
Start-Process "ms-settings:defaultapps"
