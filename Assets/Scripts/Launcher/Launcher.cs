using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace LabyrinthGame {

    namespace Launcher
    {

        public class Launcher : MonoBehaviourPunCallbacks
        {
            #region Private Serializable Fields


            #endregion


            #region Private Fields


            /// <summary>
            /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
            /// </summary>
            string gameVersion = "1";


            #endregion


            #region MonoBehaviour CallBacks


            /// <summary>
            /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
            /// </summary>
            void Awake()
            {
                // #Critical
                // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
                PhotonNetwork.AutomaticallySyncScene = true;
            }

            #endregion


            #region Public Methods


            /// <summary>
            /// Start the connection process.
            /// - If already connected, we attempt joining a random room
            /// - if not yet connected, Connect this application instance to Photon Cloud Network
            /// </summary>
            public void Connect()
            {
                Debug.LogFormat("{0}: Connect ot photon network.", GetType().Name);

                if (!PhotonNetwork.IsConnectedAndReady)
                {
                    // #Critical, we must first and foremost connect to Photon Online Server.
                    PhotonNetwork.ConnectUsingSettings();
                    PhotonNetwork.GameVersion = gameVersion;
                }
            }

            public void Disconnect()
            {
                Debug.LogFormat("{0}: Disconnect ot photon network.", GetType().Name);

                if (PhotonNetwork.IsConnectedAndReady)
                {
                    PhotonNetwork.Disconnect();
                    m_joinRoomButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Join room";
                }
            }

            
            public void JoinRoom()
            {
                PhotonNetwork.NickName = m_playerNameInputField.text;
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.LoadLevel("OnlineGame");
                }
                else
                {
                    if (m_isRoomPrivateToggle.isOn)
                    {
                        Debug.LogFormat("{0}: Join or create private room", GetType().Name);
                        var roomName = m_roomNameInputField.text;

                        RoomOptions roomOptions = new RoomOptions();
                        roomOptions.IsVisible = false;
                        roomOptions.MaxPlayers = 4;

                        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
                    }
                    else
                    {
                        Debug.LogFormat("{0}: join random room", GetType().Name);
                        //PhotonNetwork.JoinRandomRoom();
                        //var roomOptions = new RoomOptions();
                        //roomOptions.IsVisible = m_isRoomPrivateToggle.isOn;
                        //roomOptions.MaxPlayers = 4;

                        //var enterRoomParams = new EnterRoomParams();
                        //enterRoomParams.RoomOptions = roomOptions;
                        PhotonNetwork.NetworkingClient.OpJoinRandomOrCreateRoom(null, null);
                    }
                }
            }

            #endregion



            #region MonoBehaviourPunCallbacks Callbacks

            public override void OnConnectedToMaster()
            {
                Debug.LogFormat("{0}: OnConnectedToMaster() was called by PUN", GetType().Name);
            }

            public override void OnDisconnected(DisconnectCause cause)
            {
                Debug.LogWarningFormat("{0}: OnDisconnected() was called by PUN with reason {1}", GetType().Name, cause);
            }

            public override void OnJoinRandomFailed(short returnCode, string message)
            {
                Debug.LogFormat("{0}:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom", GetType().Name);

                // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
                //RoomOptions roomOptions = new RoomOptions();
                //roomOptions.IsVisible = m_isRoomPrivateToggle.isOn;
                //roomOptions.IsOpen = true;
                //roomOptions.MaxPlayers = 4;
                //PhotonNetwork.CreateRoom("hell", roomOptions);

                //PhotonNetwork.CreateRoom("hell", new RoomOptions());
            }

            public override void OnJoinedRoom()
            {
                Debug.LogFormat("{0}: OnJoinedRoom() called by PUN. Now this client is in a room.", GetType().Name);

                Debug.LogFormat("{0}: Is client master: {1}", GetType().Name, PhotonNetwork.IsMasterClient);
                if (PhotonNetwork.IsMasterClient)
                {
                    var text = m_joinRoomButton.GetComponentInChildren<UnityEngine.UI.Text>();
                    text.text = "Start game";
                }
                else
                {
                    m_joinRoomButton.interactable = false;
                }
            }

            #endregion

            [SerializeField]
            private UnityEngine.UI.InputField m_playerNameInputField;

            [SerializeField]
            private UnityEngine.UI.InputField m_roomNameInputField;

            [SerializeField]
            private UnityEngine.UI.Toggle m_isRoomPrivateToggle;

            [SerializeField]
            private UnityEngine.UI.Button m_joinRoomButton;
        }

    } // namespace Launcher

} // namespace LabyrinthGame
