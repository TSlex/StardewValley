using StardewValley;

namespace ItemResearchSpawner.Utils
{
    internal static class SaveHelper
    {
        public static string DirectoryName => $"{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}";

        public const string PriceConfigPath = "assets/config/pricelist";
        
        public const string ModStatesKey = "tslex-research-states";
        public const string ProgressionsKey = "tslex-research-progressions";
        
        public const string PriceConfigKey = "tslex-research-pricelist";
        public const string CategoriesConfigKey = "tslex-research-categories";
    }
}