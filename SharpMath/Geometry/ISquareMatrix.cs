// Author: Dominic Beger (Trade/ProgTrade) 2016

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
        bool IsSingular { get; }
        bool IsTriangle { get; }
    }

    public interface ISquareMatrix<out T> : ISquareMatrix where T : ISquareMatrix
    {
        T Adjugate { get; }
        T CofactorMatrix { get; }
        T Inverse { get; }
        T Negate { get; }
        T Transpose { get; }
    }
}