using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabyrinthGame
{
    namespace Utils
    {
        public class Randomization
        {
            public static void Shuffle<T>(IList<T> list, System.Random randomizer)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = randomizer.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
        }

    } //namespace GameLogic

} //namespace LabyrinthGame
