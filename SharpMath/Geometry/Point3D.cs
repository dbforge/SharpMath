using System;

namespace SharpMath.Geometry
{
    public class Point3D : Point
    {
        public Point3D()
        { }

        public Point3D(double x, double y, double z) : base(x, y, z)
        { }

        public Point3D(Vector3 vector) : base(vector.X, vector.Y, vector.Z)
        { }

        /// <summary>
        ///     Gets or sets the value of the X-coordinate.
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
        ///     Gets or sets the value of the Y-coordinate.
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
        ///     Gets or sets the value of the Z-coordinate.
        /// </summary>
        public double Z
        {
            get
            {
                return this[2];
            }
            set
            {
                this[2] = value;
            }
        }

        /// <summary>
        ///     Gets the position <see cref="Vector3"/> of this <see cref="Point3D"/>.
        /// </summary>
        public new Vector3 PositionVector => new Vector3(X, Y, Z);

        /// <summary>
        ///     Generates a <see cref="Point3D"/> from the <see cref="Point"/> base class, if the dimension is correct.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> to generate a <see cref="Point3D"/> from.</param>
        /// <returns>The generated <see cref="Point3D"/>.</returns>
        /// <exception cref="ArgumentException">The dimension of the given <see cref="Point"/> is invalid. It must be 3.</exception>
        public static Point3D FromPoint(Point point)
        {
            if (point.Dimension != 3)
                throw new ArgumentException("The dimension of the given point is invalid. It must be 3.");
            return new Point3D(point[0], point[1], point[2]);
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first <see cref="Point3D"/>.</param>
        /// <param name="second">The second <see cref="Point3D"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Point3D operator +(Point3D first, Point3D second)
        {
            return FromPoint(Add(first, second));
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="first">The first <see cref="Point3D"/>.</param>
        /// <param name="second">The second <see cref="Point3D"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Point3D operator -(Point3D first, Point3D second)
        {
            return FromPoint(Subtract(first, second));
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
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
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() && this == (Point3D)obj;
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
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Point3D"/>.</param>
        /// <param name="right">The right <see cref="Point3D"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Point3D left, Point3D right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            for (uint i = 0; i < 3; ++i)
            {
                if (!FloatingNumber.AreApproximatelyEqual(left[i], right[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Point3D"/>.</param>
        /// <param name="right">The right <see cref="Point3D"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Point3D left, Point3D right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            for (uint i = 0; i < 3; ++i)
            {
                if (FloatingNumber.AreApproximatelyEqual(left[i], right[i]))
                    return false;
            }

            return true;
        }
    }
}