using System.Collections.Generic;

namespace SharpMath.Geometry
{
    internal class MatrixEnumerator : IEnumerator<double>
    {
        IMatrix matrix;
        int index;

        internal MatrixEnumerator(IMatrix m)
        {
            matrix = m;
            index = -1;
        }

        public double Current => matrix[(uint)index % matrix.ColumnCount, (uint)index / matrix.RowCount];

        public void Dispose() { }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            index++;
            return index < matrix.ColumnCount + matrix.RowCount;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}