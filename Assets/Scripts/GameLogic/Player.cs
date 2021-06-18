using System.Collections.Generic;
using System;
using UnityEngine;

namespace LabyrinthGame
{

    namespace GameLogic
    {

        public enum Color
        {
            Green,
            Red,
            Yellow,
            Blue
        }

        public class Player
        {
            public static IDictionary<Color, Vector2Int> InitialPositionsForColor = new Dictionary<Color, Vector2Int>()
            {
                {Color.Yellow,  new Vector2Int(0, 0)},
                {Color.Red,     new Vector2Int(0, 6)},
                {Color.Blue,    new Vector2Int(6, 6)},
                {Color.Green,   new Vector2Int(6, 0)},
            };

            public Player(Color color, PlayerSettings playerSettings)
            {
                Color = color;
                Settings = playerSettings;
                if (!InitialPositionsForColor.TryGetValue(color, out m_position))
                {
                    throw new ArgumentException("Can not provide initial position for specified color");
                }
            }

            public Vector2Int Position
            {
                get { return m_position; }
                set
                {
                    m_position = value;
                }
            }

            public bool IsNeedShifting(Labyrinth.Shift shift)
            {
                switch (shift.orientation)
                {
                    case Labyrinth.Shift.Orientation.Horizontal:
                    {
                        return Position.x == shift.index;
                    }
                    case Labyrinth.Shift.Orientation.Vertical:
                    {
                        return Position.y == shift.index;
                    }
                    default:
                    {
                        return false; // do nothing
                    }
                }
            }

            public void Shift(Labyrinth.Shift shift)
            {
                if (IsShifted)
                {
                    throw new Exception("Player already shifted");
                }

                IsShifted = true;
                m_positionBeforeShift = m_position;

                var borderCoordinates = Labyrinth.Shift.BorderCoordinates[shift];
                if (m_position == borderCoordinates.remove)
                {
                    m_position = borderCoordinates.insert;
                    return;
                }

                switch (shift.orientation)
                {
                    case Labyrinth.Shift.Orientation.Horizontal:
                    {
                        m_position = new Vector2Int(m_position.x, m_position.y + (int)shift.direction);
                    }
                    break;
                    case Labyrinth.Shift.Orientation.Vertical:
                    {
                        m_position = new Vector2Int(m_position.x + (int)shift.direction, m_position.y);
                    }
                    break;
                    default:
                    {
                        return; //do nothing
                    }
                }
            }

            public void Unshift()
            {
                if (!IsShifted)
                {
                    throw new Exception("Player not shifted yet");
                }
                m_position = m_positionBeforeShift;
                IsShifted = false;
            }

            public void SetCurrentItemFound()
            {
                ++m_currentItemIndex;
            }

            public int GetItemsLeftCount()
            {
                var numberOfItems = ItemsToFind.Count - 1 - m_currentItemIndex;
                return numberOfItems >= 0 ? numberOfItems : 0;
            }

            public Color Color { get; set; }
            public PlayerSettings Settings{ get; set;}

            public IList<Labyrinth.Item> ItemsToFind { get; set; } = new List<Labyrinth.Item>();
            public Labyrinth.Item CurrentItemToFind 
            { 
                get 
                { 
                    return m_currentItemIndex < ItemsToFind.Count ? ItemsToFind[m_currentItemIndex] : Labyrinth.Item.None;
                } 
            }

            private int m_currentItemIndex = 0;

            public bool IsShifted { get; set; } = false;
            private Vector2Int m_position;
            private Vector2Int m_positionBeforeShift;
        }

    } // namespace GameLogic

} // namespace LabyrinthGame