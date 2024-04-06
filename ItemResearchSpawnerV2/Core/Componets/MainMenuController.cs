using ItemResearchSpawnerV2.Core.Enums;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using ItemResearchSpawnerV2.Models.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core.Componets {
    internal class MainMenuController : MainMenu {
        private List<ProgressionItem> ProgressionItems;
        private List<ProgressionItem> FilteredProgressionItems;

        private string LastSearchQuery;

        public MainMenuController() : base() {
            behaviorOnItemGrab = OnItemGrab;
            LastSearchQuery = SearchBar.Text;

            UpdateView(true);
        }

        // ===============================================================================================

        public override void update(GameTime time) {
            SearchBar.Update(time);

            if (SearchBar.Text != LastSearchQuery) {
                LastSearchQuery = SearchBar.Text;
                Game1.playSound("drumkit6");
                UpdateView(rebuild: false, filter: true, resetScroll: true, reloadCategories: false);
            }

            base.update(time);
        }

        // -----------------------------------------------------------------------------------------------

        private void UpdateCategories() {
            var displayCategories = ModManager.Instance.GetDisplayCategories(FilteredProgressionItems);

            CategoryDropdown.SetOptions(new List<string>(displayCategories));
        }

        private void UpdateCreativeMenu() {
            CreativeMenu.actualInventory.Clear();

            //if (FilteredProgressionItems.Count() == 0) {
            //    return;
            //}

            foreach (var prefab in FilteredProgressionItems
                .Skip(TopRowIndex * CreativeMenu.ItemsPerRow * 2)
                .Take(CreativeMenu.ItemsPerView)
                ) {

                var item = prefab.Item.CreateItem();
                var quality = ModManager.Instance.ItemQuality;

                switch (ModManager.Instance.ModMode) {
                    case ModMode.Combined:
                    case ModMode.BuySell:
                        item.Stack = item.maximumStackSize();
                        break;
                    default:
                        item.Stack = item.maximumStackSize();

                        //quality = item is SObject
                        //    ? ItemQuality.Iridium
                        //    : ItemQuality.Normal;

                        break;
                }


                if (item is SObject obj) {
                    obj.Quality = (int)quality;
                }

                CreativeMenu.actualInventory.Add(item);
            }
        }

        private void FilterProgressionItems() {
            IEnumerable<ProgressionItem> items = ProgressionItems;

            var search = SearchBar.Text;

            if (search != "") {
                items = items.Where(item =>
                    item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                );
            }

            if (!CommonHelper.EqualsCaseInsensitive(CategoryDropdown.Selected, I18n.Category_All())) {
                items = items.Where(item => CommonHelper.EqualsCaseInsensitive(item.Category.Label, CategoryDropdown.Selected));
            }

            var activeSortOption = Enum.GetValues(typeof(ItemSortOption)).Cast<ItemSortOption>()
                .Where(op => op.GetString() == SortDropdown.Selected).FirstOrDefault();

            items = activeSortOption switch {
                ItemSortOption.NameUp => items.OrderBy(p => p.Item.DisplayName),
                ItemSortOption.NameDown => items.OrderByDescending(p => p.Item.DisplayName),
                ItemSortOption.CategoryUp => items.OrderBy(p => p.Item.Item.Category),
                ItemSortOption.CategoryDown => items.OrderByDescending(p => p.Item.Item.Category),
                ItemSortOption.IDUp => items.OrderBy(p => p.Item.Item.ParentSheetIndex),
                ItemSortOption.IDDown => items.OrderByDescending(p => p.Item.Item.ParentSheetIndex),
                ItemSortOption.PriceUp => items.OrderBy(p => p.Price),
                ItemSortOption.PriceDown => items.OrderByDescending(p => p.Price),
                _ => items.OrderBy(p => p.Item.DisplayName)
            };

            switch (ModManager.Instance.FavoriteDisplay) {
                case Data.Enums.FavoriteDisplayMode.FavoriteOnly:
                    var fsearch = "di";
                    items = items.Where(item =>
                        item.Item.Name.Contains(fsearch, StringComparison.InvariantCultureIgnoreCase)
                        || item.Item.DisplayName.Contains(fsearch, StringComparison.InvariantCultureIgnoreCase)
                    );
                    break;
            }

            switch (ModManager.Instance.ProgressionDisplay) {
                case Data.Enums.ProgressionDisplayMode.ResearchStarted:
                    var dasearch = "pi";
                    items = items.Where(item =>
                        item.Item.Name.Contains(dasearch, StringComparison.InvariantCultureIgnoreCase)
                        || item.Item.DisplayName.Contains(dasearch, StringComparison.InvariantCultureIgnoreCase)
                    );
                    break;
                case Data.Enums.ProgressionDisplayMode.Combined:
                    var dbsearch = "op";
                    items = items.Where(item =>
                        item.Item.Name.Contains(dbsearch, StringComparison.InvariantCultureIgnoreCase)
                        || item.Item.DisplayName.Contains(dbsearch, StringComparison.InvariantCultureIgnoreCase)
                    );
                    break;
            }

            FilteredProgressionItems = items.ToList();
        }

        private void UpdateView(bool rebuild = false, bool filter = false, bool resetScroll = false, bool reloadCategories = false) {

            if (rebuild) {
                ProgressionItems = ModManager.Instance.GetProgressionItems();
                FilterProgressionItems();
                TopRowIndex = 0;
                UpdateCategories();
                UpdateCreativeMenu();
                MaxTopRowIndex = Math.Max(0, (int)Math.Ceiling(FilteredProgressionItems.Count / (CreativeMenu.ItemsPerRow * 2m) - 2));

                return;
            }

            if (filter) {
                FilterProgressionItems();
            }

            if (resetScroll || TopRowIndex > MaxTopRowIndex) {
                TopRowIndex = 0;
            }

            //if (reloadCategories) {
            //    UpdateCategories();
            //}

            UpdateCreativeMenu();

            MaxTopRowIndex = Math.Max(0, (int)Math.Ceiling(FilteredProgressionItems.Count / (CreativeMenu.ItemsPerRow * 2m) - 2));
        }

        // ------------------------------------------------------------------------------------------------

        protected void SetCategoryDropdown(bool expanded) {
            if (expanded) {
                SetSortDropdown(false);
            }

            CategoryDropdown.IsExpanded = expanded;
            inventory.highlightMethod = _ => !expanded;
            CreativeMenu.highlightMethod = _ => !expanded;

            if (!expanded && !Game1.lastCursorMotionWasMouse) {
                setCurrentlySnappedComponentTo(CategoryDropdown.myID);
                snapCursorToCurrentSnappedComponent();
            }
        }

        protected void SetSortDropdown(bool expanded) {
            if (expanded) {
                SetCategoryDropdown(false);
            }

            SortDropdown.IsExpanded = expanded;
            inventory.highlightMethod = _ => !expanded;
            CreativeMenu.highlightMethod = _ => !expanded;

            if (!expanded && !Game1.lastCursorMotionWasMouse) {
                setCurrentlySnappedComponentTo(SortDropdown.myID);
                snapCursorToCurrentSnappedComponent();
            }
        }

        protected void SetCategory(string category) {
            if (!CategoryDropdown.TrySelect(category)) {
                ModManager.Instance.Monitor.Log($"Failed selecting category '{category}'.", LogLevel.Warn);
                if (category != I18n.Category_All()) {
                    SetCategory(I18n.Category_All());
                }
            }

            UpdateView(filter: true, resetScroll: true);
        }

        protected void SetSortOption(string sortOption) {
            if (!SortDropdown.TrySelect(sortOption)) {
                ModManager.Instance.Monitor.Log($"Failed selecting sort option '{sortOption}'.", LogLevel.Warn);
                if (sortOption != I18n.Sort_ByCategoryAsc())
                    SetCategory(I18n.Sort_ByCategoryAsc());
                return;
            }

            UpdateView(filter: true, resetScroll: true);
        }

        // ------------------------------------------------------------------------------------------------

        protected override void cleanupBeforeExit() {
            if (ItemResearchArea.ResearchItem != null) {
                TryReturnItemToInventory(ItemResearchArea.ReturnItem());
            }

            base.cleanupBeforeExit();
        }

        public override void receiveKeyPress(Keys key) {
            var isEscape = key == Keys.Escape;
            var isExitButton =
                isEscape
                || Game1.options.doesInputListContain(Game1.options.menuButton, key)
                || Game1.options.doesInputListContain(Game1.options.cancelButton, key);

            //bool inDropdown = this.CategoryDropdown.IsExpanded;

            //// clear textbox
            //if (isEscape && (this.IsSearchBoxSelectedExplicitly || (this.SearchBox.Selected && !string.IsNullOrEmpty(this.SearchBox.Text)))) {
            //    this.SearchBox.Text = "";
            //    this.DeselectSearchBox();
            //}

            //// close dropdown
            //else if (inDropdown && isExitButton)
            //    this.SetDropdown(false);

            //// allow trashing any item
            //else if (key == Keys.Delete && this.heldItem != null)
            //    this.TrashHeldItem();

            //// navigate
            //else if (key is Keys.Left or Keys.Right) {
            //    int direction = key == Keys.Left ? -1 : 1;
            //    this.NextCategory(direction);
            //}

            //// scroll
            //else if (key is Keys.Up or Keys.Down) {
            //    int direction = key == Keys.Up ? -1 : 1;

            //    if (inDropdown)
            //        this.CategoryDropdown.ReceiveScrollWheelAction(direction);
            //    else
            //        this.ScrollView(direction);
            //}

            if (isEscape && SearchBar.Selected) {
                if (SearchBar.PersistFocus && SearchBar.Text == "") {
                    SearchBar.Blur();
                }
                else {
                    SearchBar.Clear();
                }
            }

            else if (!(isExitButton && !isEscape && SearchBar.Selected)) {
                base.receiveKeyPress(key);
            }

            //else {
            //    bool isIgnoredExitKey = this.SearchBox.Selected && isExitButton && !isEscape;
            //    if (!isIgnoredExitKey && !this.IsSearchBoxSelectionChanging)
            //        base.receiveKeyPress(key);
            //}
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (trashCan.containsPoint(x, y) && heldItem != null) {
                TrashHeldItem();
            }

            if (ItemResearchArea.Bounds.Contains(x, y)) {
                ItemResearchArea.SetItem(heldItem, out var returnItem);
                heldItem = returnItem;
            }

            else if (LeftArrow.bounds.Contains(x, y)) {
                receiveScrollWheelAction(1);
            }

            else if (RightArrow.bounds.Contains(x, y)) {
                receiveScrollWheelAction(-1);
            }

            else if (CategoryDropdown.TryLeftClick(x, y, out bool itemClicked1, out bool dropdownToggled1)) {
                if (dropdownToggled1) {
                    SetCategoryDropdown(CategoryDropdown.IsExpanded);
                }
                if (itemClicked1) {
                    SetCategory(CategoryDropdown.Selected);
                }
            }

            else if (SortDropdown.TryLeftClick(x, y, out bool itemClicked2, out bool dropdownToggled2)) {
                if (dropdownToggled2) {
                    SetSortDropdown(SortDropdown.IsExpanded);
                }
                if (itemClicked2) {
                    SetSortOption(SortDropdown.Selected);
                }
            }

            else if (SearchBar.Contains(x, y)) {
                SearchBar.HandleLeftClick(x, y);
            }

            else if (QualityButton.HoveredOver) {
                QualityButton.HandleLeftClick(x, y);
                UpdateView();
            }

            else if (FavoriteButton.HoveredOver) {
                FavoriteButton.HandleLeftClick(x, y);
                UpdateView(filter: true, resetScroll: true, reloadCategories: true);
            }

            else if (DisplayButton.HoveredOver) {
                DisplayButton.HandleLeftClick(x, y);
                UpdateView(filter: true, resetScroll: true, reloadCategories: true);
            }

            else if (SettingsButton.HoveredOver) {
                SettingsButton.HandleLeftClick(x, y);
            }

            else if (ItemResearchArea.ResearchButton.HoveredOver) {
                ItemResearchArea.ResearchButton.HandleLeftClick(x, y);
            }

            else {
                if (SearchBar.Selected) {
                    SearchBar.Blur();
                }

                base.receiveLeftClick(x, y, playSound);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            if (CategoryDropdown.IsExpanded || CategoryDropdown.containsPoint(x, y)) {
                if (CategoryDropdown.Selected != I18n.Category_All()) {
                    SetCategory(I18n.Category_All());
                }
                SetCategoryDropdown(false);
            }
            else if (SortDropdown.IsExpanded || SortDropdown.containsPoint(x, y)) {
                if (SortDropdown.Selected != I18n.Sort_ByCategoryAsc()) {
                    SetSortOption(I18n.Sort_ByCategoryAsc());
                }

                SetSortDropdown(false);
            }

            else if (QualityButton.HoveredOver) {
                QualityButton.HandleRightClick(x, y);
                UpdateView();
            }

            else if (FavoriteButton.HoveredOver) {
                FavoriteButton.HandleRightClick(x, y);
                UpdateView(filter: true, resetScroll: true, reloadCategories: true);
            }

            else if (DisplayButton.HoveredOver) {
                DisplayButton.HandleRightClick(x, y);
                UpdateView(filter: true, resetScroll: true, reloadCategories: true);
            }

            else {
                base.receiveRightClick(x, y, playSound);
            }
        }

        public override void performHoverAction(int x, int y) {
            //if (!SearchBar.PersistFocus && !Game1.options.gamepadControls && Game1.lastCursorMotionWasMouse) {
            //    var overSearchBox = SearchBar.Contains(x, y);

            //    if (SearchBar.Selected != overSearchBox) {
            //        if (overSearchBox) {
            //            SearchBar.Focus(false);
            //        }
            //        else {
            //            SearchBar.Blur();
            //        }

            //    }
            //}

            QualityButton.HandleHover(x, y);
            FavoriteButton.HandleHover(x, y);
            DisplayButton.HandleHover(x, y);
            SettingsButton.HandleHover(x, y);
            ItemResearchArea.ResearchButton.HandleHover(x, y);

            base.performHoverAction(x, y);
        }

        public override void receiveScrollWheelAction(int direction) {
            if (CategoryDropdown.IsExpanded) {
                CategoryDropdown.OnScrollWheel(direction);
            }
            else if (SortDropdown.IsExpanded) {
                SortDropdown.OnScrollWheel(direction);
            }
            else {
                ScrollView(-direction);
            }
        }

        public void ScrollView(int direction, bool updateView = true) {
            if (direction < 0 && ShowLeftButton) {
                TopRowIndex -= 1;
                Game1.playSound("newRecipe");
            }
            else if (direction > 0 && ShowRightButton) {
                TopRowIndex += 1;
                Game1.playSound("newRecipe");
            }

            TopRowIndex = MathHelper.Clamp(TopRowIndex, 0, MaxTopRowIndex);

            if (updateView) {
                UpdateView();
            }

        }

        // --------------------------------------------------------------------------------------

        private void TryReturnItemToInventory(Item item) {
            if (item != null) {
                if (Game1.player.isInventoryFull()) {
                    DropItem(item);
                }
                else {
                    Game1.player.addItemByMenuIfNecessary(item);
                }
            }
        }

        private static void DropItem(Item item) {
            Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
        }


        private void OnItemGrab(Item item, Farmer player) {
            //if (ModManager.Instance.ModMode != ModMode.Research && ModManager.Instance.GetItemBuyPrice(item, true) <= Game1.player._money) {
            //    ModManager.Instance.BuyItem(item);
            //}

            UpdateView();
        }

        private void TrashHeldItem() {
            var item = heldItem;

            if (item is null) {
                return;
            }

            Utility.trashItem(item);

            heldItem = null;
        }
    }
}
