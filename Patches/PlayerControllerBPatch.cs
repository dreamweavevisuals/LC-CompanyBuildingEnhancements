using CompanyBuildingEnhancements.Configuration;
using GameNetcodeStuff;
using HarmonyLib;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void InfiniteStaminaAtCompanyPatch(ref float ___sprintMeter)
        {
            if (!Config.Default.INFINITE_SPRINT_AT_COMPANY)
                return;

            var curLevel = StartOfRound.Instance?.currentLevel;
            if (curLevel?.levelID == 3)
            {
                ___sprintMeter = 1f;
            }
        }
    }
}
