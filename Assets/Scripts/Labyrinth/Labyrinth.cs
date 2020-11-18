using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using QuikGraph.Algorithms;

namespace LabyrinthGame
{
    namespace Labyrinth
    {
        public class Labyrinth
        {
            public static readonly int BoardLength = 7;
            public static readonly int TilesNumber = 49;
            public static readonly int FixedTilesNumber = 16;
            public static readonly int MovableTilesNumber = 34;
            public static readonly int MovableJunctionTilesNumber = 6;
            public static readonly int MovableTurnTilesNumber = 15;
            public static readonly int MovableStraightTilesNumber = 13;
            public static readonly int TileSidesNumber = 4;

            private static readonly string DumpName = "LabyrinthDump {0}";
            private static readonly string DumpFolder = Application.dataPath + @"\..\Logs\LabyrinthDumps\";

            public static readonly Tile[] MovableTiles = CreateMovableTiles();

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

            static void Shuffle(int[] list, System.Random randomizer)
            {
                int n = list.Length;
                while (n > 1)
                {
                    n--;
                    int k = randomizer.Next(n + 1);
                    int value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }

            private static Tile[] CreateMovableTiles()
            {
                //Tile[] tiles = new Tile[Labyrinth.MovableTilesNumber];
                //var endIndex = Labyrinth.MovableStraightTilesNumber;
                //for (var i = 0; i < endIndex; ++i)
                //{
                //    tiles[i] = new Tile(Tile.Type.Straight);
                //}
                //var startIndex = endIndex;
                //endIndex += Labyrinth.MovableTurnTilesNumber;
                //for (var i = startIndex; i < endIndex; ++i)
                //{
                //    tiles[i] = new Tile(Tile.Type.Turn);
                //}
                //startIndex = endIndex;
                //endIndex += Labyrinth.MovableJunctionTilesNumber;
                //for (var i = startIndex; i < endIndex; ++i)
                //{
                //    tiles[i] = new Tile(Tile.Type.Junction);
                //}

                var straightTiles = new Tile[MovableStraightTilesNumber];
                for (var i = 0; i < straightTiles.Length; ++i)
                {
                    straightTiles[i] = new Tile(Tile.Type.Straight);
                }

                var turnTiles = new Tile[MovableTurnTilesNumber];
                for (var i = 0; i < turnTiles.Length; ++i)
                {
                    turnTiles[i] = new Tile(Tile.Type.Turn);
                }
                turnTiles[0].Item = Item.Item13;
                turnTiles[1].Item = Item.Item14;
                turnTiles[2].Item = Item.Item15;
                turnTiles[3].Item = Item.Item16;
                turnTiles[4].Item = Item.Item17;
                turnTiles[5].Item = Item.Item18;

                var junctionTiles = new Tile[MovableJunctionTilesNumber];
                for (var i = 0; i < junctionTiles.Length; ++i)
                {
                    junctionTiles[i] = new Tile(Tile.Type.Junction);
                }
                junctionTiles[0].Item = Item.Item19;
                junctionTiles[1].Item = Item.Item20;
                junctionTiles[2].Item = Item.Item21;
                junctionTiles[3].Item = Item.Item22;
                junctionTiles[4].Item = Item.Item23;
                junctionTiles[5].Item = Item.Item24;

                return straightTiles.Concat(turnTiles).Concat(junctionTiles).ToArray();
            }

            

            #endregion



            #region Constructors

            /// <summary>
            /// Initialized labyrith based on position and rotation seeds.
            /// </summary>
            /// <param name="positionSeed"></param>
            /// <param name="rotationSeed"></param>
            public Labyrinth(int positionSeed, int rotationSeed)
            {
                m_positionSeed = positionSeed;
                m_rotationSeed = rotationSeed;
                Initialize();
            }

            #endregion



            #region Public methods

            public void Dump()
            {
                var time = DateTime.UtcNow.ToString();
                var path = DumpFolder + @"\" + string.Format(DumpName, time).Replace(':', '_').Replace(' ', '_').Replace('.', '_') + ".txt";

                if (!System.IO.Directory.Exists(DumpFolder))
                {
                    System.IO.Directory.CreateDirectory(DumpFolder);
                }

                Debug.LogFormat("{0}: Saving dump {1}", GetType().Name, path);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                {
                    file.WriteLine("    0     1     2     3     4     5     6  ");
                    for (var i = 0; i < BoardLength; ++i)
                    {
                        var upLine = "  ";
                        for (var j = 0; j < BoardLength; ++j)
                        {
                            var tile = m_vertices[i, j].tile;
                            string itemNumber;
                            if (tile.Item == Item.None)
                            {
                                itemNumber = "  ";
                            }
                            else
                            {
                                itemNumber = tile.Item.ToString();
                                itemNumber = itemNumber.Remove(0, 4);
                                itemNumber = itemNumber.Length == 1 ? " " + itemNumber : itemNumber;
                            }                            
                            if (m_vertices[i, j].tile.up)
                            {
                                upLine += "  |";
                            }
                            else
                            {
                                upLine += "   ";
                            }
                            upLine += itemNumber + " ";
                        }
                        var middleLine = i.ToString() + " ";
                        for (var j = 0; j < BoardLength; ++j)
                        {
                            if (m_vertices[i, j].tile.left)
                            {
                                middleLine += "--";
                            }
                            else
                            {
                                middleLine += "  ";
                            }
                            middleLine += "+";
                            if (m_vertices[i, j].tile.right)
                            {
                                middleLine += "-- ";
                            }
                            else
                            {
                                middleLine += "   ";
                            }
                        }
                        var bottomLine = "  ";
                        for (var j = 0; j < BoardLength; ++j)
                        {
                            if (m_vertices[i, j].tile.down)
                            {
                                bottomLine += "  |   ";
                            }
                            else
                            {
                                bottomLine += "      ";
                            }
                        }
                        file.WriteLine(upLine);
                        file.WriteLine(middleLine);
                        file.WriteLine(bottomLine);
                    }

                    string freeTileItemNumber;
                    if (m_freeTile.Item == Item.None)
                    {
                        freeTileItemNumber = "  ";
                    }
                    else
                    {
                        freeTileItemNumber = m_freeTile.Item.ToString();
                        freeTileItemNumber = freeTileItemNumber.Remove(0, 4);
                        freeTileItemNumber = freeTileItemNumber.Length == 1 ? " " + freeTileItemNumber : freeTileItemNumber;
                    }

                    var freeTileUpLine = "  ";
                    if (m_freeTile.up)
                    {
                        freeTileUpLine += "  |";
                    }
                    else
                    {
                        freeTileUpLine += "   ";
                    }
                    freeTileUpLine += freeTileItemNumber + " ";
                    var freeTileMiddleLine = "  ";
                    if (m_freeTile.left)
                    {
                        freeTileMiddleLine += "--";
                    }
                    else
                    {
                        freeTileMiddleLine += "  ";
                    }

                    freeTileMiddleLine += "+";
                    if (m_freeTile.right)
                    {
                        freeTileMiddleLine += "-- ";
                    }
                    else
                    {
                        freeTileMiddleLine += "   ";
                    }

                    var freeTileBottomLine = "  ";
                    for (var j = 0; j < BoardLength; ++j)
                        if (m_freeTile.down)
                        {
                            freeTileBottomLine += "  |   ";
                        }
                        else
                        {
                            freeTileBottomLine += "      ";
                        }
                    file.WriteLine("\nfree tile:");
                    file.WriteLine(freeTileUpLine);
                    file.WriteLine(freeTileMiddleLine);
                    file.WriteLine(freeTileBottomLine);
                }


            }

            public Quaternion GetFreeTileRotation()
            {
                return m_freeTile.GetRotation();
            }

            public Item GetTileItem(Vector2Int tileCoordinates)
            {
                if (!AreIndicesValid(tileCoordinates))
                {
                    throw new IndexOutOfRangeException("Invalid indices were provided");
                }

                return m_vertices[tileCoordinates.x, tileCoordinates.y].tile.Item;
            }

            public void RotateFreeTile(Tile.RotationDirection rotationDirection)
            {
                m_freeTile.Rotate(rotationDirection);
            }

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

            /// <summary>
            /// 
            /// </summary>
            /// <returns> Copies of labyrinth's tiles and free tile</returns>
            public (Tile[,], Tile) GetTiles()
            {
                var tiles = new Tile[BoardLength, BoardLength];
                for (var i = 0; i < BoardLength; ++i)
                {
                    for (var j = 0; j < BoardLength; ++j)
                    {
                        tiles[i, j] = m_vertices[i, j].tile.Copy();
                    }
                }
                var freeTile = m_freeTile.Copy();

                return (tiles, freeTile);
            }

            #endregion



            #region Private methods



            bool IsTileFixed(int i, int j)
            {
                return (i % 2 == 0) && (j % 2 == 0);
            }

            void RotateTileRandomly(Tile tile, System.Random rotationRandomizer)
            {
                if (tile == null)
                {
                    return;
                }

                var rotationsNumber = rotationRandomizer.Next(TileSidesNumber);
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
                m_vertices = new Vertex[BoardLength, BoardLength];
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
                m_vertices[0, 2].tile.Item = Item.Item1;
                m_vertices[0, 4].tile = junctionTile.Copy();
                m_vertices[0, 4].tile.Item = Item.Item2;

                junctionTile.RotateCW();
                m_vertices[2, 6].tile = junctionTile.Copy();
                m_vertices[2, 6].tile.Item = Item.Item3;
                m_vertices[4, 6].tile = junctionTile.Copy();
                m_vertices[4, 6].tile.Item = Item.Item4;

                junctionTile.RotateCW();
                m_vertices[6, 2].tile = junctionTile.Copy();
                m_vertices[6, 2].tile.Item = Item.Item5;
                m_vertices[6, 4].tile = junctionTile.Copy();
                m_vertices[6, 4].tile.Item = Item.Item6;

                junctionTile.RotateCW();
                m_vertices[2, 0].tile = junctionTile.Copy();
                m_vertices[2, 0].tile.Item = Item.Item7;
                m_vertices[4, 0].tile = junctionTile.Copy();
                m_vertices[4, 0].tile.Item = Item.Item8;

                // Inner fixed tiles
                m_vertices[2, 2].tile = junctionTile.Copy();
                m_vertices[2, 2].tile.Item = Item.Item9;

                junctionTile.RotateCW();
                m_vertices[2, 4].tile = junctionTile.Copy();
                m_vertices[2, 4].tile.Item = Item.Item10;

                junctionTile.RotateCW();
                m_vertices[4, 4].tile = junctionTile.Copy();
                m_vertices[4, 4].tile.Item = Item.Item11;

                junctionTile.RotateCW();
                m_vertices[4, 2].tile = junctionTile.Copy();
                m_vertices[4, 2].tile.Item = Item.Item12;

                int[] range = new int[MovableTilesNumber];
                for (var i = 0; i < range.Length; ++i)
                {
                    range[i] = i;
                }

                var positionRandomier = new System.Random(m_positionSeed);
                Shuffle(range, positionRandomier);

                var rotationRandomizer = new System.Random(m_rotationSeed);
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

                        m_vertices[i, j].tile = MovableTiles[range[counter]].Copy();
                        RotateTileRandomly(m_vertices[i, j].tile, rotationRandomizer);
                        ++counter;
                    }
                }

                // Free tile
                m_freeTile = MovableTiles[range[counter]].Copy();
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
                m_graph = new QuikGraph.UndirectedGraph<Vertex, QuikGraph.EquatableUndirectedEdge<Vertex>>(false);

                for (var i = 0; i < BoardLength; ++i)
                {
                    for (var j = 0; j < BoardLength; ++j)
                    {
                        m_graph.AddVertex(m_vertices[i, j]);
                    }
                }

                // TODO: use GetVerticesRange instead
                AddEdges(
                    (i, j) =>
                    {
                        return new Tuple<Vertex, Vertex>(m_vertices[i, j], m_vertices[i + 1, j]);
                    },
                    Tile.Side.Down
                );
                AddEdges(
                    (i, j) =>
                    {
                        return new Tuple<Vertex, Vertex>(m_vertices[j, i], m_vertices[j, i + 1]);
                    },
                    Tile.Side.Right
                );
            }

            void Initialize()
            {
                InitializeVertices();
                InitializeGraph();
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
                        orthogonalSides = new Tile.Side[] { Tile.Side.Up, Tile.Side.Down };

                        if (shift.direction == Shift.Direction.Positive)
                        {
                            shiftDirection = new Tile.Side[] { Tile.Side.Right };
                        }
                        else
                        {
                            shiftDirection = new Tile.Side[] { Tile.Side.Left };
                        }
                    }
                    break;
                    case Shift.Orientation.Vertical:
                    {
                        orthogonalSides = new Tile.Side[] { Tile.Side.Left, Tile.Side.Right };

                        if (shift.direction == Shift.Direction.Positive)
                        {
                            shiftDirection = new Tile.Side[] { Tile.Side.Down };
                        }
                        else
                        {
                            shiftDirection = new Tile.Side[] { Tile.Side.Up };
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
                for (var i = BoardLength - 2; i >= 0; --i)
                {
                    var adjacentVertices = adjacentVerticesProvider(i);
                    var vertex = adjacentVertices.Item1;
                    var nextVertex = adjacentVertices.Item2;

                    nextVertex.tile = vertex.tile;
                }
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
                            vertexProvider = (i) =>
                            {
                                return new Tuple<Vertex, Vertex>(m_vertices[line, i], m_vertices[line, i + 1]);
                            };
                        }
                        else
                        {
                            vertexProvider = (i) =>
                            {
                                return new Tuple<Vertex, Vertex>(m_vertices[line, BoardLength - i - 1], m_vertices[line, BoardLength - i - 2]);
                            };
                        }
                    }
                    break;
                    case Shift.Orientation.Vertical:
                    {
                        if (shift.direction == Shift.Direction.Positive)
                        {
                            vertexProvider = (i) =>
                            {
                                return new Tuple<Vertex, Vertex>(m_vertices[i, line], m_vertices[i + 1, line]);
                            };
                        }
                        else
                        {
                            vertexProvider = (i) =>
                            {
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
                var removePlace = m_vertices[borderCoordinates.remove.x, borderCoordinates.remove.y];
                var removedTile = removePlace.tile;

                MoveTiles(vertexProvider);

                insertPlace.tile = m_freeTile;
                m_freeTile = removedTile;
            }

            bool IsShiftValid(Shift shift)
            {
                return IsIndexValid(shift.index) && shift.index % 2 == 1;
            }

            #endregion



            #region Private fields



            private QuikGraph.UndirectedGraph<Vertex, QuikGraph.EquatableUndirectedEdge<Vertex>> m_graph;

            private Vertex[,] m_vertices;

            private Tile m_freeTile = null;

            private int m_positionSeed;
            private int m_rotationSeed;

            #endregion
        }

    } // namespace Labyrinth

} // namespace LabyrinthGame