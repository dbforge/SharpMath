// Author: Dominic Beger (Trade/ProgTrade) 2016
// Improvements: Stefan Baumann 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SharpMath.Geometry.Exceptions;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a point.
    /// </summary>
    public class Point : IEnumerable<double>, IEquatable<Point>
    {
        private readonly double[] _coordinateValues;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point" /> class.
        /// </summary>
        /// <param name="dimension">The dimension of the <see cref="Point" />.</param>
        public Point(uint dimension)
        {
            Dimension = dimension;
            _coordinateValues = new double[dimension];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point" /> class.
        /// </summary>
        /// <param name="coordinates">The coordinates of the <see cref="Point" />.</param>
        public Point(params double[] coordinates)
        {
            Dimension = (uint) coordinates.Length;
            _coordinateValues = coordinates;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point" /> class.
        /// </summary>
        /// <param name="point">The exisiting <see cref="Point" /> to copy.</param>
        public Point(Point point)
        {
            Dimension = point.Dimension;
            _coordinateValues = new double[Dimension];
            for (uint i = 0; i < point.Dimension; ++i)
                this[i] = point[i];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point" /> class.
        /// </summary>
        /// <param name="vector">The position <see cref="Vector" /> of the <see cref="Point" /> to create.</param>
        public Point(Vector vector)
        {
            Dimension = vector.Dimension;
            _coordinateValues = new double[Dimension];
            for (uint i = 0; i < vector.Dimension; ++i)
                this[i] = vector[i];
        }

        /// <summary>
        ///     Gets the dimension of this <see cref="Point" />.
        /// </summary>
        public uint Dimension { get; }

        /// <summary>
        ///     Gets or sets the value of the coordinate at the specified index.
        /// </summary>
        /// <param name="index">The index to use.</param>
        /// <returns>Returns the value of the coordinate.</returns>
        public double this[uint index]
        {
            get { return _coordinateValues[index]; }
            set { _coordinateValues[index] = value; }
        }

        /// <summary>
        ///     Gets the position <see cref="Vector" /> of this <see cref="Point" />.
        /// </summary>
        public Vector PositionVector
        {
            get
            {
                var resultVector = new Vector(Dimension);
                for (uint i = 0; i < Dimension; ++i)
                    resultVector[i] = this[i];
                return resultVector;
            }
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection of coordinates.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<double> GetEnumerator()
        {
            for (int i = 0; i < this.Dimension; i++)
            {
                yield return this[(uint)i];
            }
            yield break;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection of coordinates.
        /// </summary>
        /// <returns>
        ///     An enumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.Dimension; i++)
            {
                yield return this[(uint)i];
            }
            yield break;
        }

        public bool Equals(Point other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return this == other;
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clonePoint = new Point(Dimension);
            for (uint i = 0; i < Dimension; ++i)
                clonePoint[i] = this[i];
            return clonePoint;
        }

        /// <summary>
        ///     Converts this <see cref="Point" /> into a <see cref="Point" /> of another dimension.
        /// </summary>
        /// <typeparam name="T">The <see cref="Point" /> type that the current <see cref="Point" /> should be converted to.</typeparam>
        /// <returns>This <see cref="Point" /> converted into the given type.</returns>
        public T Convert<T>() where T : Point, new()
            // Type parameter because we need to create an instance of that specific type
        {
            var resultPoint = new T();
            if (resultPoint.Dimension == Dimension)
                Debug.Print(
                    $"Point conversion method (Point{Dimension}.To<T>()) is currently used to convert a point into one of the same dimension. Please check if this has been your intention.");
            for (uint i = 0; i < Math.Min(Dimension, resultPoint.Dimension); ++i)
                resultPoint[i] = this[i];
            return resultPoint;
        }

        /// <summary>
        ///     Adds two <see cref="Point" /> instances.
        /// </summary>
        /// <param name="first">The first <see cref="Point" />.</param>
        /// <param name="second">The second <see cref="Point" />.</param>
        /// <returns>The resulting <see cref="Point" />.</returns>
        public static Point Add(Point first, Point second)
        {
            if (first.Dimension != second.Dimension)
                throw new DimensionException("The dimensions of the points do not equal each other.");

            var resultPoint = new Point(first.Dimension);
            for (uint i = 0; i < resultPoint.Dimension; ++i)
                resultPoint[i] = first[i] + second[i];
            return resultPoint;
        }

        /// <summary>
        ///     Subtracts two <see cref="Point" /> instances.
        /// </summary>
        /// <param name="first">The first <see cref="Point" />.</param>
        /// <param name="second">The second <see cref="Point" />.</param>
        /// <returns>The resulting <see cref="Point" />.</returns>
        public static Point Subtract(Point first, Point second)
        {
            if (first.Dimension != second.Dimension)
                throw new DimensionException("The dimensions of the points do not equal each other.");

            var resultPoint = new Point(first.Dimension);
            for (uint i = 0; i < resultPoint.Dimension; ++i)
                resultPoint[i] = first[i] - second[i];
            return resultPoint;
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first <see cref="Point" />.</param>
        /// <param name="second">The second <see cref="Point" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point operator +(Point first, Point second)
        {
            return Add(first, second);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first <see cref="Point" />.</param>
        /// <param name="second">The second <see cref="Point" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point operator -(Point first, Point second)
        {
            return Subtract(first, second);
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public T Clone<T>() where T : Point, new()
        {
            var clonePoint = new T();
            for (uint i = 0; i < Dimension; ++i)
                clonePoint[i] = this[i];
            return clonePoint;
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
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return this == obj as Point;
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
                for (uint i = 0; i < Dimension; ++i)
                    hash = hash*23 + this[i].GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Point" />.</param>
        /// <param name="right">The right <see cref="Point" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Point left, Point right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            if (left.Dimension != right.Dimension)
                return false;

            for (uint i = 0; i < left.Dimension; ++i)
            {
                if (!FloatingNumber.AreApproximatelyEqual(left[i], right[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Point" />.</param>
        /// <param name="right">The right <see cref="Point" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Point left, Point right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return ReferenceEquals(left, right);

            if (left.Dimension != right.Dimension)
                return true;

            for (uint i = 0; i < left.Dimension; ++i)
            {
                if (!FloatingNumber.AreApproximatelyEqual(left[i], right[i]))
                    return true;
            }

            return false;
        }
    }
}