using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {

    public class ItemMoneyTooltip {

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

            var mousePos = Game1.getMousePosition();

            var textOffsetX = UIConstants.CoinIcon.Width + 4 * 1;
            var textWidth = Game1.smallFont.MeasureString(costText).X;
            var boxWidth = textWidth + UIConstants.CoinIcon.Width;

            var boxPosX = mousePos.X - 4 * 16;
            var boxPosY = mousePos.Y - 4 * 1;

            var basePosition = new Vector2(boxPosX, boxPosY);
            var bounds = Game1.smallFont.MeasureString(costText);

            DrawHelper.DrawTextMenuBox((int)(basePosition.X - boxWidth), (int)(basePosition.Y - 40),
                Game1.smallFont, costText, textOffsetX, paddingY: 4 * 2 + 1, paddingX: 4 * 2);

            var coinIcon = ModManager.Instance.ModMode switch {
                ModMode.JunimoMagicTrade => UIConstants.JMTCoinIcon,
                ModMode.JunimoMagicTradePlus => UIConstants.JMTCoinIcon,
                _ => UIConstants.CoinIcon,
            };

            b.Draw(ModManager.UITextureInstance, basePosition + new Vector2(-boxWidth + 4 * 2, -4 * 10 - 1 + bounds.Y / 4),
                coinIcon, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        }
    }
}