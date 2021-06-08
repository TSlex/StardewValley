using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Utils
{
    internal static class RenderHelper
    {
        public static void DrawMenuBox(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition)
        {
            var spriteBatch = Game1.spriteBatch;

            var outerWidth = innerWidth + Constants.BorderWidth * 2;
            var outerHeight = innerHeight + Constants.BorderWidth * 2;

            IClickableMenu.drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                TextureRects.MenuSmallBorder,
                x,
                y,
                outerWidth,
                outerHeight,
                Color.White,
                1, 
                false);

            innerDrawPosition = new Vector2(x + Constants.BorderWidth, y + Constants.BorderWidth);
        }
        
        public static void DrawItemBox(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition)
        {
            var spriteBatch = Game1.spriteBatch;
            
            IClickableMenu.drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                TextureRects.ItemCell,
                x,
                y,
                innerWidth,
                innerHeight,
                Color.White,
                1, 
                false);

            innerDrawPosition = new Vector2(x, y);
        }
    }
}