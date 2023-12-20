using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

namespace CompanyBuildingEnhancements
{
    [Serializable]
    public class SyncedInstance<T>
    {
        public static bool Synced { get; internal set; }

        public static T Default { get; private set; }
        public static T Instance { get; private set; }

        internal static CustomMessagingManager MessageManager => NetworkManager.Singleton.CustomMessagingManager;
        internal static bool IsClient => NetworkManager.Singleton.IsClient;
        internal static bool IsHost => NetworkManager.Singleton.IsHost;

        protected void InitInstance(T instance)
        {
            Default = instance;
            Instance = instance;
        }

        internal static void SyncInstance(byte[] data)
        {
            Instance = DeserializeFromBytes(data);
            Synced = true;
        }

        internal static void RevertSync()
        {
            Instance = Default;
            Synced = false;
        }

        public static byte[] SerializeToBytes(T val)
        {
            BinaryFormatter bf = new();
            using MemoryStream stream = new();

            try
            {
                bf.Serialize(stream, val);
                return stream.ToArray();
            }
            catch (Exception e)
            {
                CompanyBuildingEnhancementsBase.Logger.LogError($"Error serializing instance: {e}");
                return null;
            }
        }

        public static T DeserializeFromBytes(byte[] data)
        {
            BinaryFormatter bf = new();
            using MemoryStream stream = new(data);

            try
            {
                return (T)bf.Deserialize(stream);
            }
            catch (Exception e)
            {
                CompanyBuildingEnhancementsBase.Logger.LogError($"Error deserializing instance: {e}");
                return default;
            }
        }
    }
}
