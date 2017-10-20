using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRCLI.Utils
{
    struct CoursorPos
    {
        public long Col;
        public long Line;
    }

    internal class TextBuffer
    {
        private const long InitialNumberOfLines = 80;
        private const long InitialNumberOfColumns = 120;
        private const long ResizeColumnDelta = 10;

        private Char[][] _buffer;
        private long _maxLine = 0, _maxCol = 0;
        private CoursorPos _pos;

        public TextBuffer() : this(InitialNumberOfLines, InitialNumberOfColumns)
        {}

        public TextBuffer(long lines, long columns)
        {
            _maxCol = columns;
            _maxLine = lines;

            _buffer = new Char[_maxLine][];
            for (int l = 0; l < _maxLine; ++l)
                _buffer[l] = new Char[_maxCol];
        }

        private long ResizeLine(long line, long delta = ResizeColumnDelta)
        {
            long c;
            Char[] tmpLine = new Char[_buffer[line].LongLength + delta];
            for (c = 0; c < tmpLine.LongLength && c < _buffer[line].LongLength && _buffer[line][c] != Char.MinValue; ++c)
                tmpLine[c] = _buffer[line][c];
            _buffer[_pos.Line] = tmpLine;
            return c;
        }

        public TextBuffer Add(Char ch)
        {
            if(_buffer.LongLength <= _pos.Line)
            {
                Char[][] tmp = new Char[_pos.Line+1][];
                _buffer.CopyTo(tmp, 0);
                for (long l = _buffer.LongLength; l < tmp.LongLength; ++l)
                    tmp[l] = new Char[ResizeColumnDelta];
            }

            if (_buffer[_pos.Line].LongLength - 1 <= _pos.Col)
            {
                long lastChar = ResizeLine(_pos.Line, _pos.Col + ResizeColumnDelta - _buffer[_pos.Line].LongLength);
                for (long c = lastChar; c < _pos.Col; ++c)
                    if (_buffer[_pos.Line][c] == Char.MinValue)
                        _buffer[_pos.Line][c] = ' ';
                _buffer[_pos.Line][_pos.Col] = ch;
            }
            else
            {
                long c;
                for (c = _buffer[_pos.Line].LongLength - 1; c > _pos.Col && _buffer[_pos.Line][c] == Char.MinValue; --c)
                    ;
                if (c == _buffer[_pos.Line].LongLength - 1)
                    ResizeLine(_pos.Line, ResizeColumnDelta);

                for (; c > _pos.Col; --c)
                {
                    _buffer[_pos.Line][c + 1] = _buffer[_pos.Line][c];
                }
                _buffer[_pos.Line][_pos.Col] = ch;
                for (c = _pos.Col - 1; c >= 0; --c)
                    if (_buffer[_pos.Line][c] == Char.MinValue)
                        _buffer[_pos.Line][c] = ' ';
            }
            ++_pos.Col;

            return this;
        }

        public TextBuffer Backspace()
        {
            if (_buffer.LongLength <= _pos.Line)
            {
                if (_pos.Col > 0)
                    --_pos.Col;
                else if (_buffer.LongLength == _pos.Line)
                {
                    _pos.Col = _buffer[--_pos.Line].LongLength;
                }
                else
                    --_pos.Line;
                
            }
            else if (_buffer[_pos.Line].LongLength <= _pos.Col)
            {
                if(_buffer[_pos.Line].LongLength == _pos.Col)
                {
                    _buffer[_pos.Line][_pos.Col - 1] = Char.MinValue;
                }
                --_pos.Col;
            }
            else
            {
                if (_pos.Col == 0)
                {
                    if (_pos.Line == 0)
                        return this;

                    Char[] tmpLine = new Char[_buffer[_pos.Line - 1].LongLength + _buffer[_pos.Line].LongLength];
                    _buffer[_pos.Line - 1].CopyTo(tmpLine, 0L);
                    _buffer[_pos.Line].CopyTo(tmpLine, _buffer[_pos.Line - 1].LongLength);
                    _buffer[_pos.Line - 1] = tmpLine;
                    for (long l = _pos.Line; l < _buffer.LongLength - 1; ++l)
                        _buffer[l] = _buffer[l + 1];
                    _buffer[_buffer.LongLength - 1] = new Char[ResizeColumnDelta];
                }
                else
                {
                    for (long c = _pos.Col - 1; c <= _buffer[_pos.Line].LongLength - 2; ++c)
                    {
                        _buffer[_pos.Line][c] = _buffer[_pos.Line][c + 1];
                    }
                    _buffer[_pos.Line][_buffer[_pos.Line].LongLength - 1] = Char.MinValue;
                    --_pos.Col;
                }
            }
            return this;
        }

        public TextBuffer Del()
        {
            if (_buffer.LongLength <= _pos.Line)
            {
                //there is nothing
                return this;
            }
            if (_buffer[_pos.Line].LongLength <= _pos.Col)
            {
                if (_buffer.LongLength - 1 == _pos.Line)
                    return this;
                
                Char[] tmpLine = new Char[_pos.Col + _buffer[_pos.Line+1].LongLength];
                for (long c = 0; c < _buffer[_pos.Line].LongLength; ++c)
                    tmpLine[c] = _buffer[_pos.Line][c];
                for (long c = _buffer[_pos.Line].LongLength; c < _pos.Col; ++c)
                    tmpLine[c] = ' ';
                for (long c = _pos.Col; c < tmpLine.LongLength; ++c)
                    tmpLine[c] = tmpLine[c- _pos.Col];
                _buffer[_pos.Line] = tmpLine;
                for (long l = _pos.Line + 1; l < _buffer.LongLength - 1; ++l)
                    _buffer[l] = _buffer[l + 1];
                _buffer[_buffer.LongLength - 1] = new Char[ResizeColumnDelta];
            }
            else
            {
                for (long c = _pos.Col; c <= _buffer[_pos.Line].LongLength - 2; ++c)
                {
                    _buffer[_pos.Line][c] = _buffer[_pos.Line][c + 1];
                }
                _buffer[_pos.Line][_buffer[_pos.Line].LongLength - 1] = Char.MinValue;
            }
            return this;
        }

        public TextBuffer MoveCoursor(long x, long y)
        {
            _pos.Col += x;
            _pos.Line += y;
            if (_pos.Col < 0)
                _pos.Col = 0;
            if (_pos.Line < 0)
                _pos.Line = 0;
            return this;
        }
        
        public List<string> RenderString(long x, long y, long width, long height, bool markCoursor)
        {
            List<string> lines = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (long l = y; l < _buffer.LongLength && l < y + height; ++l)
            {
                for (long c = x; c < _buffer[l].LongLength && c < x + width && _buffer[l][c] != Char.MinValue; ++c)
                {
                    if (l == _pos.Line && c == _pos.Col)
                    {
                        sb.Append("\x1B[4m");
                        sb.Append(_buffer[l][c]);
                        sb.Append("\x1B[0m");
                    }
                    else
                        sb.Append(_buffer[l][c]);
                }
                lines.Add(sb.ToString());
                sb.Clear();
            }
            return lines;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in _buffer)
                sb.Append(line);
            return sb.ToString();
        }
    }
}
