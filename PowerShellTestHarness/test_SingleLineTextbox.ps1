cd $PSScriptRoot
$destinationDll = "..\CLRCLI\bin\Debug\netstandard2.0\CLRCLI.$(Get-Date -Format 'yyyyMMddhhmmss').dll"
Copy-Item "..\CLRCLI\bin\Debug\netstandard2.0\CLRCLI.dll" $destinationDll
Import-Module $destinationDll

$orgFore = $Host.UI.RawUI.ForegroundColor
$orgBack = $Host.UI.RawUI.BackgroundColor

$Root = [CLRCLI.Widgets.RootWindow]::new()
$Dialog = [CLRCLI.Widgets.Dialog]::new($Root)

$Dialog.Text = "SingleLineTextbox Test $PID"
$Dialog.Width = 60
$Dialog.Height = 32
$Dialog.Top = 4
$Dialog.Left = 4
$Dialog.Border = [CLRCLI.BorderStyle]::Thick
$Dialog2.Visible = $true

$Label = [CLRCLI.Widgets.Label]::new($Dialog)
$Label.Text = "SingleLineTextbox"
$Label.Top = 2
$Label.Left = 2
    
$SingleLineTextbox = [CLRCLI.Widgets.SingleLineTextbox]::new($Dialog)
$SingleLineTextbox.Text = "Default Text"
$SingleLineTextbox.Top = 5
$SingleLineTextbox.Left = 2
    
$SingleLineTextbox.Add_TextChanged( { $Label.Text = $SingleLineTextbox.Text })

$Root.Run()

$Host.UI.RawUI.ForegroundColor = $orgFore
$Host.UI.RawUI.BackgroundColor = $orgBack
