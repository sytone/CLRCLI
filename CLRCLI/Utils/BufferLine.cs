using System;

namespace CLRCLI.Utils
{
    internal class BufferLine
    {
        private Char[] _bufferLine;
        private long _occupyBajts;

        public BufferLine(long capacity)
        {
            _bufferLine = new Char[capacity];
            _occupyBajts = 0;
        }

        public BufferLine Resize(long delta)
        {
            Char[] tmp = new Char[_bufferLine.LongLength + delta];
            _bufferLine.CopyTo(tmp, 0L);
            _bufferLine = tmp;
            return this;
        }

        public BufferLine Merge(BufferLine source)
        {
            if ((TextLength + source.TextLength + 1) >= LongLength)
                Resize(TextLength + source.LongLength + 1);
            CopyFrom(source, TextLength);

            return this;
        }

        public long LongLength
        {
            get { return _bufferLine.LongLength; }
        }

        public long TextLength
        {
            get { return _occupyBajts; }
        }

        public Char this[long index]
        {
            get { return _bufferLine[index]; }
            set
            {
                _bufferLine[index] = value;
                if (index >= _occupyBajts && value != Char.MinValue)
                {
                    for (long c = _occupyBajts; c < index; ++c)
                        _bufferLine[c] = ' ';
                    _occupyBajts = index + 1;
                }
                else if ((index + 1) == _occupyBajts && value == Char.MinValue)
                    _occupyBajts = index;
            }
        }

        public void Erase(long index)
        {
            for (long c = index; c < TextLength; ++c)
                _bufferLine[c] = char.MinValue;
            _occupyBajts = index;
        }

        public void CopyFrom(BufferLine source, long dstIndex, long srcIndex = 0L)
        {
            if (LongLength <= dstIndex + source.TextLength - srcIndex)
                throw new IndexOutOfRangeException("Copied content is too big for destination!");

            for (long c = 0; c + srcIndex < source.TextLength && c + dstIndex < _bufferLine.LongLength; ++c)
            {
                _bufferLine[dstIndex + c] = source[c + srcIndex];
            }
            _occupyBajts = dstIndex + source.TextLength - srcIndex;
        }

        public void CopyTo(BufferLine destination, long dstIndex, long srcIndex = 0L)
        {
            destination.CopyFrom(this, dstIndex, srcIndex);
        }
    }
}