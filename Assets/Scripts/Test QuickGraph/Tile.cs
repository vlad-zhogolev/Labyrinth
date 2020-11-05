using System;
using UnityEngine;

namespace QuickGraphTest {

public class Tile
{
    public enum Type
    {
        Straight,
        Junction,
        Turn
    }

    public enum Side
    {
        Up,
        Down,
        Right,
        Left
    }

    private static Tile[] CreateAvailableTiles()
    {
        Tile[] tiles = new Tile[Labyrinth.MovableTilesNumber];
        var endIndex = Labyrinth.MovableStraightTilesNumber;
        for (var i = 0; i < endIndex; ++i)
        {
            tiles[i] = new Tile(Type.Straight);
        }
        var startIndex = endIndex;
        endIndex += Labyrinth.MovableTurnTilesNumber;
        for (var i = startIndex; i < endIndex; ++i)
        {
            tiles[i] = new Tile(Type.Turn);
        }
        startIndex = endIndex;
        endIndex += Labyrinth.MovableJunctionTilesNumber;
        for (var i = startIndex; i < endIndex; ++i)
        {
            tiles[i] = new Tile(Type.Junction);
        }
        
        return tiles;
    }
    public static readonly Tile[] MovableTiles = CreateAvailableTiles();
    

    public Tile(Type type)
    {
        switch (type)
        {
            case Type.Straight:
            {
                left = true;
                right = true;
                down = false;
                up = false;
            }
            break;
            case Type.Junction:
            {
                left = true;
                right = true;
                down = false;
                up = true;
            }
            break;
            case Type.Turn:
            {
                left = true;
                right = false;
                down = false;
                up = true;
            }
            break;
            default:
            {
                throw new ArgumentException("Invalid tile type.");
            }
        }
        this.type = type;
    }

    public Tile RotateCW()
    {
        bool tmp = up;

        up = left;
        left = down;
        down = right;
        right = tmp;

        return this;
    }

    public Tile RotateCCW()
    {
        bool tmp = up;

        up = right;
        right = down;
        down = left;
        left = tmp;

        return this;
    }

    public Tile Copy()
    {
        var result = new Tile(type);

        result.up = up;
        result.down = down;
        result.right = right;
        result.left = left;

        return result;
    }

    private Quaternion GetRotationForStraight()
    {
        if (up)
        {
            return Quaternion.Euler(0, 90, 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    private Quaternion GetRotationForTurn()
    {
        if (up)
        {
            if (left)
            {
                return Quaternion.identity;
            }
            else
            {
                return Quaternion.Euler(0, 90, 0);
            }
        }
        else
        {
            if (right)
            {
                return Quaternion.Euler(0, 180, 0);
            }
            else
            {
                return Quaternion.Euler(0, 270, 0);
            }
        }
    }

    private Quaternion GetRotationForJunction()
    {
        if (!down)
        {
            return Quaternion.identity;
        }
        else if (!left)
        {
            return Quaternion.Euler(0, 90, 0);
        }
        else if (!up)
        {
            return Quaternion.Euler(0, 180, 0);
        }
        else
        {
            return Quaternion.Euler(0, 270, 0);
        }
    }

    public Quaternion GetRotation()
    {
        switch (type)
        {
            case Type.Straight:
            {
                return GetRotationForStraight();
            }
            case Type.Turn:
            {
                return GetRotationForTurn();
            }
            case Type.Junction:
            {
                return GetRotationForJunction();
            }
            default:
            {
                throw new ArgumentException("Invalid tile type");
            }
        }
    }

    public bool IsConnected(Tile other, Side side)
    {
        switch (side)
        {
            case Side.Up:
            {
                return up && other.down;
            }
            case Side.Down:
            {
                return down && other.up;
            }
            case Side.Right:
            {
                return right && other.left;
            }
            case Side.Left:
            {
                return left && other.right;
            }
            default:
            {
                throw new ArgumentException("Invalid side type");
            }
        }
    }

    public Type type;

    // These variables show if the tile has a path going in a certain direction

    public bool left;

    public bool right;

    public bool down;

    public bool up;

    //Also information about connections with adjacent tiles can be added
}

} // namespace labirynth
