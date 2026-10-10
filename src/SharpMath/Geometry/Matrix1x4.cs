// Matrix1x4.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 1x4 matrix for representing four-dimensional vectors horizontally.
    /// </summary>
    [Serializable]
    public struct Matrix1x4 : IEnumerable<double>, IMatrix
    {
        /// <summary>
        ///     Gets the value at row 1 and column 1.
        /// </summary>
        public double M11 { get; set; }

        /// <summary>
        ///     Gets the value at row 1 and column 2.
        /// </summary>
        public double M12 { get; set; }

        /// <summary>
        ///     Gets the value at row 1 and column 3.
        /// </summary>
        public double M13 { get; set; }

        /// <summary>
        ///     Gets the value at row 1 and column 4.
        /// </summary>
        public double M14 { get; set; }

        public IEnumerator<double> GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        /// <summary>
        ///     Gets or sets the value at the specified index. The values are accessed as: (row +) column.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value at the specified index.</returns>
        public double this[uint index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return M11;
                    case 1:
                        return M12;
                    case 2:
                        return M13;
                    case 3:
                        return M14;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 3.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        M11 = value;
                        break;
                    case 1:
                        M12 = value;
                        break;
                    case 2:
                        M13 = value;
                        break;
                    case 3:
                        M14 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 3.");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value at the specified row and column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>The value at the specified row and column.</returns>
        public double this[uint row, uint column]
        {
            get => this[row + column];

            set => this[row + column] = value;
        }

        /// <summary>
        ///     Gets the column count of the <see cref="Matrix4x1" />.
        /// </summary>
        public uint ColumnCount => 4;

        /// <summary>
        ///     Gets the row count of the <see cref="Matrix4x1" />.
        /// </summary>
        public uint RowCount => 1;
    }
}