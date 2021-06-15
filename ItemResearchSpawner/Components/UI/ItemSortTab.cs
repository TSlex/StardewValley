using System;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components
{
    /**
        MIT License

        Copyright (c) 2018 CJBok

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
     **/
    internal class ItemSortTab
    {
        private readonly Texture2D _sortTexture;

        private readonly ClickableComponent _sortButton;
        private readonly ClickableTextureComponent _sortIcon;

        // private readonly string _sortLabelIndent;

        private ItemSortOption _sortOption;

        public delegate void SortOptionChange(ItemSortOption newOption);

        public event SortOptionChange OnSortOptionChange;

        public ItemSortTab(IContentHelper content, IMonitor monitor, int x, int y, ItemSortOption initialSortOption)
        {
            _sortTexture = content.Load<Texture2D>("assets/sort-icon.png");
            // _sortLabelIndent = RenderHelpers.GetSpaceIndent(Game1.smallFont, _sortTexture.Width) + " ";

            _sortOption = initialSortOption;

            _sortButton =
                new ClickableComponent(
                    new Rectangle(x, y,
                        GetMaxSortLabelWidth(Game1.smallFont) + _sortTexture.Width * Game1.pixelZoom +
                        5 + UIConstants.BorderWidth, Game1.tileSize), GetSortLabel(_sortOption));

            _sortIcon = new ClickableTextureComponent(
                new Rectangle(_sortButton.bounds.X + UIConstants.BorderWidth,
                    y + UIConstants.BorderWidth, _sortTexture.Width, Game1.tileSize), _sortTexture,
                new Rectangle(0, 0, _sortTexture.Width, _sortTexture.Height), Game1.pixelZoom);
        }

        public Rectangle Bounds => _sortButton.bounds;

        public void HandleLeftClick()
        {
            _sortOption = _sortOption.GetNext();
            _sortButton.label = _sortButton.name = GetSortLabel(_sortOption);
            OnSortOptionChange?.Invoke(_sortOption);
        }

        public void HandleRightClick()
        {
            _sortOption = _sortOption.GetPrevious();
            _sortButton.label = _sortButton.name = GetSortLabel(_sortOption);
            OnSortOptionChange?.Invoke(_sortOption);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawTextMenuBox(_sortButton.bounds.X, _sortButton.bounds.Y, Game1.smallFont,
                _sortButton.name, _sortTexture.Width * Game1.pixelZoom + 5);
            _sortIcon.draw(spriteBatch);
        }

        private int GetMaxSortLabelWidth(SpriteFont font)
        {
            return
                (
                    from ItemSortOption key in Enum.GetValues(typeof(ItemSortOption))
                    let text = GetSortLabel(key)
                    select (int) font.MeasureString(text).X
                )
                .Max();
        }

        private string GetSortLabel(ItemSortOption sort)
        {
            return /*_sortLabelIndent + */sort switch
            {
                ItemSortOption.Name => "Name",
                ItemSortOption.Category => "Category",
                ItemSortOption.ID => "ID",
                _ => throw new NotSupportedException($"Invalid sort type {sort}.")
            };
        }
    }
}