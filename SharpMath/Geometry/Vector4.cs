// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a four-dimensional vector with a homogeneous coordinate.
    /// </summary>
    public struct Vector4 : IVector, IEquatable<Vector4>, IEnumerable<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector4" /> class.
        /// </summary>
        /// <param name="x">The value of the X-coordinate (X2 in mathematic coordinate systems).</param>
        /// <param name="y">The value of the Y-coordinate (X3 in mathematic coordinate systems).</param>
        /// <param name="z">The value of the Z-coordinate (X1 in mathematic coordinate systems).</param>
        /// <param name="w">The value of the homogeneous coordinate.</param>
        public Vector4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector4" /> class.
        /// </summary>
        /// <param name="vector">The existing <see cref="Vector4" /> to copy.</param>
        public Vector4(Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = vector.W;
        }

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
                    case 3: return W;
                    default: throw new IndexOutOfRangeException("The index must be between 0 and 3.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new IndexOutOfRangeException("The index must be between 0 and 3.");
                }
            }
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
        ///     Gets or sets the value of the homogenous coordinate.
        /// </summary>
        public double W { get; set; }

        /// <summary>
        ///     A <see cref="Vector4" /> with all values set to zero.
        /// </summary>
        public static Vector4 Zero => new Vector4();

        /// <summary>
        ///     A unit <see cref="Vector3" /> with all values set to one.
        /// </summary>
        public static Vector4 One => new Vector4(1, 1, 1, 1);

        /// <summary>
        ///     A unit <see cref="Vector4" />  with coordinates 1, 0, 0, 0.
        /// </summary>
        public static Vector4 UnitX => new Vector4(1, 0, 0, 0);

        /// <summary>
        ///     A unit <see cref="Vector4" />  with coordinates 0, 1, 0, 0.
        /// </summary>
        public static Vector4 UnitY => new Vector4(0, 1, 0, 0);

        /// <summary>
        ///     A unit <see cref="Vector4" />  with coordinates 0, 0, 1, 0.
        /// </summary>
        public static Vector4 UnitZ => new Vector4(0, 0, 1, 0);

        /// <summary>
        ///     A unit <see cref="Vector4" />  with coordinates 0, 0, 0, 1.
        /// </summary>
        public static Vector4 UnitW => new Vector4(0, 0, 1, 0);

        /// <summary>
        ///     Gets the dimension of the <see cref="Vector3" />.
        /// </summary>
        public uint Dimension => 4;

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
                for (uint i = 0; i < 4; ++i)
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
            =>
                @"\left( \begin{array}{c} " + this[0] + @" \\ " + this[1] + @" \\ " + this[2] + @" \\ " + this[3] +
                @" \end{array} \right)";

        /// <summary>
        ///     Generates a <see cref="Vector4" /> from an object implementing the <see cref="IVector" /> interface, if the dimension is correct.
        /// </summary>
        /// <param name="vector">The <see cref="IVector" /> to generate a <see cref="Vector4" /> from.</param>
        /// <returns>The generated <see cref="Vector4" />.</returns>
        /// <exception cref="ArgumentException">The dimension of the given vector is invalid. It must be 4.</exception>
        public static Vector4 FromVector(IVector vector)
        {
            if (vector.Dimension != 4)
                throw new ArgumentException("The dimension of the given vector is invalid. It must be 4.");
            return new Vector4(vector[0], vector[1], vector[2], vector[3]);
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector4" /> with the specified <see cref="Matrix4x4" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector4" /> that should be transformed.</param>
        /// <param name="matrix">The transformation <see cref="Matrix4x4" />.</param>
        /// <returns>The transformed <see cref="Vector4" />.</returns>
        public static Vector4 Transform(Vector4 vector, Matrix4x4 matrix)
        {
            return matrix*vector;
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"X: {this[0]}, Y: {this[1]}, Z: {this[2]}, W: {this[3]}";
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
            if (obj.GetType() == typeof (Vector4))
                return this == (Vector4) obj;
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
                hash = hash*23 + W.GetHashCode();
                return hash;
            }
        }

        public bool Equals(Vector4 other)
        {
            return this == other;
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (uint i = 0; i < 4; i++)
            {
                yield return this[i];
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (uint i = 0; i < 4; i++)
            {
                yield return this[i];
            }
            yield break;
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Vector4" />.</param>
        /// <param name="right">The right <see cref="Vector4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Vector4 left, Vector4 right)
        {
            return left.SequenceEqual(right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Vector4" />.</param>
        /// <param name="right">The right <see cref="Vector4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Vector4 left, Vector4 right)
        {
            return !left.SequenceEqual(right);
        }
    }
}