namespace SharpMath.Geometry
{
    public interface ISquareMatrix : IMatrix
    {
        uint Dimension { get; }
        double Determinant { get; }
        double Trace { get; }
        bool IsIdentity { get; }
        bool IsOrthogonal { get; }
        bool IsSymmetric { get; }
        bool IsSkewSymmetric { get; }
        bool IsAntiSymmetric { get; }
        bool IsDiagonal { get; }
        bool IsTriangle { get; }
    }
}