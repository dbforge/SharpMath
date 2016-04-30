using System;
using System.ComponentModel;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a matrix that can vary in its column and row count. 
    /// </summary>
    internal sealed class VariableMatrix : IMatrix
    {
        private readonly double[,] _fields;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VariableMatrix" /> class.
        /// </summary>
        /// <param name="rowCount">The row count of the <see cref="VariableMatrix" />.</param>
        /// <param name="columnCount">The column count of the <see cref="VariableMatrix" />.</param>
        public VariableMatrix(uint rowCount, uint columnCount)
        {
            _fields = new double[rowCount, columnCount];
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        /// <summary>
        ///     Gets or sets the field value at the specified row and column indices.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The field value at the specified row and column indices.</returns>
        public double this[uint row, uint column]
        {
            get { return _fields[row, column]; }
            set { _fields[row, column] = value; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Simple indexer is not supported in this class.", true)]
        public double this[uint index]
        {
            get
            {
                return double.NaN;
            }

            set
            {
                return;
            }
        }

        /// <summary>
        ///     Gets the column count of the <see cref="VariableMatrix" />.
        /// </summary>
        public uint ColumnCount { get; }

        /// <summary>
        ///     Gets the row count of the <see cref="VariableMatrix" />.
        /// </summary>
        public uint RowCount { get; }
    }
}