using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using System.Linq;

namespace LabyrinthGame
{

    namespace GameLogic
    {
        public class GameSettingsConfigurator : MonoBehaviour, IOnEventCallback
        {
            public const byte ConfigureGameSettingsEventCode = 1;

            private static string AiName = "Bot";

            private void Start()
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.LogFormat("{0}: Configure game settings", GetType().Name);

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
                    GameLogic.GameSettings.PlayersSettings[Color.Yellow] = playerSettings;

                    if (players.Length > 1)
                    {
                        playerSettings = new GameLogic.PlayerSettings(false, players[1].NickName);
                    }
                    else
                    {
                        playerSettings = new GameLogic.PlayerSettings(true, AiName);
                    }
                    GameLogic.GameSettings.PlayersSettings[Color.Red] = playerSettings;

                    if (players.Length > 2)
                    {
                        playerSettings = new GameLogic.PlayerSettings(false, players[2].NickName);
                    }
                    else
                    {
                        playerSettings = new GameLogic.PlayerSettings(true, AiName);
                    }
                    GameLogic.GameSettings.PlayersSettings[Color.Blue] = playerSettings;

                    if (players.Length > 3)
                    {
                        playerSettings = new GameLogic.PlayerSettings(false, players[3].NickName);
                    }
                    else
                    {
                        playerSettings = new GameLogic.PlayerSettings(true, AiName);
                    }
                    GameLogic.GameSettings.PlayersSettings[Color.Green] = playerSettings;

                    var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    string name = "name";
                    var color = GameLogic.Color.Yellow;

                    IDictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte) Color.Yellow, new GameLogic.PlayerSettings(true, "h"));

                    var settings = new GameLogic.PlayerSettings(true, "h");

                    PhotonNetwork.RaiseEvent(ConfigureGameSettingsEventCode, settings, raiseEventOptions, SendOptions.SendReliable);
                    Debug.LogFormat("{0}: Send {1}", GetType().Name, actorNumbers);
                }
            }

            public void OnEvent(EventData photonEvent)
            {
                byte eventCode = photonEvent.Code;
                if (eventCode == ConfigureGameSettingsEventCode)
                {
                    //var playersSettings = (IDictionary<byte, object>)photonEvent.CustomData;               
                    //Debug.LogFormat("{0}: Received players settings {1}", GetType().Name, ((PlayerSettings)playersSettings[(byte)Color.Yellow]).Name);

                    var playersSettings = (PlayerSettings)photonEvent.CustomData;
                    Debug.LogFormat("{0}: Received players settings {1}", GetType().Name, ((PlayerSettings)playersSettings).Name);
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


