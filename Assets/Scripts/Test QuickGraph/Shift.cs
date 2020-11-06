using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickGraphTest {
public class Shift
{
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
}

} // namespace QuickGraphTest
