using CompanyBuildingEnhancements.Networking;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch]
    public class NetworkObjectManager
    {
        public static GameObject networkPrefab;

        public static AssetBundle CBEAssetBundle { get; private set; }

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
        public static void Init()
        {
            CBEAssetBundle = AssetBundle.LoadFromMemory(Properties.Resources.cbeassets);

            if (networkPrefab != null)
                return;

            networkPrefab = CBEAssetBundle.LoadAsset<GameObject>("Assets/Custom/CBENetworkHandler.prefab");
            networkPrefab.AddComponent<CBENetworkHandler>();

            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Awake))]
        static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
