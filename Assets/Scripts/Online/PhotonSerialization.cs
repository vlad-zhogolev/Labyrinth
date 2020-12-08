using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace LabyrinthGame
{
    namespace PhotonSerialization
    {
        public class PhotonSerialization : MonoBehaviour
        {
            public static byte[] GameLogicColorSerialize(object obj)
            {
                var color = (GameLogic.Color) obj;
                return new byte[] { (byte)color };
            }

            public static object GameLogicColorDeserialize(byte[] data)
            {
                return (GameLogic.Color) data[0];
            }

            public static byte[] GameLogicPlayerSettingsSerialize(object obj)
            {
                // See: https://forum.unity.com/threads/pun-serialization.405031/

                var settings = (GameLogic.PlayerSettings)obj;
                using (var memoryStream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, settings);

                    return memoryStream.ToArray();
                }
            }

            public static object GameLogicPlayerSettingsDeserialize(byte[] data)
            {
                using (var memoryStream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    memoryStream.Write(data, 0, data.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return (GameLogic.PlayerSettings)formatter.Deserialize(memoryStream);
                }
            }

            void Awake()
            {
                // See this for reserved codes: https://forum.photonengine.com/discussion/9314/explain-how-to-use-byte-code-on-photonpeer-registertype
                PhotonPeer.RegisterType(typeof(GameLogic.Color),          1, GameLogicColorSerialize,           GameLogicColorDeserialize);
                PhotonPeer.RegisterType(typeof(GameLogic.PlayerSettings), 2, GameLogicPlayerSettingsSerialize,  GameLogicPlayerSettingsDeserialize);
            }
        }
    }
}


