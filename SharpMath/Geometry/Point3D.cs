using System;

namespace SharpMath.Geometry
{
    public class Point3D : Point
    {
        public Point3D()
        { }

        public Point3D(double x, double y, double z) : base(x, y, z)
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
        /// <param name="vector">The <see cref="Point"/> to generate a <see cref="Point3D"/> from.</param>
        /// <returns>The generated <see cref="Point3D"/>.</returns>
        /// <exception cref="ArgumentException">The dimension of the given <see cref="Point"/> is invalid. It must be 3.</exception>
        public static Point3D FromPoint(Point point)
        {
            if (point.Dimension != 3)
                throw new ArgumentException("The dimension of the given point is invalid. It must be 3.");
            return new Point3D(point[0], point[1], point[2]);
        }

        public static Point3D operator +(Point3D first, Point3D second)
        {
            return FromPoint(Add(first, second));
        }

        public static Point3D operator -(Point3D first, Point3D second)
        {
            return FromPoint(Subtract(first, second));
        }
    }
}