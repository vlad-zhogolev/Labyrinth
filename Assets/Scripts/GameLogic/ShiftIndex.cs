using System;
using System.Collections.Generic;

namespace LabyrinthGame
{

    namespace GameLogic
    {

        public static class ShiftIndex
        {

            static int[] oppositeIndices = new int[] { 8, 7, 6, 11, 10, 9, 2, 1, 0, 5, 4, 3 };

            static HashSet<int> cornerIndices = new HashSet<int> { 0, 2, 3, 5, 6, 8, 9, 11 };

            static HashSet<int> horizontalSideIndices = new HashSet<int> { 0, 2, 6, 8 };

            static HashSet<int> verticalSideIndices = new HashSet<int> { 3, 5, 9, 11 };

            static void CheckIndex(int index)
            {
                if (index < 0 && index > 11) throw new ArgumentOutOfRangeException($"The index {index} is out of range [0, 11]");
            }

            public static int Prev(int index)
            {
                CheckIndex(index);

                index--;

                if (index < 0) index = 11;

                return index;
            }

            public static int Next(int index)
            {
                CheckIndex(index);

                index++;

                if (index > 11) index = 0;

                return index;
            }

            public static int Inverse(int index)
            {
                CheckIndex(index);

                return oppositeIndices[index];
            }

            public static bool AtCorner(int index)
            {
                CheckIndex(index);

                return cornerIndices.Contains(index);
            }

            public static bool AtHorizontalSide(int index)
            {
                CheckIndex(index);

                return horizontalSideIndices.Contains(index);
            }

            public static bool AtVerticalSide(int index)
            {
                CheckIndex(index);

                return verticalSideIndices.Contains(index);
            }

        }

    } // GameLogic

} // LabyrinthGame
