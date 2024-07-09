using Force.DeepCloner;
using ItemResearchSpawnerV2.Core.Data;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class MainMenuController : MainMenu {
        private List<ProgressionItem> ProgressionItems;
        private List<ProgressionItem> FilteredProgressionItems;

        private static bool ControlPressed => ModManager.Instance.Helper.Input.IsDown(SButton.LeftControl);
        private static bool ShiftPressed => ModManager.Instance.Helper.Input.IsDown(SButton.LeftShift);

        private string LastSearchQuery;

        private bool MenuWasBuild = false;

        public MainMenuController() : base() {
            behaviorOnItemGrab = OnItemGrab;
            CreativeMenu.highlightMethod = GetDefaultHighlightMethod();

            // -----------------------------------------------------------------

            UpdateView(rebuild: true);

            // -----------------------------------------------------------------

            SearchBar.SetText(ModManager.Instance.SearchText);
            LastSearchQuery = SearchBar.Text;

            SetCategory(ModManager.Instance.SelectedCategory);
            SetSortOption(ModManager.Instance.SortOption);

            // -----------------------------------------------------------------

            MenuWasBuild = true;
            UpdateView(filter: true);
        }

        // ===============================================================================================

        public override void update(GameTime time) {
            SearchBar.Update(time);

            if (SearchBar.Text != LastSearchQuery) {
                LastSearchQuery = SearchBar.Text;
                ModManager.Instance.SearchText = SearchBar.Text;

                if (ModManager.Instance.Config.GetEnableSounds()) {
                    Game1.playSound("drumkit6");
                }

                UpdateView(rebuild: false, filter: true, resetScroll: true, reloadCategories: false);
            }

            ItemResearchArea.Update(time);
            CreativeMenu.Update(time);

            base.update(time);
        }

        // -----------------------------------------------------------------------------------------------

        private void UpdateCategories() {
            var displayCategories = ModManager.Instance.GetDisplayCategories(ProgressionItems);

            CategoryDropdown.SetOptions(new List<string>(displayCategories));
        }

        private void UpdateCreativeMenu() {
            //CreativeMenu.actualInventory.Clear();

            //foreach (var progressionItem in FilteredProgressionItems
            //    .Skip(TopRowIndex * CreativeMenu.ItemsPerRow * 2)
            //    .Take(CreativeMenu.ItemsPerView)
            //    ) {

            //    var item = progressionItem.Item.CreateItem();
            //    var quality = ModManager.Instance.ItemQuality;

            //    switch (ModManager.Instance.ModMode) {
            //        case ModMode.Combined:
            //        case ModMode.BuySell:
            //            item.Stack = item.maximumStackSize();
            //            break;
            //        default:
            //            item.Stack = item.maximumStackSize();

            //            //quality = item is SObject
            //            //    ? ItemQuality.Iridium
            //            //    : ItemQuality.Normal;

            //            break;
            //    }


            //    if (item is SObject obj) {
            //        obj.Quality = (int)quality;
            //    }

            //    CreativeMenu.actualInventory.Add(item);
            //    CreativeMenu.OnInventoryChange();
            //}

            IEnumerable<ProgressionItem> GetMenuItems() {
                var recentlyUnlockedItem = ModManager.Instance.RecentlyUnlockedItem;

                if (recentlyUnlockedItem != null &&
                    FilteredProgressionItems
                    .Select(item => CommonHelper.GetItemUniqueKey(item.GameItem))
                    .Contains(CommonHelper.GetItemUniqueKey(recentlyUnlockedItem.GameItem))) {

                    var unlockedItemIndex = FilteredProgressionItems.FindIndex(
                        item => CommonHelper.GetItemUniqueKey(item.GameItem) == CommonHelper.GetItemUniqueKey(recentlyUnlockedItem.GameItem));

                    ModManager.Instance.Monitor.Log($"found index is {unlockedItemIndex}");
                    ModManager.Instance.Monitor.Log($"found item is {CommonHelper.GetItemUniqueKey(FilteredProgressionItems[unlockedItemIndex].GameItem)}");
                    ModManager.Instance.Monitor.Log($"must be  {CommonHelper.GetItemUniqueKey(recentlyUnlockedItem.GameItem)}");

                    PageIndex = unlockedItemIndex / (CreativeMenu.ItemsPerRow * 2) - 1;
                    PageIndex = PageIndex < 0 ? 0 : PageIndex;

                    ModManager.Instance.RecentlyUnlockedItemIndex = unlockedItemIndex - PageIndex * CreativeMenu.ItemsPerRow * 2;

                    ModManager.Instance.Monitor.Log($"index on page {ModManager.Instance.RecentlyUnlockedItemIndex}");
                }
                else {
                    recentlyUnlockedItem = null;
                }

                //ModManager.Instance.RecentlyUnlockedItem = null;

                ModManager.Instance.Monitor.Log($"number of filtered items {FilteredProgressionItems.Count()}");

                var items = FilteredProgressionItems
                    .Skip(PageIndex * CreativeMenu.ItemsPerRow * 2)
                    .Take(CreativeMenu.ItemsPerView).ToList();


                foreach (var pi in items) {

                    //pi.Item.Item = pi.Item.CreateItem();
                    pi.Item.Item = pi.InstanciateItem();


                    var availableQuality = ItemQuality.Normal;
                    var availableQuantity = pi.GameItem.maximumStackSize();

                    if (ModManager.Instance.ModMode == ModMode.Research || ModManager.Instance.ModMode == ModMode.ResearchPlus) {
                        availableQuality = pi.GetAvailableQuality(ModManager.Instance.ItemQuality);
                        availableQuantity = pi.GameItem.maximumStackSize();
                    }
                    else {
                        availableQuantity = pi.GetAvailableQuantity(Game1.player._money, ModManager.Instance.ItemQuality, out availableQuality);
                    }


                    pi.GameItem.Quality = (int) availableQuality;

                    pi.Stack = ModManager.Instance.ModMode switch {
                        ModMode.BuySell => availableQuantity,
                        ModMode.Combined => availableQuantity,
                        ModMode.BuySellPlus => availableQuantity,
                        _ => pi.GameItem.maximumStackSize()
                    };

                    //if (!(ModManager.Instance.ProgressionDisplay == ProgressionDisplayMode.ResearchStarted && pi.ResearchCompleted)) {
                    //    yield return pi;
                    //}

                    yield return pi;

                    //ModManager.Instance.Monitor.Log($"{ModManager.Instance.ProgressionDisplay}");

                    if (availableQuality != ModManager.Instance.ItemQuality
                        && ModManager.Instance.ProgressionDisplay != ProgressionDisplayMode.ResearchedOnly
                        && pi.BaseResearchCompleted && !pi.NormalQualityForced
                        ) {

                        var pi_c = new ProgressionItem(pi.Item.ShallowClone(), pi.SaveData, pi.Category, pi.Price);

                        pi_c.Item.Item = pi_c.InstanciateItem();
                        pi_c.GameItem.Quality = (int) ModManager.Instance.ItemQuality;
                        pi_c.Stack = pi_c.GameItem.maximumStackSize();

                        if (pi_c.ResearchStarted) {
                            yield return pi_c;
                        }
                    }
                }
            }

            //var items = FilteredProgressionItems
            //    .Skip(TopRowIndex * CreativeMenu.ItemsPerRow * 2)
            //    .Take(CreativeMenu.ItemsPerView).ToList()
            //    .Select(pi => {
            //        pi.Item.Item = pi.Item.CreateItem();

            //        //pi.GameItem.Quality = (int)ModManager.Instance.ItemQuality;
            //        pi.GameItem.Quality = (int)pi.GetAvailableQuality(ModManager.Instance.ItemQuality);

            //        pi.Stack = ModManager.Instance.ModMode switch {
            //            ModMode.BuySell => pi.GameItem.maximumStackSize(),
            //            ModMode.Combined => pi.GameItem.maximumStackSize(),
            //            ModMode.BuySellPlus => pi.GameItem.maximumStackSize(),
            //            _ => pi.GameItem.maximumStackSize()
            //        };

            //        return pi;
            //    });

            CreativeMenu.SetItems(GetMenuItems().ToList());
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
                ItemSortOption.NameASC => items.OrderBy(p => p.Item.DisplayName),
                ItemSortOption.NameDESC => items.OrderByDescending(p => p.Item.DisplayName),
                ItemSortOption.CategoryASC => items.OrderBy(p => p.Category.Label).ThenBy(p => p.Item.Item.Category),
                ItemSortOption.CategoryDESC => items.OrderByDescending(p => p.Category.Label).ThenByDescending(p => p.Item.Item.Category),
                ItemSortOption.IDASC => items.OrderBy(p => p.Item.Item.ParentSheetIndex),
                ItemSortOption.IDDESC => items.OrderByDescending(p => p.Item.Item.ParentSheetIndex),
                ItemSortOption.PriceASC => items.OrderBy(p => p.Price),
                ItemSortOption.PriceDESC => items.OrderByDescending(p => p.Price),
                _ => items.OrderBy(p => p.Item.DisplayName)
            };

            items = items.ToList();

            // ------------------------------------------------------------------------------------------------

            switch (ModManager.Instance.FavoriteDisplay) {
                case FavoriteDisplayMode.FavoriteOnly:
                    items = items.Where(item => item.Favorited);
                    break;
            }

            //items = items.Where(item => item.BaseResearchStarted || item.BaseResearchCompleted);

            switch (ModManager.Instance.ProgressionDisplay) {
                case ProgressionDisplayMode.ResearchStarted:
                    items = items.Where(item => item.BaseResearchStarted || (item.BaseResearchCompleted && item.RequestedResearchStarted));
                    //items = items.Where(item => item.BaseResearchStarted || item.BaseResearchCompleted);
                    break;
                case ProgressionDisplayMode.ResearchedOnly:
                    items = items.Where(item => item.BaseResearchCompleted);
                    break;
                case ProgressionDisplayMode.Combined:
                    items = items.Where(item => item.BaseResearchStarted || item.BaseResearchCompleted);
                    break;
                case ProgressionDisplayMode.NotResearched:
                    items = items.Where(item => !(item.BaseResearchStarted || item.BaseResearchCompleted));
                    break;
            }

            FilteredProgressionItems = items.ToList();
        }

        public void UpdateView(bool rebuild = false, bool filter = false, bool resetScroll = false, bool reloadCategories = false) {

            if (rebuild) {
                ProgressionItems = ModManager.Instance.GetProgressionItems().Where(i => !i.Forbidden).ToList();

                if (!ModManager.Instance.Config.GetShowMissingItems()) {
                    ProgressionItems = ProgressionItems.Where(i => !i.Missing).ToList();
                }

                FilterProgressionItems();
                PageIndex = 0;
                UpdateCategories();
                UpdateCreativeMenu();
                MaxTopRowIndex = Math.Max(0, (int) Math.Ceiling(FilteredProgressionItems.Count / (CreativeMenu.ItemsPerRow * 2m) - 2));
                ItemResearchArea.BookTurnRightRequested = true;
                return;
            }

            if (!MenuWasBuild) {
                return;
            }

            if (filter) {
                FilterProgressionItems();
                ItemResearchArea.BookTurnRightRequested = true;
                if (ModManager.Instance.Config.GetEnableSounds()) {
                    Game1.playSound("newRecipe");
                }
            }

            if (resetScroll || PageIndex > MaxTopRowIndex) {
                PageIndex = 0;
            }

            //if (reloadCategories) {
            //    UpdateCategories();
            //}

            UpdateCreativeMenu();

            MaxTopRowIndex = Math.Max(0, (int) Math.Ceiling(FilteredProgressionItems.Count / (CreativeMenu.ItemsPerRow * 2m) - 2));
        }

        // ------------------------------------------------------------------------------------------------

        protected void SetCategoryDropdown(bool expanded) {
            if (expanded) {
                SetSortDropdown(false);
            }

            CategoryDropdown.IsExpanded = expanded;
            inventory.highlightMethod = _ => !expanded;
            CreativeMenu.highlightMethod = expanded ? _ => false : GetDefaultHighlightMethod();

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
            CreativeMenu.highlightMethod = expanded ? _ => false : GetDefaultHighlightMethod();

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

            ModManager.Instance.SelectedCategory = category;

            UpdateView(filter: true, resetScroll: true);
        }

        protected void SetSortOption(string sortOption) {
            if (!SortDropdown.TrySelect(sortOption)) {
                ModManager.Instance.Monitor.Log($"Failed selecting sort option '{sortOption}'.", LogLevel.Warn);
                if (sortOption != I18n.Sort_ByCategoryAsc())
                    SetSortOption(I18n.Sort_ByCategoryAsc());
                return;
            }

            ModManager.Instance.SortOption = sortOption;

            UpdateView(filter: true, resetScroll: true);
        }

        // ------------------------------------------------------------------------------------------------

        protected override void cleanupBeforeExit() {
            if (ItemResearchArea.ResearchItem != null) {
                CommonHelper.TryReturnItemToInventory(ItemResearchArea.ReturnItem());
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

            else if (ControlPressed) {
                OnControlLeftClickPressed(x, y);
            }

            else if (SortDropdown.TryLeftClick(x, y, out bool itemClicked2, out bool dropdownToggled2)) {
                if (dropdownToggled2) {
                    SetSortDropdown(SortDropdown.IsExpanded);
                }
                if (itemClicked2) {
                    SetSortOption(SortDropdown.Selected);
                    if (ModManager.Instance.Config.GetEnableSounds()) {
                        Game1.playSound("drumkit6");
                    }
                }
            }

            else if (CategoryDropdown.TryLeftClick(x, y, out bool itemClicked1, out bool dropdownToggled1)) {
                if (dropdownToggled1) {
                    SetCategoryDropdown(CategoryDropdown.IsExpanded);
                }
                if (itemClicked1) {
                    SetCategory(CategoryDropdown.Selected);
                    if (ModManager.Instance.Config.GetEnableSounds()) {
                        Game1.playSound("drumkit6");
                    }
                }
            }

            else if (ItemResearchArea.Bounds.Contains(x, y)) {
                ItemResearchArea.SetItem(heldItem, out var returnItem);
                heldItem = returnItem;
            }

            else if (LeftArrow.Bounds.Contains(x, y)) {
                receiveScrollWheelAction(1);
            }

            else if (RightArrow.Bounds.Contains(x, y)) {
                receiveScrollWheelAction(-1);
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

            else if (ProgressButton.HoveredOver) {
                ProgressButton.HandleLeftClick(x, y);
                UpdateView(filter: true, resetScroll: true, reloadCategories: true);
            }

            //else if (SettingsButton.HoveredOver) {
            //    SettingsButton.HandleLeftClick(x, y);
            //}

            else if (ItemResearchArea.ResearchButton.HoveredOver) {
                ItemResearchArea.ResearchButton.HandleLeftClick(x, y);
            }

            else {
                if (SearchBar.Selected) {
                    SearchBar.Blur();
                }

                else {
                    base.receiveLeftClick(x, y, playSound);
                }

                if (ShiftPressed && heldItem != null && !CreativeMenu.isWithinBounds(x, y)) {
                    CommonHelper.TryReturnItemToInventory(heldItem);
                    heldItem = null;
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            if (CategoryDropdown.IsExpanded || CategoryDropdown.containsPoint(x, y)) {
                if (CategoryDropdown.Selected != I18n.Category_All()) {
                    SetCategory(I18n.Category_All());
                    if (ModManager.Instance.Config.GetEnableSounds()) {
                        Game1.playSound("smallSelect");
                    }
                }
                SetCategoryDropdown(false);
            }
            else if (SortDropdown.IsExpanded || SortDropdown.containsPoint(x, y)) {
                if (SortDropdown.Selected != I18n.Sort_ByCategoryAsc()) {
                    SetSortOption(I18n.Sort_ByCategoryAsc());
                    if (ModManager.Instance.Config.GetEnableSounds()) {
                        Game1.playSound("smallSelect");
                    }
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

            else if (ProgressButton.HoveredOver) {
                ProgressButton.HandleRightClick(x, y);
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

            LeftArrow.HandleHover(x, y);
            RightArrow.HandleHover(x, y);

            QualityButton.HandleHover(x, y);
            FavoriteButton.HandleHover(x, y);
            ProgressButton.HandleHover(x, y);
            //SettingsButton.HandleHover(x, y);
            ItemResearchArea.ResearchButton.HandleHover(x, y);

            base.performHoverAction(x, y);
        }

        private bool OnControlLeftClickPressed(int x, int y) {
            if (ItemResearchArea.Bounds.Contains(x, y)) {
                if (ItemResearchArea.ResearchItem != null) {
                    CommonHelper.TryReturnItemToInventory(ItemResearchArea.ReturnItem());
                }

                return true;
            }

            if (trashCan.containsPoint(x, y)) {
                var removedAny = false;
                var removeSound = ModManager.Instance.ModMode != ModMode.Research && ModManager.Instance.ModMode != ModMode.ResearchPlus ?
                    "purchase" : "fireball";

                foreach (var item in Game1.player.Items.Where(item => item != null)) {
                    var progressionItem = ModManager.ProgressionManagerInstance.GetProgressionItem(item);

                    if (progressionItem.ResearchCompleted) {
                        switch (ModManager.Instance.ModMode) {
                            case ModMode.Research:
                            case ModMode.ResearchPlus:
                                Game1.player.removeItemFromInventory(item);
                                removedAny = true;
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (removedAny) {
                    ItemResearchArea.BookTurnLeftRequested = true;

                    if (ModManager.Instance.Config.GetEnableSounds()) {
                        Game1.playSound(removeSound);
                    }
                }

                return true;
            }

            if (hoveredItem != null && Game1.player.Items.Contains(hoveredItem)) {
                var progressionItem = ModManager.ProgressionManagerInstance.GetProgressionItem(hoveredItem);

                if (progressionItem?.Forbidden ?? true) {
                    return false;
                }

                if (progressionItem.ResearchCompleted) {
                    var removeSound = ModManager.Instance.ModMode != ModMode.Research && ModManager.Instance.ModMode != ModMode.ResearchPlus ?
                        "purchase" : "fireball";

                    switch (ModManager.Instance.ModMode) {
                        case ModMode.BuySell:
                        case ModMode.Combined:
                            ModManager.Instance.SellItem(hoveredItem);
                            UpdateView();
                            break;
                        default:
                            break;
                    }

                    Game1.player.removeItemFromInventory(hoveredItem);

                    ItemResearchArea.BookTurnLeftRequested = true;

                    if (ModManager.Instance.Config.GetEnableSounds()) {
                        Game1.playSound(removeSound);
                    }

                    return true;
                }

                else if (ItemResearchArea.ResearchItem != null) {
                    CommonHelper.TryReturnItemToInventory(ItemResearchArea.ReturnItem());

                    ItemResearchArea.SetItem(hoveredItem, out var _);
                }

                else {
                    ItemResearchArea.SetItem(hoveredItem, out var _);
                }

                Game1.player.removeItemFromInventory(hoveredItem);

                return true;
            }

            return false;
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

        public void ScrollView(int direction) {
            if (direction < 0 && ShowLeftButton) {
                PageIndex -= 1;
                ItemResearchArea.BookTurnLeftRequested = true;
                if (ModManager.Instance.Config.GetEnableSounds()) {
                    Game1.playSound("newRecipe");
                }
                UpdateView();
            }
            else if (direction > 0 && ShowRightButton) {
                PageIndex += 1;
                ItemResearchArea.BookTurnRightRequested = true;
                if (ModManager.Instance.Config.GetEnableSounds()) {
                    Game1.playSound("newRecipe");
                }
                UpdateView();
            }

            PageIndex = MathHelper.Clamp(PageIndex, 0, MaxTopRowIndex);
        }

        // --------------------------------------------------------------------------------------

        private StardewValley.Menus.InventoryMenu.highlightThisItem GetDefaultHighlightMethod() {
            return ModManager.Instance.ModMode switch {
                //ModMode.Research or ModMode.ResearchPlus => (item) => true,
                //ModMode.BuySell or ModMode.Combined or ModMode.BuySellPlus => (item) => ModManager.Instance.CanBuyItem(item) && item.Stack > 0,
                _ => (item) => true,
            };
        }


        private void OnItemGrab(Item item, Farmer player) {
            if ((ModManager.Instance.ModMode != ModMode.Research && ModManager.Instance.ModMode != ModMode.ResearchPlus)
                && ModManager.Instance.CanBuyItem(item)) {
                ModManager.Instance.BuyItem(item);
            }

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
