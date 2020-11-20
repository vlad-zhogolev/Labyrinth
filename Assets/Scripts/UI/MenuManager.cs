using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabyrinthGame
{
    namespace UI
    {
        public class MenuManager : MonoBehaviour
        {
            private void Start()
            {
                SwitchToMainMenu();
            }

            public void SwitchToMainMenu()
            {
                m_mainMenu.SetActive(true);
                m_localGameSettings.SetActive(false);
            }

            public void SwitchToLocalGameSettings()
            {
                m_localGameSettings.SetActive(true);
                m_mainMenu.SetActive(false);
            }

            [SerializeField]
            private GameObject m_mainMenu;

            [SerializeField]
            private GameObject m_localGameSettings;
        }
    } // UI
} // namespace LabyrinthGame
