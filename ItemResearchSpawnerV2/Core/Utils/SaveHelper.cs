﻿using StardewValley;

namespace ItemResearchSpawnerV2.Core.Utils {
    internal static class SaveHelper {
        public static string AssetsPath => "assets";
        public static string AssetsConfigPath => Path.Combine(AssetsPath, "config");
        public static string DumpBasePath => "saves";

        // public static readonly Func<string, string> PlayerPath = playerID => $"saves/{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}/{playerID}";

        public static readonly Func<string, string> PlayerPath = playerID =>
            Path.Combine(DumpBasePath, $"{Game1.player.Name}_{Game1.getFarm().NameOrUniqueName}", playerID);

        // public const string PricelistConfigPath = "assets/config/pricelist.json";
        // public const string CategoriesConfigPath = "assets/config/categories.json";

        public static string PricelistConfigPath => Path.Combine(AssetsConfigPath, "pricelist.json");
        public static string CategoriesConfigPath => Path.Combine(AssetsConfigPath, "categories.json");
        public static string BannedItemsConfigPath => Path.Combine(AssetsConfigPath, "banlist.json");

        // public const string PricelistDumpPath = "saves/pricelist.json";
        // public const string CategoriesDumpPath = "saves/categories.json";

        public static string PricelistDumpPath => Path.Combine(DumpBasePath, "pricelist.json");
        public static string CategoriesDumpPath => Path.Combine(DumpBasePath, "categories.json");

        // public static readonly Func<string, string> ProgressionDumpPath =
        //     playerID => $"{PlayerPath(playerID)}/progression.json";

        public static readonly Func<string, string> ProgressionDumpPath =
            playerID => Path.Combine(PlayerPath(playerID), "progression.json");

        public const string ModStatesKey = "tslex-research-states";
        public const string ProgressionsKey = "tslex-research-progressions";

        public const string PriceConfigKey = "tslex-research-pricelist";
        public const string CategoriesConfigKey = "tslex-research-categories";
    }
}