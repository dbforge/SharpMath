using System;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a four-dimensional vector with a homogeneous coordinate.
    /// </summary>
    public class Vector4 : Vector
    { 
        public static Vector4 FromVector(Vector vector)
        {
            if (vector.Dimension != 4)
                throw new ArgumentException("The dimension of the given vector is invalid. It must be 4.");
            return new Vector4(vector[0], vector[1], vector[2], vector[3]);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector4"/> class.
        /// </summary>
        public Vector4() : base(4)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector4"/> class.
        /// </summary>
        /// <param name="x">The value of the X-coordinate (X2 in mathematic coordinate systems).</param>
        /// <param name="y">The value of the Y-coordinate (X3 in mathematic coordinate systems).</param>
        /// <param name="z">The value of the Z-coordinate (X1 in mathematic coordinate systems).</param>
        /// <param name="w">The value of the homogeneous coordinate.</param>
        public Vector4(double x, double y, double z, double w)
            : base(x, y, z, w)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Vector4"/> class.
        /// </summary>
        /// <param name="vector">The existing <see cref="Vector4"/> to copy.</param>
        public Vector4(Vector4 vector)
            : base(vector)
        {
        }

        /// <summary>
        ///     Gets or sets the value of the X-coordinate (X2 in mathematic coordinate systems).
        /// </summary>
        public double X
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the Y-coordinate (X3 in mathematic coordinate systems).
        /// </summary>
        public double Y
        {
            get
            {
                return this[1];
            }
            set
            {
                this[1] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the Z-coordinate (X1 in mathematic coordinate systems).
        /// </summary>
        public double Z
        {
            get
            {
                return this[2];
            }
            set
            {
                this[2] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value of the homogenous coordinate.
        /// </summary>
        public double W
        {
            get
            {
                return this[3];
            }
            set
            {
                this[3] = value;
            }
        }

        /// <summary>
        ///     A unit <see cref="Vector4"/> with all values set to zero.
        /// </summary>
        public static Vector4 Zero => new Vector4();

        /// <summary>
        ///     A unit <see cref="Vector3"/> with all values set to one.
        /// </summary>
        public static Vector4 One => new Vector4(1, 1, 1, 1);

        /// <summary>
        ///     A unit <see cref="Vector4"/>  with coordinates 1, 0, 0, 0.
        /// </summary>
        public static Vector4 UnitX => new Vector4(1, 0, 0, 0);

        /// <summary>
        ///     A unit <see cref="Vector4"/>  with coordinates 0, 1, 0, 0.
        /// </summary>
        public static Vector4 UnitY => new Vector4(0, 1, 0, 0);

        /// <summary>
        ///     A unit <see cref="Vector4"/>  with coordinates 0, 0, 1, 0.
        /// </summary>
        public static Vector4 UnitZ => new Vector4(0, 0, 1, 0);

        /// <summary>
        ///     A unit <see cref="Vector4"/>  with coordinates 0, 0, 0, 1.
        /// </summary>
        public static Vector4 UnitW => new Vector4(0, 0, 1, 0);

        /// <summary>
        ///     Gets the LaTeX-string representing this vector graphically.
        /// </summary>
        public string LaTeXString => @"\left( \begin{array}{c} " + this[0] + @" \\ " + this[1] + @" \\ " + this[2] + @" \\ " + this[3] + @" \end{array} \right)";

        /// <summary>
        ///     Transforms the specified <see cref="Vector4"/> with the specified <see cref="Matrix4x4"/>.
        /// </summary>
        /// <param name="vector">The <see cref="Vector4"/> that should be transformed.</param>
        /// <param name="matrix">The transformation <see cref="Matrix4x4"/>.</param>
        /// <returns>The transformed <see cref="Vector4"/>.</returns>
        public static Vector4 Transform(Vector4 vector, Matrix4x4 matrix)
        {
            return matrix * vector;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Vector4 left, Vector4 right)
        {
            for (uint i = 0; i < 4; ++i)
            {
                if (left[i] != right[i])
                    return false;
            }

            return true;
        }

        public static bool operator !=(Vector4 left, Vector4 right)
        {
            for (uint i = 0; i < 4; ++i)
            {
                if (left[i] == right[i])
                    return false;
            }

            return true;
        }
    }
}