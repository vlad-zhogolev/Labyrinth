using NUnit.Framework;
using LabyrinthGame.Labyrinth;
using UnityEngine.TestTools.Utils;

namespace Tests
{
    public class LabyrinthTests
    {
        [Test]
        public void CreateLabyrinth()
        {
            var labyrinth = new Labyrinth(0, 0);

            (var tiles, var freeTile) = labyrinth.GetTiles();

            Assert.That(tiles, Is.Not.Null, "Check that the tiles array is not null");

            Assert.That(freeTile, Is.Not.Null, "Check that the free tile is not null");

            Assert.That(tiles.Rank, Is.EqualTo(2), "Check that the tiles array has two dimensions");

            Assert.That(tiles.GetUpperBound(0) + 1, Is.EqualTo(Labyrinth.BoardLength), "Check that the tiles array has the right number of rows");

            Assert.That(tiles.GetUpperBound(1) + 1, Is.EqualTo(Labyrinth.BoardLength), "Check that the tiles array has the right number of columns");
        }

        [Test]
        public void ShiftLabyrinth()
        {
            var labyrinth = new Labyrinth(0, 0);

            (var tiles, var freeTile) = labyrinth.GetTiles();

            labyrinth.ShiftTiles(new Shift(Shift.Orientation.Vertical, Shift.Direction.Positive, 1));

            (var newTiles, var newFreeTile) = labyrinth.GetTiles();

            Assert.That(newTiles[0, 1].type, Is.EqualTo(freeTile.type), "Check that the inserted tile has the same type as the free tile");

            Assert.That(newTiles[0, 1].GetRotation(), Is.EqualTo(freeTile.GetRotation()).Using(QuaternionEqualityComparer.Instance), "Check that the inserted tile has the same rotation as the free tile");

            for (int i = 0; i < Labyrinth.BoardLength - 1; i++)
            {
                var tile = tiles[i, 1];
                var newTile = newTiles[i + 1, 1];

                Assert.That(tile.type, Is.EqualTo(newTile.type), $"Check that the moved tile with the index {i} has the same type");

                Assert.That(tile.GetRotation(), Is.EqualTo(newTile.GetRotation()).Using(QuaternionEqualityComparer.Instance), $"Check that the moved tile with the index {i} has the same rotation");
            }

            Assert.That(tiles[Labyrinth.BoardLength - 1, 1].type, Is.EqualTo(newFreeTile.type), "Check that the removed tile has the same type as the new free tile");

            Assert.That(tiles[Labyrinth.BoardLength - 1, 1].GetRotation(), Is.EqualTo(newFreeTile.GetRotation()).Using(QuaternionEqualityComparer.Instance), "Check that the removed tile has the same rotation as the new free tile");
        }
    }
}
