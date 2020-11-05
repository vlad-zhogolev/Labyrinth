using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickGraph;
using System.Linq;
using System;
using QuickGraph.Algorithms;

namespace QuickGraphTest {
public class Labyrinth : MonoBehaviour
{
    public static readonly int BoardLength = 7;
    public static readonly int TilesNumber = 49;
    public static readonly int FixedTilesNumber = 16;
    public static readonly int MovableTilesNumber = 34;
    public static readonly int MovableJunctionTilesNumber = 6;
    public static readonly int MovableTurnTilesNumber = 15;
    public static readonly int MovableStraightTilesNumber = 13;

    private static System.Random rng = new System.Random();  

    public static void Shuffle(int[] list)  
    {  
        int n = list.Length;  
        while (n > 1)
        {  
            n--;  
            int k = rng.Next(n + 1);
            int value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }

    void InitializeVertices()
    {
        // Corner fixed tiles
        var turnTile = new Tile(Tile.Type.Turn).RotateCCW();
        m_vertices[6, 0].tile = turnTile.Copy();
        m_vertices[0, 0].tile = turnTile.Copy().RotateCW();
        m_vertices[0, 6].tile = turnTile.Copy().RotateCW().RotateCW();
        m_vertices[6, 6].tile = turnTile.Copy().RotateCCW();

        // Border fixed tiles
        var junctionTile = new Tile(Tile.Type.Junction);
        m_vertices[0, 2].tile = junctionTile.Copy();
        m_vertices[0, 4].tile = junctionTile.Copy();
        
        junctionTile.RotateCW();
        m_vertices[2, 6].tile = junctionTile.Copy();
        m_vertices[4, 6].tile = junctionTile.Copy();

        junctionTile.RotateCW();
        m_vertices[6, 2].tile = junctionTile.Copy();
        m_vertices[6, 4].tile = junctionTile.Copy();

        junctionTile.RotateCW();
        m_vertices[2, 0].tile = junctionTile.Copy();
        m_vertices[4, 0].tile = junctionTile.Copy();

        // Inner fixed tiles
        m_vertices[2, 2].tile = junctionTile.Copy();
        
        junctionTile.RotateCW();
        m_vertices[2, 4].tile = junctionTile.Copy();

        junctionTile.RotateCW();
        m_vertices[4, 4].tile = junctionTile.Copy();

        junctionTile.RotateCW();
        m_vertices[4, 2].tile = junctionTile.Copy();

        int[] range = new int[MovableTilesNumber];
        for (var i = 0; i < range.Length; ++i)
        {
            range[i] = i;
        }
        Shuffle(range);

        // Fill movable tiles
        var counter = 0;
        for (var i = 0; i < BoardLength; ++i)
        {
            for (var j = 0; j < BoardLength; ++j)
            {
                if (i % 2 == 0 && j % 2 == 0)
                {
                    continue;
                }

                m_vertices[i, j].tile = Tile.MovableTiles[range[counter]].Copy();
                ++counter;
            }
        }
    }

    private Transform GetPrefabByTileType(Tile.Type type)
    {
        switch (type)
        {
            case Tile.Type.Junction:
            {
                return junctionTilePrefab;
            }
            case Tile.Type.Turn:
            {
                return turnTilePrefab;
            }
            case Tile.Type.Straight:
            {
                return straightTilePrefab;
            }
            default:
            {
                throw new ArgumentException("Invalid type provided");
            }
        }
    }

    void Initialize()
    {
        InitializeVertices();

        var x = 3.0f;
        var step = 1.0f;
        for (var i = 0; i < m_vertices.GetLength(0); ++i)
        {
            var z = 3.0f;
            for (var j = 0; j < m_vertices.GetLength(1); ++j)
            {
                var tile = m_vertices[i, j].tile;
                var rotationsNumber = rng.Next(4);
                for (var k = 0; k < rotationsNumber; ++k)
                {
                    tile.RotateCW();
                }
                Transform prefab = GetPrefabByTileType(tile.type);
                var instance = Instantiate(prefab, new Vector3(x, 0, z), tile.GetRotation());
                var scale = 0.9f;
                instance.localScale = new Vector3(scale, scale, scale);
                Debug.LogFormat("{0}: tile [{1}, {2}] type = {3}, rotation = {4}",
                    GetType().Name, i, j, tile.type, tile.GetRotation());
                z -= step;
            }
            x -= step;
        }

        
    }


    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < m_vertices.GetLength(0); ++i)
        {
            for (var j = 0; j < m_vertices.GetLength(1); ++j)
            {
                m_vertices[i, j] = new Vertex();
            }
        }
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AdjacencyGraph<Vertex, Edge<Vertex>> m_graph = null;
    public Vertex [,] m_vertices = new Vertex[BoardLength, BoardLength];

    public Transform junctionTilePrefab;
    public Transform turnTilePrefab;
    public Transform straightTilePrefab;
}

} // namespace QuickGraphTest
