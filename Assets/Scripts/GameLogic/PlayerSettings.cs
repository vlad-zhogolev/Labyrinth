using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            public int ActorId { get; set; } = -1;

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

            public static bool IsMultipleHumanPlayers()
            {
                var humanPlayersNumber = 
                    (from pair in PlayersSettings
                     where !pair.Value.IsAi
                     select pair).Count();

                return humanPlayersNumber > 1;
            }
        }

    } // namespace GameLogic

} // namespace LabyrinthGame
