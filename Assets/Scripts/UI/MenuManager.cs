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
                m_onlineGameScreen.SetActive(false);
                m_gameRulesScreen.SetActive(false);
            }

            public void SwitchToLocalGameSettings()
            {
                m_localGameSettings.SetActive(true);
                m_mainMenu.SetActive(false);
            }

            public void SwitchToOnlineGameScreen()
            {
                m_onlineGameScreen.SetActive(true);
                m_mainMenu.SetActive(false);
            }

            public void SwitchToGameRulesScreen()
            {
                m_gameRulesScreen.SetActive(true);
                m_mainMenu.SetActive(false);
            }

            [SerializeField]
            private GameObject m_mainMenu;

            [SerializeField]
            private GameObject m_localGameSettings;

            [SerializeField]
            private GameObject m_onlineGameScreen;

            [SerializeField]
            private GameObject m_gameRulesScreen;
        }
    } // UI
} // namespace LabyrinthGame
