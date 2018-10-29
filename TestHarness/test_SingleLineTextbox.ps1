cd $PSScriptRoot
Import-Module ..\CLRCLI\bin\Debug\netstandard2.0\CLRCLI.dll

$orgFore = $Host.UI.RawUI.ForegroundColor
$orgBack = $Host.UI.RawUI.BackgroundColor

$Root = [CLRCLI.Widgets.RootWindow]::new()
$Dialog = [CLRCLI.Widgets.Dialog]::new($Root)

$Dialog.Text = "SingleLineTextbox Test"
$Dialog.Width = 60
$Dialog.Height = 32
$Dialog.Top = 4
$Dialog.Left = 4
$Dialog.Border = [CLRCLI.BorderStyle]::Thick
$Dialog2.Visible = $true

$Label = [CLRCLI.Widgets.Label]::new($Dialog)
$Label.Text = "Current Text:"
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
