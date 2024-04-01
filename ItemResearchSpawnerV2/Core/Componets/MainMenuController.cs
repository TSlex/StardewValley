using ItemResearchSpawnerV2.Core.Enums;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Models;
using ItemResearchSpawnerV2.Models.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core.Componets {
    internal class MainMenuController : MainMenu {
        private IEnumerable<SpawnableItem> ProgressionItems;

        private readonly List<SpawnableItem> FilteredProgressionItems = new();

        private string LastSearchQuery;

        public MainMenuController() : base() {
            behaviorOnItemGrab = OnItemGrab;
            LastSearchQuery = SearchBar.Text;

            UpdateView(true, true, true);
        }

        // ===============================================================================================

        public override void update(GameTime time) {
            SearchBar.Update(time);

            if (SearchBar.Text != LastSearchQuery) {
                LastSearchQuery = SearchBar.Text;
                UpdateView(rebuild: false, filter: true, resetScroll: true);
            }

            base.update(time);
        }

        // -----------------------------------------------------------------------------------------------

        private void UpdateCreativeMenu() {
            CreativeMenu.actualInventory.Clear();

            foreach (var prefab in FilteredProgressionItems
                .Skip(TopRowIndex * CreativeMenu.ItemsPerRow).Take(CreativeMenu.ItemsPerView)) {

                var item = prefab.CreateItem();
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
            var items = ProgressionItems;

            //var predicate = PredicateBuilder.True<SpawnableItem>();

            var search = SearchBar.Text;
            if (search != "") {
                items = items.Where(item =>
                    item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                ).ToList();

                //predicate = predicate.And(item =>
                //    item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                //    || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                //);
            }

            switch (ModManager.Instance.FavoriteDisplay) {
                case Data.Enums.FavoriteDisplayMode.FavoriteOnly:
                    search = "di";
                    items = items.Where(item =>
                        item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                        || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    ).ToList();
                    //predicate = predicate.And(item =>
                    //    item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    //    || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    //);
                    break;
            }

            switch (ModManager.Instance.ProgressionDisplay) {
                case Data.Enums.ProgressionDisplayMode.ResearchStarted:
                    search = "pi";
                    items = items.Where(item =>
                        item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                        || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    ).ToList();
                    //predicate = predicate.And(item =>
                    //    item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    //    || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    //);
                    break;
                case Data.Enums.ProgressionDisplayMode.Combined:
                    search = "op";
                    items = items.Where(item =>
                        item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                        || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    ).ToList();
                    //predicate = predicate.And(item =>
                    //    item.Item.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    //    || item.Item.DisplayName.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                    //);
                    break;
            }

            //items = items.Where(predicate.Compile());

            FilteredProgressionItems.Clear();
            FilteredProgressionItems.AddRange(items);
        }

        private void UpdateView(bool rebuild = false, bool filter = false, bool resetScroll = false) {

            if (rebuild) {
                ProgressionItems = ModManager.Instance.GetProgressionItems();
                FilterProgressionItems();
                TopRowIndex = 0;
                UpdateCreativeMenu();
                MaxTopRowIndex = Math.Max(0,
                    (int)Math.Ceiling(FilteredProgressionItems.Count() / (CreativeMenu.ItemsPerRow * 1m)) - 3);
            }

            if (filter) {
                FilterProgressionItems();
            }

            if (resetScroll || TopRowIndex > MaxTopRowIndex) {
                TopRowIndex = 0;
            }

            UpdateCreativeMenu();

            MaxTopRowIndex = Math.Max(0,
                (int)Math.Ceiling(FilteredProgressionItems.Count() / (CreativeMenu.ItemsPerRow * 1m)) - 3);
        }

        // ------------------------------------------------------------------------------------------------

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

            //else if (this.SortButton.bounds.Contains(x, y)) {

            //    this.SortBy = this.SortBy.GetNext();
            //    this.SortButton.label = this.SortButton.name = this.GetSortLabel(this.SortBy);

            //    UpdateView(rebuild: true);
            //}

            else if (LeftArrow.bounds.Contains(x, y)) {
                receiveScrollWheelAction(1);
            }

            else if (RightArrow.bounds.Contains(x, y)) {
                receiveScrollWheelAction(-1);
            }

            //else if (CategoryDropdown.TryClick(x, y, out bool itemClicked, out bool dropdownToggled)) {
            //    if (dropdownToggled) {
            //        this.SetDropdown(CategoryDropdown.IsExpanded);
            //    }
            //    if (itemClicked) {
            //        this.SetCategory(CategoryDropdown.Selected);
            //    }

            //}

            else if (SearchBar.Contains(x, y)) {
                SearchBar.HandleLeftClick(x, y);
            }

            else if (QualityButton.HoveredOver) {
                QualityButton.HandleLeftClick(x, y);
                UpdateView();
            }

            else if (FavoriteButton.HoveredOver) {
                FavoriteButton.HandleLeftClick(x, y);
                UpdateView(filter: true, resetScroll: true);
            }

            else if (DisplayButton.HoveredOver) {
                DisplayButton.HandleLeftClick(x, y);
                UpdateView(filter: true, resetScroll: true);
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
            if (QualityButton.HoveredOver) {
                QualityButton.HandleRightClick(x, y);
                UpdateView();
            }

            else if (FavoriteButton.HoveredOver) {
                FavoriteButton.HandleRightClick(x, y);
                UpdateView(filter: true, resetScroll: true);
            }

            else if (DisplayButton.HoveredOver) {
                DisplayButton.HandleRightClick(x, y);
                UpdateView(filter: true, resetScroll: true);
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
            //base.receiveScrollWheelAction(direction);
            ScrollView(-direction);
        }

        public void ScrollView(int direction, bool updateView = true) {
            if (direction < 0) {
                TopRowIndex--;
                Game1.playSound("newRecipe");
            }
            else if (direction > 0) {
                TopRowIndex++;
                Game1.playSound("newRecipe");
            }

            TopRowIndex = MathHelper.Clamp(TopRowIndex, 0, MaxTopRowIndex);

            if (updateView) {
                UpdateView();
            }

        }

        // --------------------------------------------------------------------------------------

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
