using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal static class UIConstants {
        public const int BorderWidth = 4 * Game1.pixelZoom;

        public static string AssetsPath => "assets";
        public static string UISheet => Path.Combine(AssetsPath, "images", "spritesheet.png");
    }
}