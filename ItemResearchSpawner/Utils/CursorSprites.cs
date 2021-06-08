using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawner.Utils
{
    public static class CursorSprites
    {
        public static Texture2D SpriteMap => Game1.mouseCursors;

        public static readonly Rectangle DownArrow = new Rectangle(12, 76, 40, 44);
        public static readonly Rectangle UpArrow = new Rectangle(76, 72, 40, 44);
        
        public static readonly Rectangle ActiveBackground = new Rectangle(258, 258, 4, 4);
        public static readonly Rectangle InactiveBackground = new Rectangle(269, 258, 4, 4);
        public static readonly Rectangle HoverBackground = new Rectangle(161, 340, 4, 4);

        public static readonly Rectangle SilverStarQuality = new Rectangle(338, 400, 8, 8);
        public static readonly Rectangle GoldStarQuality = new Rectangle(346, 400, 8, 8);
        public static readonly Rectangle IridiumStarQuality = new Rectangle(346, 392, 8, 8);
    }
}