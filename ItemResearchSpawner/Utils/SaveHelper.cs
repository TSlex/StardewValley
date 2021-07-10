using StardewValley;

namespace ItemResearchSpawner.Utils
{
    internal static class SaveHelper
    {
        public static string DirectoryName => $"{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}";

        public const string PriceConfigPath = "assets/config/pricelist";
        
        public const string ModStateKey = "tslex-research-state";
        public const string ProgressionKey = "tslex-research-progression";
        
        public const string PriceConfigKey = "tslex-research-pricelist";
        public const string CategoriesConfigKey = "tslex-research-categories";
    }
}