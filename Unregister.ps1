# Unregister BrowserPicker from current user registry
$ErrorActionPreference = "SilentlyContinue"

$AppName = "BrowserPicker"
$UrlProgId = "BrowserPickerURL"
$HtmlProgId = "BrowserPickerHTML"

Write-Host "Unregistering $AppName..." -ForegroundColor Cyan

Remove-Item -Path "HKCU:\Software\Clients\StartMenuInternet\$AppName" -Recurse -Force
Remove-ItemProperty -Path "HKCU:\Software\RegisteredApplications" -Name $AppName -Force
Remove-Item -Path "HKCU:\Software\Classes\$UrlProgId" -Recurse -Force
Remove-Item -Path "HKCU:\Software\Classes\$HtmlProgId" -Recurse -Force

Write-Host "Browser Picker has been unregistered." -ForegroundColor Green
Write-Host "Opening Windows Default Apps settings so you can re-select Firefox..." -ForegroundColor Yellow
Start-Process "ms-settings:defaultapps"
