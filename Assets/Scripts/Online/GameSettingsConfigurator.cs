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

            private void Awake()
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

                    var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    PhotonNetwork.RaiseEvent(ConfigureGameSettingsEventCode, actorNumbers, raiseEventOptions, SendOptions.SendReliable);
                    Debug.LogFormat("{0}: Send {1}", GetType().Name, actorNumbers);
                }
            }

            public void OnEvent(EventData photonEvent)
            {
                byte eventCode = photonEvent.Code;
                if (eventCode == ConfigureGameSettingsEventCode)
                {
                    var actorNumbers = (int[])photonEvent.CustomData;
                    Debug.LogFormat("{0}: Received {1}", GetType().Name, actorNumbers);
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


