using CompanyBuildingEnhancements.Configuration;
using HarmonyLib;
using UnityEngine;

namespace CompanyBuildingEnhancements.Patches {
    [HarmonyPatch(typeof(StartMatchLever))]
    internal class StartMatchLeverPatch
    {
        internal static GameObject shipHub;
        internal static Animator shipHubAnimator;

        static bool logged = false;

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static void AutoLandAtCompany(ref StartMatchLever __instance)
        {
            #region Synced Config Variables
            bool syncedInstantLanding = Config.Instance.INSTANT_LAND_AT_COMPANY;
            bool syncedAutoLanding = Config.Instance.AUTO_LAND_ON_DEADLINE;

            if (!logged) {
                CompanyBuildingEnhancementsBase.Logger.LogInfo(syncedInstantLanding);
                CompanyBuildingEnhancementsBase.Logger.LogInfo(syncedAutoLanding);

                logged = true;
            }
            #endregion

            #region Deadline = 0, Auto Land
            bool hitDeadline = TimeOfDay.Instance.daysUntilDeadline == 0;

            StartOfRound pManager = __instance.playersManager;
            int levelID = pManager.currentLevel.levelID;

            if (hitDeadline && syncedAutoLanding && levelID != 3 && pManager.CanChangeLevels())
            {
                pManager.ChangeLevel(3);
                __instance.StartGame();

                CompanyBuildingEnhancementsBase.Logger.LogInfo("Auto Landing successfully synced with host config");

            }
            #endregion

            #region Instantly Land at Company Building
            if (!shipHub)
            {
                shipHub = GameObject.Find("Environment/HangarShip");
                shipHubAnimator = shipHub?.GetComponent<Animator>();

                if (!shipHub || !shipHubAnimator)
                {
                    CompanyBuildingEnhancementsBase.Logger.LogError(
                        "Ship object or animator not found? Cannot instantly land at company building!"
                    );

                    return;
                }
            }

            if (syncedInstantLanding && levelID == 3) {
                shipHubAnimator.speed = 10f;
                CompanyBuildingEnhancementsBase.Logger.LogInfo("Instant Landing successfully synced with host config");

                return;
            }

            shipHubAnimator.speed = 1f;
            CompanyBuildingEnhancementsBase.Logger.LogInfo("Instant Landing did not sync with host config");
            #endregion
        }
    }
}
