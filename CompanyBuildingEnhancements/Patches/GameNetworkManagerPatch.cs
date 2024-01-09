using CompanyBuildingEnhancements.Configuration;
using HarmonyLib;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        #region  Revert Config Sync
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void PlayerLeave()
        {
            StartMatchLeverPatch.logged = false;
            Config.RevertSync();
            CompanyBuildingEnhancementsBase.Logger.LogInfo("Disconnected From Lobby: Config Sync Reverted.");
        }
        #endregion
    }
}