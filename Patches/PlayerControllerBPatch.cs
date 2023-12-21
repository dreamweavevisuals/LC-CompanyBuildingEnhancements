using CompanyBuildingEnhancements.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using System;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ConnectClientToPlayerObject")]
        public static void InitializeLocalPlayer() {
            CompanyBuildingEnhancementsBase.Logger.LogInfo("Called InitializeLocalPlayer");
            StartMatchLeverPatch.logged = false;

            if (Config.IsHost) {
                try {
                    Config.MessageManager.RegisterNamedMessageHandler("CompanyBuildingEnhancements_OnRequestConfigSync", Config.OnRequestSync);
                    Config.Synced = true;
                }
                catch (Exception e) {
                    CompanyBuildingEnhancementsBase.Logger.LogError(e);
                }

                return;
            }

            Config.Synced = false;
            Config.MessageManager.RegisterNamedMessageHandler("CompanyBuildingEnhancements_OnReceiveConfigSync", Config.OnReceiveSync);
            Config.RequestSync();
        }

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
