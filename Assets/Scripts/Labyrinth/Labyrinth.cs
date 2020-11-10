using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using QuikGraph.Algorithms;

namespace Labyrinth {

// TODO: Remove inheritance from MonoBehaviour. It needed only for tile instantiation
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

    #region Private static methods

    static QuikGraph.EquatableUndirectedEdge<Vertex> CreateEdge(Vertex first, Vertex second)
    {
        if (first.CompareTo(second) > 0)
        {
            return new QuikGraph.EquatableUndirectedEdge<Vertex>(second, first);
        }
        else
        {
            return new QuikGraph.EquatableUndirectedEdge<Vertex>(first, second);
        }
    }

    static QuikGraph.UndirectedEdge<Vertex> CreateEdge(int x1, int y1, int x2, int y2)
    {
        return CreateEdge(new Vertex(x1, y1), new Vertex(x2, y2));
    }

    static void Shuffle(int[] list)  
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

    #endregion



    #region Public methods



    /// <summary>
    /// Moves tiles in labyrinth according to provided shift.
    /// </summary>
    /// <param name="shift"></param>
    /// <exception cref="ArgumentException">If index in provided shift was invalid.</exception>
    public void ShiftTiles(Shift shift)
    {
        if (!IsShiftValid(shift))
        {
            throw new ArgumentException("Invalid shift provided");
        }

        RemoveEdgesForShift(shift);
        MoveTilesForShift(shift);
        AddEdgesForShift(shift);
    }

    /// <summary>
    /// Checks if there is path between tiles with specified indices for current labyrinth state.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns>True - if path is present. False - otherwise</returns>
    /// <exception cref="IndexOutOfRangeException">If index is out of bounds.</exception>
    public bool IsReachable(Vector2Int source, Vector2Int target)
    {
        if (!AreIndicesValid(source) || !AreIndicesValid(target))
        {
            throw new IndexOutOfRangeException("Invalid indices were provided");
        }

        var sourceVertex = m_vertices[source.x, source.y];
        var tryGetPath = m_graph.ShortestPathsDijkstra(edge => 1.0, sourceVertex);
        
        var targetVertex = m_vertices[target.x, target.y];
        IEnumerable<QuikGraph.EquatableUndirectedEdge<Vertex>> path;
        return tryGetPath(targetVertex, out path);
    }



    #endregion



    #region Private methods



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

    void LogEdges(IEnumerable<QuikGraph.EquatableUndirectedEdge<Vertex>> edges)
    {
        foreach (var edge in edges)
        {
            Debug.LogFormat("{0}: edge {1}", GetType().Name, edge);
        }
    }

    void LogEdges()
    {
        foreach (var edge in m_graph.Edges)
        {
            Debug.LogFormat("{0}: edge {1}", GetType().Name, edge);
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
                RotateTileRandomly(m_vertices[i, j].tile);
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
                return m_junctionTilePrefab;
            }
            case Tile.Type.Turn:
            {
                return m_turnTilePrefab;
            }
            case Tile.Type.Straight:
            {
                return m_straightTilePrefab;
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
        m_freeTileInstance = InstantiateTile(m_freeTile, freeTileX, freeTileY);
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
                    m_graph.AddEdge(CreateEdge(firstVertex, secondVertex));
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

        // TODO: use enumerations instead
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

        var source = m_vertices[1, 0];
        var startTime = Time.realtimeSinceStartup;
        var tryGetPath = m_graph.ShortestPathsDijkstra(distance => 1.0, source);
        var endTime = Time.realtimeSinceStartup;

        Debug.LogFormat("{0}: Time passed {1}", GetType().Name, endTime - startTime);

        foreach (var vertex in m_vertices)
        {
            IEnumerable<QuikGraph.EquatableUndirectedEdge<Vertex>> path;
            if (tryGetPath(vertex, out path))
            {
                var logPath = path.Aggregate(string.Empty, (log, edge) => log + ":" + edge.ToString());
                Debug.LogFormat("{0}: Shortest path from {1} to {2} is: {3}", GetType().Name, source, vertex, logPath);
            }
        }

    }

    void Initialize()
    {
        InitializeVertices();
        InitializeGraph();
        InstantiateLabyrinth();
    }

    void RemoveEdgesForShift(Shift shift)
    {
        var vertices = GetVerticesRange(shift, 0, BoardLength);
        foreach (var vertex in vertices)
        {
            m_graph.RemoveAdjacentEdgeIf(vertex, edge => true);
        }
    }   

    Vector2Int OffsetForSide(Tile.Side side)
    {
        switch (side)
        {
            case Tile.Side.Up:
            {
                return new Vector2Int(-1, 0);
            }
            case Tile.Side.Down:
            {
                return new Vector2Int(1, 0);
            }
            case Tile.Side.Left:
            {
                return new Vector2Int(0, -1);
            }
            case Tile.Side.Right:
            {
                return new Vector2Int(0, 1);
            }
            default:
            {
                throw new ArgumentException("Invalid side provided");
            }
        }
    }

    Vertex GetAdjacentVertexUnsafe(Vertex vertex, Tile.Side side)
    {
        var indices = vertex.indices + OffsetForSide(side);
        return m_vertices[indices.x, indices.y];
    }

    bool IsIndexValid(int index)
    {
        return (0 <= index) && (index < BoardLength);
    }

    bool AreIndicesValid(Vector2Int indices)
    {
        return IsIndexValid(indices.x) && IsIndexValid(indices.y);
    }

    Vertex GetAdjacentVertex(Vertex vertex, Tile.Side side)
    {
        var indices = vertex.indices + OffsetForSide(side);
        if (IsIndexValid(indices.x) && IsIndexValid(indices.y))
        {
            return m_vertices[indices.x, indices.y];
        }
        return null;
    }

    IEnumerable<Vertex> GetVerticesRange(Shift shift, int start, int length)
    {
        // TODO: checks of arguments
        Func<int, int> next;
        int end;
        if (shift.direction == Shift.Direction.Positive)
        {
            next = i => ++i;
            end = start + length;
        }
        else
        {
            next = i => --i;
            end = BoardLength - start - length - 1;
            start = BoardLength - start - 1;
        }

        if (shift.orientation == Shift.Orientation.Horizontal)
        {
            for (var i = start; i != end; i = next(i))
            {
                yield return m_vertices[shift.index, i];
            }
        }
        else
        {
            for (var i = start; i != end; i = next(i))
            {
                yield return m_vertices[i, shift.index];
            }
        }
    }

    void AddEdgesForAdjacentTilesUnsafe(IEnumerable<Vertex> vertices, IEnumerable<Tile.Side> sides)
    {
        foreach (var vertex in vertices)
        {
            foreach (var side in sides)
            {
                var adjacentVertex = GetAdjacentVertexUnsafe(vertex, side);
                if (vertex.tile.IsConnected(adjacentVertex.tile, side))
                {
                    m_graph.AddEdge(CreateEdge(vertex, adjacentVertex));
                }
            }
        }
    }

    void AddEdgesForShift(Shift shift)
    {
        IEnumerable<Tile.Side> orthogonalSides;
        IEnumerable<Tile.Side> shiftDirection;

        switch (shift.orientation)
        {
            case Shift.Orientation.Horizontal:
            {
                orthogonalSides = new Tile.Side[] {Tile.Side.Up, Tile.Side.Down};

                if (shift.direction == Shift.Direction.Positive)
                {
                    shiftDirection = new Tile.Side[] {Tile.Side.Right};
                }
                else
                {
                    shiftDirection = new Tile.Side[] {Tile.Side.Left};
                }
            }
            break;
            case Shift.Orientation.Vertical:
            {
                orthogonalSides = new Tile.Side[] {Tile.Side.Left, Tile.Side.Right};

                if (shift.direction == Shift.Direction.Positive)
                {
                    shiftDirection = new Tile.Side[] {Tile.Side.Down};
                }
                else
                {
                    shiftDirection = new Tile.Side[] {Tile.Side.Up};
                }
            }
            break;
            default:
            {
                throw new ArgumentException("Invalid orientation");
            }
        }

        var vertices = GetVerticesRange(shift, 0, BoardLength).ToList();
        AddEdgesForAdjacentTilesUnsafe(vertices, orthogonalSides);

        vertices.Remove(vertices.Last());
        AddEdgesForAdjacentTilesUnsafe(vertices, shiftDirection);
    }

    void MoveTiles(Func<int, Tuple<Vertex, Vertex>> adjacentVerticesProvider /*, Vertex removePlace */)
    {
        var lastTileInstancePosition = adjacentVerticesProvider(BoardLength - 2).Item2.TileInstance.position;
        //var lastTileInstancePosition = removePlace.TileInstance.position;
   
        for (var i = BoardLength - 2; i >= 0 ; --i)
        {
            var adjacentVertices = adjacentVerticesProvider(i);
            var vertex = adjacentVertices.Item1;
            var nextVertex = adjacentVertices.Item2;

            nextVertex.tile = vertex.tile;
            nextVertex.TileInstance = vertex.TileInstance; // Just for visual debugging
        }

        // Just for visual debugging
        for (var i = 0; i < BoardLength - 1 ; ++i)
        {
            var adjacentVertices = adjacentVerticesProvider(i);
            var vertex = adjacentVertices.Item1;
            var nextVertex = adjacentVertices.Item2;

            vertex.TileInstance.position = nextVertex.TileInstance.position;
        }

        adjacentVerticesProvider(BoardLength - 2).Item2.TileInstance.position = lastTileInstancePosition;
        //removePlace.TileInstance.position = lastTileInstancePosition;
    }

    void MoveTilesForShift(Shift shift)
    {
        var line = shift.index;
        Func<int, Tuple<Vertex, Vertex>> vertexProvider;
        switch (shift.orientation)
        {
            case Shift.Orientation.Horizontal:
            {
                if (shift.direction == Shift.Direction.Positive)
                {
                    vertexProvider = (i) => { 
                        return new Tuple<Vertex, Vertex>(m_vertices[line, i], m_vertices[line, i + 1]);
                    };
                }
                else
                {
                    vertexProvider = (i) => { 
                        return new Tuple<Vertex, Vertex>(m_vertices[line, BoardLength - i - 1], m_vertices[line, BoardLength - i - 2]);
                    };
                }
            }
            break;
            case Shift.Orientation.Vertical:
            {
                if (shift.direction == Shift.Direction.Positive)
                {
                    vertexProvider = (i) => { 
                        return new Tuple<Vertex, Vertex>(m_vertices[i, line], m_vertices[i + 1, line]);
                    };
                }
                else
                {
                    vertexProvider = (i) => { 
                        return new Tuple<Vertex, Vertex>(m_vertices[BoardLength - i - 1, line], m_vertices[BoardLength - i - 2, line]);
                    };
                }
            }
            break;
            default:
            {
                throw new ArgumentException("Invalid orientation");
            }
        }

        var borderCoordinates = Shift.BorderCoordinates[shift];
        var insertPlace = m_vertices[borderCoordinates.insert.x, borderCoordinates.insert.y];
        var insertTileInstancePosition = insertPlace.TileInstance.position;

        var removePlace = m_vertices[borderCoordinates.remove.x, borderCoordinates.remove.y];
        var removedTile = removePlace.tile;
        var removedTileInstance = removePlace.TileInstance;

        MoveTiles(vertexProvider /*, removePlace*/);

        removedTileInstance.position = m_freeTileInstance.position;
        insertPlace.tile = m_freeTile;
        insertPlace.TileInstance = m_freeTileInstance;
        insertPlace.TileInstance.position = insertTileInstancePosition;

        m_freeTile = removedTile;
        m_freeTileInstance = removedTileInstance;
    }

    bool IsShiftValid(Shift shift)
    {
        return IsIndexValid(shift.index) && shift.index % 2 == 1;
    }

    // TODO: Replace with constructor when MonoBehaviour will be removed
    void Start()
    {
        Initialize();
    }

    #endregion



    #region Private fields



    private QuikGraph.UndirectedGraph<Vertex, QuikGraph.EquatableUndirectedEdge<Vertex>> m_graph =
        new QuikGraph.UndirectedGraph<Vertex, QuikGraph.EquatableUndirectedEdge<Vertex>>(false);
    
    private Vertex [,] m_vertices = new Vertex[BoardLength, BoardLength];

    public Dictionary<Shift, Tuple<int, int, int, int>> m_shiftsToCoordinates = new Dictionary<Shift, Tuple<int, int, int, int>>();

    private Tile m_freeTile = null;
    private Transform m_freeTileInstance = null;

    [SerializeField]
    private float m_tileScale = 0.9f;

    [SerializeField]
    private Transform m_junctionTilePrefab;

    [SerializeField]
    private Transform m_turnTilePrefab;

    [SerializeField]
    private Transform m_straightTilePrefab;



    #endregion
}

} // namespace QuickGraphTest
