﻿using System.IO;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components.UI
{
    public class CashTab
    {
        private readonly int _width;
        private readonly Texture2D _coinTexture;

        private readonly ClickableComponent _balanceArea;
        private readonly ClickableTextureComponent _coin;

        public CashTab(IContentHelper content, IMonitor monitor, int x, int y, int width)
        {
            _width = width;
            // _coinTexture = content.Load<Texture2D>("assets/images/coin-icon.png");
            _coinTexture = content.Load<Texture2D>(Path.Combine("assets", "images", "coin-icon.png"));

            _balanceArea = new ClickableComponent(
                new Rectangle(x, y, width, Game1.tileSize), "");

            _coin = new ClickableTextureComponent(
                new Rectangle(_balanceArea.bounds.X + UIConstants.BorderWidth,
                    y + UIConstants.BorderWidth, _coinTexture.Width, Game1.tileSize), _coinTexture,
                new Rectangle(0, 0, _coinTexture.Width, _coinTexture.Height), Game1.pixelZoom);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var textOffsetX = _coinTexture.Width * Game1.pixelZoom + 5;
            RenderHelpers.DrawTextMenuBox(_balanceArea.bounds.X, _balanceArea.bounds.Y, _width, Game1.smallFont,
                RenderHelpers.FillString(Game1.player._money.ToString(), "0", Game1.smallFont, _width - textOffsetX - 5, "+"),
                textOffsetX);

            _coin.draw(spriteBatch);
        }
    }
}