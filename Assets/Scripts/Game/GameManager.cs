using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabyrinthGame {

namespace GameLogic {

public class GameManager : MonoBehaviour
{
    void Initiallize()
    {
        m_labyrinth = new Labyrinth.Labyrinth(4, 0);
       
        m_labyrinthView = GetComponent<View.LabyrinthView>();
        (var tiles, var freeTile) = m_labyrinth.GetTiles();
        m_labyrinthView.Initialize(tiles, freeTile, new Vector2(0, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        Initiallize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Labyrinth.Labyrinth m_labyrinth;

    private View.LabyrinthView m_labyrinthView;
}

} // GameLogic

} // LabyrinthGame