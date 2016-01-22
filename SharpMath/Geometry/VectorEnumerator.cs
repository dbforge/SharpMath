using System.Collections;
using System.Collections.Generic;

namespace SharpMath.Geometry
{
    public class VectorEnumerator : IEnumerator<double>
    {
        private readonly Vector _vector;
        private int _index;

        internal VectorEnumerator(Vector vector)
        {
            _vector = vector;
        }

        public double Current => _vector[(uint)_index];

        object IEnumerator.Current => _vector[(uint)_index];

        public void Dispose()
        { }

        public bool MoveNext()
        {
            _index++;
            return _index < _vector.Dimension;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}
