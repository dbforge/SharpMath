using System.Collections.Generic;

namespace SharpMath.Analysis
{
    public struct Domain
    {
        public IEnumerable<double> Exceptions { get; set; }
        public Signs SignAreas { get; set; }
    }
}
