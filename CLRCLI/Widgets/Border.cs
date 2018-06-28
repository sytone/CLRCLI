﻿using System;

namespace CLRCLI.Widgets
{
    public class Border : Widget
    {
        internal Border()
        {
        }

        public Border(Widget parent) : base(parent)
        {
            Top = 0;
            Left = 0;
            Width = 25;
            Height = 10;
            Background = ConsoleColor.DarkBlue;
            Foreground = ConsoleColor.White;
        }

        internal override void Render()
        {
            DrawBackground();
            BorderDrawMethod(DisplayLeft, DisplayTop, Width, Height, Foreground);
            DrawLabel();
        }

        private void DrawLabel()
        {
            ConsoleHelper.DrawText(DisplayLeft + 2, DisplayTop, Background, Foreground, " {0} ", Text);
        }
    }
}