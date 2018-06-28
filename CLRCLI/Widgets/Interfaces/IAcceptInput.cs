using System;

namespace CLRCLI.Widgets
{
    internal interface IAcceptInput
    {
        bool Keypress(ConsoleKeyInfo key);
    }
}