﻿using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private bool _overDropdown;

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
            _sortOption = ItemSortOption.Category;
            _searchText = "";

            InitializeComponents();
            UpdateView(true);

            _qualitySelector.OnQualityChange += OnQualityChange;
            _itemSortTab.OnSortOptionChange += OnSortOptionChange;
            _categorySelector.OnDropdownToggle += OnDropdownToggle;
            _categorySelector.OnCategorySelected += OnCategorySelected;
            _searchBarTab.OnSearchTextInput += OnSearchTextInput;
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

        private void OnSearchTextInput(string key)
        {
            _searchText = key;
            _topRowIndex = 0;
            UpdateView(rebuild: true);
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
            else if (_researchArea.Bounds.Contains(x, y))
            {
                OnResearchAreaLeftClick();
            }
            else if (_researchArea.ButtonBounds.Contains(x, y))
            {
                _researchArea.HandleResearch();
            }
            else if (_searchBarTab.Bounds.Contains(x, y))
            {
                if (!_searchBarTab.Selected || !_searchBarTab.PersistFocus)
                    _searchBarTab.Focus(true);
            }
            else if (trashCan.containsPoint(x, y) && heldItem != null)
            {
                TryTrashItem();
            }
            else
            {
                if (_searchBarTab.Selected)
                {
                    _searchBarTab.Blur();
                }

                base.receiveLeftClick(x, y, playSound);
            }
        }

        private void OnResearchAreaLeftClick()
        {
            if (_researchArea.ResearchItem != null)
            {
                if (heldItem != null)
                {
                    var temp = _researchArea.ReturnItem();

                    if (heldItem.Name.Equals(temp.Name, StringComparison.InvariantCultureIgnoreCase)
                        && heldItem is Object heldObj && temp is Object resObj &&
                        heldObj.quality.Equals(resObj.quality))
                    {
                        var rest = 0;

                        if (temp.Stack + heldItem.Stack > temp.maximumStackSize())
                        {
                            rest = temp.Stack + heldItem.Stack - temp.maximumStackSize();
                        }

                        var quantityToTransfer = heldItem.Stack - rest;

                        temp.Stack += quantityToTransfer;

                        _researchArea.TrySetItem(temp);

                        if (heldItem.Stack - quantityToTransfer <= 0)
                        {
                            heldItem = null;
                        }
                        else
                        {
                            heldItem.Stack -= quantityToTransfer;
                        }
                    }
                    else
                    {
                        _researchArea.TrySetItem(heldItem);
                        heldItem = temp;
                    }
                }
                else
                {
                    heldItem = _researchArea.ReturnItem();
                }
            }
            else
            {
                _researchArea.TrySetItem(heldItem);
                heldItem = null;
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
            else if (_searchBarTab.Bounds.Contains(x, y))
            {
                _searchBarTab.Clear();
            }
            else if (_categorySelector.IsExpanded)
            {
                _categorySelector.Close();
            }
            else if (_categorySelector.Bounds.Contains(x, y))
            {
                _categorySelector.ResetCategory();
            }
            else if (_researchArea.Bounds.Contains(x, y))
            {
                OnResearchAreaRightClick();
            }
            else
            {
                base.receiveRightClick(x, y, playSound);
            }
        }

        private void OnResearchAreaRightClick()
        {
            if (_researchArea.ResearchItem != null)
            {
                var temp = _researchArea.ReturnItem();

                if (heldItem == null)
                {
                    var newItem = temp.DeepClone();

                    newItem.Stack = 1;
                    temp.Stack--;

                    heldItem = newItem;
                    _researchArea.TrySetItem(temp);
                }
                else if (heldItem.Name.Equals(temp.Name, StringComparison.InvariantCultureIgnoreCase)
                         && heldItem is Object heldObj && temp is Object resObj &&
                         heldObj.quality.Equals(resObj.quality)
                         && heldItem.Stack + 1 <= heldItem.maximumStackSize())
                {
                    heldItem.Stack++;

                    if (temp.Stack - 1 > 0)
                    {
                        temp.Stack--;
                        _researchArea.TrySetItem(temp);
                    }
                }
                else
                {
                    _researchArea.TrySetItem(temp);
                }
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            var inDropdown = _categorySelector.IsExpanded;
            var isEscape = key == Keys.Escape;
            var isExitButton = isEscape || Game1.options.doesInputListContain(Game1.options.menuButton, key) ||
                               Game1.options.doesInputListContain(Game1.options.cancelButton, key);

            if (isEscape && (_searchBarTab.PersistFocus ||
                             _searchBarTab.Selected && !string.IsNullOrEmpty(_searchText)))
            {
                _searchBarTab.Clear();
                _searchBarTab.Blur();
            }
            else if (inDropdown && isExitButton)
            {
                _categorySelector.Close();
            }
            else if (key == Keys.Left || key == Keys.Right)
            {
                var direction = key == Keys.Left ? -1 : 1;
                _categorySelector.NextCategory(direction);
            }
            else if (key == Keys.Up || key == Keys.Down)
            {
                var direction = key == Keys.Up ? -1 : 1;

                if (inDropdown)
                {
                    _categorySelector.HandleScroll(direction);
                }
                else
                {
                    ScrollView(direction);
                }
            }
            else if (key == Keys.Delete && heldItem != null)
            {
                TryTrashItem();
            }
            else
            {
                var isIgnoredExitKey = _searchBarTab.Selected && isExitButton && !isEscape;
                if (!isIgnoredExitKey && !_searchBarTab.IsSearchBoxSelectionChanging)
                {
                    base.receiveKeyPress(key);
                }
            }
        }

        private void TryTrashItem()
        {
            if (ProgressionManager.Instance.ItemResearched(heldItem))
            {
                Utility.trashItem(heldItem);
                heldItem = null;
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);

            if (_overDropdown)
            {
                _categorySelector.HandleScroll(-direction);
            }
            else
            {
                ScrollView(-direction);
            }
        }

        public override void performHoverAction(int x, int y)
        {
            _overDropdown = _categorySelector.Bounds.Contains(x, y);

            if (!_searchBarTab.PersistFocus)
            {
                var overSearchBox = _searchBarTab.Bounds.Contains(x, y);

                if (_searchBarTab.Selected != overSearchBox)
                {
                    if (overSearchBox)
                    {
                        _searchBarTab.Focus(false);
                    }
                    else
                    {
                        _searchBarTab.Blur();
                    }
                }
            }

            base.performHoverAction(x, y);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.fadeToBlackRect,
                new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);

            _researchArea.Draw(spriteBatch);

            _baseDraw(spriteBatch);

            _qualitySelector.Draw(spriteBatch);
            _itemSortTab.Draw(spriteBatch);
            _categorySelector.Draw(spriteBatch);
            _searchBarTab.Draw(spriteBatch);

            DrawHeldItem(spriteBatch);
            drawMouse(spriteBatch);
        }

        private void DrawHeldItem(SpriteBatch spriteBatch)
        {
            if (hoverText != null && (hoveredItem == null || ItemsToGrabMenu == null))
            {
                if (hoverAmount > 0)
                {
                    drawToolTip(spriteBatch, hoverText, "", null, true, moneyAmountToShowAtBottom: hoverAmount);
                }
                else
                {
                    drawHoverText(spriteBatch, hoverText, Game1.smallFont);
                }
            }

            if (hoveredItem != null)
            {
                drawToolTip(spriteBatch, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem,
                    heldItem != null);
            }
            else if (hoveredItem != null && ItemsToGrabMenu != null)
            {
                drawToolTip(spriteBatch, ItemsToGrabMenu.descriptionText, ItemsToGrabMenu.descriptionTitle, hoveredItem,
                    heldItem != null);
            }

            heldItem?.drawInMenu(spriteBatch, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
        }

        public override void update(GameTime time)
        {
            _searchBarTab.Update(time);
            base.update(time);
        }

        protected override void cleanupBeforeExit()
        {
            if (_researchArea.ResearchItem != null)
            {
                Game1.createItemDebris(_researchArea.ReturnItem(), Game1.player.getStandingPosition(),
                    Game1.player.FacingDirection);
            }

            base.cleanupBeforeExit();
        }

        private void InitializeComponents()
        {
            var rootLeftAnchor = xPositionOnScreen;
            var rootTopAnchor = yPositionOnScreen;
            var rootRightAnchor = rootLeftAnchor + width;
            var rootBottomAnchor = rootTopAnchor + height;

            var sideTopAnchor = rootTopAnchor - Game1.tileSize + UIConstants.BorderWidth - 2 * Game1.pixelZoom;
            var sideRightAnchor = rootRightAnchor;

            var barTopAnchor = rootTopAnchor - Game1.tileSize * 2;

            _researchArea = new ItemResearchArea(_content, _monitor, sideRightAnchor, sideTopAnchor);
            _qualitySelector =
                new ItemQualitySelectorTab(_content, _monitor, rootLeftAnchor - 8, barTopAnchor, _quality);
            _itemSortTab = new ItemSortTab(_content, _monitor, _qualitySelector.Bounds.Right + 20, barTopAnchor,
                _sortOption);
            _categorySelector = new ItemCategorySelectorTab(_content, _monitor, _spawnableItems,
                _itemSortTab.Bounds.Right + 20, _itemSortTab.Bounds.Y);

            _searchBarTab = new ItemSearchBarTab(_content, _monitor, _categorySelector.Right + 20, barTopAnchor,
                _researchArea.Bounds.Right - _categorySelector.Right + 20 - 10 * Game1.pixelZoom);
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
                items = items.Where(item =>
                    Helpers.EqualsCaseInsensitive(item.Category, _categorySelector.SelectedCategory));
            }

            var search = _searchText.Trim();

            if (search != "")
            {
                items = items.Where(item =>
                    item.Name.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0
                    || item.DisplayName.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0
                );
            }

            return items;
        }
    }
}