// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a two-dimensional point.
    /// </summary>
    public struct Point2D : IEquatable<Point2D>, IEnumerable<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Point2D" /> class.
        /// </summary>
        /// <param name="x">The X-coordinate.</param>
        /// <param name="y">The Y-coordinate.</param>
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point2D" /> class.
        /// </summary>
        /// <param name="point">The exisiting <see cref="Point2D" /> to copy.</param>
        public Point2D(Point2D point)
        {
            X = point.X;
            Y = point.Y;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point2D" /> class.
        /// </summary>
        /// <param name="vector">The position <see cref="Vector2" /> of the <see cref="Point2D" /> to create.</param>
        public Point2D(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        ///     Gets or sets the value of the coordinate at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value of the coordinate at the specified index.</returns>
        public double this[uint index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    default: throw new IndexOutOfRangeException("The index must be between 0 and 1.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default: throw new IndexOutOfRangeException("The index must be between 0 and 1.");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value of the X-coordinate.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     Gets or sets the value of the Y-coordinate.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        ///     Gets the position <see cref="Vector2" /> of this <see cref="Point2D" />.
        /// </summary>
        public Vector2 PositionVector => new Vector2(X, Y);

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first <see cref="Point2D" />.</param>
        /// <param name="second">The second <see cref="Point2D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point2D operator +(Point2D first, Point2D second)
        {
            var resultPoint = new Point2D();
            for (uint i = 0; i < 2; ++i)
                resultPoint[i] = first[i] + second[i];
            return resultPoint;
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first <see cref="Point2D" />.</param>
        /// <param name="second">The second <see cref="Point2D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point2D operator -(Point2D first, Point2D second)
        {
            var resultPoint = new Point2D();
            for (uint i = 0; i < 2; ++i)
                resultPoint[i] = first[i] - second[i];
            return resultPoint;
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Point2D [X: {this[0]}, Y: {this[1]}]";
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof (Point2D))
                return this == (Point2D) obj;
            return false;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash*23 + X.GetHashCode();
                hash = hash*23 + Y.GetHashCode();
                return hash;
            }
        }

        public bool Equals(Point2D other)
        {
            return this == other;
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (uint i = 0; i < 2; i++)
            {
                yield return this[i];
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (uint i = 0; i < 2; i++)
            {
                yield return this[i];
            }
            yield break;
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Point2D" />.</param>
        /// <param name="right">The right <see cref="Point2D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.SequenceEqual(right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Point2D" />.</param>
        /// <param name="right">The right <see cref="Point2D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Point2D left, Point2D right)
        {
            return !left.SequenceEqual(right);
        }
    }
}