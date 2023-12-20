using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    internal class PlayerControllerBPatch
    {
        [HarmonyPostfix]
        static void infiniteStaminaAtCompanyPatch(ref float ___sprintMeter)
        {
            var startOfRound = StartOfRound.Instance;
            if (Config.Default.enableInfiniteSprintAtCompanyConfig == true && startOfRound is not null && startOfRound.currentLevel.levelID == 3)
            {
                ___sprintMeter = 1f;
            }
        }
    }
}
