using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
// ReSharper disable ForCanBeConvertedToForeach

namespace SharpMath.Geometry
{
    public class Polygon : IGeometricFigure<Point2D>
    {
        public List<Point2D> Points { get; protected set; } = new List<Point2D>();

        public Polygon()
        { }

        public Polygon(params Point2D[] points)
        {
            if (points.Length < 3)
                throw new ArgumentException("A polygon needs to have at least 3 points.");
            Points = points.ToList();
        }

        /// <summary>
        ///     Gets the perimeter of this <see cref="Polygon"/>.
        /// </summary>
        public double Perimeter
        {
            get
            {
                double perimeter = 0;
                int j = 1;
                for (int i = 0;  i < Points.Count; ++i)
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
        ///     Gets the area of this <see cref="Polygon"/>.
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
                return Math.Abs(value * 0.5);
            }
        }
        
        /// <summary>
        ///     Gets the center of this <see cref="Polygon"/>.
        /// </summary>
        public Point2D Center
        {
            get
            {
                double x = double.NaN;
                double y = double.NaN;
                for (int i = 0; i < Points.Count - 1; ++i)
                {
                    var factor = (Points[i].X * Points[i + 1].Y - Points[i + 1].X * Points[i].Y);
                    x += (Points[i].X + Points[i + 1].X) * factor;
                    y += (Points[i].Y + Points[i + 1].Y) * factor;
                }

                var areaFactor = (1d / 6) * Area;
                return new Point2D(areaFactor * x, areaFactor * y);
            }
        }

        /// <summary>
        ///     Gets the bounding box of this <see cref="Polygon"/> as a <see cref="RectangleF"/>.
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

                return RectangleF.FromLTRB((float)minX, (float)minY, (float)maxX, (float)maxY);
            }
        }

        /// <summary>
        ///     Determines whether this <see cref="Polygon"/> fully contains the specified <see cref="Polygon"/>.
        /// </summary>
        /// <param name="other">The other <see cref="Polygon"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Polygon"/> is inside this one; otherwise <c>false</c>.</returns>
        public bool Contains(Polygon other)
        {
            return other.Points.All(ContainsPoint);
        }

        /// <summary>
        ///     Determines whether this <see cref="Polygon"/> contains the specified <see cref="Point"/>.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> to check.</param>
        /// <returns><c>true</c> if this <see cref="Polygon"/> contains the specified <see cref="Point"/>; otherwise <c>false</c>.</returns>
        public bool ContainsPoint(Point2D point)
        {
            double t = -1;
            var points = Points;
            points[0] = points[points.Count - 1];

            for (int i = 0; i < points.Count - 1; i++)
                t = t * ContainsPointInternal(point, points[i], points[i + 1]);

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

            var delta = (p1.X - q.X) * (p2.Y - q.Y) - (p1.Y - q.Y) * (p2.X - q.X);
            if (delta > 0)
                return -1;
            return delta < 0 ? 1 : 0;
        }

        public Polygon ConvexHull()
        {
            return ConvexHull(this);
        }

        public static Polygon ConvexHull(Polygon sourcePolygon)
        {
            var points = sourcePolygon.Points;
            var minPoint = points.First(point => FloatingNumber.AreApproximatelyEqual(point.X, points.Min(p => p.X)));
            var maxPoint = points.First(point => FloatingNumber.AreApproximatelyEqual(point.X, points.Max(p => p.X)));

            // Build a line with the two border points
            var line = new Line2D(minPoint, maxPoint);

            //foreach (var item in collection)
            //{
            //var leftSidePoints = new List<Point2D>();
            //var rightSidePoints = new List<Point2D>();

            //var lineDirectionVector = new Vector2(line.Slope, 1);
            //foreach (var point in points)
            //{
            //    double angle = (point - minPoint).Angle(lineDirectionVector);
            //    if (angle < Math.PI)
            //        rightSidePoints.Add(point);
            //    else
            //        leftSidePoints.Add(point);
            //}

            //// TODO: Simplify
            //Point2D maxDistancePoint;
            //double distance = 0;
            //foreach (var point in points)
            //{
            //    distance = Math.Max(distance, PointDistanceToLine(point, line));
            //}

            //maxDistancePoint = points.First(p => PointDistanceToLine(p, line) == distance);
            //var triangle = new Polygon(minPoint, maxDistancePoint, maxPoint);
            //points.RemoveAll(triangle.ContainsPoint);


            //}
            //TODO: Fix
            return null;
        }

        private static double PointDistanceToLine(Point2D point, Line2D line)
        {
            // Parameter point in 3D
            var point3D = point.Convert<Point3D>();

            // Get any point on the line to define the line 3-dimensional
            var lineOriginPoint = line.GetPoint(0);
            var line3D = new Line3D(new Point3D(lineOriginPoint.X, lineOriginPoint.Y, 0), new Vector3(1, line.Slope, 0));

            Vector3 crossProduct = Vector3.CrossProduct(line3D.Direction, (point3D - line3D.Point).PositionVector);
            return crossProduct.Magnitude / point3D.PositionVector.Magnitude;
        }
    }
}