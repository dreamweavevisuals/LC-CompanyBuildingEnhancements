using HarmonyLib;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("DiscardItem")]
        private static void WeightlessInventoryDropPatch(GrabbableObject __instance)
        {
            if (__instance.IsOwner)
                PlayerControllerBPatch.WeightlessInventoryPickupPatch();
        }
    }
}
