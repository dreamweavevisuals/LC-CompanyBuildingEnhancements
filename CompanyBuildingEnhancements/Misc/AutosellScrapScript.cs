using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace CompanyBuildingEnhancements.Misc
{
    internal class AutosellScrapScript
    {
        internal static GameObject shipHub;
        public static float CalculateAmountOfScrapSold()
        {
            shipHub = GameObject.Find("Environment/HangarShip");
            GrabbableObject[] componentsInChildren = shipHub.GetComponentsInChildren<GrabbableObject>();
            List<GrabbableObject> objectsInShip = componentsInChildren.OrderBy(obj => obj.scrapValue).ToList();

            int quotaRequirement = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            int amountOfScrapSold = 0;
            int totalProfit = 0;
            foreach (GrabbableObject obj in objectsInShip)
            {
                if (obj.itemProperties.itemName == "Gift") continue;
                if (obj.itemProperties.isScrap && obj.scrapValue > 0)
                {
                    amountOfScrapSold++;
                    float itemValue = obj.scrapValue * StartOfRound.Instance.companyBuyingRate;
                    totalProfit += (int)Math.Round(itemValue);
                    if (totalProfit >= quotaRequirement)
                    {
                        break;
                    }
                }
            }
            return amountOfScrapSold;
        }
        public static float CalculateTotalProfit()
        {
            shipHub = GameObject.Find("Environment/HangarShip");
            GrabbableObject[] componentsInChildren = shipHub.GetComponentsInChildren<GrabbableObject>();
            List<GrabbableObject> objectsInShip = componentsInChildren.OrderBy(obj => obj.scrapValue).ToList();

            int quotaRequirement = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            int totalProfit = 0;
            foreach (GrabbableObject obj in objectsInShip)
            {
                if (obj.itemProperties.itemName == "Gift") continue;
                if (obj.itemProperties.isScrap && obj.scrapValue > 0)
                {
                    float itemValue = obj.scrapValue * StartOfRound.Instance.companyBuyingRate;
                    totalProfit += (int)Math.Round(itemValue);
                    if (totalProfit >= quotaRequirement)
                    {
                        break;
                    }
                }
            }
            return totalProfit;
        }
        public static void SellScrapInShip()
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
                        obj.GetComponent<NetworkObject>().Despawn(true);
                        GrabbableObject.Destroy(obj.radarIcon.gameObject);
                        #endregion
                        if (totalProfit >= quotaRequirement)
                        {
                            break;
                        }
                    }
                }
            HUDManager.Instance.DisplayGlobalNotification("You Sold " + amountOfScrapSold + " items for " + currSymbol + totalProfit + "!");
        }
        public static float CalculateAmountOfAllScrapSold()
        {
            shipHub = GameObject.Find("Environment/HangarShip");
            GrabbableObject[] componentsInChildren = shipHub.GetComponentsInChildren<GrabbableObject>();
            List<GrabbableObject> objectsInShip = componentsInChildren.OrderBy(obj => obj.scrapValue).ToList();

            int amountOfScrapSold = 0;
            foreach (GrabbableObject obj in objectsInShip)
            {
                if (obj.itemProperties.itemName == "Gift") continue;
                if (obj.itemProperties.isScrap && obj.scrapValue > 0)
                {
                    amountOfScrapSold++;
                }
            }
            return amountOfScrapSold;
        }
        public static float CalculateTotalProfitAll()
        {
            shipHub = GameObject.Find("Environment/HangarShip");
            GrabbableObject[] componentsInChildren = shipHub.GetComponentsInChildren<GrabbableObject>();
            List<GrabbableObject> objectsInShip = componentsInChildren.OrderBy(obj => obj.scrapValue).ToList();

            int quotaRequirement = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            int totalProfit = 0;
            foreach (GrabbableObject obj in objectsInShip)
            {
                if (obj.itemProperties.itemName == "Gift") continue;
                if (obj.itemProperties.isScrap && obj.scrapValue > 0)
                {
                    float itemValue = obj.scrapValue * StartOfRound.Instance.companyBuyingRate;
                    totalProfit += (int)Math.Round(itemValue);
                }
            }
            return totalProfit;
        }
        public static void SellAllScrapInShip()
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
                    obj.GetComponent<NetworkObject>().Despawn(true);
                    GrabbableObject.Destroy(obj.radarIcon.gameObject);
                    #endregion
                }
            }
            HUDManager.Instance.DisplayGlobalNotification("You Sold " + amountOfScrapSold + " items for " + currSymbol + totalProfit + "!");
        }
    }
}
