﻿using System;
using System.Reflection;
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

        public static void DrawTextMenuBox(int x, int y, SpriteFont font, string text)
        {
            var spriteBatch = Game1.spriteBatch;
            var bounds = font.MeasureString(text);

            DrawMenuBox(x, y, (int) bounds.X, (int) bounds.Y, out var textPosition);
            Utility.drawTextWithShadow(spriteBatch, text, font, textPosition, Game1.textColor);
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

        public static string GetSpaceIndent(SpriteFont font, int width)
        {
            if (width <= 0)
                return "";

            var indent = " ";

            while (font.MeasureString(indent).X < width)
                indent += " ";

            return indent;
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