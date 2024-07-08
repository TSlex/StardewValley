using System.IO;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using static System.Net.Mime.MediaTypeNames;

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
                costText = hoveredItem.Stack > 1 ? $"{prices.buy * hoveredItem.Stack} ({prices.buy})" : $"{prices.buy}";
            }
            else {
                costText = hoveredItem.Stack > 1 ?
                    $"{I18n.Ui_PriceTooltip_Buy()} {prices.buy * hoveredItem.Stack} ({prices.buy}) \n{I18n.Ui_PriceTooltip_Sell()} {prices.sell * hoveredItem.Stack} ({prices.sell})" :
                    $"{I18n.Ui_PriceTooltip_Buy()} {prices.buy} \n{I18n.Ui_PriceTooltip_Sell()} {prices.sell}";
            }

            //var costText = "0";

            var mousePos = Game1.getMousePosition();

            var textOffsetX = UIConstants.CoinIcon.Width + 4 * 1;
            var textWidth = Game1.smallFont.MeasureString(costText).X;
            var boxWidth = textWidth + CoinTexture.Width;

            //var boxPosX = mousePos.X - boxWidth / 2 + 4 * 12;
            var boxPosX = mousePos.X - 4 * 16;
            var boxPosY = mousePos.Y - 4 * 1;

            //boxPosX = boxPosX < 250 ? 250 : boxPosX;

            var basePosition = new Vector2(boxPosX, boxPosY);
            var bounds = Game1.smallFont.MeasureString(costText);

            DrawHelper.DrawTextMenuBox((int)(basePosition.X - boxWidth), (int)(basePosition.Y - 40),
                Game1.smallFont, costText, textOffsetX, paddingY: 4 * 2 + 1, paddingX: 4 * 2);

            b.Draw(ModManager.UITextureInstance, basePosition + new Vector2(-boxWidth + 4 * 2, -4 * 10 - 1 + bounds.Y / 4),
                UIConstants.CoinIcon, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);


            //Utility.drawWithShadow(b, CoinTexture, basePosition + new Vector2(-boxWidth + 16, -24),
            //    CoinTexture.Bounds, Color.White, 0f, Vector2.Zero, shadowIntensity: 0f);
        }
    }
}