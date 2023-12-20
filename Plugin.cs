using BepInEx;
using BepInEx.Logging;
using CompanyBuildingEnhancements.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private readonly Harmony harmony = new Harmony(modGUID);

        private static CompanyBuildingEnhancementsBase Instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Config = new(base.Config);
            Logger = base.Logger;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo(modName + " loaded successfully");

            harmony.PatchAll(typeof(CompanyBuildingEnhancementsBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(StartMatchLeverPatch));
        }
    }
}
