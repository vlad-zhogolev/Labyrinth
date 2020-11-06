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

    public bool IsConnected(Vertex other)
    {
        if (Vector2Int.Distance(indices, other.indices) != 1)
        {
            return false;
        }

        var xDiff = indices.x - other.indices.x;
        var yDiff = indices.y - other.indices.y;
        Tile.Side side;
        if (xDiff == 0)
        {
            if (yDiff < 0)
            {
                side = Tile.Side.Left;
            }
            else
            {
                side = Tile.Side.Right;
            }
        }
        else if (yDiff == 0)
        {
            if (xDiff < 0)
            {
                side = Tile.Side.Down;
            }
            else
            {
                side = Tile.Side.Up;
            }
        }
        else
        {
            return false;
        }

        return tile.IsConnected(other.tile, side);
    }

    public bool Equals(Vertex other)
    {
        if (this == other)
        {
            return true;
        }

        return indices.Equals(other.indices);
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", Row, Column);
    }

    public Tile tile;

    public Vector2Int indices;

    public int Row { get {return indices.x; } set { indices.x = value; } }
    public int Column { get {return indices.y; } set { indices.y = value; } }
    public Transform TileInstance { get; set; }
}

} // namespace QuickGraphTest
