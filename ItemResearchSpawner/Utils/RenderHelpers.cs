using System;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Utils
{
    internal static class RenderHelpers
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

        public static void DrawTextMenuBox(int x, int y, SpriteFont font, string text, int offsetX = 0, int offsetY = 0)
        {
            var spriteBatch = Game1.spriteBatch;
            var bounds = font.MeasureString(text);

            var additionalBounds = new Vector2(offsetX > 0 ? offsetX : 0, offsetY > 0 ? offsetY : 0);

            DrawMenuBox(x, y, (int) ((int) bounds.X + additionalBounds.X), (int) ((int) bounds.Y + additionalBounds.Y),
                out var textPosition);

            Utility.drawTextWithShadow(spriteBatch, text, font,
                new Vector2(textPosition.X + offsetX, textPosition.Y + offsetY), Game1.textColor);
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

        public static int GetChildCenterPosition(int pos, int parentLenght, int childLenght)
        {
            return (int) (pos + parentLenght / 2f - childLenght / 2f);
        }

        public static int GetLabelWidth(SpriteFont font)
        {
            return (int) font.MeasureString("THISISLABELWIDTHYEAH").X;
        }

        public static string TruncateString(string value, SpriteFont font, int maxWidth)
        {
            // var smallSymWidth = font.MeasureString("a").X;
            // var capSymWidth = font.MeasureString("A").X;
            var overflowWidth = font.MeasureString("...").X;

            var newString = new StringBuilder();
            var width = 0f;

            foreach (var ch in value)
            {
                var charWidth = font.MeasureString(ch.ToString()).X;

                if (width + charWidth > maxWidth)
                {
                    newString.Append("...");
                    break;
                }

                newString.Append(ch);
                width += charWidth;
            }

            return newString.ToString();
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