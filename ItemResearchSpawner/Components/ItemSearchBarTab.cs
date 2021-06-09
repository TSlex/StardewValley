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

        // private readonly Rectangle _searchBoxBounds;

        private readonly ClickableComponent _searchBoxArea;
        private readonly ClickableTextureComponent _searchIcon;

        private string _searchText;

        public ItemSearchBarTab(IContentHelper content, IMonitor monitor, int x, int y, int width)
        {
            _searchText = "Pumpkin";

            _searchBoxArea =
                new ClickableComponent(new Rectangle(x, y, width, 36 - 2), "");

            _searchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont,
                Game1.textColor)
            {
                X = _searchBoxArea.bounds.X,
                Y = _searchBoxArea.bounds.Y + 5,
                Height = 0,
                Width = _searchBoxArea.bounds.Width,
                Text = _searchText
            };

            var iconRect = new Rectangle(80, 0, 13, 13);
            const float iconScale = 2.5f;
            
            var iconBounds = new Rectangle((int) (_searchBoxArea.bounds.Right - iconRect.Width * iconScale + 16),
                (int) (_searchBoxArea.bounds.Center.Y + UIConstants.BorderWidth - iconRect.Height / 2f * iconScale + 2),
                (int) (iconRect.Width * iconScale), (int) (iconRect.Height * iconScale)
            );
            
            _searchIcon = new ClickableTextureComponent(iconBounds, Game1.mouseCursors, iconRect, iconScale);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawMenuBox(_searchBoxArea.bounds.X, _searchBoxArea.bounds.Y,
                _searchBoxArea.bounds.Width, _searchBoxArea.bounds.Height, out _);
            
            _searchBox.Draw(spriteBatch);
            
            spriteBatch.Draw(_searchIcon.texture, _searchIcon.bounds, _searchIcon.sourceRect, Color.White);
        }
    }
}