using Microsoft.Xna.Framework;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal static class UIConstants {
        public const int BorderWidth = 4 * Game1.pixelZoom;

        public static string AssetsPath => "assets";
        public static string UISheet => Path.Combine(AssetsPath, "images", "spritesheet.png");

        //public static Color ResearchModeColor => new Color(160, 80, 0);
        public static Color ResearchModeColor => new Color(240, 60, 60);
        public static Color BuySellModeColor => new Color(255, 200, 20);
        public static Color CombinedModeColor => new Color(80, 220, 0);
        public static Color ResearchPlusModeColor => new Color(120, 50, 255);
        public static Color BuySellPlusModeColor => new Color(0, 150, 255);

        public static Rectangle CreativeMenuBase => new Rectangle(64, 256, 832, 384);
        public static Rectangle CreativeMenuFrame => new Rectangle(896, 256, 960, 384);

        public static Rectangle BookAnimBase => new Rectangle(256, 640, 192, 192);
        public static Rectangle BookAnimFrame => new Rectangle(256, 832, 192, 192);

        public static Rectangle ItemCell => new Rectangle(1208, 56, 80, 80);
        public static Rectangle FavoriteItemIcon => new Rectangle(200, 144, 24, 24);

        public static Rectangle BarBase => new Rectangle(1408, 64, 192, 48);
        public static Rectangle BarFrame => new Rectangle(1400, 192, 208, 64);
        public const int BarInnerOffcet = 4;

        public static Rectangle CoinIcon => new Rectangle(136, 80, 32, 32);
        public static Rectangle DropdownIcon => new Rectangle(224, 144, 20, 20);
        public static Rectangle SearchbarIcon => new Rectangle(200, 80, 28, 28);
        public static Rectangle SearchbarClearIcon => new Rectangle(138, 146, 26, 26);
        public static Rectangle RNSButtonResearchIcon => new Rectangle(524, 200, 32, 32);
        public static Rectangle RNSJojaButtonIcon => new Rectangle(588, 200, 32, 32);
        public static Rectangle RNSQiButtonIcon => new Rectangle(652, 200, 32, 32);

        public static Rectangle QualitySilverIcon => new Rectangle(200, 208, 32, 32);
        public static Rectangle QualityGoldIcon => new Rectangle(136, 208, 32, 32);
        public static Rectangle QualityIridiumIcon => new Rectangle(72, 208, 32, 32);

        public static Rectangle FavoriteButtonIcon => new Rectangle(72, 80, 36, 32);
        public static Rectangle ProgressButtonIconBase => new Rectangle(72, 144, 32, 32);
        public static Rectangle ProgressButtonIconFill => new Rectangle(200, 176, 16, 16);

        public static Rectangle ProgressButton => new Rectangle(320, 64, 128, 64);
        public static Rectangle FavoriteButton => new Rectangle(320, 128, 128, 64);
        public static Rectangle QualityButton => new Rectangle(320, 192, 128, 64);

        public static Rectangle RNSOutlineEffect => new Rectangle(1800, 40, 116, 116);
        public static Rectangle RNSPentagramEffect => new Rectangle(1928, 40, 116, 116);
        public static Rectangle RNSPentagramEffect2 => new Rectangle(2060, 44, 132, 132);
        public static Rectangle RNSSplashEffect => new Rectangle(1696, 32, 92, 164);

        public static Rectangle DropdownBorderH => new Rectangle(2016, 584, 32, 4);
        public static Rectangle DropdownBorderV => new Rectangle(1992, 256, 4, 32);
        public static Rectangle DropdownGradient => new Rectangle(1920, 256, 32, 320);
        public static Rectangle DropdownBase => new Rectangle(1412, 80, 4, 4);
        public static Rectangle DropdownSelected => new Rectangle(1412, 100, 4, 4);
        public static Rectangle DropdownHover => new Rectangle(1412, 96, 4, 4);

        public static Rectangle LeftArrow => new Rectangle(648, 136, 44, 40);
        public static Rectangle RightArrow => new Rectangle(588, 136, 44, 40);
        public static Rectangle UpArrow => new Rectangle(524, 72, 40, 44);
        public static Rectangle DownArrow => new Rectangle(524, 140, 40, 44);
    }
}