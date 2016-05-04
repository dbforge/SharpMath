// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace SharpMath.Geometry
{
    public interface IMatrix
    {
        uint RowCount { get; }
        uint ColumnCount { get; }
        double this[uint row, uint column] { get; set; }
        double this[uint index] { get; set; }
    }
}