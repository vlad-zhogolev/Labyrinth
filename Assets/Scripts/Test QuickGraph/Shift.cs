using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QuickGraphTest {
public class Shift : IEquatable<Shift>
{
    public static Dictionary<Shift, Cooridinates> BorderCoordinates = new Dictionary<Shift, Cooridinates>()
    {
        {new Shift(Orientation.Horizontal, Direction.Positive, 1), new Cooridinates(new Vector2Int(0, 1), new Vector2Int(6, 1))},
        {new Shift(Orientation.Horizontal, Direction.Positive, 3), new Cooridinates(new Vector2Int(0, 3), new Vector2Int(6, 3))},
        {new Shift(Orientation.Horizontal, Direction.Positive, 5), new Cooridinates(new Vector2Int(0, 5), new Vector2Int(6, 5))},

        {new Shift(Orientation.Horizontal, Direction.Negative, 1), new Cooridinates(new Vector2Int(6, 1), new Vector2Int(0, 1))},
        {new Shift(Orientation.Horizontal, Direction.Negative, 3), new Cooridinates(new Vector2Int(6, 3), new Vector2Int(0, 3))},
        {new Shift(Orientation.Horizontal, Direction.Negative, 5), new Cooridinates(new Vector2Int(6, 5), new Vector2Int(0, 5))},

        {new Shift(Orientation.Vertical, Direction.Positive, 1), new Cooridinates(new Vector2Int(1, 0), new Vector2Int(1, 6))},
        {new Shift(Orientation.Vertical, Direction.Positive, 3), new Cooridinates(new Vector2Int(3, 0), new Vector2Int(3, 6))},
        {new Shift(Orientation.Vertical, Direction.Positive, 5), new Cooridinates(new Vector2Int(5, 0), new Vector2Int(5, 6))},

        {new Shift(Orientation.Vertical, Direction.Negative, 1), new Cooridinates(new Vector2Int(1, 6), new Vector2Int(1, 0))},
        {new Shift(Orientation.Vertical, Direction.Negative, 3), new Cooridinates(new Vector2Int(3, 6), new Vector2Int(3, 0))},
        {new Shift(Orientation.Vertical, Direction.Negative, 5), new Cooridinates(new Vector2Int(5, 6), new Vector2Int(5, 0))},
    };


    public struct Cooridinates
    {
        public Cooridinates(Vector2Int insert, Vector2Int remove)
        {
            this.insert = insert;
            this.remove = remove;
        }

        public Vector2Int insert;
        public Vector2Int remove;
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum Direction
    {
        Positive = 1,
        Negative = -1
    }
    
    public Shift(Orientation orientation, Direction direction, int index)
    {
        this.orientation = orientation;
        this.direction = direction;
        this.index = index;
    }

    public Orientation orientation;

    public Direction direction;

    public int index;

    public bool Equals(Shift other)
    {
        if (this == other)
        {
            return true;
        }

        return (orientation == other.orientation) && (direction == other.direction) && (index == other.index);
    }

    public override int GetHashCode()
    {
        return 31 * orientation.GetHashCode() + 7 * direction.GetHashCode() + index.GetHashCode();
    }
}

} // namespace QuickGraphTest
