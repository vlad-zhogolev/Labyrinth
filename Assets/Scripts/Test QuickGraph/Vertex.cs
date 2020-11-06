using UnityEngine;
using System;

namespace QuickGraphTest {

public class Vertex : IEquatable<Vertex>
{
    public Vertex()
    {

    }

    public Vertex(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public Tile tile;
    public int Row { get; set; }

    public int Column { get; set; }
    public Transform TileInstance { get; set; }

    public bool Equals(Vertex other)
    {
        if (this == other)
        {
            return true;
        }

        return (Row == other.Row) && (Column == other.Column);
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", Row, Column);
    }
}

} // namespace QuickGraphTest
