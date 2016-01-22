using System;

namespace SharpMath.Geometry
{
    public class Line2D
    {
        public Line2D() 
            : this (0, 0)
        {
        }

        public Line2D(double slope, double offset)
        {
            Slope = slope;
            Offset = offset;
            Term = $"{Slope}x+{Offset}";
        }

        public Line2D(Point2D a, Point2D b)
        {
            var line = FromPoints(a, b);
            Slope = line.Slope;
            Offset = line.Offset;
            Term = line.Term;
        }

        public double Slope { get; set; }
        public double Offset { get; set; }
        public string Term { get; set; }

        public static Line2D FromPoints(Point2D a, Point2D b)
        {
            var vectorA = Vector2.FromVector(a);
            var vectorB = Vector2.FromVector(b);

            var offset = (vectorB.Y - vectorA.Y) / (vectorB.X - vectorA.X);
            var slope = vectorA.Y / (offset * vectorA.X); // Insert a point
            return new Line2D(offset, slope);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Point2D"/> is on the <see cref="Line2D"/>, or not.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Returns <c>true</c> if the <see cref="Point2D"/> is on the <see cref="Line2D"/>, otherwise <c>false</c>.</returns>
        public bool IsPointOnLine(Point2D point)
        {
            return Math.Abs(GetPoint(point.X).Y - point.Y) < FloatingNumber.Epsilon;
        }

        public bool IsParallelTo(Line2D line)
        {
            return AreParallel(this, line);
        }

        public static bool AreParallel(Line2D firstLine, Line2D secondLine)
        {
            return FloatingNumber.AreApproximatelyEqual(firstLine.Slope, secondLine.Slope);
        }

        public bool IntersectsWith(Line2D line)
        {
            return !IsParallelTo(line);
        }

        public Point2D GetIntersectionPoint(Line2D line)
        {
            return GetIntersectionPoint(this, line);
        }

        public static Point2D GetIntersectionPoint(Line2D firstLine, Line2D secondLine)
        {
            if (AreParallel(firstLine, secondLine))
                return new Point2D(double.NaN, double.NaN);

            var pointX = (secondLine.Offset - firstLine.Offset) / (firstLine.Slope - secondLine.Slope);
            return new Point2D(pointX, firstLine.Slope * pointX + firstLine.Offset);
        }

        public Point2D GetPoint(double x)
        {
            return new Point2D(x, Slope * x + Offset);
        }
    }
}