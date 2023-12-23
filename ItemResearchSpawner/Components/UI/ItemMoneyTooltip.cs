using System.IO;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

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
            var buyPrice = ModManager.Instance.GetItemBuyPrice(hoveredItem);
            var sellPrice = ModManager.Instance.GetItemSellPrice(hoveredItem);

            string costText;

            if (buyPrice == sellPrice)
            {
                costText = hoveredItem.Stack > 1 ? $"{buyPrice * hoveredItem.Stack}({buyPrice})" : $"{buyPrice}";
            }
            else
            {
                costText = hoveredItem.Stack > 1 ? 
                    $"Buy {buyPrice * hoveredItem.Stack}({buyPrice}) \nSell {sellPrice * hoveredItem.Stack}({sellPrice})" : 
                    $"Buy {buyPrice} \nSell {sellPrice}";
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