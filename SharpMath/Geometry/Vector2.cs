using System;

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

        public static double Area(Vector2 firstVector, Vector2 secondVector)
        {
            return firstVector.X * secondVector.Y - firstVector.Y * secondVector.X;
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
    }
}