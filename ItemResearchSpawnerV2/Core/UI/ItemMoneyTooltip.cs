using System.IO;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace ItemResearchSpawnerV2.Core.UI {

    public class ItemMoneyTooltip {
        private readonly Texture2D CoinTexture;

        public ItemMoneyTooltip() {
            CoinTexture = ModManager.Instance.Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "images", "coin-icon.png"));
        }

        public void Draw(SpriteBatch b, Item hoveredItem) {
            if (hoveredItem == null) { 
                return;
            }

            var prices = ModManager.Instance.GetItemPrices(hoveredItem);

            string costText;

            if (prices.buy == prices.sell) {
                costText = hoveredItem.Stack > 1 ? $"{prices.buy * hoveredItem.Stack}({prices.buy})" : $"{prices.buy}";
            }
            else {
                costText = hoveredItem.Stack > 1 ?
                    $"{I18n.Ui_PriceTooltip_Buy()} {prices.buy * hoveredItem.Stack}({prices.buy}) \n{I18n.Ui_PriceTooltip_Sell()} {prices.sell * hoveredItem.Stack}({prices.sell})" :
                    $"{I18n.Ui_PriceTooltip_Buy()} {prices.buy} \n{I18n.Ui_PriceTooltip_Sell()} {prices.sell}";
            }

            //var costText = "0";

            var mousePos = Game1.getMousePosition();
            var basePosition = new Vector2(mousePos.X, mousePos.Y) + new Vector2(-38, 0);

            var textOffsetX = CoinTexture.Width * Game1.pixelZoom + 5;
            var textWidth = Game1.smallFont.MeasureString(costText).X;

            var boxWidth = textWidth + UIConstants.BorderWidth * 2 + CoinTexture.Width;

            DrawHelper.DrawTextMenuBox((int)(basePosition.X - boxWidth), (int)(basePosition.Y - 40),
                Game1.smallFont, costText, textOffsetX);

            Utility.drawWithShadow(b, CoinTexture, basePosition + new Vector2(-boxWidth + 16, -24),
                CoinTexture.Bounds, Color.White, 0f, Vector2.Zero, shadowIntensity: 0f);
        }
    }
}