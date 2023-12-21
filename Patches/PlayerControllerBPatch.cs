using CompanyBuildingEnhancements.Configuration;
using GameNetcodeStuff;
using HarmonyLib;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    internal class PlayerControllerBPatch
    {
        [HarmonyPostfix]
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
