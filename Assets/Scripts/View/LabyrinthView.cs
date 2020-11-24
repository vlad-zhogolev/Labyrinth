using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace LabyrinthGame
{

    namespace View
    {

        public class LabyrinthView : MonoBehaviour
        {
            Coroutine MoveTiles(List<AnimatedView> tiles, List<Vector3> positions)
            {
                return StartCoroutine(MoveTilesCoroutine(tiles, positions));
            }

            IEnumerator MoveTilesCoroutine(List<AnimatedView> tiles, List<Vector3> positions)
            {
                bool moving = true;

                while (moving)
                {
                    moving = false;

                    for (int i = 0; i < Labyrinth.Labyrinth.BoardLength; ++i)
                    {
                        AnimatedView tile = tiles[i];
                        Vector3 position = positions[i];

                        tile.MoveToStep(position);

                        moving = tile.transform.position != position;
                    }

                    yield return null;
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
                var removedTilePosition = m_freeTileInstance.transform.position;

                List<AnimatedView> tiles = new List<AnimatedView>(Labyrinth.Labyrinth.BoardLength);
                List<Vector3> positions = new List<Vector3>(Labyrinth.Labyrinth.BoardLength);

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

                StartCoroutine(ShiftTilesCoroutine(tiles, positions, insertedTile, insertedTilePosition, removedTile, removedTilePosition));
            }

            public void ShiftPlayers(in IList<GameLogic.Player> players)
            {
                foreach (var player in players)
                {
                    var position = new Vector3(player.Position.y - 3, 0, 3 - player.Position.x);
                    m_mageInstanceForColor[player.Color].position = position;
                }
            }

            IEnumerator ShiftTilesCoroutine(List<AnimatedView> tiles, List<Vector3> positions, AnimatedView insertedTile, Vector3 insertedTilePosition, AnimatedView removedTile, Vector3 removedTilePosition)
            {
                AnimationRunning = true;

                yield return MoveTiles(tiles, positions);

                float speed = 10;

                // Move the inserted tile

                yield return insertedTile.MoveTo(insertedTile.transform.position + new Vector3(0, 2, 0), speed);

                yield return insertedTile.MoveTo(insertedTilePosition + new Vector3(0, 2, 0), speed);

                yield return insertedTile.MoveTo(insertedTilePosition, speed);

                // Move the removed tile

                yield return removedTile.MoveTo(removedTile.transform.position + new Vector3(0, 2, 0), speed);

                yield return removedTile.MoveTo(removedTilePosition + new Vector3(0, 2, 0), speed);

                yield return removedTile.MoveTo(removedTilePosition, speed);

                AnimationRunning = false;
            }

            public void RotateFreeTile(Quaternion rotation)
            {
                StartCoroutine(nameof(RotateFreeTileCoroutine), rotation);
            }

            IEnumerator RotateFreeTileCoroutine(Quaternion rotation)
            {
                AnimationRunning = true;

                yield return m_freeTileInstance.RotateTo(rotation);

                AnimationRunning = false;
            }

            public void SetPlayerPosition(GameLogic.Color playerColor, Vector2Int indices)
            {
                var mage = m_mageInstanceForColor[playerColor];
                var position = m_tiles[indices.x, indices.y].transform.position;
                mage.position = new Vector3(position.x, 0, position.z);
            }

            private AnimatedView GetPrefabByTileType(Labyrinth.Tile.Type type)
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

            private AnimatedView InstantiateTile(Labyrinth.Tile tile, float x, float z)
            {
                AnimatedView prefab = GetPrefabByTileType(tile.type);
                var instance = Instantiate(prefab, new Vector3(x, 0, z), tile.GetRotation());
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

                m_mageInstanceForColor = new Dictionary<GameLogic.Color, Transform>()
                {
                    {GameLogic.Color.Yellow, Instantiate(m_magePrefab, new Vector3(-3.0f, 0,  3.0f), Quaternion.identity)},
                    {GameLogic.Color.Red,    Instantiate(m_magePrefab, new Vector3( 3.0f, 0,  3.0f), Quaternion.identity)},
                    {GameLogic.Color.Blue,   Instantiate(m_magePrefab, new Vector3( 3.0f, 0, -3.0f), Quaternion.identity)},
                    {GameLogic.Color.Green,  Instantiate(m_magePrefab, new Vector3(-3.0f, 0, -3.0f), Quaternion.identity)},
                };

                var yellowMage = m_mageInstanceForColor[GameLogic.Color.Yellow];
                yellowMage.GetComponent<Renderer>().material.color = Color.yellow;
                var blueMage = m_mageInstanceForColor[GameLogic.Color.Blue];
                blueMage.GetComponent<Renderer>().material.color = Color.blue;
                var greenMage = m_mageInstanceForColor[GameLogic.Color.Green];
                greenMage.GetComponent<Renderer>().material.color = Color.green;
                var redMage = m_mageInstanceForColor[GameLogic.Color.Red];
                redMage.GetComponent<Renderer>().material.color = Color.red;
            }

            public void Initialize(in Labyrinth.Tile[,] tiles, in Labyrinth.Tile freeTile)
            {
                m_tiles = new AnimatedView[Labyrinth.Labyrinth.BoardLength, Labyrinth.Labyrinth.BoardLength];

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

                InitializeMages();
            }

            public bool AnimationRunning { get; private set; } = false;

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

            private AnimatedView[,] m_tiles;

            private AnimatedView m_freeTileInstance;

            private IDictionary<GameLogic.Color, Transform> m_mageInstanceForColor;

            [SerializeField]
            private AnimatedView m_junctionTilePrefab;

            [SerializeField]
            private AnimatedView m_turnTilePrefab;

            [SerializeField]
            private AnimatedView m_straightTilePrefab;

            [SerializeField]
            private Transform m_magePrefab;

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

    }

} // namespace LabyrinthGame
