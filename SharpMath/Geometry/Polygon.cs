// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

// ReSharper disable ForCanBeConvertedToForeach

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a polygon.
    /// </summary>
    public class Polygon : IGeometricFigure<Point2D>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Polygon" /> class.
        /// </summary>
        public Polygon()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Polygon" /> class.
        /// </summary>
        /// <param name="points">The <see cref="Point2D" /> instances that the <see cref="Polygon" /> consists of.</param>
        public Polygon(params Point2D[] points)
        {
            if (points.Length < 3)
                throw new ArgumentException("A polygon needs to have at least 3 points.");
            Points = points.ToList();
        }

        /// <summary>
        ///     Gets the <see cref="Point2D" /> instances that this <see cref="Polygon" /> consists of.
        /// </summary>
        public List<Point2D> Points { get; protected set; } = new List<Point2D>();

        /// <summary>
        ///     Gets the bounding box of this <see cref="Polygon" /> as a <see cref="RectangleF" />.
        /// </summary>
        public RectangleF BoundingBox
        {
            get
            {
                double minX = 0;
                double maxX = 0;
                double minY = 0;
                double maxY = 0;

                foreach (var point in Points)
                {
                    if (point.X < minX)
                        minX = point.X;
                    else if (point.X > maxX)
                        maxX = point.X;

                    if (point.Y < minY)
                        minY = point.Y;
                    else if (point.Y > maxY)
                        maxY = point.Y;
                }

                return RectangleF.FromLTRB((float) minX, (float) minY, (float) maxX, (float) maxY);
            }
        }

        /// <summary>
        ///     Gets the perimeter of this <see cref="Polygon" />.
        /// </summary>
        public double Perimeter
        {
            get
            {
                double perimeter = 0;
                int j = 1;
                for (int i = 0; i < Points.Count; ++i)
                {
                    perimeter += (Points[j] - Points[i]).PositionVector.Magnitude;
                    j++;
                    if (j == Points.Count)
                        j = 0;
                }

                return perimeter;
            }
        }

        /// <summary>
        ///     Gets the area of this <see cref="Polygon" />.
        /// </summary>
        public double Area
        {
            get
            {
                double value = 0;
                int j = 1;
                for (int i = 0; i < Points.Count; ++i)
                {
                    value += Vector2.Area(Points[i].PositionVector, Points[j].PositionVector);
                    j++;
                    if (j == Points.Count)
                        j = 0;
                }
                return Math.Abs(value*0.5);
            }
        }

        /// <summary>
        ///     Gets the center of this <see cref="Polygon" />.
        /// </summary>
        public Point2D Center
        {
            get
            {
                double x = double.NaN;
                double y = double.NaN;
                for (int i = 0; i < Points.Count - 1; ++i)
                {
                    var factor = (Points[i].X*Points[i + 1].Y - Points[i + 1].X*Points[i].Y);
                    x += (Points[i].X + Points[i + 1].X)*factor;
                    y += (Points[i].Y + Points[i + 1].Y)*factor;
                }

                var areaFactor = (1d/6)*Area;
                return new Point2D(areaFactor*x, areaFactor*y);
            }
        }

        /// <summary>
        ///     Determines whether this <see cref="Polygon" /> fully contains the specified <see cref="Polygon" />.
        /// </summary>
        /// <param name="other">The other <see cref="Polygon" />.</param>
        /// <returns><c>true</c> if the specified <see cref="Polygon" /> is inside this one; otherwise <c>false</c>.</returns>
        public bool Contains(Polygon other)
        {
            return other.Points.All(ContainsPoint);
        }

        /// <summary>
        ///     Determines whether this <see cref="Polygon" /> contains the specified <see cref="Point" />.
        /// </summary>
        /// <param name="point">The <see cref="Point" /> to check.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Polygon" /> contains the specified <see cref="Point" />; otherwise <c>false</c>
        ///     .
        /// </returns>
        public bool ContainsPoint(Point2D point)
        {
            double t = -1;
            var points = Points;
            points[0] = points[points.Count - 1];

            for (int i = 0; i < points.Count - 1; i++)
                t = t*ContainsPointInternal(point, points[i], points[i + 1]);

            return t >= 0;
        }

        // https://de.wikipedia.org/wiki/Punkt-in-Polygon-Test_nach_Jordan
        private int ContainsPointInternal(Point2D q, Point2D p1, Point2D p2)
        {
            if (q == p1 && p1 == p2)
            {
                if ((p1.X <= q.X && q.X <= p2.X) || (p2.X <= q.X && q.X <= p1.X))
                    return 0;
                return 1;
            }
            if (p1.Y > p2.Y)
            {
                var currentP1 = p1;
                p1 = p2;
                p2 = currentP1;
            }
            else if (FloatingNumber.AreApproximatelyEqual(q.Y, p1.Y) && FloatingNumber.AreApproximatelyEqual(q.X, p1.X))
                return 0;
            else if (q.Y <= p1.Y || q.Y > p2.Y)
                return 1;

            var delta = (p1.X - q.X)*(p2.Y - q.Y) - (p1.Y - q.Y)*(p2.X - q.X);
            if (delta > 0)
                return -1;
            return delta < 0 ? 1 : 0;
        }
    }
}