using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabyrinthGame {

namespace GameLogic {

public class GameManager : MonoBehaviour
{
    public void ShiftTiles(Labyrinth.Shift shift)
    {
        if (!m_availableShifts.Contains(shift))
        {
            Debug.LogFormat("{0}: Attempt to make unavailable shift: {1}", GetType().Name, shift);
            return;
        }

        m_availableShifts.Add(m_unavailableShift);
        m_availableShifts.Remove(shift);
        m_unavailableShift = shift;

        Debug.LogFormat("{0}: Shifting tiles", GetType().Name);
        m_labyrinth.ShiftTiles(shift);
        m_labyrinthView.ShiftTiles(shift);
    }
    
    void Initiallize()
    {
        m_players = new List<Player>()
        {
            new Player(Color.Yellow, new List<Labyrinth.Item>()),
            new Player(Color.Red, new List<Labyrinth.Item>()),
            new Player(Color.Blue, new List<Labyrinth.Item>()),
            new Player(Color.Green, new List<Labyrinth.Item>()),
        };

        m_availableShifts = new HashSet<Labyrinth.Shift>()
        {
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Positive, 1),
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Positive, 3),
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Positive, 5),

            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Negative, 1),
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Negative, 3),
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Negative, 5),

            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Positive, 1),  
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Positive, 3),  
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Positive, 5),  

            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Negative, 1),  
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Negative, 3),  
            new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Negative, 5),  
        };

        m_labyrinth = new Labyrinth.Labyrinth(m_positionSeed, m_rotationSeed);
       
        m_labyrinthView = GetComponent<View.LabyrinthView>();
        (var tiles, var freeTile) = m_labyrinth.GetTiles();
        m_labyrinthView.Initialize(tiles, freeTile);
    }

    Player CurrentPlayer
    {
        get
        {
            return m_players[m_currentPlayerIndex];
        }
    }

    void SwitchToNextPlayer()
    {
        m_currentPlayerIndex = ++m_currentPlayerIndex % m_players.Count;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initiallize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Labyrinth.Labyrinth m_labyrinth;
    
    private View.LabyrinthView m_labyrinthView;

    private IList<Player> m_players;

    private int m_currentPlayerIndex = 0;

    private ISet<Labyrinth.Shift> m_availableShifts;
    private Labyrinth.Shift m_unavailableShift;

    [SerializeField]
    private int m_positionSeed = 4;

    [SerializeField]
    private int m_rotationSeed = 0;
}

} // GameLogic

} // LabyrinthGame