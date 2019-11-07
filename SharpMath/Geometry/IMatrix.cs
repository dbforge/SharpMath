// IMatrix.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

namespace SharpMath.Geometry
{
    public interface IMatrix
    {
        uint ColumnCount { get; }
        double this[uint row, uint column] { get; set; }
        double this[uint index] { get; set; }
        uint RowCount { get; }
    }
}