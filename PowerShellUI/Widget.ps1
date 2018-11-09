Enum BorderStyle
{
    Block
    Thick
    Thin
    None
}


class Widget
{
    # Properties
    [String] $Id
    [int] $Top
    [int] $Left
    [int] $Width
    [int] $Height
    [ConsoleColor] $Background = [ConsoleColor]::Gray
    [ConsoleColor] $Foreground = [ConsoleColor]::White
    [ConsoleColor] $SelectedBackground = [ConsoleColor]::Magenta
    [ConsoleColor] $ActiveBackground = [ConsoleColor]::DarkMagenta
    [ConsoleColor] $ActiveForeground = [ConsoleColor]::Black
    [int] $TabStop
    [BorderStyle] $Border
    [bool] $Enabled
    [bool] $DrawShadow
    [Widget] $Parent
    [scriptblock[]] $GotFocus = @()
    [scriptblock[]] $LostFocus = @()
    [scriptblock[]] $TextChanged = @()

    # Static Properties
    # static [String] $fred = "zzz"

    # Hidden Properties
    hidden [bool] $hasFocus
    hidden [string] $text = ""

    # Parameterless Constructor
    CyberNinja ()
    {
    }

    # Constructor
    CyberNinja ([String] $Alias, [int32] $HitPoints)
    {
        $this.Alias = $Alias
        $this.HitPoints = $HitPoints
    }


    # Method
    [bool] getHasFocus()
    {
        return $this.hasFocus
    }

    hidden [void] setHasFocus([bool]$Value)
    {
        if ($Value -ne $this.hasFocus)
        {
            $this.hasFocus = $Value
            if ($Value)
            {
                if ($this.GotFocus) { $this.GotFocus | ForEach-Object {Invoke-Command -ScriptBlock $_ -ArgumentList $this, $null } }
            }
            else
            {
                if ($this.LostFocus) { $this.LostFocus | ForEach-Object {Invoke-Command -ScriptBlock $_ -ArgumentList $this, $null }}
            }
        }
    }

    [string] getText()
    {
        return $this.text
    }

    [void] setText([string]$Value)
    {
        if ($Value -ne $this.text)
        {
            [bool] $parentRedraw = $false
            if ($Value.Length -lt $this.text.Length)
            {
                $parentRedraw = $true
            }
            $this.text = $Value;

            if ($this.TextChanged) { $this.TextChanged | ForEach-Object {Invoke-Command -ScriptBlock $_ -ArgumentList $this, $null }}

            if ($null -ne $this.Parent)
            {
                if ($parentRedraw) { $this.Parent.Draw() }
                else { Draw }
            }
        }        
    }

    [void] Draw()
    {
        if ($this.RootWindow.AllowDraw -eq $false) { return }

        if ($this.Visible -and ($null -eq $this.Parent -or $this.Parent.Visible))
        {
            #     #lock (Console.Out)
            #     #{
            Render
            #     #}

            if ($this.Children -ne $null)
            {
                #$this.Children.Where($c => $c.Visible).ToList().ForEach($c => $c.Draw())
            }
        }
    }

    [void] FireTextChanged()
    {
        if ($this.TextChanged) { $this.TextChanged | ForEach-Object {Invoke-Command -ScriptBlock $_ -ArgumentList $this, $null }}
    }    

    [String] getAlias()
    {
        return $this.Alias
    }

    # Static Method
    # static [String] getClan()
    # {
    #     return [CyberNinja]::Clan
    # }

    # ToString Method
    [String] ToString()
    {
        return "" #$this.Alias + ":" + $this.HitPoints
    }
}