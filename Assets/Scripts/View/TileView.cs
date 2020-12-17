using UnityEngine;

namespace LabyrinthGame
{
    namespace View
    {
        public class TileView : AnimatedView
        {

            void Start()
            {
                m_rendeer = GetComponent<Renderer>();

                m_normalMaterials = m_rendeer.materials;
            }

            public void ShowAsNormal()
            {
                m_rendeer.materials = m_normalMaterials;
            }

            public void ShowAsReachable()
            {
                m_rendeer.materials = m_reachableMaterials;
            }

            public void ShowAsUnreachable()
            {
                m_rendeer.materials = m_unreachableMaterials;
            }

            Renderer m_rendeer;

            Material[] m_normalMaterials;

            [SerializeField]
            Material[] m_reachableMaterials;

            [SerializeField]
            Material[] m_unreachableMaterials;

        }

    } // namespace View

} // namespace LabyrinthGame
