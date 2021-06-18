using System;
using UnityEngine;


namespace LabyrinthGame
{
    namespace View
    {

        public class ItemProvider : MonoBehaviour
        {

            public Transform GetItemPrefab(Labyrinth.Item item)
            {
                if (item == Labyrinth.Item.None) throw new ArgumentException("The item is Item.None");

                return m_items[(int) item - 2]; // 2 for Item.None and Item.Home
            }

            [SerializeField]
            Transform[] m_items;

        }

    }
}
