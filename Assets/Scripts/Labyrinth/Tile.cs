using System;
using System.Collections.Generic;
using UnityEngine;

namespace LabyrinthGame
{

    namespace Labyrinth
    {

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

            public enum RotationDirection
            {
                Clockwise,
                CounterClockwise,
            }

            public static HashSet<Side> AllSides = new HashSet<Side>{
                Side.Up,
                Side.Down,
                Side.Right,
                Side.Left
            };

            public Tile(Type type)
            {
                switch (type)
                {
                    case Type.Straight:
                    {
                        left = false;
                        right = false;
                        down = true;
                        up = true;
                    }
                    break;
                    case Type.Junction:
                    {
                        left = true;
                        right = false;
                        down = true;
                        up = true;
                    }
                    break;
                    case Type.Turn:
                    {
                        left = true;
                        right = false;
                        down = true;
                        up = false;
                    }
                    break;
                    default:
                    {
                        throw new ArgumentException("Invalid tile type.");
                    }
                }
                this.type = type;
            }

            public Tile Rotate(RotationDirection rotationDirection)
            {
                switch (rotationDirection)
                {
                    case RotationDirection.Clockwise:
                    {
                        RotateCW();
                    }
                    break;
                    case RotationDirection.CounterClockwise:
                    {
                        RotateCCW();
                    }
                    break;
                    default:
                    {
                        throw new ArgumentException("Invalid rotation");
                    }
                }
                return this;
            }

            public Tile RotateCW()
            {
                bool tmp = up;

                up = left;
                left = down;
                down = right;
                right = tmp;

                rotation += 90;

                if (rotation == 360) rotation = 0;

                return this;
            }

            public Tile RotateCCW()
            {
                bool tmp = up;

                up = right;
                right = down;
                down = left;
                left = tmp;

                rotation -= 90;

                if (rotation == -90) rotation = 270;

                return this;
            }

            public Tile Copy()
            {
                var result = new Tile(type);

                result.up = up;
                result.down = down;
                result.right = right;
                result.left = left;
                result.rotation = rotation;
                result.Item = Item;

                return result;
            }

            public Quaternion GetRotation()
            {
                return Quaternion.Euler(0, rotation, 0);
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

            public Item Item { get; set; } = Item.None;

            // These variables show if the tile has a path going in a certain direction

            public bool left;

            public bool right;

            public bool down;

            public bool up;

            int rotation = 0;
        }

    } // namespace Labirynth

} // namespace LabyrinthGame
