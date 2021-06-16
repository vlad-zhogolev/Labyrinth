using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LabyrinthGame.Labyrinth;
using UnityEngine.TestTools.Utils;

namespace Tests
{

    public class TileTests
    {

        [Test]
        public void CreateTile()
        {
            var type = Tile.Type.Straight;

            var tile = new Tile(type);

            Assert.That(tile.type, Is.EqualTo(type), "Check that the tile type is set right");

            Assert.That(tile.GetRotation(), Is.EqualTo(Quaternion.identity).Using(QuaternionEqualityComparer.Instance), "Check that the tile has the identity rotation");
        }

        [Test]
        public void CopyTile()
        {
            var tile = new Tile(Tile.Type.Straight);
            var copy = tile.Copy();

            Assert.That(tile.type, Is.EqualTo(copy.type), "Check that the tile and the tile copy have the same type");

            Assert.That(tile.GetRotation(), Is.EqualTo(copy.GetRotation()).Using(QuaternionEqualityComparer.Instance), "Check that the tile and the tile copy have the same rotation");
        }

        [Test]
        public void RotateTile()
        {
            var tile = new Tile(Tile.Type.Straight);

            tile.Rotate(Tile.RotationDirection.Clockwise);

            Assert.That(tile.GetRotation(), Is.EqualTo(Quaternion.Euler(0, 90, 0)).Using(QuaternionEqualityComparer.Instance), "Check that the tile is rotated clockwise");

            tile.Rotate(Tile.RotationDirection.CounterClockwise);
            tile.Rotate(Tile.RotationDirection.CounterClockwise);

            Assert.That(tile.GetRotation(), Is.EqualTo(Quaternion.Euler(0, -90, 0)).Using(QuaternionEqualityComparer.Instance), "Check that the tile is rotated counterclockwise");
        }

        [Test]
        public void Connection()
        {
            var tile = new Tile(Tile.Type.Straight);
            var anotherTile = new Tile(Tile.Type.Straight);

            Assert.That(tile.IsConnected(anotherTile, Tile.Side.Up), Is.True, "Check that the tiles are connected vertically");

            Assert.That(tile.IsConnected(anotherTile, Tile.Side.Right), Is.False, "Check that the tiles aren't connected horizontally");
        }

    }

}