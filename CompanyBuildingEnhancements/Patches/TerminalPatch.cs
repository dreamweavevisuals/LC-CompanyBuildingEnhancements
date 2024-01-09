using CompanyBuildingEnhancements.Configuration;
using CompanyBuildingEnhancements.Networking;
using HarmonyLib;
using UnityEngine;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ParsePlayerSentence")]
        private static void HandleSellScrap(ref Terminal __instance, ref TerminalNode __result)
        {
            bool syncedSellScrap = Config.Instance.EnableSellScrapFromTerminal;
            string currSymbol = "\u258C";
            string text = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (!syncedSellScrap)
            {
                return;
            }
            if (!(text.ToLower() == "sell scrap"))
            {
                return;
            }
            int quotaRequirement = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            __result = ScriptableObject.CreateInstance<TerminalNode>();
            if (StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase && Misc.AutosellScrapScript.CalculateTotalProfitAll() >= quotaRequirement)
            {
                __result.clearPreviousText = true;
                __result.displayText = "The Company buying rate is currently " + StartOfRound.Instance.companyBuyingRate.ToString("P0") + "\n" + "\n" + "You currently have " + Misc.AutosellScrapScript.CalculateAmountOfAllScrapSold() + " items worth " + currSymbol + Misc.AutosellScrapScript.CalculateTotalProfitAll() + "!" + "\n" + "\n" + "You can sell " + Misc.AutosellScrapScript.CalculateAmountOfScrapSold() + " items for " + currSymbol + Misc.AutosellScrapScript.CalculateTotalProfit() + " to reach the profit quota." + "\n" + "\n" + "Alternatively, you can choose to sell all of the scrap on your ship." + "\n" + "\n" + ">sell  >sell all" + "\n" + "\n";
            }
            else if (StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase && Misc.AutosellScrapScript.CalculateTotalProfitAll() < quotaRequirement)
            {
                __result.clearPreviousText = true;
                __result.displayText = "The Company buying rate is currently " + StartOfRound.Instance.companyBuyingRate.ToString("P0") + "\n" + "\n" + "You currently have " + Misc.AutosellScrapScript.CalculateAmountOfAllScrapSold() + " items worth " + currSymbol + Misc.AutosellScrapScript.CalculateTotalProfitAll() + "!" + "\n" + "\n" + "You do not have enough scrap to meet the profit quota, but you can sell your scrap anyway if you would like." + "\n" + "\n" + ">sell" + "\n" + "\n";
            }
            else 
            {
                __result.clearPreviousText = true;
                __result.displayText = "The ship needs to be landed at the Company Building to sell scrap items." + "\n" + "\n";
            }
            
        }

        [HarmonyPostfix]
        [HarmonyPatch("ParsePlayerSentence")]
        private static void HandleSell(ref Terminal __instance, ref TerminalNode __result)
        {
            bool syncedSellScrap = Config.Instance.EnableSellScrapFromTerminal;
            string currSymbol = "\u258C";
            string text = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (!syncedSellScrap)
            {
                return;
            }
            if (!(text.ToLower() == "sell"))
            {
                return;
            }
            __result = ScriptableObject.CreateInstance<TerminalNode>();
            if (StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase)
            {
                __result.clearPreviousText = true;
                __result.displayText = "You sold " + Misc.AutosellScrapScript.CalculateAmountOfScrapSold() + " items for " + currSymbol + Misc.AutosellScrapScript.CalculateTotalProfit() + "!" + "\n" + "\n" + "The Company appreciates your commitment." + "\n" + "\n";
                //Misc.AutosellScrapScript.SellScrapInShip();
                CBENetworkHandler.Instance.SellScrapServerRpc();
            }
            else
            {
                __result.clearPreviousText = true;
                __result.displayText = "The ship needs to be landed at the Company Building to sell scrap items." + "\n" + "\n";
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch("ParsePlayerSentence")]
        private static void HandleSellAll(ref Terminal __instance, ref TerminalNode __result)
        {
            bool syncedSellScrap = Config.Instance.EnableSellScrapFromTerminal;
            string currSymbol = "\u258C";
            string text = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (!syncedSellScrap)
            {
                return;
            }
            if (!(text.ToLower() == "sell all"))
            {
                return;
            }
            __result = ScriptableObject.CreateInstance<TerminalNode>();
            if (StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase)
            {
                __result.clearPreviousText = true;
                __result.displayText = "You sold " + Misc.AutosellScrapScript.CalculateAmountOfAllScrapSold() + " items for " + currSymbol + Misc.AutosellScrapScript.CalculateTotalProfitAll() + "!" + "\n" + "\n" + "The Company appreciates your commitment." + "\n" + "\n";
                Misc.AutosellScrapScript.SellAllScrapInShip();
            }
            else
            {
                __result.clearPreviousText = true;
                __result.displayText = "The ship needs to be landed at the Company Building to sell scrap items." + "\n" + "\n";
            }

        }
    }
}
