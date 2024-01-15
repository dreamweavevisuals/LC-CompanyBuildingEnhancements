using BepInEx;
using BepInEx.Logging;
using CompanyBuildingEnhancements.Configuration;
using HarmonyLib;
using System;
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
        private const string modVersion = "2.7.0";

        public static new Config Config { get; internal set; }
        internal static new ManualLogSource Logger { get; private set; }

        private Harmony harmony;
        void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Config = new(base.Config);

            #region Netcode Patcher
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
            #endregion
            
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
