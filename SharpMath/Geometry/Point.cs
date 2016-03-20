using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMath.Geometry
{
    public class Point : IEnumerable<double>, ICloneable
    {
        private readonly double[] _coordinateValues;

        public Point(uint dimension)
        {
            Dimension = dimension;
            _coordinateValues = new double[dimension];
        }

        public Point(params double[] coordinates)
        {
            Dimension = (uint)coordinates.Length;
            _coordinateValues = coordinates;
        }

        public uint Dimension { get; }

        public double this[uint index]
        {
            get
            {
                return _coordinateValues[index];
            }
            set
            {
                _coordinateValues[index] = value;
            }
        }

        /// <summary>
        ///     Gets the position <see cref="Vector"/> of this <see cref="Point"/>.
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
        ///     Converts this <see cref="Point"/> into a <see cref="Point"/> of another dimension.
        /// </summary>
        /// <typeparam name="T">The <see cref="Point"/> type that the current <see cref="Point"/> should be converted to.</typeparam>
        /// <returns>This <see cref="Point"/> converted into the given type.</returns>
        public T Convert<T>() where T : Point, new() // Type parameter because we need to create an instance of that specific type
        {
            var resultPoint = new T();
            if (resultPoint.Dimension == Dimension)
                Debug.Print(
                    $"Point conversion method (Point{Dimension}.To<T>()) is currently used to convert a point into one of the same dimension. Please check if this has been your intention.");
            for (uint i = 0; i < Math.Min(Dimension, resultPoint.Dimension); ++i)
                resultPoint[i] = this[i];
            return resultPoint;
        }

        public static Point Add(Point first, Point second)
        {
            if (first.Dimension != second.Dimension)
                throw new ArgumentException("The dimensions of the points do not equal each other.");

            var resultPoint = new Point(first.Dimension);
            for (uint i = 0; i < resultPoint.Dimension; ++i)
                resultPoint[i] = first[i] + second[i];
            return resultPoint;
        }

        public static Point Subtract(Point first, Point second)
        {
            if (first.Dimension != second.Dimension)
                throw new ArgumentException("The dimensions of the points do not equal each other.");

            var resultPoint = new Point(first.Dimension);
            for (uint i = 0; i < resultPoint.Dimension; ++i)
                resultPoint[i] = first[i] - second[i];
            return resultPoint;
        }

        public IEnumerator<double> GetEnumerator()
        {
            return new PointEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PointEnumerator(this);
        }

        public object Clone()
        {
            var clonePoint = new Point(Dimension);
            for (uint i = 0; i < Dimension; ++i)
                clonePoint[i] = this[i];
            return clonePoint;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
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
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                for (uint i = 0; i < Dimension; ++i)
                    hash = hash * 23 + this[i].GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Point"/>.</param>
        /// <param name="right">The right <see cref="Point"/>.</param>
        /// <returns>
        /// The result of the operator.
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
        /// <param name="left">The left <see cref="Point"/>.</param>
        /// <param name="right">The right <see cref="Point"/>.</param>
        /// <returns>
        /// The result of the operator.
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