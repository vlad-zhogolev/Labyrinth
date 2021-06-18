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
                return string.Format("IsAi: {0}, Name: '{1}', ActorId: {2}", IsAi, Name, ActorId);
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

            public static int TilesPositionsSeed;
            public static int TilesRotationsSeed;
            public static int ItemsSeed;

            public static object[] GetPhotonCompatibleSettings()
            {
                var playerSettings = new Dictionary<byte, object>();

                foreach (var pair in PlayersSettings)
                {
                    playerSettings.Add((byte)pair.Key, (object)pair.Value);
                }

                var settings = new List<object>();
                settings.Add(playerSettings);
                settings.Add(TilesPositionsSeed);
                settings.Add(TilesRotationsSeed);

                return settings.ToArray();
            }

            public static void SetPhotonCompatibleSettings(object[] settings)
            {
                PlayersSettings.Clear();

                var playerSettings = (Dictionary<byte, object>)settings[0];
                foreach (var pair in playerSettings)
                {
                    PlayersSettings.Add((GameLogic.Color)pair.Key, (PlayerSettings)pair.Value);
                }

                Trace();

                TilesPositionsSeed = (int)settings[1];
                TilesRotationsSeed = (int)settings[2];

                Debug.LogFormat("GameSettings: Settings TilesPositionsSeed {0}  TilesRotationsSeed: {1}", TilesPositionsSeed, TilesRotationsSeed);
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
