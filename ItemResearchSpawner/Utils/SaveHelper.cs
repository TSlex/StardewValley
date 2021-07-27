using System;
using StardewValley;

namespace ItemResearchSpawner.Utils
{
    internal static class SaveHelper
    {
        public static readonly Func<string, string> PlayerPath = playerID => $"saves/{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}/{playerID}";
        
        public const string PricelistConfigPath = "assets/config/pricelist.json";
        public const string CategoriesConfigPath = "assets/config/categories.json";
        
        public const string PricelistDumpPath = "saves/pricelist.json";
        public const string CategoriesDumpPath = "saves/categories.json";
        
        public static readonly Func<string, string> ProgressionDumpPath = playerID => $"{PlayerPath(playerID)}/progression.json";
        
        public const string ModStatesKey = "tslex-research-states";
        public const string ProgressionsKey = "tslex-research-progressions";

        public const string PriceConfigKey = "tslex-research-pricelist";
        public const string CategoriesConfigKey = "tslex-research-categories";
    }
}