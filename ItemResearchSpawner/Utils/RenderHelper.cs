using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Utils
{
    internal static class RenderHelper
    {
        public static void DrawMenuBox(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition)
        {
            var spriteBatch = Game1.spriteBatch;

            var outerWidth = innerWidth + UIConstants.BorderWidth * 2;
            var outerHeight = innerHeight + UIConstants.BorderWidth * 2;

            IClickableMenu.drawTextureBox(
                spriteBatch,
                MenuSprites.SpriteMap,
                MenuSprites.MenuSmallBorder,
                x,
                y,
                outerWidth,
                outerHeight,
                Color.White,
                1, 
                false);

            innerDrawPosition = new Vector2(x + UIConstants.BorderWidth, y + UIConstants.BorderWidth);
        }
        
        public static void DrawItemBox(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition)
        {
            var spriteBatch = Game1.spriteBatch;
            
            IClickableMenu.drawTextureBox(
                spriteBatch,
                MenuSprites.SpriteMap,
                MenuSprites.ItemCell,
                x,
                y,
                innerWidth,
                innerHeight,
                Color.White,
                1, 
                false);

            innerDrawPosition = new Vector2(x, y);
        }
        
        public static Action<SpriteBatch> GetBaseDraw(object instance)
        {
            var method =
                typeof(ItemGrabMenu).GetMethod("draw", BindingFlags.Instance | BindingFlags.Public, null,
                    new[] {typeof(SpriteBatch)}, null) ??
                throw new InvalidOperationException(
                    $"Can't find {nameof(ItemGrabMenu)}.{nameof(ItemGrabMenu.draw)} method.");

            var pointer = method.MethodHandle.GetFunctionPointer();

            return (Action<SpriteBatch>) Activator.CreateInstance(typeof(Action<SpriteBatch>), instance, pointer);
        }
    }
}