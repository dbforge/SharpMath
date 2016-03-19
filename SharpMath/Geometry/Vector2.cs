using System;
using SharpMath.Geometry.Exceptions;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a two-dimensional vector.
    /// </summary>
    public class Vector2 : Vector
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2"/> class.
        /// </summary>
        public Vector2()
            : base(2)
        {
            // We don't need to set anything as value types are initialized by default with the values we want
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2"/> class.
        /// </summary>
        /// <param name="vector">The existing <see cref="Vector2"/> to copy.</param>
        public Vector2(Vector2 vector)
            : base(vector)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2"/> class.
        /// </summary>
        /// <param name="x">The value of the X-coordinate (X1 in mathematic coordinate systems).</param>
        /// <param name="y">The value of the Y-coordinate (X2 in mathematic coordinate systems).</param>
        public Vector2(double x, double y)
            : base(x, y)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2"/> class.
        /// </summary>
        /// <param name="point">The <see cref="Point2D"/> that a position <see cref="Vector2"/> should be created for.</param>
        public Vector2(Point2D point)
            : base(point.PositionVector)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector2"/> class.
        /// </summary>
        /// <param name="bottom">The tail of the <see cref="Vector2"/>.</param>
        /// <param name="tip">The head of the <see cref="Vector2"/>.</param>
        public Vector2(Point2D bottom, Point2D tip)
            : base((tip - bottom).PositionVector)
        {
        }

        /// <summary>
        ///     Generates a <see cref="Vector2"/> from the <see cref="Vector"/> base class, if the dimension is correct.
        /// </summary>
        /// <param name="vector">The <see cref="Vector"/> to generate a <see cref="Vector2"/> from.</param>
        /// <returns>The generated <see cref="Vector2"/>.</returns>
        /// <exception cref="ArgumentException">The dimension of the given vector is invalid. It must be 2.</exception>
        public static Vector2 FromVector(Vector vector)
        {
            if (vector.Dimension != 2)
                throw new ArgumentException("The dimension of the given vector is invalid. It must be 2.");
            return new Vector2(vector[0], vector[1]);
        }

        /// <summary>
        ///     Gets or sets the value of the X-coordinate (X1 in mathematic coordinate systems).
        /// </summary>
        public double X
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the Y-coordinate (X2 in mathematic coordinate systems).
        /// </summary>
        public double Y
        {
            get
            {
                return this[1];
            }
            set
            {
                this[1] = value;
            }
        }

        /// <summary>
        ///     A <see cref="Vector2"/> with all values set to zero.
        /// </summary>
        public static Vector2 Zero => new Vector2(0, 0);

        /// <summary>
        ///     A <see cref="Vector2"/> with all values set to one.
        /// </summary>
        public static Vector2 One => new Vector2(1, 1);

        /// <summary>
        ///     A unit <see cref="Vector2"/> pointing up.
        /// </summary>
        public static Vector2 Up => new Vector2(0, 1);

        /// <summary>
        ///     A unit <see cref="Vector2"/> pointing down.
        /// </summary>
        public static Vector2 Down => new Vector2(0, -1);

        /// <summary>
        ///     A unit <see cref="Vector2"/> pointing to the left.
        /// </summary>
        public static Vector2 Left => new Vector2(-1, 0);

        /// <summary>
        ///     A unit <see cref="Vector2"/> pointing to the right.
        /// </summary>
        public static Vector2 Right => new Vector2(1, 0);

        /// <summary>
        ///     A unit <see cref="Vector2"/>  with coordinates 1, 0.
        /// </summary>
        public static Vector2 UnitX => new Vector2(1, 0);

        /// <summary>
        ///     A unit <see cref="Vector2"/>  with coordinates 0, 1.
        /// </summary>
        public static Vector2 UnitY => new Vector2(0, 1);

        /// <summary>
        ///     Gets the <see cref="Vector2"/> that is perpendicular to this <see cref="Vector2"/>.
        /// </summary>
        public Vector2 CrossProduct => new Vector2(Y, -X);

        /// <summary>
        ///    Linearly interpolates between two <see cref="Vector2"/> instances.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns>The position <see cref="Vector2"/> of the new point.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public static Vector2 Lerp(Vector2 source, Vector2 target, double fraction)
        {
            return FromVector(Vector.Lerp(source, target, fraction));
        }

        /// <summary>
        ///    Linearly interpolates between two <see cref="Vector2"/> instances.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="fraction">The fraction.</param>
        /// <returns>The position <see cref="Vector2"/> of the new point.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public static Vector2 LerpUnclamped(Vector2 source, Vector2 target, double fraction)
        {
            return FromVector(Vector.LerpUnclamped(source, target, fraction));
        }

        /// <summary>
        ///    Moves a source point in a straight line towards a target point by adding the given distance delta and returns its new position.
        /// </summary>
        /// <param name="source">The source point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="maxDistanceDelta">The distance delta that the source point is moved by in all directions.</param>
        /// <returns>The position <see cref="Vector2"/> of the new point.</returns>
        /// <exception cref="DimensionException">The dimensions of the vectors do not equal each other.</exception>
        public static Vector2 MoveTowards(Vector2 source, Vector2 target, double maxDistanceDelta)
        {
            return FromVector(Vector.MoveTowards(source, target, maxDistanceDelta));
        }

        /// <summary>
        ///     Calculates the area of the parallelogram that this and the specified <see cref="Vector2"/> instances span.
        /// </summary>
        /// <param name="other">The other <see cref="Vector2"/>.</param>
        /// <returns>The area of the spanned parallelogram.</returns>
        public double Area(Vector2 other)
        {
            if (this == Zero || other == Zero)
                return 0;
           return Magnitude * Math.Sin(Angle(other)) * other.Magnitude;
        }

        /// <summary>
        ///     Calculates the area of the parallelogram that the two specified <see cref="Vector2"/> instances span.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2"/>.</param>
        /// <param name="secondVector">The second <see cref="Vector2"/>.</param>
        /// <returns>The area of the spanned parallelogram.</returns>
        public static double Area(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.Area(secondVector);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="firstVector">The first vector.</param>
        /// <param name="secondVector">The second vector.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2"/>.
        /// </returns>
        public static Vector2 operator +(Vector2 firstVector, Vector2 secondVector)
        {
            return FromVector(Add(firstVector, secondVector));
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="firstVector">The first vector.</param>
        /// <param name="secondVector">The second vector.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2"/>.
        /// </returns>
        public static Vector2 operator -(Vector2 firstVector, Vector2 secondVector)
        {
            return FromVector(Subtract(firstVector, secondVector));
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="current">The vector to negate.</param>
        /// <returns>
        ///     The negated <see cref="Vector2"/>.
        /// </returns>
        public static Vector2 operator -(Vector2 current)
        {
            return FromVector(current.Negate());
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The resulting <see cref="Vector2"/>.
        /// </returns>
        public static Vector2 operator *(Vector2 vector, double scalar)
        {
            return FromVector(Multiply(vector, scalar));
        }

        /// <summary>
        ///     Implements the operator * for calculating the scalar product of two <see cref="Vector2"/> instances.
        /// </summary>
        /// <param name="firstVector">The first <see cref="Vector2"/>.</param>
        /// <param name="secondVector">The second <see cref="Vector2"/>.</param>
        /// <returns>
        ///     The scalar that has been calculated.
        /// </returns>
        public static double operator *(Vector2 firstVector, Vector2 secondVector)
        {
            return ScalarProduct(firstVector, secondVector);
        }

        /// <summary>
        ///     Transforms this <see cref="Vector2"/> with the specified <see cref="Matrix3x3"/>.
        /// </summary>
        /// <param name="matrix">The transformation <see cref="Matrix3x3"/>.</param>
        /// <returns>The transformed <see cref="Vector2"/>.</returns>
        public Vector2 Transform(Matrix3x3 matrix)
        {
            return matrix * this;
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2"/> with the specified <see cref="Matrix3x3"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2"/> that should be transformed.</param>
        /// <param name="matrix">The transformation <see cref="Matrix3x3"/>.</param>
        /// <returns>The transformed <see cref="Vector2"/>.</returns>
        public static Vector2 Transform(Vector2 vector, Matrix3x3 matrix)
        {
            return vector.Transform(matrix);
        }

        /// <summary>
        ///     Transforms the specified <see cref="Vector2"/> with the specified <see cref="Matrix3x3"/>.
        /// </summary>
        /// <param name="matrix">The transformation <see cref="Matrix3x3"/>.</param>
        /// <param name="vector">The <see cref="Vector2"/> that should be transformed.</param>
        /// <returns>The transformed <see cref="Vector2"/>.</returns>
        public static Vector2 Transform(Matrix3x3 matrix, Vector2 vector)
        {
            return vector.Transform(matrix);
        }

        /// <summary>
        ///     Gets the LaTeX-string representing this vector graphically.
        /// </summary>
        public string LaTeXString => @"\left( \begin{array}{c} " + this[0] + @" \\ " + this[1] + @" \end{array} \right)";

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"X: {this[0]}, Y: {this[1]}";
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && this == (Vector2)obj;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Vector2"/>.</param>
        /// <param name="right">The right <see cref="Vector2"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            for (uint i = 0; i < 2; ++i)
            {
                if (!FloatingNumber.AreApproximatelyEqual(left[i], right[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Vector2"/>.</param>
        /// <param name="right">The right <see cref="Vector2"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            for (uint i = 0; i < 2; ++i)
            {
                if (FloatingNumber.AreApproximatelyEqual(left[i], right[i]))
                    return false;
            }

            return true;
        }
    }
}