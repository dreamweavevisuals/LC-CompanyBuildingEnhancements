using BepInEx.Configuration;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace CompanyBuildingEnhancements.Configuration
{
    [Serializable]
    public class Config : SyncedInstance<Config>
    {
        public bool EnableAutoLandOnDeadline { get; private set; }
        public bool EnableInstantLandAtCompany { get; private set; }
        public bool EnableInfiniteSprintAtCompany { get; private set; }
        public bool EnableWeightlessInventoryAtCompany { get; private set; }
        public bool EnableSellScrapFromTerminal { get; private set; }

        [NonSerialized]
        readonly ConfigFile file;

        public Config(ConfigFile cfg)
        {
            InitInstance(this);
            file = cfg;

            EnableInstantLandAtCompany = NewEntry("Company Building Enhancements",
                                                  "Instant Landing At Company Building",
                                                  true,
                                                  "If set to true, the ship will land instantly at the Company Building. You will be able to run to the shelf to sell your items as soon as the ship doors open. \n" + "This setting will automatically sync with the host's config file to avoid any desync issues.");
            EnableAutoLandOnDeadline = NewEntry("Company Building Enhancements",
                                                "Automatic Landing At Company Building When Deadline Reaches 0 Days",
                                                false,
                                                "If set to true, the ship will automatically re-route & land at the Company Building when the deadline reaches 0 days. \n" + "This setting will automatically sync with the host's config file to avoid any desync issues.");
            EnableSellScrapFromTerminal = NewEntry("Company Building Enhancements",
                                               "Sell Scrap From Terminal",
                                               true,
                                               "If set to true, you will be able to sell scrap from the terminal while the ship is landed at the Company Building. \n" + "Just type 'sell scrap' in the terminal to get started! \n" + "This setting will automatically sync with the host's config file.");
            EnableInfiniteSprintAtCompany = NewEntry("Company Building Enhancements",
                                                     "Infinite Stamina/Sprint At Company Building",
                                                     true,
                                                     "If set to true, you will have infinite stamina/sprint at the Company Building.");
            EnableWeightlessInventoryAtCompany = NewEntry("Company Building Enhancements",
                                                     "Weightless Inventory At Company Building",
                                                     false,
                                                     "If set to true, you will have a weightless inventory at the Company Building. \n" + "This feature is disabled by default as it has the potential to function improperly depending on your modlist. If you experience incompatibilities, please disable the feature and let me know in the Discord. A list of known incompatibilities can be found in the ReadMe. \n" + "If you have BetterStamina by FlipMods enabled in your mod list, this feature will be automatically disabled, regardless of your config setting.");
        }

        private T NewEntry<T>(string category, string key, T defaultVal, string desc)
        {
            return file.Bind(category, key, defaultVal, desc).Value;
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

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);

            CompanyBuildingEnhancementsBase.Logger.LogInfo("Successfully synced config with host.");
        }
        #endregion
    }
}