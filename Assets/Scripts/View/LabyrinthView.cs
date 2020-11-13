using UnityEngine;
using System;

namespace LabyrinthGame {

namespace View {

public class LabyrinthView : MonoBehaviour
{
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

    public void Initialize(in Labyrinth.Tile[,] tiles, in Labyrinth.Tile freeTile, Vector2 origin)
    {
        // TODO: check that origin is in horizontal plane
        m_origin = origin;

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

    private Vector2 m_origin;

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
