using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace LabyrinthGame
{
    namespace View
    {

        public enum TileMovement
        {
            NORMAL, HORIZONTAL_TO_VERTICAL, VERTICAL_TO_HORIZONTAL
        }

        public class LabyrinthView : MonoBehaviour
        {

            public readonly static Vector2Int FREE_TILE_POSITION = new Vector2Int(-1, -1);

            async Task MoveTiles(List<AnimatedView> tiles, List<Vector3> positions)
            {
                var tasks = new Task[tiles.Count];

                for (int i = 0; i < tiles.Count; ++i)
                {
                    AnimatedView tile = tiles[i];
                    Vector3 position = positions[i];

                    tasks[i] = tile.MoveTo(position);
                }

                await Task.WhenAll(tasks);
            }

            public async Task ShiftTiles(int shiftIndex, Labyrinth.Shift shift)
            {
                var line = shift.index;
                Func<int, (Vector2Int, Vector2Int)> tilesIndicesProvider;
                switch (shift.orientation)
                {
                    case Labyrinth.Shift.Orientation.Horizontal:
                    {
                        if (shift.direction == Labyrinth.Shift.Direction.Positive)
                        {
                            tilesIndicesProvider = (i) =>
                            {
                                return (new Vector2Int(line, i), new Vector2Int(line, i + 1));
                            };
                        }
                        else
                        {
                            tilesIndicesProvider = (i) =>
                            {
                                return (new Vector2Int(line, Labyrinth.Labyrinth.BoardLength - i - 1), new Vector2Int(line, Labyrinth.Labyrinth.BoardLength - i - 2));
                            };
                        }
                    }
                    break;
                    case Labyrinth.Shift.Orientation.Vertical:
                    {
                        if (shift.direction == Labyrinth.Shift.Direction.Positive)
                        {
                            tilesIndicesProvider = (i) =>
                            {
                                return (new Vector2Int(i, line), new Vector2Int(i + 1, line));
                            };
                        }
                        else
                        {
                            tilesIndicesProvider = (i) =>
                            {
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

                var insertedTile = m_freeTileInstance;
                var insertedTilePosition = m_tiles[borderCoordinates.insert.x, borderCoordinates.insert.y].transform.position;

                var removedTile = m_tiles[borderCoordinates.remove.x, borderCoordinates.remove.y];

                var tiles = new List<AnimatedView>(Labyrinth.Labyrinth.BoardLength);
                var positions = new List<Vector3>(Labyrinth.Labyrinth.BoardLength);

                tiles.Add(insertedTile);
                positions.Add(insertedTilePosition);

                for (var i = 0; i < Labyrinth.Labyrinth.BoardLength; ++i)
                {
                    (var current, var next) = tilesIndicesProvider(i);
                    tiles.Add(m_tiles[current.x, current.y]);
                    positions.Add(m_tiles[current.x, current.y].transform.position + new Vector3(next.y - current.y, 0, current.x - next.x));
                }

                for (var i = Labyrinth.Labyrinth.BoardLength - 2; i >= 0; --i)
                {
                    (var current, var next) = tilesIndicesProvider(i);
                    m_tiles[next.x, next.y] = m_tiles[current.x, current.y];
                }

                m_tiles[borderCoordinates.insert.x, borderCoordinates.insert.y] = m_freeTileInstance;

                m_freeTileInstance = removedTile;

                await insertedTile.MoveTo((insertedTile.transform.position + insertedTilePosition) / 2);

                await MoveTiles(tiles, positions);

                foreach (var entry in m_mageInstanceForColor)
                {
                    var color = entry.Key;
                    var mage = entry.Value;

                    if (mage.transform.parent == removedTile.transform)
                    {
                        mage.transform.SetParent(null);
                        await mage.CarryTo(insertedTilePosition + m_magePlaceForColor[color], 1, 10);
                        mage.transform.SetParent(insertedTile.transform);
                    }
                }

                await removedTile.MoveTo(m_freeTilePositions[GameLogic.ShiftIndex.Inverse(shiftIndex)]);
            }

            public async Task RotateFreeTile(Quaternion rotation)
            {
                await m_freeTileInstance.RotateTo(rotation);
            }

            public async Task MoveFreeTile(int shiftIndex, Quaternion rotation, TileMovement tileMovement)
            {
                float speed = 8;

                var position = m_freeTilePositions[shiftIndex];

                if (tileMovement == TileMovement.HORIZONTAL_TO_VERTICAL)
                {
                    await m_freeTileInstance.MoveTo(new Vector3(position.x, 0, m_freeTileInstance.transform.position.z), speed);
                }
                else if (tileMovement == TileMovement.VERTICAL_TO_HORIZONTAL)
                {
                    await m_freeTileInstance.MoveTo(new Vector3(m_freeTileInstance.transform.position.x, 0, position.z), speed);
                }

                if (tileMovement != TileMovement.NORMAL) await m_freeTileInstance.RotateTo(rotation);

                await m_freeTileInstance.MoveTo(position, speed);
            }

            public async Task SetPlayerPosition(GameLogic.Color playerColor, Vector2Int indices)
            {
                var mage = m_mageInstanceForColor[playerColor];
                var tile = m_tiles[indices.x, indices.y];
                var position = tile.transform.position + m_magePlaceForColor[playerColor];
                mage.transform.SetParent(null);
                await mage.CarryTo(position, 1, 10);
                mage.transform.SetParent(tile.transform);
            }

            public Vector2Int GetTilePosition(TileView tile)
            {
                for (int i = 0; i < Labyrinth.Labyrinth.BoardLength; i++)
                {
                    for (int j = 0; j < Labyrinth.Labyrinth.BoardLength; j++)
                    {
                        if (m_tiles[i, j] == tile) return new Vector2Int(i, j);
                    }
                }

                return FREE_TILE_POSITION;
            }

            private TileView GetPrefabByTileType(Labyrinth.Tile.Type type)
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

            private TileView InstantiateTile(Labyrinth.Tile tile, float x, float z)
            {
                TileView prefab = GetPrefabByTileType(tile.type);
                var instance = Instantiate(prefab, new Vector3(x, 0, z), tile.GetRotation());

                if (tile.Item != Labyrinth.Item.None)
                {
                    var item = Instantiate(m_itemProvider.GetItemPrefab(tile.Item), new Vector3(x, 0.1f, z), Quaternion.Euler(0, 180, 0));
                    item.localScale = new Vector3(m_itemScale, m_itemScale, m_itemScale);
                    item.SetParent(instance.transform);
                }

                instance.transform.localScale = new Vector3(m_tileScale, m_tileScale, m_tileScale);

                return instance;
            }

            void InitializeMages()
            {
                m_materialForColor = new Dictionary<GameLogic.Color, Material>()
                {
                    {GameLogic.Color.Yellow, m_yellowMaterial},
                    {GameLogic.Color.Red,    m_redMaterial},
                    {GameLogic.Color.Blue,   m_blueMaterial},
                    {GameLogic.Color.Green,  m_greenMaterial},
                };

                m_mageInstanceForColor = new Dictionary<GameLogic.Color, AnimatedView>()
                {
                    {GameLogic.Color.Yellow, Instantiate(m_magePrefab, new Vector3(-3.0f, 0,  3.0f) + m_magePlaceForColor[GameLogic.Color.Yellow], Quaternion.identity)},
                    {GameLogic.Color.Red,    Instantiate(m_magePrefab, new Vector3( 3.0f, 0,  3.0f) + m_magePlaceForColor[GameLogic.Color.Red], Quaternion.identity)},
                    {GameLogic.Color.Blue,   Instantiate(m_magePrefab, new Vector3( 3.0f, 0, -3.0f) + m_magePlaceForColor[GameLogic.Color.Blue], Quaternion.identity)},
                    {GameLogic.Color.Green,  Instantiate(m_magePrefab, new Vector3(-3.0f, 0, -3.0f) + m_magePlaceForColor[GameLogic.Color.Green], Quaternion.identity)},
                };

                var yellowMage = m_mageInstanceForColor[GameLogic.Color.Yellow];
                yellowMage.GetComponent<Renderer>().material.color = Color.yellow;
                yellowMage.transform.SetParent(m_tiles[0, 0].transform);

                var blueMage = m_mageInstanceForColor[GameLogic.Color.Blue];
                blueMage.GetComponent<Renderer>().material.color = Color.blue;
                blueMage.transform.SetParent(m_tiles[6, 6].transform);

                var greenMage = m_mageInstanceForColor[GameLogic.Color.Green];
                greenMage.GetComponent<Renderer>().material.color = Color.green;
                greenMage.transform.SetParent(m_tiles[6, 0].transform);

                var redMage = m_mageInstanceForColor[GameLogic.Color.Red];
                redMage.GetComponent<Renderer>().material.color = Color.red;
                redMage.transform.SetParent(m_tiles[0, 6].transform);
            }

            public void Initialize(in Labyrinth.Tile[,] tiles, in Labyrinth.Tile freeTile)
            {
                m_tiles = new TileView[Labyrinth.Labyrinth.BoardLength, Labyrinth.Labyrinth.BoardLength];

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

                var freeTileX = m_freeTilePositions[0].x;
                var freeTileZ = m_freeTilePositions[0].z;
                m_freeTileInstance = InstantiateTile(freeTile, freeTileX, freeTileZ);

                for (int i = 0; i < 3; i++)
                {
                    var marker = Instantiate(m_markerPrefab, new Vector3(-4, 0, 2 - i * 2), Quaternion.Euler(0, 270, 0));
                    marker.localScale = new Vector3(m_markerScale, m_markerScale, m_markerScale);
                }

                for (int i = 0; i < 3; i++)
                {
                    var marker = Instantiate(m_markerPrefab, new Vector3(4, 0, 2 - i * 2), Quaternion.Euler(0, 90, 0));
                    marker.localScale = new Vector3(m_markerScale, m_markerScale, m_markerScale);
                }

                for (int i = 0; i < 3; i++)
                {
                    var marker = Instantiate(m_markerPrefab, new Vector3(-2 + i * 2, 0, 4), Quaternion.identity);
                    marker.localScale = new Vector3(m_markerScale, m_markerScale, m_markerScale);
                }

                for (int i = 0; i < 3; i++)
                {
                    var marker = Instantiate(m_markerPrefab, new Vector3(-2 + i * 2, 0, -4), Quaternion.Euler(0, 180, 0));
                    marker.localScale = new Vector3(m_markerScale, m_markerScale, m_markerScale);
                }

                InitializeMages();
            }

            void Awake()
            {
                m_itemProvider = GameObject.Find("Item Provider").GetComponent<ItemProvider>();
            }

            // Update is called once per frame
            void Update()
            {

            }

            Vector3[] m_freeTilePositions = new Vector3[]
            {
                new Vector3(-2, 0, 5),
                new Vector3(0, 0, 5),
                new Vector3(2, 0, 5),

                new Vector3(5, 0, 2),
                new Vector3(5, 0, 0),
                new Vector3(5, 0, -2),

                new Vector3(2, 0, -5),
                new Vector3(0, 0, -5),
                new Vector3(-2, 0, -5),

                new Vector3(-5, 0, -2),
                new Vector3(-5, 0, 0),
                new Vector3(-5, 0, 2)
            };

            [SerializeField]
            private float m_tileScale = 0.98f;

            [SerializeField]
            float m_itemScale = 0.4f;

            [SerializeField]
            float m_markerScale = 0.25f;

            private TileView[,] m_tiles;

            private TileView m_freeTileInstance;

            private IDictionary<GameLogic.Color, AnimatedView> m_mageInstanceForColor;

            IDictionary<GameLogic.Color, Vector3> m_magePlaceForColor = new Dictionary<GameLogic.Color, Vector3>
            {
                { GameLogic.Color.Yellow, new Vector3(-0.25f, 0.4f, 0.25f) },
                { GameLogic.Color.Red, new Vector3(0.25f, 0.4f, 0.25f) },
                { GameLogic.Color.Blue, new Vector3(0.25f, 0.4f, -0.25f) },
                { GameLogic.Color.Green, new Vector3(-0.25f, 0.4f, -0.25f) }
            };

            ItemProvider m_itemProvider;

            [SerializeField]
            private TileView m_junctionTilePrefab;

            [SerializeField]
            private TileView m_turnTilePrefab;

            [SerializeField]
            private TileView m_straightTilePrefab;

            [SerializeField]
            private Transform m_markerPrefab;

            [SerializeField]
            private AnimatedView m_magePrefab;

            [SerializeField]
            private Material m_redMaterial;

            [SerializeField]
            private Material m_blueMaterial;

            [SerializeField]
            private Material m_greenMaterial;

            [SerializeField]
            private Material m_yellowMaterial;

            private IDictionary<GameLogic.Color, Material> m_materialForColor;
        }

    } // namespace View

} // namespace LabyrinthGame
