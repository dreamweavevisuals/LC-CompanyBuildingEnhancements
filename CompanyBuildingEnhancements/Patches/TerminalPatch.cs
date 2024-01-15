using CompanyBuildingEnhancements.Configuration;
using CompanyBuildingEnhancements.Misc;
using CompanyBuildingEnhancements.Networking;
using HarmonyLib;
using UnityEngine;

namespace CompanyBuildingEnhancements.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class TerminalPatch
    {
        private static bool IsSellScrapFromTerminalEnabled => Config.Instance.EnableSellScrapFromTerminal;
        #region Terminal Commands
        private const string SellScrapCommand = "sell scrap";
        private const string SellCommand = "sell";
        private const string SellAllCommand = "sell all";
        private static bool IsCommand(string text, string command) => text.ToLower() == command;
        private static bool IsSellScrapCommand(string text) => IsCommand(text, SellScrapCommand);
        private static bool IsSellCommand(string text) => IsCommand(text, SellCommand);
        private static bool IsSellAllCommand(string text) => IsCommand(text, SellAllCommand);
        #endregion

        [HarmonyPostfix]
        [HarmonyPatch("ParsePlayerSentence")]
        private static void HandleSellScrap(ref Terminal __instance, ref TerminalNode __result)
        {
            string text = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (!IsSellScrapFromTerminalEnabled || !IsSellScrapCommand(text))
                return;

            int quotaRequirement = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            int companyBuyingRate = Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f);
            string companyBuyingRateResult = "│ Company buying rate: " + companyBuyingRate + "%";
            var quotaResult = SellScrapResults.CalculateAmountAndProfitSoldQuota();
            var allResult = SellScrapResults.CalculateAmountAndProfitSoldAll();
            int amountSoldQuota = quotaResult.AmountSoldQuota;
            int totalProfitQuota = quotaResult.TotalProfitQuota;
            int amountSoldAll = allResult.AmountSoldAll;
            int totalProfitAll = allResult.TotalProfitAll;
            string scrapOnShipResult = "│ You have " + Mathf.Clamp(amountSoldAll, 0, 999) + " scrap items worth $" + Mathf.Clamp(totalProfitAll, 0, 999999);
            string thinSpace = "\u2009";
            __result = ScriptableObject.CreateInstance<TerminalNode>();

            if (StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase && totalProfitAll >= quotaRequirement)
            {
                __result.clearPreviousText = true;
                __result.displayText = "╭────────────────────────────────────────╮\n" +
                                       companyBuyingRateResult.PadRight(40).Substring(0, 40) + thinSpace + thinSpace + thinSpace + "│\n" +
                                       "│                                        │\n" +
                                       scrapOnShipResult.PadRight(40).Substring(0, 40) + thinSpace + thinSpace + "│\n" +
                                       "╰────────────────────────────────────────╯\n\n" +
                                       "You can sell " + amountSoldQuota + " scrap items for $" + totalProfitQuota + " to reach the quota.\n\n" + 
                                       "Alternatively, you can choose to sell all of the scrap on your ship.\n\n" + 
                                       ">sell  >sell all\n\n";
            }
            else if (StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase && totalProfitAll < quotaRequirement)
            {
                __result.clearPreviousText = true;
                __result.displayText = "╭────────────────────────────────────────╮\n" +
                                       companyBuyingRateResult.PadRight(40).Substring(0, 40) + thinSpace + thinSpace + thinSpace + "│\n" +
                                       "│                                        │\n" +
                                       scrapOnShipResult.PadRight(40).Substring(0, 40) + thinSpace + thinSpace + "│\n" +
                                       "╰────────────────────────────────────────╯\n\n" +
                                       "You have not met the quota requirement, but you can sell your scrap anyway if you'd like.\n\n" +
                                       ">sell\n\n";
            }
            else 
            {
                __result.clearPreviousText = true;
                __result.displayText = "╭────────────────────────────────────────╮\n" +
                                       companyBuyingRateResult.PadRight(40).Substring(0, 40) + thinSpace + thinSpace + thinSpace + "│\n" +
                                       "│                                        │\n" +
                                       scrapOnShipResult.PadRight(40).Substring(0, 40) + thinSpace + thinSpace + "│\n" +
                                       "╰────────────────────────────────────────╯\n\n" + 
                                       "The ship needs to be landed at the Company Building to sell scrap items.\n\n";
            }
            
        }

        [HarmonyPostfix]
        [HarmonyPatch("ParsePlayerSentence")]
        private static void HandleSell(ref Terminal __instance, ref TerminalNode __result)
        {
            string text = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (!IsSellScrapFromTerminalEnabled || !IsSellCommand(text))
                return;

            var quotaResult = SellScrapResults.CalculateAmountAndProfitSoldQuota();
            int amountSoldQuota = quotaResult.AmountSoldQuota;
            int totalProfitQuota = quotaResult.TotalProfitQuota;
            __result = ScriptableObject.CreateInstance<TerminalNode>();

            if (StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase)
            {
                __result.clearPreviousText = true;
                __result.displayText = "You sold " + amountSoldQuota + " scrap items for $" + totalProfitQuota + "!" + "\n" + "\n" + "The Company appreciates your commitment." + "\n" + "\n";

                CBENetworkHandler.instance.SellScrapServerRpc();
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
            string text = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
            if (!IsSellScrapFromTerminalEnabled || !IsSellAllCommand(text))
                return;

            var allResult = SellScrapResults.CalculateAmountAndProfitSoldAll();
            int amountSoldAll = allResult.AmountSoldAll;
            int totalProfitAll = allResult.TotalProfitAll;
            __result = ScriptableObject.CreateInstance<TerminalNode>();

            if (StartOfRound.Instance.currentLevel.levelID == 3 && !StartOfRound.Instance.inShipPhase)
            {
                __result.clearPreviousText = true;
                __result.displayText = "You sold " + amountSoldAll + " scrap items for $" + totalProfitAll + "!" + "\n" + "\n" + "The Company appreciates your commitment." + "\n" + "\n";

                CBENetworkHandler.instance.SellAllScrapServerRpc();
            }
            else
            {
                __result.clearPreviousText = true;
                __result.displayText = "The ship needs to be landed at the Company Building to sell scrap items." + "\n" + "\n";
            }

        }
    }
}
