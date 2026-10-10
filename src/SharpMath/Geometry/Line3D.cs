// Line3D.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 3D-line.
    /// </summary>
    [Serializable]
    public struct Line3D : IEquatable<Line3D>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Line3D" /> class.
        /// </summary>
        /// <param name="point">A point on the <see cref="Line3D" />.</param>
        /// <param name="direction">The direction <see cref="Vector3" />.</param>
        public Line3D(Point3D point, Vector3 direction)
        {
            Point = point;
            if (direction == Vector3.Zero)
                throw new InvalidOperationException("The direction vector of the line must not be the zero vector.");
            Direction = direction;
        }

        /// <summary>
        ///     Gets or sets a <see cref="Vector3" /> that represents the direction of the <see cref="Line3D" />.
        /// </summary>
        public Vector3 Direction { get; }

        /// <summary>
        ///     Gets or sets a point on the <see cref="Line3D" /> that the line intersects with.
        /// </summary>
        public Point3D Point { get; }

        /// <summary>
        ///     Determines whether the specified <see cref="Line3D" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Line3D" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Line3D" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Line3D other)
        {
            return ReferenceEquals(other, null)
                ? ReferenceEquals(this, null)
                : other.Direction == Direction && other.Point == Point;
        }

        /// <summary>
        ///     Creates a <see cref="Line3D" /> from two <see cref="Point3D" /> instances.
        /// </summary>
        /// <param name="firstPoint">The first <see cref="Point3D" />.</param>
        /// <param name="secondPoint">The second <see cref="Point3D" />.</param>
        /// <returns>The created <see cref="Line3D" />.</returns>
        public static Line3D FromPoints(Point3D firstPoint, Point3D secondPoint)
        {
            return new Line3D(firstPoint, (secondPoint - firstPoint).PositionVector);
        }

        /// <summary>
        ///     Gets a <see cref="Point3D" /> on the <see cref="Line3D" />.
        /// </summary>
        /// <param name="lambda">The scalar that the direction vector should be multiplied with.</param>
        /// <returns>The relating <see cref="Point3D" /> on the <see cref="Line3D" />.</returns>
        public Point3D GetPoint(double lambda)
        {
            var resultVector = Point.PositionVector + Direction * lambda;
            return new Point3D(resultVector.X, resultVector.Y, resultVector.Z);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Point3D" /> is on the <see cref="Line3D" />, or not.
        /// </summary>
        /// <param name="point">The <see cref="Point3D" />.</param>
        /// <returns><c>true</c>, if the <see cref="Point3D" /> is on the <see cref="Line3D" />; otherwise <c>false</c>.</returns>
        public bool IsPointOnLine(Point3D point)
        {
            double lambda = 0;
            for (uint i = 0; i < 3; ++i)
            {
                var value = (point[i] - Point[i]) / Direction[i];
                if (i != 0 && !FloatingNumber.AreApproximatelyEqual(value, lambda))
                    return false;
                lambda = value;
            }

            return true;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(obj, null)
                ? ReferenceEquals(this, null)
                : obj is Line3D && ((Line3D) obj).Direction == Direction && ((Line3D) obj).Point == Point;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Direction.GetHashCode() ^ Point.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Line3D [Direction={0}; Point={1}]", Direction, Point);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="Line3D" /> instances are equal to each other.
        /// </summary>
        /// <param name="left">The first <see cref="Line3D" />.</param>
        /// <param name="right">The <see cref="Line3D" /> to compare with the other <see cref="Line3D" />.</param>
        /// <returns>
        ///     <c>true</c> if the two specified <see cref="Line3D" /> are equal to each other; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Line3D left, Line3D right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="Line3D" /> instances are not equal to each other.
        /// </summary>
        /// <param name="left">The first <see cref="Line3D" />.</param>
        /// <param name="right">The <see cref="Line3D" /> to compare with the other <see cref="Line3D" />.</param>
        /// <returns>
        ///     <c>true</c> if the two specified <see cref="Line3D" /> are not equal to each other; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Line3D left, Line3D right)
        {
            return !left.Equals(right);
        }
    }
}