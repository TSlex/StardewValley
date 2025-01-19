using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Components.UI
{
    public class CashTab
    {

        //private readonly Texture2D _coinTexture;

        private readonly ClickableComponent BalanceArea;
        //private readonly ClickableTextureComponent Coin;

        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;
        private readonly int Width;

        private static IModContentHelper Content => ModManager.Instance.Helper.ModContent;


        public CashTab(Func<int> getXPos, Func<int> getYPos, int width)
        {
            GetXPos = getXPos;
            GetYPos = getYPos;
            Width = width;

            //_coinTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "coin-icon.png"));

            BalanceArea = new ClickableComponent(
                new Rectangle(getXPos(), getYPos(), width, Game1.tileSize), "");

            //Coin = new ClickableTextureComponent(
            //    new Rectangle(BalanceArea.bounds.X + UIConstants.BorderWidth,
            //        BalanceArea.bounds.Y + UIConstants.BorderWidth, _coinTexture.Width, Game1.tileSize), _coinTexture,
            //    new Rectangle(0, 0, _coinTexture.Width, _coinTexture.Height), Game1.pixelZoom);
        }

        public void Draw(SpriteBatch b)
        {
            BalanceArea.bounds.X = GetXPos();
            BalanceArea.bounds.Y = GetYPos();

            DrawHelper.DrawMenuBox(BalanceArea.bounds.X, BalanceArea.bounds.Y, Width, 48, out var textPosition);

            var coinIcon = ModManager.Instance.ModMode switch {
                ModMode.JunimoMagicTrade => UIConstants.JMTCoinIcon,
                ModMode.JunimoMagicTradePlus => UIConstants.JMTCoinIcon,
                _ => UIConstants.CoinIcon,
            };

            b.Draw(ModManager.UITextureInstance, new Vector2(BalanceArea.bounds.X + 4 * 4, BalanceArea.bounds.Y + 4 * 4),
                coinIcon, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);

            var textOffsetX = UIConstants.CoinIcon.Width + 4 * 8;

            //var playerMoney = Game1.player._money + 10000000000;
            var playerMoney = ModManager.Instance.PlayerMoney + 10000000000;

            playerMoney = playerMoney >= 20000000000 ? 19999999999 : playerMoney;
            var playerMoneyStr = new string(playerMoney.ToString().Skip(1).ToArray());

            //Utility.drawTextWithShadow(b, 
            //    DrawHelper.FillString(Game1.player._money.ToString(), "0", Game1.smallFont, Width - textOffsetX, "+"), 
            //    Game1.smallFont,
            //    new Vector2(textPosition.X + textOffsetX - 12, textPosition.Y + 8), 
            //    Game1.textColor);

            Utility.drawTextWithShadow(b,
                playerMoneyStr,
                Game1.smallFont,
                new Vector2(textPosition.X + textOffsetX - 12, textPosition.Y + 8),
                Game1.textColor);


            //Coin.bounds = new Rectangle(BalanceArea.bounds.X + UIConstants.BorderWidth,
            //    BalanceArea.bounds.Y + UIConstants.BorderWidth, _coinTexture.Width, Game1.tileSize);

            //var textOffsetX = _coinTexture.Width * Game1.pixelZoom + 5;

            //DrawHelper.DrawTextMenuBox(BalanceArea.bounds.X, BalanceArea.bounds.Y, Width, Game1.smallFont,
            //    DrawHelper.FillString(Game1.player._money.ToString(), "0", Game1.smallFont, Width - textOffsetX - 5, "+"),
            //    textOffsetX);

            //Coin.draw(spriteBatch);
        }
    }
}