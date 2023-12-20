using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;

namespace CompanyBuildingEnhancements
{
    [Serializable]
    public class Config : SyncedInstance<Config>
    {
        public bool enableInstantLandingAtCompanyConfig { get; private set; }
        public bool enableAutoLandingOnDeadlineConfig { get; private set; }
        public bool enableInfiniteSprintAtCompanyConfig { get; private set; }

        public Config(ConfigFile cfg)
        {
            InitInstance(this);

            enableInstantLandingAtCompanyConfig = cfg.Bind("Company Building Enhancements",
                                                     "Instant Landing At Company Building",
                                                     true,
                                                     "If set to true, the ship will land instantly at the Company Building. You will be able to run to the shelf to sell your items as soon as the ship doors open. This setting will automatically sync with the host's config file to avoid any desync issues.").Value;
            enableAutoLandingOnDeadlineConfig = cfg.Bind("Company Building Enhancements",
                                                     "Automatic Landing At Company Building When Deadline Reaches 0 Days",
                                                     false,
                                                     "If set to true, the ship will automatically re-route & land at the Company Building when the deadline reaches 0 days. This setting will automatically sync with the host's config file to avoid any desync issues.").Value;
            enableInfiniteSprintAtCompanyConfig = cfg.Bind("Company Building Enhancements",
                                                     "Infinite Stamina/Sprint At Company Building",
                                                     true,
                                                     "If set to true, you will have infinite stamina/sprint at the Company Building.").Value;
        }

        //Request/Receiver Methods
        public static void RequestSync()
        {
            if (!IsClient) return;

            using FastBufferWriter stream = new(4, Allocator.Temp);
            MessageManager.SendNamedMessage("CompanyBuildingEnhancements_OnRequestConfigSync", 0uL, stream);
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost) return;

            CompanyBuildingEnhancementsBase.Logger.LogInfo($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            using FastBufferWriter stream = new(array.Length + 4, Allocator.Temp);

            try
            {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage("CompanyBuildingEnhancements_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e)
            {
                CompanyBuildingEnhancementsBase.Logger.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
            }
        }

        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(4))
            {
                CompanyBuildingEnhancementsBase.Logger.LogError("Config sync error: Could not begin reading buffer.");
                return;
            }

            reader.ReadValueSafe(out int val, default);
            if (!reader.TryBeginRead(val))
            {
                CompanyBuildingEnhancementsBase.Logger.LogError("Config sync error: Host could not sync.");
                return;
            }

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);

            CompanyBuildingEnhancementsBase.Logger.LogInfo("Successfully synced config with host.");
        }

        //Join/Leave Patches
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        public static void InitializeLocalPlayer()
        {
            if (IsHost)
            {
                MessageManager.RegisterNamedMessageHandler("CompanyBuildingEnhancements_OnRequestConfigSync", OnRequestSync);
                Synced = true;

                return;
            }

            Synced = false;
            MessageManager.RegisterNamedMessageHandler("CompanyBuildingEnhancements_OnReceiveConfigSync", OnReceiveSync);
            RequestSync();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void PlayerLeave()
        {
            Config.RevertSync();
        }
    }
}
