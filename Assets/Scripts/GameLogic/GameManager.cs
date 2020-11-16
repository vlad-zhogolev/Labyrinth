using System;
using System.Collections.Generic;
using UnityEngine;

namespace LabyrinthGame
{
    namespace GameLogic
    {
        public class GameManager : MonoBehaviour
        {
            public void ShiftTiles(Labyrinth.Shift shift)
            {
                if (m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Current player already made shift: {1}", GetType().Name, m_unavailableShift);
                    return;
                }
                if (!m_availableShifts.Contains(shift))
                {
                    Debug.LogFormat("{0}: Attempt to make unavailable shift: {1}", GetType().Name, shift);
                    return;
                }

                Debug.LogFormat("{0}: Moving tiles for shift: {1}", GetType().Name, shift);

                if (m_unavailableShift != null)
                {
                    m_availableShifts.Add(m_unavailableShift);
                    m_previousUnavailableShift = m_unavailableShift;
                }
                m_availableShifts.Remove(shift);
                m_unavailableShift = shift;

                m_labyrinth.ShiftTiles(shift);
                m_labyrinthView.ShiftTiles(shift);
                m_isShiftAlreadyDone = true;

                m_labyrinth.Dump();
            }

            public void CancelShift()
            {
                if (!m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: No shift to cancel", GetType().Name);
                    return;
                }

                Debug.LogFormat("{0}: Cancel shift: {1}", GetType().Name, m_unavailableShift);

                var shiftWithInversedDirection = m_unavailableShift.GetShiftWithInversedDirection();
                m_labyrinth.ShiftTiles(shiftWithInversedDirection);
                m_labyrinthView.ShiftTiles(shiftWithInversedDirection);

                m_availableShifts.Add(m_unavailableShift);
                if (m_previousUnavailableShift != null)
                {
                    m_availableShifts.Remove(m_previousUnavailableShift);
                }

                m_unavailableShift = m_previousUnavailableShift;
                m_previousUnavailableShift = null;

                m_isShiftAlreadyDone = false;

                m_labyrinth.Dump();
            }

            public void RotateFreeTile(Labyrinth.Tile.RotationDirection rotationDirection)
            {
                if (m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Can not rotate free tile. Shift already made.", GetType().Name);
                    return;
                }

                Debug.LogFormat("{0}: Rotate free tile {1}", GetType().Name, rotationDirection);

                m_labyrinth.RotateFreeTile(rotationDirection);

                var freeTileRotation = m_labyrinth.GetFreeTileRotation();
                m_labyrinthView.RotateFreeTile(freeTileRotation);

                m_labyrinth.Dump();
            }

            public void MakeMove(Vector2Int position)
            {
                if (!m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Can not move player. Shift must be made first.", GetType().Name, CurrentPlayer.Position, position);
                    return;
                }
                if (!m_labyrinth.IsReachable(position, CurrentPlayer.Position))
                {
                    Debug.LogFormat("{0}: Can not move player from {1} to {2}", GetType().Name, CurrentPlayer.Position, position);
                    return;
                }

                Debug.LogFormat("{0}: Move player from {1} to {2}", GetType().Name, CurrentPlayer.Position, position);

                CurrentPlayer.Position = position;
                m_labyrinthView.SetPlayerPosition(CurrentPlayer.Color, position);

                PassTurn();
            }

            void PassTurn()
            {
                SwitchToNextPlayer();
                m_isShiftAlreadyDone = false;

                var shiftWithInversedDirection = m_unavailableShift.GetShiftWithInversedDirection();
                m_availableShifts.Add(m_unavailableShift);
                m_availableShifts.Remove(shiftWithInversedDirection);
                m_unavailableShift = shiftWithInversedDirection;
            }

            void Initiallize()
            {
                m_players = new List<Player>()
                {
                    new Player(Color.Yellow,    new List<Labyrinth.Item>()),
                    new Player(Color.Red,       new List<Labyrinth.Item>()),
                    new Player(Color.Blue,      new List<Labyrinth.Item>()),
                    new Player(Color.Green,     new List<Labyrinth.Item>()),
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
            private Labyrinth.Shift m_previousUnavailableShift;
            private bool m_isShiftAlreadyDone = false;

            [SerializeField]
            private int m_positionSeed = 4;

            [SerializeField]
            private int m_rotationSeed = 0;
        }

    } // GameLogic

} // LabyrinthGame