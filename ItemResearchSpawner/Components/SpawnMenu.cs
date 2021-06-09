using System;
using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ItemResearchSpawner.Components
{
    internal class SpawnMenu : ItemGrabMenu
    {
        private readonly SpawnableItem[] _spawnableItems;
        private readonly IMonitor _monitor;
        private readonly Action<SpriteBatch> _baseDraw;
        private readonly IContentHelper _content;

        private const int ItemsPerView = Chest.capacity;
        private const int ItemsPerRow = Chest.capacity / 3;

        private static bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;

        private ItemResearchArea _researchArea;
        private ItemQualitySelectorTab _qualitySelector;
        private ItemSortTab _itemSortTab;
        private ItemCategorySelectorTab _categorySelector;
        private ItemSearchBarTab _searchBarTab;

        private readonly SpawnableItem[] _allItems;
        private readonly List<SpawnableItem> _filteredItems = new List<SpawnableItem>();
        private readonly IList<Item> _itemsInView;

        private int _topRowIndex;
        private int _maxTopRowIndex;

        private ItemQuality _quality;

        public SpawnMenu(SpawnableItem[] spawnableItems, IContentHelper content, IMonitor monitor) : base(
            inventory: new List<Item>(),
            reverseGrab: false,
            showReceivingMenu: true,
            highlightFunction: item => true,
            behaviorOnItemGrab: (item, player) => { },
            behaviorOnItemSelectFunction: (item, player) => { },
            message: null,
            canBeExitedWithKey: true,
            showOrganizeButton: false,
            source: IsAndroid ? source_chest : source_none
        )
        {
            _monitor = monitor;
            _content = content;

            _spawnableItems = spawnableItems;
            _allItems = spawnableItems;
            _itemsInView = ItemsToGrabMenu.actualInventory;

            _baseDraw = RenderHelpers.GetBaseDraw(this);
            drawBG = false; // disable to draw default ui over new menu

            _quality = ItemQuality.Normal;

            InitializeComponents();
            UpdateView(true);
        }

        private void InitializeComponents()
        {
            var rootLeftAnchor = xPositionOnScreen;
            var rootTopAnchor = yPositionOnScreen;
            var rootRightAnchor = rootLeftAnchor + width;
            var rootBottomAnchor = rootTopAnchor + height;

            var sideTopAnchor = rootTopAnchor;
            var sideRightAnchor = rootRightAnchor;

            var barTopAnchor = rootTopAnchor - Game1.tileSize * 2;

            _researchArea = new ItemResearchArea(_content, _monitor, sideRightAnchor, sideTopAnchor);
            _qualitySelector = new ItemQualitySelectorTab(_content, _monitor, rootLeftAnchor - 8, barTopAnchor);
            _itemSortTab = new ItemSortTab(_content, _monitor, _qualitySelector.Bounds.Right + 20, barTopAnchor);
            _categorySelector = new ItemCategorySelectorTab(_content, _monitor, _spawnableItems,
                _itemSortTab.Bounds.Right + 20, _itemSortTab.Bounds.Y);
            _searchBarTab = new ItemSearchBarTab(_content, _monitor, _categorySelector.Bounds.Right + 20, barTopAnchor);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.fadeToBlackRect,
                new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

            _researchArea.Draw(spriteBatch);
            _qualitySelector.Draw(spriteBatch);
            _itemSortTab.Draw(spriteBatch);
            _categorySelector.Draw(spriteBatch);
            _searchBarTab.Draw(spriteBatch);

            //TODO: draw held item

            _baseDraw(spriteBatch);

            // drawMouse(spriteBatch);
        }

        private void UpdateView(bool rebuild = false)
        {
            if (rebuild)
            {
                _filteredItems.Clear();
                _filteredItems.AddRange(SearchItems());
                _topRowIndex = 0;
            }

            var totalRows = (int) Math.Ceiling(_filteredItems.Count / (ItemsPerRow * 1m));

            _maxTopRowIndex = Math.Max(0, totalRows - 3);
            
            ScrollView(0, resetItemView: false);

            _itemsInView.Clear();

            foreach (var prefab in _filteredItems.Skip(_topRowIndex * ItemsPerRow).Take(ItemsPerView))
            {
                var item = prefab.CreateItem();

                item.Stack = item.maximumStackSize();

                if (item is Object obj)
                {
                    obj.Quality = (int) _quality;
                }

                _itemsInView.Add(item);
            }
        }

        private void ScrollView(int direction, bool resetItemView = true)
        {
            if (direction < 0)
            {
                _topRowIndex--;
            }
            else if (direction > 0)
            {
                _topRowIndex++;
            }

            _topRowIndex = (int) MathHelper.Clamp(_topRowIndex, 0, _maxTopRowIndex);

            if (resetItemView)
            {
                UpdateView();
            }
        }

        private IEnumerable<SpawnableItem> SearchItems()
        {
            IEnumerable<SpawnableItem> items = _allItems;

            // items = _sortBy switch
            // {
            //     ItemSortOption.Category => items.OrderBy(p => p.Item.Category),
            //     ItemSortOption.ID => items.OrderBy(p => p.Item.ParentSheetIndex),
            //     _ => items.OrderBy(p => p.Item.DisplayName)
            // };

            // if (!Helpers.EqualsCaseInsensitive(this.CategoryDropdown.Selected, I18n.Filter_All()))
            //     items = items.Where(item => this.EqualsCaseInsensitive(item.Category, this.CategoryDropdown.Selected));

            // string search = this.SearchBox.Text.Trim();

            // if (search != "")
            // {
            //     items = items.Where(item =>
            //         item.Name.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0
            //         || item.DisplayName.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0
            //     );
            // }

            return items;
        }
    }
}