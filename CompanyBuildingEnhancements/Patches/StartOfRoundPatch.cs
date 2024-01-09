using CompanyBuildingEnhancements.Configuration;
using CompanyBuildingEnhancements.Misc;
using HarmonyLib;
using UnityEngine;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        #region Automatic Landing At Company
        [HarmonyPostfix]
        [HarmonyPatch("SetShipReadyToLand")]
        private static void AutoLandPatch(StartOfRound __instance)
        {
            bool syncedAutoLanding = Config.Instance.EnableAutoLandOnDeadline;

            bool hitDeadline = TimeOfDay.Instance.daysUntilDeadline == 0;
            int levelID = __instance.currentLevel.levelID;

            if (hitDeadline && syncedAutoLanding && levelID != 3)
            {
                __instance.ChangeLevel(3);
                __instance.ArriveAtLevel();
                __instance.AutoSaveShipData();
                Object.FindObjectOfType<StartMatchLever>().PlayLeverPullEffectsServerRpc(true);
                Object.FindObjectOfType<StartMatchLever>().PullLever();
                CompanyBuildingEnhancementsBase.Logger.LogInfo("Automatic Landing Activated");
            }
        }
        #endregion

        #region Set Weightless Inventory At Company
        [HarmonyPostfix]
        [HarmonyPatch("StartGame")]
        private static void SetWeightlessInventoryPatch(StartOfRound __instance)
        {
            int levelID = __instance.currentLevel.levelID;

            if (Config.Default.EnableWeightlessInventoryAtCompany && levelID == 3 && !StartOfRound.Instance.inShipPhase)
            {
                WeightlessInventoryScript.SetWeightlessInventory();
            }
        }
        #endregion

        #region Reset Weightless Inventory At Company
        [HarmonyPostfix]
        [HarmonyPatch("SetShipReadyToLand")]
        private static void ResetWeightlessInventoryPatch(StartOfRound __instance)
        {
            int levelID = __instance.currentLevel.levelID;

            if (Config.Default.EnableWeightlessInventoryAtCompany && levelID == 3 && StartOfRound.Instance.inShipPhase)
            {
                WeightlessInventoryScript.ResetWeightlessInventory();
            }
        }
        #endregion
    }
}
