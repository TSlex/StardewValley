using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components
{
    public class ItemResearchArea
    {
        private readonly ClickableComponent _researchArea;
        private readonly ClickableTextureComponent _researchButton;
        private readonly Texture2D _researchTexture;

        public ItemResearchArea(IContentHelper content, IMonitor monitor, int x, int y)
        {
            _researchTexture = content.Load<Texture2D>("assets/search-button.png");

            _researchArea = new ClickableComponent(new Rectangle(x, y, Game1.tileSize + 60, Game1.tileSize + 50), "");

            _researchButton = new ClickableTextureComponent(
                new Rectangle(
                    RenderHelpers.GetChildCenterPosition(x, _researchArea.bounds.Width + 2 * UIConstants.BorderWidth,
                        _researchTexture.Width),
                    _researchArea.bounds.Height + 48 + y, _researchTexture.Width,
                    _researchTexture.Height), _researchTexture,
                new Rectangle(0, 0, _researchTexture.Width, _researchTexture.Height), 1f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawMenuBox(_researchArea.bounds.X, _researchArea.bounds.Y,
                _researchArea.bounds.Width, _researchArea.bounds.Height, out var areaInnerAnchors);

            var researchItemCellX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f - Game1.tileSize / 2f;
            RenderHelpers.DrawItemBox((int) researchItemCellX, (int) areaInnerAnchors.Y + 10, Game1.tileSize,
                Game1.tileSize,
                out _);

            const string researchProgressString = "(0 / 20)";

            var progressFont = Game1.dialogueFont;
            var progressPositionX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f -
                                    progressFont.MeasureString(researchProgressString).X / 2f;

            spriteBatch.DrawString(progressFont, researchProgressString,
                new Vector2(progressPositionX, areaInnerAnchors.Y + Game1.tileSize + 10), Color.Black);

            spriteBatch.Draw(_researchButton.texture, _researchButton.bounds, _researchButton.sourceRect, Color.White);
        }
    }
}