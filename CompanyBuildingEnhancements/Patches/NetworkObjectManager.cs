using CompanyBuildingEnhancements.Networking;
using HarmonyLib;
using System.IO;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch]
    public class NetworkObjectManager
    {
        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Awake))/*(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))*/]
        public static void Init()
        {
            CBENetworkHandler.Instance.Awake();
            //CBEAssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("CompanyBuildingEnhancements.Assets.cbeassets"));
            //CBEAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cbeassets"));
            string assetDirectory = Path.Combine(Path.GetDirectoryName(typeof(NetworkObjectManager).Assembly.Location), "cbeassets");
            CBEAssetBundle = AssetBundle.LoadFromFile(assetDirectory);

            if (networkPrefab != null)
                return;

            networkPrefab = CBEAssetBundle.LoadAsset<GameObject>("Assets/Custom/CBENetworkHandler.prefab");
            networkPrefab.AddComponent<CBENetworkHandler>();

            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))/*(typeof(StartOfRound), nameof(StartOfRound.Awake))*/]
        static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }

        public static GameObject networkPrefab;

        public static AssetBundle CBEAssetBundle { get; private set; }
    }
}
