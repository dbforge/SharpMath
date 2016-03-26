// Author: Dominic Beger (Trade/ProgTrade) 2016
// Improvements: Stefan Baumann 2016

using System;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a line in a 2-dimensional room.
    /// </summary>
    public class Line2D : IEquatable<Line2D>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2D" /> class.
        /// </summary>
        public Line2D()
            : this(0, 0)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2D" /> class.
        /// </summary>
        /// <param name="slope">The slope of the <see cref="Line2D" />.</param>
        /// <param name="offset">The offset of the <see cref="Line2D" />.</param>
        public Line2D(double slope, double offset)
        {
            Slope = slope;
            Offset = offset;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2D" /> class.
        /// </summary>
        /// <param name="a">The first <see cref="Point2D" /> that should be used to create the <see cref="Line2D" />.</param>
        /// <param name="b">The second <see cref="Point2D" /> that should be used to create the <see cref="Line2D" />.</param>
        public Line2D(Point2D a, Point2D b)
        {
            var line = FromPoints(a, b);
            Slope = line.Slope;
            Offset = line.Offset;
        }

        /// <summary>
        ///     Gets or sets the slope of the <see cref="Line2D" />.
        /// </summary>
        public double Slope { get; set; }

        /// <summary>
        ///     Gets or sets the offset of the <see cref="Line2D" />.
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        ///     Creates a <see cref="Line2D" /> using the two specified <see cref="Point2D" /> instances.
        /// </summary>
        /// <param name="a">The first <see cref="Point2D" />.</param>
        /// <param name="b">The second <see cref="Point2D" />.</param>
        /// <returns></returns>
        public static Line2D FromPoints(Point2D a, Point2D b)
        {
            var vectorA = a.PositionVector;
            var vectorB = b.PositionVector;

            var slope = (vectorB.Y - vectorA.Y)/(vectorB.X - vectorA.X);
            var offset = vectorA.Y - (slope*vectorA.X); // Insert a point
            return new Line2D(slope, offset);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Point2D" /> is on the <see cref="Line2D" />, or not.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if the <see cref="Point2D" /> is on the <see cref="Line2D" />; otherwise <c>false</c>.</returns>
        public bool IsPointOnLine(Point2D point)
        {
            return Math.Abs(GetPoint(point.X).Y - point.Y) < FloatingNumber.Epsilon;
        }

        /// <summary>
        ///     Determines whether this <see cref="Line2D" /> is parallel to the specified <see cref="Line2D" />.
        /// </summary>
        /// <param name="line">The other <see cref="Line2D" />.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Line2D" /> is parallel to the specified <see cref="Line2D" />, otherwise
        ///     <c>false</c>.
        /// </returns>
        public bool IsParallelTo(Line2D line)
        {
            return FloatingNumber.CheckApproximatelyEqual(Slope, line.Slope);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Line2D" /> instances are parallel to each other, or not.
        /// </summary>
        /// <param name="firstLine">The first <see cref="Line2D" />.</param>
        /// <param name="secondLine">The second <see cref="Line2D" />.</param>
        /// <returns><c>true</c> if the <see cref="Line2D" /> instances are parallel to each other, otherwise <c>false</c>.</returns>
        public static bool AreParallel(Line2D firstLine, Line2D secondLine)
        {
            return firstLine.IsParallelTo(secondLine);
        }

        /// <summary>
        ///     Determines whether this <see cref="Line2D" /> intersects with the specified <see cref="Line2D" />, or not.
        /// </summary>
        /// <param name="line">The other <see cref="Line2D" />.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Line2D" /> intersects with the specified <see cref="Line2D" />, otherwise
        ///     <c>false</c>.
        /// </returns>
        public bool IntersectsWith(Line2D line)
        {
            return !IsParallelTo(line);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Line2D" /> instances intersect, or not.
        /// </summary>
        /// <param name="firstLine">The first <see cref="Line2D" />.</param>
        /// <param name="secondLine">The second <see cref="Line2D" />.</param>
        /// <returns><c>true</c> if the <see cref="Line2D" /> instances intersect, otherwise <c>false</c>.</returns>
        public static bool AreIntersecting(Line2D firstLine, Line2D secondLine)
        {
            return firstLine.IntersectsWith(secondLine);
        }

        /// <summary>
        ///     Gets the intersection <see cref="Point2D" /> of this and the specified <see cref="Line2D" />.
        /// </summary>
        /// <param name="line">The other <see cref="Line2D" />.</param>
        /// <returns>The intersection <see cref="Point2D" />.</returns>
        public Point2D GetIntersectionPoint(Line2D line)
        {
            if (IsParallelTo(line))
                return new Point2D(double.NaN, double.NaN);

            var pointX = (line.Offset - Offset)/(Slope - line.Slope);
            return new Point2D(pointX, Slope*pointX + Offset);
        }

        /// <summary>
        ///     Gets the intersection <see cref="Point2D" /> of the specified <see cref="Line2D" /> instances.
        /// </summary>
        /// <param name="firstLine">The first <see cref="Line2D" />.</param>
        /// <param name="secondLine">The second <see cref="Line2D" />.</param>
        /// <returns>The intersection <see cref="Point2D" />.</returns>
        public static Point2D GetIntersectionPoint(Line2D firstLine, Line2D secondLine)
        {
            return firstLine.GetIntersectionPoint(secondLine);
        }

        /// <summary>
        ///     Gets the <see cref="Point2D" /> on the <see cref="Line2D" /> with the specified X-coordinate.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <returns>The <see cref="Point2D" /> with the specified X-coordinate.</returns>
        public Point2D GetPoint(double x)
        {
            return new Point2D(x, Slope*x + Offset);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj == null ? this == null : obj is Line2D && ((Line2D)obj).Offset == Offset && ((Line2D)obj).Slope == Slope;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Line2D" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Line2D" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Line2D" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Line2D other)
        {
            return other == null ? this == null : other.Offset == Offset && other.Slope == Slope;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Offset.GetHashCode() ^ Slope.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Line2D [Offset={0}; Slope={1}]", Offset, Slope);
        }

        /// <summary>
        /// Determines whether the two specified <see cref="Line2D" /> instances are equal to each other.
        /// </summary>
        /// <param name="left">The first <see cref="Line2D" />.</param>
        /// <param name="right">The <see cref="Line2D" /> to compare with the other <see cref="Line2D" />.</param>
        /// <returns>
        ///   <c>true</c> if the two specified <see cref="Line2D" /> are equal to each other; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Line2D left, Line2D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the two specified <see cref="Line2D" /> instances are not equal to each other.
        /// </summary>
        /// <param name="left">The first <see cref="Line2D" />.</param>
        /// <param name="right">The <see cref="Line2D" /> to compare with the other <see cref="Line2D" />.</param>
        /// <returns>
        ///   <c>true</c> if the two specified <see cref="Line2D" /> are not equal to each other; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Line2D left, Line2D right)
        {
            return !left.Equals(right);
        }
    }
}