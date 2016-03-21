// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Collections;
using System.Collections.Generic;

namespace SharpMath.Geometry
{
    public class PointEnumerator : IEnumerator<double>
    {
        private readonly Point _point;
        private int _index = -1;

        internal PointEnumerator(Point point)
        {
            _point = point;
        }

        public double Current => _point[(uint) _index];
        object IEnumerator.Current => _point[(uint) _index];

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _point.Dimension;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}