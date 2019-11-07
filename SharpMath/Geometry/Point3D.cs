// Point3D.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SharpMath.Geometry
{
    [Serializable]
    public struct Point3D : IEquatable<Point3D>, IEnumerable<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D" /> class.
        /// </summary>
        /// <param name="x">The value of the X-coordinate.</param>
        /// <param name="y">The value of the Y-coordinate.</param>
        /// <param name="z">The value of the Z-coordinate.</param>
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D" /> class.
        /// </summary>
        /// <param name="point">The exisiting <see cref="Point3D" /> to copy.</param>
        public Point3D(Point3D point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3D" /> class.
        /// </summary>
        /// <param name="vector">The position <see cref="Vector3" /> of the <see cref="Point3D" /> to create.</param>
        public Point3D(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
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
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 2.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 2.");
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
        ///     Gets or sets the value of the Z-coordinate.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        ///     Gets the position <see cref="Vector3" /> of this <see cref="Point3D" />.
        /// </summary>
        public Vector3 PositionVector => new Vector3(X, Y, Z);

        public IEnumerator<double> GetEnumerator()
        {
            for (uint i = 0; i < 3; i++) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (uint i = 0; i < 3; i++) yield return this[i];
        }

        public bool Equals(Point3D other)
        {
            return this == other;
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="first">The first <see cref="Point3D" />.</param>
        /// <param name="second">The second <see cref="Point3D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point3D operator +(Point3D first, Point3D second)
        {
            var resultPoint = new Point3D();
            for (uint i = 0; i < 3; ++i)
                resultPoint[i] = first[i] + second[i];
            return resultPoint;
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="first">The first <see cref="Point3D" />.</param>
        /// <param name="second">The second <see cref="Point3D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point3D operator -(Point3D first, Point3D second)
        {
            var resultPoint = new Point3D();
            for (uint i = 0; i < 3; ++i)
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
            return $"Point3D [X: {this[0]}, Y: {this[1]}, Z: {this[2]}]";
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
            if (obj is Point3D)
                return this == (Point3D) obj;
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
                var hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Point3D" />.</param>
        /// <param name="right">The right <see cref="Point3D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Point3D left, Point3D right)
        {
            return left.SequenceEqual(right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Point3D" />.</param>
        /// <param name="right">The right <see cref="Point3D" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Point3D left, Point3D right)
        {
            return !left.SequenceEqual(right);
        }
    }
}