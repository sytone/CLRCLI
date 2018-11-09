. $PSScriptRoot\Widget.ps1
. $PSScriptRoot\RootWindow.ps1

[Widget] $w = [Widget]::new()

#$w.GotFocus += {param($obj, $event) Write-Host "fred"}

$w.setHasFocus($True)