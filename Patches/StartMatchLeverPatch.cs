using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(StartMatchLever), "Update")]
    internal class StartMatchLeverPatch
    {
        internal static GameObject shipHub;

        [HarmonyPrefix]
        private static void autoLandAtCompanyPatch(ref StartMatchLever __instance)
        {
            //Synced Config Variables
            bool syncedInstantLanding = Config.Instance.enableInstantLandingAtCompanyConfig;
            bool syncedAutoLanding = Config.Instance.enableAutoLandingOnDeadlineConfig;
            CompanyBuildingEnhancementsBase.Logger.LogInfo(syncedInstantLanding);
            CompanyBuildingEnhancementsBase.Logger.LogInfo(syncedAutoLanding);

            //Auto Landing At Company Building When Deadline Reaches 0 Days
            bool flag = TimeOfDay.Instance.daysUntilDeadline == 0;
            if (syncedAutoLanding == true && __instance.playersManager.currentLevel.levelID != 3 && flag && __instance.playersManager.CanChangeLevels())
            {
                __instance.playersManager.ChangeLevel(3);
                __instance.StartGame();
                CompanyBuildingEnhancementsBase.Logger.LogInfo("Auto Landing successfully synced with host config");

            }

            //Instant Ship Landing At Company Building
            if ((UnityEngine.Object)(object)shipHub == (UnityEngine.Object)null)
            {
                shipHub = GameObject.Find("Environment/HangarShip");
                if ((UnityEngine.Object)(object)shipHub == (UnityEngine.Object)null)
                {
                    return;
                }
            }
            GameObject obj = shipHub;
            if ((UnityEngine.Object)(object)((obj != null) ? obj.GetComponent<Animator>() : null) != (UnityEngine.Object)null)
            {
                if (syncedInstantLanding == true && __instance.playersManager.currentLevel.levelID == 3)
                {
                    shipHub.GetComponent<Animator>().speed = 10f;
                    CompanyBuildingEnhancementsBase.Logger.LogInfo("Instant Landing successfully synced with host config");
                }
                else
                {
                    shipHub.GetComponent<Animator>().speed = 1f;
                    CompanyBuildingEnhancementsBase.Logger.LogInfo("Instant Landing did not sync with host config");
                }
            }
        }
    }
}
