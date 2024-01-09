using BepInEx;
using BepInEx.Logging;
using CompanyBuildingEnhancements.Configuration;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CompanyBuildingEnhancements
{
    [BepInPlugin(modGUID, modName, modVersion)]
    #region Soft Mod Dependencies
    [BepInDependency("FlipMods.BetterStamina", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mikes.lethalcompany.mikestweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.malco.lethalcompany.moreshipupgrades", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.potatoepet.AdvancedCompany", BepInDependency.DependencyFlags.SoftDependency)]
    #endregion
    public class CompanyBuildingEnhancementsBase : BaseUnityPlugin
    {
        private const string Author = "Dreamweave";
        private const string modGUID = Author + ".CompanyBuildingEnhancements";
        private const string modName = "CompanyBuildingEnhancements";
        private const string modVersion = "2.6.0";

        public static new Config Config { get; internal set; }
        //public static AssetBundle CBEAssetBundle { get; private set; }
        internal static new ManualLogSource Logger { get; private set; }

        private Harmony harmony;
        void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Config = new(base.Config);

            //CBEAssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("CompanyBuildingEnhancements.Assets.cbeassets"));
            //CBEAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cbeassets"));

            try
            {
                harmony = new(modGUID);

                harmony.PatchAll();

                Logger.LogInfo("Company Building Enhancements loaded successfully");
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
