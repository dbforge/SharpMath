using System;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 3D-line.
    /// </summary>
    public class Line3D
    {
        private Vector3 _direction;

        /// <summary>
        ///     Gets or sets a <see cref="Vector3"/> that represents the direction of the <see cref="Line3D"/>.
        /// </summary>
        public Vector3 Direction
        {
            get { return _direction; }
            set
            {
                if (value == Vector3.Zero)
                    throw new InvalidOperationException("The direction vector of the line must not be the zero vector.");
                _direction = value;
            }
        }
        
        /// <summary>
        ///     Gets or sets a point on the <see cref="Line3D"/> that the line intersects with.
        /// </summary>
        public Point3D Point { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line3D"/> class.
        /// </summary>
        /// <param name="point">A point on the <see cref="Line3D"/>.</param>
        /// <param name="direction">The direction <see cref="Vector3"/>.</param>
        public Line3D(Point3D point, Vector3 direction)
        {
            Point = point;
            Direction = direction;
        }

        /// <summary>
        ///     Creates a <see cref="Line3D"/> from two <see cref="Point3D"/> instances.
        /// </summary>
        /// <param name="firstPoint">The first <see cref="Point3D"/>.</param>
        /// <param name="secondPoint">The second <see cref="Point3D"/>.</param>
        /// <returns>The created <see cref="Line3D"/>.</returns>
        public static Line3D FromPoints(Point3D firstPoint, Point3D secondPoint)
        {
            return new Line3D(firstPoint, (secondPoint - firstPoint).PositionVector);
        }

        /// <summary>
        ///     Gets a <see cref="Point3D"/> on the <see cref="Line3D"/>.
        /// </summary>
        /// <param name="lambda">The scalar that the direction vector should be multiplied with.</param>
        /// <returns>The relating <see cref="Point3D"/> on the <see cref="Line3D"/>.</returns>
        public Point3D GetPoint(double lambda)
        {
            var resultVector = Point.PositionVector + Direction*lambda;
            return new Point3D(resultVector.X, resultVector.Y, resultVector.Z);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Point3D"/> is on the <see cref="Line3D"/>, or not.
        /// </summary>
        /// <param name="point">The <see cref="Point3D"/>.</param>
        /// <returns><c>true</c>, if the <see cref="Point3D"/> is on the <see cref="Line3D"/>; otherwise <c>false</c>.</returns>
        public bool IsPointOnLine(Point3D point)
        {
            double lambda = 0;
            for (uint i = 0; i < 3; ++i)
            {
                double value = (point[i] - Point[i]) / Direction[i];
                if (i != 0 && !FloatingNumber.AreApproximatelyEqual(value, lambda))
                    return false;
                lambda = value;
            }

            return true;
        }
    }
}