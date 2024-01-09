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
        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
        public static void Init()
        {
            AssetBundle CBEAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cbeassets"));

            if (networkPrefab != null)
                return;

            networkPrefab = (GameObject)CBEAssetBundle.LoadAsset("CBENetworkHandler");
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

        static GameObject networkPrefab;
    }
}
