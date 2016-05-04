// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpMath.Geometry.Exceptions;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a two-dimensional vector.
    /// </summary>
    [Serializable]
    public struct Vector2 : IVector, IEquatable<Vector2>, IEnumerable<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2" /> class.
        /// </summary>
        /// <param name="vector">The existing <see cref="Vector2" /> to copy.</param>
        public Vector2(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2" /> class.
        /// </summary>
        /// <param name="x">The value of the X-coordinate (X1 in mathematic coordinate systems).</param>
        /// <param name="y">The value of the Y-coordinate (X2 in mathematic coordinate systems).</param>
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2" /> class.
        /// </summary>
        /// <param name="point">The <see cref="Point2D" /> that a position <see cref="Vector2" /> should be created for.</param>
        public Vector2(Point2D point)
            : this(point.PositionVector)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2" /> class.
        /// </summary>
        /// <param name="bottom">The tail of the <see cref="Vector2" />.</param>
        /// <param name="tip">The head of the <see cref="Vector2" />.</param>
        public Vector2(Point2D bottom, Point2D tip)
            : this((tip - bottom).PositionVector)
        {
        }

        /// <summary>
        ///     Gets or sets the value of the X-coordinate (X1 in mathematic coordinate systems).
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     Gets or sets the value of the Y-coordinate (X2 in mathematic coordinate systems).
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        ///     A <see cref="Vector2" /> with all values set to zero.
        /// </summary>
        public static Vector2 Zero => new Vector2(0, 0);

        /// <summary>
        ///     A <see cref="Vector2" /> with all values set to one.
        /// </summary>
        public static Vector2 One => new Vector2(1, 1);

        /// <summary>
        ///     A unit <see cref="Vector2" /> pointing up.
        /// </summary>
        public static Vector2 Up => new Vector2(0, 1);

        /// <summary>
        ///     A unit <see cref="Vector2" /> pointing down.
        /// </summary>
        public static Vector2 Down => new Vector2(0, -1);

        /// <summary>
        ///     A unit <see cref="Vector2" /> pointing to the left.
        /// </summary>
        public static Vector2 Left => new Vector2(-1, 0);

        /// <summary>
        ///     A unit <see cref="Vector2" /> pointing to the right.
        /// </summary>
        public static Vector2 Right => new Vector2(1, 0);

        /// <summary>
        ///     A unit <see cref="Vector2" />  with coordinates 1, 0.
        /// </summary>
        public static Vector2 UnitX => new Vector2(1, 0);

        /// <summary>
        ///     A unit <see cref="Vector2" />  with coordinates 0, 1.
        /// </summary>
        public static Vector2 UnitY => new Vector2(0, 1);

        /// <summary>
        ///     Gets the <see cref="Vector2" /> that is perpendicular to the <see cref="Vector2" />.
        /// </summary>
        public Vector2 Perpendicular => new Vector2(-Y, X);

        /// <summary>
        ///     Gets or sets the value of the coordinate at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value of the coordinate at the specified index.</returns>
        public double this[uint index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 1.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 1.");
                }
            }
        }

        /// <summary>
        ///     Gets the dimension of the <see cref="Vector2" />.
        /// </summary>
        public uint Dimension => 2;

        /// <summary>
        ///     Gets the length of the <see cref="Vector2" />.
        /// </summary>
        public double Magnitude => Math.Sqrt(SquareMagnitude);

        /// <summary>
        ///     Gets the squared length of the <see cref="Vector2" />.
        /// </summary>
        public double SquareMagnitude
        {
            get
            {
                double result = 0;
                for (uint i = 0; i < 2; ++i)
                    result += Math.Pow(this[i], 2);
                return result;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Vector2" /> is normalized, or not.
        /// </summary>
        public bool IsNormalized => Magnitude.IsApproximatelyEqualTo(1);

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Vector2" /> has all of its components set to zero, or not.
        /// </summary>
        public bool IsZero => this.All(c => FloatingNumber.AreApproximatelyEqual(c, 0));

        /// <summary>
        ///     Gets the LaTeX-string representing this vector graphically.
        /// </summary>
        public string ToLaTeXString()
            => @"\left( \begin{array}{c} " + this[0] + @" \\ " + this[1] + @" \end{array} \right)";

        /// <summary>
        ///     Generates a <see cref="Vector2" /> from an object that implements the <see cref="IVector" /> interface, if the
        ///     dimension is correct.
        /// </summary>
        /// <param name="vector">The <see cref="IVector" /> to generate a <see cref="Vector2" /> from.</param>
        /// <returns>The generated <see cref="Vector2" />.</returns>
        /// <exception cref="Dimension">The dimension of the given vector is invalid. It must be 2.</exception>
        public static Vector2 FromVector(IVector vector)
        {
            if (vector.Dimension != 2)
                throw new DimensionException("The dimension of the given vector is invalid. It must be 2.");
            return new Vector2(vector[0], vector[1]);
        }

        /// <summary>
        ///     Calculates the angle between two <see cref="Vector2" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns>The angle between the <see cref="Vector2" /> instances.</returns>
        public static double Angle(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.Angle(secondVector);
        }

        /// <summary>
        ///     Calculates the area of the parallelogram that this and the specified <see cref="Vector2" /> instances span.
        /// </summary>
        /// <param name="other">The other <see cref="Vector2" />.</param>
        /// <returns>The area of the spanned parallelogram.</returns>
        public double Area(Vector2 other)
        {
            return Math.Abs(VectorProduct(other));
        }

        /// <summary>
        ///     Calculates the area of the parallelogram that the two specified <see cref="Vector2" /> instances span.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns>The area of the spanned parallelogram.</returns>
        public static double Area(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.Area(secondVector);
        }

        /// <summary>
        ///     Determines whether two <see cref="Vector2" /> instances are orthogonal to each other, or not.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns><c>true</c>, if the <see cref="Vector2" /> instances are orthogonal to each other, otherwise <c>false</c>.</returns>
        public static bool AreOrthogonal(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.CheckForOrthogonality(secondVector);
        }

        /// <summary>
        ///     Determines whether two <see cref="Vector2" /> instances are orthonormal to each other, or not.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns><c>true</c> if the <see cref="Vector2" /> instances are orthonormal to each other, otherwise <c>false</c>.</returns>
        public static bool AreOrthonormal(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.CheckForOrthonormality(secondVector);
        }

        /// <summary>
        ///     Determines whether two <see cref="Vector2" /> instances are parallel to each other, or not.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns><c>true</c> if the <see cref="Vector2" /> instances are parallel to each other, otherwise <c>false</c>.</returns>
        public static bool AreParallel(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.CheckForParallelism(secondVector);
        }

        /// <summary>
        ///     Calculates the distance between two points.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <returns>The distance between specified points.</returns>
        public static double Distance(Vector2 source, Vector2 target)
        {
            return source.Distance(target);
        }

        /// <summary>
        ///     Divides a <see cref="Vector2" /> by multipling it with the reciprocal of the scalar.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2" />.</param>
        /// <param name="scalar">The scalar whose reciprocal will be calculated.</param>
        /// <returns>The resulting <see cref="Vector2" />.</returns>
        public static Vector2 Divide(Vector2 vector, double scalar)
        {
            return vector*(1/scalar);
        }

        /// <summary>
        ///     Calculates the dot product of the specified <see cref="Vector2" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" /> that should be included into the calculation.</param>
        /// <param name="secondVector">The second <see cref="Vector2" /> that should be included into the calculation.</param>
        /// <returns>The calculated scalar as a <see cref="double" />.</returns>
        public static double DotProduct(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.DotProduct(secondVector);
        }

        /// <summary>
        ///     Linearly interpolates between two <see cref="Vector2" /> instances.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns>The position <see cref="Vector2" /> of the new point.</returns>
        public static Vector2 Lerp(Vector2 source, Vector2 target, double fraction)
        {
            return source.Lerp(target, fraction);
        }

        /// <summary>
        ///     Linearly interpolates between two <see cref="Vector2" /> instances.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns>The position <see cref="Vector2" /> of the new point.</returns>
        public static Vector2 LerpUnclamped(Vector2 source, Vector2 target, double fraction)
        {
            return source.LerpUnclamped(target, fraction);
        }

        /// <summary>
        ///     Moves a source point in a straight line towards a target point by adding the given distance delta and returns its
        ///     new position.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="maxDistanceDelta">The distance delta that the source point is moved by in all directions.</param>
        /// <returns>The position <see cref="Vector2" /> of the new point.</returns>
        public static Vector2 MoveTowards(Vector2 source, Vector2 target, double maxDistanceDelta)
        {
            return source.MoveTowards(target, maxDistanceDelta);
        }

        /// <summary>
        ///     Calculates the vector product of the current and the specified <see cref="Vector2" /> instance.
        /// </summary>
        /// <param name="other">The other <see cref="Vector2" />.</param>
        /// <returns>The vector product of the current and the specified <see cref="Vector2" />.</returns>
        public double VectorProduct(Vector2 other)
        {
            return (X*other.Y) - (Y*other.X);
        }

        /// <summary>
        ///     Calculates the vector product of the specified <see cref="Vector2" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns>The vector product of the specified <see cref="Vector2" /> instances.</returns>
        public static double VectorProduct(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.VectorProduct(secondVector);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2" />.
        /// </returns>
        public static Vector2 operator +(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.Add(secondVector);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2" />.
        /// </returns>
        public static Vector2 operator -(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.Subtract(secondVector);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="current">The <see cref="Vector2" /> to negate.</param>
        /// <returns>
        ///     The negated <see cref="Vector2" />.
        /// </returns>
        public static Vector2 operator -(Vector2 current)
        {
            return current.Negate();
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2" />.
        /// </returns>
        public static Vector2 operator *(Vector2 vector, double scalar)
        {
            return VectorUtils.Multiply(vector, scalar);
        }

        /// <summary>
        ///     Implements the operator * for calculating the dot product of two <see cref="Vector2" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2" />.</param>
        /// <param name="secondVector">The second <see cref="Vector2" />.</param>
        /// <returns>
        ///     The scalar that has been calculated.
        /// </returns>
        public static double operator *(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.DotProduct(secondVector);
        }

        /// <summary>
        ///     Transforms this <see cref="Vector2" /> with the specified <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="matrix">The transformation <see cref="Matrix3x3" />.</param>
        /// <returns>The transformed <see cref="Vector2" />.</returns>
        public Vector2 Transform(Matrix3x3 matrix)
        {
            return matrix*this;
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2" /> with the specified <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2" /> that should be transformed.</param>
        /// <param name="matrix">The transformation <see cref="Matrix3x3" />.</param>
        /// <returns>The transformed <see cref="Vector2" />.</returns>
        public static Vector2 Transform(Vector2 vector, Matrix3x3 matrix)
        {
            return vector.Transform(matrix);
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2" /> with the specified <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="matrix">The transformation <see cref="Matrix3x3" />.</param>
        /// <param name="vector">The <see cref="Vector2" /> that should be transformed.</param>
        /// <returns>The transformed <see cref="Vector2" />.</returns>
        public static Vector2 Transform(Matrix3x3 matrix, Vector2 vector)
        {
            return vector.Transform(matrix);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Vector2 {{X: {this[0]}, Y: {this[1]}}}";
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to the current instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with the current instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector2)
                return this == (Vector2) obj;
            var vector = obj as IVector;
            if (Dimension != vector?.Dimension)
                return false;
            return this == FromVector(vector);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                // ReSharper disable NonReadonlyMemberInGetHashCode
                hash = hash*23 + X.GetHashCode();
                hash = hash*23 + Y.GetHashCode();
                return hash;
            }
        }

        public bool Equals(Vector2 other)
        {
            return this == other;
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (uint i = 0; i < 2; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (uint i = 0; i < 2; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Vector2" />.</param>
        /// <param name="right">The right <see cref="Vector2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.SequenceEqual(right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Vector2" />.</param>
        /// <param name="right">The right <see cref="Vector2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.SequenceEqual(right);
        }
    }
}