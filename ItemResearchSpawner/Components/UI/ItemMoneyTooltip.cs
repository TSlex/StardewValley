using System.IO;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawner.Components.UI
{
    public class ItemMoneyTooltip
    {
        private readonly Texture2D _coinTexture;

        public ItemMoneyTooltip(IContentHelper content, IMonitor monitor)
        {
            // _coinTexture = content.Load<Texture2D>("assets/images/coin-icon.png");
            _coinTexture = content.Load<Texture2D>(Path.Combine("assets", "images", "coin-icon.png"));
        }

        public void Draw(SpriteBatch spriteBatch, Item hoveredItem)
        {
            var cost = ModManager.Instance.GetItemPrice(hoveredItem);
            
            var costText = hoveredItem.Stack > 1 ? $"{cost * hoveredItem.Stack}({cost})" : $"{cost}";
            
            if (cost <= 0)
            {
                costText = "0";
            }

            var mousePos = Game1.getMousePosition();
            var basePosition = new Vector2(mousePos.X, mousePos.Y) + new Vector2(-38, 0);

            var textOffsetX = _coinTexture.Width * Game1.pixelZoom + 5;
            var textWidth = Game1.smallFont.MeasureString(costText).X;

            var boxWidth = textWidth + UIConstants.BorderWidth * 2 + _coinTexture.Width;

            RenderHelpers.DrawTextMenuBox((int) (basePosition.X - boxWidth), (int) (basePosition.Y - 40),
                Game1.smallFont, costText, textOffsetX);

            Utility.drawWithShadow(spriteBatch, _coinTexture, basePosition + new Vector2(-boxWidth + 16, -24),
                _coinTexture.Bounds, Color.White, 0f, Vector2.Zero, shadowIntensity: 0f);
        }
    }
}