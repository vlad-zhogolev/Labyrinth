using UnityEngine;
using System;


namespace LabyrinthGame {

namespace View {

public class LabyrinthView : MonoBehaviour
{
    void MoveTiles(Func<int, (Vector2Int, Vector2Int)> tilesIndicesProvider)
    {
        for (var i = 0; i < Labyrinth.Labyrinth.BoardLength - 1 ; ++i)
        {
            (var current, var next) = tilesIndicesProvider(i);
            m_tiles[current.x, current.y].position = m_tiles[next.x, next.y].position;
        }

        for (var i =  Labyrinth.Labyrinth.BoardLength - 2; i >= 0 ; --i)
        {
            (var current, var next) = tilesIndicesProvider(i);
            m_tiles[next.x, next.y] = null;
            m_tiles[next.x, next.y] = m_tiles[current.x, current.y];
        }
    }

    public void ShiftTiles(Labyrinth.Shift shift)
    {
        var line = shift.index;
        Func<int, (Vector2Int, Vector2Int)> tilesIndicesProvider;
        switch (shift.orientation)
        {
            case Labyrinth.Shift.Orientation.Horizontal:
            {
                if (shift.direction == Labyrinth.Shift.Direction.Positive)
                {
                    tilesIndicesProvider = (i) => { 
                        return (new Vector2Int(line, i), new Vector2Int(line, i + 1));
                    };
                }
                else
                {
                    tilesIndicesProvider = (i) => { 
                        return (new Vector2Int(line, Labyrinth.Labyrinth.BoardLength - i - 1), new Vector2Int(line, Labyrinth.Labyrinth.BoardLength - i - 2));
                    };
                }
            }
            break;
            case Labyrinth.Shift.Orientation.Vertical:
            {
                if (shift.direction == Labyrinth.Shift.Direction.Positive)
                {
                    tilesIndicesProvider = (i) => { 
                        return (new Vector2Int(i, line), new Vector2Int(i + 1, line));
                    };
                }
                else
                {
                    tilesIndicesProvider = (i) => { 
                        return (new Vector2Int(Labyrinth.Labyrinth.BoardLength - i - 1, line), new Vector2Int(Labyrinth.Labyrinth.BoardLength - i - 2, line));
                    };
                }
            }
            break;
            default:
            {
                throw new ArgumentException("Invalid orientation");
            }
        }

        var borderCoordinates = Labyrinth.Shift.BorderCoordinates[shift];
        var insertTileInstancePosition = m_tiles[borderCoordinates.insert.x, borderCoordinates.insert.y].position;

        var removedTile = m_tiles[borderCoordinates.remove.x, borderCoordinates.remove.y];

        MoveTiles(tilesIndicesProvider);

        removedTile.position = m_freeTileInstance.position;
        m_tiles[borderCoordinates.insert.x, borderCoordinates.insert.y] = m_freeTileInstance;
        m_tiles[borderCoordinates.insert.x, borderCoordinates.insert.y].position = insertTileInstancePosition;

        m_freeTileInstance = removedTile;
    }

    public void RotateFreeTile(Quaternion rotation)
    {
        m_freeTileInstance.rotation = rotation;
    }

    private Transform GetPrefabByTileType(Labyrinth.Tile.Type type)
    {
        switch (type)
        {
            case Labyrinth.Tile.Type.Junction:
            {
                return m_junctionTilePrefab;
            }
            case Labyrinth.Tile.Type.Turn:
            {
                return m_turnTilePrefab;
            }
            case Labyrinth.Tile.Type.Straight:
            {
                return m_straightTilePrefab;
            }
            default:
            {
                throw new ArgumentException("Invalid type provided");
            }
        }
    }

    private Transform InstantiateTile(Labyrinth.Tile tile, float x, float z)
    {
        Transform prefab = GetPrefabByTileType(tile.type);
        var instance = Instantiate(prefab, new Vector3(x, 0, z), tile.GetRotation());
        instance.localScale = new Vector3(m_tileScale, m_tileScale, m_tileScale);

        return instance;
    }

    public void Initialize(in Labyrinth.Tile[,] tiles, in Labyrinth.Tile freeTile)
    {
        m_tiles = new Transform[Labyrinth.Labyrinth.BoardLength, Labyrinth.Labyrinth.BoardLength];

        var z = 3.0f;
        var step = 1.0f;
        for (var i = 0; i < Labyrinth.Labyrinth.BoardLength; ++i)
        {
            var x = -3.0f;
            for (var j = 0; j < Labyrinth.Labyrinth.BoardLength; ++j)
            {
                var tile = tiles[i, j];
                m_tiles[i, j] = InstantiateTile(tile, x, z);

                Debug.LogFormat("{0}: tile [{1}, {2}] type = {3}, rotation = {4}",
                    GetType().Name, i, j, tile.type, tile.GetRotation());
                x += step;
            }
            z -= step;
        }

        var freeTileX = 5.0f;
        var freeTileY = 5.0f;
        m_freeTileInstance = InstantiateTile(freeTile, freeTileX, freeTileY);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField]
    private float m_tileScale = 0.95f;

    private Transform[,] m_tiles;

    private Transform m_freeTileInstance;

    [SerializeField]
    private Transform m_junctionTilePrefab;

    [SerializeField]
    private Transform m_turnTilePrefab;

    [SerializeField]
    private Transform m_straightTilePrefab;
}

}

} // namespace LabyrinthGame
