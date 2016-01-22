namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 3D-line.
    /// </summary>
    public class Line3D
    {
        /// <summary>
        ///     Gets or sets a <see cref="Vector3"/> that represents the direction of the <see cref="Line3D"/>.
        /// </summary>
        public Vector3 Direction { get; set; }

        // TODO: Check doc
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

        public static Line3D FromPoints(Point3D firstPoint, Point3D secondPoint)
        {
            return new Line3D(firstPoint, Vector3.FromVector(Vector.Subtract(secondPoint, firstPoint)));
        }

        public Point3D GetPoint(double lambda)
        {
            return (Point3D)Vector.Add(Point, Direction*lambda);
        }

        public bool IsPointOnLine(Point3D point)
        {
            return true;
        }

        public string ParameterFormEquation => $"{Point} + λ*{Direction}";
    }
}