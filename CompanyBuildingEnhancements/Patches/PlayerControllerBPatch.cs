using CompanyBuildingEnhancements.Configuration;
using CompanyBuildingEnhancements.Misc;
using GameNetcodeStuff;
using HarmonyLib;
using System;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        #region Config Sync On Join
        [HarmonyPostfix]
        [HarmonyPatch("ConnectClientToPlayerObject")]
        public static void InitializeLocalPlayer()
        {
            if (Config.IsHost)
            {
                try
                {
                    Config.MessageManager.RegisterNamedMessageHandler("CompanyBuildingEnhancements_OnRequestConfigSync", Config.OnRequestSync);
                    Config.Synced = true;
                }
                catch (Exception e)
                {
                    CompanyBuildingEnhancementsBase.Logger.LogError(e);
                }

                return;
            }

            Config.Synced = false;
            Config.MessageManager.RegisterNamedMessageHandler("CompanyBuildingEnhancements_OnReceiveConfigSync", Config.OnReceiveSync);
            Config.RequestSync();
        }
        #endregion

        #region Infinite Sprint At Company
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void InfiniteStaminaAtCompanyPatch(ref float ___sprintMeter)
        {
            if (!Config.Default.EnableInfiniteSprintAtCompany)
                return;

            var currentLevel = StartOfRound.Instance?.currentLevel;
            if (currentLevel?.levelID == 3)
            {
                ___sprintMeter = 1f;
            }
        }
        #endregion

        #region Set Weightless Inventory On Pickup At Company
        [HarmonyPostfix]
        [HarmonyPatch("GrabObjectClientRpc")]
        public static void WeightlessInventoryPickupPatch()
        {
            if (Config.Default.EnableWeightlessInventoryAtCompany && StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase)
            {
                WeightlessInventoryScript.SetWeightlessInventory();
            }
        }
        #endregion
    }
}