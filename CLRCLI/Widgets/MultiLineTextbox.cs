using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class MultiLineTextbox : Widget, IFocusable, IAcceptInput
    {
        [XmlAttribute]
        [DefaultValue("")]
        public String PasswordChar { get; set; }

        private Timer ToggleCursorTimer = new Timer(500);

        private Utils.TextBuffer _buffer = new Utils.TextBuffer();

        internal MultiLineTextbox()
        {
            ToggleCursorTimer.Elapsed += ToggleCursorTimer_Elapsed;
            ToggleCursorTimer.Start();
        }

        public MultiLineTextbox(Widget parent) : base(parent)
        {
            Background = ConsoleColor.DarkGray;
            Foreground = ConsoleColor.White;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
            ToggleCursorTimer.Elapsed += ToggleCursorTimer_Elapsed;
            ToggleCursorTimer.Start();
        }

        void ToggleCursorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CursorVisible = !CursorVisible;
            Draw();
        }

        private bool CursorVisible = false;
        private char CursorChar = '_';

        internal override void Render()
        {
            var drawBG = (HasFocus) ? ActiveBackground : Background;
            var drawText = _buffer.RenderString(0, 0, this.Width, this.Height, HasFocus);
            for (int i = 0; i < drawText.Count; ++i)
            {
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop+i, Foreground, drawBG, drawText[i]);
            }
        }

        public bool Keypress(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                //Keys we don't specifically want to handle, just return true.
                case ConsoleKey.UpArrow:
                    _buffer.MoveCoursor(0, -1);
                    break;
                case ConsoleKey.DownArrow:
                    _buffer.MoveCoursor(0, 1);
                    break;
                case ConsoleKey.Enter:
                    _buffer.Add('\n');
                    break;
                case ConsoleKey.Escape:
                    return true;
                case ConsoleKey.Backspace:
                    _buffer.Backspace();
                    break;
                case ConsoleKey.RightArrow:
                    _buffer.MoveCoursor(1, 0);
                    break;
                case ConsoleKey.LeftArrow:
                    _buffer.MoveCoursor(-1, 0);
                    break;
                case ConsoleKey.Delete:
                    _buffer.Del();
                    break;
                case ConsoleKey.Home:
                    _buffer.Home();
                    break;
                case ConsoleKey.End:
                    _buffer.End();
                    break;
                default:
                    if (!Char.IsControl(key.KeyChar))
                    {
                        _buffer.Add(key.KeyChar);
                    }
                    else
                    {
                        return true;
                    }
                    break;
            }
            Draw();
            return false;
        }

    }
}
