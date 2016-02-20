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
        /// <param name="vector">The <see cref="Point"/> to generate a <see cref="Point2D"/> from.</param>
        /// <returns>The generated <see cref="Point2D"/>.</returns>
        /// <exception cref="ArgumentException">The dimension of the given <see cref="Point"/> is invalid. It must be 2.</exception>
        public static Point2D FromPoint(Point point)
        {
            if (point.Dimension != 2)
                throw new ArgumentException("The dimension of the given point is invalid. It must be 2.");
            return new Point2D(point[0], point[1]);
        }

        public static Point2D operator +(Point2D first, Point2D second)
        {
            return FromPoint(Add(first, second));
        }

        public static Point2D operator -(Point2D first, Point2D second)
        {
            return FromPoint(Subtract(first, second));
        }
    }
}