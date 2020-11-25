using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace LabyrinthGame
{
    namespace GameLogic
    {
        public class GameManager : MonoBehaviour
        {
            public async void ShiftTiles(Labyrinth.Shift shift)
            {
                await ShiftTilesAsync(shift);
            }

            async Task ShiftTilesAsync(Labyrinth.Shift shift)
            {
                m_controls.DisableInput();

                if (m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Player {1, -10} Already made shift: {2}", GetType().Name, CurrentPlayer.Color, shift);
                    m_controls.EnableInput();
                    return;
                }
                if (!m_availableShifts.Contains(shift))
                {
                    Debug.LogFormat("{0}: Player {1, -10} Attempt to make unavailable shift: {2}", GetType().Name, CurrentPlayer.Color, shift);
                    m_controls.EnableInput();
                    return;
                }

                Debug.LogFormat("{0}: Player {1, -10} Makes shift: {2}", GetType().Name, CurrentPlayer.Color, shift);

                if (m_unavailableShift != null)
                {
                    m_availableShifts.Add(m_unavailableShift);
                    m_previousUnavailableShift = m_unavailableShift;
                }
                m_availableShifts.Remove(shift);
                m_unavailableShift = shift;

                m_labyrinth.ShiftTiles(shift);
                ShiftPlayers(shift);

                await m_labyrinthView.ShiftTiles(shift);
                m_labyrinthView.ShiftPlayers(m_players);
                m_isShiftAlreadyDone = true;
                Debug.LogFormat("{0}: Player {1, -10} Shift completed", GetType().Name, CurrentPlayer.Color);

                if (m_dumpLabyrinth)
                {
                    m_labyrinth.Dump();
                }

                m_controls.EnableInput();
            }

            public async void CancelShift()
            {
                m_controls.DisableInput();

                if (!m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Player {1, -10} No shift to cancel", GetType().Name, CurrentPlayer.Color);
                    m_controls.EnableInput();
                    return;
                }

                Debug.LogFormat("{0}: Player {1, -10} Cancel shift: {2}", GetType().Name, CurrentPlayer.Color, m_unavailableShift);

                var shiftWithInversedDirection = m_unavailableShift.GetShiftWithInversedDirection();
                m_labyrinth.ShiftTiles(shiftWithInversedDirection);
                UnshiftPlayers();

                await m_labyrinthView.ShiftTiles(shiftWithInversedDirection);
                m_labyrinthView.ShiftPlayers(m_players);

                m_availableShifts.Add(m_unavailableShift);
                if (m_previousUnavailableShift != null)
                {
                    m_availableShifts.Remove(m_previousUnavailableShift);
                }
                m_unavailableShift = m_previousUnavailableShift;
                m_previousUnavailableShift = null;

                m_isShiftAlreadyDone = false;

                Debug.LogFormat("{0}: Player {1, -10} Cancel shift completed", GetType().Name, CurrentPlayer.Color);

                if (m_dumpLabyrinth)
                {
                    m_labyrinth.Dump();
                }

                m_controls.EnableInput();
            }

            public async void RotateFreeTile(Labyrinth.Tile.RotationDirection rotationDirection)
            {
                m_controls.DisableInput();

                if (m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Player {1, -10} Can not rotate free tile. Shift already made.", GetType().Name, CurrentPlayer.Color);
                    m_controls.EnableInput();
                    return;
                }

                Debug.LogFormat("{0}: Player {1, -10} Rotate free tile {2}", GetType().Name, CurrentPlayer.Color, rotationDirection);

                m_labyrinth.RotateFreeTile(rotationDirection);

                var freeTileRotation = m_labyrinth.GetFreeTileRotation();
                await m_labyrinthView.RotateFreeTile(freeTileRotation);

                if (m_dumpLabyrinth)
                {
                    m_labyrinth.Dump();
                }

                m_controls.EnableInput();
            }

            public async void MakeMove(Vector2Int position)
            {
                if (!CanMakeMove())
                {
                    return;
                }
                if (!m_labyrinth.IsReachable(position, CurrentPlayer.Position))
                {
                    Debug.LogFormat("{0}: Player {1, -10} Can not move from {2} to {3}", GetType().Name, CurrentPlayer.Color, CurrentPlayer.Position, position);
                    return;
                }

                Debug.LogFormat("{0}: Player {1, -10} Move from {2} to {3}", GetType().Name, CurrentPlayer.Color, CurrentPlayer.Position, position);

                CurrentPlayer.Position = position;
                m_labyrinthView.SetPlayerPosition(CurrentPlayer.Color, position);

                if (IsCurrentPlayerFoundItem())
                {
                    Debug.LogFormat("{0}: Player {1, -10} Found item {2}", GetType().Name, CurrentPlayer.Color, CurrentPlayer.CurrentItemToFind);
                    CurrentPlayer.SetCurrentItemFound();
                }
                if (IsCurrentPlayerFoundAllItems())
                {
                    EndGame();

                    return;
                }

                await PassTurn();
            }

            public async void SkipMove()
            {
                if (!CanMakeMove())
                {
                    return;
                }

                Debug.LogFormat("{0}: Player {1, -10} Skiping move", GetType().Name, CurrentPlayer.Color);
                await PassTurn();
            }

            void EndGame()
            {
                Debug.LogFormat("{0}: Player {1, -10} GAME OVER", GetType().Name, CurrentPlayer.Color);
            }

            bool IsCurrentPlayerFoundItem()
            {
                var itemOnTile = m_labyrinth.GetTileItem(CurrentPlayer.Position);
                return CurrentPlayer.CurrentItemToFind == itemOnTile;
            }

            bool IsCurrentPlayerFoundAllItems()
            {
                return CurrentPlayer.CurrentItemToFind == Labyrinth.Item.None;
            }

            void ShiftPlayers(Labyrinth.Shift shift)
            {
                foreach (var player in m_players)
                {
                    if (player.IsNeedShifting(shift))
                    {
                        player.Shift(shift);
                    }
                }
            }

            void UnshiftPlayers()
            {
                foreach (var player in m_players)
                {
                    if (player.IsShifted)
                    {
                        player.Unshift();
                    }
                }
            }

            void ResetShiftedForPlayers()
            {
                foreach (var player in m_players)
                {
                    player.IsShifted = false;
                }
            }

            bool CanMakeMove()
            {
                if (!m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Player {1, -10} Can not move. Shift must be made first.", GetType().Name, CurrentPlayer.Color);
                    return false;
                }

                return true;
            }

            async Task PassTurn()
            {
                SwitchToNextPlayer();
                m_isShiftAlreadyDone = false;
                ResetShiftedForPlayers();

                var shiftWithInversedDirection = m_unavailableShift.GetShiftWithInversedDirection();
                m_availableShifts.Add(m_unavailableShift);
                m_availableShifts.Remove(shiftWithInversedDirection);
                m_unavailableShift = shiftWithInversedDirection;

                UpdateCurrentPlayerInformation();

                Debug.LogFormat("{0}: Player {1, -10} It is your turn now.", GetType().Name, CurrentPlayer.Color);

                if (CurrentPlayer.Settings.IsAi)
                {
                    await MakeAiTurnAsync();
                }
            }

            void Initiallize()
            {
                foreach (var playerSettings in GameSettings.PlayersSettings)
                {
                    Debug.LogFormat("{0}: settings for {1} player: {2}", GetType().Name, playerSettings.Key, playerSettings.Value);
                }

                m_players = new List<Player>()
                {
                    new Player(Color.Yellow, GameSettings.PlayersSettings[Color.Yellow]),
                    new Player(Color.Red, GameSettings.PlayersSettings[Color.Red]),
                    new Player(Color.Blue, GameSettings.PlayersSettings[Color.Blue]),
                    new Player(Color.Green, GameSettings.PlayersSettings[Color.Green]),
                };

                m_itemsDealer = new ItemsDealer(m_itemsSeed);
                m_itemsDealer.DealItems(m_players);

                var item = m_players[0].ItemsToFind[0];
                m_players[0].ItemsToFind.Clear();
                m_players[0].ItemsToFind.Add(item);

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

                m_controls = GetComponent<Controls.TestControls>();

                m_labyrinthView = GetComponent<View.LabyrinthView>();
                (var tiles, var freeTile) = m_labyrinth.GetTiles();
                m_labyrinthView.Initialize(tiles, freeTile);

                m_currentPlayerText = GameObject.Find("Current Player Text").GetComponent<Text>();
                m_currentPlayerItemText = GameObject.Find("Current Player Item Text").GetComponent<Text>();
                UpdateCurrentPlayerInformation();
            }

            void UpdateCurrentPlayerInformation()
            {
                m_currentPlayerText.text = "Current Player: " + m_players[m_currentPlayerIndex].Color;
                m_currentPlayerItemText.text = "Current Player Item: " + m_players[m_currentPlayerIndex].CurrentItemToFind;
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

            async Task MakeAiTurnAsync()
            {
                await Task.Delay(1000);
                Debug.LogFormat("{0}: AI Player {1, -10} Makes turn", GetType().Name, CurrentPlayer.Color);

                var enumerator = m_availableShifts.GetEnumerator();
                enumerator.MoveNext();
                var shift = enumerator.Current;

                await ShiftTilesAsync(shift);
                SkipMove();
            }

            [SerializeField]
            bool m_dumpLabyrinth = true;
            [SerializeField]
            private int m_positionSeed = 4;
            [SerializeField]
            private int m_rotationSeed = 0;
            private Labyrinth.Labyrinth m_labyrinth;

            private View.LabyrinthView m_labyrinthView;

            [SerializeField]
            private int m_itemsSeed = 0;
            private ItemsDealer m_itemsDealer;         

            private IList<Player> m_players;
            private int m_currentPlayerIndex = 0;

            private ISet<Labyrinth.Shift> m_availableShifts;
            private Labyrinth.Shift m_unavailableShift;
            private Labyrinth.Shift m_previousUnavailableShift;
            private bool m_isShiftAlreadyDone = false;

            Controls.TestControls m_controls;

            Text m_currentPlayerText;
            Text m_currentPlayerItemText;
        }

    } // GameLogic

} // LabyrinthGame