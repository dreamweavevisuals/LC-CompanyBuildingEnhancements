using CompanyBuildingEnhancements.Compatibility;
using GameNetcodeStuff;
using UnityEngine;

namespace CompanyBuildingEnhancements.Misc
{
    internal class WeightlessInventoryScript
    {
        public static float CalculateWeightlessInventory(float defaultWeight)
        {
            float weightReductionMultiplier = 0 / 100;
            return defaultWeight * weightReductionMultiplier;
        }
        public static void SetWeightlessInventory()
        {
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (player.ItemSlots.Length <= 0) return;

            if (BetterStaminaCompatibility.enabled) return;

            float newCarryWeight = 1f;
            for (int i = 0; i < player.ItemSlots.Length; i++)
            {
                GrabbableObject obj = player.ItemSlots[i];
                if (obj == null) continue;

                newCarryWeight += Mathf.Clamp(CalculateWeightlessInventory(obj.itemProperties.weight - 1f), 0f, 10f);
            }
            player.carryWeight = newCarryWeight;
        }
        public static void ResetWeightlessInventory()
        {
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (player.ItemSlots.Length <= 0) return;

            if (BetterStaminaCompatibility.enabled) return;

            if (LategameUpgradesCompatibility.enabled)
            {
                LategameUpgradesCompatibility.InvokeUpdatePlayerWeight();
            }
            else
            {
                float newCarryWeight = 1f;
                for (int i = 0; i < player.ItemSlots.Length; i++)
                {
                    GrabbableObject obj = player.ItemSlots[i];
                    if (obj == null) continue;

                    newCarryWeight += Mathf.Clamp(obj.itemProperties.weight - 1f, 0f, 10f);
                }
                player.carryWeight = newCarryWeight;
                if (player.carryWeight < 1f) { player.carryWeight = 1f; }
            }
            
        }
    }
}
