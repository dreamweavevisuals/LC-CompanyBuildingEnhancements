using CompanyBuildingEnhancements.Configuration;
using HarmonyLib;
using UnityEngine;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(StartMatchLever))]
    internal class StartMatchLeverPatch
    {
        internal static GameObject shipHub;
        internal static Animator shipHubAnimator;

        internal static bool logged = false;

        [HarmonyPostfix]
        [HarmonyPatch("PlayLeverPullEffectsClientRpc")]
        private static void InstantLandPatch()
        {
            bool syncedInstantLanding = Config.Instance.EnableInstantLandAtCompany;

            int levelID = StartOfRound.Instance.currentLevel.levelID;

            if (!shipHub)
            {
                shipHub = GameObject.Find("Environment/HangarShip");
                shipHubAnimator = shipHub?.GetComponent<Animator>();

                if (!shipHub || !shipHubAnimator)
                {
                    if (logged)
                        return;

                    CompanyBuildingEnhancementsBase.Logger.LogError("Ship object or animator not found. Cannot instantly land at company building.");
                    return;
                }
            }
            if (syncedInstantLanding && levelID == 3)
            {
                shipHubAnimator.speed = 10f;
                CompanyBuildingEnhancementsBase.Logger.LogInfo("Instant landing activated");
            }
            else
            {
                shipHubAnimator.speed = 1f;
                //CompanyBuildingEnhancementsBase.Logger.LogInfo("Ship operating at normal speed");
            }

            logged = true;
        }
    }
}
