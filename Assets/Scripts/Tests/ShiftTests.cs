using NUnit.Framework;
using LabyrinthGame.Labyrinth;

namespace Tests
{
    public class ShiftTests
    {

        [Test]
        public void CreateShift()
        {
            var orientation = Shift.Orientation.Vertical;
            var direction = Shift.Direction.Positive;
            var index = 0;

            var shift = new Shift(orientation, direction, index);

            Assert.That(orientation, Is.EqualTo(shift.orientation), "Check that the shift orientation is set");

            Assert.That(direction, Is.EqualTo(shift.direction), "Check that the shift direction is set");

            Assert.That(index, Is.EqualTo(shift.index), "Check that the shift index is set");
        }

        [Test]
        public void CopyShift()
        {
            var shift = new Shift(Shift.Orientation.Vertical, Shift.Direction.Positive, 0);
            var copy = shift.Copy();

            Assert.That(shift.orientation, Is.EqualTo(copy.orientation), "Check that the shift and the shift cope have the same orientation");

            Assert.That(shift.direction, Is.EqualTo(copy.direction), "Check that the shift and the shift cope have the same direction");

            Assert.That(shift.index, Is.EqualTo(copy.index), "Check that the shift and the shift cope have the same index");
        }

        [Test]
        public void CompareShift()
        {
            var shift = new Shift(Shift.Orientation.Vertical, Shift.Direction.Positive, 0);
            var reference = shift;
            var copy = shift.Copy();
            var anotherShift = new Shift(Shift.Orientation.Horizontal, Shift.Direction.Positive, 0);

            Assert.That(reference, Is.EqualTo(shift), "Check that the reference is equal to the original shift");

            Assert.That(copy, Is.EqualTo(shift), "Check that the copy is equal to the original shift");

            Assert.That(anotherShift, Is.Not.EqualTo(shift), "Check that the another shift is not equal to the original shift");
        }

        [Test]
        public void InverseShift()
        {
            var shift = new Shift(Shift.Orientation.Vertical, Shift.Direction.Positive, 0);
            var inversedShift = shift.GetShiftWithInversedDirection();

            Assert.That(shift.orientation, Is.EqualTo(inversedShift.orientation), "Check that the shift and the inversed shift have the same orientation");

            Assert.That(shift.direction, Is.Not.EqualTo(inversedShift.direction), "Check that the shift and the inversed shift have different directions");

            Assert.That(shift.index, Is.EqualTo(inversedShift.index), "Check that the shift and the inversed shift has the same index");
        }

    }

}