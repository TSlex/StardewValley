using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {

    internal static class ItemTooltip {

        public static void Draw(SpriteBatch b, Item hoveredItem) {

            if (hoveredItem == null) {
                return;
            }

            // --------------------------------------------------------------

            ProgressionItem progressionItem = ModManager.ProgressionManagerInstance.GetProgressionItem(hoveredItem);

            string costText = GetPricesString(hoveredItem);
            string researchText = ModManager.ProgressionManagerInstance.GetItemProgressionText(hoveredItem);

            bool showResearchText = !(progressionItem.ResearchCompleted || progressionItem.CannotResearch) &&
                !(ModManager.Instance.ModMode == ModMode.ResearchPlus || ModManager.Instance.ModMode == ModMode.BuySellPlus);

            bool showCostText = ModManager.Instance.ModMenuActive && ModManager.Instance.ModMode.HasPriceBehaviour() && !progressionItem.CannotResearch;

            string tooltipText = "";
            int numberLines = (showResearchText && showCostText) ? 2 : 1;

            if (showResearchText && showCostText) {
                tooltipText = $"{researchText} \n\n{costText}";
            }
            else if (showResearchText) {
                tooltipText = $"{researchText}";
            }
            else if (showCostText) {
                tooltipText = $"{costText}";
            }

            if (tooltipText == "") { 
                return;
            }

            // --------------------------------------------------------------

            var mousePos = Game1.getMousePosition();

            var textOffsetX = UIConstants.CoinIcon.Width + 4 * 1;
            var textWidth = Game1.smallFont.MeasureString(tooltipText).X;
            var boxWidth = textWidth + UIConstants.CoinIcon.Width;

            var boxPosX = mousePos.X - 4 * 16;
            var boxPosY = mousePos.Y - 4 * 1;

            var basePosition = new Vector2(boxPosX, boxPosY);
            var bounds = Game1.smallFont.MeasureString(tooltipText);

            DrawHelper.DrawTextMenuBox((int) (basePosition.X - boxWidth), (int) (basePosition.Y - 40),
                Game1.smallFont, tooltipText, textOffsetX, paddingY: 4 * 2 + 1, paddingX: 4 * 2);

            var researchIcon = UIConstants.RNSButtonResearchIcon;
            var coinIcon = ModManager.Instance.ModMode switch {
                ModMode.JunimoMagicTrade => UIConstants.JMTCoinIcon,
                ModMode.JunimoMagicTradePlus => UIConstants.JMTCoinIcon,
                _ => UIConstants.CoinIcon,
            };

            // --------------------------------------------------------------

            Vector2 iconNextPos = basePosition + new Vector2(-boxWidth + 4 * 2, -8 * 4 - 1);

            iconNextPos += new Vector2(0, 0);

            if (showResearchText) {
                b.Draw(ModManager.UITextureInstance, iconNextPos,
                    researchIcon, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            }

            if (showResearchText && showCostText) {
                iconNextPos += new Vector2(0, 14 * 4);
            }

            if (showCostText) {
                b.Draw(ModManager.UITextureInstance, iconNextPos,
                    coinIcon, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            }
        }

        private static string GetPricesString(Item hoveredItem) {
            var prices = ModManager.Instance.GetItemPrices(hoveredItem);

            if (prices.buy == prices.sell) {
                return hoveredItem.Stack > 1 ? $"{I18n.Ui_PriceTooltip_Price()} {prices.buy * hoveredItem.Stack} ({prices.buy})" : $"{I18n.Ui_PriceTooltip_Price()} {prices.buy}";
            }
            else {
                return hoveredItem.Stack > 1 ?
                    $"{I18n.Ui_PriceTooltip_Buy()} {prices.buy * hoveredItem.Stack} ({prices.buy}) \n{I18n.Ui_PriceTooltip_Sell()} {prices.sell * hoveredItem.Stack} ({prices.sell})" :
                    $"{I18n.Ui_PriceTooltip_Buy()} {prices.buy} \n{I18n.Ui_PriceTooltip_Sell()} {prices.sell}";
            }
        }
    }
}