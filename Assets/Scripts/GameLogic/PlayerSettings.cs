using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabyrinthGame
{
    namespace GameLogic
    {
        public class PlayerSettings
        {
            public PlayerSettings(bool isAi, string name)
            {
                Name = name;
                IsAi = isAi;
            }
            public string Name { get; set; } = string.Empty;
            public bool IsAi { get; set; } = true;

            public override string ToString()
            {
                return string.Format("IsAi: {0}, Name: '{1}'", IsAi, Name);
            }
        }

        public static class GameSettings
        {
            public static IDictionary<Color, PlayerSettings> PlayersSettings = new Dictionary<Color, PlayerSettings>()
            {
                {Color.Yellow,  null},
                {Color.Red,     null},
                {Color.Green,   null},
                {Color.Blue,    null},
            };
        }

    } // namespace GameLogic

} // namespace LabyrinthGame
