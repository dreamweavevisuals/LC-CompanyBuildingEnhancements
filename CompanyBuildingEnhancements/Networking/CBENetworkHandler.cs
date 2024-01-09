using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Netcode;
using UnityEngine;

namespace CompanyBuildingEnhancements.Networking
{
    public class CBENetworkHandler : NetworkBehaviour
    {
        public static CBENetworkHandler Instance { get; private set; }

        internal static GameObject shipHub;

        [ClientRpc]
        public void SellScrapClientRpc()
        {
            shipHub = GameObject.Find("Environment/HangarShip");
            GrabbableObject[] componentsInChildren = shipHub.GetComponentsInChildren<GrabbableObject>();
            List<GrabbableObject> objectsInShip = componentsInChildren.OrderBy(obj => obj.scrapValue).ToList();

            string currSymbol = "\u258C";
            int quotaRequirement = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            int amountOfScrapSold = 0;
            int totalProfit = 0;
            foreach (GrabbableObject obj in objectsInShip)
            {
                if (obj.itemProperties.itemName == "Gift") continue;
                if (obj.itemProperties.isScrap && obj.scrapValue > 0)
                {
                    //CompanyBuildingEnhancementsBase.Logger.LogInfo(obj.itemProperties.itemName + ": " + obj.scrapValue);
                    amountOfScrapSold++;
                    float itemValue = obj.scrapValue * StartOfRound.Instance.companyBuyingRate;
                    totalProfit += (int)Math.Round(itemValue);
                    //CompanyBuildingEnhancementsBase.Logger.LogInfo("Running Total: " + totalProfit);
                    #region Actually Sell Scrap LOL
                    Terminal terminalScript = HUDManager.Instance.terminalScript;
                    terminalScript.groupCredits += (int)Math.Round(itemValue);
                    EndOfGameStats gameStats = StartOfRound.Instance.gameStats;
                    gameStats.scrapValueCollected += (int)Math.Round(itemValue);
                    TimeOfDay.Instance.quotaFulfilled += (int)Math.Round(itemValue);
                    TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
                    if (NetworkManager.Singleton.IsServer)
                    {
                        obj.GetComponent<NetworkObject>().Despawn(true);
                        GrabbableObject.Destroy(obj.radarIcon.gameObject);
                    }
                    #endregion
                    if (totalProfit >= quotaRequirement)
                    {
                        break;
                    }
                }
            }
            HUDManager.Instance.DisplayGlobalNotification("You Sold " + amountOfScrapSold + " items for " + currSymbol + totalProfit + "!");
        }

        [ServerRpc(RequireOwnership = false)]
        public void SellScrapServerRpc()
        {
            SellScrapClientRpc();
        }
    }
}
