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
    public enum Side
    {
        Up,
        Down,
        Right,
        Left
    }

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

    bool IsTileFixed(int i, int j)
    {
        return (i % 2 == 0) && (j % 2 == 0);
    }

    void RotateTileRandomly(Tile tile)
    {
        if (tile == null)
        {
            return;
        }

        var rotationsNumber = rng.Next(4);
        for (var k = 0; k < rotationsNumber; ++k)
        {
            tile.RotateCW();
        }
    }

    void InitializeVertices()
    {
        // Create vertices
        for (var i = 0; i < m_vertices.GetLength(0); ++i)
        {
            for (var j = 0; j < m_vertices.GetLength(1); ++j)
            {
                m_vertices[i, j] = new Vertex(i, j);
            }
        }

        // Associate tile with each vertex
        // Corner fixed tiles
        var turnTile = new Tile(Tile.Type.Turn);
        m_vertices[6, 0].tile = turnTile.Copy().RotateCCW();
        m_vertices[0, 0].tile = turnTile.Copy();
        m_vertices[0, 6].tile = turnTile.Copy().RotateCW();
        m_vertices[6, 6].tile = turnTile.Copy().RotateCW().RotateCW();

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

        // Movable tiles
        var counter = 0;
        for (var i = 0; i < BoardLength; ++i)
        {
            for (var j = 0; j < BoardLength; ++j)
            {
                if (IsTileFixed(i, j))
                {
                    continue;
                }

                m_vertices[i, j].tile = Tile.MovableTiles[range[counter]].Copy();
                RotateTileRandomly( m_vertices[i, j].tile);
                ++counter;
            }
        }

        // Free tile
        m_freeTile = Tile.MovableTiles[range[counter]].Copy();
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

    Transform InstantiateTile(Tile tile, float x, float z)
    {
        Transform prefab = GetPrefabByTileType(tile.type);
        var instance = Instantiate(prefab, new Vector3(x, 0, z), tile.GetRotation());
        var scale = 0.9f;
        instance.localScale = new Vector3(scale, scale, scale);

        return instance;
    }

    void InstantiateLabyrinth()
    {
        var x = 3.0f;
        var step = 1.0f;
        for (var i = 0; i < m_vertices.GetLength(0); ++i)
        {
            var z = 3.0f;
            for (var j = 0; j < m_vertices.GetLength(1); ++j)
            {
                var tile = m_vertices[i, j].tile;
                InstantiateTile(tile, x, z);
                //Debug.LogFormat("{0}: tile [{1}, {2}] type = {3}, rotation = {4}",
                //    GetType().Name, i, j, tile.type, tile.GetRotation());
                z -= step;
            }
            x -= step;
        }

        var freeTileX = 5.0f;
        var freeTileY = 5.0f;
        InstantiateTile(m_freeTile, freeTileX, freeTileY);
    }

    bool IsVertexIndexValid(int index)
    {
        return (0 <= index) && (index < BoardLength);
    }

    void AddEdges(Func<int, int, Tuple<Vertex, Vertex>> adjacentVerticesProvider, Tile.Side side)
    {
        for (var i = 0; i < BoardLength - 1; ++i)
        {
            for (var j = 0; j < BoardLength; ++j)
            {
                var adjacentVertices = adjacentVerticesProvider(i, j);
                var firstVertex = adjacentVertices.Item1;
                var secondVertex = adjacentVertices.Item2;
                if (firstVertex.tile.IsConnected(secondVertex.tile, side))
                {
                    m_graph.AddEdge(new QuickGraph.Edge<Vertex>(firstVertex, secondVertex));
                    m_graph.AddEdge(new QuickGraph.Edge<Vertex>(secondVertex, firstVertex));
                }
            }
        }
    }

    void InitializeGraph()
    {
        for (var i = 0; i < BoardLength; ++i)
        {
            for (var j = 0; j < BoardLength; ++j)
            {
                m_graph.AddVertex(m_vertices[i, j]);
            }
        }

        AddEdges(
            (i, j) => { 
                return new Tuple<Vertex, Vertex>(m_vertices[i, j], m_vertices[i + 1, j]);
            },
            Tile.Side.Up
        );
        AddEdges(
            (i, j) => { 
                return new Tuple<Vertex, Vertex>(m_vertices[j, i], m_vertices[j, i + 1]);
            },
            Tile.Side.Left
        );

        Func<Edge<Vertex>, double> distances = x => 1.0;
        var source = m_vertices[0, 0];
        var startTime = Time.realtimeSinceStartup;
        var tryGetPath = m_graph.ShortestPathsDijkstra(distances, source);
        var endTime = Time.realtimeSinceStartup;

        Debug.LogFormat("{0}: Time passed {1}", GetType().Name, endTime - startTime);

        foreach (var vertex in m_vertices)
        {
            IEnumerable<Edge<Vertex>> path;
            if (tryGetPath(vertex, out path))
            {
                Debug.LogFormat("{0}: Shortest path from ({1}, {2}) to ({3}, {4})", GetType().Name, source.Row, source.Сolumn, vertex.Row, vertex.Сolumn);
                foreach (var e in path)
                {
                    Debug.LogFormat("{0}: {1}, ({2}, {3})->({4}, {5})", GetType().Name, e, e.Source.Row, e.Source.Сolumn, e.Target.Row, e.Target.Сolumn);
                }
            }
        }
    }

    void Initialize()
    {
        InitializeVertices();
        InitializeGraph();
        InstantiateLabyrinth();
    }


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AdjacencyGraph<Vertex, Edge<Vertex>> m_graph = new AdjacencyGraph<Vertex, Edge<Vertex>>();
    public Vertex [,] m_vertices = new Vertex[BoardLength, BoardLength];

    public Tile m_freeTile = null;

    public Transform junctionTilePrefab;
    public Transform turnTilePrefab;
    public Transform straightTilePrefab;
}

} // namespace QuickGraphTest
