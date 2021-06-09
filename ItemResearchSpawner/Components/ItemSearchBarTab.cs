using System;
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
        private float _iconOpacity;
        private bool _persistFocus;

        public delegate void SearchTextInput(string key);

        public event SearchTextInput OnSearchTextInput;

        public ItemSearchBarTab(IContentHelper content, IMonitor monitor, int x, int y, int width)
        {
            _searchText = "";
            _iconOpacity = 1f;

            _searchBoxArea =
                new ClickableComponent(
                    new Rectangle(x, y, width + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");

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

            var iconBounds = new Rectangle((int) (_searchBoxArea.bounds.Right - iconRect.Width * iconScale),
                (int) (_searchBoxArea.bounds.Center.Y  + UIConstants.BorderWidth / 2 - iconRect.Height / 2f * iconScale + 2),
                (int) (iconRect.Width * iconScale), (int) (iconRect.Height * iconScale)
            );

            _searchIcon = new ClickableTextureComponent(iconBounds, Game1.mouseCursors, iconRect, iconScale);
        }

        public void Focus(bool persist)
        {
            _searchBox.Selected = true;
            _persistFocus = persist;
        }

        public void Blur()
        {
            _searchBox.Selected = false;
            _persistFocus = false;
        }

        public bool Selected => _searchBox.Selected;
        public bool PersistFocus => _persistFocus;

        public Rectangle Bounds => _searchBoxArea.bounds;

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawMenuBox(_searchBoxArea.bounds.X, _searchBoxArea.bounds.Y,
                _searchBoxArea.bounds.Width - UIConstants.BorderWidth,
                _searchBoxArea.bounds.Height - UIConstants.BorderWidth, out _);

            _searchBox.Draw(spriteBatch);

            spriteBatch.Draw(_searchIcon.texture, _searchIcon.bounds, _searchIcon.sourceRect,
                Color.White * _iconOpacity);
        }

        public void Update(GameTime time)
        {
            if (_persistFocus && !_searchBox.Selected)
            {
                Blur();
            }

            if (_searchText != _searchBox.Text.Trim())
            {
                _searchText = _searchBox.Text.Trim();
                OnSearchTextInput?.Invoke(_searchText);
            }


            var delta = 1.5f / time.ElapsedGameTime.Milliseconds;

            if (!_searchBox.Selected && _iconOpacity < 1f)
            {
                _iconOpacity = Math.Min(1f, _iconOpacity + delta);
            }
            else if (_searchBox.Selected && _iconOpacity > 0f)
            {
                _iconOpacity = Math.Max(0f, _iconOpacity - delta);
            }
        }

        public void Clear()
        {
            _searchBox.Text = "";
        }
    }
}