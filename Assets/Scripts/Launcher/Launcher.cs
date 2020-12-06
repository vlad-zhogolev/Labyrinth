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
                }
            }


            #endregion



            #region MonoBehaviourPunCallbacks Callbacks

            public override void OnConnectedToMaster()
            {
                Debug.LogFormat("{0}: OnConnectedToMaster() was called by PUN", GetType().Name);
                //PhotonNetwork.JoinRandomRoom();
            }

            public override void OnDisconnected(DisconnectCause cause)
            {
                Debug.LogWarningFormat("{0}: OnDisconnected() was called by PUN with reason {1}", GetType().Name, cause);
            }

            public override void OnJoinRandomFailed(short returnCode, string message)
            {
                Debug.LogFormat("{0}:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom", GetType().Name);

                // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
                PhotonNetwork.CreateRoom(null, new RoomOptions());
            }

            public override void OnJoinedRoom()
            {
                Debug.LogFormat("{0}: OnJoinedRoom() called by PUN. Now this client is in a room.", GetType().Name);
            }

            #endregion
        }

    } // namespace Launcher

} // namespace LabyrinthGame
