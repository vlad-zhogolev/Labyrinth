using UnityEngine;
using System;

namespace Labyrinth {

class Vertex : IEquatable<Vertex>, IComparable<Vertex>
{
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

    public override int GetHashCode()
    {
        return indices.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", Row, Column);
    }

    public int CompareTo(Vertex other)
    {
        if (this == other)
        {
            return 0;
        }

        if (indices.x == other.indices.x && indices.y == other.indices.y)
        {
            return 0;
        }

        if (indices.x < other.indices.x)
        {
            return -1;
        }
        else if (indices.x == other.indices.x)
        {
            if (indices.y < other.indices.y)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            return 1;
        }
    }

    public Tile tile;

    public Vector2Int indices;

    public int Row { get {return indices.x; } set { indices.x = value; } }
    public int Column { get {return indices.y; } set { indices.y = value; } }
    public Transform TileInstance { get; set; }
}

} // namespace QuickGraphTest
