using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


namespace LabyrinthGame
{
    namespace UI
    {
        public class GameOver : MonoBehaviourPunCallbacks
        {
            public void GoToMainMenuHandler()
            {
                Debug.LogFormat("{0}: Load start scene", GetType().Name);
                SceneManager.LoadScene("StartScene");
            }

            public void GoToMainMenuOnlineHandler()
            {
                Debug.LogFormat("{0}: Leave room", GetType().Name);
                Time.timeScale = 1;
                PhotonNetwork.LeaveRoom();
            }

            public override void OnLeftRoom()
            {
                GoToMainMenuHandler();
            }
        }
    } // namespace UI
} // namespace LabyrinthGame
