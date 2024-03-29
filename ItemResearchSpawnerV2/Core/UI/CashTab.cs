using System.IO;
using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using static StardewValley.Minigames.MineCart;

namespace ItemResearchSpawnerV2.Components.UI
{
    public class CashTab
    {

        private readonly Texture2D _coinTexture;

        private readonly ClickableComponent _balanceArea;
        private readonly ClickableTextureComponent _coin;

        private readonly Func<int> getXPos;
        private readonly Func<int> getYPos;
        private readonly int _width;

        private static IModContentHelper Content => ModManager.Instance.helper.ModContent;

        public CashTab(Func<int> getXPos, Func<int> getYPos, int width)
        {
            this.getXPos = getXPos;
            this.getYPos = getYPos;
            _width = width;

            _coinTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "coin-icon.png"));

            _balanceArea = new ClickableComponent(
                new Rectangle(getXPos(), getYPos(), width, Game1.tileSize), "");

            _coin = new ClickableTextureComponent(
                new Rectangle(_balanceArea.bounds.X + UIConstants.BorderWidth,
                    _balanceArea.bounds.Y + UIConstants.BorderWidth, _coinTexture.Width, Game1.tileSize), _coinTexture,
                new Rectangle(0, 0, _coinTexture.Width, _coinTexture.Height), Game1.pixelZoom);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _balanceArea.bounds.X = getXPos();
            _balanceArea.bounds.Y = getYPos();
            _coin.bounds = new Rectangle(_balanceArea.bounds.X + UIConstants.BorderWidth,
                _balanceArea.bounds.Y + UIConstants.BorderWidth, _coinTexture.Width, Game1.tileSize);

            var textOffsetX = _coinTexture.Width * Game1.pixelZoom + 5;

            DrawHelper.DrawTextMenuBox(_balanceArea.bounds.X, _balanceArea.bounds.Y, _width, Game1.smallFont,
                DrawHelper.FillString(Game1.player._money.ToString(), "0", Game1.smallFont, _width - textOffsetX - 5, "+"),
                textOffsetX);

            _coin.draw(spriteBatch);
        }
    }
}