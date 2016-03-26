// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace SharpMath.Geometry
{
    public class Point3D : Point
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D" /> class.
        /// </summary>
        public Point3D() : base(3)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D" /> class.
        /// </summary>
        /// <param name="x">The value of the X-coordinate.</param>
        /// <param name="y">The value of the Y-coordinate.</param>
        /// <param name="z">The value of the Z-coordinate.</param>
        public Point3D(double x, double y, double z) : base(x, y, z)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D" /> class.
        /// </summary>
        /// <param name="point">The exisiting <see cref="Point3D" /> to copy.</param>
        public Point3D(Point3D point) : base(point)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D" /> class.
        /// </summary>
        /// <param name="vector">The position <see cref="Vector3" /> of the <see cref="Point3D" /> to create.</param>
        public Point3D(Vector3 vector) : base(vector)
        {
        }

        /// <summary>
        ///     Gets or sets the value of the X-coordinate.
        /// </summary>
        public double X
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        /// <summary>
        ///     Gets or sets the value of the Y-coordinate.
        /// </summary>
        public double Y
        {
            get { return this[1]; }
            set { this[1] = value; }
        }

        /// <summary>
        ///     Gets or sets the value of the Z-coordinate.
        /// </summary>
        public double Z
        {
            get { return this[2]; }
            set { this[2] = value; }
        }

        /// <summary>
        ///     Gets the position <see cref="Vector3" /> of this <see cref="Point3D" />.
        /// </summary>
        public new Vector3 PositionVector => new Vector3(X, Y, Z);

        /// <summary>
        ///     Generates a <see cref="Point3D" /> from the <see cref="Point" /> base class, if the dimension is correct.
        /// </summary>
        /// <param name="point">The <see cref="Point" /> to generate a <see cref="Point3D" /> from.</param>
        /// <returns>The generated <see cref="Point3D" />.</returns>
        /// <exception cref="ArgumentException">The dimension of the given <see cref="Point" /> is invalid. It must be 3.</exception>
        public static Point3D FromPoint(Point point)
        {
            if (point.Dimension != 3)
                throw new ArgumentException("The dimension of the given point is invalid. It must be 3.");
            return new Point3D(point[0], point[1], point[2]);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first <see cref="Point3D" />.</param>
        /// <param name="second">The second <see cref="Point3D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point3D operator +(Point3D first, Point3D second)
        {
            return FromPoint(Add(first, second));
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first <see cref="Point3D" />.</param>
        /// <param name="second">The second <see cref="Point3D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point3D operator -(Point3D first, Point3D second)
        {
            return FromPoint(Subtract(first, second));
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
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() == typeof (Point3D))
                return this == (Point3D) obj;
            var point = obj as Point;
            if (Dimension != point?.Dimension)
                return false;
            return this == FromPoint(point);
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

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Point3D" />.</param>
        /// <param name="right">The right <see cref="Point3D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Point3D left, Point3D right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            for (uint i = 0; i < 3; ++i)
            {
                if (!FloatingNumber.CheckApproximatelyEqual(left[i], right[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Point3D" />.</param>
        /// <param name="right">The right <see cref="Point3D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Point3D left, Point3D right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            for (uint i = 0; i < 3; ++i)
            {
                if (FloatingNumber.CheckApproximatelyEqual(left[i], right[i]))
                    return false;
            }

            return true;
        }
    }
}