using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace LabyrinthGame
{

    namespace GameLogic
    {
        public class GameSettingsConfigurator : MonoBehaviour, IOnEventCallback
        {
            public UnityEvent startGame;

            public const byte ConfigureGameSettingsEventCode = 1;
            public const byte GameSettingsConfiguredEventCode = 2;

            private static string AiName = "Bot";
            public static int PlayersConfiguredCounter = 0;

            private void Start()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.LogFormat("{0}: Configure game settings", GetType().Name);

                    // Configure players settings

                    var players = PhotonNetwork.PlayerList;
                    var actorNumbers = new int[4];
                    
                    for (var i = 0; i < players.Length; ++i)
                    {
                        actorNumbers[i] = players[i].ActorNumber; // Human players
                    }

                    for (var i = 0; i < 4 - players.Length; ++i)
                    {
                        actorNumbers[players.Length + i] = -1; // Ai players
                    }

                    var playerSettings = new GameLogic.PlayerSettings(false, players[0].NickName);
                    playerSettings.ActorId = players[0].ActorNumber;
                    GameLogic.GameSettings.PlayersSettings[Color.Yellow] = playerSettings;

                    if (players.Length > 1)
                    {
                        playerSettings = new GameLogic.PlayerSettings(false, players[1].NickName);
                        playerSettings.ActorId = players[1].ActorNumber;
                    }
                    else
                    {
                        playerSettings = new GameLogic.PlayerSettings(true, AiName);
                    }
                    GameLogic.GameSettings.PlayersSettings[Color.Red] = playerSettings;

                    if (players.Length > 2)
                    {
                        playerSettings = new GameLogic.PlayerSettings(false, players[2].NickName);
                        playerSettings.ActorId = players[2].ActorNumber;
                    }
                    else
                    {
                        playerSettings = new GameLogic.PlayerSettings(true, AiName);
                    }
                    GameLogic.GameSettings.PlayersSettings[Color.Blue] = playerSettings;

                    if (players.Length > 3)
                    {
                        playerSettings = new GameLogic.PlayerSettings(false, players[3].NickName);
                        playerSettings.ActorId = players[3].ActorNumber;
                    }
                    else
                    {
                        playerSettings = new GameLogic.PlayerSettings(true, AiName);
                    }
                    GameLogic.GameSettings.PlayersSettings[Color.Green] = playerSettings;

                    // Configure maze settings

                    var randomizer = new System.Random();
                    GameSettings.TilesPositionsSeed = randomizer.Next();
                    GameSettings.TilesRotationsSeed = randomizer.Next();

                    //GameSettings.TilesPositionsSeed = 4;
                    //GameSettings.TilesRotationsSeed = 0;


                    var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                   
                    PhotonNetwork.RaiseEvent(ConfigureGameSettingsEventCode, GameSettings.GetPhotonCompatibleSettings(), raiseEventOptions, SendOptions.SendReliable);
                    Debug.LogFormat("{0}: Send game settings", GetType().Name);
                }
            }

            public void OnEvent(EventData photonEvent)
            {
                byte eventCode = photonEvent.Code;
                if (eventCode == ConfigureGameSettingsEventCode)
                {
                    var playersSettings = (object[])photonEvent.CustomData;
                    GameSettings.SetPhotonCompatibleSettings(playersSettings);
                    Debug.LogFormat("{0}: Received game settings", GetType().Name);

                    GameSettings.Trace();

                    var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
                    PhotonNetwork.RaiseEvent(GameSettingsConfiguredEventCode, null, raiseEventOptions, SendOptions.SendReliable);
                    Debug.LogFormat("{0}: Send game settings received approve", GetType().Name);
                }
                if (eventCode == GameSettingsConfiguredEventCode && PhotonNetwork.IsMasterClient)
                {
                    Debug.LogFormat("{0}: Received approval", GetType().Name);
                    ++PlayersConfiguredCounter;
                    if (PlayersConfiguredCounter == PhotonNetwork.PlayerList.Length)
                    {
                        Debug.LogFormat("{0}: Everyone set game settings, number of players: {1}", GetType().Name, PlayersConfiguredCounter);
                        startGame?.Invoke();
                    }
                }
            }

            private void OnEnable()
            {
                PhotonNetwork.AddCallbackTarget(this);
            }

            private void OnDisable()
            {
                PhotonNetwork.RemoveCallbackTarget(this);
            }
        }

    } // namespace GameLogic

} // namespace LabyrinthGame


