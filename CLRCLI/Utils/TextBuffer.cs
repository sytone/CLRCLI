using System;
using System.Collections.Generic;
using System.Text;

namespace CLRCLI.Utils
{
    internal struct CoursorPos
    {
        public long Col;
        public long Line;
    }

    internal class TextBuffer
    {
        private const long InitialNumberOfLines = 80;
        private const long InitialNumberOfColumns = 120;
        private const long ResizeColumnDelta = 10;

        private BufferLine[] _buffer;
        private long _maxLine = 0, _maxCol = 0;
        private CoursorPos _pos;

        public TextBuffer() : this(InitialNumberOfLines, InitialNumberOfColumns)
        { }

        public TextBuffer(long lines, long columns)
        {
            _maxCol = columns;
            _maxLine = lines;

            _buffer = new BufferLine[_maxLine];
            for (int l = 0; l < _maxLine; ++l)
                _buffer[l] = new BufferLine(_maxCol);
        }

        public TextBuffer NewLine(long newLineSize = ResizeColumnDelta, long howManyLines = 1)
        {
            BufferLine[] tmp = new BufferLine[_buffer.LongLength + howManyLines];
            BufferLine newLine;
            if (_pos.Col < _buffer[_pos.Line].TextLength)
            {
                newLine = new BufferLine(_buffer[_pos.Line].TextLength - _pos.Col + newLineSize);
                newLine.CopyFrom(_buffer[_pos.Line], 0, _pos.Col);
                _buffer[_pos.Line].Erase(_pos.Col);
            }
            else
                newLine = new BufferLine(newLineSize);
            long l, dl;
            for (l = 0; l <= _pos.Line && l < _buffer.LongLength; ++l)
                tmp[l] = _buffer[l];
            for (dl = 0; dl < howManyLines; ++dl)
                tmp[l + dl] = newLine;
            for (; l < _buffer.LongLength; ++l)
                tmp[l + dl] = _buffer[l];

            _buffer = tmp;
            return this;
        }

        public TextBuffer Add(Char ch)
        {
            if (ch == '\n')
            {
                NewLine();
                MoveCoursor(-_pos.Col, 1);
                return this;
            }
            if (_buffer.LongLength <= _pos.Line)
            {
                NewLine(howManyLines: _pos.Line + 1 - _buffer.LongLength);
            }

            if (_buffer[_pos.Line].LongLength - 1 <= _pos.Col)
            {
                _buffer[_pos.Line].Resize(_pos.Col + ResizeColumnDelta - _buffer[_pos.Line].LongLength);
                _buffer[_pos.Line][_pos.Col] = ch;
            }
            else
            {
                long c;
                for (c = _buffer[_pos.Line].LongLength - 1; c >= _pos.Col && _buffer[_pos.Line][c] == Char.MinValue; --c)
                    ;
                if (c == _buffer[_pos.Line].LongLength - 1)
                    _buffer[_pos.Line].Resize(ResizeColumnDelta);

                for (; c >= _pos.Col; --c)
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
                if (_buffer[_pos.Line].LongLength == _pos.Col)
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

                    long endOfPrevLine = _buffer[_pos.Line - 1].TextLength;

                    _buffer[_pos.Line - 1].Merge(_buffer[_pos.Line]);
                    for (long l = _pos.Line; l < _buffer.LongLength - 1; ++l)
                        _buffer[l] = _buffer[l + 1];
                    _buffer[_buffer.LongLength - 1] = new BufferLine(ResizeColumnDelta);

                    _pos.Col = endOfPrevLine;
                    --_pos.Line;
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
            if (_buffer[_pos.Line].TextLength <= _pos.Col)
            {
                if (_buffer.LongLength - 1 == _pos.Line)
                    return this;
                if (_buffer[_pos.Line].LongLength <= _pos.Col)
                    _buffer[_pos.Line].Resize(_pos.Col + ResizeColumnDelta - _buffer[_pos.Line].LongLength);
                _buffer[_pos.Line][_pos.Col - 1] = ' ';
                _buffer[_pos.Line].Merge(_buffer[_pos.Line + 1]);

                for (long l = _pos.Line + 1; l < _buffer.LongLength - 1; ++l)
                    _buffer[l] = _buffer[l + 1];
                _buffer[_buffer.LongLength - 1] = new BufferLine(ResizeColumnDelta);
            }
            else
            {
                for (long c = _pos.Col; c < _buffer[_pos.Line].TextLength - 1; ++c)
                {
                    _buffer[_pos.Line][c] = _buffer[_pos.Line][c + 1];
                }
                _buffer[_pos.Line][_buffer[_pos.Line].TextLength - 1] = Char.MinValue;
            }
            return this;
        }

        public TextBuffer Home()
        {
            _pos.Col = 0;
            return this;
        }

        public TextBuffer End()
        {
            _pos.Col = _buffer[_pos.Line].TextLength;
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
            for (long l = y; l < y + height; ++l)
            {
                if (l > _buffer.LongLength)
                {
                    lines.Add(new string(' ', (int)width));
                    continue;
                }
                for (long c = x; c < x + width; ++c)
                {
                    Char character = c < _buffer[l].LongLength ? _buffer[l][c] : ' ';
                    if (character == Char.MinValue)
                        character = ' ';
                    if (l == _pos.Line && c == _pos.Col)
                    {
                        sb.Append("\x1B[4m");
                        sb.Append(character);
                        sb.Append("\x1B[0m\x1B[45m\x1B[97m");
                    }
                    else
                        sb.Append(character);
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