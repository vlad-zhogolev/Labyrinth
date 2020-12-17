
using UnityEngine;


namespace LabyrinthGame
{
    namespace View
    {

        public class CameraPivot : MonoBehaviour
        {

            static readonly float[] VIEW_ANGLES = { 45, 67.5f, 90 };
            
            public void NextView()
            {
                m_viewIndex++;

                if (m_viewIndex == VIEW_ANGLES.Length) m_viewIndex = 0;

                transform.rotation = Quaternion.Euler(VIEW_ANGLES[m_viewIndex], 0, 0);
            }

            int m_viewIndex = 0;

        }

    }
}
