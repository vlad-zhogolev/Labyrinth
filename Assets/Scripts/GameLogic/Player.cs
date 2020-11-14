using System.Collections.Generic;
using System;
using UnityEngine;

namespace LabyrinthGame {

namespace GameLogic {

public enum Color
{
    Green,
    Red,
    Yellow,
    Blue
}

public class Player
{
    private static IDictionary<Color, Vector2Int> InitialPositionsForColor = new Dictionary<Color, Vector2Int>()
    {
        {Color.Yellow,  new Vector2Int(0, 0)},
        {Color.Red,     new Vector2Int(0, 6)},
        {Color.Blue,    new Vector2Int(6, 6)},
        {Color.Green,   new Vector2Int(6, 0)},
    };

    public Player(Color color, IList<Labyrinth.Item> cards)
    {
        if (cards == null)
        {
            throw new NullReferenceException("Cards can not be null");
        }
        Cards = cards;
        Color = color;
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
    public Color Color { get; set; }
    public IList<Labyrinth.Item> Cards { get; set; } = new List<Labyrinth.Item>();

    private Vector2Int m_position;
}

} // namespace GameLogic

} // namespace LabyrinthGame