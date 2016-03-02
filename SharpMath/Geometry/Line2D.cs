using System;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a line in a 2-dimensional room.
    /// </summary>
    public class Line2D
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2D"/> class.
        /// </summary>
        public Line2D() 
            : this (0, 0)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2D"/> class.
        /// </summary>
        /// <param name="slope">The slope of the <see cref="Line2D"/>.</param>
        /// <param name="offset">The offset of the <see cref="Line2D"/>.</param>
        public Line2D(double slope, double offset)
        {
            Slope = slope;
            Offset = offset;
            Term = $"{Slope}x+{Offset}";
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2D"/> class.
        /// </summary>
        /// <param name="a">The first <see cref="Point2D"/> that should be used to create the <see cref="Line2D"/>.</param>
        /// <param name="b">The second <see cref="Point2D"/> that should be used to create the <see cref="Line2D"/>.</param>
        public Line2D(Point2D a, Point2D b)
        {
            var line = FromPoints(a, b);
            Slope = line.Slope;
            Offset = line.Offset;
            Term = line.Term;
        }

        /// <summary>
        ///     Gets or sets the slope of the <see cref="Line2D"/>.
        /// </summary>
        public double Slope { get; set; }

        /// <summary>
        ///     Gets or sets the offset of the <see cref="Line2D"/>.
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        ///     Gets or sets the term of the <see cref="Line2D"/>.
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        ///     Creates a <see cref="Line2D"/> using the two specified <see cref="Point2D"/> instances.
        /// </summary>
        /// <param name="a">The first <see cref="Point2D"/>.</param>
        /// <param name="b">The second <see cref="Point2D"/>.</param>
        /// <returns></returns>
        public static Line2D FromPoints(Point2D a, Point2D b)
        {
            var vectorA = a.PositionVector;
            var vectorB = b.PositionVector;

            var slope = (vectorB.Y - vectorA.Y) / (vectorB.X - vectorA.X);
            var offset = vectorA.Y / (slope * vectorA.X); // Insert a point
            return new Line2D(slope, offset);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Point2D"/> is on the <see cref="Line2D"/>, or not.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if the <see cref="Point2D"/> is on the <see cref="Line2D"/>; otherwise <c>false</c>.</returns>
        public bool IsPointOnLine(Point2D point)
        {
            return Math.Abs(GetPoint(point.X).Y - point.Y) < FloatingNumber.Epsilon;
        }

        /// <summary>
        ///     Determines whether this <see cref="Line2D"/> is parallel to the specified <see cref="Line2D"/>.
        /// </summary>
        /// <param name="line">The other <see cref="Line2D"/>.</param>
        /// <returns><c>true</c> if this <see cref="Line2D"/> is parallel to the specified <see cref="Line2D"/>, otherwise <c>false</c>.</returns>
        public bool IsParallelTo(Line2D line)
        {
            return AreParallel(this, line);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Line2D"/> instances are parallel to each other, or not.
        /// </summary>
        /// <param name="firstLine">The first <see cref="Line2D"/>.</param>
        /// <param name="secondLine">The second <see cref="Line2D"/>.</param>
        /// <returns><c>true</c> if the <see cref="Line2D"/> instances are parallel to each other, otherwise <c>false</c>.</returns>
        public static bool AreParallel(Line2D firstLine, Line2D secondLine)
        {
            return FloatingNumber.AreApproximatelyEqual(firstLine.Slope, secondLine.Slope);
        }

        /// <summary>
        ///     Determines whether this <see cref="Line2D"/> intersects with the specified <see cref="Line2D"/>, or not.
        /// </summary>
        /// <param name="line">The other <see cref="Line2D"/>.</param>
        /// <returns><c>true</c> if this <see cref="Line2D"/> intersects with the specified <see cref="Line2D"/>, otherwise <c>false</c>.</returns>
        public bool IntersectsWith(Line2D line)
        {
            return !IsParallelTo(line);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Line2D"/> instances intersect, or not.
        /// </summary>
        /// <param name="firstLine">The first <see cref="Line2D"/>.</param>
        /// <param name="secondLine">The second <see cref="Line2D"/>.</param>
        /// <returns><c>true</c> if the <see cref="Line2D"/> instances intersect, otherwise <c>false</c>.</returns>
        public static bool AreIntersecting(Line2D firstLine, Line2D secondLine)
        {
            return firstLine.IntersectsWith(secondLine);
        }

        /// <summary>
        ///     Gets the intersection <see cref="Point2D"/> of this and the specified <see cref="Line2D"/>.
        /// </summary>
        /// <param name="line">The other <see cref="Line2D"/>.</param>
        /// <returns>The intersection <see cref="Point2D"/>.</returns>
        public Point2D GetIntersectionPoint(Line2D line)
        {
            return GetIntersectionPoint(this, line);
        }

        /// <summary>
        ///     Gets the intersection <see cref="Point2D"/> of the specified <see cref="Line2D"/> instances.
        /// </summary>
        /// <param name="firstLine">The first <see cref="Line2D"/>.</param>
        /// <param name="secondLine">The second <see cref="Line2D"/>.</param>
        /// <returns>The intersection <see cref="Point2D"/>.</returns>
        public static Point2D GetIntersectionPoint(Line2D firstLine, Line2D secondLine)
        {
            if (firstLine.IsParallelTo(secondLine))
                return new Point2D(double.NaN, double.NaN);

            var pointX = (secondLine.Offset - firstLine.Offset) / (firstLine.Slope - secondLine.Slope);
            return new Point2D(pointX, firstLine.Slope * pointX + firstLine.Offset);
        }

        /// <summary>
        ///     Gets the <see cref="Point2D"/> on the <see cref="Line2D"/> with the specified X-coordinate.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <returns>The <see cref="Point2D"/> with the specified X-coordinate.</returns>
        public Point2D GetPoint(double x)
        {
            return new Point2D(x, Slope * x + Offset);
        }
    }
}