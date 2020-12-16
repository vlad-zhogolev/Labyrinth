using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace LabyrinthGame
{
    namespace GameLogic
    {
        public class OnlineGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
        {
            public const byte InitializeEventCode = 199;
            public const byte InitializationCompleteEventCode = 198;
            public const byte MakeTurnEventCode = 197;
            public const byte SynchronizeGameStateEventCode = 196;
            public const byte GameStateSynchronizedEventCode = 195;

            public void InitializeGame()
            {
                var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(InitializeEventCode, null, raiseEventOptions, SendOptions.SendReliable);
                Debug.LogFormat("{0}: Send game settings received approve", GetType().Name);
            }

            public void OnEvent(EventData photonEvent)
            {
                byte eventCode = photonEvent.Code;
                if (eventCode == InitializeEventCode)
                {
                    Debug.LogFormat("{0}: Received InitializeEvent", GetType().Name);

                    Initialize();

                    var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                    PhotonNetwork.RaiseEvent(InitializationCompleteEventCode, null, raiseEventOptions, SendOptions.SendReliable);
                }
                if (eventCode == InitializationCompleteEventCode)
                {
                    Debug.LogFormat("{0}: Received InitializationCompleteEvent", GetType().Name);

                    ++PlayersInitializedCounter;
                    if (PlayersInitializedCounter == PhotonNetwork.PlayerList.Length)
                    {
                        Debug.LogFormat("{0}: Everyone initialized game, number of players: {1}", GetType().Name, PlayersInitializedCounter);
                        SendMakeTurnEvent(true, CurrentPlayer.Settings.ActorId, CurrentPlayer.Settings.IsAi);
                    }
                }
                if (eventCode == MakeTurnEventCode)
                {
                    Debug.LogFormat("{0}: Received MakeTurnEvent", GetType().Name);

                    HandleMakeTurnEvent(photonEvent);
                }
                if (eventCode == SynchronizeGameStateEventCode)
                {
                    Debug.LogFormat("{0}: Received SynchronizeGameStateEvent", GetType().Name);
                    HandleSyncronizeGameState(photonEvent);
                }
                if (eventCode == GameStateSynchronizedEventCode)
                {
                    Debug.LogFormat("{0}: Received GameStateSynchronizedEventCode", GetType().Name);

                    ++PlayersSynchronizedGameStateCounter;
                    if (PlayersSynchronizedGameStateCounter == PhotonNetwork.PlayerList.Length)
                    {
                        Debug.LogFormat("{0}: Everyone synchronized game, number of players: {1}", GetType().Name, PlayersSynchronizedGameStateCounter);
                        PlayersSynchronizedGameStateCounter = 0;

                        Debug.LogFormat("{0}: Making turn for player: {1}, IsAi: {2}", GetType().Name, NextPlayer.Color, NextPlayer.Settings.IsAi);
                        SendMakeTurnEvent(false, NextPlayer.Settings.ActorId, NextPlayer.Settings.IsAi);
                    }
                }
            }

            void SendMakeTurnEvent(bool isFirst, int actorId, bool isAi)
            {
                Debug.LogFormat("{0}: Send MakeTurnEvent", GetType().Name);
                var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                var data = new object[] { isFirst, actorId, isAi };
                PhotonNetwork.RaiseEvent(MakeTurnEventCode, data, raiseEventOptions, SendOptions.SendReliable);
            }

            void HandleMakeTurnEvent(EventData photonEvent)
            {
                Debug.LogFormat("{0}: Handling MakeTurnEvent", GetType().Name);

                var data = (object[])photonEvent.CustomData;
                var isFirst = (bool)data[0];
                var actorId = (int)data[1];
                var isAi = (bool)data[2];

                if (!isFirst)
                {
                    SwitchToNextPlayer();
                }

                Debug.LogFormat("{0}: Current player - {1}", GetType().Name, CurrentPlayer.Color);

                if (actorId < 0 && PhotonNetwork.LocalPlayer.ActorNumber == photonEvent.Sender)
                {
                    Debug.LogFormat("{0}: Ai player makes turn {1}", GetType().Name, CurrentPlayer.Color);
                    MakeAiTurnAsync();
                }
                else if (actorId == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    Debug.LogFormat("{0}: This player makes turn", GetType().Name);
                    m_controls.InputEnabled = true;
                }
                
            }

            void SendSyncronizeGameState(bool isPlayerFoundItem)
            {
                var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                var shiftIndex = ShiftIndex.Inverse(m_shiftIndex); // m_shiftIndex contains index of inverse shift after ShiftTiles(). Maybe need to split it to two fields for simplicity?

                var coords = Labyrinth.Shift.BorderCoordinates[m_unavailableShift].insert; // note: at this point we hadn't run PassTurn 
                var directions = m_labyrinth.GetTileDirections(coords);


                var data = new object[] { shiftIndex, CurrentPlayer.Position.x, CurrentPlayer.Position.y, CurrentPlayer.Settings.ActorId, directions, isPlayerFoundItem };
                
                Debug.LogFormat("{0}: Send SyncronizeGameState, shiftIndex = {1}, CurrentPlayer position = {2}", GetType().Name, shiftIndex, CurrentPlayer.Position);
                PhotonNetwork.RaiseEvent(SynchronizeGameStateEventCode, data, raiseEventOptions, SendOptions.SendReliable);
            }

            async void HandleSyncronizeGameState(EventData photonEvent)
            {
                Debug.LogFormat("{0}: Handling SyncronizeGameState from actorId = {1}", GetType().Name, photonEvent.Sender);
                var data = (object[])photonEvent.CustomData;

                if (photonEvent.Sender != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    Debug.LogFormat("{0}: Syncronizing state", GetType().Name);
                   
                    var shiftIndex = (int)data[0];
                    var x = (int)data[1];
                    var y = (int)data[2];
                    var freeTileRotation = (bool[])data[4];
                    var isPlayerFoundItem = (bool)data[5];

                    await MoveFreeTileAsync(shiftIndex);
                    await SynchornizeTileRotation(freeTileRotation);
                    await SynchronizeShiftTiles(shiftIndex);
                    await SynchronizeMakeMove(new Vector2Int(x, y));
                    if (isPlayerFoundItem)
                    {
                        Debug.LogFormat("{0}: Syncronize found item for player {1}", GetType().Name, CurrentPlayer.Color);
                        CurrentPlayer.SetCurrentItemFound();
                    }
                }

                Debug.LogFormat("{0}: Send GameStateSynchronizedEventCode", GetType().Name, photonEvent.Sender);
                var raiseEventOptions = new RaiseEventOptions { TargetActors = new int[] { photonEvent.Sender } };
                PhotonNetwork.RaiseEvent(GameStateSynchronizedEventCode, null, raiseEventOptions, SendOptions.SendReliable);
            }

            private void OnEnable()
            {
                PhotonNetwork.AddCallbackTarget(this);
            }

            private void OnDisable()
            {
                PhotonNetwork.RemoveCallbackTarget(this);
            }

            public async Task SynchornizeTileRotation(bool[] directions)
            {
                Debug.LogFormat("{0}: Synchronize rotation of free tile", GetType().Name);

                var up = directions[0];
                var down = directions[1];
                var right = directions[2];
                var left = directions[3];

                var tile = m_labyrinth.GetFreeTile();
                Debug.LogFormat("{0}: Tile: {1}, need up: {2}, down: {3}, left: {4}, right: {5} ", GetType().Name, tile, up, down, left, right);

                while (!(tile.up == up && tile.down == down && tile.right == right && tile.left == left))
                {
                    Debug.LogFormat("{0}: Tile: {1}, need up: {2}, down: {3}, left: {4}, right: {5} ", GetType().Name, tile, up, down, left, right);
                    await RotateFreeTileAsync(Labyrinth.Tile.RotationDirection.Clockwise);
                }
            }

            public async void ShiftTiles()
            {
                await ShiftTilesAsync();
            }

            async Task ShiftTilesAsync()
            {
                var shift = m_shifts[m_shiftIndex];

                if (m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Player {1, -10} Already made shift: {2}", GetType().Name, CurrentPlayer.Color, shift);
                    return;
                }
                if (!m_availableShifts.Contains(shift))
                {
                    Debug.LogFormat("{0}: Player {1, -10} Attempt to make unavailable shift: {2}", GetType().Name, CurrentPlayer.Color, shift);
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

                await WaitForAnimation(m_labyrinthView.ShiftTiles(m_shiftIndex, shift));

                m_shiftIndex = ShiftIndex.Inverse(m_shiftIndex);

                m_isShiftAlreadyDone = true;
                Debug.LogFormat("{0}: Player {1, -10} Shift completed", GetType().Name, CurrentPlayer.Color);

                if (m_dumpLabyrinth) m_labyrinth.Dump();
            }

            async Task SynchronizeShiftTiles(int shiftIndex)
            {
                Debug.LogFormat("{0}: Synchronizing shift for index:  {1} ", GetType().Name, shiftIndex);

                var shift = m_shifts[m_shiftIndex];
                if (m_unavailableShift != null)
                {
                    m_availableShifts.Add(m_unavailableShift);
                    m_previousUnavailableShift = m_unavailableShift;
                }
                m_availableShifts.Remove(shift);
                m_unavailableShift = shift;

                m_labyrinth.ShiftTiles(shift);
                ShiftPlayers(shift);

                await m_labyrinthView.ShiftTiles(m_shiftIndex, shift);

                m_shiftIndex = ShiftIndex.Inverse(m_shiftIndex);
                m_isShiftAlreadyDone = true;
            }

            public async void CancelShift()
            {
                if (!m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Player {1, -10} No shift to cancel", GetType().Name, CurrentPlayer.Color);
                    return;
                }

                Debug.LogFormat("{0}: Player {1, -10} Cancel shift: {2}", GetType().Name, CurrentPlayer.Color, m_unavailableShift);

                var shiftWithInversedDirection = m_unavailableShift.GetShiftWithInversedDirection();
                m_labyrinth.ShiftTiles(shiftWithInversedDirection);
                UnshiftPlayers();

                await WaitForAnimation(m_labyrinthView.ShiftTiles(m_shiftIndex, shiftWithInversedDirection));

                m_shiftIndex = ShiftIndex.Inverse(m_shiftIndex);

                m_availableShifts.Add(m_unavailableShift);
                if (m_previousUnavailableShift != null)
                {
                    m_availableShifts.Remove(m_previousUnavailableShift);
                }
                m_unavailableShift = m_previousUnavailableShift;
                m_previousUnavailableShift = null;

                m_isShiftAlreadyDone = false;

                Debug.LogFormat("{0}: Player {1, -10} Cancel shift completed", GetType().Name, CurrentPlayer.Color);

                if (m_dumpLabyrinth) m_labyrinth.Dump();
            }

            public async void RotateFreeTile(Labyrinth.Tile.RotationDirection rotationDirection)
            {
                if (m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Player {1, -10} Can not rotate free tile. Shift already made.", GetType().Name, CurrentPlayer.Color);
                    return;
                }

                await RotateFreeTileAsync(rotationDirection);
            }

            async Task RotateFreeTileAsync(Labyrinth.Tile.RotationDirection rotationDirection)
            {
                Debug.LogFormat("{0}: Player {1, -10} Rotate free tile {2}", GetType().Name, CurrentPlayer.Color, rotationDirection);

                m_labyrinth.RotateFreeTile(rotationDirection);

                var freeTileRotation = m_labyrinth.GetFreeTileRotation();

                await WaitForAnimation(m_labyrinthView.RotateFreeTile(freeTileRotation));

                if (m_dumpLabyrinth) m_labyrinth.Dump();
            }

            public async void MoveFreeTile(Labyrinth.Tile.RotationDirection movementDirection)
            {
                await MoveFreeTileAsync(movementDirection);
            }

            public async Task MoveFreeTileAsync(Labyrinth.Tile.RotationDirection movementDirection)
            {
                if (m_isShiftAlreadyDone)
                {
                    Debug.LogFormat("{0}: Can not move free tile. Shift already made.", GetType().Name);
                    return;
                }

                int newShiftIndex = m_shiftIndex;

                if (movementDirection == Labyrinth.Tile.RotationDirection.Clockwise) newShiftIndex = ShiftIndex.Next(m_shiftIndex);
                else if (movementDirection == Labyrinth.Tile.RotationDirection.CounterClockwise) newShiftIndex = ShiftIndex.Prev(m_shiftIndex);

                View.TileMovement tileMovement = View.TileMovement.NORMAL;

                if (ShiftIndex.AtCorner(m_shiftIndex) && ShiftIndex.AtCorner(newShiftIndex))
                {
                    if (ShiftIndex.AtHorizontalSide(m_shiftIndex)) tileMovement = View.TileMovement.HORIZONTAL_TO_VERTICAL;
                    else if (ShiftIndex.AtVerticalSide(m_shiftIndex)) tileMovement = View.TileMovement.VERTICAL_TO_HORIZONTAL;

                    m_labyrinth.RotateFreeTile(movementDirection);
                }
                Debug.LogFormat("{0}: Making tileMovement: {1}, tile rotation: {2}", GetType().Name, tileMovement, m_labyrinth.GetFreeTileRotation());

                m_shiftIndex = newShiftIndex;

                await WaitForAnimation(m_labyrinthView.MoveFreeTile(m_shiftIndex, m_labyrinth.GetFreeTileRotation(), tileMovement));
            }

            // Consider uniting MakeMove and SkipMove. Player can start turn on tile with item he needs to find.
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

                await WaitForAnimation(m_labyrinthView.SetPlayerPosition(CurrentPlayer.Color, position));

                var isCurrentItemFound = IsCurrentPlayerFoundItem();
                if (isCurrentItemFound)
                {
                    Debug.LogFormat("{0}: Player {1, -10} Found item {2}", GetType().Name, CurrentPlayer.Color, CurrentPlayer.CurrentItemToFind);
                    CurrentPlayer.SetCurrentItemFound();
                }
                if (IsCurrentPlayerFoundAllItems())
                {
                    EndGame();

                    return;
                }

                SendSyncronizeGameState(isCurrentItemFound);

                PassTurn(); // Consider moving it before SendSyncronizeGameState()
            }

            async Task SynchronizeMakeMove(Vector2Int position)
            {
                CurrentPlayer.Position = position;

                await m_labyrinthView.SetPlayerPosition(CurrentPlayer.Color, position);

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

                PassTurn();
            }

            public async void SkipMove()
            {
                if (!CanMakeMove())
                {
                    return;
                }

                Debug.LogFormat("{0}: Player {1, -10} Skiping move", GetType().Name, CurrentPlayer.Color);

                SendSyncronizeGameState(false);

                PassTurn(); // Consider moving it before SendSyncronizeGameState()
            }

            void EndGame()
            {
                Debug.LogFormat("{0}: Player {1, -10} GAME OVER", GetType().Name, CurrentPlayer.Color);

                m_gameOverPanel.SetActive(true);
                var text = GameObject.Find("PlayerWinText").GetComponent<Text>();
                text.text = CurrentPlayer.Color + " player wins!";
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

            void PassTurn()
            {
                m_isShiftAlreadyDone = false;
                ResetShiftedForPlayers();

                var shiftWithInversedDirection = m_unavailableShift.GetShiftWithInversedDirection();
                m_availableShifts.Add(m_unavailableShift);
                m_availableShifts.Remove(shiftWithInversedDirection);
                m_unavailableShift = shiftWithInversedDirection;

                UpdateCurrentPlayerInformation();

                Debug.LogFormat("{0}: Player {1, -10} It is your turn now.", GetType().Name, CurrentPlayer.Color);

                //if (CurrentPlayer.Settings.IsAi)
                //{
                //    await MakeAiTurnAsync();
                //}

                m_controls.InputEnabled = false;

                //SendMakeTurnEvent(false, CurrentPlayer.Settings.ActorId);
            }


            void Initialize()
            {
                m_gameOverPanel.SetActive(false);

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

                //if (CurrentPlayer.Settings.IsAi)
                //{
                //    await MakeAiTurnAsync();
                //}
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

            Player NextPlayer
            {
                get
                {
                    return m_players[NextPlayerIndex()];
                }
            }

            void SwitchToNextPlayer()
            {
                m_currentPlayerIndex = ++m_currentPlayerIndex % m_players.Count;
            }

            int NextPlayerIndex()
            {
                return (m_currentPlayerIndex + 1) % m_players.Count;
            }

            async Task WaitForAnimation(Task task)
            {
                m_controls.InputEnabled = false;

                await task;

                m_controls.InputEnabled = true;
            }

            async void MakeAiTurnAsync()
            {
                //await Task.Delay(1000);
                Debug.LogFormat("{0}: AI Player {1, -10} Makes turn", GetType().Name, CurrentPlayer.Color);

                var enumerator = m_availableShifts.GetEnumerator();
                enumerator.MoveNext();
                var shift = enumerator.Current;

                var newShiftIndex = Array.IndexOf(m_shifts, shift);

                if (newShiftIndex != m_shiftIndex)
                {
                    await MoveFreeTileAsync(newShiftIndex);
                }

                await ShiftTilesAsync();
                SkipMove();
            }

            async Task MakeAiTurnAsyncPrev()
            {
                await Task.Delay(1000);
                Debug.LogFormat("{0}: AI Player {1, -10} Makes turn", GetType().Name, CurrentPlayer.Color);

                var enumerator = m_availableShifts.GetEnumerator();
                enumerator.MoveNext();
                var shift = enumerator.Current;

                var newShiftIndex = Array.IndexOf(m_shifts, shift);

                if (newShiftIndex != m_shiftIndex)
                {
                    await MoveFreeTileAsync(newShiftIndex);
                }

                await ShiftTilesAsync();
                SkipMove();
            }

            async Task MoveFreeTileAsync(int newShiftIndex)
            {
                var index = m_shiftIndex;
                var countCW = 0;
                while (index != newShiftIndex)
                {
                    index = ShiftIndex.Next(index);
                    countCW++;
                }

                index = m_shiftIndex;
                var countCCW = 0;
                while (index != newShiftIndex)
                {
                    index = ShiftIndex.Prev(index);
                    countCCW++;
                }

                var movementDirection = countCW <= countCCW 
                    ? Labyrinth.Tile.RotationDirection.Clockwise 
                    : Labyrinth.Tile.RotationDirection.CounterClockwise;

                while (m_shiftIndex != newShiftIndex)
                {
                    await MoveFreeTileAsync(movementDirection);
                }
            }

            Labyrinth.Shift[] m_shifts =
            {
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Positive, 1),
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Positive, 3),
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Positive, 5),

                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Negative, 1),
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Negative, 3),
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Negative, 5),

                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Negative, 5),
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Negative, 3),
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Vertical,   Labyrinth.Shift.Direction.Negative, 1),

                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Positive, 5),
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Positive, 3),
                new Labyrinth.Shift(Labyrinth.Shift.Orientation.Horizontal, Labyrinth.Shift.Direction.Positive, 1)
            };

            int m_shiftIndex = 0;

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

            private static int PlayersInitializedCounter = 0;
            private int PlayersSynchronizedGameStateCounter = 0;

            Controls.TestControls m_controls;

            Text m_currentPlayerText;
            Text m_currentPlayerItemText;

            [SerializeField]
            private GameObject m_gameOverPanel;
        }

    } // namespace GameLogic
} // namespace LabyrinthGame
