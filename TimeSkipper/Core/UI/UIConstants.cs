using Microsoft.Xna.Framework;
using StardewValley;

namespace TimeSkipper.Core.UI {
    internal static class UIConstants {
        public const int BorderWidth = 4 * Game1.pixelZoom;

        public static string AssetsPath => "assets";
        public static string UISheet => Path.Combine(AssetsPath, "images", "spritesheet.png");

        public static Rectangle BarBase => new Rectangle(196, 264, 56, 48);
        public static Rectangle BarFrame => new Rectangle(0, 256, 60, 60);

        public static Rectangle MenuFrame => new Rectangle(16, 16, 160, 160);

        public static Rectangle DropdownIcon => new Rectangle(132, 324, 20, 20);

        public static Rectangle RightArrow => new Rectangle(4, 324, 44, 40);
        public static Rectangle LeftArrow => new Rectangle(68, 324, 44, 40);
        public static Rectangle UpArrow => new Rectangle(196, 324, 40, 44);
        public static Rectangle DownArrow => new Rectangle(196, 388, 40, 44);

        public static Rectangle BaseBackground => new Rectangle(128, 256, 64, 64);

        public static Rectangle CalendarBackground => new Rectangle(64, 384, 4, 4);
        public static Rectangle CalendarCell => new Rectangle(64, 384, 64, 64);
        public static Rectangle CalendarCellGap => new Rectangle(60, 380, 4, 4);
        public static Rectangle CalendarCellWithFrame => new Rectangle(60, 380, 72, 72);

        public static Rectangle CalendarCellSelectedFrame => new Rectangle(212, 20, 88, 84);
        public static Rectangle CalendarCellSelectedBackground => new Rectangle(192, 128, 64, 64);

        public static Rectangle SleepButton => new Rectangle(64, 256, 52, 52);

        public static Rectangle HeaderText => new Rectangle(8, 520, 272, 46);

        public static Rectangle DropdownBorderH => new Rectangle(8, 264, 4, 4);
        public static Rectangle DropdownBorderV => new Rectangle(8, 264, 4, 4);
        public static Rectangle DropdownGradient => new Rectangle(256, 128, 32, 320);
        public static Rectangle DropdownBase => new Rectangle(200, 272, 4, 4);
        public static Rectangle DropdownSelected => new Rectangle(200, 300, 4, 4);
        public static Rectangle DropdownHover => new Rectangle(200, 284, 4, 4);

        //public const int BarInnerOffcet = 4;

        //public static Rectangle DownArrow => new Rectangle(524, 140, 40, 44);
    }
}