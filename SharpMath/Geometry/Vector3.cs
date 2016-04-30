// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a three-dimensional vector.
    /// </summary>
    [Serializable]
    public struct Vector3 : IVector, IEquatable<Vector3>, IEnumerable<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector3" /> class.
        /// </summary>
        /// <param name="vector">The existing <see cref="Vector3" /> to copy.</param>
        public Vector3(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector3" /> class.
        /// </summary>
        /// <param name="x">The value of the X-coordinate (X2 in mathematic coordinate systems).</param>
        /// <param name="y">The value of the Y-coordinate (X3 in mathematic coordinate systems).</param>
        /// <param name="z">The value of the Z-coordinate (X1 in mathematic coordinate systems).</param>
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector3" /> class.
        /// </summary>
        /// <param name="point">The <see cref="Point3D" /> that a position <see cref="Vector3" /> should be created for.</param>
        public Vector3(Point3D point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector3" /> class.
        /// </summary>
        /// <param name="bottom">The tail of the <see cref="Vector3" />.</param>
        /// <param name="tip">The head of the <see cref="Vector3" />.</param>
        public Vector3(Point3D bottom, Point3D tip)
            : this((tip - bottom).PositionVector)
        {
        }

        /// <summary>
        ///     Gets or sets the value of the X-coordinate (X2 in mathematic coordinate systems).
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     Gets or sets the value of the Y-coordinate (X3 in mathematic coordinate systems).
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        ///     Gets or sets the value of the Z-coordinate (X1 in mathematic coordinate systems).
        /// </summary>
        public double Z { get; set; }

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
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default: throw new IndexOutOfRangeException("The index must be between 0 and 2.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new IndexOutOfRangeException("The index must be between 0 and 2.");
                }
            }
        }

        /// <summary>
        ///     A <see cref="Vector3" /> with all values set to zero.
        /// </summary>
        public static Vector3 Zero => new Vector3(0, 0, 0);

        /// <summary>
        ///     A <see cref="Vector3" /> with all values set to one.
        /// </summary>
        public static Vector3 One => new Vector3(1, 1, 1);

        /// <summary>
        ///     A unit <see cref="Vector3" /> pointing up.
        /// </summary>
        public static Vector3 Up => new Vector3(0, 1, 0);

        /// <summary>
        ///     A unit <see cref="Vector3" /> pointing down.
        /// </summary>
        public static Vector3 Down => new Vector3(0, -1, 0);

        /// <summary>
        ///     A unit <see cref="Vector3" /> pointing to the left.
        /// </summary>
        public static Vector3 Left => new Vector3(-1, 0, 0);

        /// <summary>
        ///     A unit <see cref="Vector3" /> pointing to the right.
        /// </summary>
        public static Vector3 Right => new Vector3(1, 0, 0);

        /// <summary>
        ///     A unit <see cref="Vector3" /> pointing forward.
        /// </summary>
        public static Vector3 Forward => new Vector3(0, 0, 1);

        /// <summary>
        ///     A unit <see cref="Vector3" /> pointing backward.
        /// </summary>
        public static Vector3 Back => new Vector3(0, 0, -1);

        /// <summary>
        ///     A unit <see cref="Vector3" />  with coordinates 1, 0, 0.
        /// </summary>
        public static Vector3 UnitX => new Vector3(1, 0, 0);

        /// <summary>
        ///     A unit <see cref="Vector3" />  with coordinates 0, 1, 0.
        /// </summary>
        public static Vector3 UnitY => new Vector3(0, 1, 0);

        /// <summary>
        ///     A unit <see cref="Vector3" />  with coordinates 0, 0, 1.
        /// </summary>
        public static Vector3 UnitZ => new Vector3(0, 0, 1);

        /// <summary>
        ///     Gets the dimension of the <see cref="Vector3" />.
        /// </summary>
        public uint Dimension => 3;

        /// <summary>
        ///     Gets the length of the <see cref="Vector3" />.
        /// </summary>
        public double Magnitude => Math.Sqrt(SquareMagnitude);

        /// <summary>
        ///     Gets the squared length of the <see cref="Vector3" />.
        /// </summary>
        public double SquareMagnitude
        {
            get
            {
                double result = 0;
                for (uint i = 0; i < 3; ++i)
                    result += Math.Pow(this[i], 2);
                return result;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Vector3" /> is normalized, or not.
        /// </summary>
        public bool IsNormalized => Magnitude.IsApproximatelyEqualTo(1);

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Vector3" /> has all of its components set to zero, or not.
        /// </summary>
        public bool IsZero => this.All(c => FloatingNumber.AreApproximatelyEqual(c, 0));

        /// <summary>
        ///     Gets the LaTeX-string representing this vector graphically.
        /// </summary>
        public string ToLaTeXString()
            => @"\left( \begin{array}{c} " + this[0] + @" \\ " + this[1] + @" \\ " + this[2] + @" \end{array} \right)";

        /// <summary>
        ///     Generates a <see cref="Vector3" /> from an object implementing the <see cref="IVector" /> interface, if the dimension is correct.
        /// </summary>
        /// <param name="vector">The <see cref="IVector" /> to generate a <see cref="Vector3" /> from.</param>
        /// <returns>The generated <see cref="Vector3" />.</returns>
        /// <exception cref="ArgumentException">The dimension of the given vector is invalid. It must be 3.</exception>
        public static Vector3 FromVector(IVector vector)
        {
            if (vector.Dimension != 3)
                throw new ArgumentException("The dimension of the given vector is invalid. It must be 3.");
            return new Vector3(vector[0], vector[1], vector[2]);
        }

        /// <summary>
        ///     Calculates the area of the parallelogram that this and the specified <see cref="Vector3" /> instances span.
        /// </summary>
        /// <param name="other">The other <see cref="Vector3" />.</param>
        /// <returns>The area of the spanned parallelogram.</returns>
        public double Area(Vector3 other)
        {
            return VectorProduct(this, other).Magnitude;
        }

        /// <summary>
        ///     Calculates the area of the parallelogram that the two specified <see cref="Vector3" /> instances span.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector3" />.</param>
        /// <param name="secondVector">The second <see cref="Vector3" />.</param>
        /// <returns>The area of the spanned parallelogram.</returns>
        public static double Area(Vector3 firstVector, Vector3 secondVector)
        {
            return firstVector.Area(secondVector);
        }

        /// <summary>
        ///     Linearly interpolates between two <see cref="Vector3" /> instances.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns>The position <see cref="Vector3" /> of the new point.</returns>
        public static Vector3 Lerp(Vector3 source, Vector3 target, double fraction)
        {
            return VectorUtils.Lerp(source, target, fraction);
        }

        /// <summary>
        ///     Linearly interpolates between two <see cref="Vector3" /> instances.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns>The position <see cref="Vector3" /> of the new point.</returns>
        public static Vector3 LerpUnclamped(Vector3 source, Vector3 target, double fraction)
        {
            return VectorUtils.LerpUnclamped(source, target, fraction);
        }

        /// <summary>
        ///     Moves a source point in a straight line towards a target point by adding the given distance delta and returns its
        ///     new position.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="maxDistanceDelta">The distance delta that the source point is moved by in all directions.</param>
        /// <returns>The position <see cref="Vector3" /> of the new point.</returns>
        public static Vector3 MoveTowards(Vector3 source, Vector3 target, double maxDistanceDelta)
        {
            return VectorUtils.MoveTowards(source, target, maxDistanceDelta);
        }

        /// <summary>
        ///     Calculates the volume of the parallelepiped that is being created by the three specified <see cref="Vector3" />
        ///     instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector3" />.</param>
        /// <param name="secondVector">The second <see cref="Vector3" />.</param>
        /// <param name="thirdVector">The third <see cref="Vector3" />.</param>
        /// <returns>Returns the calculated volume.</returns>
        public static double ScalarTripleProduct(Vector3 firstVector, Vector3 secondVector, Vector3 thirdVector)
        {
            //var matrix = SquareMatrix.FromMatrix(firstVector.AsHorizontalMatrix().AugmentVertically(secondVector.AsHorizontalMatrix()).AugmentVertically(thirdVector.AsHorizontalMatrix()));
            //return Math.Abs(matrix.Determinant);

            return Math.Abs(VectorUtils.DotProduct(VectorProduct(firstVector, secondVector), thirdVector));
        }

        /// <summary>
        ///     Calculates the vector product of the current and the specified <see cref="Vector3" />.
        /// </summary>
        /// <param name="other">The other <see cref="Vector3" />.</param>
        /// <returns>The calculated <see cref="Vector3" />.</returns>
        public Vector3 VectorProduct(Vector3 other)
        {
            return new Vector3((Y * other.Z - Z * other.Y), (Z * other.X - X * other.Z), (X * other.Y - Y * other.X));
        }

        /// <summary>
        ///     Calculates the <see cref="Vector3" /> that is perpendicular to the specified <see cref="Vector3" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector3" /> that should be included into the calculation.</param>
        /// <param name="secondVector">The second <see cref="Vector3" /> that should be included into the calculation.</param>
        /// <returns>Returns the calculated <see cref="Vector3" />.</returns>
        public static Vector3 VectorProduct(Vector3 firstVector, Vector3 secondVector)
        {
            return firstVector.VectorProduct(secondVector);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector3" />.</param>
        /// <param name="secondVector">The second <see cref="Vector3" />.</param>
        /// <returns>
        ///     The resulting <see cref="Vector3" />.
        /// </returns>
        public static Vector3 operator +(Vector3 firstVector, Vector3 secondVector)
        {
            return VectorUtils.Add(firstVector, secondVector);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector3" />.</param>
        /// <param name="secondVector">The second <see cref="Vector3" />.</param>
        /// <returns>
        ///     The resulting <see cref="Vector3" />.
        /// </returns>
        public static Vector3 operator -(Vector3 firstVector, Vector3 secondVector)
        {
            return VectorUtils.Subtract(firstVector, secondVector);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="current">The <see cref="Vector3" /> to negate.</param>
        /// <returns>
        ///     The negated <see cref="Vector3" />.
        /// </returns>
        public static Vector3 operator -(Vector3 current)
        {
            return current.Negate();
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The resulting <see cref="Vector3" />.
        /// </returns>
        public static Vector3 operator *(Vector3 vector, double scalar)
        {
            return VectorUtils.Multiply(vector, scalar);
        }

        /// <summary>
        ///     Implements the operator * for calculating the scalar product of two <see cref="Vector3" /> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector3" />.</param>
        /// <param name="secondVector">The second <see cref="Vector3" />.</param>
        /// <returns>
        ///     The scalar that has been calculated.
        /// </returns>
        public static double operator *(Vector3 firstVector, Vector3 secondVector)
        {
            return VectorUtils.DotProduct(firstVector, secondVector);
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector3" /> with the specified <see cref="Matrix4x4" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3" /> that should be transformed.</param>
        /// <param name="matrix">The transformation <see cref="Matrix4x4" />.</param>
        /// <returns>The transformed <see cref="Vector3" />.</returns>
        public static Vector3 Transform(Vector3 vector, Matrix4x4 matrix)
        {
            var result = matrix*new Vector4(vector.X, vector.Y, vector.Z, 1);
            result.X /= result.W;
            result.Y /= result.W;
            result.Z /= result.W;
            return result.Convert<Vector3>();
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"X: {this[0]}, Y: {this[1]}, Z: {this[2]}";
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof (Vector3))
                return this == (Vector3) obj;
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
                hash = hash*23 + X.GetHashCode();
                hash = hash*23 + Y.GetHashCode();
                hash = hash*23 + Z.GetHashCode();
                return hash;
            }
        }

        public bool Equals(Vector3 other)
        {
            return this == other;
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (uint i = 0; i < 3; i++)
            {
                yield return this[i];
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (uint i = 0; i < 3; i++)
            {
                yield return this[i];
            }
            yield break;
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Vector3" />.</param>
        /// <param name="right">The right <see cref="Vector3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.SequenceEqual(right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Vector3" />.</param>
        /// <param name="right">The right <see cref="Vector3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !left.SequenceEqual(right);
        }
    }
}