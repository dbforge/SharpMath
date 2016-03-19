using System;

namespace SharpMath.Geometry
{
    public class Point2D : Point
    {
        public Point2D(double x, double y) : base(x, y)
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
        ///     Gets the position <see cref="Vector2"/> of this <see cref="Point2D"/>.
        /// </summary>
        public new Vector2 PositionVector => new Vector2(X, Y);

        /// <summary>
        ///     Generates a <see cref="Point2D"/> from the <see cref="Point"/> base class, if the dimension is correct.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> to generate a <see cref="Point2D"/> from.</param>
        /// <returns>The generated <see cref="Point2D"/>.</returns>
        /// <exception cref="ArgumentException">The dimension of the given <see cref="Point"/> is invalid. It must be 2.</exception>
        public static Point2D FromPoint(Point point)
        {
            if (point.Dimension != 2)
                throw new ArgumentException("The dimension of the given point is invalid. It must be 2.");
            return new Point2D(point[0], point[1]);
        }
        
        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first <see cref="Point2D"/>.</param>
        /// <param name="second">The second <see cref="Point2D"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Point2D operator +(Point2D first, Point2D second)
        {
            return FromPoint(Add(first, second));
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first <see cref="Point2D"/>.</param>
        /// <param name="second">The second <see cref="Point2D"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Point2D operator -(Point2D first, Point2D second)
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

            return obj.GetType() == GetType() && this == (Point2D)obj;
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
        /// <param name="left">The left <see cref="Point2D"/>.</param>
        /// <param name="right">The right <see cref="Point2D"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Point2D left, Point2D right)
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
        /// <param name="left">The left <see cref="Point2D"/>.</param>
        /// <param name="right">The right <see cref="Point2D"/>.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Point2D left, Point2D right)
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