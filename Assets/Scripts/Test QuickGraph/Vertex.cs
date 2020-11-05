using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickGraphTest {

public class Vertex
{
    public int Row { get; set; }
    public int Сolumn { get; set; }

    public Vertex()
    {

    }

    public Vertex(int row, int column)
    {
        Row = row;
        Сolumn = column;
    }

    public Tile tile;
    
}

}
