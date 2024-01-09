

namespace CompanyBuildingEnhancements.Compatibility
{
    internal class LategameUpgradesCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.malco.lethalcompany.moreshipupgrades");
                }
                return (bool)_enabled;
            }
        }

        public static void InvokeUpdatePlayerWeight()
        {
            MoreShipUpgrades.UpgradeComponents.exoskeletonScript.UpdatePlayerWeight();
        }
    }
}
