namespace SharpMath.Geometry
{
    public interface IGeometricFigure<out T> where T : Vector
    {
        double Perimeter { get; }
        double Area { get; }
        T Center { get; }
    }
}
