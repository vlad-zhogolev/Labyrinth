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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Labyrinth.Labyrinth m_labyrinth;
}

} // GameLogic

} // LabyrinthGame