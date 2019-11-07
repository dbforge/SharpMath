// VectorUtils.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;
using System.Diagnostics;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Defines static methods and extensions for working with <see cref="IVector" /> instances.
    /// </summary>
    public static class VectorUtils
    {
        /// <summary>
        ///     Adds two <see cref="IVector" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="IVector" />.</param>
        /// <param name="secondVector">The second <see cref="IVector" />.</param>
        /// <returns>The resulting <see cref="IVector" />.</returns>
        public static T Add<T>(this T firstVector, T secondVector) where T : IVector, new()
        {
            var resultVector = new T();
            for (uint i = 0; i < resultVector.Dimension; ++i)
                resultVector[i] = firstVector[i] + secondVector[i];
            return resultVector;
        }

        /// <summary>
        ///     Calculates the angle between two <see cref="IVector" /> instances.
        /// </summary>
        /// <param name="first">The first <see cref="IVector" />.</param>
        /// <param name="second">The second <see cref="IVector" />.</param>
        /// <returns>The angle between the current and the specified <see cref="IVector" /> instance.</returns>
        /// <exception cref="InvalidOperationException">Cannot calculate the angle between two vectors, if one is zero.</exception>
        public static double Angle<T>(this T first, T second) where T : IVector
        {
            if (first.IsZero || second.IsZero)
                throw new InvalidOperationException("Cannot calculate the angle between two vectors, if one is zero.");
            return Math.Acos(first.DotProduct(second) / (first.Magnitude * second.Magnitude));
        }

        /// <summary>
        ///     Represents an <see cref="IVector" /> as a horizontal <see cref="IMatrix" /> whose column count is equal to its
        ///     dimension.
        /// </summary>
        /// <returns>The <see cref="IVector" /> represented as horizontal <see cref="IMatrix" />.</returns>
        public static TOut AsHorizontalMatrix<TOut>(this IVector vector) where TOut : IMatrix, new()
        {
            var matrix = new TOut();
            if (matrix.RowCount != 1 && matrix.ColumnCount != vector.Dimension)
                throw new ArgumentException(
                    $"Type parameter TOut is not an adequate IMatrix-type as its constraints do not fit those of the resulting matrix. The constraints must be 1x{vector.Dimension}.");

            for (uint i = 0; i < vector.Dimension; ++i)
                matrix[0, i] = vector[i];
            return matrix;
        }

        /// <summary>
        ///     Represents an <see cref="IVector" /> as a vertical <see cref="IMatrix" /> whose row count is equal to its
        ///     dimension.
        /// </summary>
        /// <returns>The <see cref="IVector" /> represented as vertical <see cref="IMatrix" />.</returns>
        public static TOut AsVerticalMatrix<TOut>(this IVector vector) where TOut : IMatrix, new()
        {
            var matrix = new TOut();
            if (matrix.RowCount != vector.Dimension && matrix.ColumnCount != 1)
                throw new ArgumentException(
                    $"Type parameter TOut is not an adequate IMatrix-type as its constraints do not fit those of the resulting matrix. The constraints must be {vector.Dimension}x1.");

            for (uint i = 0; i < vector.Dimension; ++i)
                matrix[i, 0] = vector[i];
            return matrix;
        }

        /// <summary>
        ///     Determines whether two <see cref="IVector" /> instances are orthogonal to each other, or not.
        /// </summary>
        /// <param name="first">The first <see cref="IVector" />.</param>
        /// <param name="second">The second <see cref="IVector" />.</param>
        /// <returns><c>true</c>, if the <see cref="IVector" /> instances are orthogonal to each other, otherwise <c>false</c>.</returns>
        public static bool CheckForOrthogonality<T>(this T first, T second) where T : IVector
        {
            return !first.IsZero && !second.IsZero &&
                   FloatingNumber.AreApproximatelyEqual(DotProduct(first, second), 0);
        }

        /// <summary>
        ///     Determines whether two <see cref="IVector" /> instances are orthonormal to each other, or not.
        /// </summary>
        /// <param name="first">The first <see cref="IVector" />.</param>
        /// <param name="second">The second <see cref="IVector" />.</param>
        /// <returns><c>true</c>, if the <see cref="IVector" /> instances are orthonormal to each other, otherwise <c>false</c>.</returns>
        public static bool CheckForOrthonormality<T>(this T first, T second) where T : IVector
        {
            return first.CheckForOrthogonality(second) && first.IsNormalized && second.IsNormalized;
        }

        /// <summary>
        ///     Determines whether two <see cref="IVector" /> instances are parallel to each other, or not.
        /// </summary>
        /// <param name="first">The first <see cref="IVector" />.</param>
        /// <param name="second">The second <see cref="IVector" />.</param>
        /// <returns><c>true</c> if the <see cref="IVector" /> instances are parallel to each other, otherwise <c>false</c>.</returns>
        public static bool CheckForParallelism<T>(this T first, T second) where T : IVector
        {
            if (first.IsZero || second.IsZero)
                return false;

            double firstResult = 0;
            for (uint i = 0; i < first.Dimension; ++i)
                if (i == 0)
                {
                    firstResult = second[i] / first[i];
                }
                else
                {
                    if (!FloatingNumber.AreApproximatelyEqual(second[i] / first[i], firstResult))
                        return false;
                }

            return true;
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public static T Clone<T>(this T vector) where T : IVector, new()
        {
            var cloneVector = new T();
            for (uint i = 0; i < vector.Dimension; ++i)
                cloneVector[i] = vector[i];
            return cloneVector;
        }

        /// <summary>
        ///     Converts an <see cref="IVector" /> into an <see cref="IVector" /> of another dimension.
        /// </summary>
        /// <typeparam name="TOut">The <see cref="IVector" /> type that the <see cref="IVector" /> should be converted to.</typeparam>
        /// <returns>The <see cref="IVector" /> converted into the given type.</returns>
        public static TOut Convert<TOut>(this IVector vector) where TOut : IVector, new()
        {
            var resultVector = new TOut();
            if (resultVector.Dimension == vector.Dimension)
                Debug.Print(
                    $"Vector conversion method (Vector{vector.Dimension}.To<T>()) is currently used to convert a vector into one of the same dimension. Please check if this has been your intention.");
            for (uint i = 0; i < Math.Min(vector.Dimension, resultVector.Dimension); ++i)
                resultVector[i] = vector[i];
            return resultVector;
        }

        /// <summary>
        ///     Calculates the distance between two <see cref="IVector" /> instances that are the position <see cref="IVector" />s
        ///     of two points.
        /// </summary>
        /// <param name="source">The source <see cref="IVector" />.</param>
        /// <param name="target">The target <see cref="IVector" />.</param>
        /// <returns>The distance between the two <see cref="IVector" /> instances.</returns>
        public static double Distance<T>(this T source, T target) where T : IVector, new()
        {
            return Subtract(source, target).Magnitude;
        }

        /// <summary>
        ///     Divides a <see cref="IVector" /> by multipling it with the reciprocal of the scalar.
        /// </summary>
        /// <param name="vector">The <see cref="IVector" />.</param>
        /// <param name="scalar">The scalar whose reciprocal will be calculated.</param>
        /// <returns>The resulting <see cref="IVector" />.</returns>
        public static T Divide<T>(this T vector, double scalar) where T : IVector, new()
        {
            var resultVector = new T();
            for (uint i = 0; i < resultVector.Dimension; ++i)
                resultVector[i] = vector[i] * (1 / scalar);
            return resultVector;
        }

        /// <summary>
        ///     Calculates the dot product of two <see cref="IVector" /> instances.
        /// </summary>
        /// <param name="first">The first <see cref="IVector" />.</param>
        /// <param name="second">The second <see cref="IVector" />.</param>
        /// <returns>The calculated scalar as a <see cref="double" />.</returns>
        public static double DotProduct<T>(this T first, T second) where T : IVector
        {
            double result = 0;
            for (uint i = 0; i < first.Dimension; ++i)
                result += first[i] * second[i];
            return result;
        }

        /// <summary>
        ///     Linearly interpolates between two <see cref="IVector" /> instances.
        /// </summary>
        /// <param name="current">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="interpolant">The fraction.</param>
        /// <returns>The position <see cref="IVector" /> of the new point.</returns>
        public static T Lerp<T>(this T current, T target, double interpolant) where T : IVector, new()
        {
            if (interpolant > 1)
                interpolant = 1;
            else if (interpolant < 0)
                interpolant = 0;
            return LerpUnclamped(current, target, interpolant);
        }

        /// <summary>
        ///     Linearly interpolates between two <see cref="IVector" /> instances.
        /// </summary>
        /// <param name="current">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="interpolant">The interpolant factor.</param>
        /// <returns>The position <see cref="IVector" /> of the new point.</returns>
        public static T LerpUnclamped<T>(this T current, T target, double interpolant) where T : IVector, new()
        {
            // source + (target - source) * fraction
            return Add(current, Multiply(Subtract(target, current), interpolant));
        }

        /// <summary>
        ///     Moves the source point in a straight line towards a target point by adding the given distance delta and returns
        ///     its new position.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="maxDistanceDelta">The distance delta that this source point is moved by in all directions.</param>
        /// <returns>The position <see cref="IVector" /> of the new point.</returns>
        public static T MoveTowards<T>(this T source, T target, double maxDistanceDelta) where T : IVector, new()
        {
            return source.LerpUnclamped(target, maxDistanceDelta / source.Distance(target));
        }

        /// <summary>
        ///     Multiplies a <see cref="IVector" /> with a specified scalar.
        /// </summary>
        /// <param name="vector">The <see cref="IVector" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The resulting <see cref="IVector" />.</returns>
        public static T Multiply<T>(this T vector, double scalar) where T : IVector, new()
        {
            var resultVector = new T();
            for (uint i = 0; i < resultVector.Dimension; ++i)
                resultVector[i] = vector[i] * scalar;
            return resultVector;
        }

        /// <summary>
        ///     Negates the specified <see cref="IVector" />.
        /// </summary>
        /// <returns>The negated <see cref="IVector" />.</returns>
        public static T Negate<T>(this T vector) where T : IVector, new()
        {
            var resultVector = new T();
            for (uint i = 0; i < vector.Dimension; ++i)
                resultVector[i] = -vector[i];
            return resultVector;
        }

        /// <summary>
        ///     Calculates the normalized <see cref="IVector" /> of the <see cref="IVector" />.
        /// </summary>
        /// <returns>The normalized <see cref="IVector" />.</returns>
        public static T Normalize<T>(this T vector) where T : IVector, new()
        {
            var resultVector = new T();
            for (uint i = 0; i < vector.Dimension; ++i)
                resultVector[i] = vector[i] / vector.Magnitude;
            return resultVector;
        }

        /// <summary>
        ///     Subtracts two <see cref="IVector" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="IVector" />.</param>
        /// <param name="secondVector">The second <see cref="IVector" />.</param>
        /// <returns>The resulting <see cref="IVector" />.</returns>
        public static T Subtract<T>(this T firstVector, T secondVector) where T : IVector, new()
        {
            var resultVector = new T();
            for (uint i = 0; i < resultVector.Dimension; ++i)
                resultVector[i] = firstVector[i] - secondVector[i];
            return resultVector;
        }
    }
}