using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawner.Components
{
    public class ItemMoneyTooltip
    {
        private readonly Texture2D _coinTexture;

        public ItemMoneyTooltip(IContentHelper content, IMonitor monitor)
        {
            _coinTexture = content.Load<Texture2D>("assets/coin-icon.png");
        }

        public void Draw(SpriteBatch spriteBatch, Item hoveredItem)
        {
            var cost = ModManager.Instance.GetItemBuyPrice(hoveredItem);

            if (cost <= 0)
            {
                return;
            }
            
            var mousePos = Game1.getMousePosition();
            var mousePosVector = new Vector2(mousePos.X, mousePos.Y);

            var textOffsetX = _coinTexture.Width * Game1.pixelZoom + 5;

            RenderHelpers.DrawTextMenuBox(mousePos.X + 32, mousePos.Y - 40, Game1.smallFont, cost.ToString(), textOffsetX);
            Utility.drawWithShadow(spriteBatch, _coinTexture, mousePosVector + new Vector2(48, -24),
                _coinTexture.Bounds, Color.White, 0f, Vector2.Zero, shadowIntensity: 0f);
        }
    }
}