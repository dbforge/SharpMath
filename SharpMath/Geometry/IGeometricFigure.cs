// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace SharpMath.Geometry
{
    public interface IGeometricFigure<out T> where T : Point
    {
        double Perimeter { get; }
        double Area { get; }
        T Center { get; }
    }
}