using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace CompanyBuildingEnhancements.Misc
{
    public class SellScrapResults
    {
        public int AmountSoldQuota { get; set; }
        public int TotalProfitQuota { get; set; }
        public int AmountSoldAll { get; set; }
        public int TotalProfitAll { get; set; }

        internal static GameObject shipHub;

        public static SellScrapResults CalculateAmountAndProfitSoldQuota()
        {
            shipHub = GameObject.Find("Environment/HangarShip");
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
                //CompanyBuildingEnhancementsBase.Logger.LogInfo($"Total Profit: {totalProfit}, Quota Requirement: {quotaRequirement}");
                if (totalProfit >= quotaRequirement)
                {
                    break;
                }
            }
            return new SellScrapResults
            {AmountSoldQuota = amountOfScrapSold, TotalProfitQuota = totalProfit};
        }
        public static SellScrapResults CalculateAmountAndProfitSoldAll()
        {
            shipHub = GameObject.Find("Environment/HangarShip");
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
                //CompanyBuildingEnhancementsBase.Logger.LogInfo($"Total Profit: {totalProfit}, Quota Requirement: {quotaRequirement}");
            }
            return new SellScrapResults
            {AmountSoldAll = amountOfScrapSold, TotalProfitAll = totalProfit};
        }
    }
}
