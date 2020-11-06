using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using QuickGraph;
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
    public static readonly int TileSidesNumber = 4;

    private static System.Random TilePositionRandomizer = new System.Random(4);
    private static System.Random TileRotationRandomizer = new System.Random(0);

    public static void Shuffle(int[] list)  
    {  
        int n = list.Length;  
        while (n > 1)
        {  
            n--;  
            int k = TilePositionRandomizer.Next(n + 1);
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

        var rotationsNumber = TileRotationRandomizer.Next(TileSidesNumber);
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
        m_vertices[0, 0].tile = turnTile.Copy().RotateCCW();
        m_vertices[6, 0].tile = turnTile.Copy().RotateCW().RotateCW();
        m_vertices[6, 6].tile = turnTile.Copy().RotateCW();
        m_vertices[0, 6].tile = turnTile.Copy();

        // Border fixed tiles
        var junctionTile = new Tile(Tile.Type.Junction).RotateCCW();
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
        instance.localScale = new Vector3(m_tileScale, m_tileScale, m_tileScale);

        return instance;
    }

    void InstantiateLabyrinth()
    {
        var z = 3.0f;
        var step = 1.0f;
        for (var i = 0; i < BoardLength; ++i)
        {
            var x = -3.0f;
            for (var j = 0; j < BoardLength; ++j)
            {
                var tile = m_vertices[i, j].tile;
                m_vertices[i, j].TileInstance = InstantiateTile(tile, x, z);

                Debug.LogFormat("{0}: tile [{1}, {2}] type = {3}, rotation = {4}",
                    GetType().Name, i, j, tile.type, tile.GetRotation());
                x += step;
            }
            z -= step;
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
                    m_graph.AddEdge(new QuickGraph.SEquatableUndirectedEdge<Vertex>(firstVertex, secondVertex));
                    //m_graph.AddEdge(new QuickGraph.Edge<Vertex>(secondVertex, firstVertex));
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
            Tile.Side.Down
        );
        AddEdges(
            (i, j) => { 
                return new Tuple<Vertex, Vertex>(m_vertices[j, i], m_vertices[j, i + 1]);
            },
            Tile.Side.Right
        );

        //var shift = new Shift(Shift.Orientation.Horizontal, Shift.Direction.Positive, 1);
        //UpdateGraphForShift(shift, m_freeTile);
        //Edge<Vertex> edge;
        //var result = m_graph.TryGetEdge(m_vertices[0, 0], m_vertices[1, 0], out edge);
        //if (edge != null)
        //{
        //    m_graph.RemoveEdge(edge);
        //}

        if (m_graph.ContainsEdge(new QuickGraph.SEquatableUndirectedEdge<Vertex>(m_vertices[0, 0], m_vertices[1, 0])))
        {
            Debug.LogFormat("{0}: graph contains edge (0, 0) <-> (1, 0)", GetType().Name);
        }

        var res1 = m_graph.AddEdge(new QuickGraph.SEquatableUndirectedEdge<Vertex>(m_vertices[0, 0], m_vertices[1, 0]));

        var res2 = m_graph.RemoveEdge(new QuickGraph.SEquatableUndirectedEdge<Vertex>(m_vertices[0, 0], m_vertices[1, 0]));

        foreach (var e in m_graph.AdjacentEdges(m_vertices[0,0]))
        {
            Debug.LogFormat("{0}: edge = {1}", GetType().Name, e);
        }

        //var first = new Edge<Vertex>(m_vertices[0, 0], m_vertices[1, 0]);
        //var second = new Edge<Vertex>(m_vertices[0, 0], m_vertices[1, 0]);

        //var equal = first == second;

        //result = m_graph.TryGetEdge(m_vertices[0, 0], m_vertices[0, 1], out edge);
        //if (edge != null)
        //{
        //    m_graph.RemoveEdge(edge);
        //}
        
        var source = m_vertices[0, 0];
        var startTime = Time.realtimeSinceStartup;
        var tryGetPath = m_graph.ShortestPathsDijkstra(distance => 1.0, source);
        var endTime = Time.realtimeSinceStartup;

        Debug.LogFormat("{0}: Time passed {1}", GetType().Name, endTime - startTime);

        foreach (var vertex in m_vertices)
        {
            IEnumerable<QuickGraph.SEquatableUndirectedEdge<Vertex>> path;
            if (tryGetPath(vertex, out path))
            {
                Debug.LogFormat("{0}: Shortest path from ({1}, {2}) to ({3}, {4})", GetType().Name, source.Row, source.Column, vertex.Row, vertex.Column);
                foreach (var e in path)
                {
                    Debug.LogFormat("{0}: {1}, ({2}, {3})->({4}, {5})", GetType().Name, e, e.Source.Row, e.Source.Column, e.Target.Row, e.Target.Column);
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

    void RemoveEdges(Func<int, Tuple<Vertex, Vertex>> adjacentVerticesProvider)
    {
        for (var i = 0; i < BoardLength; ++i)
        {
            var vertices = adjacentVerticesProvider(i);
            var firstVertex = vertices.Item1;
            var secondVertex = vertices.Item2;
            m_graph.RemoveEdge(new QuickGraph.SEquatableUndirectedEdge<Vertex>(firstVertex, secondVertex));
        }
    }

    void AddEdges(int line, int neighbourLine, Shift.Orientation orientation)
    {
        Tile.Side side;
        var isLineBefore = line < neighbourLine;
        if (orientation == Shift.Orientation.Horizontal)
        {
            if (isLineBefore)
            {
                side = Tile.Side.Up;
            }
            else
            {
                side = Tile.Side.Down;
            }
        }
        else
        {
            if (isLineBefore)
            {
                side = Tile.Side.Right;
            }
            else
            {
                side = Tile.Side.Left;
            }
        }

        for (var i = 0; i < BoardLength; ++i)
        {
            var firstVertex = m_vertices[line, i];
            var secondVertex = m_vertices[neighbourLine, i];
            if (firstVertex.tile.IsConnected(secondVertex.tile, side))
            {
                m_graph.AddEdge(new QuickGraph.SEquatableUndirectedEdge<Vertex>(firstVertex, secondVertex));
                //m_graph.AddEdge(new QuickGraph.Edge<Vertex>(secondVertex, firstVertex));
            }
        }
    }

    void UpdateGraphForShift(Shift shift, Tile insertedTile)
    {
        var line = shift.index;
        Func<int, Tuple<Vertex, Vertex>> vertexProviderPreviousLine;
        Func<int, Tuple<Vertex, Vertex>> vertexProviderNextLine;
        switch (shift.orientation)
        {
            case Shift.Orientation.Horizontal:
            {
                vertexProviderPreviousLine = (i) => { 
                    return new Tuple<Vertex, Vertex>(m_vertices[line, i], m_vertices[line - 1, i]);
                };
                vertexProviderNextLine = (i) => { 
                    return new Tuple<Vertex, Vertex>(m_vertices[line, i], m_vertices[line + 1, i]);
                };
            }
            break;
            case Shift.Orientation.Vertical:
            {
                vertexProviderPreviousLine = (i) => { 
                    return new Tuple<Vertex, Vertex>(m_vertices[i, line], m_vertices[i, line - 1]);
                };
                vertexProviderNextLine = (i) => { 
                    return new Tuple<Vertex, Vertex>(m_vertices[i, line], m_vertices[i, line + 1]);
                };
            }
            break;
            default:
            {
                throw new ArgumentException("Invalid orientation");
            }
        }
        
        RemoveEdges(vertexProviderPreviousLine);
        RemoveEdges(vertexProviderNextLine);
    }

    void ShiftTilesInScene(Shift shift, Tile insertedTile)
    {

    }

    public void ShiftTiles(Shift shift, Tile insertedTile)
    {
        
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

    public QuickGraph.UndirectedGraph<Vertex, QuickGraph.SEquatableUndirectedEdge<Vertex>> m_graph =
        new QuickGraph.UndirectedGraph<Vertex, QuickGraph.SEquatableUndirectedEdge<Vertex>>(false);
    public Vertex [,] m_vertices = new Vertex[BoardLength, BoardLength];

    public Tile m_freeTile = null;

    public float m_tileScale = 0.9f;

    public Transform junctionTilePrefab;
    public Transform turnTilePrefab;
    public Transform straightTilePrefab;
}

} // namespace QuickGraphTest
