using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace LabyrinthGame
{
    namespace UI
    {
        public class GameOver : MonoBehaviour
        {
            public void GoToMainMenuHandler()
            {
                SceneManager.LoadScene("StartScene");
            }
        }
    } // namespace UI
} // namespace LabyrinthGame
