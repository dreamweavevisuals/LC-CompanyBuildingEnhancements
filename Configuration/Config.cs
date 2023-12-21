using BepInEx.Configuration;
using CompanyBuildingEnhancements.Patches;
using HarmonyLib;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace CompanyBuildingEnhancements.Configuration {
    [Serializable]
    public class Config : SyncedInstance<Config>
    {
        public bool AUTO_LAND_ON_DEADLINE { get; private set; }
        public bool INSTANT_LAND_AT_COMPANY { get; private set; }
        public bool INFINITE_SPRINT_AT_COMPANY { get; private set; }

        [NonSerialized]
        readonly ConfigFile file;

        public Config(ConfigFile cfg)
        {
            InitInstance(this);
            file = cfg;

            INSTANT_LAND_AT_COMPANY = NewEntry("bInstantLand", true,
                "If set to true, the ship will land instantly at the Company Building. " +
                "You will be able to run to the shelf to sell your items as soon as the ship doors open.\n" +
                "This setting will automatically sync with the host's config file to avoid any desync issues."
            );

            AUTO_LAND_ON_DEADLINE = NewEntry("bAutoLandOnDeadline", false,
                "If set to true, the ship will automatically re-route & land at the Company Building when the deadline reaches 0 days.\n" +
                "This setting will automatically sync with the host's config file to avoid any desync issues."
            );

            INFINITE_SPRINT_AT_COMPANY = NewEntry("bInfiniteSprintAtCompanyBuilding", true,
                "If set to true, you will have infinite stamina/sprint at the Company Building."
            );
        }

        private T NewEntry<T>(string key, T defaultVal, string desc)
        {
            return file.Bind("Company Building Enhancements", key, defaultVal, desc).Value;
        }

        #region Request/Receiver Methods
        public static void RequestSync()
        {
            if (!IsClient) return;

            using FastBufferWriter stream = new(IntSize, Allocator.Temp);
            MessageManager.SendNamedMessage("CompanyBuildingEnhancements_OnRequestConfigSync", 0uL, stream);
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost) return;

            CompanyBuildingEnhancementsBase.Logger.LogInfo($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            using FastBufferWriter stream = new(value + IntSize, Allocator.Temp);

            try {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage("CompanyBuildingEnhancements_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e) {
                CompanyBuildingEnhancementsBase.Logger.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
            }
        }

        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(IntSize))
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

            try {
                byte[] data = new byte[val];
                reader.ReadBytesSafe(ref data, val);

                SyncInstance(data);
                CompanyBuildingEnhancementsBase.Logger.LogInfo("Successfully synced config with host.");
            } catch (Exception e) {
                CompanyBuildingEnhancementsBase.Logger.LogError(e);
            }
        }
        #endregion

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void PlayerLeave()
        {
            StartMatchLeverPatch.logged = false;
            RevertSync();
        }
    }
}
