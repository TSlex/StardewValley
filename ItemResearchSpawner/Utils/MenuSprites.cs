﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawner.Utils
{
    public static class MenuSprites
    {
        public static Texture2D SpriteMap => Game1.menuTexture;
        
        public static readonly Rectangle MenuSmallBorder = new Rectangle(0, 256, 60, 60);
        public static readonly Rectangle ItemCell = new Rectangle(128, 128, 64, 64);
    }
}