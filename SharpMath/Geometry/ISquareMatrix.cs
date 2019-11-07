// ISquareMatrix.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

namespace SharpMath.Geometry
{
    public interface ISquareMatrix : IMatrix
    {
        double Determinant { get; }
        uint Dimension { get; }
        bool IsAntiSymmetric { get; }
        bool IsDiagonal { get; }
        bool IsIdentity { get; }
        bool IsOrthogonal { get; }
        bool IsSingular { get; }
        bool IsSkewSymmetric { get; }
        bool IsSymmetric { get; }
        bool IsTriangle { get; }
        double Trace { get; }
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