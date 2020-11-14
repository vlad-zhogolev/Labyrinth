using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LabyrinthGame {

namespace Labyrinth {
public class Shift : IEquatable<Shift>
{
    public static Dictionary<Shift, Cooridinates> BorderCoordinates = new Dictionary<Shift, Cooridinates>()
    {
        {new Shift(Orientation.Horizontal, Direction.Positive, 1), new Cooridinates(new Vector2Int(1, 0), new Vector2Int(1, 6))},
        {new Shift(Orientation.Horizontal, Direction.Positive, 3), new Cooridinates(new Vector2Int(3, 0), new Vector2Int(3, 6))},
        {new Shift(Orientation.Horizontal, Direction.Positive, 5), new Cooridinates(new Vector2Int(5, 0), new Vector2Int(5, 6))},

        {new Shift(Orientation.Horizontal, Direction.Negative, 1), new Cooridinates(new Vector2Int(1, 6), new Vector2Int(1, 0))},
        {new Shift(Orientation.Horizontal, Direction.Negative, 3), new Cooridinates(new Vector2Int(3, 6), new Vector2Int(3, 0))},
        {new Shift(Orientation.Horizontal, Direction.Negative, 5), new Cooridinates(new Vector2Int(5, 6), new Vector2Int(5, 0))},

        {new Shift(Orientation.Vertical, Direction.Positive, 1),  new Cooridinates(new Vector2Int(0, 1), new Vector2Int(6, 1))},
        {new Shift(Orientation.Vertical, Direction.Positive, 3),  new Cooridinates(new Vector2Int(0, 3), new Vector2Int(6, 3))},
        {new Shift(Orientation.Vertical, Direction.Positive, 5),  new Cooridinates(new Vector2Int(0, 5), new Vector2Int(6, 5))},

        {new Shift(Orientation.Vertical, Direction.Negative, 1),  new Cooridinates(new Vector2Int(6, 1), new Vector2Int(0, 1))},
        {new Shift(Orientation.Vertical, Direction.Negative, 3),  new Cooridinates(new Vector2Int(6, 3), new Vector2Int(0, 3))},
        {new Shift(Orientation.Vertical, Direction.Negative, 5),  new Cooridinates(new Vector2Int(6, 5), new Vector2Int(0, 5))},
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

    public Shift Copy()
    {
        return new Shift(orientation, direction, index);
    }

    public Shift GetShiftWithInversedDirection()
    {
        var inverseShift = Copy();
        inverseShift.direction = inverseShift.direction == Direction.Positive ? Direction.Negative : Direction.Positive;
        return inverseShift;
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

    public override string ToString()
    {
        return string.Format("orientation:{0}, direction:{1}, line:{2}", orientation.ToString(), direction.ToString(), index);
    }
}

} // namespace QuickGraphTest

} // namespace LabyrinthGame