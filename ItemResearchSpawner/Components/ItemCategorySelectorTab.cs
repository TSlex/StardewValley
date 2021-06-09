using System;
using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawner.Components
{
    public class ItemCategorySelectorTab
    {
        private readonly Dropdown<string> _categoryDropdown;

        private readonly string[] _availableCategories;

        public ItemCategorySelectorTab(IContentHelper content, IMonitor monitor, SpawnableItem[] spawnableItems, int x,
            int y)
        {
            _availableCategories = GetDisplayCategories(spawnableItems).ToArray();

            _categoryDropdown = new Dropdown<string>(x, y, Game1.smallFont, _categoryDropdown?.Selected ?? "All",
                _availableCategories, p => p);

            // _categoryDropdown.IsExpanded = false;
        }

        public Rectangle Bounds => _categoryDropdown.bounds;

        public void Draw(SpriteBatch spriteBatch)
        {
            var position = new Vector2(
                x: _categoryDropdown.bounds.X + _categoryDropdown.bounds.Width - 12,
                y: _categoryDropdown.bounds.Y + 8
            );

            var sourceRect = CursorSprites.DropdownButton;

            spriteBatch.Draw(Game1.mouseCursors, position, sourceRect, Color.White, 0, Vector2.Zero, Game1.pixelZoom,
                SpriteEffects.None, 1f);

            if (_categoryDropdown.IsExpanded)
            {
                spriteBatch.Draw(Game1.mouseCursors,
                    new Vector2(position.X + 2 * Game1.pixelZoom, position.Y + 3 * Game1.pixelZoom),
                    new Rectangle(sourceRect.X + 2, sourceRect.Y + 3, 5, 6), Color.White, 0, Vector2.Zero,
                    Game1.pixelZoom, SpriteEffects.FlipVertically, 1f);
            }

            _categoryDropdown.Draw(spriteBatch);
        }

        private IEnumerable<string> GetDisplayCategories(SpawnableItem[] items)
        {
            var categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                if (item.Category.ToLower().Equals("all") || item.Category.ToLower().Equals("misc"))
                {
                    continue;
                }

                categories.Add(item.Category);
            }

            yield return "all";

            foreach (var category in categories.OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                yield return category;
            }

            yield return "misc";
        }
    }
}