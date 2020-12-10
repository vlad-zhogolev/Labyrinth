using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace LabyrinthGame
{
    namespace GameLogic
    {
        [Serializable]
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

            public static Dictionary<byte, object> GetPhotonCompatibleSettings()
            {
                var settings = new Dictionary<byte, object>();
                foreach (var pair in PlayersSettings)
                {
                    settings.Add((byte)pair.Key, (object)pair.Value);
                }

                return settings;
            }

            public static void SetPhotonCompatibleSettings(Dictionary<byte, object> settings)
            {
                PlayersSettings.Clear();
                foreach (var pair in settings)
                {
                    PlayersSettings.Add((GameLogic.Color)pair.Key, (PlayerSettings)pair.Value);
                }
            }

            public static void Trace()
            {
                foreach (var pair in PlayersSettings)
                {
                    Debug.LogFormat("GameSettings: Settings for {0} are IsAi: {1}, Name: {2}, ActorId: {3}", pair.Key, pair.Value.IsAi, pair.Value.Name, pair.Value.ActorId);
                }
            }

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
