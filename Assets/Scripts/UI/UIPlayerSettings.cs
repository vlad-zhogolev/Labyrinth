using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabyrinthGame
{
    namespace UI
    {
        public class UIPlayerSettings : MonoBehaviour
        {
            public bool IsAi 
            { 
                get
                {
                    return m_toggle.GetComponent<UnityEngine.UI.Toggle>().isOn;
                } 
            }


            public string Name
            {
                get
                {
                    return m_playerName.text;
                }
            }
            
            [SerializeField]
            private GameObject m_toggle;

            [SerializeField]
            private UnityEngine.UI.Text m_playerName;
        }
    } // namespace UI
} // namespace LabyrinthGame
