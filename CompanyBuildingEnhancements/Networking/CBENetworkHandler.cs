using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Netcode;
using UnityEngine;

namespace CompanyBuildingEnhancements.Networking
{
    public class CBENetworkHandler : NetworkBehaviour
    {
        public static CBENetworkHandler instance { get; private set; }

        internal static GameObject shipHub;

        void Awake()
        {
            instance = this;
            shipHub = GameObject.Find("Environment/HangarShip");
        }

        public List<GrabbableObject> soldObjectsQuota;
        public List<GrabbableObject> soldObjectsAll;

        [ClientRpc]
        public void SellScrapClientRpc()
        {
            GrabbableObject[] componentsInChildren = shipHub.GetComponentsInChildren<GrabbableObject>();
            List<GrabbableObject> objectsInShip = componentsInChildren
                .Where(obj => obj.itemProperties.itemName != "Gift" && !obj.isHeld && obj.itemProperties.isScrap && obj.scrapValue > 0)
                .OrderBy(obj => obj.scrapValue)
                .ToList();

            int quotaRequirement = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            int amountOfScrapSold = 0;
            int totalProfit = 0;

            foreach (GrabbableObject obj in objectsInShip)
            {
                amountOfScrapSold++;
                float itemValue = obj.scrapValue * StartOfRound.Instance.companyBuyingRate;
                totalProfit += (int)Math.Round(itemValue);
                //CompanyBuildingEnhancementsBase.Logger.LogInfo(obj.itemProperties.itemName + ": (Original Value: " + obj.scrapValue + " / Buying Rate Value: " + itemValue);
                //CompanyBuildingEnhancementsBase.Logger.LogInfo("Running Total: " + totalProfit);
                #region Actually Sell Scrap LOL
                Terminal terminalScript = HUDManager.Instance.terminalScript;
                terminalScript.groupCredits += (int)Math.Round(itemValue);
                EndOfGameStats gameStats = StartOfRound.Instance.gameStats;
                gameStats.scrapValueCollected += (int)Math.Round(itemValue);
                TimeOfDay.Instance.quotaFulfilled += (int)Math.Round(itemValue);
                TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                if (NetworkManager.Singleton.IsServer && obj.IsSpawned)
                {
                    obj.GetComponent<NetworkObject>().Despawn(true);
                }
                if (obj.radarIcon != null)
                {
                    GrabbableObject.Destroy(obj.radarIcon.gameObject);
                }
                #endregion
                soldObjectsQuota.Add(obj);
                if (totalProfit >= quotaRequirement)
                {
                    break;
                }
            }
            HUDManager.Instance.DisplayCreditsEarning(totalProfit, soldObjectsQuota.ToArray(), HUDManager.Instance.terminalScript.groupCredits);
            HUDManager.Instance.UIAudio.PlayOneShot(HUDManager.Instance.globalNotificationSFX);
            soldObjectsQuota.Clear();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SellScrapServerRpc()
        {
            SellScrapClientRpc();
        }

        [ClientRpc]
        public void SellAllScrapClientRpc()
        {
            GrabbableObject[] componentsInChildren = shipHub.GetComponentsInChildren<GrabbableObject>();
            List<GrabbableObject> objectsInShip = componentsInChildren
                .Where(obj => obj.itemProperties.itemName != "Gift" && !obj.isHeld && obj.itemProperties.isScrap && obj.scrapValue > 0)
                .OrderBy(obj => obj.scrapValue)
                .ToList();

            int quotaRequirement = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            int amountOfScrapSold = 0;
            int totalProfit = 0;

            foreach (GrabbableObject obj in objectsInShip)
            {
                amountOfScrapSold++;
                float itemValue = obj.scrapValue * StartOfRound.Instance.companyBuyingRate;
                totalProfit += (int)Math.Round(itemValue);
                //CompanyBuildingEnhancementsBase.Logger.LogInfo(obj.itemProperties.itemName + ": (Original Value: " + obj.scrapValue + " / Buying Rate Value: " + itemValue);
                //CompanyBuildingEnhancementsBase.Logger.LogInfo("Running Total: " + totalProfit);
                #region Actually Sell Scrap LOL
                Terminal terminalScript = HUDManager.Instance.terminalScript;
                terminalScript.groupCredits += (int)Math.Round(itemValue);
                EndOfGameStats gameStats = StartOfRound.Instance.gameStats;
                gameStats.scrapValueCollected += (int)Math.Round(itemValue);
                TimeOfDay.Instance.quotaFulfilled += (int)Math.Round(itemValue);
                TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                if (NetworkManager.Singleton.IsServer && obj.IsSpawned)
                {
                    obj.GetComponent<NetworkObject>().Despawn(true);
                }
                if (obj.radarIcon != null)
                {
                    GrabbableObject.Destroy(obj.radarIcon.gameObject);
                }
                #endregion
                soldObjectsAll.Add(obj);
            }
            HUDManager.Instance.DisplayCreditsEarning(totalProfit, soldObjectsAll.ToArray(), HUDManager.Instance.terminalScript.groupCredits);
            HUDManager.Instance.UIAudio.PlayOneShot(HUDManager.Instance.globalNotificationSFX);
            soldObjectsAll.Clear();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SellAllScrapServerRpc()
        {
            SellAllScrapClientRpc();
        }
    }
}
