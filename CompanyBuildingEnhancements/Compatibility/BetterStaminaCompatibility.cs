

namespace CompanyBuildingEnhancements.Compatibility
{
    internal class BetterStaminaCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("FlipMods.BetterStamina");
                }
                return (bool)_enabled;
            }
        }
    }
}
