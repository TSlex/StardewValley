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
        private ItemSortOption _sortOption;
        private string _searchText;

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

            _qualitySelector.OnQualityChange += OnQualityChange;
            _itemSortTab.OnSortOptionChange += OnSortOptionChange;
            _categorySelector.OnDropdownToggle += OnDropdownToggle;
            _categorySelector.OnCategorySelected += OnCategorySelected;
        }

        private void OnQualityChange(ItemQuality newQuality)
        {
            _quality = newQuality;
            UpdateView();
        }

        private void OnSortOptionChange(ItemSortOption newOption)
        {
            _sortOption = newOption;
            UpdateView(true);
        }

        private void OnDropdownToggle(bool expanded)
        {
            inventory.highlightMethod = _ => !expanded;
            ItemsToGrabMenu.highlightMethod = _ => !expanded;

            if (!expanded && !Game1.lastCursorMotionWasMouse)
            {
                setCurrentlySnappedComponentTo(_categorySelector.MyID);
                snapCursorToCurrentSnappedComponent();
            }
        }

        private void OnCategorySelected(string category)
        {
            UpdateView(true);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (_qualitySelector.Bounds.Contains(x, y))
            {
                _qualitySelector.HandleLeftClick();
            }
            else if (_itemSortTab.Bounds.Contains(x, y))
            {
                _itemSortTab.HandleLeftClick();
            }
            else if (_categorySelector.TryClick(x, y))
            {
            }
            else
            {
                base.receiveLeftClick(x, y, playSound);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (_qualitySelector.Bounds.Contains(x, y))
            {
                _qualitySelector.HandleRightClick();
            }
            else if (_itemSortTab.Bounds.Contains(x, y))
            {
                _itemSortTab.HandleRightClick();
            }
            else
            {
                base.receiveRightClick(x, y, playSound);
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            ScrollView(-direction);
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
            _qualitySelector =
                new ItemQualitySelectorTab(_content, _monitor, rootLeftAnchor - 8, barTopAnchor, _quality);
            _itemSortTab = new ItemSortTab(_content, _monitor, _qualitySelector.Bounds.Right + 20, barTopAnchor,
                _sortOption);
            _categorySelector = new ItemCategorySelectorTab(_content, _monitor, _spawnableItems,
                _itemSortTab.Bounds.Right + 20, _itemSortTab.Bounds.Y);
            _searchBarTab = new ItemSearchBarTab(_content, _monitor, _categorySelector.Bounds.Right + 20, barTopAnchor);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.fadeToBlackRect,
                new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

            _baseDraw(spriteBatch);

            _researchArea.Draw(spriteBatch);
            _qualitySelector.Draw(spriteBatch);
            _itemSortTab.Draw(spriteBatch);
            _categorySelector.Draw(spriteBatch);
            // _searchBarTab.Draw(spriteBatch);

            //TODO: draw held item

            // _baseDraw(spriteBatch);

            drawMouse(spriteBatch);
        }

        private void UpdateView(bool rebuild = false)
        {
            if (rebuild)
            {
                _filteredItems.Clear();
                _filteredItems.AddRange(GetFilteredItems());
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

        private IEnumerable<SpawnableItem> GetFilteredItems()
        {
            IEnumerable<SpawnableItem> items = _allItems;

            items = _sortOption switch
            {
                ItemSortOption.Category => items.OrderBy(p => p.Item.Category),
                ItemSortOption.ID => items.OrderBy(p => p.Item.ParentSheetIndex),
                _ => items.OrderBy(p => p.Item.DisplayName)
            };

            if (!Helpers.EqualsCaseInsensitive(_categorySelector.SelectedCategory, "All"))
            {
                items = items.Where(item => Helpers.EqualsCaseInsensitive(item.Category, _categorySelector.SelectedCategory));
            }

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