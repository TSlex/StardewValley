using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components
{
    public class ItemSearchBarTab
    {
        private TextBox _searchBox;

        private readonly Rectangle _searchBoxBounds;

        private readonly ClickableComponent _searchBoxArea;
        private readonly ClickableTextureComponent _searchIcon;

        private string _searchText;


        public ItemSearchBarTab(IContentHelper content, IMonitor monitor, int x, int y)
        {
            _searchText = "Pumpkin";

            _searchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont,
                Game1.textColor)
            {
                X = x,
                Y = y,
                Height = 0,
                Width = 200,
                Text = _searchText
            };

            _searchBoxBounds = new Rectangle(_searchBox.X, _searchBox.Y + 4, _searchBox.Width, 36);

            _searchBoxArea =
                new ClickableComponent(
                    new Rectangle(_searchBoxBounds.X, _searchBoxBounds.Y, _searchBoxBounds.Width,
                        _searchBoxBounds.Height), "");

            var iconRect = new Rectangle(80, 0, 13, 13);
            const float iconScale = 2.5f;

            var iconBounds = new Rectangle((int) (_searchBoxBounds.Right - iconRect.Width * iconScale),
                (int) (_searchBoxBounds.Center.Y - iconRect.Height / 2f * iconScale),
                (int) (iconRect.Width * iconScale), (int) (iconRect.Height * iconScale)
            );

            _searchIcon = new ClickableTextureComponent(iconBounds, Game1.mouseCursors, iconRect, iconScale);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawMenuBox(_searchBoxBounds.X, _searchBoxBounds.Y - UIConstants.BorderWidth / 2,
                _searchBoxBounds.Width - UIConstants.BorderWidth * 3 / 2,
                _searchBoxBounds.Height - UIConstants.BorderWidth, out _);

            _searchBox.Draw(spriteBatch);
            spriteBatch.Draw(_searchIcon.texture, _searchIcon.bounds, _searchIcon.sourceRect, Color.White);
        }
    }
}