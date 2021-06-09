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
    public class ItemSortTab
    {
        private readonly Texture2D _sortTexture;

        private readonly ClickableComponent _sortButton;
        private readonly ClickableTextureComponent _sortIcon;

        private readonly string _sortLabelIndent;

        private ItemSortOption _sortOption;

        public delegate void SortOptionChange(ItemSortOption newOption);

        public event SortOptionChange OnSortOptionChange;

        public ItemSortTab(IContentHelper content, IMonitor monitor, int x, int y, ItemSortOption initialSortOption)
        {
            _sortTexture = content.Load<Texture2D>("assets/sort-icon.png");
            _sortLabelIndent = RenderHelpers.GetSpaceIndent(Game1.smallFont, _sortTexture.Width) + " ";

            _sortOption = initialSortOption;

            _sortButton =
                new ClickableComponent(
                    new Rectangle(x, y, GetMaxSortLabelWidth(Game1.smallFont) + UIConstants.BorderWidth,
                        Game1.tileSize), GetSortLabel(_sortOption));

            _sortIcon = new ClickableTextureComponent(
                new Rectangle(_sortButton.bounds.X + UIConstants.BorderWidth,
                    y + UIConstants.BorderWidth, _sortTexture.Width, Game1.tileSize), _sortTexture,
                new Rectangle(0, 0, _sortTexture.Width, _sortTexture.Height), 1f);
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
                _sortButton.name);
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
            return _sortLabelIndent + sort switch
            {
                ItemSortOption.Name => "Name",
                ItemSortOption.Category => "Category",
                ItemSortOption.ID => "ID",
                _ => throw new NotSupportedException($"Invalid sort type {sort}.")
            };
        }
    }
}