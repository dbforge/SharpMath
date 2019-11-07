// MatrixEnumerator.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System.Collections;
using System.Collections.Generic;

namespace SharpMath.Geometry
{
    internal class MatrixEnumerator : IEnumerator<double>
    {
        private readonly IMatrix _matrix;
        private int _index;

        internal MatrixEnumerator(IMatrix m)
        {
            _matrix = m;
            _index = -1;
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            _index++;
            return _index < _matrix.ColumnCount + _matrix.RowCount;
        }

        public void Reset()
        {
            _index = -1;
        }

        public double Current => _matrix[(uint) _index % _matrix.ColumnCount, (uint) _index / _matrix.RowCount];
    }
}