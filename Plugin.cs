using BepInEx;
using BepInEx.Logging;
using CompanyBuildingEnhancements.Configuration;
using CompanyBuildingEnhancements.Patches;
using HarmonyLib;
using System;

namespace CompanyBuildingEnhancements
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class CompanyBuildingEnhancementsBase : BaseUnityPlugin
    {
        private const string Author = "Dreamweave";
        private const string modGUID = Author + ".CompanyBuildingEnhancements";
        private const string modName = "CompanyBuildingEnhancements";
        private const string modVersion = "2.0.0";

        public static new Config Config { get; internal set; }
        internal static new ManualLogSource Logger { get; private set; }

        private Harmony harmony;

        private static CompanyBuildingEnhancementsBase Instance;

        void Awake()
        {
            if (!Instance) Instance = this;

            Logger = base.Logger;
            Config = new(base.Config);

            try {
                harmony = new(modGUID);

                harmony.PatchAll(typeof(PlayerControllerBPatch));
                harmony.PatchAll(typeof(StartMatchLeverPatch));

                Logger.LogInfo($"{modName} loaded successfully");
            } catch(Exception e) {
                Logger.LogError(e);
            }
        }
    }
}
