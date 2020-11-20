using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LabyrinthGame
{
    namespace UI
    {
        public class LocalGameSettings : MonoBehaviour
        {
            public void StartLocalGameButtonClickHandler()
            {
                foreach (var uiPlayerSettings in m_uiPlayersSettings)
                {
                    var settings = m_uiPlayersSettings[uiPlayerSettings.Key].GetComponent<UIPlayerSettings>();
                    var playerSettings = new GameLogic.PlayerSettings(settings.IsAi, settings.Name);
                    GameLogic.GameSettings.PlayersSettings[uiPlayerSettings.Key] = playerSettings;
                }

                SceneManager.LoadScene("Game");
            }

            public void IsAiToggleValueChangedHandler()
            {
                var isAnyHumanPresent = false;
                foreach (var uiPlayerSettings in m_uiPlayersSettings)
                {
                    var settings = m_uiPlayersSettings[uiPlayerSettings.Key].GetComponent<UIPlayerSettings>();
                    if (!settings.IsAi)
                    {
                        isAnyHumanPresent = true;
                        break;
                    }
                }

                m_startLocalGameButton.enabled = isAnyHumanPresent;
                m_startLocalGameButton.image.color = isAnyHumanPresent ? Color.white : Color.grey;
            }
            
            // Start is called before the first frame update
            void Start()
            {
                m_uiPlayersSettings[GameLogic.Color.Yellow] = m_yellowPlayerSettings;
                m_uiPlayersSettings[GameLogic.Color.Green] = m_greenPlayerSettings;
                m_uiPlayersSettings[GameLogic.Color.Red] = m_redPlayerSettings;
                m_uiPlayersSettings[GameLogic.Color.Blue] = m_bluePlayerSettings;
            }

            [SerializeField]
            private UnityEngine.UI.Button m_startLocalGameButton;

            [SerializeField]
            private GameObject m_yellowPlayerSettings;
            [SerializeField]
            private GameObject m_redPlayerSettings;
            [SerializeField]
            private GameObject m_greenPlayerSettings;
            [SerializeField]
            private GameObject m_bluePlayerSettings;

            [SerializeField]
            private IDictionary<GameLogic.Color, GameObject> m_uiPlayersSettings = new Dictionary<GameLogic.Color, GameObject>()
            {
                {GameLogic.Color.Yellow,  null},
                {GameLogic.Color.Red,     null},
                {GameLogic.Color.Green,   null},
                {GameLogic.Color.Blue,    null},
            };
        }

    } // namespace UI

} // namespace LabyrinthGame 


